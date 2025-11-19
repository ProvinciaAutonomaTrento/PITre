// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocAccessRights;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocStateDiagram;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentEvents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentFilters;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentTemplate;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentTemplates;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetModifiedDocuments;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SearchDocEvents;
using System.Net.Mime;
using System.Text;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SearchDocuments;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadBigFileInChunks;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadFileToDocument;
using System.Reflection;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentAndAddInProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ProtocolPredisposed;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.RemoveDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ImportPreviousDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetStampAndSignature;
using Org.BouncyCastle.Crypto.Generators;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateFolder;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFileWithSignatureOrStamp;
using DocsPaVO.ProfilazioneDinamicaLite;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.EditDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SendDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.EditDocStateDiagram;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentsInProject;

namespace Pi3.App.Legacy.Pis.WebApi.Tests
{
    public class DocumentsV1Tests
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
        public async Task GetDocumentsInProjectByCodeProjectAndClassificationSchemeIdTest()
        {
            var getDocumentsInProjectCommand = JsonConvert.DeserializeObject<GetDocumentsInProjectCommand>(Files.GetDocumentsInProjectByCodeProjectAndClassificationSchemeId_Actual);
            var requestMsg = TestUtils.SerializeRequest(getDocumentsInProjectCommand, HttpMethod.Post, "GetDocumentsInProject");

            var getDocumentsInProjectResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (getDocumentsInProjectResponse.IsSuccessStatusCode)
            {
                string responseContent = await getDocumentsInProjectResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<GetDocumentsInProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {getDocumentsInProjectResponse.StatusCode}");
                string responseContent = await getDocumentsInProjectResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task GetDocumentsInProjectByIdProjectTest()
        {
            var getDocumentsInProjectCommand = JsonConvert.DeserializeObject<GetDocumentsInProjectCommand>(Files.GetDocumentsInProjectByIdProject_Actual);
            var requestMsg = TestUtils.SerializeRequest(getDocumentsInProjectCommand, HttpMethod.Post, "GetDocumentsInProject");

            var getDocumentsInProjectResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (getDocumentsInProjectResponse.IsSuccessStatusCode)
            {
                string responseContent = await getDocumentsInProjectResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<GetDocumentsInProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {getDocumentsInProjectResponse.StatusCode}");
                string responseContent = await getDocumentsInProjectResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task EditDocStateDiagramWithTrasmTest()
        {
            var createDocumentRequest = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommand>(Files.CreateDocumentAndAddInProjectWithTempAndStateDiagramTrasm_Actual);
            string idDoc = string.Empty;
            var requestMsg = TestUtils.SerializeRequest(createDocumentRequest, HttpMethod.Put, "CreateDocumentAndAddInProject");

            var createDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (createDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
                idDoc = responseDecoded.Document.Id;

            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }

            var req = JsonConvert.DeserializeObject<EditDocStateDiagramCommand>(Files.EditDocStateDiagram_Actual);

            req.IdDocument = idDoc;

            var editMsg = TestUtils.SerializeRequest(req, HttpMethod.Post, "EditDocStateDiagram");

            var resp = await this._apiHttpClient.SendAsync(editMsg);
            editMsg.Dispose();


            if (resp.IsSuccessStatusCode)
            {
                string responseContent = await resp.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<EditDocStateDiagramCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();

            }
            else
            {
                Console.WriteLine($"Errore: {resp.StatusCode}");
                string responseContent = await resp.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        /*
        [Test]
        public async Task SendDocumentWithInteropCaseTest()
        {
            #region Gen Token
            var codAmm = Environment.GetEnvironmentVariable("tenant");
            var userId = Environment.GetEnvironmentVariable("userId");
            var groupCode = Environment.GetEnvironmentVariable("groupCode");
            var instance = Environment.GetEnvironmentVariable("instance");
            var codeApp = Environment.GetEnvironmentVariable("codeApp");

            string token = await TestUtils.GetToken(codAmm, userId, groupCode, codeApp, instance, this._apiHttpClient);
            if (string.IsNullOrEmpty(token)) Assert.Fail("Token non generato");
            else _apiHttpClient.DefaultRequestHeaders.Add("AuthToken", token);
            #endregion


            var createDocumentRequest = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommand>(Files.CreateDocumentAndAddInProjectWithRecInterop);
            string idDoc = string.Empty;
            var requestMsg = TestUtils.SerializeRequest(createDocumentRequest, HttpMethod.Put, "CreateDocumentAndAddInProject");

            var createDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (createDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
                idDoc = responseDecoded.Document.Id;

            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }

            SendDocumentCommand sendReq = new()
            {
                IdDocument = idDoc
            };


            var req = TestUtils.SerializeRequest(sendReq, HttpMethod.Post, "SendDocument");

            var sendResp = await this._apiHttpClient.SendAsync(req);

            if (sendResp.IsSuccessStatusCode)
            {
                string responseContent = await sendResp.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<SendDocumentCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {sendResp.StatusCode}");
                string responseContent = await sendResp.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }

        }
        */
        [Test]
        public async Task GetStampAndSignatureTest()
        {         
            var createDocumentRequest = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommand>(Files.CreateDocumentAndAddInProject_Actual);
            string idDoc = string.Empty;
            var requestMsg = TestUtils.SerializeRequest(createDocumentRequest, HttpMethod.Put, "CreateDocumentAndAddInProject");

            var createDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (createDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
                idDoc = responseDecoded.Document.Id;

            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }


            GetStampAndSignatureCommand reqGetStampAndSignature = new()
            {
                idDocument = idDoc,
                signature = string.Empty
            };

            var queryParams = TestUtils.GetQueryStringFromEncodedJson(JsonConvert.SerializeObject(reqGetStampAndSignature));
            var resGetStampAndSignature = await this._apiHttpClient.GetAsync("GetStampAndSignature" + queryParams);

            if (resGetStampAndSignature.IsSuccessStatusCode)
            {
                string responseContent = await resGetStampAndSignature.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<GetStampAndSignatureCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();

            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }


        [Test]
        public async Task RemoveDocumentTest()
        {
            var createDocumentRequest = JsonConvert.DeserializeObject<CreateDocumentCommand>(Files.CreateDocumentGrigio_Actual);
            var requestMsg = TestUtils.SerializeRequest(createDocumentRequest, HttpMethod.Put, "CreateDocument");
            string idDoc = string.Empty;
            var createDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (createDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateDocumentCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
                idDoc = responseDecoded.Document.Id;
            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }

            var removeDocumentRequest = JsonConvert.DeserializeObject<RemoveDocumentCommand>(Files.RemoveDocument_Actual);
            removeDocumentRequest.IdDocument = idDoc;
            var msg = TestUtils.SerializeRequest(removeDocumentRequest, HttpMethod.Post, "RemoveDocument");

            var removeDocumentResponse = await this._apiHttpClient.SendAsync(msg);
            msg.Dispose();

            if (removeDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await removeDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<RemoveDocumentCommandResponse>(responseContent);
                if (responseDecoded == null)
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {removeDocumentResponse.StatusCode}");
                string responseContent = await removeDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task EditDocumentCounterTest()
        {
            var editDocumentRequest = JsonConvert.DeserializeObject<EditDocumentCommand>(Files.EditDocumentContatore_Actual);
            var requestMsg = TestUtils.SerializeRequest(editDocumentRequest, HttpMethod.Post, "EditDocument");

            var editDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (editDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await editDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<EditDocumentCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {editDocumentResponse.StatusCode}");
                string responseContent = await editDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task EditDocumentAssociazioneCampiDaXMLTest()
        {
            var editDocumentRequest = JsonConvert.DeserializeObject<EditDocumentCommand>(Files.EditDocumentAssociazioneCampiDaXML_Actual);
            var requestMsg = TestUtils.SerializeRequest(editDocumentRequest, HttpMethod.Post, "EditDocument");

            var editDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (editDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await editDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<EditDocumentCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {editDocumentResponse.StatusCode}");
                string responseContent = await editDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task EditDocumentFieldDiTestoTest()
        {
            var editDocumentRequest = JsonConvert.DeserializeObject<EditDocumentCommand>(Files.EditDocumentFieldDiTesto_Actual);
            var requestMsg = TestUtils.SerializeRequest(editDocumentRequest, HttpMethod.Post, "EditDocument");

            var editDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (editDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await editDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<EditDocumentCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {editDocumentResponse.StatusCode}");
                string responseContent = await editDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task EditDocumentDataTest()
        {
            var editDocumentRequest = JsonConvert.DeserializeObject<EditDocumentCommand>(Files.EditDocumentModificaData_Actual);
            var requestMsg = TestUtils.SerializeRequest(editDocumentRequest, HttpMethod.Post, "EditDocument");

            var editDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (editDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await editDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<EditDocumentCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {editDocumentResponse.StatusCode}");
                string responseContent = await editDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task EditDocumentSubCounterTest()
        {
            var editDocumentRequest = JsonConvert.DeserializeObject<EditDocumentCommand>(Files.EditDocumentSottocontatore_Actual);
            var requestMsg = TestUtils.SerializeRequest(editDocumentRequest, HttpMethod.Post, "EditDocument");

            var editDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (editDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await editDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<EditDocumentCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {editDocumentResponse.StatusCode}");
                string responseContent = await editDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task CreateDocumentTest()
        {
            var createDocumentRequest = JsonConvert.DeserializeObject<CreateDocumentCommand>(Files.CreateDocumentGrigio_Actual);
            var requestMsg = TestUtils.SerializeRequest(createDocumentRequest, HttpMethod.Put, "CreateDocument");

            var createDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (createDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateDocumentCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task CreateDocumentAndAddInProjectTest()
        {
            var createDocumentRequest = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommand>(Files.CreateDocumentAndAddInProject_Actual);

            var requestMsg = TestUtils.SerializeRequest(createDocumentRequest, HttpMethod.Put, "CreateDocumentAndAddInProject");

            var createDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (createDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task CreateDocumentAndAddInProjectWithAutomaticAssTest()
        {
            var createDocumentRequest = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommand>(Files.CreateDocumentAndAddInProjectWithAutomaticAssoc_Actual);

            var requestMsg = TestUtils.SerializeRequest(createDocumentRequest, HttpMethod.Put, "CreateDocumentAndAddInProject");

            var createDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            if (createDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task ProtocolPredisposedTest()
        {
            var createDocumentRequest = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommand>(Files.CreateDocumentAndAddInProject_Actual);

            createDocumentRequest.Document.Predisposed = true;
            var requestMsg = TestUtils.SerializeRequest(createDocumentRequest, HttpMethod.Put, "CreateDocumentAndAddInProject");

            var createDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();
            string docId = string.Empty;
            if (createDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();
                else
                    docId = responseDecoded.Document.Id;
            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
            ProtocolPredisposedCommand req = new()
            {
                IdDocument = docId,
                CodeRegister = createDocumentRequest.CodeRegister,
            };
            var protoPredMsg = TestUtils.SerializeRequest(req, HttpMethod.Post, "ProtocolPredisposed");
            var protoPredResponse = await this._apiHttpClient.SendAsync(protoPredMsg);
            protoPredMsg.Dispose();

            if (protoPredResponse.IsSuccessStatusCode)
            {
                string responseContent = await protoPredResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<ProtocolPredisposedCommandResponse>(responseContent);
                if (responseDecoded == null)
                    Assert.Fail();
            }
            else
            {
                Console.WriteLine($"Errore: {protoPredResponse.StatusCode}");
                string responseContent = await protoPredResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task UploadBigFileInChunksTest()
        {
            #region compute chunks, expected hash, fetch fileName from request
            if (Files.UploadBigFileInChunks_File.Length == 0)
                Assert.Fail();
            int chunkMaxSize = 1024 * 4096;
            string? hash = TestUtils.ComputeHash(Files.UploadBigFileInChunks_File);
            IEnumerable<byte[]> chunks = Files.UploadBigFileInChunks_File.Chunk(chunkMaxSize);
            var fileNameJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(Files.UploadBigFileInChunks_FileName);
            if (fileNameJson == null || !fileNameJson.ContainsKey("fileName") || string.IsNullOrEmpty(fileNameJson["fileName"]))
                Assert.Fail();

            string fileName = fileNameJson["fileName"];

            if (fileName.Length == 0)
                Assert.Fail();
            #endregion
            #region create document
            var createDocumentRequest = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommand>(Files.CreateDocumentAndAddInProjectGrigio_Actual);
            var requestMsg = TestUtils.SerializeRequest(createDocumentRequest, HttpMethod.Put, "CreateDocumentAndAddInProject");

            var createDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();

            string idDocumentCreated = string.Empty;

            if (createDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommandResponse>(responseContent);
                if (responseDecoded == null)
                    Assert.Fail();
                else
                    idDocumentCreated = responseDecoded.Document.Id;
            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
            #endregion
            #region upload
            HttpResponseMessage uploadChunkResponseMsg = null;
            UploadBigFileInChunksCommand? uploadBigFileInChunksRequest = new()
            {
                FileName = fileName,
                IdDocument = idDocumentCreated,
            };
            string phase = "START";
            int chunkNumber = 1;
            foreach (var chunk in chunks)
            {
                if (chunkNumber != 1)
                {
                    phase = "CHUNK";
                }
                uploadBigFileInChunksRequest.ChunkContent = chunk;
                uploadBigFileInChunksRequest.ChunkNumber = chunkNumber;
                uploadBigFileInChunksRequest.Phase = phase;
                chunkNumber++;

                requestMsg = TestUtils.SerializeRequest(uploadBigFileInChunksRequest, HttpMethod.Post ,"UploadBigFileInChunks");
                uploadChunkResponseMsg = await this._apiHttpClient.SendAsync(requestMsg);
                requestMsg.Dispose();
                if (!uploadChunkResponseMsg.IsSuccessStatusCode)
                {
                    string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                    Assert.Fail(responseContent);
                }
                else
                {
                    string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                    var responseDecoded = JsonConvert.DeserializeObject<UploadBigFileInChunksCommandResponse>(responseContent);
                    if(responseDecoded == null || responseDecoded.Code != MessageResponseCode.OK)
                    {
                        Assert.Fail(responseContent);
                    }
                }

            }

            uploadBigFileInChunksRequest.Phase = "END";
            requestMsg = TestUtils.SerializeRequest(uploadBigFileInChunksRequest, HttpMethod.Post, "UploadBigFileInChunks");
            uploadChunkResponseMsg = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();
            if (!uploadChunkResponseMsg.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
            else
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<UploadBigFileInChunksCommandResponse>(responseContent);
                if (responseDecoded == null || responseDecoded.Code != MessageResponseCode.OK)
                {
                    Assert.Fail(responseContent);
                }
            }
            #endregion
            Console.WriteLine($" UploadBigFileInChunks upload successfull on doc: {idDocumentCreated}");
            Assert.Pass();
        }
        

        [Test]
        public async Task UploadFileToDocumentTest()
        {
            #region create document
            string idDocumentCreated = string.Empty;
            var createDocumentRequest = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommand>(Files.CreateDocumentAndAddInProjectGrigio_Actual);
            var requestMsg = TestUtils.SerializeRequest(createDocumentRequest, HttpMethod.Put, "CreateDocumentAndAddInProject");

            var createDocumentResponse = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();


            if (createDocumentResponse.IsSuccessStatusCode)
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<CreateDocumentAndAddInProjectCommandResponse>(responseContent);
                if (responseDecoded == null)
                    Assert.Fail();
                else
                    idDocumentCreated = responseDecoded.Document.Id;
            }
            else
            {
                Console.WriteLine($"Errore: {createDocumentResponse.StatusCode}");
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }

            #endregion

            var requestFilePart = JsonConvert.DeserializeObject<Dictionary<string, string>>(Files.UploadFileToDocument_FileMetadata);
            #region file data validation
            if (requestFilePart == null || 
                !requestFilePart.ContainsKey("Description") ||
                !requestFilePart.ContainsKey("Name") ||
                !requestFilePart.ContainsKey("MimeType")
                )
            {
                Assert.Fail();
                Console.WriteLine(Messages.UploadFileToDocumentMissingParams);
            }
            #endregion

            string? hash = TestUtils.ComputeHash(Files.UploadFileToDocument_File);
            UploadFileToDocumentCommand request = new()
            {
                File = new() 
                { 
                    Description = requestFilePart["Description"],
                    Name = requestFilePart["Name"],
                    MimeType = requestFilePart["MimeType"],
                    Content = Files.UploadFileToDocument_File,
                    Id = idDocumentCreated
                },
                IdDocument = idDocumentCreated,
                Description = requestFilePart["Description"],
                HashFile = hash,
                CreateAttachment = false,
                AttachmentType = "N"

            };
            requestMsg = TestUtils.SerializeRequest(request, HttpMethod.Put, "UploadFileToDocument");
            var responseMsg = await this._apiHttpClient.SendAsync(requestMsg);
            requestMsg.Dispose();
            if (!responseMsg.IsSuccessStatusCode)
                Assert.Fail();
            else
            {
                string responseContent = await createDocumentResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<UploadFileToDocumentCommandResponse>(responseContent);
                if (responseDecoded == null)
                {
                    Assert.Fail(responseContent);
                }
            }
                
            Console.WriteLine(string.Format(Messages.UploadFileToDocumentSuccessfull,idDocumentCreated));
            Assert.Pass();
        }
        
        [Test]
        public async Task GetModifiedDocumentsTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetModifiedDocuments_Actual);

            var response = await this._apiHttpClient.GetAsync("GetModifiedDocuments"+ queryString);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<GetModifiedDocumentsCommandResponse>(responseContent);

                if (responseDecoded == null)
                {
                    Assert.Fail();
                }
                else if (!string.IsNullOrEmpty(responseDecoded.ErrorMessage) || !responseDecoded.Code.Equals(Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetModifiedDocuments.SearchDocumentsResponseCode.OK))
                {
                    Assert.Fail(responseDecoded.ErrorMessage);
                }
                else
                {
                    Assert.Pass();
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
        public async Task GetDocumentFiltersTest()
        {
            var response = await this._apiHttpClient.GetAsync("GetDocumentFilters");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetDocumentFiltersCommandResponse result = JsonConvert.DeserializeObject<GetDocumentFiltersCommandResponse>(responseContent);
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
                Console.WriteLine($"Errore: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }

        [Test]
        public async Task GetDocumentTemplatesTest()
        {   
            var response = await this._apiHttpClient.GetAsync("GetDocumentTemplates");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                GetDocumentTemplatesCommandResponse result = JsonConvert.DeserializeObject<GetDocumentTemplatesCommandResponse>(responseContent);
                if (result.Templates != null && result.Templates.Any())
                {
                    Console.WriteLine($"Template trovati");
                    foreach (var f in result.Templates)
                    {
                        Console.WriteLine($"Id: {f.Id}, Descrizione: {f.Name}");
                    }
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Template non trovati");
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
        public async Task GetDocumentEventsTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetDocumentEvents_Actual);

            var response = await this._apiHttpClient.GetAsync("GetDocumentEvents" + queryString);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetDocumentEventsCommandResponse>(responseContent);
                if (result.Events != null && result.Events.Any())
                {
                    Console.WriteLine($"Trovati {result.Events.Count} eventi.");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Eventi non trovati");
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
        public async Task GetDocAccessRightsTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetDocAccessRights_Actual);

            var response = await this._apiHttpClient.GetAsync("GetDocAccessRights" + queryString);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetDocAccessRightsCommandResponse>(responseContent);
                if (result.AccessRights != null && result.AccessRights.Any())
                {
                    Console.WriteLine($"Trovate {result.AccessRights.Count} visibilità sul documento.");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Visibilità non trovate");
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
        public async Task GetDocStateDiagramTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetDocStateDiagram_Actual);

            var response = await this._apiHttpClient.GetAsync("GetDocStateDiagram" + queryString);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetDocStateDiagramCommandResponse>(responseContent);
                if (result.StateOfDiagram != null && !string.IsNullOrWhiteSpace(result.StateOfDiagram.Description))
                {
                    Console.WriteLine($"Il documento è nello stato {result.StateOfDiagram.Description}");
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Stato del documento non trovato");
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
        public async Task GetDocumentTemplateTest()
        {
            var queryString = TestUtils.GetQueryStringFromEncodedJson(Files.GetDocumentTemplate_Actual);

            var response = await this._apiHttpClient.GetAsync("GetDocumentTemplate" + queryString);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetDocumentTemplateCommandResponse>(responseContent);
                if (result.Template != null && !string.IsNullOrWhiteSpace(result.Template.Id))
                {
                    Console.WriteLine($"Trovato il template Id {result.Template.Id}, descrizione: {result.Template.Name}");
                    if(result.Template.Fields!= null)
                    {
                        Console.WriteLine("Contenente i seguenti campi:");
                        foreach (var f in result.Template.Fields)
                        {
                            Console.WriteLine(f.Name);
                        }
                    }
                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Template non trovato");
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
        public async Task GetDocumentGrigioNoFileTest()
        {
            var getDocReq = JsonConvert.DeserializeObject<GetDocumentCommand>(Files.GetDocumentGrigioNoFile_Actual);

            var json = JsonConvert.SerializeObject(getDocReq);
            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "GetDocument")
            {
                Content = content2
            };
            var response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<GetDocumentCommandResponse>(responseContent);
                if (result.Document != null && !string.IsNullOrWhiteSpace(result.Document.Id))
                {
                    Console.WriteLine($"Prelevato dettaglio del documento con id \"{result.Document.Id}\" e oggetto \"{result.Document.Object}\"");

                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("Documento non trovato");
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
        public async Task SearchDocEventsTest()
        {
            var srcDocEvReq = new SearchDocEventsCommand()
            {
                FromDate = "17/04/2024",
                ToDate = "18/04/2024",
                Events = new[] { "OPEN_DET_DOC"}
            };

            var json = JsonConvert.SerializeObject(srcDocEvReq);
            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "SearchDocEvents")
            {
                Content = content2
            };
            var response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<SearchDocEventsCommandResponse>(responseContent);
                if (result.DocEvents != null && result.DocEvents.Any())
                {
                    Console.WriteLine($"Trovati {result.TotalEvents} eventi.");

                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("La ricerca non ha restituito risultati.");
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
        public async Task SearchDocumentsTest()
        {
            List<Filter> filters = new List<Filter>();
            filters.Add(new Filter() { Name = "NOT_PROTOCOL", Value = "TRUE" });
            filters.Add(new Filter() { Name = "CREATION_DATE_FROM", Value = "01/06/2024" });
            filters.Add(new Filter() { Name = "CREATION_DATE_TO", Value = "15/06/2024" });


            var srcDocsReq = new SearchDocumentsCommand()
            {
                Filters = filters.ToArray(),
                PageNumber = 2,
                ElementsInPage = 5
            };

            

            var json = JsonConvert.SerializeObject(srcDocsReq);
            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "SearchDocuments")
            {
                Content = content2
            };
            var response = await this._apiHttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}");
                string responseContent = await response.Content.ReadAsStringAsync();
                //Console.WriteLine($"Risposta JSON: {responseContent}");

                var result = JsonConvert.DeserializeObject<SearchDocumentsCommandResponse>(responseContent);
                if (result.Documents != null && result.Documents.Any())
                {
                    Console.WriteLine($"Restituiti {result.Documents.Length} documenti su {result.TotalDocumentsNumber} totali.");

                    Assert.Pass("Test terminato con successo.");
                }
                else
                {
                    Assert.Fail("La ricerca non ha restituito risultati.");
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
        public async Task GetFileWithSignatureOrStampTest()
        {
            var getFileWithSignatureOrStampRequest = JsonConvert.DeserializeObject<GetFileWithSignatureOrStampCommand>(Files.GetFileWithSignatureOrStamp_Actual);

            var queryParams = TestUtils.GetQueryStringFromEncodedJson(JsonConvert.SerializeObject(getFileWithSignatureOrStampRequest));
            var getFileWithSignatureOrStampResponse = await this._apiHttpClient.GetAsync("GetFileWithSignatureOrStamp" + queryParams);

            if (getFileWithSignatureOrStampResponse.IsSuccessStatusCode)
            {
                string responseContent = await getFileWithSignatureOrStampResponse.Content.ReadAsStringAsync();
                var responseDecoded = JsonConvert.DeserializeObject<GetFileWithSignatureOrStampCommandResponse>(responseContent);
                if (responseDecoded == null || !string.IsNullOrEmpty(responseDecoded.ErrorMessage))
                    Assert.Fail();

                Assert.Pass("Test terminato con successo.");

            }
            else
            {
                Console.WriteLine($"Errore: {getFileWithSignatureOrStampResponse.StatusCode}");
                string responseContent = await getFileWithSignatureOrStampResponse.Content.ReadAsStringAsync();
                Assert.Fail(responseContent);
            }
        }
    }
}
