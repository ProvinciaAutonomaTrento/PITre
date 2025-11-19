// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.ProspettiRiepilogativi;
using DocsPaVO.Report;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Infrastructure.DocumentFormat.OpenXml.Services.ReportGenerator;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ExportRicercaCasellaIstituzionaleRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportRicercaCasellaIstituzionale;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportRicercaCasellaIstituzionale
{
    public class ExportRicercaCasellaIstituzionaleHandler : IRequestHandler<ExportRicercaCasellaIstituzionaleRequest, PrintReportResponse>
    {

        protected readonly ILogger<ExportRicercaCasellaIstituzionaleHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IReportGeneratorService _reportGeneratorService;
        protected readonly ISpreadsheetService _spreadsheetService;
        protected readonly IFileConverterService _fileConverterService;


        public ExportRicercaCasellaIstituzionaleHandler(
            ILogger<ExportRicercaCasellaIstituzionaleHandler> logger,
            IPi3DbContext dbContext,
            IReportGeneratorService reportGeneratorService,
            ISpreadsheetService spreadsheetService,
            IFileConverterService fileConverterService
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._reportGeneratorService = reportGeneratorService;
            this._fileConverterService = fileConverterService;
            this._spreadsheetService = spreadsheetService;
        }



        public async Task<PrintReportResponse> Handle(ExportRicercaCasellaIstituzionaleRequest request, CancellationToken cancellationToken)
        {
            PrintReportResponse output = new();
            try
            {
                var data = ((PrintReportRequestDataset)request.request).InputDataset;
                switch (request.request.ReportType.ToString().ToUpper())
                {
                    case "PDF":
                        output.Document = await this.GenerateReportPdf(data, request.request);
                        break;
                    case "EXCEL":
                    case "ODS":
                        output.Document = await this.GenerateReportXLSX(data, request.request);
                        break;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
            }
            return output;
        }


        #region Private Members 

        private async Task<FileDocumento> GenerateReportXLSX(DataSet dataSet, PrintReportRequest request)
        {
            FileDocumento output = null;
            List<VirtualCell> rows = new();
            HeaderColumnCollection header = null;
            string summary = string.Format(Resources.Summary, rows.Count);
            const string fontName = "Arial";
            const int fontSize = 8;

            var model = new SpreadsheetModel();
            var sheet = new SheetModel()
            {
                Name = Resources.SheetName
            };



            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
            {
                (rows, header) = this.GetRows(dataSet, request);
                int maxRows = ((rows.Select(c => (int)c.RowIndex)).Max()) + 1;
                int rowBase = 0;
                int column = 0;

                if (!string.IsNullOrEmpty(request.Title))
                {
                    this.AddCell(sheet,rowBase,column,fontName,fontSize, request.Title);
                    rowBase += 2;
                }
                if (!string.IsNullOrEmpty(request.SubTitle))
                {
                    this.AddCell(sheet, rowBase, column, fontName, fontSize, request.SubTitle);
                    rowBase += 2;
                }
                if (!string.IsNullOrEmpty(request.AdditionalInformation))
                {
                    var utRuo = request.AdditionalInformation.Split(",");
                    foreach(var s in utRuo)
                    {
                        this.AddCell(sheet, rowBase, column, fontName, fontSize, s);
                        rowBase += 1;
                    }
                }

                this.AddCell(sheet, rowBase, column, fontName, fontSize, $"Righe estratte: {maxRows}");
                rowBase += 2;

                foreach (var headerCell in header)
                {
                    sheet.AddCell(new CellModel()
                    {
                        Row = rowBase,
                        Column = column,
                        ValueAsString = headerCell.ColumnName,
                        CellStyle = new CellStyleModel()
                        {
                            FontIsBold = true,
                            ForegroundColor = System.Drawing.Color.Gray,
                            FontName = fontName,
                            FontSize = fontSize,
                            VerticalAlignment = CellTextAlignments.Center,
                            HorizontalAlignment = CellTextAlignments.Center,
                            Width = 4 * headerCell.ColumnName.Length,
                            BorderColor = System.Drawing.Color.Black,
                            HasBorder = true
                            
                        }
                    });
                    column++;

                }

                int row = rowBase + 1;
                column = 0;

                var rs = rows.GroupBy(cell => cell.RowIndex, cu => cu, (c, cu) => new
                {
                    c,
                    cu
                });

                
                foreach (var rKey in rs)
                {

                    var rowCell = rKey.cu;
                    foreach (var c in rowCell)
                    {
                        sheet.AddCell(new CellModel()
                        {
                            Row = row,
                            Column = column,
                            ValueAsString = c.CellValue,
                            CellStyle = new CellStyleModel()
                            {
                                FontIsBold = false,
                                FontName = fontName,
                                FontSize = fontSize,
                                FontColor = System.Drawing.Color.Black,
                                FontIsStrikeout = true,
                                VerticalAlignment = CellTextAlignments.Center,
                                HorizontalAlignment = CellTextAlignments.Center,
                                WrapText = true
                            }
                        });
                        column++;
                    }
                    column = 0;
                    row++;
                }


                model.AddSheet(sheet);

                using MemoryStream stream = new MemoryStream();
                var reportGenerated = await _spreadsheetService.Write(model, stream);

                output = new FileDocumento()
                {
                    content = stream.ToArray(),
                    length = Convert.ToInt32(stream.Length),
                    contentType = reportGenerated.ContentType,
                    estensioneFile = Path.GetExtension(reportGenerated.FileName),
                    fullName = string.Format(Resources.FullNameExportXLSX, DateTime.Now.ToString("dd-MM-yyyy")),
                    name = string.Format(Resources.FullNameExportXLSX, DateTime.Now.ToString("dd-MM-yyyy")),
                };

            }


            

            return output;
        }

        private async Task<FileDocumento> GenerateReportPdf(DataSet dataSet, PrintReportRequest request)
        {
            FileDocumento document = null;
            ReportModel reportModel = null;
            List<VirtualCell> rows = new();
            HeaderColumnCollection header = null;
            string summary = string.Format(Resources.Summary, rows.Count);
            reportModel = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsPdf
            };


            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
            {
                (rows,header) = this.GetRows(dataSet,request);
                int maxRows = ((rows.Select(c => (int) c.RowIndex)).Max()) + 1;
                this.AddHeaderSec(reportModel, request, maxRows);

                GridRowModel headerRow = new GridRowModel();
                GridSectionModel pdfSection = new GridSectionModel
                {
                    Style = new GridSectionStyleModel()
                    {
                        WithPercentage = 100,
                    }
                };

                foreach (var cell in header)
                {
                    headerRow.AddCell(new GridCellModel()
                    {
                        Style = new GridCellStyleModel()
                        {
                            WithPercentage = 5,
                            ForegroundColor = System.Drawing.Color.Silver,
                            VerticalAlignment = VerticalAlignments.Center,
                            Justification = Justifications.Center,
                        },
                        Content = GetHeaderPhrase(cell.ColumnName)
                    });

                }
                pdfSection.AddRow(headerRow);

                GridRowModel dataRow = new GridRowModel();

                var rs = rows.GroupBy( cell => cell.RowIndex , cu => cu , (c,cu) => new
                {
                    c,
                    cu
                });

                foreach (var rKey in rs)
                {
                    dataRow = new();
                    var row = rKey.cu;

                    foreach (var c in row)
                    {
                        dataRow.AddCell(GetDataPhrase(c.CellValue));
                    }

                    pdfSection.AddRow(dataRow);
                }

                reportModel.AddSection(pdfSection);


                var pageNumber = new PageNumberSectionModel()
                {
                    TextStyle = new TextStyleModel
                    {
                        FontName = "Arial",
                        FontSize = 14
                    },

                    Style = new TextSectionStyleModel
                    {
                        Justification = Justifications.Left
                    },
                    Format = $"Pagina {{{Pi3.Infrastructure.IText.ReportGenerator.Services.CommandMarkersHelper.GetCurrentPageNumberMarker()}}}",
                };
                reportModel.AddFooterSection(pageNumber);
            }

            using MemoryStream stream = new();
            var reportGenerated = await this._reportGeneratorService.Generate(reportModel, stream);

            document = new FileDocumento();
            document.path = String.Empty;
            document.name = string.Format(Resources.FullNameExportPDF, DateTime.Now.ToString("dd-MM-yyyy"));
            document.fullName = document.name;
            document.content = stream.ToArray();
            document.contentType = reportGenerated.ContentType;

            return document;

        }
        private void AddHeaderSec(ReportModel model, PrintReportRequest report, int rowsExported)
        {
            // Aggiunta di titolo, sottotitolo e summary
            model.AddSection(this.AddReportTitle(report.Title));
            model.AddSection(this.AddReportSubtitle(report.SubTitle));
            
            if(!string.IsNullOrEmpty(report.AdditionalInformation))
                model.AddSection(this.AddReportAdditionalInformation(report.AdditionalInformation));
            
            model.AddSection(this.AddReportSummary($"Righe estratte: {rowsExported}"));

        }

        private TextSectionModel AddReportTitle(string title)
        {
            return new TextSectionModel()
            {
                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Left
                },
                Content = new TextContentModel()
                {
                    Value = title,
                    Style = new TextStyleModel()
                    {
                        FontName = "Arial",
                        FontSize = 16,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }
        private TextSectionModel AddReportSubtitle(string subtitle)
        {
            return new TextSectionModel()
            {
                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Left
                },
                Content = new TextContentModel()
                {
                    Value = subtitle,
                    Style = new TextStyleModel()
                    {
                        FontName = "Arial",
                        FontSize = 12,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }

        private TextSectionModel AddReportAdditionalInformation(string additionalInformation)
        {
            return new TextSectionModel()
            {
                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Left
                },
                Content = new TextContentModel()
                {
                    Value = additionalInformation,
                    Style = new TextStyleModel()
                    {
                        FontName = "Arial",
                        FontSize = 10,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }

        private void AddCell(SheetModel sheet, int row,int column,string fontName,int fontSize,string cellValue)
        {
            sheet.AddCell(new CellModel()
            {
                Row = row,
                Column = column,
                ValueAsString = cellValue,
                CellStyle = new CellStyleModel()
                {
                    FontIsBold = true,
                    FontName = fontName,
                    FontSize = fontSize + 2,
                    VerticalAlignment = CellTextAlignments.Center,
                    HorizontalAlignment = CellTextAlignments.Center,
                    WrapText = true,

                }
            });
        }


        private TextSectionModel AddReportSummary(string summary)
        {
            return new TextSectionModel()
            {
                Style = new TextSectionStyleModel
                {
                    Justification = Justifications.Left
                },
                Content = new TextContentModel()
                {
                    Value = summary,
                    Style = new TextStyleModel()
                    {
                        FontName = "Arial",
                        FontSize = 10,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }
        public GridCellModel GetDataPhrase(string columnName)
        {

            return new GridCellModel()
            {
                Style = new GridCellStyleModel()
                {
                    WithPercentage = 3,
                    ForegroundColor = System.Drawing.Color.White,
                    VerticalAlignment = VerticalAlignments.Center,
                    Justification = Justifications.Center,
                },
                Content = new TextContentModel()
                {
                    Value = columnName,
                    Style = new TextStyleModel()
                    {
                        FontName = "Arial",
                        FontSize = 8,
                        FontColor = System.Drawing.Color.Black,
                        FontIsBold = false,
                    }
                }
            };
        }


        private TextContentModel GetHeaderPhrase(string columnName)
        {

            return new()
            {
                Value = columnName,
                Style = new TextStyleModel()
                {
                    FontName = "Arial",
                    FontSize = 8,
                    FontColor = System.Drawing.Color.Black,
                    FontIsBold = true,
                }
            };

        }


        private void AddHeader(ReportModel model, PrintReportRequest report, int rowsExported)
        {
            // Aggiunta di titolo, sottotitolo e summary
            model.AddSection(this.AddHeaderPhrase(report.Title));
            model.AddSection(this.AddHeaderPhrase(report.SubTitle));
            model.AddSection(this.AddHeaderPhrase(report.AdditionalInformation));
            model.AddSection(this.AddHeaderPhrase($"Righe estratte: {rowsExported}"));

        }
        private TextSectionModel AddHeaderPhrase(string title)
        {
            return new TextSectionModel()
            {
                Content = new TextContentModel()
                {
                    Value = title,
                    Style = new TextStyleModel()
                    {
                        FontName = "Arial",
                        FontSize = 16,
                        FontIsItalic = true,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }


        private (List<VirtualCell>, HeaderColumnCollection) GetRows(DataSet dataSet, PrintReportRequest request)
        {
            string idCounter = string.Empty;
            HeaderColumnCollection header = null;
            var rows = new List<VirtualCell>();

            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {

                if (request.SearchFilters != null)
                {
                    DocsPaVO.filtri.FiltroRicerca filtro = request.SearchFilters.Where(f => f.argomento == "idCounter").FirstOrDefault();
                    if (filtro != null)
                        idCounter = filtro.valore;
                }
                if (string.IsNullOrEmpty(idCounter))
                {
                    // Generazione dell'header del report
                    header = this.GenerateReportHeader(dataSet, request.ColumnsToExport);
                    rows.AddRange(this.GenerateReportRows(dataSet, header));
                }
                else
                {
                    // Generazione dell'header del report
                    header = this.GenerateReportHeader(dataSet, request.ColumnsToExport, idCounter);
                    rows.AddRange(this.GenerateReportRows(dataSet, header, idCounter));
                }
            }

            return (rows,header);

        }


        private List<VirtualCell> GenerateReportRows(DataSet dataSet, HeaderColumnCollection reportHeader,string idCounter = "")
        {
            List<VirtualCell> cells = new();

            for (int rowIndex = 0; rowIndex < dataSet.Tables[0].Rows.Count; rowIndex++)
            {
                DataRow row = dataSet.Tables[0].Rows[rowIndex];

                for (int colInd = 0; colInd < row.Table.Columns.Count; colInd++)
                {
                    DataColumn dataColumn = row.Table.Columns[colInd];
                    if (reportHeader[dataColumn.ColumnName] != null && reportHeader[dataColumn.ColumnName].Export)
                    {
                        cells.Add(new VirtualCell()
                        {
                            CellIndex = colInd,
                            RowIndex = rowIndex,
                            CellValue = row[dataColumn].ToString(),

                        });
                    }
                }
            }

            return cells;
        }



        private class VirtualCell
        {
            public string? CellValue { get; set; }

            public int? RowIndex { get; set; }
            public int? CellIndex { get; set; }
        }

        private HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport, string idCounter = "")
        {
            HeaderColumnCollection header = new HeaderColumnCollection();

            // Se la lista dei campi esportabili è nulla o se è vuota, ci si basa sul dataset
            if (fieldsToExport == null || fieldsToExport.Count == 0)
                header = this.GenerateReportHeaderFromDataSet(dataSet);
            else
                header = this.GenerateReportHeaderFromColumnCollection(fieldsToExport);

            return header;
        }


        private HeaderColumnCollection GenerateReportHeaderFromDataSet(DataSet dataSet)
        {
            HeaderColumnCollection header = new HeaderColumnCollection();

            foreach (DataColumn row in dataSet.Tables[0].Columns)
            {
                header.Add(new HeaderProperty()
                {
                    ColumnName = row.ColumnName,
                    OriginalName = row.ColumnName,
                    ColumnSize = 0,
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
            }

            return header;

        }


        private HeaderColumnCollection GenerateReportHeaderFromColumnCollection(HeaderColumnCollection exportableCollection)
        {
            return new HeaderColumnCollection(exportableCollection.Where(e => e.Export == true));
        }


        #endregion



    }
}
