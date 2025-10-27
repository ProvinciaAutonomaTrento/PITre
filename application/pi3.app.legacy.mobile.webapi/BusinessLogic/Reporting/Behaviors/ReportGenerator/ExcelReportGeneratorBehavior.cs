// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.Report;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;

namespace BusinessLogic.Reporting.Behaviors.ReportGenerator
{
    class ExcelReportGeneratorBehavior : IReportGeneratorBehavior
    {
        /// <summary>
        /// Enumerazione degli stili ammessi
        /// </summary>
        private enum StylesEnum
        {
            /// <summary>
            /// Stile dell'header
            /// </summary>
            HeaderStyle,
            /// <summary>
            /// Stile del titolo
            /// </summary>
            TitleStyle,
            /// <summary>
            /// Stile del sottotitolo
            /// </summary>
            SubtitleStyle,
            /// <summary>
            /// Stile delle AdditionalInformation
            /// </summary>
            AdditionalInformation,
            /// <summary>
            /// Stile del sommario
            /// </summary>
            SummaryStyle,
            /// <summary>
            /// Stile del contenuto
            /// </summary>
            ContentStyle
        }

        /// <summary>
        /// Generazione del report Excel
        /// </summary>
        /// <param name="request">Informazioni sul report da generare</param>
        /// <param name="reports">Report da esportare</param>
        /// <returns>Documento Excel</returns>
        public FileDocumento GenerateReport(PrintReportRequest request, List<DocsPaVO.Report.Report> reports)
        {
            var outputStream = new MemoryStream();
            using SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(outputStream, SpreadsheetDocumentType.Workbook);


            // Add a WorkbookPart to the document.
            WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            // Add Sheets to the Workbook.
            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

            // Creazione del foglio Excel
            //Workbook book = new Workbook();

            // Attivazione del primo foglio
            //book.ExcelWorkbook.ActiveSheetIndex = 0;

            // Impostazione proprietà del foglio
            spreadsheetDocument.PackageProperties.Title = "Report";
            spreadsheetDocument.PackageProperties.Created = DateTime.Now;

            // Impostazione degli stili
#if FALSE
            this.SetStyles(book);


            // Generazione di un foglio per ogni report
            foreach (var report in reports)
                // Generazione di un foglio con i dati contenuti nel report
                this.AddWorksheet(book, report);

            // Generazione del risultato dell'export
            FileDocumento document = new FileDocumento();
            using (MemoryStream stream = new MemoryStream())
            {
                book.Save(stream);
                stream.Position = 0;

                document.name = String.Format("Report_{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
                document.path = String.Empty;
                document.fullName = document.name;
                document.contentType = "application/vnd.ms-excel";
                document.content = new Byte[stream.Length];

                stream.Read(document.content, 0, document.content.Length);
            }

            return document;

#endif
            return null;
        }

#if false
        /// <summary>
        /// Metodo per l'aggiunta degli stili al workbook Excel
        /// </summary>
        /// <param name="book">workbook a cui aggiungere gli stili</param>
        private void SetStyles(Workbook book)
        {
            WorksheetStyle style;

            #region Title

            style = book.Styles.Add(StylesEnum.TitleStyle.ToString());
            style.Font.FontName = "Arial";
            style.Font.Size = 16;
            style.Alignment.WrapText = true;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            style.Font.Color = "Black";
            style.Interior.Color = "White";

            #endregion

            #region Subtitle

            style = book.Styles.Add(StylesEnum.SubtitleStyle.ToString());
            style.Font.FontName = "Arial";
            style.Font.Size = 12;
            style.Alignment.WrapText = true;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            style.Font.Color = "Black";
            style.Interior.Color = "White";

            #endregion

            #region AdditionalInformation

            style = book.Styles.Add(StylesEnum.AdditionalInformation.ToString());
            style.Font.FontName = "Arial";
            style.Font.Size = 12;
            style.Alignment.WrapText = true;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            style.Alignment.Vertical = StyleVerticalAlignment.Center;
            style.Font.Color = "Black";
            style.Interior.Color = "White";

            #endregion

            #region Summary

            style = book.Styles.Add(StylesEnum.SummaryStyle.ToString());
            style.Font.FontName = "Arial";
            style.Font.Size = 10;
            style.Alignment.WrapText = true;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            style.Font.Color = "Black";
            style.Interior.Color = "White";

            #endregion

            #region Header
            style = book.Styles.Add(StylesEnum.HeaderStyle.ToString());
            style.Font.FontName = "Arial";
            style.Font.Size = 8;
            style.Alignment.WrapText = true;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            style.Alignment.Vertical = StyleVerticalAlignment.Bottom;
            style.Font.Color = "Black";
            style.Interior.Color = "#C0C0C0";
            style.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 2, "Black");
            style.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 2, "Black");
            style.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 2, "Black");
            style.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 2, "Black");
            style.Interior.Pattern = StyleInteriorPattern.Solid;
            #endregion

            #region Content

            style = book.Styles.Add(StylesEnum.ContentStyle.ToString());
            style.Font.FontName = "Arial";
            style.Font.Size = 8;
            style.Alignment.WrapText = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Justify;
            style.Alignment.Vertical = StyleVerticalAlignment.Top;
            style.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1, "Black");
            style.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1, "Black");
            style.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1, "Black");
            style.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1, "Black");

            #endregion

            #region Default

            style = book.Styles.Add("Default");
            style.Font.FontName = "Arial";
            style.Font.Size = 8;

            #endregion

        }

        /// <summary>
        /// Metodo per la generazione di un foglio Excel
        /// </summary>
        /// <param name="book">Workbook a cui aggiungere il foglio</param>
        /// <param name="report">Report da cui prelevare i dati da esportare nel folgio corrente</param>
        private void AddWorksheet(Workbook book, DocsPaVO.Report.Report report)
        {

            // Worksheet da restituire
            String sheetName = String.Empty;
            if (!String.IsNullOrEmpty(report.SectionName))
            {
                sheetName = report.SectionName;
            }
            else
            {
                sheetName = String.Format("Foglio {0}", book.Worksheets.Count);
            }
            Worksheet sheet = book.Worksheets.Add(sheetName);

            sheet.Options.PageSetup.Layout.Orientation = Orientation.NotSet;

            if (report.ShowHeaderRow)
            {
                // Aggiunta del titolo
                WorksheetRow titleRow = sheet.Table.Rows.Add();
                if (report.ReportHeader.Count > 0)
                    titleRow.Cells.Add(new WorksheetCell(report.Title, DataType.String, StylesEnum.TitleStyle.ToString()) { MergeAcross = report.ReportHeader.Count - 1 });
                else
                {
                    titleRow.Cells.Add(new WorksheetCell(report.Title, DataType.String, StylesEnum.TitleStyle.ToString()) { MergeAcross = report.ReportHeader.Count });
                }
                WorksheetRow fooRow = sheet.Table.Rows.Add();
                // Aggiunta del sottotitolo
                WorksheetRow subTitleRow = sheet.Table.Rows.Add();
                if (report.ReportHeader.Count > 0)
                    subTitleRow.Cells.Add(new WorksheetCell(report.Subtitle, DataType.String, StylesEnum.SubtitleStyle.ToString()) { MergeAcross = report.ReportHeader.Count - 1 });
                else
                {
                    subTitleRow.Cells.Add(new WorksheetCell(report.Subtitle, DataType.String, StylesEnum.SubtitleStyle.ToString()) { MergeAcross = report.ReportHeader.Count });
                }
                subTitleRow.Height = 30;
                subTitleRow.AutoFitHeight = true;
                WorksheetRow fooRow2 = sheet.Table.Rows.Add();
                // Aggiunta del Informazioni aggiuntive
                if (!string.IsNullOrEmpty(report.AdditionalInformation))
                {
                    WorksheetRow addInfo = sheet.Table.Rows.Add();
                    if (report.ReportHeader.Count > 0)
                        addInfo.Cells.Add(new WorksheetCell(report.AdditionalInformation, DataType.String, StylesEnum.AdditionalInformation.ToString()) { MergeAcross = report.ReportHeader.Count - 1 });
                    else
                    {
                        addInfo.Cells.Add(new WorksheetCell(report.AdditionalInformation, DataType.String, StylesEnum.AdditionalInformation.ToString()) { MergeAcross = report.ReportHeader.Count });
                    }
                    addInfo.Height = 80;
                    addInfo.AutoFitHeight = true;
                }

                // Aggiunta del summary
                WorksheetRow summaryRow = sheet.Table.Rows.Add();
                summaryRow.Cells.Add(new WorksheetCell(report.Summary, DataType.String, StylesEnum.SummaryStyle.ToString()) { MergeAcross = report.ReportHeader.Count });
            }
            // Aggiunta dell'header
            if (report.ShowHeaderRow)
            {
                // Aggiunta di una riga vuota
                sheet.Table.Rows.Add();
            }
            this.AddHeader(sheet, report.ReportHeader);

            if (string.IsNullOrEmpty(report.AdditionalInformation) && report.ShowHeaderRow)
            {
                // Impostazione dell'autofilter
                sheet.AutoFilter.Range = String.Format("R7C1:R7C{0}", report.ReportHeader.Count);
            }
            // Aggiunta dei dati al foglio
            this.AddContent(sheet, report.ReportMapRow.Rows);

        }

        /// <summary>
        /// Metodo per la generazione della riga di intestazione
        /// </summary>
        /// <param name="sheet">Foglio in cui generare l'intestazione</param>
        /// <param name="list">Lista delle colonne da aggiungere all'header</param>
        private void AddHeader(Worksheet sheet, HeaderColumnCollection headerColumns)
        {
            // Generazione della riga con l'header
            WorksheetRow headerRow = sheet.Table.Rows.Add();
            headerRow.AutoFitHeight = true;

            foreach (var column in headerColumns)
            {

                if (column.ColumnSize == 0)
                    sheet.Table.Columns.Add(new WorksheetColumn() { AutoFitWidth = true });
                else
                    sheet.Table.Columns.Add(column.ColumnSize);

                headerRow.Cells.Add(column.ColumnName, DataType.String, StylesEnum.HeaderStyle.ToString());
            }
        }

        /// <summary>
        /// Metodo per la generazione del contenuto del report
        /// </summary>
        /// <param name="sheet">Foglio a cui aggiungere i dati</param>
        /// <param name="contentRows">Lista delle righe</param>
        private void AddContent(Worksheet sheet, List<ReportMapRowProperty> contentRows)
        {
            // Generazione delle righe
            foreach (var row in contentRows)
            {
                WorksheetRow worksheetRow = sheet.Table.Rows.Add();

                // Compilazione della riga
                foreach (var column in row.Columns)
                    //worksheetRow.Cells.Add(column.Value, String.IsNullOrEmpty(column.Value) ? DataType.String : this.GetDataType(column.ContantDataType), StylesEnum.ContentStyle.ToString());
                    worksheetRow.Cells.Add(column.Value, DataType.String, StylesEnum.ContentStyle.ToString());

            }
        }

        /// <summary>
        /// Metodo per l'interpretazione del tipo di doto contenuto nella cella
        /// </summary>
        /// <param name="contentDataType">Tipo da interpretare</param>
        /// <returns>Tipo tradotto</returns>
        private DataType GetDataType(HeaderProperty.ContentDataType contentDataType)
        {
            DataType retVal = DataType.String;

            switch (contentDataType)
            {
                case HeaderProperty.ContentDataType.Boolean:
                    retVal = DataType.Boolean;
                    break;
                case HeaderProperty.ContentDataType.DateTime:
                    retVal = DataType.DateTime;
                    break;
                case HeaderProperty.ContentDataType.Integer:
                    retVal = DataType.Integer;
                    break;
                case HeaderProperty.ContentDataType.Number:
                    retVal = DataType.Number;
                    break;
                case HeaderProperty.ContentDataType.String:
                    retVal = DataType.String;
                    break;
                default:
                    retVal = DataType.String;
                    break;
            }

            return retVal;
        } 
#endif

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
        //private string GetDateTimeFormat(UInt32Value numberFormatId, string text)
        //{
        //    var valueAsString = string.Empty;

        //    if (DateFormatDictionary.ContainsKey(numberFormatId))
        //    {
        //        valueAsString = DateTime.FromOADate(Convert.ToDouble(text)).AsDateFormat();
        //    }
        //    else if (DateTimeFormatDictionary.ContainsKey(numberFormatId))
        //    {
        //        valueAsString = DateTime.FromOADate(Convert.ToDouble(text)).AsDateTimeFormat();
        //    }
        //    else if (TimeFormatDictionary.ContainsKey(numberFormatId))
        //    {
        //        valueAsString = DateTime.FromOADate(Convert.ToDouble(text)).ToString("HH:mm:ss");
        //    }

        //    return valueAsString;
        //}

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

    }
}
