// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.PrjDocImport;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
using ImportRDEDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.ImportRDEDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ImportRDEDocument
{

    // Richiede libreria MediatR
    public class ImportRDEDocumentHandler : IRequestHandler<ImportRDEDocumentRequest, ImportRDEDocumentResult>
    {
        #region Public Members

        public ImportRDEDocumentHandler(ILogger<ImportRDEDocumentHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._configurationService = configurationService;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<ImportRDEDocumentResult> Handle(ImportRDEDocumentRequest request, CancellationToken cancellationToken)
        {
            var importResult = new ImportResult();

            try
            {
                if(request.role.funzioni.Where(e => e.codice == "DO_NUOVOPROT").FirstOrDefault() == null ||
                   (request.protoType == ProtoType.A && request.role.funzioni.Where(e => e.codice == "PROTO_IN").FirstOrDefault() == null) ||
                   (request.protoType == ProtoType.P && request.role.funzioni.Where(e => e.codice == "PROTO_OUT").FirstOrDefault() == null))
                {
                    throw new UtenteNonAbilitatoAllaCreazioneDelProtocolloPi3Exception(request.protoType.ToString());
                }

                importResult = await ImportDocument(request.rowData, request.userInfo, request.role, request.isRapidClassificationRequired, request.protoType.ToString());
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                importResult = new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = pi3Ex.Message
                };
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new ImportRDEDocumentResult(importResult);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ImportRDEDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

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
                                idAmministrazione = registroEntity.ID_AMM.ToString(),
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
                    // Questo array dovrà contenere due elementi: Il codice corrispondente
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

                var firstProject = true;
                foreach (string projectId in projectIds)
                {
                    try
                    {
                        var fascicolo = (await this._mediator.Send(new Requests.FascicolazioneGetFascicoloById(projectId, infoUtente))).output;
                        var result = await this._mediator.Send(new Application.Requests.FascicolazioneAddDocFascicolo(infoUtente, documentoAmministrativoAggregate.Id, fascicolo, firstProject));
                        firstProject = false;

                        if(result != null && !result.output)
                           creationProblems.Add(Resources.ErroreDuranteLaFascicolazione);
                    }
                    catch(Exception ex) 
                    {
                        creationProblems.Add(Resources.ErroreDuranteLaFascicolazione);
                    }
                }

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
            // si suppone uguale alla data odierna. visto che il protocolla in giallo è stata dismessa 
            // nel senso che non la usa più nessuno.
            if (!string.IsNullOrEmpty(documentRowDataRDE.EmergencyProtocolDate) &&
                !string.IsNullOrEmpty(documentRowDataRDE.EmergencyProtocolTime))
            {
                //l'excel è formattato un modo tale che la data e l'ora possono solo arrivare nel formato valido.
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

            // Se almeno uno fra i due campi "Data arrivo" e "Ora Arrivo", è valorizzato,
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

        /// <summary>
        /// Funzione utilizzata per verificare se una stringa è una data
        /// </summary>
        /// <param name="date">Stringa da verificare</param>
        /// <returns>True se date è una data, false altrimenti</returns>
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
}
