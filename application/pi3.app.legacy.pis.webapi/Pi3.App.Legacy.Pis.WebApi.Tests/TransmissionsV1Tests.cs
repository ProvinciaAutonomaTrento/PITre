// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentAndAddInProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmDocModel;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmissionDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmissionProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmPrjModel;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetTransmissionModel;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetTransmissionModels;
using System.Text;

namespace Pi3.App.Legacy.Pis.WebApi.Tests
{
    public class TransmissionsV1Tests
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
        public async Task ExecuteTransmissionDocumentCommand()
        {
            var createDocumentRequest = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommand>(Files.CreateDocumentAndAddInProjectWithoutTemp_Actual);
            string idDoc = string.Empty;
            var requestMsg = TestUtils.SerializeRequest(createDocumentRequest, HttpMethod.Put, "CreateDocumentAndAddInProject");

            var createDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (createDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
                idDoc = responseDecoded.Document.Id;

            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }

            var req = JsonConvert.DeserializeObject<ExecuteTransmissionDocumentCommand>(Files.ExecuteTransmissionDocument_Actual);

            req.IdDocument = idDoc;

            var trasmMsg = TestUtils.SerializeRequest(req, HttpMethod.Post, "ExecuteTransmissionDocument");

            var resp = await this._apiHttpClient.SendAsync(trasmMsg);
            trasmMsg.Dispose();


            if (resp.IsSuccessStatusCode)
            {
                string responseContent = await resp.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<ExecuteTransmissionDocumentCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();

            }
            else
            {
                Console.WriteLine($"Errore: {resp.StatusCode}");
                string responseContent = await resp.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task ExecuteTransmPrjModelTest()
        {
            var createReq = JsonConvert.DeserializeObject<CreateProjectCommand>(Files.CreateProject_Actual);

            var requestMsg = TestUtils.SerializeRequest(createReq, HttpMethod.Put, "CreateProject");
            var createResp = await this._apiHttpClient.SendAsync(requestMsg);
            string idPrj = string.Empty;
            requestMsg.Dispose();

            if (createResp.IsSuccessStatusCode)
            {
                string responseContent = await createResp.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
                idPrj = responseDecoded.Project.Id;
            }
            else
            {
                Console.WriteLine($"Errore: {createResp.StatusCode}");
                string responseContent = await createResp.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }


            var req = JsonConvert.DeserializeObject<ExecuteTransmPrjModelCommand>(Files.ExecuteTransmPrjModel_Actual);

            req.IdProject = idPrj;

            var trasmMsg = TestUtils.SerializeRequest(req, HttpMethod.Post, "ExecuteTransmPrjModel");

            var resp = await this._apiHttpClient.SendAsync(trasmMsg);
            trasmMsg.Dispose();


            if (resp.IsSuccessStatusCode)
            {
                string responseContent = await resp.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<ExecuteTransmPrjModelCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();

            }
            else
            {
                Console.WriteLine($"Errore: {resp.StatusCode}");
                string responseContent = await resp.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task ExecuteTransmDocModelTest()
        {      
            var createDocumentRequest = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommand>(Files.CreateDocumentAndAddInProjectWithoutTemp_Actual);
            string idDoc = string.Empty;
            var requestMsg = TestUtils.SerializeRequest(createDocumentRequest, HttpMethod.Put, "CreateDocumentAndAddInProject");

            var createDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (createDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
                idDoc = responseDecoded.Document.Id;

            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }

            var req = JsonConvert.DeserializeObject<ExecuteTransmDocModelCommand>(Files.ExecuteTransmDocModel_Actual);

            req.DocumentId = idDoc;

            var trasmMsg = TestUtils.SerializeRequest(req, HttpMethod.Post, "ExecuteTransmDocModel");

            var resp = await this._apiHttpClient.SendAsync(trasmMsg);
            trasmMsg.Dispose();


            if (resp.IsSuccessStatusCode)
            {
                string responseContent = await resp.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<ExecuteTransmDocModelCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();

            }
            else
            {
                Console.WriteLine($"Errore: {resp.StatusCode}");
                string responseContent = await resp.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task GetTransmModelTest()
        {
            HttpResponseMessage response2 = await this._apiHttpClient.GetAsync("GetTransmissionModel?idModel=214270");
            if (response2.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response2.StatusCode}");
                string responseContent = await response2.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetTransmissionModelCommandResponse result = JsonConvert.DeserializeObject<GetTransmissionModelCommandResponse>(responseContent);
                if (result.TransmissionModel != null && !String.IsNullOrWhiteSpace(result?.TransmissionModel.Id))
                {
                    Console.WriteLine($"Modello di trasmissione con id {result.TransmissionModel.Id}, descrizione {result.TransmissionModel.Description}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Modello di trasmissione non trovato");
                }
            }
            else
            {
                Console.WriteLine($"Errore: {response2.StatusCode}");
                string responseContent = await response2.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task ExecTrasmDocTest()
        {
            var execTransmDoc = new ExecuteTransmissionDocumentCommand()
            {
                IdDocument = "132894925",
                Notify = true,
                TransmissionReason = "COMPETENZA",
                TransmissionType = "S",
                Receiver = new Application.Commands.AddressBook.Correspondent() { Code = "B001SEG" }
            };
            var json = JsonConvert.SerializeObject(execTransmDoc);
            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "ExecuteTransmissionDocument")
            {
                Content = content2
            };
            var response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<ExecuteTransmissionDocumentCommandResponse>(responseContent);
                if (!String.IsNullOrWhiteSpace(result?.TransmMessage) && result?.Code == Application.Commands.Transmissions.TransmissionResponseCode.OK)
                {
                    Console.WriteLine($"Trasmissione effettuata");
                    Assert.Pass(result.TransmMessage);
                }
                else
                {
                    Assert.Fail("Trasmissione non effettuata");
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

        [Test]
        public async Task ExecTrasmPrjTest()
        {
            var execTransmPrj = new ExecuteTransmissionProjectCommand()
            {
                IdProject = "141659506",
                Notify = true,
                TransmissionReason = "COMPETENZA",
                TransmissionType = "S",
                Receiver = new Application.Commands.AddressBook.Correspondent() { Code = "B001SEG" }
            };
            var json = JsonConvert.SerializeObject(execTransmPrj);
            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "ExecuteTransmissionProject")
            {
                Content = content2
            };
            var response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<ExecuteTransmissionProjectCommandResponse>(responseContent);
                if (!String.IsNullOrWhiteSpace(result?.TransmMessage) && result?.Code == Application.Commands.Transmissions.TransmissionResponseCode.OK)
                {
                    Console.WriteLine($"Trasmissione effettuata");
                    Assert.Pass(result.TransmMessage);
                }
                else
                {
                    Assert.Fail("Trasmissione non effettuata");
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

        [Test]
        public async Task GetTransmModelsTest()
        {
            var getTrasmModels = new GetTransmissionModelsCommand()
            {
                Type="D",
                Registers = new Application.Commands.Registers.Register[1]
                {
                    new Application.Commands.Registers.Register()
                    {
                         Code="PAT"
                    }
                }
            };
            var json = JsonConvert.SerializeObject(getTrasmModels);
            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "GetTransmissionModels")
            {
                Content = content2
            };
            var response = await this._apiHttpClient.SendAsync(request); 
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetTransmissionModelsCommandResponse result = JsonConvert.DeserializeObject<GetTransmissionModelsCommandResponse>(responseContent);
                if (result!= null && result.TransmissionModels != null && result.TransmissionModels.Any())
                {
                    Console.WriteLine($"Trovati {result.TransmissionModels.Length} modelli di trasmissione");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Modelli di trasmissione non trovati");
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
