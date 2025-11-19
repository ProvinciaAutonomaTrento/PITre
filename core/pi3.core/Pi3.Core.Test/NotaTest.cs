// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class NotaTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<Nota> _repository;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<Nota>, InMemoryElementRepository<Nota>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IElementRepository<Nota>>();
        }

        [Test]
        public async Task AddNotaPublicTest()
        {
            var aggregate = new Nota(
                Guid.NewGuid().ToString(),
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("Testo della nota"),
                new SeedWork.TextValue("Nome della nota"),
                new AutoreNota()
                {
                    IdRuolo = Guid.NewGuid().ToString(),
                    IdUtente = Guid.NewGuid().ToString()
                },
                Guid.NewGuid().ToString(),
                TipiOggettoEnum.Documento,
                TipoAccessoNotaEnum.Pubblica,
                null);

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }

        [Test]
        public async Task AddNotaPersonaleTest()
        {
            var aggregate = new Nota(
                Guid.NewGuid().ToString(),
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("Testo della nota"),
                new SeedWork.TextValue("Nome della nota"),
                new AutoreNota()
                {
                    IdRuolo = Guid.NewGuid().ToString(),
                    IdUtente = Guid.NewGuid().ToString()
                },
                Guid.NewGuid().ToString(),
                TipiOggettoEnum.Documento,
                TipoAccessoNotaEnum.Personale,
                null);

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }

        [Test]
        public async Task AddNotaRFTest()
        {
            var aggregate = new Nota(
                Guid.NewGuid().ToString(),
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("Testo della nota"),
                new SeedWork.TextValue("Nome della nota"),
                new AutoreNota()
                {
                    IdRuolo = Guid.NewGuid().ToString(),
                    IdUtente = Guid.NewGuid().ToString()
                },
                Guid.NewGuid().ToString(),
                TipiOggettoEnum.Documento,
                TipoAccessoNotaEnum.RF,
                Guid.NewGuid().ToString());


            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }

        [Test]
        public async Task AddNotaRuoloTest()
        {
            var aggregate = new Nota(
                Guid.NewGuid().ToString(),
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("Testo della nota"),
                new SeedWork.TextValue("Nome della nota"),
                new AutoreNota()
                {
                    IdRuolo = Guid.NewGuid().ToString(),
                    IdUtente = Guid.NewGuid().ToString()
                },
                Guid.NewGuid().ToString(),
                TipiOggettoEnum.Documento,
                TipoAccessoNotaEnum.Personale,
                null);


            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }

        [Test]
        public async Task EditNotaTest()
        {
            var aggregate = new Nota(
                Guid.NewGuid().ToString(),
                "idTenant",
                DateTime.Now,
                new SeedWork.TextValue("Testo della nota"),
                new SeedWork.TextValue("Nome della nota"),
                new AutoreNota()
                {
                    IdRuolo = Guid.NewGuid().ToString(),
                    IdUtente = Guid.NewGuid().ToString()
                },
                Guid.NewGuid().ToString(),
                TipiOggettoEnum.Documento,
                TipoAccessoNotaEnum.RF,
                Guid.NewGuid().ToString());


            //metodo per cambiare nome dell'element
            aggregate.ChangeName(new SeedWork.TextValue("testo della nota modificato"));

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }
    }
}
