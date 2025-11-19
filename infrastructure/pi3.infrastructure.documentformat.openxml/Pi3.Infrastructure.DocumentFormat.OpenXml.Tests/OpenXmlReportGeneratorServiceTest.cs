// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Infrastructure.DocumentFormat.OpenXml;
using Pi3.Infrastructure.DocumentFormat.OpenXml.Services.ReportGenerator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.DocumentFormat.OpenXml.Tests
{
    internal class OpenXmlReportGeneratorServiceTest
    {
        ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddInfrastructureOpenXmlReportGeneratorService()
                .BuildServiceProvider();
        }

        [TearDown]
        public void TearDown()
        {
            this._serviceProvider.Dispose();
        }


        [Test]
        [Order(1)]
        public async Task GenerateTestBase()
        {
            var rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape
            };

            rm.AddSection(new TextSectionModel()
            {
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Right
                },
                Content = new TextContentModel()
                {
                    Value = "Titolo di prova",
                    Style = new TextStyleModel()
                    {
                        FontName = "Arial",
                        FontSize = 24, 
                        FontIsBold = true,
                        FontColor = System.Drawing.Color.Magenta
                    }
                }   
            });

            rm.AddSection(new TextSectionModel()
            {
                Content = new TextContentModel()
                {
                    Value = "Titolo di prova 2",
                    Style = new TextStyleModel()
                    {
                        FontName = "Algerian",
                        FontSize = 12,
                        FontIsItalic = true,
                        FontColor = System.Drawing.Color.Red
                    }
                }
            });

            GridSectionModel tsm = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 70
                }
            };
            GridRowModel row1 = new GridRowModel();
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 5, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Registro", Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Prot. / Id Doc.", Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 5, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Data", Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() {Style = new GridCellStyleModel() { WithPercentage = 30, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Oggetto" , Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() {Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Tipo" , Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() {Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Mitt. / Dest." , Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() {Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Codice fascicolo"    } });
            row1.AddCell(new GridCellModel() {Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Annullato"   } });
            row1.AddCell(new GridCellModel() {Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "File" , Style = new TextStyleModel() { FontIsBold = true } } });
            tsm.AddRow(row1);

            GridRowModel row2 = new GridRowModel();
            row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "PAT" } });
            row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "133014903" } });
            row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "13/02/2024" } });
            row2.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Center }, Content = new TextContentModel() { Value = "Test per modifica versione", Style = new TextStyleModel() { FontName = "Colibri", FontIsBold = true, FontColor = System.Drawing.Color.Green, HighlightColor = System.Drawing.Color.Yellow } } });
            row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "NP" } });
            row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "" } });
            row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "6.4-2024-50" } });
            row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "" } });
            row2.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "pdf" } });
            tsm.AddRow(row2);

            rm.AddSection(tsm);

            rm.AddSection(new EmptySectionModel());

            rm.AddSection(new TextSectionModel()
            {
                Content = new TextContentModel() { Value = "Testo di prova" },
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Center
                }
            });
            rm.AddSection(new TextSectionModel()
            {
                Content = new TextContentModel() { Value = "Testo di prova 2" },
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Center
                }
            });

            var reportGenerator = (OpenXmlReportGeneratorService) _serviceProvider.GetRequiredService<IReportGeneratorService>();

            // Scrivi il contenuto del MemoryStream su un file
            using (var stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
            }

            Assert.Pass();
        }

        private static TextSectionModel GetSectionTitoloProva() 
        {
            return new TextSectionModel()
            {
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Right
                },
                Content = new TextContentModel()
                {
                    Value = "Titolo di prova",
                    Style = new TextStyleModel()
                    {
                        FontName = "Arial",
                        FontSize = 24,
                        FontIsBold = true,
                        FontColor = System.Drawing.Color.Magenta
                    }
                }
            };
        }

        private static TextSectionModel GetSectionTitoloProva2()
        {
            return new TextSectionModel()
            {
                Content = new TextContentModel()
                {
                    Value = "Titolo di prova 2",
                    Style = new TextStyleModel()
                    {
                        FontName = "Algerian",
                        FontSize = 12,
                        FontIsItalic = true,
                        FontColor = System.Drawing.Color.Red
                    }
                }
            };
        }

        private static GridRowModel GetGridHeader() 
        {
            var row1 = new GridRowModel();
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 5, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Registro", Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Prot. / Id Doc.", Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 5, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Data", Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 30, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Oggetto", Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Tipo", Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Mitt. / Dest.", Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Codice fascicolo" } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Annullato" } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "File", Style = new TextStyleModel() { FontIsBold = true } } });

            return row1;
        }

        private static GridRowModel GetGridRow()
        {
            var row1 = new GridRowModel();
            row1.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "PAT" } });
            row1.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "133014903" } });
            row1.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "13/02/2024" } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Center }, Content = new TextContentModel() { Value = "Test per modifica versione", Style = new TextStyleModel() { FontName = "Colibri", FontIsBold = true, FontColor = System.Drawing.Color.Green, HighlightColor = System.Drawing.Color.Yellow } } });
            row1.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "NP" } });
            row1.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "" } });
            row1.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "6.4-2024-50" } });
            row1.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "" } });
            row1.AddCell(new GridCellModel() { Content = new TextContentModel() { Value = "pdf" } });

            return row1;
        }

        [Test]
        [Order(2)]
        public async Task GenerateTestBasePageNumber_Header()
        {
            var rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape
            };

            rm.AddSection(GetSectionTitoloProva());
            rm.AddSection(GetSectionTitoloProva2());

            GridSectionModel tsm = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 70
                }
            };
            tsm.AddRow(GetGridHeader());

            tsm.AddRow(GetGridRow());

            rm.AddSection(tsm);

            rm.AddSection(new EmptySectionModel());

            rm.AddSection(new TextSectionModel()
            {
                Content = new TextContentModel() { Value = "Testo di prova" },
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Center
                }
            });

            var pageNumber = new PageNumberSectionModel()
            {
                TextStyle = new TextStyleModel
                {
                    FontName = "Arial",
                    FontSize = 14,
                    FontIsBold = true,
                    FontColor = System.Drawing.Color.Red
                },

                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Right
                },
                Format = $"Pagina {{{CommandMarkersHelper.CurrentPageNumberMarker}}} di {{{CommandMarkersHelper.NumPagesMarker}}}",
            };            
            rm.AddHeaderSection(pageNumber);
            rm.AddSection(pageNumber);

            var reportGenerator = (OpenXmlReportGeneratorService)_serviceProvider.GetRequiredService<IReportGeneratorService>();

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
            }

            Assert.Pass();
        }

        [Test]
        [Order(3)]
        public async Task GenerateTestBasePageNumber_Footer()
        {
            var rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsDocx
            };

            rm.AddSection(GetSectionTitoloProva());
            rm.AddSection(GetSectionTitoloProva2());

            GridSectionModel tsm = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 70
                }
            };
            tsm.AddRow(GetGridHeader());

            tsm.AddRow(GetGridRow());

            rm.AddSection(tsm);

            rm.AddSection(new EmptySectionModel());

            rm.AddSection(new TextSectionModel()
            {
                Content = new TextContentModel() { Value = "Testo di prova" },
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Center
                }
            });

            var pageNumber = new PageNumberSectionModel()
            {
                TextStyle = new TextStyleModel
                {
                    FontName = "Arial",
                    FontSize = 14,
                    FontIsBold = true,
                    FontColor = System.Drawing.Color.Red
                },

                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Right
                },
                Format = $"Pagina {{{CommandMarkersHelper.CurrentPageNumberMarker}}} di {{{CommandMarkersHelper.NumPagesMarker}}}",
            };
            rm.AddFooterSection(pageNumber);
            rm.AddSection(pageNumber);

            var reportGenerator = (OpenXmlReportGeneratorService)_serviceProvider.GetRequiredService<IReportGeneratorService>();

            var capabilities = await reportGenerator.GetCapabilities();

            if (!capabilities.SupportedOutputTypes.Contains(ReportOutputTypes.AsDocx))
            {
                Assert.Fail("ReportOutputTypes non supportato");
            }

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
            }

            Assert.Pass();
        }

        [Test]
        [Order(4)]
        public async Task GenerateTestPageBreak()
        {
            var rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsHtml
            };

            rm.AddSection(GetSectionTitoloProva());
            rm.AddSection(GetSectionTitoloProva2());

            GridSectionModel tsm = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 70
                }
            };
            tsm.AddRow(GetGridHeader());

            tsm.AddRow(GetGridRow());

            rm.AddSection(tsm);

            rm.AddSection(new BreakPageSectionModel());

            rm.AddSection(new EmptySectionModel());

            rm.AddSection(new TextSectionModel()
            {
                Content = new TextContentModel() { Value = "Testo di prova" },
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Center
                }
            });

            var reportGenerator = (OpenXmlReportGeneratorService)_serviceProvider.GetRequiredService<IReportGeneratorService>();

            var capabilities = await reportGenerator.GetCapabilities();

            if (!capabilities.SupportedOutputTypes.Contains(ReportOutputTypes.AsDocx))
            {
                Assert.Fail("ReportOutputTypes non supportato");
            }

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
            }

            Assert.Pass();
        }

        [Test]
        [Order(5)]
        public async Task GenerateTestHorizonalLine()
        {
            var rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsDocx
            };

            rm.AddSection(GetSectionTitoloProva());
            rm.AddSection(GetSectionTitoloProva2());

            GridSectionModel tsm = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 70
                }
            };
            tsm.AddRow(GetGridHeader());

            tsm.AddRow(GetGridRow());

            rm.AddSection(tsm);

            rm.AddSection(new LineSectionModel() {
                Color = System.Drawing.Color.Red,
                Size = 6,
                Style = LineStyles.Dotted
            });

            rm.AddSection(new EmptySectionModel());

            rm.AddSection(new TextSectionModel()
            {
                Content = new TextContentModel() { Value = "Testo di prova" },
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Center
                }
            });

            var reportGenerator = (OpenXmlReportGeneratorService)_serviceProvider.GetRequiredService<IReportGeneratorService>();

            var capabilities = await reportGenerator.GetCapabilities();

            if (!capabilities.SupportedOutputTypes.Contains(ReportOutputTypes.AsDocx))
            {
                Assert.Fail("ReportOutputTypes non supportato");
            }

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
            }

            Assert.Pass();
        }

        [Test]
        [Order(6)]
        public async Task GenerateTestImage()
        {
            var rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsDocx
            };

            rm.AddSection(GetSectionTitoloProva());
            rm.AddSection(GetSectionTitoloProva2());

            GridSectionModel tsm = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 70
                }
            };
            tsm.AddRow(GetGridHeader());

            tsm.AddRow(GetGridRow());

            rm.AddSection(tsm);

            var imageSection = new ImageSectionModel()
            {
                ImageContent = Files.hype_cycle,
                ImageContentType = "image/png",
                ImageName = "Hype_Cycle.png",
            };

            rm.AddSection(imageSection);
            rm.AddHeaderSection(imageSection);
            rm.AddFooterSection(imageSection);

            rm.AddSection(new EmptySectionModel());

            rm.AddSection(new TextSectionModel()
            {
                Content = new TextContentModel() { Value = "Testo di prova" },
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Center
                }
            });


            var reportGenerator = (OpenXmlReportGeneratorService)_serviceProvider.GetRequiredService<IReportGeneratorService>();

            var capabilities = await reportGenerator.GetCapabilities();

            if (!capabilities.SupportedOutputTypes.Contains(ReportOutputTypes.AsDocx))
            {
                Assert.Fail("ReportOutputTypes non supportato");
            }

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
            }

            Assert.Pass();
        }

        [Test]
        [Order(7)]
        public async Task GenerateTestImage_Header()
        {
            var rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsDocx
            };

            rm.AddSection(GetSectionTitoloProva());
            rm.AddSection(GetSectionTitoloProva2());

            GridSectionModel tsm = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 70
                }
            };
            tsm.AddRow(GetGridHeader());

            tsm.AddRow(GetGridRow());

            rm.AddSection(tsm);

            rm.AddHeaderSection(new ImageSectionModel()
            {
                ImageContent = Files.innovation,
                ImageContentType = "image/png",
                ImageName = "innovation.png",
            });

            rm.AddSection(new EmptySectionModel());

            rm.AddSection(new TextSectionModel()
            {
                Content = new TextContentModel() { Value = "Testo di prova" },
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Center
                }
            });

            var reportGenerator = (OpenXmlReportGeneratorService)_serviceProvider.GetRequiredService<IReportGeneratorService>();

            var capabilities = await reportGenerator.GetCapabilities();

            if (!capabilities.SupportedOutputTypes.Contains(ReportOutputTypes.AsDocx))
            {
                Assert.Fail("ReportOutputTypes non supportato");
            }

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
            }

            Assert.Pass();
        }
    }
}
