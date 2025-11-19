// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.File.ReportGenerator;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class ReportGeneratorServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Order(1)]
        public async Task ReportGeneratorTest()
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
            row2.AddCell(new GridCellModel() { Style = new GridCellStyleModel() { Justification = Justifications.Center, VerticalAlignment = VerticalAlignments.Center }, Content = new TextContentModel() { Value = "Test per modifica versione", Style = new TextStyleModel() { FontName = "Colibri", FontIsBold = true, FontColor = Color.Green, HighlightColor = Color.Yellow } } });
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

            Assert.Pass();
        }
    }
}
