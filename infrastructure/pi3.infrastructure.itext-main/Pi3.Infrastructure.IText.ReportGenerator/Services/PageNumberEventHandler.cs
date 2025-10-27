// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Infrastructure.IText.ReportGenerator.Resources;
using Pi3.Infrastructure.IText.ReportGenerator.Services;

namespace Pi3.Infrastructure.IText.Decorator.Services;

public class PageNumberEventHandler : iText.Kernel.Events.IEventHandler
{
    private Document document;
    private PdfFont font;
    private ReportModel reportModel;

    private Dictionary<string, PdfFont> fonts;

    public PageNumberEventHandler(Document document, ReportModel reportModel, Dictionary<string, PdfFont> fonts)
    {
        this.document = document;
        this.reportModel = reportModel;
        this.fonts = fonts;
        this.font = this.fonts["helvetica"];
    }

    public void HandleEvent(Event @event)
    {
        PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
        PdfDocument pdfDoc = docEvent.GetDocument();
        PdfPage page = docEvent.GetPage();
        int pageNumber = pdfDoc.GetPageNumber(page);
        int totalPages = pdfDoc.GetNumberOfPages();
        Rectangle pageSize = page.GetPageSize();


        // Create separate canvas for headers and footers to avoid interfering with main content
        PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);

        HandleHeader();
        HandleFooter();

        void HandleHeader() {
            var headerText = this.reportModel.HeaderSections.OfType<PageNumberSectionModel>().FirstOrDefault();

            if (headerText != null)
            {
                float headerY = pageSize.GetTop() - 20;
                Canvas headerCanvas = new Canvas(pdfCanvas,
                    new Rectangle(pageSize.GetLeft() + 35, headerY - 40, pageSize.GetWidth() - 70, 40));

                var text = ConvertText(headerText!.Format!);

                var textAlignment = ConvertAlignment(headerText.Style!.Justification);
                var myFont = ConvertFont(headerText.TextStyle!.FontName!);
                var fontSize = headerText.TextStyle.FontSize ?? 10;

                // Example header content - you can customize this
                Paragraph header = new Paragraph(text)
                    .SetFont(myFont)
                    .SetFontSize(fontSize)
                    .SetTextAlignment(textAlignment);
                if (headerText.TextStyle.FontIsBold ?? false)
                {
                    header = header.SetBold();
                }
                if (headerText.TextStyle.FontColor != null)
                {
                    header = header.SetFontColor(new DeviceRgb(headerText.TextStyle.FontColor.Value.R,
                        headerText.TextStyle.FontColor.Value.G, headerText.TextStyle.FontColor.Value.B));
                }

                headerCanvas.Add(header);
                headerCanvas.Close();
            }

        }

        void HandleFooter() {
            var footerPageNumber = this.reportModel.FooterSections.OfType<PageNumberSectionModel>().FirstOrDefault();
            if (footerPageNumber != null)
            {
                float footerY = pageSize.GetBottom() + 30;
                Canvas footerCanvas = new Canvas(pdfCanvas,
                    new Rectangle(pageSize.GetLeft() + 35, footerY - 20, pageSize.GetWidth() - 70, 20));

                var text = ConvertText(footerPageNumber.Format!);

                var textAlignment = ConvertAlignment(footerPageNumber.Style!.Justification);
                var myFont = ConvertFont(footerPageNumber.TextStyle!.FontName!);
                var fontSize = footerPageNumber.TextStyle!.FontSize ?? 10;

                // Example header content - you can customize this
                Paragraph footer = new Paragraph(text)
                    .SetFont(myFont)
                    .SetFontSize(fontSize)
                    .SetTextAlignment(textAlignment);
                if (footerPageNumber.TextStyle!.FontIsBold ?? false)
                {
                    footer = footer.SetBold();
                }
                if (footerPageNumber.TextStyle!.FontColor != null)
                {
                    footer = footer.SetFontColor(new DeviceRgb(footerPageNumber.TextStyle!.FontColor!.Value.R,
                        footerPageNumber.TextStyle!.FontColor.Value.G, footerPageNumber.TextStyle!.FontColor.Value.B));
                }

                footerCanvas.Add(footer);
                footerCanvas.Close();
            }

            var footTexts = this.reportModel.FooterSections.OfType<TextSectionModel>().ToList();
            float footerYText = pageSize.GetBottom() + 30;
            foreach (var item in footTexts ?? Enumerable.Empty<TextSectionModel>())
            {
                Canvas footerCanvas = new Canvas(pdfCanvas,
                    new Rectangle(pageSize.GetLeft() + 35, footerYText -20, pageSize.GetWidth() - 70, 20));
                footerYText = footerYText + 20;


                // Example header content - you can customize this
                Paragraph footer = new Paragraph(item.Content.Value).SetFont(this.fonts["helvetica"]);

                if (item.Content.Style != null)
                {
                    if (!string.IsNullOrWhiteSpace(item.Content.Style.FontName))
                    {
                        var myFont = ConvertFont(item.Content.Style.FontName!);
                        footer = footer.SetFont(myFont);
                    }
                    if (item.Content.Style.FontIsBold ?? false)
                    {
                        footer = footer.SetBold();
                    }
                    var fontSize = item.Content.Style!.FontSize ?? 10;
                    footer = footer.SetFontSize(fontSize);

                    var textAlignment = ConvertAlignment(item.Style?.Justification ?? Justifications.Left);
                    footer = footer.SetTextAlignment(textAlignment);

                    if (item.Content.Style!.FontColor != null)
                    {
                        footer = footer.SetFontColor(new DeviceRgb(item.Content.Style!.FontColor!.Value.R,
                            item.Content.Style!.FontColor.Value.G, item.Content.Style!.FontColor.Value.B));
                    }
                }



                footerCanvas.Add(footer);
                footerCanvas.Close();

            }
        }

        PdfFont ConvertFont(string fontName)
        {
            return this.fonts[fontName.Trim().ToLower()];
            //return fontName.Trim().ToLower() switch
            //{
            //    "helvetica" => PdfFontFactory.CreateFont(Files.helvetica, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED),
            //    "courier" => PdfFontFactory.CreateFont(Files.cour, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED),
            //    "arial" => PdfFontFactory.CreateFont(Files.arial, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED),
            //    _ => PdfFontFactory.CreateFont(Files.helvetica, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED)
            //};
        }

        TextAlignment ConvertAlignment(Justifications? justification)
        {
            return justification switch
            {
                Justifications.Left => TextAlignment.LEFT,
                Justifications.Center => TextAlignment.CENTER,
                Justifications.Right => TextAlignment.RIGHT,
                _ => TextAlignment.LEFT
            };
        }

        string ConvertText(string format) {
            return format.Replace($"{{{CommandMarkersHelper.GetCurrentPageNumberMarker()}}}", pageNumber.ToString())
                            .Replace($"{{{CommandMarkersHelper.GetNumPagesMarker()}}}", totalPages.ToString());
        }
    }
}
