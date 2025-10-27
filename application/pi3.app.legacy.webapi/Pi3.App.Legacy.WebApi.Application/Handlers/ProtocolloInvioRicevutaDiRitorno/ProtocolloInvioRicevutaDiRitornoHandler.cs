// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.addressbook;
using DocsPaVO.documento;
using DocsPaVO.Fatturazione;
using DocsPaVO.Interoperabilita.Segnatura;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Corrispondente = DocsPaVO.utente.Corrispondente;
using AutoMapper;
using DocsPaVO.areaConservazione;
using Registro = DocsPaVO.utente.Registro;
using DocsPaVO.Conservazione.PARER;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.SeedWork;
using System.Net.Mail;
using System.Drawing;
using Pi3.Core.Services.Configuration;
using System.Collections.Specialized;
using Pi3.App.Legacy.WebApi.Application.Handlers.ProtocolloInvioNotificaAnnulla;
using Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetMailRegistro;
using RabbitMQ.Client;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;
using Pi3.Core.Services.Factory;
using Pi3.Infrastructure.Graph.Services.Email.Sender;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ProtocolloInvioRicevutaDiRitorno
{

    // Richiede libreria MediatR
    public class ProtocolloInvioRicevutaDiRitornoHandler : IRequestHandler<Application.Requests.ProtocolloInvioRicevutaDiRitorno, ProtocolloInvioRicevutaDiRitornoResult>
    {
        #region Public Members

        public ProtocolloInvioRicevutaDiRitornoHandler(
            ILogger<ProtocolloInvioRicevutaDiRitornoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            //IEmailSenderService emailSenderService,
            IFactoryService factoryService)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
            _configurationService = configurationService;
            //_emailSenderService = emailSenderService;
            _factoryService = factoryService;
        }

        public async Task<ProtocolloInvioRicevutaDiRitornoResult> Handle(Application.Requests.ProtocolloInvioRicevutaDiRitorno request, CancellationToken cancellationToken)
        {
            var esito = true;
            var errorMessage = string.Empty;

            try
            {
                var idProfile = request.schedaDoc.systemId.AsLong();
                var idRegistro = request.registro.systemId.AsLong();
                var email = request.registro.email;

                var infoRegEntity = await _dbContext.MailRegistriEntities
                    .Join(_dbContext.RegistroEntities,
                        m => m.ID_REGISTRO,
                        r => r.SYSTEM_ID,
                        (m, r) => new { m, r })
                    .Join(_dbContext.AmministraEntities,
                        j => j.r.ID_AMM,
                        a => a.SYSTEM_ID,
                        (j, a) => new { j.m, j.r, a })
                    .Where(j => j.m.ID_REGISTRO == idRegistro && j.m.VAR_EMAIL_REGISTRO.Equals(email))
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
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (infoRegEntity == null)
                    throw new RegistroMittenteNotFoundPi3Exception(idRegistro.ToString());

                var separatore = infoRegEntity.CHA_STR_SEGNATURA ?? "/";

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
                           c.VAR_EMAIL,
                           j.p.CREATION_TIME,
                           j.p.NUM_PROTO,
                           j.p.DTA_PROTO
                       })
                   .Where(i => i.ID_PROFILE == idProfile)
                   .AsNoTracking()
                   .FirstOrDefaultAsync();

                //Recupero la mail del mittente
                var corrEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == infoMittEntity.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                var mailCorr = corrEntity.VAR_EMAIL;
                var mailMitt = string.Empty;
                var invioConfermaConEccezione = false;

                var mailMittenteProfile = await _dbContext.MailElaborataEntities.AsNoTracking()
                    .Where(m => m.ID_PROFILE == idProfile)
                    .Select(m => m.VAR_EMAIL)
                    .FirstOrDefaultAsync();

                if (mailMittenteProfile != null &&
                    await _dbContext.MailCorrEsterniEntities.AsNoTracking().AnyAsync(m => m.ID_CORR == infoMittEntity.SYSTEM_ID && m.VAR_EMAIL.ToUpper().Equals(mailMittenteProfile.ToUpper())))
                {
                    mailCorr = mailMittenteProfile;
                }

                if (!string.IsNullOrEmpty(mailCorr))
                {
                    if ((request.schedaDoc.interop == "P") || (!string.IsNullOrEmpty(corrEntity.VAR_CODICE_AMM) && !string.IsNullOrEmpty(corrEntity.VAR_CODICE_AOO)))
                    {
                        mailMitt = mailCorr;
                    }
                    else if (request.schedaDoc.interop == "E")
                    {
                        string[] s = email.Split('@');
                        string[] emails = new string[] { mailCorr.ToUpper(), "*" + s[1].ToUpper() };
                        invioConfermaConEccezione = await _dbContext.LivelloAnomaliaSegnaturaEntities.AsNoTracking()
                            .AnyAsync(a => a.VAR_ANOMALIA.ToUpper().Equals("INVIO_CONFERMA") && emails.Contains(a.VAR_EMAIL.ToUpper()) && a.CHA_LIVELLO_ECCEZIONE == "0");
                        if (invioConfermaConEccezione)
                            mailMitt = mailCorr;
                    }
                }

                if (string.IsNullOrEmpty(mailMitt))
                    throw new EmailUtenteNotFoundPi3Exception(corrEntity.SYSTEM_ID.ToString());

                //Fine recupero email mittente

                byte[]? conferma = null;
                if ((request.schedaDoc.typeId.ToUpper().Trim() == "INTEROPERABILITA") || (request.schedaDoc.interop.Equals("E") && invioConfermaConEccezione))
                {
                    var confermaXML = await GetConfermaXML(idProfile, request.registro, separatore);
                    conferma = System.Text.Encoding.UTF8.GetBytes(confermaXML);
                }

                //Invio della mail
                var oggetto = request.schedaDoc.oggetto.descrizione;
                var dataCreazione = infoMittEntity.CREATION_TIME.AsDateFormat();
                var segnatura = ((DocsPaVO.documento.Protocollo)request.schedaDoc.protocollo).segnatura;
                var mittente = ((DocsPaVO.documento.ProtocolloEntrata)request.schedaDoc.protocollo).mittente.descrizione;

                var subject = string.Format(Resources.MailSubject, oggetto, dataCreazione);
                if (subject.Length > 256)
                    subject = subject.Substring(0, 256);
                subject += $"#{request.schedaDoc.docNumber}#";

                var body = string.Format(Resources.MailBody, oggetto, dataCreazione, segnatura, infoRegEntity.VAR_CODICE_AMM, infoRegEntity.VAR_CODICE, infoMittEntity.NUM_PROTO, infoMittEntity.DTA_PROTO.AsDateFormat(), mittente);

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
                    Subject = new Core.SeedWork.TextValue(subject),
                    Body = new Core.SeedWork.TextValue(body),
                    BodyIsHtml = true,
                    Attachments = conferma != null ? new List<EmailContentAttachment>()
                    {
                        new EmailContentAttachment()
                        {
                             Content = conferma,
                             ContentType = "text/xml",
                             FileName = Resources.FileNameConfermaXml
                        }
                    } : null,
                };

                StringDictionary arguments = new StringDictionary();

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
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);

                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = ErrorDescriptions.GenericErrorInvioRicevuta;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);

                esito = false;
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = ErrorDescriptions.GenericErrorInvioRicevuta;
            }

            return new ProtocolloInvioRicevutaDiRitornoResult(esito, errorMessage);
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
        #endregion

        #region Private Members

        protected readonly ILogger<ProtocolloInvioRicevutaDiRitornoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        //protected IEmailSenderService _emailSenderService;
        protected readonly IFactoryService _factoryService;

        protected async Task<string> GetConfermaXML(long idProfile, Registro registro, string separatore)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var codiceIpaAmm = await this._dbContext.AmministraEntities.Where(x => x.SYSTEM_ID == idTenant).Select(x => x.VAR_CODICE_AMM_IPA).FirstOrDefaultAsync();

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
                           c.VAR_CODICE_AOO,
                           c.CHA_TIPO_IE,
                           c.VAR_COD_RUBRICA,
                           c.ID_AMM,
                           c.SYSTEM_ID,
                           c.VAR_EMAIL,
                           j.p.NUM_PROTO,
                           j.p.DTA_PROTO
                       })
                   .Where(i => i.ID_PROFILE == idProfile)
                   .AsNoTracking()
                   .FirstOrDefaultAsync();

            int MAX_LENGTH = 7;
            string zeroes = "";
            string numeroRegString = infoMittEntity.NUM_PROTO.ToString();
            for (int ind = 1; ind <= MAX_LENGTH - numeroRegString.Length; ind++)
            {
                zeroes = zeroes + "0";
            }
            numeroRegString = zeroes + numeroRegString;

            var codiceAOOMittString = infoMittEntity.VAR_CODICE_AOO;
            var numeroRegMittString = infoMittEntity.VAR_PROTO_IN;
            if (!string.IsNullOrEmpty(infoMittEntity.VAR_PROTO_IN))
            {
                codiceAOOMittString = infoMittEntity.VAR_PROTO_IN.Split(separatore)[0];
                if (infoMittEntity.VAR_PROTO_IN.Contains(separatore))
                {
                    numeroRegMittString = infoMittEntity.VAR_PROTO_IN.Split(separatore)[1];
                }
            }

            if (registro.chaRF != null && registro.chaRF == "1")
            {
                if (string.IsNullOrEmpty(registro.idAOOCollegata))
                    throw new AOOCollegataNotFoundPi3Exception(registro.descrizione);

                registro = (await _mediator.Send(new Requests.GetRegistroBySistemId(registro.idAOOCollegata))).output;
            }

            ConfermaType conferma = new ConfermaType()
            {
                Identificatore = new IdentificatoreType()
                {
                    CodiceAmministrazione = new CodiceIPA()
                    {
                        Value = codiceIpaAmm
                    },
                    CodiceAOO = new CodiceIPA()
                    {
                        Value = registro.codiceIpa
                    },
                    CodiceRegistro = registro.codRegistro,
                    NumeroRegistrazione = numeroRegString,
                    DataRegistrazione = infoMittEntity.DTA_PROTO.Value
                },
                MessaggioRicevuto = new MessaggioRicevutoType()
                {
                    ItemsElementName = new ItemsChoiceType3[]
                    {
                        ItemsChoiceType3.Identificatore
                    },
                    Items = new object[]
                    {
                        new IdentificatoreType()
                        {
                            CodiceAmministrazione = new DocsPaVO.Interoperabilita.Segnatura.CodiceIPA()
                            {
                                Value = infoMittEntity.VAR_CODICE_AMM
                            },
                            CodiceAOO = new DocsPaVO.Interoperabilita.Segnatura.CodiceIPA()
                            {
                                Value = infoMittEntity.VAR_CODICE_AOO
                            },
                            CodiceRegistro = codiceAOOMittString,
                            NumeroRegistrazione = numeroRegMittString,
                            DataRegistrazione = infoMittEntity.DTA_PROTO_IN.GetValueOrDefault()
                        }
                    }
                }
            };

            return conferma.ToXmlString(true, false, false, Encoding.UTF8);
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
