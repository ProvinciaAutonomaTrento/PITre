// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Aggregazioni.Aggiungi;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Annullamento;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.CambioMittente;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Cancella;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Classificazioni.Aggiungi;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Consolida;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Entrata;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.NonProtocollato;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Destinatari;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Destinatari.Add;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Documenti.AddCollegato;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Keywords.Aggiungi;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Keywords.Rimuovi;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.MittentiMultipli;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Note.Add;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Note.Delete;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Oggetto;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Profili;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Protocollo.Emergenza;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Protocollo.Mittente;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Protocollo.Registrazione;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Ripristina;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Spedizione;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Accettazione;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Add;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.AddModello;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Delete;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Invio;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Rifiuto;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetDocumentoAmministrativo;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetTrasmissione;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.RicercaDocumenti;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.RicercaTrasmissioni;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using System.Text;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Tests
{
    public class DocumentiAmministrativiV1ApiTests
    {
        private HttpClient _apiHttpClient;
        private readonly string _controllerUrl = "api/v1/{0}/DocumentiAmministrativi";
        private readonly string _controllerRicercheUrl = "api/v1/{0}/Ricerche";
        
        private string _testIdNonProtocollatoCreato = null!;
        private string _testIdEntrataCreato = null!;
        private string _testIdUscitaCreato = null!;
        private string _testIdInternoCreato = null!;
        private string _testIdNotaCreata = null!;
        private string _testIdTrasmissioneCreata = null!;
        private string _testIdTrasmissioneModelloCreata = null!;
        private string _testIdVersioneCreata = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

            var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(Files.Variables);

            variables?.ToList().ForEach(v => Environment.SetEnvironmentVariable(v.Key, v.Value));

            WebApplicationFactory<Program> webAppFactory = new();
            this._apiHttpClient = webAppFactory.CreateClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            this._apiHttpClient?.Dispose();
        }

        [Test, Order(1)]
        [TestCase(false)]
        public async Task CreaNonProtocollatoTest(bool skipAssert)
        {
            using var content = new StringContent(Files.CreaNonProtocollatoTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance")) + "/NonProtocollato")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                CreaNonProtocollatoCommandResponse? result = JsonConvert.DeserializeObject<CreaNonProtocollatoCommandResponse>(responseContent);
                if (!String.IsNullOrWhiteSpace(result?.Id))
                {
                    this._testIdNonProtocollatoCreato = result.Id;
                    Console.WriteLine($"Creato Documento Non Protocollato con ID: {this._testIdNonProtocollatoCreato}");
                    if (!skipAssert) {
                        //Assert.That(
                        //    !string.IsNullOrWhiteSpace(result.Id)
                        //    && !string.IsNullOrWhiteSpace(result.CodiceRegistro)
                        //    && !string.IsNullOrWhiteSpace(result.Segnatura)
                        //    && result.Data.HasValue
                        //    && result.Numero.HasValue, Is.True);
                        Assert.Pass("Test concluso con successo");
                    }
                }
                else
                {
                    if (!skipAssert)
                        Assert.Fail("Nessun risultato restituito dalla creazione");
                }
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(2)]
        [TestCase(false)]
        public async Task CreaEntrataTest(bool skipAssert)
        {
            using var content = new StringContent(Files.CreaEntrataTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance")) + "/Entrata")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                CreaEntrataCommandResponse? result = JsonConvert.DeserializeObject<CreaEntrataCommandResponse>(responseContent);
                if (!String.IsNullOrWhiteSpace(result?.Id))
                {
                    this._testIdEntrataCreato = result.Id;
                    Console.WriteLine($"Creato Documento Entrata con ID: {this._testIdEntrataCreato}");

                    //Assert.IsTrue(
                    //    !string.IsNullOrWhiteSpace(response.Id)
                    //    && string.IsNullOrWhiteSpace(response.CodiceRegistro)
                    //    && string.IsNullOrWhiteSpace(response.Segnatura)
                    //    && response.Data.HasValue
                    //    && !response.Numero.HasValue);

                    if (!skipAssert)
                        Assert.Pass("Test concluso con successo");
                }
                else
                {
                    if (!skipAssert)
                        Assert.Fail("Nessun risultato restituito dalla creazione");
                }
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(3)]
        [TestCase(false)]
        public async Task CreaUscitaTest(bool skipAssert)
        {
            using var content = new StringContent(Files.CreaUscitaTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance")) + "/Uscita")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                CreaUscitaCommandResponse? result = JsonConvert.DeserializeObject<CreaUscitaCommandResponse>(responseContent);
                if (!String.IsNullOrWhiteSpace(result?.Id))
                {
                    this._testIdUscitaCreato = result.Id;
                    Console.WriteLine($"Creato Documento Uscita con ID: {this._testIdUscitaCreato}");
                    if (!skipAssert)
                        Assert.Pass("Test concluso con successo");
                }
                else
                {
                    if (!skipAssert)
                        Assert.Fail("Nessun risultato restituito dalla creazione");
                }
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(4)]
        [TestCase(false)]
        public async Task CreaInternoTest(bool skipAssert)
        {
            using var content = new StringContent(Files.CreaInternoTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance")) + "/Interno")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                CreaInternoCommandResponse? result = JsonConvert.DeserializeObject<CreaInternoCommandResponse>(responseContent);
                if (!String.IsNullOrWhiteSpace(result?.Id))
                {
                    this._testIdInternoCreato = result.Id;
                    Console.WriteLine($"Creato Documento Interno con ID: {this._testIdInternoCreato}");
                    if(!skipAssert)
                        Assert.Pass("Test concluso con successo");
                }
                else
                {
                    if (!skipAssert)
                        Assert.Fail("Nessun risultato restituito dalla creazione");
                }
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(5)]
        public async Task GetDocumentoAmministrativoTest()
        {
            Assert.That(this._testIdNonProtocollatoCreato, Is.Not.Empty);
            Console.WriteLine($"Recupero il documento con ID: {this._testIdNonProtocollatoCreato}");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))}/{this._testIdNonProtocollatoCreato}/?versioni=true&classificazioni=true&aggregazioni=true&permessi=true&profili=true&allegati=true&soggetti=true&note=true&keywords=true");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetDocumentoAmministrativoQueryResult>(responseContent);
                Assert.That(result, Is.Not.Null);
                Assert.That(result.DocumentoAmministrativo, Is.Not.Null);
                Assert.That(result.DocumentoAmministrativo.Id, Is.Not.Null);
                Assert.That(this._testIdNonProtocollatoCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(6)]
        public async Task GetSearchDocumentiNonProtocollatoTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{string.Format(this._controllerRicercheUrl, Environment.GetEnvironmentVariable("instance"))}/NonProtocollati/?annoCreazione=2024");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<RicercaDocumentiQueryResponse>(responseContent);
                Assert.That(result?.Documenti, Is.Not.Null);
                Assert.That(result?.Documenti.Count, Is.GreaterThan(0));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(7)]
        public async Task GetSearchDocumentiEntrataTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                @$"{string.Format(this._controllerRicercheUrl,
                Environment.GetEnvironmentVariable("instance"))}/Protocollati/?annoProtocollazione=2024&registro=PAT&tipologiaFlusso=E");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<RicercaDocumentiQueryResponse>(responseContent);
                Assert.That(result?.Documenti, Is.Not.Null);
                Assert.That(result?.Documenti.Count, Is.GreaterThan(0));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(8)]
        public async Task GetSearchDocumentiUscitaTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                @$"{string.Format(this._controllerRicercheUrl,
                Environment.GetEnvironmentVariable("instance"))}/Protocollati/?annoProtocollazione=2024&registro=PAT&tipologiaFlusso=U");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<RicercaDocumentiQueryResponse>(responseContent);
                Assert.That(result?.Documenti, Is.Not.Null);
                Assert.That(result?.Documenti.Count, Is.GreaterThan(0));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(9)]
        public async Task GetSearchDocumentiInternoTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                @$"{string.Format(this._controllerRicercheUrl,
                Environment.GetEnvironmentVariable("instance"))}/Protocollati/?annoProtocollazione=2024&registro=PAT&tipologiaFlusso=I");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<RicercaDocumentiQueryResponse>(responseContent);
                Assert.That(result?.Documenti, Is.Not.Null);
                Assert.That(result?.Documenti.Count, Is.GreaterThan(0));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(10)]
        public async Task PutPredisponiProtocolloTest()
        {
            using var content = new StringContent(Files.PredisponiProtocolloTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Predisposto")
            {
                Content = content
            };


            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<RegistrazioneProtocolloCommandResponse>(responseContent);
                Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(11)]
        public async Task PutRegistrazioneProtocolloTest()
        {
            await CreaEntrataTest(true);

            using var content = new StringContent(Files.RegistrazioneProtocolloTest_Actual, Encoding.UTF8, "application/json");


            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Registrazione")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<RegistrazioneProtocolloCommandResponse>(responseContent);
                Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(12)]
        public async Task PutAggiungiClassificazioneTest()
        {
            await this.CreaNonProtocollatoTest(true);

            using var content = new StringContent(Files.AggiungiClassificazioneTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdNonProtocollatoCreato}/Classificazioni/" + "2.1");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AggiungiClassificazioneCommandResponse>(responseContent);
                Assert.That(this._testIdNonProtocollatoCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(13)]
        public async Task PutAggiungiAggregazioneTest()
        {
            using var content = new StringContent(Files.AggiungiAggregazioneTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdNonProtocollatoCreato}/Aggregazione")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AggiungiAggregazioneCommandResponse>(responseContent);
                Assert.That(this._testIdNonProtocollatoCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(14)]
        public async Task PutCambiaOggettoTest()
        {
            using var content = new StringContent(Files.CambiaOggettoTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdNonProtocollatoCreato}/Oggetto")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<OggettoDocumentoCommandResponse>(responseContent);
                Assert.That(this._testIdNonProtocollatoCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(15)]
        public async Task PutCambioMittenteTest()
        {
            using var content = new StringContent(Files.CambioMittenteTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdUscitaCreato}/Mittente")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<CambioMittenteCommandResponse>(responseContent);
                Assert.That(this._testIdUscitaCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(16)]
        public async Task PutCambioMittentiMultipliTest()
        {
            await CreaEntrataTest(true);

            using var content = new StringContent(Files.CambioMittenteMultiploTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/MittentiMultipli")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<CambioMittentiMultipliCommandResponse>(responseContent);
                Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(17)]
        public async Task PutAddDestinatariTest()
        {
            using var content = new StringContent(Files.AddDestinatariTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdUscitaCreato}/Destinatari")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AddDestinatariCommandResponse>(responseContent);
                Assert.That(this._testIdUscitaCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(18)]
        public async Task PutAddDestinatariCCTest()
        {
            using var content = new StringContent(Files.AddDestinatariCCTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdUscitaCreato}/DestinatariCC")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AddDestinatariCCCommandResponse>(responseContent);
                Assert.That(this._testIdUscitaCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(19)]
        public async Task PutAddKeywordTest()
        {
            using var content = new StringContent(Files.AddKeywordTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/ParoleChiave")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AggiungiKeyWordCommandResponse>(responseContent);
                Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(20)]
        public async Task DelAddKeywordTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/ParoleChiave/parolatest");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<RimuoviKeyWordCommandResponse>(responseContent);
                Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(21)]
        public async Task ConsolidamentoTest()
        {
            using var content = new StringContent(Files.ConsolidamentoTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Consolidamento")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<ConsolidamentoCommandResponse>(responseContent);
                Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(22)]
        public async Task SpedizioneTest()
        {
            using var content = new StringContent(Files.SpedizioneTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/MezzoSpedizione")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<SpedizioneCommandResponse>(responseContent);
                Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(23)]
        public async Task AddNoteTest()
        {
            using var content = new StringContent(Files.AddNoteTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Note")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AggiungiNotaCommandResponse>(responseContent);
                _testIdNotaCreata = result?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(24)]
        public async Task DeleteNoteTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Note/{this._testIdNotaCreata}");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<CancellaNotaCommandResponse>(responseContent);
                //_testIdNotaCreata = result?.DocumentoAmministrativo?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(25)]
        public async Task PutProtocolloMittenteTest()
        {
            using var content = new StringContent(Files.ProtocolloMittenteTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/ProtocolloMittente")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<ProtocolloMittenteCommandResponse>(responseContent);
                _testIdNotaCreata = result?.DocumentoAmministrativo?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(26)]
        public async Task PutProtocolloEmergenzaTest()
        {
            using var content = new StringContent(Files.ProtocolloEmergenzaTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/ProtocolloEmergenza")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<ProtocolloEmergenzaCommandResponse>(responseContent);
                _testIdNotaCreata = result?.DocumentoAmministrativo?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(27)]
        public async Task PutProfiloTest()
        {
            using var content = new StringContent(Files.ProfiloPut_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Profili")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<StoreProfiloCommandResponse>(responseContent);
                //_testIdNotaCreata = result?.DocumentoAmministrativo?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(28)]
        public async Task PutAnnuullamentoTest()
        {
            using var content = new StringContent(Files.AnnullamentoTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Annullamento")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AnnullamentoCommandResponse>(responseContent);
                //_testIdNotaCreata = result?.DocumentoAmministrativo?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(29)]
        public async Task DeleteCancellazioneTest()
        {
            using var content = new StringContent(Files.DeleteDocumentoTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdNonProtocollatoCreato}")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<CancellaCommandResponse>(responseContent);
                //_testIdNotaCreata = result?.DocumentoAmministrativo?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(30)]
        public async Task PutRipristinaTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdNonProtocollatoCreato}/Ripristina");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<RipristinaCommandResponse>(responseContent);
                _testIdNonProtocollatoCreato = result?.DocumentoAmministrativo?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(31)]
        [TestCase(false)]
        public async Task PostTrasmissioneTest(bool skipAssert = false)
        {
            using var content = new StringContent(Files.TrasmissioniPost_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Trasmissioni")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AggiungiTrasmissioneCommandResponse>(responseContent);
                _testIdTrasmissioneCreata = result?.Id;
                
                if (!skipAssert)
                    Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                if (!skipAssert)
                    Assert.Fail(responseContent);
            }
        }

        [Test, Order(32)]
        public async Task RicercaTrasmissioniTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                @$"{string.Format(this._controllerRicercheUrl,
                Environment.GetEnvironmentVariable("instance"))}/Trasmissioni?idDocumento={this._testIdEntrataCreato}");
            
            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<RicercaTrasmissioniQueryResponse>(responseContent);
                
                Assert.That(result?.Trasmissioni.Count(), Is.GreaterThan(0));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(33)]
        public async Task GetTrasmissioneTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Trasmissioni/{this._testIdTrasmissioneCreata}");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetTrasmissioneQueryResponse>(responseContent);
                
                Assert.That(result!.Trasmissione != null!);
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }


        [Test, Order(33)]
        public async Task PostTrasmissioneModelloTest()
        {
            using var content = new StringContent(Files.TrasmissioniPost_Actual, Encoding.UTF8, "application/json");


            var request = new HttpRequestMessage(HttpMethod.Post,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Trasmissioni/Modello/" + "MD_218746");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AggiungiTrasmissioneModelloCommandResponse>(responseContent);
                _testIdTrasmissioneModelloCreata = result?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(34)]
        public async Task PutTrasmissioneTest()
        {
            using var content = new StringContent(Files.TrasmissioniPut_Actual, Encoding.UTF8, "application/json");


            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Trasmissioni/{this._testIdTrasmissioneCreata}")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AggiungiTrasmissioneCommandResponse>(responseContent);
                _testIdTrasmissioneCreata = result?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(35)]
        public async Task DeleteTrasmissioneTest()
        {
            await PostTrasmissioneTest(true);

            using var content = new StringContent(Files.TrasmissioniPut_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Trasmissioni/{this._testIdTrasmissioneCreata}");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<CancellaTrasmissioneCommandResponse>(responseContent);
                //_testIdTrasmissioneCreata = result?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(36)]
        public async Task PutInvioTrasmissioneTest()
        {
            await PostTrasmissioneTest(true);

            var body = new
            {
                DataInvio = DateTime.Now
            };
            var stringBody = JsonConvert.SerializeObject(body);

            using var content = new StringContent(stringBody, Encoding.UTF8, "application/json");


            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Trasmissioni/{this._testIdTrasmissioneCreata}/Invio")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<InvioTrasmissioneCommandResponse>(responseContent);
                //_testIdTrasmissioneCreata = result?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(37)]
        public async Task PutAccettazioneTrasmissioneTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Trasmissioni/{this._testIdTrasmissioneCreata}/Accettazione");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AccettazioneTrasmissioneCommandResponse>(responseContent);
                //_testIdTrasmissioneCreata = result?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(38)]
        public async Task PutRifiutoTrasmissioneTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Trasmissioni/{this._testIdTrasmissioneCreata}/Rifiuto");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<RifiutoTrasmissioneCommandResponse>(responseContent);
                //_testIdTrasmissioneCreata = result?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(39)]
        public async Task PutVistoTrasmissioneTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Trasmissioni/{this._testIdTrasmissioneCreata}/Visto");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<VistoTrasmissioneCommandResponse>(responseContent);
                //_testIdTrasmissioneCreata = result?.Id;
                //Assert.That(this._testIdEntrataCreato, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        //        [Test, Order(5)]
        public async Task<GetDocumentoAmministrativoQueryResult> GetDocumentoTest(TipologiaFlussoEnum? tipoFlusso)
        {
            var idDocumento = tipoFlusso switch
            {
                TipologiaFlussoEnum.E => _testIdEntrataCreato,
                TipologiaFlussoEnum.I => _testIdInternoCreato,
                TipologiaFlussoEnum.U => _testIdUscitaCreato,
                _ => _testIdNonProtocollatoCreato
            };

            Assert.That(idDocumento, Is.Not.Empty);
            Console.WriteLine($"Recupero il documento con ID: {idDocumento}");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))}/{idDocumento}/?versioni=true&classificazioni=true&aggregazioni=true");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetDocumentoAmministrativoQueryResult>(responseContent);
                Assert.That(idDocumento, Is.EqualTo(result?.DocumentoAmministrativo?.Id));
                //Assert.Pass("Test concluso con successo");
                return result;
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                //Assert.Fail(responseContent);
                return null;
            }
        }

        [Test, Order(41)]
        public async Task PutVersioniTest()
        {
            var body = new
            {
                UploadId = Guid.NewGuid(),
                Descrizione = "file di prova"
            };
            var stringBody = JsonConvert.SerializeObject(body);

            using var content = new StringContent(stringBody, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdInternoCreato}/Versioni")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                //var result = JsonConvert.DeserializeObject<UploadVersionRequestRespon>(responseContent);

                var documento = await GetDocumentoTest(TipologiaFlussoEnum.I);
                Assert.That(documento, Is.Not.Null);
                Assert.That(documento.DocumentoAmministrativo.Id, Is.Not.Null);
                Assert.That(documento.DocumentoAmministrativo.Versions, Is.Not.Null);
                Assert.That(documento.DocumentoAmministrativo.Versions.Count, Is.GreaterThan(0));
                _testIdVersioneCreata = documento.DocumentoAmministrativo.Versions[documento.DocumentoAmministrativo.Versions.Count - 1].Id;
                Assert.That(this._testIdInternoCreato, Is.EqualTo(documento.DocumentoAmministrativo.Id));

                //_testIdTrasmissioneCreata = result?.Id;
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(42)]
        public async Task PutDocumentoCollegatoTest()
        {
            using var content = new StringContent(Files.AddDocumentoCollegatoTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/DocumentiCollegati")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                AddDocumentoCollegatoCommandResponse? result = JsonConvert.DeserializeObject<AddDocumentoCollegatoCommandResponse>(responseContent);
                if (!String.IsNullOrWhiteSpace(result?.DocumentoAmministrativo.Id))
                {
                    this._testIdEntrataCreato = result.DocumentoAmministrativo.Id;
                    Console.WriteLine($"Creato Documento Entrata con ID: {this._testIdEntrataCreato}");
                    Assert.Pass("Test concluso con successo");
                }
                else
                {
                    Assert.Fail("Nessun risultato restituito dalla creazione");
                }
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(43)]
        [TestCase(false)]
        public async Task CreaEntrataConUploadIdTest(bool skipAssert)
        {
            var objPayload = JsonConvert.DeserializeObject<CreaEntrataCommand>(Files.CreaEntrataUploadTest_Actual);
            var strPaylod = JsonConvert.SerializeObject(objPayload);

            using var content = new StringContent(strPaylod, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance")) + "/Entrata")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                CreaEntrataCommandResponse? result = JsonConvert.DeserializeObject<CreaEntrataCommandResponse>(responseContent);
                if (!String.IsNullOrWhiteSpace(result?.Id))
                {
                    this._testIdEntrataCreato = result.Id;
                    Console.WriteLine($"Creato Documento Entrata con ID: {this._testIdEntrataCreato}");
                    if (!skipAssert)
                        Assert.Pass("Test concluso con successo");
                }
                else
                {
                    if (!skipAssert)
                        Assert.Fail("Nessun risultato restituito dalla creazione");
                }
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(44)]
        public async Task DelDocumentoCollegatoTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/DocumentiCollegati/" + "136605352");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                //CreaEntrataCommandResponse? result = JsonConvert.DeserializeObject<CreaEntrataCommandResponse>(responseContent);
                //if (!String.IsNullOrWhiteSpace(result?.Id))
                //{
                //    this._testIdEntrataCreato = result.Id;
                //    Console.WriteLine($"Creato Documento Entrata con ID: {this._testIdEntrataCreato}");
                //    Assert.Pass("Test concluso con successo");
                //}
                //else
                //{
                //    Assert.Fail("Nessun risultato restituito dalla creazione");
                //}
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(45)]
        public async Task GetContentTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Contenuti");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                var content = await response.Content.ReadAsByteArrayAsync();
                Console.WriteLine($"Risposta Binary: {content}");

                if (content != null && content.Length > 0)
                {
                    //this._testIdEntrataCreato = result.Id;
                    Console.WriteLine($"Documento scaricato con successo");
                    Assert.Pass("Test concluso con successo");
                }
                else
                {
                    Assert.Fail("Nessun risultato restituito dalla creazione");
                }
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(46)]
        public async Task PutConvertToPdfTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdEntrataCreato}/Contenuti/Pdf");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(47)]
        public async Task GetContentVersionTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdInternoCreato}/Versioni/{this._testIdVersioneCreata}/Contenuti");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                var content = await response.Content.ReadAsByteArrayAsync();
                Console.WriteLine($"Risposta Binary: {content}");

                if (content != null && content.Length > 0)
                {
                    //this._testIdEntrataCreato = result.Id;
                    Console.WriteLine($"Documento scaricato con successo");
                    Assert.Pass("Test concluso con successo");
                }
                else
                {
                    Assert.Fail("Nessun risultato restituito dalla creazione");
                }
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(48)]
        public async Task PutConvertToPdfVersionTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Put,
                @$"{string.Format(this._controllerUrl,
                Environment.GetEnvironmentVariable("instance"))}/{this._testIdInternoCreato}/Versioni/{this._testIdVersioneCreata}/Contenuti/Pdf");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }
    }
}