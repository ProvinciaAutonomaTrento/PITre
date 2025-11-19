// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchCorrespondentsAdvanced;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetInstanceSearchFilters;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignatureProcess;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignatureProcesses;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignProcessInstance;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.SearchSignProcessInstances;
using System.Text;

namespace Pi3.App.Legacy.Pis.WebApi.Tests
{
    public class SignBookV1Tests
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
        public async Task GetSignProcessInstanceTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetSignProcessInstance_Actual);

            var response = await this._apiHttpClient.GetAsync("GetSignProcessInstance" + queryString);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GetSignProcessInstanceCommandResponse>(responseContent);

                if (result == null || !string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Assert.Fail(string.Format(Messages.TestFailed, "GetSignProcessInstance"));
                }
                Assert.Pass(string.Format(Messages.TestPassed, "GetSignProcessInstance"));
            }
            else
            {
                Assert.Fail(string.Format(Messages.TestFailed, "GetSignProcessInstance"));
            }

        }

        [Test]
        public async Task SearchSignProcessInstancesTest()
        {
            var request = JsonConvert.DeserializeObject<SearchSignProcessInstancesCommand>(Files.SearchSignProcessInstances_Actual);
            var requestMsg = TestUtils.SerializeRequest(request, HttpMethod.Post, "SearchSignProcessInstances");
            var response = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SearchSignProcessInstancesCommandResponse>(responseContent);

                if (result == null || !string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Assert.Fail(string.Format(Messages.TestFailed, "SearchSignProcessInstances"));
                }
                Assert.Pass(string.Format(Messages.TestPassed, "SearchSignProcessInstances"));
            }
            else
            {
                Assert.Fail(string.Format(Messages.TestFailed, "SearchSignProcessInstances"));
            }
        }

        [Test]
        public async Task GetInstanceSearchFilters()
        {
            var response = await this._apiHttpClient.GetAsync("GetInstanceSearchFilters");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetInstanceSearchFiltersCommandResponse result = JsonConvert.DeserializeObject<GetInstanceSearchFiltersCommandResponse>(responseContent);
                if (result.Filters != null && result.Filters.Any())
                {
                    Console.WriteLine($"Filtri trovati");
                    foreach (var f in result.Filters)
                    {
                        Console.WriteLine(f.Name);
                    }
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Filtri non trovati");
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
        public async Task GetSignatureProcessTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetSignatureProcess?idProcess=281");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetSignatureProcessCommandResponse result = JsonConvert.DeserializeObject<GetSignatureProcessCommandResponse>(responseContent);
                if (result.SignatureProcess != null && !string.IsNullOrEmpty(result.SignatureProcess.IdProcess))
                {
                    Console.WriteLine($"Prelevato il dettaglio del processo di firma {result.SignatureProcess.IdProcess}, desc {result.SignatureProcess.Name}, n passi {result.SignatureProcess.Steps.Count}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Processo di firma non trovato.");
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
        public async Task GetSignatureProcessesTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetSignatureProcesses");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();

                GetSignatureProcessesCommandResponse result = JsonConvert.DeserializeObject<GetSignatureProcessesCommandResponse>(responseContent);
                if (result.Processes != null && result.Processes.Length>0)
                {
                    Console.WriteLine($"Prelevati i processi di firma. Trovati {result.TotalProcessesNumber} processi.");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Processo di firma non trovato.");
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
