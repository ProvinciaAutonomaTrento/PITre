// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate;
using Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    internal class RagioneTrasmissioneTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<RagioneTrasmissione> _repository;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<RagioneTrasmissione>, InMemoryElementRepository<RagioneTrasmissione>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IElementRepository<RagioneTrasmissione>>();
        }


        [Test]
        public async Task AddRagioneTrasmissioneTest()
        {
            var ragioneTrasmissione = new RagioneTrasmissione(Guid.NewGuid().ToString(), "IdTenant", DateTime.Now, new SeedWork.TextValue("RAGIONE 1"), new SeedWork.TextValue("DESCRIPTION RAGIONE 1"));

            var ragioneTrasmissione2 = new RagioneTrasmissione(Guid.NewGuid().ToString(), "IdTenant", DateTime.Now, new SeedWork.TextValue("RAGIONE 2"), new SeedWork.TextValue("DESCRIPTION RAGIONE 2"));
            ragioneTrasmissione2.ChangeOpzioni(new OpzioniRagioneTrasmissione() { TipoRagione = TipoRagioneTrasmissioneEnum.ConWorkflow, TipoDestinatario = TipiDestinatariTrasmissioneEnum.PariLivello });

            await _repository.Add(ragioneTrasmissione);
            await _repository.Add(ragioneTrasmissione2);

            OpzioniRagioneTrasmissione opzioni = new OpzioniRagioneTrasmissione() { TipoRagione = ragioneTrasmissione2.Opzioni.TipoRagione, TipoDestinatario = ragioneTrasmissione2.Opzioni.TipoDestinatario, TipoCessioneDiritto = TipiCessioneDirittiEnum.Si };
            ragioneTrasmissione2.ChangeOpzioni(opzioni);

            await _repository.Update(ragioneTrasmissione2);

        }

    }
}
