// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.OggettoAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class OggettoTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<Oggetto> _repository;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<Oggetto>, InMemoryElementRepository<Oggetto>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IElementRepository<Oggetto>>();
        }

        [Test]
        public async Task AddOggettoTest()
        {
            var aggregate = new Oggetto(
                //Guid.NewGuid().ToString(),
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("CODICE"),
                "86107",
                null,
                null,
                 new SeedWork.TextValue("DESCRIZIONE"));

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }

        [Test]
        public async Task UpdateOggettoTest()
        {
            var aggregate = new Oggetto(
                Guid.NewGuid().ToString(),
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("CODICE"),
                "86107",
                null,
                null,
                 new SeedWork.TextValue("DESCRIZIONE"));

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);

            //Update

        }
    }
}
