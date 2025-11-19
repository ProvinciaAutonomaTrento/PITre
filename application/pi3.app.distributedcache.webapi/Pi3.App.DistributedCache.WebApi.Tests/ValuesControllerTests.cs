// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Text;

namespace Pi3.App.DistributedCache.WebApi.Test
{
    public class ValuesControllerTests
    {
        private HttpClient _apiHttpClient;
        private readonly string _controllerUrl = "api/v1/Values";
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
        public async Task PutTestWithCorrectUser()
        {            
            var request = new HttpRequestMessage(HttpMethod.Put, this._controllerUrl);

            request.Headers.Add("Authorization", $"Basic {_credentials}");
            request.Content = new MultipartFormDataContent
            {
                { new StringContent(_unitTestKey), "key" },
                { new StringContent(_unitTestValue), "value" },
                { new StringContent(_expiresInMinutes), "expiresInMinutes" }
            };

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    Assert.Pass($"Test concluso con successo. StatusCode: {response.StatusCode}");
                else
                    Assert.Fail($"Errore nell'esecuzione del test. StatusCode: {response.StatusCode}");
            }
            else
            {
                Assert.Fail($"Errore nell'esecuzione del test. StatusCode: {response.StatusCode}");
            }
        }

        [Test]
        [Order(2)]
        public async Task PutTestWithFakeUser()
        {
            var request = new HttpRequestMessage(HttpMethod.Put, this._controllerUrl);

            request.Headers.Add("Authorization", $"Basic {_fakeCredentials}");
            request.Content = new MultipartFormDataContent
            {
                { new StringContent(_unitTestKey), "key" },
                { new StringContent(_unitTestValue), "value" },
                { new StringContent(_expiresInMinutes), "expiresInMinutes" }
            };

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    Assert.Pass($"Test concluso con successo. StatusCode: {response.StatusCode}");
                else
                    Assert.Fail($"Errore nell'esecuzione del test. StatusCode: {response.StatusCode}");
            }
            else
            {
                Assert.Fail($"Errore nell'esecuzione del test. StatusCode: {response.StatusCode}");
            }
        }

        [Test]
        [Order(3)]
        public async Task GetTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{this._controllerUrl}?key={_unitTestKey}" );

            request.Headers.Add("Authorization", $"Basic {_credentials}");

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    Assert.Pass($"Test concluso con successo. StatusCode: {response.StatusCode}");
                else
                    Assert.Fail($"Errore nell'esecuzione del test. StatusCode: {response.StatusCode}");
            }
            else
            {
                Assert.Fail($"Errore nell'esecuzione del test. StatusCode: {response.StatusCode}");
            }
        }

        [Test]
        [Order(4)]
        public async Task DeleteTest()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, this._controllerUrl);

            request.Headers.Add("Authorization", $"Basic {_credentials}");
            request.Content = new MultipartFormDataContent
            {
                { new StringContent(_unitTestKey), "key" }
            };

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    Assert.Pass($"Test concluso con successo. StatusCode: {response.StatusCode}");
                else
                    Assert.Fail($"Errore nell'esecuzione del test. StatusCode: {response.StatusCode}");
            }
            else
            {
                Assert.Fail($"Errore nell'esecuzione del test. StatusCode: {response.StatusCode}");
            }
        }
    }
}