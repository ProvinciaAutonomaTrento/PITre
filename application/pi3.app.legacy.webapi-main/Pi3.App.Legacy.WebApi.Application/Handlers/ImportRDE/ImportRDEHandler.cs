// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;
using DocsPaVO.Mobile;
using DocsPaVO.PrjDocImport;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Pi3.App.Legacy.WebApi.Application.Handlers.ReadDocumentDataFromExcelFile;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Infrastructure.EF.Extensions;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImportRDERequest = Pi3.App.Legacy.WebApi.Application.Requests.ImportRDE;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ImportRDE
{
    public class ImportRDEHandler : IRequestHandler<ImportRDERequest, ImportRDEResult>
    {
        #region Public Members

        public ImportRDEHandler(ILogger<ImportRDEHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            ISpreadsheetService spreadsheetService,
            IConfigurationService configurationService,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._spreadsheetService = spreadsheetService;
            this._configurationService = configurationService;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<ImportRDEResult> Handle(ImportRDERequest request, CancellationToken cancellationToken)
        {
            var output = new ResultsContainer();

            try
            {
                var spreadsheetModel = await _spreadsheetService.Read(new MemoryStream(request.content));

                var sheetModel = spreadsheetModel.Sheets.FirstOrDefault(s => s.Name.Equals("RDE", StringComparison.InvariantCultureIgnoreCase));
                if (sheetModel == null)
                    throw new SheetNotFoundPi3Exception();

                var documentRowDataContainer = await ReadDataFromExcel(sheetModel, request.versionNumber);

                //Creazione protocolli in ARRIVO
                if (request.role.funzioni.Where(e => e.codice == "DO_NUOVOPROT").FirstOrDefault() != null &&
                   request.role.funzioni.Where(e => e.codice == "PROTO_IN").FirstOrDefault() != null)
                {
                    output.InDocument = await ImportDocuments(documentRowDataContainer.InDocument, request.userInfo, request.role, request.isRapidClassificationRequired, "A");
                }
                else
                {
                    output.InDocument.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = Resources.RuoloNonAbilitatoProtocolliArrivo
                    });
                }

                //Creazione protocolli in PARTENZA
                if (request.role.funzioni.Where(e => e.codice == "DO_NUOVOPROT").FirstOrDefault() != null &&
                    request.role.funzioni.Where(e => e.codice == "PROTO_OUT").FirstOrDefault() != null)
                {
                    output.OutDocument = await ImportDocuments(documentRowDataContainer.OutDocument, request.userInfo, request.role, request.isRapidClassificationRequired, "P");
                }
                else
                {
                    output.OutDocument.Add(new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = Resources.RuoloNonAbilitatoProtocolliPartenza
                    });
                }

                //Creazione protocolli in INTERNO
                //if (request.role.funzioni.Where(e => e.codice == "DO_NUOVOPROT").FirstOrDefault() != null &&
                //    request.role.funzioni.Where(e => e.codice == "PROTO_OWN").FirstOrDefault() != null)
                //{
                //    output.OwnDocument = await ImportDocuments(documentRowDataContainer.OwnDocument, request.userInfo, request.role, request.isRapidClassificationRequired, "I");
                //}
                //else
                //{
                //    output.OwnDocument.Add(new ImportResult()
                //    {
                //        Outcome = ImportResult.OutcomeEnumeration.KO,
                //        Message = Resources.RuoloNonAbilitatoProtocolliInterni
                //    });
                //}
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);

                output = new ResultsContainer()
                {
                    General = new List<ImportResult>()
                    {
                        new ImportResult()
                        {
                            Message = ex.Message,
                            Outcome = ImportResult.OutcomeEnumeration.KO
                        }
                    }
                };
            }

            return new ImportRDEResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ImportRDEHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly ISpreadsheetService _spreadsheetService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

        protected virtual async Task<List<ImportResult>> ImportDocuments(List<DocumentRowData> documentRowData, InfoUtente infoUtente, Ruolo ruolo, bool isRapidClassificationRequired, string tipoProto)
        {
            var importResults = new List<ImportResult>();
            ImportResult importResult = null;
            var importedDocuments = 0;
            var notImportedDocuments = 0;

            if (documentRowData.Count == 0)
            {
                importResults.Add(new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = Resources.DocumentiNotFound
                });
            }

            foreach (var documentData in documentRowData)
            {
                importResult = await ImportDocument(documentData, infoUtente, ruolo, isRapidClassificationRequired, tipoProto);
                notImportedDocuments = importResult.Outcome == ImportResult.OutcomeEnumeration.KO ? notImportedDocuments + 1 : notImportedDocuments;
                importedDocuments = importResult.Outcome == ImportResult.OutcomeEnumeration.OK ? importedDocuments + 1 : importedDocuments;

                importResults.Add(importResult);
            }

            // Aggiunta di un ultimo risultato con il numero di documenti importati e non importati
            importResults.Add(new ImportResult()
            {
                Outcome = ImportResult.OutcomeEnumeration.OK,
                Message = string.Format(Resources.ResultDocumentiImportatiNonImportati, importedDocuments, notImportedDocuments)
            });

            return importResults;
        }

        protected virtual async Task<ImportResult> ImportDocument(DocumentRowData documentRowData, InfoUtente infoUtente, Ruolo ruolo, bool isRapidClassificationRequired, string tipoProto)
        {
            var importResult = new ImportResult();
            List<string> creationProblems = new List<string>();
            var documentRowDataRDE = documentRowData as RDEDocumentRowData;

            try
            {
                creationProblems = await CheckDataValidity(documentRowData, isRapidClassificationRequired, tipoProto);
                if (creationProblems.Any())
                    throw new InvalidParametersPi3Exception();

                //Se tutti i campi sono validi si procede con la creazione del protocollo
                var idTenant = await _dbContext.AmministraEntities.AsNoTracking()
                    .Where(a => a.VAR_CODICE_AMM.ToUpper().Equals(documentRowData.AdminCode.ToUpper()))
                    .Select(a => a.SYSTEM_ID)
                    .FirstAsync();

                var registroEntity = await _dbContext.RegistroEntities.AsNoTracking()
                    .Where(r => r.ID_AMM == idTenant && r.VAR_CODICE.ToUpper().Equals(documentRowData.RegCode.ToUpper()))
                    .FirstOrDefaultAsync();

                if (registroEntity == null)
                    throw new RegistroNotFoundPi3Exception(documentRowData.RegCode);

                long idRF = 0;
                if (!string.IsNullOrEmpty(documentRowData.RFCode))
                    idRF = await _dbContext.RegistroEntities.AsNoTracking()
                        .Where(r => r.CHA_RF == "1" && r.ID_AMM == idTenant && r.VAR_CODICE.ToUpper().Equals(documentRowData.RFCode.ToUpper()))
                        .Select(r => r.SYSTEM_ID)
                        .FirstOrDefaultAsync();

                var idTitolario = await _dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => p.ID_AMM == idTenant &&
                        p.CHA_STATO != null &&
                        p.CHA_STATO.Equals("A") &&
                        p.ID_TITOLARIO == 0 &&
                        p.VAR_CODICE != null &&
                        p.VAR_CODICE.Equals("T") &&
                        p.ID_PARENT == 0)
                    .Select(p => p.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                //Recupero dei fascicoli
                List<string> projectIds = new List<string>();
                if (documentRowData.ProjectCodes != null && documentRowData.ProjectCodes.Count() > 0)
                {
                    foreach (var codeProject in documentRowData.ProjectCodes)
                    {
                        var fascicolo = (await this._mediator.Send(new Requests.FascicolazioneGetFascicoloDaCodice2(idTenant.ToString(), 
                            ruolo.idGruppo, infoUtente.idPeople, codeProject, 
                            new Registro()
                            {
                                systemId = registroEntity.SYSTEM_ID.ToString(),
                                descrizione = registroEntity.VAR_DESC_REGISTRO,
                                idAmministrazione= registroEntity.ID_AMM.ToString(),
                                codice = registroEntity.VAR_CODICE
                            }, 
                            true, true,
                            idTitolario.ToString())))
                            .output;

                        if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
                        {
                            projectIds.Add(fascicolo.systemID);
                        }
                        else
                        {
                            creationProblems.Add(string.Format(Resources.FascicoloNotFound, codeProject));
                        }
                    }
                }

                if (projectIds.Count == 0 && isRapidClassificationRequired)
                    throw new FascicoloNotFoundPi3Exception();

                var documentoAmministrativoAggregate = new DocumentoAmministrativo(idTenant.ToString(), DateTime.Now,
                    new OggettoDelDocumento()
                    {
                        Descrizione = new TextValue(documentRowData.Obj)
                    },
                    new DatiRegistro()
                    {
                        IdRegistro = registroEntity.SYSTEM_ID.ToString()
                    },
                    tipoProto.AsTipologiaFlusso(),
                    TipologieVisibilitaEnum.Gerarchica);

                documentoAmministrativoAggregate.AssignProtocolloEmergenza(new ProtocolloEmergenza()
                {
                    Data = (documentRowDataRDE.EmergencyProtocolDate + " " + documentRowDataRDE.EmergencyProtocolTime).AsDateTime(),
                    Segnatura = documentRowDataRDE.EmergencyProtocolSignature
                });

                if (tipoProto.ToUpper() == "A")
                {
                    documentoAmministrativoAggregate.AssignProtocolloMittente(new ProtocolloMittente()
                    {
                        Data = !string.IsNullOrEmpty(documentRowDataRDE.SenderProtocolDate) ? documentRowDataRDE.SenderProtocolDate.AsDateTime() : null,
                        Segnatura = !string.IsNullOrEmpty(documentRowDataRDE.SenderProtocolNumber) ? documentRowDataRDE.SenderProtocolNumber : null,
                        DataArrivo = !string.IsNullOrEmpty(documentRowDataRDE.ArrivalDate) ? (documentRowDataRDE.ArrivalDate + " " + documentRowDataRDE.ArrivalTime).AsDateTime() : null
                    });

                    documentoAmministrativoAggregate.AssignMittente(new Mittente(new PG()
                    {
                        DenominazioneUfficio = new TextValue(documentRowData.CorrDesc[0]),
                    }));
                }

                if (tipoProto.ToUpper() == "P")
                {
                    // Il codice corrispondente da analizzare
                    // Questo array dovr� contenere due elementi: Il codice corrispondente
                    // e la tipologia (M, D, CC)
                    string[] corrToAdd = null;
                    documentRowData.CorrDesc.ForEach(c =>
                    {
                        corrToAdd = c.Split('#');
                        if (corrToAdd.Length != 3)
                            throw new SpecificaCorrispondenteNonValidaPi3Exception(c);

                        if (corrToAdd[1].ToUpper().Trim() == "D")
                        {
                            documentoAmministrativoAggregate.AddDestinatario(new Destinatario(
                                new PG()
                                {
                                    DenominazioneUfficio = new TextValue(corrToAdd[0])
                                }));
                        }

                        if (corrToAdd[1].ToUpper().Trim() == "CC")
                        {
                            documentoAmministrativoAggregate.AddDestinatarioCc(new Destinatario(
                                new PG()
                                {
                                    DenominazioneUfficio = new TextValue(corrToAdd[0])
                                }));
                        }
                    });

                    documentoAmministrativoAggregate.AssignMittente(new Mittente(new PG()
                    {
                        DenominazioneUfficio = new TextValue(ruolo.uo.descrizione),
                    },
                    ruolo.uo.systemId));
                }

                documentoAmministrativoAggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione()
                {
                    DatiRegistro = new DatiRegistro()
                    {
                        IdRegistro = idRF != null ? idRF.ToString() : registroEntity.SYSTEM_ID.ToString()
                    }
                });

                await this._documentoAmministrativoRepository.Add(documentoAmministrativoAggregate);

                importResult.DocNumber = documentoAmministrativoAggregate.Id;
                importResult.Ordinal = documentRowData.OrdinalNumber;
                importResult.OtherInformation.AddRange(creationProblems);
                importResult.Message = String.Format(Resources.DocumentoCreatoConSuccesso, documentoAmministrativoAggregate.Id, documentoAmministrativoAggregate.IdDoc.Segnatura);
                importResult.Outcome = importResult.OtherInformation.Count > 0 ? ImportResult.OutcomeEnumeration.Warnings : ImportResult.OutcomeEnumeration.OK;
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                importResult = new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = pi3Ex.Message,
                    OtherInformation = creationProblems,
                    Ordinal = documentRowData.OrdinalNumber
                };
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                importResult = new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = ex.Message,
                    Ordinal = documentRowData.OrdinalNumber
                };
            }

            return importResult;
        }

        protected virtual async Task<List<string>> CheckDataValidity(DocumentRowData documentRowData, bool isRapidClassificationRequired, string tipoProto)
        {
            var validationProblems = new List<string>();

            if (string.IsNullOrEmpty(documentRowData.OrdinalNumber))
                validationProblems.Add(Resources.ValidationDataNumeroProtocolloEmergenza);

            if (string.IsNullOrEmpty(documentRowData.AdminCode))
                validationProblems.Add(Resources.ValidationDataAdminCode);

            if (string.IsNullOrEmpty(documentRowData.RegCode))
                validationProblems.Add(Resources.ValidationDataRegCode);

            // Il codice RF deve essere obbligatorio se richiesto dalla segnatura
            if (!string.IsNullOrEmpty(documentRowData.AdminCode))
            {
                var amministraEntity = await _dbContext.AmministraEntities.AsNoTracking()
                    .Where(a => a.VAR_CODICE_AMM.ToUpper().Equals(documentRowData.AdminCode.ToUpper()))
                    .FirstOrDefaultAsync();

                if (amministraEntity == null)
                    validationProblems.Add(string.Format(Resources.AmministrazioneNotFound, documentRowData.AdminCode));

                if (amministraEntity != null &&
                    amministraEntity.VAR_FORMATO_SEGNATURA.Contains("COD_RF_PROT") &&
                    string.IsNullOrEmpty(documentRowData.RFCode))
                {
                    validationProblems.Add(Resources.ValidationDataRFCod);
                }
            }

            if (string.IsNullOrEmpty(documentRowData.Obj))
                validationProblems.Add(Resources.ValidationDataOggetto);

            if (isRapidClassificationRequired && documentRowData.ProjectCodes == null)
                validationProblems.Add(Resources.ValidationDataCodiceClassifica);

            var documentRowDataRDE = documentRowData as RDEDocumentRowData;
            if (string.IsNullOrEmpty(documentRowDataRDE.EmergencyProtocolSignature))
                validationProblems.Add(Resources.ValidationDataStringaProtocolloEmergenza);

            if (string.IsNullOrEmpty(documentRowDataRDE.EmergencyProtocolDate))
                validationProblems.Add(Resources.ValidationDataDataProtocolloEmergenza);

            if (string.IsNullOrEmpty(documentRowDataRDE.EmergencyProtocolTime))
                validationProblems.Add(Resources.ValidationDataOraProtocolloEmergenza);

            // Il campo Data protocollo emergenza deve essere minore della data di protocollazione, che 
            // si suppone uguale alla data odierna. visto che il protocolla in giallo � stata dismessa 
            // nel senso che non la usa pi� nessuno.
            if (!string.IsNullOrEmpty(documentRowDataRDE.EmergencyProtocolDate) &&
                !string.IsNullOrEmpty(documentRowDataRDE.EmergencyProtocolTime))
            {
                //l'excel � formattato un modo tale che la data e l'ora possono solo arrivare nel formato valido.
                string dataEmerg = documentRowDataRDE.EmergencyProtocolDate + " " + documentRowDataRDE.EmergencyProtocolTime;
                if (IsDate(dataEmerg))
                {
                    if (dataEmerg.AsDateTime() > System.DateTime.Now)
                        validationProblems.Add(Resources.ValidationDataDataProtocolloEmergenzaMinoreDataOdierna);
                }
                else
                {
                    validationProblems.Add(Resources.ValidationDataDataProtocolloEmergenzaNonValido);
                }
            }

            // Se almeno uno fra i due campi "Data arrivo" e "Ora Arrivo", � valorizzato,
            // viene controllato che antrambi lo siano.
            if ((!string.IsNullOrEmpty(documentRowDataRDE.ArrivalTime) && string.IsNullOrEmpty(documentRowDataRDE.ArrivalDate)) ||
                (!string.IsNullOrEmpty(documentRowDataRDE.ArrivalDate) && string.IsNullOrEmpty(documentRowDataRDE.ArrivalTime)))
            {
                validationProblems.Add(Resources.ValidationDataDataArrivo);
            }

            //La data di protocollo mittente deve essere minore della data attuale e minore della data di arrivo
            if (!string.IsNullOrEmpty(documentRowDataRDE.SenderProtocolDate) &&
                documentRowDataRDE.SenderProtocolDate.AsDateTime() > DateTime.Now)
            {
                validationProblems.Add(Resources.ValidationDataDataProtocolloMittenteMinoreDataOdierna);
            }

            if (!string.IsNullOrEmpty(documentRowDataRDE.ArrivalDate))
            {
                if (!IsDate(documentRowDataRDE.ArrivalDate))
                {
                    validationProblems.Add(Resources.ValidationDataDataArrivoNotValid);
                }
                else
                {
                    if (documentRowDataRDE.ArrivalDate.AsDateTime() > DateTime.Now)
                        validationProblems.Add(Resources.ValidationDataDataArrivoMinoreDataOdierna);

                    if (!string.IsNullOrEmpty(documentRowDataRDE.SenderProtocolDate) &&
                        documentRowDataRDE.SenderProtocolDate.AsDateTime() > documentRowDataRDE.ArrivalDate.AsDateTime())
                    {
                        validationProblems.Add(Resources.ValidationDateDataProtocolloMittenteMinoreDataArrivo);
                    }

                }
            }

            if (tipoProto.ToUpper() == "P")
            {
                if (documentRowData.CorrDesc == null)
                    validationProblems.Add(Resources.ValidationDataCorrispondentiNotFound);

                if (documentRowData.CorrDesc != null)
                {
                    // Se sono stati individuati mittenti, errore
                    if (documentRowData.CorrDesc.Count(c => c.ToUpper().Contains("#M#")) != 0)
                        validationProblems.Add(Resources.ValidationDataMittentiNonPossonoEssereInseriti);

                    if (!documentRowData.CorrDesc.Any(c => c.ToUpper().Contains("#D#") && c.Trim().Length > 3))
                        validationProblems.Add(Resources.ValidationDataDestinatarioPrimarioNotFound);
                }
            }

            return validationProblems;
        }

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
            var oraProtocolloEmergenza = row.GetCellValueAsString(columns[Resources.ColumnOraProtocolloEmergenza]);
            var dataArrivo = row.GetCellValueAsString(columns[Resources.ColumnDataArrivo]);
            var oraArrivo = row.GetCellValueAsString(columns[Resources.ColumnOraArrivo]);

            documentRowData.AdminCode = row.GetCellValueAsString(columns[Resources.ColumnCodiceAmministrazione]);
            documentRowData.EmergencyProtocolDate = !string.IsNullOrEmpty(dataProtocolloEmergenza) ? dataProtocolloEmergenza.AsDateTime().AsDateFormat() : string.Empty;
            documentRowData.EmergencyProtocolTime = !string.IsNullOrEmpty(oraProtocolloEmergenza) ? oraProtocolloEmergenza : string.Empty;
            documentRowData.OrdinalNumber = row.GetCellValueAsString(columns[Resources.ColumnNumeroProtocolloEmergenza]);
            documentRowData.EmergencyProtocolSignature = row.GetCellValueAsString(columns[Resources.ColumnStringaProtocolloEmergenza]);
            documentRowData.Obj = row.GetCellValueAsString(columns[Resources.ColumnOggetto]);
            documentRowData.SenderProtocolDate = row.GetCellValueAsString(columns[Resources.ColumnDataProtocolloMittente]);
            documentRowData.SenderProtocolNumber = row.GetCellValueAsString(columns[Resources.ColumnNumeroProtocolloMittente]);
            documentRowData.ArrivalDate = !string.IsNullOrEmpty(dataArrivo) ? dataArrivo.AsDateTime().AsDateFormat() : string.Empty;

            if (!string.IsNullOrEmpty(dataArrivo))
            {
                documentRowData.ArrivalDate = dataArrivo.AsDateTime().AsDateFormat();
                if (!string.IsNullOrEmpty(oraArrivo))
                {
                    TimeSpan time;
                    TimeSpan.TryParse(oraArrivo, out time);

                    if (time != null)
                        documentRowData.ArrivalTime = time.ToString();
                }
            }

            documentRowData.ProjectCodes = row.GetCellValueAsString(columns[Resources.ColumnCodiceClassifica])?.ToString().Split(';');
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

        /// <summary>
        /// Funzione utilizzata per verificare se una stringa � una data
        /// </summary>
        /// <param name="date">Stringa da verificare</param>
        /// <returns>True se date � una data, false altrimenti</returns>
        protected bool IsDate(string date)
        {
            CultureInfo ci = new CultureInfo("it-IT");
            string[] dateFormats = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy", "HH:mm:ss" };

            try
            {
                date = date.Trim();
                DateTime d_ap = DateTime.ParseExact(date, dateFormats, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return true;

            }
            catch (Exception)
            {
                return false;
            }

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