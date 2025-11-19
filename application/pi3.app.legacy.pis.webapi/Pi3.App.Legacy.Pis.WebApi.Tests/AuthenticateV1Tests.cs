// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;

namespace Pi3.App.Legacy.Pis.WebApi.Tests
{
    public class AuthenticateV1Tests
    {
        private HttpClient _apiHttpClient;

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

        [Test]
        public async Task GetTokenNewTest()
        {
            AuthenticateCommand getTokenRequest = new AuthenticateCommand()
            {
                CodeAdm = Environment.GetEnvironmentVariable("tenant")!,
                CodeApplication = Environment.GetEnvironmentVariable("codeApp")!,
                CodeRole = "",
                Username = Environment.GetEnvironmentVariable("userId")!
            };

            string json = JsonConvert.SerializeObject(getTokenRequest);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "GetToken")
            {
                Content = content
            };
            //request.Headers.Add("Content-Type", "application/json");
            //request.Headers.Add("Instance", Environment.GetEnvironmentVariable("instance"));
            _apiHttpClient.DefaultRequestHeaders.Add("Instance", Environment.GetEnvironmentVariable("instance"));

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                AuthenticateCommandResponse result = JsonConvert.DeserializeObject<AuthenticateCommandResponse>(responseContent);
                if (!String.IsNullOrWhiteSpace(result?.Token))
                {
                    Console.WriteLine($"Token Generato: {result.Token}");
                    Assert.Pass("Test concluso con successo");
                }
                else
                {
                    Assert.Fail("Token non generato");
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

        
    }
}
