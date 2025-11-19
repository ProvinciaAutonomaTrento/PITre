// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using Microsoft.Extensions.Logging;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Decorators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using Pi3.Infrastructure.IText.Decorator.Exceptions;
using Pi3.Core.Extensions;
using iText.Layout.Renderer;
using iText.Kernel.Geom;
using Pi3.Core.Services.File.ReportGenerator;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using iText.IO.Font;
using iText.Layout.Properties;
using Path = System.IO.Path;
using Org.BouncyCastle.Pkcs;

namespace Pi3.Infrastructure.IText.Decorator.Services
{
    public class ITextFileDecoratorService : IFileDecoratorService
    {
        protected readonly ILogger<ITextFileDecoratorService> _logger;
        protected readonly List<string> _supportedFormats;
        protected IMapper? _mapper;

        public ITextFileDecoratorService(ILogger<ITextFileDecoratorService> logger)
        {
            this._logger = logger;
            this._supportedFormats = new()
            {
                ".pdf"
            };
        }

        public string FileName { get; set; }

        #region Public Members

        public async Task<bool> IsSupportedInputFileFormat(string inputFileFormat)
        {
            if (!this._supportedFormats.Any()) 
                throw new MissingInputFormatConfigurationException();

            string? extension = System.IO.Path.GetExtension(inputFileFormat);

            return this._supportedFormats.Any(f => f.Equals(extension, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<IReadOnlyList<string>> GetSupportedInputFileFormats()
        {
            return this._supportedFormats.AsReadOnly();
        }



       

        public async virtual Task<FileDecoratedContent> Decorate(string inputFileFormat, Stream inputFileStream, FileDecoratorInstructions decoratorInstructions, FileDecoratorOutputFormatsEnum outputFormat)
        {
            if (!await this.IsSupportedInputFileFormat(inputFileFormat))
            {
                // Formato file in input non supportato per la decorazione
                throw new NotSupportedPi3Exception(Resources.ErrorDescriptions.FormatoInputNonSupportato, Resources.ErrorDescriptions.ResourceManager, inputFileFormat);
            }

            if (outputFormat != FileDecoratorOutputFormatsEnum.ToPdf)
            {
                // Formato di output non supportato
                throw new NotSupportedPi3Exception(Resources.ErrorDescriptions.FormatoOutputNonSupportato, Resources.ErrorDescriptions.ResourceManager);
            }

            var metadata = new List<KeyValuePair<string, string>>();

            using MemoryStream output = new MemoryStream();
            {
                using PdfReader pdfReader = new PdfReader(inputFileStream);
                using PdfWriter pdfWriter = new PdfWriter(output);
                using PdfDocument pdfDoc = new PdfDocument(pdfReader, pdfWriter);

                // Crea un oggetto Document per aggiungere contenuti al PDF
                using var document = new Document(pdfDoc);

                foreach (var textLayer in decoratorInstructions.Layers.Where(l => l.GetType() == typeof(TextLayer)).OfType<TextLayer>())
                {
                    if (textLayer.Position != null)
                        throw new NotSupportedPi3Exception(Resources.ErrorDescriptions.PositionNotSupported, Resources.ErrorDescriptions.ResourceManager);

                    //var pageNumbers = textLayer.PageNumbersToApplyLayer ?? new int[0];

                    foreach (var p in textLayer.PageNumbersToApplyLayer ?? new int[0])
                    {
                        var page = pdfDoc.GetPage(p);

                        await this.AppendTextLayer(textLayer, pdfDoc, document, page);
                    }
                    
                    //PdfPage page = null!;
                    
                    //if (pageNumbers.Count() > 1)
                    //    throw new NotSupportedPi3Exception(Resources.ErrorDescriptions.PageLayerNotSupported, Resources.ErrorDescriptions.ResourceManager);
                    //else if (!pageNumbers.Any() || pageNumbers.Contains(1))
                    //    page = pdfDoc.GetFirstPage();
                    //else if (pageNumbers.Contains(pdfDoc.GetNumberOfPages()))
                    //    page = pdfDoc.GetLastPage();
                    //else
                    //    throw new NotSupportedPi3Exception(Resources.ErrorDescriptions.PageLayerNotSupported, Resources.ErrorDescriptions.ResourceManager);

                    //var pageNumber = pdfDoc.GetPageNumber(page);
                    //var pageMediaBox = page.GetMediaBox();
                    
                    //// Crea un font personalizzato a partire dal nome del font
                    //PdfFont font = PdfFontFactory.CreateFont(this.MapTextLayerFont(textLayer), PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED);
                        
                    //int r = 255, g = 0, b = 0; // Default red
                    //if (textLayer.FontForeColor != null)
                    //{
                    //    r = textLayer!.FontForeColor.R;
                    //    g = textLayer.FontForeColor.G;
                    //    b = textLayer.FontForeColor.B;
                    //}
                        
                    //var fontColor = new DeviceRgb(r, g, b);
                        
                    //// Crea un paragrafo con il testo
                    //Paragraph paragraph = new Paragraph(textLayer.Text)
                    //    .SetFont(font)
                    //    .SetFontColor(fontColor) // Imposta il colore del font
                    //    .SetFontSize(textLayer.FontSize ?? 12); // Impostazione della dimensione del font

                    //// Ottieni il renderer del paragrafo
                    //ParagraphRenderer renderer = (ParagraphRenderer)paragraph.CreateRendererSubTree();

                    //// Calcola l'area disponibile per il paragrafo
                    //float availableWidth = pageMediaBox.GetWidth(); // - document.GetLeftMargin() - document.GetRightMargin();
                    //float availableHeight = pageMediaBox.GetHeight(); // - document.GetTopMargin() - document.GetBottomMargin();

                    //// Simula il layout per ottenere le dimensioni
                    //var area = new iText.Kernel.Geom.Rectangle(0, 0, availableWidth, availableHeight);
                    //renderer.SetParent(document.GetRenderer());
                    //renderer.Layout(new iText.Layout.Layout.LayoutContext(new iText.Layout.Layout.LayoutArea(pageNumber, area)));

                    //// Creazione di un oggetto Text
                    //Text textElement = new Text(textLayer.Text).SetFont(font).SetFontSize(textLayer.FontSize ?? 12);

                    //// Ottieni il renderer del testo
                    //TextRenderer textRenderer = (TextRenderer)textElement.CreateRendererSubTree();
                    //var textArea = new iText.Kernel.Geom.Rectangle(0, 0, availableWidth, availableHeight);

                    //// Simula il layout per calcolare la larghezza effettiva
                    //textRenderer.SetParent(document.GetRenderer());
                    //textRenderer.Layout(new iText.Layout.Layout.LayoutContext(new iText.Layout.Layout.LayoutArea(pageNumber, textArea)));

                    //// Ottieni le dimensioni effettive occupate dal testo
                    //float textWidth = textRenderer.GetOccupiedArea().GetBBox().GetWidth();
                    //float textHeight = textRenderer.GetOccupiedArea().GetBBox().GetHeight();

                    //var x = (float) textLayer.CustomPosition!.Value.X;
                    //if ((x + textWidth) > availableWidth)
                    //{
                    //    // Se le coordinate x piu la larghezza del testo eccedono la dimensione disponibile della pagina,
                    //    // allora sottrae alla dimensione disponibile della pagina la larghezza del testo
                    //    x = availableWidth - textWidth;
                    //}

                    //// Ottieni le dimensioni effettive del paragrafo
                    //float paragraphWidth = renderer.GetOccupiedArea().GetBBox().GetWidth();
                    //float paragraphHeight = renderer.GetOccupiedArea().GetBBox().GetHeight();

                    //var y = (float)textLayer.CustomPosition!.Value.Y;
                    //if ((y + paragraphHeight) > availableHeight)
                    //    y = availableHeight;
                    //else
                    //    y = (y + paragraphHeight);
                    
                    //y = pageMediaBox.GetHeight() - y;

                    //paragraph.SetFixedPosition(
                    //    pageNumber, 
                    //    x, 
                    //    y,
                    //    availableWidth);  

                    //// Aggiungi il paragrafo al documento
                    //document.Add(paragraph);
                }
                //PlotLines(pdfDoc, 1, true);


                var info = pdfDoc.GetDocumentInfo();

                metadata.Add(new KeyValuePair<string, string>("Document.Author", info.GetAuthor()));
                metadata.Add(new KeyValuePair<string, string>("document.Comment", info.GetMoreInfo("Comment")));
                metadata.Add(new KeyValuePair<string, string>("document.CreationDate", info.GetMoreInfo("CreationDate")));
                metadata.Add(new KeyValuePair<string, string>("document.Creator", info.GetCreator()));
                metadata.Add(new KeyValuePair<string, string>("document.Keywords", info.GetKeywords()));
                metadata.Add(new KeyValuePair<string, string>("document.ModificationDate", info.GetMoreInfo("ModDate")));
                metadata.Add(new KeyValuePair<string, string>("document.Producer", info.GetProducer()));
                metadata.Add(new KeyValuePair<string, string>("document.Subject", info.GetSubject()));
                metadata.Add(new KeyValuePair<string, string>("document.Title", info.GetTitle()));

                var firstPage = pdfDoc.GetFirstPage();

                var mediaBox = firstPage.GetMediaBox();
                float width = mediaBox.GetWidth();
                float height = mediaBox.GetHeight();

                metadata.Add(new KeyValuePair<string, string>("PageInfo.Width", width.ToString()));
                metadata.Add(new KeyValuePair<string, string>("PageInfo.Height", height.ToString()));
                metadata.Add(new KeyValuePair<string, string>("PageInfo.IsLandscape", width > height ? true.ToString() : false.ToString()));
                metadata.Add(new KeyValuePair<string, string>("document.Pages.Count", pdfDoc.GetNumberOfPages().ToString()));
            }

            return new FileDecoratedContent()
            {
                Content = output.ToArray(),
                ContentType = "application/pdf",
                Name = (string.IsNullOrEmpty(System.IO.Path.GetFileNameWithoutExtension(inputFileFormat)) ?
                        $"decorated_{DateTime.Now:yyyyMMddHHmmss}.pdf" : $"{System.IO.Path.GetFileNameWithoutExtension(inputFileFormat)}.pdf"),
                Metadata = metadata
            };
        }

        public async virtual Task<FileDecoratedContent> Decorate(string inputFileFormat, byte[] inputFileContent, FileDecoratorInstructions decoratorInstructions, FileDecoratorOutputFormatsEnum outputFormat)
        {
            using MemoryStream memoryStream = new(inputFileContent);
            return await this.Decorate(inputFileFormat, memoryStream, decoratorInstructions, outputFormat);
        }

        #endregion

        #region Private Members
        protected virtual string MapTextLayerFont(TextLayer textLayer)
        {
            switch ((textLayer.FontName ?? string.Empty)!.ToLowerInvariant())
            {
                case "times":
                    return iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN;
                case "times_bold":
                    return iText.IO.Font.Constants.StandardFonts.TIMES_BOLD;
                case "helvetica":
                    return iText.IO.Font.Constants.StandardFonts.HELVETICA;
                case "helvetica_bold":
                    return iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD;
                case "courier":
                    return iText.IO.Font.Constants.StandardFonts.COURIER;
                case "courier_bold":
                    return iText.IO.Font.Constants.StandardFonts.COURIER_BOLD;
                default:
                    throw new NotSupportedPi3Exception(Resources.ErrorDescriptions.FontNotSupported, Resources.ErrorDescriptions.ResourceManager);
            }
        }

        public async Task PlotLines(PdfDocument pdfDoc, int pageNumber, bool reversed)
        {
            await Task.Run(() =>
            {
                var page = pdfDoc.GetPage(pageNumber);
                var document = new Document(pdfDoc);
                //var canvas = new PdfCanvas(page);

                var xPositions = new int[] { 5, 100, 500 };

                var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                var top = GetHeight(page);
                float paragraphHeight = 8;

                foreach (var xPosition in xPositions)
                {
                    for (int i = 0; i < 500; i++)
                    {
                        var yPosition = 10 + i * 50;
                        if (yPosition > top)
                            break;
                        var yPagePosition = reversed ? (int)top - yPosition : yPosition;


                        //if (page.GetRotation() == 270) {
                        //    var temp = xPosition;
                        //    xPosition = yPagePosition;
                        //    yPagePosition = temp;
                        //    //paragraph.SetRotationAngle(rotationAngle);
                        //}
                        // Compensate for page rotation
                        float x = xPosition;
                        float y = yPagePosition;
                        switch (page.GetRotation())
                        {
                            case 90:
                                {
                                    float temp = x;
                                    x = y;
                                    y = top - temp;
                                    break;
                                }
                            case 180:
                                {
                                    x = top - x;
                                    y = top - y;
                                    break;
                                }
                            case 270:
                                {
                                    //float temp = x;
                                    //x = top - y;
                                    //y = temp;
                                    float temp = x;
                                    x = y;
                                    y = top - temp;

                                    break;
                                }
                        }

                        // Create a paragraph with the text
                        Paragraph paragraph = new Paragraph($"({xPosition}, {yPagePosition}, {yPosition})")
                            .SetFont(font)
                            .SetFontSize(12)
                            .SetRotationAngle(page.GetRotation() * (Math.PI / 180))
                            //.SetFixedPosition(pageNumber, x, y + paragraphHeight)
                            .SetFixedPosition(pageNumber, x, y)
                            //.SetRotationAngle(page.GetRotation());
                            ;

                        //if (paragraphHeight == 0) { 
                        //    // Calculate the height of the paragraph
                        //    ParagraphRenderer renderer = (ParagraphRenderer)paragraph.CreateRendererSubTree();
                        //    var area = new iText.Kernel.Geom.Rectangle(0, 0, 200, float.MaxValue); // Adjust width as needed
                        //    renderer.SetParent(document.GetRenderer());
                        //    renderer.Layout(new iText.Layout.Layout.LayoutContext(new iText.Layout.Layout.LayoutArea(pageNumber, area)));
                        //    paragraphHeight = renderer.GetOccupiedArea().GetBBox().GetHeight();
                        //}

                        //Thread.Sleep(50);
                        // Add the paragraph to the document
                        document.Add(paragraph);

                        // Draw the text at the specified position
                        //canvas.BeginText();
                        //canvas.SetFontAndSize(font, 12);
                        //canvas.SetTextMatrix(xPosition, yPagePosition);
                        //canvas.ShowText($"({xPosition}, {yPagePosition}, {yPosition})");
                        //canvas.EndText();
                    }
                }

                document.Flush();
            });
        }

        private int GetHeight(PdfPage page)
        {
            var top = 0.0;
            var src = String.Empty;
            var pageSize = page.GetPageSize();
            if (page.GetRotation() == 90 || page.GetRotation() == 270)
            {
                top = pageSize.GetWidth();
                src = "Width";
            }
            else
            {
                top = pageSize.GetHeight();
                src = "Height";
            }
            return (int)top;
        }

        protected async virtual Task AppendTextLayer(TextLayer textLayer, PdfDocument pdfDoc, Document document, PdfPage page)
        {
            // Leggi i margini attuali (se impostati a livello di documento)
            float marginLeft = document.GetLeftMargin();
            float marginRight = document.GetRightMargin();
            float marginTop = document.GetTopMargin();
            float marginBottom = document.GetBottomMargin();
            //if (marginLeft != 0 || marginRight != 0 || marginTop != 0 || marginBottom != 0)
            //    document.SetMargins(0, 0, 0, 0);


            var pageNumber = pdfDoc.GetPageNumber(page);
            var pageMediaBox = page.GetMediaBox();
            var rotation = page.GetRotation();

            // Ottieni la dimensione della pagina corrente
            var pageSize = page.GetPageSize();
            var cropBox = page.GetCropBox();

            // Determina se la pagina è portrait o landscape
            bool isLandscape = pageSize.GetWidth() > pageSize.GetHeight() || rotation == 90 || rotation == 270;

            // Crea un font personalizzato a partire dal nome del font
            PdfFont font = PdfFontFactory.CreateFont(MapTextLayerFont(textLayer), PdfFontFactory.EmbeddingStrategy.PREFER_NOT_EMBEDDED);

            int r = 255, g = 0, b = 0; // Default red
            if (textLayer.FontForeColor != null)
            {
                r = textLayer!.FontForeColor.R;
                g = textLayer.FontForeColor.G;
                b = textLayer.FontForeColor.B;
            }

            var fontColor = new DeviceRgb(r, g, b);

            // Crea un paragrafo con il testo
            Paragraph paragraph = new Paragraph(textLayer.Text)
                .SetFont(font)
                .SetFontColor(fontColor) // Imposta il colore del font
                .SetFontSize(textLayer.FontSize ?? 12); // Impostazione della dimensione del font

            //if (isLandscape)
            //{
            //    // Applica una trasformazione di rotazione
            //    paragraph.SetRotationAngle(Math.PI / 2); // Rotazione di 90 gradi
            //}
            if (rotation != 0)
            {
                double rotationAngle = (180 - rotation) * (Math.PI / 180); // Converti i gradi in radianti e inverti il segno
                paragraph.SetRotationAngle(rotationAngle);
            }


            // Ottieni il renderer del paragrafo
            ParagraphRenderer renderer = (ParagraphRenderer)paragraph.CreateRendererSubTree();

            // Calcola l'area disponibile per il paragrafo
            float availableWidth = pageMediaBox.GetWidth(); // - document.GetLeftMargin() - document.GetRightMargin();
            float availableHeight = pageMediaBox.GetHeight(); // - document.GetTopMargin() - document.GetBottomMargin();

            if (rotation == 90 || rotation == 270)
            {
                availableHeight = pageMediaBox.GetWidth(); // - document.GetLeftMargin() - document.GetRightMargin();
                availableWidth = pageMediaBox.GetHeight(); // - document.GetTopMargin() - document.GetBottomMargin();
            }

            // Simula il layout per ottenere le dimensioni
            var area = new iText.Kernel.Geom.Rectangle(0, 0, availableWidth, availableHeight);
            renderer.SetParent(document.GetRenderer());
            renderer.Layout(new iText.Layout.Layout.LayoutContext(new iText.Layout.Layout.LayoutArea(pageNumber, area)));

            // Creazione di un oggetto Text
            Text textElement = new Text(textLayer.Text).SetFont(font).SetFontSize(textLayer.FontSize ?? 12);

            // Ottieni il renderer del testo
            TextRenderer textRenderer = (TextRenderer)textElement.CreateRendererSubTree();
            var textArea = new iText.Kernel.Geom.Rectangle(0, 0, availableWidth, availableHeight);

            // Simula il layout per calcolare la larghezza effettiva
            textRenderer.SetParent(document.GetRenderer());
            textRenderer.Layout(new iText.Layout.Layout.LayoutContext(new iText.Layout.Layout.LayoutArea(pageNumber, textArea)));

            // Ottieni le dimensioni effettive occupate dal testo
            float textWidth = textRenderer.GetOccupiedArea().GetBBox().GetWidth();
            float textHeight = textRenderer.GetOccupiedArea().GetBBox().GetHeight();

            var numLines = textLayer.Text.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if (rotation == 270 || rotation == 90)
            {
                var temp = textWidth;
                textWidth = textHeight;
                textHeight = temp;
            }

            var x = (float)textLayer.CustomPosition!.Value.X;

            if (rotation == 270)
            {
                x = availableWidth - x;
            }
            // temp: da capire come gestirlo con altri orientamenti
            if (rotation == 0)
            {
                if ((x + textWidth) > availableWidth)
                {
                    // Se le coordinate x piu la larghezza del testo eccedono la dimensione disponibile della pagina,
                    // allora sottrae alla dimensione disponibile della pagina la larghezza del testo
                    x = availableWidth - textWidth;
                }
            }

            // Ottieni le dimensioni effettive del paragrafo
            float paragraphWidth = renderer.GetOccupiedArea().GetBBox().GetWidth();
            float paragraphHeight = renderer.GetOccupiedArea().GetBBox().GetHeight();
            //paragraphHeight = textLayer.FontSize.Value * numLines.Count();

            if (rotation == 270 || rotation == 90)
            {
                var temp = paragraphWidth;
                paragraphWidth = paragraphHeight;
                paragraphHeight = temp;
            }

            var y = (float)textLayer.CustomPosition!.Value.Y;
            if ((y + paragraphHeight) > availableHeight)
                y = availableHeight;
            else { 
                y = (y + paragraphHeight);
            }

            y = availableHeight - y;

            if (rotation == 90 || rotation == 270)
            {
                var temp = x;
                x = y;
                y = temp;
            }

            // temp
            //x += marginBottom;

            paragraph.SetFixedPosition(
                pageNumber,
                x,
                y,
                availableWidth);

            // Aggiungi il paragrafo al documento
            document.Add(paragraph);


            // Disegna un rettangolo attorno all'area occupata dal paragrafo

        }


        #endregion

    }


}
