// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.DelegaAggregate;
using Pi3.Core.AggregateModels.DelegaAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
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
    internal class DelegaTest
    {
        ServiceProvider _serviceProvider;
        IDelegaRepository _repository;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFDelegaAggregate()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IDelegaRepository>();
        }


        [Test]
        public async Task TestGet()
        {
            var aggregate = await this._repository.Get("361", "132725261");

            Assert.IsTrue(!string.IsNullOrWhiteSpace(aggregate.Id));
        }


        [Test]
        public async Task TestAdd()
        {
            var aggregate = new Delega("361", DateTime.Now, null, null);

            aggregate.AssignUtenteDelegante("130297936", null, null, null);
            aggregate.AssignGruppoDelegante("1031884", null, null);

            aggregate.AssignUtenteDelegato("14454991", null, null, null);
            aggregate.AssignGruppoDelegato("283971", null, null);

            aggregate.ChangeDataDecorrenza("12/10/2023 09:00:00".AsDateTime());
            aggregate.ChangeDataScadenza("30/10/2023 09:00:00".AsDateTime());

            await this._repository.Add(aggregate);

            Assert.IsTrue(!aggregate.GetUncommittedChanges().Any() && !string.IsNullOrWhiteSpace(aggregate.Id));
        }


        [Test]
        public async Task TestExists()
        {
            bool exists = await this._repository.Exists("361", "132725261");

            Assert.Pass();
        }


        [Test]
        public async Task TestDelete()
        {
            var aggregate = await this._repository.Get("361", "132608866");

            await this._repository.Delete(aggregate);

            Assert.Pass();
        }


        [Test]
        public async Task TestUpdate()
        {
            var aggregate = await this._repository.Get("361", "132725261");
            //aggregate.ChangeDataScadenza(DateTime.Now.AddDays(30));
            //aggregate.AssignGruppoDelegante("1031884", null, null);
            //aggregate.ChangeDataDecorrenza(DateTime.Now.AddDays(1));
            aggregate.ChangeDataScadenza(DateTime.Now.AddDays(30));
            aggregate.AssignGruppoDelegato("283971", null, null);
            //aggregate.AssignGruppoDelegante("283949", null, null);
            await this._repository.Update(aggregate);

            Assert.IsTrue(!aggregate.GetUncommittedChanges().Any() && !string.IsNullOrWhiteSpace(aggregate.Id));
        }
    }
}
