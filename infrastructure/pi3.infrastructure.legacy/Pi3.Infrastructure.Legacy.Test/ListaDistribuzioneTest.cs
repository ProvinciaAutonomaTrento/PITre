// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Repositories;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
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
    internal class ListaDistribuzioneTest
    {
        ServiceProvider _serviceProvider;
        IListaDistribuzioneRepository _repository;
        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFListaDistribuzioneAggregate()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IListaDistribuzioneRepository>();
        }
        [Test]
        public async Task TestGet()
        {
            var idTenant = "361";
            // Nota pubblica
            var aggregate = await this._repository.Get(idTenant, "176042");
            Assert.IsTrue(
                /* !string.IsNullOrWhiteSpace(aggregate.Id)
                &&*/ !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue);

        }
        [Test]
        public async Task TestAdd()
        {
            var idTenant = "361";
            var nomeLista = "TEST_SIMO_2";
            var descrizioneLista = "TEST_SIMO_2_DESCRIZIONE";

            var aggregate = new ListaDistribuzione(idTenant, DateTime.Now, new TextValue(nomeLista), new TextValue(descrizioneLista));

            aggregate.AssignAutoreRuolo("1031876", "D319DG", new TextValue("Dirigente Generale Dip. Organizzazione Personale e Affari Generali"));

            aggregate.AddDestinatario("88003", TipiDestinatariEnum.Persona, new TextValue("Fedrigotti Silvio"), false);

            await this._repository.Add(aggregate);

            Assert.IsTrue(
               !aggregate.GetUncommittedChanges().Any()
               && !string.IsNullOrWhiteSpace(aggregate.Id)
               && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
               && aggregate.CreationDate > DateTime.MinValue
               && aggregate.Name != null
               && aggregate.Description != null
               && aggregate.Autore != null
               && aggregate.Destinatari.Count > 0);
        }

        [Test]
        public async Task TestDelete()
        {
            var idTenant = "361";
            var aggregate = await this._repository.Get(idTenant, "132609189");
            await this._repository.Delete(aggregate);

        }
    }
}
