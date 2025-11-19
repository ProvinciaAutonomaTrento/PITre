// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Internal;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Tests
{

    public class TestContextData
    {
        public Dictionary<string, string> Variables { get; } = new();

        public void Set(string key, string value) => Variables[key] = value;
        public string Get(string key) => Variables.TryGetValue(key, out var value) ? value : null;
    }


    public interface ITestPreProcessor
    {
        void Execute(TestContextData context);
    }


    public interface ITestCasePostProcessor
    {
        void Execute(string responseJson, TestContextData context);
    }

    public static class JsonPlaceholderReplacer
    {
        public static string ReplacePlaceholders(string json, Dictionary<string, string> variables)
        {
            foreach (var kvp in variables)
            {
                json = json.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
            }
            return json;
        }
    }


    public interface ITestCaseHandler
    {
        void Validate(string actualResponse, string expectedResponse, TestContextData context);

    }

    public class ValidJsonHandler : ITestCaseHandler
    {
        public void Validate(string actualJson, string expectedJson, TestContextData context)
        {
            try
            {
                JToken.Parse(actualJson);
                Assert.Pass();
            }
            catch (JsonReaderException)
            {
                Assert.Fail("L'output non è un formato JSON valido.");
            }
        }
    }

    public class EqualityJsonHandler : ITestCaseHandler
    {
        public void Validate(string actualJson, string expectedJson, TestContextData context)
        {
            Assert.AreEqual(
                JToken.Parse(expectedJson).ToString(Formatting.None),
                JToken.Parse(actualJson).ToString(Formatting.None),
                "JSON output does not match expected");
        }
    }

    public class AddDocGrigiaWithNotaHandler : ITestCaseHandler
    {
        public void Validate(string actualJson, string expectedJson, TestContextData context)
        {
            var actual = JsonConvert.DeserializeObject<DocumentoAddDocGrigiaResult>(actualJson);
            var expected = JsonConvert.DeserializeObject<DocumentoAddDocGrigiaResult>(expectedJson);

            //Assert.AreEqual(expected.Nota, actual.Nota, "Nota mismatch");
            // Aggiungi altre validazioni specifiche
        }
    }

    public class GenerateIdPreProcessor : ITestPreProcessor
    {
        public void Execute(TestContextData context)
        {
            var generatedId = Guid.NewGuid().ToString();
            context.Set("generatedId", generatedId);
        }
    }

    public class ExtractIdPostProcessor : ITestCasePostProcessor
    {
        public void Execute(string responseJson, TestContextData context)
        {
            var json = JObject.Parse(responseJson);
            var id = json["id"]?.ToString();
            if (!string.IsNullOrEmpty(id))
            {
                context.Set("generatedId", id);
            }
        }
    }



    [TestFixture]
    [Parallelizable]
    public class DataPortalTests
    {
        private readonly string _uri = "/api/v1/{0}/DataPortal"; 
        private HttpClient _apiHttpClient;
        private static string _testDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestCases");
        private static Dictionary<string, string> _variables = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

            _variables = GetVariables();
            _variables?.ToList().ForEach(v => Environment.SetEnvironmentVariable(v.Key, v.Value));

            WebApplicationFactory<Program> webAppFactory = new();
            this._apiHttpClient = webAppFactory.CreateClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            this._apiHttpClient?.Dispose();
        }

        private static Dictionary<string, string> GetVariables()
        {
            var variablesFile = Path.Combine(_testDir, $"variables.json");

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(variablesFile))!;
        }

        public static void CleanUp(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"La cartella '{folderPath}' non esiste.");
                return;
            }

            var files = Directory.GetFiles(folderPath, "*.json");
            var fileGroups = new Dictionary<string, List<string>>();

            // Pattern: {name}.request.json o {name}.expected.json (dopo aver rimosso il prefisso numerico)
            var regex = new Regex(@"^(.+?)\.(request|expected)\.json$", RegexOptions.IgnoreCase);

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);

                // Rimuove il prefisso numerico e il trattino iniziale
                var nameWithoutPrefix = Regex.Replace(fileName, @"^\d+-", "");

                var match = regex.Match(nameWithoutPrefix);
                if (match.Success)
                {
                    string name = match.Groups[1].Value;
                    string type = match.Groups[2].Value.ToLower();
                    string key = $"{name}.{type}";

                    if (!fileGroups.ContainsKey(key))
                        fileGroups[key] = new List<string>();

                    fileGroups[key].Add(file);
                }
            }

            foreach (var group in fileGroups)
            {
                var duplicates = group.Value;
                if (duplicates.Count > 1)
                {
                    var toDelete = duplicates.OrderBy(f => f).Skip(1);
                    foreach (var file in toDelete)
                    {
                        Console.WriteLine($"Eliminazione duplicato: {file}");

                        var attributes = File.GetAttributes(file);

                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            // Rimuove l'attributo di sola lettura
                            File.SetAttributes(file, attributes & ~FileAttributes.ReadOnly);
                        }

                        File.Delete(file);
                    }
                }
            }

            Console.WriteLine("Pulizia completata.");
        }

        public static IEnumerable<TestCaseData> GetTestCases()
        {
            //CleanUp("C:\\TnDigit\\pi3.app.legacy.webapi.git\\Pi3.App.Legacy.WebApi.Tests\\TestCases\\008.RicercaDocumenti");


            var variables = GetVariables();

            var directories = new List<DirectoryInfo>();

            variables.TryGetValue("testSuite", out string? testSuite);

            if (!string.IsNullOrWhiteSpace(testSuite))
            {
                directories.Add(new DirectoryInfo(Path.Combine(_testDir, testSuite)));
            }
            else
            {
                directories.AddRange(new DirectoryInfo(_testDir)
                        .EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                        .OrderBy(d => d.Name));
            }

            foreach (var dir in directories)
            {
                foreach (var requestFile in dir.EnumerateFiles("*.request.json").OrderBy(f => f.Name))
                {
                    var testName = Path.GetFileNameWithoutExtension(requestFile.Name).Replace(".request", "");
                    var expectedFile = Path.Combine(_testDir, $"{testName}.expected.json");

                    var requestJson = File.ReadAllText(requestFile.FullName);
                    var expectedJson = File.Exists(expectedFile) ? File.ReadAllText(expectedFile) : null;

                    var request = JsonConvert.DeserializeObject<DataPortalRequest>(requestJson);
                    var expected = expectedJson;

                    yield return new TestCaseData(dir.Name, testName, request, expected).SetName(testName);
                }
            }
        }

        private ITestCaseHandler GetHandlerForTest(string testName)
        {
            return testName switch
            {
                "AddDocGrigiaWithNotaTest" => new AddDocGrigiaWithNotaHandler(),
                // altri handler personalizzati
                _ => new ValidJsonHandler()
            };
        }


        private ITestPreProcessor? GetPreProcessorForTest(string testName)
        {
            return testName switch
            {
                "InsertWithDynamicId" => new GenerateIdPreProcessor(),
                _ => null
            };
        }

        private ITestCasePostProcessor? GetPostProcessorForTest(string testName)
        {
            return testName switch
            {
                "InsertWithDynamicId" => new ExtractIdPostProcessor(),
                _ => null
            };
        }

        [Test, TestCaseSource(nameof(GetTestCases))]
        public async Task DataPortalTest(string testSuite, string testName, DataPortalRequest request, string expectedJson)
        {
            var uri = string.Format(_uri, _variables["instance"]);
            var context = new TestContextData();

            // Pre-processing dinamico
            var preProcessor = GetPreProcessorForTest(testName);
            preProcessor?.Execute(context);

            // Serializza e sostituisci placeholder
            var json = JsonConvert.SerializeObject(request);
            json = JsonPlaceholderReplacer.ReplacePlaceholders(json, context.Variables);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri) { Content = content };
            var response = await _apiHttpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.IsTrue(response.IsSuccessStatusCode, "Request failed");

            string resultContentJson = null!;
            if (request.ResponseAsRaw ?? false)
                resultContentJson = responseContent;
            else
            {
                var result = JsonConvert.DeserializeObject<DataPortalResponse>(responseContent);
                resultContentJson = result!.ResponseContext.AsJson;
            }

            var handler = GetHandlerForTest(testName);
            handler.Validate(resultContentJson, expectedJson, context);

            var postProcessor = GetPostProcessorForTest(testName);
            postProcessor?.Execute(resultContentJson, context);
        }
    }
}
