// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Test.ExampleAggregate;
using Pi3.Core.Test.OrderAggregate.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class OrderAggregateTest
    {
        IServiceProvider _serviceProvider;
        IOrderRepository _repository;
        Order _aggregate;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddPi3Core()
                .AddScoped<IOrderRepository, InMemoryOrderRepository>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IOrderRepository>();
        }

        [Test]
        [Order(1)]
        public async Task AddOrderTest()
        {
            this._aggregate = new Order(Guid.NewGuid().ToString());

            this._aggregate.AddCustomer(
                "MROSSI", 
                "Mario Rossi", 
                new OrderAggregate.ValueObjects.ShipAddress()
                {
                    Address = "Via Roma, 1",
                    City = "Roma",
                    Country = "Italia"
                });

            this._aggregate.AddOrderDetail(
                1,
                new TextValue("Product 1"),
                12,
                2);

            this._aggregate.AddOrderDetail(
                2,
                new TextValue("Product 2"),
                25.5,
                3,
                15);

            await _repository.Add(this._aggregate);

            Assert.True(!this._aggregate.GetUncommittedChanges().Any());
        }

        [Test]
        [Order(2)]
        public async Task ChangeCustomerShipAddressTest()
        {
            this._aggregate.ChangeCustomerShipAddress(new OrderAggregate.ValueObjects.ShipAddress()
            {
                Address = "Via Milano, 1",
                City = "Milano",
                Country = "Italia"
            });

            await _repository.Update(this._aggregate);

            Assert.True(!this._aggregate.GetUncommittedChanges().Any());
        }

        [Test]
        [Order(3)]
        public async Task ChangeOrderDetailQuantityTest()
        {
            var idOrderDetail = this._aggregate.OrderDetails.First().Id;

            this._aggregate.ChangeOrderDetailQuantity(idOrderDetail, 20);

            await _repository.Update(this._aggregate);

            Assert.True(!this._aggregate.GetUncommittedChanges().Any());
        }
    }
}
