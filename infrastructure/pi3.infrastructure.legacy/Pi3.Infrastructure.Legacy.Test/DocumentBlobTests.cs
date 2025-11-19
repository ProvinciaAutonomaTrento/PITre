// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper.Configuration.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
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
    public class DocumentBlobTests
    {
        ServiceProvider _serviceProvider;
        IDocumentBlobRepository _repository;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                .AddInfrastructureLegacyEFServices()
                // Registra Infrastructure per accesso al repository dei documenti su file system
                .AddInfrastructureLegacyDocumentBlobFileSystemRepository()
                .RemoveAll<IConfigurationService>().AddScoped<IConfigurationService, MockConfigurationService>()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IDocumentBlobRepository>();
        }

        [Test]
        public async Task TestGet()
        {
            var id = @$"\\sharespitre-test.dcad.infotn.it\TEST_DOC_PAT\DocServer\PAT_TEST\2015\PAT\S105\ARRIVO\20150914\130009486.pdf";

            var aggregate = await this._repository.Get("361", id);
            
            aggregate.ComputeHash(HashNamesEnum.SHA512);

            Assert.IsTrue(
                !aggregate.GetUncommittedChanges().Any()
                && !string.IsNullOrWhiteSpace(aggregate.Id)
                && !string.IsNullOrWhiteSpace(aggregate.IdTenant)
                && aggregate.CreationDate > DateTime.MinValue
                && aggregate.Name != null
                && !string.IsNullOrEmpty(aggregate.FileName)
                && !string.IsNullOrEmpty(aggregate.ContentType));
        }


        [Test]
        public async Task TestAddUploadFromStream()
        {
            var aggregate = new DocumentBlob(
                "361",
                DateTime.Now,
                new TextValue("Nuovo file"));

            var fileContent = File.ReadAllBytes(@"C:\Users\frezzas\Desktop\eBooks\Uno sguardo a lucene.pdf");

            aggregate.UploadStream(new MemoryStream(fileContent, 0, fileContent.Length), "Uno sguardo a lucene.pdf");
            aggregate.ComputeHash(HashNamesEnum.SHA256);

            await this._repository.Add(aggregate);
            
            Assert.IsTrue(!string.IsNullOrWhiteSpace(aggregate.Id));
        }

        [Test]
        public async Task TestAddUploadFromPath()
        {
            var aggregate = new DocumentBlob(
                "361",
                DateTime.Now,
                new TextValue("Nuovo file"));

            aggregate.UploadFromPath(@"C:\Users\frezzas\Desktop\eBooks\Uno sguardo a lucene.pdf");
            aggregate.ComputeHash(HashNamesEnum.SHA256);

            await this._repository.Add(aggregate);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(aggregate.Id));
        }
    }
}
