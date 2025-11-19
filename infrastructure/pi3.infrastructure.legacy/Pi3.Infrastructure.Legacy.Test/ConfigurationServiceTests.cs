// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.Services;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using Pi3.Infrastructure.Legacy.EF.Services.Configuration;
using Pi3.Infrastructure.Legacy.EF.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.Test
{
    [Ignore("Ignorati temporaneamente")]
    internal class ConfigurationServiceTests
    {
        ServiceProvider _serviceProvider;
        IConfigurationService _service;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                .AddInfrastructureLegacyEFServices()
                .BuildServiceProvider();

            this._service = this._serviceProvider.GetRequiredService<IConfigurationService>();
        }

        [Test]
        public async Task TestGetValue()
        {
            var valueString = await this._service.GetValue<string>("REPOSITORY_ROOT_PATH");

            Assert.True(!string.IsNullOrWhiteSpace(valueString));

            var valueInt = await this._service.GetValue<int>("361", "FE_MAX_LENGTH_NOTE");
            
            Assert.True(valueInt > 0);

            var valueNotExists = await this._service.GetValue<string>("NOT_EXISTS", false, "Non esiste");

            Assert.True(valueNotExists == "Non esiste");            

            valueInt = await this._service.GetValue<int>("361", "FE_ATTIVA_GESTIONE_MULTIMAIL");
            valueInt = await this._service.GetValue<int>("131976804", "FE_ATTIVA_GESTIONE_MULTIMAIL");


        }
    }
}
