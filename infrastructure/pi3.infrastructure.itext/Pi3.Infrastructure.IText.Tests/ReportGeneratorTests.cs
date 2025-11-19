// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Infrastructure.IText.ReportGenerator;
using Pi3.Infrastructure.IText.ReportGenerator.Services;

namespace Pi3.Infrastructure.IText.Tests
{
    public class ReportGeneratorTests
    {
        ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddPi3Core()
                .AddInfrastructureITextReportGenerator()
                .BuildServiceProvider();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
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
                    Value = "Titolo di itextsharp",
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
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 30, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Oggetto", Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Tipo", Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Mitt. / Dest.", Style = new TextStyleModel() { FontIsBold = true } } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Codice fascicolo" } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "Annullato" } });
            row1.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { WithPercentage = 10, ForegroundColor = System.Drawing.Color.Gray }, Content = new TextContentModel() { Value = "File", Style = new TextStyleModel() { FontIsBold = true } } });
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

            var reportGenerator = _serviceProvider.GetRequiredService<IReportGeneratorService>();

            // Scrivi il contenuto del MemoryStream su un file
            using (var stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
                var content = stream.ToArray();
                //File.WriteAllBytes("C:\\temp\\itext\\reporto.pdf", content);

            }

            Assert.Pass();
        }

        [Test]
        [Order(2)]
        public async Task GenerateTestBasePageNumber_Header()
        {
            var rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsPdf
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
                Format = $"Pagina {{{CommandMarkersHelper.GetCurrentPageNumberMarker()}}} di {{{CommandMarkersHelper.GetNumPagesMarker()}}}",
            };
            rm.AddHeaderSection(pageNumber);

            var reportGenerator = _serviceProvider.GetRequiredService<IReportGeneratorService>();

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
                var content = stream.ToArray();
                //File.WriteAllBytes("C:\\temp\\itext\\test_header.pdf", content);
                //File.WriteAllBytes("/mnt/approot/Temp/test_header.pdf", content);
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
                OutputType = ReportOutputTypes.AsPdf
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

            for(var i = 0; i < 20; i++)
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
                    FontColor = System.Drawing.Color.Black
                },

                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Right
                },
                Format = $"Pagina {{{CommandMarkersHelper.GetCurrentPageNumberMarker()}}} di {{{CommandMarkersHelper.GetNumPagesMarker()}}}",
            };
            rm.AddFooterSection(pageNumber);

            rm.AddFooterSection(new TextSectionModel()
            {
                Content = new TextContentModel() { Value = "Testo di prova" },
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Left
                }
            });

            rm.AddFooterSection(new TextSectionModel()
            {
                Content = new TextContentModel() { Value = "Stampa effettuata il : " + DateTime.Today.ToShortDateString() },
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Left
                }
            });


             var reportGenerator = _serviceProvider.GetRequiredService<IReportGeneratorService>();

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);

                // Crea un FileStream per scrivere direttamente il file
                //using (FileStream fileStream = new FileStream("C:\\temp\\itext\\test_pagenumber.pdf", FileMode.Create, FileAccess.Write))
                //{
                //    // Copia direttamente da MemoryStream a FileStream senza conversione in byte[]
                //    await stream.CopyToAsync(fileStream);
                //}
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
                OutputType = ReportOutputTypes.AsPdf
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


            var reportGenerator = _serviceProvider.GetRequiredService<IReportGeneratorService>();

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
                var content = stream.ToArray();
                //File.WriteAllBytes("C:\\temp\\itext\\test_break.pdf", content);
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
                OutputType = ReportOutputTypes.AsPdf
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

            rm.AddSection(new LineSectionModel()
            {
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


            var reportGenerator = _serviceProvider.GetRequiredService<IReportGeneratorService>();

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
                var content = stream.ToArray();
                //File.WriteAllBytes("C:\\temp\\itext\\test_line.pdf", content);

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
                OutputType = ReportOutputTypes.AsPdf
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


            var reportGenerator = _serviceProvider.GetRequiredService<IReportGeneratorService>();

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
                var content = stream.ToArray();
                //File.WriteAllBytes("C:\\temp\\itext\\test_image_header.pdf", content);
            }

            Assert.Pass();
        }

        [Test]
        [Order(6)]
        public async Task GenerateTestBasePageNumber_Header_Docx()
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
                Content = new TextContentModel() { Value = "Testo di prova",
                },
                Style = new TextSectionStyleModel()
                {
                    Justification = Justifications.Center,
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
                    Justification = Justifications.Right,
                },
                Format = $"Pagina {{{CommandMarkersHelper.GetCurrentPageNumberMarker()}}} di {{{CommandMarkersHelper.GetNumPagesMarker()}}}",
            };
            rm.AddHeaderSection(pageNumber);

            var reportGenerator = _serviceProvider.GetRequiredService<IReportGeneratorService>();

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
                var content = stream.ToArray();
                //File.WriteAllBytes("C:\\temp\\itext\\test_header.pdf", content);
            }

            Assert.Pass();
        }


        [Test]
        [Order(7)]
        public async Task GenerateTestBasePageNumber_Header_Portrait()
        {
            var rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Portrait,
                OutputType = ReportOutputTypes.AsPdf
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
                Format = $"Pagina {{{CommandMarkersHelper.GetCurrentPageNumberMarker()}}} di {{{CommandMarkersHelper.GetNumPagesMarker()}}}",
            };
            rm.AddHeaderSection(pageNumber);

            var reportGenerator = _serviceProvider.GetRequiredService<IReportGeneratorService>();

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
                var content = stream.ToArray();
                //File.WriteAllBytes("C:\\temp\\itext\\test_header_portrait.pdf", content);
            }

            Assert.Pass();
        }


        [Test]
        [Order(8)]
        public async Task GenerateTestImage()
        {
            var rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsPdf
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

            rm.AddSection(new ImageSectionModel()
            {
                ImageContent = Files.hype_cycle,
                ImageContentType = "image/png",
                ImageName = "Hype_Cycle.png",
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


            var reportGenerator = _serviceProvider.GetRequiredService<IReportGeneratorService>();

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(rm, stream);
                var content = stream.ToArray();
                //File.WriteAllBytes("C:\\temp\\itext\\test_image.pdf", content);
            }

            Assert.Pass();
        }


        [Test]
        [Order(9)]
        public async Task GenerateTestImage_Docx()
        {
            var report = new ReportModel
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsPdf
            };

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
                Format = $"Pagina {{{CommandMarkersHelper.GetCurrentPageNumberMarker()}}} di {{{CommandMarkersHelper.GetNumPagesMarker()}}}",
            };
            report.AddHeaderSection(pageNumber);
            report.AddHeaderSection(this.TextBodySection("Provincia Autonoma di Trento"));
            report.AddHeaderSection(this.GetTitleSection("Registro: PAT - Provincia Autonoma di Trento"));
            report.AddHeaderSection(this.GetSubTitleSection("Stampa del Registro Ufficiale di Protocollo dal numero 13337 al numero 13524"));

            report.AddSection(this.GetTitleSection("Prova testo nel corpo"));
            // Intestazione
            var gridNewItems = new GridSectionModel
            {
                Style = new GridSectionStyleModel { WithPercentage = 100 }
            };

            gridNewItems.AddRow(this.GetHeaderRow());

            var entities = new List<ReportRegistriItem>();
            entities.Add(new ReportRegistriItem
            {
                RecordNumber = "13337",
                EmergencyRecordNumber = "13337",
                RecordDate = DateTime.Now,
                CancellationDate = DateTime.Now,
                RecordType = "NP",
                Subject = "Test per modifica versione",
                SenderRecipient = "Mittente",
                Folders = "6.4-2024-50",
                Hash = "hashpippoplutopaeroBimBumBamapplication/vnd.openxmlformats.officedocument.wordprocessingml.documentapplication/vnd.openxmlformats.officedocument.wordprocessingml.document",
                AttachmentsNumber = 1
            });
            entities.Add(new ReportRegistriItem
            {
                RecordNumber = "13347",
                EmergencyRecordNumber = "13347",
                RecordDate = DateTime.Now,
                CancellationDate = DateTime.Now,
                RecordType = "NP",
                Subject = "Test 2 per modifica versione",
                SenderRecipient = "Altro Mittente",
                Folders = "6.4-2024-50",
                Hash = "hash",
                AttachmentsNumber = 1
            });
            entities.Add(new ReportRegistriItem
            {
                RecordNumber = "13337",
                EmergencyRecordNumber = "13337",
                RecordDate = DateTime.Now,
                CancellationDate = DateTime.Now,
                RecordType = "NP",
                Subject = "Test per modifica versione",
                SenderRecipient = "Mittente",
                Folders = "6.4-2024-50",
                Hash = "hashpippoplutopaeroBimBumBamapplication/vnd.openxmlformats.officedocument.wordprocessingml.documentapplication/vnd.openxmlformats.officedocument.wordprocessingml.document",
                AttachmentsNumber = 1
            });
            entities.Add(new ReportRegistriItem
            {
                RecordNumber = "13337",
                EmergencyRecordNumber = "13337",
                RecordDate = DateTime.Now,
                CancellationDate = DateTime.Now,
                RecordType = "NP",
                Subject = "Test per modifica versione",
                SenderRecipient = "Mittente",
                Folders = "6.4-2024-50",
                Hash = "hashpippoplutopaeroBimBumBamapplication/vnd.openxmlformats.officedocument.wordprocessingml.documentapplication/vnd.openxmlformats.officedocument.wordprocessingml.document",
                AttachmentsNumber = 1
            });
            entities.Add(new ReportRegistriItem
            {
                RecordNumber = "13337",
                EmergencyRecordNumber = "13337",
                RecordDate = DateTime.Now,
                CancellationDate = DateTime.Now,
                RecordType = "NP",
                Subject = "Test per modifica versione",
                SenderRecipient = "Mittente",
                Folders = "6.4-2024-50",
                Hash = "hashpippoplutopaeroBimBumBamapplication/vnd.openxmlformats.officedocument.wordprocessingml.documentapplication/vnd.openxmlformats.officedocument.wordprocessingml.document",
                AttachmentsNumber = 1
            });
            entities.Add(new ReportRegistriItem
            {
                RecordNumber = "13337",
                EmergencyRecordNumber = "13337",
                RecordDate = DateTime.Now,
                CancellationDate = DateTime.Now,
                RecordType = "NP",
                Subject = "Test per modifica versione",
                SenderRecipient = "Mittente",
                Folders = "6.4-2024-50",
                Hash = "hashpippoplutopaeroBimBumBamapplication/vnd.openxmlformats.officedocument.wordprocessingml.documentapplication/vnd.openxmlformats.officedocument.wordprocessingml.document",
                AttachmentsNumber = 1
            });

            entities.ForEach(x => gridNewItems.AddRow(this.GetReportRow(x)));

            var reportGenerator = _serviceProvider.GetRequiredService<IReportGeneratorService>();

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(report, stream);
                var content = stream.ToArray();
                //File.WriteAllBytes("C:\\temp\\itext\\test_registro.pdf", content);
            }

            Assert.Pass();

        }


        [Test]
        [Order(10)]
        public async Task GeneratePercentuale()
        {
            var title = "Provincia Autonoma di Trento";

            var report = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsPdf
            };

            GridSectionModel model = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 100
                }
            };
            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Right
                },
                Content = new TextContentModel
                {
                    Value = string.Format("Stampa Centro Notifiche effettuata il: ", DateTime.Now.AsDateFormat()),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 8,
                        FontIsBold = false
                    }
                }
            });

            report.AddSection(new EmptySectionModel());

            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Left
                },
                Content = new TextContentModel
                {
                    Value = title,
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 20,
                        FontIsBold = true
                    }
                }
            });
            report.AddSection(new TextSectionModel
            {
                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Left
                },
                Content = new TextContentModel
                {
                    Value = string.Format("Righe stampate: ", 1),
                    Style = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 10,
                        FontIsBold = false
                    }
                }
            });

            GridRowModel header = new GridRowModel();
            header.AddCell(HeaderCell(10, "Evento"));
            header.AddCell(HeaderCell(20, "Autore"));
            header.AddCell(HeaderCell(10, "Data Evento"));
            header.AddCell(HeaderCell(10, "Doc/Fasc"));
            header.AddCell(HeaderCell(20, "Oggetto/Descrizione"));
            header.AddCell(HeaderCell(30, "Dettaglio"));

            model.AddRow(header);

            GridRowModel row = new GridRowModel();
            row.AddCell(Cell("a", VerticalAlignments.Center, Justifications.Center));
            row.AddCell(Cell("b", VerticalAlignments.Center, Justifications.Center));
            row.AddCell(Cell("c", VerticalAlignments.Center, Justifications.Center));
            row.AddCell(Cell("d", VerticalAlignments.Center, Justifications.Center));
            row.AddCell(Cell("f", VerticalAlignments.Center, Justifications.Center));
            row.AddCell(Cell("g", VerticalAlignments.Center, Justifications.Center));

            model.AddRow(row);

            report.AddSection(model);

            var reportGenerator = _serviceProvider.GetRequiredService<IReportGeneratorService>();

            using (MemoryStream stream = new MemoryStream())
            {
                await reportGenerator.Generate(report, stream);
                var content = stream.ToArray();
                //File.WriteAllBytes("C:\\temp\\itext\\test_centronotifiche.pdf", content);
            }

            Assert.Pass();
        }

        #region private methods

        private GridRowModel GetGridHeader()
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


        private GridRowModel GetGridRow()
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

        private TextSectionModel GetSectionTitoloProva()
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

        private TextSectionModel GetSectionTitoloProva2()
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

        protected GridRowModel GetHeaderRow()
        {
            var header = new GridRowModel();
            var cellStyle = new TextStyleModel
            {
                FontName = "ARIAL",
                FontSize = 12,
                FontIsBold = true
            };

            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 7), Content = new TextContentModel { Style = cellStyle, Value = "Protocollo" } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 6), Content = new TextContentModel { Style = cellStyle, Value = "Data" } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 8), Content = new TextContentModel { Style = cellStyle, Value = "Annullato il" } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 3), Content = new TextContentModel { Style = cellStyle, Value = "Tipo" } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 28), Content = new TextContentModel { Style = cellStyle, Value = "Oggetto" } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 24), Content = new TextContentModel { Style = cellStyle, Value = "Mitt./Dest." } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 8), Content = new TextContentModel { Style = cellStyle, Value = "Fascioli" } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 13), Content = new TextContentModel { Style = cellStyle, Value = "Impronta" } });
            header.AddCell(new GridCellModel { Style = HeaderCellStyle(percentage: 3), Content = new TextContentModel { Style = cellStyle, Value = "Allegati" } });
            return header;
        }

        private GridRowModel GetReportRow(ReportRegistriItem item)
        {
            var row = new GridRowModel();
            var textStyle = new TextStyleModel
            {
                FontName = "ARIAL",
                FontSize = 7
            };
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = $"{item.RecordNumber} {item.EmergencyRecordNumber ?? string.Empty}" } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.RecordDate.HasValue ? item.RecordDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.CancellationDate.HasValue ? item.CancellationDate.Value.ToString("dd/MM/yyyy") : string.Empty } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.RecordType } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleLeft, Content = new TextContentModel { Style = textStyle, Value = item.Subject } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleLeft, Content = new TextContentModel { Style = textStyle, Value = item.SenderRecipient } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.Folders ?? string.Empty } }); // FASCICOLI
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleLeft, Content = new TextContentModel { Style = textStyle, Value = item.Hash ?? string.Empty } });
            row.AddCell(cell: new GridCellModel { Style = this.CellStyleCentered, Content = new TextContentModel { Style = textStyle, Value = item.AttachmentsNumber.ToString() } });
            return row;
        }


        protected GridCellModel HeaderCell(int withPercentage, string textContentModel)
        {
            var cell = new GridCellModel()
            {
                Style = new GridCellStyleModel()
                {
                    WithPercentage = withPercentage,
                    Justification = Justifications.Center,
                    ForegroundColor = System.Drawing.Color.Gray,
                    VerticalAlignment = VerticalAlignments.Center
                },
                Content = new TextContentModel()
                {
                    Value = textContentModel,
                    Style = new TextStyleModel()
                    {
                        FontIsBold = true,
                        FontName = "Arial"
                    }
                }
            };

            return cell;
        }

        protected GridCellModel Cell(string textContentModel, VerticalAlignments verticalAlignment, Justifications justification = Justifications.Left)
        {
            var cell = new GridCellModel()
            {
                Content = new TextContentModel()
                {
                    Value = textContentModel,
                    Style = new TextStyleModel()
                    {
                        FontName = "Arial"
                    }
                },
                Style = new GridCellStyleModel()
                {
                    VerticalAlignment = verticalAlignment,
                    Justification = justification

                }
            };

            return cell;
        }

        private class ReportRegistriItem
        {
            public string RecordNumber { get; set; }
            public string EmergencyRecordNumber { get; set; }
            public DateTime? RecordDate { get; set; }
            public DateTime? CancellationDate { get; set; }
            public string RecordType { get; set; }
            public string Subject { get; set; }
            public string SenderRecipient { get; set; }
            public string Folders { get; set; }
            public string Hash { get; set; }
            public int AttachmentsNumber { get; set; }
        }

        private TextSectionModel GetTitleSection(string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Style = new TextStyleModel
                    {
                        FontName = "ARIAL",
                        FontSize = 16,
                        FontIsBold = true,
                    },
                    Value = text
                }
            };
        }

        private TextSectionModel TextBodySection(string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Right },
                Content = new TextContentModel
                {
                    Style = new TextStyleModel
                    {
                        FontName = "ARIAL",
                        FontSize = 8,
                        FontIsBold = true,
                        FontColor = System.Drawing.Color.Red
                    },
                    Value = text
                }
            };
        }


        private TextSectionModel GetSubTitleSection(string text)
        {
            return new TextSectionModel
            {
                Style = new TextSectionStyleModel { Justification = Justifications.Left },
                Content = new TextContentModel
                {
                    Style = new TextStyleModel
                    {
                        FontName = "ARIAL",
                        FontSize = 14,
                        FontIsBold = true,
                        FontColor = System.Drawing.Color.Blue
                    },
                    Value = text
                }
            };
        }

        private GridCellStyleModel CellStyleCentered
            => new GridCellStyleModel
            {
                Justification = Justifications.Center,
                VerticalAlignment = VerticalAlignments.Top
            };

        private GridCellStyleModel CellStyleLeft
            => new GridCellStyleModel
            {
                Justification = Justifications.Left,
                VerticalAlignment = VerticalAlignments.Top
            };

        private GridCellStyleModel HeaderCellStyle(int percentage)
            => new GridCellStyleModel
            {
                Justification = Justifications.Center,
                WithPercentage = percentage,
                ForegroundColor = System.Drawing.Color.Silver
            };
        #endregion
    }
}
