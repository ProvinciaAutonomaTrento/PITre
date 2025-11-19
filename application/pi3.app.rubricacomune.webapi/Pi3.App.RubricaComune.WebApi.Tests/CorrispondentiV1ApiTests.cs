// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Text;

using QUERY = Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti;
using COMMANDS = Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti;
using Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Update;
using Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.PubblicaAggiornamento;
using Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.GetEmails;
using Microsoft.Extensions.Configuration;
using Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti;

namespace Pi3.App.RubricaComune.WebApi.Tests
{
    public class CorrispondentiV1ApiTests
    {
        private HttpClient _apiHttpClient;
        private readonly string _controllerUrl = $"api/v1/Corrispondenti";

        private string _testIdCorrispondenteCreato = String.Empty;
        private string _testIdCorrispondentePubblicato = String.Empty;
        private readonly string _testEmailToEdit = "edit@test.it";

        [SetUp]
        public void Setup()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
            }

            var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(Files.Variables);

            variables?.ToList().ForEach(v => Environment.SetEnvironmentVariable(v.Key, v.Value));

            WebApplicationFactory<Program> webAppFactory = new();
            this._apiHttpClient = webAppFactory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            this._apiHttpClient?.Dispose();
        }

        [Test, Order(1)]
        public async Task CreateCorrispondente()
        {
            Random random = new();
            int numeroRandom = random.Next(10000, 100000);
            COMMANDS.Create.CreateCorrispondenteRequest createCorrispondenteRequest = new ()
            {
                Codice = $"test_{numeroRandom}",
                DatiCorrispondente = new COMMANDS.DatiCorrispondente()
                {
                    Tipo = Application.Commands.Corrispondenti.Tipi.UnitaOrganizzativa,
                    Denominazione = "Creato in fase di test",
                    Indirizzo = "Indirizzo test",
                    Amministrazione = "amministrazione test",
                    CAP = "00100",
                    Citta = "Roma",
                    CodiceFiscale = "xxxyyy00x00y000x",
                    Fax = "0000-1234567",
                    Nazione = "Italia",
                    PartitaIva = "01234567890",
                    Provincia = "RM",
                    Telefono = "0000-7654321",
                    UrlApiInteroperabilita = "http://localhost",
                    AOO = "test"
                }
            };

            string json = JsonConvert.SerializeObject(createCorrispondenteRequest);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");


            var request = new HttpRequestMessage(HttpMethod.Post, this._controllerUrl)
            {
                Content = content
            };

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                Corrispondente? result = JsonConvert.DeserializeObject<Corrispondente>(responseContent);
                if ( !String.IsNullOrWhiteSpace(result?.Id) )
                {
                    this._testIdCorrispondenteCreato = result.Id;
                    Console.WriteLine($"Creato corrispondente con ID: {this._testIdCorrispondenteCreato}");
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

        [Test, Order(2)]
        public async Task GetCorrispondentiById()
        {
            Assert.That(this._testIdCorrispondenteCreato, Is.Not.Empty);
            Console.WriteLine($"Recupero il corrispondente con ID: {this._testIdCorrispondenteCreato}");

            var request = new HttpRequestMessage(HttpMethod.Get,$"{this._controllerUrl}/{this._testIdCorrispondenteCreato}");

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                Corrispondente? result = JsonConvert.DeserializeObject<Corrispondente>(responseContent);
                Assert.That(this._testIdCorrispondenteCreato, Is.EqualTo(result?.Id));
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

        [Test, Order(3)]
        public async Task PutCorrispondente()
        {
            Assert.That(this._testIdCorrispondenteCreato, Is.Not.Empty);
            Console.WriteLine($"Modifico il corrispondente con ID: {this._testIdCorrispondenteCreato}");

            COMMANDS.DatiCorrispondente datiCorrispondente = new()
            { 
                Tipo = COMMANDS.Tipi.UnitaOrganizzativa,
                Denominazione = "Creato in fase di test e modificato",
                Indirizzo = "Indirizzo test modificata",
                Amministrazione = "amministrazione test modificata",
                CAP = "00111",
                Citta = "Roma edit",
                CodiceFiscale = "xxxyyy00x00y000x",
                Fax = "1234-1234567",
                Nazione = "Italia-edit",
                PartitaIva = "09876543210",
                Provincia = "RM",
                Telefono = "1111-7654321",
                UrlApiInteroperabilita = "http://localhost/modificato",
                AOO = "test modificato"
            };

            string json = JsonConvert.SerializeObject(datiCorrispondente);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{this._controllerUrl}/{this._testIdCorrispondenteCreato}")
            {
                Content = content
            };

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                Assert.Pass("Test concluso con successo");
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        
        [Test, Order(4)]
        public async Task SearchCorrispondenti()
        {
            Application.Queries.Corrispondenti.Search.SearchRequest searchRequest = new()
            {
                ElementiPerPagina = 10,
                Pagina = 1,
                CriteriRicerca = new[] { new Application.Queries.Corrispondenti.Search.CriterioRicerca() { 
                    Campo = Application.Queries.Corrispondenti.Search.CampiRicercaEnum.Aoo,
                    Valore = "test"
                } },
                CriteriOrdinamento = new[] { new Application.Queries.Corrispondenti.Search.CriterioOrdinamento()
                {
                    Campo = Application.Queries.Corrispondenti.Search.CampiRicercaEnum.Citta, 
                    Tipo = Application.Queries.Corrispondenti.Search.TipiOrdinamentoEnum.Asc
                } }
            };
            string json = JsonConvert.SerializeObject(searchRequest);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, $"{this._controllerUrl}/search")
            {
                Content = content
            };

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");
                QUERY.Search.SearchResponse ? result = JsonConvert.DeserializeObject< QUERY.Search.SearchResponse>(responseContent);
                
                Assert.That( result?.Corrispondenti?.Any(), Is.True );
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
        public async Task PubblicaCorrispondente()
        {
            Console.WriteLine("Pubblico il corrispondente");
            Random random = new();
            int numeroRandom = random.Next(10000, 100000);

            COMMANDS.Pubblica.PubblicaCorrispondenteRequest pubblicaCorrispondenteRequest = new()
            {
                Codice = $"PUB_TEST_{numeroRandom}",
                DatiCorrispondente = new COMMANDS.DatiCorrispondente()
                {
                    Denominazione = "test pubblicazione",
                    Tipo = COMMANDS.Tipi.UnitaOrganizzativa,
                    Indirizzo = "via Roma, 1",
                    CAP = "00100",
                    Citta = "Roma",
                    CodiceFiscale = "xxxyyy00x00y000x",
                    Fax = "0000-1234567",
                    Nazione = "Italia",
                    PartitaIva = "01234567890",
                    Provincia = "RM",
                    Telefono = "0000-7654321",
                    Amministrazione = "amministrazione test",
                    UrlApiInteroperabilita = "http://localhost",
                    AOO = "test"
                },
                Emails = new[] { 
                    new COMMANDS.Pubblica.Email() { Indirizzo = "info@test.it", Preferita = true, Note = "nota di test" },
                    new COMMANDS.Pubblica.Email() { Indirizzo = "support@test.it", Preferita = false, Note = "nota di test 2" }
                }
            };

            string json = JsonConvert.SerializeObject(pubblicaCorrispondenteRequest);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{this._controllerUrl}/pubblicazione")
            {
                Content = content
            };
            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                COMMANDS.Pubblica.PubblicaCorrispondenteResponse? result = JsonConvert.DeserializeObject<COMMANDS.Pubblica.PubblicaCorrispondenteResponse>(responseContent);
                if (!String.IsNullOrWhiteSpace(result?.Id))
                {
                    this._testIdCorrispondentePubblicato = result.Id;
                    Console.WriteLine($"Pubblicato corrispondente con ID: {this._testIdCorrispondentePubblicato}");
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

        [Test, Order(6)]
        public async Task PubblicaAggiornamentoCorrispondente()
        {
            Assert.That(this._testIdCorrispondentePubblicato, Is.Not.Empty);
            Console.WriteLine($"Pubblico aggiornamenti per il corrispondente con ID: {this._testIdCorrispondentePubblicato}");

            COMMANDS.PubblicaAggiornamento.DatiAggiornamento datiAggiornamento = new()
            {
                DatiCorrispondente = new COMMANDS.DatiCorrispondente()
                {
                    Denominazione = "modificato",
                    Tipo = COMMANDS.Tipi.UnitaOrganizzativa,
                    Indirizzo = "Indirizzo test modificata",
                    Amministrazione = "amministrazione test modificata",
                    CAP = "00111",
                    Citta = "Roma edit",
                    CodiceFiscale = "xxxyyy00x00y000x",
                    Fax = "1234-1234567",
                    Nazione = "Italia-edit",
                    PartitaIva = "09876543210",
                    Provincia = "RM",
                    Telefono = "1111-7654321",
                    UrlApiInteroperabilita = "http://localhost/modificato",
                    AOO = "test modificato"
                },
                Emails = new[] {
                    new COMMANDS.PubblicaAggiornamento.Email() { Indirizzo = "infoedit@test.it", Preferita = true, Note = "nota di test modificata" }
                }
            };

            string json = JsonConvert.SerializeObject(datiAggiornamento);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{this._controllerUrl}/{this._testIdCorrispondentePubblicato}/pubblicazione")
            {
                Content = content
            };

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                Assert.Pass("Test concluso con successo");
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(7)]
        public async Task AddEmail()
        {
            Assert.That(this._testIdCorrispondenteCreato, Is.Not.Empty);
            Console.WriteLine($"Aggiungo email per il corrispondente con ID: {this._testIdCorrispondenteCreato}");

            COMMANDS.AddEmail.DatiEmail datiEmail = new()
            {
                Email = this._testEmailToEdit,
                Preferita = false,
                Note = "aggiunta in test"
            };

            string json = JsonConvert.SerializeObject(datiEmail);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{this._controllerUrl}/{this._testIdCorrispondenteCreato}/emails")
            {
                Content = content
            };

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                Assert.Pass("Test concluso con successo");
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(8)]
        public async Task GetEmails()
        {
            Assert.That(this._testIdCorrispondentePubblicato, Is.Not.Empty);
            Console.WriteLine($"Recupero le email del corrispondente con ID: {this._testIdCorrispondentePubblicato}");

            var request = new HttpRequestMessage(HttpMethod.Get, $"{this._controllerUrl}/{this._testIdCorrispondentePubblicato}/emails");

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Risposta JSON: {responseContent}");

                QUERY.GetEmails.Email[]? result = JsonConvert.DeserializeObject<QUERY.GetEmails.Email[]>(responseContent);
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.Not.Empty);
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
        public async Task EditEmail()
        {
            Assert.That(this._testIdCorrispondenteCreato, Is.Not.Empty);
            Console.WriteLine($"Modifico email per il corrispondente con ID: {this._testIdCorrispondenteCreato}");

            COMMANDS.UpdateEmail.DatiEmail datiEmail = new()
            {
                Note = "modificata",
                Preferita = true
            };

            string json = JsonConvert.SerializeObject(datiEmail);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{this._controllerUrl}/{this._testIdCorrispondenteCreato}/emails/{this._testEmailToEdit}")
            {
                Content = content
            };

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                Assert.Pass("Test concluso con successo");
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(10)]
        public async Task DeleteEmail()
        {
            Assert.That(this._testIdCorrispondenteCreato, Is.Not.Empty);
            Console.WriteLine($"Elimino l'email per il corrispondente con ID: {this._testIdCorrispondenteCreato}");

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{this._controllerUrl}/{this._testIdCorrispondenteCreato}/emails/{this._testEmailToEdit}");
            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                Assert.Pass("Test concluso con successo");
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(11)]
        public async Task DeleteCorrispondente()
        {
            Assert.That(this._testIdCorrispondenteCreato, Is.Not.Empty);
            Console.WriteLine($"Elimino il corrispondente con ID: {this._testIdCorrispondenteCreato}");

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{this._controllerUrl}/{this._testIdCorrispondenteCreato}");
            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                Assert.Pass("Test concluso con successo");
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }
    }
}