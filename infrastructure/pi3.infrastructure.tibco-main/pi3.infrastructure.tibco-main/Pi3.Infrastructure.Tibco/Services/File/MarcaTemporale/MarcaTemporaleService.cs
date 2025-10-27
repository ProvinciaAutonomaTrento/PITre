// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.MarcaTemporale;
using Pi3.Infrastructure.Tibco.Services.File.MarcaTemporale.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Tibco.Services.File.MarcaTemporale
{
    public class MarcaTemporaleService : IMarcaTemporaleService
    {
        #region Public Members

        public MarcaTemporaleService(ILogger<MarcaTemporaleService> logger,
            IOptions<MarcaTemporaleServiceOptions> options)
        {
            this._logger = logger;
            this._options = options;
        }

        public virtual async Task<MarcaTsdResponse> MarcaTsd(MarcaTsdRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            Validator.ValidateObject(request, new ValidationContext(request), true);

            try
            {
                MarcaTsdResponse? response = default;
                string apiUrl = this._options.Value.ServiceUrl + "/marcatsd";
                var options = new JsonSerializerOptions();
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.PropertyNameCaseInsensitive = true;

                var clientHandler = new HttpClientHandler();

                clientHandler.ClientCertificates.Add(this._options.Value.Certificate);

                using (var client = new HttpClient(clientHandler))
                {
                    var multiPartContent = new MultipartFormDataContent();

                    var fileContent = new ByteArrayContent(request.FileDaMarcare.FileBase64);
                    fileContent.Headers.Add("Content-Disposition", "form-data; name=\"fileDaMarcare\"; filename=\"" + request.FileDaMarcare.FileName + "\"");
                    multiPartContent.Add(fileContent, "file");

                    client.DefaultRequestHeaders.Add("ContentType", "multipart/form-data");

                    this._logger.LogDebug("Richiesta Marca Tsd in corso...");

                    var serviceResponse = await client.PostAsync(apiUrl, multiPartContent);

                    this._logger.LogDebug($"Richiesta Marca Tsd completata. Esito: {serviceResponse.IsSuccessStatusCode} - Reason: {serviceResponse.ReasonPhrase}");

                    string responseBody = await serviceResponse.Content.ReadAsStringAsync();

                    if (!serviceResponse.IsSuccessStatusCode)
                        throw new MarcaturaPi3Exception(
                            ErrorDescriptions.ServiceError,
                            ErrorDescriptions.ResourceManager,
                            $"{serviceResponse.ReasonPhrase} - {responseBody}");

                    var output = System.Text.Json.JsonSerializer.Deserialize<MarcaTsdJsonResponse>(responseBody, options);

                    response = new MarcaTsdResponse()
                    {
                        DataOraMarca = output.DataOraMarca,
                        SerialNumberMarca = output.serialNumberMarca,
                        TSAName = output.TSAName,
                        FileMarcato = new FileMarcatura()
                        {
                            FileBase64 = Encoding.UTF8.GetBytes(output.FileBase64),
                            FileName = output.FileName
                        }
                    };
                }

                return response;
            }
            catch (Pi3Exception)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new MarcaturaPi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex);
            }
        }

        public virtual async Task<MarcaTsrResponse> MarcaTsr(MarcaTsrRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            Validator.ValidateObject(request, new ValidationContext(request), true);

            try
            {
                MarcaTsrResponse? response = default;
                string apiUrl = this._options.Value.ServiceUrl + "/marcatsr";
                var options = new JsonSerializerOptions();
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.PropertyNameCaseInsensitive = true;

                var clientHandler = new HttpClientHandler();

                clientHandler.ClientCertificates.Add(this._options.Value.Certificate);

                using (var client = new HttpClient(clientHandler))
                {
                    var multiPartContent = new MultipartFormDataContent();

                    var fileContent = new ByteArrayContent(request.FileDaMarcare.FileBase64);
                    fileContent.Headers.Add("Content-Disposition", "form-data; name=\"fileDaMarcare\"; filename=\"" + request.FileDaMarcare.FileName + "\"");
                    multiPartContent.Add(fileContent, "file");

                    client.DefaultRequestHeaders.Add("ContentType", "multipart/form-data");

                    this._logger.LogDebug("Richiesta Marca Tsr in corso...");

                    var serviceResponse = await client.PostAsync(apiUrl, multiPartContent);

                    this._logger.LogDebug($"Richiesta Marca Tsr completata. Esito: {serviceResponse.IsSuccessStatusCode} - Reason: {serviceResponse.ReasonPhrase}");

                    string responseBody = await serviceResponse.Content.ReadAsStringAsync();

                    if (!serviceResponse.IsSuccessStatusCode)
                        throw new MarcaturaPi3Exception(
                            ErrorDescriptions.ServiceError,
                            ErrorDescriptions.ResourceManager,
                            $"{serviceResponse.ReasonPhrase} - {responseBody}");

                    response = System.Text.Json.JsonSerializer.Deserialize<MarcaTsrResponse>(responseBody, options);
                }

                return response;
            }
            catch (Pi3Exception)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new MarcaturaPi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex);
            }
        }

        public virtual async Task<MarcaTsrHashResponse> MarcaTsrHash(MarcaTsrHashRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            Validator.ValidateObject(request, new ValidationContext(request), true);

            try
            {
                MarcaTsrHashResponse? response = default;
                string apiUrl = this._options.Value.ServiceUrl + "/marcatsrhash";
                var options = new JsonSerializerOptions();
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.PropertyNameCaseInsensitive = true;

                var clientHandler = new HttpClientHandler();

                clientHandler.ClientCertificates.Add(this._options.Value.Certificate);

                using (var client = new HttpClient(clientHandler))
                {
                    var requestAsJson = new MarcaTsrHashJsonRequest(
                        BitConverter.ToString(request.Hash).Replace("-", ""),
                        request.HashAlgorithm.HasValue ?
                            (request.HashAlgorithm.Value == HashAlgorithms.SHA256 ?
                                "sha-256" :
                                "sha-512")
                            : null);

                    var asJson = System.Text.Json.JsonSerializer.Serialize(requestAsJson, options);

                    var content = new StringContent(asJson, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Add("ContentType", "application/json");

                    this._logger.LogDebug("Richiesta Marca TsrHash in corso...");

                    var serviceResponse = await client.PostAsync(apiUrl, content);

                    this._logger.LogDebug($"Richiesta Marca TsrHash completata. Esito: {serviceResponse.IsSuccessStatusCode} - Reason: {serviceResponse.ReasonPhrase}");

                    string responseBody = await serviceResponse.Content.ReadAsStringAsync();

                    if (!serviceResponse.IsSuccessStatusCode)
                        throw new MarcaturaPi3Exception(
                            ErrorDescriptions.ServiceError,
                            ErrorDescriptions.ResourceManager,
                            $"{serviceResponse.ReasonPhrase} - {responseBody}");

                    response = System.Text.Json.JsonSerializer.Deserialize<MarcaTsrHashResponse>(responseBody, options);
                }

                return response;
            }
            catch (Pi3Exception)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new MarcaturaPi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex);
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<MarcaTemporaleService> _logger;
        protected readonly IOptions<MarcaTemporaleServiceOptions> _options;
                
        protected record MarcaTsdJsonResponse(
            DateTime DataOraMarca, string serialNumberMarca, string TSAName, string FileBase64, string FileName);

        protected record MarcaTsrHashJsonRequest(
            string hash, string? hashingAlgorithm = null);

        #endregion

    }
}
