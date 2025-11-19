// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Runtime.CompilerServices;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.Test;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories.Tests
{
    public class AggregazioneDocumentaleTests
    {
        ServiceProvider _serviceProvider;
        IElementRepository<AggregazioneDocumentale> _repository;
        
        [OneTimeSetUp]
        public void Setup()
        {
            var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(Pi3.Core.Test.Resources.AggregazioneDocumentaleTestsVariables);

            variables?.ToList().ForEach(v => Environment.SetEnvironmentVariable(v.Key, v.Value));

            _idTenant = Environment.GetEnvironmentVariable("IdTenant")!;

            this._serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddPi3Core()
                .AddScoped<IClaimsPrincipalService, UnitTestClaimsPrincipalService>()
                .AddTransient(s => s.GetRequiredService<IClaimsPrincipalService>().Current)
                .AddScoped<IElementRepository<AggregazioneDocumentale>, InMemoryElementRepository<AggregazioneDocumentale>>()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IElementRepository<AggregazioneDocumentale>>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
        }

        private string _idTenant = null!;
        private string _idAggregazioneDocumentale = null!;

        [Test]
        [Order(1)]
        public async Task AddAggregazioneDocumentaleTest()
        {
            await TestRunner.Run(async () =>
            {   
                var aggregate = new AggregazioneDocumentale(
                       _idTenant,
                       DateTime.Now,
                       new TextValue("Aggregazione documentale creata da test unitario"),
                       TipiAggregazioneEnum.SerieDocumentale,
                       null,
                       TipologieVisibilitaEnum.Gerarchica);

                aggregate.AddClassification(
                        Environment.GetEnvironmentVariable("IdClassification")!,
                        new TextValue(Environment.GetEnvironmentVariable("ClassificationName")!),
                        Environment.GetEnvironmentVariable("IdSerieDocumentale")!,
                        new TextValue(Environment.GetEnvironmentVariable("SerieDocumentaleName")!));

                aggregate.AddIdDoc(new IdDoc()
                {
                    Identiticativo = Environment.GetEnvironmentVariable("IdDoc1")!
                });

                aggregate.CreateFolderHierarchy(
                    new FolderHierarcy()
                    {
                        Name = new TextValue("SubFolder1"),
                        IdDocs = (new List<IdDoc>()
                                    {
                                    new IdDoc()
                                    {
                                        Identiticativo = Environment.GetEnvironmentVariable("IdDoc2")!
                                    },
                                    new IdDoc()
                                    {
                                        Identiticativo = Environment.GetEnvironmentVariable("IdDoc3")!
                                    }
                                    }).AsReadOnly(),
                        Folders = (new List<FolderHierarcy>()
                        {
                        new FolderHierarcy()
                        {
                            Name = new TextValue("SubFolder1.1"),
                            Folders = (new List<FolderHierarcy>()
                            {
                                new FolderHierarcy()
                                {
                                    Name = new TextValue("SubFolder1.1.1"),
                                    IdDocs = (new List<IdDoc>()
                                    {
                                        new IdDoc()
                                        {
                                            Identiticativo = Environment.GetEnvironmentVariable("IdDoc4")!
                                        },
                                        new IdDoc()
                                        {
                                            Identiticativo = Environment.GetEnvironmentVariable("IdDoc5")!
                                        }
                                    }).AsReadOnly()
                                },
                                new FolderHierarcy()
                                {
                                    Name = new TextValue("SubFolder1.1.2")
                                }
                            }).AsReadOnly()
                        }
                        }).AsReadOnly()
                    });

                aggregate.AssignCollocazioneFisica(new CollocazioneFisica()
                {
                    Id = Environment.GetEnvironmentVariable("IdCollocazioneFisica")!,
                    Cartaceo = false,
                    DataCollocazione = DateTime.Now,
                });

                await _repository.Add(aggregate);

                _idAggregazioneDocumentale = aggregate.Id;

                Assert.That(aggregate.Id != null);
            });
        }

        [Test]
        [Order(2)]
        public async Task ChiudiAggregazioneDocumentaleTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await this.GetAggregazioneDocumentale(_idAggregazioneDocumentale);

                aggregate.Chiudi();

                await _repository.Update(aggregate);

                Assert.That(aggregate.DataChiusura.HasValue);
            });
        }

        [Test]
        [Order(3)]
        public async Task ApriAggregazioneDocumentaleTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await this.GetAggregazioneDocumentale(_idAggregazioneDocumentale); 

                aggregate.Apri();

                await _repository.Update(aggregate);

                Assert.That(!aggregate.DataChiusura.HasValue);
            });
        }

        [Test]
        [Order(4)]
        public async Task AssignDataScadenzaTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await this.GetAggregazioneDocumentale(_idAggregazioneDocumentale);  

                aggregate.AssignDataScadenza(DateTime.Now.AddDays(1));
                
                await _repository.Update(aggregate);

                Assert.That(aggregate.DataScadenza.HasValue);
            });
        }

        [Test]
        [Order(5)]
        public async Task ChangeDescrizioneTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await this.GetAggregazioneDocumentale(_idAggregazioneDocumentale); 

                aggregate.ChangeDescription(new TextValue("Aggregazione documentale modificata da test unitario"));

                await _repository.Update(aggregate);

                Assert.That(aggregate.Description!.Value == "Aggregazione documentale modificata da test unitario");
            });
        }


        [Test]
        [Order(6)]
        public async Task AssignIdAggPrimarioTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await this.GetAggregazioneDocumentale(_idAggregazioneDocumentale);

                aggregate.AssignIdAggPrimario(new IdAgg() { IdAggregazione = Environment.GetEnvironmentVariable("IdAggPrimario")!, TipoAggregazione = TipiAggregazioneEnum.SerieDocumentale });

                await _repository.Update(aggregate);

                Assert.That(aggregate.IdAggPrimario! != null!);
            });
        }

        [Test]
        [Order(7)]
        public async Task RemoveIdDocTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await this.GetAggregazioneDocumentale(_idAggregazioneDocumentale);

                var docs = aggregate.IdDocs.Count;

                aggregate.RemoveIdDoc(aggregate.IdDocs[0]);

                await _repository.Update(aggregate);

                Assert.That(docs - aggregate.IdDocs.Count == 1);
            });
        }


        private async Task<AggregazioneDocumentale> GetAggregazioneDocumentale(string id, bool bypassSecurityCheck = false)
        {
            return await _repository.Get(_idTenant, id);
        }
    }
}