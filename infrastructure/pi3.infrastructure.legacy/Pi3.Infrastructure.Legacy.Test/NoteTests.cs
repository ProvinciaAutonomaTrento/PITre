// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core;
using Pi3.Core.AggregateModels.NotaAggregate;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
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
    internal class NotaTests
    {
        ServiceProvider _serviceProvider;
        INotaRepository _repository;
        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                // Registra Infrastructure EF
                .AddInfrastructureLegacyEFNotaAggregate()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<INotaRepository>();
        }
        [Test]
        public async Task TestGetNota()
        {
            var idTenant = "361";
            // Nota pubblica
            var aggregate = await this._repository.Get(idTenant, "132600402");

            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null
                && aggregate.TipoAccesso == TipoAccessoNotaEnum.Pubblica
                && string.IsNullOrWhiteSpace(aggregate.IdAccessoRF)
                && aggregate.Autore != null);
            // Nota RF
            aggregate = await this._repository.Get(idTenant, "132592518");
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null
                && aggregate.TipoAccesso == TipoAccessoNotaEnum.RF
                && !string.IsNullOrWhiteSpace(aggregate.IdAccessoRF)
                && aggregate.Autore != null);
            // Nota ruolo
            aggregate = await this._repository.Get(idTenant, "132592598");
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null
                && aggregate.TipoAccesso == TipoAccessoNotaEnum.Ruolo
                && string.IsNullOrWhiteSpace(aggregate.IdAccessoRF)
                && aggregate.Autore != null);
            aggregate = await this._repository.Get(idTenant, "132592599");
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null
                && aggregate.TipoAccesso == TipoAccessoNotaEnum.Personale
                && string.IsNullOrWhiteSpace(aggregate.IdAccessoRF)
                && aggregate.Autore != null);
        }
        [Test]
        public async Task TestAdd()
        {
            var idTenant = "361";
            var nomeNota = new TextValue("Nome nota");
            var idOggetto = "132591188";
            var tipoOggetto = TipiOggettoEnum.Documento;
            var autoreNota = new AutoreNota() { IdUtente = "131978444", IdRuolo = "132003043" };
            // Add nota pubblica
            var aggregate = new Nota(
                            idTenant,
                            DateTime.Now,
                            nomeNota,
                            new TextValue("Testo della nota pubblica"),
                            autoreNota,
                            idOggetto,
                            tipoOggetto,
                            TipoAccessoNotaEnum.Pubblica,
                            null);
            await this._repository.Add(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null
                && aggregate.TipoAccesso == TipoAccessoNotaEnum.Pubblica
                && string.IsNullOrWhiteSpace(aggregate.IdAccessoRF)
                && aggregate.Autore != null);
            // Add nota RF
            aggregate = new Nota(
                            idTenant,
                            DateTime.Now,
                            nomeNota,
                            new TextValue("Testo della nota RF - 1"),
                            autoreNota,
                            idOggetto,
                            tipoOggetto,
                            TipoAccessoNotaEnum.RF,
                            "131976885");
            await this._repository.Add(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null
                && aggregate.TipoAccesso == TipoAccessoNotaEnum.RF
                && !string.IsNullOrWhiteSpace(aggregate.IdAccessoRF)
                && aggregate.Autore != null);
            // Add nota Ruolo
            aggregate = new Nota(
                            idTenant,
                            DateTime.Now,
                            nomeNota,
                            new TextValue("Testo della nota Ruolo - 1"),
                            autoreNota,
                            idOggetto,
                            tipoOggetto,
                            TipoAccessoNotaEnum.Ruolo,
                            null);
            await this._repository.Add(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null
                && aggregate.TipoAccesso == TipoAccessoNotaEnum.Ruolo
                && string.IsNullOrWhiteSpace(aggregate.IdAccessoRF)
                && aggregate.Autore != null);
            // Add nota Personale
            aggregate = new Nota(
                            idTenant,
                            DateTime.Now,
                            nomeNota,
                            new TextValue("Testo della nota Personale"),
                            autoreNota,
                            idOggetto,
                            tipoOggetto,
                            TipoAccessoNotaEnum.Personale,
                            null);
            await this._repository.Add(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null
                && aggregate.TipoAccesso == TipoAccessoNotaEnum.Personale
                && string.IsNullOrWhiteSpace(aggregate.IdAccessoRF)
                && aggregate.Autore != null);
        }
        [Test]
        public async Task TestUpdate()
        {
            var idTenant = "361";
            var nomeNota = new TextValue("Nome nota");
            var idOggetto = "132591188";
            var tipoOggetto = TipiOggettoEnum.Documento;
            var autoreNota = new AutoreNota() { IdUtente = "132024822", IdRuolo = "132024824" };
            // Add nota pubblica
            var aggregate = new Nota(
                            "132592650",
                            idTenant,
                            DateTime.Now,
                            nomeNota,
                            new TextValue("Testo della nota pubblica"),
                            autoreNota,
                            idOggetto,
                            tipoOggetto,
                            TipoAccessoNotaEnum.Pubblica,
                            null);
            await this._repository.Update(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null
                && aggregate.TipoAccesso == TipoAccessoNotaEnum.Pubblica
                && string.IsNullOrWhiteSpace(aggregate.IdAccessoRF)
                && aggregate.Autore != null);
            // Add nota RF
            aggregate = new Nota(
                            "132592597", 
                            idTenant,
                            DateTime.Now,
                            nomeNota,
                            new TextValue("Testo della nota RF"),
                            autoreNota,
                            idOggetto,
                            tipoOggetto,
                            TipoAccessoNotaEnum.RF,
                            "131976885");
            await this._repository.Update(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null
                && aggregate.TipoAccesso == TipoAccessoNotaEnum.RF
                && !string.IsNullOrWhiteSpace(aggregate.IdAccessoRF)
                && aggregate.Autore != null);
            // Add nota Ruolo
            aggregate = new Nota(
                            "132592598",
                            idTenant,
                            DateTime.Now,
                            nomeNota,
                            new TextValue("Testo della nota Ruolo"),
                            autoreNota,
                            idOggetto,
                            tipoOggetto,
                            TipoAccessoNotaEnum.Ruolo,
                            null);
            await this._repository.Update(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null
                && aggregate.TipoAccesso == TipoAccessoNotaEnum.Ruolo
                && string.IsNullOrWhiteSpace(aggregate.IdAccessoRF)
                && aggregate.Autore != null);
            // Add nota Personale
            aggregate = new Nota(
                            "132592599",
                            idTenant,
                            DateTime.Now,
                            nomeNota,
                            new TextValue("Testo della nota Personale"),
                            autoreNota,
                            idOggetto,
                            tipoOggetto,
                            TipoAccessoNotaEnum.Personale,
                            null);
            await this._repository.Update(aggregate);
            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && aggregate.Description != null
                && aggregate.TipoAccesso == TipoAccessoNotaEnum.Personale
                && string.IsNullOrWhiteSpace(aggregate.IdAccessoRF)
                && aggregate.Autore != null);
        }
        [Test]
        public async Task TestDelete()
        {
            var idTenant = "361";
            var aggregate = await this._repository.Get(idTenant, "132591455");
            await this._repository.Delete(aggregate);
            Assert.IsTrue(!aggregate.GetUncommittedChanges().Any());
        }
    }
}
