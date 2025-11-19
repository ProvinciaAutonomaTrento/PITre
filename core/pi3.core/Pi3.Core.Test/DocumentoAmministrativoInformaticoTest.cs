// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
//using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
//using Pi3.Core.AggregateModels.ElementAggregate;
//using Pi3.Core.SeedWork;
//using Microsoft.EntityFrameworkCore.Metadata.Conventions;
//using System.Reflection;
//using System.Text.Encodings.Web;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using Microsoft.Extensions.DependencyInjection;
//using Pi3.Core.AggregateModels.DocumentBlobAggregate;
//using Pi3.Core.Services.Principal;
//using System.Collections.ObjectModel;
//using AutoMapper;
//using Newtonsoft.Json;
//using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
//using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
//using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
//using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
//using Pi3.Core.Extensions;

//namespace Pi3.Core.Test
//{
//    public class Tests
//    {
//        IServiceProvider _serviceProvider;
//        IDocumentoAmministrativoRepository _repository;

//        [SetUp]
//        public void Setup()
//        {
//            _serviceProvider = new ServiceCollection()
//                .AddLogging()
//                .AddPi3Core()
//                .AddScoped<IClaimsPrincipalService, UnitTestPrincipalService>()
//                .AddTransient(s => s.GetService<IClaimsPrincipalService>().Current)
//                .AddScoped<IDocumentoAmministrativoRepository, DocumentoAmministrativoInMemoryRepository>()
//                .BuildServiceProvider();

//            _repository = _serviceProvider.GetService<IDocumentoAmministrativoRepository>();
//        }

//        [Test]
//        [Order(1)]
//        public async Task AddDocumentoAmministrativoTest()
//        {
//            var aggregate = new DocumentoAmministrativo(
//                Guid.NewGuid().ToString(),
//                "Tenant",
//                DateTime.Now,
//                new OggettoDelDocumento() { Descrizione = new TextValue("Prova") },
//                new DatiRegistro() { IdRegistro = "1", CodiceRegistro = "PAT", DescrizioneRegistro = new TextValue("Registro di Protocollo Provincia Autonoma di Trento") },
//                TipologiaFlussoEnum.U,
//                TipologieVisibilitaEnum.Gerarchica);

//            aggregate.AssignIdDoc(new IdDoc()
//            {
//                Identiticativo = "1111",
//                ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDocumento()
//                {
//                    Impronta = new byte[0],
//                    Algoritmo = "SHA256"
//                },
//                Segnatura = "segnatura"
//            });

//            aggregate.AssignDatiRegistrazioneProtocollo(new DatiRegistrazioneProtocollo()
//            {
//                IdRegistro = "1",
//                DataProtocollazione = DateTime.Now,
//                CodiceRegistro = "PAT",
//                NumeroProtocollo = 112,
//                TipologiaFlusso = TipologiaFlussoEnum.U
//            });
//            aggregate.AssignAutore(new Autore(new PF()
//                        {
//                            Cognome = "stefano",
//                            Nome = "Ddd",
//                            CodiceFiscale = "frzsfn78p24l719e"
//                        }));

//            aggregate.AssignMittente(new Mittente()
//            {
//                PAE = new PAE()
//                {
//                    DenominazioneAmministrazione = new TextValue("amministrazione"),
//                    DenominazioneUfficio = new TextValue("ufficio"),
//                    IndirizziDigitaliDiRiferimento = new List<string>()
//                    {
//                        "test@text.it"
//                    }
//                }
//            });

//            aggregate.AddDestinatario(new Destinatario()
//            {
//                PAE = new PAE()
//                {
//                    DenominazioneAmministrazione = new TextValue("amministrazione"),
//                    DenominazioneUfficio = new TextValue("ufficio"),
//                    IndirizziDigitaliDiRiferimento = new List<string>()
//                    {
//                        "test@text.it"
//                    }
//                }
//            });

//            aggregate.AddAggFascicolo(Guid.NewGuid().ToString(), new TextValue("prova", new MultiLanguageTextValue("de", "asdfafd"), new MultiLanguageTextValue("en", "asdf")));
//            aggregate.AddClassification(Guid.NewGuid().ToString(), new TextValue("1.1"), "1.1");
//            aggregate.AssignClassificationOrAggAsPrincipale(aggregate.Aggregazioni[0]);
//            aggregate.AddAllegato(new Allegato()
//            {
//                IdDoc = new IdDoc()
//                {
//                    Identiticativo = "11111",
//                    ImprontaCrittograficaDelDocumento = new ImprontaCrittograficaDocumento()
//                    {
//                        Impronta = new byte[0],
//                        Algoritmo = "SHA512"
//                    }
//                },
//                Descrizione = new TextValue("allegato1")
//            });

//            aggregate.AddVersion(
//                Guid.NewGuid().ToString(),
//                new TextValue("versione"),
//                1,
//                DateTime.Now,
//                new DocumentBlobRef()
//                {
//                    IdBlob = Guid.NewGuid().ToString(),
//                    CreationDate = DateTime.Now,
//                    FileName = "test.pdf",
//                    FileSize = 2000,
//                    ContentType = "application/pdf",
//                    Hash = new byte[50],
//                    HashName = HashNamesEnum.SHA256,
//                    Cartaceo = true,
//                    TipoFirma = TipoFirmaEnum.TsdElettronica,
//                    SegnaturaPermanente = true
//                });

//            var idProfile = 2365.ToString();

//            aggregate.AddProfile(
//                          idProfile,
//                          new TextValue("Autorizzazione"));

//            aggregate.AddProfileField(
//                idProfile,
//                3270.ToString(),
//                new TextValue("Nome"),
//                "Campo di testo",
//                new ElementFieldSingleValue(new TextValue("Stefano")));

//            aggregate.AddProfileField(
//                idProfile,
//                3271.ToString(),
//                new TextValue("Cognome"),
//                "Campo di testo",
//                new ElementFieldSingleValue(new TextValue("Frezza")));

//            aggregate.AddProfileField(
//                idProfile,
//                3272.ToString(),
//                new TextValue("Matricola"),
//                "Campo di testo",
//                new ElementFieldSingleValue(new TextValue("000000000")));

//            aggregate.AddProfileField(
//                idProfile,
//                3273.ToString(),
//                new TextValue("Contatore1"),
//                "Contatore",
//                new ContatoreRepertorioFieldValue("1", true, true));

//            aggregate.AddProfileField(
//                idProfile,
//                3274.ToString(),
//                new TextValue("Contatore1"),
//                "Contatore",
//                new ContatoreRepertorioFieldValue("1", 33));

//            aggregate.Consolida(new Consolidamento()
//            {
//                Data = DateTime.Now,
//                Stato = StatiConsolidamentoEnum.Livello1,
//                Autore = new Autore(new PF()
//                {
//                    Nome = "Stefano",
//                    Cognome = "Frezza"
//                })
//            });


//            await _repository.Add(aggregate);

//            var xml = aggregate.GetMetadatiXml();

//            Assert.True(aggregate.Id != null);
//        }

//        [Test]
//        [Order(2)]
//        public async Task AddDocumentoAmministrativoAllegatoTest()
//        {
//            var aggregate = new DocumentoAmministrativo(
//                Guid.NewGuid().ToString(),
//                "Tenant",
//                DateTime.Now,
//                new OggettoDelDocumento() { Descrizione = new TextValue("Prova") },
//                null,
//                null,
//                null,
//                new IdDoc() { Identiticativo = "12345" });

//            aggregate.AssignAutore(new Autore(new PF()
//            {
//                Cognome = "stefano",
//                Nome = "Ddd",
//                CodiceFiscale = "frzsfn78p24l719e"
//            }));

//            aggregate.AssignDocumentBlobRef(new DocumentBlobRef()
//            {
//                IdBlob = Guid.NewGuid().ToString(),
//                CreationDate = DateTime.Now,
//                FileName = "test.pdf",
//                FileSize = 2000,
//                ContentType = "application/pdf",
//                Hash = new byte[50],
//                HashName = HashNamesEnum.SHA256
//            }, new TargetVersionBehavior()
//            {
//                CreateNewVersion = true
//            });

//            await _repository.Add(aggregate);

//            Assert.True(aggregate.Id != null);
//        }

//        [Test]
//        [Order(3)]
//        public async Task AddDocumentoAmministrativoEntrataTest()
//        {
//            var aggregate = new DocumentoAmministrativo(
//                Guid.NewGuid().ToString(),
//                "Tenant", 
//                DateTime.Now,
//                new OggettoDelDocumento() { Descrizione = new TextValue("Prova") }, 
//                new DatiRegistro() { IdRegistro = "1", CodiceRegistro = "PAT", DescrizioneRegistro = new TextValue("Registro di Protocollo Provincia Autonoma di Trento") },
//                TipologiaFlussoEnum.E,
//                TipologieVisibilitaEnum.Gerarchica);
            
//            aggregate.AssignAutore(new Autore(new PF()
//            {
//                Cognome = "stefano",
//                Nome = "Ddd",
//                CodiceFiscale = "frzsfn78p24l719e"
//            }));

//            aggregate.AssignMittente(new Mittente(
//                new PAI()
//                {
//                    Amministrazione = new Amministrazione() { Denominazione = new TextValue("test") },
//                    AOO = new Amministrazione() { Denominazione = new TextValue("test") },
//                    UOR = new Amministrazione() { Denominazione = new TextValue("test ufficio") },
//                    IndirizziDigitaliDiRiferimento = new List<string>() { "asdfasdfasdf@fasdf" }
//                }));

//            aggregate.AddMittenteMultiplo(new Mittente(
//                new PAI()
//                {
//                    Amministrazione = new Amministrazione() { Denominazione = new TextValue("test") },
//                    AOO = new Amministrazione() { Denominazione = new TextValue("test") },
//                    UOR = new Amministrazione() { Denominazione = new TextValue("test ufficio") },
//                    IndirizziDigitaliDiRiferimento = new List<string>() { "asdfasdfasdf@fasdf" }
//                }));
            
//            aggregate.AddAggFascicolo(Guid.NewGuid().ToString(), new TextValue("prova", new MultiLanguageTextValue("de", "asdfafd"), new MultiLanguageTextValue("en", "asdf")));
                       
//            aggregate.AssignClassificationOrAggAsPrincipale(aggregate.Aggregazioni[0]);

//            aggregate.AssignDocumentBlobRef(new DocumentBlobRef()
//            {
//                IdBlob = Guid.NewGuid().ToString(),
//                CreationDate = DateTime.Now,
//                FileName = "test.pdf",
//                FileSize = 2000,
//                ContentType = "application/pdf",
//                Hash =  new byte[50],
//                HashName = HashNamesEnum.SHA256
//            }, new TargetVersionBehavior()
//            {
//                CreateNewVersion = true
//            });

//            var idProfile = 2365.ToString();

//            aggregate.AddProfile(
//                          idProfile,
//                          new TextValue("Autorizzazione"));

//            aggregate.AddProfileField(
//                idProfile,
//                3270.ToString(),
//                new TextValue("Nome"),
//                "Campo di testo",
//                new ElementFieldSingleValue(new TextValue("Stefano")));

//            aggregate.AddProfileField(
//                idProfile,
//                3271.ToString(),
//                new TextValue("Cognome"),
//                "Campo di testo",
//                new ElementFieldSingleValue(new TextValue("Frezza")));

//            aggregate.AddProfileField(
//                idProfile,
//                3272.ToString(),
//                new TextValue("Matricola"),
//                "Campo di testo",
//                new ElementFieldSingleValue(new TextValue("000000000")));

//            aggregate.AddProfileField(
//                idProfile,
//                3273.ToString(),
//                new TextValue("Contatore1"),
//                "Contatore",
//                new ContatoreRepertorioFieldValue("1", true, true));

//            aggregate.AddProfileField(
//                idProfile,
//                3274.ToString(),
//                new TextValue("Contatore1"),
//                "Contatore",
//                new ContatoreRepertorioFieldValue("1", 33));

//            aggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione() );
                        
//            aggregate.Consolida(new Consolidamento()
//            {
//                Data = DateTime.Now,
//                Stato = StatiConsolidamentoEnum.Livello1,
//                Autore = new Autore(new PF()
//                {
//                    Nome = "Stefano",
//                    Cognome = "Frezza"
//                })
//            });
            
//            await _repository.Add(aggregate);

//            Assert.True(aggregate.Id != null);
//        }
//    }
//}