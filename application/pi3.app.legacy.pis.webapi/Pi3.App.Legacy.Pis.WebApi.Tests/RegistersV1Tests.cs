// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers.GetRegisterOrRF;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers.GetRegistersOrRF;
using System.Text;

namespace Pi3.App.Legacy.Pis.WebApi.Tests
{
    public class RegistersV1Tests
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
        public async Task GetRegistersOrRFTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetRegistersOrRF?codeRole=D319SEG");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetRegistersOrRFCommandResponse result = JsonConvert.DeserializeObject<GetRegistersOrRFCommandResponse>(responseContent);
                if (result != null && result.Registers != null && result.Registers.Any())
                {
                    Console.WriteLine($"Trovati {result.Registers.Length} registri");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Registri non trovati.");
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
        public async Task GetRegisterOrRFTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetRegisterOrRF?idRegister=86107");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetRegisterOrRFCommandResponse result = JsonConvert.DeserializeObject<GetRegisterOrRFCommandResponse>(responseContent);
                if (result != null && result.Register != null && !string.IsNullOrWhiteSpace(result.Register.Id))
                {
                    Console.WriteLine($"Registro Id {result.Register.Id}, Codice {result.Register.Code}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Registro non trovato.");
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
