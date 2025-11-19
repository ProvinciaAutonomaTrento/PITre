// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.File.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Pi3.Core.Extensions;
using System.Diagnostics;
using System.Runtime;
using System.Buffers;

namespace Pi3.Infrastructure.DocumentFormat.OpenXml.Services.Spreadsheet
{
    public class OpenXmlShreadsheetService : ISpreadsheetService
    {
        #region Public Members

        public OpenXmlShreadsheetService(ILogger<OpenXmlShreadsheetService> logger)
        {
            this._logger = logger;
        }

        // https://learn.microsoft.com/en-us/office/open-xml/spreadsheet/how-to-insert-a-new-worksheet-into-a-spreadsheet?tabs=cs
        public async Task<GeneratedSpreadsheet> Write(SpreadsheetModel spreadsheetModel, Stream outputStream)
        {
            spreadsheetModel = spreadsheetModel ?? throw new ArgumentNullException(nameof(spreadsheetModel));
            outputStream = outputStream ?? throw new ArgumentNullException(nameof(outputStream));

            Validator.ValidateObject(spreadsheetModel, new ValidationContext(spreadsheetModel), true);

            using SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(outputStream, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            // Add Sheets to the Workbook.
            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

            UInt32Value i = 0;

            var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
            stylesPart.Stylesheet = new Stylesheet();
            Dictionary<string, int> cellRefToStyleIndex = new();
            Dictionary<int, int> colMaxWidth = new();
            UInt32 fontCounter = 1;
            //It seems that openxml apis have some quirks that require some default values i.e. fills with certain fills already inside and so on
            //otherwise styles don't work
            UInt32 fillCounter = 2;
            UInt32 borderCounter = 1;

            Fonts fonts = new Fonts(new Font());
            const int defaultWidth = 10;
            CellFormats cellFormats = new();
            Fills fills = this.GetDefaultFills();
            Borders borders = new Borders();
            Border borderDef = this.GetDefaultBorder();

            borders.AppendChild(borderDef);

            int cellFormatIndex = 1;

            CellFormat defFormat = new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 };
            cellFormats.Append(defFormat.AsArray());

            UInt32Value sCount = 0;

            foreach (var sheetModel in spreadsheetModel.Sheets)
            {
                sCount++;
                var maxRows = sheetModel.Cells.Max(c => c.Row) + 1;

                for (int irow = 0; irow < maxRows; irow++)
                {

                    if (sheetModel.Cells.Any(c => c.Row == irow))
                    {
                        foreach (var cellModel in sheetModel.Cells.Where(c => c.Row == irow))
                        {
                            ForegroundColor foregroundColor = new();
                            Color fontColor = new();
                            Font font = new();
                            CellFormat cFormat = new();
                            Fill fill = new();
                            Alignment allignment = new();
                            Border borderCell = new();



                            if (cellModel != null && cellModel.CellStyle! != null!)
                            {
                                cFormat.ApplyFont = cellModel.CellStyle.FontIsItalic == true
                                || cellModel.CellStyle.FontIsBold == true
                                || cellModel.CellStyle.FontIsStrikeout == true
                                || cellModel.CellStyle.FontColor != null;


                                cFormat.ApplyAlignment = cellModel.CellStyle.VerticalAlignment != null || cellModel.CellStyle.HorizontalAlignment != null;

                                cFormat.ApplyFill = cellModel.CellStyle.ForegroundColor != null;

                                cFormat.ApplyBorder = cellModel.CellStyle.HasBorder == true;

                                if (cellModel.CellStyle.HasBorder == true)
                                {
                                    if (cellModel.CellStyle.BorderColor != null)
                                    {
                                        borderCell = this.GetBorder((System.Drawing.Color)cellModel.CellStyle.BorderColor);
                                    }
                                    else
                                    {
                                        borderCell = this.GetBorder(System.Drawing.Color.Black);
                                    }

                                }


                                if (colMaxWidth.ContainsKey(cellModel.Column))
                                {
                                    if (cellModel.CellStyle.Width != null)
                                    {
                                        colMaxWidth[cellModel.Column] = Math.Max(colMaxWidth[cellModel.Column], (int)cellModel.CellStyle.Width);
                                    }
                                }
                                else
                                {
                                    colMaxWidth[cellModel.Column] = cellModel.CellStyle.Width ?? defaultWidth;
                                }

                                if (cellModel.CellStyle.ForegroundColor != null)
                                {
                                    foregroundColor.Rgb = new HexBinaryValue()
                                    {
                                        Value = System.Drawing.Color.FromArgb(((System.Drawing.Color)cellModel.CellStyle.ForegroundColor).ToArgb()).Name.ToUpper()
                                    };
                                    var patternFill = new PatternFill() { PatternType = PatternValues.Solid };
                                    patternFill.Append(foregroundColor.AsArray());
                                    fill.Append(patternFill.AsArray());
                                }

                                if (!string.IsNullOrEmpty(cellModel.CellStyle.FontName))
                                {
                                    font.Append(new FontName()
                                    {
                                        Val = cellModel.CellStyle.FontName
                                    }.AsArray());
                                }

                                if (cellModel.CellStyle.FontColor != null)
                                {
                                    fontColor.Rgb =
                                        new HexBinaryValue()
                                        {
                                            Value = System.Drawing.Color.FromArgb(((System.Drawing.Color)cellModel.CellStyle.FontColor).ToArgb()).Name
                                        };

                                    font.Append(fontColor.AsArray());
                                }
                                if (cellModel.CellStyle.FontIsItalic == true)
                                {
                                    font.Append(new Italic().AsArray());
                                }
                                if (cellModel.CellStyle.FontIsBold == true)
                                {
                                    font.Append(new Bold().AsArray());

                                }

                                if (cellModel.CellStyle.FontSize != null && cellModel.CellStyle.FontSize > 0)
                                {
                                    font.Append(new FontSize() { Val = cellModel.CellStyle.FontSize }.AsArray());

                                }

                                if (cellModel.CellStyle.VerticalAlignment != null)
                                {
                                    switch (cellModel.CellStyle.VerticalAlignment)
                                    {
                                        case CellTextAlignments.Center:
                                            allignment.Vertical = VerticalAlignmentValues.Center;
                                            break;
                                        case CellTextAlignments.Top:
                                            allignment.Vertical = VerticalAlignmentValues.Top;
                                            break;
                                        default:
                                            allignment.Vertical = VerticalAlignmentValues.Bottom;
                                            break;
                                    }
                                }
                                if (cellModel.CellStyle.WrapText)
                                {
                                    allignment.WrapText = true;
                                }


                                if (cellModel.CellStyle.HorizontalAlignment != null)
                                {
                                    switch (cellModel.CellStyle.HorizontalAlignment)
                                    {
                                        case CellTextAlignments.Left:
                                            allignment.Horizontal = HorizontalAlignmentValues.Left;
                                            break;
                                        case CellTextAlignments.Right:
                                            allignment.Horizontal = HorizontalAlignmentValues.Right;
                                            break;
                                        case CellTextAlignments.Fill:
                                            allignment.Horizontal = HorizontalAlignmentValues.Fill;
                                            break;
                                        default:
                                            allignment.Horizontal = HorizontalAlignmentValues.Center;
                                            break;
                                    }

                                }



                                if (cFormat.ApplyBorder != null && cFormat.ApplyBorder.Value)
                                {
                                    borders.Append(borderCell.AsArray());
                                    cFormat.BorderId = borderCounter++;
                                }

                                if (cFormat.ApplyFont != null && cFormat.ApplyFont.Value)
                                {
                                    fonts.Append(font.AsArray());
                                    cFormat.FontId = fontCounter++;
                                }

                                if (cFormat.ApplyAlignment != null && cFormat.ApplyAlignment.Value)
                                {

                                    cFormat.Append(allignment.AsArray());
                                }

                                if (cFormat.ApplyFill != null && cFormat.ApplyFill.Value)
                                {
                                    cFormat.FillId = fillCounter++;
                                    fills.Append(fill.AsArray());
                                }

                                cellFormats.Append(cFormat.AsArray());
                                string myLetter = this.GetColumnName(cellModel.Column);
                                cellRefToStyleIndex[sCount.ToString() + $"{myLetter}{cellModel.Row + 1}"] = cellFormatIndex++;
                            }
                        }
                    }
                }

            }



            stylesPart.Stylesheet.Append(fonts.AsArray());
            stylesPart.Stylesheet.Append(fills.AsArray());
            stylesPart.Stylesheet.Append(borders.AsArray());
            stylesPart.Stylesheet.Append(cellFormats.AsArray());


            stylesPart.Stylesheet.Save();


            foreach (var sheetModel in spreadsheetModel.Sheets)
            {
                i++;

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet();

                var maxRows = sheetModel.Cells.Max(c => c.Row) + 1;
                var maxCols = sheetModel.Cells.Max(c => c.Column) + 1;

                Columns cols = new Columns();

                for (int icol = 0; icol < maxCols; icol++)
                {
                    var hasWidth = colMaxWidth.ContainsKey(icol);
                    Column col = new Column()
                    {
                        Min = new((uint)icol + 1),
                        Max = new((uint)icol + 1),
                        CustomWidth = true,
                        Width = hasWidth ? colMaxWidth[icol] : defaultWidth
                    };
                    cols.Append(col.AsArray());

                }

                worksheetPart.Worksheet.Append(cols.AsArray());


                SheetData sheetData = new();

                for (int irow = 0; irow < maxRows; irow++)
                {
                    Row row = new Row();

                    if (sheetModel.Cells.Any(c => c.Row == irow))
                    {
                        foreach (var cellModel in sheetModel.Cells.Where(c => c.Row == irow))
                        {
                            string myLetter = this.GetColumnName(cellModel.Column);

                            if (cellModel.CellStyle! == null!)
                            {
                                Cell cell = new Cell()
                                {
                                    CellReference = $"{myLetter}{cellModel.Row + 1}",
                                    DataType = CellValues.String,
                                    CellValue = new CellValue(cellModel.ValueAsString ?? String.Empty),
                                };
                                row.Append(cell.AsArray());
                            }
                            else
                            {
                                Cell cell = new Cell()
                                {
                                    CellReference = $"{myLetter}{cellModel.Row + 1}",
                                    DataType = CellValues.String,
                                    CellValue = new CellValue(cellModel.ValueAsString ?? String.Empty),
                                    StyleIndex = (UInt32)cellRefToStyleIndex[i.ToString() + $"{myLetter}{cellModel.Row + 1}"]
                                };
                                row.Append(cell.AsArray());
                            }

                        }
                    }

                    sheetData.Append(row.AsArray());
                }

                // Append a new worksheet and associate it with the workbook.
                sheets.Append(
                    new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = i,
                        Name = sheetModel.Name
                    }.AsArray());

                worksheetPart.Worksheet.AppendChild(sheetData);
            }

            workbookPart.Workbook.Save();

            return new GeneratedSpreadsheet()
            {
                FileName = $"Spreadsheet_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

        public async Task<GeneratedSpreadsheet> WriteLarge(SpreadsheetModel spreadsheetModel, Stream outputStream)
        {
            throw new NotImplementedException();
        }

        public async Task<SpreadsheetModel> Read(Stream inputFileStream)
        {
            inputFileStream = inputFileStream ?? throw new ArgumentNullException(nameof(inputFileStream));

            var model = new SpreadsheetModel();

            using SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(inputFileStream, false);

            var workbookPart = spreadsheetDocument.WorkbookPart;
            var workbook = workbookPart!.Workbook;

            var sheets = workbook.Descendants<Sheet>();
            foreach (var sheet in sheets)
            {
                var sheetModel = new SheetModel()
                {
                    Name = sheet.Name ?? sheet.LocalName
                };

                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id!);
                var sharedStringPart = workbookPart.SharedStringTablePart;
                var sst = sharedStringPart?.SharedStringTable;

                foreach (var row in worksheetPart.Worksheet.Descendants<Row>())
                {
                    int count = row.Elements<Cell>().Count();

                    foreach (Cell c in row.Elements<Cell>())
                    {
                        string? valueAsString = null;

                        if ((c.DataType != null) && (c.DataType == CellValues.SharedString))
                        {
                            int ssid = int.Parse(c.CellValue!.Text);

                            valueAsString = sst?.ChildElements[ssid].InnerText;
                        }
                        else if (c.CellValue != null)
                        {
                            if (c.StyleIndex != null)
                            {
                                var cellFormat = workbookPart.WorkbookStylesPart!.Stylesheet.CellFormats!.ChildElements[int.Parse(c.StyleIndex.InnerText)] as CellFormat;
                                if (cellFormat != null && cellFormat.NumberFormatId != null)
                                {
                                    valueAsString = GetDateTimeFormat(cellFormat.NumberFormatId, c.CellValue.Text);
                                }
                            }

                            if(string.IsNullOrEmpty(valueAsString))
                                valueAsString = c.CellValue.Text;
                        }

                        sheetModel.AddCell(new CellModel()
                        {
                            Row = (row.RowIndex!.HasValue ? (int)row.RowIndex?.Value! - 1 : 0),
                            Column = (c.CellReference != null ? this.GetColumnIndex(c.CellReference!)!.Value - 1 : 0),
                            Name = c.CellReference,
                            DataType = c.DataType?.InnerText,
                            ValueAsString = valueAsString
                        });
                    }
                }

                model.AddSheet(sheetModel);
            }

            return model;
        }

        #endregion

        #region Private Members

        protected ILogger<OpenXmlShreadsheetService> _logger;
        
        private string GetColumnName(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var value = "";
            if (index >= letters.Length)
                value += letters[index / letters.Length - 1];
            value += letters[index % letters.Length];
            return value;
        }

        private int? GetColumnIndex(string cellReference)
        {
            if (string.IsNullOrEmpty(cellReference))
            {
                return null;
            }

            //remove digits
            string columnReference = Regex.Replace(cellReference.ToUpper(), @"[\d]", string.Empty, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));

            int columnNumber = -1;
            int mulitplier = 1;

            //working from the end of the letters take the ASCII code less 64 (so A = 1, B =2...etc)
            //then multiply that number by our multiplier (which starts at 1)
            //multiply our multiplier by 26 as there are 26 letters
            foreach (char c in columnReference.ToCharArray().Reverse())
            {
                columnNumber += mulitplier * ((int)c - 64);

                mulitplier = mulitplier * 26;
            }

            //the result is zero based so return columnnumber + 1 for a 1 based answer
            //this will match Excel's COLUMN function
            return columnNumber + 1;
        }


        private Border GetBorder(System.Drawing.Color color)
        {
            return new(
                    new LeftBorder(
                    new Color()
                    {
                        Rgb = System.Drawing.Color.FromArgb(((System.Drawing.Color)color).ToArgb()).Name.ToUpper()
                    })
                    { Style = BorderStyleValues.Thick },
                    new RightBorder(new Color()
                    {
                        Rgb = System.Drawing.Color.FromArgb(((System.Drawing.Color)color).ToArgb()).Name.ToUpper()
                    })
                    { Style = BorderStyleValues.Thick },
                    new TopBorder(new Color()
                    {
                        Rgb = System.Drawing.Color.FromArgb(((System.Drawing.Color)color).ToArgb()).Name.ToUpper()
                    })
                    { Style = BorderStyleValues.Thick },
                    new BottomBorder(new Color()
                    {
                        Rgb = System.Drawing.Color.FromArgb(((System.Drawing.Color)color).ToArgb()).Name.ToUpper()
                    })
                    { Style = BorderStyleValues.Thick });
        }

        private Border GetDefaultBorder()
        {
            return new(
                new LeftBorder(),
                new RightBorder(),
                new TopBorder(),
                new BottomBorder(),
                new DiagonalBorder());
        }

        private Fills GetDefaultFills()
        {
            return new Fills(
                new Fill(new PatternFill() { PatternType = PatternValues.None }),
                new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }));
        }
        private string GetDateTimeFormat(UInt32Value numberFormatId, string text)
        {
            var valueAsString = string.Empty;

            if (DateFormatDictionary.ContainsKey(numberFormatId))
            {
                valueAsString = DateTime.FromOADate(Convert.ToDouble(text)).AsDateFormat();
            }
            else if (DateTimeFormatDictionary.ContainsKey(numberFormatId))
            {
                valueAsString = DateTime.FromOADate(Convert.ToDouble(text)).AsDateTimeFormat();
            }
            else if (TimeFormatDictionary.ContainsKey(numberFormatId))
            {
                valueAsString = DateTime.FromOADate(Convert.ToDouble(text)).ToString("HH:mm:ss");
            }

            return valueAsString;
        }

        private readonly Dictionary<uint, string> DateFormatDictionary = new Dictionary<uint, string>()
        {
            [14] = "dd/MM/yyyy",
            [15] = "d-MMM-yy",
            [16] = "d-MMM",
            [17] = "MMM-yy",          
            [30] = "M/d/yy",
            [34] = "yyyy-MM-dd",         
            [51] = "MM-dd",
            [52] = "yyyy-MM-dd",
            [53] = "yyyy-MM-dd",
            [55] = "yyyy-MM-dd",
            [56] = "yyyy-MM-dd",
            [58] = "MM-dd",
            [165] = "M/d/yy",
            [166] = "dd MMMM yyyy",
            [167] = "dd/MM/yyyy",
            [168] = "dd/MM/yy",
            [169] = "d.M.yy",
            [170] = "yyyy-MM-dd",
            [171] = "dd MMMM yyyy",
            [172] = "d MMMM yyyy",
            [173] = "M/d",
            [174] = "M/d/yy",
            [175] = "MM/dd/yy",
            [176] = "d-MMM",
            [177] = "d-MMM-yy",
            [178] = "dd-MMM-yy",
            [179] = "MMM-yy",
            [180] = "MMMM-yy",
            [181] = "MMMM d, yyyy",
            [184] = "MMM",
            [185] = "MMM-dd",
            [186] = "M/d/yyyy",
            [187] = "d-MMM-yyyy"
        };

        private readonly Dictionary<uint, string> DateTimeFormatDictionary = new Dictionary<uint, string>()
        {           
            [22] = "M/d/yy h:mm",       
            [182] = "M/d/yy hh:mm t",
            [183] = "M/d/y HH:mm",
        };

        private readonly Dictionary<uint, string> TimeFormatDictionary = new Dictionary<uint, string>()
        {
          
            [18] = "h:mm AM/PM",
            [19] = "h:mm:ss AM/PM",
            [20] = "h:mm",
            [21] = "h:mm:ss",          
            [45] = "mm:ss",
            [46] = "[h]:mm:ss",
            [47] = "mmss.0",
        };
        #endregion
    }
}
