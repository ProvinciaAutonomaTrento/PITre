// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.OggettoAggregate;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using Pi3.Infrastructure.Legacy.EF.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.SeedWork;
using Pi3.Core;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.OggettoAggregate;
using Pi3.Core.AggregateModels.OggettoAggregate.Repositories;

namespace Pi3.Infrastructure.Legacy.Test
{
    [Ignore("Ignorati temporaneamente")]
    internal class OggettarioTest
    {
        ServiceProvider _serviceProvider;
        IOggettoRepository _repository;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFOggettoAggregate()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IOggettoRepository>();
        }

        [Test]
        public async Task TestGetOggetto()
        {
            var idTenant = "361";

            var aggregate = await this._repository.Get(idTenant, "25309074");

            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && !string.IsNullOrWhiteSpace(aggregate.Registro.Id));

        }

        [Test]
        public async Task TestAdd()
        {
            var idTenant = "361";
            var nomeOggetto = new TextValue("Codice - Test DALILA 2");
            var descrizioneOggetto = new TextValue("DESCRIZIONE - Test Dalila 2");
            var registerId = "86107";

            var aggregate = new Oggetto(idTenant, DateTime.Now, nomeOggetto, registerId ,null, null,  descrizioneOggetto);

            await this._repository.Add(aggregate);

            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && !string.IsNullOrWhiteSpace(aggregate.Registro.Id));

        }

        [Test]
        public async Task TestUpdate()
        {
            var idTenant = "361";

            //Modifica codice

            //GET dell'oggetto e poi update
            var aggregate = await this._repository.Get(idTenant, "132602044");
            aggregate.ChangeName(new TextValue("Codice modificato"));

            await this._repository.Update(aggregate);

            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && !string.IsNullOrWhiteSpace(aggregate.Registro.Id));

            //Modifica descrizione
            var idOggetto = aggregate.Id;

            aggregate = await this._repository.Get(idTenant, idOggetto);
            aggregate.ChangeDescription(new TextValue("Descrizione modificata"));

            await this._repository.Update(aggregate);

            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && !string.IsNullOrWhiteSpace(aggregate.Registro.Id));
        }
    }
}
