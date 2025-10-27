// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2016.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Oracle.EntityFrameworkCore.Query.Internal;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Annullamento;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Conferma;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Daticert;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Eccezione;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.EmailCallback;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Segnatura;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.StandardCase;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.Factory;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Chilkat.Services.Email.BoxScanner;
using Pi3.Infrastructure.Graph.Services.Email.BoxScanner;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed;
using CheckMailboxRequest = Pi3.App.Legacy.WebApi.Application.Requests.CheckMailBox;
using CheckScaricoOtherRoleRequest = Pi3.App.Legacy.WebApi.Application.Requests.CheckScaricoOtherRole;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox
{
    public class CheckMailboxHandler : IRequestHandler<CheckMailboxRequest, CheckMailBoxResult>
    {
        #region Public members

        public CheckMailboxHandler(ILogger<CheckMailboxHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            //IEmailBoxScannerService emailBoxScannerService,
            IFactoryService factoryService,
            IWebMethodLoggerService webMethodLoggerService,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
            this._factoryService = factoryService;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<CheckMailBoxResult> Handle(CheckMailboxRequest request, CancellationToken cancellationToken)
        {
            bool output = false;


            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, false);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, false);
            var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, false);
            var idTenant = request.ruolo.idAmministrazione.AsLong();
            var tenantCode = await this._dbContext.AmministraEntities.Where(x => x.SYSTEM_ID == idTenant).Select(x => x.VAR_CODICE_AMM).FirstOrDefaultAsync();

            if (idPeople == 0)
                idPeople = request.ut.idPeople.AsLong();
            if (idGroup == 0)
                idGroup = request.ruolo.idGruppo.AsLong();

            var ut = request.ut;
            var ruolo = request.ruolo;

            var deleteMessages = await _configurationService.GetValue<string>(idTenant.ToString(), "BE_ELIMINA_MAIL_ELABORATE");
            this._deleteMessages = !string.IsNullOrEmpty(deleteMessages) && deleteMessages.Equals("1");

            //costruisco i claims
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(Pi3ClaimTypes.IdTenant, idTenant.ToString()));
            claims.Add(new Claim(Pi3ClaimTypes.TenantCode, tenantCode));
            claims.Add(new Claim(Pi3ClaimTypes.Instance, instance));
            claims.Add(new Claim(Pi3ClaimTypes.UserId, ut.userId));
            claims.Add(new Claim(Pi3ClaimTypes.IdUser, ut.idPeople));
            claims.Add(new Claim(Pi3ClaimTypes.UserName, ut.nome));
            claims.Add(new Claim(Pi3ClaimTypes.UserSurname, ut.cognome));
            claims.Add(new Claim(Pi3ClaimTypes.IdGroup, idGroup.ToString()));

            var ruoloEntity = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroup).FirstOrDefaultAsync();
            claims.Add(new Claim(Pi3ClaimTypes.GroupCode, ruoloEntity.VAR_COD_RUBRICA));
            claims.Add(new Claim(Pi3ClaimTypes.GroupDescription, ruoloEntity.VAR_DESC_CORR));

            claims.Add(new Claim(Pi3ClaimTypes.Authorization, "DO_NUOVODOC"));
            claims.Add(new Claim(Pi3ClaimTypes.Authorization, "DO_NUOVOPROT"));
            claims.Add(new Claim(Pi3ClaimTypes.Authorization, "DO_PROT_PROTOCOLLA"));
            claims.Add(new Claim(Pi3ClaimTypes.Authorization, "DO_CREA_PERSONALE"));
            claims.Add(new Claim(Pi3ClaimTypes.Authorization, "DO_PROTO_PRIVATO"));
            claims.Add(new Claim(Pi3ClaimTypes.Authorization, "PROTO_IN"));
            claims.Add(new Claim(Pi3ClaimTypes.Authorization, "PROTO_OUT"));
            claims.Add(new Claim(Pi3ClaimTypes.Authorization, "PROTO_OWN"));
            claims.Add(new Claim(Pi3ClaimTypes.Authorization, "DO_ALL_AGGIUNGI"));

            var claimsIdentity = new ClaimsIdentity(
                authenticationType: "Pi3Authentication",
                claims: claims
                );

            _claimsPrincipalService.Current.AddIdentity(claimsIdentity);

            var checkMailboxEntity = await _dbContext.CheckMailboxEntities.FirstAsync(c => c.ID == request.idCheckMailbox.AsLong());

            var checkScaricoOtherRoleRequest = new CheckScaricoOtherRoleRequest(request.reg.systemId, request.reg.email);
            while ((await this._mediator.Send(checkScaricoOtherRoleRequest)).output)
            {
                System.Threading.Thread.Sleep(5000);
            }

            this._idCheckMailbox = checkMailboxEntity.ID;
            this._idRegister = request.reg.systemId.AsLong();
            this._registro = request.reg;

            try
            {
                var paramRegistroEntity = await this._dbContext.MailRegistriEntities.AsNoTracking()
                    .Join(this._dbContext.RegistroEntities.AsNoTracking(), m => m.ID_REGISTRO, r => r.SYSTEM_ID, (m, r) => new { m, r })
                    .Join(this._dbContext.AmministraEntities.AsNoTracking(), x => x.r.ID_AMM, a => a.SYSTEM_ID, (x, a) => new { x.m, x.r, a })
                    .Where(x => x.m.ID_REGISTRO == request.reg.systemId.AsLong()
                    && x.m.VAR_EMAIL_REGISTRO == request.reg.email)
                    .Select(x => new InfoRegEntity
                    {
                        SYSTEM_ID = x.r.SYSTEM_ID,
                        VAR_TIPO_CONNESSIONE = x.m.VAR_TIPO_CONNESSIONE,
                        VAR_USER_MAIL = x.m.VAR_USER_MAIL,
                        VAR_PWD_MAIL = x.m.VAR_PWD_MAIL,
                        VAR_SERVER_POP = x.m.VAR_SERVER_POP,
                        NUM_PORTA_POP = x.m.NUM_PORTA_POP,
                        CHA_POP_SSL = x.m.CHA_POP_SSL,
                        VAR_SERVER_IMAP = x.m.VAR_SERVER_IMAP,
                        NUM_PORTA_IMAP = x.m.NUM_PORTA_IMAP,
                        CHA_IMAP_SSL = x.m.CHA_IMAP_SSL,
                        VAR_INBOX_IMAP = x.m.VAR_INBOX_IMAP,
                        VAR_BOX_MAIL_ELABORATE = x.m.VAR_BOX_MAIL_ELABORATE,
                        VAR_MAIL_NON_ELABORATE = x.m.VAR_MAIL_NON_ELABORATE,
                        VAR_SOLO_MAIL_PEC = x.m.VAR_SOLO_MAIL_PEC,
                        VAR_EMAIL_REGISTRO = x.m.VAR_EMAIL_REGISTRO,
                        PROVIDER_ID = x.m.PROVIDER_ID,
                        MS_TENANT_ID = x.m.MS_TENANT_ID,
                        MS_CLIENT_ID = x.m.MS_CLIENT_ID,
                        MS_CLIENT_SEC = x.m.MS_CLIENT_SEC,
                        MS_FOLD_TO_READ = x.m.MS_FOLD_TO_READ,
                        CHA_SMTP_STA = x.m.CHA_SMTP_STA
                    })
                    .FirstOrDefaultAsync();

                if (paramRegistroEntity is null)
                {
                    throw new RegisterNotFoundPi3Exception(String.Format(ErrorDescriptions.RegisterNotFound, request.reg.email));
                }

                var provider = await this._dbContext.AssProviderLibEntities
                    .Where(x => x.PROVIDER_ID == paramRegistroEntity.PROVIDER_ID)
                    .Select(x => x.LIB)
                    .FirstOrDefaultAsync(); ;

                var creation = await _factoryService.TryCreate<IEmailBoxScannerService>(s => s.Provider == provider);

                if (!creation.Success)
                {
                    throw new ProviderNotFoundPi3Exception(String.Format(ErrorDescriptions.ProviderNotFound, provider));
                }

                this._emailAddress = paramRegistroEntity.VAR_EMAIL_REGISTRO;
                this._processOnlyPec = paramRegistroEntity.VAR_SOLO_MAIL_PEC == "1";

                EmailBoxConfigurations emailBoxConfiguration = default!;

                checkMailboxEntity.MAILSERVER = paramRegistroEntity.VAR_TIPO_CONNESSIONE == "POP" ? paramRegistroEntity.VAR_SERVER_POP : paramRegistroEntity.VAR_SERVER_IMAP;
                checkMailboxEntity.MAILUSERID = paramRegistroEntity.VAR_USER_MAIL;

                await ((DbContext)this._dbContext).SaveChangesAsync();

                this._emailBoxType = paramRegistroEntity.VAR_TIPO_CONNESSIONE == "POP" ? EmailBoxTypeEnum.POP : EmailBoxTypeEnum.IMAP;

                await creation.Service!.Scan(
                    (configurations) =>
                    {
                        switch (configurations)
                        {
                            case ChilkatEmailBoxConfigurations chilkatEmailBoxConfigurations:
                                this.LoadChilkatEmailBoxConfigurations((ChilkatEmailBoxConfigurations)configurations, paramRegistroEntity);
                                break;
                            case GraphEmailBoxConfiguration graphEmailBoxConfigurations:
                                this.LoadGraphEmailBoxConfigurations((GraphEmailBoxConfiguration)configurations, paramRegistroEntity);
                                break;
                            default:
                                throw new CheckMailBoxPi3Exception(Resources.ProviderNonGestito);
                        }
                    },
                    this.AnalyzeMail);

                output = true;
            }
            catch (Pi3Exception pi3ex)
            {
                this._logger.LogError(exception: pi3ex, message: pi3ex.Message);
                output = false;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                output = false;
            }
            finally
            {
                // Aggiornamento DPA_CHECK_MAILBOX
                checkMailboxEntity.CONCLUDED = "1";
                checkMailboxEntity.DTA_CONCLUDED = await _dbContext.GetSystemDateTime(); // DateTime.Now;

                await ((DbContext)this._dbContext).SaveChangesAsync();

                var report = await _dbContext.ReportMailboxEntities.AsNoTracking().Where(r => r.ID_CHECK_MAILBOX == checkMailboxEntity.ID).ToListAsync();
                var total = report.Count;
                var countNonElaborate = report.Count(r => !string.IsNullOrEmpty(r.ERROR));

                var objectDescription = string.Format(Resources.LogInfo, request.reg.codRegistro, request.reg.email);
                if (!string.IsNullOrEmpty(checkMailboxEntity.ERRORMESSAGE))
                    objectDescription += Resources.LogErrorServerPosta;
                else if (countNonElaborate > 0)
                {
                    objectDescription += string.Format(Resources.LogMessaggiNonProcessati, countNonElaborate.ToString());
                }
                else
                {
                    objectDescription += string.Format(Resources.LogMessaggiProcessati, total.ToString());
                }
                if (output)
                    await this._webMethodLoggerService.LogOK("INTEROPERABILITARICEZIONE", request.reg.systemId, objectDescription);
                else
                    await this._webMethodLoggerService.LogKO("INTEROPERABILITARICEZIONE", request.reg.systemId, objectDescription);
            }

            return new CheckMailBoxResult(output);
        }

        private void LoadGraphEmailBoxConfigurations(GraphEmailBoxConfiguration configurations, InfoRegEntity paramRegistroEntity)
        {
            configurations.TenantId = paramRegistroEntity.MS_TENANT_ID;
            configurations.ClientId = paramRegistroEntity.MS_CLIENT_ID;
            configurations.ClientSecret = paramRegistroEntity.MS_CLIENT_SEC;
            configurations.MailBox = paramRegistroEntity.VAR_EMAIL_REGISTRO;
            configurations.FolderToRead = paramRegistroEntity.MS_FOLD_TO_READ;
        }

        private void LoadChilkatEmailBoxConfigurations(ChilkatEmailBoxConfigurations configurations, InfoRegEntity paramRegistroEntity)
        {
            configurations.UserName = paramRegistroEntity.VAR_USER_MAIL;
            configurations.Password = DecodePassword(paramRegistroEntity.VAR_PWD_MAIL, paramRegistroEntity.VAR_USER_MAIL);
            configurations.StartTls = !string.IsNullOrEmpty(paramRegistroEntity.CHA_SMTP_STA) && paramRegistroEntity.CHA_SMTP_STA == "1";
            configurations.IsEmailProcessedByMessageId = (messageId) =>
            {
                this._logger.LogDebug($"MessageId >>>>> {messageId}, idRegistro >>>>>>>> {paramRegistroEntity.SYSTEM_ID}");
                return this._dbContext.MailElaborataEntities.AsNoTracking()
                   .Where(x => x.VAR_MESSAGE == messageId
                   && x.CHA_RAGIONE_ELAB == "E"
                   && (x.ID_REGISTRO == null || x.ID_REGISTRO == paramRegistroEntity.SYSTEM_ID))
                   .AnyAsync().GetAwaiter().GetResult();

            };

            switch (paramRegistroEntity.VAR_TIPO_CONNESSIONE)
            {
                case "POP":
                    configurations.EmailBoxTypeEnum = EmailBoxTypeEnum.POP;
                    configurations.Host = paramRegistroEntity.VAR_SERVER_POP!;
                    configurations.Port = paramRegistroEntity.NUM_PORTA_POP.HasValue ? Convert.ToInt32(paramRegistroEntity.NUM_PORTA_POP) : 110;
                    configurations.RequireSsl = paramRegistroEntity!.CHA_POP_SSL.Equals("1");
                    configurations.POPBehavior = new EmailBoxPOPBehavior()
                    {
                        DeleteEmailIfProcessed = this._deleteMessages
                    };
                    break;
                case "IMAP":
                    configurations.EmailBoxTypeEnum = EmailBoxTypeEnum.IMAP;
                    configurations.Host = paramRegistroEntity.VAR_SERVER_IMAP!;
                    configurations.Port = paramRegistroEntity.NUM_PORTA_IMAP.HasValue ? Convert.ToInt32(paramRegistroEntity.NUM_PORTA_IMAP) : 110;
                    configurations.RequireSsl = paramRegistroEntity!.CHA_IMAP_SSL.Equals("1");
                    configurations.IMAPBehavior = new EmailBoxIMAPBehavior()
                    {
                        Folders = new EmailBoxFolders()
                        {
                            CurrentFolder = paramRegistroEntity.VAR_INBOX_IMAP ?? string.Empty,
                            ProcessedFolder = paramRegistroEntity.VAR_BOX_MAIL_ELABORATE,
                            FailedFolder = paramRegistroEntity.VAR_MAIL_NON_ELABORATE
                        }
                    };
                    break;
            }

        }

        #endregion

        #region Private members

        protected readonly ILogger<CheckMailboxHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly IFactoryService _factoryService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        private EmailBoxTypeEnum? _emailBoxType = null;
        private long? _idCheckMailbox = null;
        private string? _emailAddress = null;
        private bool _processOnlyPec = false;
        private long? _idRegister = null;
        private bool _deleteMessages;

        private Registro _registro = null!;

        private Utente _ut;
        private Ruolo _ruolo;

        protected bool AnalyzeMail(EmailBoxScannerCallback callback)
        {
            return this._mediator.Send(new EmailCallbackRequest()
            {
                Email = callback.Email,
                Item = callback.Current,
                Total = callback.Total,
                EmailAddress = this._emailAddress!,
                EmailBoxType = this._emailBoxType!.Value,
                IdCheckMailbox = this._idCheckMailbox!.Value,
                IdRegister = this._idRegister!.Value,
                ProcessOnlyPec = this._processOnlyPec,
                Registro = this._registro
            })
                .GetAwaiter()
                .GetResult()
                .Output;
        }

        private string DecodePassword(string? password, string? key)
        {
            try
            {
                string decode = password ?? string.Empty;

                if (!string.IsNullOrEmpty(key))
                {
                    using (var aes = Aes.Create())
                    {
                        aes.KeySize = 128;
                        aes.BlockSize = 128;

                        aes.Key = ASCIIEncoding.ASCII.GetBytes(GenerateKey16(key));
                        aes.IV = ASCIIEncoding.ASCII.GetBytes(GenerateKey16(key));

                        byte[] input = Convert.FromBase64String(password ?? string.Empty);
                        byte[] output = aes.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
                        decode = Encoding.UTF8.GetString(output);
                        //caso di mancanza di password
                        if (decode.Equals(" "))
                            decode = string.Empty;
                    }
                }
                return decode;
            }
            catch (Exception)
            {
                return password ?? string.Empty;
            }
        }

        private string GenerateKey16(string stringa)
        {
            if (stringa.Length < 16)
                for (int i = stringa.Length; i < 16; i++)
                    stringa = "0" + stringa;
            else
                return stringa.Substring(0, 16);

            return stringa;
        }

        #endregion
    }
}
