// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.File.ReportGenerator;
using System.ComponentModel.DataAnnotations;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
//
namespace Pi3.Infrastructure.DocumentFormat.OpenXml.Services.ReportGenerator
{

    public class OpenXmlReportGeneratorService : IReportGeneratorService
    {
        #region Public Members

        public OpenXmlReportGeneratorService(ILogger<OpenXmlReportGeneratorService> logger)
        {
            this._logger = logger;
        }

        public async Task<ReportGeneratorCapabilities> GetCapabilities()
        {
            return new ReportGeneratorCapabilities()
            {
                SupportedOutputTypes = new ReportOutputTypes[1] { ReportOutputTypes.AsDocx },
                SupportedOrientations = new PageOrientations[2] { PageOrientations.Landscape, PageOrientations.Portrait },
                SupportedPageTypes = new PageSizes[2] { PageSizes.A3, PageSizes.A4 },
                SupportedSections = new Type[6]
                {
                    typeof(TextSectionModel), typeof(GridSectionModel), typeof(PageNumberSectionModel),
                    typeof(BreakPageSectionModel), typeof(ImageSectionModel), typeof(LineSectionModel)
                },
                HeaderSupported = true,
                FooterSupported = true
            };
        }

        public async virtual Task<GeneratedReport> Generate(ReportModel reportModel, Stream outputStream)
        {
            var defaultPageSettings = new List<PageSettings>()
            {
                new PageSettings(PageSizes.A4, PageOrientations.Portrait, 11906, 16838),
                new PageSettings(PageSizes.A4, PageOrientations.Landscape, 16838, 11906),
                new PageSettings(PageSizes.A3, PageOrientations.Portrait, 16838, 23811),
                new PageSettings(PageSizes.A3, PageOrientations.Landscape, 23811, 16838),
            };

            reportModel = reportModel ?? throw new ArgumentNullException(nameof(reportModel));
            outputStream = outputStream ?? throw new ArgumentNullException(nameof(outputStream));

            Validator.ValidateObject(reportModel, new ValidationContext(reportModel));

            using WordprocessingDocument doc = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document);

            MainDocumentPart mainPart = doc.AddMainDocumentPart();

            mainPart.Document = new Document();

            Body body = new Body();
            OpenXmlCompositeElement? footer = null;
            OpenXmlCompositeElement? header = null;

            SectionProperties sp = new SectionProperties();
            
            var pageSize = defaultPageSettings.First(ps => ps.Size == reportModel.Size && ps.Orientation == reportModel.Orientation);            
            sp.AppendChild(new PageSize() { Width = pageSize.Width, Height = pageSize.Height, Orient = new PageOrientationValues(pageSize.Orientation.ToString().ToLowerInvariant()) });

            body.Append(sp.AsArray());
            mainPart.Document.AppendChild(body);

            var headers = reportModel.HeaderSections ?? Enumerable.Empty<ISectionModel>();
       
            InitializeAndProcessSections(headers, InitHeader!, ref header!);

            foreach (var section in reportModel.Sections ?? Enumerable.Empty<ISectionModel>())
            {
                ProcessSection(section, body);    
            }

            var footers = reportModel.FooterSections ?? Enumerable.Empty<ISectionModel>();
            InitializeAndProcessSections(footers, InitFooter!, ref footer!);

            return new GeneratedReport()
            {
                FileName = $"Report_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.docx",
                ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            };

            // Funzione interna per inizializzare e processare le sezioni di intestazione e piè di pagina
            void InitializeAndProcessSections(IEnumerable<ISectionModel> sections, Func<MainDocumentPart, IEnumerable<ISectionModel>, OpenXmlCompositeElement> initMethod, ref OpenXmlCompositeElement element)
            {
                if (sections.OfType<ImageSectionModel>().Any())
                {
                    element = initMethod(mainPart, sections);

                    foreach (var section in sections.Where(s => !(s is ImageSectionModel)))
                    {
                        ProcessSection(section, element);
                    }
                }
            }

            void ProcessSection(ISectionModel section, OpenXmlCompositeElement rootElement) {
                if (section.GetType() == typeof(TextSectionModel))
                {
                    rootElement.Append(this.DrawParagraph((TextSectionModel)section, 10, 10, 10).AsArray());
                }
                else if (section.GetType() == typeof(GridSectionModel))
                {
                    rootElement.Append(this.DrawTable((GridSectionModel)section).AsArray());
                }
                else if (section.GetType() == typeof(PageNumberSectionModel))
                {
                    rootElement.Append(AddPageNumber((PageNumberSectionModel)section).AsArray());
                }
                else if (section.GetType() == typeof(BreakPageSectionModel))
                {
                    rootElement.Append(CreatePageBreak().AsArray());
                }
                else if (section.GetType() == typeof(ImageSectionModel))
                {
                    rootElement.Append(AddImageToBody(mainPart, (ImageSectionModel)section).AsArray());
                }
                else if (section.GetType() == typeof(LineSectionModel))
                {
                    rootElement.Append(CreateHorizontalLine((LineSectionModel)section).AsArray());
                }

                rootElement.Append(new Paragraph().AsArray());
            }
        }

        private Footer? InitFooter(MainDocumentPart mainPart, IEnumerable<ISectionModel> footerSections)
        {
            FooterPart footerPart = mainPart.AddNewPart<FooterPart>();
            string footerPartId = mainPart.GetIdOfPart(footerPart);

            Footer footer = new Footer();

            footerPart.Footer = footer;


            // Aggiungi tutte le immagini al piè di pagina
            foreach (var section in footerSections.OfType<ImageSectionModel>())
            {
                AddImage(footer, mainPart, section);
            }

            SectionProperties sectionProperties = mainPart.Document.Body!.Elements<SectionProperties>()!.FirstOrDefault()!;
            if (sectionProperties == null)
            {
                sectionProperties = new SectionProperties();
                mainPart.Document.Body.Append(sectionProperties.AsArray());
            }

            FooterReference footerReference = new FooterReference() { Type = HeaderFooterValues.Default, Id = footerPartId };
            sectionProperties.Append(footerReference.AsArray());
            
            return footer;
        }

        private Header? InitHeader(MainDocumentPart mainPart, IEnumerable<ISectionModel> headerSections)
        {
            HeaderPart headerPart = mainPart.AddNewPart<HeaderPart>();
            string headerPartId = mainPart.GetIdOfPart(headerPart);

            Header header = new Header();
            headerPart.Header = header;

            // Aggiungi tutte le immagini all'intestazione
            foreach (var section in headerSections.OfType<ImageSectionModel>())
            {
                AddImage(header, mainPart, section);
            }
            header.Save();

            SectionProperties sectionProperties = mainPart.Document.Body.Elements<SectionProperties>().FirstOrDefault();
            if (sectionProperties == null)
            {
                sectionProperties = new SectionProperties();
                mainPart.Document.Body.Append(sectionProperties.AsArray());
            }

            HeaderReference headerReference = new HeaderReference() { Type = HeaderFooterValues.Default, Id = headerPartId };
            sectionProperties.Append(headerReference.AsArray());
            
            return header;
        }

        #endregion

        #region Public Members

        protected record PageSettings(PageSizes Size, PageOrientations Orientation, uint Width, uint Height);
        protected readonly ILogger<OpenXmlReportGeneratorService> _logger;

        protected string ColorToHex(System.Drawing.Color color)
        {
            return $"{color.R:X2}{color.G:X2}{color.B:X2}";
        }
        protected virtual Table DrawTable(GridSectionModel sectionModel)
        {
            Table table = new Table();

            const int tableFullWidth = 5000;
            int tableWidth = tableFullWidth;

            if (sectionModel.Style != null && sectionModel.Style.WithPercentage.HasValue)
                tableWidth = (tableFullWidth * sectionModel.Style.WithPercentage.Value) / 100;

            TableProperties tableProperties = new TableProperties(
                new TableStyle() { Val = "TableGrid" },
                new TableWidth() { Width = tableWidth.ToString(), Type = TableWidthUnitValues.Pct },
                new TableBorders(new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Birds), Size = 1 },
                    new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Birds), Size = 1 },
                    new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Birds), Size = 1 },
                    new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Birds), Size = 1 },
                    new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Birds), Size = 1 },
                    new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Birds), Size = 1 }));

            table.AppendChild(tableProperties);

            foreach (var rowModel in sectionModel.Rows)
            {
                var tr = new TableRow();

                foreach (var cellModel in rowModel.Cells)
                {
                    var tc = new TableCell();
                    var tcp = new TableCellProperties();
                    var p = new Paragraph();
                    var pp = new ParagraphProperties();

                    if (cellModel.Style! != null!)
                    {
                        if (cellModel.Style.VerticalAlignment.HasValue)
                            tcp.Append(new TableCellVerticalAlignment() { Val = new TableVerticalAlignmentValues(cellModel.Style.VerticalAlignment.Value.ToString().ToLowerInvariant()) }.AsArray());

                        if (cellModel.Style.Justification.HasValue)
                            pp.Append(new Justification() { Val = new JustificationValues(cellModel.Style.Justification.Value.ToString().ToLowerInvariant()) }.AsArray());

                        if (cellModel.Style.WithPercentage.HasValue)
                            tcp.Append(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = ((tableWidth * cellModel.Style.WithPercentage) / 100).ToString() }.AsArray());

                        if (cellModel.Style.ForegroundColor.HasValue)
                        {
                            tcp.Append(new Shading()
                            {
                                Color = "auto",
                                Fill = this.ColorToHex(cellModel.Style.ForegroundColor.Value),
                                Val = ShadingPatternValues.Clear
                            }.AsArray());
                        }
                    }

                    tc.Append(tcp.AsArray());
                    p.Append(pp.AsArray());
                    p.Append(this.DrawRun(cellModel.Content).AsArray());
                    tc.Append(p.AsArray());

                    tr.Append(tc.AsArray());
                }

                table.Append(tr.AsArray());
            }

            return table;
        }

        protected virtual Paragraph DrawParagraph(TextSectionModel textSectionModel, int? spacingBefore = null, int? spacingAfter = null, int? lineSpacing = null)
        {
            Paragraph para = new Paragraph();

            var run = this.DrawRun(textSectionModel.Content);

            if (textSectionModel.Style! != null!)
            {
                ParagraphProperties pp = new ParagraphProperties();

                if (textSectionModel.Style.Justification.HasValue)
                    pp.AppendChild(new Justification() { Val = new JustificationValues(textSectionModel.Style.Justification.Value.ToString().ToLowerInvariant()) });

                // Imposta la spaziatura del paragrafo
                SpacingBetweenLines spacing = new SpacingBetweenLines();
                if (spacingBefore.HasValue)
                    spacing.Before = (spacingBefore.Value * 20).ToString(); // OpenXML usa 1/20 di punto
                if (spacingAfter.HasValue)
                    spacing.After = (spacingAfter.Value * 20).ToString(); // OpenXML usa 1/20 di punto
                if (lineSpacing.HasValue)
                    spacing.Line = (lineSpacing.Value * 20).ToString(); // OpenXML usa 1/20 di punto
                spacing.AfterAutoSpacing = new OnOffValue(false);
                spacing.BeforeAutoSpacing = new OnOffValue(false);
                spacing.AfterLines = 0;
                spacing.BeforeLines = 0;
                

                pp.AppendChild(spacing);

                // Aggiungi l'elemento ContextualSpacing, che gestisce la spaziatura tra i paragrafi
                // "Don't add space between paragraphs of the same style" 
                pp.AppendChild(new ContextualSpacing() { Val = true });
                para.AppendChild(pp);
            }
            para.AppendChild(run);

            return para;
        }

        protected virtual Run DrawRun(TextContentModel textContentModel)
        {
            Run formattedRun = new Run();
            RunProperties runPro = new RunProperties();

            if (textContentModel.Style != null)
            {
                runPro.Append(new RunFonts()
                {
                    Ascii = textContentModel.Style.FontName,
                    HighAnsi = textContentModel.Style.FontName
                }.AsArray());

                if (textContentModel.Style.FontIsBold ?? false)
                    runPro.Append(new Bold().AsArray());

                if (textContentModel.Style.FontIsItalic ?? false)
                    runPro.Append(new Italic().AsArray());

                if (textContentModel.Style.FontIsStrikeout ?? false)
                    runPro.Append(new Strike().AsArray());

                if (textContentModel.Style.FontSize.HasValue)
                {
                    runPro.Append(new FontSize()
                    {
                        Val = (textContentModel.Style.FontSize.Value * 2).ToString()  // / 2 ottengo dimensione reale
                    }.AsArray());
                }

                if (textContentModel.Style.FontColor != null)
                {
                    runPro.Append(new Color()
                    {
                        Val = this.ColorToHex(textContentModel.Style.FontColor.Value)
                    }.AsArray());
                }

                if (textContentModel.Style.HighlightColor.HasValue)
                {
                    runPro.Append(new Highlight()
                    {
                        Val = new HighlightColorValues(textContentModel.Style.HighlightColor.Value.Name.ToLowerInvariant())
                    }.AsArray());
                }
            }

            runPro.Append(new Text(textContentModel.Value) { Space = SpaceProcessingModeValues.Preserve  }.AsArray());
            formattedRun.Append(runPro.AsArray());

            return formattedRun;
        }

        private RunProperties ApplyStyling(TextStyleModel? TextStyle, TextSectionStyleModel? Style) {
            var runPro = new RunProperties();

            if (Style!.Justification.HasValue)
            {
                runPro.Append(new Justification() { Val = new JustificationValues(Style.Justification.Value.ToString().ToLowerInvariant()) }.AsArray());
            }

            if (TextStyle != null)
            {
                runPro.Append(new RunFonts()
                {
                    Ascii = TextStyle.FontName,
                    HighAnsi = TextStyle.FontName
                }.AsArray());

                if (TextStyle.FontIsBold ?? false)
                    runPro.Append(new Bold().AsArray());

                if (TextStyle.FontIsItalic ?? false)
                    runPro.Append(new Italic().AsArray());

                if (TextStyle.FontIsStrikeout ?? false)
                    runPro.Append(new Strike().AsArray());

                if (TextStyle.FontSize.HasValue)
                {
                    runPro.Append(new FontSize()
                    {
                        Val = (TextStyle.FontSize.Value * 2).ToString()  // / 2 ottengo dimensione reale
                    }.AsArray());
                }

                if (TextStyle.FontColor != null)
                {
                    runPro.Append(new Color()
                    {
                        Val = this.ColorToHex(TextStyle.FontColor.Value)
                    }.AsArray());
                }

                if (TextStyle.HighlightColor.HasValue)
                {
                    runPro.Append(new Highlight()
                    {
                        Val = new HighlightColorValues(TextStyle.HighlightColor.Value.Name.ToLowerInvariant())
                    }.AsArray());
                }
            }

            return runPro;
        }

        private Paragraph AddPageNumber(PageNumberSectionModel pageNumberSection)
        {
            var para = new Paragraph();
            var run = new Run();
            if(!string.IsNullOrWhiteSpace(pageNumberSection.Format)){

                var runPro = ApplyStyling(pageNumberSection.TextStyle, pageNumberSection.Style);
                run.Append(runPro.AsArray());
                StringToTextFields(pageNumberSection.Format, run);
            }
            para.Append(run.AsArray());

            return para;
        }

        private void StringToTextFields(string text, Run run)
        {
            var parts = SplitLabel(text);
            foreach (var part in parts)
            {
                if (part.StartsWith("{") && part.EndsWith("}"))
                {
                    string fieldInstruction = part.Trim('{', '}');
                    run.Append(new SimpleField() { Instruction = fieldInstruction }.AsArray());
                }
                else
                {
                    run.Append(new Text(part).AsArray());
                }
            }
        }

        protected virtual Paragraph CreatePageBreak()
        {
            Paragraph para = new Paragraph();
            Run run = new Run();
            Break pageBreak = new Break() { Type = BreakValues.Page };
            run.Append(pageBreak.AsArray());
            para.Append(run.AsArray());
            return para;
        }

        private Drawing GetImageReference(ImageSectionModel imageSection, string relationshipId)
        {
            return
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L, Cy = 792000L },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = imageSection.ImageName
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = imageSection.ImageName
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                       "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle })))
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                      )
                );
        }

        private void AddImage(OpenXmlCompositeElement parentElement, MainDocumentPart mainPart, ImageSectionModel imageSection)
        {
            // Determina il tipo di immagine in base all'estensione del file
            string extension = Path.GetExtension(imageSection.ImageName).ToLowerInvariant();
            PartTypeInfo imagePartType;

            switch (extension)
            {
                case ".jpeg":
                case ".jpg":
                    imagePartType = ImagePartType.Jpeg;
                    break;
                case ".png":
                    imagePartType = ImagePartType.Png;
                    break;
                case ".gif":
                    imagePartType = ImagePartType.Gif;
                    break;
                case ".bmp":
                    imagePartType = ImagePartType.Bmp;
                    break;
                case ".tiff":
                    imagePartType = ImagePartType.Tiff;
                    break;
                default:
                    throw new NotSupportedException($"Unsupported image format: {extension}");
            }

            // Create the image part
            ImagePart imagePart = mainPart.AddImagePart(imagePartType);
            using (var stream = new MemoryStream(imageSection.ImageContent))
            {
                imagePart.FeedData(stream);
            }

            // Get the relationship ID of the image part
            string relationshipId = mainPart.GetIdOfPart(imagePart);

            // Define the reference of the image
            var element = GetImageReference(imageSection, relationshipId);
            
            // Append the reference to the parent element
            parentElement.AppendChild(new Paragraph(new Run(element.AsArray()).AsArray()));
        }

        private Paragraph AddImageToBody(MainDocumentPart mainPart, ImageSectionModel imageSection)
        {
            // Create the image part
            var extensios = Path.GetExtension(imageSection.ImageName).ToLower();
            ImagePart imagePart = null!;

            if (extensios == ".png")
            {
                imagePart = mainPart.AddImagePart(ImagePartType.Png);
            }
            else if (extensios == ".jpeg" || extensios == ".jpg")
            {
                imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
            }
            else if (extensios == ".bmp")
            {
                imagePart = mainPart.AddImagePart(ImagePartType.Bmp);
            }
            else if (extensios == ".gif")
            {
                imagePart = mainPart.AddImagePart(ImagePartType.Gif);
            }
            else if (extensios == ".tiff")
            {
                imagePart = mainPart.AddImagePart(ImagePartType.Tiff);
            }
            else
            {
                throw new NotSupportedException($"Image type {extensios} not supported");
            }

            using (var stream = new MemoryStream(imageSection.ImageContent))
            {
                imagePart.FeedData(stream);
            }

            // Get the relationship ID of the image part
            string relationshipId = mainPart.GetIdOfPart(imagePart);

            // Define the reference of the image
            var element = GetImageReference(imageSection, relationshipId);

            // Append the reference to the body
            return new Paragraph(new Run(element));
        }


        private Paragraph CreateHorizontalLine(LineSectionModel lineSection)
        {
            var paragraph = new Paragraph();
            var paragraphProperties = new ParagraphProperties();
            var borders = new ParagraphBorders();

            var bottomBorder = new BottomBorder
            {
                Val = lineSection.Style == LineStyles.Dotted ? BorderValues.Dotted : BorderValues.Single,
                Color = lineSection.Color.HasValue ? ColorToHex(lineSection.Color.Value) : "auto",
                Size = (UInt32Value)(lineSection.Size.HasValue ? lineSection.Size.Value * 2U : 4U), // OpenXML usa 1/8 di punto, quindi moltiplichiamo per 2 per ottenere 1/4 di punto
                Space = (UInt32Value)1U
            };

            borders.Append(bottomBorder.AsArray());
            paragraphProperties.Append(borders.AsArray());
            paragraph.Append(paragraphProperties.AsArray());

            return paragraph;
        }

        private List<string> SplitLabel(string label)
        {
            var parts = new List<string>();
            int currentIndex = 0;

            while (currentIndex < label.Length)
            {
                int startIndex = label.IndexOf('{', currentIndex);
                int endIndex = label.IndexOf('}', currentIndex);

                if (startIndex == -1 && endIndex == -1)
                {
                    parts.Add(label.Substring(currentIndex));
                    break;
                }

                if (startIndex != -1 && (endIndex == -1 || startIndex < endIndex))
                {
                    if (startIndex > currentIndex)
                    {
                        parts.Add(label.Substring(currentIndex, startIndex - currentIndex));
                    }
                    int closeIndex = label.IndexOf('}', startIndex);
                    if (closeIndex != -1)
                    {
                        parts.Add(label.Substring(startIndex, closeIndex - startIndex + 1));
                        currentIndex = closeIndex + 1;
                    }
                    else
                    {
                        parts.Add(label.Substring(startIndex));
                        break;
                    }
                }
                else if (endIndex != -1)
                {
                    parts.Add(label.Substring(currentIndex, endIndex - currentIndex + 1));
                    currentIndex = endIndex + 1;
                }
            }

            return parts;
        }

        #endregion
    }
}
