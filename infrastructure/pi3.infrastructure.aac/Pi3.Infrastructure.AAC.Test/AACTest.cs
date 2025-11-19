// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Infrastructure.Services.AAC;

namespace Pi3.Infrastructure.AAC.Test
{
    public class AACTest
    {
        ServiceProvider _serviceProvider;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddInfrastructureAAC("https://aac-test.cloud-test.tndigit.it/jwk")
                .BuildServiceProvider();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}