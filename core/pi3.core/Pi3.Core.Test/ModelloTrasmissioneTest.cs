// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
//using Microsoft.Extensions.DependencyInjection;
//using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
//using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate;
//using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects;
//using Pi3.Core.AggregateModels.TrasmissioneAggregate;
//using Pi3.Core.SeedWork;
//using Pi3.Core.Services.Principal;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Pi3.Core.Test
//{
//    internal class ModelloTrasmissioneTest
//    {
//        IServiceProvider _serviceProvider;
//        IElementRepository<ModelloTrasmissione> _repository;

//        [OneTimeSetUp]
//        public void Setup()
//        {
//            _serviceProvider = new ServiceCollection()
//                .AddLogging()
//                .AddPi3Core()
//                .AddScoped<IClaimsPrincipalService, UnitTestPrincipalService>()
//                .AddTransient(s => s.GetService<IClaimsPrincipalService>().Current)
//                .AddScoped<IElementRepository<ModelloTrasmissione>, InMemoryElementRepository<ModelloTrasmissione>>()
//                .BuildServiceProvider();

//            _repository = _serviceProvider.GetService<IElementRepository<ModelloTrasmissione>>();
//        }

//        [Test]
//        [Order(1)]
//        public async Task TestAdd()
//        {
//            var aggregate = new ModelloTrasmissione(
//                Guid.NewGuid().ToString(),
//                Guid.NewGuid().ToString(),
//                DateTime.Now,
//                new TextValue("Modello 1"),
//                new TextValue("Note generali"),
//                Guid.NewGuid().ToString(),
//                null,
//                null,
//                TipiOggettiTrasmessiEnum.DocumentoAmministrativo);

//            aggregate.AssignAutorePersona("idAutorePersona");
//            aggregate.AssignAutoreGruppo("idAutoreGruppo");

//            var idGruppoDestinatario = Guid.NewGuid().ToString();
//            var idUtenteNotificato = Guid.NewGuid().ToString();

//            aggregate.AddGruppoDestinatario(
//                idGruppoDestinatario,
//                null,
//                null,
//                Guid.NewGuid().ToString(),
//                null,
//                new CessioneDirittiRagioneTrasmissione() { MantieniLettura = true, MantieniScrittura = false },
//                TipiTrasmissioneSingolaEnum.Uno,
//                new List<DatiUtenteNotificato>()
//                {
//                    new DatiUtenteNotificato()
//                    {
//                        IdUtente = idUtenteNotificato
//                    }
//                },
//                new TextValue("Note tx singola"),
//                10,
//                null);

//            aggregate.CediDiritti(new CessioneDiritti() { IdDestinatario = idGruppoDestinatario, IdUtente = idUtenteNotificato });

//            await this._repository.Add(aggregate);
//        }
//    }
//}
