// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.PrjDocImport;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2016.Excel;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pi3.App.Legacy.WebApi.Application.Handlers.ReadDocumentDataFromExcelFile.ReadDocumentDataFromExcelFileHandler;
using ReadDocumentDataFromExcelFileRequest = Pi3.App.Legacy.WebApi.Application.Requests.ReadDocumentDataFromExcelFile;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ReadDocumentDataFromExcelFile
{
    public class ReadDocumentDataFromExcelFileHandler : IRequestHandler<ReadDocumentDataFromExcelFileRequest, ReadDocumentDataFromExcelFileResult>
    {
        #region Public Members

        public ReadDocumentDataFromExcelFileHandler(
            ILogger<ReadDocumentDataFromExcelFileHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            ISpreadsheetService spreadsheetService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._spreadsheetService = spreadsheetService;
        }

        public async Task<ReadDocumentDataFromExcelFileResult> Handle(ReadDocumentDataFromExcelFileRequest request, CancellationToken cancellationToken)
        {
            string error = null!;
            var output = new DocumentRowDataContainer();
            
            try
            {
                using var stream = new MemoryStream(request.content);

                var spreadsheetModel = await this._spreadsheetService.Read(stream);

                output.InDocument = await this.FillSheetData(request, spreadsheetModel, "Arrivo");
                output.OutDocument = await this.FillSheetData(request, spreadsheetModel, "Partenza");
                output.OwnDocument = await this.FillSheetData(request, spreadsheetModel, "Interni");
                output.GrayDocument = await this.FillSheetData(request, spreadsheetModel, "Non protocollati");
                if (request.isStampaUnione)
                    output.AttachmentDocument = await this.FillSheetData(request, spreadsheetModel, "Allegati");
                else
                    output.AttachmentDocument = new();
            }
            catch (Pi3Exception pi3Ex)
            {
                output = null!;
                error = pi3Ex.Message;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                output = null!;
                error = ex.Message;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new ReadDocumentDataFromExcelFileResult(output, error);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ReadDocumentDataFromExcelFileHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly ISpreadsheetService _spreadsheetService;

        internal class NomiColonne
        {
            public const string Ordinale = "Ordinale";
            public const string CodiceAmministrazione = "Codice Amministrazione";
            public const string CodiceRegistro = "Codice Registro";
            public const string CodiceRF = "Codice RF";
            public const string CodiceOggetto = "Codice Oggetto";
            public const string Oggetto = "Oggetto";
            public const string CodiceCorrispondenti = "Codice Corrispondenti";
            public const string Corrispondenti = "Corrispondenti";
            public const string CodiceCorrispondente = "Codice Corrispondente";
            public const string Corrispondente = "Corrispondente";
            public const string PathName = "Pathname";
            public const string ADL = "ADL";
            public const string Note = "Note";
            public const string CodiceModelloTrasmissione = "Codice Modello Trasmissione";
            public const string TipologiaDocumento = "Tipologia Documento";
            public const string Dcampo = "Dcampo";
            public const string CodiceFascicolo = "Codice Fascicolo";
            public const string DescrizioneFascicolo = "Descrizione Fascicolo";
            public const string Descrizione = "Descrizione";
            public const string DescrizioneSottofascicolo = "Descrizione Sottofascicolo";
            public const string Titolario = "Titolario";
            public const string CodiceNodo = "Codice Nodo";
            public const string TipologiaFascicolo = "Tipologia Fascicolo";
            public const string Fcampo = "Fcampo";
            public const string Predisposto = "Predisposto";
            public const string NumeroDiProtocollo = "Numero di protocollo";
            public const string DataProtocollo = "Data protocollo";
            public const string CodiceUtenteCreatore = "Codice Utente Creatore";
            public const string CodiceRuoloCreatore = "Codice Ruolo Creatore";
            public const string IdDocumentoPrincipale = "Id Documento Principale";
            public const string OrdinalePrincipale = "Ordinale Principale";
        }

        protected virtual async Task<List<DocumentRowData>> FillSheetData(
            ReadDocumentDataFromExcelFileRequest request,
            SpreadsheetModel spreadsheetModel,
            string sheetName)
        {
            var sheetModel = spreadsheetModel.Sheets.FirstOrDefault(s =>
                    s.Name.Equals(sheetName, StringComparison.InvariantCultureIgnoreCase));
            if (sheetModel == null)
            {
                throw new ReadDocumentDataFromExcelFilePi3Exception(string.Format(ErrorDescriptions.SheetNonTrovato, sheetName));
            }

            var output = new List<DocumentRowData>();

            // Estrazione indici colonne / nomi
            var columns = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
            sheetModel.Cells.Where(c => c.Row == 0)
                .ForEach(c =>
                {
                    if (c != null && !string.IsNullOrEmpty(c.ValueAsString))
                        columns.Add(c.ValueAsString, c.Column);
                });

            for (int rowIndex = 1; rowIndex <= sheetModel.Cells.Max(c => c.Row); rowIndex++)
            {
                var cellsPerRow = sheetModel.Cells.Where(c => c.Row == rowIndex).ToList();
                var cellsFilledPerRow = cellsPerRow.Where(c => c.ValueAsString != null).Count();
                if (cellsFilledPerRow == 0)
                    continue;
                var rowToReturn = await this.FillDocumentRowData(request, columns, cellsPerRow, sheetName.Equals("Allegati"), sheetName.Equals("Arrivo"));

                output.Add(rowToReturn);
            }

            return output;
        }

        protected virtual async Task<DocumentRowData> FillDocumentRowData(
            ReadDocumentDataFromExcelFileRequest request,
            Dictionary<string, int> columns,
            List<CellModel> cellsPerRow,
            bool isAttachmentSheet,
            bool isArrivoSheet)
        {
            var rowToReturn = new DocumentRowData();

            if (request.isEnabledPregressi)
            {
                rowToReturn.ProtocolNumber = cellsPerRow.GetCellValueAsString(columns, NomiColonne.NumeroDiProtocollo);
                rowToReturn.ProtocolDate = string.IsNullOrWhiteSpace(cellsPerRow.GetCellValueAsString(columns, NomiColonne.DataProtocollo))
                                        ? DateTime.Now
                                        : cellsPerRow.GetCellValueAsString(columns, NomiColonne.DataProtocollo).AsDateTime();
                rowToReturn.CodiceUtenteCreatore = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceUtenteCreatore);
                rowToReturn.CodiceRuoloCreatore = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceRuoloCreatore);
            }
            else if (columns.ContainsKey(NomiColonne.NumeroDiProtocollo)
                    || columns.ContainsKey(NomiColonne.DataProtocollo)
                    || columns.ContainsKey(NomiColonne.CodiceUtenteCreatore)
                    || columns.ContainsKey(NomiColonne.CodiceRuoloCreatore))
            {
                throw new ReadDocumentDataFromExcelFilePi3Exception(ErrorDescriptions.RuoloNonAbilitato);
            }
            rowToReturn.MainDocumentId = cellsPerRow.GetCellValueAsString(columns, NomiColonne.IdDocumentoPrincipale);
            rowToReturn.MainOrdinal = cellsPerRow.GetCellValueAsString(columns, NomiColonne.OrdinalePrincipale);
            rowToReturn.OrdinalNumber = cellsPerRow.GetCellValueAsString(columns, NomiColonne.Ordinale);
            rowToReturn.AdminCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceAmministrazione);
            rowToReturn.RegCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceRegistro);
            rowToReturn.RFCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceRF);
            rowToReturn.ObjCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceOggetto);
            rowToReturn.Obj = cellsPerRow.GetCellValueAsString(columns, NomiColonne.Oggetto);
            if (isAttachmentSheet)
            {
                rowToReturn.Obj = cellsPerRow.GetCellValueAsString(columns, NomiColonne.Descrizione);
            }
            rowToReturn.CorrCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceCorrispondenti)
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim())
                .ToList();
            rowToReturn.CorrDesc = cellsPerRow.GetCellValueAsString(columns, NomiColonne.Corrispondenti)
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim())
                .ToList();

            if (isArrivoSheet)
            {
                rowToReturn.CorrCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceCorrispondente)
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim())
                .ToList();
                rowToReturn.CorrDesc = cellsPerRow.GetCellValueAsString(columns, NomiColonne.Corrispondente)
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => v.Trim())
                    .ToList();
            }
            rowToReturn.Pathname = cellsPerRow.GetCellValueAsString(columns, NomiColonne.PathName);
            rowToReturn.InWorkingArea = cellsPerRow.GetCellValueAsString(columns, NomiColonne.ADL).ToUpperInvariant() == "SI";
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
                        IdRuolo = request.role.systemId,
                        DescrizioneRuolo = request.role.descrizione,
                    },
                    DaInserire = true,
                    DataCreazione = DateTime.Now
                };
            }
            rowToReturn.TransmissionModelCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceModelloTrasmissione)
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim())
                .ToArray();
            rowToReturn.DocumentTipology = cellsPerRow.GetCellValueAsString(columns, NomiColonne.TipologiaDocumento).ToUpperInvariant();
            rowToReturn.ProjectCodes = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceFascicolo)
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim())
                .ToArray();
            rowToReturn.ProjectDescription = cellsPerRow.GetCellValueAsString(columns, NomiColonne.DescrizioneFascicolo);
            rowToReturn.FolderDescrition = cellsPerRow.GetCellValueAsString(columns, NomiColonne.DescrizioneSottofascicolo);
            rowToReturn.Titolario = cellsPerRow.GetCellValueAsString(columns, NomiColonne.Titolario);
            rowToReturn.NodeCode = cellsPerRow.GetCellValueAsString(columns, NomiColonne.CodiceNodo);
            rowToReturn.ProjectTipology = cellsPerRow.GetCellValueAsString(columns, NomiColonne.TipologiaFascicolo).ToUpperInvariant();
            rowToReturn.Predisposto = cellsPerRow.GetCellValueAsString(columns, NomiColonne.Predisposto).ToUpperInvariant() == "SI";

            columns.Keys
                .Where(k => k.StartsWith(NomiColonne.Dcampo))
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
                                throw new ReadDocumentDataFromExcelFilePi3Exception(
                                String.Format(ErrorDescriptions.ErroreInEstrazioneContenutoCella,
                                    f,
                                    cellsPerRow.GetCellValueAsString(columns, NomiColonne.Ordinale),
                                    fieldInformation[0]));

                            throw new ReadDocumentDataFromExcelFilePi3Exception(
                                String.Format(ErrorDescriptions.CampoProfilatoNonValorizzatoCorrettamente,
                                    f,
                                    cellsPerRow.GetCellValueAsString(columns, NomiColonne.Ordinale)));
                        }

                        // ...si aggiunge al dizionario dei campi profilati la chiave
                        // ed i valori
                        rowToReturn.AddDocumentProfilationField(
                            fieldInformation[0],
                            fieldInformation[1].Split(';'));
                    }
                });

            return rowToReturn;
        }

        #endregion
    }

    internal static class CellModelExtensions
    {
        public static string GetCellValueAsString(
            this IEnumerable<CellModel> cellsPerRow, 
            Dictionary<string, int> columns, 
            string nomeColonna)
        {
            string value = null!;

            if (columns.ContainsKey(nomeColonna))
            {
                var cell = cellsPerRow.FirstOrDefault(c => c.Column == columns[nomeColonna]);
                if (cell != null)
                {
                    value = (cell?.ValueAsString! ?? String.Empty).Trim();
                }
            }

            return value! ?? String.Empty;
        }
    }

}