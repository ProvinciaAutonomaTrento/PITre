// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Text;

namespace Pi3.App.DistributedCache.WebApi.Test
{
    public class KeysControllerTests
    {
        private HttpClient _apiHttpClient;
        private readonly string _controllerUrl = "api/v1/Keys";
        private string _credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("UnitTest:UnitTestPwd123!"));
        private string _fakeCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("FakeUser:FakeUserPwd!"));
        private string _unitTestKey = "UnitTestKey";
        private string _unitTestValue = "UnitTestValue";
        private string _expiresInMinutes = "10";

        [OneTimeSetUpAttribute]
        public void Setup()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

            WebApplicationFactory<Program> webAppFactory = new();
            this._apiHttpClient = webAppFactory.CreateClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            this._apiHttpClient?.Dispose();
        }

        [Test]
        [Order(1)]
        public async Task GetKeysTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{this._controllerUrl}");

            request.Headers.Add("Authorization", $"Basic {_credentials}");

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Assert.Pass($"Test concluso con successo. StatusCode: {response.StatusCode}");
            }
            else
            {
                Assert.Fail($"Errore nell'esecuzione del test. StatusCode: {response.StatusCode}");
            }
        }
    }
}