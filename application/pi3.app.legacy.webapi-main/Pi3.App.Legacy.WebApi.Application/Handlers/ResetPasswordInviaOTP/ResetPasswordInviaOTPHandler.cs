// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DocsPaVO.utente.UserLogin;
using ResetPasswordInviaOTPRequest = Pi3.App.Legacy.WebApi.Application.Requests.ResetPasswordInviaOTP;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ResetPasswordInviaOTP
{
    public class ResetPasswordInviaOTPHandler : IRequestHandler<ResetPasswordInviaOTPRequest, ResetPasswordInviaOTPResult>
    {
        #region Public Members

        public ResetPasswordInviaOTPHandler(ILogger<ResetPasswordInviaOTPHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            //IEmailSenderService emailSenderService,
            IFactoryService factoryService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            //this._emailSenderService = emailSenderService;
            this._factoryService = factoryService;
        }

        public async Task<ResetPasswordInviaOTPResult> Handle(ResetPasswordInviaOTPRequest request, CancellationToken cancellationToken)
        {
            var output = ResetPasswordResult.OK;
            var email = string.Empty;

            try
            {
                var peopleEntities = await _dbContext.PeopleEntities.AsNoTracking()
                    .Join(_dbContext.AmministraEntities.AsNoTracking(),
                        p => p.ID_AMM,
                        a => a.SYSTEM_ID,
                        (p, a) => new { p, a})
                    .Where(j => j.p.USER_ID.ToUpper().Equals(request.userId.ToUpper()) && j.p.DISABLED == "N")
                    .Select(j => new PeopleObj
                    {
                        SYSTEM_ID = j.p.SYSTEM_ID,
                        ID_AMM = j.p.ID_AMM,
                        EMAIL_ADDRESS = j.p.EMAIL_ADDRESS,
                        FULL_NAME = j.p.FULL_NAME,
                        FROM_EMAIL_ADDRESS_USER = j.p.FROM_EMAIL_ADDRESS,
                        FROM_EMAIL_ADDRESS_AMMINISTRA = j.a.FROM_EMAIL_ADDRESS,
                        VAR_SMTP = j.a.VAR_SMTP,
                        VAR_USER_SMTP = j.a.VAR_USER_SMTP, 
                        VAR_PWD_SMTP = j.a.VAR_PWD_SMTP,
                        NUM_PORTA_SMTP = j.a.NUM_PORTA_SMTP,
                        CHA_SMTP_SSL = j.a.CHA_SMTP_SSL,
                        CHA_SMTP_STA = j.a.CHA_SMTP_STA,
                        PROVIDER_ID = j.a.PROVIDER_ID,
                        MS_TENANT_ID = j.a.MS_TENANT_ID,
                        MS_CLIENT_ID = j.a.MS_CLIENT_ID,
                        MS_CLIENT_SEC = j.a.MS_CLIENT_SEC
                    })
                    .ToListAsync();

                if(peopleEntities == null || peopleEntities.Count == 0)
                {
                    output = ResetPasswordResult.INVALID_USERID;
                    throw new UserIdNotFoundPi3Exception(request.userId);
                }

                var peopleEntity = peopleEntities.FirstOrDefault(p => !string.IsNullOrEmpty(p.EMAIL_ADDRESS));

                if (peopleEntity == null)
                {
                    output = ResetPasswordResult.INVALID_EMAIL;
                    throw new EmailNotValidPi3Exception(request.userId);
                }

                if(await _dbContext.NetworkAliasesEntities.AsNoTracking()
                    .AnyAsync(n => n.PERSONORGROUP == peopleEntity.SYSTEM_ID && !string.IsNullOrEmpty(n.NETWORK_ID)))
                {
                    output = ResetPasswordResult.DOMAIN_USER;
                    throw new UtenteDiDominioPi3Exception(request.userId);
                }

                var otp = GetRundomString(12);
                var peopleOtpEntity = await _dbContext.PeopleOtpResetPasswordEntities.Where(p => p.USER_ID_PEOPLE.ToUpper().Equals(request.userId)).FirstOrDefaultAsync();
                if (peopleOtpEntity != null)
                    _dbContext.PeopleOtpResetPasswordEntities.Remove(peopleOtpEntity);

                await _dbContext.PeopleOtpResetPasswordEntities.AddAsync(new PeopleOtpResetPasswordEntity()
                {
                    USER_ID_PEOPLE = request.userId.ToUpper(),
                    VAR_EMAIL = peopleEntity.EMAIL_ADDRESS,
                    VAR_OTP = otp,
                    DTA_INSERIMENTO = await _dbContext.GetSystemDateTime()
                });

                await ((DbContext)_dbContext).SaveChangesAsync();

                string pattern = @"(?<=[\w]{2})[\w-\._\+%]*(?=[\w]{2}@)";
                email = System.Text.RegularExpressions.Regex.Replace(peopleEntity.EMAIL_ADDRESS, pattern, m => new string('*', m.Length), RegexOptions.None, TimeSpan.FromSeconds(5));

                var emailFrom = !string.IsNullOrEmpty(peopleEntity.FROM_EMAIL_ADDRESS_USER) ? peopleEntity.FROM_EMAIL_ADDRESS_USER : peopleEntity.FROM_EMAIL_ADDRESS_AMMINISTRA;
                var mailSubject = Resources.ResetPassword;
                var mailBody = string.Format(Resources.MailBody, otp);

                var instructions = new SendEmailInstructions
                {
                    Sender = new EmailSender
                    {
                        Address = emailFrom
                    },
                    To = new List<EmailRecipient>
                    {
                        new EmailRecipient
                        {
                            Address = peopleEntity.EMAIL_ADDRESS,
                            DisplayName = peopleEntity.FULL_NAME
                        }
                    },
                    Subject = new Core.SeedWork.TextValue(mailSubject),
                    Body = new Core.SeedWork.TextValue(mailBody),
                    BodyIsHtml = true
                };
                try
                {
                    StringDictionary arguments = new StringDictionary();
                    //TO DO distinguere per provider
                    arguments.Add("Host", peopleEntity.VAR_SMTP!);
                    arguments.Add("Port", peopleEntity.NUM_PORTA_SMTP!.ToString());
                    arguments.Add("RequireSsl", peopleEntity?.CHA_SMTP_SSL == "1" ? "true" : "false");
                    arguments.Add("UserName", peopleEntity?.VAR_USER_SMTP);
                    arguments.Add("Password", Crypter.Decode(peopleEntity?.VAR_PWD_SMTP, peopleEntity.VAR_USER_SMTP));


                    var provider = await this._dbContext.AssProviderLibEntities
                        .Where(x => x.PROVIDER_ID == peopleEntity.PROVIDER_ID)
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
                                this.LoadChilkatSendEmailConfigurations((ChilkatSendEmailConfiguration)configurations, peopleEntity);
                                break;
                            case GraphSendEmailConfiguration graphSendEmailConfigurations:
                                this.LoadGraphSendEmailConfigurations((GraphSendEmailConfiguration)configurations, peopleEntity);
                                break;
                            default:
                                throw new SendMailPi3Exception(ErrorDescriptions.ProviderNonGestito);
                        }
                    },
                    instructions
                    );
                }
                catch (Exception pi3Ex)
                {
                    this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                    output = ResetPasswordResult.ERROR_SEND_MAIL;
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                output = ResetPasswordResult.KO;
            }

            return new ResetPasswordInviaOTPResult(output, email);
        }

        private void LoadGraphSendEmailConfigurations(GraphSendEmailConfiguration configurations, PeopleObj peopleEntity)
        {
            configurations.TenantId = peopleEntity.MS_TENANT_ID;
            configurations.ClientId = peopleEntity.MS_CLIENT_ID;
            configurations.ClientSecret = peopleEntity.MS_CLIENT_SEC;
            configurations.MailBox = peopleEntity.FROM_EMAIL_ADDRESS_AMMINISTRA;
        }

        private void LoadChilkatSendEmailConfigurations(ChilkatSendEmailConfiguration configurations, PeopleObj peopleEntity)
        {
            configurations.Host = peopleEntity.VAR_SMTP!;
            configurations.Port = peopleEntity.NUM_PORTA_SMTP.HasValue ? Convert.ToInt32(peopleEntity.NUM_PORTA_SMTP) : 0;
            configurations.StartTLS = !string.IsNullOrEmpty(peopleEntity.CHA_SMTP_STA) && peopleEntity.CHA_SMTP_STA == "1";
            configurations.RequireSsl = peopleEntity.CHA_SMTP_SSL == "1";
            configurations.UserName = peopleEntity.VAR_USER_SMTP;
            configurations.Password = Crypter.Decode(peopleEntity.VAR_PWD_SMTP, peopleEntity.VAR_USER_SMTP);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ResetPasswordInviaOTPHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        //protected IEmailSenderService _emailSenderService; 
        protected readonly IFactoryService _factoryService;

        protected string GetRundomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@$?";
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                int index = RandomNumberGenerator.GetInt32(chars.Length);
                result[i] = chars[index];
            }

            return new string(result);
        }

        #endregion
    }

    internal class PeopleObj
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_AMM { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public string FULL_NAME { get; set; }
        public string FROM_EMAIL_ADDRESS_USER { get; set; }
        public string FROM_EMAIL_ADDRESS_AMMINISTRA { get; set; }
        public string VAR_SMTP { get; set; }
        public string VAR_USER_SMTP { get; set; }
        public string VAR_PWD_SMTP { get; set; }
        public long? NUM_PORTA_SMTP { get; set; }
        public string CHA_SMTP_SSL { get; set; }
        public string CHA_SMTP_STA { get; set; }
        public string? PROVIDER_ID { get; internal set; }
        public string? MS_TENANT_ID { get; internal set; }
        public string? MS_CLIENT_ID { get; internal set; }
        public string? MS_CLIENT_SEC { get; internal set; }
    }
}