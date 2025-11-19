// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.SeedWork;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;

namespace Pi3.Core.Test
{
    internal class DocumentBlobTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<DocumentBlob> _repository;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<DocumentBlob>, InMemoryElementRepository<DocumentBlob>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetRequiredService<IElementRepository<DocumentBlob>>();
        }

        [Test]
        public async Task AddDocumentBlobFromStreamTest()
        {   
            var aggregate = new DocumentBlob(
                Guid.NewGuid().ToString(),
                "idTenant", 
                DateTime.Now,
                new SeedWork.TextValue("Nome documento"), 
                new SeedWork.TextValue("Descrizione documento"));

            using var ms = new MemoryStream(Files.Domain_Driven_Design__Tackling___Eric_Evans_14);

            aggregate.UploadStream(ms, "Test.pdf");
            aggregate.ComputeHash(HashNamesEnum.SHA256);

            var hash = BitConverter.ToString(aggregate.Hash!).Replace("-", string.Empty);

            await _repository.Add(aggregate);

            var expectedHash = "32DCF8115DECDC044CD562CFDD51A7E624B09D127E2BC76D203F611AD7F2FC00";

            Assert.True(aggregate.Id != null 
                && hash == expectedHash);
        }
    }
}
