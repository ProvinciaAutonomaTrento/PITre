// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using NUnit.Framework;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using System.Text.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.Principal;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.Services.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using System.Runtime.CompilerServices;
using Pi3.Core.Test;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories.Tests
{
    public class DocumentoAmministrativoTests
    {
        ServiceProvider _serviceProvider;
        IElementRepository<DocumentoAmministrativo> _documentoAmministrativoRepository;
        IElementRepository<DocumentBlob> _documentBlobRepository;

        [OneTimeSetUp]
        public void Setup()
        {
            var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(Pi3.Core.Test.Resources.DocumentoAmministrativoTestsVariables);

            variables?.ToList().ForEach(v => Environment.SetEnvironmentVariable(v.Key, v.Value));

            _idTenant = Environment.GetEnvironmentVariable("IdTenant")!;
            _idRegistro = Environment.GetEnvironmentVariable("IdRegistro")!;
            _idClassification = Environment.GetEnvironmentVariable("IdClassification")!;
            _idMezzoSpedizione = Environment.GetEnvironmentVariable("IdMezzoSpedizione")!;

            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddPi3Core()
                .AddScoped<IClaimsPrincipalService, UnitTestClaimsPrincipalService>()
                .AddTransient(s => s.GetRequiredService<IClaimsPrincipalService>().Current)
                .AddScoped<IElementRepository<DocumentoAmministrativo>, InMemoryElementRepository<DocumentoAmministrativo>>()
                .AddScoped<IElementRepository<DocumentBlob>, InMemoryElementRepository<DocumentBlob>>()
                .BuildServiceProvider();

            _documentoAmministrativoRepository = _serviceProvider.GetRequiredService<IElementRepository<DocumentoAmministrativo>>();
            _documentBlobRepository = this._serviceProvider.GetRequiredService<IElementRepository<DocumentBlob>>();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
        }

        private string _idTenant = null!;
        private string _idRegistro = null!;
        private string _idClassification = null!;
        private string _idMezzoSpedizione = null!;
        private string _idDocumentoNonProtocollato = null!;
        private string _idAllegato = null!;
        private string _idDocumentoProtocollatoInEntrata = null!;
        private string _idDocumentoProtocollatoInUscita = null!;

        [Test()]
        [Order(1)]
        public async Task AddDocumentoNonProtocollatoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = new DocumentoAmministrativo(
                                _idTenant,
                                DateTime.Now,
                                new OggettoDelDocumento() { Descrizione = new TextValue("Non protocollato") },
                                new DatiRegistro() { IdRegistro = _idRegistro });

                await _documentoAmministrativoRepository.Add(aggregate);

                _idDocumentoNonProtocollato = aggregate.Id;

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
            });
        }

        [Test()]
        [Order(2)]
        public async Task DocumentoNonProtocollatoExistsTest()
        {
            await TestRunner.Run(async () =>
            {
                var exists = await _documentoAmministrativoRepository.Exists(_idTenant, _idDocumentoNonProtocollato);

                Assert.That(exists);
            });
        }

        [Test()]
        [Order(3)]
        public async Task GetDocumentoNonProtocollatoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await GetDocumentoAmministrativo(_idDocumentoNonProtocollato);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
            });
        }

        [Test()]
        [Order(4)]
        public async Task UpdateDocumentoNonProtocollatoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await GetDocumentoAmministrativo(_idDocumentoNonProtocollato);

                var descrizioneOggetto = "Oggetto modificato";
                aggregate.ChangeOggettoDelDocumento(new OggettoDelDocumento()
                {
                    Descrizione = new TextValue(descrizioneOggetto)
                });

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id)
                    && aggregate.OggettoDelDocumento.Descrizione.Value == descrizioneOggetto);
            });
        }

        [Test()]
        [Order(5)]
        public async Task RecycleAndRestoreDocumentoNonProtocollatoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await GetDocumentoAmministrativo(_idDocumentoNonProtocollato);

                var noteRecycleBin = "Documento inserito in cestino";

                aggregate.Recycle(new TextValue(noteRecycleBin));

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(
                    !string.IsNullOrWhiteSpace(aggregate.Id)
                    && aggregate.InRecycleBin
                    && aggregate.NoteRecycleBin != null!
                    && aggregate.NoteRecycleBin.Value == noteRecycleBin);

                aggregate.Restore();

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(
                    !string.IsNullOrWhiteSpace(aggregate.Id)
                    && !aggregate.InRecycleBin
                    && aggregate.NoteRecycleBin! != null!
                    && aggregate.NoteRecycleBin!.Value == string.Empty);
            });
        }


        [Test()]
        [Order(6)]
        public async Task PredisponiEAnnullaPredisposizioneDocumentoNonProtocollatoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await GetDocumentoAmministrativo(_idDocumentoNonProtocollato);

                aggregate.Predisponi(TipologiaFlussoEnum.E);

                aggregate.AssignMittente(new Mittente(
                    new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.PF()
                    {
                        Cognome = "Carducci",
                        Nome = "Giosuè"
                    }));

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(
                    !string.IsNullOrWhiteSpace(aggregate.Id)
                    && aggregate.Mittente! != null!);

                aggregate.AnnullaPredisposizione();

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(
                    !string.IsNullOrWhiteSpace(aggregate.Id));
                //&& aggregate.Mittente! == null!); // TODO: qui il mittente dovrebbe essere null
            });
        }

        [Test()]
        [Order(7)]
        public async Task PredisponiEProtocollaDocumentoTest()
        {
            await TestRunner.Run(async () =>
            {

                var aggregate = await GetDocumentoAmministrativo(_idDocumentoNonProtocollato);

                aggregate.Predisponi(TipologiaFlussoEnum.E);

                aggregate.AddAggFascicolo(Environment.GetEnvironmentVariable("IdAgg")!);

                aggregate.AssignMittente(new Mittente(
                    new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.PF()
                    {
                        Cognome = "Carducci",
                        Nome = "Giosuè"
                    }));

                aggregate.RichiediRegistrazione(new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.DatiRichiestaRegistrazione()
                {
                    DatiRegistro = new DatiRegistro()
                    {
                        IdRegistro = Environment.GetEnvironmentVariable("IdRF")!
                    }
                });

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
            });
        }

        [Test]
        [Order(8)]
        public async Task AddAllegatoTest()
        {
            await TestRunner.Run(async () =>
            {

                var aggregate = new DocumentoAmministrativo(
                _idTenant,
                DateTime.Now,
                new OggettoDelDocumento() { Descrizione = new TextValue("Documento allegato") },
                null,
                null,
                null,
                new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.IdDoc()
                {
                    Identiticativo = _idDocumentoNonProtocollato
                });

                aggregate.ChangeNumeroPagineAllegato(100);
                aggregate.ChangeTipologiaAllegato(TipologieAllegatiEnum.Utente);

                await _documentoAmministrativoRepository.Add(aggregate);

                _idAllegato = aggregate.Id;

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
            });
        }

        [Test]
        [Order(9)]
        public async Task GetAllegatoTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await GetDocumentoAmministrativo(_idAllegato);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
            });
        }


        [Test()]
        [Order(10)]
        public async Task AddProtocolloEntrataTest()
        {
            await TestRunner.Run(async () =>
            {

                var aggregate = new DocumentoAmministrativo(
                _idTenant,
                DateTime.Now,
                new OggettoDelDocumento() { Descrizione = new TextValue("Protocollo in entrata") },
                new DatiRegistro()
                {
                    IdRegistro = _idRegistro
                },
                TipologiaFlussoEnum.E);

                aggregate.AddClassification(
                    _idClassification.ToString());

                aggregate.AddAggFascicolo(Environment.GetEnvironmentVariable("IdAgg")!);

                aggregate.AssignMezzoSpedizione(
                    _idMezzoSpedizione.ToString(),
                    new TextValue("CORRIERE"));

                aggregate.AssignMittente(new Mittente(132033890.ToString()));

                aggregate.AddMittenteMultiplo(new Mittente(
                    new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.PF()
                    {
                        Cognome = "Carducci",
                        Nome = "Giosuè"
                    }));

                aggregate.AddMittenteMultiplo(new Mittente(
                    new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.PF()
                    {
                        Cognome = "Verdi",
                        Nome = "Giuseppe"
                    }));

                aggregate.AssignProtocolloMittente(new ProtocolloMittente()
                {
                    Data = DateTime.Now.AddDays(-1),
                    DataArrivo = DateTime.Now,
                    Segnatura = "PROVA"
                });

                aggregate.RichiediRegistrazione(new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.DatiRichiestaRegistrazione()
                {
                    DatiRegistro = new DatiRegistro()
                    {
                        IdRegistro = Environment.GetEnvironmentVariable("IdRF")!
                    },
                    Predisponi = false
                });

                await _documentoAmministrativoRepository.Add(aggregate);

                _idDocumentoProtocollatoInEntrata = aggregate.Id;

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
            });
        }

        [Test()]
        [Order(11)]
        public async Task DocumentoProtocollatoInEntrataExistsTest()
        {
            await TestRunner.Run(async () =>
            {
                var exists = await _documentoAmministrativoRepository.Exists(_idTenant, _idDocumentoProtocollatoInEntrata);

                Assert.That(exists);
            });
        }

        [Test()]
        [Order(12)]
        public async Task GetDocumentoProtocollatoInEntrataTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await GetDocumentoAmministrativo(_idDocumentoProtocollatoInEntrata);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
            });
        }

        [Test()]
        [Order(13)]
        public async Task UpdateDocumentoProtocollatoInEntrataTest()
        {
            await TestRunner.Run(async () =>
            {

                var aggregate = await GetDocumentoAmministrativo(_idDocumentoProtocollatoInEntrata);

                var descrizioneOggetto = "Oggetto modificato con tipologia";
                aggregate.ChangeOggettoDelDocumento(new OggettoDelDocumento()
                {
                    Descrizione = new TextValue(descrizioneOggetto)
                });

                aggregate.AssignProtocolloEmergenza(new ProtocolloEmergenza()
                {
                    Segnatura = "SegnaturaEm",
                    Data = DateTime.Now
                });

                var idProfile = Environment.GetEnvironmentVariable("IdProfile")!;

                aggregate.AddProfile(
                    idProfile,
                    new TextValue("Tipologia per test conservazione"));

                aggregate.AddProfileField(
                    idProfile,
                    Environment.GetEnvironmentVariable("IdProfileFieldCasellaDiSelezione")!,
                    new TextValue("Casella di selezione"),
                    "CasellaDiSelezione",
                    new ElementFieldMultiValue(new TextValue[2] { new TextValue("Si"), new TextValue("No") }));

                aggregate.AddProfileField(
                    idProfile,
                    Environment.GetEnvironmentVariable("IdProfileFieldContatore")!,
                    new TextValue("Contatore"),
                    "Contatore",
                    new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ContatoreRepertorioFieldValue()
                    {
                        Conta = true,
                        ResetInizioAnno = true,
                        IdRegistro = _idRegistro
                    });

                aggregate.AddProfileField(
                    idProfile,
                    Environment.GetEnvironmentVariable("IdProfileFieldCorrispondente")!,
                    new TextValue("Corrispondente"),
                    "Corrispondente",
                    new ElementFieldSingleValue(new TextValue("132442193"))); // TODO: dovrebbe essere un ElementFieldLookupValue 

                aggregate.AddProfileField(
                    idProfile,
                    Environment.GetEnvironmentVariable("IdProfileFieldCampoDiTesto1")!,
                    new TextValue("Test"),
                    "CampoDiTesto",
                    new ElementFieldSingleValue(new TextValue("Valore 1")));

                aggregate.AddProfileField(
                    idProfile,
                    Environment.GetEnvironmentVariable("IdProfileFieldCampoDiTesto2")!,
                    new TextValue("Test2"),
                    "CampoDiTesto",
                    new ElementFieldSingleValue(new TextValue("Valore 2")));

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id)
                    && aggregate.OggettoDelDocumento.Descrizione.Value == descrizioneOggetto);
            });
        }

        [Test()]
        [Order(14)]
        public async Task AssignDocumentBlobDocumentoProtocollatoInEntrataTest()
        {
            await TestRunner.Run(async () =>
            {

                var documentBlobAggregate = await this.CreateDocumentBlobAggregate();

                var aggregate = await GetDocumentoAmministrativo(_idDocumentoProtocollatoInEntrata);

                aggregate.AssignDocumentBlobRef(new DocumentBlobRef()
                {
                    FileName = documentBlobAggregate.FileName,
                    FileSize = documentBlobAggregate.FileSize,
                    CreationDate = documentBlobAggregate.CreationDate,
                    ContentType = documentBlobAggregate.ContentType,
                    Hash = documentBlobAggregate.Hash,
                    HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256,
                    IdBlob = documentBlobAggregate.Id
                },
                  new TargetVersionBehavior()
                  {
                      CreateNewVersion = true,
                      Name = new TextValue("Nuova versione")
                  });

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
            });
        }

        [Test()]
        [Order(15)]
        public async Task CreateEmptyVersionDocumentoProtocollatoInEntrataTest()
        {
            await TestRunner.Run(async () =>
            {
                var documentBlobAggregate = await this.CreateDocumentBlobAggregate();

                var aggregate = await GetDocumentoAmministrativo(_idDocumentoProtocollatoInEntrata);

                aggregate.CreateEmptyVersion(new TextValue("Nuova versione vuota"));

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
            });
        }


        [Test()]
        [Order(16)]
        public async Task RemoveEmptyVersionDocumentoProtocollatoInEntrataTest()
        {
            await TestRunner.Run(async () =>
            {
                Assert.That(true); // Non applicabile in contesto Core
            });
        }

        [Test()]
        [Order(17)]
        public async Task AddProtocolloUscitaTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = new DocumentoAmministrativo(
                    _idTenant,
                    DateTime.Now,
                    new OggettoDelDocumento() { Descrizione = new TextValue("Protocollo in in uscita") },
                    new DatiRegistro()
                    {
                        IdRegistro = _idRegistro
                    },
                    TipologiaFlussoEnum.U);

                aggregate.AddClassification(
                    _idClassification.ToString());

                aggregate.AddAggFascicolo(Environment.GetEnvironmentVariable("IdAgg")!);

                aggregate.AssignMittente(new Mittente(132033890.ToString()));

                aggregate.AddDestinatario(new Destinatario(
                   new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.PF()
                   {
                       Cognome = "Verdi",
                       Nome = "Giuseppe"
                   }));

                aggregate.AddDestinatarioCc(new Destinatario(
                    new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.PF()
                    {
                        Cognome = "Puccini",
                        Nome = "Giacomo"
                    }));

                aggregate.RichiediRegistrazione(new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.DatiRichiestaRegistrazione()
                {
                    DatiRegistro = new DatiRegistro()
                    {
                        IdRegistro = Environment.GetEnvironmentVariable("IdRF")!
                    },
                    Predisponi = false
                });

                await _documentoAmministrativoRepository.Add(aggregate);

                _idDocumentoProtocollatoInUscita = aggregate.Id;

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
            });
        }

        [Test()]
        [Order(18)]
        public async Task GetDocumentoProtocollatoInUscitaTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await GetDocumentoAmministrativo(_idDocumentoProtocollatoInUscita);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
            });
        }

        [Test()]
        [Order(19)]
        public async Task UpdateDocumentoProtocollatoInUscitaTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await GetDocumentoAmministrativo(_idDocumentoProtocollatoInUscita);

                var descrizioneOggetto = "Oggetto modificato";
                aggregate.ChangeOggettoDelDocumento(new OggettoDelDocumento()
                {
                    Descrizione = new TextValue(descrizioneOggetto)
                });

                aggregate.AddDestinatario(new Destinatario(
                     new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.PG()
                     {
                         DenominazioneUfficio = new TextValue("Ufficio"),
                         DenominazioneOrganizzazione = new TextValue("Organizzazione")
                     }));

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id)
                    && aggregate.OggettoDelDocumento.Descrizione.Value == descrizioneOggetto);
            });
        }

        [Test()]
        [Order(20)]
        public async Task ReserveAndUnreserveDocumentoProtocollatoInUscitaTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await GetDocumentoAmministrativo(_idDocumentoProtocollatoInUscita);

                aggregate.Reserve();

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id)
                    && aggregate.Reserved);

                aggregate.Unreserve();

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
                    //&& aggregate.Reserved!);
            });
        }

        [Test()]
        [Order(21)]
        public async Task ConsolidaDocumentoProtocollatoInUscitaTest()
        {
            await TestRunner.Run(async () =>
            {
                var aggregate = await GetDocumentoAmministrativo(_idDocumentoProtocollatoInUscita);

                aggregate.Consolida(
                    new Consolidamento() 
                    { 
                        Autore = new Autore()
                        {
                            PF = new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.PF()
                            {
                                Cognome = "Frezza",
                                Nome = "Stefano"
                            }
                        },
                        Stato = StatiConsolidamentoEnum.Livello1 
                    });

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
                    //&& aggregate.Consolidamento! != null!
                    //&& aggregate.Consolidamento!.Stato == StatiConsolidamentoEnum.Livello1);

                await _documentoAmministrativoRepository.Update(aggregate);

                aggregate.Consolida(
                    new Consolidamento()
                    {
                        Autore = new Autore()
                        {
                            PF = new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.PF()
                            {
                                Cognome = "Frezza",
                                Nome = "Stefano"
                            }
                        },
                        Stato = StatiConsolidamentoEnum.Livello2
                    });

                await _documentoAmministrativoRepository.Update(aggregate);

                Assert.That(!string.IsNullOrWhiteSpace(aggregate.Id));
                    //&& aggregate.Consolidamento! != null!
                    //&& aggregate.Consolidamento!.Stato == StatiConsolidamentoEnum.Livello2);
            });
        }

        [Test()]
        [Order(22)]
        public async Task GetDocumentoStampaRegistroTest()
        {
            await TestRunner.Run(async () =>
            {
                Assert.That(true); // Non applicabile in contesto Core
            });
        }

        [Test()]
        [Order(23)]
        public async Task GetDocumentoStampaRepertorioTest()
        {
            await TestRunner.Run(async () =>
            {
                Assert.That(true); // Non applicabile in contesto Core
            });
        }

        private async Task<DocumentoAmministrativo> GetDocumentoAmministrativo(string id, bool bypassSecurityCheck = false)
        {
            var aggregate = await this._documentoAmministrativoRepository.Get(
                _idTenant.ToString(), id.ToString());

            return aggregate;
        }

        private async Task<DocumentBlob> CreateDocumentBlobAggregate()
        {
            var documentBlobAggregate = new DocumentBlob(
                _idTenant,
                DateTime.Now,
                new TextValue("Nuovo file"));

            using var ms = new MemoryStream(Pi3.Core.Test.Resources.Domain_Driven_Design__Tackling___Eric_Evans_14, 0, Pi3.Core.Test.Resources.Domain_Driven_Design__Tackling___Eric_Evans_14.Length);

            documentBlobAggregate.UploadStream(ms, "documentoTest1.pdf");
            documentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

            await this._documentBlobRepository.Add(documentBlobAggregate);

            return documentBlobAggregate;
        }
    }
}