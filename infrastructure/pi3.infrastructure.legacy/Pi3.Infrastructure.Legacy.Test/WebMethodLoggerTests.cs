// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using Pi3.Infrastructure.Legacy.EF.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.Test
{
    [Ignore("Ignorati temporaneamente")]
    public class WebMethodLoggerTests
    {
        ServiceProvider _serviceProvider;
        IWebMethodLoggerService _service;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                .AddInfrastructureLegacyEFServices()
                .BuildServiceProvider();

            this._service = this._serviceProvider.GetRequiredService<IWebMethodLoggerService>();
        }

        [Test]
        public async Task TestLogOK()
        {
            await this._service.LogOK("TRASM_DOC_CEDI_DIRITTI", idTenant: "361");
        }
    }
}
