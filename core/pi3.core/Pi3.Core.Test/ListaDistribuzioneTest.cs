// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.KeywordAggregate;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class ListaDistribuzioneTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<AggregateModels.ListaDistribuzioneAggregate.ListaDistribuzione> _repository;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<ListaDistribuzione>, InMemoryElementRepository<ListaDistribuzione>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IElementRepository<AggregateModels.ListaDistribuzioneAggregate.ListaDistribuzione>>();
        }

        [Test]
        public async Task AddListaDistribuzioneTest()
        {
            var aggregate = new AggregateModels.ListaDistribuzioneAggregate.ListaDistribuzione(
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("ListaDistribuzione_test"),
                new SeedWork.TextValue("Descrizione")
               );

            aggregate.AddDestinatario("123456", TipiDestinatariEnum.Persona, new TextValue("test"), false);
            aggregate.AddDestinatario("123457",  TipiDestinatariEnum.Gruppo, new TextValue("test"), false);
            aggregate.AddDestinatario("123458", TipiDestinatariEnum.Ufficio, new TextValue("test"), false);
            aggregate.RemoveDestinatario("123457");
            aggregate.AssignAutorePersona("123456", "Destinatario", "nome", "cognome");

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null
                && aggregate.Autore != null);
        }
    }
}
