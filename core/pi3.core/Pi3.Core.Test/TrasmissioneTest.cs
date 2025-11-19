// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
//using Microsoft.Extensions.DependencyInjection;
//using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
//using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
//using Pi3.Core.AggregateModels.TrasmissioneAggregate;
//using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
//using Pi3.Core.SeedWork;
//using Pi3.Core.Services.Principal;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Pi3.Core.Test
//{
//    internal class TrasmissioneTest
//    {
//        IServiceProvider _serviceProvider;
//        IElementRepository<Trasmissione> _repository;

//        [SetUp]
//        public void Setup()
//        {
//            _serviceProvider = new ServiceCollection()
//                .AddLogging()
//                .AddPi3Core()
//                .AddScoped<IClaimsPrincipalService, UnitTestPrincipalService>()
//                .AddTransient(s => s.GetService<IClaimsPrincipalService>().Current)
//                .AddScoped<IElementRepository<Trasmissione>, InMemoryElementRepository<Trasmissione>>()
//                //.AddScoped<ITrasmissioneRepository, DocumentoAmministrativoInMemoryRepository>()
//                .BuildServiceProvider();

//            _repository = _serviceProvider.GetService<IElementRepository<Trasmissione>>();
//        }

//        [Test]
//        public async Task AddTrasmissioneTest()
//        {
//            var aggregate = new Trasmissione(
//               "Tenant",
//               DateTime.Now,
//               Guid.NewGuid().ToString(),
//               TipiOggettiTrasmessiEnum.DocumentoAmministrativo,
//               new Autore()
//               {
//                   IdUtente = Guid.NewGuid().ToString(),
//                   IdGruppo = Guid.NewGuid().ToString()
//               });

//            aggregate.ChangeNoteGenerali(new TextValue("Nuove note generali"));

//            var idTs = Guid.NewGuid().ToString();
//            var idRagione = Guid.NewGuid().ToString();
//            var idUtente = Guid.NewGuid().ToString();
//            var idUtente2 = Guid.NewGuid().ToString();
//            var idRuolo = Guid.NewGuid().ToString();

//            aggregate.AddTrasmissioneSingolaGruppo(
//                idTs,
//                idRagione,
//                "COMPETENZA",
//                true,
//                TipiTrasmissioneSingolaEnum.Uno,
//                Guid.NewGuid().ToString(),
//                "COMPONENTE",
//                new TextValue("Componente di test"),
//                new TextValue("Note trasmissione singola"),
//                DateTime.Now.AddDays(10),
//                null);

//            var idTu = Guid.NewGuid().ToString();
//            var idTu2 = Guid.NewGuid().ToString();
//            //aggregate.Accetta("", new Accetta() { });
//            aggregate.AddTrasmissioneUtente(
//                idTs,
//                idTu,
//                idUtente,
//                null, null, null, null);
//            aggregate.AddTrasmissioneUtente(
//                idTs,
//                idTu2,
//                idUtente2,
//                null, null, null, null);

//            var idTs2 = Guid.NewGuid().ToString();

//            aggregate.AddTrasmissioneSingolaGruppo(
//               idTs2,
//               idRagione,
//               "COMPETENZA",
//               true,
//               TipiTrasmissioneSingolaEnum.Uno,
//               Guid.NewGuid().ToString(),
//               "COMPONENTE",
//               new TextValue("Componente di test"),
//               new TextValue("Note trasmissione singola"),
//               DateTime.Now.AddDays(10),
//               null);

//            var idTu3 = Guid.NewGuid().ToString();
//            aggregate.AddTrasmissioneUtente(
//                idTs2,
//                idTu3,
//                idUtente,
//                null, null, null, null);

//            aggregate.Accetta(idRuolo,idUtente2, new Accetta()
//            {
//                Data = DateTime.Now,
//                Note = new TextValue("Accettato")
//            });

//            aggregate.Accetta(idRuolo, idUtente, new Accetta()
//            {
//                Data = DateTime.Now,
//                Note = new TextValue("Accettato")
//            });

//            await this._repository.Add(aggregate);
//        }

//        [Test]
//        public async Task PrepareTrasmissioniSingoleTest()
//        {
//            var aggregate = new Trasmissione(
//                "Tenant",
//                DateTime.Now,
//                Guid.NewGuid().ToString(),
//                TipiOggettiTrasmessiEnum.DocumentoAmministrativo,
//                new Autore()
//                {
//                    IdUtente = Guid.NewGuid().ToString(),
//                    IdGruppo = Guid.NewGuid().ToString()
//                });

//            aggregate.PrepareTrasmissioneSingolaUtente(new DatiTrasmissioneSingolaUtente()
//            {
//                IdRagioneTrasmissione = Guid.NewGuid().ToString(),
//                NomeRagioneTrasmissione = "Conoscenza",
//                RagioneConWorkflow = false,
//                IdUtente = Guid.NewGuid().ToString(),
//                UserId = "GIALLIM",
//                Cognome = "Gialli",
//                Nome = "Marco",
//                Note = new TextValue("Trasmissione per conoscenza ad utente")
//            });

//            var idUtenteFrezza = Guid.NewGuid().ToString();
//            aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
//            {
//                IdRagioneTrasmissione = Guid.NewGuid().ToString(),
//                NomeRagioneTrasmissione = "Competenza",
//                RagioneConWorkflow = true,
//                CessioneDirittiRagione = new CessioneDirittiRagione() { MantieniLettura = true, MantieniScrittura = false, ConSceltaUtente = false },
//                IdGruppoDestinatario = Guid.NewGuid().ToString(),
//                CodiceGruppoDestinatario = "CodiceRuolo",
//                DescrizioneGruppoDestinatario = new TextValue("Descrizione ruolo"),
//                Tipo = TipiTrasmissioneSingolaEnum.Tutti,
//                Note = new TextValue("Trasmissione per competenza a tutti"),
//                UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>()
//                {
//                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
//                    {
//                        IdUtente = idUtenteFrezza,
//                        UserId = "FREZZAST",
//                        Cognome = "Frezza",
//                        Nome = "Stefano"
//                    },
//                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
//                    {
//                        IdUtente = Guid.NewGuid().ToString(),
//                        UserId = "ROSSIM",
//                        Cognome = "Rossi",
//                        Nome = "Mario"
//                    },
//                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
//                    {
//                        IdUtente = Guid.NewGuid().ToString(),
//                        UserId = "VERDIG",
//                        Cognome = "Verdi",
//                        Nome = "Giuseppe"
//                    }
//                }
//            });

//            await this._repository.Add(aggregate);
//        }
//    }
//}
