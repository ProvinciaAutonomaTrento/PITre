// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
//using Microsoft.Extensions.DependencyInjection;
//using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
//using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
//using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
//using Pi3.Core.SeedWork;
//using Pi3.Core.Services.Principal;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Pi3.Core.Test
//{
//    internal class AggregazioneDocumentaleTest
//    {
//        IServiceProvider _serviceProvider;
//        IElementRepository<AggregazioneDocumentale> _repository;

//        [SetUp]
//        public void Setup()
//        {
//            _serviceProvider = new ServiceCollection()
//                .AddLogging()
//                .AddPi3Core()
//                .AddScoped<IClaimsPrincipalService, UnitTestPrincipalService>()
//                .AddTransient(s => s.GetService<IClaimsPrincipalService>().Current)
//                .AddScoped<IElementRepository<AggregazioneDocumentale>, InMemoryElementRepository<AggregazioneDocumentale>>()
//                .BuildServiceProvider();

//            _repository = _serviceProvider.GetService<IElementRepository<AggregazioneDocumentale>>();
//        }

//        [Test]
//        public async Task AddAggregazioneDocumentaleTest()
//        {
//            var aggregate = new AggregazioneDocumentale(
//                361.ToString(),
//                DateTime.Now,
//                new TextValue("Fascicolo di prova 2"),
//                TipiAggregazioneEnum.SerieDiFascicoli,
//                null,
//                TipologieVisibilitaEnum.Gerarchica);

//            aggregate.AddClassification("130277119", new TextValue("Società di promozione turistica e territoriale"));
            
//            aggregate.AddIdDoc(new IdDoc()
//            {
//                Identiticativo = "132676097"
//            });

//            aggregate.CreateFolderHierarchy(
//                new FolderHierarcy()
//                {
//                    Name = new TextValue("SubFolder1"),
//                    IdDocs = (new List<IdDoc>()
//                                {
//                                    new IdDoc()
//                                    {
//                                        Identiticativo = "132722870"
//                                    },
//                                    new IdDoc()
//                                    {
//                                        Identiticativo = "132722902"
//                                    }
//                                }).AsReadOnly(),
//                    Folders = (new List<FolderHierarcy>()
//                    {
//                        new FolderHierarcy()
//                        {
//                            Name = new TextValue("SubFolder1.1"),
//                            Folders = (new List<FolderHierarcy>()
//                            {
//                                new FolderHierarcy()
//                                {
//                                    Name = new TextValue("SubFolder1.1.1"),
//                                    IdDocs = (new List<IdDoc>()
//                                    {
//                                        new IdDoc()
//                                        {
//                                            Identiticativo = "132722902"
//                                        },
//                                        new IdDoc()
//                                        {
//                                            Identiticativo = "132722886"
//                                        }
//                                    }).AsReadOnly()
//                                },
//                                new FolderHierarcy()
//                                {
//                                    Name = new TextValue("SubFolder1.1.2")
//                                }
//                            }).AsReadOnly()
//                        }
//                    }).AsReadOnly()
//                });

//            aggregate.AssignCollocazioneFisica(new CollocazioneFisica()
//            {
//                Id = "348971",
//                Descrizione = new TextValue("test"),
//                Cartaceo = false,
//                DataCollocazione = DateTime.Now,

//            });

//            await _repository.Add(aggregate);

//            Assert.True(aggregate.Id != null);
//        }

//        public async Task Get() {
//            var aggregate = new AggregazioneDocumentale(
//                    "132905739",
//                   361.ToString(),
//                   DateTime.Now,
//                   new TextValue("Fascicolo di prova 2"),
//                   TipiAggregazioneEnum.SerieDiFascicoli,
//                   null,
//                   TipologieVisibilitaEnum.Gerarchica);

//            aggregate.AddFolder("132906010", new TextValue("aaa2"), null, null);
//            aggregate.AddFolder("132906024", new TextValue("aaa"), null, null);
//            aggregate.AddFolder("132906025", new TextValue("aaa"), null, "132906010");
//            aggregate.AddFolder("132906026", new TextValue("aaa"), null, "132906024");
//            //var aggregate1 = await this._repository.Get()

//            //var aggregate = await this._repository.Get(
//            //    361.ToString(),
//            //    132905739.ToString(), 
//            //    new ILoadBehavior[1]
//            //    {
//            //        new GetAggregatoDocumentaleLoadBehavior()
//            //        {
//            //            BypassSecurityCheck = true,
//            //            LoadFolderHierarchy = true,
//            //        }
//            //    }
//            //    );

//            aggregate.RemoveFolder("1329060");
//            //await _repository.Update(aggregate);
//            Assert.Pass();

//        }

//    }
//}
