// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Conservazione.Rapporto;
using DocsPaVO.documento;
using DocsPaVO.Report;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Packaging;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExportFindAndReplaceRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportFindAndReplace;
namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportFindAndReplace
{
    public class ExportFindAndReplaceHandler : IRequestHandler<ExportFindAndReplaceRequest, PrintReportResponse>
    {


        protected readonly ILogger<ExportFindAndReplaceHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IReportGeneratorService _reportGeneratorService;
        protected readonly ISpreadsheetService _spreadsheetService;
        protected readonly IFileConverterService _fileConverterService;

        public ExportFindAndReplaceHandler(
            ILogger<ExportFindAndReplaceHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IReportGeneratorService reportGeneratorService,
            ISpreadsheetService spreadsheetService,
            IFileConverterService fileConverterService
            )
        {
            this._logger = logger;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._reportGeneratorService = reportGeneratorService;
            this._spreadsheetService = spreadsheetService;
            this._fileConverterService = fileConverterService;
            this._claimsPrincipalService = claimsPrincipalService;
        }

        public async Task<PrintReportResponse> Handle(ExportFindAndReplaceRequest request,CancellationToken cancellationToken)
        {
            
            PrintReportResponse output = new PrintReportResponse();

            try
            {
                
                var data = ExtractData(request);

                switch (request.request.ReportType.ToString().ToUpper())
                {
                    case "PDF":
                        output.Document = await GenerateReportPdf(request, data);
                        break;
                    case "EXCEL":
                    case "ODS":
                        output.Document = await GenerateReportXLSX(request, data);
                        break;
                }
                
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);

            }
            return output;
            
        }

        private async Task<FileDocumento> GenerateReportXLSX(ExportFindAndReplaceRequest request,List<DataRow> data)
        {
            FileDocumento output = null;

            var model = new SpreadsheetModel();
            var sheet = new SheetModel()
            {
                Name = Resources.SheetName
            };

            int column = 0;

            var dataRow = data.FirstOrDefault();

            if (dataRow != null)
            {
                foreach (var cell in dataRow.DataCells)
                {
                    sheet.AddCell(new CellModel()
                    {
                        Row = 0,
                        Column = column,
                        ValueAsString = cell.ColumnName,
                        CellStyle = new CellStyleModel()
                        {
                            FontIsBold = true,
                            ForegroundColor = System.Drawing.Color.Black,
                            FontName = "Arial",
                            FontSize = 20,
                            VerticalAlignment = CellTextAlignments.Center,
                            HorizontalAlignment = CellTextAlignments.Center
                        }
                    });

                    column++;
                }
            }

            int row = 1;
            column = 0;

            foreach (var dataRw in data)
            {
                foreach (var cell in dataRw.DataCells)
                {
                    
                    sheet.AddCell(new CellModel()
                    {
                        Row = row,
                        Column = column,
                        ValueAsString = cell.Value,
                        CellStyle = new CellStyleModel()
                        {
                            FontIsBold = false,
                            FontName = "Arial",
                            FontSize = 18,
                            FontColor = System.Drawing.Color.Red,
                            FontIsStrikeout = true,
                            VerticalAlignment = CellTextAlignments.Center,
                            HorizontalAlignment = CellTextAlignments.Center
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

            return output;
        }

        private List<DataRow> ExtractData(ExportFindAndReplaceRequest request)
        {
            var dataRows = new List<DataRow>();
            // Casting dell'oggetto request ad oggetto PrintReportRequestObjectTrasformation
            PrintReportObjectTransformationRequest casted = request.request as PrintReportObjectTransformationRequest;

            // Se casted è null -> eccezione
            if (casted == null)
                throw new Exception();

            foreach (object obj in casted.DataObject)
            {
                DataRow row = new();
                // Prelevamento delle proprietà decorate con l'attributo PropertyToExportAttribute
                PropertyInfo[] properties = (PropertyInfo[])obj.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(PropertyToExportAttribute), false).Count() > 0).ToArray<PropertyInfo>();
                row.DataCells = new();
                foreach (PropertyInfo prop in properties)
                {
                    DataCell cell = new();
                    cell.ColumnName = ((PropertyToExportAttribute)(prop.GetCustomAttributes(typeof(PropertyToExportAttribute), false)[0])).Name;
                    cell.Value = prop.GetValue(obj, null).ToString().Replace("<br />", " ");
                    row.DataCells.Add(cell);
                }
                dataRows.Add(row);
            }
            return dataRows;
        }

        private class DataRow
        {
            public List<DataCell> DataCells { get; set; }
        }
        private class DataCell
        {
            public string ColumnName { get; set; }
            public string Value { get; set; }
        }

        private async Task<FileDocumento> GenerateReportPdf(ExportFindAndReplaceRequest request, List<DataRow> data)
        {
            // Risultato da restituire
            FileDocumento document = null;

            ReportModel rm = null;




            rm = new ReportModel()
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsPdf
            };


            // Aggiunta del contenuto alle pagine
            this.AddPage(rm, request, data);


            using MemoryStream stream = new MemoryStream();
            var reportGenerated = await this._reportGeneratorService.Generate(rm, stream);

            // Generazione del risultato dell'export
            document = new FileDocumento();
            document.name = string.Format(Resources.FullNameExportPDF, DateTime.Now.ToString("dd-MM-yyyy"));
            document.path = string.Empty;
            document.fullName = document.name;
            document.contentType = reportGenerated.ContentType;
            document.content = stream.ToArray();

            return document;
        }

        private void AddPage(ReportModel rm, ExportFindAndReplaceRequest request, List<DataRow> data)
        {
            GridSectionModel gridSectionModel = new GridSectionModel()
            {
                Style = new GridSectionStyleModel()
                {
                    WithPercentage = 100,

                }
            };

            GridRowModel headerRow = new GridRowModel();
            GridRowModel dataRow = new GridRowModel();
            this.AddHeader(rm,request,data.Count);
            if (data.Count < 1)
            {
                return;
            }
            foreach (var cell in data[0].DataCells)
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
            gridSectionModel.AddRow(headerRow);

            foreach (var row in data)
            {
                dataRow = new();
                foreach (var cell in row.DataCells)
                {
                    dataRow.AddCell(GetDataPhrase(cell.Value));
                }
                gridSectionModel.AddRow(dataRow);

            }
            rm.AddSection(gridSectionModel);
             
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
                    FontIsItalic = true,
                    FontColor = System.Drawing.Color.Black,
                    FontIsBold = true,
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
                        FontName = "Helvetica",
                        FontSize = 8,
                        FontColor = System.Drawing.Color.Black,
                        FontIsBold = true,
                    }
                }
            };
        }

        private void AddHeader(ReportModel model, ExportFindAndReplaceRequest report, int rowsExported)
        {
            // Aggiunta di titolo, sottotitolo e summary
            model.AddSection(this.AddReportTitle(report.request.Title));
            model.AddSection(this.AddReportSubtitle(report.request.SubTitle));
            model.AddSection(this.AddReportAdditionalInformation(report.request.AdditionalInformation));
            model.AddSection(this.AddReportSummary($"Righe estratte: {rowsExported}"));

        }

        private TextSectionModel AddReportTitle(string title)
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
        private TextSectionModel AddReportSubtitle(string subtitle)
        {
            return new TextSectionModel()
            {
                Content = new TextContentModel()
                {
                    Value = subtitle,
                    Style = new TextStyleModel()
                    {
                        FontName = "Helvetica",
                        FontSize = 12,
                        FontIsItalic = true,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }

        private TextSectionModel AddReportAdditionalInformation(string additionalInformation)
        {
            return new TextSectionModel()
            {
                Content = new TextContentModel()
                {
                    Value = additionalInformation,
                    Style = new TextStyleModel()
                    {
                        FontName = "Helvetica",
                        FontSize = 10,
                        FontIsItalic = true,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }
        private TextSectionModel AddReportSummary(string summary)
        {
            return new TextSectionModel()
            {
                Content = new TextContentModel()
                {
                    Value = summary,
                    Style = new TextStyleModel()
                    {
                        FontName = "Helvetica",
                        FontSize = 10,
                        FontIsItalic = true,
                        FontColor = System.Drawing.Color.Black
                    }
                }
            };
        }
    }
}
