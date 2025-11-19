// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.Test.AggregateModels.TrasmissioneAggregate.Repositories
{
    internal class TrasmissioneTests
    {
        ServiceProvider _serviceProvider;
        ITrasmissioneRepository _repository;
        ClaimsPrincipal _claimsPrincipal;

        [OneTimeSetUp]
        public void Setup()
        {
            var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(Pi3.Infrastructure.Legacy.Test.Resources.TrasmissioneTestsVariables);

            variables?.ToList().ForEach(v => Environment.SetEnvironmentVariable(v.Key, v.Value));

            _idTenant = Environment.GetEnvironmentVariable("IdTenant")!;

            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                .AddInfrastructureLegacyEFServices()
                .AddInfrastructureLegacyEFTrasmissioneAggregate()
                .BuildServiceProvider();

            _repository = this._serviceProvider.GetRequiredService<ITrasmissioneRepository>();
            _claimsPrincipal = this._serviceProvider.GetRequiredService<IClaimsPrincipalService>().Current;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
        }

        private string _idTenant = null!;
        private string _idTrasmissione = null!;

        [Test]
        [Order(1)]
        public async Task AddTrasmissioneDocumentoDaAccettareAGruppoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = new Trasmissione(
                    _idTenant,
                    DateTime.Now,
                    Environment.GetEnvironmentVariable("IdDocumentoTrasmesso")!,
                    TipiOggettiTrasmessiEnum.DocumentoAmministrativo);

                aggregate.ChangeNoteGenerali(new TextValue("Note generali della trasmissione da accettare inserite da test unitario"));
                
                aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                {
                    IdGruppoDestinatario = Environment.GetEnvironmentVariable("IdGruppoDestinatario")!,
                    Tipo = TipiTrasmissioneSingolaEnum.Tutti,
                    IdRagioneTrasmissione = Environment.GetEnvironmentVariable("IdRagioneTrasmissioneCompetenza")!, // TODO: qui se si passa una ragione inesistente, il messaggio di errore è fuorviante
                    Note = new TextValue("Note singole della trasmissione inserite da test unitario"),
                    UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>()
                    {
                        new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                        {
                            IdUtente = Environment.GetEnvironmentVariable("IdUtenteNotificato1")!
                        }
                    }
                });

                aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                {
                    IdGruppoDestinatario = _claimsPrincipal.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup)!,
                    Tipo = TipiTrasmissioneSingolaEnum.Tutti,
                    IdRagioneTrasmissione = Environment.GetEnvironmentVariable("IdRagioneTrasmissioneCompetenza")!, // TODO: qui se si passa una ragione inesistente, il messaggio di errore è fuorviante
                    Note = new TextValue("Note singole della trasmissione inserite da test unitario"),
                    UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>()
                    {
                        new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                        {
                            IdUtente = _claimsPrincipal.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser)!
                        }
                    }
                });

                await this._repository.Add(aggregate);

                _idTrasmissione = aggregate.Id;

                Assert.That(aggregate.Id != null);
            });
        }

        [Test]
        [Order(2)]
        public async Task ExistsTrasmissioneDocumentoDaAccettareAGruppoTest()
        {
            await TestRunner.Run(async () =>
            {
                var exists = await _repository.Exists(_idTenant, _idTrasmissione);

                Assert.That(exists);
            });
        }

        [Test]
        [Order(3)]
        public async Task InviaTrasmissioneDocumentoDaAccettareAGruppoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await _repository.Get(_idTenant, _idTrasmissione);

                aggregate.Invia();

                await this._repository.Update(aggregate);

                Assert.That(aggregate.DataInvio.HasValue);
            });
        }

        [Test]
        [Order(4)]
        public async Task AccettaTrasmissioneDocumentoDaAccettareAGruppoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await _repository.Get(_idTenant, _idTrasmissione);

                aggregate.Accetta(
                            _claimsPrincipal.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup)!,
                            _claimsPrincipal.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser)!,
                            new Accetta()
                            {
                                Note = new TextValue("Accettazione tramite test unitario")
                            });

                await this._repository.Update(aggregate);

                Assert.That(aggregate.Id! != null!);
            });
        }

        [Test]
        [Order(5)]
        public async Task InviaTrasmissioneDocumentoDaRifiutareAGruppoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = new Trasmissione(
                    _idTenant,
                    DateTime.Now,
                    Environment.GetEnvironmentVariable("IdDocumentoTrasmesso")!,
                    TipiOggettiTrasmessiEnum.DocumentoAmministrativo);

                aggregate.ChangeNoteGenerali(new TextValue("Note generali della trasmissione da rifiutare inserite da test unitario"));

                aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                {
                    IdGruppoDestinatario = Environment.GetEnvironmentVariable("IdGruppoDestinatario")!,
                    Tipo = TipiTrasmissioneSingolaEnum.Tutti,
                    IdRagioneTrasmissione = Environment.GetEnvironmentVariable("IdRagioneTrasmissioneCompetenza")!, // TODO: qui se si passa una ragione inesistente, il messaggio di errore è fuorviante
                    Note = new TextValue("Note singole della trasmissione inserite da test unitario"),
                    UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>()
                    {
                        new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                        {
                            IdUtente = Environment.GetEnvironmentVariable("IdUtenteNotificato1")!
                        }
                    }
                });

                aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                {
                    IdGruppoDestinatario = _claimsPrincipal.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup)!,
                    Tipo = TipiTrasmissioneSingolaEnum.Tutti,
                    IdRagioneTrasmissione = Environment.GetEnvironmentVariable("IdRagioneTrasmissioneCompetenza")!, // TODO: qui se si passa una ragione inesistente, il messaggio di errore è fuorviante
                    Note = new TextValue("Note singole della trasmissione inserite da test unitario"),
                    UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>()
                    {
                        new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                        {
                            IdUtente = _claimsPrincipal.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser)!
                        }
                    }
                });

                aggregate.Invia();

                await this._repository.Add(aggregate);

                _idTrasmissione = aggregate.Id;

                Assert.That(aggregate.Id != null && aggregate.DataInvio.HasValue);
            });
        }

        [Test]
        [Order(6)]
        public async Task RifiutaTrasmissioneDocumentoDaRifiutareAGruppoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await _repository.Get(_idTenant, _idTrasmissione);

                aggregate.Rifiuta(
                            _claimsPrincipal.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup)!,
                            _claimsPrincipal.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser)!,
                            new Rifiuta()
                            {
                                Note = new TextValue("Rifiutata tramite test unitario")
                            });

                await this._repository.Update(aggregate);

                Assert.That(aggregate.Id! != null!);
            });
        }

        [Test]
        [Order(7)]
        public async Task InviaTrasmissioneDocumentoDaVistareAGruppoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = new Trasmissione(
                    _idTenant,
                    DateTime.Now,
                    Environment.GetEnvironmentVariable("IdDocumentoTrasmesso")!,
                    TipiOggettiTrasmessiEnum.DocumentoAmministrativo);

                aggregate.ChangeNoteGenerali(new TextValue("Note generali della trasmissione da vistare inserite da test unitario"));

                aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                {
                    IdGruppoDestinatario = _claimsPrincipal.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup)!,
                    Tipo = TipiTrasmissioneSingolaEnum.Tutti,
                    IdRagioneTrasmissione = Environment.GetEnvironmentVariable("IdRagioneTrasmissioneConoscenza")!, // TODO: qui se si passa una ragione inesistente, il messaggio di errore è fuorviante
                    Note = new TextValue("Note singole della trasmissione inserite da test unitario"),
                    UtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>()
                    {
                        new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                        {
                            IdUtente = _claimsPrincipal.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser)!
                        }
                    }
                });

                aggregate.Invia();

                await this._repository.Add(aggregate);

                _idTrasmissione = aggregate.Id;

                Assert.That(aggregate.Id != null && aggregate.DataInvio.HasValue);
            });
        }

        [Test]
        [Order(8)]
        public async Task VistaTrasmissioneDocumentoDaVistareAGruppoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await _repository.Get(_idTenant, _idTrasmissione);

                aggregate.Visto(
                            _claimsPrincipal.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup)!,
                            _claimsPrincipal.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser)!,
                            new Visto());

                await this._repository.Update(aggregate);

                Assert.That(aggregate.Id! != null!);
            });
        }

    }
}
