// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.PrjDocImport;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadRDEDataFromExcelequest = Pi3.App.Legacy.WebApi.Application.Requests.ReadRDEDataFromExcel;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ReadRDEDataFromExcel
{
    public class ReadRDEDataFromExcelHandler : IRequestHandler<ReadRDEDataFromExcelequest, ReadRDEDataFromExcelResult>
    {
        #region Public Members

        public ReadRDEDataFromExcelHandler(ILogger<ReadRDEDataFromExcelHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            ISpreadsheetService spreadsheetService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._spreadsheetService = spreadsheetService;
        }

        public async Task<ReadRDEDataFromExcelResult> Handle(ReadRDEDataFromExcelequest request, CancellationToken cancellationToken)
        {
            var documentRowDataContainer = new DocumentRowDataContainer();
            var error = string.Empty;

            try
            {
                var spreadsheetModel = await _spreadsheetService.Read(new MemoryStream(request.content));

                var sheetModel = spreadsheetModel.Sheets.FirstOrDefault(s => s.Name.Equals("RDE", StringComparison.InvariantCultureIgnoreCase));
                if (sheetModel == null)
                    throw new SheetNotFoundPi3Exception();

                documentRowDataContainer = await ReadDataFromExcel(sheetModel, request.versionNumber);         
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                error = pi3Ex.Message;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                error = ex.Message;
            }

            return new ReadRDEDataFromExcelResult(documentRowDataContainer, error);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ReadRDEDataFromExcelHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly ISpreadsheetService _spreadsheetService;

        protected virtual async Task<DocumentRowDataContainer> ReadDataFromExcel(SheetModel sheetModel, int versionNumber)
        {
            var documentRowDataContainer = new DocumentRowDataContainer()
            {
                InDocument = new List<DocumentRowData>(),
                OwnDocument = new List<DocumentRowData>(),
                OutDocument = new List<DocumentRowData>()
            };

            var columns = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            sheetModel.Cells.Where(c => c.Row == 0).ForEach(c => columns.Add(c.ValueAsString, c.Column));

            var totalRows = sheetModel.Cells.Max(c => c.Row);
            for (int rowIndex = 1; rowIndex <= totalRows; rowIndex++)
            {
                var row = sheetModel.Cells.Where(c => c.Row == rowIndex).ToList();
                var tipoProto = row.GetCellValueAsString(columns[Resources.ColumnTipoProtocollo]);
                switch (tipoProto.ToUpper())
                {
                    case "A":
                        documentRowDataContainer.InDocument.Add(await ReadDocumentData(row, columns, versionNumber));
                        break;
                    case "P":
                        documentRowDataContainer.OutDocument.Add(await ReadDocumentData(row, columns, versionNumber));
                        break;
                }

            }

            return documentRowDataContainer;
        }

        protected virtual async Task<DocumentRowData> ReadDocumentData(List<CellModel> row, Dictionary<string, int> columns, int versionNumber)
        {
            var documentRowData = new RDEDocumentRowData();

            var tipoProto = row.GetCellValueAsString(columns[Resources.ColumnTipoProtocollo]);
            var dataProtocolloEmergenza = row.GetCellValueAsString(columns[Resources.ColumnDataProtocolloEmergenza]);
            var dataArrivo = row.GetCellValueAsString(columns[Resources.ColumnDataArrivo]);
            var oraArrivo = row.GetCellValueAsString(columns[Resources.ColumnOraArrivo]);

            documentRowData.AdminCode = row.GetCellValueAsString(columns[Resources.ColumnCodiceAmministrazione]);
            documentRowData.EmergencyProtocolDate = !string.IsNullOrEmpty(dataProtocolloEmergenza) ? dataProtocolloEmergenza.AsDateTime().AsDateFormat() : string.Empty;
            documentRowData.EmergencyProtocolTime = row.GetCellValueAsString(columns[Resources.ColumnOraProtocolloEmergenza]);
            documentRowData.OrdinalNumber = row.GetCellValueAsString(columns[Resources.ColumnNumeroProtocolloEmergenza]);
            documentRowData.EmergencyProtocolSignature = row.GetCellValueAsString(columns[Resources.ColumnStringaProtocolloEmergenza]);
            documentRowData.Obj = row.GetCellValueAsString(columns[Resources.ColumnOggetto]);
            documentRowData.SenderProtocolDate = row.GetCellValueAsString(columns[Resources.ColumnDataProtocolloMittente]);
            documentRowData.SenderProtocolNumber = row.GetCellValueAsString(columns[Resources.ColumnNumeroProtocolloMittente]);
            documentRowData.ArrivalDate = !string.IsNullOrEmpty(dataArrivo) ? dataArrivo.AsDateTime().AsDateFormat() : string.Empty;

            if (!string.IsNullOrEmpty(documentRowData.ArrivalDate))
            {
                if (!string.IsNullOrEmpty(row.GetCellValueAsString(columns[Resources.ColumnOraArrivo])))
                {
                     documentRowData.ArrivalTime = row.GetCellValueAsString(columns[Resources.ColumnOraArrivo]);
                }
            }

            documentRowData.ProjectCodes = !string.IsNullOrEmpty(row.GetCellValueAsString(columns[Resources.ColumnCodiceClassifica])) ? row.GetCellValueAsString(columns[Resources.ColumnCodiceClassifica]).Split(';') : null;
            documentRowData.RFCode = row.GetCellValueAsString(columns[Resources.ColumnCodiceRF]);
            documentRowData.RegCode = row.GetCellValueAsString(columns[Resources.ColumnCodiceRegistro]);

            var mittente = row.GetCellValueAsString(columns[Resources.ColumnMittente]);
            switch (tipoProto.ToUpper())
            {
                case "A":
                    documentRowData.CorrDesc = !string.IsNullOrEmpty(mittente) ? new List<string>(mittente.Trim().Split(';')) : null;
                    break;
                case "P":
                case "I":
                    documentRowData.CorrDesc = new List<string>();
                    if (!string.IsNullOrEmpty(mittente))
                    {
                        mittente.ToString().Trim().Split(';').ForEach(m => documentRowData.CorrDesc.Add(m.Trim() + "#M#"));
                    }

                    var destinatari = row.GetCellValueAsString(columns[Resources.ColumnDestinatari]);
                    if (!string.IsNullOrEmpty(destinatari))
                    {
                        destinatari.ToString().Trim().Split(';').ForEach(d => documentRowData.CorrDesc.Add(d.Trim() + "#D#"));
                    }

                    var destinatariCC = row.GetCellValueAsString(columns[Resources.ColumnDestinatariCC]);
                    if (!string.IsNullOrEmpty(destinatari))
                    {
                        destinatari.ToString().Trim().Split(';').ForEach(d => documentRowData.CorrDesc.Add(d.Trim() + "#CC#"));
                    }
                    break;
            }

            return documentRowData;
        }

        #endregion
    }

    internal static class CellModelExtensions
    {
        public static string GetCellValueAsString(this IEnumerable<CellModel> cellsPerRow, int column)
        {
            string value = null!;

            var cell = cellsPerRow.FirstOrDefault(c => c.Column == column);
            if (cell != null)
            {
                value = (cell?.ValueAsString! ?? String.Empty).Trim();
            }

            return value! ?? String.Empty;
        }
    }
}
