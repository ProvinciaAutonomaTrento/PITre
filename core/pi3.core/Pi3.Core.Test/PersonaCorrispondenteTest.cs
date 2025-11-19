// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class PersonaCorrispondenteTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<PersonaCorrispondente> _repository;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<PersonaCorrispondente>, InMemoryElementRepository<PersonaCorrispondente>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IElementRepository<PersonaCorrispondente>>();
        }

        [Test]
        public async Task AddPersonaCorrispondenteTest()
        {
            var persona = new PersonaCorrispondente(Guid.NewGuid().ToString(), "361", DateTime.Now, new SeedWork.TextValue("PANICIEM_P"), new SeedWork.TextValue("Emanuela Panici"), "Emanuela", "Panici", null);
            persona.AddEmail("1", "emanuela.panic@m.it", new SeedWork.TextValue("mia nota"));
            persona.AddEmail("2", "emanuela.panic2@m.it", new SeedWork.TextValue("mia nota"));
            persona.SetEmailPrincipale("emanuela.panic2@m.it");
            persona.ModifyEmail("1", "emanuela.panic1@m.it", null);
            persona.RemoveEmail("1");
            persona.SetCanalePreferenziale("1");
            persona.SetLuogoNascita("Fer");

            //persona.SetCodiceFiscale("123");

            await _repository.Add(persona);

        }
    }
}
