// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using Pi3.Infrastructure.Legacy.EF.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.Test
{
    [Ignore("Ignorati temporaneamente")]
    internal class TrasmissioneTest
    {
        ServiceProvider _serviceProvider;
        ITrasmissioneRepository _repository;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFTrasmissioneAggregate()
                .AddInfrastructureLegacyEFServices()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<ITrasmissioneRepository>();
        }

        [Test]
        public async Task TestGetTrasmissioneDocumento()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                140511426.ToString());

            aggregate.Invia();

            await this._repository.Update(aggregate);

            Assert.Pass();
        }

        [Test]
        public async Task TestGetTrasmissioneFascicolo()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                 132942498.ToString());//132940295.ToString());

            aggregate.Invia();

            await this._repository.Update(aggregate);

            Assert.Pass();
        }

        [Test]
        public async Task TestAddTrasmissioneDocumentoAGruppo()
        {
            var aggregate = new Trasmissione(
              131976804.ToString(),
              DateTime.Now,
              132598699.ToString(),
              TipiOggettiTrasmessiEnum.DocumentoAmministrativo);
            
            aggregate.ChangeNoteGenerali(new TextValue("Nuove note generali"));

            aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
            {
                IdGruppoDestinatario = 132003064.ToString(),
                Tipo = TipiTrasmissioneSingolaEnum.Tutti,
                IdRagioneTrasmissione = 131976805.ToString(),
                Note = new TextValue("Note trasmissione singola"),
                UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>()
                {
                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = 132684596.ToString()
                    }
                }
            });

            await this._repository.Add(aggregate);
        }


        [Test]
        public async Task TestAddTrasmissioneFascicoloAGruppo()
        {
            var aggregate = new Trasmissione(
              361.ToString(),
              DateTime.Now,
              132599331.ToString(),
              TipiOggettiTrasmessiEnum.Fascicolo);
            
            aggregate.ChangeNoteGenerali(new TextValue("Nuove note generali"));

            aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
            {
                IdGruppoDestinatario = 24909132.ToString(),
                Tipo = TipiTrasmissioneSingolaEnum.Tutti,
                IdRagioneTrasmissione = 96111.ToString(),
                Note = new TextValue("Note trasmissione singola"),
                UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>()
                {
                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = 88741.ToString()
                    },
                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = 89023.ToString()
                    },
                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = 24294007.ToString()
                    },
                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = 88553.ToString()
                    }
                }
            });

            await this._repository.Add(aggregate);
        }

        [Test]
        public async Task TestAddEInviaTrasmissioneDocumentoAGruppo()
        {
            var aggregate = new Trasmissione(
              361.ToString(),
              DateTime.Now,
              140511279.ToString(),
              TipiOggettiTrasmessiEnum.DocumentoAmministrativo);

            aggregate.ChangeNoteGenerali(new TextValue("Nuove note generali"));
            
            aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
            {
                IdGruppoDestinatario = 95497.ToString(), // S015SEG
                Tipo = TipiTrasmissioneSingolaEnum.Tutti,
                IdRagioneTrasmissione = 96111.ToString(),
                Note = new TextValue("Note trasmissione singola"),
                UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>()
                {
                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = 566214.ToString()
                    }
                }
            });

            aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
            {
                IdGruppoDestinatario = 131128608.ToString(), // S015CONTABILE
                Tipo = TipiTrasmissioneSingolaEnum.Tutti,
                IdRagioneTrasmissione = 96112.ToString(),
                Note = new TextValue("Note trasmissione singola"),
                UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>()
                {
                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = 131127879.ToString()
                    }
                }
            });

            aggregate.Invia();

            await this._repository.Add(aggregate);
        }

        [Test]
        public async Task TestAddEInviaTrasmissioneDocumentoAGruppoConCessione()
        {
            var aggregate = new Trasmissione(
              361.ToString(),
              DateTime.Now,
              140511145.ToString(),
              TipiOggettiTrasmessiEnum.DocumentoAmministrativo);

            aggregate.ChangeNoteGenerali(new TextValue("Nuove note generali"));

            aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
            {
                IdGruppoDestinatario = 283949.ToString(),
                Tipo = TipiTrasmissioneSingolaEnum.Tutti,
                IdRagioneTrasmissione = 129605426.ToString(),
                CessioneDirittiRagione = new CessioneDirittiRagione { MantieniLettura = true, MantieniScrittura = false, ConSceltaUtente = false},
                Note = new TextValue("Note trasmissione singola"),
                UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>()
                {
                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = 14454991.ToString()
                    }
                }
            });

            aggregate.Invia();

            await this._repository.Add(aggregate);
        }

        [Test]
        public async Task TestAddEInviaTrasmissioneFascicoloAGruppo()
        {
            var aggregate = new Trasmissione(
              361.ToString(),
              DateTime.Now,
              132936715.ToString(),
              TipiOggettiTrasmessiEnum.Fascicolo);

            aggregate.ChangeNoteGenerali(new TextValue("Nuove note generali"));

            aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
            {
                IdGruppoDestinatario = 24909132.ToString(),
                Tipo = TipiTrasmissioneSingolaEnum.Uno,
                IdRagioneTrasmissione = 96111.ToString(),
                Note = new TextValue("Note trasmissione singola"),
                UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>()
                {
                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = 88741.ToString()
                    },
                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = 89023.ToString()
                    },
                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = 24294007.ToString()
                    },
                    new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = 88553.ToString()
                    }
                }
            });

            aggregate.Invia();

            await this._repository.Add(aggregate);
        }

        [Test]
        public async Task TestAccettaTrasmissioneFascicoloAGruppo()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                132754868.ToString());

            aggregate.Accetta(
                "",
                132754879.ToString(),
                new Accetta()
                {
                    Note = new TextValue("Accettazione")
                });

            await this._repository.Update(aggregate);
        }

        [Test]
        public async Task TestRifiutaTrasmissioneFascicoloAGruppo()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                132599636.ToString());

            aggregate.RifiutaTrasmissioneUtente(
                132599638.ToString(),
                new Rifiuta()
                {
                    Note = new TextValue("Rifiuto perché non di mia competenza")
                });

            await this._repository.Update(aggregate);
        }

        [Test]
        public async Task TestAccettaTrasmissioneDocumentoAdUtente()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                132927836.ToString());

            aggregate.Accetta("",
                130297936.ToString(),
                new Accetta()
                {
                    Note = new TextValue("Accettazione")
                });

            await this._repository.Update(aggregate);
        }

        [Test]
        public async Task TestVistoaTrasmissioneDocumentoAdRuolo()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                 132788166.ToString());

            aggregate.Visto("",130297936.ToString(), new Visto()
            { Data = DateTime.Now});

            await this._repository.Update(aggregate);
        }

        [Test]
        public async Task TestRifiutaTrasmissioneDocumentoAdUtente()
        {
            var aggregate = await this._repository.Get(
                131976804.ToString(),
                132599135.ToString());

            aggregate.RifiutaTrasmissioneUtente(
                "132599138",
                new Rifiuta()
                {
                    Note = new TextValue("Rifiuto perché non di mia competenza")
                });

            await this._repository.Update(aggregate);
        }

        [Test]
        public async Task TestAddTrasmissioneDocumentoAdUtente()
        {
            var aggregate = new Trasmissione(
              131976804.ToString(),
              DateTime.Now,
              132599043.ToString(),
              TipiOggettiTrasmessiEnum.DocumentoAmministrativo);

            aggregate.ChangeNoteGenerali(new TextValue("Nuove note generali"));

            aggregate.PrepareTrasmissioneSingolaUtente(new DatiTrasmissioneSingolaUtente()
            {
                IdUtente = 132003027.ToString(),
                IdRagioneTrasmissione = 131976805.ToString(),
                Note = new TextValue("Note trasmissione singola"), 
                DataScadenza = DateTime.Now.AddDays(10)
            });

            await this._repository.Add(aggregate);
        }

        [Test]
        public async Task TestAddEInviaTrasmissioneDocumentoAdUtente()
        {
            var aggregate = new Trasmissione(
              131976804.ToString(),
              DateTime.Now,
              132599034.ToString(),
              TipiOggettiTrasmessiEnum.DocumentoAmministrativo);

            aggregate.ChangeNoteGenerali(new TextValue("Nuove note generali"));

            aggregate.PrepareTrasmissioneSingolaUtente(new DatiTrasmissioneSingolaUtente()
            {
                IdUtente = 132003027.ToString(),
                IdRagioneTrasmissione = 131976805.ToString(),
                Note = new TextValue("Note trasmissione singola"),
                DataScadenza = DateTime.Now.AddDays(10)
            });

            aggregate.Invia();

            await this._repository.Add(aggregate);
        }
    }
}
