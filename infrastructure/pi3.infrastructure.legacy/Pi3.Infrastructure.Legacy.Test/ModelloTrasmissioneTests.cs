// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
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
    internal class ModelloTrasmissioneTests
    {
        ServiceProvider _serviceProvider;
        IModelloTrasmissioneRepository _repository;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFModelloTrasmissioneAggregate()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IModelloTrasmissioneRepository>();
        }


        [Test]
        public async Task TestGet()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                217419.ToString());
            
            Assert.Pass();
        }

        [Test]
        public async Task TestAdd()
        {
            var aggregate = new ModelloTrasmissione(
                361.ToString(),
                DateTime.Now,
                new Core.SeedWork.TextValue("Modello prova"),
                new Core.SeedWork.TextValue("Note generali"),
                86107.ToString(),
                null,
                null,
                TipiOggettiTrasmessiEnum.DocumentoAmministrativo);

            aggregate.AssignAutorePersona(132442192.ToString());

            aggregate.AddUtenteDestinatario(
                94813.ToString(),
                "PR43079",
                "Bolner",
                "Lorenza",
                96112.ToString(),
                "CONOSCENZA",
                null,
                new Core.SeedWork.TextValue("Note tx singola"),
                10,
                false);

            aggregate.AddUfficioDestinatario(
                1031849.ToString(),
                "U007-7",
                new Core.SeedWork.TextValue("Affari inform./statist., Conto ann. e Dotaz. compless."),
                96111.ToString(),
                "COMPETENZA",
                TipiTrasmissioneSingolaEnum.Tutti,
                new Core.SeedWork.TextValue("Note tx singola"),
                20,
                false);

            aggregate.AddGruppoDestinatario(
                132283504.ToString(),
                "U188ADOC",
                new Core.SeedWork.TextValue("Accesso Documentale"),
                129605426.ToString(),
                "CEDI DIRITTI",
                new CessioneDirittiRagioneTrasmissione()
                {
                    MantieniLettura = true,
                    MantieniScrittura = false
                },
                TipiTrasmissioneSingolaEnum.Uno,
                new List<DatiUtenteNotificato>()
                {
                    new DatiUtenteNotificato()
                    {
                        IdUtente = 89347.ToString()
                    }
                },
                new Core.SeedWork.TextValue("Note trasmissione singola"),
                10,
                false);

            aggregate.CediDiritti(new CessioneDiritti()
            {
                IdDestinatario = 132283504.ToString(),
                IdUtente = 89347.ToString()
            });

            await this._repository.Add(aggregate);
        }


        [Test]
        public async Task TestChangeName()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                217499.ToString());

            aggregate.ChangeName(new Core.SeedWork.TextValue("Nuovo nome"));

            await this._repository.Update(aggregate);

            Assert.Pass();
        }

        [Test]
        public async Task TestChangeNoteGenerali()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                217499.ToString());

            aggregate.ChangeNoteGenerali(new Core.SeedWork.TextValue("Nuove note generali"));

            await this._repository.Update(aggregate);

            Assert.Pass();
        }
    }
}
