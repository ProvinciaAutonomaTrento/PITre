// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExportDistributionListRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExportDistributionList;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExportDistributionList
{
    public class ExportDistributionListHandler : IRequestHandler<ExportDistributionListRequest, ExportDistributionListResult>
    {
        #region Public members
        public ExportDistributionListHandler(ILogger<ExportDistributionListHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IPi3DbContext dbContext, IReportGeneratorService reportGeneratorService, IFileConverterService converterService, ISpreadsheetService spreadsheetService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._reportGeneratorService = reportGeneratorService;
            this._converterService = converterService;
            this._spreadsheetService = spreadsheetService;
        }
        public async Task<ExportDistributionListResult> Handle(ExportDistributionListRequest request, CancellationToken cancellationToken)
        {
            FileDocumento output = null;

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var groupCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode );
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            // Estrazione dati
            var reportData = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(this._dbContext.ListeDistrEntities.AsNoTracking(), cl => cl.SYSTEM_ID, l => l.ID_LISTA_DPA_CORR, (cl, l) => new { cl, l })
                .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(), a => a.l.ID_DPA_CORR, cg => cg.SYSTEM_ID, (a, cg) => new { a.cl, a.l, cg })
                .Join(this._dbContext.AmministraEntities.AsNoTracking(), x => x.cl.ID_AMM, a => a.SYSTEM_ID, (x, a) => new { x.cl, x.l, x.cg, a })
                .Where(x => x.cl.SYSTEM_ID == request.selectedListId.AsLong())
                .Select(x => new ReportItem
                {
                    Action = "I",
                    User = userId,
                    Role = groupCode,
                    TenantCode = x.a.VAR_CODICE_AMM,
                    //RFCode,
                    Permission = x.cl.ID_PEOPLE_LISTE.HasValue ? "UTENTE": "RUOLO",
                    ListCode = x.cl.VAR_COD_RUBRICA,
                    ListName = x.cl.VAR_DESC_CORR,
                    CorrespondentCode = x.cg.VAR_COD_RUBRICA,
                    Correspondent = x.cg.VAR_DESC_CORR,
                    AddressBook = x.cg.CHA_TIPO_CORR == "C" ? "RC" : null,
                    RegisterId = x.cg.ID_REGISTRO
                })
                .ToListAsync();

            var rfList = await this._dbContext.RegistroEntities.AsNoTracking()
                .Join(this._dbContext.RuoloRegistroEntities.AsNoTracking(), r => r.SYSTEM_ID, l => l.ID_REGISTRO, (r, l) => new { r, l })
                .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(), a => a.l.ID_RUOLO_IN_UO, c => c.SYSTEM_ID, (a, c) => new { a.r, a.l, c })
                .Where(x => x.r.CHA_RF == "1"
                && x.c.ID_GRUPPO == idGroup
                && x.r.ID_AMM == idTenant)
                .Select(x => x.r.VAR_CODICE)
                .ToListAsync();

            var joinedRfList = rfList is not null && rfList.Any() ?
                string.Join(";", rfList) :
                string.Empty;

            foreach (var x in reportData)
            {
                x.AddressBook ??= (await this._dbContext.RegistroEntities.FirstOrDefaultAsync(r => r.SYSTEM_ID == x.RegisterId))?.VAR_CODICE ?? string.Empty;
                x.RFCode = joinedRfList;
            }

            byte[]? content = default;

            switch(request.tipologia.ToUpper())
            {
                case "PDF":
                    content = await this.GetReportContentPDF(reportData);
                    output = new FileDocumento
                    {
                        content = content,
                        contentType = "application/pdf",
                        estensioneFile = "PDF",
                        length = content.Length,
                        name = string.Format(Resources.ReportFileName, "pdf")
                    };
                    break;

                case "XLS":
                case "ODS":
                    content = await this.GetReportContentSpreadsheet(reportData);
                    output = new FileDocumento
                    {
                        content = content,
                        contentType = "",
                        estensioneFile = "XLSX",
                        length = content.Length,
                        name = string.Format(Resources.ReportFileName, "xlsx")
                    };
                    break;

                default:
                    break;
            }

            return new ExportDistributionListResult(output);
        }
        #endregion

        #region Private members
        protected ILogger<ExportDistributionListHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        protected IReportGeneratorService _reportGeneratorService;
        protected IFileConverterService _converterService;
        protected ISpreadsheetService _spreadsheetService;

        protected async Task<byte[]> GetReportContentPDF(List<ReportItem> items)
        {
            byte[]? content = null;

            var report = new ReportModel
            {
                Size = PageSizes.A4,
                Orientation = PageOrientations.Landscape,
                OutputType = ReportOutputTypes.AsPdf
            };

            var grid = new GridSectionModel
            {
                Style = new GridSectionStyleModel { WithPercentage = 100 }
            };

            // Header
            var header = new GridRowModel();
            header.AddCell(cell: new GridCellModel { Style = this.HeaderCellStyle(6), Content = new TextContentModel { Style = this.HeaderTextStyle, Value = Resources.HeaderAction } });
            header.AddCell(cell: new GridCellModel { Style = this.HeaderCellStyle(10), Content = new TextContentModel { Style = this.HeaderTextStyle, Value = Resources.HeaderUser  } });
            header.AddCell(cell: new GridCellModel { Style = this.HeaderCellStyle(10), Content = new TextContentModel { Style = this.HeaderTextStyle, Value = Resources.HeaderRole } });
            header.AddCell(cell: new GridCellModel { Style = this.HeaderCellStyle(6), Content = new TextContentModel { Style = this.HeaderTextStyle, Value = Resources.HeaderAdministraion  } });
            header.AddCell(cell: new GridCellModel { Style = this.HeaderCellStyle(10), Content = new TextContentModel { Style = this.HeaderTextStyle, Value = Resources.HeaderRF  } });
            header.AddCell(cell: new GridCellModel { Style = this.HeaderCellStyle(10), Content = new TextContentModel { Style = this.HeaderTextStyle, Value = Resources.HeaderPermissions  } });
            header.AddCell(cell: new GridCellModel { Style = this.HeaderCellStyle(5), Content = new TextContentModel { Style = this.HeaderTextStyle, Value = Resources.HeaderListCode  } });
            header.AddCell(cell: new GridCellModel { Style = this.HeaderCellStyle(10), Content = new TextContentModel { Style = this.HeaderTextStyle, Value = Resources.HeaderListDescription } });
            header.AddCell(cell: new GridCellModel { Style = this.HeaderCellStyle(8), Content = new TextContentModel { Style = this.HeaderTextStyle, Value = Resources.HeaderCorrespondentCode } });
            header.AddCell(cell: new GridCellModel { Style = this.HeaderCellStyle(12), Content = new TextContentModel { Style = this.HeaderTextStyle, Value = Resources.HeaderCorrespondentName  } });
            header.AddCell(cell: new GridCellModel { Style = this.HeaderCellStyle(10), Content = new TextContentModel { Style = this.HeaderTextStyle, Value = Resources.HeaderAddressBook  } });

            grid.AddRow(header);

            // Dati report
            var properties = typeof(ReportItem).GetProperties().Where(p => p.Name != "RegisterId");

            items.ForEach(item =>
            {
                var row = new GridRowModel();
                properties.ForEach(p =>
                {
                    row.AddCell(cell: new GridCellModel
                    {
                        Style = this.RowCellStyle,
                        Content = new TextContentModel
                        {
                            Style = this.RowTextStyle,
                            Value = p.GetValue(item)?.ToString() ?? string.Empty
                        }
                    });
                });
                grid.AddRow(row);
            });

            report.AddSection(grid);

            using(var stream = new MemoryStream())
            {
                var generatedReport = await this._reportGeneratorService.Generate(report, stream);

                content = stream.ToArray();
            }

            return content;
        }

        protected async Task<byte[]> GetReportContentSpreadsheet(List<ReportItem> items)
        {
            byte[]? content = default;

            var reportModel = new SpreadsheetModel();

            var sheet = new SheetModel { Name = Resources.SheetName };

            // Intestazione
            int i = 0;
            this.Headers.ForEach(x =>
            {
                sheet.AddCell(cellModel: new CellModel
                {
                    Row = 0,
                    Column = i,
                    ValueAsString = x,
                    CellStyle = this.SpreadsheetHeaderCellStyle
                });
                i++;
            });

            // Righe report
            var properties = typeof(ReportItem).GetProperties().Where(p => p.Name != "RegisterId");
            int row = 1;
            items.ForEach(item =>
            {
                int c = 0;
                properties.ForEach(p =>
                {
                    sheet.AddCell(cellModel: new CellModel
                    {
                        Row = row,
                        Column = c,
                        ValueAsString = p.GetValue(item)?.ToString() ?? string.Empty,
                        CellStyle = this.SpreadsheetRowCellStyle
                    });
                    c++;
                });
                row++;
            });

            reportModel.AddSheet(sheet);

            using (var stream = new MemoryStream())
            {
                var generatedReport = await this._spreadsheetService.Write(reportModel, stream);

                content = stream.ToArray();
            }

            return content;
        }

        public class ReportItem
        {
            public string Action { get; set; }

            public string? User { get; set; }

            public string? Role { get; set; }

            public string? TenantCode { get; set; }

            public string? RFCode { get; set; }

            public string? Permission { get; set; }

            public string? ListCode { get; set; }

            public string? ListName { get; set; }

            public string? CorrespondentCode { get; set; }

            public string? Correspondent { get; set; }

            public string? AddressBook { get; set; }

            public long? RegisterId { get; init; }
        }

        private IReadOnlyList<string> Headers = new List<string>
        {
            Resources.HeaderAction,
            Resources.HeaderUser,
            Resources.HeaderRole,
            Resources.HeaderAdministraion,
            Resources.HeaderRF,
            Resources.HeaderPermissions,
            Resources.HeaderListCode,
            Resources.HeaderListDescription,
            Resources.HeaderCorrespondentCode,
            Resources.HeaderCorrespondentName,
            Resources.HeaderAddressBook
        };

        #region Styles

        private GridCellStyleModel HeaderCellStyle(int percentage)
            => new GridCellStyleModel
            {
                Justification = Justifications.Center,
                VerticalAlignment = VerticalAlignments.Center,
                WithPercentage = percentage,
                ForegroundColor = System.Drawing.Color.Silver
            };

        private TextStyleModel HeaderTextStyle = new TextStyleModel
        {
            FontName = "ARIAL",
            FontSize = 8,
            FontIsBold = true
        };

        private GridCellStyleModel RowCellStyle = new GridCellStyleModel
        {
            Justification = Justifications.Center,
            VerticalAlignment = VerticalAlignments.Top
        };

        private TextStyleModel RowTextStyle = new TextStyleModel
        {
            FontName = "HELVETICA",
            FontSize = 8,
            FontIsBold = false
        };

        private CellStyleModel SpreadsheetHeaderCellStyle = new CellStyleModel
        {
            FontName = "ARIAL",
            FontSize = 8,
            FontIsBold = true,
            ForegroundColor = System.Drawing.Color.Silver
        };

        private CellStyleModel SpreadsheetRowCellStyle = new CellStyleModel
        {
            FontName = "Arial",
            FontSize = 8,
            FontIsBold = false
        };

        #endregion

        #endregion
    }
}
