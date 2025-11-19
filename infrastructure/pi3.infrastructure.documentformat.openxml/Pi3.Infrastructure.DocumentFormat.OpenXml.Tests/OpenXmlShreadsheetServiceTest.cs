// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Infrastructure.DocumentFormat.OpenXml;
using Pi3.Infrastructure.DocumentFormat.OpenXml.Tests;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.DocumentFormat.OpenXml.Tests
{
    internal class OpenXmlShreadsheetServiceTest
    {
        ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddInfrastructureOpenXmlSpreadsheetService()
                .BuildServiceProvider();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
        }

        [Test]
        [Order(1)]
        public async Task ReadTest()
        {
            var service = _serviceProvider.GetRequiredService<ISpreadsheetService>();

            var model = await service.Read(new MemoryStream(Files.Monitoraggio_Cluster_PiTre_K8s));

            using (var stream = new MemoryStream())
                await service.Write(model, stream);

            Assert.Pass();
        }

        [Test]
        [Order(2)]
        public async Task WriteTest()
        {
            var service = _serviceProvider.GetRequiredService<ISpreadsheetService>();

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
                    FontColor = System.Drawing.Color.Red,
                    FontIsBold = true,
                    ForegroundColor = System.Drawing.Color.Yellow,
                    FontName = "Arial",
                    FontSize = 20,
                    Width = 50,
                    HasBorder = true
                }
            });
            sheetModel1.AddCell(new CellModel()
            {
                Row = 0,
                Column = 1,
                ValueAsString = "Testo di prova",
                CellStyle = new CellStyleModel()
                {
                    FontColor = System.Drawing.Color.White,
                    FontIsBold = true,
                    ForegroundColor = System.Drawing.Color.Black
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
                ValueAsString = "aaaaa",
                CellStyle = new CellStyleModel()
                {
                    FontColor = System.Drawing.Color.White,
                    FontIsBold = true,
                    HasBorder = true,
                    BorderColor = System.Drawing.Color.GreenYellow,
                    ForegroundColor = System.Drawing.Color.Black,
                    VerticalAlignment = CellTextAlignments.Center,
                    HorizontalAlignment = CellTextAlignments.Center,
                    Width = 36
                }
            });

            sheetModel1.AddCell(new CellModel()
            {
                Row = 10,
                Column = 6,
                ValueAsString = "aaaaa",
                CellStyle = new CellStyleModel()
                {
                    FontColor = System.Drawing.Color.Red,
                    FontIsBold = true,
                    HasBorder = true,
                    BorderColor = System.Drawing.Color.GreenYellow,
                    ForegroundColor = System.Drawing.Color.Black,
                    VerticalAlignment = CellTextAlignments.Center,
                    HorizontalAlignment = CellTextAlignments.Center,
                    Width = 36
                }
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
            sheetModel2.AddCell(new CellModel()
            {
                Row = 10,
                Column = 6,
                ValueAsString = "aaaaaColors",
                CellStyle = new CellStyleModel()
                {
                    FontColor = System.Drawing.Color.Red,
                    FontIsBold = true,
                    HasBorder = true,
                    BorderColor = System.Drawing.Color.GreenYellow,
                    ForegroundColor = System.Drawing.Color.Black,
                    VerticalAlignment = CellTextAlignments.Center,
                    HorizontalAlignment = CellTextAlignments.Center,
                    Width = 36
                }
            });
            model.AddSheet(sheetModel2);

            using (var stream = new MemoryStream()) { 
                await service.Write(model, stream);
            }

            Assert.Pass();
        }
    }
}
