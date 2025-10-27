// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.DatiCert;
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteBySystemId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetListaCorrispondenti;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetDettaglioDocumentoNoSecurity;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.AvvioProcessoDiFirma;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.EseguiPassoAutomatico;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using System.Globalization;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.StartSignatureProcess
{
    // Richiede libreria MediatR
    public class StartSignatureProcessCommandHandler : IRequestHandler<StartSignatureProcessCommand, StartSignatureProcessCommandResponse>
    {
        #region Public Members

        public StartSignatureProcessCommandHandler(ILogger<StartSignatureProcessCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IConfigurationService configurationService, ITrasmissioneRepository trasmissioneRepository, IWebMethodLoggerService loggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._trasmissioneRepository = trasmissioneRepository;
            this._configurationService = configurationService;
            this._loggerService = loggerService;
        }


        public async Task<StartSignatureProcessCommandResponse> Handle(StartSignatureProcessCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartSignatureProcess - START");

            var resultAvvioProcesso = ResultProcessoFirma.OK;

            var response = new StartSignatureProcessCommandResponse();

            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                #endregion

                #region controlli parametri richiesta
                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new RestException("REQUIRED_ID");
                }
                if (string.IsNullOrEmpty(request.Note))
                {
                    throw new RestException("MISSING_PARAMETER");
                }
                if (request.SignatureProcess == null)
                {
                    throw new RestException("MISSING_PARAMETER");
                }
                #endregion

                var processoDiFirma = SignBookUtils.GetProcessoFirmaFromDomain(request.SignatureProcess, _pi3DbContext) ?? throw new RestException("SIGN_PROCESS_NOT_FOUND");

                var docNumber = request.IdDocument.AsLong();
                var profileEntity = await this._pi3DbContext.ProfileEntities.Where(p => p.SYSTEM_ID == docNumber).FirstAsync();
                bool isAllegato = false;
                long? idDocumentoPrincipale = await this._pi3DbContext.ProfileEntities
                        .Where(p => p.SYSTEM_ID == docNumber)
                        .Select(p => p.ID_DOCUMENTO_PRINCIPALE)
                        .FirstOrDefaultAsync();
                if (idDocumentoPrincipale == null || idDocumentoPrincipale == 0)
                {
                    idDocumentoPrincipale = request.IdDocument.AsLong();
                }
                else isAllegato = true;
                idDocumentoPrincipale = request.IdDocument.AsLong();

                FileRequest frequest;
                var documento = (await _mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityCommand()
                {
                    Infoutente = infoUtente,
                    DocNumber = docNumber.ToString(),
                    IdProfile = docNumber.ToString()
                })).Output;

                if (documento is not null)
                {
                    frequest = documento.documenti[0];
                }
                else
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                bool retBool = false;

                ResultProcessoFirma resultAvvio = ResultProcessoFirma.OK;

                //var avvioProcessodiFirma = (await StartProcessoDiFirma(processoDiFirma, profileEntity, idTenant,
                //    frequest,
                //    infoUtente,
                //    "A", request.Note, opzioniNotifica,
                //     resultAvvio));

                var avvioProcessodiFirma = (await this._mediator.Send(new AvvioProcessoDiFirmaCommand()
                {
                    processoDiFirma = processoDiFirma,
                    file = frequest,
                    modalita = "A",
                    note = request.Note,
                    opzioniNotifiche = new OpzioniNotifica()
                    {
                        Notifica_interrotto = true,
                        Notifica_concluso = false
                    },
                    daCambioStato = false,
                }));

                retBool = avvioProcessodiFirma.output;
                resultAvvio = avvioProcessodiFirma.resultAvvioProcesso;

                if (retBool)
                {
                    string method = "AVVIATO_PROCESSO_DI_FIRMA_DOCUMENTO";
                    string description = "Avviato processo di firma per la versione " + frequest.version;
                    response.ResultMessage = description;
                    if (frequest.GetType().Equals(typeof(DocsPaVO.documento.Allegato)))
                    {
                        method = "AVVIATO_PROCESSO_DI_FIRMA_ALLEGATO";
                    }
                    _logger.LogDebug(infoUtente.userId, infoUtente.idPeople, infoUtente.idGruppo, infoUtente.idAmministrazione, method, frequest.docNumber,
                        description, DocsPaVO.Logger.CodAzione.Esito.OK, (infoUtente != null && infoUtente.delegato != null ? infoUtente.delegato : null), "1");
                }

                switch (resultAvvio)
                {
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_BLOCCATO:
                        throw new Exception("DOCUMENTO BLOCCATO");
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_CONSOLIDATO:
                        throw new Exception("DOCUMENTO CONSOLIDATO");
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.DOCUMENTO_GIA_IN_LIBRO_FIRMA:
                        throw new Exception("DOCUMENTO GIA IN LIBRO FIRMA");
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.EXISTING_PROCESS_NAME:
                        throw new Exception("ERRORE GENERICO");
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.FILE_NON_AMMESSO_ALLA_FIRMA:
                        throw new Exception("FILE NON AMMESSO ALLA FIRMA");
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.FILE_NON_ACQUISITO:
                        throw new Exception("FILE NON ACQUISITO");
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.PASSO_PROTO_DOC_GIA_PROTOCOLLATO:
                        throw new Exception("DOCUMENTO GIA PROTOCOLLATO");
                    case DocsPaVO.LibroFirma.ResultProcessoFirma.KO:
                        throw new Exception("ERRORE GENERICO");
                }
                if (!retBool) throw new Exception("ERRORE GENERICO");

                response.Code = Documents.MessageResponseCode.OK;

                _logger.LogInformation("end StartSignatureProcess");

            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione StartSignatureProcess: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new StartSignatureProcessCommandResponse
                {
                    Code = Documents.MessageResponseCode.SYSTEM_ERROR,
                    ErrorMessage = pisEx.Description
                };
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione StartSignatureProcess");
                response = new StartSignatureProcessCommandResponse
                {
                    Code = Documents.MessageResponseCode.SYSTEM_ERROR,
                    ErrorMessage = e.Message
                };

            }

            return response;
        }


        private string GetEstensioneIntoSignedFile(string fullname)
        {
            string retValue = string.Empty;

            // Reperimento del nome del file con estensione
            string fileName = new System.IO.FileInfo(fullname).Name;

            string[] items = fileName.Split('.');

            for (int i = (items.Length - 1); i >= 0; i--)
            {
                if (!(items[i].ToUpper().EndsWith("P7M") ||
                    items[i].ToUpper().EndsWith("TSD") ||
                    items[i].ToUpper().EndsWith("M7M"))
                    )
                {
                    retValue = items[i];
                    break;
                }
            }
            return retValue;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<StartSignatureProcessCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _loggerService;

        private async Task<(bool, DocsPaVO.LibroFirma.ResultProcessoFirma)> StartProcessoDiFirma(ProcessoFirma processoDiFirma, ProfileEntity profileEntity, long idTenant, DocsPaVO.documento.FileRequest file, DocsPaVO.utente.InfoUtente infoUtente, string modalita, string note, DocsPaVO.LibroFirma.OpzioniNotifica opzioniNotifiche, DocsPaVO.LibroFirma.ResultProcessoFirma resultAvvioProcesso, bool daCambioStato = false)
        {
            bool retVal = false;
            resultAvvioProcesso = ResultProcessoFirma.OK;
            IstanzaProcessoDiFirma istanzaProcesso = null!;
            IstanzaPassoDiFirma istanzaPassoCorrente = new();

            //TODO
            ////Verifico se � possibile avviare il processo di firma
            //if (!CanAvvioProcessoFirma(processoDiFirma, file, infoUtente, out resultAvvioProcesso))
            //{
            //    return false;
            //}

            bool canExecuteTransmission = false;
            string newIdElemento = string.Empty;

            try
            {
                using var transaction = await ((DbContext)this._pi3DbContext).Database.BeginTransactionAsync();
                //TODO
                var isDocInLibroFirma = await _pi3DbContext.ProfileEntities.AnyAsync(p => p.DOCNUMBER == profileEntity.DOCNUMBER && p.IN_LIBROFIRMA.Equals("1"));
                var aggiornamento = await UpdateLockDocument(_pi3DbContext.ProfileEntities.FirstOrDefault(p => p.DOCNUMBER == profileEntity.DOCNUMBER).DOCNUMBER.ToString(), "1");
                if (!isDocInLibroFirma && aggiornamento)
                {
                    //TODO
                    //Se la chiave � attiva la notifica di presenza di destintari interoperanti � obbligatoria
                    var attiva = await this._configurationService.GetValue<string>(idTenant.ToString(), "NOTIFICA_DEST_NO_INTEROP_OBB");
                    if (!string.IsNullOrEmpty(attiva) && attiva.Equals("1"))
                    {
                        opzioniNotifiche.NotificaPresenzaDestNonInterop = true;
                    }
                    istanzaProcesso = await CreateIstanzaFromProcesso(processoDiFirma, file, infoUtente, note, opzioniNotifiche, daCambioStato);
                    DocsPaVO.documento.InfoDocumento infoDoc = await GetInfoDocumentoLite(istanzaProcesso.docNumber);
                    istanzaPassoCorrente = await GetIstanzaPassoDiFirmaInAttesa(istanzaProcesso.idIstanzaProcesso);

                    if (!istanzaPassoCorrente.Evento.TipoEvento.Equals("W"))
                    {

                        if (!istanzaPassoCorrente.Evento.TipoEvento.Equals("E"))
                        {
                            newIdElemento = await InsertElementoInLibroFirma(istanzaProcesso, infoUtente, modalita);
                            if (!string.IsNullOrEmpty(newIdElemento))
                            {
                                retVal = true;
                                //PER EVITARE DI TRASMETTERE PI� VOLTE LO STESSO DOCUMENTO, CONTROLLO CHE NON ESISTE NEL LIBRO FIRMA DELL'UTENTE UN ALLEGATO DI UN DOCUMENTO GI� TRASMESSO                           
                                canExecuteTransmission = await CanExecuteTransmission(infoDoc.docNumber, istanzaPassoCorrente.RuoloCoinvolto.idGruppo, istanzaPassoCorrente.UtenteCoinvolto.idPeople, newIdElemento);
                            }
                            else
                            {
                                await RollbackStartProcessoDiFirma(istanzaProcesso, file);
                                return (false, ResultProcessoFirma.KO);
                            }
                        }
                        else
                        {
                            canExecuteTransmission = true;
                            retVal = true;
                        }
                    }
                    else
                    {
                        //Se � passo di wait verifico se ci sono allegati in lf, se non ci sono vado al passo successivo
                        List<DocsPaVO.LibroFirma.IstanzaProcessoDiFirma> listAllInProcess = await GetInfoProcessesStartedForDocument(istanzaProcesso.docNumber.AsLong());
                        listAllInProcess = (from l in listAllInProcess where l.docAll.Equals("A") select l).ToList();
                        if (listAllInProcess == null || listAllInProcess.Count == 0)
                        {
                            bool result = await UpdateStatoIstanzaPasso(istanzaPassoCorrente.idIstanzaPasso, string.Empty, DocsPaVO.LibroFirma.TipoStatoPasso.CLOSE.ToString(), infoUtente);
                            istanzaPassoCorrente = await GetNextIstanzaPasso(istanzaPassoCorrente.idIstanzaProcesso.AsLong(), istanzaPassoCorrente.numeroSequenza, istanzaProcesso.versionId.AsLong());
                            if (istanzaPassoCorrente != null && (!string.IsNullOrEmpty(istanzaPassoCorrente.idIstanzaPasso)))
                            {
                                bool resultPassoCorrente = await UpdateStatoIstanzaPasso(istanzaPassoCorrente.idIstanzaPasso, istanzaProcesso.versionId, DocsPaVO.LibroFirma.TipoStatoPasso.LOOK.ToString(), infoUtente);
                            }

                            //Devo inserire in libro firma
                            if (!istanzaPassoCorrente.Evento.TipoEvento.Equals("E"))
                            {
                                newIdElemento = await InsertElementoInLibroFirma(istanzaProcesso, infoUtente, modalita);
                                if (!string.IsNullOrEmpty(newIdElemento))
                                {
                                    retVal = true;
                                    //PER EVITARE DI TRASMETTERE PI� VOLTE LO STESSO DOCUMENTO, CONTROLLO CHE NON ESISTE NEL LIBRO FIRMA DELL'UTENTE UN ALLEGATO DI UN DOCUMENTO GI� TRASMESSO                           
                                    canExecuteTransmission = await CanExecuteTransmission(infoDoc.docNumber, istanzaPassoCorrente.RuoloCoinvolto.idGruppo, istanzaPassoCorrente.UtenteCoinvolto.idPeople, newIdElemento);
                                }
                                else
                                {
                                    await RollbackStartProcessoDiFirma(istanzaProcesso, file);
                                    return (false, ResultProcessoFirma.KO);
                                }
                            }
                            else
                            {
                                canExecuteTransmission = true;
                                retVal = true;
                            }
                        }
                        retVal = true;
                    }

                    #region TRASMISSIONE
                    if (canExecuteTransmission)
                    {
                        var trasmResult = await ExecuteTransmission(istanzaProcesso, istanzaPassoCorrente, infoDoc, infoUtente);
                        bool update = false;
                        if (!string.IsNullOrEmpty(newIdElemento))
                            update = await UpdateIdTrasmInElementoLF(newIdElemento, trasmResult.TrasmissioniSingole[0].Id.AsLong(), string.Empty);
                    }

                    if (retVal && istanzaPassoCorrente.IsAutomatico) //Se il passo � un passo automatico proseguo con la sua esecuzione
                        await this._mediator.Send(new EseguiPassoAutomaticoCommand() { idIstanzaProcessoFirma = istanzaProcesso.idIstanzaProcesso.AsLong() });

                    if (retVal)
                        await SalvaStoricoIstanzaProcessoFirma(istanzaProcesso, "Avviato processo di firma", infoUtente);

                    #endregion
                }
                else
                {
                    resultAvvioProcesso = ResultProcessoFirma.DOCUMENTO_GIA_IN_LIBRO_FIRMA;
                }
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await RollbackStartProcessoDiFirma(istanzaProcesso, file);
            }

            return (retVal, resultAvvioProcesso);
        }

        private async Task SalvaStoricoIstanzaProcessoFirma(IstanzaProcessoDiFirma istanzaProcesso, string azione, InfoUtente infoUtente)
        {
            var cambioStato = await this._pi3DbContext.IstanzaProcessoFirmaEntities.Where(x => x.ID_ISTANZA == istanzaProcesso.idIstanzaProcesso.AsLong()).Select(x => x.CHA_CAMBIO_STATO_DIAG).FirstOrDefaultAsync();

            var entityToInsert = new IstanzaProcFirmaStoEntity()
            {
                ID_USER = infoUtente.userId,
                DOC_NUMBER = istanzaProcesso.docNumber.AsLong(),
                ID_ISTANZA_PROCESSO = istanzaProcesso.idIstanzaProcesso.AsLong(), 
                DTA_DATE = await _pi3DbContext.GetSystemDateTime(),
                VAR_DESC_AZIONE = azione,
                ID_PEOPLE = infoUtente.idPeople.AsLong(),
                ID_RUOLO = string.IsNullOrEmpty(infoUtente.idGruppo) ? 0 : infoUtente.idCorrGlobali.AsLong(),
                ID_PEOPLE_DELEGATO = infoUtente.delegato == null ? 0 : infoUtente.delegato.idPeople.AsLong(),
                CHA_CAMBIO_STATO_DIAG = cambioStato
            };

            await this._pi3DbContext.IstanzaProcFirmaStoEntities.AddAsync(entityToInsert);
        }

        private async Task<Core.AggregateModels.TrasmissioneAggregate.Trasmissione> ExecuteTransmission(IstanzaProcessoDiFirma istanzaProcesso, IstanzaPassoDiFirma istanzaPasso, InfoDocumento infoDoc, InfoUtente infoUtente)
        {
            var idRuoloAsLong = istanzaPasso.RuoloCoinvolto.idAmministrazione.AsLong();
            string noteGenerali = string.Empty;
            string trasmType = "S";
            string receiverChaTipoUrp = "R";

            string notePasso = string.IsNullOrEmpty(istanzaProcesso.NoteDiAvvio) ? istanzaPasso.Note : (string.IsNullOrEmpty(istanzaPasso.Note)) ? istanzaProcesso.NoteDiAvvio : (istanzaProcesso.NoteDiAvvio + " - " + istanzaPasso.Note);

            string tipoPasso = string.Empty;
            if (istanzaPasso.Evento.TipoEvento.Equals("E"))
            {
                noteGenerali = "Azione richiesta " + istanzaPasso.Evento.Descrizione + ". " + notePasso;
                tipoPasso = istanzaPasso.Evento.Gruppo;
            }
            else
            {
                noteGenerali = notePasso;
                tipoPasso = istanzaPasso.Evento.CodiceAzione;
            }

            if (istanzaPasso.IsAutomatico)
                tipoPasso += "_AUTOMATICO";

            var ragioneEntity = await this._pi3DbContext.RagioneTrasmissioneEntities
                .Where(x => x.ID_AMM == idRuoloAsLong && x.CHA_PROC_RES == tipoPasso)
                .FirstOrDefaultAsync();

            if (ragioneEntity == null)
                throw new RestException("TRANSMISSION_REASON_NOT_FOUND");

            var ragione = new RagioneTrasmissione()
            {
                systemId = ragioneEntity.SYSTEM_ID.ToString(),
                descrizione = ragioneEntity.VAR_DESC_RAGIONE,
                tipo = ragioneEntity.CHA_TIPO_RAGIONE,
                tipoDiritti = (DocsPaVO.trasmissione.TipoDiritto)this.GetKeyFromVal(DocsPaVO.trasmissione.RagioneTrasmissione.tipoDirittoStringa, ragioneEntity.CHA_TIPO_DIRITTI),
                risposta = ragioneEntity.CHA_RISPOSTA,
                tipoDestinatario = (DocsPaVO.trasmissione.TipoGerarchia)this.GetKeyFromVal(DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa, ragioneEntity.CHA_TIPO_DEST),
                note = ragioneEntity.VAR_NOTE,
                eredita = ragioneEntity.CHA_EREDITA,
                tipoRisposta = ragioneEntity.CHA_TIPO_RISPOSTA,
                notifica = ragioneEntity.VAR_NOTIFICA_TRASM,
                testoMsgNotificaDoc = ragioneEntity.VAR_TESTO_MSG_NOTIFICA_DOC,
                testoMsgNotificaFasc = ragioneEntity.VAR_TESTO_MSG_NOTIFICA_FASC,
                prevedeCessione = ragioneEntity.CHA_CEDE_DIRITTI ?? "N",
                mantieniLettura = ragioneEntity.CHA_MANTIENI_LETT ?? "false",
                mantieniScrittura = ragioneEntity.CHA_MANTIENI_SCRITT ?? "false"
            };

            var documento = (await this._mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityCommand()
            {
                Infoutente = infoUtente,
                DocNumber = infoDoc.docNumber,
                IdProfile = infoDoc.docNumber
            })).Output;

            var aggregate = new Core.AggregateModels.TrasmissioneAggregate.Trasmissione(
                    infoUtente.idAmministrazione,
                    await _pi3DbContext.GetSystemDateTime(),
                    documento.systemId,
                    TipiOggettiTrasmessiEnum.DocumentoAmministrativo,
                    new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.Autore()
                    {
                        IdUtente = infoUtente.idPeople,
                        IdGruppo = infoUtente.idGruppo
                    },
                    new Core.SeedWork.TextValue()
                    {
                        Value = noteGenerali
                    }
                );


            CorrGlobaliEntity dest = (from a in _pi3DbContext.CorrGlobaliEntities where a.ID_GRUPPO == istanzaPasso.RuoloCoinvolto.idGruppo.AsLong() && a.DTA_FINE == null select a).FirstOrDefault();

            //receiverChaTipoUrp = dest.CHA_TIPO_URP;

            if (dest == null || dest.SYSTEM_ID == 0 ||
                dest.CHA_TIPO_IE == "E" || dest.DTA_FINE != null) throw new RestException("CORRESPONDENT_NOT_FOUND");

            var destUt = (await this._mediator.Send(new AddressbookGetCorrispondenteBySystemIdCommand()
            {
                SystemId = dest.SYSTEM_ID.ToString()
            })).Output;

            var tipoCorr = this._pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(w => w.SYSTEM_ID == dest.SYSTEM_ID).FirstOrDefault();

            //Se l'id utente titolare non � specificato, il documento � stato inserito  nel libro firma di tutti gli utenti del ruolo,
            //e quindi la trasmissione andr� notificata a tutti gli utenti, altrimenti al solo utente titolare
            List<DatiUtenteNotificatoTrasmissioneSingolaGruppo> utentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
            if (string.IsNullOrEmpty(istanzaPasso.UtenteCoinvolto.idPeople))
            {
                utentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                {
                    IdUtente = istanzaPasso.UtenteCoinvolto.idPeople,
                    UserId = istanzaPasso.UtenteCoinvolto.userId,
                    Cognome = istanzaPasso.UtenteCoinvolto.cognome,
                    Nome = istanzaPasso.UtenteCoinvolto.nome
                });
            }
            else
            {
                var usersToNotify = (await this.GetUsers(dest.VAR_COD_RUBRICA, dest.ID_AMM.ToString())).ToList();
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
                Tipo = TipiTrasmissioneSingolaEnum.Uno,
                UtentiNotificati = utentiNotificati
            };

            aggregate.PrepareTrasmissioneSingolaGruppo(datiG);
            await _trasmissioneRepository.Add(aggregate);

            var createdAggregate = await _trasmissioneRepository.Get(infoUtente.idAmministrazione, aggregate.Id);
            createdAggregate.Invia(DateTime.Now);

            await _trasmissioneRepository.Update(createdAggregate);

            if (createdAggregate.DataInvio != null)
            {
                var checkNotify = istanzaPasso.Evento.TipoEvento.Equals("E") ? true
                    : await (from p in this._pi3DbContext.PassoEventoEntities
                             join e in this._pi3DbContext.AnagraficaEventiEntities on p.ID_EVENTO equals e.ID_EVENTO
                             where p.ID_ISTANZA_PASSO == istanzaPasso.idIstanzaPasso.AsLong() && e.VAR_COD_AZIONE.Equals("INSERIMENTO_DOCUMENTO_LF")
                             select new { p, e })
                                      .AnyAsync();

                await _loggerService.LogOK("TRASM_DOC_" + ragione.descrizione.ToUpper().Replace(" ", "_"), createdAggregate.OggettoTrasmesso.Id, $"Trasmesso Documento {aggregate.OggettoTrasmesso.Id}", createdAggregate.TrasmissioniSingole[0].Id, infoUtente.codWorkingApplication, checkNotify);
            }
            else
            {

                await _loggerService.LogOK("TRASM_DOC_" + ragione.descrizione.ToUpper().Replace(" ", "_"), createdAggregate.OggettoTrasmesso.Id, $"Trasmesso Documento {aggregate.OggettoTrasmesso.Id}", createdAggregate.TrasmissioniSingole[0].Id, infoUtente.codWorkingApplication, false);
            }

            return createdAggregate;
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

        private async Task<IstanzaPassoDiFirma> GetNextIstanzaPasso(long idIstanzaProcesso, int numeroSequenza, long versionId)
        {
            /*Select Dpa_Istanza_Passo_Firma.*, Dpa_Anagrafica_Eventi.Var_Cod_Azione, Dpa_Anagrafica_Eventi.Descrizione, Dpa_Anagrafica_Eventi.Gruppo,
              Dpa_Istanza_Processo_Firma.Id_Ruolo_Proponente, Dpa_Istanza_Processo_Firma.Id_Utente_Proponente
              From Dpa_Istanza_Passo_Firma Inner Join Dpa_Istanza_Processo_Firma
              ON Dpa_Istanza_Passo_Firma.Id_Istanza_Processo = Dpa_Istanza_Processo_Firma.Id_Istanza
              Inner join Dpa_Anagrafica_Eventi on Dpa_Istanza_Passo_Firma.Tipo_Evento = Dpa_Anagrafica_Eventi.Id_Evento
              Where Dpa_Istanza_Processo_Firma.Id_Istanza = @idIstanzaProcesso@
              And Dpa_Istanza_Passo_Firma.numero_sequenza = @numeroSequenza@
              And Dpa_Istanza_Processo_Firma.VERSION_ID = @versionId@*/

            var nextIstanza = await (from Dpa_Istanza_Passo_Firma in this._pi3DbContext.IstanzaPassoFirmaEntities
                                     join Dpa_Istanza_Processo_Firma in this._pi3DbContext.IstanzaProcessoFirmaEntities
                                        on Dpa_Istanza_Passo_Firma.ID_ISTANZA_PROCESSO equals Dpa_Istanza_Processo_Firma.ID_ISTANZA
                                     join Dpa_Anagrafica_Eventi in this._pi3DbContext.AnagraficaEventiEntities
                                        on Dpa_Istanza_Passo_Firma.TIPO_EVENTO equals Dpa_Anagrafica_Eventi.ID_EVENTO
                                     join Dpa_Corr_Globali in this._pi3DbContext.CorrGlobaliEntities on Dpa_Istanza_Passo_Firma.ID_RUOLO_COINVOLTO equals Dpa_Corr_Globali.ID_GRUPPO
                                     join People in this._pi3DbContext.PeopleEntities on Dpa_Istanza_Passo_Firma.ID_UTENTE_COINVOLTO equals People.SYSTEM_ID
                                     where Dpa_Istanza_Processo_Firma.ID_ISTANZA == idIstanzaProcesso
                                        && Dpa_Istanza_Passo_Firma.NUMERO_SEQUENZA == (numeroSequenza + 1)
                                        && Dpa_Istanza_Processo_Firma.VERSION_ID == versionId
                                     select new
                                     {
                                         Dpa_Istanza_Passo_Firma,
                                         Dpa_Istanza_Processo_Firma,
                                         Dpa_Anagrafica_Eventi,
                                         Dpa_Corr_Globali,
                                         People
                                     })
                                   .FirstOrDefaultAsync();

            return new IstanzaPassoDiFirma()
            {
                CodiceTipoEvento = nextIstanza.Dpa_Anagrafica_Eventi.VAR_COD_AZIONE,
                dataEsecuzione = nextIstanza.Dpa_Istanza_Passo_Firma.ESEGUITO_IL.AsDateTimeFormat(),
                dataScadenza = nextIstanza.Dpa_Istanza_Passo_Firma.SCADENZA.AsDateTimeFormat(),
                descrizioneStatoPasso = nextIstanza.Dpa_Istanza_Passo_Firma.STATO_PASSO,
                idIstanzaPasso = nextIstanza.Dpa_Istanza_Passo_Firma.ID_ISTANZA_PASSO.ToString(),
                idIstanzaProcesso = nextIstanza.Dpa_Istanza_Passo_Firma.ID_ISTANZA_PROCESSO.ToString(),
                idNotificaEffettuata = nextIstanza.Dpa_Istanza_Passo_Firma.ID_NOTIFICA_EFFETTUATA.ToString(),
                idPasso = nextIstanza.Dpa_Istanza_Passo_Firma.ID_PASSO.ToString(),
                statoPasso = (TipoStatoPasso)Enum.Parse(typeof(TipoStatoPasso), nextIstanza.Dpa_Istanza_Passo_Firma.STATO_PASSO),
                motivoRespingimento = nextIstanza.Dpa_Istanza_Passo_Firma.MOTIVO_RESPINGIMENTO,
                Note = nextIstanza.Dpa_Istanza_Passo_Firma.NOTE,
                numeroSequenza = Convert.ToInt32(nextIstanza.Dpa_Istanza_Passo_Firma.NUMERO_SEQUENZA),
                RuoloCoinvolto = new Ruolo()
                {
                    idGruppo = nextIstanza.Dpa_Corr_Globali.ID_GRUPPO.ToString(),
                    codiceRubrica = nextIstanza.Dpa_Corr_Globali.VAR_COD_RUBRICA,
                    descrizione = nextIstanza.Dpa_Corr_Globali.VAR_DESC_CORR,
                    systemId = nextIstanza.Dpa_Corr_Globali.SYSTEM_ID.ToString()
                },
                TipoFirma = nextIstanza.Dpa_Istanza_Passo_Firma.TIPO_FIRMA,
                UtenteCoinvolto = new Utente()
                {
                    idPeople = nextIstanza.People.SYSTEM_ID.ToString(),
                    userId = nextIstanza.People.USER_ID,
                    descrizione = $"{nextIstanza.People.VAR_COGNOME} {nextIstanza.People.VAR_NOME}",
                    email = nextIstanza.People.EMAIL_ADDRESS,
                    notifica = nextIstanza.People.CHA_NOTIFICA,
                    amministratore = !string.IsNullOrEmpty(nextIstanza.People.CHA_AMMINISTRATORE) && nextIstanza.People.CHA_AMMINISTRATORE.Equals("1"),
                    assegnante = !string.IsNullOrEmpty(nextIstanza.People.CHA_AMMINISTRATORE) && nextIstanza.People.CHA_AMMINISTRATORE.Equals("1"),
                    assegnatario = !string.IsNullOrEmpty(nextIstanza.People.CHA_AMMINISTRATORE) && nextIstanza.People.CHA_AMMINISTRATORE.Equals("1"),
                    idAmministrazione = nextIstanza.People.ID_AMM.ToString(),
                    notificaConAllegato = !string.IsNullOrEmpty(nextIstanza.People.CHA_NOTIFICA_CON_ALLEGATO) && nextIstanza.People.CHA_NOTIFICA_CON_ALLEGATO.Equals("1"),
                    sede = nextIstanza.People.VAR_SEDE,
                    matricola = nextIstanza.People.MATRICOLA,
                    tipoCorrispondente = "P",
                    cognome = nextIstanza.People.VAR_COGNOME,
                    nome = nextIstanza.People.VAR_NOME
                },
                IsAutomatico = !string.IsNullOrEmpty(nextIstanza.Dpa_Istanza_Passo_Firma.CHA_AUTOMATICO) && nextIstanza.Dpa_Istanza_Passo_Firma.CHA_AUTOMATICO.Equals("1"),
                IdAOO = nextIstanza.Dpa_Istanza_Passo_Firma.ID_AOO.ToString(),
                IdRF = nextIstanza.Dpa_Istanza_Passo_Firma.ID_RF.ToString(),
                IdMailRegistro = nextIstanza.Dpa_Istanza_Passo_Firma.ID_MAIL_REGISTRO.ToString(),
                IdTipologia = nextIstanza.Dpa_Istanza_Passo_Firma.ID_TIPOLOGIA.ToString(),
                IdStatoDiagramma = nextIstanza.Dpa_Istanza_Passo_Firma.ID_STATO_DIAGRAMMA.ToString(),
                Errore = nextIstanza.Dpa_Istanza_Passo_Firma.VAR_ERRORE.ToString(),
                Evento = new Evento()
                {
                    IdEvento = nextIstanza.Dpa_Anagrafica_Eventi.ID_EVENTO.ToString(),
                    CodiceAzione = nextIstanza.Dpa_Anagrafica_Eventi.VAR_COD_AZIONE,
                    Descrizione = nextIstanza.Dpa_Anagrafica_Eventi.DESCRIZIONE,
                    TipoEvento = nextIstanza.Dpa_Anagrafica_Eventi.CHA_TIPO_EVENTO,
                    Gruppo = nextIstanza.Dpa_Anagrafica_Eventi.GRUPPO
                },
                ApplicaSegnaturaPermanente = nextIstanza.Dpa_Istanza_Passo_Firma.VAR_POS_SEGNATURA,
                PosizioneSegnaturaPermanente = nextIstanza.Dpa_Istanza_Passo_Firma.CHA_POS_SEGNATURA
            };
        }
        private async Task<bool> UpdateStatoIstanzaPasso(string idPasso, string versionId, string stato, InfoUtente infoUtente)
        {
            bool retValue = true;
            DocsPaVO.utente.Utente user;
            long idAmministrazioneAsLong = infoUtente.idAmministrazione?.AsLong() ?? 0;
            long idAmministrazioneDelegatoAsLong = infoUtente.delegato.idAmministrazione?.AsLong() ?? 0;

            if (!string.IsNullOrEmpty(idPasso))
            {
                try
                {
                    long idPassoAsLong = idPasso.AsLong();
                    string descUserLocker = string.Empty;

                    if (infoUtente.delegato != null)
                    {
                        //utente che delega
                        string descDelegante = string.Empty;
                        user = await GetUtente(infoUtente.userId, idAmministrazioneAsLong);
                        if (user != null)
                            descDelegante = user.descrizione;

                        //utente delegato
                        string descDelegato = string.Empty;
                        user = await GetUtente(infoUtente.delegato.userId, idAmministrazioneDelegatoAsLong);
                        if (user != null)
                            descDelegato = user.descrizione;
                        descUserLocker = descDelegato + " SOSTITUTO DI " + descDelegante;

                    }
                    else
                    {
                        string descUtente = string.Empty;
                        user = await GetUtente(infoUtente.userId, idAmministrazioneAsLong);
                        if (user != null)
                            descUtente = user.descrizione;
                        descUserLocker = descUtente;
                    }

                    /*UPDATE Dpa_Istanza_Passo_Firma SET STATO_PASSO = '@stato@'
                      @dataEvento@
                      Where Dpa_Istanza_Passo_Firma.ID_ISTANZA_PASSO = @Id_Passo@*/

                    var passoToUpdate = await this._pi3DbContext.IstanzaPassoFirmaEntities
                        .Where(x => x.ID_ISTANZA_PASSO == idPassoAsLong)
                        .FirstOrDefaultAsync();

                    if (passoToUpdate != null)
                    {
                        passoToUpdate.STATO_PASSO = stato;
                        await ((DbContext)_pi3DbContext).SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError($"Errore durante l'aggiornamento dell'istanza di passo: {idPasso}");
                    return false;
                }
            }

            return retValue;
        }
        private async Task<Utente> GetUtente(string userId, long idAmministrazione)
        {
            /*SELECT @param1@
              FROM PEOPLE P, DPA_CORR_GLOBALI A
              WHERE @param2@ AND NOT(P.DISABLED='Y') AND A.ID_PEOPLE = P.SYSTEM_ID AND A.ID_AMM = P.ID_AMM*/

            Utente utente = new Utente();

            var peopleCorrGlobaliEntity = await this._pi3DbContext.PeopleEntities
                .Join(this._pi3DbContext.CorrGlobaliEntities, p => p.SYSTEM_ID, a => a.ID_PEOPLE, (p, a) => new { p, a })
                .Where(x => x.p.DISABLED != "Y" && x.a.ID_AMM == idAmministrazione && x.p.USER_ID.ToUpper().Equals(userId.ToUpper()))
                .Select(x => new
                {
                    x.p.SYSTEM_ID,
                    x.p.USER_ID,
                    x.p.ID_AMM,
                    x.p.VAR_COGNOME,
                    x.p.VAR_NOME,
                    x.p.VAR_TELEFONO,
                    x.p.EMAIL_ADDRESS,
                    x.p.CHA_NOTIFICA,
                    x.p.CHA_AMMINISTRATORE,
                    x.p.CHA_NOTIFICA_CON_ALLEGATO,
                    x.p.VAR_SEDE,
                    x.p.MATRICOLA,
                    IDCORRGLOBALI = x.a.SYSTEM_ID
                })
                .FirstOrDefaultAsync();

            return new Utente()
            {
                idPeople = peopleCorrGlobaliEntity.SYSTEM_ID.ToString(),
                userId = peopleCorrGlobaliEntity.USER_ID,
                descrizione = $"{peopleCorrGlobaliEntity.VAR_COGNOME} {peopleCorrGlobaliEntity.VAR_NOME}",
                telefono = peopleCorrGlobaliEntity.VAR_TELEFONO,
                email = peopleCorrGlobaliEntity.EMAIL_ADDRESS,
                notifica = peopleCorrGlobaliEntity.CHA_NOTIFICA,
                amministratore = !string.IsNullOrEmpty(peopleCorrGlobaliEntity.CHA_AMMINISTRATORE) && peopleCorrGlobaliEntity.CHA_AMMINISTRATORE.Equals("1"),
                assegnante = !string.IsNullOrEmpty(peopleCorrGlobaliEntity.CHA_AMMINISTRATORE) && peopleCorrGlobaliEntity.CHA_AMMINISTRATORE.Equals("1"),
                assegnatario = !string.IsNullOrEmpty(peopleCorrGlobaliEntity.CHA_AMMINISTRATORE) && peopleCorrGlobaliEntity.CHA_AMMINISTRATORE.Equals("1"),
                idAmministrazione = peopleCorrGlobaliEntity.ID_AMM.ToString(),
                notificaConAllegato = !string.IsNullOrEmpty(peopleCorrGlobaliEntity.CHA_NOTIFICA_CON_ALLEGATO) && peopleCorrGlobaliEntity.CHA_NOTIFICA_CON_ALLEGATO.Equals("1"),
                sede = peopleCorrGlobaliEntity.VAR_SEDE,
                matricola = peopleCorrGlobaliEntity.MATRICOLA,
                tipoCorrispondente = "P",
                cognome = peopleCorrGlobaliEntity.VAR_COGNOME,
                nome = peopleCorrGlobaliEntity.VAR_NOME,
                systemId = peopleCorrGlobaliEntity.IDCORRGLOBALI.ToString()
            };
        }
        private async Task<List<IstanzaProcessoDiFirma>> GetInfoProcessesStartedForDocument(long idMainDocument)
        {
            /*SELECT I.ID_ISTANZA, I.ID_DOCUMENTO, I.DOC_ALL, I.NUM_ALL, I.DESCRIZIONE, P.VAR_PROF_OGGETTO
              FROM DPA_ISTANZA_PROCESSO_FIRMA I
              INNER JOIN PROFILE P
              ON I.ID_DOCUMENTO = P.DOCNUMBER
              WHERE I.CONCLUSO_IL IS NULL
              AND (P.ID_DOCUMENTO_PRINCIPALE = @docnumber@
              OR P.DOCNUMBER = @docnumber@)*/

            List<IstanzaProcessoDiFirma> toReturn = new List<IstanzaProcessoDiFirma>();

            var infoProcessesStartedForDocumentEntities = await this._pi3DbContext.IstanzaProcessoFirmaEntities
                    .Join(this._pi3DbContext.ProfileEntities, istanza => istanza.ID_DOCUMENTO, profile => profile.DOCNUMBER, (istanza, profile) => new { istanza, profile })
                    .Where(j => j.istanza.CONCLUSO_IL == null && (j.profile.ID_DOCUMENTO_PRINCIPALE == idMainDocument || j.profile.DOCNUMBER == idMainDocument))
                    .AsNoTracking()
                    .Select(j => new InfoProcessesStartedForDocumentEntity()
                    {
                        ID_ISTANZA = j.istanza.ID_ISTANZA,
                        ID_DOCUMENTO = j.istanza.ID_DOCUMENTO,
                        DOC_ALL = j.istanza.DOC_ALL,
                        NUM_ALL = j.istanza.NUM_ALL,
                        DESCRIZIONE = j.istanza.DESCRIZIONE,
                        VAR_PROF_OGGETTO = j.profile.VAR_PROF_OGGETTO
                    })
                    .ToListAsync();

            foreach (var i in infoProcessesStartedForDocumentEntities ?? new List<InfoProcessesStartedForDocumentEntity>())
            {
                toReturn.Add(new IstanzaProcessoDiFirma()
                {
                    idIstanzaProcesso = i.ID_ISTANZA.ToString(),
                    Descrizione = i.DESCRIZIONE,
                    docNumber = i.ID_DOCUMENTO.ToString(),
                    docAll = i.DOC_ALL,
                    numeroAllegato = i.NUM_ALL.ToString(),
                    oggetto = i.VAR_PROF_OGGETTO
                });
            }

            return toReturn;
        }
        private async Task<bool> CanExecuteTransmission(string docnumber, string idRuoloTitolare, string idUtenteTitolare, string idElemento)
        {
            _logger.LogDebug("Inizio Metodo CanExecuteTransmission");
            bool result = true;

            var number = docnumber.AsLong();
            var idRuolo = idRuoloTitolare.AsLong();
            var elementoId = idElemento.AsLong();

            try
            {
                var query = _pi3DbContext.ElementoInLibroFirmaEntities
                    .Where(e => e.ID_DOC_PRINCIPALE == number && e.ID_RUOLO_TITOLARE == idRuolo);

                if (string.IsNullOrEmpty(idUtenteTitolare))
                {
                    query = query.Where(e => e.ID_UTENTE_TITOLARE == null);
                }
                else
                {
                    var idUtente = idUtenteTitolare.AsLong();
                    query = query.Where(e => e.ID_UTENTE_TITOLARE == null || e.ID_UTENTE_TITOLARE == idUtente);
                }

                if (!string.IsNullOrEmpty(idElemento))
                {
                    query = query.Where(e => e.ID_ELEMENTO != elementoId);
                }

                var elemento = await query.Select(e => new
                {
                    e.ID_TRASM_SINGOLA,
                    e.DTA_ACCETTAZIONE
                }).FirstAsync();

                // Se esiste un risultato, significa che una trasmissione esiste gi�
                if (elemento != null)
                {
                    if (!string.IsNullOrEmpty(idElemento))
                    {
                        var idTrasmSingola = elemento.ID_TRASM_SINGOLA;
                        var dtaAccettata = elemento.DTA_ACCETTAZIONE;

                        await UpdateIdTrasmInElementoLF(idElemento, idTrasmSingola, dtaAccettata.ToString());
                    }

                    result = false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Errore nel Metodo CanExecuteTransmission: " + e.Message);
                return false;
            }

            _logger.LogDebug("Fine Metodo CanExecuteTransmission");
            return result;
        }
        private async Task<bool> UpdateIdTrasmInElementoLF(string idElemento, long? idTrasmSingola, string? dtaAccettazione)
        {
            var elementoId = idElemento.AsLong();
            _logger.LogDebug("Inizio Metodo UpdateIdTrasmInElementoLF");
            bool result = false;

            try
            {
                // Recupero l'elemento da aggiornare dal contesto EF
                var elemento = await _pi3DbContext.ElementoInLibroFirmaEntities
                    .FirstOrDefaultAsync(e => e.ID_ELEMENTO == elementoId);

                if (elemento != null)
                {
                    // Aggiorno i campi necessari
                    elemento.ID_TRASM_SINGOLA = idTrasmSingola;

                    if (string.IsNullOrEmpty(dtaAccettazione))
                    {
                        elemento.DTA_ACCETTAZIONE = null;
                    }
                    else
                    {
                        elemento.DTA_ACCETTAZIONE = DateTime.ParseExact(dtaAccettazione, "dd/MM/yyyy HH:mm:ss", null);
                    }

                    // Salvo le modifiche nel contesto
                    await ((DbContext)_pi3DbContext).SaveChangesAsync();
                }
                else
                {
                    throw new Exception($"Elemento con ID {idElemento} non trovato.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Errore nel Metodo UpdateIdTrasmInElementoLF: " + ex.Message);
                return false;
            }

            _logger.LogDebug("Fine Metodo UpdateIdTrasmInElementoLF");
            return result;
        }
        private async Task<string> InsertElementoInLibroFirma(IstanzaProcessoDiFirma istanzaProcesso, InfoUtente infoUtente, string modalita)
        {
            string retval = string.Empty;
            if (istanzaProcesso != null)
            {
                try
                {
                    string TipoFirma = string.Empty;
                    string idStatoPasso = string.Empty;
                    string motivoRespingimento = string.Empty;
                    string dataScadenza = string.Empty;
                    string RuoloCoinvolto = string.Empty;
                    string UtenteCoinvolto = string.Empty;
                    string Id_istanza_passo = string.Empty;
                    string note = string.Empty;

                    var idIstanzaProcesso = istanzaProcesso.idIstanzaProcesso.AsLong();
                    var idVersione = istanzaProcesso.versionId.AsLong();

                    var result = await (from passoFirma in _pi3DbContext.IstanzaPassoFirmaEntities
                                        join processoFirma in _pi3DbContext.IstanzaProcessoFirmaEntities
                                        on passoFirma.ID_ISTANZA_PROCESSO equals processoFirma.ID_ISTANZA
                                        join evento in _pi3DbContext.AnagraficaEventiEntities
                                        on passoFirma.TIPO_EVENTO equals evento.ID_EVENTO
                                        where processoFirma.ID_ISTANZA == idIstanzaProcesso
                                        && passoFirma.NUMERO_SEQUENZA == 1
                                        && processoFirma.VERSION_ID == idVersione
                                        select new IstanzaPassoDiFirma
                                        {
                                            // Propriet� di IstanzaPassoFirma
                                            idIstanzaPasso = passoFirma.ID_ISTANZA_PASSO.ToString(),
                                            idIstanzaProcesso = passoFirma.ID_ISTANZA_PROCESSO.ToString(),
                                            idPasso = passoFirma.ID_PASSO.ToString(),
                                            statoPasso = ImpostaTipoStato(passoFirma.STATO_PASSO.ToString()),
                                            TipoFirma = passoFirma.TIPO_FIRMA.ToString(),
                                            IdTipoEvento = passoFirma.TIPO_EVENTO.ToString(),
                                            CodiceTipoEvento = evento.VAR_COD_AZIONE.ToString(),
                                            dataEsecuzione = passoFirma.ESEGUITO_IL.HasValue ? passoFirma.ESEGUITO_IL.Value.ToString() : string.Empty,
                                            dataScadenza = passoFirma.SCADENZA.HasValue ? passoFirma.SCADENZA.Value.ToString() : string.Empty,
                                            motivoRespingimento = passoFirma.MOTIVO_RESPINGIMENTO.ToString(),
                                            numeroSequenza = Convert.ToInt32(passoFirma.NUMERO_SEQUENZA),
                                            Note = passoFirma.NOTE.ToString(),
                                            IdAOO = passoFirma.ID_AOO.ToString(),
                                            IdRF = passoFirma.ID_RF.ToString(),
                                            IdMailRegistro = passoFirma.ID_MAIL_REGISTRO.ToString(),
                                            IdTipologia = passoFirma.ID_TIPOLOGIA.ToString(),
                                            IdStatoDiagramma = passoFirma.ID_STATO_DIAGRAMMA.ToString(),
                                            IsAutomatico = passoFirma.CHA_AUTOMATICO == "1",
                                            ApplicaSegnaturaPermanente = passoFirma.CHA_POS_SEGNATURA.ToString(),
                                            PosizioneSegnaturaPermanente = passoFirma.VAR_POS_SEGNATURA.ToString(),

                                            Evento = new Evento
                                            {
                                                IdEvento = evento.ID_EVENTO.ToString(),
                                                CodiceAzione = evento.VAR_COD_AZIONE.ToString(),
                                                Descrizione = evento.DESCRIZIONE.ToString(),
                                                TipoEvento = evento.CHA_TIPO_EVENTO.ToString(),
                                                Gruppo = evento.GRUPPO.ToString()
                                            },

                                            RuoloCoinvolto = new DocsPaVO.utente.Ruolo
                                            {
                                                idGruppo = passoFirma.ID_RUOLO_COINVOLTO.ToString(),

                                            },

                                            UtenteCoinvolto = new DocsPaVO.utente.Utente
                                            {
                                                idPeople = passoFirma.ID_UTENTE_COINVOLTO.ToString()
                                            }
                                        }).FirstOrDefaultAsync();
                    if (result is not null)
                    {
                        TipoFirma = result.TipoFirma ?? string.Empty;
                        idStatoPasso = !string.IsNullOrEmpty(result.statoPasso.ToString()) ? result.statoPasso.ToString() : string.Empty;
                        motivoRespingimento = result.motivoRespingimento ?? string.Empty;
                        dataScadenza = !string.IsNullOrEmpty(result.dataScadenza) ? result.dataScadenza.ToString() : string.Empty;
                        RuoloCoinvolto = result.RuoloCoinvolto.idGruppo ?? string.Empty;
                        UtenteCoinvolto = result.UtenteCoinvolto.systemId ?? string.Empty;
                        Id_istanza_passo = result.idIstanzaPasso ?? string.Empty;
                        note = result.Note ?? string.Empty;
                    }

                    var docNumber = istanzaProcesso.docNumber;

                    var versionId = await _pi3DbContext.VersionEntities
                    .Where(v => v.DOCNUMBER == docNumber.AsLong())
                    .MaxAsync(v => v.VERSION_ID);

                    var documento = await _pi3DbContext.ProfileEntities
                     .Where(p => p.SYSTEM_ID == docNumber.AsLong())
                     .Select(p => new
                     {
                         IdDocumentoPrincipale = p.ID_DOCUMENTO_PRINCIPALE,
                         SystemId = p.SYSTEM_ID
                     }).FirstOrDefaultAsync();

                    var IdPeopleProponenteDelegato = string.Empty;
                    if (infoUtente.delegato != null && !string.IsNullOrEmpty(infoUtente.delegato.idPeople))
                    {
                        IdPeopleProponenteDelegato = infoUtente.delegato.idPeople;
                    }
                    else
                    {
                        IdPeopleProponenteDelegato = "0";
                    }

                    var ruolo = RuoloCoinvolto.AsLong();
                    var numberDoc = istanzaProcesso.docNumber.AsLong();
                    var version = istanzaProcesso.versionId.AsLong();
                    var numAll = istanzaProcesso.numeroAllegato.AsLong();
                    long? utenteTitolare = !string.IsNullOrEmpty(UtenteCoinvolto) ? UtenteCoinvolto.AsLong() : null;
                    var istProcesso = istanzaProcesso.idIstanzaProcesso.AsLong();
                    var istPasso = Id_istanza_passo.AsLong();
                    var proponenteDelegato = IdPeopleProponenteDelegato.AsLong();


                    string noteElemento = string.IsNullOrEmpty(istanzaProcesso.NoteDiAvvio) ? note : (string.IsNullOrEmpty(note) ? istanzaProcesso.NoteDiAvvio : istanzaProcesso.NoteDiAvvio + " - " + note);
                    var elementoInLibroFirma = new ElementoInLibroFirmaEntity
                    {
                        ID_RUOLO_TITOLARE = ruolo,
                        TIPO_FIRMA = TipoFirma,
                        STATO_FIRMA = TipoStatoElemento.PROPOSTO.ToString(),
                        NOTE = noteElemento.Replace("'", "''"),
                        RUOLO_PROPONENTE = istanzaProcesso.RuoloProponente.idGruppo,
                        UTENTE_PROPONENTE = istanzaProcesso.UtenteProponente.idPeople,
                        MODALITA = modalita,
                        DATA_INSERIMENTO = DateTime.Now,
                        DOC_NUMBER = numberDoc,
                        VERSION_ID = version,
                        NUM_ALL = numAll,
                        NUM_VERSIONE = istanzaProcesso.numeroVersione,
                        ID_UTENTE_TITOLARE = utenteTitolare.HasValue ? utenteTitolare : null,
                        ID_UTENTE_LOCKER = null,
                        ISTANZA_PROCESSO = istProcesso,
                        ID_ISTANZA_PASSO = istPasso,
                        ID_PEOPLE_PROPONENTE_DELEGATO = proponenteDelegato,
                        ID_DOC_PRINCIPALE = documento.IdDocumentoPrincipale ?? documento.SystemId
                    };
                    await _pi3DbContext.ElementoInLibroFirmaEntities.AddAsync(elementoInLibroFirma);
                    await ((DbContext)_pi3DbContext).SaveChangesAsync();
                    retval = elementoInLibroFirma.ID_ELEMENTO.ToString();

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
            return retval;
        }

        private async Task<IstanzaPassoDiFirma> GetIstanzaPassoDiFirmaInAttesa(string idIstanzaProcesso)
        {
            var statoPasso = TipoStatoPasso.LOOK.ToString();
            IstanzaPassoDiFirma istanzaPassoDiFirmaEntity = null!;
            IstanzaPassoDiFirma istanzaPassoDiFirma = null!;
            var idIstanza = idIstanzaProcesso.AsLong();
            try
            {
                var query = await (from istanzaPassoFirma in _pi3DbContext.IstanzaPassoFirmaEntities
                                   join anagraficaEventi in _pi3DbContext.AnagraficaEventiEntities
                                   on istanzaPassoFirma.TIPO_EVENTO equals anagraficaEventi.ID_EVENTO
                                   where istanzaPassoFirma.ID_ISTANZA_PROCESSO == idIstanza
                                         && istanzaPassoFirma.STATO_PASSO == statoPasso
                                   select new
                                   {
                                       anagraficaEventi.VAR_COD_AZIONE,
                                       istanzaPassoFirma.ESEGUITO_IL,
                                       istanzaPassoFirma.SCADENZA,
                                       istanzaPassoFirma.ID_ISTANZA_PASSO,
                                       istanzaPassoFirma.ID_ISTANZA_PROCESSO,
                                       istanzaPassoFirma.ID_NOTIFICA_EFFETTUATA,
                                       istanzaPassoFirma.ID_PASSO,
                                       istanzaPassoFirma.MOTIVO_RESPINGIMENTO,
                                       istanzaPassoFirma.NUMERO_SEQUENZA,
                                       istanzaPassoFirma.NOTE,
                                       istanzaPassoFirma.TIPO_FIRMA,
                                       istanzaPassoFirma.ID_AOO,
                                       istanzaPassoFirma.ID_RF,
                                       istanzaPassoFirma.ID_MAIL_REGISTRO,
                                       istanzaPassoFirma.ID_TIPOLOGIA,
                                       istanzaPassoFirma.ID_STATO_DIAGRAMMA,
                                       istanzaPassoFirma.CHA_AUTOMATICO,
                                       anagraficaEventi.ID_EVENTO,
                                       anagraficaEventi.DESCRIZIONE,
                                       anagraficaEventi.CHA_TIPO_EVENTO,
                                       anagraficaEventi.GRUPPO,
                                       istanzaPassoFirma.ID_RUOLO_COINVOLTO,
                                       istanzaPassoFirma.ID_UTENTE_COINVOLTO,
                                       istanzaPassoFirma.CHA_POS_SEGNATURA,
                                       istanzaPassoFirma.VAR_POS_SEGNATURA
                                   }).FirstOrDefaultAsync();


                istanzaPassoDiFirma = new IstanzaPassoDiFirma()
                {
                    CodiceTipoEvento = query.VAR_COD_AZIONE,
                    dataEsecuzione = query.ESEGUITO_IL.HasValue ? query.ESEGUITO_IL.Value.ToString() : string.Empty,
                    dataScadenza = query.SCADENZA.HasValue ? query.SCADENZA.Value.ToString() : string.Empty,
                    idIstanzaPasso = query.ID_ISTANZA_PASSO.ToString(),
                    idIstanzaProcesso = query.ID_ISTANZA_PROCESSO.ToString(),
                    idNotificaEffettuata = query.ID_NOTIFICA_EFFETTUATA.HasValue
              ? query.ID_NOTIFICA_EFFETTUATA.Value.ToString()
              : string.Empty,
                    idPasso = query.ID_PASSO.ToString(),
                    motivoRespingimento = !string.IsNullOrEmpty(query.MOTIVO_RESPINGIMENTO)
              ? query.MOTIVO_RESPINGIMENTO
              : string.Empty,
                    numeroSequenza = Convert.ToInt32(query.NUMERO_SEQUENZA),
                    Note = !string.IsNullOrEmpty(query.NOTE) ? query.NOTE : string.Empty,
                    TipoFirma = query.TIPO_FIRMA,
                    IdAOO = query.ID_AOO.ToString(),
                    IdRF = query.ID_RF.ToString(),
                    IdMailRegistro = query.ID_MAIL_REGISTRO.ToString(),
                    IdTipologia = query.ID_TIPOLOGIA.ToString(),
                    IdStatoDiagramma = query.ID_STATO_DIAGRAMMA.ToString(),
                    IsAutomatico = query.CHA_AUTOMATICO == "1",
                    Evento = new Evento
                    {
                        IdEvento = query.ID_EVENTO.ToString(),
                        CodiceAzione = query.VAR_COD_AZIONE,
                        Descrizione = query.DESCRIZIONE,
                        TipoEvento = query.CHA_TIPO_EVENTO,
                        Gruppo = query.GRUPPO
                    },
                    RuoloCoinvolto = new Ruolo
                    {
                        idGruppo = !string.IsNullOrEmpty(query.ID_RUOLO_COINVOLTO.ToString()) ? query.ID_RUOLO_COINVOLTO.ToString() : string.Empty
                    },
                    UtenteCoinvolto = new Utente
                    {
                        idPeople = !string.IsNullOrEmpty(query.ID_UTENTE_COINVOLTO.ToString()) ? query.ID_UTENTE_COINVOLTO.ToString() : string.Empty
                    },
                    ApplicaSegnaturaPermanente = query.CHA_POS_SEGNATURA,
                    PosizioneSegnaturaPermanente = query.VAR_POS_SEGNATURA
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return istanzaPassoDiFirma;
        }

        private static TipoStatoPasso ImpostaTipoStato(string STATO_PASSO)
        {
            return (TipoStatoPasso)Enum.Parse(typeof(TipoStatoPasso), STATO_PASSO);
        }

        private async Task RollbackStartProcessoDiFirma(IstanzaProcessoDiFirma istanzaProcesso, DocsPaVO.documento.FileRequest file)
        {
            if (istanzaProcesso != null)
            {
                var istanzaPassoFirmaDaEliminare = _pi3DbContext.IstanzaPassoFirmaEntities.FirstOrDefault(i => i.ID_ISTANZA_PROCESSO == istanzaProcesso.idProcesso.AsLong());
                _logger.LogDebug("RollbackStartProcessoDiFirma fase 1: eliminazione Istanza Passo Firma");

                if (istanzaPassoFirmaDaEliminare is not null)
                {
                    try
                    {
                        // Rimuove l'entit� dal contesto
                        _pi3DbContext.IstanzaPassoFirmaEntities.Remove(istanzaPassoFirmaDaEliminare);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
                var istanzaProcessoFirmaDaEliminare = _pi3DbContext.IstanzaProcessoFirmaEntities.FirstOrDefault(i => i.ID_ISTANZA == istanzaProcesso.idProcesso.AsLong());
                _logger.LogDebug("RollbackStartProcessoDiFirma fase 2: eliminazione Istanza Processo Firma");
                if (istanzaProcessoFirmaDaEliminare is not null)
                {
                    try
                    {
                        // Rimuove l'entit� dal contesto
                        _pi3DbContext.IstanzaProcessoFirmaEntities.Remove(istanzaProcessoFirmaDaEliminare);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }

                var elementoInLibroFirmaDaEliminare = _pi3DbContext.ElementoInLibroFirmaEntities.FirstOrDefault(i => i.ISTANZA_PROCESSO == istanzaProcesso.idProcesso.AsLong());
                _logger.LogDebug("RollbackStartProcessoDiFirma fase 2: eliminazione Elemento In Libro Firma");
                if (elementoInLibroFirmaDaEliminare is not null)
                {
                    try
                    {
                        // Rimuove l'entit� dal contesto
                        _pi3DbContext.ElementoInLibroFirmaEntities.Remove(elementoInLibroFirmaDaEliminare);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }

            }
            if (file != null)
            {
                await UpdateLockDocument(file.docNumber, "0");
            }
        }

        private async Task<bool> UpdateLockDocument(string docNumber, string stato)
        {
            if (!string.IsNullOrEmpty(docNumber))
            {
                try
                {
                    var elementi = await (_pi3DbContext.ProfileEntities
                      .Where(x => x.DOCNUMBER == docNumber.AsLong() && (x.IN_LIBROFIRMA == null ? "0" != stato : x.IN_LIBROFIRMA != stato))
                      .ToListAsync());
                    foreach (var elemento in elementi)
                    {
                        elemento.IN_LIBROFIRMA = stato;
                    }
                    await ((DbContext)_pi3DbContext).SaveChangesAsync();
                    return true;
                }
                catch (Exception ex) { _logger.LogError(ex, ex.Message); }
            }
            return false;
        }

        private async Task<IstanzaProcessoDiFirma> CreateIstanzaFromProcesso(ProcessoFirma processoDiFirma, DocsPaVO.documento.FileRequest file, DocsPaVO.utente.InfoUtente infoUtente, string note, OpzioniNotifica opzioniNotifiche, bool daCambioStato)
        {
            //IstanzaProcessoDiFirma istanzaProcesso = null!;
            List<PassoFirma> passiDaNonIncludere = null!;
            IstanzaProcessoDiFirma istanzaProcesso = null!;

            _logger.LogDebug("Inizio Metodo CreateIstanzaFromProcesso in StartSignature");

            if (processoDiFirma is not null && file is not null)
            {
                try
                {
                    string strDocAll = "D";
                    if (file.GetType().Equals(typeof(DocsPaVO.documento.Allegato)))
                        strDocAll = "A";
                    string dataDB = (await _pi3DbContext.GetSystemDateTime()).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);


                    long newIdProcessoDiFirma = await InserisciIstanzaProcessoFirma(processoDiFirma, infoUtente, file, opzioniNotifiche, note, daCambioStato);

                    await InserisciIstanzaPassoFirma(newIdProcessoDiFirma, processoDiFirma, infoUtente.idGruppo, TipoStatoPasso.NEW.ToString(), strDocAll);

                    await UpdatePassoFirma(processoDiFirma, newIdProcessoDiFirma);

                    await UpdateStateIstanzaPassoNum(newIdProcessoDiFirma, "1", TipoStatoPasso.LOOK.ToString());

                    istanzaProcesso = new IstanzaProcessoDiFirma()
                    {
                        idIstanzaProcesso = newIdProcessoDiFirma.ToString(),
                        idProcesso = processoDiFirma.idProcesso,
                        statoProcesso = DocsPaVO.LibroFirma.TipoStatoProcesso.IN_EXEC,
                        dataAttivazione = dataDB,
                        dataChiusura = null,
                        RuoloProponente = DBUtils.getRuoloById(infoUtente.idCorrGlobali, _pi3DbContext),
                        UtenteProponente = DBUtils.getUtenteById(infoUtente.idPeople, _pi3DbContext),
                        docNumber = file.docNumber,
                        versionId = file.versionId,
                        docAll = strDocAll,
                        numeroAllegato = file.versionLabel,
                        numeroVersione = !string.IsNullOrEmpty(file.version) ? Convert.ToInt32(file.version) : 0,
                        Descrizione = processoDiFirma.nome,
                        NoteDiAvvio = note
                    };

                    /* Commentato - dava errore nella insert perch� nella DPA_PASSO_DPA_EVENTO non c'� una chiave primaria
                    await InsertIdEventoEIdIstanzaPassoInPassoEvento(newIdProcessoDiFirma);
                    */
                    //vado di raw swl
                    var rowsAffected = await ((DbContext)this._pi3DbContext).Database.ExecuteSqlAsync($"INSERT INTO DPA_PASSO_DPA_EVENTO (ID_EVENTO, ID_ISTANZA_PASSO) SELECT P.ID_EVENTO, I.ID_ISTANZA_PASSO FROM DPA_ISTANZA_PASSO_FIRMA I JOIN DPA_PASSO_DPA_EVENTO P ON P.ID_PASSO = I.ID_PASSO WHERE I.ID_ISTANZA_PROCESSO = {newIdProcessoDiFirma}");
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
            return istanzaProcesso;
        }

        private async Task InsertIdEventoEIdIstanzaPassoInPassoEvento(long idIstanza)
        {

            var query = _pi3DbContext.IstanzaPassoFirmaEntities.AsNoTracking().Join(_pi3DbContext.PassoEventoEntities.AsNoTracking(), i => i.ID_PASSO, p => p.ID_PASSO, (i, p) => new { i, p })
                            .Where(joined => joined.i.ID_ISTANZA_PROCESSO == idIstanza)
                            .Select(joined => new
                            {
                                ID_EVENTO = joined.p.ID_EVENTO,
                                ID_ISTANZA_PASSO = joined.i.ID_ISTANZA_PASSO
                            });


            foreach (var result in query)
            {
                var newRecord = new PassoEventoEntity
                {
                    ID_EVENTO = result.ID_EVENTO,
                    ID_ISTANZA_PASSO = result.ID_ISTANZA_PASSO
                };

                await _pi3DbContext.PassoEventoEntities.AddAsync(newRecord);
            }

            await ((DbContext)_pi3DbContext).SaveChangesAsync();
        }

        private async Task<DocsPaVO.documento.InfoDocumento> GetInfoDocumentoLite(string idProfile)
        {
            _logger.LogDebug("BEGIN");
            var query = _pi3DbContext.ProfileEntities
                .Where(a => (a.SYSTEM_ID == idProfile.AsLong() && a.ID_DOCUMENTO_PRINCIPALE == null)
                || _pi3DbContext.ProfileEntities.Any(b => b.SYSTEM_ID == idProfile.AsLong() && a.SYSTEM_ID == b.ID_DOCUMENTO_PRINCIPALE))
                .Select(a => new
                {
                    a.SYSTEM_ID,
                    a.ID_DOCUMENTO_PRINCIPALE,
                    a.DOCNUMBER,
                    a.VAR_PROF_OGGETTO,
                    a.CHA_TIPO_PROTO,
                    a.NUM_PROTO,
                    a.VAR_SEGNATURA
                });

            DocsPaVO.documento.InfoDocumento toReturn = new();

            var risultato = await query.FirstOrDefaultAsync();
            if (risultato is not null)
            {
                toReturn.docNumber = risultato.SYSTEM_ID.ToString();
                toReturn.idProfile = risultato.DOCNUMBER.ToString();
                toReturn.oggetto = risultato.VAR_PROF_OGGETTO.ToString();
                toReturn.tipoProto = !string.IsNullOrEmpty(risultato.CHA_TIPO_PROTO) ? risultato.CHA_TIPO_PROTO : string.Empty;



            }
            return toReturn;
        }
        private async Task<long> InserisciIstanzaProcessoFirma(ProcessoFirma processoDiFirma, InfoUtente infoUtente, DocsPaVO.documento.FileRequest file, OpzioniNotifica opzioniNotifiche, string note, bool daCambioStato)
        {
            long idToReturn = 0;
            //IstanzaProcessoDiFirma retval = null!;
            try
            {

                // Ottieni la data dal database
                string dataDB = (await _pi3DbContext.GetSystemDateTime()).ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                // Crea una nuova istanza del processo
                var istanzaProcesso = new IstanzaProcessoFirmaEntity
                {
                    ID_PROCESSO = processoDiFirma.idProcesso.AsLong(),
                    STATO = TipoStatoProcesso.IN_EXEC.ToString(),
                    ATTIVATO_IL = await _pi3DbContext.GetSystemDateTime(),//DateTime.Parse(dataDB),
                    CHA_INTERROTTO_DA = null,
                    ID_RUOLO_PROPONENTE = infoUtente.idGruppo.AsLong(),
                    ID_UTENTE_PROPONENTE = infoUtente.idPeople.AsLong(),
                    ID_DOCUMENTO = file.docNumber.AsLong(),
                    VERSION_ID = file.versionId.AsLong(),
                    NOTIFICA_INTERROTTO = opzioniNotifiche.Notifica_interrotto ? "1" : "0",
                    NOTIFICA_CONCLUSO = opzioniNotifiche.Notifica_concluso ? "1" : "0",
                    NOTIFICA_ERRORE = opzioniNotifiche.NotificaErrore ? "1" : "0",
                    NOTIFICA_DEST_NON_INTEROP = opzioniNotifiche.NotificaPresenzaDestNonInterop ? "1" : "0",
                    NOTE = note.Replace("'", "''"),
                    DOC_ALL = file.GetType().Equals(typeof(DocsPaVO.documento.Allegato)) ? "A" : "D",
                    NUM_ALL = file.versionLabel.Replace("A", "").AsLong(),
                    DESCRIZIONE = processoDiFirma.nome.Replace("'", "''"),
                    CHA_CAMBIO_STATO_DIAG = daCambioStato ? "1" : "0",
                    ID_STATO_INTERRUZIONE = string.IsNullOrEmpty(processoDiFirma.IdStatoInterruzione) ? (long?)null : processoDiFirma.IdStatoInterruzione.AsLong()
                };

                // Gestione del delegato
                if (infoUtente.delegato != null && !string.IsNullOrEmpty(infoUtente.delegato.idPeople))
                {
                    istanzaProcesso.ID_UTENTE_PROPONENTE = infoUtente.delegato.idPeople.AsLong();
                }
                else
                {
                    istanzaProcesso.ID_PEOPLE_DELEGATO = 0;
                }

                // Gestione della versione
                bool isNumeric = int.TryParse(file.version, out int versione);
                istanzaProcesso.NUM_VERSIONE = isNumeric ? versione : 1;


                // Aggiungi e salva l'istanza del processo nel contesto
                await _pi3DbContext.IstanzaProcessoFirmaEntities.AddAsync(istanzaProcesso);
                await ((DbContext)_pi3DbContext).SaveChangesAsync();

                idToReturn = istanzaProcesso.ID_ISTANZA;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return idToReturn;
        }


        private async Task<long?> GetRoleSupByIdRoleAndTypeRole(long idGruppo, long idTipoRuolo)
        {
            var idUO = await _pi3DbContext.CorrGlobaliEntities
                .Where(dcg => dcg.ID_GRUPPO == idGruppo)
                .Select(dcg => dcg.ID_UO)
                .FirstOrDefaultAsync();

            var risultato = await _pi3DbContext.CorrGlobaliEntities
                .Where(dcg => dcg.ID_TIPO_RUOLO == idTipoRuolo && dcg.DTA_FINE == null)
                .Where(dcg => _pi3DbContext.CorrGlobaliEntities
                    .Where(subDcg => subDcg.DTA_FINE == null && subDcg.SYSTEM_ID == dcg.ID_UO)
                    .Any())
                .Select(dcg => dcg.ID_GRUPPO)
                .FirstOrDefaultAsync();

            return risultato;

        }
        private async Task InserisciIstanzaPassoFirma(long newIdProcessoDiFirma, ProcessoFirma processoDiFirma, string idGruppo, string statoPasso, string strDocAll)
        {
            string condizionePassiDaNonIncludere = string.Empty;
            List<long> passiDaNonIncludereIds = new();

            if (processoDiFirma.IsProcessModel)
            {
                var passiDaNonIncludere = (from p in processoDiFirma.passi
                                           where p.IsFacoltativo && p.IncludiInIstanzaPasso == IncludiInIstanzaPasso.NO
                                           select p).ToList();

                if (passiDaNonIncludere != null && passiDaNonIncludere.Count > 0)
                {
                    condizionePassiDaNonIncludere = string.Join(" ,", passiDaNonIncludere.Select(p => p.idPasso));

                    passiDaNonIncludereIds = passiDaNonIncludere.Select(p => p.idPasso.AsLong()).ToList();
                }
            }

            var passi = await (from p in _pi3DbContext.PassoDiFirmaEntities
                               join e in _pi3DbContext.AnagraficaEventiEntities
                               on p.TIPO_EVENTO equals e.ID_EVENTO
                               where p.ID_PROCESSO == processoDiFirma.idProcesso.AsLong()
                               && (strDocAll != "A" || !new[] { "W", "E" }.Contains(e.CHA_TIPO_EVENTO))
                               // Escludi i passi usando la lista degli ID
                               && (!passiDaNonIncludereIds.Any() || !passiDaNonIncludereIds.Contains(p.ID_PASSO))
                               select new
                               {
                                   p.ID_PASSO,
                                   p.ID_TIPO_RUOLO_COINVOLTO,
                                   p.ID_RUOLO_COINVOLTO,
                                   p.ID_UTENTE_COINVOLTO,
                                   p.TIPO_FIRMA,
                                   p.SCADENZA,
                                   p.TIPO_EVENTO,
                                   p.NOTE,
                                   p.CHA_AUTOMATICO,
                                   p.ID_AOO,
                                   p.ID_RF,
                                   p.ID_MAIL_REGISTRO,
                                   p.ID_TIPOLOGIA,
                                   p.ID_STATO_DIAGRAMMA,
                                   p.VAR_POS_SEGNATURA,
                                   p.CHA_POS_SEGNATURA
                               })
             .OrderBy(p => p.ID_PASSO)
             .ToListAsync();

            foreach (var passo in passi)
            {
                var idRuoloCoinvolto = passo.ID_TIPO_RUOLO_COINVOLTO == null
                    ? passo.ID_RUOLO_COINVOLTO
                    : await (GetRoleSupByIdRoleAndTypeRole(idGruppo.AsLong(), passo.ID_TIPO_RUOLO_COINVOLTO.Value));

                var newRecord = new IstanzaPassoFirmaEntity
                {

                    ID_ISTANZA_PROCESSO = newIdProcessoDiFirma,
                    ID_PASSO = passo.ID_PASSO,
                    STATO_PASSO = statoPasso,
                    ESEGUITO_IL = null,
                    MOTIVO_RESPINGIMENTO = null,
                    ID_RUOLO_COINVOLTO = idRuoloCoinvolto,
                    ID_UTENTE_COINVOLTO = passo.ID_UTENTE_COINVOLTO,
                    TIPO_FIRMA = passo.TIPO_FIRMA,
                    SCADENZA = passo.SCADENZA,
                    TIPO_EVENTO = passo.TIPO_EVENTO,
                    NUMERO_SEQUENZA = passi.IndexOf(passo) + 1,
                    ID_NOTIFICA_EFFETTUATA = null,
                    NOTE = passo.NOTE,
                    CHA_AUTOMATICO = passo.CHA_AUTOMATICO,
                    ID_AOO = passo.ID_AOO,
                    ID_RF = passo.ID_RF,
                    ID_MAIL_REGISTRO = passo.ID_MAIL_REGISTRO,
                    ID_TIPOLOGIA = passo.ID_TIPOLOGIA,
                    ID_STATO_DIAGRAMMA = passo.ID_STATO_DIAGRAMMA,
                    VAR_POS_SEGNATURA = passo.VAR_POS_SEGNATURA,
                    CHA_POS_SEGNATURA = passo.CHA_POS_SEGNATURA
                };

                await _pi3DbContext.IstanzaPassoFirmaEntities.AddAsync(newRecord);
            }
            await ((DbContext)_pi3DbContext).SaveChangesAsync();

        }
        private async Task UpdatePassoFirma(ProcessoFirma processoDiFirma, long? idIstanza = null)
        {
            var listPassiFirmaToUpdate = processoDiFirma.passi
                .Where(p => p.DaAggiornare && p.IncludiInIstanzaPasso != IncludiInIstanzaPasso.NO)
                .ToList();

            foreach (var passo in listPassiFirmaToUpdate)
            {
                var idPasso = passo.idPasso.AsLong();
                var passoFirmaToUpdate = await _pi3DbContext.IstanzaPassoFirmaEntities
                    .FirstOrDefaultAsync(pf => (idIstanza == null || pf.ID_ISTANZA_PROCESSO == idIstanza) && pf.ID_PASSO == idPasso);

                if (passoFirmaToUpdate != null)
                {
                    passoFirmaToUpdate.ID_RUOLO_COINVOLTO = passo.ruoloCoinvolto?.idGruppo.AsLong();
                    passoFirmaToUpdate.ID_UTENTE_COINVOLTO = string.IsNullOrEmpty(passo.utenteCoinvolto?.idPeople)
                        ? (long?)null
                        : passo.utenteCoinvolto.idPeople.AsLong();
                    passoFirmaToUpdate.TIPO_FIRMA = passo.Evento?.CodiceAzione;

                    passoFirmaToUpdate.TIPO_EVENTO = await _pi3DbContext.AnagraficaEventiEntities
                        .Where(e => e.VAR_COD_AZIONE == passo.Evento.CodiceAzione)
                        .Select(e => e.ID_EVENTO)
                        .FirstOrDefaultAsync();

                    passoFirmaToUpdate.ID_AOO = string.IsNullOrEmpty(passo.IdAOO) ? (long?)null : passo.IdAOO.AsLong();
                    passoFirmaToUpdate.ID_RF = string.IsNullOrEmpty(passo.IdRF) ? (long?)null : passo.IdRF.AsLong();
                    passoFirmaToUpdate.ID_MAIL_REGISTRO = string.IsNullOrEmpty(passo.IdMailRegistro) ? (long?)null : passo.IdMailRegistro.AsLong();
                    passoFirmaToUpdate.ID_TIPOLOGIA = string.IsNullOrEmpty(passo.IdTipologia) ? (long?)null : passo.IdTipologia.AsLong();
                    passoFirmaToUpdate.ID_STATO_DIAGRAMMA = string.IsNullOrEmpty(passo.IdStatoDiagramma) ? (long?)null : passo.IdStatoDiagramma.AsLong();

                    var applicaSegnatura = string.IsNullOrEmpty(passo.ApplicaSegnaturaPermanente) ? "0" : passo.ApplicaSegnaturaPermanente;
                    passoFirmaToUpdate.CHA_POS_SEGNATURA = applicaSegnatura;

                    if (applicaSegnatura == "0")
                    {
                        passo.PosizioneSegnaturaPermanente = string.Empty;
                    }

                    passoFirmaToUpdate.VAR_POS_SEGNATURA = string.IsNullOrEmpty(passo.PosizioneSegnaturaPermanente)
                        ? string.Empty
                        : passo.PosizioneSegnaturaPermanente;

                    await ((DbContext)_pi3DbContext).SaveChangesAsync();
                    _logger.LogDebug($"Popolo il modello di passo passo con ID: {passo.idPasso}");
                }
                else
                {
                    throw new Exception($"Errore nell'associazione del ruolo al passo di modello per l'ID passo: {passo.idPasso}");
                }
            }
        }


        private async Task UpdateStateIstanzaPassoNum(long IdIstanzaProcesso, string numPasso, string stato)
        {
            //var idIstanza = IdIstanzaProcesso.AsLong();
            try
            {
                var numeroPasso = numPasso.AsLong();

                var result = await _pi3DbContext.IstanzaPassoFirmaEntities.FirstOrDefaultAsync(i => i.ID_ISTANZA_PROCESSO == IdIstanzaProcesso && i.NUMERO_SEQUENZA == numeroPasso);
                result.STATO_PASSO = stato;
                await ((DbContext)_pi3DbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        private async Task<IstanzaProcessoFirmaEntity> CreateIstanzaProcessoDiFirma(string idIstanza, ProcessoFirma processoDiFirma, DocsPaVO.documento.FileRequest file, string dataDB, InfoUtente infoUtente, string strDocAll, string note)
        {
            if (!int.TryParse(file.version, out int numVers))
            {
                numVers = 1;
            }

            var nuovaIstanzaProcesso = new IstanzaProcessoFirmaEntity()
            {
                ID_ISTANZA = idIstanza.AsLong(),
                ID_PROCESSO = processoDiFirma.idProcesso.AsLong(),
                STATO = DocsPaVO.LibroFirma.TipoStatoProcesso.IN_EXEC.ToString(),
                ATTIVATO_IL = DateTime.Parse(dataDB),
                ID_RUOLO_PROPONENTE = infoUtente.idCorrGlobali.AsLong(),
                ID_UTENTE_PROPONENTE = infoUtente.idPeople.AsLong(),
                ID_DOCUMENTO = file.docNumber.AsLong(),
                VERSION_ID = file.versionId.AsLong(),
                DOC_ALL = strDocAll,
                NUM_ALL = file.versionLabel.AsLong(),
                NUM_VERSIONE = numVers,
                DESCRIZIONE = processoDiFirma.nome.Replace("'", "''"),
                NOTE = note.Replace("'", "''")
            };

            await _pi3DbContext.IstanzaProcessoFirmaEntities.AddAsync(nuovaIstanzaProcesso);

            await ((DbContext)_pi3DbContext).SaveChangesAsync();

            return nuovaIstanzaProcesso;
        }
        private object GetKeyFromVal(Hashtable ht, string val)
        {
            object res = null;
            IEnumerator keys = ht.Keys.GetEnumerator();

            while (keys.MoveNext())
            {
                if (((string)ht[keys.Current]).Equals(val))
                {
                    res = keys.Current;
                }
            }

            return res;
        }

        protected class InfoProcessesStartedForDocumentEntity
        {
            public long? ID_ISTANZA { get; set; }
            public long? ID_DOCUMENTO { get; set; }
            public string? DOC_ALL { get; set; }
            public long? NUM_ALL { get; set; }
            public string? DESCRIZIONE { get; set; }
            public string? VAR_PROF_OGGETTO { get; set; }

        }
    }
    #endregion
}
