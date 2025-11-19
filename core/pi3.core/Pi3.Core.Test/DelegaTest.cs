// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.DelegaAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class DelegaTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<Delega> _repository;
        private string _idDelega = "132608866";
        private string _idTenant = "361";

        [OneTimeSetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<Delega>, InMemoryElementRepository<Delega>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetRequiredService<IElementRepository<Delega>>();
        }

        [Test]
        [Order(1)]
        public async Task TestAdd()
        {
            var aggregate = new Delega(_idDelega, _idTenant, DateTime.Now, null, null);

            aggregate.AssignUtenteDelegante("130297936", null, null, null);
            aggregate.AssignGruppoDelegante("1031884", null, null);

            aggregate.AssignUtenteDelegato("14454991", null, null, null);
            aggregate.AssignGruppoDelegato("283971", null, null);

            aggregate.ChangeDataDecorrenza(DateTime.Now.AddDays(10));

            await this._repository.Add(aggregate);
        }

        [Test]
        [Order(2)]
        public async Task TestExists()
        {
            bool exists = await this._repository.Exists(_idTenant, _idDelega);

            Assert.IsTrue(exists);
        }


        [Test]
        [Order(3)]
        public async Task TestGet()
        {
            var aggregate = await this._repository.Get(_idTenant, _idDelega);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(aggregate.Id));
        }

        [Test]
        [Order(4)]
        public async Task TestUpdate()
        {
            var aggregate = await this._repository.Get(_idTenant, _idDelega);

            aggregate.Revoca();
            
            await this._repository.Update(aggregate);

            Assert.Pass();
        }

        [Test]
        [Order(5)]
        public async Task TestDelete()
        {
            var aggregate = await this._repository.Get(_idTenant, _idDelega);

            await this._repository.Delete(aggregate);

            Assert.Pass();
        }
    }
}
