// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core;
using Pi3.Core.AggregateModels.DocumentAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
using Pi3.Infrastructure.Legacy.DocumentFSRepository.AggregateModels.DocumentBlobAggregate;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using Pi3.Infrastructure.Legacy.EF.Test;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Pi3.Core.Services.Principal;
using Microsoft.Extensions.Caching.Distributed;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;

namespace Pi3.Infrastructure.Legacy.Test
{
    [Ignore("Ignorati temporaneamente")]
    public class AggregazioneDocumentaleTest
    {
        ServiceProvider _serviceProvider;
        IAggregazioneDocumentaleRepository _repository;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                .AddInfrastructureLegacyEFAggregazioneDocumentaleAggregate()
                .AddInfrastructureLegacyEFServices()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IAggregazioneDocumentaleRepository>();
    }

        [Test]
        public async Task TestGet()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                140640004.ToString(), new ILoadBehavior[1]
                {
                    new GetAggregatoDocumentaleLoadBehavior()
                    {
                        BypassSecurityCheck = true,
                        LoadFolderHierarchy = true,                        
                    }
                });

            Assert.Pass();
        }

        [Test]
        public async Task AddAggregazioneDocumentaleComprensoriTest()
        {
            var aggregate = new AggregazioneDocumentale(
                4885388.ToString(),
                DateTime.Now,
                new TextValue("Fascicolo di prova"),
                TipiAggregazioneEnum.Fascicolo,
                TipologieFascicoloEnum.ProcedimentoAmministrativo,
                TipologieVisibilitaEnum.Gerarchica);

            aggregate.AddClassification("19125085");

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }

        [Test]
        public async Task AddAggregazioneDocumentaleTest()
        {
            var aggregate = new AggregazioneDocumentale(
                361.ToString(),
                DateTime.Now,
                new TextValue("Fascicolo di prova 2"),
                TipiAggregazioneEnum.SerieDocumentale, 
                null, 
                TipologieVisibilitaEnum.Gerarchica);

            //aggregate.AssignSerieDocumentale("56822", new TextValue("Conferenza dei servizi – documentazione organizzativa"));
            aggregate.AddClassification("130277119", new TextValue("Società di promozione turistica e territoriale"), "56822", new TextValue("Conferenza dei servizi – documentazione organizzativa"));
            
            aggregate.AddIdDoc(new IdDoc()
            {
                Identiticativo = "132676097"
            });

            aggregate.CreateFolderHierarchy(
                new FolderHierarcy()
                {
                    Name = new TextValue("SubFolder1"),
                    IdDocs = (new List<IdDoc>()
                                {
                                    new IdDoc()
                                    {
                                        Identiticativo = "132722870"
                                    },
                                    new IdDoc()
                                    {
                                        Identiticativo = "132722902"
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
                                            Identiticativo = "132722902"
                                        },
                                        new IdDoc()
                                        {
                                            Identiticativo = "132722886"
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
                Id = "348971",
                Descrizione = new TextValue("test"),
                Cartaceo = false,
                DataCollocazione = DateTime.Now,

            });

            await _repository.Add(aggregate);

            Assert.True(aggregate.Id != null);
        }


        [Test]
        public async Task CreateFolderHierarchyTest()
        {       
            var idTenant = "361";
            var aggregate = await this._repository.Get(idTenant, "132905738", new ILoadBehavior[1] { new GetAggregatoDocumentaleLoadBehavior() { BypassSecurityCheck = true, LoadFolderHierarchy = true } }); ;

         
            aggregate.CreateFolderHierarchy(
                new FolderHierarcy()
                {
                    IdParentFolder = "132906010",
                    Name = new TextValue("SubFolder di test numero 5"),
                    IdDocs = (new List<IdDoc>()
                    {
                        new IdDoc()
                        {
                            Identiticativo = "132738275"
                        }
                    }).AsReadOnly(),
                    Folders = (new List<FolderHierarcy>()
                    {
                        new FolderHierarcy()
                        {
                            Name = new TextValue("SubFolder2.1 di test numero 2")
                        }
                    }).AsReadOnly()
                });
           
            await _repository.Update(aggregate);

            Assert.True(aggregate.Id != null);
        }

        [Test]
        public async Task TestLoad()
        {
            var aggregate = await this._repository.Get(
               131976804.ToString(),
               132590727.ToString(),
                new ILoadBehavior[1]
                {
                    new GetAggregatoDocumentaleLoadBehavior()
                    { 
                        LoadProfiles = false,
                        LoadProfilesMetadata = false, 
                        LoadFolderHierarchy = false,
                        LoadDocuments = false,                                             
                        BypassSecurityCheck = false,
                        LoadPermissions = false,                  
                        LoadNote = false
                    }
                });

            Assert.IsTrue(
                aggregate.Profiles.Count == 0
                //&& aggregate.Classifications.Count == 0
                //&& aggregate.Allegati.Count == 0
                //&& aggregate.Aggregazioni.Count == 0
                //&& aggregate.Versions.Count == 0
                //&& aggregate.Permissions.Count == 0
                //&& aggregate.Mittente == null
                //&& aggregate.MittentiMultipli.Count == 0
                //&& aggregate.Destinatari.Count == 0
                //&& aggregate.DestinatariCc.Count == 0
                //&& aggregate.Keywords.Count == 0
                //&& aggregate.Note.Count == 0
                );

            await this._repository.Load(aggregate,
                new ILoadBehavior[1]
                {
                    new GetAggregatoDocumentaleLoadBehavior()
                    {
                        LoadFolderHierarchy = true,
                        LoadDocuments = true,
                        LoadNote = true
                    }
                });
        }


        [Test]
        public async Task RemoveFolderTest()
        {
            var idTenant = "361";
            var aggregate = await this._repository.Get(idTenant, "132905739");

            aggregate.RemoveFolder("132906010");

            await _repository.Update(aggregate);

            Assert.True(aggregate.Id != null);
        }


    }

}