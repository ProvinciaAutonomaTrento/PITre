// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.MezzoSpedizioneAggregate;
using Pi3.Core.AggregateModels.MezzoSpedizioneAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
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
    internal class MezzoSpedizioneTest
    {
        ServiceProvider _serviceProvider;
        IMezzoSpedizioneRepository _repository;
        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFMezzoSpedizioneAggregate()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IMezzoSpedizioneRepository>();
        }
        [Test]
        public async Task TestGetMezzoSpedizione()
        {
            var idTenant = "361";
            
            // Nota pubblica
            var aggregate = await this._repository.Get(idTenant, "132600417");
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null 
                && aggregate.Description != null);
        }
        [Test]
        public async Task TestAdd()
        {
            var idTenant = "361";
            var descrizione = new TextValue("MEZZO SPED4");
            var tipeId = new TextValue("MEZZO SPED4_ID");
            // Add nota pubblica
            var aggregate = new MezzoSpedizione(
                            idTenant,
                            DateTime.Now,
                            tipeId,
                            descrizione);

            await this._repository.Add(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null 
                && aggregate.Description != null);
        }
        [Test]
        public async Task TestUpdate()
        {
            var idTenant = "361";

            var descrizione = new TextValue("MEZZO DI SPEDIZIONE - 2");
            var tipeId = new TextValue("MEZZO DI SPEDIZIONE - 2");
            // Add nota pubblica
            var aggregate = new MezzoSpedizione(
                            "132600940",
                            idTenant,
                            DateTime.Now,
                            descrizione,
                            tipeId);
            await this._repository.Update(aggregate);

            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null //description
                && aggregate.Description != null);
        }
        [Test]
        public async Task TestDelete()
        {
            var idTenant = "361";
            var aggregate = await this._repository.Get(idTenant, "132600940");

            await this._repository.Delete(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null //description
                && aggregate.Description != null); //typeID
        }
    }
}
