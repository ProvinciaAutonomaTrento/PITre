// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate.Repositories;
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
    internal class UOCorrispondenteTest
    {
        ServiceProvider _serviceProvider;
        IUOCorrispondenteRepository _repository;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFUOCorrispondenteAggregate()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IUOCorrispondenteRepository>();
        }


        [Test]
        public async Task TestGet()
        {
            var aggregate = await this._repository.Get("361", "132611983");

            Assert.IsTrue(!string.IsNullOrWhiteSpace(aggregate.Id)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null);
        }


        [Test]
        public async Task TestAdd()
        {
            var aggregate = new UOCorrispondente("361", DateTime.Now, new TextValue("PANICIEM_UO_1"), new TextValue("PANICIEM UO 1"));
            aggregate.SetCanalePreferenziale("9");
            aggregate.AddEmail(null, "emanuela.panici@gmail.com", new TextValue("mail principale"));
            aggregate.AddEmail(null, "emanuela1.panici@gmail.com", new TextValue("mail principale"));

            await this._repository.Add(aggregate);

            Assert.IsTrue(
               !aggregate.GetUncommittedChanges().Any()
               && !string.IsNullOrWhiteSpace(aggregate.Id)
               && aggregate.CreationDate > DateTime.MinValue
               && aggregate.Name != null
               && aggregate.Description != null);
        }


        [Test]
        public async Task TestExists()
        {
            bool exists = await this._repository.Exists("361", "132611983");

            Assert.Pass();
        }


        [Test]
        public async Task TestDelete()
        {
            var aggregate = await this._repository.Get("361", "132611983");

            await this._repository.Delete(aggregate);

            Assert.Pass();
        }


        [Test]
        public async Task TestUpdate()
        {
            var aggregate = await this._repository.Get("361", "132611983");
            aggregate.ChangeDescription(new TextValue("prova modifica descrizione"));
            aggregate.SetCodiceFiscale("PNCMNL86T58D539L");
            aggregate.SetCodiceAOO("PAT");
            aggregate.SetCodiceAmministrazione("PAT_AMM");

            await this._repository.Update(aggregate);


            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null);
        }
    }
}
