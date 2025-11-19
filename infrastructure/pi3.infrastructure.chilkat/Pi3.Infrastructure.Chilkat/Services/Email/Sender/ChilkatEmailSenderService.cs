// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Chilkat.Services.Email.BoxScanner;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChilkatLib = Chilkat;

namespace Pi3.Infrastructure.Chilkat.Services.Email.Sender
{
    public class ChilkatEmailSenderService : IEmailSenderService
    {
        #region Public Members

        public ChilkatEmailSenderService(
            ILogger<ChilkatEmailSenderService> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IOptions<ChilkatOptions> options)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._options = options;
        }

        public string Provider => "CHILKAT";

        public virtual async Task<EmailSended> SendEmail(
            Action<SendEmailConfigurations> loadSendEmailConfigurations,
            SendEmailInstructions instructions)
        {
            var configurations = new ChilkatSendEmailConfiguration();

            loadSendEmailConfigurations(configurations);

            Validator.ValidateObject(configurations, new ValidationContext(configurations), true);
            Validator.ValidateObject(instructions, new ValidationContext(instructions), true);

            this.AssertArgument(configurations, "Host");
            this.AssertArgument(configurations, "Port");
            this.AssertArgument(configurations, "RequireSsl");

            ChilkatLib.Global global = new ChilkatLib.Global();
            if (!global.UnlockBundle(this._options.Value.LicenseKey))
                throw new ChilkatUnlockPi3Exception();

            EmailSended emailSended = null!;

            using (var mailManager = new ChilkatLib.MailMan())
            {
                mailManager.SmtpHost = configurations.Host;
                mailManager.SmtpPort = Convert.ToInt32(configurations.Port);
                mailManager.SmtpSsl = Convert.ToBoolean(configurations.RequireSsl);
                mailManager.StartTLS = Convert.ToBoolean(configurations.StartTLS);
                mailManager.AutoFix = true;
                mailManager.UncommonOptions = "DisableTls13";
                mailManager.ReadTimeout = 120;

                if (!string.IsNullOrEmpty(configurations.UserName))
                    mailManager.SmtpUsername = configurations.UserName;

                if (!string.IsNullOrEmpty(configurations.Password))
                    mailManager.SmtpPassword = configurations.Password;

                //mailManager.StartTLS = smtpConfigurations.RequireSsl;
                _logger.LogDebug("SendEmail StartTLS: " + mailManager.StartTLS);
                _logger.LogDebug("SendEmail TlsVersion: " + mailManager.TlsVersion);

                var emailMessage = new ChilkatLib.Email()
                { 
                    FromAddress = instructions.Sender.Address,
                    FromName = instructions.Sender.DisplayName! ?? null!
                };

                if (instructions.HighPriority.GetValueOrDefault())
                {
                    emailMessage.RemoveHeaderField("X-Priority");
                    emailMessage.AddHeaderField2("X-Priority", "1");
                }
                
                if(instructions.Headers != null && instructions.Headers.Any())
                    foreach (var header in instructions.Headers)
                        emailMessage.AddHeaderField2(header.Name, header!.Value!); 

                emailSended = new EmailSended()
                {
                    MessageId = emailMessage.GetHeaderField("Message-ID")
                };

                if (instructions.To != null)
                    emailMessage.AddMultipleTo(string.Join(',', instructions.To.Select(t => t.Address)));

                if (instructions.Cc != null)
                    emailMessage.AddMultipleCC(string.Join(',', instructions.Cc.Select(t => t.Address)));

                if (instructions.Bcc != null)
                    emailMessage.AddMultipleBcc(string.Join(',', instructions.Bcc.Select(t => t.Address)));

                if (instructions.Subject != null!)
                {
                    emailMessage.Subject = instructions.Subject.ToString();
                }

                if (instructions.Body != null!)
                {
                    if (instructions.BodyIsHtml)
                        emailMessage.SetHtmlBody(instructions.Body.ToString());
                    else
                        emailMessage.Body = instructions.Body.ToString();
                }

                instructions.Attachments?
                    .Where(a => a.Content != null)
                    .Select(a => a)
                    .ToList()
                    .ForEach(a =>
                {
                    emailMessage.AddDataAttachment2(a.FileName, a.Content, a.ContentType);
                });

                if (!mailManager.SendEmail(emailMessage))
                {
                    this._logger.LogError($"LastErrorText: {mailManager.LastErrorText}");

                    throw new ChilkatSendEmailPi3Exception(mailManager.LastErrorText);
                }
            }
            
            return emailSended;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ChilkatEmailSenderService> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IOptions<ChilkatOptions> _options;

        //protected virtual void AssertArgument(SendEmailConfigurations configurations, string argument)
        //{
        //    if (!configurations.Arguments!.ContainsKey(argument))
        //        throw new ChilkatArgumentNotFoundPi3Exception(argument);
        //}

        protected virtual void AssertArgument(ChilkatSendEmailConfiguration configurations, string argument)
        {
            if(configurations.GetType().GetProperty(argument).GetValue(configurations) == null)
                throw new ChilkatArgumentNotFoundPi3Exception(argument);
        }


        #endregion
    }
}
