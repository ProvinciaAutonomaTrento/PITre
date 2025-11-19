// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.Factory;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    public class Test1EmailBoxScanner : IEmailBoxScannerService
    {
        public string Provider => "TEST1";

        public Task<Email> Parse(Stream stream)
        {
            throw new NotImplementedException();
        }

        public Task Scan(Action<EmailBoxConfigurations> loadEmailBoxConfigurations, Func<EmailBoxScannerCallback, bool> callback)
        {
            throw new NotImplementedException();
        }
    }

    public class Test2EmailBoxScanner : IEmailBoxScannerService
    {
        public string Provider => "TEST2";

        public Task<Email> Parse(Stream stream)
        {
            throw new NotImplementedException();
        }

        public Task Scan(Action<EmailBoxConfigurations> loadEmailBoxConfigurations, Func<EmailBoxScannerCallback, bool> callback)
        {
            throw new NotImplementedException();
        }
    }

    internal class FactoryServiceTest
    {
        IServiceProvider _serviceProvider;
        IFactoryService _factoryService;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddPi3Core()
                .AddScoped<IEmailBoxScannerService, Test1EmailBoxScanner>()
                .AddScoped<IEmailBoxScannerService, Test2EmailBoxScanner>()
                .BuildServiceProvider();

            _factoryService = _serviceProvider.GetRequiredService<IFactoryService>();
        }

        [Test]
        [Order(1)]
        public async Task TryCreateTest()
        {
            var creation = await _factoryService.TryCreate<IEmailBoxScannerService>(s => s.Provider == "TEST1");

            Assert.IsTrue(creation.Success);
        }

        [Test]
        [Order(2)]
        public async Task CreateTest()
        {
            try
            {
                var service = await _factoryService.Create<IEmailBoxScannerService>(s => s.Provider == "TEST1");

                Assert.IsTrue(service != null);
            }
            catch (ServiceNotFoundPi3Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
