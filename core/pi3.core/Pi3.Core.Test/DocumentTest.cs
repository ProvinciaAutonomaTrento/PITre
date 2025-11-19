// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.ContentElementAggregate;
using Pi3.Core.AggregateModels.DocumentAggregate;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ContentElementAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Document = Pi3.Core.AggregateModels.DocumentAggregate.Document;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;

namespace Pi3.Core.Test
{
    public class DocumentTest
    {
        IServiceProvider _serviceProvider;
        IElementRepository<Document> _repository;

        [SetUp]
        public void Setup()
        {
            _serviceProvider = new ServiceCollection()
                .AddScoped<IElementRepository<Document>, InMemoryElementRepository<Document>>()
                .BuildServiceProvider();

            _repository = _serviceProvider.GetService<IElementRepository<Document>>();
        }

        [Test]
        public async Task AddDocumentTest()
        {
            var aggregate = new Document(
                Guid.NewGuid().ToString(),
                "IdTenant",
                DateTime.Now,
                new TextValue(
                    new MultiLanguageTextValue("it", "Nome documento"),
                    new MultiLanguageTextValue("en", "Document name")),
                new TextValue(
                    new MultiLanguageTextValue("it", "Descrizione documento"),
                    new MultiLanguageTextValue("en", "Document description")));

            aggregate.SetOwnerPermission("SF", "Stefano Frezza", 
                ContentElementMemberTypesEnum.User, 
                ContentElementRightTypesEnum.FullControlAllowed);

            aggregate.SetMemberPermission("MR", "Mario Rossi",
                ContentElementMemberTypesEnum.User,
                ContentElementRightTypesEnum.ReadAllowed);

            aggregate.SetMemberPermission("UA1", "Ufficio Amministrazione 1",
                ContentElementMemberTypesEnum.Role,
                ContentElementRightTypesEnum.ReadAllowed);

            aggregate.AddProfile("FA",
                new TextValue(
                    new MultiLanguageTextValue("it", "Fattura"),
                    new MultiLanguageTextValue("en", "Invoice")));

            aggregate.AddProfileField("FA", 
                    "NumeroFattura",
                    new TextValue(
                        new MultiLanguageTextValue("it", "Numero fattura"),
                        new MultiLanguageTextValue("en", "Invoice number")),
                    "Numeric",
                    new ElementFieldSingleValue(140));

            aggregate.AddClassification("001.001",
                                new TextValue(
                                    new MultiLanguageTextValue("it", "Fatturazioni"),
                                    new MultiLanguageTextValue("en", "Billings")));

            aggregate.AddVersion(Guid.NewGuid().ToString(),
                                new TextValue(
                                    new MultiLanguageTextValue("it", "Prima versione"),
                                    new MultiLanguageTextValue("en", "First version")),
                                1, DateTime.Now,
                                new DocumentBlobRef()
                                {
                                    IdBlob = Guid.NewGuid().ToString(),
                                    ContentType = "application/pdf",
                                    FileName = "Fattura num. 140.pdf",
                                    FileSize = 150000
                                });

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }
    }
}
