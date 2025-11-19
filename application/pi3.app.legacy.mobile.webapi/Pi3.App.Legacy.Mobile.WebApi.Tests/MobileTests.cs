// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pi3.App.Legacy.Mobile.Shared.Helpers;
using System.Text;

namespace Pi3.App.Legacy.Mobile.WebApi.Tests;

public class Tests
{
    readonly HttpClient _apiHttpClient = MobileTestContext.Instance.ApiHttpClient;
    readonly dynamic _testData = MobileTestContext.Instance.TestData;
    readonly PathResolutionService? _pathResolutionService = MobileTestContext.Instance.PathResolutionService;
    private string _authenticationToken = null!;

    [SetUp]
    public void Setup()
    {
        this._authenticationToken = MobileTestContext.Instance.AuthenticationToken ?? throw new Exception("Authenticazione necessaria per procedere con i test");
    }

    [TearDown]
    public void TearDown()
    {
    }


    //[Test(Description = "Test Login con password errata")]
    public async Task LoginPasswordErrata()
    {
        string? url = this._pathResolutionService!.ResolvePath("Authenticate", "Login");

        var requestBody = new
        {
            Username = $"{this._testData.Login.Username}",
            Password = "12345",
            IdAmministrazione = $"{this._testData.Login.IdAmministrazione}"
        };
        string jsonContent = System.Text.Json.JsonSerializer.Serialize(requestBody);
        StringContent? content = new(jsonContent, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);

        HttpRequestMessage? request = new(HttpMethod.Post, url)
        {
            Content = content
        };

        request.Headers.Add("Instance", $"{this._testData.Instance}");

        HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

        if ( response.IsSuccessStatusCode )
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic? body = JsonConvert.DeserializeObject(responseContent);
            string errorCode = $"{body?.Code}";
            if(int.TryParse(errorCode, out int code) && code > 0 )
            {
                Assert.Pass("Test concluso con successo");
            }
            else
            {
                Assert.Fail("Test fallito");
            }
        }
        else
        {
            Assert.Fail("Test fallito");
        }
    }

    //[Test(Description = "Recupero documento")]
    public async Task GetDocumentInfo()
    {
        string? url = this._pathResolutionService!.ResolvePath("Documents", "GetDocInfo");

        var data = new
        {
            IdDoc = $"{this._testData.DocumentInfo.IdDocumento}",
            IdEvento = $"{this._testData.DocumentInfo.IdEvento}",
            IdTrasm = $"{this._testData.DocumentInfo.IdTrasmissione}"
        };

        var query = $"?IdDoc={Uri.EscapeDataString(data.IdDoc)}&IdEvento={Uri.EscapeDataString(data.IdEvento)}&IdTrasm={Uri.EscapeDataString(data.IdTrasm)}";

        var fullUrl = $"{url}{query}";

        HttpRequestMessage? request = new(HttpMethod.Get, fullUrl);
        request.Headers.Add("Instance", $"{this._testData.Instance}");
        request.Headers.Add("AuthToken", this._authenticationToken);

        HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

        if ( response.IsSuccessStatusCode )
        {
            Assert.Pass("Test concluso con successo");
        }
        else
        {
            Console.WriteLine($"Errore: {response.StatusCode}");
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Fail(responseContent);
        }
    }

    //[Test(Description = "Recupero documento con token non valido")]
    public async Task GetDocumentInfoConTokenErrato()
    {
        string? url = this._pathResolutionService!.ResolvePath("Documents", "GetDocInfo");

        var data = new
        {
            IdDoc = $"{this._testData.DocumentInfo.IdDocumento}",
            IdEvento = $"{this._testData.DocumentInfo.IdEvento}",
            IdTrasm = $"{this._testData.DocumentInfo.IdTrasmissione}"
        };

        var query = $"?IdDoc={Uri.EscapeDataString(data.IdDoc)}&IdEvento={Uri.EscapeDataString(data.IdEvento)}&IdTrasm={Uri.EscapeDataString(data.IdTrasm)}";

        var fullUrl = $"{url}{query}";

        HttpRequestMessage? request = new(HttpMethod.Get, fullUrl);
        request.Headers.Add("Instance", $"{this._testData.Instance}");
        request.Headers.Add("AuthToken", "SSO=wowLlyIyThJUDEtfNz0cg52l/4y3Rg8F0sX3VClA4rq+bZRj8YmeaZ0hdrzKPHFKd/N1uEm2z4VgQ1YwfDOGuobcEh16NLS4v3xUkovNyPWwHq6WpLr6JeViO5ai/pfe");

        HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

        if ( response.StatusCode == System.Net.HttpStatusCode.NotFound )
        {
            Assert.Pass("Test concluso con successo");
        }
        else
        {
            Console.WriteLine($"Errore: {response.StatusCode}");
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Fail(responseContent);
        }
    }

    //[Test(Description = "Recupero documento con trasmissione")]
    public async Task GetDocumentInfoConTrasmissione()
    {
        string? url = this._pathResolutionService!.ResolvePath("Documents", "GetDocInfo");

        var data = new
        {
            IdDoc = $"{this._testData.DocumentInfoTrasmissione.IdDocumento}",
            IdEvento = $"{this._testData.DocumentInfoTrasmissione.IdEvento}",
            IdTrasm = $"{this._testData.DocumentInfoTrasmissione.IdTrasmissione}"
        };

        var query = $"?IdDoc={Uri.EscapeDataString(data.IdDoc)}&IdEvento={Uri.EscapeDataString(data.IdEvento)}&IdTrasm={Uri.EscapeDataString(data.IdTrasm)}";

        var fullUrl = $"{url}{query}";

        HttpRequestMessage? request = new(HttpMethod.Get, fullUrl);
        request.Headers.Add("Instance", $"{this._testData.Instance}");
        request.Headers.Add("AuthToken", this._authenticationToken);

        HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

        if ( response.IsSuccessStatusCode )
        {
            Assert.Pass("Test concluso con successo");
        }
        else
        {
            Console.WriteLine($"Errore: {response.StatusCode}");
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Fail(responseContent);
        }
    }

    //[Test(Description = "Recupero documento con trasmissione inesistente")]
    public async Task GetDocumentInfoConTrasmissioneInesistente()
    {
        string? url = this._pathResolutionService!.ResolvePath("Documents", "GetDocInfo");

        var data = new
        {
            IdDoc = $"{this._testData.DocumentInfoTrasmissione.IdDocumento}",
            IdEvento = $"{this._testData.DocumentInfoTrasmissione.IdEvento}",
            IdTrasm = "111111111111"
        };

        var query = $"?IdDoc={Uri.EscapeDataString(data.IdDoc)}&IdEvento={Uri.EscapeDataString(data.IdEvento)}&IdTrasm={Uri.EscapeDataString(data.IdTrasm)}";

        var fullUrl = $"{url}{query}";

        HttpRequestMessage? request = new(HttpMethod.Get, fullUrl);
        request.Headers.Add("Instance", $"{this._testData.Instance}");
        request.Headers.Add("AuthToken", this._authenticationToken);

        HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

        if ( response.StatusCode == System.Net.HttpStatusCode.NotFound )
        {
            Assert.Pass("Test concluso con successo");
        }
        else
        {
            Console.WriteLine($"Errore: {response.StatusCode}");
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Fail(responseContent);
        }
    }

    //[Test(Description = "Check Health Page")]
    public async Task CheckHealtPage()
    {
        HttpRequestMessage? request = new(HttpMethod.Get, "health");
        request.Headers.Add("Instance", $"{this._testData.Instance}");  
        request.Headers.Add("AuthToken", this._authenticationToken);

        HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

        if ( response.IsSuccessStatusCode )
        {
            Assert.Pass("Test concluso con successo");
        }
        else
        {
            Console.WriteLine($"Errore: {response.StatusCode}");
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Fail(responseContent);
        }
    }

    //[Test(Description = "Recupero documento con con connection string assente")]
    public async Task GetDocumentInfoConConnectionStringAssente()
    {
        string? url = this._pathResolutionService!.ResolvePath("Documents", "GetDocInfo");

        var data = new
        {
            IdDoc = $"{this._testData.DocumentInfo.IdDocumento}",
            IdEvento = $"{this._testData.DocumentInfo.IdEvento}",
            IdTrasm = $"{this._testData.DocumentInfo.IdTrasmissione}"
        };

        var query = $"?IdDoc={Uri.EscapeDataString(data.IdDoc)}&IdEvento={Uri.EscapeDataString(data.IdEvento)}&IdTrasm={Uri.EscapeDataString(data.IdTrasm)}";

        var fullUrl = $"{url}{query}";

        HttpRequestMessage? request = new(HttpMethod.Get, fullUrl);
        request.Headers.Add("Instance", "INESISTENTE");
        request.Headers.Add("AuthToken", this._authenticationToken);

        HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

        if ( response.StatusCode == System.Net.HttpStatusCode.InternalServerError )
        {
            Assert.Pass("Test concluso con successo");
        }
        else
        {
            Console.WriteLine($"Errore: {response.StatusCode}");
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Fail(responseContent);
        }
    }

    //[Test(Description = "Recupero documento protocollo Arrivo")]
    public async Task GetDocumentInfoProtocolloA()
    {
        string? url = this._pathResolutionService!.ResolvePath("Documents", "GetDocInfo");

        var data = new
        {
            IdDoc = $"{this._testData.DocumentInfoProtocolloA.IdDocumento}",
            IdEvento = $"{this._testData.DocumentInfoProtocolloA.IdEvento}",
            IdTrasm = $"{this._testData.DocumentInfoProtocolloA.IdTrasmissione}"
        };

        var query = $"?IdDoc={Uri.EscapeDataString(data.IdDoc)}&IdEvento={Uri.EscapeDataString(data.IdEvento)}&IdTrasm={Uri.EscapeDataString(data.IdTrasm)}";

        var fullUrl = $"{url}{query}";

        HttpRequestMessage? request = new(HttpMethod.Get, fullUrl);
        request.Headers.Add("Instance", $"{this._testData.Instance}");
        request.Headers.Add("AuthToken", this._authenticationToken);

        HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

        if ( response.IsSuccessStatusCode )
        {
            Assert.Pass("Test concluso con successo");
        }
        else
        {
            Console.WriteLine($"Errore: {response.StatusCode}");
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Fail(responseContent);
        }
    }

    //[Test(Description = "Recupero documento con trasmissione inesistente")]
    public async Task GetDocumentInfoParametroAssente()
    {
        string? url = this._pathResolutionService!.ResolvePath("Documents", "GetDocInfo");

        var data = new
        {
            IdDoc = "",
            IdEvento = "",
            IdTrasm = ""
        };

        var query = $"?IdDoc={Uri.EscapeDataString(data.IdDoc)}&IdEvento={Uri.EscapeDataString(data.IdEvento)}&IdTrasm={Uri.EscapeDataString(data.IdTrasm)}";

        var fullUrl = $"{url}{query}";

        HttpRequestMessage? request = new(HttpMethod.Get, fullUrl);
        request.Headers.Add("Instance", $"{this._testData.Instance}");
        request.Headers.Add("AuthToken", this._authenticationToken);

        HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

        if ( response.StatusCode == System.Net.HttpStatusCode.BadRequest )
        {
            Assert.Pass("Test concluso con successo");
        }
        else
        {
            Console.WriteLine($"Errore: {response.StatusCode}");
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Fail(responseContent);
        }
    }

    //[Test(Description = "Recupero documento con Note")]
    public async Task GetDocumentInfoConNote()
    {
        string? url = this._pathResolutionService!.ResolvePath("Documents", "GetDocInfo");

        var data = new
        {
            IdDoc = $"{this._testData.DocumentInfoConNote.IdDocumento}",
            IdEvento = $"{this._testData.DocumentInfoConNote.IdEvento}",
            IdTrasm = $"{this._testData.DocumentInfoConNote.IdTrasmissione}"
        };

        var query = $"?IdDoc={Uri.EscapeDataString(data.IdDoc)}&IdEvento={Uri.EscapeDataString(data.IdEvento)}&IdTrasm={Uri.EscapeDataString(data.IdTrasm)}";

        var fullUrl = $"{url}{query}";

        HttpRequestMessage? request = new(HttpMethod.Get, fullUrl);
        request.Headers.Add("Instance", $"{this._testData.Instance}");
        request.Headers.Add("AuthToken", this._authenticationToken);

        HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);

        if ( response.IsSuccessStatusCode )
        {
            Assert.Pass("Test concluso con successo");
        }
        else
        {
            Console.WriteLine($"Errore: {response.StatusCode}");
            string responseContent = await response.Content.ReadAsStringAsync();
            Assert.Fail(responseContent);
        }
    }


}