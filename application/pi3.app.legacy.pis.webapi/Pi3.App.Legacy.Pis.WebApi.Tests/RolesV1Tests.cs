// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetRole;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetRoles;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetRolesForEnabledActions;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetUsersInRole;
using System.Text;

namespace Pi3.App.Legacy.Pis.WebApi.Tests
{
    public class RolesV1Tests
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
        public async Task GetRolesTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetRoles?userId=a.lembo");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetRolesCommandResponse result = JsonConvert.DeserializeObject<GetRolesCommandResponse>(responseContent);
                if (result != null && result.Roles != null && result.Roles.Any())
                {
                    Console.WriteLine($"Trovati {result.Roles.Length} ruoli");
                    foreach(var r in result.Roles)
                    {
                        Console.WriteLine($"Id {r.Id}, codice {r.Code}, descrizione: {r.Description}");
                    }
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Ruoli non trovati.");
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
        public async Task GetRoleTest()
        {
            string instance = Environment.GetEnvironmentVariable("instance");
        
            var response = await this._apiHttpClient.GetAsync("GetRole?codeRole=D319SEG");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetRoleCommandResponse result = JsonConvert.DeserializeObject<GetRoleCommandResponse>(responseContent);
                if (result != null && result.Role != null && !string.IsNullOrWhiteSpace(result.Role.Id))
                {
                    Console.WriteLine($"Ruolo trovato. Id {result.Role.Id}, codice {result.Role.Code}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Ruolo non trovato.");
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
        public async Task GetUsersInRoleTest()
        {
            string instance = Environment.GetEnvironmentVariable("instance");

            var response = await this._apiHttpClient.GetAsync("GetUsersInRole?codeRole=D319SEG");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetUsersInRoleCommandResponse result = JsonConvert.DeserializeObject<GetUsersInRoleCommandResponse>(responseContent);
                if (result != null && result.Users != null && result.Users.Any())
                {
                    Console.WriteLine($"Trovati {result.Users.Length} utenti");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Utenti non trovati.");
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
        public async Task GetRolesForEnabledActionsTest()
        {
            string instance = Environment.GetEnvironmentVariable("instance");

            var response = await this._apiHttpClient.GetAsync("GetRolesForEnabledActions?userId=a.lembo&codeFunction=GESTREG");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetRolesForEnabledActionsCommandResponse result = JsonConvert.DeserializeObject<GetRolesForEnabledActionsCommandResponse>(responseContent);
                if (result != null && result.Roles != null && result.Roles.Any())
                {
                    Console.WriteLine($"Trovati {result.Roles.Length} ruoli");
                    foreach (var r in result.Roles)
                    {
                        Console.WriteLine($"Id {r.Id}, codice {r.Code}, descrizione: {r.Description}");
                    }
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Ruoli non trovati.");
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
