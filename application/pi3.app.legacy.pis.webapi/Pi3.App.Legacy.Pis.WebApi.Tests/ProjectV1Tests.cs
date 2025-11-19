// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateFolder;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateProjectWithArchivePlan;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.EditProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetArchivePlan;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetArchivePlanFilters;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectFilters;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectFolders;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectsByDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectStateDiagram;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectTemplates;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectWithArchivePlan;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.OpenCloseProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SearchArchivePlans;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SearchProjects;
using Pi3.Core.Extensions;
using RabbitMQ.Client;
using System.Text;

namespace Pi3.App.Legacy.Pis.WebApi.Tests
{
    public class ProjectV1Tests
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
        public async Task CreateProjectWithArchivePlanAndTemplate()
        {
            var searchProjects = JsonConvert.DeserializeObject<CreateProjectWithArchivePlanCommand>(Files.CreateProjWithArchivePlanAndTemplate_Actual);
            var requestMsg = TestUtils.SerializeRequest(searchProjects, HttpMethod.Put, "CreateProjectWithArchivePlan");
            var createProjResp = await this._apiHttpClient.SendAsync(requestMsg);

            requestMsg.Dispose();

            if (createProjResp.IsSuccessStatusCode)
            {
                string responseContent = await createProjResp.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateProjectWithArchivePlanCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();

            }
            else
            {
                Console.WriteLine($"Errore: {createProjResp.StatusCode}");
                string responseContent = await createProjResp.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task SearchProjectsTest()
        {
            var searchProjects = JsonConvert.DeserializeObject<SearchProjectsCommand>(Files.SearchProjects_Actual);
            var requestMsg = TestUtils.SerializeRequest(searchProjects, HttpMethod.Post, "SearchProjects");
            var searchProjectsResp = await this._apiHttpClient.SendAsync(requestMsg);

            requestMsg.Dispose();

            if (searchProjectsResp.IsSuccessStatusCode)
            {
                string responseContent = await searchProjectsResp.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<SearchProjectsCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();

            }
            else
            {
                Console.WriteLine($"Errore: {searchProjectsResp.StatusCode}");
                string responseContent = await searchProjectsResp.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task EditProjectTest()
        {
            var editProject = JsonConvert.DeserializeObject<EditProjectCommand>(Files.EditProject_Actual);
            editProject.Project.Description = string.Join("_",editProject.Project.Description, DateTime.Now.AsDateTimeFormat());
            var requestMsg = TestUtils.SerializeRequest(editProject, HttpMethod.Post, "EditProject");
            var editProjectResp = await this._apiHttpClient.SendAsync(requestMsg);

            requestMsg.Dispose();

            if (editProjectResp.IsSuccessStatusCode)
            {
                string responseContent = await editProjectResp.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<EditProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();

            }
            else
            {
                Console.WriteLine($"Errore: {editProjectResp.StatusCode}");
                string responseContent = await editProjectResp.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task CreateFolderTest()
        {
            var createFolder = JsonConvert.DeserializeObject<CreateFolderCommand>(Files.CreateFolder_Actual);
            createFolder.FolderDescription = string.Join("_",createFolder.FolderDescription, DateTime.Now.AsDateTimeFormat());
            var requestMsg = TestUtils.SerializeRequest(createFolder, HttpMethod.Put, "CreateFolder");
            var createFolderResp = await this._apiHttpClient.SendAsync(requestMsg);

            requestMsg.Dispose();

            if (createFolderResp.IsSuccessStatusCode)
            {
                string responseContent = await createFolderResp.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateFolderCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();

            }
            else
            {
                Console.WriteLine($"Errore: {createFolderResp.StatusCode}");
                string responseContent = await createFolderResp.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }

        }
        [Test]
        public async Task GetProjectFiltersTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetProjectFilters");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetProjectFiltersCommandResponse>(responseContent);
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
        public async Task GetArchivePlanFiltersTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetArchivePlanFilters");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetArchivePlanFiltersCommandResponse>(responseContent);
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
        public async Task GetProjectTemplatesTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetProjectTemplates");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetProjectTemplatesCommandResponse>(responseContent);
                if (result.Templates != null && result.Templates.Any())
                {
                    Console.WriteLine($"{result.Templates.Length} Template trovati:");
                    foreach (var f in result.Templates)
                    {
                        Console.WriteLine($"Id: {f.Id}, {f.Name}");
                    }
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Template non trovati");
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
        public async Task GetArchivePlanTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetArchivePlan_Actual);

            var response = await this._apiHttpClient.GetAsync("GetArchivePlan" + queryString);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetArchivePlanCommandResponse>(responseContent);
                if (result.ArchivePlan != null && !string.IsNullOrWhiteSpace(result.ArchivePlan.Id))
                {
                    Console.WriteLine($"Piano conservazione trovato. ID {result.ArchivePlan.Id}, Nodo {result.ArchivePlan.ClassificationNodeCode}, Descrizione: {result.ArchivePlan.Description}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Piano di conservazione non trovato");
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
        public async Task getProjectTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetProject_Actual);

            var response = await this._apiHttpClient.GetAsync("GetProject" + queryString);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetProjectCommandResponse>(responseContent);
                if (result.Project != null && !string.IsNullOrWhiteSpace(result.Project.Id))
                {
                    Console.WriteLine($"Fascicolo trovato. ID {result.Project.Id}, codice {result.Project.Code}, Descrizione: {result.Project.Description}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Fascicolo non trovato");
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
        public async Task getProjectWithArchivePlanTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetProjectWithArchivePlan_Actual);

            var response = await this._apiHttpClient.GetAsync("GetProjectWithArchivePlan" + queryString);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetProjectWithArchivePlanCommandResponse>(responseContent);
                if (result.Project != null && !string.IsNullOrWhiteSpace(result.Project.Id))
                {
                    Console.WriteLine($"Fascicolo trovato. ID {result.Project.Id}, codice {result.Project.Code}, Descrizione: {result.Project.Description}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Fascicolo non trovato");
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
        public async Task GetProjectFoldersTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetProjectFolders_Actual);

            var response = await this._apiHttpClient.GetAsync("GetProjectFolders" + queryString);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetProjectFoldersCommandResponse>(responseContent);
                if (result.Folders != null && result.Folders.Any())
                {
                    Console.WriteLine($"{result.Folders.Length} Cartelle trovate nel fascicolo {result.Code}");
                    foreach (var f in result.Folders)
                    {
                        Console.WriteLine($"Id: {f.Id}, {f.Description}");
                    }
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Cartelle non trovate");
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
        public async Task getProjectTemplateTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetProjectTemplate_Actual);

            var response = await this._apiHttpClient.GetAsync("GetTemplateProject" + queryString);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetTemplateProjectCommandResponse>(responseContent);
                if (result.Template != null && !string.IsNullOrWhiteSpace(result.Template.Id))
                {
                    Console.WriteLine($"Template trovato. ID {result.Template.Id}, Descrizione: {result.Template.Name}");
                    if (result.Template.Fields != null && result.Template.Fields.Any())
                    {
                        Console.WriteLine($"Trovati {result.Template.Fields.Length} campi:");
                        foreach (var f in result.Template.Fields)
                        {
                            Console.WriteLine($"Id: {f.Id}, {f.Name}");
                        }
                    }
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Template non trovato");
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
        public async Task GetProjectsByDocumentTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetProjectsByDocument_Actual);

            var response = await this._apiHttpClient.GetAsync("GetProjectsByDocument" + queryString);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetProjectsByDocumentCommandResponse>(responseContent);
                if (result.Projects != null && result.Projects.Any())
                {
                    Console.WriteLine($"{result.TotalProjectsNumber} fascicoli trovati");

                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Fascicoli non trovati");
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
        public async Task GetProjectStateDiagramTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetProjectStateDiagram_Actual);

            var response = await this._apiHttpClient.GetAsync("GetProjectStateDiagram" + queryString);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetProjectStateDiagramCommandResponse>(responseContent);
                if (result.StateOfDiagram != null && !string.IsNullOrWhiteSpace(result.StateOfDiagram.Id))
                {
                    Console.WriteLine($"Stato del diagramma trovato. ID {result.StateOfDiagram.Id}, Descrizione: {result.StateOfDiagram.Description}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Stato del diagramma non trovato");
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
        public async Task searchArchivePlansTest()
        {
            var filters = JsonConvert.DeserializeObject<List<Filter>>(Files.searchArchivePlans_Actual);

            var searchAPRequest = new SearchArchivePlansCommand()
            {
                Filters = filters.ToArray(),
                PageNumber = 1,
                ElementsInPage = 20
            };
            var json = JsonConvert.SerializeObject(searchAPRequest);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "SearchArchivePlans")
            {
                Content = content
            };
            var response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<SearchArchivePlansCommandResponse>(responseContent);
                if (result.ArchivePlans != null && result.ArchivePlans.Any())
                {
                    Console.WriteLine($"{result.TotalArchivePlansNumber} piani di conservazione trovati relativi ai filtri inseriti");

                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Piani di conservazione non trovati");
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
        public async Task OpenCloseProjectTest()
        {
            var reqOpClose = JsonConvert.DeserializeObject<OpenCloseProjectCommand>(Files.OpenCloseProject_Actual);
            reqOpClose!.Action = "C";

            var json = JsonConvert.SerializeObject(reqOpClose);
            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "OpenCloseProject")
            {
                Content = content2
            };
            var response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<OpenCloseProjectCommandResponse>(responseContent);
                if (result!= null && result.Project!= null && !result.Project.Open)
                {
                    Console.WriteLine($"Effettuata chiusura del fascicolo {result.Project.Code} in data {result.Project.ClosureDate}");
                    
                }
                else
                {
                    Assert.Fail("Chiusura fascicolo non effettuata");
                }
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }

            reqOpClose!.Action = "O";

            json = JsonConvert.SerializeObject(reqOpClose);
            using var content3 = new StringContent(json, Encoding.UTF8, "application/json");
            request = new HttpRequestMessage(HttpMethod.Post, "OpenCloseProject")
            {
                Content = content3
            };
            response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<OpenCloseProjectCommandResponse>(responseContent);
                if (result != null && result.Project != null && result.Project.Open)
                {
                    Console.WriteLine($"Effettuata apertura del fascicolo {result.Project.Code} in data {result.Project.OpeningDate}");
                    Assert.Pass("Test effettuato con successo");
                }
                else
                {
                    Assert.Fail("Apertura fascicolo non effettuata");
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
        public async Task CreateProjectTest()
        {
            var createReq = JsonConvert.DeserializeObject<CreateProjectCommand>(Files.CreateProject_Actual);

            var requestMsg = TestUtils.SerializeRequest(createReq, HttpMethod.Put, "CreateProject");
            var createResp = await this._apiHttpClient.SendAsync(requestMsg);

            requestMsg.Dispose();

            if (createResp.IsSuccessStatusCode)
            {
                string responseContent = await createResp.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {createResp.StatusCode}");
                string responseContent = await createResp.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }
        record CreateProjWithArchivePlanTestInput(string idClassificationScheme, string codeNodeClassification, string idArchivePlan, string registerCode);

        [Test]
        public async Task CreateProjWithArchivePlanTest()
        {
            var input = JsonConvert.DeserializeObject<CreateProjWithArchivePlanTestInput>(Files.CreateProjWithArchivePlan_Actual);
            
            var createPrj = new CreateProjectWithArchivePlanCommand()
            {
                Project = new Application.Commands.Projects.ProjectWithArchivePlan()
                {
                    Description = "Creazione Project con piano Conservazione da Test PITRE Core - " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"),
                    ClassificationScheme = new Application.Commands.ClassificationSchemes.ClassificationScheme() { Id = input!.idClassificationScheme },
                    CodeNodeClassification = input!.codeNodeClassification,
                    ArchivePlan= new Application.Commands.Projects.ArchivePlan() { Id = input!.idArchivePlan },
                    Register = new Application.Commands.Registers.Register() {  Code = input!.registerCode}
                }
            };

            var json = JsonConvert.SerializeObject(createPrj);
            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, "CreateProjectWithArchivePlan")
            {
                Content = content2
            };
            
            var response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<CreateProjectWithArchivePlanCommandResponse>(responseContent);
                if ( result != null && result.Project!= null && !String.IsNullOrWhiteSpace(result.Project.Id))
                {
                    Console.WriteLine($"Creato fascicolo con id {result.Project.Id} e codice {result.Project.Code}");
                    Assert.Pass("Test passato con successo");
                }
                else
                {
                    Assert.Fail("Errore nella creazione del fascicolo");
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
