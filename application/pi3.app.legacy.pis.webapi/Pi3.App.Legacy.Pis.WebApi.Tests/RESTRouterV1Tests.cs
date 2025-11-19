// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddCorrespondent;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchUsers;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetActiveClassificationScheme;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetClassificationSchemeById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.Text;

namespace Pi3.App.Legacy.Pis.WebApi.Tests
{
    [Ignore("Test ignorato temporaneamente")]
    public class RESTRouterV1Tests
    {
        private HttpClient _apiHttpClient;

        [SetUp]
        public void Setup()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
            }
            WebApplicationFactory<Program> webAppFactory = new();
            this._apiHttpClient = webAppFactory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            this._apiHttpClient?.Dispose();
        }

        //[Test]
        //public async Task GetTokenRoutingTest()
        //{
        //    AuthenticateCommand getTokenRequest = new AuthenticateCommand()
        //    {
        //        CodeAdm = "PAT_TEST",
        //        CodeApplication = "AUTOTEST PI3CORE",
        //        CodeRole = "",
        //        Username = "A.lembo"
        //    };

        //    string json = JsonConvert.SerializeObject(getTokenRequest);
        //    using var content = new StringContent(json, Encoding.UTF8, "application/json");
        //    var request = new HttpRequestMessage(HttpMethod.Post, "RouteRestRequest")
        //    {
        //        Content = content
        //    };
        //    _apiHttpClient.DefaultRequestHeaders.Add("CODE_ADM", "PAT_TEST");
        //    _apiHttpClient.DefaultRequestHeaders.Add("ROUTED_ACTION", "GetToken");
        //    _apiHttpClient.DefaultRequestHeaders.Add("APPLICATION_NAME", "APP1");


        //    HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine($"StatusCode: {response.StatusCode}");
        //        string responseContent = await response.Content.ReadAsStringAsync();
        //        Console.WriteLine($"Risposta JSON: {responseContent}");

        //        AuthenticateCommandResponse result = JsonConvert.DeserializeObject<AuthenticateCommandResponse>(responseContent);
        //        if (!String.IsNullOrWhiteSpace(result?.Token))
        //        {
        //            Console.WriteLine($"Token Generato: {result.Token}");
        //            Assert.Pass("Test concluso con successo");
        //        }
        //        else
        //        {
        //            Assert.Fail("Token non generato");
        //        }
        //    }
        //    else
        //    {
        //        // Gestisci qui la risposta fallita
        //        Console.WriteLine($"Errore: {response.StatusCode}");
        //        string responseContent = await response.Content.ReadAsStringAsync();
        //        Assert.Fail(responseContent);
        //    }
        //}

        //[Test]
        //public async Task GetActClassSchemeRouting()
        //{
        //    #region Token routing
        //    string token = "";
        //    AuthenticateCommand getTokenRequest = new AuthenticateCommand()
        //    {
        //        CodeAdm = "PAT_TEST",
        //        CodeApplication = "AUTOTEST PI3CORE",
        //        CodeRole = "",
        //        Username = "A.lembo"
        //    };

        //    string json = JsonConvert.SerializeObject(getTokenRequest);
        //    using var content = new StringContent(json, Encoding.UTF8, "application/json");
        //    var request = new HttpRequestMessage(HttpMethod.Post, "RouteRestRequest")
        //    {
        //        Content = content
        //    };
        //    _apiHttpClient.DefaultRequestHeaders.Add("CODE_ADM", "PAT_TEST");
        //    _apiHttpClient.DefaultRequestHeaders.Add("ROUTED_ACTION", "GetToken");
        //    _apiHttpClient.DefaultRequestHeaders.Add("APPLICATION_NAME", "APP1");


        //    HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine($"StatusCode: {response.StatusCode}");
        //        string responseContent = await response.Content.ReadAsStringAsync();
        //        Console.WriteLine($"Risposta JSON: {responseContent}");

        //        AuthenticateCommandResponse result = JsonConvert.DeserializeObject<AuthenticateCommandResponse>(responseContent);
        //        if (!String.IsNullOrWhiteSpace(result?.Token))
        //        {
        //            Console.WriteLine($"Token Generato: {result.Token}");
        //            token = result.Token;
        //        }
        //        else
        //        {
        //            Assert.Fail("Token non generato");
        //        }
        //    }
        //    else
        //    {
        //        // Gestisci qui la risposta fallita
        //        Console.WriteLine($"Errore: {response.StatusCode}");
        //        string responseContent = await response.Content.ReadAsStringAsync();
        //        Assert.Fail(responseContent);
        //    }
        //    _apiHttpClient.DefaultRequestHeaders.Add("AuthToken", token);
        //    #endregion
        //    _apiHttpClient.DefaultRequestHeaders.Remove("ROUTED_ACTION");
        //    _apiHttpClient.DefaultRequestHeaders.Add("ROUTED_ACTION", "GetActiveClassificationScheme");
        //    response = await this._apiHttpClient.GetAsync("RouteRestRequest");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine($"StatusCode: {response.StatusCode}");
        //        string responseContent = await response.Content.ReadAsStringAsync();
        //        Console.WriteLine($"Risposta JSON: {responseContent}");

        //        GetActiveClassificationSchemeCommandResponse result = JsonConvert.DeserializeObject<GetActiveClassificationSchemeCommandResponse>(responseContent);
        //        if (result.ClassificationScheme != null && !string.IsNullOrWhiteSpace(result.ClassificationScheme.Id))
        //        {
        //            Console.WriteLine($"Titolario attivo, ID: {result.ClassificationScheme.Id}");
        //            Assert.Pass("Test terminato con successo.");
        //        }
        //        else
        //        {
        //            Assert.Fail("Titolario attivo non trovato.");
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Errore: {response.StatusCode}");
        //        string responseContent = await response.Content.ReadAsStringAsync();
        //        Assert.Fail(responseContent);
        //    }
        //}

        //[Test]
        //public async Task GetClassSchemeByIDRouting()
        //{
        //    #region Token routing          
        //    string token = await TestUtils.GetTokenRouting("PAT_TEST", "A.lembo", "", "AUTOTEST PI3CORE", this._apiHttpClient);
        //    if (string.IsNullOrEmpty(token)) Assert.Fail("Token non generato");
        //    else _apiHttpClient.DefaultRequestHeaders.Add("AuthToken", token);
        //    # endregion 
            
        //    _apiHttpClient.DefaultRequestHeaders.Remove("ROUTED_ACTION");
        //    _apiHttpClient.DefaultRequestHeaders.Add("ROUTED_ACTION", "GetClassificationSchemeById");
        //    var response = await this._apiHttpClient.GetAsync("RouteRestRequest?idClassificationScheme=130275944");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine($"StatusCode: {response.StatusCode}");
        //        string responseContent = await response.Content.ReadAsStringAsync();
        //        Console.WriteLine($"Risposta JSON: {responseContent}");

        //        var result = JsonConvert.DeserializeObject<GetClassificationSchemeByIdCommandResponse>(responseContent);
        //        if (result.ClassificationScheme != null && !string.IsNullOrWhiteSpace(result.ClassificationScheme.Id))
        //        {
        //            Console.WriteLine($"Titolario trovato, ID: {result.ClassificationScheme.Id}");
        //            Assert.Pass("Test terminato con successo.");
        //        }
        //        else
        //        {
        //            Assert.Fail("Titolario non trovato.");
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Errore: {response.StatusCode}");
        //        string responseContent = await response.Content.ReadAsStringAsync();
        //        Assert.Fail(responseContent);
        //    }
        //}

        //[Test]
        //public async Task SearchUsersRoutingTest()
        //{
        //    #region Token routing          
        //    string token = await TestUtils.GetTokenRouting("PAT_TEST", "A.lembo", "", "AUTOTEST PI3CORE", this._apiHttpClient);
        //    if (string.IsNullOrEmpty(token)) Assert.Fail("Token non generato");
        //    else _apiHttpClient.DefaultRequestHeaders.Add("AuthToken", token);
        //    # endregion 
        //    _apiHttpClient.DefaultRequestHeaders.Remove("ROUTED_ACTION");
        //    _apiHttpClient.DefaultRequestHeaders.Add("ROUTED_ACTION", "SearchUsers");
        //    List<Filter> filters = new List<Filter>();
        //    filters.Add(new Filter() { Name = "USER_NAME", Value = "alessandro" });

        //    var suComm = new SearchUsersCommand()
        //    {
        //        Filters = filters.ToArray()
        //    };
        //    var json = JsonConvert.SerializeObject(suComm);
        //    using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
        //    var request = new HttpRequestMessage(HttpMethod.Post, "RouteRestRequest")
        //    {
        //        Content = content2
        //    };
        //    var response = await this._apiHttpClient.SendAsync(request);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine($"StatusCode: {response.StatusCode}");
        //        string responseContent = await response.Content.ReadAsStringAsync();
        //        //Console.WriteLine($"Risposta JSON: {responseContent}");

        //        var result = JsonConvert.DeserializeObject<SearchUsersCommandResponse>(responseContent);
        //        if (result != null && result.Users != null && result.Users.Any())
        //        {
        //            Console.WriteLine($"Utenti trovati: {result.Users.Length}");
        //            Assert.Pass("Test terminato con successo.");
        //        }
        //        else
        //        {
        //            Assert.Fail("Utenti non trovati");
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Errore: {response.StatusCode}");
        //        string responseContent = await response.Content.ReadAsStringAsync();
        //        Assert.Fail(responseContent);
        //    }
        //}

        //[Test]
        //public async Task AddCorrespondentRoutingTest()
        //{
        //    #region Token routing          
        //    string token = await TestUtils.GetTokenRouting("PAT_TEST", "A.lembo", "", "AUTOTEST PI3CORE", this._apiHttpClient);
        //    if (string.IsNullOrEmpty(token)) Assert.Fail("Token non generato");
        //    else _apiHttpClient.DefaultRequestHeaders.Add("AuthToken", token);
        //    # endregion 
        //    _apiHttpClient.DefaultRequestHeaders.Remove("ROUTED_ACTION");
        //    _apiHttpClient.DefaultRequestHeaders.Add("ROUTED_ACTION", "AddCorrespondent");
        //    var newCorr = new Correspondent()
        //    {
        //        Code = "TST_P3CORE_R_" + DateTime.Now.ToString("yyyyMMddHHmmss"),
        //        Description = "Test Creazione Corr Routing P3Core " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //        Email = "testmail@testmail.it",
        //        Note = "Creazione tramite test Automatico P3Core Routing",
        //        Type = "E",
        //        CorrespondentType = "U",
        //        PreferredChannel = "MAIL"
        //    };

        //    var addCorrReq = new AddCorrespondentCommand()
        //    {
        //        Correspondent = newCorr
        //    };
        //    var json = JsonConvert.SerializeObject(addCorrReq);
        //    Console.WriteLine($"Richiesta JSON: {json}");
        //    using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
        //    var response = await this._apiHttpClient.PutAsync("RouteRestRequest", content2);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        Console.WriteLine($"StatusCode: {response.StatusCode}");
        //        string responseContent = await response.Content.ReadAsStringAsync();
        //        Console.WriteLine($"Risposta JSON: {responseContent}");

        //        var result = JsonConvert.DeserializeObject<AddCorrespondentCommandResponse>(responseContent);
        //        if (result != null && result.Correspondent != null && !string.IsNullOrWhiteSpace(result.Correspondent.Id))
        //        {
        //            Console.WriteLine($"Corrispondente creato. ID: {result.Correspondent.Id}");
        //            Assert.Pass("Test superato.");
        //        }
        //        else
        //        {
        //            Assert.Fail("Errore nella creazione del corrispondente.");
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Errore: {response.StatusCode}");
        //        string responseContent = await response.Content.ReadAsStringAsync();
        //        Assert.Fail(responseContent);
        //    }
        //}
    }
}
