// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.addressbook;
using DocsPaVO.areaConservazione;
using DocsPaVO.documento;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.FirmaDigitale2;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaAggregate.Repository;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Pipelines;
using System.Text;
using System.Xml;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.StandardCase
{
    internal class StandardCaseHandler : IRequestHandler<StandardCaseRequest, StandardCaseResult>
    {
        #region Public members
        public StandardCaseHandler(ILogger<StandardCaseHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository documentoAmministrativo,
            IRubricaComuneService rubricaComuneService,
            IHttpContextAccessor httpContextAccessor,
            IDocumentoAmministrativoRepository documentRepository,
            IDocumentBlobRepository documentBlobRepository,
            IFirmaDigitale2Service firmaDigitale2Service,
            IUOCorrispondenteRepository uOCorrispondenteRepository,
            IWebMethodLoggerService webMethodLoggerService,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._rubricaComuneService = rubricaComuneService;
            this._httpContextAccessor = httpContextAccessor;
            this._documentRepository = documentRepository;
            this._documentBlobRepository = documentBlobRepository;
            this._uoCorrispondenteRepository = uOCorrispondenteRepository;
            this._configurationService = configurationService;
            this._firmaDigitale2Service = firmaDigitale2Service;
            this._documentoAmministrativoRepository = documentoAmministrativo;
            this._webMethodLoggerService = webMethodLoggerService;

            this.InitializeMapper();
        }
        public async Task<StandardCaseResult> Handle(StandardCaseRequest request, CancellationToken cancellationToken)
        {
            var output = new ProcessorOutput();

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            int processedAttachments = 0;

            var ruoloEntity = await this._dbContext.CorrGlobaliEntities.FirstAsync(x => x.ID_GRUPPO == idGroup);
            var idRole = ruoloEntity?.SYSTEM_ID;

            var docnumber = string.Empty;

            System.IO.FileStream fs = null;
            bool codint1 = false;
            System.IO.FileStream fsAll = null;
            bool daAggiornareUffRef = false;
            DocsPaVO.documento.SchedaDocumento sd = null;
            string err = string.Empty;
            docnumber = string.Empty;
            string messaggioRC = string.Empty;
            string docPrincipaleName = Resources.DocPrincipaleName;
            var mailId = request.messageId;

            var email = request.email;
            var reg = request.reg;
            var mailAddress = request.emailAddress;
            var eccSegnatura = request.eccSegnatura;
            var salvataggioMail = request.salvataggioMail;

            DocsPaVO.utente.Ruolo ruolo = this._mapper.Map<Ruolo>(ruoloEntity);
            try
            {
                var registroEntity = await this._dbContext.RegistroEntities.AsNoTracking().FirstAsync(x => x.SYSTEM_ID == request.reg.systemId.AsLong());
                bool mailPendente = await this.MantieniMailRicevutePendenti(reg.systemId.AsLong(), request.emailAddress);

                #region Controlli nel formato dei files
                //CONTROLLI NEL FORMATO DEI FILES
                //Documento principale 
                if (GetApp(docPrincipaleName) == null)
                {
                    err = Resources.DocPrincipaleFormatoNonGestito; // "La mail viene sospesa. Il documento principale ha un formato non gestito";
                    this._logger.LogDebug(err); // "La mail viene sospesa. Il documento principale ha un formato non gestito");
                    if (await this.MailElaborata(mailId, "U"))
                        _logger.LogDebug(Resources.SospensioneEseguita);
                    else
                        _logger.LogDebug(Resources.SospensioneNonEseguita);

                    return new StandardCaseResult(new ProcessorOutput()
                    {
                        Success = false,
                        DocNumber = null,
                        ErrorMessage = err,
                        ProcessedAttachments = 0
                    });
                }

                //Attachment
                List<Core.Services.Email.BoxScanner.EmailContentAttachment> attachments = email.Attachments;
                for (int ind = 0; ind < attachments.Count; ind++)
                {
                    var a = attachments[ind];
                    if (GetApp(a.FileName) == null)
                    {
                        err = string.Format(Resources.AttachFormatoNonGestito, a.FileName); //"La mail viene sospesa. Il documento " + mc.attachments[ind].name + " ha un formato non gestito";
                        this._logger.LogDebug(err);
                        if (await this.MailElaborata(mailId, "U"))
                            _logger.LogDebug(Resources.SospensioneEseguita);
                        else
                            _logger.LogDebug(Resources.SospensioneNonEseguita);

                        return new StandardCaseResult(new ProcessorOutput()
                        {
                            Success = false,
                            DocNumber = null,
                            ErrorMessage = err,
                            ProcessedAttachments = 0
                        });
                    }
                }
                #endregion

                #region Elaborazione mail
                //mittente
                DocsPaVO.utente.Corrispondente mittente = null;

                //corrispondente 
                _logger.LogDebug($"email.Sender.Address = {email.Sender.Address}");
                DocsPaVO.utente.Corrispondente mittente_appoggio = null;
                // Il mittente va ricercato in tutti i registri associati al ruolo
                DocsPaVO.utente.Registro[] registriRuolo = (await this._mediator.Send(new Requests.UtenteGetRegistriWithRf(idRole.ToString(), null, null, false))).output; 
                foreach (var registro in registriRuolo)
                {
                    (mittente, messaggioRC) = await this.GetCorrispondenteByEmail(request.email.Sender.Address, registro.systemId, idTenant);

                    if (mittente != null)
                        if (!mittente.codiceRubrica.Contains("@"))
                            break;
                        else
                            mittente_appoggio = mittente;
                }
                if (mittente == null)
                    mittente = mittente_appoggio;
                string checkInteroperante = "";
                string checkSameMail = "";
                if (mittente == null)
                {
                    mittente = new DocsPaVO.utente.UnitaOrganizzativa();
                    mittente.descrizione = email.Sender.Address;
                    mittente.tipoIE = "E";
                    mittente.tipoCorrispondente = "S";
                    mittente.codiceRubrica = email.Sender.Address;
                    mittente.codiceAOO = reg.codice;
                    mittente.email = email.Sender.Address;
                    var documentTypeEntity = await this._dbContext.DocumentTypesEntities.FirstAsync(x => x.TYPE_ID == Resources.MAIL);
                    mittente.canalePref = this._mapper.Map<DocsPaVO.utente.Canale>(documentTypeEntity);
                    mittente.cognome = email.Sender.Address;
                    mittente.idAmministrazione = reg.idAmministrazione;
                    mittente.idRegistro = reg.systemId;
                }

                this._logger.LogDebug("Inserimento documento principale");

                sd = new DocsPaVO.documento.SchedaDocumento();
                sd.appId = (await this.GetApp(docPrincipaleName)).application;
                DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();
                ogg.descrizione = email.Subject.Value; 
                ogg = (DocsPaVO.documento.Oggetto)XML_Serialization_Deserialization_By_Encode(ogg, typeof(DocsPaVO.documento.Oggetto), null, System.Text.Encoding.UTF8);
                sd.oggetto = ogg;
                sd.tipoProto = Resources.Grigio;
                sd.typeId = Resources.MAIL;
                sd.mezzoSpedizione = await this._dbContext.DocumentTypesEntities.Where(x => x.TYPE_ID == Resources.MAIL).Select(x => x.SYSTEM_ID.ToString()).FirstOrDefaultAsync();
                sd.descMezzoSpedizione = Resources.MAIL;
                sd.registro = await this.CaricaRegistroInScheda(reg);

                // Per gestione pendenti tramite PEC
                if (mailPendente)
                    sd.privato = "1";

                this._logger.LogDebug("Salvataggio doc...");

                sd = await AddDocGrigia(sd, idTenant.ToString(), ruolo);

                docnumber = sd.docNumber;

                if (await this.InsertAssDocAddress(sd.systemId.AsLong(), reg.systemId.AsLong(), mailAddress))
                    _logger.LogDebug(Resources.InsertAssDocAddressLogOK);
                else
                    _logger.LogError(Resources.InsertAssDocAddressLogKO);

                sd.predisponiProtocollazione = true;

                sd.tipoProto = Resources.Arrivo;
                sd.typeId = Resources.MAIL;

                if (eccSegnatura)
                    sd.interop = "E";
                else
                    sd.interop = "P";

                DocsPaVO.documento.ProtocolloEntrata protEntr = new DocsPaVO.documento.ProtocolloEntrata();
                protEntr.mittente = mittente;

                if (request.email.Headers.Any(x => x.Name == "utenteDocspa"))
                {
                    this._logger.LogDebug("Risoluzione mittente intermedio ...");

                    Core.Services.Email.BoxScanner.EmailHeader emailHeader = request.email.Headers.First(x => x.Name == "utenteDocspa");
                    var mittenteIntermedioMail = emailHeader.Value;

                    var mittenteIntermedio = await this._dbContext.PeopleEntities.AsNoTracking()
                        .Where(x => x.EMAIL_ADDRESS.ToUpper() == mittenteIntermedioMail.ToUpper() && x.ID_AMM == idTenant)
                        .FirstOrDefaultAsync();

                    if (mittenteIntermedio != null)
                    {
                        var utenteMittIntermedio = this._mapper.Map<DocsPaVO.utente.Utente>(mittenteIntermedio);
                        long idP = utenteMittIntermedio.idPeople.AsLong();
                        var corrMittIntermedio = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_PEOPLE == idP).Select(x => new { SYSTEM_ID = x.SYSTEM_ID, CHA_TIPO_IE = x.CHA_TIPO_IE }).FirstOrDefaultAsync();
                        utenteMittIntermedio.systemId = corrMittIntermedio.SYSTEM_ID.ToString();
                        utenteMittIntermedio.tipoCorrispondente = corrMittIntermedio.CHA_TIPO_IE;
                        protEntr.mittenteIntermedio = utenteMittIntermedio;
                    }
                    request.email.Headers.Remove(emailHeader);
                    this._logger.LogDebug("Risoluzione mittente intermedio eseguita ...");
                }

                sd.protocollo = protEntr;

                if (eccSegnatura && (await CheckMailBox.Segnatura.ValidaSegnatura.GetLivelloAnomaliaSegnatura(email.Sender.Address, "INVIO_CONFERMA", this._dbContext)).Equals("0"))
                    sd.protocollo.invioConferma = "1";
                else
                    sd.protocollo.invioConferma = "0";
                           

                if (attachments.Count == 1 && System.IO.Path.GetExtension(attachments[0].FileName).ToUpper() == ".PDF")
                {
                    this._logger.LogDebug("Recupero il fileDocumento");
                    DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                    Core.Services.Email.BoxScanner.EmailContentAttachment att = attachments[0];
                    fdAll.content = att.Content; 
                    fdAll.length = att.Content.Length; 
                    fdAll.name = att.FileName;

                }

                //salvo mittente dopo cambio descrizione se c'è modulo PDF.
                mittente = ((DocsPaVO.documento.ProtocolloEntrata)sd.protocollo).mittente;

                try
                {
                    string[] splitted = messaggioRC.Split('#');
                    if (splitted.Length > 1)
                        messaggioRC = splitted[0];

                    if (mittente != null
                        && string.IsNullOrEmpty(mittente.systemId)
                        && !messaggioRC.Equals("RC"))
                    {
                        //inserico il corrispondente in rubrica comune
                        mittente = (await this._mediator.Send(new Application.Requests.AddressbookInsertCorrispondente(mittente, null, new InfoUtente()))).output;
                        //inserisc ola mail associata al corrispondente esterno in DPA_MAIL_CORR_ESTERNI
                        List<DocsPaVO.utente.MailCorrispondente> casella = new List<DocsPaVO.utente.MailCorrispondente>();
                        casella.Add(new DocsPaVO.utente.MailCorrispondente()
                        {
                            Email = mittente.email,
                            Note = "",
                            Principale = "1"
                        });
                        //BusinessLogic.Utenti.addressBookManager.InsertMailCorrispondente(casella, mittente.systemId);
                        var resInsert = (await this._mediator.Send(new Application.Requests.InsertMailCorrispondenteEsterno(casella, mittente.systemId))).output;
                        if (!resInsert)
                            this._logger.LogError(Resources.ErrorModifyMail);
                    }
                }
                catch (Exception errmes)
                {
                    this._logger.LogError("errore durante l'inserimento di un corrispondente. il corrispondente esisite già. - errore:" + errmes.Message + " stacktrace: " + errmes.StackTrace);
                    err = "(CODINTEROP3)";
                }

                try
                {
                    sd.documento_da_pec = request.isPec ? "1" : "0";
                    ((DocsPaVO.documento.Documento)sd.documenti[0]).dataArrivo = email.DeliveryDate.ToString("dd/MM/yyyy HH:mm:ss");
                    sd = (await this._mediator.Send(new Requests.DocumentoSaveDocumento(ruolo, new InfoUtente(), sd, false))).output;
                }
                catch (Exception ex)
                {
                    err = string.Format(Resources.ErroreSalvataggioDocumento, ex.Message); 
                    this._logger.LogError(err);
                    throw new StandardCaseException(err);
                }
                this._logger.LogDebug("Salvataggio eseguito");

                sd.predisponiProtocollazione = !string.IsNullOrEmpty(sd.interop) && sd.interop.Equals("E");

                DocsPaVO.utente.InfoUtente info = new DocsPaVO.utente.InfoUtente();
                info.idAmministrazione = sd.registro.idAmministrazione;

                if (sd != null && sd.mezzoSpedizione != null && !string.IsNullOrEmpty(sd.mezzoSpedizione))
                {
                    bool mezzoSpedizioneCollegato = (await this._mediator.Send(new Requests.collegaMezzoSpedizioneDocumento(new InfoUtente(), sd.mezzoSpedizione, sd.systemId))).output;
                }
                this._logger.LogDebug("eseguito insert mezzo sped");

                /* Update per sd.interop e sd.documento_da_pec */
                long sysIdAsLong = sd.systemId.AsLong();
                var profileEntity = await this._dbContext.ProfileEntities.Where(x => x.SYSTEM_ID == sysIdAsLong).FirstOrDefaultAsync();
                if (profileEntity != null)
                {
                    profileEntity.CHA_INTEROP = sd.interop;
                    profileEntity.CHA_DOCUMENTO_DA_PEC = !string.IsNullOrEmpty(sd.documento_da_pec) ? sd.documento_da_pec : (request.isPec ? "1" : "0");
                    profileEntity.CHA_INVIO_CONFERMA = sd.protocollo.invioConferma == "1" ? "1" : "0";
                    ((DbContext)_dbContext).Update(profileEntity);
                    await ((DbContext)_dbContext).SaveChangesAsync();
                }

                DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();

                var body = !string.IsNullOrEmpty(email.Body.Value) ? email.Body.Value : Resources.BodyHtmlEmpty;
                var attachDocPrincipale = Encoding.UTF8.GetBytes(body);
                fd.content = attachDocPrincipale;
                fd.length = attachDocPrincipale.Length;
                fd.name = docPrincipaleName;

                DocsPaVO.documento.FileRequest frSch = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                frSch.fileName = docPrincipaleName;

                if (fd.content.Length > 0)
                {
                    var putFileResult = await this._mediator.Send(new Requests.DocumentoPutFileNoException(frSch, fd, new InfoUtente()));
                    var putFileOk = putFileResult.output;
                    err = putFileResult.errorMessage;
                    if (!putFileOk)
                        throw new StandardCaseException(Resources.ErroreFilePrincipale);
                    else
                    {
                        bool tsrOk = await this.MatchTSR(email, frSch, fd, idPeople);
                        await _mediator.Send(new Requests.ExtractXmlSuapRequest(sd, docPrincipaleName, fd.content));
                    }
                }

                this._logger.LogDebug("Inserimento degli allegati");

                for (int i = 0; i < attachments.Count; i++)
                {
                    var att = attachments[i];
                    string nomeAllegato = att.FileName;
                    string nomeAllegatoAlternativo = att.FileName;

                    this._logger.LogDebug("Inserimento allegato " + nomeAllegato);

                    //TO DO dopo - BOUNCYCASTLE
                    /*
                    if (await this.FindTSRMatch(email.Attachments, nomeAllegato))
                        continue; //il TSR fa parte di un o dei doc, salto l'acquisizione e l'aggiunta dell'allegato
                    */

                    DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                    all.docNumber = sd.docNumber;
                    all.fileName = GetFileName(nomeAllegato);
                    all.version = "0";
                    all.position = i + 1;
                    all.descrizione = String.Format(Resources.AllegatoDescrizione, i);
                    if (!String.IsNullOrEmpty(all.fileName))
                        all.descrizione = all.fileName;

                    //Aggiunge la descrizione allegato segnatura.xml all'allegato
                    bool isSegnaturaXml = eccSegnatura && all.fileName.ToLower().Equals(Resources.NomeFileSegnatura) || all.fileName.ToLower().Equals(Resources.NomeFileSegnaturaSenzaExt);
                    if (isSegnaturaXml)
                        all.descrizione = Resources.AllegatoSegnaturaDescrizione; // "allegato segnatura.xml";

                    DocsPaVO.documento.Allegato res = (await this._mediator.Send(new Requests.DocumentoAggiungiAllegato(new InfoUtente(), all))).output;
                    if (res == null)
                    {
                        err = string.Format(Resources.ErroreAggiuntaAllegato, i);
                        throw new StandardCaseException(err);
                    }
                    DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                    fdAll.content = email.Attachments.Where(x => x.FileName.ToLower().Equals(nomeAllegato.ToLower())).Select(x => x.Content).FirstOrDefault();
                    fdAll.length = fdAll.content.Length;
                    fdAll.name = nomeAllegato;
                    DocsPaVO.documento.FileRequest fRAll = new DocsPaVO.documento.FileRequest();
                    fRAll = (DocsPaVO.documento.FileRequest)all;

                    fRAll.fileName = nomeAllegato;

                    if (fdAll.content.Length > 0)
                    {
                        var putFileResult = await this._mediator.Send(new Requests.DocumentoPutFileNoException(fRAll, fdAll, new InfoUtente()));
                        var putFileOk = putFileResult.output;
                        err = putFileResult.errorMessage;
                        if (!putFileOk)
                            throw new StandardCaseException(Resources.ErroreFileAllegato);
                        else
                        {
                            bool tsrOk = false;
                            if (!isSegnaturaXml)
                                tsrOk = await this.MatchTSR(email, fRAll, fdAll, idPeople);

                            await _mediator.Send(new Requests.ExtractXmlSuapRequest(sd, nomeAllegato, fdAll.content));

                            processedAttachments++;
                        }
                    }

                }

                //Inserimento allegato mail - solo se abilitato da chiave BE_SALVA_EMAIL_IN_LOCALE oppure CHA_SALVA_MAIL_LOC dalla DPA_MAIL_REGISTRI 
                if (!string.IsNullOrEmpty(salvataggioMail) && salvataggioMail.Equals("1") && email.BinaryContent != null)
                {
                    string nomeMail = System.Guid.NewGuid().ToString().Substring(0, 25) + ".eml";
                    DocsPaVO.documento.Allegato allMail = new DocsPaVO.documento.Allegato();
                    allMail.descrizione = Resources.DescrizioneAllegatoMailRicevuta;
                    allMail.docNumber = sd.docNumber;
                    allMail.fileName = GetFileName(nomeMail);
                    allMail.version = "0";
                    allMail.numeroPagine = 0;
                    allMail.position = attachments.Count + 1;

                    DocsPaVO.documento.Allegato res3 = (await this._mediator.Send(new Requests.DocumentoAggiungiAllegato(new InfoUtente(), allMail))).output;

                    if (res3 == null)
                    {
                        err = Resources.ErroreAggiuntaAllEmailRicevuta;
                        throw new StandardCaseException(err);
                    }


                    DocsPaVO.documento.FileDocumento fdAllMail = new DocsPaVO.documento.FileDocumento();
                    fdAllMail.content = email.BinaryContent; //Funzionerà?
                    fdAllMail.length = fdAllMail.content.Length;
                    fdAllMail.name = nomeMail;
                    DocsPaVO.documento.FileRequest fRAllMail = new DocsPaVO.documento.FileRequest();
                    fRAllMail = (DocsPaVO.documento.FileRequest)allMail;

                    if (fdAllMail.content.Length > 0)
                    {
                        var putFileResult = await this._mediator.Send(new Requests.DocumentoPutFileNoException(fRAllMail, fdAllMail, new InfoUtente()));
                        var putFileOk = putFileResult.output;
                        err = putFileResult.errorMessage;
                        if (!putFileOk)
                            throw new StandardCaseException(Resources.ErroreFileAllegato);
                        else
                        {
                            bool tsrOk = await this.MatchTSR(email, fRAllMail, fdAllMail, idPeople);
                        }
                    } 
                }

                #endregion

                output.Success = true;
                output.ProcessedAttachments = processedAttachments;
                output.DocNumber = sd.docNumber?.AsLong() ?? 0;

                #region Trasmissione per interoperabilità
                // Per gestione pendenti tramite PEC
                DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                trasm.ruolo = this._mapper.Map<Ruolo>(ruolo);
                trasm.utente = (await this._mediator.Send(new Application.Requests.getUtenteById(idPeople.ToString()))).output;
                trasm.utente.dst = ""; // ?? poi vedo
                trasm.noteGenerali = "";
                DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
                trasm.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                trasm.infoDocumento = (await this._mediator.Send(new Application.Requests.GetInfoDocumento(new InfoUtente(), sd.systemId, sd.docNumber))).output;
                var ragioneEntity = await this._dbContext.RagioneTrasmissioneEntities
                    .Where(x => x.CHA_TIPO_RAGIONE.Equals("I") && x.CHA_TIPO_DIRITTI.Equals("W") && x.ID_AMM == idTenant)
                    .FirstOrDefaultAsync();
                DocsPaVO.trasmissione.RagioneTrasmissione ragione = this._mapper.Map<RagioneTrasmissione>(ragioneEntity);

                // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento ricevuto per interop interno solo a ruoli nella UO
                // destinataria della spedizione e non a tutta la AOO.
                List<Ruolo> ruoliDest = await this.GetRuoliDestTrasm(registroEntity, mailAddress);
                List<TrasmissioneSingola> trasmissioniSing = new List<TrasmissioneSingola>();
                foreach (var r in ruoliDest)
                {
                    //Aggiunta trasmissione singola
                    DocsPaVO.trasmissione.TrasmissioneSingola trSing = new DocsPaVO.trasmissione.TrasmissioneSingola();
                    trSing.ragione = ragione;
                    trSing.corrispondenteInterno = r;
                    trSing.tipoTrasm = "S";

                    trSing.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                    if (mailPendente)
                        trSing.ragione.eredita = "0";

                    DocsPaVO.addressbook.QueryCorrispondente qc = new DocsPaVO.addressbook.QueryCorrispondente();
                    qc.codiceRubrica = r.codiceRubrica;
                    List<string> registri = new List<string>();
                    registri.Add(registroEntity.SYSTEM_ID.ToString());
                    qc.idRegistri = registri.ToArray();
                    qc.idAmministrazione = registroEntity.ID_AMM.ToString();
                    qc.getChildren = true;
                    qc.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                    qc.fineValidita = true;

                    List<DocsPaVO.utente.Corrispondente> utenti = (await this._mediator.Send(new Requests.AddressbookGetListaCorrispondenti(qc))).output.ToList();

                    List<DocsPaVO.trasmissione.TrasmissioneUtente> trasmissioniUt = new List<DocsPaVO.trasmissione.TrasmissioneUtente>();

                    foreach (var utente in utenti)
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                        trUt.utente = (DocsPaVO.utente.Utente)utente;
                        trasmissioniUt.Add(trUt);
                    }
                    trSing.trasmissioneUtente = trasmissioniUt.ToArray();
                    trasmissioniSing.Add(trSing);
                }
                trasm.trasmissioniSingole = trasmissioniSing.ToArray();
                DocsPaVO.trasmissione.Trasmissione result = null;
                string desc = string.Empty;
                string method;
                result = (await this._mediator.Send(new Requests.TrasmissioneSaveExecuteTrasm(string.Empty, trasm, new InfoUtente()))).output;
                #endregion

            }
            catch (Pi3Exception pi3ex)
            {
                _logger.LogError(pi3ex.Message);
                output = new ProcessorOutput
                {
                    Success = false,
                    DocNumber = null,
                    ErrorMessage = string.Format(ErrorDescriptions.MailNonElaborata, !string.IsNullOrEmpty(err) ? err : pi3ex.Message),
                    ProcessedAttachments = 0
                };

                // Se il predisposto è stato creato lo devo cestinare
                if (!string.IsNullOrWhiteSpace(docnumber)) await this.TrashDocument(docnumber.AsLong());
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                if (string.IsNullOrEmpty(err))
                {
                    err = ErrorDescriptions.ErroreGenericoElaborazioneMail;
                }

                output = new ProcessorOutput
                {
                    Success = false,
                    DocNumber = null,
                    ErrorMessage = string.Format(ErrorDescriptions.MailNonElaborata, err),
                    ProcessedAttachments = 0
                };

                // Se il predisposto è stato creato lo devo cestinare
                if (!string.IsNullOrWhiteSpace(docnumber)) await this.TrashDocument(docnumber.AsLong());
            }

            return new StandardCaseResult(output);
        }

        //N.B: metodo derivato da DocumentoAddDocGrigiaHandler, stesso metodoo anche in SegnaturaHandler
        private async Task<SchedaDocumento> AddDocGrigia(SchedaDocumento schedaDocumento, string idTenant, Ruolo ruolo)
        {
            SchedaDocumento result = new SchedaDocumento();
            var fileRequest = schedaDocumento.documenti == null ? new() : schedaDocumento.documenti[0];
            try
            {

                //INIZIO CREAZIONE DOCUMENTO
                var documentoAmministrativoAggregate = new DocumentoAmministrativo(idTenant, DateTime.Now,
                    new OggettoDelDocumento()
                    {
                        Id = schedaDocumento.oggetto.systemId,
                        Descrizione = new TextValue(schedaDocumento.oggetto.descrizione),
                    },
                    new DatiRegistro()
                    {
                        IdRegistro = schedaDocumento.registro.systemId
                    },
                    null,
                    schedaDocumento.privato == "1" ? TipologieVisibilitaEnum.Privata : (schedaDocumento.personale == "1" ? TipologieVisibilitaEnum.Personale : TipologieVisibilitaEnum.Gerarchica)
                     );

                await _documentoAmministrativoRepository.Add(documentoAmministrativoAggregate);

                schedaDocumento.systemId = documentoAmministrativoAggregate.Id;
                schedaDocumento.docNumber = documentoAmministrativoAggregate.Id;
                schedaDocumento.dataCreazione = documentoAmministrativoAggregate.CreationDate.AsDateFormat();
                schedaDocumento.accessRights = "255";

                fileRequest.docNumber = schedaDocumento.systemId;
                fileRequest.versionId = documentoAmministrativoAggregate.Versions[0].Id;
                if (schedaDocumento.documenti == null)
                    schedaDocumento.documenti = new DocsPaVO.documento.Documento[] { fileRequest };
                else
                    schedaDocumento.documenti[0] = fileRequest;

                //FINE CREAZIONE DOCUMENTO

                await this._webMethodLoggerService.LogOK("ADDDOCGRIGIA", schedaDocumento.systemId, string.Format("N.ro Doc.: " + schedaDocumento.systemId));

             

                result = schedaDocumento;
            }
            catch (Exception ex)
            {

                this._logger.LogCritical(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("ADDDOCGRIGIA", schedaDocumento.systemId, string.Format("N.ro Doc.: " + schedaDocumento.systemId));
                schedaDocumento = null;
            }

            return result;
        }

        private async Task<bool> MatchTSR(Core.Services.Email.BoxScanner.Email email, DocsPaVO.documento.FileRequest fr, FileDocumento fd, long idPeople)
        {
            bool retval = false;
            string[] tsrFiles = email.Attachments.Where(x => x.FileName.ToLowerInvariant().EndsWith(".tsr")).Select(x => x.FileName).ToArray();
            //Se non ci sono TSR
            if (tsrFiles.Length == 0)
                return false;

            //Il file è un TSR, non associo un TSR a un TSR
            if (System.IO.Path.GetExtension(fr.fileName).ToLowerInvariant() == ".tsr")
                return false;

            string fdFileName = fr.fileName;
            foreach (string file in tsrFiles)
            {
                byte[] tsrFile;
                if (fdFileName.ToLowerInvariant().Equals(file.ToLowerInvariant()))
                    tsrFile = fd.content;
                else
                    tsrFile = email.Attachments.Where(x => x.FileName.Equals(file)).Select(x => x.Content).FirstOrDefault();

                //if(await this.ConfrontaTSR(tsrFile, fd.content))
                //{
                DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = await this.VerificaMarca(fd.content, tsrFile);
                if (resultMarca == null)
                    //throw new TimestampNullPi3Exception();
                    this._logger.LogError("ExecuteAndSaveTSR > Marca nulla");

                //Salvo la marca generata sul database
                var tsEntityToInsert = new TimestampDocEntity()
                {
                    DOC_NUMBER = fr.docNumber.AsLong(),
                    VERSION_ID = fr.versionId.AsLong(),
                    ID_PEOPLE = idPeople,
                    DTA_CREAZIONE = !string.IsNullOrEmpty(resultMarca.docm_date) ? resultMarca.docm_date.AsDateTime() : null,
                    DTA_SCADENZA = !string.IsNullOrEmpty(resultMarca.dsm) ? resultMarca.dsm.AsDateTime() : null,
                    NUM_SERIE = resultMarca.sernum,
                    S_N_CERTIFICATO = resultMarca.snCertificato,
                    ALG_HASH = resultMarca.algHash,
                    SOGGETTO = resultMarca.TSA.O,
                    PAESE = resultMarca.TSA.C,
                    TSR_FILE = resultMarca.marca
                };

                await this._dbContext.TimestampDocEntities.AddAsync(tsEntityToInsert);
                int rowsInserted = await ((DbContext)_dbContext).SaveChangesAsync();

                if (rowsInserted == 0)
                    this._logger.LogError("SegnaturaHandler > Errore nell'inserimento della marca temporale nella DPA_TIMESTAMP_DOC");
                else
                    retval = true;
                //}
            }
            return retval;
        }

        #endregion

        #region Private members 
        protected readonly ILogger<StandardCaseHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IRubricaComuneService _rubricaComuneService;
        protected readonly IFirmaDigitale2Service _firmaDigitale2Service;
        protected readonly IConfigurationService _configurationService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IDocumentoAmministrativoRepository _documentRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IUOCorrispondenteRepository _uoCorrispondenteRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        private readonly string BearerPrefix = "Bearer ";

        protected IMapper _mapper = null;
        private async Task<OutputResponseMarca> VerificaMarca(byte[] p7m, byte[] tSR)
        {
            OutputResponseMarca outTSR = null;
            var verificaMarcaResponse = await this._firmaDigitale2Service.Verifica(new VerificaRequest()
            {
                VerificaCompleta = false,
                FileFirmato = tSR,
                FileOriginale = p7m,
                DataVerifica = DateTime.Now,
                TipoVerifica = TipiVerifica.Incapsulata,
                ReturnFileOriginale = true,
                ReturnXmlCompleto = true,
            });

            if (verificaMarcaResponse != null)
            {
                outTSR = new OutputResponseMarca();
                XmlDocument doc = new XmlDocument();

                if (verificaMarcaResponse.Esito != null && verificaMarcaResponse.Esito.DatiGeneraliVerifica != null)
                    doc.LoadXml(verificaMarcaResponse.Esito.DatiGeneraliVerifica);
                else if (verificaMarcaResponse.Warning != null &&
                        verificaMarcaResponse.Warning.WarningFault != null &&
                        verificaMarcaResponse.Warning.WarningFault.Length != 0 &&
                        verificaMarcaResponse.Warning.DettaglioFirmaDigitale.FileMarcato &&
                        verificaMarcaResponse.Warning.DettaglioFirmaDigitale != null &&
                        verificaMarcaResponse.Warning.DettaglioFirmaDigitale.DatiGeneraliVerifica != null)
                {
                    doc.LoadXml(verificaMarcaResponse.Warning.DettaglioFirmaDigitale.DatiGeneraliVerifica);
                    for (int i = 0; i < verificaMarcaResponse.Warning.WarningFault.Length; i++)
                        outTSR.descrizioneErrore += !string.IsNullOrEmpty(outTSR.descrizioneErrore) ?
                            string.Concat(outTSR.descrizioneErrore, " - ", verificaMarcaResponse.Warning.WarningFault[i].ErrorMsg)
                            : verificaMarcaResponse.Warning.WarningFault[i].ErrorMsg;
                }

                if (doc != null)
                {
                    XmlNode node = doc.DocumentElement;
                    XmlNode timestampNode = node.SelectSingleNode("/deSign/timeStamp");
                    XmlNode certNode = timestampNode.SelectSingleNode("certificate");

                    var certBytes = Encoding.UTF8.GetBytes(certNode.InnerText);
                    var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certBytes);

                    if (DateTime.Now.CompareTo(cert.NotAfter.ToLocalTime()) > 0)
                        outTSR.descrizioneErrore = Resources.ElapsedTimestamp;

                    outTSR.dsm = cert.NotAfter.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                    outTSR.sernum = int.Parse(timestampNode.SelectSingleNode("timeStampSerial")?.InnerText, System.Globalization.NumberStyles.HexNumber).ToString() ?? string.Empty;
                    string hexHash = BitConverter.ToString(p7m.ComputeHashAsSha256()).Replace("-", "").ToLowerInvariant();
                    outTSR.fhash = hexHash;
                    if (!string.IsNullOrEmpty(timestampNode.SelectSingleNode("verificationTime")?.InnerText))
                    {
                        outTSR.docm = DateTime.ParseExact(timestampNode.SelectSingleNode("verificationTime")?.InnerText, "yyMMddHHmmssZ", CultureInfo.InvariantCulture).ToString("HH:mm:ss");
                        outTSR.docm_date = DateTime.ParseExact(timestampNode.SelectSingleNode("verificationTime")?.InnerText, "yyMMddHHmmssZ", CultureInfo.InvariantCulture).AsDateTimeFormat();
                    }
                    outTSR.marca = Convert.ToBase64String(tSR); //BitConverter.ToString(tSR.ComputeHashAsSha256()).Replace("-", "").ToLowerInvariant(); //Convert.ToBase64String(ParseHex(timestampNode.SelectSingleNode("timeStampImprint")?.InnerText));
                    outTSR.fromDate = cert.NotBefore.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                    outTSR.snCertificato = int.Parse(cert.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
                    outTSR.TSA = new TSARFC2253()
                    {
                        TSARFC2253Name = String.Format("CN={0},OU={1},O={2},C={3}",
                            timestampNode.SelectSingleNode("issuer/CN")?.InnerText,
                            timestampNode.SelectSingleNode("issuer/OU")?.InnerText,
                            timestampNode.SelectSingleNode("issuer/O")?.InnerText,
                            timestampNode.SelectSingleNode("issuer/C")?.InnerText),
                        C = timestampNode.SelectSingleNode("issuer/C")?.InnerText,
                        CN = timestampNode.SelectSingleNode("issuer/CN")?.InnerText,
                        O = timestampNode.SelectSingleNode("issuer/O")?.InnerText,
                        OU = timestampNode.SelectSingleNode("issuer/OU")?.InnerText
                    };
                    System.Security.Cryptography.Oid oidHash = new System.Security.Cryptography.Oid(timestampNode.SelectSingleNode("timeStampImprintAlgorithm")?.InnerText);
                    outTSR.algHash = oidHash.FriendlyName;
                    outTSR.esito = "OK";
                }
            }
            return outTSR;
        }
        private async Task<DocsPaVO.utente.Registro> CaricaRegistroInScheda(DocsPaVO.utente.Registro registro)
        {
            DocsPaVO.utente.Registro reg = registro;

            //se è un RF allora ricerco il registro relativo alla Aoo Collegata
            if (registro.chaRF != null && registro.chaRF == "1")
                reg = (await this._mediator.Send(new GetRegistroBySistemId(registro.idAOOCollegata))).output;

            return reg;
        }
        private async Task<bool> InsertAssDocAddress(long docNumber, long idRegistro, string mailAddress)
        {
            var assDocMailInterop = new AssDocMailInteropEntity()
            {
                ID_PROFILE = docNumber,
                ID_REGISTRO = idRegistro,
                VAR_EMAIL_REGISTRO = mailAddress
            };

            await this._dbContext.AssDocMailInteropEntities.AddAsync(assDocMailInterop);
            return (await ((DbContext)this._dbContext).SaveChangesAsync()) == 1;

        }
        private Object XML_Serialization_Deserialization_By_Encode(Object objToSerialize, Type t1, Type[] t2, System.Text.Encoding encode)
        {
            #region serializzazione
            System.Xml.Serialization.XmlSerializer serializer = null;
            if (t2 != null)
            {
                serializer = new System.Xml.Serialization.XmlSerializer(
                    t1,
                    t2
                    );
            }
            else
                serializer = new System.Xml.Serialization.XmlSerializer(
                    t1
                    );

            MemoryStream memoryStream = new MemoryStream();
            //Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            //StreamWriter streamWriter = new StreamWriter(memoryStream, iso);
            StreamWriter streamWriter = new StreamWriter(memoryStream, encode);
            serializer.Serialize(streamWriter, objToSerialize);

            streamWriter.Flush();

            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);

            string resultSer = sr.ReadToEnd();

            if (!string.IsNullOrEmpty(resultSer))
            {
                //
                // Invalid Character for XML 1.0
                // &#xhhhhhhh..hh;, essentially is random binary data that can not be printed
                while (resultSer.Contains("&#x"))
                {
                    int startIndx = -1;
                    int endIndx = -1;
                    startIndx = resultSer.IndexOf("&#x");
                    if (startIndx != -1)
                    {
                        endIndx = resultSer.IndexOf(";", startIndx);
                        int numCharToDelete = (endIndx + 1) - startIndx;

                        resultSer = resultSer.Remove(startIndx, numCharToDelete);
                    }
                }
            }
            #endregion

            #region deserializzazione

            //// Test 1
            TextReader reader = new StringReader(resultSer);
            Object obj = serializer.Deserialize(reader);
            reader.Close();

            #endregion
            return obj;
        }
        private async Task<bool> MantieniMailRicevutePendenti(long registerId, string mailAddress)
        {
            var entities = await this._dbContext.MailRegistriEntities
                .Where(x => x.ID_REGISTRO == registerId && x.VAR_EMAIL_REGISTRO.Equals(mailAddress))
                .Select(x => new
                {
                    x.VAR_MAIL_RIC_PENDENTE,
                    x.VAR_SOLO_MAIL_PEC
                }).ToListAsync();

            return entities.Any(x => x.VAR_SOLO_MAIL_PEC != null && !x.VAR_SOLO_MAIL_PEC.Equals("1") && x.VAR_MAIL_RIC_PENDENTE != null && x.VAR_MAIL_RIC_PENDENTE.Equals("1"));
        }

        private async Task TrashDocument(long docnumber)
        {
            var profileEntity = await this._dbContext.ProfileEntities.FirstAsync(x => x.DOCNUMBER == docnumber);

            profileEntity.CHA_IN_CESTINO = "1";

            var attachmentList = await this._dbContext.ProfileEntities
                .Where(x => x.ID_DOCUMENTO_PRINCIPALE == docnumber)
                .ToListAsync();

            attachmentList.ForEach(a =>
            {
                a.CHA_IN_CESTINO = "1";
            });

            await ((DbContext)this._dbContext).SaveChangesAsync();
        }
        private async Task<(DocsPaVO.utente.Corrispondente?, string)> GetCorrispondenteByEmail(string email, string idRegistro, long idTenant)
        {
            var idPeople = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGruppo = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idRuoloInUo = await _dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();
            var infoUtente = new InfoUtente()
            {
                idPeople = idPeople.ToString(),
                idGruppo = idGruppo.ToString(),
                idCorrGlobali = idRuoloInUo.ToString()
            };

            string checkSameMailbox = string.Empty;
            string messaggioErrore = string.Empty;
            DocsPaVO.utente.Corrispondente? corr = default;
            int rows = 0;

            var corrGlobaliQueryable = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(x => !x.DTA_FINE.HasValue
                        && (x.ID_AMM == idTenant || !x.ID_AMM.HasValue)
                        && (x.ID_REGISTRO == idRegistro.AsLong() || !x.ID_REGISTRO.HasValue)
                        && !(x.CHA_TIPO_CORR == "O"));

            var mailCorrEsterniQueryable = this._dbContext.MailCorrEsterniEntities.AsNoTracking()
            .Where(x => x.VAR_EMAIL.ToUpper() == email.ToUpper())
            .Select(x => x.ID_CORR);

            var predicate = PredicateBuilder.New<CorrGlobaliEntity>(true);

            predicate.Or(x => x.VAR_EMAIL.ToUpper() == email.ToUpper()
                && x.CHA_TIPO_IE == "I"
                && !(x.CHA_TIPO_URP == "P"));
            predicate.Or(x => mailCorrEsterniQueryable.Any(m => m == x.SYSTEM_ID));

            var corrGlobaliEntities = await corrGlobaliQueryable
                .Where(predicate)
                .ToListAsync();

            CorrGlobaliEntity? corrGlobaliEntity = null;
            foreach(var c in corrGlobaliEntities)
            {
                corrGlobaliEntity = c;
                if (!c.VAR_COD_RUBRICA.Contains("@") && (c.VAR_COD_RUBRICA.Length < 8 || !c.VAR_COD_RUBRICA.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                    break;
            }

            if (corrGlobaliEntity is not null)
            {
                var tipoIE = new DocsPaVO.addressbook.TipoUtente();
                switch (corrGlobaliEntity.CHA_TIPO_IE)
                {
                    case "I":
                        tipoIE = DocsPaVO.addressbook.TipoUtente.INTERNO;
                        break;
                    case "E":
                        tipoIE = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                        break;
                }

                corr = (await this._mediator.Send(new Requests.AddressbookGetCorrispondenteCompletoBySystemId(
                    corrGlobaliEntity.SYSTEM_ID.ToString(),
                    tipoIE,
                    infoUtente)))?.output;
            }

            // Rubrica comune

            bool confAb = (await this._mediator.Send(new Application.Requests.GetConfigurazioniRubricaComune(infoUtente))).output.GestioneAbilitata;
            if (confAb)
            {
                try
                {
                    DocsPaVO.utente.Corrispondente corr1 = null;
                    if (corr is null)
                    {
                        if (!string.IsNullOrEmpty(email))
                            corr1 = await this.UpdateCorrispondenteByEmail(email);
                        if (corr1 != null)
                        {
                            corr = corr1;
                            messaggioErrore = "RC";
                        }
                    }
                    else if (string.IsNullOrEmpty(corr.systemId) || corr.inRubricaComune)
                    {
                        corr1 = await this.UpdateCorrispondente(corr);
                        if (corr1 != null)
                        {
                            // inserisco un ulteriore controllo per evitare di cancellare i corrispondenti interni
                            if (!corr.tipoIE.ToUpper().Equals("I") && corr.systemId != corr1.systemId)
                            {
                                var corrispondentiDeleteModifyCorrispondenteEsternoRes = await this._mediator.Send(new Requests.CorrispondentiDeleteModifyCorrispondenteEsterno(infoUtente, new DatiModificaCorr { idCorrGlobali = corr.systemId }, 0, "D"));
                                bool cancellato = corrispondentiDeleteModifyCorrispondenteEsternoRes.output;
                                messaggioErrore = corrispondentiDeleteModifyCorrispondenteEsternoRes.message;
                                if (!string.IsNullOrEmpty(messaggioErrore)
                                    && !cancellato)
                                    messaggioErrore = "(CODINTEROP3)";
                            }
                            corr = corr1;
                            messaggioErrore = "RC";
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(email)
                            && corr != null)
                        {
                            corr1 = await this.UpdateCorrispondenteByEmail(email);
                            if (corr1 != null)
                            {
                                // inserisco un ulteriore controllo per evitare di cancellare i corrispondenti interni
                                if (!corr.tipoIE.ToUpper().Equals("I") && corr.systemId != corr1.systemId)
                                {
                                    var corrispondentiDeleteModifyCorrispondenteEsternoRes = await this._mediator.Send(new Requests.CorrispondentiDeleteModifyCorrispondenteEsterno(infoUtente, new DatiModificaCorr { idCorrGlobali = corr.systemId }, 0, "D"));
                                    bool cancellato = corrispondentiDeleteModifyCorrispondenteEsternoRes.output;
                                    messaggioErrore = corrispondentiDeleteModifyCorrispondenteEsternoRes.message;
                                    if (!string.IsNullOrEmpty(messaggioErrore)
                                        && !cancellato)
                                        messaggioErrore = "(CODINTEROP3)";
                                }
                                corr = corr1;
                                messaggioErrore = "RC";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogWarning($"Errore in ricerca corrispondente in rubrica comune (StandardCaseHandler): {ex.Message}");
                }
            }

            checkSameMailbox = await this._configurationService.GetValue<string>(idTenant.ToString(), "CHECK_MAILBOX_INTEROPERANTE");
            if (!string.IsNullOrEmpty(checkSameMailbox) && checkSameMailbox.Equals("1"))
            {
                if (rows > 1)
                    messaggioErrore += "#2";
                else if (rows == 1)
                    messaggioErrore += "#1";
            }

            return (corr, messaggioErrore);
        }

        private async Task<DocsPaVO.utente.Corrispondente> UpdateCorrispondenteByEmail(string email)
        {
            string idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            bool rubEstAttive = false;

            (string value, bool keyFound) = await this._configurationService.TryGetValue<string>(idTenant, "BE_ENABLE_RUBRICHE_ESTERNE");

            if (keyFound && !string.IsNullOrEmpty(value) && value.Equals("1"))
            {
                rubEstAttive = true;
            }

            (string beKey, bool beRubFound) = await this._configurationService.TryGetValue<string>("BE_RUBRICHE_ESTERNE");


            if (rubEstAttive && beRubFound && !string.IsNullOrEmpty(beKey) && !beKey.Equals("0"))
            {
                var elemento = await this.GetElementiInRubricaCom(email, beKey);

                if (elemento != null)
                    return await this.GetDettAndUpdateCorr(elemento);
                else
                    return null;
            }

            return null;

        }
        private async Task<(bool, string)> CorrInRubCom(string codiceRubrica, string idAmministrazione)
        {
            bool found = false;

            long corr = 0;
            if (!string.IsNullOrEmpty(codiceRubrica))
                corr = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                              where a.VAR_COD_RUBRICA != null &&
                              a.VAR_COD_RUBRICA.ToUpper().Equals(codiceRubrica.ToUpper()) &&
                              a.CHA_TIPO_CORR != null && a.CHA_TIPO_CORR.Equals("C")
                              select a.SYSTEM_ID).FirstOrDefaultAsync();
            if (corr != 0)
                found = true;

            return (found, corr != 0 ? corr.ToString() : string.Empty);
        }
        private async Task<DocsPaVO.addressbook.DettagliCorrispondente> GetDettagliCorr(string sysId)
        {

            DocsPaVO.addressbook.DettagliCorrispondente dett = new();

            var dettCorr = await (from a in this._dbContext.DettGlobaliEntities.AsNoTracking()
                                  where a.ID_CORR_GLOBALI == sysId.AsLong()
                                  select a).FirstOrDefaultAsync();

            if (dettCorr != null)
            {
                dett.Corrispondente.AddCorrispondenteRow(
                    dettCorr.VAR_INDIRIZZO ?? string.Empty,
                    dettCorr.VAR_CITTA ?? string.Empty,
                    dettCorr.VAR_CAP ?? string.Empty,
                    dettCorr.VAR_PROVINCIA ?? string.Empty,
                    dettCorr.VAR_NAZIONE ?? string.Empty,
                    dettCorr.VAR_TELEFONO ?? string.Empty,
                    dettCorr.VAR_TELEFONO2 ?? string.Empty,
                    dettCorr.VAR_FAX ?? string.Empty,
                    dettCorr.VAR_COD_FISC ?? string.Empty,
                    dettCorr.VAR_NOTE ?? string.Empty,
                    dettCorr.VAR_LOCALITA ?? string.Empty,
                    dettCorr.VAR_LUOGO_NASCITA ?? string.Empty,
                    dettCorr.DTA_NASCITA != null ? dettCorr.DTA_NASCITA : string.Empty,
                    dettCorr.VAR_TITOLO ?? string.Empty,
                    dettCorr.VAR_COD_PI ?? string.Empty
                    );
            }
            else
            {
                dett.Corrispondente.AddCorrispondenteRow("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }
            return dett;
        }
        private async Task<(DocsPaVO.utente.Corrispondente, bool)> Update(DocsPaVO.utente.Corrispondente corrispondente, Services.RubricaComune.Corrispondente elemento)
        {
            var idPeople = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGruppo = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idRuoloInUo = await _dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();
            var infoUtente = new InfoUtente()
            {
                idPeople = idPeople.ToString(),
                idGruppo = idGruppo.ToString(),
                idCorrGlobali = idRuoloInUo.ToString()
            };

            bool isCorrModified = false;
            bool requestNew = (corrispondente == null);
            bool idDirty = false;
            string codFisc = "";
            string pIva = "";
            bool variazioneEmail = false;

            var elUrls = new List<string?>();
            if (!string.IsNullOrEmpty(elemento.UrlApiInteroperabilita))
            {
                elUrls.Add(elemento.UrlApiInteroperabilita);
            }

            if (!requestNew)
            {
                if (corrispondente.dettagli)
                {
                    if (corrispondente.info == null)
                    {
                        corrispondente.info = await this.GetDettagliCorr(corrispondente.systemId);
                    }

                    codFisc = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corrispondente.info.Tables[0].Rows[0])).codiceFiscale ?? string.Empty;
                    pIva = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corrispondente.info.Tables[0].Rows[0])).partitaIva ?? string.Empty;
                }

                idDirty = (corrispondente.descrizione != elemento.Denominazione ||
                            (corrispondente.codiceAmm ?? string.Empty) != (elemento.Amministrazione ?? string.Empty) ||
                            (corrispondente.codiceAOO ?? string.Empty) != (elemento.AOO ?? string.Empty) ||
                            (corrispondente.email ?? string.Empty) != (elemento.Email ?? string.Empty) ||
                            codFisc.ToUpper() != (elemento.CodiceFiscale ?? string.Empty).ToUpper() ||
                            pIva.ToUpper() != (elemento.PartitaIva ?? string.Empty).ToUpper());

                if (!idDirty)
                {
                    var urlCount = !string.IsNullOrEmpty(elemento.UrlApiInteroperabilita) ? 1 : 0;
                    idDirty = urlCount != corrispondente.Url.Count;
                    if (!idDirty && urlCount == corrispondente.Url.Count && corrispondente.Url.Count > 0)
                    {
                        idDirty = corrispondente.Url[0].Url != elemento.UrlApiInteroperabilita;
                    }
                }

                if (!idDirty)
                {
                    if (elemento.Emails != null && elemento.Emails.Count > 0) //verifica se da RC torna che il corr ha almeno una mail, altrimenti tutto il controllo sotto non ha senso
                    {
                        if (elemento.Emails.Count > 1)
                            idDirty = elemento.Emails.Count != corrispondente.Emails.Count;
                        else
                            idDirty = (elemento.Email ?? string.Empty).ToLower() != (corrispondente.email ?? string.Empty).ToLower();

                        if (elemento.Emails.Count != corrispondente.Emails.Count)
                            variazioneEmail = true;
                    }

                    if (!idDirty)
                    {
                        foreach (var mail in elemento.Emails)
                        {
                            idDirty &= !corrispondente.Emails.Contains(new MailCorrispondente() { Email = mail.Indirizzo });
                        }
                    }
                }
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                (string isEnabledSimplifiedInterop, bool keyFound) = await this._configurationService.TryGetValue<string>(idTenant, "INTEROP_SERVICE_ACTIVE");

                if (string.IsNullOrEmpty(isEnabledSimplifiedInterop))
                {
                    (isEnabledSimplifiedInterop, keyFound) = await this._configurationService.TryGetValue<string>("INTEROP_SERVICE_ACTIVE");
                }
                bool IsEnabledSimplifiedInteroperability = isEnabledSimplifiedInterop != null ? isEnabledSimplifiedInterop.Equals("1") : false;


                if (((corrispondente.canalePref != null && (corrispondente.canalePref.typeId == Resources.InteroperabilityCode ||
                        corrispondente.canalePref.tipoCanale == Resources.InteroperabilityCode))
                        && !IsEnabledSimplifiedInteroperability) ||
                        (((corrispondente.canalePref != null && corrispondente.canalePref.typeId != Resources.InteroperabilityCode &
                        corrispondente.canalePref.tipoCanale != Resources.InteroperabilityCode)) &&
                        corrispondente.Url.Count > 0 && Uri.IsWellFormedUriString(corrispondente.Url[0].Url, UriKind.Absolute)))
                    idDirty = true;

                if (!idDirty)
                {
                    DocsPaVO.addressbook.DettagliCorrispondente oldDettagli = (DocsPaVO.addressbook.DettagliCorrispondente)corrispondente.info;

                    if (oldDettagli == null)
                    {
                        oldDettagli = await this.GetDettagliCorr(corrispondente.systemId);
                    }
                    if (oldDettagli.Corrispondente.Rows.Count > 0)
                    {
                        DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow oldRow = (DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)oldDettagli.Corrispondente.Rows[0];

                        idDirty = ((oldRow.indirizzo ?? string.Empty) != (elemento.Indirizzo ?? string.Empty) ||
                                            (oldRow.citta ?? string.Empty) != (elemento.Citta ?? string.Empty) ||
                                            (oldRow.cap ?? string.Empty) != (elemento.CAP ?? string.Empty) ||
                                            (oldRow.provincia ?? string.Empty) != (elemento.Provincia ?? string.Empty) ||
                                            (oldRow.nazione ?? string.Empty) != (elemento.Nazione ?? string.Empty) ||
                                            (oldRow.telefono ?? string.Empty) != (elemento.Telefono ?? string.Empty) ||
                                            (oldRow.fax ?? string.Empty) != (elemento.Fax ?? string.Empty) ||
                                            (oldRow.codiceFiscale ?? string.Empty) != (elemento.CodiceFiscale ?? string.Empty) ||
                                            (oldRow.partitaIva ?? string.Empty) != (elemento.PartitaIva ?? string.Empty));
                    }
                }
            }
            if (requestNew)
            {
                DocsPaVO.utente.Corrispondente newCorr = await this.GetNuovoCorr(elemento, elemento.Email, elUrls);
                corrispondente = (await this._mediator.Send(new Application.Requests.AddressbookInsertCorrispondente(newCorr, null, new InfoUtente()))).output;
                if (corrispondente != null && !string.IsNullOrEmpty(corrispondente.errore))
                {
                    if (!newCorr.inRubricaComune)
                        throw new ApplicationException(corrispondente.errore);
                    else
                    {
                        newCorr.errore = corrispondente.errore;
                        isCorrModified = true;
                        return (corrispondente, isCorrModified);
                    }
                }
            }
            else if (idDirty)
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                corrispondente.descrizione = elemento.Denominazione;
                corrispondente.codiceAmm = elemento.Amministrazione;
                corrispondente.codiceAOO = elemento.AOO;
                corrispondente.email = elemento.Email;
                corrispondente.Url = GetInternalUrlsCollection(elUrls);
                corrispondente.canalePref = await GetCanaleCorrispondente(corrispondente, idTenant);
                DocsPaVO.utente.DatiModificaCorr datiModifica = this.GetDatiPerModifica(corrispondente.systemId, corrispondente.canalePref.systemId, elemento, elemento.Email, elUrls);

                string newIdCorrGlobali;
                string message;

                var res = (await this._mediator.Send(new Application.Requests.CorrispondentiDeleteModifyCorrispondenteEsternoWithId(infoUtente, datiModifica, 0, "M")));

                if (res.output)
                {
                    var newCorrispondente = res.newIdCorr;
                    if (!string.IsNullOrEmpty(newCorrispondente) && (!newCorrispondente.Equals("0")))
                    {
                        corrispondente.idOld = corrispondente.systemId;
                        corrispondente.systemId = newCorrispondente;
                    }

                    DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();
                    dettagli.Corrispondente.AddCorrispondenteRow(elemento.Indirizzo, elemento.Citta, elemento.CAP, elemento.Provincia,
                                                                 elemento.Nazione, elemento.Telefono, string.Empty, elemento.Fax, elemento.CodiceFiscale,
                                                                 string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, elemento.PartitaIva);
                    corrispondente.info = dettagli;
                }
                else
                {
                    throw new StandardCaseException(Resources.ModifyCorrFailed);
                }

                if (variazioneEmail)
                {
                    List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();

                    foreach (var mail in elemento.Emails)
                    {
                        listCaselle.Add(new MailCorrispondente
                        {
                            Email = mail.Indirizzo,
                            Note = mail.Note,
                            Principale = mail.Preferita == true ? "1" : "0"
                        });
                    }

                    var re = await this._mediator.Send(new Application.Requests.InsertMailCorrispondenteEsterno(listCaselle, corrispondente.systemId));
                    if (!re.output)
                    {
                        throw new StandardCaseException(Resources.ErrorModifyMail);
                    }

                }

            }
            isCorrModified = idDirty;
            return (corrispondente, isCorrModified);
        }
        private DocsPaVO.utente.DatiModificaCorr GetDatiPerModifica(string idOld, string idCanalePref, Services.RubricaComune.Corrispondente elemento, string email, List<string> urls)
        {
            DocsPaVO.utente.DatiModificaCorr datiModifica = new DocsPaVO.utente.DatiModificaCorr();

            datiModifica.idCorrGlobali = idOld;
            datiModifica.idCanalePref = idCanalePref;
            datiModifica.codice = elemento.Codice;
            datiModifica.codRubrica = elemento.Codice;
            datiModifica.descCorr = elemento.Denominazione;
            datiModifica.codiceAmm = elemento.Amministrazione;
            datiModifica.codiceAoo = elemento.AOO;
            datiModifica.indirizzo = elemento.Indirizzo;
            datiModifica.citta = elemento.Citta;
            datiModifica.cap = elemento.CAP;
            datiModifica.provincia = elemento.Provincia;
            datiModifica.nazione = elemento.Nazione;
            datiModifica.telefono = elemento.Telefono;
            datiModifica.telefono2 = string.Empty;
            datiModifica.email = email;
            datiModifica.fax = elemento.Fax;
            datiModifica.codFiscale = string.Empty;
            datiModifica.nome = string.Empty;
            datiModifica.inRubricaComune = true;
            datiModifica.Urls = GetInternalUrlsCollection(urls);
            datiModifica.codFiscale = elemento.CodiceFiscale;
            datiModifica.partitaIva = elemento.PartitaIva;
            if (elemento.Tipo.ToString().Equals("RaggruppamentoFunzionale"))
                datiModifica.tipoCorrispondente = "F";
            else
                if (elemento.Tipo.ToString().Equals("UnitaOrganizzativa"))
                datiModifica.tipoCorrispondente = "U";

            return datiModifica;
        }

        // CHAN |MAIL <=> email has val and aoo,amm are without val
        // CHAN |interop semp <=> aoo,amm has val and url
        // CHAN |interop classica <=> aoo,amm has val and has email 
        private async Task<DocsPaVO.utente.Canale?> GetCanaleCorrispondente(DocsPaVO.utente.Corrispondente corrispondente, string idTenant)
        {
            DocsPaVO.utente.Canale? output = null;
            bool canaleMail = false;
            bool canaleInterop = false;
            bool canaleInteropSemplificata = false;


            if (!string.IsNullOrEmpty(corrispondente.codiceAmm) && !string.IsNullOrEmpty(corrispondente.codiceAOO))
            {
                canaleInteropSemplificata = await this.IsEnabledSimplifiedInteroperability(idTenant) &&
                    corrispondente.Url != null &&
                    corrispondente.Url.Count > 0 &&
                    Uri.IsWellFormedUriString(corrispondente.Url[0].Url, UriKind.Absolute);
                canaleInterop = !string.IsNullOrEmpty(corrispondente.email) && !canaleInteropSemplificata;
            }
            else if (!string.IsNullOrEmpty(corrispondente.email))
            {
                canaleInterop = false;
                canaleInteropSemplificata = false;
                canaleMail = true;
            }

            //GET CHANNEL LIST
            var orderdChanEnt = await this._dbContext.DocumentTypesEntities.AsNoTracking().OrderBy(c => c.DESCRIPTION).ToListAsync();
            foreach (var c in orderdChanEnt)
            {
                if ((Channel.Interop == c.TYPE_ID && canaleInterop && !canaleInteropSemplificata) ||
                    (Channel.Mail == c.TYPE_ID && canaleMail) ||
                    (Channel.Lettera == c.TYPE_ID && !canaleMail && !canaleInterop && !canaleInteropSemplificata) ||
                    (Channel.Simpinterop == c.TYPE_ID && canaleInteropSemplificata))
                {
                    output = new()
                    {
                        systemId = c.SYSTEM_ID.ToString(),
                        typeId = c.TYPE_ID,
                        descrizione = c.DESCRIPTION ?? string.Empty,
                        tipoCanale = c.CHA_TIPO_CANALE ?? string.Empty
                    };
                    return output;

                }
            }
            return output;
        }

        private async Task<bool> IsEnabledSimplifiedInteroperability(string idAmm)
        {
            bool enabled = false;
            (string? enabledForAmm, bool keyFound) = await this._configurationService.TryGetValue<string>(idAmm, "INTEROP_SERVICE_ACTIVE");

            if (keyFound)
            {
                enabled = "1".Equals(enabledForAmm);
            }
            else
            {
                (string? en, bool found) = await this._configurationService.TryGetValue<string>("INTEROP_SERVICE_ACTIVE");
                enabled = "1".Equals(en);
            }


            return enabled;
        }

        protected string? GetAuthToken()
        {
            //if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            //var authorizationHeader = authorizationStrings[0]!.StartsWith(BearerPrefix) ? authorizationStrings[0]!.Substring(BearerPrefix.Length) : authorizationStrings[0];
            var authorizationHeader = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>("KeyToken");

            return authorizationHeader;
        }
        private List<DocsPaVO.utente.Corrispondente.UrlInfo> GetInternalUrlsCollection(List<string> urlInfo)
        {
            List<DocsPaVO.utente.Corrispondente.UrlInfo> retCollection = new();
            if (urlInfo != null)
                retCollection.AddRange(from url in urlInfo
                                       select new DocsPaVO.utente.Corrispondente.UrlInfo() { Url = url });

            return retCollection;

        }
        private DocsPaVO.utente.Corrispondente InitializeSpecificAttributes(Tipi tipo, string codice)
        {
            DocsPaVO.utente.Corrispondente corrispondente = new();

            switch (tipo)
            {
                case Tipi.UnitaOrganizzativa:
                    corrispondente = new DocsPaVO.utente.UnitaOrganizzativa() { codice = codice, tipoCorrispondente = "U" };
                    break;
                case Tipi.RaggruppamentoFunzionale:
                    corrispondente = new DocsPaVO.utente.RaggruppamentoFunzionale() { Codice = codice, tipoCorrispondente = "F" };
                    break;
                default:
                    corrispondente = new DocsPaVO.utente.Corrispondente();
                    break;

            }

            return corrispondente;
        }
        private async Task<DocsPaVO.utente.Corrispondente> GetNuovoCorr(Services.RubricaComune.Corrispondente elemento, string email, List<string> urls)
        {

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            var corrispondente = this.InitializeSpecificAttributes(elemento.Tipo, elemento.Codice);
            corrispondente.inRubricaComune = true;
            corrispondente.codiceRubrica = elemento.Codice;
            corrispondente.descrizione = elemento.Denominazione;
            corrispondente.email = email;
            corrispondente.codiceAmm = elemento.Amministrazione;
            corrispondente.codiceAOO = elemento.AOO;
            corrispondente.tipoIE = "E";
            corrispondente.indirizzo = elemento.Indirizzo;
            corrispondente.Url = GetInternalUrlsCollection(urls);

            corrispondente.canalePref = await GetCanaleCorrispondente(corrispondente, idTenant);
            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();

            dettagliCorrispondente.Corrispondente.AddCorrispondenteRow
                (
                    elemento.Indirizzo, elemento.Citta, elemento.CAP, elemento.Provincia, elemento.Nazione,
                    elemento.Telefono, string.Empty, elemento.Fax, elemento.CodiceFiscale, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, elemento.PartitaIva
                );

            corrispondente.dettagli = true;
            corrispondente.info = dettagliCorrispondente;

            return corrispondente;
        }
        private async Task<DocsPaVO.utente.Corrispondente> GetDettAndUpdateCorr(Services.RubricaComune.Corrispondente elemento)
        {
            DocsPaVO.utente.Corrispondente corr = null;

            (bool inRubCom, string sysId) = await this.CorrInRubCom(elemento.Codice, string.Empty);

            if (inRubCom)
            {
                corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteCompletoBySystemId(sysId, TipoUtente.ESTERNO, new InfoUtente()))).output;
                if (corr != null)
                {
                    corr.inRubricaComune = true;
                    corr.info = await this.GetDettagliCorr(corr.systemId);
                    corr.dettagli = true;
                }
            }

            (DocsPaVO.utente.Corrispondente corrispondente, bool isCorrModified) = await this.Update(corr, elemento);

            if (corrispondente != null && (!string.IsNullOrEmpty(corrispondente.systemId)))
            {
                var elementoEmails = await this._rubricaComuneService.GetEmails(this.GetAuthToken(), Convert.ToInt32(elemento.Id));
                var elementoEmail = elementoEmails.FirstOrDefault(e => e.Preferita == true);
                var elUrls = new List<string?>();
                if (elemento.UrlApiInteroperabilita != null)
                {
                    elUrls.Add(elemento.UrlApiInteroperabilita);
                }

                if (elementoEmails.Count > 0)
                {
                    List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();
                    foreach (var mail in elementoEmails)
                    {
                        listCaselle.Add(new MailCorrispondente
                        {
                            Email = mail.Indirizzo,
                            Note = mail.Note,
                            Principale = (mail.Preferita == true) ? "1" : "0"
                        });
                    }
                    var re = await this._mediator.Send(new Application.Requests.InsertMailCorrispondenteEsterno(listCaselle, corr.systemId));
                }
            }
            return corrispondente;
        }
        private async Task<Services.RubricaComune.Corrispondente> GetElementiInRubricaCom(string email, string rubEsterna)
        {
            var elementiRubrica = new List<Services.RubricaComune.Corrispondente>();
            Services.RubricaComune.Corrispondente elemento = null;
            var criteriRicerca = new List<CriterioRicerca>
            {
                new CriterioRicerca 
                { 
                    Campo = CampiRicercaEnum.Email, 
                    Valore = email,
                    TipoRicercaParola = TipiRicercaParolaEnum.ParolaIntera
                }
            };

            if (!string.IsNullOrEmpty(rubEsterna))
            {
                criteriRicerca.Add(
                    new()
                    {
                        Campo = CampiRicercaEnum.RubricaEsterna,
                        Valore = rubEsterna.ToUpper()
                    });

            }

            try
            {
                var response = await this._rubricaComuneService.Search(this.GetAuthToken(), new SearchRequest
                {
                    CriteriRicerca = criteriRicerca,
                    ElementiPerPagina = 50,
                    Pagina = 1
                });

                if (response.Corrispondenti.Any())
                {
                    elemento = new();
                    response.Corrispondenti.ForEach(c => elementiRubrica.Add(c));
                    elemento = elementiRubrica.FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return elemento;


        }

        private async Task<DocsPaVO.utente.Corrispondente?> UpdateCorrispondente(DocsPaVO.utente.Corrispondente corr)
        {
            var codiceRubrica = corr.codiceRubrica;
            bool rubEstAttive = false;
            string idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            (string value, bool keyFound) = await this._configurationService.TryGetValue<string>(idTenant, "BE_ENABLE_RUBRICHE_ESTERNE");

            if (keyFound && !string.IsNullOrEmpty(value) && value.Equals("1"))
            {
                rubEstAttive = true;
            }

            (string beKey, bool beRubFound) = await this._configurationService.TryGetValue<string>("BE_RUBRICHE_ESTERNE");

            if (rubEstAttive && beRubFound && !string.IsNullOrEmpty(beKey) && !beKey.Equals("0"))
            {
                var elemento = await this.GetElementiInRubricaComCod(codiceRubrica, beKey);

                if (elemento != null)
                    return await this.GetDettAndUpdateCorr(elemento);
                else
                    return null;
            }
            return null;
        }
        private async Task<Services.RubricaComune.Corrispondente> GetElementiInRubricaComCod(string codice, string rubEsterna)
        {
            var elementiRubrica = new List<Services.RubricaComune.Corrispondente>();
            Services.RubricaComune.Corrispondente elemento = null;
            var criteriRicerca = new List<CriterioRicerca>
            {
                new CriterioRicerca 
                {
                    Campo = CampiRicercaEnum.Codice, 
                    Valore = codice.Replace("'", "''"), 
                    TipoRicercaParola = TipiRicercaParolaEnum.ParolaIntera
                }
            };

            if (!string.IsNullOrEmpty(rubEsterna))
            {
                criteriRicerca.Add(
                    new()
                    {
                        Campo = CampiRicercaEnum.RubricaEsterna,
                        Valore = rubEsterna.ToUpper()
                    });
            }

            try
            {
                var response = await this._rubricaComuneService.Search(this.GetAuthToken(), new SearchRequest
                {
                    CriteriRicerca = criteriRicerca,
                    ElementiPerPagina = 50,
                    Pagina = 1
                });

                if (response.Corrispondenti.Any())
                {
                    response.Corrispondenti.ForEach(c => elementiRubrica.Add(c));
                    elemento = elementiRubrica.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return elemento;
        }
        private async Task<List<Ruolo>> GetRuoliDestTrasm(RegistroEntity registroEntity, string mailAddress)
        {
            List<Ruolo> ruoliDestTrasm = new List<Ruolo>();
            var interopNoMail = await this._configurationService.GetValue<string>("INTEROP_INT_NO_MAIL");

            var j1 = this._dbContext.CorrGlobaliEntities.Join(this._dbContext.TipoRuoloEntities, a => a.ID_TIPO_RUOLO, d => d.SYSTEM_ID, (a, d) => new { a, d });
            var j2 = j1.Join(this._dbContext.CorrGlobaliEntities, j1 => j1.a.ID_UO, e => e.SYSTEM_ID, (j1, e) => new { a = j1.a, d = j1.d, e });
            var j3 = j2.Join(this._dbContext.RuoloRegistroEntities, j2 => j2.a.SYSTEM_ID, f => f.ID_RUOLO_IN_UO, (j2, f) => new { a = j2.a, d = j2.d, e = j2.e, f });
            var j4 = j3.SelectMany(t1 => this._dbContext.VisMailRegistriEntities.Where(t2 => t1.f.ID_RUOLO_IN_UO == t2.ID_RUOLO_IN_UO && t1.f.ID_REGISTRO == t2.ID_REGISTRO).DefaultIfEmpty(),
                (j3, v) => new JoinEntity { a = j3.a, d = j3.d, e = j3.e, f = j3.f, v = v });

            var predicate = PredicateBuilder.New<JoinEntity>();

            predicate = predicate.And(x => x.a.CHA_TIPO_URP.Equals("R") && !x.a.DTA_FINE.HasValue);
            predicate = predicate.And(x => x.f.ID_REGISTRO == registroEntity.SYSTEM_ID);
            predicate = predicate.And(x => (x.a.CHA_DISABLED_TRASM != "1" || x.a.CHA_DISABLED_TRASM == null));
            predicate = predicate.And(x => x.v.CHA_NOTIFICA.Equals("1"));
            if (string.IsNullOrEmpty(mailAddress) && interopNoMail != null && interopNoMail != "0")
                predicate = predicate.And(x => string.IsNullOrEmpty(x.v.VAR_EMAIL_REGISTRO));
            else if (!string.IsNullOrEmpty(mailAddress))
                predicate = predicate.And(x => x.v.VAR_EMAIL_REGISTRO.Equals(mailAddress) || string.IsNullOrEmpty(x.v.VAR_EMAIL_REGISTRO));
            var resultRuoloFun = await j4.Where(predicate)
                .Select(x => new
                {
                    SYSTEM_ID = x.a.SYSTEM_ID,
                    VAR_CODICE = x.a.VAR_CODICE,
                    VAR_COD_RUBRICA = x.a.VAR_COD_RUBRICA,
                    VAR_DESC_RUOLO = x.d.VAR_DESC_RUOLO,
                    ID_GRUPPO = x.a.ID_GRUPPO,
                    ID_UO = x.a.ID_UO,
                    NUM_LIVELLO = x.d.NUM_LIVELLO,
                    VAR_DESC_CORR = x.e.VAR_DESC_CORR
                })
                .ToListAsync();

            foreach (var item in resultRuoloFun)
            {
                ruoliDestTrasm.Add(new Ruolo()
                {
                    systemId = item.SYSTEM_ID.ToString(),
                    codiceCorrispondente = item.VAR_CODICE,
                    codiceRubrica = item.VAR_COD_RUBRICA,
                    descrizione = string.Concat(item.VAR_DESC_RUOLO, " ", item.VAR_DESC_CORR),
                    livello = item.NUM_LIVELLO.ToString(),
                    idGruppo = item.ID_GRUPPO.ToString(),
                    uo = new DocsPaVO.utente.UnitaOrganizzativa() { systemId = item.ID_UO.ToString() },
                    tipoCorrispondente = "R"
                });
            }

            return ruoliDestTrasm;
        }

        private async Task<DocsPaVO.documento.Applicazione> GetApp(string filename)
        {
            char[] dot = { '.' };
            string[] parts = filename.Split(dot);
            string suffix = parts[parts.Length - 1];
            if (suffix.ToUpper().Equals("P7M"))
                _logger.LogDebug("GetApplicazioni > File p7m");

            _logger.LogDebug("GetApplicazioni > Suffisso:" + suffix);
            DocsPaVO.documento.Applicazione res = null;

            System.Collections.ArrayList result = new System.Collections.ArrayList();
            System.Collections.ArrayList apps = await this.GetApplicazioni(suffix, result);
            _logger.LogDebug("App:" + apps.Count);

            if (apps.Count > 0)
                res = (DocsPaVO.documento.Applicazione)apps[0];
            return res;
        }

        private async Task<ArrayList> GetApplicazioni(string estensione, ArrayList res)
        {
            var apps = await this._dbContext.AppEntities
                .Where(x => x.DEFAULT_EXTENSION.ToUpper().Equals(estensione.ToUpper()))
                .ToListAsync();

            foreach (var app in apps)
                res.Add(this._mapper.Map<Applicazione>(app));

            _logger.LogDebug($"GetApplicazioni > apps.Count: {apps.Count}, res.Count: {res.Count}");
            if (apps.Count == 0)
            {
                if (await InsertApp(estensione))
                    return await this.GetApplicazioni(estensione, res);
                else
                    return res;
            }
            else
                return res;
        }

        private async Task<bool> InsertApp(string? estensione)
        {
            var result = false;
            try
            {
                _logger.LogInformation("InsertApp:" + estensione);
                if (!string.IsNullOrEmpty(estensione) && !await _dbContext.AppEntities.AnyAsync(a => a.APPLICATION.ToUpper() == "GEN_" + estensione.ToUpper()))
                {
                    var appEntity = new AppEntity()
                    {
                        APPLICATION = "GEN_" + estensione,
                        DESCRIPTION = "GEN_" + estensione,
                        FILING_SCHEME = 2,
                        DEFAULT_EXTENSION = estensione
                    };

                    await this._dbContext.AppEntities.AddAsync(appEntity);
                    result = await ((DbContext)this._dbContext).SaveChangesAsync() == 1;
                }
            }
            catch (Exception ex)
            {
                var pendingAppEntity = ((DbContext)_dbContext).ChangeTracker.Entries<AppEntity>()
                    .Where(a => a.State == EntityState.Added)
                    .FirstOrDefault();

                if (pendingAppEntity != null)
                    pendingAppEntity.State = EntityState.Detached;

                _logger.LogError("InsertApp errore:" + ex.Message);
                result = false;
            }
            return result;
        }

        private async Task<bool> MailElaborata(string idMessage, string ragione)
        {
            var mailElaborataEntity = new MailElaborataEntity()
            {
                VAR_MESSAGE = idMessage.Replace("'", "''"),
                CHA_RAGIONE_ELAB = ragione,
                DTA_ELAB = DateTime.Now,
                ID_REGISTRO = null,
                ID_PROFILE = null,
                VAR_EMAIL = null
            };

            await this._dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);
            return await ((DbContext)this._dbContext).SaveChangesAsync() == 1;
        }

        private string GetFileName(string fileName)
        {
            string res = null;
            if (fileName.Substring(fileName.LastIndexOf(".") + 1).ToUpper().Equals("P7M"))
            {
                res = fileName.Substring(0, fileName.LastIndexOf("."));
            }
            else
            {
                res = fileName;
            }
            return res;
        }

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrGlobaliEntity, Ruolo>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_CORR));

                cfg.CreateMap<RagioneTrasmissioneEntity, RagioneTrasmissione>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_RAGIONE))
                    .ForMember(dest => dest.risposta, opt => opt.MapFrom(src => src.CHA_RISPOSTA))
                    .ForMember(dest => dest.tipo, opt => opt.MapFrom(src => "N"))
                    .ForMember(dest => dest.tipoDestinatario, src => src.MapFrom(opt => RagioneTrasmissione.tipoGerarchiaStringa.Keys.OfType<TipoGerarchia>().FirstOrDefault(s => RagioneTrasmissione.tipoGerarchiaStringa[s].Equals(opt.CHA_TIPO_DEST))))
                    .ForMember(dest => dest.tipoDiritti, opt => opt.MapFrom(src => DocsPaVO.trasmissione.TipoDiritto.WRITE))
                    .ForMember(dest => dest.eredita, opt => opt.MapFrom(src => src.CHA_EREDITA));

                cfg.CreateMap<DocumentTypesEntity, Canale>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.tipoCanale, opt => opt.MapFrom(src => src.CHA_TIPO_CANALE))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.DESCRIPTION))
                    .ForMember(dest => dest.typeId, opt => opt.MapFrom(src => src.TYPE_ID));

                cfg.CreateMap<PeopleEntity, DocsPaVO.utente.Utente>()
                    .ForMember(dest => dest.idPeople, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.USER_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => $"{src.VAR_COGNOME} {src.VAR_NOME}"))
                    .ForMember(dest => dest.telefono, opt => opt.MapFrom(src => src.VAR_TELEFONO))
                    .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.EMAIL_ADDRESS))
                    .ForMember(dest => dest.notifica, opt => opt.MapFrom(src => src.CHA_NOTIFICA))
                    .ForMember(dest => dest.amministratore, opt => opt.MapFrom(src => src.CHA_AMMINISTRATORE == "1"))
                    .ForMember(dest => dest.assegnante, opt => opt.MapFrom(src => src.CHA_AMMINISTRATORE == "1"))
                    .ForMember(dest => dest.assegnatario, opt => opt.MapFrom(src => src.CHA_AMMINISTRATORE == "1"))
                    .ForMember(dest => dest.idAmministrazione, opt => opt.MapFrom(src => src.ID_AMM))
                    .ForMember(dest => dest.notificaConAllegato, opt => opt.MapFrom(src => src.CHA_NOTIFICA_CON_ALLEGATO == "1"))
                    .ForMember(dest => dest.sede, opt => opt.MapFrom(src => src.VAR_SEDE))
                    .ForMember(dest => dest.matricola, opt => opt.MapFrom(src => src.MATRICOLA ?? null))
                    .ForMember(dest => dest.tipoCorrispondente, opt => opt.MapFrom(src => "P"))
                    .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.VAR_COGNOME))
                    .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.VAR_NOME));

                cfg.CreateMap<AppEntity, DocsPaVO.documento.Applicazione>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.DESCRIPTION))
                    .ForMember(dest => dest.mimeType, opt => opt.MapFrom(src => src.MIME_TYPE))
                    .ForMember(dest => dest.estensione, opt => opt.MapFrom(src => src.DEFAULT_EXTENSION));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
