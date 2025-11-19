// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


using QUERY = Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti;
using COMMANDS = Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti;

namespace Pi3.App.RubricaComune.WebApi.Tests
{
    public class CorrispondentiIpaV1ApiTests
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
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
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

        [Test]
        public async Task SearchCorrispondentiIpa()
        {
            Application.Queries.Corrispondenti.Search.SearchIpaRequest searchRequest = new()
            {
                ElementiPerPagina = 10,
                Pagina = 1,
                CriteriRicerca = new[] { 
                    
                    new Application.Queries.Corrispondenti.Search.CriterioRicerca() {
                        Campo = Application.Queries.Corrispondenti.Search.CampiRicercaEnum.Aoo,
                        Valore = "ADE502D"
                    },
                    new Application.Queries.Corrispondenti.Search.CriterioRicerca() {
                        Campo = Application.Queries.Corrispondenti.Search.CampiRicercaEnum.RubricaEsterna,
                        Valore = "IPA"
                    },
                    new Application.Queries.Corrispondenti.Search.CriterioRicerca() {
                        Campo = Application.Queries.Corrispondenti.Search.CampiRicercaEnum.Denominazione,
                        Valore = "TEST"
                    },
                    new Application.Queries.Corrispondenti.Search.CriterioRicerca() {
                        Campo = Application.Queries.Corrispondenti.Search.CampiRicercaEnum.Email,
                        Valore = "enis01900t@pec.istruzione.it"
                    },
                    new Application.Queries.Corrispondenti.Search.CriterioRicerca() {
                        Campo = Application.Queries.Corrispondenti.Search.CampiRicercaEnum.CodiceFiscale,
                        Valore = "test"
                    },
                    new Application.Queries.Corrispondenti.Search.CriterioRicerca() {
                        Campo = Application.Queries.Corrispondenti.Search.CampiRicercaEnum.PartitaIva,
                        Valore = "test"
                    },
                    new Application.Queries.Corrispondenti.Search.CriterioRicerca() {
                        Campo = Application.Queries.Corrispondenti.Search.CampiRicercaEnum.Codice,
                        Valore = "iisft-A7DFE71"
                    }
                },

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
                QUERY.Search.SearchIpaResponse? result = JsonConvert.DeserializeObject<QUERY.Search.SearchIpaResponse>(responseContent);

                Assert.That(result?.Corrispondenti?.Any(), Is.True);
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
