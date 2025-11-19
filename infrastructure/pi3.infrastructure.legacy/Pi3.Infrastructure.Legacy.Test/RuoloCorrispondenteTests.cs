// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.RuoloCorrispondenteAggregate;
using Pi3.Core.AggregateModels.RuoloCorrispondenteAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using Pi3.Infrastructure.Legacy.EF.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.Test
{
    [Ignore("Ignorati temporaneamente")]
    internal class RuoloCorrispondenteTests
    {
        ServiceProvider _serviceProvider;
        IRuoloCorrispondenteRepository _repository;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFRuoloCorrispondenteAggregate()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IRuoloCorrispondenteRepository>();
        }


        [Test]
        public async Task TestGet()
        {
            var aggregate = await this._repository.Get("361", "894027");

            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null);
        }


        [Test]
        public async Task TestAdd()
        {
            var aggregate = new RuoloCorrispondente("361", DateTime.Now, new TextValue("PANICIEM_RUOLO_9"), new TextValue("PANICIEM RUOLO"), "86107");

            aggregate.SetCanalePreferenziale("375650");
            aggregate.AddEmail(null, "emanuela.panici@gmail.com", new TextValue("mail principale"));

            await this._repository.Add(aggregate);

            Assert.IsTrue(
               !aggregate.GetUncommittedChanges().Any()
               && !string.IsNullOrWhiteSpace(aggregate.Id)
               && aggregate.CreationDate > DateTime.MinValue
               && aggregate.Name != null
               && aggregate.Description != null);
        }


        [Test]
        public async Task TestExists()
        {
            bool exists = await this._repository.Exists("361", "132602160");

            Assert.Pass();
        }


        [Test]
        public async Task TestDelete()
        {
            var aggregate = await this._repository.Get("361", "132608866");

            await this._repository.Delete(aggregate);

            Assert.Pass();
        }


        [Test]
        public async Task TestUpdate()
        {
            var aggregate = await this._repository.Get("361", "132608351");
            //aggregate.ModifyEmail("599953", "emanuela@gmail.com", new TextValue("Nota_1"));
            //aggregate.AddEmail(null, "emanuela.panici@gmail.com", new TextValue("mail principale"));
            //aggregate.ChangeDescription(new TextValue("Nuova descrizione ruolo"));
            //aggregate.RemoveEmail("599952");
            aggregate.SetCanalePreferenziale("8");

            await this._repository.Update(aggregate);


            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null);
        }
    }
}
