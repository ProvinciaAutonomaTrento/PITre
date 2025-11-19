// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.RuoloCorrispondenteAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class RuoloCorrispondenteTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<RuoloCorrispondente> _repository;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<RuoloCorrispondente>, InMemoryElementRepository<RuoloCorrispondente>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IElementRepository<RuoloCorrispondente>>();
        }


        [Test]
        public async Task AddRuoloCorrispondenteTest()
        {
            var ruolo = new RuoloCorrispondente(Guid.NewGuid().ToString(), "361", DateTime.Now, new SeedWork.TextValue("PANICIEM_R"), new SeedWork.TextValue("Emanuela Panici RUOLO"), null);

            ruolo.SetCanalePreferenzialeMail("1");

            //ruolo.ChangeName(new SeedWork.TextValue("PANICIEM_R1"));
            //persona.SetCodiceFiscale("123");

            await _repository.Add(ruolo);

        }
    }
}
