// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using BusinessLogic.Interoperabilita.Semplificata;
using DocsPaVO.documento;
using DocsPaVO.Interoperabilita.Semplificata;
using DocsPaVO.Spedizione;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Mobile.WebApi.Helpers.RubricaComune;
using Pi3.App.Legacy.Mobile.WebApi.Helpers.Spedizione.Exceptions;
using Pi3.App.Legacy.Mobile.WebApi.Helpers.Spedizione.InteroperabilitaSemplificata;
using Pi3.App.Legacy.Mobile.WebApi.Requests;
using Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService;
using Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService.Domain;
using Pi3.App.Legacy.Mobile.WebApi.Services.RabbitMQ;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.Factory;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Tibco.Services.File.SigilloElettronico;
using System.Collections;
using static Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService.InteroperabilityMessage;
using InteroperabilityMessage = Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService.InteroperabilityMessage;
using SpedisciDocumentoRequest = Pi3.App.Legacy.Mobile.WebApi.Helpers.Spedizione.SpedisciDocumento;

namespace Pi3.App.Legacy.Mobile.WebApi.Helpers.Spedizione
{
    public class SpedisciDocumentoHandler : IRequestHandler<SpedisciDocumentoRequest, SpedisciDocumentoResult>
    {
        #region Public members
        public SpedisciDocumentoHandler(ILogger<SpedisciDocumentoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IConfigurationService configurationService,
            ISigilloElettronicoService sigilloElettronicoService,
            IDocumentBlobRepository documentBlobRepository,
            ITrasmissioneRepository trasmissioneRepository,
            IEmailSenderService emailSenderService,
            IInteroperabilityService interoperabilityService,
            IHttpContextAccessor httpContextAccessor,
            IFactoryService factoryService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._configurationService = configurationService;
            this._sigilloElettronicoService = sigilloElettronicoService;
            this._documentBlobRepository = documentBlobRepository;
            this._trasmissioneRepository = trasmissioneRepository;
            this._emailSenderService = emailSenderService;
            this._interoperabilityService = interoperabilityService;
            this._httpContextAccessor = httpContextAccessor;
            this._factoryService = factoryService;
        }

        public async Task<SpedisciDocumentoResult> Handle(SpedisciDocumentoRequest request, CancellationToken cancellationToken)
        {
            var infoSpedizioni = request.infoSpedizione;
            var documento = request.documento;
            var erroreSpedizione = string.Empty;
            var infoUtente = request.infoUtente;
            bool sendDestinatariEsterni = true;
            ArrayList inclusiInSpedizione = new ArrayList();

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var idGruppo = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            try
            {
                BusinessLogic.Spedizione.SpedizioneManager.CheckProtocolloUscita(documento);
                //AssertLibroFirma(schedaDocumento.docNumber);
                if ((BusinessLogic.LibroFirma.LibroFirmaManager.IsDocInLibroFirma(documento.systemId)))
                {
                    if (!BusinessLogic.LibroFirma.LibroFirmaManager.IsTitolarePassoInAttesa(documento.systemId, infoUtente, DocsPaVO.LibroFirma.Azione.DOCUMENTOSPEDISCI))
                    {
                        throw new DocumentoInLibroFirmaPassoNonAttesoPi3Exception(documento.docNumber);
                    }
                }

                infoSpedizioni.listaDestinatariNonRaggiungibili = new List<string>();

                //Aggiorno le versioni del documento principale da DB perchè in alcuni casi quello inviato dal FE non è aggiornato(caso di firma documento e poi spedisci INC000001057841)
                documento.documenti = new ArrayList();
                documento.documenti.AddRange(BusinessLogic.Documenti.DocManager.GetVersionsMainDocument(infoUtente, documento.docNumber));

                // Registro o RF mittente della spedizione per interoperabilità
                DocsPaVO.utente.Registro registroRfMittente = null;
                if (!string.IsNullOrEmpty(infoSpedizioni.IdRegistroRfMittente))
                    registroRfMittente = BusinessLogic.Utenti.RegistriManager.getRegistro(infoSpedizioni.IdRegistroRfMittente);
                if (registroRfMittente != null)
                    registroRfMittente.email = infoSpedizioni.mailAddress;

                //Creo il file di segnatura solo se presente nella spedizione almen in destinatario interoperante
                bool existsIterop = infoSpedizioni.DestinatariEsterni
                    .Where(d => d.IncludiInSpedizione && d.Interoperante && d.DatiDestinatari
                        .Where(d1 => d1.canalePref != null && d1.canalePref.descrizione.Equals("INTEROPERABILITA")).Count() > 0)
                    .Count() > 0;

                string creaAllegatoSegnaturaXml = await this._configurationService.GetValue<string>("BE_SEGNATURA_PROTO_ALLEGATO6");
                if (!string.IsNullOrEmpty(creaAllegatoSegnaturaXml) && creaAllegatoSegnaturaXml.Equals("1") && existsIterop)
                {
                    //Creo la nuova versione del file segnatura.xml
                    DocsPaVO.documento.Allegato allegatoSegnatura = (from DocsPaVO.documento.Allegato a in documento.allegati where a.TypeAttachment == 6 select a).FirstOrDefault();
                    if (allegatoSegnatura == null)
                    {
                        //Estraggo gli allegati
                        documento.allegati = BusinessLogic.Documenti.AllegatiManager.getAllegati(documento.systemId, string.Empty);
                        allegatoSegnatura = (from DocsPaVO.documento.Allegato a in documento.allegati where a.TypeAttachment == 6 select a).FirstOrDefault();
                    }
                    if (allegatoSegnatura != null)
                    {
                        DocsPaVO.documento.ResultSigilloElettronico resultSigillo = DocsPaVO.documento.ResultSigilloElettronico.OK;
                        allegatoSegnatura = BusinessLogic.Spedizione.SpedizioneManager.CreaSegnaturaProtocollo(allegatoSegnatura, documento, registroRfMittente, infoUtente, out resultSigillo, this._sigilloElettronicoService) as DocsPaVO.documento.Allegato;

                        if (resultSigillo != DocsPaVO.documento.ResultSigilloElettronico.OK)
                        {
                            //Se non ho allegato la segnatura di protocollo con sigillo non spedisco all'esterno
                            sendDestinatariEsterni = false;
                            string errorMessage = Spedizione.Exceptions.ErrorDescriptions.SigilloFileSegnaturaXMLNonApplicato;// "Errore durante l'applicazione del sigillo sul file segnatura.xml: non è stato possibile procedere con la spedizione";
                            //switch (resultSigillo)
                            //{
                            //    case DocsPaVO.documento.ResultSigilloElettronico.SERVICE_UNAVAILABLE:
                            //        errorMessage = "Per problemi tecnici sul servizio sigillo non è stato possibile spedire il documento. Verrà data comunicazione del ripristino del servizio per poter effettauare la spedizione";
                            //        break;
                            //    case DocsPaVO.documento.ResultSigilloElettronico.DATI_DI_FIRMA_ERRATI:
                            //        errorMessage = "Errore durante l'applicazione del sigillo sul file segnatura.xml, configurazione errata contattare l'amministratore: non è stato possibile procedere con la spedizione";
                            //        break;
                            //}

                            infoSpedizioni.Spedito = false;
                            DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                            interop.UpdateStoricoSpedizione(documento.systemId, "", infoUtente.idGruppo,
                                (!string.IsNullOrEmpty(infoSpedizioni.IdRegistroRfMittente) ? infoSpedizioni.IdRegistroRfMittente : ""), "Errore nella spedizione del documento", true);
                            foreach (DocsPaVO.Spedizione.DestinatarioEsterno destinatario in infoSpedizioni.DestinatariEsterni.Where(e => e.IncludiInSpedizione))
                            {
                                inclusiInSpedizione.Add(destinatario.Id);
                                destinatario.StatoSpedizione = BusinessLogic.Spedizione.SpedizioneManager.GetStatoErroreInSpedizione(errorMessage);
                                interop.UpdateStoricoSpedizione(
                                    documento.systemId,
                                    destinatario.Id,
                                    infoUtente.idGruppo,
                                    (!string.IsNullOrEmpty(infoSpedizioni.IdRegistroRfMittente) ? infoSpedizioni.IdRegistroRfMittente : ""),
                                    destinatario.StatoSpedizione.Descrizione,
                                    false);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < documento.allegati.Count; i++)
                            {
                                if ((documento.allegati[i] as DocsPaVO.documento.Allegato) != null && (documento.allegati[i] as DocsPaVO.documento.Allegato).docNumber.Equals(allegatoSegnatura.docNumber))
                                {
                                    (documento.allegati[i] as DocsPaVO.documento.Allegato).dataInserimento = allegatoSegnatura.dataInserimento;
                                    (documento.allegati[i] as DocsPaVO.documento.Allegato).fileName = allegatoSegnatura.fileName;
                                    (documento.allegati[i] as DocsPaVO.documento.Allegato).fileSize = allegatoSegnatura.fileSize;
                                    (documento.allegati[i] as DocsPaVO.documento.Allegato).idPeople = allegatoSegnatura.idPeople;
                                    (documento.allegati[i] as DocsPaVO.documento.Allegato).path = allegatoSegnatura.path;
                                    (documento.allegati[i] as DocsPaVO.documento.Allegato).subVersion = allegatoSegnatura.version;
                                    (documento.allegati[i] as DocsPaVO.documento.Allegato).version = allegatoSegnatura.version;
                                    (documento.allegati[i] as DocsPaVO.documento.Allegato).versionId = allegatoSegnatura.versionId;
                                    (documento.allegati[i] as DocsPaVO.documento.Allegato).versionLabel = allegatoSegnatura.versionLabel;
                                }
                            }
                        }
                    }
                }

                if (sendDestinatariEsterni)
                {
                    // PEC 4 - requisito 5 - storico spedizioni
                    // Dato che alla fine i destinatari perdono l'includiInSpedizione, mi salvo i loro ID in un arraylist.
                    // Modifica per un errore della notifica di mancata consegna IS
                    foreach (DocsPaVO.Spedizione.DestinatarioEsterno corr in infoSpedizioni.DestinatariEsterni)
                    {
                        if (corr.IncludiInSpedizione)
                        {
                            DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                            // Modifica per Occasionali.
                            //string canalePref = (corr.DatiDestinatari[0].canalePref != null ? corr.DatiDestinatari[0].canalePref.descrizione : "MAIL");
                            DocsPaVO.utente.Canale canalePref = corr.DatiDestinatari[0].canalePref;
                            interop.InsertInStoricoSpedizioni(
                                documento.systemId,
                                corr.Id,
                                "In fase di spedizione",
                                corr.Email,
                                infoUtente.idGruppo,
                                canalePref,
                                (!string.IsNullOrEmpty(infoSpedizioni.mailAddress) ? infoSpedizioni.mailAddress : ""),
                                (!string.IsNullOrEmpty(infoSpedizioni.IdRegistroRfMittente) ? infoSpedizioni.IdRegistroRfMittente : "")
                                );
                        }
                    }

                    // Reperimento del protocollo in uscita
                    DocsPaVO.documento.ProtocolloUscita protocolloUscita = (DocsPaVO.documento.ProtocolloUscita)documento.protocollo;

                    // Prelevamento di tutti i corrispondenti a cui bisogna spedire per Interoperabilita semplificata (se attiva)
                    IEnumerable<DocsPaVO.Spedizione.DestinatarioEsterno> destIS = new List<DocsPaVO.Spedizione.DestinatarioEsterno>();
                    if (InteroperabilitaSemplificataManager.IsEnabledSimplifiedInteroperability(infoUtente.idAmministrazione))
                    {
                        destIS =
                            infoSpedizioni.DestinatariEsterni.Where(
                                d => d.IncludiInSpedizione &&
                                    d.Interoperante &&
                                    d.DatiDestinatari.Where(d1 => d1.canalePref != null &&
                                        d1.canalePref.typeId.Equals(InteroperabilitaSemplificataManager.InteroperabilityCode) &&
                                        d1.Url != null && d1.Url.Count > 0 &&
                                        !String.IsNullOrEmpty(d1.Url[0].Url) &&
                                        Uri.IsWellFormedUriString(d1.Url[0].Url, UriKind.Absolute)).Count() > 0);
                        if (destIS.Count() > 0)
                        {
                            foreach (DocsPaVO.Spedizione.DestinatarioEsterno dtemp in destIS)
                            {
                                inclusiInSpedizione.Add(dtemp.Id);
                            }

                            await SpedisciPerInteroperabilitaSemplificata(documento, destIS.ToList(), request.infoUtente);
                        }
                    }

                    //spedisco ai destinatari interoperanti esterni(PEC - IS)
                    foreach (DocsPaVO.Spedizione.DestinatarioEsterno destinatario in infoSpedizioni.DestinatariEsterni.Where(e => e.IncludiInSpedizione))
                    {
                        // Se il destinatario non è già stato spedito per IS si procede con la spedizione classica
                        if (!destIS.Contains(destinatario))
                        {
                            inclusiInSpedizione.Add(destinatario.Id);
                            if (destinatario.Interoperante)
                            {
                                if (registroRfMittente != null)
                                {
                                    // Il destinatario è esterno all'AOO su cui è stato protocollato il documento,
                                    // pertanto il documento viene inviato al destinatario per Interoperabilita
                                    // modifica per l'email dell corrispondente occasionale
                                    if ((!string.IsNullOrEmpty(destinatario.DatiDestinatari[0].tipoCorrispondente)
                                        && destinatario.DatiDestinatari[0].tipoCorrispondente == "O") || destinatario.DatiDestinatari[0].tipoIE.Equals("E"))
                                        BusinessLogic.Spedizione.SpedizioneManager.SpedisciPerInteroperabilita(infoUtente, documento, registroRfMittente, destinatario);

                                }
                                else
                                {
                                    destinatario.StatoSpedizione = BusinessLogic.Spedizione.SpedizioneManager.GetStatoErroreInSpedizione(Exceptions.ErrorDescriptions.RegistroRFMittenteNotFound);
                                }
                            }
                            else
                                destinatario.StatoSpedizione = BusinessLogic.Spedizione.SpedizioneManager.GetStatoErroreInSpedizione(Exceptions.ErrorDescriptions.DestinatarioNonInteroperante);
                        }
                    }

                    //spedisco ai destinatari interoperanti interni
                    if (DocsPaVO.Settings.AppSettings.Instance.INTEROP_INT_NO_MAIL != null &&
                                DocsPaVO.Settings.AppSettings.Instance.INTEROP_INT_NO_MAIL.ToString() != "0")
                    {
                        System.Collections.Generic.List<string> aoo = new List<string>();
                        foreach (DocsPaVO.Spedizione.DestinatarioEsterno destinatario in infoSpedizioni.DestinatariEsterni.
                            Where(e => e.DatiDestinatari[0].tipoIE != null && e.DatiDestinatari[0].tipoIE.Equals("I") && e.IncludiInSpedizione))
                        {
                            inclusiInSpedizione.Add(destinatario.Id);
                            if (!aoo.Contains(destinatario.DatiDestinatari[0].codiceAOO))
                            {
                                if (registroRfMittente != null)
                                {
                                    BusinessLogic.Spedizione.SpedizioneManager.SpedisciPerInteroperabilita(infoUtente, documento, registroRfMittente, destinatario);
                                    if (destinatario.StatoSpedizione.Stato == DocsPaVO.Spedizione.StatiSpedizioneDocumentoEnum.Spedito)
                                        aoo.Add(destinatario.DatiDestinatari[0].codiceAOO);
                                }
                                else
                                {
                                    destinatario.StatoSpedizione = BusinessLogic.Spedizione.SpedizioneManager.GetStatoErroreInSpedizione(Exceptions.ErrorDescriptions.RegistroRFMittenteNotFound);
                                }
                            }
                            else
                            {
                                foreach (DocsPaVO.Spedizione.DestinatarioEsterno dest in infoSpedizioni.DestinatariEsterni.
                                    Where(e => e.DatiDestinatari[0].tipoIE.Equals("I")))
                                {
                                    if (dest.DatiDestinatari[0].codiceAOO.Equals(destinatario.DatiDestinatari[0].codiceAOO))
                                    {
                                        destinatario.StatoSpedizione = dest.StatoSpedizione;
                                        destinatario.DataUltimaSpedizione = dest.DataUltimaSpedizione;
                                        destinatario.IncludiInSpedizione = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                //spedisco ai destinatari interni(Intra AOO)
                foreach (DocsPaVO.Spedizione.DestinatarioInterno destinatario in infoSpedizioni.DestinatariInterni.Where(e => e.IncludiInSpedizione && !e.DisabledTrasm))
                {
                    // Il destinatario è interno, cioè ha visibilità sull'AOO sui cui è stato procollato il documento,
                    // pertanto viene effettuata una nuova trasmissione

                    //Andrea - Aggiunto parametro infospedizione per gestire Alert - lista destinatari non raggiungibili.
                    BusinessLogic.Spedizione.SpedizioneManager.SpedisciPerTrasmissione(infoUtente, documento, destinatario, infoSpedizioni);
                    //End Andrea
                }

                BusinessLogic.Spedizione.SpedizioneManager.SetStatoSpedito(infoSpedizioni);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                infoSpedizioni.Spedito = false;
                erroreSpedizione = Helpers.Spedizione.Exceptions.ErrorDescriptions.LogSpedizioneDocError;
            }

            try
            {
                if (infoSpedizioni.DestinatariInterni != null)
                    infoSpedizioni.DestinatariInterni = new List<DocsPaVO.Spedizione.DestinatarioInterno>(infoSpedizioni.DestinatariInterni.OrderBy(destInt => destInt.StatoSpedizione.Descrizione));

                if (infoSpedizioni.DestinatariEsterni != null)
                    infoSpedizioni.DestinatariEsterni = new List<DocsPaVO.Spedizione.DestinatarioEsterno>(infoSpedizioni.DestinatariEsterni.OrderBy(destEst => destEst.StatoSpedizione.Descrizione));
            }
            catch (Exception ex1)
            {
                // in caso di eccezione i destinatari vengono restituiti non ordinati
                this._logger.LogError("Errore nell'ordinamento dei destinatari");
            }

            foreach (DestinatarioEsterno corr in infoSpedizioni.DestinatariEsterni.Where(d => inclusiInSpedizione.Contains(d.Id)))
            {
                DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                interop.UpdateStoricoSpedizione(
                     documento.systemId,
                     corr.Id,
                     infoUtente.idGruppo,
                     (!string.IsNullOrEmpty(infoSpedizioni.IdRegistroRfMittente) ? infoSpedizioni.IdRegistroRfMittente : ""),
                     corr.StatoSpedizione.Descrizione,
                     false);
            }

            if (infoSpedizioni.Spedito)
            {
                await this._webMethodLoggerService.LogOK("DOCUMENTOSPEDISCI", documento.docNumber,
                            string.Format(Resources.LogSpedizioneDoc, documento.docNumber), null, "PITRE");

                //Inserisco nella coda del motore di Libro firma
                if (BusinessLogic.LibroFirma.LibroFirmaManager.IsDocInLibroFirma(documento.systemId))
                {
                    await this._mediator.Send(
                    new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                    {
                        IdProfile = documento.docNumber,
                        Evento = "DOCUMENTOSPEDISCI",
                    }));
                }
            }
            else
            {
                DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                interop.UpdateStoricoSpedizione(documento.systemId,
                    string.Empty, idGruppo.ToString(), (!string.IsNullOrEmpty(infoSpedizioni.IdRegistroRfMittente) ? infoSpedizioni.IdRegistroRfMittente : ""), erroreSpedizione, true);

                await this._webMethodLoggerService.LogKO("DOCUMENTOSPEDISCI", documento.docNumber,
                            string.Format(Resources.LogSpedizioneDoc, documento.docNumber), null, "PITRE");
            }

            return new SpedisciDocumentoResult(infoSpedizioni);
        }

        #endregion

        #region Private members
        protected readonly ILogger<SpedisciDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IConfigurationService _configurationService;
        protected readonly ISigilloElettronicoService _sigilloElettronicoService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected IEmailSenderService _emailSenderService;
        protected readonly IInteroperabilityService _interoperabilityService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IFactoryService _factoryService;


        private async Task SpedisciPerInteroperabilitaSemplificata(SchedaDocumento schedaDocumento, List<DestinatarioEsterno> receivers, InfoUtente infoUtente)
        {
            var sendSucceded = true;
            var errorMessage = string.Empty;
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var keyToken = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>("KeyToken");

            //Suddivisione dei destinatari per amministrazione
            List<List<DestinatarioEsterno>> splittedReceivers = receivers.GroupBy(r => r.DatiDestinatari[0].codiceAmm).Select(group => group.ToList()).ToList();

            foreach (List<DestinatarioEsterno> recs in splittedReceivers)
            {
                sendSucceded = true;
                errorMessage = string.Empty;

                // Costruzione della lista dei destinatari
                List<Corrispondente> corrs = new List<Corrispondente>();
                foreach (DestinatarioEsterno c in recs)
                    corrs.AddRange(c.DatiDestinatari);

                var receiverUlr = recs[0].DatiDestinatari[0].Url[0].Url;

                bool useNewServiceInteropPiTre = false;
                var switchInteropPitreEntity = await _dbContext.SwitchServiceInteropEntities.AsNoTracking()
                    .Where(s => s.VAR_INTEROP_URL.ToUpper().Equals(receiverUlr.ToUpper()))
                    .FirstOrDefaultAsync();
                if (switchInteropPitreEntity != null)
                {
                    useNewServiceInteropPiTre = switchInteropPitreEntity.CHA_USE_NEW_INTEROP == "1";
                }


                var instance = switchInteropPitreEntity?.VAR_INSTANCE;

                //Costruzione dell'oggetto con le informazioni sulla spedizione
                var interoperabilityMessage = await CreateInteroperabilityMessage(schedaDocumento, infoUtente, corrs.ToArray());

                try
                {
                    if (!string.IsNullOrEmpty(instance))
                    {
                        //Invio del messaggio al sistema di interoperabilità
                        await this._mediator.Send(
                            new MessageQueueCommandWrapper(new SpedizioneInteropPiTreRequest(this._claimsPrincipalService.Current)
                            {
                                Instance = instance,
                                Authorization = keyToken,
                                Tenant = interoperabilityMessage.Receivers[0].AdministrationCode,
                                InteroperabilityMessage = interoperabilityMessage
                            }));

                        foreach (var corr in corrs)
                            await InsertStatoInvio(schedaDocumento.systemId.AsLong(), corr, receiverUlr); 
                    }
                }
                catch (SenderNotInteroperablePi3Exception pi3Exception)
                {
                    this._logger.LogError(exception: pi3Exception, message: "Errore in SpedizionePerInteroperabilitaSemplificata: " + pi3Exception.Message);
                    sendSucceded = false;
                    errorMessage = pi3Exception.Message;
                }
                catch (Exception ex)
                {
                    this._logger.LogCritical(exception: ex, message: "Errore in SpedizionePerInteroperabilitaSemplificata: " + ex.Message);
                    sendSucceded = false;
                    errorMessage = Exceptions.ErrorDescriptions.LogErroreSpedizionePerInteroperabilitaSemplificata;
                }

                foreach (var destinatario in recs)
                {
                    destinatario.StatoSpedizione.Stato = sendSucceded ? StatiSpedizioneDocumentoEnum.Spedito : StatiSpedizioneDocumentoEnum.ErroreInSpedizione;
                    if (destinatario.StatoSpedizione.Stato != StatiSpedizioneDocumentoEnum.Spedito)
                    {
                        destinatario.DataUltimaSpedizione = string.Empty;
                        destinatario.IncludiInSpedizione = true;
                        destinatario.StatoSpedizione.Descrizione = errorMessage;
                    }
                    else
                    {
                        destinatario.DataUltimaSpedizione = BusinessLogic.Spedizione.SpedizioneManager.GetDataUltimaSpedizione(infoUtente, schedaDocumento, destinatario.DatiDestinatari[0]);
                        destinatario.StatoSpedizione.Stato = StatiSpedizioneDocumentoEnum.Spedito;
                        destinatario.StatoSpedizione.Descrizione = Resources.LogStatoSpedizioneSpedito;
                        destinatario.IncludiInSpedizione = false;
                    }
                }
            }
        }

        protected async Task InsertStatoInvio(long idProfile, Corrispondente corrispondente, string mail)
        {
            var statoInvioEntityNew = new StatoInvioEntity();

            var idCorrispondente = corrispondente.systemId.AsLong();
            var idDocArrivoPar = await _dbContext.DocArrivoParEntities
                .Where(d => d.ID_PROFILE == idProfile && d.ID_MITT_DEST == idCorrispondente &&
                    (new string[] { "D", "C", "F" }.Contains(d.CHA_TIPO_MITT_DEST)))
                .Select(d => d.SYSTEM_ID)
                .FirstAsync();

            statoInvioEntityNew.ID_PROFILE = idProfile;
            statoInvioEntityNew.ID_DOC_ARRIVO_PAR = idDocArrivoPar;
            statoInvioEntityNew.ID_CORR_GLOBALE = idCorrispondente;
            statoInvioEntityNew.VAR_INDIRIZZO = mail;

            if (corrispondente.tipoCorrispondente == "O")
            {
                statoInvioEntityNew.ID_DOCUMENTTYPE = await _dbContext.DocumentTypesEntities
                    .Where(d => d.TYPE_ID == "MAIL")
                    .Select(d => d.SYSTEM_ID)
                    .FirstAsync();
                statoInvioEntityNew.CHA_INTEROP = "1";
            }

            if (corrispondente.tipoIE == "E")
            {
                statoInvioEntityNew.ID_DOCUMENTTYPE = corrispondente.canalePref != null ? corrispondente.canalePref.systemId.AsLong() :
                    await _dbContext.CanaleCorrEntities
                        .Where(c => c.CHA_PREFERITO == "1" && c.ID_CORR_GLOBALE == idCorrispondente)
                        .Select(c => c.ID_DOCUMENTTYPE)
                        .FirstAsync();

                if (corrispondente.tipoCorrispondente != "O")
                {
                    var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities
                        .Where(c => c.SYSTEM_ID == idCorrispondente)
                        .Select(c => new
                        {
                            c.VAR_EMAIL,
                            c.VAR_SMTP,
                            c.NUM_PORTA_SMTP,
                            c.CHA_PA,
                            c.VAR_CODICE_AMM,
                            c.VAR_CODICE_AOO
                        })
                        .FirstAsync();

                    statoInvioEntityNew.VAR_SERVER_SMTP = corrGlobaliEntity.VAR_SMTP;
                    statoInvioEntityNew.NUM_PORTA_SMTP = corrGlobaliEntity.NUM_PORTA_SMTP;
                    statoInvioEntityNew.CHA_INTEROP = corrGlobaliEntity.CHA_PA;
                    statoInvioEntityNew.VAR_CODICE_AMM = corrGlobaliEntity.VAR_CODICE_AMM;
                    statoInvioEntityNew.VAR_CODICE_AOO = corrGlobaliEntity.VAR_CODICE_AOO;
                }
            }

            var dettaglioCorrispondenteEntity = await _dbContext.DettGlobaliEntities
                .Where(c => c.ID_CORR_GLOBALI == idCorrispondente)
                .Select(c => new
                {
                    c.VAR_FAX,
                    c.VAR_INDIRIZZO,
                    c.VAR_CAP,
                    c.VAR_PROVINCIA,
                    c.VAR_CITTA
                })
                .FirstOrDefaultAsync();
            if (dettaglioCorrispondenteEntity != null)
            {
                statoInvioEntityNew.VAR_CAP = dettaglioCorrispondenteEntity.VAR_CAP;
                statoInvioEntityNew.VAR_PROVINCIA = dettaglioCorrispondenteEntity.VAR_PROVINCIA;
                statoInvioEntityNew.VAR_CITTA = dettaglioCorrispondenteEntity.VAR_CITTA;
            }

            statoInvioEntityNew.STATUS_C_MASK = await GetStatoInvioMaskByTypeId(statoInvioEntityNew.ID_DOCUMENTTYPE.Value);
            statoInvioEntityNew.DTA_SPEDIZIONE = await _dbContext.GetSystemDateTime();

            var statoInvioEntity = await _dbContext.StatoInvioEntities
                .Where(s => s.ID_PROFILE == idProfile && s.ID_CORR_GLOBALE == idCorrispondente)
                .FirstOrDefaultAsync();
            if (statoInvioEntity != null)
            {
                statoInvioEntity.ID_CANALE = statoInvioEntityNew.ID_CANALE;
                statoInvioEntity.VAR_INDIRIZZO = statoInvioEntityNew.VAR_INDIRIZZO;
                statoInvioEntity.VAR_CAP = statoInvioEntityNew.VAR_CAP;
                statoInvioEntity.VAR_CITTA = statoInvioEntityNew.VAR_CITTA;
                statoInvioEntity.CHA_INTEROP = statoInvioEntityNew.CHA_INTEROP;
                statoInvioEntity.VAR_PROVINCIA = statoInvioEntityNew.VAR_PROVINCIA;
                statoInvioEntity.ID_DOCUMENTTYPE = statoInvioEntityNew.ID_DOCUMENTTYPE;
                statoInvioEntity.VAR_SERVER_SMTP = statoInvioEntityNew.VAR_SERVER_SMTP;
                statoInvioEntity.NUM_PORTA_SMTP = statoInvioEntityNew.NUM_PORTA_SMTP;
                statoInvioEntity.VAR_CODICE_AOO = statoInvioEntityNew.VAR_CODICE_AOO;
                statoInvioEntity.VAR_CODICE_AMM = statoInvioEntityNew.VAR_CODICE_AMM;
                statoInvioEntity.DTA_SPEDIZIONE = statoInvioEntityNew.DTA_SPEDIZIONE;
                statoInvioEntity.STATUS_C_MASK = statoInvioEntityNew.STATUS_C_MASK;
                statoInvioEntity.VAR_MOTIVO_ANNULLA = null;
                statoInvioEntity.CHA_ANNULLATO = null;
                statoInvioEntity.VAR_PROTO_DEST = null;
                statoInvioEntity.DTA_PROTO_DEST = null;
            }
            else
            {
                await _dbContext.StatoInvioEntities.AddAsync(statoInvioEntityNew);
            }

            await ((DbContext)_dbContext).SaveChangesAsync();
        }
        protected async Task<string> GetStatoInvioMaskByTypeId(long idType)
        {
            var mask = string.Empty;

            var typeId = await _dbContext.DocumentTypesEntities
                .Where(d => d.SYSTEM_ID == idType)
                .Select(d => d.TYPE_ID)
                .FirstAsync();

            switch (typeId)
            {
                case "MAIL":
                    mask = "AANNNNA";
                    break;
                case "INTEROPERABILITA":
                    mask = "AAAAAAA";
                    break;
                case "SIMPLIFIEDINTEROPERABILITY":
                    mask = "ANAAAAN";
                    break;
            }

            return mask;
        }
        protected async Task<InteroperabilityMessage> CreateInteroperabilityMessage(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Corrispondente[] destinatari)
        {
            InteroperabilityMessage interoperabilityMessage = new InteroperabilityMessage();

            interoperabilityMessage.Record = await CreateRecordInfo(schedaDocumento);
            interoperabilityMessage.MainDocument = await CreateMainDocumentInfo(schedaDocumento);
            interoperabilityMessage.Sender = await CreateSendersInfo(schedaDocumento, infoUtente, destinatari);
            interoperabilityMessage.Receivers = await CreateReceiversInfo(destinatari);
            interoperabilityMessage.Attachments = await CreateAttachmentsInfo(schedaDocumento, infoUtente);

            interoperabilityMessage.IsPrivate = schedaDocumento.privato == "1";
            interoperabilityMessage.ReceiverAdministrationCode = destinatari[0].codiceAmm;

            interoperabilityMessage.Note = string.Empty;

            return interoperabilityMessage;
        }

        /// <summary>
        /// Metodo per il recupero delle informazioni sul protocollo miittente
        /// </summary>
        /// <param name="schedaDocumento">Documento da cui estrarre le informazioni</param>
        /// <returns>Informazioni sul protocollo mittente</returns>
        protected async Task<RecordInfo> CreateRecordInfo(SchedaDocumento schedaDocumento)
        {
            RecordInfo recordInfo = new RecordInfo()
            {
                AdministrationCode = schedaDocumento.registro.codAmministrazione,
                AOOCode = schedaDocumento.registro.codRegistro,
                RecordDate = await _dbContext.ProfileEntities.Where(p => p.SYSTEM_ID == schedaDocumento.systemId.AsLong()).Select(p => p.DTA_PROTO.Value).FirstAsync(),
                RecordNumber = schedaDocumento.protocollo.numero,
                Subject = schedaDocumento.oggetto.descrizione
            };

            return recordInfo;
        }

        /// <summary>
        /// Metodo per la creazione delle informazioni sul mittente della spedizione
        /// </summary>
        /// <param name="schedaDocumento">Documento da cui estrarre le informazioni</param>
        /// <param name="infoUtente">Informazioni sull'utente richiedente</param>
        /// <param name="destinatari">Lista dei destinatari della spedizione</param>
        /// <returns>Informazioni sul mittente della spedizione</returns>
        protected async Task<SenderInfo> CreateSendersInfo(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Corrispondente[] destinatari)
        {
            var id_Tenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idUo = ((ProtocolloUscita)schedaDocumento.protocollo).mittente.systemId.AsLong();

            var url = await this._configurationService.GetValue<string>(infoUtente.idAmministrazione, "INTEROP_SERVICE_URL");
            if (string.IsNullOrEmpty(url))
                url = await this._configurationService.GetValue<string>("INTEROP_SERVICE_URL");

            var fileManagerUrl = await this._configurationService.GetValue<string>(infoUtente.idAmministrazione, "FILE_SERVICE_URL");
            if (string.IsNullOrEmpty(fileManagerUrl))
                fileManagerUrl = await this._configurationService.GetValue<string>("FILE_SERVICE_URL");

            var codiceAmm = await _dbContext.AmministraEntities.Where(a => a.SYSTEM_ID == id_Tenant).Select(a => a.VAR_CODICE_AMM).FirstAsync();
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities
                .Where(c => c.SYSTEM_ID == idUo)
                .Select(c => new
                {
                    c.VAR_COD_RUBRICA,
                    c.SYSTEM_ID,
                    c.INTEROPREGISTRYID,
                    c.INTEROPRFID
                })
                .FirstAsync();
            string codiceRegistro = null;
            if (corrGlobaliEntity.INTEROPRFID != null)
            {
                codiceRegistro = await _dbContext.RegistroEntities
                    .Where(r => r.SYSTEM_ID == corrGlobaliEntity.INTEROPRFID)
                    .Select(r => r.VAR_CODICE)
                    .FirstOrDefaultAsync();
            }

            // Recupero del codice del mittente
            var senderCode = $"{codiceAmm}-{codiceRegistro ?? corrGlobaliEntity.VAR_COD_RUBRICA}";

            // Se il mittente non è interoperante o se non è presente in RC, non si può procedere con la spedizione
            //Metodo utilizzato per verificare se un corrispondente (mittente di un protocollo)
            //è abilitato all'interoperabilità. Per essere abilitato deve avere valorizzati l'id del registro
            var isCorrEnabledToInterop = corrGlobaliEntity.INTEROPREGISTRYID != null;
            if (!isCorrEnabledToInterop || (await _mediator.Send(new GetElementoInRubricaComuneNoEsterna(senderCode, infoUtente))).output == null)
                throw new SenderNotInteroperablePi3Exception();

            SenderInfo senderInfo = new SenderInfo()
            {
                AdministrationId = infoUtente.idAmministrazione,
                Code = senderCode,
                Url = url,
                UserId = infoUtente.userId,
                FileManagerUrl = fileManagerUrl
            };

            return senderInfo;
        }

        /// <summary>
        /// Metodo per la creazione delle informazioni sui destinatari della spedizione
        /// </summary>
        /// <param name="receivers">Destinatari da cui estrarre le informazioni di interesse</param>
        /// <returns>Lista con le informazioni sui destinatari della spedizione</returns>
        private async Task<List<ReceiverInfo>> CreateReceiversInfo(Corrispondente[] receivers)
        {
            List<ReceiverInfo> reciversInfo = new List<ReceiverInfo>();
            foreach (var receiver in receivers)
            {
                // Se il corrispondente è storicizzato, ne viene recuperata l'ultima versione
                string corrCode = receiver.codiceRubrica;
                if (!string.IsNullOrEmpty(receiver.dta_fine))
                {
                    long? idOld = receiver.systemId.AsLong();
                    while (idOld != null)
                    {
                        var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities
                            .Where(c => c.ID_OLD == idOld)
                            .Select(c => new
                            {
                                c.SYSTEM_ID,
                                c.VAR_COD_RUBRICA,
                                c.DTA_FINE
                            })
                            .FirstOrDefaultAsync();

                        idOld = null;
                        if (corrGlobaliEntity != null)
                        {
                            corrCode = corrGlobaliEntity.VAR_COD_RUBRICA;
                            idOld = corrGlobaliEntity.SYSTEM_ID;
                        }
                    }
                }

                reciversInfo.Add(new ReceiverInfo()
                {
                    AdministrationCode = receiver.codiceAmm,
                    AOOCode = receiver.codiceAOO,
                    Code = corrCode
                });
            }
            return reciversInfo;
        }

        /// <summary>
        /// Metodo per la generazione delle informazioni sul documento principale
        /// </summary>
        /// <param name="schedaDocumento">Documento da cui spedire</param>
        /// <returns>Informazioni sul documento principale</returns>
        protected async Task<DocumentInfo> CreateMainDocumentInfo(SchedaDocumento schedaDocumento)
        {
            Documento mainDocument = schedaDocumento.documenti[0] as Documento;

            DocumentInfo documentInfo = new DocumentInfo()
            {
                DocumentNumber = mainDocument.docNumber,
                DocumentServerLocation = mainDocument.docServerLoc ?? string.Empty,
                FileName = !string.IsNullOrEmpty(mainDocument.fileName) ? Path.GetFileName(mainDocument.fileName) : string.Empty,
                FilePath = mainDocument.fileName ?? string.Empty,
                Name = !string.IsNullOrEmpty(mainDocument.fileName) ? Path.GetFileName(mainDocument.fileName) : string.Empty,
                VersionId = mainDocument.versionId,
                VersionLabel = mainDocument.versionLabel,
                Version = mainDocument.version,
                Fingerprint = mainDocument.impronta ?? string.Empty,
                Signature = await GetTipoFirma(mainDocument.tipoFirma)
            };

            return documentInfo;
        }

        /// <summary>
        /// Metodo per la creazione delle informazioni sugli allegati del documento
        /// </summary>
        /// <param name="schedaDocumento">Documento da cui estrarre le informazioni</param>
        /// <param name="infoUtente">Informazioni sul richiedente</param>
        /// <returns>Lista delle informazioni sugli allegati</returns>
        protected async Task<List<DocumentInfo>> CreateAttachmentsInfo(SchedaDocumento schedaDocumento, InfoUtente infoUtente)
        {
            List<DocumentInfo> attachmentsInfo = new List<DocumentInfo>();
            foreach (DocsPaVO.documento.Allegato allegato in schedaDocumento.allegati)
            {
                if (allegato.TypeAttachment != 2 && allegato.TypeAttachment != 3)
                {
                    attachmentsInfo.Add(new DocumentInfo()
                    {
                        DocumentNumber = allegato.docNumber,
                        DocumentServerLocation = allegato.docServerLoc ?? string.Empty,
                        FileName = !string.IsNullOrEmpty(allegato.fileName) ? Path.GetFileName(allegato.fileName) : string.Empty,
                        FilePath = allegato.fileName ?? string.Empty,
                        Name = !string.IsNullOrEmpty(allegato.fileName) ? Path.GetFileName(allegato.fileName) : string.Empty,
                        NumberOfPages = allegato.numeroPagine,
                        VersionId = allegato.versionId,
                        VersionLabel = allegato.versionLabel,
                        Version = allegato.version,
                        Fingerprint = allegato.impronta ?? string.Empty,
                        Signature = await GetTipoFirma(allegato.tipoFirma)
                    });
                }
            }

            return attachmentsInfo;
        }

        protected virtual async Task<TipoFirmaEnum> GetTipoFirma(string tipoFirma)
        {
            var tipoFirmaEnum = TipoFirmaEnum.Nessuna;

            if (!string.IsNullOrEmpty(tipoFirma))
            {
                tipoFirma = tipoFirma.Replace("E", "");
                switch (tipoFirma)
                {
                    case "P":
                        tipoFirmaEnum = TipoFirmaEnum.Pades;
                        break;
                    case "C":
                        tipoFirmaEnum = TipoFirmaEnum.Cades;
                        break;
                    case "T":
                        tipoFirmaEnum = TipoFirmaEnum.Tsd;
                        break;
                    case "X":
                        tipoFirmaEnum = TipoFirmaEnum.Xades;
                        break;
                    default:
                        tipoFirmaEnum = TipoFirmaEnum.Nessuna;
                        break;
                }
            }

            return tipoFirmaEnum;
        }
        #endregion
    }
}
