// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.AnalyzeDocumentDroppedOrErrorMessageProof;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.AnalyzeDocumentReceivedProof;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.ElaborateNewInteroperabilityMessage;
using System.Text;

namespace Pi3.App.InteropPitre.WebApi.Tests
{
    public class InteroperabilityV1Tests
    {
        private HttpClient _apiHttpClient;
        private readonly string _controllerUrl = $"api/v1/PAT_INSTANCE/InteroperabilityService";

        [SetUp]
        public void SetUp()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
            }

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

        [Test, Order(1)]
        public async Task ElaborateNewInteroperabilityMessageTest()
        {
            var random = new Random();

            int senderRecordNumber = random.Next(1, 99999);
            int receiverRecordNumber = random.Next(1, 99999);
            int corrCode = random.Next(1, 99999);

            string tenant = "PAT_TEST";

            var path = Path.Combine(Environment.CurrentDirectory, "testfile.pdf");

            File.WriteAllBytes(path, Files.LoremIpsum);

            var apiRequest = new ElaborateNewInteroperabilityMessageRequest
            {
                InteroperabilityMessage = new InteroperabilityMessage
                {
                    Record = new RecordInfo
                    {
                        AdministrationCode = tenant,
                        RecordNumber = senderRecordNumber.ToString(),
                        RecordDate = DateTime.Now,
                        AOOCode = "PAT",
                        Subject = "Test_" + Guid.NewGuid().ToString()
                    },
                    Sender = new SenderInfo
                    {
                        Url = "http://localhost",
                        Code = "PAT_TEST-RFS" + corrCode,
                        AdministrationId = tenant,
                        UserId = "TEST_PARER",
                        FileManagerUrl = string.Empty
                    },
                    Receivers = new List<ReceiverInfo> { new ReceiverInfo
                    {
                        Code = "PAT_TEST_RFA034",
                        AOOCode = "PAT",
                        AdministrationCode = "PAT_TEST"
                    } },
                    MainDocument = new DocumentInfo
                    {
                        Name = "test.pdf",
                        NumberOfPages = 1,
                        DocumentServerLocation = string.Empty,
                        FilePath = path,
                        FileName = "test.pdf",
                        DocumentNumber = receiverRecordNumber.ToString(),
                        VersionId = receiverRecordNumber.ToString(),
                        Version = "1",
                        VersionLabel = "Versione test",
                        Fingerprint = string.Empty
                    },
                    Attachments = new List<DocumentInfo> { new DocumentInfo
                    {
                        Name = "attachment.pdf",
                        NumberOfPages = 1,
                        DocumentServerLocation = string.Empty,
                        FilePath = path,
                        FileName = "attachment.pdf",
                        DocumentNumber = (receiverRecordNumber+2).ToString(),
                        VersionId = (receiverRecordNumber+2).ToString(),
                        Version = "1",
                        VersionLabel = "Versione allegato test",
                        Fingerprint = string.Empty
                    } },
                    Note = "Nota di test",
                    IsPrivate = false,
                    ReceiverAdministrationCode = tenant
                }
            };

            var json = JsonConvert.SerializeObject(apiRequest);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{this._controllerUrl}/ElaborateNewInteroperabilityMessage") { Content = content };

            _apiHttpClient.DefaultRequestHeaders.Add("Tenant", tenant);

            var response = await this._apiHttpClient.SendAsync(request);

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var apiResponse = JsonConvert.DeserializeObject<ElaborateNewInteroperabilityMessageResponse>(responseContent);

                if(!string.IsNullOrWhiteSpace(apiResponse?.Result?.DocumentDelivered?.MainDocument?.DocumentNumber) && !apiResponse.Result.SingleRequestErrors.Any())
                {
                    Assert.Pass($"Creato documento con id={apiResponse?.Result?.DocumentDelivered?.MainDocument?.DocumentNumber}");
                }
                else
                {
                    if(string.IsNullOrWhiteSpace(apiResponse?.Result?.DocumentDelivered?.MainDocument?.DocumentNumber))
                    {
                        Assert.Fail("Documento non creato");
                    }
                    else
                    {
                        var errors = string.Join(Environment.NewLine, apiResponse.Result.SingleRequestErrors.Select(x => x.ErrorMessage));
                        Assert.Fail($"Errori nell'elaborazione: {errors}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(2)]
        [Ignore("test ingorato temporaneamente")]
        public async Task ElaborateNewInteroperabilityMessageUnreachableRecipientTest()
        {
            var random = new Random();

            int senderRecordNumber = random.Next(1, 99999);
            int receiverRecordNumber = random.Next(1, 99999);

            string tenant = "PAT_TEST";

            var path = Path.Combine(Environment.CurrentDirectory, "testfile.pdf");

            File.WriteAllBytes(path, Files.LoremIpsum);

            var apiRequest = new ElaborateNewInteroperabilityMessageRequest
            {
                InteroperabilityMessage = new InteroperabilityMessage
                {
                    Record = new RecordInfo
                    {
                        AdministrationCode = tenant,
                        RecordNumber = senderRecordNumber.ToString(),
                        RecordDate = DateTime.Now,
                        AOOCode = "PAT",
                        Subject = "Test_" + Guid.NewGuid().ToString()
                    },
                    Sender = new SenderInfo
                    {
                        Url = "http://localhost",
                        Code = "PAT_TEST-RFS202",
                        AdministrationId = tenant,
                        UserId = "TEST_PARER",
                        FileManagerUrl = string.Empty
                    },
                    Receivers = new List<ReceiverInfo> { new ReceiverInfo
                    {
                        Code = "PAT_TEST_RFA034",
                        AOOCode = "PAT",
                        AdministrationCode = "PAT_TEST"
                    }, new ReceiverInfo
                    {
                        Code = "UNREACHABLE",
                        AOOCode = "PAT",
                        AdministrationCode = "PAT_TEST"
                    } },
                    MainDocument = new DocumentInfo
                    {
                        Name = "test.pdf",
                        NumberOfPages = 1,
                        DocumentServerLocation = string.Empty,
                        FilePath = path,
                        FileName = "test.pdf",
                        DocumentNumber = receiverRecordNumber.ToString(),
                        VersionId = receiverRecordNumber.ToString(),
                        Version = "1",
                        VersionLabel = "Versione test",
                        Fingerprint = string.Empty
                    },
                    Attachments = { },
                    Note = string.Empty,
                    IsPrivate = true,
                    ReceiverAdministrationCode = tenant
                }
            };

            var json = JsonConvert.SerializeObject(apiRequest);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{this._controllerUrl}/ElaborateNewInteroperabilityMessage") { Content = content };

            _apiHttpClient.DefaultRequestHeaders.Add("Tenant", tenant);

            var response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var apiResponse = JsonConvert.DeserializeObject<ElaborateNewInteroperabilityMessageResponse>(responseContent);

                if (!string.IsNullOrWhiteSpace(apiResponse?.Result?.DocumentDelivered?.MainDocument?.DocumentNumber) && apiResponse.Result.SingleRequestErrors.Any(x=> x.ErrorMessage == "Nessun destinatario trovato per la trasmissione del documento."))
                {
                    Assert.Pass($"Creato documento con id={apiResponse?.Result?.DocumentDelivered?.MainDocument?.DocumentNumber}\r\nDestinatario con codice {apiResponse?.Result?.SingleRequestErrors.First().Receivers.First().Code} non raggiunto.");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(apiResponse?.Result?.DocumentDelivered?.MainDocument?.DocumentNumber))
                    {
                        Assert.Fail("Documento non creato");
                    }
                    else
                    {
                        var errors = string.Join(Environment.NewLine, apiResponse.Result.SingleRequestErrors.Select(x => x.ErrorMessage));
                        Assert.Fail($"Errori nell'elaborazione: {errors}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(3)]
        [Ignore("test ingorato temporaneamente")]
        public async Task ElaborateNewInteroperabilityMessageRequestSaveErrorTest()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int length = 2500;

            int senderRecordNumber = random.Next(1, 99999);
            int receiverRecordNumber = random.Next(1, 99999);

            var receiverCode = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());

            string tenant = "PAT_TEST";

            var path = Path.Combine(Environment.CurrentDirectory, "testfile.pdf");

            File.WriteAllBytes(path, Files.LoremIpsum);

            var apiRequest = new ElaborateNewInteroperabilityMessageRequest
            {
                InteroperabilityMessage = new InteroperabilityMessage
                {
                    Record = new RecordInfo
                    {
                        AdministrationCode = tenant,
                        RecordNumber = senderRecordNumber.ToString(),
                        RecordDate = DateTime.Now,
                        AOOCode = "PAT",
                        Subject = "Test_" + Guid.NewGuid().ToString()
                    },
                    Sender = new SenderInfo
                    {
                        Url = "http://localhost",
                        Code = "PAT_TEST-RFS202",
                        AdministrationId = tenant,
                        UserId = "TEST_PARER",
                        FileManagerUrl = string.Empty
                    },
                    Receivers = new List<ReceiverInfo> { new ReceiverInfo
                    {
                        Code = "PAT_TEST_RFA034",
                        AOOCode = receiverCode,
                        AdministrationCode = "PAT_TEST"
                    }},
                    MainDocument = new DocumentInfo
                    {
                        Name = "test.pdf",
                        NumberOfPages = 1,
                        DocumentServerLocation = string.Empty,
                        FilePath = path,
                        FileName = "test.pdf",
                        DocumentNumber = receiverRecordNumber.ToString(),
                        VersionId = receiverRecordNumber.ToString(),
                        Version = "1",
                        VersionLabel = "Versione test",
                        Fingerprint = string.Empty
                    },
                    Attachments = { },
                    Note = string.Empty,
                    IsPrivate = true,
                    ReceiverAdministrationCode = tenant
                }
            };

            var json = JsonConvert.SerializeObject(apiRequest);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{this._controllerUrl}/ElaborateNewInteroperabilityMessage") { Content = content };

            _apiHttpClient.DefaultRequestHeaders.Add("Tenant", tenant);

            var response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var apiResponse = JsonConvert.DeserializeObject<ElaborateNewInteroperabilityMessageResponse>(responseContent);

                if(string.IsNullOrWhiteSpace(apiResponse.Result.MessageId) && apiResponse.Result.SingleRequestErrors.Count == 1
                    && apiResponse.Result.SingleRequestErrors.First().ErrorMessage == "Errore durante il salvataggio delle informazioni sulla richiesta di interoperabilità.")
                {
                    Assert.Pass("Gestita eccezione per mancato inserimento tra i messaggi ricevuti.");
                }
                else
                {
                    Assert.Fail("Eccezione non gestita");
                }
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }


        [Test, Order(4)]
        public async Task AnalyzeDocumentReceivedProofTest()
        {
            var random = new Random();

            int senderRecordNumber = random.Next(1, 99999);
            int receiverRecordNumber = random.Next(1, 99999);

            string tenant = "PAT_TEST";

            var apiRequest = new AnalyzeDocumentReceivedProofRequest
            {
                SenderRecordInfo = new RecordInfo
                {
                    AdministrationCode = "PAT_TEST",
                    RecordNumber = "15456",
                    RecordDate = new DateTime(2023, 7, 30, 12, 8, 22),
                    AOOCode = "PAT",
                    Subject = "TEST_SENDER" + senderRecordNumber.ToString()
                },
                ReceiverRecordInfo = new RecordInfo
                {
                    AdministrationCode = "C6",
                    RecordNumber = receiverRecordNumber.ToString(),
                    RecordDate = DateTime.Now,
                    AOOCode = "C6",
                    Subject = "TEST_RECEIVER_" + receiverRecordNumber.ToString()
                },
                ReceiverUrl = "http://test.pitre.tn.it/comprensori_test-be/FileService.svc",
                ReceiverCode = "C6-C6"
            };

            var json = JsonConvert.SerializeObject(apiRequest);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{this._controllerUrl}/AnalyzeDocumentReceivedProof") { Content = content };

            _apiHttpClient.DefaultRequestHeaders.Add("Tenant", tenant);

            var response = await this._apiHttpClient.SendAsync(request);

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                Assert.Pass("Test concluso con successo");
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test, Order(5)]        
        public async Task AnalyzeDocumentDroppedProofTest()
        {
            var random = new Random();

            int senderRecordNumber = random.Next(1, 99999);
            int receiverRecordNumber = random.Next(1, 99999);

            string tenant = "PAT_TEST";

            var apiRequest = new AnalyzeDocumentDroppedOrErrorMessageProofRequest
            {
                SenderRecordInfo = new RecordInfo
                {
                    AdministrationCode = "PAT_TEST",
                    RecordNumber = "35231",
                    RecordDate = new DateTime(2023, 11, 23, 17, 41, 32),
                    AOOCode = "PAT",
                    Subject = "Test_" + senderRecordNumber.ToString()
                },
                ReceiverRecordInfo = new RecordInfo
                {
                    AdministrationCode = "PAT_TEST",
                    RecordNumber = receiverRecordNumber.ToString(),
                    RecordDate = DateTime.Now,
                    AOOCode = "PAT",
                    Subject = "Test_" + receiverRecordNumber.ToString()
                },
                Reason = "Test motivo rifiuto",
                ReceiverUrl = "http://localhost",
                Operation = OperationDiscriminator.Drop,
                ReceiverCode = "PAT"
            };

            var json = JsonConvert.SerializeObject(apiRequest);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{this._controllerUrl}/AnalyzeDocumentDroppedOrErrorMessageProof") { Content = content };

            _apiHttpClient.DefaultRequestHeaders.Add("Tenant", tenant);

            var response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                Assert.Pass("Test concluso con successo");
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }

        }

        [Test, Order(6)]
        public async Task AnalyzeDocumentErrorProofTest()
        {
            var random = new Random();

            int senderRecordNumber = random.Next(1, 99999);
            int receiverRecordNumber = random.Next(1, 99999);

            string tenant = "PAT_TEST";

            var apiRequest = new AnalyzeDocumentDroppedOrErrorMessageProofRequest
            {
                SenderRecordInfo = new RecordInfo
                {
                    AdministrationCode = "PAT_TEST",
                    RecordNumber = "35231",
                    RecordDate = new DateTime(2023, 11, 23, 17, 41, 32),
                    AOOCode = "PAT",
                    Subject = "Test_" + senderRecordNumber.ToString()
                },
                ReceiverRecordInfo = new RecordInfo
                {
                    AdministrationCode = "PAT_TEST",
                    RecordNumber = receiverRecordNumber.ToString(),
                    RecordDate = DateTime.Now,
                    AOOCode = "PAT",
                    Subject = "Test_" + receiverRecordNumber.ToString()
                },
                Reason = "Test motivo eccezione",
                ReceiverUrl = "http://localhost",
                Operation = OperationDiscriminator.Error,
                ReceiverCode = "PAT"
            };

            var json = JsonConvert.SerializeObject(apiRequest);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{this._controllerUrl}/AnalyzeDocumentDroppedOrErrorMessageProof") { Content = content };

            _apiHttpClient.DefaultRequestHeaders.Add("Tenant", tenant);

            var response = await this._apiHttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                Assert.Pass("Test concluso con successo");
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }
    }
}
