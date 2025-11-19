// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using iText.Commons.Actions;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Pdfa;
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Infrastructure.IText.Decorator.Services;
using Pi3.Infrastructure.IText.ReportGenerator.Resources;

namespace Pi3.Infrastructure.IText.ReportGenerator.Services;

public class ITextReportGeneratorService : IReportGeneratorService
{
    #region Public Members

    public ITextReportGeneratorService(ILogger<ITextReportGeneratorService> logger)
    {
        this._logger = logger;

        var config = new MapperConfiguration(cfg => {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = config.CreateMapper();
    }


    public async Task<ReportGeneratorCapabilities> GetCapabilities()
    {
        return new ReportGeneratorCapabilities()
        {
            SupportedOutputTypes = new ReportOutputTypes[1] { ReportOutputTypes.AsPdf },
            SupportedOrientations = new PageOrientations[2] { PageOrientations.Landscape, PageOrientations.Portrait },
            SupportedPageTypes = new PageSizes[2] { PageSizes.A3, PageSizes.A4 },
            SupportedSections = new Type[7]
            {
                typeof(TextSectionModel), typeof(GridSectionModel), typeof(PageNumberSectionModel),
                typeof(BreakPageSectionModel), typeof(ImageSectionModel), typeof(LineSectionModel),
                typeof(EmptySectionModel)
            },
            HeaderSupported = true,
            FooterSupported = true
        };
    }


    #endregion

    #region Private Members

    protected ILogger<ITextReportGeneratorService> _logger;
    protected IMapper? _mapper;
    protected ReportModel _reportModel;
    protected Dictionary<string, PdfFont> fonts = new Dictionary<string, PdfFont>();

    private enum Location { Body, Header, Footer }
    private bool headerAdded;


    public async Task<GeneratedReport> Generate(ReportModel reportModel, Stream outputStream)
    {
        this.fonts["helvetica"] = PdfFontFactory.CreateFont(Files.helvetica, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
        this.fonts["courier"] = PdfFontFactory.CreateFont(Files.cour, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
        this.fonts["arial"] = PdfFontFactory.CreateFont(Files.arial, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);

        // Create a memory stream to hold the PDF content
        using var memoryStream = new MemoryStream();

        // 1. Create writer with PDF/A conformance level
        using var writer = new PdfWriter(memoryStream);

        // 2. Create PDF document with PDF/A compliance
        using var pdfDoc = new PdfADocument(
            writer,
            PdfAConformanceLevel.PDF_A_3B,
            new PdfOutputIntent("Custom", "", Urls.www_color_org,
                               "sRGB IEC61966-2.1",
                               new MemoryStream(Files.color_profile))
        );

        // Add XMP metadata (required for PDF/A)
        PdfDocumentInfo docinfo = pdfDoc.GetDocumentInfo();
        XMPMeta xmp = XMPMetaFactory.Create();

        // Register the PDF/A namespace
        XMPSchemaRegistry registry = XMPMetaFactory.GetSchemaRegistry();
        registry.RegisterNamespace(Urls.www_aiim_org_pdfa_ns_id, "pdfaid");
        // Set standard properties
        xmp.SetProperty(XMPConst.NS_DC, "format", "application/pdf");
        xmp.SetProperty(XMPConst.NS_PDF, "Producer", docinfo.GetProducer());
        xmp.SetProperty(XMPConst.NS_PDF, "PDFVersion", pdfDoc.GetPdfVersion().ToString());

        // Add PDF/A identification schema
        XMPMeta pdfaXmp = XMPMetaFactory.Create();
        pdfaXmp.SetProperty(XMPConst.NS_PDFA_ID, "part", "3");
        pdfaXmp.SetProperty(XMPConst.NS_PDFA_ID, "conformance", "B");

        // Create the schema description for PDF/A
        xmp.SetProperty(Urls.www_aiim_org_pdfa_ns_id, "part", "3");
        xmp.SetProperty(Urls.www_aiim_org_pdfa_ns_id, "conformance", "B");

        // Set XMP metadata in the document
        pdfDoc.SetXmpMetadata(xmp);        //using var pdfDoc = new PdfDocument(writer);
        var document = new Document(pdfDoc);
        headerAdded = false;
        _reportModel = reportModel;

        // 3. Embed all fonts (required for PDF/A)
        //PdfFont font = PdfFontFactory.CreateFont(Files.helvetica, PdfEncodings.WINANSI,
        //                                       PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
        PdfFont font = this.fonts["helvetica"];

        var handler = new PageNumberEventHandler(document, reportModel, this.fonts);
        pdfDoc.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);

        if (reportModel.Orientation == PageOrientations.Landscape)
        {
            pdfDoc.SetDefaultPageSize(iText.Kernel.Geom.PageSize.A4.Rotate());
        }

        // 4. Add document metadata (required for PDF/A)
        pdfDoc.GetCatalog().GetPdfObject().Put(PdfName.Lang, new PdfString("en-US"));

        // Add document information
        PdfDocumentInfo info = pdfDoc.GetDocumentInfo();
        info.SetTitle(/*reportModel.Title ??*/ "Report");
        info.SetAuthor("Your Application");
        info.SetSubject("Generated Report");
        info.SetCreator("iText PDF/A Generator");
        info.SetKeywords("Report, PDF/A");

        foreach (var section in reportModel.Sections ?? Enumerable.Empty<ISectionModel>())
        {
            if (section.GetType() == typeof(TextSectionModel))
            {
                this.DrawParagraph(document, (TextSectionModel)section, font);
            }
            else if (section.GetType() == typeof(GridSectionModel))
            {
                this.DrawTable(document, (GridSectionModel)section, font);
            }
            else if (section.GetType() == typeof(PageNumberSectionModel))
            {
                AddPageNumber(document, (PageNumberSectionModel)section, pdfDoc, 0);
            }
            else if (section.GetType() == typeof(BreakPageSectionModel))
            {
                CreatePageBreak(document);
            }
            else if (section.GetType() == typeof(ImageSectionModel))
            {
                AddImageToBody(document, (ImageSectionModel)section);
            }
            else if (section.GetType() == typeof(LineSectionModel))
            {
                CreateHorizontalLine(document);
            }
            else if (section.GetType() == typeof(EmptySectionModel))
            {
                CreateEmptyParagraph(document);
            }
        }

        //writer.Flush(); // Ensure all content is written to the stream
        // Close document to finalize PDF creation
        document.Close();

        string fileName = $"Report_{DateTime.Now:yyyyMMddHHmmssfff}", contentType = null!;

        if (reportModel.OutputType == ReportOutputTypes.AsPdf)
        {
            fileName += ".pdf";
            contentType = "application/pdf";
        }
        else if (reportModel.OutputType == ReportOutputTypes.AsDocx)
        {
            fileName += ".docx";
            contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        }
        else if (reportModel.OutputType == ReportOutputTypes.AsHtml)
        {
            fileName += ".html";
            contentType = "text/html";
        }

        // Get the byte array from memory stream
        byte[] content = memoryStream.ToArray();

        // Write the content to the output stream
        await outputStream.WriteAsync(content, 0, content.Length);
        outputStream.Position = 0;

        return new GeneratedReport()
        {
            FileName = fileName,
            ContentType = contentType
        };
    }

    private void CreateEmptyParagraph(Document document)
    {
        // Get the font from the document context
        //PdfFont font = PdfFontFactory.CreateFont(Files.helvetica, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
        // If font isn't available in document context, you'll need to pass it to this method
        document.Add(new Paragraph("\n").SetFont(this.fonts["helvetica"]));
    }

    private void CreateHorizontalLine(Document document)
    {
        document.Add(new LineSeparator(new SolidLine()));
    }

    private void AddImageToBody(Document document, ImageSectionModel section)
    {
        AddImageToBodyPdfA(document, section);
        return;
        var imageData = ImageDataFactory.Create(section.ImageContent);
        var image = new iText.Layout.Element.Image(imageData);


        // Imposta le dimensioni dell'immagine (opzionale)
        //image.SetWidth(section.Width);
        //image.SetHeight(section.Height);
        document.Add(image);
    }

    private void AddImageToBodyPdfA(Document document, ImageSectionModel section)
    {
        // For PDF/A, images need to have a specific color profile
        // Using ImageDataFactory ensures proper handling
        var imageData = ImageDataFactory.Create(section.ImageContent);
        var image = new iText.Layout.Element.Image(imageData);

        // For accessibility in PDF/A, use standard property
        // The Image class doesn't have SetAlternateDescription method
        image.GetAccessibilityProperties().SetAlternateDescription(section.ImageName ?? "Image");
        document.Add(image);
    }

    private void CreatePageBreak(Document document)
    {
        document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
    }

    private void AddPageNumber(Document document, PageNumberSectionModel section, PdfDocument pdfDoc, int pageNumber)
    {
        //int numberOfPages = pdfDoc.GetNumberOfPages();
        //for (int i = 1; i <= numberOfPages; i++)
        //{
        //    var page = pdfDoc.GetPage(i);
        //    var canvas = new PdfCanvas(page);
        //    var text = new Paragraph($"Page {i} of {numberOfPages}")
        //        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
        //        .SetFontSize(12)
        //        .SetFontColor(ColorConstants.BLACK)
        //        .SetTextAlignment(TextAlignment.RIGHT);
        //    document.ShowTextAligned(text, 559, 806, i, TextAlignment.RIGHT, VerticalAlignment.TOP, 0);
        //}
        // Registra l'EventHandler per i numeri di pagina                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
        if (!headerAdded) { 
            var handler = new PageNumberEventHandler(document, null, null);
            pdfDoc.AddEventHandler(PdfDocumentEvent.END_PAGE, handler);
        }

    }

    private void DrawTable(Document document, GridSectionModel section, PdfFont font)
    {
        DrawTableWithEmbeddedFont(document, section, font);
        return;
        var table = new Table(UnitValue.CreatePercentArray(
            section.Rows[0].Cells.Select(c => (c.Style?.WithPercentage ?? 1)).Select(i => (float)i).ToArray())
            );
        foreach (var row in section.Rows)
        {
            foreach (var cell in row.Cells)
            {
                var cellElement = new Cell()
                    .Add(new Paragraph(cell.Content.Value))
                    .SetFontSize(cell.Content.Style?.FontSize ?? 12)
                    .SetFontColor(new DeviceRgb(cell.Content.Style?.FontColor?.R ?? 0, cell.Content.Style?.FontColor?.G ?? 0, cell.Content.Style?.FontColor?.B ?? 0))
                    .SetBackgroundColor(new DeviceRgb(cell.Style?.ForegroundColor?.R ?? 255, cell.Style?.ForegroundColor?.G ?? 255, cell.Style?.ForegroundColor?.B ?? 255))
                    .SetTextAlignment(this._mapper.Map<TextAlignment>(cell.Style?.Justification ?? Justifications.Left))
                    .SetVerticalAlignment(this._mapper.Map<VerticalAlignment>(cell.Style?.VerticalAlignment ?? VerticalAlignments.Top));
                table.AddCell(cellElement);
            }
        }
        document.Add(table);
    }

    private void DrawTableWithEmbeddedFont(Document document, GridSectionModel section, PdfFont font)
    {
        var table = new Table(UnitValue.CreatePercentArray(
            section.Rows[0].Cells.Select(c => (c.Style?.WithPercentage ?? 1)).Select(i => (float)i).ToArray())
            );
        foreach (var row in section.Rows)
        {
            foreach (var cell in row.Cells)
            {
                var cellElement = new Cell()
                    .Add(new Paragraph(cell.Content.Value ?? string.Empty).SetFont(font)) // Use embedded font
                    .SetFontSize(cell.Content.Style?.FontSize ?? 12)
                    .SetFontColor(new DeviceRgb(cell.Content.Style?.FontColor?.R ?? 0,
                                              cell.Content.Style?.FontColor?.G ?? 0,
                                              cell.Content.Style?.FontColor?.B ?? 0))
                    .SetBackgroundColor(new DeviceRgb(cell.Style?.ForegroundColor?.R ?? 255,
                                                    cell.Style?.ForegroundColor?.G ?? 255,
                                                    cell.Style?.ForegroundColor?.B ?? 255))
                    .SetTextAlignment(this._mapper.Map<TextAlignment>(cell.Style?.Justification ?? Justifications.Left))
                    .SetVerticalAlignment(this._mapper.Map<VerticalAlignment>(cell.Style?.VerticalAlignment ?? VerticalAlignments.Top));

                if (cell.Content?.Style?.FontIsBold ?? false)
                    cellElement.SetBold();

                table.AddCell(cellElement);
            }
        }
        document.Add(table);
    }

    private void DrawParagraph(Document document, TextSectionModel section, PdfFont font)
    {
        DrawParagraphWithEmbeddedFont(document, section, font);
        return;
        //var paragraph = new Paragraph(section.Content.Value);
        //paragraph = paragraph.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
        //paragraph = paragraph.SetFontSize(section.Content.Style?.FontSize ?? 12);
        //paragraph = paragraph.SetFontColor(new DeviceRgb(section.Content.Style?.FontColor?.R ?? 0, section.Content.Style?.FontColor?.G ?? 0, section.Content.Style?.FontColor?.B ?? 0));
        //paragraph = paragraph.SetTextAlignment(this._mapper.Map<TextAlignment>(section.Style?.Justification ?? Justifications.Left));
        //document.Add(paragraph);
    }

    private void DrawParagraphWithEmbeddedFont(Document document, TextSectionModel section, PdfFont font)
    {
        var paragraph = new Paragraph(section.Content.Value ?? string.Empty);
        paragraph = paragraph.SetFont(font); // Use the embedded font
        paragraph = paragraph.SetFontSize(section.Content.Style?.FontSize ?? 12);
        paragraph = paragraph.SetFontColor(new DeviceRgb(section.Content.Style?.FontColor?.R ?? 0,
                                                       section.Content.Style?.FontColor?.G ?? 0,
                                                       section.Content.Style?.FontColor?.B ?? 0));
        paragraph = paragraph.SetTextAlignment(this._mapper.Map<TextAlignment>(section.Style?.Justification ?? Justifications.Left));
        
        if (section.Content.Style?.FontIsBold ?? false)
        {
            paragraph = paragraph.SetBold();
        }
        document.Add(paragraph);
    }

    #endregion
}
