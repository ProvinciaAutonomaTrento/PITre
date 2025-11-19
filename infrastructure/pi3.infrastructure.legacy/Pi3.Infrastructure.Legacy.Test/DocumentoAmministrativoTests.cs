// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core;
using Pi3.Core.AggregateModels.DocumentAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
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
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using AutoMapper;

namespace Pi3.Infrastructure.Legacy.Test
{
    [Ignore("Ignorati temporaneamente")]
    public class DocumentoAmministrativoTests
    {
        ServiceProvider _serviceProvider;
        IDocumentoAmministrativoRepository _repository;

        [SetUp]
        public void Setup()
        {
            this._serviceProvider = new ServiceCollection()
                .AddBaseDependenciesForTest()
                .AddInfrastructureLegacyEFDocumentoAmministrativoAggregate()
                .BuildServiceProvider();

            this._repository = this._serviceProvider.GetRequiredService<IDocumentoAmministrativoRepository>();
        }

        [Test]
        public async Task TestGet()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                140618528.ToString(),new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadProfilesMetadata = true,
                        LoadClassifications = true,
                        LoadAllegati = true,
                        LoadAggregazioni = true,
                        LoadVersions = true,
                        LoadPermissions = true,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = true,
                        LoadNote = true,
                        BypassSecurityCheck = false
                    }
                });

            var segnaturaXML = aggregate.GetSegnaturaXml();

            Assert.Pass();
        }

        [Test]
        public async Task TestLoad()
        {
            var aggregate = await this._repository.Get(
               132901715.ToString(),               
                new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadProfilesMetadata = false,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = false,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = false,
                        LoadNote = false
                    }
                });

            Assert.IsTrue(
                aggregate.Profiles.Count == 0 
                && aggregate.Classifications.Count == 0
                && aggregate.Allegati.Count == 0
                && aggregate.Aggregazioni.Count == 0
                && aggregate.Versions.Count == 0
                && aggregate.Permissions.Count == 0
                && aggregate.Mittente == null
                && aggregate.MittentiMultipli.Count == 0
                && aggregate.Destinatari.Count == 0
                && aggregate.DestinatariCc.Count == 0
                && aggregate.Keywords.Count == 0
                && aggregate.Note.Count == 0
                );

            await this._repository.Load(aggregate,
                new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadClassifications = true,
                        LoadVersions = true,
                        LoadPermissions = true
                    }
                });
        }

        [Test]
        public async Task TestGetWithLoadBehavior()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                132901715.ToString(),
                new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        BypassSecurityCheck = true,
                        LoadProfiles = true,
                        LoadClassifications = false,
                        LoadAllegati = false,                        
                        LoadAggregazioni = false,
                        LoadVersions = false,
                        VersionsPagination = new Pagination()
                        {
                            Skip = 10,
                            Take = 10
                        },
                        LoadPermissions = false, 
                        LoadMittentiDestinatari = true
                    } 
                });

            Assert.Pass();
        }

        [Test]
        public async Task TestGetAllegato()
        {
            var aggregate = await this._repository.Get("361", "140557060",
                new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadClassifications = true,
                        LoadAllegati = true,
                        LoadAggregazioni = true,
                        LoadVersions = true,
                        LoadPermissions = true,
                        LoadMittentiDestinatari = true
                    }
                });

            aggregate.AddProfile(
               "3305",
               new TextValue("TIPOLOGIA CONTATORE DIFFERITO"));

            aggregate.AddProfileField(
                "3305",
                4127.ToString(),
                new TextValue("Contatore"),
                "Cont REP",
                new ContatoreRepertorioFieldValue() {
                   Conta = true,
                   ResetInizioAnno = true,
                   IdRegistro = "86107"
                });


            await _repository.Update(aggregate);
            Assert.Pass();
        }

        [Test]
        public async Task TestAddNonProtocollato()
        {
            var aggregate = new DocumentoAmministrativo(
                361.ToString(),
                DateTime.Now,
                new OggettoDelDocumento() { Descrizione = new TextValue("Non protocollato") },
                new DatiRegistro() { IdRegistro = 1031582.ToString() });

            await _repository.Add(aggregate);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(aggregate.Id));
        }

        [Test]
        public async Task TestAddAllegato()
        {
            var aggregate = new DocumentoAmministrativo(
                361.ToString(),
                DateTime.Now,
                new OggettoDelDocumento() { Descrizione = new TextValue("Documento allegato") },
                null,
                null,
                null,
                new IdDoc()
                {
                    Identiticativo = 140637895.ToString()
                });

            aggregate.ChangeNumeroPagineAllegato(100);
            aggregate.ChangeTipologiaAllegato(TipologieAllegatiEnum.PEC);

            await _repository.Add(aggregate);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(aggregate.Id));
        }


        [Test]
        public async Task TestAddProtocolloEntrata()
        {
            var aggregate = new DocumentoAmministrativo(
                361.ToString(),
                DateTime.Now,
                new OggettoDelDocumento() { Descrizione = new TextValue("Protocollo in entrata") },
                new DatiRegistro() { IdRegistro = 86107.ToString() },
                TipologiaFlussoEnum.E);

            aggregate.AddClassification(
                130275975.ToString());

            aggregate.AssignMezzoSpedizione(
                17200465.ToString(), 
                new TextValue("CORRIERE"));

            aggregate.AssignMittente(new Mittente(132033890.ToString()));

            aggregate.AddMittenteMultiplo(new Mittente(
                new PF()
                {
                    Cognome = "Frezza",
                    Nome = "Stefano"
                }));

            aggregate.AddMittenteMultiplo(new Mittente(
                new PF()
                {
                    Cognome = "Verdi",
                    Nome = "Antonio"
                }));
            
            aggregate.AssignProtocolloMittente(new ProtocolloMittente()
            {
                Data = DateTime.Now.AddDays(-1),
                DataArrivo = DateTime.Now,
                Segnatura = "PROVA"
            });
            
            aggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione()
            { 
                DatiRegistro = new DatiRegistro()
                {
                    IdRegistro = 1031582.ToString()
                },
                Predisponi = false
            });
            
            await _repository.Add(aggregate);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(aggregate.Id));
        }

        [Test]
        public async Task TestProtocollaEntrataPredisposto()
        {
            var aggregate = await this._repository.Get(361.ToString(), 132902091.ToString());

            aggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione()
            {
                Predisponi = false
            });

            await this._repository.Update(aggregate);
        }


        [Test]
        public async Task TestAddProtocolloEntrataWithBlobRef()
        {
            var aggregate = new DocumentoAmministrativo(
                131976804.ToString(),
                DateTime.Now,
                new OggettoDelDocumento() { Descrizione = new TextValue("Protocollo in entrata") },
                new DatiRegistro() { IdRegistro = 86107.ToString() },
                TipologiaFlussoEnum.E);

            aggregate.AddClassification(
                //130275975.ToString(), 
                130275974.ToString());


            aggregate.AssignMezzoSpedizione(
                17200465.ToString(),
                new TextValue("CORRIERE"));

            aggregate.AssignMittente(new Mittente(132033890.ToString()));

            aggregate.AddMittenteMultiplo(new Mittente(
                new PF()
                {
                    Cognome = "Frezza",
                    Nome = "Stefano"
                }));

            aggregate.AddMittenteMultiplo(new Mittente(
                new PF()
                {
                    Cognome = "Verdi",
                    Nome = "Antonio"
                }));

            aggregate.AssignProtocolloMittente(new ProtocolloMittente()
            {
                Data = DateTime.Now.AddDays(-1),
                DataArrivo = DateTime.Now,
                Segnatura = "PROVA"
            });

            aggregate.AssignDocumentBlobRef(new DocumentBlobRef()
                {
                    FileName = "Test.pdf",
                    FileSize = 750,
                    CreationDate = DateTime.Now,
                    ContentType = "application/pdf",
                    Hash = Encoding.UTF8.GetBytes("B0FB8C9E90A179A3B7196A026CC2DF32082D1A4C746FFC73608E25003FE2C733"),
                    HashName = HashNamesEnum.SHA256,
                    IdBlob = @"C:\Lavoro\Logs\PiTre\DOC_PAT\DocServer\PAT_TEST\2023\10\10\15\1d8c14f5-bff2-425a-a049-b2a5280e0634\Uno sguardo a lucene.pdf"
                },
                new TargetVersionBehavior()
                {
                    CreateNewVersion = true,
                    Name = new TextValue("Nuova versione")
                });

            aggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione());

            await _repository.Add(aggregate);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(aggregate.Id));
        }



        [Test]
        public async Task TestAddProtocolloEntrataProfilato()
        {
            var aggregate = new DocumentoAmministrativo(
                //361.ToString(),
                131976804.ToString(),
                DateTime.Now,
                new OggettoDelDocumento() { Descrizione = new TextValue("Protocollo in entrata profilato") },
                new DatiRegistro() { IdRegistro = 1031582.ToString() },
                TipologiaFlussoEnum.E);

            aggregate.AddClassification(
                132030745.ToString()
                //130275975.ToString(),
                //new TextValue("1 - Affari istituzionali, organizzazione e comunicazione")
                );

            aggregate.AddAggFascicolo(
                132041163.ToString());
                //132552815.ToString());

            aggregate.AssignMezzoSpedizione(
                17200465.ToString(),
                new TextValue("CORRIERE"));

            aggregate.AssignMittente(new Mittente(
                new PF()
                {
                    Cognome = "Frezza",
                    Nome = "Stefano"
                }));

            aggregate.AddMittenteMultiplo(new Mittente(
                new PF()
                {
                    Cognome = "Verdi",
                    Nome = "Antonio"
                }));

            aggregate.AssignProtocolloMittente(new ProtocolloMittente()
            {
                Data = DateTime.Now.AddDays(-1),
                DataArrivo = DateTime.Now,
                Segnatura = "PROVA"
            });

            var idProfile = 2365.ToString();

            aggregate.AddProfile(
                idProfile,
                new TextValue("Autorizzazione"));

            aggregate.AddProfileField(
                idProfile,
                3270.ToString(),
                new TextValue("Nome"),
                "Campo di testo",
                new ElementFieldSingleValue(new TextValue("Stefano")));

            aggregate.AddProfileField(
                idProfile,
                3271.ToString(),
                new TextValue("Cognome"),
                "Campo di testo",
                new ElementFieldSingleValue(new TextValue("Frezza")));

            aggregate.AddProfileField(
                idProfile,
                3272.ToString(),
                new TextValue("Matricola"),
                "Campo di testo",
                new ElementFieldSingleValue(new TextValue("000000000")));

            aggregate.AddProfileField(
                idProfile,
                3272.ToString(),
                new TextValue("Matricola"),
                "Campo di testo",
                new ElementFieldMultiValue(new int[3] { 2,5,6}));

            aggregate.AddProfileField(
                idProfile,
                3272.ToString(),
                new TextValue("Matricola"),
                "Campo di testo",
                new ElementFieldMultiValue(new bool[3] { false, true, true }));

            aggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione());

            await _repository.Add(aggregate);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(aggregate.Id));
        }

        [Test]
        public async Task TestAddProtocolloUscita()
        {
            var aggregate = new DocumentoAmministrativo(
                "361", 
                DateTime.Now, 
                new OggettoDelDocumento() { Descrizione = new TextValue("Protocollo in uscita") },
                new DatiRegistro() { IdRegistro = 1031582.ToString() },
                TipologiaFlussoEnum.U);
            
            aggregate.AddClassification(
                "130275975", 
                new TextValue("1 - Affari istituzionali, organizzazione e comunicazione"));

            aggregate.AssignMittente(new Mittente(
                new PF()
                {
                    Cognome = "Frezza",
                    Nome = "Stefano"
                }));

            aggregate.AddDestinatario(new Destinatario(
                new PF()
                {
                    Cognome = "Verdi",
                    Nome = "Giuseppe"
                }));

            aggregate.AddDestinatario(new Destinatario(
                new PF()
                {
                    Cognome = "Rossi",
                    Nome = "Mario"
                }));

            aggregate.AddDestinatarioCc(new Destinatario(
                new PF()
                {
                    Cognome = "Bianchi",
                    Nome = "Antonio"
                }));

            aggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione());

            await _repository.Add(aggregate);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(aggregate.Id));
        }

        public async Task TestAddStampa()
        {
            var aggregate = new DocumentoAmministrativo(
                361.ToString(),
                DateTime.Now,
                new OggettoDelDocumento()
                {
                    Descrizione = new TextValue("Stampa registro di protocollo")
                });

            aggregate.AssignDatiStampa(new DatiStampa
            {
                TipoStampa = TipologieStampaEnum.StampaRegistroProtocollo,
                AnnoStampa = DateTime.Now.Year,
                PrimoElementoStampato = new DatiRegistrazioneProtocollo
                {
                    NumeroProtocollo = 123,
                    DataProtocollazione = DateTime.Now
                },
                UltimoElementoStampato = new DatiRegistrazioneProtocollo
                {
                    NumeroProtocollo = 456,
                    DataProtocollazione = DateTime.Now
                }
            });
        }

        [Test]
        public async Task TestRimuoviVersione()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                132940228.ToString(), new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadProfilesMetadata = false,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = true,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = false,
                        LoadNote = false
                    }
                });

            aggregate.RemoveVersion(132940238.ToString());

            await this._repository.Update(aggregate);

            Assert.Pass();
        }

        [Test]
        public async Task TestAnnullaContatoreRepertorio()
        {
            var aggregate = await this._repository.Get(
                361.ToString(),
                136546060.ToString(), new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadProfilesMetadata = true,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = true,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = false,
                        LoadNote = false
                    }
                });

            aggregate.AnnullaContatoreRepertorio("2905", "3730");

            await this._repository.Update(aggregate);

            Assert.Pass();
        }
    }
}
