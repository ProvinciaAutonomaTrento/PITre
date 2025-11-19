// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddCorrespondent;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddCorrespondentAdvanced;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.EditCorrespondent;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.EditCorrespondentAdvanced;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrespondent;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrespondentAdvanced;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrespondentFilters;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetUserFilters;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchCorrespondents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchCorrespondentsAdvanced;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchUsers;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentAndAddInProject;
using System.Text;

namespace Pi3.App.Legacy.Pis.WebApi.Tests
{
    public class AddressBookV1Tests
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
        public async Task SearchCorrespondentsAdvanced()
        {
            var request = JsonConvert.DeserializeObject<SearchCorrespondentsAdvancedCommand>(Files.SearchCorrespondentsAdvanced_Actual);
            var requestMsg = TestUtils.SerializeRequest(request, HttpMethod.Post, "SearchCorrespondentsAdvanced");
            var response = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SearchCorrespondentsAdvancedCommandResponse>(responseContent);

                if (result == null || !string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Assert.Fail(string.Format(Messages.TestFailed, "SearchCorrespondentsAdvanced"));
                }
                Assert.Pass(string.Format(Messages.TestPassed, "SearchCorrespondentsAdvanced"));
            }
            else
            {
                Assert.Fail(string.Format(Messages.TestFailed, "SearchCorrespondentsAdvanced"));
            }

        }

        [Test]
        public async Task SearchCorrespondentsTest()
        { 
            SearchCorrespondentsCommand request = JsonConvert.DeserializeObject<SearchCorrespondentsCommand>(Files.SearchCorrespondents_Actual);
            var requestMsg = TestUtils.SerializeRequest(request, HttpMethod.Post, "SearchCorrespondents");
            var response = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                SearchCorrespondentsCommandResponse result = JsonConvert.DeserializeObject<SearchCorrespondentsCommandResponse>(responseContent);

                if(result == null || !string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Assert.Fail(string.Format(Messages.TestFailed, "SearchCorrespondents"));
                }
                Assert.Pass(string.Format(Messages.TestPassed, "SearchCorrespondents"));
            }
            else
            {
                Assert.Fail(string.Format(Messages.TestFailed, "SearchCorrespondents"));
            }

        }

        [Test]
        public async Task GetCorrespondentFiltersTest()
        {
            HttpResponseMessage response2 = await this._apiHttpClient.GetAsync("GetCorrespondentFilters");
            if (response2.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response2.StatusCode}");
                string responseContent = await response2.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetCorrespondentFiltersCommandResponse result = JsonConvert.DeserializeObject<GetCorrespondentFiltersCommandResponse>(responseContent);
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
                Console.WriteLine($"Errore: {response2.StatusCode}");
                string responseContent = await response2.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task GetUserFiltersTest()
        {
            HttpResponseMessage response2 = await this._apiHttpClient.GetAsync("GetUserFilters");
            if (response2.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response2.StatusCode}");
                string responseContent = await response2.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetUserFiltersCommandResponse result = JsonConvert.DeserializeObject<GetUserFiltersCommandResponse>(responseContent);
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
                Console.WriteLine($"Errore: {response2.StatusCode}");
                string responseContent = await response2.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task SearchUsersTest()
        {
            List<Filter> filters = new List<Filter>();
            filters.Add(new Filter() { Name = "USER_NAME", Value = "alessandro" });

            var suComm = new SearchUsersCommand()
            {
                Filters = filters.ToArray()
            };
            var json = JsonConvert.SerializeObject(suComm);
            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "SearchUsers")
            {
                Content = content2
            };
            var response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine($"Risposta JSON: {responseContent}");

                SearchUsersCommandResponse result = JsonConvert.DeserializeObject<SearchUsersCommandResponse>(responseContent);
                if (result != null && result.Users != null && result.Users.Any())
                {
                    Console.WriteLine($"Utenti trovati: {result.Users.Length}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Utenti non trovati");
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
        public async Task GetCorrespondentTest()
        {
            var getCorrespondentReq = JsonConvert.DeserializeObject<GetCorrespondentCommand>(Files.GetCorrespondent_Actual);
            var queryParams = TestUtils.GetQueryStringFromEncodedJson(JsonConvert.SerializeObject(getCorrespondentReq));

            var getCorrespondentRes = await this._apiHttpClient.GetAsync("GetCorrespondent" + queryParams);

            if (getCorrespondentRes.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {getCorrespondentRes.StatusCode}");
                string responseContent = await getCorrespondentRes.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetCorrespondentCommandResponse result = JsonConvert.DeserializeObject<GetCorrespondentCommandResponse>(responseContent);
                if (result != null && result.Correspondent != null && !string.IsNullOrWhiteSpace(result.Correspondent.Id))
                {
                    Console.WriteLine($"Corrispondente trovato. ID: {result.Correspondent.Id}, Codice: {result.Correspondent.Code}, descrizione: {result.Correspondent.Description}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Corrispondente non trovato");
                }

            }
            else
            {
                Console.WriteLine($"Errore: {getCorrespondentRes.StatusCode}");
                string responseContent = await getCorrespondentRes.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }

        }

        [Test]
        public async Task GetCorrespondentAdvancedTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetCorrespondentAdvanced?IdCorrespondent=131266924");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetCorrespondentAdvancedCommandResponse result = JsonConvert.DeserializeObject<GetCorrespondentAdvancedCommandResponse>(responseContent);
                if (result != null && result.Correspondent != null && !string.IsNullOrWhiteSpace(result.Correspondent.Id))
                {
                    Console.WriteLine($"Corrispondente trovato. ID: {result.Correspondent.Id}, Codice: {result.Correspondent.Code}, descrizione: {result.Correspondent.Description}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Corrispondente non trovato");
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
        public async Task AddCorrespondentTest()
        {
            var uniqueDate = DateTime.Now.ToString("yyyyMMddHHmmssffffff");

            var newCorr = new Correspondent()
            {
                Code = "TST_P3CORE_" + uniqueDate,
                Description = "Test Creazione Corr P3Core " + uniqueDate,
                Email = "testmail@testmail.it",
                Note = "Creazione tramite test Automatico P3Core",
                Type = "E",
                CorrespondentType = "U",
                PreferredChannel = "MAIL"
            };

            var addCorrReq = new AddCorrespondentCommand()
            {
                Correspondent = newCorr
            };
            var json = JsonConvert.SerializeObject(addCorrReq);
            Console.WriteLine($"Richiesta JSON: {json}");
            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, "AddCorrespondent")
            {
                Content = content2
            };
            var response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AddCorrespondentCommandResponse>(responseContent);
                if (result != null && result.Correspondent != null && !string.IsNullOrWhiteSpace(result.Correspondent.Id))
                {
                    Console.WriteLine($"Corrispondente creato. ID: {result.Correspondent.Id}");
                    Assert.Pass("Test superato.");
                }
                else
                {
                    Assert.Fail("Errore nella creazione del corrispondente.");
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
        public async Task FlussoAggiuntaEModifica()
        {
            var uniqueDate = DateTime.Now.ToString("yyyyMMddHHmmssffffff");

            var newCorr = new Correspondent()
            {
                Code = "TST_P3CORE_" + uniqueDate,
                Description = "Test Flusso Corr P3Core " + uniqueDate,
                Email = "testmail@testmail.it",
                Note = "Creazione tramite test Automatico P3Core",
                Type = "E",
                CorrespondentType = "U",
                PreferredChannel = "MAIL",
                Address = "Via Roma 1",
                Cap = "80121",
                City = "Benevento",
                IsCommonAddress = false,
                AdmCode = "",
                AOOCode = "",
                CodeRegisterOrRF = null!,
                Fax = "123123123",
                Location = "Rione Libertà",
                Nation="Italia",
                NationalIdentificationNumber="",
                PhoneNumber="123123123",
                PhoneNumber2="345345345",
                Province="BN",
                VatNumber= "00488410010"

            };

            var addCorrReq = new AddCorrespondentCommand()
            {
                Correspondent = newCorr
            };
            var json = JsonConvert.SerializeObject(addCorrReq);
            Console.WriteLine($"Richiesta JSON: {json}");
            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, "AddCorrespondent")
            {
                Content = content2
            };
            var response = await this._apiHttpClient.SendAsync(request);
            string idCorrCreato = "";
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AddCorrespondentCommandResponse>(responseContent);
                if (result != null && result.Correspondent != null && !string.IsNullOrWhiteSpace(result.Correspondent.Id))
                {
                    Console.WriteLine($"Corrispondente creato. ID: {result.Correspondent.Id}");
                    idCorrCreato = result.Correspondent.Id;
                }
                else
                {
                    Assert.Fail("Errore nella creazione del corrispondente.");
                }
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }

            var editCorr = new Correspondent()
            {
                Id= idCorrCreato,
                Description = "Test Flusso Corr P3Core - Modifica - " + uniqueDate,
                Email = "testmail@testmail.it",
                Note = "Creazione e modifica tramite test Automatico P3Core",
                Type = "E",
                CorrespondentType = "U",
                PreferredChannel = "MAIL",
                Address = "Via Napoli 1",
                Cap = "80121",
                City = "Benevento",
                IsCommonAddress = false,
                AdmCode = "",
                AOOCode = "",
                CodeRegisterOrRF = null!,
                Fax = "123123123",
                Location = "Rione Libertà",
                Nation = "Italia",
                NationalIdentificationNumber = "",
                PhoneNumber = "123123123",
                PhoneNumber2 = "345345345",
                Province = "BN",
                VatNumber = "00488410010"
            };

            var editCorrReq = new EditCorrespondentCommand()
            {
                Correspondent = editCorr
            };
            json = JsonConvert.SerializeObject(editCorrReq);
            Console.WriteLine($"Richiesta JSON: {json}");
            using var content3 = new StringContent(json, Encoding.UTF8, "application/json");
            request = new HttpRequestMessage(HttpMethod.Post, "EditCorrespondent")
            {
                Content = content3
            };
            response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<EditCorrespondentCommandResponse>(responseContent);
                if (result != null && result.Correspondent != null && !string.IsNullOrWhiteSpace(result.Correspondent.Id))
                {
                    Console.WriteLine($"Corrispondente modificato. ID: {result.Correspondent.Id}");
                    Assert.Pass($"Corrispondente modificato. ID: {result.Correspondent.Id}, Descrizione: {result.Correspondent.Description}");
                }
                else
                {
                    Assert.Fail("Errore nella modifica del corrispondente.");
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
        public async Task FlussoAvanzatoAggiuntaEModifica()
        {
            var uniqueDate = DateTime.Now.ToString("yyyyMMddHHmmssffffff");

            var listEmails = new List<CorrespondentEmail>();
            string emailPrinc = $"TestPrincipale{uniqueDate}@email.it";
            listEmails.Add(new CorrespondentEmail() { Email = emailPrinc, Main="1", Note="principale" });
            listEmails.Add(new CorrespondentEmail() { Email = $"TestSecondaria{uniqueDate}@email.it", Main = "0", Note = "secondaria" });
            

            var newCorr = new CorrespondentAdvanced()
            {
                Code = "TST_P3CORE_" + uniqueDate,
                Description = "Test Flusso Corr P3Core " + uniqueDate,
                Email = emailPrinc,
                Note = "Creazione corrispondente avanzato Automatico P3Core",
                Type = "E",
                CorrespondentType = "U",
                PreferredChannel = "MAIL",
                Address = "Via Roma 1",
                Cap = "80121",
                City = "Benevento",
                IsCommonAddress = false,
                AdmCode = "",
                AOOCode = "",
                CodeRegisterOrRF = null!,
                Fax = "123123123",
                Location = "Rione Libertà",
                Nation = "Italia",
                NationalIdentificationNumber = "",
                PhoneNumber = "123123123",
                PhoneNumber2 = "345345345",
                Province = "BN",
                VatNumber = "00488410010",
                EmailsDetailed = listEmails
            };

            var addCorrReq = new AddCorrespondentAdvancedCommand()
            {
                Correspondent = newCorr
            };
            var json = JsonConvert.SerializeObject(addCorrReq);
            Console.WriteLine($"Richiesta JSON: {json}");
            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, "AddCorrespondentAdvanced")
            {
                Content = content2
            };
            var response = await this._apiHttpClient.SendAsync(request);
            string idCorrCreato = "";
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<AddCorrespondentAdvancedCommandResponse>(responseContent);
                if (result != null && result.Correspondent != null && !string.IsNullOrWhiteSpace(result.Correspondent.Id))
                {
                    Console.WriteLine($"Corrispondente creato. ID: {result.Correspondent.Id}");
                    idCorrCreato = result.Correspondent.Id;
                }
                else
                {
                    Assert.Fail("Errore nella creazione del corrispondente.");
                }
            }
            else
            {
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
            listEmails.Add(new CorrespondentEmail() { Email = $"altra{uniqueDate}@email.it", Main = "0", Note = "terza" });

            var editCorr = new CorrespondentAdvanced()
            {
                Id = idCorrCreato,
                Description = "Test Flusso Corr P3Core - Modifica - " + uniqueDate,
                Email = emailPrinc,
                Note = "Creazione e modifica corrispondente avanzato Automatico P3Core",
                Type = "E",
                CorrespondentType = "U",
                PreferredChannel = "MAIL",
                Address = "Via Napoli 1",
                Cap = "80121",
                City = "Benevento",
                IsCommonAddress = false,
                AdmCode = "",
                AOOCode = "",
                CodeRegisterOrRF = null!,
                Fax = "123123123",
                Location = "Rione Libertà",
                Nation = "Italia",
                NationalIdentificationNumber = "",
                PhoneNumber = "123123123",
                PhoneNumber2 = "345345345",
                Province = "BN",
                VatNumber = "00488410010"
            };

            var editCorrReq = new EditCorrespondentAdvancedCommand()
            {
                Correspondent = editCorr
            };
            json = JsonConvert.SerializeObject(editCorrReq);
            Console.WriteLine($"Richiesta JSON: {json}");
            using var content3 = new StringContent(json, Encoding.UTF8, "application/json");
            request = new HttpRequestMessage(HttpMethod.Post, "EditCorrespondentAdvanced")
            {
                Content = content3
            };
            response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<EditCorrespondentAdvancedCommandResponse>(responseContent);
                if (result != null && result.Correspondent != null && !string.IsNullOrWhiteSpace(result.Correspondent.Id))
                {
                    Console.WriteLine($"Corrispondente modificato. ID: {result.Correspondent.Id}");
                    Assert.Pass($"Corrispondente modificato. ID: {result.Correspondent.Id}, Descrizione: {result.Correspondent.Description}");
                }
                else
                {
                    Assert.Fail("Errore nella modifica del corrispondente.");
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
