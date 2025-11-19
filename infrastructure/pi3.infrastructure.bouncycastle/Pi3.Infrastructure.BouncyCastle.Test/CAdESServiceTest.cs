// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.TextExtractors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.BouncyCastle.Test
{
    internal class CAdESServiceTest
    {
        ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddInfrastructureBouncyCastle()
                .BuildServiceProvider();
        }


        [Test]
        public async Task BouncyCastleCAdESServiceTest()
        {
            var service = _serviceProvider.GetRequiredService<ICAdESService>();

            using var signedFile = new MemoryStream(InputFiles.Test1_pdf);
            using var originalFile = new MemoryStream();

            await service.LoadOriginalFile("", signedFile, originalFile);
        }

    }
}
