// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Tests
{
    public class AggregazioniDocumentaliTests
    {
        private HttpClient _apiHttpClient;
        private readonly string _controllerUrl = "api/v1/{0}/AggregazioniDocumentali";
        private readonly string _controllerRicercheUrl = "api/v1/{0}/Ricerche";
        private string _idAggregatoCreato = string.Empty;
        private string _idFolderCancellabile = string.Empty;

        private string _idFolderTestDocumenti = string.Empty;
        private string _idDocumentoCancellabile = string.Empty;
        private string _idNotaCancellabile = string.Empty;

        private string _idTrasmissioneCreata = string.Empty;
        private string _idTrasmissioneCreataInviata = string.Empty;

        [SetUp]
        public void Setup()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

            var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(Files.Variables);

            variables?.ToList().ForEach(v => Environment.SetEnvironmentVariable(v.Key, v.Value));

            var webAppFactory = new WebApplicationFactory<Program>();
            this._apiHttpClient = webAppFactory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            this._apiHttpClient?.Dispose();
        }

        [Test, Order(1)]
        public async Task CreaAggregazioneTest()
        {
            using var content = new StringContent(Files.IndexPostTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, string.Format(this._controllerUrl, 
                Environment.GetEnvironmentVariable("instance")))
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
                var jObject = JObject.Parse(responseContent);
                var jToken = jObject.GetValue("id");
                _idAggregatoCreato = jToken.Value<string>();

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

        [Test, Order(2)]
        public async Task GetAggregazioneTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, 
                string.Format(this._controllerUrl + "/" + _idAggregatoCreato, Environment.GetEnvironmentVariable("instance")) +
                "?permessi=true&profili=true&sottoFascicoli=true&paginazioneSottoFascicoli={Salta:0,Prendi:10}&documenti=true&paginazioneDocumenti={Salta:0,Prendi:10}&note=true");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {

                string responseContent = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseContent);

                _idFolderCancellabile = (string)jObject.SelectToken("aggregazioneDocumentale.sottofascicoli[0].sottofascicoli[0].sottofascicoli[0].sottofascicoli[1].id");
                var folderName = (string)jObject.SelectToken("aggregazioneDocumentale.sottofascicoli[0].sottofascicoli[0].sottofascicoli[0].sottofascicoli[1].nome");
                Assert.AreEqual("SubFolder1.1.2", folderName);


                _idFolderTestDocumenti = (string)jObject.SelectToken("aggregazioneDocumentale.sottofascicoli[0].sottofascicoli[0].sottofascicoli[0].sottofascicoli[0].id");
                var folderTestName = (string)jObject.SelectToken("aggregazioneDocumentale.sottofascicoli[0].sottofascicoli[0].sottofascicoli[0].sottofascicoli[0].nome");
                Assert.AreEqual("SubFolder1.1.1", folderTestName);

                _idDocumentoCancellabile = (string)jObject.SelectToken("aggregazioneDocumentale.sottofascicoli[0].sottofascicoli[0].sottofascicoli[0].sottofascicoli[0].documenti[0].identiticativo");
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
        public async Task ChiusoPutTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Put, string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                    + "/" + _idAggregatoCreato + "/Chiuso");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {

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

        [Test, Order(4)]
        public async Task ApertoPutTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Put, string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                    + "/" + _idAggregatoCreato + "/Aperto");

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
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

        [Test, Order(5), Category("Folders")]
        public async Task FoldersPostTest()
        {
            using var content = new StringContent(Files.FoldersPostTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(
                HttpMethod.Post, 
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Sottofascicoli")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
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

        [Test, Order(6), Category("IdDocs")]
        public async Task IdDocsPutTest()
        {
            var idDocumento = "132722902";
            var request = new HttpRequestMessage(
                HttpMethod.Put,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Documenti/" + idDocumento);

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
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
        public async Task CollocazioneFisicaPutTest()
        {
            using var content = new StringContent(Files.CollocazioneFisicaPutTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/CollocazioneFisica")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
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
        public async Task DescrizionePutTest()
        {
            using var content = new StringContent(Files.DescrizionePutTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Descrizione")
            {
                Content = content
            };

            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
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

        // Handler: IdDocsDeleteCommand
        [Test, Order(9), Category("IdDocs")]
        public async Task IdDocDeleteTest()
        {
            var idDocumento = "132722902";
            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Documenti/" + idDocumento);

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        // Handler: FoldersDeleteCommand
        [Test, Order(10), Category("Folders")]
        public async Task IdFolderDeleteTest()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Sottofascicoli/" + _idFolderCancellabile);

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        // Handler: IdDocsFolderDeleteCommand
        [Test, Order(11), Category("Folders")]
        public async Task IdFolderDocDeleteTest()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Sottofascicoli/" + _idFolderTestDocumenti + "/Documenti/" + _idDocumentoCancellabile);

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        // Handler: IdDocsFolderAddCommand
        [Test, Order(12), Category("Folders")]
        public async Task IdFolderDocPutTest()
        {
            var idDocumento = "132676097";

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Sottofascicoli/" + _idFolderTestDocumenti + "/Documenti/" + idDocumento);

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        // Handler: 
        [Test, Order(13), Category("Note")]
        public async Task NotePostTest()
        {
            using var content = new StringContent(Files.NotePostTest_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Note/")
            {
                Content = content
            };

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                _idNotaCancellabile = await this.GetIdFromResponse(response);
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

        // Handler: 
        [Test, Order(14), Category("Note")]
        public async Task NoteDeleteTest()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Note/" + _idNotaCancellabile);
            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        // Handler: 
        [Test, Order(15), Category("Trasmissioni")]
        public async Task TrasmissionePostTest()
        {
            using var content = new StringContent(Files.TrasmissioniPost_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Trasmissioni/")
            {
                Content = content
            };

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseContent);
                var jToken = jObject.GetValue("id");
                _idTrasmissioneCreata = jToken.Value<string>();
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

        // Handler: 
        [Test, Order(16), Category("Trasmissioni")]
        public async Task TrasmissioneGetTest()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Trasmissioni/"+ _idTrasmissioneCreata);

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }


        // Handler: 
        [Test, Order(17), Category("Trasmissioni")]
        public async Task TrasmissionePutTest()
        {
            using var content = new StringContent(Files.TrasmissioniPut_Actual, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(
                HttpMethod.Put,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Trasmissioni/" + _idTrasmissioneCreata)
            {
                Content = content
            };

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }


        // Handler: 
        [Test, Order(18), Category("Trasmissioni")]
        public async Task ListaTrasmissioneGetTest()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Trasmissioni/Infos");

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        // Handler: 
        [Test, Order(19), Category("Trasmissioni")]
        public async Task TrasmissioneInvioPutTest()
        {
            // creazione trasmissione
            var request = GetRequest(HttpMethod.Post, _idAggregatoCreato, 
                "Trasmissioni/", Files.TrasmissioniPost_Actual);
            var response = await this._apiHttpClient.SendAsync(request);
            var idTrasmissione = await GetIdHandleResponse(response);


            var body = new
            {
                id = _idTrasmissioneCreata,
                Data = DateTime.Now
            };
            var stringBody = JsonConvert.SerializeObject(body);
            request = GetRequest(HttpMethod.Put, _idAggregatoCreato, 
                "Trasmissioni/" + idTrasmissione + "/Invio", stringBody);
            response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        // Handler: 
        [Test, Order(20), Category("Trasmissioni")]
        public async Task TrasmissioneAccettazionePutTest()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Put,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Trasmissioni/"+ _idTrasmissioneCreata + "/Accettazione");

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        [Test, Order(21), Category("Trasmissioni")]
        public async Task TrasmissioneRifiutoPutTest()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Put,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Trasmissioni/" + _idTrasmissioneCreata + "/Rifiuto");

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        [Test, Order(22), Category("Trasmissioni")]
        public async Task TrasmissioneVistoPutTest()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Put,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Trasmissioni/" + _idTrasmissioneCreata + "/Visto");

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        [Test, Order(23), Category("Trasmissioni")]
        public async Task TrasmissioneDeleteTest()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance"))
                + "/" + _idAggregatoCreato + "/Trasmissioni/" + _idTrasmissioneCreata);

            SetHeaders(request);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        [Test, Order(24), Category("Trasmissioni")]
        public async Task PostTrasmissioneModelloTest()
        {
            var request = GetRequest(HttpMethod.Post, _idAggregatoCreato,
                "Trasmissioni/Modello/" + "MD_218046");
            var response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        // Handler: 
        [Test, Order(25), Category("Trasmissioni")]
        public async Task TrasmissioneUtentiPostTest()
        {
            // creazione trasmissione
            var request = GetRequest(HttpMethod.Post, _idAggregatoCreato,
                "Trasmissioni/", Files.TrasmissioniUtentiPost_Actual);
            var response = await this._apiHttpClient.SendAsync(request);
            var idTrasmissione = await GetIdHandleResponse(response);
        }
        
        // Handler: 
        [Test, Order(26), Category("Trasmissioni")]
        public async Task TrasmissioneAppendPutTest()
        {
            // creazione trasmissione
            var idTrasmissione = await GetidNewTrasmissione();

            // aggiornamento trasmissione
            var request = GetRequest(HttpMethod.Put, _idAggregatoCreato,
                "Trasmissioni/"+ idTrasmissione, Files.TrasmissioniAppendPut_Actual);
            var response = await this._apiHttpClient.SendAsync(request);
            await StandardHandleResponse(response);
        }

        [Test, Order(26), Category("Profili")]
        public async Task ProfiloPostTest() {
            // creazione aggregato
            var idTrasmissione = await GetidNewAggregazione();

            var request = GetRequest(HttpMethod.Post, idTrasmissione,
                "Profilo/", Files.ProfiloPost_Actual);
            var response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }

        [Test, Order(27), Category("Query")]
        public async Task PostYearFilter()
        {
            var request = GetRequest(HttpMethod.Get, null,
                "RicercaPerRegistro/?anno=2024&codRegistro=PAT&paginazione={Salta:0,Prendi:100}", null, true);
            var response = await this._apiHttpClient.SendAsync(request);

            await StandardHandleResponse(response);
        }


        #region private methods
        private async Task<string> GetidNewAggregazione()
        {
            var request = GetRequest(HttpMethod.Post, string.Empty,
                string.Empty, Files.IndexPostTest_Actual);
            var response = await this._apiHttpClient.SendAsync(request);
            var idTrasmissione = await GetIdHandleResponse(response);
            return idTrasmissione;
        }

        private async Task<string> GetidNewTrasmissione() {
            var request = GetRequest(HttpMethod.Post, _idAggregatoCreato,
                "Trasmissioni/", Files.TrasmissioniPost_Actual);
            var response = await this._apiHttpClient.SendAsync(request);
            var idTrasmissione = await GetIdHandleResponse(response);
            return idTrasmissione;
        }

        private void SetHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));
        }

        private async Task StandardHandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
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

        private async Task<string> GetIdHandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseContent);
                var jToken = jObject.GetValue("id");
                return jToken.Value<string>();
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
                return null;
            }

        }

        private async Task<string> GetIdFromResponse(HttpResponseMessage response) 
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseContent);
            var jToken = jObject.GetValue("id");
            if(jToken != null)
            {
                var id = jToken.Value<string>();
                return id;
            }
            return null;
        }

        private HttpRequestMessage GetRequest(HttpMethod method, string idAggregato, string path, 
            string payload = null,
            bool ricerche = false) 
        {
            var url = ricerche ? this._controllerRicercheUrl : this._controllerUrl;

            HttpRequestMessage request = null;
            if (payload == null)
            {
                if (!string.IsNullOrWhiteSpace(idAggregato))
                {
                    request = new HttpRequestMessage(
                        method,
                        string.Format(url, Environment.GetEnvironmentVariable("instance"))
                        + "/" + idAggregato + "/" + path);
                }
                else {
                    request = new HttpRequestMessage(
                        method,
                        string.Format(url, Environment.GetEnvironmentVariable("instance"))
                        + "/" + path);
                }
            }
            else {
                var content = new StringContent(payload, Encoding.UTF8, "application/json");

                if (!string.IsNullOrWhiteSpace(idAggregato) && !string.IsNullOrWhiteSpace(path))
                {
                    request = new HttpRequestMessage(
                        method,
                        string.Format(url, Environment.GetEnvironmentVariable("instance"))
                        + "/" + idAggregato + "/" + path)
                    {
                        Content = content
                    };
                }
                else {
                    request = new HttpRequestMessage(
                        method,
                        string.Format(url, Environment.GetEnvironmentVariable("instance"))
                        )
                    {
                        Content = content
                    };
                }
            }
            SetHeaders(request);
            return request;
        }

        #endregion
    }
}