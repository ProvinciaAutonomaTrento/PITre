// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaRFAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class NotaRFTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<NotaRF> _repository;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<NotaRF>, InMemoryElementRepository<NotaRF>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IElementRepository<NotaRF>>();
        }


        [Test]
        public async Task CreaNotaRFTest()
        {            

            var aggregate = new NotaRF(
                Guid.NewGuid().ToString(),
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("Testo della nota"),
                new SeedWork.TextValue("Nome della nota"),
                Guid.NewGuid().ToString(),
                "ddsd",
                new SeedWork.TextValue("RF description")
                );

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }

        [Test]
        public async Task ChangeNotaRFTest()
        {
            
            var aggregate = new NotaRF(
                Guid.NewGuid().ToString(),
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("Testo della nota"),
                new SeedWork.TextValue("Nome della nota"),
                Guid.NewGuid().ToString(),
                "ddsd",
                new SeedWork.TextValue("RF description")
                );

            aggregate.ChangeRFNota(Guid.NewGuid().ToString(), "01.020", new SeedWork.TextValue("new RF description"));

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }

        [Test]
        public async Task EditNotaRFTest()
        {
            var aggregate = new NotaRF(
                Guid.NewGuid().ToString(),
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("Testo della nota"),
                new SeedWork.TextValue("Descrizione della nota"),
                Guid.NewGuid().ToString(), 
                "ddsd", 
                new SeedWork.TextValue("RF description")
                );


            //metodo per cambiare nome dell'element
            aggregate.ChangeName(new SeedWork.TextValue("Testo della nota modificato"));

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }

    }
}
