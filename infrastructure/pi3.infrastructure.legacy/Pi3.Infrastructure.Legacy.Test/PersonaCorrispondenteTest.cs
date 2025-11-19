// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.CorrispondenteAggregate;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.ValueObjects;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate.Repositories;
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
    internal class PersonaCorrispondenteTest
    {
        ServiceProvider _serviceProvider;
        IPersonaCorrispondenteRepository _repository;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFPersonaCorrispondenteAggregate()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IPersonaCorrispondenteRepository>();
        }

        [Test]
        public async Task TestGet()
        {
            var aggregate = await this._repository.Get("361", "132611980");

            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null);
        }

        [Test]
        public async Task TestAdd()
        {
            var aggregate = new PersonaCorrispondente("361", DateTime.Now, new TextValue("PANICIEM_PERSONA_4"), new TextValue("PANICIEM RUOLO"), "Emanuela", "Panici", "86107");

            aggregate.SetCanalePreferenziale("8");
            aggregate.AddEmail(null, "emanuela.panici@gmail.com", new TextValue("mail principale"), false);
            aggregate.SetTitolo("Ing.");

            var indirizzo = new IndirizzoCorrispondente()
            {
                Localita = "Ferentino",
                Citta = "Frentino",
                Provincia = "FR"
            };
            aggregate.ChangeIndirizzo(indirizzo);
            aggregate.SetCodiceFiscale("PNCMNL86T58D539L");

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
            bool exists = await this._repository.Exists("361", "132611980");

            Assert.Pass();
        }


        [Test]
        public async Task TestDelete()
        {
            var aggregate = await this._repository.Get("361", "132611980");

            await this._repository.Delete(aggregate);

            Assert.Pass();
        }


        [Test]
        public async Task TestUpdate()
        {
            var aggregate = await this._repository.Get("361", "132611919");
            aggregate.ChangeNome("Manuela");

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
