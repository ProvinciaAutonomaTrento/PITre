// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Interoperabilita;
using DocsPaVO.Interoperabilita.Segnatura;
using DocsPaVO.Interoperabilita.Semplificata;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetMailRegistro;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.Interoperability;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.Factory;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;
using Pi3.Infrastructure.Graph.Services.Email.Sender;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mail;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ProtocolloInvioNotificaAnnullaRequest = Pi3.App.Legacy.WebApi.Application.Requests.ProtocolloInvioNotificaAnnulla;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ProtocolloInvioNotificaAnnulla
{
    public class ProtocolloInvioNotificaAnnullaHandler : IRequestHandler<ProtocolloInvioNotificaAnnullaRequest, ProtocolloInvioNotificaAnnullaResult>
    {
        #region Public Members

        public ProtocolloInvioNotificaAnnullaHandler(ILogger<ProtocolloInvioNotificaAnnullaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            //IEmailSenderService emailSenderService,
            IInteroperabilityService interoperabilityService,
            IFactoryService factoryService,
            IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
            //this._emailSenderService = emailSenderService;
            this._interoperabilityService = interoperabilityService;
            this._httpContextAccessor = httpContextAccessor;
            this._factoryService = factoryService;
        }

        public async Task<ProtocolloInvioNotificaAnnullaResult> Handle(ProtocolloInvioNotificaAnnullaRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            try
            {
                var idProfileAsLong = request.idProfile.AsLong();

                var isDocumentReceivedWithIS = await _dbContext.SimpInteropReceivedMessageEntities.AsNoTracking().AnyAsync(s => s.PROFILEID == idProfileAsLong);
                if (isDocumentReceivedWithIS)
                {
                    output = await SendDocumentDroppedOrExceptionProofToSender(idProfileAsLong, request.registro);
                }
                else
                {
                    output = await SendNotificaAnnullamentoInteroperabilita(idProfileAsLong, request.registro);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                output = false;
            }

            return new ProtocolloInvioNotificaAnnullaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ProtocolloInvioNotificaAnnullaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        //protected IEmailSenderService _emailSenderService;
        protected readonly IInteroperabilityService _interoperabilityService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IFactoryService _factoryService;

        protected async Task<bool> SendNotificaAnnullamentoInteroperabilita(long idProfile, Registro registro)
        {
            var esito = true;
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idRegistro = registro.systemId.AsLong();

            try
            {
                var infoRegEntity = await _dbContext.MailRegistriEntities
                 .Join(_dbContext.RegistroEntities,
                     m => m.ID_REGISTRO,
                     r => r.SYSTEM_ID,
                     (m, r) => new { m, r })
                 .Join(_dbContext.AmministraEntities,
                     j => j.r.ID_AMM,
                     a => a.SYSTEM_ID,
                     (j, a) => new { j.m, j.r, a })
                 .AsNoTracking()
                 .Where(j => j.m.ID_REGISTRO == idRegistro
                     && j.m.VAR_EMAIL_REGISTRO.Equals(registro.email))
                 .Select(j => new InfoRegEntity
                 {
                     SYSTEM_ID = j.r.SYSTEM_ID,
                     VAR_CODICE = j.r.VAR_CODICE,
                     ID_AMM = j.r.ID_AMM,
                     VAR_USER_MAIL = j.m.VAR_USER_MAIL,
                     VAR_PWD_MAIL = j.m.VAR_PWD_MAIL,
                     VAR_SERVER_SMTP = j.m.VAR_SERVER_SMTP,
                     NUM_PORTA_SMTP = j.m.NUM_PORTA_SMTP,
                     VAR_EMAIL_REGISTRO = j.m.VAR_EMAIL_REGISTRO,
                     VAR_CODICE_AMM = j.a.VAR_CODICE_AMM,
                     VAR_USER_SMTP = j.m.VAR_USER_SMTP,
                     CHA_STR_SEGNATURA = j.a.CHA_STR_SEGNATURA,
                     VAR_PWD_SMTP = j.m.VAR_PWD_SMTP,
                     CHA_POP_SSL = j.m.CHA_POP_SSL,
                     CHA_SMTP_SSL = j.m.CHA_SMTP_SSL,
                     CHA_SMTP_STA = j.m.CHA_SMTP_STA,
                     VAR_SERVER_IMAP = j.m.VAR_SERVER_IMAP,
                     NUM_PORTA_IMAP = j.m.NUM_PORTA_IMAP,
                     VAR_TIPO_CONNESSIONE = j.m.VAR_TIPO_CONNESSIONE,
                     VAR_INBOX_IMAP = j.m.VAR_INBOX_IMAP,
                     VAR_BOX_MAIL_ELABORATE = j.m.VAR_BOX_MAIL_ELABORATE,
                     VAR_MAIL_NON_ELABORATE = j.m.VAR_MAIL_NON_ELABORATE,
                     CHA_IMAP_SSL = j.m.CHA_IMAP_SSL,
                     VAR_SOLO_MAIL_PEC = j.m.VAR_SOLO_MAIL_PEC,
                     NUM_PORTA_POP = j.m.NUM_PORTA_POP,
                     VAR_SERVER_POP = j.m.VAR_SERVER_POP,
                     PROVIDER_ID = j.m.PROVIDER_ID,
                     MS_TENANT_ID = j.m.MS_TENANT_ID,
                     MS_CLIENT_ID = j.m.MS_CLIENT_ID,
                     MS_CLIENT_SEC = j.m.MS_CLIENT_SEC,
                     MS_FOLD_TO_READ = j.m.MS_FOLD_TO_READ
                 })
                 .FirstOrDefaultAsync();

                var infoMittEntity = await _dbContext.ProfileEntities
                    .Join(_dbContext.DocArrivoParEntities,
                        p => p.SYSTEM_ID,
                        a => a.ID_PROFILE,
                        (p, a) => new { p, a })
                    .Join(_dbContext.CorrGlobaliEntities,
                        j => j.a.ID_MITT_DEST,
                        c => c.SYSTEM_ID,
                        (j, c) => new
                        {
                            ID_PROFILE = j.p.SYSTEM_ID,
                            j.p.VAR_PROTO_IN,
                            j.p.DTA_PROTO_IN,
                            c.VAR_CODICE_AMM,
                            c.CHA_TIPO_IE,
                            c.VAR_COD_RUBRICA,
                            c.ID_AMM,
                            c.SYSTEM_ID,
                            c.VAR_EMAIL
                        })
                    .Where(i => i.ID_PROFILE == idProfile)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                //Invio la ricevuta di protocollazione all'indirizzo Email da cui ho ricevuto il documento.
                var protoIn = infoMittEntity.VAR_PROTO_IN;
                string codiceAOOMittString = protoIn.Split('/')[0];
                string numeroRegMittString = (protoIn.Split('/').Length > 1 ? protoIn.Split('/')[1] : string.Empty);

                var mailMitt = infoMittEntity.VAR_EMAIL;
                var mailMittenteProfile = await _dbContext.MailElaborataEntities.AsNoTracking()
                    .Where(m => m.ID_PROFILE == idProfile)
                    .Select(m => m.VAR_EMAIL)
                    .FirstOrDefaultAsync();
                if(mailMittenteProfile != null &&
                    await _dbContext.MailCorrEsterniEntities.AsNoTracking().AnyAsync(m => m.ID_CORR == infoMittEntity.SYSTEM_ID && m.VAR_EMAIL.ToUpper().Equals(mailMittenteProfile.ToUpper())))
                {
                    mailMitt = mailMittenteProfile;
                }

                var annullamentoXML = await GetAnnullamentoProtocollazioneXML(idProfile, registro);
                byte[] annullamentoProtocollazioneXML = System.Text.Encoding.UTF8.GetBytes(annullamentoXML);
                //byte[] annullamentoProtocollazioneXML = System.Text.Encoding.UTF8.GetBytes(annullamentoXML.ToXmlString());

                //Invio della mail
                var instructions = new SendEmailInstructions
                {
                    Sender = new EmailSender
                    {
                        Address = infoRegEntity.VAR_EMAIL_REGISTRO
                    },
                    To = new List<EmailRecipient>
                    {
                        new EmailRecipient
                        {
                            Address = mailMitt
                        }
                    },
                    Subject = new Core.SeedWork.TextValue(Resources.MailSubject),
                    Body = new Core.SeedWork.TextValue(string.Format(Resources.MailObject, numeroRegMittString)),
                    BodyIsHtml = true,
                    Attachments = new List<EmailContentAttachment>()
                    {
                        new EmailContentAttachment()
                        {
                             Content = annullamentoProtocollazioneXML,
                             ContentType = "text/xml",
                             FileName = Resources.FileNameAnnullamento
                        }
                    },
                };

                
                StringDictionary arguments = new StringDictionary();
                //TO DO distinguere per provider
                arguments.Add("Host", infoRegEntity.VAR_SERVER_SMTP);
                arguments.Add("Port", infoRegEntity.NUM_PORTA_SMTP.HasValue ? infoRegEntity.NUM_PORTA_SMTP.ToString() : "0");
                arguments.Add("RequireSsl", infoRegEntity.CHA_SMTP_SSL == "1" ? "true" : "false");
                arguments.Add("UserName", infoRegEntity.VAR_USER_SMTP);
                arguments.Add("Password", Crypter.Decode(infoRegEntity.VAR_PWD_SMTP, infoRegEntity.VAR_USER_SMTP));

                var provider = await this._dbContext.AssProviderLibEntities
                    .Where(x => x.PROVIDER_ID == infoRegEntity.PROVIDER_ID)
                    .Select(x => x.LIB)
                    .FirstOrDefaultAsync();

                var creation = await _factoryService.TryCreate<IEmailSenderService>(s => s.Provider == provider);

                if (!creation.Success)
                    throw new ProviderNotFoundPi3Exception(String.Format(ErrorDescriptions.ProviderNotFound, provider));

                var result = await creation.Service.SendEmail((configurations) =>
                {
                    switch (configurations)
                    {
                        case ChilkatSendEmailConfiguration chilkatEmailBoxConfigurations:
                            this.LoadChilkatSendEmailConfigurations((ChilkatSendEmailConfiguration)configurations, infoRegEntity);
                            break;
                        case GraphSendEmailConfiguration graphSendEmailConfigurations:
                            this.LoadGraphSendEmailConfigurations((GraphSendEmailConfiguration)configurations, infoRegEntity);
                            break;
                        default:
                            throw new SendMailPi3Exception(ErrorDescriptions.ProviderNonGestito);

                    }
                },
                instructions
                );
            }
            catch (Pi3Exception pi3Ex)
            {
                esito = false;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);

            }
            catch (Exception ex)
            {
                esito = false;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return esito;
        }

        private void LoadGraphSendEmailConfigurations(GraphSendEmailConfiguration configurations, InfoRegEntity infoRegEntity)
        {
            configurations.TenantId = infoRegEntity.MS_TENANT_ID;
            configurations.ClientId = infoRegEntity.MS_CLIENT_ID;
            configurations.ClientSecret = infoRegEntity.MS_CLIENT_SEC;
            configurations.MailBox = infoRegEntity.VAR_EMAIL_REGISTRO;
        }

        private void LoadChilkatSendEmailConfigurations(ChilkatSendEmailConfiguration configurations, InfoRegEntity infoRegEntity)
        {
            configurations.Host = infoRegEntity.VAR_SERVER_SMTP;
            configurations.Port = infoRegEntity.NUM_PORTA_SMTP.HasValue ? Convert.ToInt32(infoRegEntity.NUM_PORTA_SMTP) : 0;
            configurations.StartTLS = !string.IsNullOrEmpty(infoRegEntity.CHA_SMTP_STA) && infoRegEntity.CHA_SMTP_STA == "1";
            configurations.RequireSsl = infoRegEntity.CHA_SMTP_SSL == "1";
            configurations.UserName = infoRegEntity.VAR_USER_SMTP;
            configurations.Password = Crypter.Decode(infoRegEntity.VAR_PWD_SMTP, infoRegEntity.VAR_USER_SMTP);
        }

        protected async Task<string> GetAnnullamentoProtocollazioneXML(long idProfile, Registro registro)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var tenant = await this._dbContext.AmministraEntities.Where(x => x.SYSTEM_ID == idTenant).FirstOrDefaultAsync();
            var tenantCode = tenant.VAR_CODICE_AMM;

                        var infoDatiProto = await _dbContext.ProfileEntities.AsNoTracking()
                       .Where(p => p.SYSTEM_ID == idProfile)
                       .Select(p => new
                       {
                           p.NUM_PROTO,
                           p.DTA_PROTO,
                           p.NUM_ANNO_PROTO,
                           p.ID_REGISTRO,
                           p.DTA_ANNULLA,
                           p.VAR_AUT_ANNULLA
                       })
                       .FirstOrDefaultAsync();

            int MAX_LENGTH = 7;
            string zeroes = "";
            string numeroRegString = infoDatiProto.NUM_PROTO.ToString();
            for (int ind = 1; ind <= MAX_LENGTH - numeroRegString.Length; ind++)
            {
                zeroes = zeroes + "0";
            }
            numeroRegString = zeroes + numeroRegString;

            if (registro.chaRF != null && registro.chaRF == "1")
            {
                if (string.IsNullOrEmpty(registro.idAOOCollegata))
                    throw new AOOCollegataNotFoundPi3Exception(registro.descrizione);

                registro = (await _mediator.Send(new Requests.GetRegistroBySistemId(registro.idAOOCollegata))).output;
            }

            AnnullamentoProtocollazioneType annullamento = new AnnullamentoProtocollazioneType()
            {
                Identificatore = new IdentificatoreType()
                {
                    CodiceAmministrazione = new CodiceIPA()
                    {
                        Value = tenant.VAR_CODICE_AMM_IPA
                    },
                    CodiceAOO = new CodiceIPA()
                    {
                        Value = registro.codiceIpa
                    },
                    CodiceRegistro = registro.codRegistro,
                    NumeroRegistrazione = numeroRegString,
                    DataRegistrazione = infoDatiProto.DTA_PROTO.Value
                },
                Motivo = infoDatiProto.VAR_AUT_ANNULLA,
                Provvedimento = string.Empty
            };

            //return annullamento.ToXmlString<AnnullamentoProtocollazioneType>()
            return annullamento.ToXmlString();
        }

        protected async Task<bool> SendDocumentDroppedOrExceptionProofToSender(long idProfile, Registro registro)
        {
            var esito = true;
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            try
            {
                var tenantCode = await this._dbContext.AmministraEntities.Where(x => x.SYSTEM_ID == idTenant).Select(x => x.VAR_CODICE_AMM).FirstOrDefaultAsync();

                var infoMittEntity = await _dbContext.ProfileEntities
                        .Join(_dbContext.DocArrivoParEntities,
                            p => p.SYSTEM_ID,
                            a => a.ID_PROFILE,
                            (p, a) => new { p, a })
                        .Join(_dbContext.CorrGlobaliEntities,
                            j => j.a.ID_MITT_DEST,
                            c => c.SYSTEM_ID,
                            (j, c) => new
                            {
                                ID_PROFILE = j.p.SYSTEM_ID,
                                j.p.VAR_PROTO_IN,
                                j.p.DTA_PROTO_IN,
                                c.VAR_CODICE_AMM,
                                c.CHA_TIPO_IE,
                                c.VAR_COD_RUBRICA,
                                c.ID_AMM,
                                c.SYSTEM_ID
                            })
                        .Where(i => i.ID_PROFILE == idProfile)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                var infoDatiProto = await _dbContext.ProfileEntities.AsNoTracking()
                            .Where(p => p.SYSTEM_ID == idProfile)
                            .Select(p => new
                            {
                                p.NUM_PROTO,
                                p.DTA_PROTO,
                                p.NUM_ANNO_PROTO,
                                p.ID_REGISTRO,
                                p.DTA_ANNULLA,
                                p.VAR_AUT_ANNULLA
                            })
                            .FirstOrDefaultAsync();

                if (infoMittEntity != null)
                {
                    var sepSegnatura = '/';
                    var codAmm = await _dbContext.AmministraEntities.AsNoTracking().Where(a => a.SYSTEM_ID == idTenant).Select(a => a.VAR_CODICE_AMM).FirstOrDefaultAsync();
                    string[] protoMitt = infoMittEntity.VAR_PROTO_IN.Split(sepSegnatura, 2);
                    var dataProtoMitt = infoMittEntity.DTA_PROTO_IN;
                    var dataProto = infoDatiProto.DTA_PROTO;
                    var numProto = infoDatiProto.NUM_PROTO;
                    var dataAnnulla = infoDatiProto.DTA_ANNULLA;
                    var motivoAnnulla = infoDatiProto.VAR_AUT_ANNULLA;

                    if (protoMitt != null && dataProtoMitt != null && protoMitt.Length > 1)
                    {
                        NotificaAnnullamento notificaAnnullamento = new NotificaAnnullamento()
                        {
                            codAmm = codAmm,
                            codAmm_Mitt = codAmm, //so che viene dalla stessa amm, ma in futuro posso valutarlo nelle multiamm, dalla dpa_stato_invio ?.
                            codAOO = registro.codRegistro,
                            codAOO_Mitt = protoMitt[0],
                            dataRegistr_Mitt = dataProtoMitt.AsDateTimeFormat().Substring(0, dataProtoMitt.AsDateTimeFormat().IndexOf(" ")),
                            dataRegistrazione = dataProto.AsDateTimeFormat().Substring(0, dataProtoMitt.AsDateTimeFormat().IndexOf(" ")),
                            numeroRegistr_Mitt = protoMitt[1],
                            numeroRegistrazione = numProto.ToString(),
                            dataAnnullamento = dataAnnulla.AsDateTimeFormat(),
                            motivoAnnullamento = motivoAnnulla
                        };

                        // Se il documento � stato ricevuto per interoperabilit� semplificata, l'elaborazione viene demandata al gestore dell'IS
                        var isEnabledSimplifiedInteroperability = await this._configurationService.GetValue<string>(idTenant.ToString(), "INTEROP_SERVICE_ACTIVE");
                        if (string.IsNullOrEmpty(isEnabledSimplifiedInteroperability))
                            isEnabledSimplifiedInteroperability = await this._configurationService.GetValue<string>("0", "INTEROP_SERVICE_ACTIVE");

                        if (isEnabledSimplifiedInteroperability == "1")
                        {
                            //Load sender doc info
                            var simpInteropReceivedMessageEntity = await _dbContext.SimpInteropReceivedMessageEntities.Where(s => s.PROFILEID == idProfile).FirstOrDefaultAsync();
                            if (simpInteropReceivedMessageEntity == null)
                                throw new CaricamentoInfoMittenteSpedizionePi3Exception();

                            var sender = await CreateSenderInfo(simpInteropReceivedMessageEntity);
                            var receiver = await CreateReceiverInfo(idProfile);

                            var senderUrl = simpInteropReceivedMessageEntity.SENDERURL ?? string.Empty;
                            var receiverCode = simpInteropReceivedMessageEntity.RECEIVERCODE ?? string.Empty;

                            var receiverUrl = await this._configurationService.GetValue<string>(idTenant.ToString(), "INTEROP_SERVICE_URL");
                            if (string.IsNullOrEmpty(receiverUrl))
                                receiverUrl = await this._configurationService.GetValue<string>("INTEROP_SERVICE_URL");

                            bool useNewServiceInteropPiTre = false;
                            var switchInteropPitreEntity = await _dbContext.SwitchServiceInteropEntities.AsNoTracking()
                                .Where(s => s.VAR_INTEROP_URL.ToUpper().Equals(senderUrl.ToUpper()))
                                .FirstOrDefaultAsync();
                            if (switchInteropPitreEntity != null)
                            {
                                useNewServiceInteropPiTre = switchInteropPitreEntity.CHA_USE_NEW_INTEROP == "1";
                            }

                            if (useNewServiceInteropPiTre)
                            {
                                var instance = switchInteropPitreEntity.VAR_INSTANCE;

                                await this._interoperabilityService.AnalyzeDocumentDroppedOrErrorMessageProof(instance,
                                this.GetAuthToken(),
                                sender.AdministrationCode,
                                 new AnalyzeDocumentDroppedOrErrorMessageProofRequest()
                                 {
                                     SenderRecordInfo = sender,
                                     ReceiverRecordInfo = receiver,
                                     Reason = notificaAnnullamento.motivoAnnullamento,
                                     Operation = OperationDiscriminator.Drop,
                                     ReceiverCode = receiverCode,
                                     ReceiverUrl = receiverUrl
                                 });
                            }
                        }
                    }
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                esito = false;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                await ((DbContext)this._dbContext).Database.ExecuteSqlInterpolatedAsync($"INSERT INTO SimpInteropDbLog (PROFILEID, ERRORMESSAGE, TEXT) VALUES ({idProfile}, {1}, {pi3Ex.Message})");

            }
            catch (Exception ex)
            {
                esito = false;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return esito;
        }

        protected async Task<RecordInfo> CreateSenderInfo(SimpInteropReceivedMessageEntity simpInteropReceivedMessageEntity)
        {
            var sender = new RecordInfo()
            {
                AdministrationCode = simpInteropReceivedMessageEntity.SENDERADMINISTRATIONCODE,
                AOOCode = simpInteropReceivedMessageEntity.AOOCODE,
                RecordDate = simpInteropReceivedMessageEntity.RECORDDATE.Value,
                RecordNumber = simpInteropReceivedMessageEntity.RECORDNUMBER.ToString(),
                Subject = simpInteropReceivedMessageEntity.SUBJECT
                
            };

            return sender;
        }

        protected async Task<RecordInfo> CreateReceiverInfo(long idProfile)
        {
            RecordInfo receiver = null;

            var profileReceiverInfoEntity = await this._dbContext.ProfileEntities
                .Join(this._dbContext.CorrGlobaliEntities, p => p.ID_UO_PROT, cg => cg.SYSTEM_ID, (p, cg) => new { p, cg })
                .Join(this._dbContext.AmministraEntities, j1 => j1.cg.ID_AMM, a => a.SYSTEM_ID, (j1, a) => new { p = j1.p, cg = j1.cg, a = a })
                .Join(this._dbContext.RegistroEntities, j2 => j2.p.ID_REGISTRO, r => r.SYSTEM_ID, (j2, r) => new { p = j2.p, cg = j2.cg, a = j2.a, r })
                .Where(x => x.p.SYSTEM_ID == idProfile)
                .Select(x => new
                {
                    RECEIVER_AOO_CODE = x.r.VAR_CODICE,
                    RECEIVER_RECORD_NUMBER = x.p.NUM_PROTO,
                    RECEIVER_RECORD_DATE = x.p.DTA_PROTO,
                    RECEIVER_ADMINISTRATOR_CODE = x.a.VAR_CODICE_AMM,
                    SUBJECT = x.p.VAR_PROF_OGGETTO
                })
                .FirstOrDefaultAsync();

            if (profileReceiverInfoEntity != null)
            {
                receiver = new RecordInfo()
                {
                    AdministrationCode = profileReceiverInfoEntity.RECEIVER_ADMINISTRATOR_CODE,
                    AOOCode = profileReceiverInfoEntity.RECEIVER_AOO_CODE,
                    RecordDate = profileReceiverInfoEntity.RECEIVER_RECORD_DATE.Value,
                    RecordNumber = profileReceiverInfoEntity.RECEIVER_RECORD_NUMBER.ToString(),
                    Subject = profileReceiverInfoEntity.SUBJECT
                };
            }

            return receiver;
        }

        protected string? GetAuthToken()
        {
            if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            var authorizationHeader = authorizationStrings[0];

            return authorizationHeader;
        }

        #endregion
    }
    internal class InfoRegEntity
    {
        public string? VAR_CODICE { get; internal set; }
        public long SYSTEM_ID { get; internal set; }
        public long? ID_AMM { get; internal set; }
        public string? VAR_USER_MAIL { get; internal set; }
        public string? VAR_PWD_MAIL { get; internal set; }
        public string? VAR_SERVER_SMTP { get; internal set; }
        public long? NUM_PORTA_SMTP { get; internal set; }
        public string? VAR_EMAIL_REGISTRO { get; internal set; }
        public string? VAR_CODICE_AMM { get; internal set; }
        public string? VAR_USER_SMTP { get; internal set; }
        public string? CHA_STR_SEGNATURA { get; internal set; }
        public string? VAR_PWD_SMTP { get; internal set; }
        public string? CHA_POP_SSL { get; internal set; }
        public string? CHA_SMTP_SSL { get; internal set; }
        public string? CHA_SMTP_STA { get; internal set; }
        public string? VAR_SERVER_IMAP { get; internal set; }
        public long? NUM_PORTA_IMAP { get; internal set; }
        public string? VAR_TIPO_CONNESSIONE { get; internal set; }
        public string? VAR_INBOX_IMAP { get; internal set; }
        public string? VAR_BOX_MAIL_ELABORATE { get; internal set; }
        public string? VAR_MAIL_NON_ELABORATE { get; internal set; }
        public string? CHA_IMAP_SSL { get; internal set; }
        public string? VAR_SOLO_MAIL_PEC { get; internal set; }
        public long? NUM_PORTA_POP { get; internal set; }
        public string? VAR_SERVER_POP { get; internal set; }
        public string? PROVIDER_ID { get; internal set; }
        public string? MS_TENANT_ID { get; internal set; }
        public string? MS_CLIENT_ID { get; internal set; }
        public string? MS_CLIENT_SEC { get; internal set; }
        public string? MS_FOLD_TO_READ { get; internal set; }
    }
}