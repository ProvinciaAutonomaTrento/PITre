// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Note;
using DocsPaVO.PrjDocImport;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.ImportProject;
using Pi3.App.Legacy.WebApi.Application.Handlers.ReadDocumentDataFromExcelFile;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadProjectDataFromExcelRequest = Pi3.App.Legacy.WebApi.Application.Requests.ReadProjectDataFromExcel;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ReadProjectDataFromExcel
{
    public class ReadProjectDataFromExcelHandler : IRequestHandler<ReadProjectDataFromExcelRequest, ReadProjectDataFromExcelResult>
    {
        #region Public Members

        public ReadProjectDataFromExcelHandler(
            ILogger<ReadProjectDataFromExcelHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            ISpreadsheetService spreadsheetService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._spreadsheetService = spreadsheetService;
        }

        public async Task<ReadProjectDataFromExcelResult> Handle(ReadProjectDataFromExcelRequest request, CancellationToken cancellationToken)
        {
            byte[] content = request.content;
            string fileName = request.fileName;
            DocsPaVO.utente.InfoUtente userInfo = request.userInfo;
            DocsPaVO.utente.Ruolo role = request.role;
            List<ProjectRowData> toReturn = new List<ProjectRowData>();

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            //var idCorrGlobaliPeople = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_PEOPLE == idUser).Select(c => c.SYSTEM_ID).FirstAsync();

            try
            {
                var model = await this._spreadsheetService.Read(new MemoryStream(content));

                var sheetModel = model.Sheets.FirstOrDefault(sheet => sheet.Name.ToUpper().Equals("FASCICOLI"));

                if (sheetModel == null)
                    throw new SheetNotFoundPi3Exception();

                // Se non ci sono fascicoli da importare, il report conterrà
                // una sola riga con un messaggio che informi l'utente
                var rowsToRead = sheetModel.Cells.Where(c => c.Column == 0 && c.Row != 0 && c.ValueAsString != null).Select(c => c.Row);
                if (rowsToRead != null && !rowsToRead.Any())
                    throw new ReadProjectDataFromExcelFilePi3Exception(Resources.NoProjectsToImport);

                var columns = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
                sheetModel.Cells.Where(c => c.Row == 0 && c.ValueAsString != null)
                    .ForEach(c => columns.Add(c.ValueAsString, c.Column));

                for (int rowIndex = 1; rowIndex <= sheetModel.Cells.Max(c => c.Row); rowIndex++)
                {
                    var cellsPerRow = sheetModel.Cells.Where(c => c.Row == rowIndex).ToList();

                    var rowToReturn = await this.FillDocumentRowData(request, columns, cellsPerRow);

                    toReturn.Add(rowToReturn);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null);
            }

            return new ReadProjectDataFromExcelResult(toReturn);
        }

        private async Task<ProjectRowData> FillDocumentRowData(ReadProjectDataFromExcelRequest request, Dictionary<string, int> columns, List<CellModel> cellsPerRow)
        {
            var rowToReturn = new ProjectRowData()
            {
                OrdinalNumber = cellsPerRow.GetCellValueAsString(columns, NomiColonne.Ordinale),
                AdminCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceAmministrazione),
                RegistryCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceRegistro),
                RFCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceRF),
                Description = cellsPerRow.GetCellValueAsString(columns, NomiColonne.Descrizione),
                NodeCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceNodo),
                ProjectNumber = cellsPerRow.GetCellValueAsString(columns, NomiColonne.NumeroFascicolo),
                ProjectTipology = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CampiProfilatiFascicolo),
                TipologiaFascicolo = cellsPerRow.GetCellValueAsString(columns, NomiColonne.TipoFascSerie),
                TransmissionModelCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceModelloTrasmissione)
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => v.Trim()).ToArray()
            };

            var inWorkingArea = cellsPerRow.GetCellValueAsString(columns, NomiColonne.ADL);
            rowToReturn.InWorkingArea = !String.IsNullOrEmpty(inWorkingArea) && inWorkingArea.ToUpper().Trim() == "SI";

            var note = cellsPerRow.GetCellValueAsString(columns, NomiColonne.Note);
            if (!string.IsNullOrWhiteSpace(note))
            {
                rowToReturn.Note = new DocsPaVO.Note.InfoNota(note)
                {
                    TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti,
                    UtenteCreatore = new DocsPaVO.Note.InfoUtenteCreatoreNota()
                    {
                        IdUtente = request.userInfo.idPeople,
                        DescrizioneUtente = request.userInfo.userId,
                        IdRuolo = request.role.idGruppo,
                        DescrizioneRuolo = request.role.descrizione,
                    },
                    DaInserire = true,
                    DataCreazione = DateTime.Now
                };
            }

            columns.Keys
                .Where(k => k.StartsWith(NomiColonne.Campo1))
                .Select(k => k)
                .ForEach(f =>
                {
                    var fieldValue = cellsPerRow.GetCellValueAsString(columns, f);

                    if (!string.IsNullOrWhiteSpace(fieldValue))
                    {
                        // ...si spezza la stringa nelle sue due componenti
                        string[] fieldInformation = fieldValue.Split('=', StringSplitOptions.RemoveEmptyEntries);

                        // ...se fieldInformation non contiene due valori, eccezione
                        if (fieldInformation.Length != 2)
                        {
                            // Nel caso in cui l'array degli argomenti contiene un solo elemento
                            // che comincia con #Error, molto probabilmente si tratta di un errore
                            // di lettura restituito dal driver Excel. In questo caso viene lanciata un'eccezione
                            // appositamente costruita (per sicurezza viene anche inviata la stringa completa
                            // in quanto potrebbe capitare che in certi casi sia riportato anche un numero identificativo
                            // dell'errore
                            if (fieldInformation.Length == 1 && fieldInformation[0].StartsWith("#Error"))
                                throw new ReadProjectDataFromExcelFilePi3Exception(
                                String.Format(ErrorDescriptions.ErroreInEstrazioneContenutoCella,
                                    f,
                                    cellsPerRow.GetCellValueAsString(columns, NomiColonne.Ordinale),
                                    fieldInformation[0]));

                            throw new ReadProjectDataFromExcelFilePi3Exception(
                                String.Format(ErrorDescriptions.CampoProfilatoNonValorizzatoCorrettamente,
                                    f,
                                    cellsPerRow.GetCellValueAsString(columns, NomiColonne.Ordinale)));
                        }

                        // ...si aggiunge al dizionario dei campi profilati la chiave
                        // ed i valori
                        rowToReturn.AddProfilationField(
                            fieldInformation[0],
                            fieldInformation[1].Split(';'));
                    }
                });

            return rowToReturn;
        }


        #endregion


        #region Private Members

        protected readonly ILogger<ReadProjectDataFromExcelHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly ISpreadsheetService _spreadsheetService;

        internal class NomiColonne
        {
            public const string Ordinale = "Ordinale";
            public const string CodiceAmministrazione = "Codice Amministrazione";
            public const string CodiceRegistro = "Codice Registro";
            public const string CodiceRF = "Codice RF";
            public const string Descrizione = "Descrizione";
            public const string CodiceNodo = "Codice nodo";
            public const string NumeroFascicolo = "Numero Fascicolo";
            public const string TipoFascSerie = "Tipologia fascicolo/serie";
            public const string DataCreazione = "Data creazione";
            public const string ADL = "ADL";
            public const string Note = "Note";
            public const string CodiceModelloTrasmissione = "Codice Modello Trasmissione";
            public const string CampiProfilatiFascicolo = "Campi profilati fascicolo";
            public const string Campo1 = "Campo";
        }

        #endregion

    }
}
