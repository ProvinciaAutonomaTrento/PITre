// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.NotaRFAggregate;
using Pi3.Core.AggregateModels.NotaRFAggregate.Repositories;
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
    internal class NotaRFTest
    {
        ServiceProvider _serviceProvider;
        INotaRFRepository _repository;
        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFNotaRFAggregate()
                .BuildServiceProvider();
            
            this._repository = this._serviceProvider.GetRequiredService<INotaRFRepository>();
        }

        [Test]
        public async Task TestGetNotaRF()
        {
            // Get nota RF  
            var idTenant = "361";
            // Nota pubblica
            var aggregate = await this._repository.Get(idTenant, "17072734");
            Assert.IsTrue(
                aggregate != null &&
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                //&& aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null              
                && aggregate.RF != null
                && !string.IsNullOrWhiteSpace(aggregate.RF.Id));
        }

        [Test]
        public async Task TestAdd()
        {
            var idTenant = "361";
            var nomeNota = new TextValue("Nome nota RF Laura test NotaRFCreatedEvent nuova 4");           
            var idRF = "14994539";
            //var codRF = "RFS120";

            var aggregate = new NotaRF(
                            idTenant,
                            DateTime.Now,
                            nomeNota,
                            null,
                            idRF,
                            //codRF,
                            null,
                            null
                            );
            await this._repository.Add(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                //&& aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.RF != null
                && !string.IsNullOrWhiteSpace(aggregate.Id));                
                     
        }

        [Test]
        public async Task TestDelete()
        {
            var idTenant = "361";
            var aggregate = await this._repository.Get(idTenant, "132608731");
            await this._repository.Delete(aggregate);
            Assert.IsTrue(!aggregate.GetUncommittedChanges().Any());
        }


        [Test]
        public async Task TestUpdate()
        {
            var idTenant = "361";
            var nomeNota = new TextValue("Nome nota RF Laura test NotaRFCreatedEvent modificata con UpdateCompleto Totale");
            var idRF = "14994539";
            var codRF = "RFS120";

            var aggregate = await this._repository.Get(
                idTenant.ToString(),
                132608883.ToString());

            aggregate.ChangeRFNota(idRF, codRF, null);

            aggregate.ChangeName(nomeNota);

            await this._repository.Update(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
              
                && aggregate.Name != null
                && aggregate.RF != null
                && !string.IsNullOrWhiteSpace(aggregate.Id));
        }

        [Test]
        public async Task TestUpdateRF()
        {
            var idTenant = "361";
            var nomeNota = new TextValue("Nome nota RF Laura test NotaRFCreatedEvent nuova con verifica registro");
            //var idRF = "19385848";
            var idRF = "1031582";
            var codRF = "RFD319";

            var aggregate = await this._repository.Get(
                idTenant.ToString(),
                132608883.ToString());

            aggregate.ChangeRFNota(idRF, codRF, null);
          
            await this._repository.Update(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)               
                && aggregate.Name != null
                && aggregate.RF != null
                && !string.IsNullOrWhiteSpace(aggregate.Id));
        }


        [Test]
        public async Task TestUpdateName()
        {
            var idTenant = "361";
            var nomeNota = new TextValue("Nome nota RF Laura test NotaRFCreatedEvent modificata con ElementChangeName");
            //var idRF = "14994539";
            //var codRF = "RFS120";

            var aggregate = await this._repository.Get(
                idTenant.ToString(),
                132608883.ToString());

            aggregate.ChangeName(nomeNota);
            //var result = await this._repository.Update(aggregate);
            await this._repository.Update(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                //&& aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.RF != null
                && !string.IsNullOrWhiteSpace(aggregate.Id));
        }
    }
}
