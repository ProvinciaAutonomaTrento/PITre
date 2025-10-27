// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.trasmissione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetDettaglioDocumentoNoSecurity;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.EditDocStateDiagram;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetRagioneByCodice;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetListaCorrispondenti;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteBySystemId;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetRuoliRiferimentoAutorizzati;
using Chilkat;
using Microsoft.Graph.Models.CallRecords;
using StackExchange.Redis;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmissionDocument
{
    // Richiede libreria MediatR
    public class ExecuteTransmissionDocumentCommandHandler : IRequestHandler<ExecuteTransmissionDocumentCommand, ExecuteTransmissionDocumentCommandResponse>
    {
        #region Public Members

        public ExecuteTransmissionDocumentCommandHandler(ILogger<ExecuteTransmissionDocumentCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, ITrasmissioneRepository trasmissioneRepository, IWebMethodLoggerService loggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._trasmissioneRepository= trasmissioneRepository;
            this._loggerService = loggerService;
        }

        public async Task<ExecuteTransmissionDocumentCommandResponse> Handle(ExecuteTransmissionDocumentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("ExecuteTransmissionDocument - START");

            ExecuteTransmissionDocumentCommandResponse response = new ExecuteTransmissionDocumentCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region Controllo parametri richiesta
                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new RestException("REQUIRED_ID");
                }


                if (request.Receiver == null)
                {
                    //Destinatario non trovato
                    throw new RestException("REQUIRED_CORRESPONDENT");
                }
                if (string.IsNullOrEmpty(request.Receiver.Code) && string.IsNullOrEmpty(request.Receiver.Id))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_RECEIVER");
                }
                if (request.TransmissionReason == null || string.IsNullOrEmpty(request.TransmissionReason))
                {
                    throw new RestException("REQUIRED_TRANSMISSION_REASON");
                }
                else if ((!string.IsNullOrEmpty(infoUtente.diSistema)) && (infoUtente.diSistema == "1") && (request.TransmissionReason != "COMPETENZA_SIST_ESTERNI"))
                {
                    throw new Exception("Ragione di trasmissione non valida per un sistema esterno. Ragione disponibile: COMPETENZA_SIST_ESTERNI");
                }


                #endregion

                #region implementazione
                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                // Controllo visibilità documento
                try
                {
                    long? idProfile = !string.IsNullOrEmpty(request.IdDocument) ? request.IdDocument.AsLong() : null;

                    await _pi3DbContext.AssertSecurityRights(idProfile.ToString(), infoUtente.idPeople, infoUtente.idGruppo);

                    documento = (await this._mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityCommand()
                    {
                        Infoutente = infoUtente,
                        DocNumber = idProfile.ToString(),
                        IdProfile = idProfile.ToString()
                    })).Output;
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                DocsPaVO.trasmissione.RagioneTrasmissione? ragione = null;
                try
                {
                    ragione = (await this._mediator.Send(new GetRagioneByCodiceCommand()
                    {
                        IdAmm = infoUtente.idAmministrazione,
                        Codice = request.TransmissionReason.ToUpper()
                    })).Output;
                    if (ragione == null)
                        throw new RestException("TRANSMISSION_REASON_NOT_FOUND");
                }
                catch (Exception e)
                {
                    throw new RestException("TRANSMISSION_REASON_NOT_FOUND");
                }

                CorrGlobaliEntity dest = null;
                string receiverChaTipoUrp = string.Empty;
                if (!string.IsNullOrWhiteSpace(request.Receiver.Id))
                {
                    dest = (from a in _pi3DbContext.CorrGlobaliEntities where a.SYSTEM_ID == request.Receiver.Id.AsLong() && a.DTA_FINE == null select a).FirstOrDefault();
                }
                else
                {
                    dest = (from a in _pi3DbContext.CorrGlobaliEntities where a.VAR_COD_RUBRICA.ToUpper() == request.Receiver.Code.ToUpper() && a.ID_AMM== infoUtente.idAmministrazione.AsLong() && a.DTA_FINE == null select a).FirstOrDefault();
                }
                receiverChaTipoUrp = dest.CHA_TIPO_URP;

                if (dest == null|| dest.SYSTEM_ID==0 ||
                    dest.CHA_TIPO_IE == "E" || dest.DTA_FINE != null) throw new RestException("CORRESPONDENT_NOT_FOUND");
                if(dest.CHA_TIPO_URP == "U")
                {
                    var destRuolo = (from a in _pi3DbContext.CorrGlobaliEntities where a.ID_UO == dest.SYSTEM_ID && a.CHA_RIFERIMENTO == "1" && a.DTA_FINE == null select a).FirstOrDefault();
                    if (destRuolo != null)
                        dest = destRuolo;
                    else throw new RestException("CORRESPONDENT_NOT_FOUND");
                }
                var destUt = (await this._mediator.Send(new AddressbookGetCorrispondenteBySystemIdCommand()
                {
                    SystemId = dest.SYSTEM_ID.ToString()
                })).Output;

                var aggregate = new Core.AggregateModels.TrasmissioneAggregate.Trasmissione(
                    infoUtente.idAmministrazione, 
                    DateTime.Now, 
                    request.IdDocument, 
                    TipiOggettiTrasmessiEnum.DocumentoAmministrativo, 
                    new Autore() 
                    { 
                        IdUtente = infoUtente.idPeople, 
                        IdGruppo = infoUtente.idGruppo
                    }
                );

                #region Cessione diritti
                DocsPaVO.utente.Utente utentePropCessione = null;
                DocsPaVO.utente.InfoUtente infoUtentePropCessione = null;
                DocsPaVO.utente.Ruolo ruoloPropCessione = null;
                if (ragione != null && !string.IsNullOrEmpty(ragione.prevedeCessione) && ragione.prevedeCessione != "N")
                {
                    ragione.cessioneImpostata = true;
                    if (documento != null && documento.accessRights == "255")
                    {
                        if (receiverChaTipoUrp != "R")
                        {
                            throw new Exception(Messages.GiveUpRightsReceiverMustBeRole);
                        }
                        if (request == null || request.Receiver == null || string.IsNullOrEmpty(request.Receiver.Note))
                        {
                            ruoloPropCessione = DBUtils.getRuoloByCodice(dest.VAR_COD_RUBRICA,this._pi3DbContext);
                            if (ruoloPropCessione != null)
                            {
                                List<DocsPaVO.utente.UserMinimalInfo> userList = DBUtils.GetUsersInRoleMinimalInfo(ruoloPropCessione.idGruppo,this._pi3DbContext);
                                if (userList != null && userList.Count > 0)
                                {
                                    utentePropCessione = DBUtils.getUtenteById(userList[0].SystemId, this._pi3DbContext);
                                    infoUtentePropCessione = new DocsPaVO.utente.InfoUtente(utentePropCessione, ruoloPropCessione);
                                }
                            }
                        }
                        else
                        {
                            utentePropCessione = DBUtils.getUtente(request.Receiver.Note, infoUtente.idAmministrazione,this._pi3DbContext);
                            if (utentePropCessione != null)
                            {
                                DocsPaVO.utente.InfoUtente infoUtenteDaCercare = new DocsPaVO.utente.InfoUtente(utentePropCessione, DBUtils.getRuoloPreferito(utentePropCessione.idPeople,this._pi3DbContext));
                                ArrayList arrayRuoli = DBUtils.getRuoliUtente(infoUtenteDaCercare.idPeople,this._pi3DbContext);
                                bool ruoloPresente = false;
                                if (arrayRuoli != null && arrayRuoli.Count > 0)
                                {
                                    foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                                    {
                                        if (rol.codiceRubrica == dest.VAR_COD_RUBRICA)
                                        {
                                            ruoloPresente = true;
                                            ruoloPropCessione = rol;
                                            break;
                                        }
                                    }
                                }
                                if (!ruoloPresente)
                                {
                                    throw new Exception(Messages.UsertNotInRole);
                                }

                            }
                            else
                            {
                                throw new RestException("USER_NO_EXIST");
                            }
                        }
                    }
                    else
                    {
                        if (receiverChaTipoUrp != "R")
                        {
                            throw new Exception(Messages.GiveUpRightsReceiverMustBeRole);
                        }
                        if (ragione.prevedeCessione == "W")
                        {
                            if (!(request != null && request.Receiver != null && !string.IsNullOrEmpty(request.Receiver.Note)
                                && request.Receiver.Note.ToUpper() == "CEDI"))
                                ragione.cessioneImpostata = false;
                        }
                    }
                }
                #endregion
                string trasmType = "S";
                if (!string.IsNullOrEmpty(request.TransmissionType) && request.TransmissionType == "T")
                {
                    trasmType = "T";
                }
                CessioneDocumento? cessione = null;

                if (ragione != null && !string.IsNullOrEmpty(ragione.prevedeCessione) && ragione.prevedeCessione != "N" && ragione.cessioneImpostata)
                {
                    cessione = new DocsPaVO.documento.CessioneDocumento();
                    cessione.idPeople = infoUtente.idPeople;
                    cessione.idRuolo = infoUtente.idGruppo;
                    cessione.docCeduto = true;
                    cessione.userId = infoUtente.userId;
                    if (utentePropCessione != null && ruoloPropCessione != null)
                    {
                        cessione.idPeopleNewPropr = utentePropCessione.idPeople;
                        cessione.idRuoloNewPropr = ruoloPropCessione.idGruppo;
                    }
                }

                var cessioneImpostata = cessione != null && cessione.docCeduto;

                var tipoCorr = this._pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(w => w.SYSTEM_ID == dest.SYSTEM_ID).FirstOrDefault();

                switch (tipoCorr.CHA_TIPO_URP)
                {
                    case "P":

                        #region prepara trasmissione singola per le persone
                        DatiTrasmissioneSingolaUtente datiU = new DatiTrasmissioneSingolaUtente()
                        {
                            Cognome = tipoCorr.VAR_COGNOME,
                            UserId = tipoCorr.VAR_DESC_CORR,
                            Nome = tipoCorr.VAR_NOME,
                            IdUtente = tipoCorr.ID_PEOPLE.ToString(),
                            IdRagioneTrasmissione = ragione.systemId,
                            NomeRagioneTrasmissione = ragione.descrizione,
                            DataScadenza = null,
                            NascondiVersioniPrecedenti = false,
                            Note = null,
                            RagioneConWorkflow = ragione.isTipoTask,
                            CessioneDirittiRagione = !cessioneImpostata ? null : new CessioneDirittiRagione
                            {
                                MantieniLettura = ragione.mantieniLettura == "1",
                                MantieniScrittura = ragione.mantieniScrittura == "1"
                            }

                        };

                        aggregate.PrepareTrasmissioneSingolaUtente(datiU);

                        #endregion
                        break;
                    case "R":
                        #region prepara trasmissione singole per ut in ruolo
                        List<DatiUtenteNotificatoTrasmissioneSingolaGruppo> utentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                        var users = await this.GetUsers(dest.VAR_COD_RUBRICA, dest.ID_AMM.ToString());
                        List<DocsPaVO.utente.Corrispondente> usersToNotify = new(); 
                        if (users.Count() == 0)
                        {
                            throw new Exception();
                        }
                        if (ragione != null && !string.IsNullOrEmpty(ragione.prevedeCessione) && ragione.prevedeCessione != "N" && utentePropCessione != null && ruoloPropCessione != null)
                        {
                            foreach (var us in users)
                            {
                                var ut = (DocsPaVO.utente.Utente)us;
                                if (ut.idPeople == utentePropCessione.idPeople)
                                    usersToNotify.Add(us);
                            }
                        }
                        else
                        {
                            usersToNotify = users.ToList();
                        }
                        foreach (var user in usersToNotify)
                        {
                            var ut = (DocsPaVO.utente.Utente)user;
                            bool isUserDisabled = string.IsNullOrEmpty(ut.disabilitato)
                                ? await this._pi3DbContext.PeopleEntities.AnyAsync(p => p.SYSTEM_ID == ut.idPeople.AsLong() && p.ID_AMM == ut.idAmministrazione.AsLong() && p.DISABLED == "Y")
                                : ut.disabilitato.Equals("Y");

                            if (!isUserDisabled)
                            {
                                utentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                                {
                                    IdUtente = ut.idPeople,
                                    UserId = ut.userId,
                                    Cognome = ut.cognome,
                                    Nome = ut.nome
                                });
                            }
                        }

                        if (cessioneImpostata && ragione.prevedeCessione != "N")
                        {
                            utentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                            utentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                            {
                                IdUtente = cessione.idPeopleNewPropr
                            });
                        }

                        DatiTrasmissioneSingolaGruppo datiG = new DatiTrasmissioneSingolaGruppo()
                        {
                            CodiceGruppoDestinatario = tipoCorr.VAR_COD_RUBRICA,
                            DescrizioneGruppoDestinatario = new TextValue(tipoCorr.VAR_DESC_CORR),
                            IdGruppoDestinatario = tipoCorr.ID_GRUPPO.ToString(),
                            IdRagioneTrasmissione = ragione.systemId,
                            NomeRagioneTrasmissione = ragione.descrizione,
                            DataScadenza = null,
                            NascondiVersioniPrecedenti = false,
                            Note = null,
                            RagioneConWorkflow = ragione.isTipoTask,
                            Tipo = trasmType == "T" ? TipiTrasmissioneSingolaEnum.Tutti : TipiTrasmissioneSingolaEnum.Uno,
                            UtentiNotificati = utentiNotificati,
                            CessioneDirittiRagione = !cessioneImpostata ? null : new CessioneDirittiRagione
                            {
                                MantieniLettura = ragione.mantieniLettura == "1",
                                MantieniScrittura = ragione.mantieniScrittura == "1"
                            }
                        };

                        aggregate.PrepareTrasmissioneSingolaGruppo(datiG);

                        #endregion

                        break;
                    case "U":
                        #region prepara trasmissione singola per le uo
                        DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
                        qca.ragione = ragione;
                        qca.ruolo = ruolo;
                        qca.queryCorrispondente = new DocsPaVO.addressbook.QueryCorrispondente();
                        qca.queryCorrispondente.fineValidita = true;
                        var uo = (DocsPaVO.utente.UnitaOrganizzativa)destUt;

                        var ruoli = (await this._mediator.Send(new AddressbookGetRuoliRiferimentoAutorizzatiCommand()
                        {
                            Qca = qca,
                            Uo = uo

                        })).Output;
                        if (!(ruoli == null || ruoli.Count() == 0))
                        {
                            aggregate = await this.PreparaTrasmSingoleRuolo(dest,aggregate,cessioneImpostata,ragione,trasmType,cessione,tipoCorr);
                        }
                        #endregion
                        break;
                };


                await _trasmissioneRepository.Add(aggregate);

                var createdAggregate = await _trasmissioneRepository.Get(infoUtente.idAmministrazione, aggregate.Id);

                InviaBehavior? inviaBehavior = !request.Notify ? new InviaBehavior()
                {
                    InibisciNotifiche = true
                } : null;

                createdAggregate.Invia(DateTime.Now, inviaBehavior);

                await _trasmissioneRepository.Update(createdAggregate);
                bool notifica = true;
                if (request.Notify != null && !request.Notify)
                {
                    notifica = false;
                }
                foreach (var ts in aggregate.TrasmissioniSingole)
                {
                    await _loggerService.LogOK("TRASM_DOC_"+ragione.descrizione.ToUpper().Replace(" ", "_"), aggregate.OggettoTrasmesso.Id, $"PIS REST: Trasmesso Documento {aggregate.OggettoTrasmesso.Id}", ts.Id, infoUtente.codWorkingApplication, !notifica);
                }

                #endregion
                response.TransmMessage = "Trasmissione effettuata";
                response.Code = TransmissionResponseCode.OK;

                _logger.LogInformation("end ExecuteTransmissionDocument");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione ExecuteTransmissionDocument: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new ExecuteTransmissionDocumentCommandResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione ExecuteTransmissionDocument");
                response = new ExecuteTransmissionDocumentCommandResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<ExecuteTransmissionDocumentCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _loggerService;

        private async System.Threading.Tasks.Task<Pi3.Core.AggregateModels.TrasmissioneAggregate.Trasmissione> PreparaTrasmSingoleRuolo(CorrGlobaliEntity dest, 
            Pi3.Core.AggregateModels.TrasmissioneAggregate.Trasmissione aggregate,bool cessioneImpostata,
            DocsPaVO.trasmissione.RagioneTrasmissione ragione, string trasmType,
            CessioneDocumento cessione,CorrGlobaliEntity tipoCorr

            )
        {
            List<DatiUtenteNotificatoTrasmissioneSingolaGruppo> utentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();

            var users = await this.GetUsers(dest.VAR_COD_RUBRICA, dest.ID_AMM.ToString());
            if (users.Count() == 0)
            {
                throw new Exception();
            }
            foreach (var user in users.ToList())
            {
                var ut = (DocsPaVO.utente.Utente)user;
                bool isUserDisabled = string.IsNullOrEmpty(ut.disabilitato)
                    ? await this._pi3DbContext.PeopleEntities.AnyAsync(p => p.SYSTEM_ID == ut.idPeople.AsLong() && p.ID_AMM == ut.idAmministrazione.AsLong() && p.DISABLED == "Y")
                    : ut.disabilitato.Equals("Y");

                if (!isUserDisabled)
                {
                    utentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = ut.idPeople,
                        UserId = ut.userId,
                        Cognome = ut.cognome,
                        Nome = ut.nome
                    });
                }
            }

            if (cessioneImpostata && ragione.prevedeCessione != "N")
            {
                utentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                utentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                {
                    IdUtente = cessione.idPeopleNewPropr
                });
            }

            DatiTrasmissioneSingolaGruppo datiG = new DatiTrasmissioneSingolaGruppo()
            {
                CodiceGruppoDestinatario = tipoCorr.VAR_COD_RUBRICA,
                DescrizioneGruppoDestinatario = new TextValue(tipoCorr.VAR_DESC_CORR),
                IdGruppoDestinatario = tipoCorr.ID_GRUPPO.ToString(),
                IdRagioneTrasmissione = ragione.systemId,
                NomeRagioneTrasmissione = ragione.descrizione,
                DataScadenza = null,
                NascondiVersioniPrecedenti = false,
                Note = null,
                RagioneConWorkflow = ragione.isTipoTask,
                Tipo = trasmType == "T" ? TipiTrasmissioneSingolaEnum.Tutti : TipiTrasmissioneSingolaEnum.Uno,
                UtentiNotificati = utentiNotificati,
                CessioneDirittiRagione = !cessioneImpostata ? null : new CessioneDirittiRagione
                {
                    MantieniLettura = ragione.mantieniLettura == "1",
                    MantieniScrittura = ragione.mantieniScrittura == "1"
                }
            };

            aggregate.PrepareTrasmissioneSingolaGruppo(datiG);

            return aggregate;
        }
        private async Task<DocsPaVO.utente.Corrispondente[]> GetUsers(string codiceRubrica, string idAmm)
        {

            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

            qco.codiceRubrica = codiceRubrica;
            qco.getChildren = true;
            qco.idAmministrazione = idAmm;
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
            qco.fineValidita = true;

            var users = (await this._mediator.Send(new AddressbookGetListaCorrispondentiCommand()
            {
                QueryCorrispondente = qco
            })).Output;

            return users ?? new DocsPaVO.utente.Corrispondente[0];
        }

        #endregion
    }

}
