// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate;
using Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate.ValueObjects;
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
    internal class RagioneTrasmissioneTests
    {
        ServiceProvider _serviceProvider;
        IRagioneTrasmissioneRepository _repository;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFRagioneTrasmissioneAggregate()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IRagioneTrasmissioneRepository>();
        }


        public async Task TestGet()
        {
            var aggregate = await this._repository.Get("361", "7634411");

            Assert.Pass();
        }


        
        public async Task TestAdd()
        {
            var aggregate = new RagioneTrasmissione("361", DateTime.Now, new TextValue("RAGIONE 1"), new TextValue("DESCRIPTION RAGIONE 1"));
            var opzioni = new OpzioniRagioneTrasmissione() { TipoRagione = aggregate.Opzioni.TipoRagione, TipoDestinatario = aggregate.Opzioni.TipoDestinatario, TipoCessioneDiritto = TipiCessioneDirittiEnum.Si };
            aggregate.ChangeOpzioni(opzioni);
            await _repository.Add(aggregate);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(aggregate.Id));
        }

        
        public async Task TestExists()
        {
            bool exists = await this._repository.Exists("361", "7634411");

            Assert.Pass();
        }

        [Test]
        public async Task TestDelete()
        {
            var aggregate = await this._repository.Get("361", "132591066");
            
            await this._repository.Delete(aggregate);

            Assert.Pass();
        }
    }
}
