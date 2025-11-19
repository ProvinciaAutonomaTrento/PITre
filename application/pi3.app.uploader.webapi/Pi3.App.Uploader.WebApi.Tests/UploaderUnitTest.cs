// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Pi3.App.Uploader.WebApi.Resources;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Pi3.App.Uploader.WebApi.Tests
{
    public class Tests
    {
        private HttpClient _apiHttpClient;
        private readonly string _controllerUrl = "api/v1/{0}/Uploads";
        private Guid _uploadId;
        private int chunkSize = 5 * 100 * 1024; // 5 MB

        [SetUp]
        public void Setup()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

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

        private async Task PrintMultipartContentToConsole(MultipartFormDataContent content)
        {
            using (var memoryStream = new MemoryStream())
            {
                await content.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(memoryStream))
                {
                    string result = await reader.ReadToEndAsync();
                    Console.WriteLine(result);
                }
            }
        }

        private void SetHeader(HttpRequestMessage request) {
            request.Headers.Add("tenant", Environment.GetEnvironmentVariable("tenant"));
            request.Headers.Add("userId", Environment.GetEnvironmentVariable("userId"));
            request.Headers.Add("groupCode", Environment.GetEnvironmentVariable("groupCode"));
        }

        private HttpRequestMessage InitNoBodyRequest(string url, HttpMethod? method = null) {
            var request = new HttpRequestMessage(method ?? HttpMethod.Get,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance")) + "/" + url);
            SetHeader(request);
            return request;
        }

        private IEnumerable<byte[]> ChunkByteArray(byte[] data, int chunkSize)
        {
            for (int i = 0; i < data.Length; i += chunkSize)
            {
                int currentChunkSize = Math.Min(chunkSize, data.Length - i);
                byte[] chunk = new byte[currentChunkSize];
                Array.Copy(data, i, chunk, 0, currentChunkSize);
                yield return chunk;
            }
        }

        private HttpRequestMessage InitPostJsonRequest<T>(string url, T body, HttpMethod? method = null)
        {
            var jsonContent = JsonConvert.SerializeObject(body);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(method ?? HttpMethod.Post,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance")) + "/" + url)
            {
                Content = content
            };
            SetHeader(request);
            return request;
        }

        private async Task SaveMultipartContentToFile(MultipartFormDataContent content, string filePath)
        {
            using (var memoryStream = new MemoryStream())
            {
                await content.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await memoryStream.CopyToAsync(fileStream);
                }
            }
        }

        private HttpRequestMessage InitPostFormRequest(string url, Dictionary<string,string> stringFields,
            Dictionary<string, FileUpload> fileFields = null, HttpMethod? method = null)
        {
            var content = new MultipartFormDataContent();
            foreach (var field in stringFields)
            {
                content.Add(new StringContent(field.Value), field.Key);
            }
            foreach (var field in fileFields ?? new Dictionary<string, FileUpload>())
            {
                content.Add(new ByteArrayContent(field.Value.content), field.Key, field.Value.fileName);
            }

            //var method = isPost ? HttpMethod.Post : HttpMethod.Put;

            var request = new HttpRequestMessage(method ?? HttpMethod.Post,
                string.Format(this._controllerUrl, Environment.GetEnvironmentVariable("instance")) + "/" + url)
            {
                Content = content
            };
            SetHeader(request);
            return request;
        }

        [Test, Order(1)]
        public async Task Init_Test()
        {
            var body = Files.cqrs_documents;
            var chunks = ChunkByteArray(body, chunkSize).ToList();
            var checksum = string.Empty;

            using (var sha256 = SHA256.Create())
            using (var stream = new MemoryStream(body))
            {
                var hash = sha256.ComputeHash(stream);
                checksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }

            var request = InitPostFormRequest(string.Empty, new Dictionary<string, string> {
                {  "fileName", "cqrs_documents.pdf" },
                {  "numeroParti", chunks.Count.ToString() },
                {  "checksum", checksum }
            });

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                Assert.That(responseContent, Is.Not.Null.And.Not.Empty);

                _uploadId = JsonConvert.DeserializeObject<Guid>(responseContent);

                Assert.Pass("Test concluso con successo");
            }
            else
            {
                // Gestisci qui la risposta fallita
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(2)]
        public async Task Upload_Test()
        {
            var body = Files.cqrs_documents;
            var chunks = ChunkByteArray(body, chunkSize).ToList();

            for (int i = 0; i < chunks.Count; i++)
            {
                var request = InitPostFormRequest($"{_uploadId}/Part", new Dictionary<string, string>
                {
                    { "partNumber", (i + 1).ToString() },
                    { "partSize", chunks[i].Length.ToString() }
                }, new Dictionary<string, FileUpload>
                {
                    { "partContent", new FileUpload { fileName = $"part_{i + 1}", content = chunks[i] } }
                }, HttpMethod.Put);

                HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Errore: {response.StatusCode}");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Assert.Fail(responseContent);
                }
            }
            Assert.Pass("Test concluso con successo");
        }

        [Test, Order(3)]
        public async Task Finalize_Test()
        {
            var request = InitNoBodyRequest($"{_uploadId}/Finalize", HttpMethod.Put);

            HttpResponseMessage response = await this._apiHttpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
            else
            {
                Assert.Pass("Test concluso con successo");
            }
        }

        private class FileUpload
        {
            public string fileName { get; init; }
            public byte[] content { get; init; }
        };

        private class InitResponse {
            public string fileName { get; init; }
        };
    }
}