// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Test.ExampleAggregate;
using Pi3.Core.Test.OrderAggregate.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class SpreadsheetServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Order(1)]
        public async Task SpreadsheetTest()
        {
            var model = new SpreadsheetModel();

            var sheetModel1 = new SheetModel();
            sheetModel1.Name = "Sheet 1";
            sheetModel1.AddCell(new CellModel()
            {
                Row = 0,
                Column = 0,
                ValueAsString = "Testo di prova",
                CellStyle = new CellStyleModel()
                {
                    FontColor = Color.Red,
                    FontIsBold = true,
                    ForegroundColor = Color.Yellow,
                    FontName = "Arial",
                    FontSize = 20
                }
            });
            sheetModel1.AddCell(new CellModel()
            {
                Row = 0,
                Column = 0,
                ValueAsString = "Testo di prova",
                CellStyle = new CellStyleModel()
                {
                    FontColor = Color.Red,
                    FontIsBold = true,
                    ForegroundColor = Color.Yellow,
                    FontName = "Arial",
                    FontSize = 20
                }
            });
            sheetModel1.AddCell(new CellModel()
            {
                Row = 0,
                Column = 1,
                ValueAsString = "Testo di prova",
                CellStyle = new CellStyleModel()
                {
                    FontColor = Color.White,
                    FontIsBold = true,
                    ForegroundColor = Color.Black
                }
            });
            sheetModel1.AddCell(new CellModel()
            {
                Row = 1,
                Column = 0,
                ValueAsString = "Testo di prova aaaa"
            });
            sheetModel1.AddCell(new CellModel()
            {
                Row = 5,
                Column = 6,
                ValueAsString = "aaaaa"
            });

            model.AddSheet(sheetModel1);

            var sheetModel2 = new SheetModel();
            sheetModel2.Name = "Sheet 2";
            sheetModel2.AddCell(new CellModel()
            {
                Row = 0,
                Column = 0,
                ValueAsString = "Testo di prova",
                CellStyle = new CellStyleModel()
                {
                    VerticalAlignment = CellTextAlignments.Center,
                    HorizontalAlignment = CellTextAlignments.Center
                }
            });
            sheetModel2.AddCell(new CellModel()
            {
                Row = 0,
                Column = 1,
                ValueAsString = "Testo di prova",
                CellStyle = new CellStyleModel()
                {
                    VerticalAlignment = CellTextAlignments.Bottom,
                    HorizontalAlignment = CellTextAlignments.Bottom
                }
            });
            sheetModel2.AddCell(new CellModel()
            {
                Row = 1,
                Column = 0,
                ValueAsString = "Testo di prova",
                CellStyle = new CellStyleModel()
                {
                    VerticalAlignment = CellTextAlignments.Bottom,
                    HorizontalAlignment = CellTextAlignments.Bottom
                }
            });

            model.AddSheet(sheetModel2);

            Assert.Pass();
        }
    }
}
