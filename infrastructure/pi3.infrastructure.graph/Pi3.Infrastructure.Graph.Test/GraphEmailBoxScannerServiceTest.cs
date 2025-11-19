// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Infrastructure.Graph.Services.Email.BoxScanner;
using System;
using System.Collections.Specialized;

namespace Pi3.Infrastructure.Graph.Test
{
    public class GraphEmailBoxScannerServiceTest
    {
        ServiceProvider _serviceProvider;
        IEmailBoxScannerService _service;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
              .AddLogging()
              .AddInfrastructureGraph()
              .BuildServiceProvider();


            this._service = this._serviceProvider.GetRequiredService<IEmailBoxScannerService>();
        }

        [Test]
        [Order(1)]
        public async Task TestScan()
        {
            var arguments = new StringDictionary();
            arguments.Add("ScopeUrl", "https://graph.microsoft.com/.default");
            arguments.Add("TenantId", "f592ff6b-2b62-4c4d-911d-6af2a914f2f7");
            arguments.Add("ClientId", "ed5399b4-6dbb-4310-a866-16828ef55277");
            arguments.Add("ClientSecret", "wkd8Q~bhfKPjxdxrlX9Vz6AIkUVr-IJFSz2~SadG");
            arguments.Add("MailBox", "protocollo@lincei.it");
            arguments.Add("FolderToRead", "da_protocollare");

            await this._service.Scan(
                (configurations) =>
                {
                    ((GraphEmailBoxConfiguration)configurations).TenantId = "f592ff6b-2b62-4c4d-911d-6af2a914f2f7";
                    ((GraphEmailBoxConfiguration)configurations).ClientId = "ed5399b4-6dbb-4310-a866-16828ef55277";
                    ((GraphEmailBoxConfiguration)configurations).ClientSecret = "wkd8Q~bhfKPjxdxrlX9Vz6AIkUVr-IJFSz2~SadG";
                    ((GraphEmailBoxConfiguration)configurations).MailBox = "protocollo@lincei.it";
                    ((GraphEmailBoxConfiguration)configurations).FolderToRead = "da_protocollare";
                },
                ScanCallback
                );

            Assert.Pass();
        }

        [Test]
        [Order(2)]
        public void TestParse()
        {
            Assert.Pass();
        }

        protected bool ScanCallback(EmailBoxScannerCallback callback)
        {
            return true;
        }
    }
}