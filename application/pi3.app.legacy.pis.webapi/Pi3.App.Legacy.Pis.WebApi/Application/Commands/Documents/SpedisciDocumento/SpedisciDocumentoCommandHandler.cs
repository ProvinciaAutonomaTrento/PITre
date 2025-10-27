// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.addressbook;
using DocsPaVO.amministrazione;
using DocsPaVO.documento;
using DocsPaVO.Interoperabilita.Semplificata;
using DocsPaVO.LibroFirma;
using DocsPaVO.Spedizione;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetElementoInRubricaComuneNoEsterna;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSpedizioneDocumento;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SpedizioneInteropPiTre;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils.GetOrSetAacToken;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Interoperability;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Interoperability.Domain;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.SigilloElettronico;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Tibco.Services.File.SigilloElettronico;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Text;
using static Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Interoperability.InteroperabilityMessage;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SpedisciDocumento
{
    public class SpedisciDocumentoCommandHandler : IRequestHandler<SpedisciDocumentoCommand, SpedisciDocumentoCommandResponse>
    {
        #region Public Members

        public SpedisciDocumentoCommandHandler(ILogger<SpedisciDocumentoCommandHandler> logger,
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
            IReportGeneratorService reportGeneratorService)
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
            this._reportGeneratorService = reportGeneratorService;

            this.InitializeMapper();
        }

        public async Task<SpedisciDocumentoCommandResponse> Handle(SpedisciDocumentoCommand request, CancellationToken cancellationToken)
        {
            var infoSpedizione = request.InfoSpedizione;
            var schedaDocumento = request.Documento;
            var erroreSpedizione = string.Empty;

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var idGruppo = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            try
            {
                AssertProtocolloUscita(schedaDocumento);
                AssertLibroFirma(schedaDocumento.docNumber);

                infoSpedizione.listaDestinatariNonRaggiungibili = new List<string>();

                //Aggiorno le versioni del documento principale da DB perchè in alcuni casi quello inviato dal FE non è aggiornato(caso di firma documento e poi spedisci INC000001057841)
                schedaDocumento.documenti = (await DBUtils.GetVersionsMainDocument(request.InfoUtente,
                       _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser),
                       _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup),
                       schedaDocumento.docNumber.AsLong(),
                       _dbContext)).ToArray();

                // Registro o RF mittente della spedizione per interoperabilità
                DocsPaVO.utente.Registro registroRfMittente = null;
                if (!string.IsNullOrEmpty(infoSpedizione.IdRegistroRfMittente))
                    registroRfMittente = await GetRegistroBySistemId(infoSpedizione.IdRegistroRfMittente.AsLong());
                if (registroRfMittente != null)
                    registroRfMittente.email = infoSpedizione.mailAddress;

                //Creo il file di segnatura solo se presente nella spedizione almen in destinatario interoperante
                bool existsIterop = infoSpedizione.DestinatariEsterni
                    .Where(d => d.IncludiInSpedizione && d.Interoperante && d.DatiDestinatari
                        .Where(d1 => d1.canalePref != null && d1.canalePref.descrizione.Equals("INTEROPERABILITA")).Count() > 0)
                    .Count() > 0;

                if (existsIterop)
                {
                    await AddVersioneAllegatoSegnaturaXML(schedaDocumento.systemId);
                }

                //Inserisco nello DPA_SEND_STO
                List<SendStoEntity> sendStoEntitiesToInsert = new List<SendStoEntity>();
                var dataSpedizione = await _dbContext.GetSystemDateTime();
                foreach (DestinatarioEsterno corr in infoSpedizione.DestinatariEsterni.Where(c => c.IncludiInSpedizione))
                {
                    var idDocumentTypes = await _dbContext.DocArrivoParEntities.AsNoTracking()
                        .Where(d => d.ID_MITT_DEST == corr.Id.AsLong() && d.ID_PROFILE == schedaDocumento.systemId.AsLong())
                        .Select(d => d.ID_DOCUMENTTYPES)
                        .FirstAsync();

                    var isCanaleMail = !(string.IsNullOrEmpty(corr.Email) || (corr.DatiDestinatari[0].canalePref != null && corr.DatiDestinatari[0].canalePref.typeId == "SIMPLIFIEDINTEROPERABILITY"));
                    sendStoEntitiesToInsert.Add(new SendStoEntity()
                    {
                        ID_CORR_GLOBALE = corr.Id.AsLong(),
                        ID_PROFILE = schedaDocumento.systemId.AsLong(),
                        MAIL = isCanaleMail ? corr.Email : "N.A.",
                        DTA_SPEDIZIONE = dataSpedizione,
                        ESITO = Resources.LogInFaseDiSpedizione,
                        ID_DOCUMENTTYPE = idDocumentTypes,
                        ID_GROUP_SENDER = idGruppo,
                        MAIL_MITTENTE = isCanaleMail ? (!string.IsNullOrEmpty(infoSpedizione.mailAddress) ? infoSpedizione.mailAddress : string.Empty) : "N.A.",
                        ID_REG_MAIL_MITTENTE = isCanaleMail && !string.IsNullOrEmpty(infoSpedizione.IdRegistroRfMittente) ? infoSpedizione.IdRegistroRfMittente.AsLong() : null
                    });
                }
                await _dbContext.SendStoEntities.AddRangeAsync(sendStoEntitiesToInsert);
                await ((DbContext)_dbContext).SaveChangesAsync();

                //Spedizione per Interoperabilità PiTre
                var isEnabledSimplifiedInteroperability = await this._configurationService.GetValue<string>(idTenant, "INTEROP_SERVICE_ACTIVE");
                if (string.IsNullOrEmpty(isEnabledSimplifiedInteroperability))
                    isEnabledSimplifiedInteroperability = await this._configurationService.GetValue<string>("INTEROP_SERVICE_ACTIVE");
                List<DestinatarioEsterno> destIS = new List<DestinatarioEsterno>();
                if (isEnabledSimplifiedInteroperability == "1")
                {
                    destIS = infoSpedizione.DestinatariEsterni.Where(d => d.IncludiInSpedizione &&
                                    d.Interoperante &&
                                    d.DatiDestinatari.Where(d1 => d1.canalePref != null &&
                                        d1.canalePref.typeId.Equals("SIMPLIFIEDINTEROPERABILITY") &&
                                        d1.Url != null && d1.Url.Count > 0 &&
                                        !String.IsNullOrEmpty(d1.Url[0].Url) &&
                                        Uri.IsWellFormedUriString(d1.Url[0].Url, UriKind.Absolute)).Count() > 0).ToList();
                    if (destIS != null && destIS.Count > 0)
                    {
                        await SpedizionePerInteroperabilitaSemplificata(schedaDocumento, destIS, request.InfoUtente);
                    }
                }

                //Spedizione ai destinatari interoperanti esterni(PEC - IS)
                var destinatariInteroperanti = infoSpedizione.DestinatariEsterni.Where(e => e.IncludiInSpedizione && !destIS.Contains(e)).ToList();
                if (destinatariInteroperanti != null && destinatariInteroperanti.Count() > 0)
                {
                    await SpedizionePerInteroperabilita(schedaDocumento, registroRfMittente, destinatariInteroperanti);
                }
                //Spedizione ai destinatari interni(Intra AOO)
                foreach (DestinatarioInterno destinatario in infoSpedizione.DestinatariInterni.Where(e => e.IncludiInSpedizione && !e.DisabledTrasm))
                {
                    await SpedizionePerTrasmissione(schedaDocumento, destinatario, infoSpedizione);
                }

                //Impostazione dello stato di spedizione del documento
                infoSpedizione.Spedito = SetStatoSpedito(infoSpedizione);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                infoSpedizione.Spedito = false;
                erroreSpedizione = Resources.LogSpedizioneDocError;
            }

            foreach (DestinatarioEsterno corr in infoSpedizione.DestinatariEsterni.Where(c => c.IncludiInSpedizione))
            {
                await UpdateStoricoSpedizione(schedaDocumento.systemId, corr.Id,
                    idGruppo.ToString(), corr.StatoSpedizione.Descrizione, false);
            }

            return new SpedisciDocumentoCommandResponse()
            {
                SpedizioneDocumento = infoSpedizione
            };
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SpedisciDocumentoCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IConfigurationService _configurationService;
        protected readonly ISigilloElettronicoService _sigilloElettronicoService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IEmailSenderService _emailSenderService;
        protected readonly IInteroperabilityService _interoperabilityService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IReportGeneratorService _reportGeneratorService;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MailRegistriEntity, CasellaRegistro>()
                    .ForMember(dest => dest.IdRegistro, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.EmailRegistro, src => src.MapFrom(opt => opt.VAR_EMAIL_REGISTRO))
                    .ForMember(dest => dest.UserMail, src => src.MapFrom(opt => opt.VAR_USER_MAIL))
                    .ForMember(dest => dest.PwdMail, src => src.MapFrom(opt => Crypter.Decode(opt.VAR_PWD_MAIL, opt.VAR_USER_MAIL)))
                    .ForMember(dest => dest.ServerSMTP, src => src.MapFrom(opt => opt.VAR_SERVER_SMTP))
                    .ForMember(dest => dest.SmtpSSL, src => src.MapFrom(opt => opt.CHA_SMTP_SSL ?? string.Empty))
                    .ForMember(dest => dest.PopSSL, src => src.MapFrom(opt => opt.CHA_POP_SSL ?? string.Empty))
                    .ForMember(dest => dest.PortaSMTP, src => src.MapFrom(opt => opt.NUM_PORTA_SMTP ?? 0))
                    .ForMember(dest => dest.SmtpSta, src => src.MapFrom(opt => opt.CHA_SMTP_STA ?? string.Empty))
                    .ForMember(dest => dest.ServerPOP, src => src.MapFrom(opt => opt.VAR_SERVER_POP))
                    .ForMember(dest => dest.PortaPOP, src => src.MapFrom(opt => opt.NUM_PORTA_POP ?? 0))
                    .ForMember(dest => dest.UserSMTP, src => src.MapFrom(opt => opt.VAR_USER_SMTP))
                    .ForMember(dest => dest.PwdSMTP, src => src.MapFrom(opt => Crypter.Decode(opt.VAR_PWD_SMTP, opt.VAR_USER_SMTP)))
                    .ForMember(dest => dest.IboxIMAP, src => src.MapFrom(opt => opt.VAR_INBOX_IMAP))
                    .ForMember(dest => dest.ServerIMAP, src => src.MapFrom(opt => opt.VAR_SERVER_IMAP))
                    .ForMember(dest => dest.PortaIMAP, src => src.MapFrom(opt => opt.NUM_PORTA_IMAP ?? 0))
                    .ForMember(dest => dest.TipoConnessione, src => src.MapFrom(opt => opt.VAR_TIPO_CONNESSIONE))
                    .ForMember(dest => dest.BoxMailElaborate, src => src.MapFrom(opt => opt.VAR_BOX_MAIL_ELABORATE))
                    .ForMember(dest => dest.MailNonElaborate, src => src.MapFrom(opt => opt.VAR_MAIL_NON_ELABORATE))
                    .ForMember(dest => dest.ImapSSL, src => src.MapFrom(opt => opt.CHA_IMAP_SSL))
                    .ForMember(dest => dest.SoloMailPEC, src => src.MapFrom(opt => opt.VAR_SOLO_MAIL_PEC))
                    .ForMember(dest => dest.RicevutaPEC, src => src.MapFrom(opt => opt.CHA_RICEVUTA_PEC ?? string.Empty))
                    .ForMember(dest => dest.Principale, src => src.MapFrom(opt => opt.VAR_PRINCIPALE ?? "0"))
                    .ForMember(dest => dest.Note, src => src.MapFrom(opt => opt.VAR_NOTE))
                    .ForMember(dest => dest.MailRicevutePendenti, src => src.MapFrom(opt => opt.VAR_MAIL_RIC_PENDENTE))
                    .ForMember(dest => dest.MessageSendMail, src => src.MapFrom(opt => opt.VAR_MESSAGE_SEND_MAIL))
                    .ForMember(dest => dest.OverwriteMessageAmm, src => src.MapFrom(opt => opt.CHA_OVERWRITE_MESSAGE_AMM == "1"));

                cfg.CreateMap<RegistroEntity, DocsPaVO.utente.Registro>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.codRegistro, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.NUM_RIF))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_REGISTRO))
                    .ForMember(dest => dest.ultimoNumeroProtocollo, src => src.MapFrom(opt => opt.NUM_RIF))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.VAR_EMAIL_REGISTRO))
                    .ForMember(dest => dest.stato, src => src.MapFrom(opt => opt.CHA_STATO))
                    .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.dataApertura, src => src.MapFrom(opt => opt.DTA_OPEN.AsDateFormat()))
                    .ForMember(dest => dest.dataChiusura, src => src.MapFrom(opt => opt.DTA_CLOSE.AsDateFormat()))
                    .ForMember(dest => dest.dataUltimoProtocollo, src => src.MapFrom(opt => opt.DTA_ULTIMO_PROTO.AsDateFormat()))
                    .ForMember(dest => dest.idRuoloAOO, src => src.MapFrom(opt => opt.ID_RUOLO_AOO))
                    .ForMember(dest => dest.idRuoloResp, src => src.MapFrom(opt => opt.ID_RUOLO_RESP))
                    .ForMember(dest => dest.idUtenteAOO, src => src.MapFrom(opt => opt.ID_PEOPLE_AOO))
                    .ForMember(dest => dest.autoInterop, src => src.MapFrom(opt => opt.CHA_AUTO_INTEROP))
                    .ForMember(dest => dest.chaRF, src => src.MapFrom(opt => opt.CHA_RF))
                    .ForMember(dest => dest.rfDisabled, src => src.MapFrom(opt => opt.CHA_DISABILITATO))
                    .ForMember(dest => dest.idAOOCollegata, src => src.MapFrom(opt => opt.ID_AOO_COLLEGATA))
                    .ForMember(dest => dest.invioRicevutaManuale, src => src.MapFrom(opt => opt.INVIO_RICEVUTA_MANUALE))
                    .ForMember(dest => dest.FlagWspia, src => src.MapFrom(opt => opt.FLAG_WSPIA == null ? "0" : opt.FLAG_WSPIA))
                    .ForMember(dest => dest.flag_pregresso, src => src.MapFrom(opt => opt.VAR_PREG == "1"))
                    .ForMember(dest => dest.anno_pregresso, src => src.MapFrom(opt => opt.ANNO_PREG))
                    .ForMember(dest => dest.Diritto_Ruolo_AOO, src => src.MapFrom(opt => opt.DIRITTO_RUOLO_AOO))
                    .ForMember(dest => dest.codiceIpa, src => src.MapFrom(opt => opt.VAR_CODICE_IPA));
            });

            _mapper = configuration.CreateMapper();
        }

        protected void AssertProtocolloUscita(SchedaDocumento schedaDocumento)
        {
            if (string.IsNullOrEmpty(schedaDocumento.systemId))
                throw new DocumentoNonSalvatoPi3Exception();

            if (((ProtocolloUscita)schedaDocumento.protocollo) == null || string.IsNullOrEmpty(((ProtocolloUscita)schedaDocumento.protocollo).numero))
                throw new DocumentoNonProtocollatoInUscitaPi3Exception(schedaDocumento.systemId);
        }

        protected void AssertLibroFirma(string docnumber)
        {
            //Verifico se il documento è in libro firma e in caso se il passo in attesa è quello di protocollazione ed il titolare è l'utente che sta effettuando la protocollazione
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

            var docnumberAsLong = docnumber.AsLong();
            var istanzaPassoFirmaAttesaEntity = this._dbContext.IstanzaProcessoFirmaEntities
                .Join(this._dbContext.IstanzaPassoFirmaEntities, p => p.ID_ISTANZA, a => a.ID_ISTANZA_PROCESSO, (p, i) => new { p, i })
                .Where(j => j.p.ID_DOCUMENTO == docnumberAsLong && j.p.STATO == "IN_EXEC" && j.i.STATO_PASSO == "LOOK")
                .Select(j => new
                {
                    j.i.ID_RUOLO_COINVOLTO,
                    j.i.ID_UTENTE_COINVOLTO,
                    j.i.ID_UTENTE_LOCKER,
                    j.i.TIPO_FIRMA
                })
                .FirstOrDefault();

            if (istanzaPassoFirmaAttesaEntity != null)
            {
                if (istanzaPassoFirmaAttesaEntity.ID_RUOLO_COINVOLTO != idGroup
                    || (istanzaPassoFirmaAttesaEntity.ID_UTENTE_COINVOLTO != null && istanzaPassoFirmaAttesaEntity.ID_UTENTE_COINVOLTO != idUser)
                    || (istanzaPassoFirmaAttesaEntity.ID_UTENTE_LOCKER != null && istanzaPassoFirmaAttesaEntity.ID_UTENTE_LOCKER != idUser)
                    || !istanzaPassoFirmaAttesaEntity.TIPO_FIRMA.Equals(Azione.DOCUMENTOSPEDISCI.ToString()))
                {
                    throw new DocumentoInLibroFirmaPassoNonAttesoPi3Exception(docnumber);
                }
            }
        }

        protected async Task<bool> AddVersioneAllegatoSegnaturaXML(string docnumber)
        {
            var wasErrors = false;
            var result = true;

            var idDocPrincipale = docnumber.AsLong();
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode);

            var aggregateDocPrincipale = await this._documentoAmministrativoRepository.Get(idTenant, docnumber, new ILoadBehavior[1]
            {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadClassifications = true,
                        LoadAllegati = true,
                        LoadAggregazioni = true,
                        LoadVersions = true,
                        LoadPermissions = true,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = true,
                        LoadNote = true
                    }
            });

            var segnaturaXml = aggregateDocPrincipale.GetSegnaturaXml();
            byte[] contentSegnaturaXML = Encoding.UTF8.GetBytes(segnaturaXml);

            var idAllegantoSegnatura = await _dbContext.ProfileEntities
                .Join(_dbContext.VersionEntities,
                    p => p.DOCNUMBER,
                    v => v.DOCNUMBER,
                    (p, v) => new { p, v })
                .Where(j => j.p.ID_DOCUMENTO_PRINCIPALE == idDocPrincipale && j.v.CHA_ALLEGATI_ESTERNO == "S")
                .Select(j => j.p.SYSTEM_ID)
                .FirstOrDefaultAsync();

            if (idAllegantoSegnatura != null)
            {
                var aggregateAllegatoSegnatura = await this._documentoAmministrativoRepository.Get(idTenant, idAllegantoSegnatura.ToString(), new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = true,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = false,
                        LoadKeywords = false,
                        LoadNote = false
                    }
                });

                var isFirmato = false;
                var keyApplicaSigillo = await this._configurationService.GetValue<string>(idTenant, "BE_APPLICA_SIGILLO_SEGNATURA");
                if (keyApplicaSigillo == "1")
                {
                    var idRegistro = aggregateDocPrincipale.DatiRegistrazione.IdRegistro.AsLong();
                    var codiceUnivocoAOOIpa = await this._dbContext.RegistroEntities.Where(r => r.SYSTEM_ID == idRegistro).Select(r => r.VAR_CODICE_IPA).FirstAsync();

                    try
                    {
                        var response = await this._sigilloElettronicoService.SignXml(new SignXMLType()
                        {
                            FileDaFirmare = contentSegnaturaXML,
                            CodiceAOOIPA = codiceUnivocoAOOIpa,
                            CodiceEnteIPA = aggregateDocPrincipale.Amministrazione.PAI.Amministrazione.CodiceIPA
                        });

                        if (response.FileFirmato != null && response.FileFirmato.Count() > 0)
                        {
                            isFirmato = true;
                            contentSegnaturaXML = response.FileFirmato;
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(exception: ex, message: ex.Message, "Errore applicazione sigillo segnatura.xml");
                        throw new SigilloFileSegnaturaXMLNonApplicatoPi3Exception();
                    }
                }
                if (!wasErrors)
                {
                    var newDocumentBlobAggregate = new DocumentBlob(idTenant, DateTime.Now, new TextValue("segnatura.xml"));
                    newDocumentBlobAggregate.UploadStream(new MemoryStream(contentSegnaturaXML), "segnatura.xml");
                    newDocumentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                    await _documentBlobRepository.Add(newDocumentBlobAggregate);

                    aggregateAllegatoSegnatura.AssignDocumentBlobRef(
                        new DocumentBlobRef()
                        {
                            IdBlob = newDocumentBlobAggregate.Id,
                            CreationDate = await _dbContext.GetSystemDateTime(),
                            ContentType = newDocumentBlobAggregate.ContentType,
                            FileName = newDocumentBlobAggregate.FileName,
                            FileSize = newDocumentBlobAggregate.FileSize,
                            Hash = newDocumentBlobAggregate.Hash,
                            HashName = newDocumentBlobAggregate.HashName == Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256
                                ? HashNamesEnum.SHA256 : HashNamesEnum.SHA512,
                            Cartaceo = false,
                            SegnaturaPermanente = false,
                            TipoFirma = isFirmato ? TipoFirmaEnum.Xades : TipoFirmaEnum.Nessuna
                        },
                        new TargetVersionBehavior()
                        {
                            CreateNewVersion = true,
                            Name = null
                        });

                    await _documentoAmministrativoRepository.Update(aggregateAllegatoSegnatura);

                    result = true;
                };
            }

            return result;
        }

        /// <summary>
        /// Impostazione dello stato di spedizione del documento
        /// </summary>
        /// <param name="infoSpedizioni"></param>
        /// <remarks>
        /// Il documento sarà in stato spedito se è stato inviato almeno una volta ad un destinatario
        /// </remarks>
        protected bool SetStatoSpedito(DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizioni)
        {
            // Determina se il documento è stato spedito almeno una volta ad uno o più destinatari
            return (infoSpedizioni.DestinatariEsterni.Count(e => !string.IsNullOrEmpty(e.DataUltimaSpedizione)) +
                    infoSpedizioni.DestinatariInterni.Count(e => !string.IsNullOrEmpty(e.DataUltimaSpedizione)) > 0 +
                    infoSpedizioni.DestinatariInterni.Count(e => e.DisabledTrasm));
        }

        protected async Task UpdateStoricoSpedizione(string idProfile, string idCorrGlobali, string idRuolo, string esito, bool errore)
        {
            try
            {
                var dataSpedizione = await _dbContext.GetSystemDateTime();

                var sendStoEntities = _dbContext.SendStoEntities
                    .Where(s => s.ID_PROFILE == idProfile.AsLong() &&
                        s.ID_GROUP_SENDER == idRuolo.AsLong() &&
                        s.ESITO == "In fase di spedizione");

                if (!errore)
                    sendStoEntities = sendStoEntities.Where(s => s.ID_CORR_GLOBALE == idCorrGlobali.AsLong());

                sendStoEntities.ForEach(s =>
                {
                    s.ESITO = esito;
                    s.DTA_SPEDIZIONE = dataSpedizione;
                });

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: "Errore in UpdateStoricoSpedizione: " + ex.Message);
            }
        }

        protected async Task SpedizionePerTrasmissione(SchedaDocumento schedaDocumento, DestinatarioInterno destinatario, SpedizioneDocumento infoSpedizione)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idGruppo = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var listaDestinatariNonRaggiungibili = infoSpedizione.listaDestinatariNonRaggiungibili;

            var idCorrGlobaliDest = destinatario.DatiDestinatario.systemId.AsLong();
            var descCorrDest = destinatario.DatiDestinatario.descrizione;

            var idCorrGlobaliDestTrasmissione = string.Empty;

            try
            {
                if (destinatario.DatiDestinatario.tipoCorrispondente == "O")
                    throw new DestinatarioTrasmissioneOccasionalePi3Exception(idCorrGlobaliDest.ToString(), descCorrDest);

                if (destinatario.DatiDestinatario.tipoCorrispondente == "R" &&
                    await _dbContext.CorrGlobaliEntities.AnyAsync(c => c.SYSTEM_ID == destinatario.DatiDestinatario.systemId.AsLong() && c.CHA_DISABLED_TRASM == "1"))
                    throw new DestinatarioTrasmissioneDisabilitataPi3Exception(idCorrGlobaliDest.ToString(), descCorrDest);

                var tipoDestinatario = IsDestinatarioPrincipale(schedaDocumento, destinatario.DatiDestinatario.systemId) ? "TO" : "CC";
                RagioneTrasmissioneEntity? ragioneTrasmissioneEntity = null;
                if (tipoDestinatario == "TO")
                {
                    ragioneTrasmissioneEntity = await _dbContext.AmministraEntities
                        .Join(_dbContext.RagioneTrasmissioneEntities,
                            a => a.ID_RAGIONE_TO,
                            r => r.SYSTEM_ID,
                            (a, r) => new { a, r })
                        .Where(j => j.a.SYSTEM_ID == idTenant)
                        .Select(j => j.r)
                        .FirstOrDefaultAsync();
                }
                if (tipoDestinatario == "CC")
                {
                    ragioneTrasmissioneEntity = await _dbContext.AmministraEntities
                        .Join(_dbContext.RagioneTrasmissioneEntities,
                            a => a.ID_RAGIONE_CC,
                            r => r.SYSTEM_ID,
                            (a, r) => new { a, r })
                        .Where(j => j.a.SYSTEM_ID == idTenant)
                        .Select(j => j.r)
                        .FirstOrDefaultAsync();
                }

                var aggregate = new Trasmissione(idTenant.ToString(), DateTime.Now, schedaDocumento.systemId, TipiOggettiTrasmessiEnum.DocumentoAmministrativo);

                if (destinatario.DatiDestinatario.GetType() == typeof(UnitaOrganizzativa))
                {
                    var ruoloEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                      .Join(this._dbContext.TipoRuoloEntities, c => c.ID_TIPO_RUOLO, t => t.SYSTEM_ID, (c, t) => new
                      {
                          c.ID_GRUPPO,
                          t.NUM_LIVELLO,
                          c.ID_UO,
                          c.SYSTEM_ID
                      })
                       .Where(c => c.ID_GRUPPO == idGruppo)
                       .Select(c => new
                       {
                           c.ID_UO,
                           c.NUM_LIVELLO,
                           c.SYSTEM_ID
                       })
                       .FirstAsync();

                    var uoEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == ruoloEntity.ID_UO)
                        .Select(c => new
                        {
                            c.NUM_LIVELLO,
                            c.SYSTEM_ID,
                            c.ID_AMM
                        })
                        .FirstAsync();

                    Ruolo ruolo = new Ruolo()
                    {
                        systemId = ruoloEntity.SYSTEM_ID.ToString(),
                        uo = new UnitaOrganizzativa()
                        {
                            systemId = uoEntity.SYSTEM_ID.ToString(),
                            livello = uoEntity.NUM_LIVELLO.ToString(),
                            idAmministrazione = uoEntity.ID_AMM.ToString()
                        },
                        livello = ruoloEntity.NUM_LIVELLO.ToString()

                    };

                    QueryCorrispondenteAutorizzato qa = new QueryCorrispondenteAutorizzato()
                    {
                        ragione = new DocsPaVO.trasmissione.RagioneTrasmissione()
                        {
                            tipoDestinatario = DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa.Keys
                                .OfType<DocsPaVO.trasmissione.TipoGerarchia>()
                                .FirstOrDefault(s => DocsPaVO.trasmissione.RagioneTrasmissione.tipoGerarchiaStringa[s]
                                .Equals(ragioneTrasmissioneEntity.CHA_TIPO_DEST))
                        },
                        tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO,
                        idRegistro = schedaDocumento.registro.systemId,
                        queryCorrispondente = new QueryCorrispondente()
                        {
                            fineValidita = true
                        },
                        ruolo = ruolo
                    };

                    var ruoliRiferimento = await AddressbookGetRuoliRiferimentoAutorizzati(qa, (UnitaOrganizzativa)destinatario.DatiDestinatario);

                    if (ruoliRiferimento == null || ruoliRiferimento.Length == 0)
                        throw new RuoloRiferimentoNotFoundPi3Exception(idCorrGlobaliDest.ToString(), descCorrDest);

                    foreach (Ruolo ruoloRif in ruoliRiferimento)
                    {
                        idCorrGlobaliDestTrasmissione = ruoloRif.systemId;

                        var idGruppoRuoloRif = ruoloRif.idGruppo.AsLong();
                        var listaUtenti = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                            .Where(u => !u.DTA_FINE.HasValue && u.GROUPS_SYSTEM_ID == idGruppoRuoloRif)
                            .Select(u => u.PEOPLE_SYSTEM_ID)
                            .ToListAsync();

                        if (listaUtenti == null || listaUtenti.Count == 0)
                            throw new UtentiRuoloNotFoundPi3Exception(idCorrGlobaliDest.ToString(), descCorrDest);

                        var datiUtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                        listaUtenti.ForEach(u =>
                        {
                            datiUtentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo
                            {
                                IdUtente = u.ToString()
                            });
                        });
                        aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                        {
                            Tipo = TipiTrasmissioneSingolaEnum.Uno,
                            IdRagioneTrasmissione = ragioneTrasmissioneEntity.SYSTEM_ID.ToString(),
                            RagioneConWorkflow = ragioneTrasmissioneEntity.CHA_TIPO_RAGIONE == "W",
                            NomeRagioneTrasmissione = ragioneTrasmissioneEntity.VAR_DESC_RAGIONE,
                            CessioneDirittiRagione = ragioneTrasmissioneEntity.CHA_CEDE_DIRITTI == "N" ? null : new CessioneDirittiRagione
                            {
                                MantieniLettura = ragioneTrasmissioneEntity.CHA_MANTIENI_LETT == "1",
                                MantieniScrittura = ragioneTrasmissioneEntity.CHA_MANTIENI_SCRITT == "1",
                                ConSceltaUtente = ragioneTrasmissioneEntity.CHA_CEDE_DIRITTI == "W"
                            },
                            IdGruppoDestinatario = ruoloRif.idGruppo,
                            UtentiNotificati = datiUtentiNotificati
                        });
                    }
                }
                if (destinatario.DatiDestinatario.GetType() == typeof(Ruolo))
                {
                    idCorrGlobaliDestTrasmissione = destinatario.DatiDestinatario.systemId;

                    var idGruppoDest = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idCorrGlobaliDest).Select(c => c.ID_GRUPPO).FirstAsync();
                    var listaUtenti = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                        .Where(u => !u.DTA_FINE.HasValue && u.GROUPS_SYSTEM_ID == idGruppoDest)
                        .Select(u => u.PEOPLE_SYSTEM_ID)
                        .ToListAsync();

                    if (listaUtenti == null || listaUtenti.Count == 0)
                        throw new UtentiRuoloNotFoundPi3Exception(idCorrGlobaliDest.ToString(), descCorrDest);

                    var datiUtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                    listaUtenti.ForEach(u =>
                    {
                        datiUtentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo
                        {
                            IdUtente = u.ToString()
                        });
                    });
                    aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                    {
                        Tipo = TipiTrasmissioneSingolaEnum.Uno,
                        IdRagioneTrasmissione = ragioneTrasmissioneEntity.SYSTEM_ID.ToString(),
                        RagioneConWorkflow = ragioneTrasmissioneEntity.CHA_TIPO_RAGIONE == "W",
                        NomeRagioneTrasmissione = ragioneTrasmissioneEntity.VAR_DESC_RAGIONE,
                        CessioneDirittiRagione = ragioneTrasmissioneEntity.CHA_CEDE_DIRITTI == "N" ? null : new CessioneDirittiRagione
                        {
                            MantieniLettura = ragioneTrasmissioneEntity.CHA_MANTIENI_LETT == "1",
                            MantieniScrittura = ragioneTrasmissioneEntity.CHA_MANTIENI_SCRITT == "1",
                            ConSceltaUtente = ragioneTrasmissioneEntity.CHA_CEDE_DIRITTI == "W"
                        },
                        IdGruppoDestinatario = idGruppoDest.ToString(),
                        UtentiNotificati = datiUtentiNotificati
                    });
                }
                if (destinatario.DatiDestinatario.GetType() == typeof(Utente))
                {
                    idCorrGlobaliDestTrasmissione = destinatario.DatiDestinatario.systemId;

                    var idPeople = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idCorrGlobaliDest).Select(c => c.ID_PEOPLE).FirstAsync();
                    aggregate.PrepareTrasmissioneSingolaUtente(new DatiTrasmissioneSingolaUtente()
                    {
                        IdUtente = idPeople.ToString(),
                        IdRagioneTrasmissione = ragioneTrasmissioneEntity.SYSTEM_ID.ToString(),
                        NomeRagioneTrasmissione = ragioneTrasmissioneEntity.VAR_DESC_RAGIONE,
                        RagioneConWorkflow = ragioneTrasmissioneEntity.CHA_TIPO_RAGIONE == "W",
                        CessioneDirittiRagione = ragioneTrasmissioneEntity.CHA_CEDE_DIRITTI == "N" ? null : new CessioneDirittiRagione
                        {
                            MantieniLettura = ragioneTrasmissioneEntity.CHA_MANTIENI_LETT == "1",
                            MantieniScrittura = ragioneTrasmissioneEntity.CHA_MANTIENI_SCRITT == "1",
                            ConSceltaUtente = ragioneTrasmissioneEntity.CHA_CEDE_DIRITTI == "W"
                        }
                    });
                }

                aggregate.Invia();

                await this._trasmissioneRepository.Add(aggregate);

                foreach (var ts in aggregate.TrasmissioniSingole)
                {
                    await this._webMethodLoggerService.LogOK("TRASM_DOC_" + ts.RagioneTrasmissione.Nome.ToUpper().Replace(" ", "_"),
                        aggregate.OggettoTrasmesso.Id,
                        string.Format(Resources.LogTrasmessoDocumento, schedaDocumento.protocollo.segnatura),
                        ts.Id);
                }

                var dataUltimaTrasmissione = await _dbContext.TrasmissioneEntities
                    .Join(_dbContext.TrasmSingolaEntities,
                        t => t.SYSTEM_ID,
                        s => s.ID_TRASMISSIONE,
                        (t, s) => new { t, s })
                    .Where(j => j.t.ID_PROFILE == schedaDocumento.systemId.AsLong() && j.t.DTA_INVIO.HasValue
                        && j.s.ID_CORR_GLOBALE == idCorrGlobaliDestTrasmissione.AsLong())
                    .OrderByDescending(j => j.t.SYSTEM_ID)
                    .Select(j => j.t.DTA_INVIO)
                    .FirstOrDefaultAsync();

                if (dataUltimaTrasmissione.HasValue)
                {
                    destinatario.DataUltimaSpedizione = dataUltimaTrasmissione.AsDateTimeFormat();
                    destinatario.StatoSpedizione.Stato = StatiSpedizioneDocumentoEnum.Spedito;
                    destinatario.StatoSpedizione.Descrizione = Resources.LogStatoTrasmissioneSpedito;
                    destinatario.IncludiInSpedizione = false;
                }
                else
                {
                    destinatario.StatoSpedizione.Stato = StatiSpedizioneDocumentoEnum.ErroreInSpedizione;
                    destinatario.StatoSpedizione.Descrizione = Resources.LogStatoTrasmissioneErroreInSpedizione;
                    destinatario.IncludiInSpedizione = false;
                }

            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: "Errore in SpedizionePerTrasmissione: " + pi3Ex.Message);

                // Errore nell'invio del documento per trasmissione
                destinatario.StatoSpedizione.Stato = StatiSpedizioneDocumentoEnum.ErroreInSpedizione;
                destinatario.StatoSpedizione.Descrizione = pi3Ex.Message;
                infoSpedizione.listaDestinatariNonRaggiungibili.Add(pi3Ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: "Errore in SpedizionePerTrasmissione: " + ex.Message);

                // Errore nell'invio del documento per trasmissione
                destinatario.StatoSpedizione.Stato = StatiSpedizioneDocumentoEnum.ErroreInSpedizione;
                destinatario.StatoSpedizione.Descrizione = Resources.LogErroreSpedizionePerTrasmissione;
            }
        }

        protected bool IsDestinatarioPrincipale(SchedaDocumento documento, string idDestinatario)
        {
            ProtocolloUscita protocolloUscita = (ProtocolloUscita)documento.protocollo;

            return (protocolloUscita.destinatari).Count(e => e.systemId == idDestinatario) > 0;
        }

        protected async Task SpedizionePerInteroperabilitaSemplificata(SchedaDocumento schedaDocumento, List<DestinatarioEsterno> receivers, InfoUtente infoUtente)
        {
            var sendSucceded = true;
            var errorMessage = string.Empty;
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
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


                var instance = switchInteropPitreEntity.VAR_INSTANCE;

                //Costruzione dell'oggetto con le informazioni sulla spedizione
                var interoperabilityMessage = await CreateInteroperabilityMessage(schedaDocumento, infoUtente, corrs.ToArray());

                try
                {
                    //Invio del messaggio al sistema di interoperabilità
                    await this._mediator.Send(
                        new MessageQueueCommandWrapper(new SpedizioneInteropPiTreCommand(this._claimsPrincipalService.Current)
                        {
                            Instance = instance,
                            Authorization = await GetAuthToken(),
                            Tenant = interoperabilityMessage.Receivers[0].AdministrationCode,
                            InteroperabilityMessage = interoperabilityMessage
                        }));

                    foreach (var corr in corrs)
                        await InsertStatoInvio(schedaDocumento.systemId.AsLong(), corr, receiverUlr);
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
                    errorMessage = Resources.LogErroreSpedizionePerInteroperabilitaSemplificata;
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
                        destinatario.DataUltimaSpedizione = await GetDataUltimaSpedizione(schedaDocumento.systemId.AsLong(), destinatario.DatiDestinatari[0].systemId.AsLong());
                        destinatario.StatoSpedizione.Stato = StatiSpedizioneDocumentoEnum.Spedito;
                        destinatario.StatoSpedizione.Descrizione = Resources.LogStatoSpedizioneSpedito;
                        destinatario.IncludiInSpedizione = false;
                    }
                }
            }
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
            if (!isCorrEnabledToInterop || (await _mediator.Send(new GetElementoInRubricaComuneNoEsternaCommand()
            {
                Codice = senderCode,
                InfoUtente = infoUtente
            })).Corrispondente == null)
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

            var originalFileName = await _dbContext.ComponentEntities
                .Where(c => c.VERSION_ID == mainDocument.versionId.AsLong()
                    && c.DOCNUMBER == mainDocument.docNumber.AsLong())
                .Select(c => c.VAR_NOMEORIGINALE)
                .FirstOrDefaultAsync();

            if (String.IsNullOrEmpty(originalFileName))
                originalFileName = mainDocument.fileName;

            DocumentInfo documentInfo = new DocumentInfo()
            {
                DocumentNumber = mainDocument.docNumber,
                DocumentServerLocation = mainDocument.docServerLoc ?? string.Empty,
                FileName = originalFileName ?? string.Empty,
                FilePath = mainDocument.path ?? string.Empty,
                Name = mainDocument.fileName ?? string.Empty,
                VersionId = mainDocument.versionId,
                VersionLabel = mainDocument.versionLabel,
                Version = mainDocument.version,
                Fingerprint = mainDocument.impronta ?? string.Empty
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
                var tipoAllegato = await _dbContext.VersionEntities
                    .Where(v => v.VERSION_ID == allegato.versionId.AsLong())
                    .Select(v => v.CHA_ALLEGATI_ESTERNO)
                    .FirstAsync();

                if (tipoAllegato != "I" && tipoAllegato != "P")
                {
                    //Non invio gli allegati di tipo ricevuta PEC e IS
                    var originalFileName = await _dbContext.ComponentEntities
                       .Where(c => c.VERSION_ID == allegato.versionId.AsLong()
                           && c.DOCNUMBER == allegato.docNumber.AsLong())
                       .Select(c => c.VAR_NOMEORIGINALE)
                       .FirstOrDefaultAsync();

                    if (string.IsNullOrEmpty(originalFileName))
                        originalFileName = allegato.fileName;

                    attachmentsInfo.Add(new DocumentInfo()
                    {
                        DocumentNumber = allegato.docNumber,
                        DocumentServerLocation = allegato.docServerLoc ?? string.Empty,
                        FileName = originalFileName ?? string.Empty,
                        FilePath = allegato.path ?? string.Empty,
                        Name = allegato.descrizione ?? string.Empty,
                        NumberOfPages = allegato.numeroPagine,
                        VersionId = allegato.versionId,
                        VersionLabel = allegato.versionLabel,
                        Version = allegato.version,
                        Fingerprint = allegato.impronta ?? string.Empty
                    });
                }
            }

            return attachmentsInfo;
        }

        protected async Task<string?> GetAuthToken()
        {
            string token = string.Empty;
            var aacResp = await this._mediator.Send(new GetOrSetAacTokenCommand());
            if (aacResp != null)
                token = aacResp.Token;
            return $"Bearer {token}";
        }

        protected async Task SpedizionePerInteroperabilita(SchedaDocumento schedaDocumento, Registro registroRfMittente, List<DestinatarioEsterno> destinatariEsterni)
        {
            var isDestInteroperante = false;

            var casellaRegistroMittente = (await AmmGetMailRegistro(registroRfMittente?.systemId.AsLong())).Where(c => c.EmailRegistro.Equals(registroRfMittente.email)).FirstOrDefault();

            //File da allegare alla mail
            var body = await GetMessageBody(schedaDocumento, casellaRegistroMittente?.MessageSendMail, casellaRegistroMittente != null ? casellaRegistroMittente.OverwriteMessageAmm : false);
            var subject = await GetMessageSubject(schedaDocumento);
            List<EmailContentAttachment> attachments = await GetMessageAttachments(schedaDocumento);

            foreach (var destinatario in destinatariEsterni)
            {
                if (registroRfMittente == null)
                    throw new RegistroRFMittenteNotFoundPi3Exception();

                if (casellaRegistroMittente == null)
                    throw new RegistroNotFoundPi3Exception();

                try
                {
                    if (!destinatario.Interoperante)
                        throw new DestinatarioNonInteroperantePi3Exception();

                    if (destinatario.DatiDestinatari[0].canalePref == null ||
                        (destinatario.DatiDestinatari[0].canalePref != null &&
                         destinatario.DatiDestinatari[0].canalePref.descrizione != "INTEROPERABILITA" &&
                         destinatario.DatiDestinatari[0].canalePref.descrizione != "MAIL" &&
                         destinatario.DatiDestinatari[0].canalePref.descrizione != "PORTALE"))
                        throw new CanalePreferenzialeNonSupportatoPi3Exception();

                    //Se il destinatario non è interoperante non devo inviare il file segnatura.xml
                    isDestInteroperante = destinatario.DatiDestinatari[0].canalePref.descrizione.Equals("INTEROPERABILITA");

                    string ricevutaPec = !string.IsNullOrEmpty(casellaRegistroMittente.RicevutaPEC) ? casellaRegistroMittente.RicevutaPEC : null;

                    string X_TipoRicevuta = null;
                    if (ricevutaPec != null)
                    {

                        if (ricevutaPec != string.Empty)
                        {
                            X_TipoRicevuta = ricevutaPec;
                            switch (ricevutaPec.Length)
                            {
                                case 1:
                                    X_TipoRicevuta = ricevutaPec.Substring(0, 1);
                                    break;
                                case 2:
                                    //Se la len è maggiore di uno, vuol dire che ho un valore diverso da quello di default
                                    //Preleverò quindi quello.
                                    X_TipoRicevuta = ricevutaPec.Substring(1, 1);
                                    break;
                                default:    //non si sa mai
                                    X_TipoRicevuta = string.Empty;
                                    break;
                            }
                            //Qui transcodifico il tipo ricevuta CHA in header 
                            //(sarebbe carino metterlo in un enum per evitare hardcoding nel codice).
                            switch (X_TipoRicevuta)
                            {
                                case "C":
                                    X_TipoRicevuta = "completa";
                                    break;
                                case "B":
                                    X_TipoRicevuta = "breve";
                                    break;
                                case "S":
                                    X_TipoRicevuta = "sintetica";
                                    break;
                                default:
                                    X_TipoRicevuta = null;
                                    break;
                            }
                        }
                    }
                    var instructions = new SendEmailInstructions
                    {
                        Sender = new EmailSender
                        {
                            Address = casellaRegistroMittente.EmailRegistro
                        },
                        To = new List<EmailRecipient>
                    {
                        new EmailRecipient
                        {
                            Address = destinatario.Email,
                            DisplayName = destinatario.DatiDestinatari[0].descrizione
                        }
                    },
                        Subject = new Core.SeedWork.TextValue(subject),
                        Body = new Core.SeedWork.TextValue(body),
                        BodyIsHtml = true,
                        Attachments = isDestInteroperante ? attachments : attachments.Where(a => !a.FileName.Equals("segnatura.xml")).ToList(),
                    };

                    StringDictionary arguments = new StringDictionary();
                    //TO DO distinguere per provider
                    arguments.Add("Host", casellaRegistroMittente.ServerSMTP);
                    arguments.Add("Port", casellaRegistroMittente.PortaSMTP.ToString());
                    arguments.Add("RequireSsl", casellaRegistroMittente.SmtpSSL == "1" ? "true" : "false");
                    arguments.Add("UserName", casellaRegistroMittente.UserSMTP);
                    arguments.Add("Password", casellaRegistroMittente.PwdSMTP);

                    var result = await _emailSenderService.SendEmail((configurations) =>
                    {
                        switch (configurations)
                        {
                            case ChilkatSendEmailConfiguration chilkatEmailBoxConfigurations:
                                this.LoadChilkatSendEmailConfigurations((ChilkatSendEmailConfiguration)configurations, casellaRegistroMittente);
                                break;
                            default:
                                throw new SendMailPi3Exception(ErrorDescriptions.ProviderNonGestito);

                        }
                    },
                     instructions
                     );

                    if (result != null && !string.IsNullOrEmpty(result.MessageId))
                        destinatario.StatoSpedizione.Stato = StatiSpedizioneDocumentoEnum.Spedito;
                }
                catch (Pi3Exception pi3Ex)
                {
                    this._logger.LogError(exception: pi3Ex, message: "Errore in SpedizionePerInteroperabilita: " + pi3Ex.Message);

                    // Errore nell'invio del documento per interoperabilità
                    destinatario.StatoSpedizione.Stato = StatiSpedizioneDocumentoEnum.ErroreInSpedizione;
                    destinatario.StatoSpedizione.Descrizione = pi3Ex.Message;
                }
                catch (Exception ex)
                {
                    this._logger.LogCritical(exception: ex, message: "Errore in SpedizionePerInteroperabilita: " + ex.Message);

                    // Errore nell'invio del documento per interoperabilità
                    destinatario.StatoSpedizione.Stato = StatiSpedizioneDocumentoEnum.ErroreInSpedizione;
                    destinatario.StatoSpedizione.Descrizione = Resources.LogErroreSpedizionePerInteroperabilita;
                }

                await InsertStatoInvio(schedaDocumento.systemId.AsLong(), destinatario.DatiDestinatari[0], destinatario.Email);

                if (destinatario.StatoSpedizione.Stato != StatiSpedizioneDocumentoEnum.Spedito)
                {
                    destinatario.DataUltimaSpedizione = string.Empty;
                    destinatario.IncludiInSpedizione = true;
                }
                else
                {
                    destinatario.DataUltimaSpedizione = await GetDataUltimaSpedizione(schedaDocumento.systemId.AsLong(), destinatario.DatiDestinatari[0].systemId.AsLong());
                    destinatario.StatoSpedizione.Stato = StatiSpedizioneDocumentoEnum.Spedito;
                    destinatario.StatoSpedizione.Descrizione = Resources.LogStatoSpedizioneSpedito;
                    destinatario.IncludiInSpedizione = false;
                }
            }
        }
        private void LoadChilkatSendEmailConfigurations(ChilkatSendEmailConfiguration configurations, CasellaRegistro casellaRegistroMittente)
        {
            configurations.Host = casellaRegistroMittente.ServerSMTP;
            configurations.Port = casellaRegistroMittente.PortaSMTP;
            configurations.RequireSsl = casellaRegistroMittente.SmtpSSL == "1";
            configurations.UserName = casellaRegistroMittente.UserSMTP;
            configurations.Password = casellaRegistroMittente.PwdSMTP;
        }
        protected async Task<string> GetDataUltimaSpedizione(long idProfile, long idCorrispondente)
        {
            var dtaSpedizione = await _dbContext.StatoInvioEntities
                    .Where(s => s.ID_PROFILE == idProfile
                        && s.ID_CORR_GLOBALE == idCorrispondente)
                    .OrderByDescending(s => s.SYSTEM_ID)
                    .Select(s => s.DTA_SPEDIZIONE)
                    .FirstOrDefaultAsync();

            var dataUltimaSpedizione = dtaSpedizione != null ? dtaSpedizione.AsDateTimeFormat() : string.Empty;

            return dataUltimaSpedizione;
        }

        protected async Task<string> GetMessageBody(SchedaDocumento schedaDocumento, string varMessageSendMail, bool overwriteMsgAmm)
        {
            var bodyMail = Resources.SpedizioneBodyMail;
            bodyMail += string.Format(Resources.SpedizioneBodyMailRegistro, schedaDocumento.registro.codRegistro);
            bodyMail += string.Format(Resources.SpedizioneBodyMailNumeroProtocollo, schedaDocumento.protocollo.numero);
            bodyMail += string.Format(Resources.SpedizioneBodyMailDataProtocollazione, schedaDocumento.protocollo.dataProtocollazione);
            bodyMail += string.Format(Resources.SpedizioneBodyMailSegnatura, schedaDocumento.protocollo.segnatura);

            if (!overwriteMsgAmm)
            {
                var keyMessageSendPEC = await this._configurationService.GetValue<string>(schedaDocumento.registro.idAmministrazione, "BE_MESSAGE_SEND_PEC");
                if (!string.IsNullOrEmpty(keyMessageSendPEC) && !schedaDocumento.Equals("0"))
                {
                    bodyMail += "<br>" + keyMessageSendPEC.Replace("\n", "<br>") + "<br>";
                }
            }

            if (!string.IsNullOrEmpty(varMessageSendMail))
            {
                bodyMail += "<br>" + varMessageSendMail.Replace("\n", "<br>") + "<br>";
            }

            return bodyMail;
        }

        protected async Task<string> GetMessageSubject(SchedaDocumento schedaDocumento)
        {
            var subject = schedaDocumento.protocollo.segnatura + " - " + schedaDocumento.oggetto.descrizione;
            subject += "#" + schedaDocumento.docNumber + "#";

            return subject;
        }

        protected async Task<List<EmailContentAttachment>> GetMessageAttachments(SchedaDocumento schedaDocumento)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            var attachments = new List<EmailContentAttachment>();

            var mainDocumentEmailAttachment = await this.GetEmailContentAttachment(schedaDocumento.docNumber.AsLong(), idTenant);

            if (mainDocumentEmailAttachment is not null)
                attachments.Add(mainDocumentEmailAttachment);

            var docnumberAsLong = schedaDocumento.docNumber.AsLong();

            string[] typeAttach = new string[] { "P", "I", "D" };

            var profileAttachmentsEntities = await _dbContext.ProfileEntities
                .Join(_dbContext.VersionEntities,
                    p => p.SYSTEM_ID,
                    v => v.DOCNUMBER,
                    (p, v) => new { p, v })
                .Where(x => x.p.ID_DOCUMENTO_PRINCIPALE == docnumberAsLong &&
                     (!typeAttach.Contains(x.v.CHA_ALLEGATI_ESTERNO) || x.v.CHA_ALLEGATI_ESTERNO == null)
                     && x.v.VERSION_ID == (_dbContext.VersionEntities.AsNoTracking()
                     .Where(v => v.DOCNUMBER != null && v.DOCNUMBER.Value == x.p.DOCNUMBER.Value).OrderByDescending(v => v.VERSION_ID).Select(v => v.VERSION_ID).First()))
                .Select(x => x.p.SYSTEM_ID)
                .ToListAsync();

            if (profileAttachmentsEntities.Any())
            {
                foreach (var attachment in profileAttachmentsEntities)
                {
                    var a = await this.GetEmailContentAttachment(attachment, idTenant);
                    if (a is not null)
                        attachments.Add(a);
                }
            }

            return attachments;
        }

        protected async Task<EmailContentAttachment?> GetEmailContentAttachment(long docnumber, long idTenant)
        {
            var maxVersionId = (await _dbContext.VersionEntities.AsNoTracking()
                .Where(x => x.DOCNUMBER == docnumber)
                .OrderByDescending(x => x.VERSION)
                .FirstAsync()).VERSION_ID;

            var componentsEntity = await _dbContext.ComponentEntities.FirstAsync(x => x.DOCNUMBER == docnumber && x.VERSION_ID == maxVersionId);

            if (!string.IsNullOrWhiteSpace(componentsEntity.PATH))
            {
                var blob = await _documentBlobRepository.Get(idTenant.ToString(), componentsEntity.PATH);
                byte[] content = null;

                using (var stream = new MemoryStream())
                {
                    await blob.Stream.CopyToAsync(stream);
                    content = stream.ToArray();
                }

                return new EmailContentAttachment
                {
                    Content = content,
                    ContentType = blob.ContentType,
                    FileName = blob.FileName
                };
            }
            else return null;
        }

        public async Task<CasellaRegistro[]> AmmGetMailRegistro(long? idRegistro)
        {
            CasellaRegistro[] output = null;

            try
            {
                var mailRegistriEntities = await this._dbContext.MailRegistriEntities.AsNoTracking()
                    .Where(m => m.ID_REGISTRO == idRegistro && m.VAR_EMAIL_REGISTRO != null)
                    .OrderBy(m => m.VAR_PRINCIPALE)
                    .ToListAsync();

                if (mailRegistriEntities != null && mailRegistriEntities.Any())
                    output = this._mapper.Map<CasellaRegistro[]>(mailRegistriEntities);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return output;
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

        public async Task<Registro> GetRegistroBySistemId(long idRegistro)
        {
            DocsPaVO.utente.Registro registro = null;
            var entity = this._dbContext.RegistroEntities.FirstOrDefault(e => e.SYSTEM_ID == idRegistro);

            if (entity != null)
            {
                registro = this._mapper.Map<DocsPaVO.utente.Registro>(entity);
                registro.codAmministrazione = await this._dbContext.AmministraEntities
                            .AsNoTracking()
                            .Where(a => a.SYSTEM_ID == entity.ID_AMM)
                            .Select(a => a.VAR_CODICE_AMM)
                            .FirstOrDefaultAsync();
            }

            return registro;
        }

        public async Task<Ruolo[]> AddressbookGetRuoliRiferimentoAutorizzati(DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca, DocsPaVO.utente.UnitaOrganizzativa uo)
        {
            Ruolo[] output = null;

            List<Ruolo> ruoli = new List<Ruolo>();
            var tipoDestinatario = qca.ragione.tipoDestinatario.ToString();
            var idUO = uo.systemId.AsLong();

            try
            {
                var listIdUO = new List<long>();
                var livelloUO = qca.ruolo.uo.livello.AsLong();
                var livelloRuolo = qca.ruolo.livello.AsLong();
                long? idCurrentUO = qca.ruolo.uo.systemId.AsLong();

                //SUPERIORI
                if (tipoDestinatario == DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString())
                {
                    while ((idCurrentUO ?? 0) != 0)
                    {
                        listIdUO.Add((long)idCurrentUO);
                        idCurrentUO = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.SYSTEM_ID == idCurrentUO)
                            .Select(c => c.ID_PARENT)
                            .FirstAsync();
                    }
                }

                //SOTTOPOSTI
                if (tipoDestinatario == DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString())
                {
                    var listUOInferiori = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.CHA_TIPO_URP == "U" && c.CHA_TIPO_IE == "I" && !c.DTA_FINE.HasValue && c.NUM_LIVELLO >= livelloUO)
                        .Select(c => new
                        {
                            c.ID_PARENT,
                            c.SYSTEM_ID
                        })
                        .ToListAsync();

                    while ((idCurrentUO ?? 0) != 0)
                    {
                        listIdUO.Add((long)idCurrentUO);
                        idCurrentUO = listUOInferiori.Where(i => i.ID_PARENT == idCurrentUO).Select(c => c.SYSTEM_ID).FirstOrDefault();
                    }
                }

                //PARILIVELLO
                if (tipoDestinatario == DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString())
                {
                    var listUOPariLivello = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.CHA_TIPO_URP == "U" && c.CHA_TIPO_IE == "I" && !c.DTA_FINE.HasValue && c.NUM_LIVELLO == livelloUO)
                        .Select(c => c.SYSTEM_ID)
                        .ToListAsync();

                    listIdUO.Add((long)idCurrentUO);
                    listIdUO.AddRange(listUOPariLivello);
                }
                var ruoliRiferimentoQueryable = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Join(this._dbContext.RuoloRegistroEntities.AsNoTracking(), c => c.SYSTEM_ID, r => r.ID_RUOLO_IN_UO, (c, r) => new { c, r })
                    .Join(this._dbContext.TipoRuoloEntities, j => j.c.ID_TIPO_RUOLO, t => t.SYSTEM_ID, (j, t) => new
                    {
                        j.c.SYSTEM_ID,
                        j.c.VAR_DESC_CORR,
                        j.c.VAR_CODICE,
                        j.c.VAR_COD_RUBRICA,
                        j.c.ID_PARENT,
                        j.c.DTA_FINE,
                        j.c.CHA_TIPO_URP,
                        j.c.CHA_TIPO_IE,
                        j.c.CHA_RIFERIMENTO,
                        j.c.ID_UO,
                        j.c.ID_GRUPPO,
                        ID_REGISTRO = j.r.SYSTEM_ID,
                        t.NUM_LIVELLO
                    })
                    .Where(c => c.CHA_TIPO_URP == "R" && c.CHA_TIPO_IE == "I" && c.CHA_RIFERIMENTO == "1" && c.ID_UO == idUO);

                if (qca.queryCorrispondente != null && qca.queryCorrispondente.fineValidita)
                    ruoliRiferimentoQueryable = ruoliRiferimentoQueryable.Where(c => !c.DTA_FINE.HasValue);

                if (!string.IsNullOrEmpty(qca.idRegistro))
                {
                    var idRegistroAsLong = qca.idRegistro.AsLong();
                    ruoliRiferimentoQueryable = ruoliRiferimentoQueryable.Where(c => c.ID_REGISTRO == idRegistroAsLong);
                }

                if (listIdUO != null && listIdUO.Count > 0)
                    ruoliRiferimentoQueryable = ruoliRiferimentoQueryable.Where(c => listIdUO.Contains((long)c.ID_UO));

                if (tipoDestinatario == DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString())
                    ruoliRiferimentoQueryable = ruoliRiferimentoQueryable.Where(c => c.NUM_LIVELLO < livelloRuolo);

                if (tipoDestinatario == DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString())
                    ruoliRiferimentoQueryable = ruoliRiferimentoQueryable.Where(c => c.NUM_LIVELLO > livelloRuolo);

                if (tipoDestinatario == DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString())
                    ruoliRiferimentoQueryable = ruoliRiferimentoQueryable.Where(c => c.NUM_LIVELLO == livelloRuolo);

                var entity = await ruoliRiferimentoQueryable.Select(c => new
                {
                    c.SYSTEM_ID,
                    c.VAR_DESC_CORR,
                    c.VAR_CODICE,
                    c.VAR_COD_RUBRICA,
                    c.ID_PARENT,
                    c.ID_UO,
                    c.ID_GRUPPO,
                    c.NUM_LIVELLO
                })
                .Distinct()
                .ToListAsync();

                foreach (var e in entity)
                {
                    ruoli.Add(new Ruolo
                    {
                        codiceRubrica = e.VAR_COD_RUBRICA,
                        descrizione = e.VAR_DESC_CORR,
                        systemId = e.SYSTEM_ID.ToString(),
                        livello = e.NUM_LIVELLO.ToString(),
                        idGruppo = e.ID_GRUPPO.ToString(),
                        codiceCorrispondente = e.VAR_CODICE,
                        idAmministrazione = qca.ruolo.uo.idAmministrazione,
                        uo = qca.ruolo.uo
                    });
                }

                output = ruoli.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = null;
            }

            return output;
        }


        #endregion
    }
}
