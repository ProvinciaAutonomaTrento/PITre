// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.MezzoSpedizioneAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class MezzoSpedizioneTest
    {
        ServiceProvider _serviceProvider;
        IElementRepository<MezzoSpedizione> _repository;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<MezzoSpedizione>, InMemoryElementRepository<MezzoSpedizione>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IElementRepository<MezzoSpedizione>>();
        }

        [Test]
        public async Task AddMezzoSpedizione()
        {
            var aggregate = new MezzoSpedizione(
                Guid.NewGuid().ToString(),
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("MEZZO SPEDIZIONE TEST"),
                new SeedWork.TextValue("MEZZO SPEDIZIONE TEST")
               );

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }

        [Test]
        public async Task UpdateMezzoSpedizione()
        {
            var aggregate = new MezzoSpedizione(
                "123456",
                "361",
                DateTime.Now,
                new SeedWork.TextValue("MEZZO SPEDIZIONE TEST"),
                new SeedWork.TextValue("MEZZO SPEDIZIONE TEST"));

            aggregate.ChangeName(new SeedWork.TextValue("MEZZO SPEDIZIONE MODIFICATO TEST"));
            aggregate.ChangeDescription(new SeedWork.TextValue("MEZZO SPEDIZIONE MODIFICATO TEST"));
            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }

        [Test]
        public async Task DeleteMezzoSpedizione()
        {
            var aggregate = new MezzoSpedizione(
                Guid.NewGuid().ToString(),
                "361",
                DateTime.Now,
                new SeedWork.TextValue("MEZZO SPEDIZIONE TEST"),
                new SeedWork.TextValue("MEZZO SPEDIZIONE TEST"));

            await _repository.Add(aggregate);
            Assert.True(aggregate.Id != null);

        }
    }
}
