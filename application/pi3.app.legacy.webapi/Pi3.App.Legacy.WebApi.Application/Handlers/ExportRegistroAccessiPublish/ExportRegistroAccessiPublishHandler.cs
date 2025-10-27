// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.ProspettiRiepilogativi;
using DocsPaVO.Report;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportRegistroAccessiPublishRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportRegistroAccessiPublish;
namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportRegistroAccessiPublish
{
    public class ExportRegistroAccessiPublishHandler : IRequestHandler<ExportRegistroAccessiPublishRequest, PrintReportResponse>
    {
        protected readonly ILogger<ExportRegistroAccessiPublishHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IReportGeneratorService _reportGeneratorService;
        protected readonly ISpreadsheetService _spreadsheetService;
        protected readonly IFileConverterService _fileConverterService;

        public ExportRegistroAccessiPublishHandler(
            ILogger<ExportRegistroAccessiPublishHandler> logger,
            IPi3DbContext dbContext ,
            IReportGeneratorService reportGeneratorService,
            ISpreadsheetService spreadsheetService,
            IFileConverterService fileConverterService
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._spreadsheetService = spreadsheetService;
            this._fileConverterService = fileConverterService;
            this._reportGeneratorService = reportGeneratorService;
        }


        public async Task<PrintReportResponse> Handle(ExportRegistroAccessiPublishRequest request, CancellationToken cancellationToken)
        {
            PrintReportResponse output = new();
            try
            {
                var data = ((PrintReportRequestDataset)request.request).InputDataset;
                switch (request.request.ReportType.ToString().ToUpper())
                {
                    case "PDF":
                        output.Document = await this.GenerateReportPdf(data,request.request);
                        break;
                    case "EXCEL":
                    case "ODS":
                        output.Document = await this.GenerateReportXLSX(data, request.request);
                        break;
                }
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex,ex.Message);
            }

            return output;
        }

        #region Pdf Generation
        private async Task<FileDocumento> GenerateReportPdf(DataSet dataSet, PrintReportRequest request)
        {
            FileDocumento document = null;
            ReportModel reportModel = null;


            reportModel = new ReportModel()
            {
                Size = PageSizes.A3,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsPdf
            };


            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
            {
                // Extraction of header's columns 
                HeaderColumnCollection reportHeader = this.GetHeaderCols(dataSet, request.ColumnsToExport);
                GridRowModel pdfRow = new GridRowModel();
                GridSectionModel pdfSection = new GridSectionModel
                {
                    Style = new GridSectionStyleModel()
                    {
                        WithPercentage = 100,
                    }
                };
                PdfBuilder pdfManipulator = new PdfBuilder(pdfSection);

                // Adding Non profiled (fixed) table header's columns
                pdfRow.AddCell(pdfManipulator.GetHeaderCell("PROGRESSIVO"));
                pdfRow.AddCell(pdfManipulator.GetHeaderCell("DESCRIZIONE"));
                pdfRow.AddCell(pdfManipulator.GetHeaderCell("UFFICIO"));



                HashSet<string> list = new HashSet<string>();

                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {

                    if (string.IsNullOrEmpty(dataRow["ID_PROJECT"].ToString()))
                    {
                        if (dataRow["NOME_CAMPO"] != null && dataRow["NOME_CAMPO"].ToString().ToUpper() != "PROGRESSIVO")
                        {
                            if (!list.Contains(dataRow["NOME_CAMPO"].ToString()))
                            {
                                list.Add(dataRow["NOME_CAMPO"].ToString());
                                pdfRow.AddCell(pdfManipulator.GetHeaderCell(dataRow["NOME_CAMPO"].ToString()));
                            }

                        }

                    }
                }


                // Adding Header 
                pdfSection.AddRow(pdfRow);

                // Adding data rows and table header to section
                var nRows = this.FillReport(dataSet, reportHeader, pdfManipulator);
                this.AddHeader(reportModel, request, nRows);

                reportModel.AddSection(pdfSection);


            }

            using MemoryStream stream = new();
            var reportGenerated = await this._reportGeneratorService.Generate(reportModel, stream);

            // Generazione del risultato dell'export
            document = new FileDocumento();
            document.name = string.Format(Resources.FullNameExportPDF, DateTime.Now.ToString("dd-MM-yyyy"));
            document.path = String.Empty;
            document.fullName = document.name;
            document.contentType = reportGenerated.ContentType;
            document.content = stream.ToArray();

            return document;



        }
        #endregion

        #region Excel Generation
        private async Task<FileDocumento> GenerateReportXLSX(DataSet dataSet,PrintReportRequest request)
        {
            FileDocumento output = null;

            var model = new SpreadsheetModel();
            var sheet = new SheetModel()
            {
                Name = Resources.FullNameExportXLSX
            };

            HashSet<string> headerColSet = new HashSet<string>();
            int column = 3;
            ExcelBuilder excelManipulator = new(sheet);


            sheet.AddCell(excelManipulator.GetHeaderCell("PROGRESSIVO",0));
            sheet.AddCell(excelManipulator.GetHeaderCell("DESCRIZIONE",1));
            sheet.AddCell(excelManipulator.GetHeaderCell("UFFICIO",2));

            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {

                if (string.IsNullOrEmpty(dataRow["ID_PROJECT"].ToString()))
                {
                    if (dataRow["NOME_CAMPO"] != null && dataRow["NOME_CAMPO"].ToString().ToUpper() != "PROGRESSIVO")
                    {
                        if (!headerColSet.Contains(dataRow["NOME_CAMPO"].ToString()))
                        {
                            headerColSet.Add(dataRow["NOME_CAMPO"].ToString());
                            var x = dataRow["NOME_CAMPO"];
                            sheet.AddCell(excelManipulator.GetHeaderCell(dataRow["NOME_CAMPO"].ToString(),column));
                            column++;
                        }

                    }

                }
            }
            HeaderColumnCollection reportHeader = this.GetHeaderCols(dataSet, request.ColumnsToExport);

            var rows = this.FillReport(dataSet, reportHeader,excelManipulator);

            sheet.AddCell(this.GetHeaderSectionsCell(request.Title,1,0));
            sheet.AddCell(this.GetHeaderSectionsCell(request.SubTitle,3,0));
            sheet.AddCell(this.GetHeaderSectionsCell(request.AdditionalInformation,5,0));
            sheet.AddCell(this.GetHeaderSectionsCell($"Righe estratte: {rows}",7,0));

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
            
            return output;
        }

        #endregion

        #region Report - Table filler
        private int FillReport<TRow, TSection>(DataSet dataSet, HeaderColumnCollection reportHeader, IReportBuilder<TRow, TSection> reportManipulator)
        {
            string folderId = string.Empty;
            ReportMapRowProperty rowToUp = null;
            bool isFirstLogicalRow = true;
            int rowsAdded = 0;

            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                DataRow dataRow = dataSet.Tables[0].Rows[i];
                MetaRow<TRow> metaRow = new();
                if (!string.IsNullOrEmpty(dataRow["ID_PROJECT"].ToString()))
                {
                    // la prima condizione crea e valorizza i campi non profilati / statici ( i = 0 )
                    // nell'update valorizzo il resto dato che ogni riga nell'else ha l'i-esimo campo profilato
                    if (string.IsNullOrEmpty(folderId) || !dataRow["ID_PROJECT"].ToString().Equals(folderId))
                    {
                        if (!isFirstLogicalRow)
                        {
                            rowsAdded++;
                            reportManipulator.AddRow(rowsAdded);

                        }
                        else
                        {
                            isFirstLogicalRow = false;
                        }
                        reportManipulator.CreateEmptyRow();

                        // Se folderId non è valorizzato sto analizzando la prima riga del dataset
                        // Se il valore del campo ID_PROJECT del dataset non coincide con folderId sto analizzando una nuova riga
                        folderId = dataRow["ID_PROJECT"].ToString();

                        metaRow = this.GenerateNewRow<TRow, TSection>(dataRow, reportHeader, reportManipulator, rowsAdded);
                        rowToUp = metaRow.rMapRow;
                    }
                    else
                    {
                        // Se il valore del campo ID_PROJECT coincide con folderId devo aggiungere il valore dell'i-esimo campo profilato
                        // alla riga del report

                        reportManipulator = this.UpdateRow<TRow, TSection>(rowToUp, reportManipulator, dataRow, reportHeader, rowsAdded);
                    }

                    if (i == dataSet.Tables[0].Rows.Count - 1)
                    {
                        rowsAdded++;
                        reportManipulator.AddRow(rowsAdded);
                    }
                }


            }
            return rowsAdded;
        }

        private MetaRow<TRow> GenerateNewRow<TRow, TSection>(DataRow dataRow, HeaderColumnCollection reportHeader, IReportBuilder<TRow, TSection> reportManipulator, int rowIndex)
        {

            if (dataRow["NOME_CAMPO"].ToString().ToUpper() == "PROGRESSIVO")
            {
                reportManipulator.AddCell(dataRow["VALORE_CAMPO"].ToString(), rowIndex);
            }


            reportManipulator.AddCell(dataRow["DESCRIZIONE"].ToString(), rowIndex);
            reportManipulator.AddCell(dataRow["UFFICIO"].ToString(), rowIndex);


            ReportMapRowProperty r = new ReportMapRowProperty();

            if (dataRow["NOME_CAMPO"].ToString().ToUpper() == "PROGRESSIVO")
            {
                r.Columns.Add(this.GenerateHeaderColumn(dataRow["VALORE_CAMPO"].ToString(), "PROGRESSIVO", "Progressivo"));
            }
            r.Columns.Add(this.GenerateHeaderColumn(dataRow["DESCRIZIONE"].ToString(), "DESCRIZIONE", "DESCRIZIONE"));
            r.Columns.Add(this.GenerateHeaderColumn(dataRow["UFFICIO"].ToString(), "UFFICIO", "UFFICIO"));



            return new MetaRow<TRow>
            {
                pdfRow = reportManipulator.GetRow(),
                rMapRow = r
            };
        }


        private IReportBuilder<TRow, TSection> UpdateRow<TRow, TSection>(ReportMapRowProperty row, IReportBuilder<TRow, TSection> reportManipulator, DataRow dataRow, HeaderColumnCollection reportHeader, int rowIndex)
        {

            if (dataRow["NOME_CAMPO"].ToString().ToUpper() != "PROGRESSIVO")
            {
                // Controllo esistenza nell'header
                if (reportHeader[dataRow["NOME_CAMPO"].ToString()] != null)
                {
                    // Controllo esistenza valore nella riga
                    if (row[dataRow["NOME_CAMPO"].ToString()] == null)
                    {
                        row.Columns.Add(this.GenerateHeaderColumn(dataRow["VALORE_CAMPO"].ToString(), dataRow["NOME_CAMPO"].ToString(), dataRow["NOME_CAMPO"].ToString()));
                    }
                    reportManipulator.AddCell(dataRow["VALORE_CAMPO"].ToString(), rowIndex);
                }
            }
            return reportManipulator;
        }

        #endregion


        #region Classes and interfaces for report generation
        private class MetaRow<T>
        {
            public T? pdfRow { get; set; }
            public ReportMapRowProperty? rMapRow { get; set; }
        }

        private class VirtualRow
        {
            public List<VirtualCell>? Cells { get; set; }

            public VirtualRow()
            {
                Cells = new List<VirtualCell>();
            }
        }

        private class VirtualCell
        {
            public int RowIndex { get; set; }
            public int ColIndex { get; set; }
            public string? Content { get; set; }
        }

        private interface IReportBuilder<Trow,Tsection>
        {
            public void AddCell(string cellValue,int rowIndex);
            public Trow GetRow();

            public void AddRow(int rowIndex);

            public void CreateEmptyRow();
        }
         


        private class ExcelBuilder : IReportBuilder<VirtualRow,SheetModel>
        {
            private const int baseRow = 9;
            public SheetModel Sheet { get; set; }
            public VirtualRow Row { get; set; }

            public int ColIndex { get; set; }

            public ExcelBuilder(SheetModel sheet)
            {
                this.Sheet = sheet;
                this.Row = new();
                this.ColIndex = 0;
            }
            public VirtualRow GetRow()
            {
                return this.Row;
            }
            public void CreateEmptyRow()
            {
                this.Row = new();
                this.ColIndex = 0;

            }
            public void AddCell(string cellValue, int rowIndex)
            {
                this.Row.Cells.Add(new VirtualCell()
                {
                    Content = cellValue,
                    RowIndex = rowIndex,
                    ColIndex = this.ColIndex++
                });
            }
            public void AddRow(int rowIndex)
            {
                foreach (var cell in this.Row.Cells)
                {
                    Sheet.AddCell(
                        new CellModel()
                        {
                            Row = rowIndex+baseRow,
                            Column = cell.ColIndex,
                            ValueAsString = cell.Content,
                            CellStyle = new CellStyleModel()
                            {
                                FontIsBold = false,
                                FontName = "Arial",
                                FontSize = 10,
                                FontColor = System.Drawing.Color.Black,
                                FontIsStrikeout = true,
                                VerticalAlignment = CellTextAlignments.Center,
                                HorizontalAlignment = CellTextAlignments.Center,
                                BorderColor = System.Drawing.Color.Black,
                                HasBorder = true
                            }
                        });
                }
            }

            public CellModel GetHeaderCell(string value,int column)
            {
                return new CellModel()
                {
                    Row = baseRow,
                    Column = column,
                    ValueAsString = value,
                    CellStyle = new CellStyleModel()
                    {
                        FontIsBold = true,
                        ForegroundColor = System.Drawing.Color.LightGray,
                        FontColor = System.Drawing.Color.Black,
                        FontName = "Arial",
                        FontSize = 10,
                        VerticalAlignment = CellTextAlignments.Center,
                        HorizontalAlignment = CellTextAlignments.Center,
                        BorderColor = System.Drawing.Color.Black,
                        HasBorder = true,
                        Width = 60
                    }
                };
            }
        }
        private CellModel GetHeaderSectionsCell(string text, int row, int col)
        {
            return new CellModel()
            {
                Row = row,
                Column = col,
                ValueAsString = text,
                CellStyle = new CellStyleModel()
                {
                    FontIsBold = true,
                    FontColor = System.Drawing.Color.Black,
                    FontName = "Arial",
                    FontSize = 10,
                    VerticalAlignment = CellTextAlignments.Center,
                    HorizontalAlignment = CellTextAlignments.Center,
                    Width = 60
                }
            };
        }
        private class PdfBuilder : IReportBuilder<GridRowModel,GridSectionModel>
        {
            public GridRowModel Row { get; set; }

            public GridSectionModel Section { get; set; }

            public PdfBuilder(GridSectionModel section)
            {
                Row = new GridRowModel();
                Section = section;
            }

            public void AddCell(string cellValue,int rowIndex)
            {
                var cell = this.GetRowCell(cellValue);
                Row.AddCell(cell);
            }

            public void CreateEmptyRow()
            {
                this.Row = new GridRowModel();
            }

            public GridRowModel GetRow()
            {
                return Row;
            }

            public void AddRow(int rowIndex)
            {
                this.Section.AddRow(this.Row);
            }

            private GridCellModel GetRowCell(string value)
            {
                return new GridCellModel
                {
                    Style = new GridCellStyleModel()
                    {
                        WithPercentage = 3,
                        ForegroundColor = System.Drawing.Color.White,
                        VerticalAlignment = VerticalAlignments.Top,
                        Justification = Justifications.Center,
                    },
                    Content = new TextContentModel()
                    {
                        Value = value,
                        Style = new TextStyleModel()
                        {
                            FontName = "Helvetica",
                            FontSize = 8,
                            FontColor = System.Drawing.Color.Black,
                        }
                    }

                };
            }
            public GridCellModel GetHeaderCell(string value)
            {
                return new GridCellModel
                {

                    Style = new GridCellStyleModel()
                    {
                        WithPercentage = 5,
                        ForegroundColor = System.Drawing.Color.Silver,
                        VerticalAlignment = VerticalAlignments.Center,
                        Justification = Justifications.Center,
                    },
                    Content = new TextContentModel()
                    {
                        Value = value,
                        Style = new TextStyleModel()
                        {
                            FontName = "Arial",
                            FontSize = 6,
                            FontIsItalic = true,
                            FontColor = System.Drawing.Color.Black,
                            FontIsBold = true,
                        }
                    }
                };
            }
        }
        #endregion


        #region utils for header generation

        private ReportMapColumnProperty GenerateHeaderColumn(string value, string columnName, String originalName)
        {
            return new ReportMapColumnProperty()
            {
                OriginalName = originalName,
                ColumnName = columnName,
                Value = value
            };
        }

        private CellModel SectionEq(string text, int row, int col)
        {
            return new CellModel()
            {
                Row = row,
                Column = col,
                ValueAsString = text,
                CellStyle = new CellStyleModel()
                {
                    FontIsBold = true,
                    FontColor = System.Drawing.Color.Black,
                    FontName = "Arial",
                    FontSize = 10,
                    VerticalAlignment = CellTextAlignments.Center,
                    HorizontalAlignment = CellTextAlignments.Center,
                    Width = 60
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
                        FontName = "Helvetica",
                        FontSize = 16,
                        FontIsItalic = true,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }

        private HeaderColumnCollection GetHeaderCols(DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {
            HeaderColumnCollection res = new HeaderColumnCollection();
            bool campiProfilatiAdded = false;
            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                if (string.IsNullOrEmpty(dataRow["ID_PROJECT"].ToString()))
                {
                    // Le prime righe con id_project = null contengono solo i campi profilati della tipologia
                    if (dataRow["NOME_CAMPO"] != null && dataRow["NOME_CAMPO"].ToString().ToUpper() != "CONTATORE")
                    {
                        res.Add(new HeaderProperty() { ColumnName = dataRow["NOME_CAMPO"].ToString(), OriginalName = dataRow["NOME_CAMPO"].ToString(), ColumnSize = 80 });
                    }
                }
                else
                {
                    // Se il valore della riga cambia sono passato all'elemento successivo e devo fermarmi
                    if (campiProfilatiAdded)
                    {
                        break;
                    }
                    campiProfilatiAdded = true;
                }
            }

            return res;


        }
        #endregion


    }
}
