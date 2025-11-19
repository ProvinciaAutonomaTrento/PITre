// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetActiveClassificationScheme;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetAllClassificationSchemes;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetClassificationSchemeById;
using System.Text;

namespace Pi3.App.Legacy.Pis.WebApi.Tests
{
    public class ClassificationSchemesV1Tests
    {
        private HttpClient _apiHttpClient;

        [OneTimeSetUp]
        public async Task Setup()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
            }

            var variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(Files.Variables);
            variables?.ToList().ForEach(v => Environment.SetEnvironmentVariable(v.Key, v.Value));

            WebApplicationFactory<Program> webAppFactory = new();
            this._apiHttpClient = webAppFactory.CreateClient();

            await TestUtils.AddAuthToken(this._apiHttpClient);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            this._apiHttpClient?.Dispose();
        }

        [Test]
        public async Task GetActiveClassificationSchemeTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetActiveClassificationScheme");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetActiveClassificationSchemeCommandResponse result = JsonConvert.DeserializeObject<GetActiveClassificationSchemeCommandResponse>(responseContent);
                if (result.ClassificationScheme != null && !string.IsNullOrWhiteSpace(result.ClassificationScheme.Id))
                {
                    Console.WriteLine($"Titolario attivo, ID: {result.ClassificationScheme.Id}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Titolario attivo non trovato.");
                }
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task GetAllClassificationSchemesTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetAllClassificationSchemes");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetAllClassificationSchemesCommandResponse result = JsonConvert.DeserializeObject<GetAllClassificationSchemesCommandResponse>(responseContent);
                if (result != null && result.ClassificationSchemes!= null && result.ClassificationSchemes.Any())
                {
                    Console.WriteLine($"Titolari trovati {result.ClassificationSchemes.Length}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Titolari non trovati.");
                }
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task GetClassificationSchemeByIdTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetClassificationSchemeById?idClassificationScheme=130275944");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetClassificationSchemeByIdCommandResponse result = JsonConvert.DeserializeObject<GetClassificationSchemeByIdCommandResponse>(responseContent);
                if (result != null && result.ClassificationScheme != null && result.ClassificationScheme.Id != null)
                {
                    Console.WriteLine($"Titolario trovato, ID: {result.ClassificationScheme.Id}, descrizione: {result.ClassificationScheme.Description}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Titolario non trovato.");
                }
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
