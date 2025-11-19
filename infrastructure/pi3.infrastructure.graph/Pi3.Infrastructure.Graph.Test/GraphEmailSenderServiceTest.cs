// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Graph.Services.Email.BoxScanner;
using Pi3.Infrastructure.Graph.Services.Email.Sender;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Graph.Test
{
    [Ignore("Test unitario ignorato temporaneamente")]
    public class GraphEmailSenderServiceTest
    {
        ServiceProvider _serviceProvider;
        IEmailSenderService _service;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
               .AddLogging()
               .AddInfrastructureGraph()
               .BuildServiceProvider();

            this._service = this._serviceProvider.GetRequiredService<IEmailSenderService>();
        }

        [Test]
        public async Task SendEmailTest()
        {
            try
            {
                var arguments = new StringDictionary();
                arguments.Add("TenantId", "36684ab0-e21c-4d20-8c98-a1ff05790c95");
                arguments.Add("ClientId", "bf21eb5c-7d10-479e-a08d-fcf120a8f8d3");
                arguments.Add("ClientSecret", "_948Q~F1hjYjYoEXF2Jc2AcGYIQRBApFNXKQ8a9.");
                arguments.Add("MailBox", "testd17@testd17.onmicrosoft.com");

                var result = await this._service.SendEmail(
                   (configurations) =>
                    {
                        ((GraphSendEmailConfiguration)configurations).TenantId = "36684ab0-e21c-4d20-8c98-a1ff05790c95";
                        ((GraphSendEmailConfiguration)configurations).ClientId = "bf21eb5c-7d10-479e-a08d-fcf120a8f8d3";
                        ((GraphSendEmailConfiguration)configurations).ClientSecret = "_948Q~F1hjYjYoEXF2Jc2AcGYIQRBApFNXKQ8a9.";
                        ((GraphSendEmailConfiguration)configurations).MailBox = "testd17@testd17.onmicrosoft.com";
                    },
                    new SendEmailInstructions()
                    {
                        Sender = new EmailSender()
                        {
                            Address = "testd17@testd17.onmicrosoft.com",
                            DisplayName = "Dalila Di Domenico"
                        },
                        To = new List<EmailRecipient>()
                        {
                        new EmailRecipient()
                        {
                            Address = "dalila.didomenico@emeal.nttdata.com",
                            DisplayName = "Dalila Di Domenico"
                        }
                        },
                        Cc = new List<EmailRecipient>()
                        {
                        new EmailRecipient()
                        {
                            Address = "chiara.catizone@emeal.nttdata.com",
                            DisplayName = "Chiara Catizone"
                        }
                        },
                        Bcc = new List<EmailRecipient>()
                        {
                        new EmailRecipient()
                        {
                            Address = "dalila.didomenico01@gmail.com",
                            DisplayName = "Dalila Di Domenico"
                        }
                        },
                        Subject = new Core.SeedWork.TextValue("Oggetto di test"),
                        Body = new Core.SeedWork.TextValue("Body di test"),
                        BodyIsHtml = true,
                        Attachments = new List<EmailContentAttachment>()
                        {
                        new EmailContentAttachment()
                        {
                            FileName = "Documento di esempio.pdf",
                            Content = Resources.documento_di_esempio,
                            ContentType = "application/pdf"
                        }
                        }
                    });

                Assert.True(!string.IsNullOrWhiteSpace(result.MessageId));
            }
            catch (GraphSendEmailPi3Exception cex)
            {
                Assert.Fail(cex.LastErrorText);
            }
        }
    }
}
