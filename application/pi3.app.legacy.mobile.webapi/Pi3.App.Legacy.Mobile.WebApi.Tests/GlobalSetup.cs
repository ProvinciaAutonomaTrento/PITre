// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pi3.App.Legacy.Mobile.Shared.Helpers;
using System.Net.Http.Json;
using System.Text;

namespace Pi3.App.Legacy.Mobile.WebApi.Tests;

[SetUpFixture]
public class GlobalSetup
{
    readonly MobileTestContext _internalTestContext = MobileTestContext.Instance;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        TestContext.WriteLine("Inizializzo l'ambiente di test");

        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test", EnvironmentVariableTarget.Process);

        this.InitTestData();

        string connectionString = $"{this._internalTestContext.TestData.ConnectionStrings}";
        Environment.SetEnvironmentVariable("ConnectionStrings__PAT_INSTANCE", connectionString, EnvironmentVariableTarget.Process);

        WebApplicationFactory<Program> webAppFactory = new();
        this._internalTestContext.ApiHttpClient = webAppFactory.CreateClient();

        IActionDescriptorCollectionProvider actionDescriptorCollectionProvider = webAppFactory.Services.GetRequiredService<IActionDescriptorCollectionProvider>();
        this._internalTestContext.PathResolutionService = new(actionDescriptorCollectionProvider);
        
        await this.InitAuthenticationToken();
    }

    [OneTimeTearDown]
    public void RunAfterAllTests()
    {
        this._internalTestContext.ApiHttpClient.Dispose();
    }

    private void InitTestData()
    {
        var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData.json");
        if ( !File.Exists(path) )
        {
            Assert.Fail("Il file TestData.json non esiste. Impossibile proseguire con i test.");
        }
        var jsonContent = File.ReadAllText(path);
        this._internalTestContext.TestData = JsonConvert.DeserializeObject<dynamic>(jsonContent) ?? throw new Exception("Parametri da utilizzare nei test non accessibili");

    }

    private async Task InitAuthenticationToken()
    {
        string? url = MobileTestContext.Instance.PathResolutionService.ResolvePath("Authenticate", "Login");

        var requestBody = new
        {
            Username = $"{MobileTestContext.Instance.TestData.Login.Username}",
            Password = $"{MobileTestContext.Instance.TestData.Login.Password}",
            IdAmministrazione = $"{MobileTestContext.Instance.TestData.Login.IdAmministrazione}"
        };
        string jsonContent = System.Text.Json.JsonSerializer.Serialize(requestBody);
        StringContent? content = new(jsonContent, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);

        HttpRequestMessage? request = new(HttpMethod.Post, url)
        {
            Content = content
        };

        request.Headers.Add("Instance", $"{MobileTestContext.Instance.TestData.Instance}");

        HttpResponseMessage response = await MobileTestContext.Instance.ApiHttpClient.SendAsync(request);

        if ( response.IsSuccessStatusCode )
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic? body = JsonConvert.DeserializeObject(responseContent);
            MobileTestContext.Instance.AuthenticationToken = $"{body?.UserInfo.Token}";
        }

    }
}
