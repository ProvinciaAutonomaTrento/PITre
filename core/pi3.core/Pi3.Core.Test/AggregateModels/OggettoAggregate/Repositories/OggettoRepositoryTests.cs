// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.OggettoAggregate.Repositories;
using Pi3.Core.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.OggettoAggregate.Repositories.Tests
{
    [TestFixture()]
    public class OggettoRepositoryTests
    {
        private IServiceProvider _serviceProvider;
        private IElementRepository<Oggetto> _repository;
        private string _id = Guid.NewGuid().ToString();
        private string _idTenant = "IdTenant";

        [OneTimeSetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<Oggetto>, InMemoryElementRepository<Oggetto>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetRequiredService<IElementRepository<Oggetto>>();
        }

        [Test()]
        [Order(1)]
        public async Task AddTest()
        {
            var aggregate = new Oggetto(
                          _id,
                          _idTenant,
                          DateTime.Now,
                          new SeedWork.TextValue("CODICE"),
                          "idRegister",
                          null,
                          null,
                           new SeedWork.TextValue("DESCRIZIONE"));

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }


        [Test()]
        [Order(2)]
        public async Task  ExistsTest()
        {
            var exists = await _repository.Exists(_idTenant, _id);

            Assert.IsTrue(exists);
        }

        [Test()]
        [Order(3)]
        public async Task GetTest()
        {
            var aggregate = await _repository.Get(_idTenant, _id);

            Assert.IsNotNull(aggregate);
        }

        [Test()]
        [Order(4)]
        public async Task LoadTest()
        {
            var aggregate = await _repository.Get(_idTenant, _id);

            await _repository.Load(aggregate, null!);

            Assert.IsNotNull(aggregate);
        }


        [Test()]
        [Order(5)]
        public async Task UpdateTest()
        {
            var aggregate = await _repository.Get(_idTenant, _id);

            aggregate.ChangeDescription(new SeedWork.TextValue("MODIFICA"));

            await _repository.Update(aggregate);

            Assert.Pass();
        }

        [Test()]
        [Order(6)]
        public async Task DeleteTest()
        {
            var aggregate = await _repository.Get(_idTenant, _id);

            await _repository.Delete(aggregate);

            Assert.Pass();
        }
    }
}