// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class UOCorrispondenteTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<UOCorrispondente> _repository;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<UOCorrispondente>, InMemoryElementRepository<UOCorrispondente>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IElementRepository<UOCorrispondente>>();
        }


        [Test]
        public async Task AddRuoloCorrispondenteTest()
        {
            var uo = new UOCorrispondente(Guid.NewGuid().ToString(), "361", DateTime.Now, new SeedWork.TextValue("PANICIEM_UO"), new SeedWork.TextValue("Emanuela Panici UO"), null);

            uo.SetCanalePreferenziale("1");

            //persona.SetCodiceFiscale("123");

            await _repository.Add(uo);

        }
    }
}
