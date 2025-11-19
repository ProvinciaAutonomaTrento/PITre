// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.KeywordAggregate;
using Pi3.Core.AggregateModels.KeywordAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
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
    internal class KeywordTests
    {
        ServiceProvider _serviceProvider;
        IKeywordRepository _repository;
        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFKeywordAggregate()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IKeywordRepository>();
        }
        [Test]
        public async Task TestGetMezzoSpedizione()
        {
            var idTenant = "361";
            // Nota pubblica
            var aggregate = await this._repository.Get(idTenant, "132601272");
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null);
        }

        [Test]
        public async Task TestAdd()
        {
            var idTenant = "361";
            var keyword = new TextValue("");

            // Add nota pubblica
            var aggregate = new Keyword(
                            idTenant,
                            DateTime.Now,
                            keyword,
                            null);

            await this._repository.Add(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null);
        }
    }
}
