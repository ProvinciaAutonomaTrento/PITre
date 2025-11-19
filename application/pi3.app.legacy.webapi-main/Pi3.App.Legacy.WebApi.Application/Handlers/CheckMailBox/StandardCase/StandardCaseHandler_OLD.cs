// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.addressbook;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.StandardCase
{
    internal class StandardCaseHandler_OLD : IRequestHandler<StandardCaseRequest, StandardCaseResult>
    {
        #region Public members
        public StandardCaseHandler_OLD(ILogger<StandardCaseHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IRubricaComuneService rubricaComuneService,
            IHttpContextAccessor httpContextAccessor,
            IDocumentoAmministrativoRepository documentRepository,
            IDocumentBlobRepository documentBlobRepository,
            IUOCorrispondenteRepository uOCorrispondenteRepository,
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

            this.InitializeMapper();
        }
        public async Task<StandardCaseResult> Handle(StandardCaseRequest request, CancellationToken cancellationToken)
        {
            var output = new ProcessorOutput();

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

            //if (idTenant == 0)
            //    idTenant = request.ruolo.idAmministrazione.AsLong();

            //if (idGroup == 0)
            //    idGroup = request.ruolo.idGruppo.AsLong();

            var ruolo = await this._dbContext.CorrGlobaliEntities.FirstAsync(x => x.ID_GRUPPO == idGroup);
            var idRole = ruolo?.SYSTEM_ID;

            var docnumber = string.Empty;

            try
            {
                string messaggioRC = string.Empty;
                var registroEntity = await this._dbContext.RegistroEntities.AsNoTracking().FirstAsync(x => x.SYSTEM_ID == request.reg.systemId.AsLong());

                // Controllo su formati documento principale e allegati
                foreach (var a in request.email.Attachments)
                {
                    // Sospensione mail
                    if (!await this.CheckFileFormat(a.FileName))
                    {
                        throw new InvalidFileFormatException(a.FileName);
                    }
                }

                var registriRuolo = (await this._mediator.Send(new Requests.UtenteGetRegistriWithRf(
                    idRole.ToString(),
                    string.Empty,
                    string.Empty,
                    false
                    ))).output.ToList();

                // Ricerca mittente
                DocsPaVO.utente.Corrispondente? mittente = default;

                var documentTypeEntity = await this._dbContext.DocumentTypesEntities.FirstAsync(x => x.TYPE_ID == "MAIL");

                foreach (var r in registriRuolo)
                {
                    (mittente, messaggioRC) = await this.GetCorrispondenteByEmail(request.email.Sender.Address, r.systemId, idTenant);

                    if (mittente is not null && !mittente.codiceRubrica.Contains("@"))
                        break;
                }

                string[] splitted = messaggioRC.Split('#');
                if (splitted.Length > 1)
                {
                    messaggioRC = splitted[0];

                }

                if (mittente is null && !messaggioRC.Equals("RC"))
                {
                    var aoo = await this._dbContext.RegistroEntities.FirstAsync(x => x.SYSTEM_ID == registroEntity.ID_AOO_COLLEGATA);

                    var mittenteAggregate = new UOCorrispondente(
                        idTenant.ToString(),
                        DateTime.Now,
                        new Core.SeedWork.TextValue(request.email.Sender.Address),
                        new Core.SeedWork.TextValue(request.email.Sender.Address)
                        );
                    mittenteAggregate.AddEmail(null, request.email.Sender.Address, new Core.SeedWork.TextValue(string.Empty));
                    mittenteAggregate.SetCanalePreferenzialeMail(documentTypeEntity.TYPE_ID);
                    //mittenteAggregate.SetCodiceAOO(aoo.VAR_CODICE);

                    await this._uoCorrispondenteRepository.Add(mittenteAggregate);
                }

                // Creazione documento
                var documentAggregate = new DocumentoAmministrativo(idTenant.ToString(),
                    DateTime.Now,
                    new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.OggettoDelDocumento
                    {
                        Descrizione = request.email.Subject
                    },
                    new DatiRegistro()
                    {
                        IdRegistro = registroEntity.ID_AOO_COLLEGATA.ToString()
                    },
                    Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.TipologiaFlussoEnum.E
                    );

                documentAggregate.AssignMezzoSpedizione(documentTypeEntity.SYSTEM_ID.ToString(), new Core.SeedWork.TextValue(documentTypeEntity.DESCRIPTION!));
                documentAggregate.AssignMittente(new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Mittente
                {
                    Id = mittente.systemId
                });
                documentAggregate.RichiediRegistrazione(new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.DatiRichiestaRegistrazione
                {
                    Predisponi = true
                });

                // Eccezione segnatura

                // Mittente intermedio
                if (request.email.Headers.Any(x => x.Name == "utenteDocspa"))
                {
                    var mittenteIntermedioMail = request.email.Headers.First(x => x.Name == "utenteDocspa").Value;

                    var mittenteIntermedio = await this._dbContext.PeopleEntities.AsNoTracking()
                        .Where(x => x.EMAIL_ADDRESS.ToUpper() == mittenteIntermedioMail.ToUpper() && x.ID_AMM == idTenant)
                        .FirstOrDefaultAsync();

                    if (mittenteIntermedio is not null)
                    {
                        documentAggregate.AssignMittenteIntermedio(new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Mittente
                        {
                            Id = mittenteIntermedio.SYSTEM_ID.ToString()
                        });
                    }
                }

                // Invio conferma - è gestita da aggregate?
                // SchedaDocumento.protocollo.invioConferma

                await this._documentRepository.Add(documentAggregate);

                docnumber = documentAggregate.Id;

                // Caricamento doc principale
                var documentBlobAggregate = new DocumentBlob(
                    idTenant.ToString(),
                    DateTime.Now,
                    new Core.SeedWork.TextValue(Resources.DefaultEmailFileName)
                    );


                using (var stream = new MemoryStream())
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write(request.email.Body);
                        sw.Flush();
                        stream.Position = 0;
                        documentBlobAggregate.UploadStream(stream, Resources.DefaultEmailFileName);
                        documentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                        var hash = documentBlobAggregate.Hash;
                        await this._documentBlobRepository.Add(documentBlobAggregate);

                        documentAggregate.AssignDocumentBlobRef(new DocumentBlobRef
                        {
                            FileName = documentBlobAggregate.FileName,
                            FileSize = documentBlobAggregate.FileSize,
                            CreationDate = await _dbContext.GetSystemDateTime(),
                            ContentType = documentBlobAggregate.ContentType,
                            Hash = hash,
                            HashName = (HashNamesEnum?)Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256,
                            IdBlob = documentBlobAggregate.Id
                        }, new TargetVersionBehavior
                        {
                            CreateNewVersion = false,
                            IdVersion = documentAggregate.Versions.First().Id
                        });
                    }
                }

                await this._documentRepository.Update(documentAggregate);

                // Caricamento allegati
                int processedAttachments = 0;
                int i = 1;
                foreach (var a in request.email.Attachments)
                {
                    var description = string.Format(Resources.AttachmentDescription, i++);

                    await this.AddAttachment(idTenant.ToString(), docnumber, a.FileName, description, a.Content);
                }

                // Caricamento email ricevuta
                if (request.email.BinaryContent is not null)
                {
                    await this.AddAttachment(
                        idTenant.ToString(),
                        docnumber,
                        $"{Guid.NewGuid().ToString().Substring(0, 25)}{DateTime.Now.ToString("ss.ff")}.eml",
                        Resources.ReceivedMessageDescription,
                        request.email.BinaryContent
                        );
                    processedAttachments++;
                }

                // Mezzo spedizione

                output.Success = true;
                output.ProcessedAttachments = processedAttachments;

                //Trasmissione per interoperabilità

                // Per gestione pendenti tramite PEC
                bool mailPendente = await this.MantieniMailRicevutePendenti(registroEntity.SYSTEM_ID, registroEntity.VAR_EMAIL_REGISTRO);

                DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                trasm.ruolo = this._mapper.Map<Ruolo>(ruolo);
                trasm.utente = (await this._mediator.Send(new Application.Requests.getUtenteById(idPeople.ToString()))).output;
                trasm.utente.dst = ""; // ?? poi vedo
                trasm.noteGenerali = "";
                DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
                infoDoc.idProfile = documentAggregate.Id;
                infoDoc.docNumber = documentAggregate.Id;
                infoDoc.oggetto = documentAggregate.OggettoDelDocumento.Descrizione.ToString();
                infoDoc.tipoProto = "A";
                infoDoc.idRegistro = registroEntity.SYSTEM_ID.ToString();
                trasm.infoDocumento = infoDoc;
                //costruzione singole trasmissioni
                var ragioneEntity = await this._dbContext.RagioneTrasmissioneEntities
                    .Where(x => x.CHA_TIPO_RAGIONE.Equals("I") && x.CHA_TIPO_DIRITTI.Equals("W") && x.ID_AMM == idTenant)
                    .FirstOrDefaultAsync();
                DocsPaVO.trasmissione.RagioneTrasmissione ragione = this._mapper.Map<RagioneTrasmissione>(ragioneEntity);

                // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento ricevuto per interop interno solo a ruoli nella UO
                // destinataria della spedizione e non a tutta la AOO.
                List<Ruolo> ruoliDest = await this.GetRuoliDestTrasm(registroEntity);
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

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                output = new ProcessorOutput
                {
                    Success = false,
                    DocNumber = null,
                    ErrorMessage = ex.Message,
                    ProcessedAttachments = 0
                };

                // Se il predisposto è stato creato lo devo cestinare
                if (!string.IsNullOrWhiteSpace(docnumber)) await this.TrashDocument(docnumber.AsLong());
            }

            return new StandardCaseResult(output);
        }


        #endregion

        #region Private members
        protected readonly ILogger<StandardCaseHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IRubricaComuneService _rubricaComuneService;
        protected readonly IConfigurationService _configurationService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IDocumentoAmministrativoRepository _documentRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IUOCorrispondenteRepository _uoCorrispondenteRepository;

        private readonly string BearerPrefix = "Bearer ";

        protected IMapper _mapper = null;
        private async Task<List<Ruolo>> GetRuoliDestTrasm(RegistroEntity registroEntity)
        {
            List<Ruolo> ruoliDestTrasm = new List<Ruolo>();
            var interopNoMail = await this._configurationService.GetValue<string>("INTEROP_INT_NO_MAIL");

            var j1 = this._dbContext.CorrGlobaliEntities.Join(this._dbContext.TipoRuoloEntities, a => a.ID_TIPO_RUOLO, d => d.SYSTEM_ID, (a, d) => new { a, d });
            var j2 = j1.Join(this._dbContext.CorrGlobaliEntities, j1 => j1.a.ID_UO, e => e.SYSTEM_ID, (j1, e) => new { a = j1.a, d = j1.d, e });
            var j3 = j2.Join(this._dbContext.RuoloRegistroEntities, j2 => j2.a.SYSTEM_ID, f => f.ID_RUOLO_IN_UO, (j2, f) => new { a = j2.a, d = j2.d, e = j2.e, f });
            var j4 = j3.SelectMany(t1 => this._dbContext.VisMailRegistriEntities.Where(t2 => t1.f.ID_RUOLO_IN_UO == t2.ID_RUOLO_IN_UO && t1.f.ID_REGISTRO == t2.ID_REGISTRO).DefaultIfEmpty(),
                (j3, v) => new JoinEntity { a = j3.a, d = j3.d, e = j3.e, f = j3.f, v = v });

            var predicate = PredicateBuilder.New<JoinEntity>();

            predicate = predicate.And(x => x.a.CHA_TIPO_URP.Equals("R"));
            predicate = predicate.And(x => x.f.ID_REGISTRO == registroEntity.SYSTEM_ID);
            predicate = predicate.And(x => (x.a.CHA_DISABLED_TRASM != "1" || x.a.CHA_DISABLED_TRASM == null));
            predicate = predicate.And(x => x.v.CHA_NOTIFICA.Equals("1"));
            if (string.IsNullOrEmpty(registroEntity.VAR_EMAIL_REGISTRO) && interopNoMail != null && interopNoMail != "0")
                predicate = predicate.And(x => string.IsNullOrEmpty(x.v.VAR_EMAIL_REGISTRO));
            else if (!string.IsNullOrEmpty(registroEntity.VAR_EMAIL_REGISTRO))
                predicate = predicate.And(x => x.v.VAR_EMAIL_REGISTRO.Equals(registroEntity.VAR_EMAIL_REGISTRO) || string.IsNullOrEmpty(x.v.VAR_EMAIL_REGISTRO));
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
                    uo = new UnitaOrganizzativa() { systemId = item.ID_UO.ToString() },
                    tipoCorrispondente = "R"
                });
            }

            return ruoliDestTrasm;
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

            return entities.Any(x => !x.VAR_SOLO_MAIL_PEC.Equals("1") && x.VAR_MAIL_RIC_PENDENTE.Equals("1"));
        }
        protected async Task<bool> CheckFileFormat(string filename)
        {
            var ext = Path.GetExtension(filename);

            if (ext.StartsWith(".")) ext = ext.Substring(1);

            var apps = await this._dbContext.AppEntities.AsNoTracking()
                .Where(x => x.DEFAULT_EXTENSION.ToUpper() == ext.ToUpper())
                .ToListAsync();

            return apps.Any();
        }

        private async Task<(DocsPaVO.utente.Corrispondente?, string)> GetCorrispondenteByEmail(string email, string idRegistro, long idTenant)
        {
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

            var corrGlobaliEntity = await this._dbContext.CorrGlobaliEntities.FirstOrDefaultAsync(predicate);

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
                    new InfoUtente())))?.output;
            }

            // Rubrica comune

            bool confAb = (await this._mediator.Send(new Application.Requests.GetConfigurazioniRubricaComune(new InfoUtente()))).output.GestioneAbilitata;
            if (confAb)
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
                            var corrispondentiDeleteModifyCorrispondenteEsternoRes = await this._mediator.Send(new Requests.CorrispondentiDeleteModifyCorrispondenteEsterno(new InfoUtente(), new DatiModificaCorr { idCorrGlobali = corr.systemId }, 0, "D"));
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
                                var corrispondentiDeleteModifyCorrispondenteEsternoRes = await this._mediator.Send(new Requests.CorrispondentiDeleteModifyCorrispondenteEsterno(new InfoUtente(), new DatiModificaCorr { idCorrGlobali = corr.systemId }, 0, "D"));
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
            Services.RubricaComune.Corrispondente elemento = new();
            var criteriRicerca = new List<CriterioRicerca>
            {
                new CriterioRicerca { Campo = CampiRicercaEnum.Codice, Valore = codice.Replace("'", "''") }
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

        private async Task<(DocsPaVO.utente.Corrispondente, bool)> Update(DocsPaVO.utente.Corrispondente corrispondente, Services.RubricaComune.Corrispondente elemento)
        {
            bool isCorrModified = false;
            bool requestNew = (corrispondente == null);
            bool idDirty = false;
            string codFisc = "";
            string pIva = "";
            bool variazioneEmail = false;

            var elementoEmails = await this._rubricaComuneService.GetEmails(this.GetAuthToken(), Convert.ToInt32(elemento.Id));
            var elementoEmail = elementoEmails.FirstOrDefault(e => e.Preferita == true);
            var elUrls = new List<string?>();
            if (elemento.UrlApiInteroperabilita != null)
            {
                elUrls.Add(elemento.UrlApiInteroperabilita);
            }

            if (!requestNew)
            {

                if (elementoEmail == null)
                    elementoEmail = new()
                    {
                        Indirizzo = string.Empty
                    };

                if (corrispondente.email == null)
                    corrispondente.email = string.Empty;


                if (corrispondente.dettagli)
                {
                    if (corrispondente.info == null)
                    {
                        corrispondente.info = await this.GetDettagliCorr(corrispondente.systemId);
                        codFisc = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corrispondente.info.Tables[0].Rows[0])).codiceFiscale;
                        pIva = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corrispondente.info.Tables[0].Rows[0])).partitaIva;
                    }
                }


                idDirty = (corrispondente.descrizione != elemento.Denominazione ||
                            corrispondente.codiceAmm != elemento.Amministrazione ||
                            (!(string.IsNullOrEmpty(corrispondente.codiceAOO) && string.IsNullOrEmpty(elemento.AOO)) && corrispondente.codiceAOO != elemento.AOO) ||
                            corrispondente.email != elementoEmail.Indirizzo ||
                            codFisc.ToUpper() != elemento.CodiceFiscale.ToUpper() ||
                            pIva.ToUpper() != elemento.PartitaIva.ToUpper());



                idDirty = corrispondente.Url.Count != elUrls.Count;
                if (!idDirty && corrispondente.Url.Count > 0 && elUrls.Count > 0)
                    idDirty = corrispondente.Url[0].Url != elemento.UrlApiInteroperabilita;

                if (!idDirty)
                {

                    if (elementoEmails != null && elementoEmails.Count > 0) //verifica se da RC torna che il corr ha almeno una mail, altrimenti tutto il controllo sotto non ha senso
                    {
                        if (elementoEmails.Count > 1)
                            idDirty = elementoEmails.Count != corrispondente.Emails.Count;
                        else
                            idDirty = elementoEmail.Indirizzo.ToLower() != corrispondente.email.ToLower();

                        if (elementoEmails.Count != corrispondente.Emails.Count)
                            variazioneEmail = true;
                    }

                    if (!idDirty)
                    {
                        foreach (var mail in elementoEmails)
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
                        idDirty = (oldRow.indirizzo != elemento.Indirizzo ||
                                        oldRow.citta != elemento.Citta ||
                                        oldRow.cap != elemento.CAP ||
                                        oldRow.provincia != elemento.Provincia ||
                                        oldRow.nazione != elemento.Nazione ||
                                        oldRow.telefono != elemento.Telefono ||
                                        oldRow.fax != elemento.Fax ||
                                        oldRow.codiceFiscale != elemento.CodiceFiscale ||
                                        oldRow.partitaIva != elemento.PartitaIva);
                    }
                }
            }
            if (requestNew)
            {
                DocsPaVO.utente.Corrispondente newCorr = await this.GetNuovoCorr(elemento, elementoEmail.Indirizzo, elUrls);
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
                corrispondente.email = elementoEmail.Indirizzo;
                corrispondente.Url = GetInternalUrlsCollection(elUrls);
                corrispondente.canalePref = await GetCanaleCorrispondente(corrispondente, idTenant);
                DocsPaVO.utente.DatiModificaCorr datiModifica = this.GetDatiPerModifica(corrispondente.systemId, corrispondente.canalePref.systemId, elemento, elementoEmail.Indirizzo, elUrls);

                string newIdCorrGlobali;
                string message;

                var res = (await this._mediator.Send(new Application.Requests.CorrispondentiDeleteModifyCorrispondenteEsternoWithId(new InfoUtente(), datiModifica, 0, "M")));

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
                    throw new Exception(Resources.ModifyCorrFailed);
                }

                if (variazioneEmail)
                {
                    List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();

                    foreach (var mail in elementoEmails)
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
                        throw new Exception(Resources.ErrorModifyMail);
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

        private async Task<Canale> GetCanaleCorrispondente(DocsPaVO.utente.Corrispondente corrispondente, string idAmministrazione)
        {
            bool canaleMail = false;
            bool canaleInterop = false;
            bool canaleInteropSemplificata = false;

            if (!string.IsNullOrEmpty(corrispondente.codiceAmm) && !string.IsNullOrEmpty(corrispondente.codiceAOO))
            {
                (string isEnabledSimplifiedInterop, bool keyFound) = await this._configurationService.TryGetValue<string>(idAmministrazione, "INTEROP_SERVICE_ACTIVE");

                if (string.IsNullOrEmpty(isEnabledSimplifiedInterop))
                {
                    (isEnabledSimplifiedInterop, keyFound) = await this._configurationService.TryGetValue<string>("INTEROP_SERVICE_ACTIVE");
                }
                canaleInteropSemplificata = isEnabledSimplifiedInterop != null ? isEnabledSimplifiedInterop.Equals("1") : false;

                canaleInterop = !string.IsNullOrEmpty(corrispondente.email) && !canaleInteropSemplificata;
            }
            else if (!string.IsNullOrEmpty(corrispondente.email))
            {

                canaleInterop = false;
                canaleInteropSemplificata = false;
                canaleMail = true;
            }

            foreach (var canale in await this.GetListaCanale())
            {
                if ((canale.typeId == Channel.Interop && canaleInterop && !canaleInteropSemplificata) ||
                    (canale.typeId == Channel.Mail && canaleMail) ||
                    (canale.typeId == Channel.Lettera && !canaleMail && !canaleInterop && !canaleInteropSemplificata) ||
                    (canale.typeId == Channel.Simpinterop && canaleInteropSemplificata))
                {
                    return canale;
                }
            }
            return null;
        }

        private async Task<List<Canale>> GetListaCanale()
        {
            List<Canale> canali = new();
            var ds = this._dbContext.DocumentTypesEntities.AsNoTracking().OrderBy(a => a.DESCRIPTION).Select(a => new Canale()
            {
                systemId = a.SYSTEM_ID.ToString(),
                typeId = a.TYPE_ID,
                descrizione = a.DESCRIPTION,
                tipoCanale = a.CHA_TIPO_CANALE
            });
            return canali;
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

        private async Task<Services.RubricaComune.Corrispondente> GetElementiInRubricaCom(string email, string rubEsterna)
        {
            var elementiRubrica = new List<Services.RubricaComune.Corrispondente>();
            Services.RubricaComune.Corrispondente elemento = null;
            var criteriRicerca = new List<CriterioRicerca>
            {
                new CriterioRicerca { Campo = CampiRicercaEnum.Email, Valore = email }
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

        protected string? GetAuthToken()
        {
            var authorizationHeader = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>("KeyToken");

            //if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            //var authorizationHeader = authorizationStrings[0]!.StartsWith(BearerPrefix) ? authorizationStrings[0]!.Substring(BearerPrefix.Length) : authorizationStrings[0];

            return authorizationHeader;
        }

        private async Task AddAttachment(string idTenant, string docnumber, string fileName, string description, byte[] content)
        {
            var attachmentAggregate = new DocumentoAmministrativo(
                    idTenant,
                    DateTime.Now,
                    new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.OggettoDelDocumento
                    {
                        Descrizione = new Core.SeedWork.TextValue(fileName ?? description)
                    },
                    null,
                    null,
                    null,
                    new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.IdDoc
                    {
                        Identiticativo = docnumber
                    });

            await this._documentRepository.Add(attachmentAggregate);

            var idAttachment = attachmentAggregate.Id;

            var attachmentBlobAggregate = new DocumentBlob(
                idTenant.ToString(),
                DateTime.Now,
                new Core.SeedWork.TextValue(fileName)
                );

            using (var stream = new MemoryStream(content))
            {
                attachmentBlobAggregate.UploadStream(stream, fileName);
                attachmentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                var attachmentHash = attachmentBlobAggregate.Hash;
                await this._documentBlobRepository.Add(attachmentBlobAggregate);

                attachmentAggregate.AssignDocumentBlobRef(new DocumentBlobRef
                {
                    FileName = attachmentBlobAggregate.FileName,
                    FileSize = attachmentBlobAggregate.FileSize,
                    CreationDate = await _dbContext.GetSystemDateTime(),
                    ContentType = attachmentBlobAggregate.ContentType,
                    Hash = attachmentHash,
                    HashName = HashNamesEnum.SHA256,
                    IdBlob = attachmentBlobAggregate.Id
                }, new TargetVersionBehavior
                {
                    CreateNewVersion = false,
                    IdVersion = attachmentAggregate.Versions.First().Id
                });


                await this._documentRepository.Update(attachmentAggregate);
            }

            //Andrea De Marco - Inserimento dell'allegato segnatura come allegato Utente (il cliente avrebbe preferito PEC ma si è scelto per il momento di inserirlo come allegato PEC)
            //Andrea De Marco - Osserva che i metodi AggiungiAllegato e AggiungiAllegatoPEC fanno la stessa cosa.
        }

        private async Task TrashDocument(long docnumber)
        {
            var profileEntity = await this._dbContext.ProfileEntities.FirstAsync(x => x.DOCNUMBER == docnumber);

            profileEntity.CHA_IN_CESTINO = "1";

            this._dbContext.ProfileEntities.Update(profileEntity);

            var attachmentList = await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(x => x.ID_DOCUMENTO_PRINCIPALE == docnumber)
                .ToListAsync();

            attachmentList.ForEach(a =>
            {
                a.CHA_IN_CESTINO = "1";
                this._dbContext.ProfileEntities.Update(a);
            });

            await ((DbContext)this._dbContext).SaveChangesAsync();
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
            });

            this._mapper = configuration.CreateMapper();
        }

        #endregion
    }

    public class JoinEntity
    {
        public CorrGlobaliEntity a { get; set; }
        public TipoRuoloEntity d { get; set; }
        public CorrGlobaliEntity e { get; set; }
        public RuoloRegistroEntity f { get; set; }
        public VisMailRegistriEntity v { get; set; }

    }
}
