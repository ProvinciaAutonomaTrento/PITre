// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetModifiedDocuments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using Pi3.Core.SeedWork;
namespace Pi3.App.Legacy.Pis.WebApi.Tests
{
    public class TestUtils
    {
        public static async Task<string> GetToken(string codeAdm, string userId, string codeRole, string codeApp, string instance, HttpClient client)
        {
            string retval = "";
            AuthenticateCommand getTokenRequest = new AuthenticateCommand()
            {
                CodeAdm = codeAdm,
                CodeApplication = codeApp,
                CodeRole = codeRole,
                Username = userId
            };
            string json = JsonConvert.SerializeObject(getTokenRequest);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "GetToken")
            {
                Content = content
            };
            client.DefaultRequestHeaders.Add("Instance", instance);

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                AuthenticateCommandResponse result = JsonConvert.DeserializeObject<AuthenticateCommandResponse>(responseContent);
                if (!String.IsNullOrWhiteSpace(result?.Token))
                {
                    Console.WriteLine($"Token Generato: {result.Token}");
                    retval = result.Token;
                }

            }
            return retval;
        }

        public static async Task AddAuthToken(HttpClient client)
        {
            var codAmm = Environment.GetEnvironmentVariable("tenant");
            var userId = Environment.GetEnvironmentVariable("userId");
            var groupCode = Environment.GetEnvironmentVariable("groupCode");
            var instance = Environment.GetEnvironmentVariable("instance");
            var codeApp = Environment.GetEnvironmentVariable("codeApp");

            string token = await TestUtils.GetToken(codAmm, userId, groupCode, codeApp, instance, client);

            if (string.IsNullOrEmpty(token))
                Assert.Fail("Token non generato");
            else
                client.DefaultRequestHeaders.Add("AuthToken", token);
        }

        public static async Task<string> GetTokenRouting(string codeAdm, string userId, string codeRole, string codeApp, HttpClient client)
        {
            string retval = "";
            AuthenticateCommand getTokenRequest = new AuthenticateCommand()
            {
                CodeAdm = codeAdm,
                CodeApplication = codeApp,
                CodeRole = codeRole,
                Username = userId
            };
            string json = JsonConvert.SerializeObject(getTokenRequest);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "RouteRestRequest")
            {
                Content = content
            };
            client.DefaultRequestHeaders.Add("CODE_ADM", codeAdm);
            client.DefaultRequestHeaders.Add("ROUTED_ACTION", "GetToken");
            client.DefaultRequestHeaders.Add("APPLICATION_NAME", "APP1");

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                AuthenticateCommandResponse result = JsonConvert.DeserializeObject<AuthenticateCommandResponse>(responseContent);
                if (!String.IsNullOrWhiteSpace(result?.Token))
                {
                    Console.WriteLine($"Token Generato: {result.Token}");
                    retval = result.Token;
                }

            }
            return retval;
        }

        public static string GetQueryStringFromEncodedJson(string? encodedRequestModel)
        {
            if (string.IsNullOrEmpty(encodedRequestModel))
                return "";

            var modelParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(encodedRequestModel);

            string queryString = "?";
            int keyC = 0;

            foreach ((var key, var val) in modelParams)
            {
                queryString = queryString + key + "=" + val;
                if (++keyC < modelParams.Count)
                    queryString += "&";
            }
            return queryString;
        }

        public static string ComputeHash(byte[] content)
        {
            string hash = string.Empty;
            using (System.Security.Cryptography.SHA256 hashSha256 = System.Security.Cryptography.SHA256.Create())
            {
                StringBuilder sBuilder = new StringBuilder();
                byte[] data = hashSha256.ComputeHash(content);

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                hash = sBuilder.ToString();
            }
            return hash;
        }

        public static HttpRequestMessage SerializeRequest<T>(T requestObject, HttpMethod method, string uri)
        {
            var json = JsonConvert.SerializeObject(requestObject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(method, uri)
            {
                Content = content
            };

            return request;
        }
    }
}
