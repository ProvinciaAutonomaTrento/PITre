// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.File.PAdES;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;
using Pi3.Infrastructure.Chilkat.Services.File.PAdES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Chilkat.Test
{
    [Ignore("Test unitari ignorati temporaneamente")]
    internal class ChilkatPAdESServiceTest
    {
        ServiceProvider _serviceProvider;
        IPAdESService _service;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddDistributedMemoryCache()
                .AddLogging()
                // Registra Infrastructure 
                .AddInfrastructureChilkat(opt =>
                {
                    opt.LicenseKey = "NTTDAT.CB8072026_bGeWUYGn508m";
                    opt.SpoolFolder = @"c:\dev";
                })
                .BuildServiceProvider();

            this._service = this._serviceProvider.GetService<IPAdESService>();
        }

        [Test]
        public async Task IsPAdESFileTest()
        {
            var result = await this._service.IsPAdESFile(new MemoryStream(Resources.SF6_23_NTT_230124_SAL_FATTURAZIONE_aqdm_signed));
            Assert.IsTrue(result);

            result = await this._service.IsPAdESFile(new MemoryStream(Resources.Test_documento));
            Assert.IsFalse(result);

            try
            {
                result = await this._service.IsPAdESFile(new MemoryStream(Resources.errore_spid));
            }
            catch (ChilkatPAdESServicePi3Exception ex)
            {
                Console.WriteLine(ex.LastErrorText);

                Assert.Pass();
            }
            
        }
    }
}
