// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.Services;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Chilkat.Services.Email.BoxScanner;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;

namespace Pi3.Infrastructure.Chilkat.Test
{
    [Ignore("Test unitari ignorati temporaneamente")]
    public class ChilkatEmailSenderServiceTest
    {
        ServiceProvider _serviceProvider;
        IEmailSenderService _service;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddDistributedMemoryCache()
                .AddLogging()
                // Registra contesto corrente
                .AddScoped<IClaimsPrincipalService, UnitTestClaimsPrincipalService>()
                // Registra Infrastructure 
                .AddInfrastructureChilkat(opt =>
                {
                    opt.LicenseKey = "NTTDAT.CB8072026_bGeWUYGn508m";
                    opt.SpoolFolder = @"c:\dev";
                })
                .BuildServiceProvider();

            this._service = this._serviceProvider.GetService<IEmailSenderService>();
        }

        [Test]
        public async Task SendEmailTest()
        {
            try
            {
                var arguments = new System.Collections.Specialized.StringDictionary();

                arguments.Add("Host", "smtp.libero.it");
                arguments.Add("Port", "465");
                arguments.Add("RequireSsl", "true");
                arguments.Add("UserName", "stefano.frezza@libero.it");
                arguments.Add("Password", "*********************");

                var result = await this._service.SendEmail(
                (configurations) =>
                    {
                        //configurations = new ChilkatSendEmailConfiguration()
                        //{
                        //    Host = "smtp.libero.it",
                        //    Port = 465,
                        //    RequireSsl = true,
                        //    UserName = "stefano.frezza@libero.it",
                        //    Password = "*****************"
                        //};
                        ((ChilkatSendEmailConfiguration)configurations).Host = "smtp.libero.it";
                        ((ChilkatSendEmailConfiguration)configurations).Port = 465;
                        ((ChilkatSendEmailConfiguration)configurations).RequireSsl = true;
                        ((ChilkatSendEmailConfiguration)configurations).UserName = "stefano.frezza@libero.it";
                        ((ChilkatSendEmailConfiguration)configurations).Password = "*****************";
                    },
                    new SendEmailInstructions()
                    {
                        Sender = new EmailSender()
                        {
                            Address = "stefano.frezza@libero.it",
                            DisplayName = "Stefano Frezza"
                        },
                        To = new List<EmailRecipient>()
                        {
                        new EmailRecipient()
                        {
                            Address = "stefano.frezza@emeal.nttdata.com",
                            DisplayName = "Stefano Frezza"
                        }
                        },
                        Cc = new List<EmailRecipient>()
                        {
                        new EmailRecipient()
                        {
                            Address = "stefano.frezza@emeal.nttdata.com",
                            DisplayName = "Stefano Frezza"
                        }
                        },
                        Bcc = new List<EmailRecipient>()
                        {
                        new EmailRecipient()
                        {
                            Address = "stefano.frezza@emeal.nttdata.com",
                            DisplayName = "Stefano Frezza"
                        }
                        },
                        Subject = new Core.SeedWork.TextValue("Oggetto di test"),
                        Body = new Core.SeedWork.TextValue("Body di test"),
                        BodyIsHtml = false,
                        Attachments = new List<EmailContentAttachment>()
                        {
                        new EmailContentAttachment()
                        {
                            FileName = "Test documento.pdf",
                            Content = Resources.Test_documento,
                            ContentType = "application/pdf"
                        }
                        }
                    });

                Assert.True(!string.IsNullOrWhiteSpace(result.MessageId));
            }
            catch (ChilkatSendEmailPi3Exception cex)
            {
                Assert.Fail(cex.LastErrorText);
            }
        }


    }
}