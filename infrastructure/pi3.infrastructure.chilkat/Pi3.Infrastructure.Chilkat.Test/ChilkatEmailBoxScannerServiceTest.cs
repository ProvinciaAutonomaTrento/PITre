// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Chilkat.Services.Email.BoxScanner;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Chilkat.Test
{
    [Ignore("Test unitari ignorati temporaneamente")]
    public class ChilkatEmailBoxScannerServiceTest
    {
        ServiceProvider _serviceProvider;
        IEmailBoxScannerService _service;

        [SetUp]
        public void SetUp()
        {
            this._serviceProvider = new ServiceCollection()
                .AddDistributedMemoryCache()
                .AddLogging()
                .AddScoped<IClaimsPrincipalService, UnitTestClaimsPrincipalService>()
                .AddInfrastructureChilkat(opt =>
                {
                    opt.LicenseKey = "NTTDAT.CB8072026_bGeWUYGn508m";
                    opt.SpoolFolder = @"c:\dev";
                })
                .BuildServiceProvider();

            this._service = this._serviceProvider.GetRequiredService<IEmailBoxScannerService>();
        }

        [Test]
        [Order(1)]
        public async Task ParseTest()
        {
            var email = await this._service.Parse(new MemoryStream(Resources.EsempioMSG));

            Assert.IsTrue(email != null);

            email = await this._service.Parse(new MemoryStream(Resources._4ba43147_d6d0_42bc_ab95_851_75));

            Assert.IsTrue(email != null && email.AttachmentsAsEmails != null);
        }

        [Test]
        [Order(2)]
        public async Task ScanPOPMailbox()
        {
            await this._service.Scan(
                (configurations) =>
                {   
                    ((ChilkatEmailBoxConfigurations)configurations).EmailBoxTypeEnum = EmailBoxTypeEnum.POP;
                    ((ChilkatEmailBoxConfigurations)configurations).Host = "mbox.cert.legalmail.it";
                    ((ChilkatEmailBoxConfigurations)configurations).Port = 995;
                    ((ChilkatEmailBoxConfigurations)configurations).RequireSsl = true;
                    ((ChilkatEmailBoxConfigurations)configurations).UserName = "KIT00426";
                    ((ChilkatEmailBoxConfigurations)configurations).Password = "WX02OYOD";
                    ((ChilkatEmailBoxConfigurations)configurations).POPBehavior = new EmailBoxPOPBehavior()
                    {
                        DeleteEmailIfProcessed = true
                    };
                    ((ChilkatEmailBoxConfigurations)configurations).IsEmailProcessedByMessageId = (messageId) =>
                    {
                        return messageId == "<A5F7328C.00616908.EC6400E6.994F0D6C.posta-certificata@legalmail.it>" ||
                               messageId == "<A60A8C3B.00617526.EC747E45.F3BF50E4.posta-certificata@legalmail.it>"; ;
                    };
                },
                (callback) =>
                {
                    return true;
                });


            Assert.Pass();
        }

        [Test]
        [Order(3)]
        public async Task ScanIMAPMailbox()
        {
            await this._service.Scan(
                (configurations) =>
                    {
                        ((ChilkatEmailBoxConfigurations)configurations).EmailBoxTypeEnum = EmailBoxTypeEnum.IMAP;
                        ((ChilkatEmailBoxConfigurations)configurations).Host = "mbox.cert.legalmail.it";
                        ((ChilkatEmailBoxConfigurations)configurations).Port = 993;
                        ((ChilkatEmailBoxConfigurations)configurations).RequireSsl = true;
                        ((ChilkatEmailBoxConfigurations)configurations).UserName = "KIT00426";
                        ((ChilkatEmailBoxConfigurations)configurations).Password = "WX02OYOD";
                        ((ChilkatEmailBoxConfigurations)configurations).IMAPBehavior = new EmailBoxIMAPBehavior()
                        {
                            Folders = new EmailBoxFolders()
                            {
                                CurrentFolder = "Inbox",
                                ProcessedFolder = "Pi3Core.Elab",
                                FailedFolder = "Pi3Core.NonElab"
                            }
                        };
                        ((ChilkatEmailBoxConfigurations)configurations).IsEmailProcessedByMessageId = (messageId) =>
                        {
                            return messageId == "<A5F7328C.00616908.EC6400E6.994F0D6C.posta-certificata@legalmail.it>" ||
                                   messageId == "<A60A8C3B.00617526.EC747E45.F3BF50E4.posta-certificata@legalmail.it>"; ;
                        };
                    },
                    (callback) =>
                    {
                        return true;
                    }
                    );

            Assert.Pass();
        }

        protected bool ScanCallback(EmailBoxScannerCallback callback)
        {
            return true;
        }
    }
}
