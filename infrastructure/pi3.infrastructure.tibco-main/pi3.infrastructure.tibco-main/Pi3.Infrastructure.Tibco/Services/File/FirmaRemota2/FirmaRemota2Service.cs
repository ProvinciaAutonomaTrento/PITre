// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.Description;
using Pi3.Core.SeedWork;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Xml;
using System.ServiceModel.Dispatcher;
using System.Text.Json;
using Pi3.Infrastructure.Tibco.Services.File.SigilloElettronico.Exceptions;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net.Mime;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static System.Net.WebRequestMethods;
using System.IO;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2.Exceptions;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2.Resources;
using Pi3.Core.Services.File.FirmaRemota2;

namespace Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2
{
    public class FirmaRemota2Service : IFirmaRemota2Service
    {
        #region Public Members

        public FirmaRemota2Service(ILogger<FirmaRemota2Service> logger, IOptions<FirmaRemota2ServiceOptions> options)
        {
            this._logger = logger;
            this._options = options;
        }

        public async Task<RichiestaOtpResponse> RichiestaOtpREST(RichiestaOtpRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            Validator.ValidateObject(request, new ValidationContext(request), true);

            try
            {
                RichiestaOtpResponse? response = default;
                string apiUrl = this._options.Value.ServiceUrl + "/richiestaotp";
                var options = new JsonSerializerOptions();
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.PropertyNameCaseInsensitive = true;

                var asJson = System.Text.Json.JsonSerializer.Serialize(
                    new
                    {
                        aliasCertificato = request.AliasCertificato,
                        dominioCertificato = request.DominioCertificato,
                        userPassword = request.UserPassword
                    }, options);

                var clientHandler = new HttpClientHandler();

                clientHandler.ClientCertificates.Add(this._options.Value.Certificate);
                using (var client = new HttpClient(clientHandler))
                {
                    var content = new StringContent(asJson, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Add("ContentType", "application/json");

                    this._logger.LogDebug("Richiesta OTP firma remota in corso...");

                    var serviceResponse = await client.PostAsync(apiUrl, content);

                    this._logger.LogDebug($"Richiesta OTP firma remota completata. Esito: {serviceResponse.IsSuccessStatusCode} - Reason: {serviceResponse.ReasonPhrase}");

                    if (!serviceResponse.IsSuccessStatusCode)
                        throw new FirmaRemota.Exceptions.FirmaRemotaPi3Exception(
                            ErrorDescriptions.ServiceError,
                            ErrorDescriptions.ResourceManager,
                            serviceResponse.ReasonPhrase ?? "");

                    string responseBody = await serviceResponse.Content.ReadAsStringAsync();

                    responseBody = Regex.Unescape(responseBody).Replace("\"{", "{").Replace("}\"", "}");

                    response = System.Text.Json.JsonSerializer.Deserialize<RichiestaOtpResponse>(responseBody, options);
                }

                return response;
            }
            catch (Pi3Exception)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FirmaRemota.Exceptions.FirmaRemotaPi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex);
            }
        }

        public async Task<FirmaPAdESResponse> FirmaPAdESREST(FirmaPAdESRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            Validator.ValidateObject(request, new ValidationContext(request), true);

            try
            {
                FirmaPAdESResponse? response = default;
                string apiUrl = this._options.Value.ServiceUrl + "/firmaremotapdf";
                var options = new JsonSerializerOptions();
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.PropertyNameCaseInsensitive = true;

                var asJson = System.Text.Json.JsonSerializer.Serialize<FirmaPAdESRequest>(request, options);

                var clientHandler = new HttpClientHandler();

                clientHandler.ClientCertificates.Add(this._options.Value.Certificate);

                using (var client = new HttpClient(clientHandler))
                {
                    var multiPartContent = new MultipartFormDataContent();

                    request.FilesDaFirmare.ForEach(f =>
                    {
                        var imageContent = new ByteArrayContent(f.FileBase64);
                        imageContent.Headers.Add("Content-Disposition", "form-data; name=\"fileDaFirmare\"; filename=\"" + f.FileName + "\"");
                        multiPartContent.Add(imageContent, "file");
                    });

                    var alias = new StringContent(request.AliasCertificato);
                    alias.Headers.Add("Content-Disposition", "form-data; name=\"aliasCertificato\"");
                    multiPartContent.Add(alias, "aliasCertificato");

                    var pin = new StringContent(request.PinCertificato);
                    pin.Headers.Add("Content-Disposition", "form-data; name=\"pinCertificato\"");
                    multiPartContent.Add(pin, "pinCertificato");

                    var otp = new StringContent(request.OtpFirma);
                    otp.Headers.Add("Content-Disposition", "form-data; name=\"otpFirma\"");
                    multiPartContent.Add(otp, "otpFirma");

                    if (!String.IsNullOrEmpty(request.DominioCertificato))
                    {
                        var dominio = new StringContent(request.DominioCertificato);
                        dominio.Headers.Add("Content-Disposition", "form-data; name=\"dominioCertificato\"");
                        multiPartContent.Add(dominio, "dominioCertificato");
                    }

                    if (request.MarcaTemporale.HasValue)
                    {
                        var marca = new StringContent(request.MarcaTemporale.Value ? "1" : "0");
                        marca.Headers.Add("Content-Disposition", "form-data; name=\"marcaTemporale\"");
                        multiPartContent.Add(marca, "marcaTemporale");
                    }

                    if (request.unzipOutput.HasValue)
                    {
                        var unzipOutput = new StringContent(request.unzipOutput.Value ? "1" : "0");
                        unzipOutput.Headers.Add("Content-Disposition", "form-data; name=\"unzipOutput\"");
                        multiPartContent.Add(unzipOutput, "unzipOutput");
                    }

                    client.DefaultRequestHeaders.Add("ContentType", "multipart/form-data");

                    this._logger.LogDebug("Richiesta firma PADES in corso...");

                    var serviceResponse = await client.PostAsync(apiUrl, multiPartContent);

                    this._logger.LogDebug($"Richiesta firma PADES completata. Esito: {serviceResponse.IsSuccessStatusCode} - Reason: {serviceResponse.ReasonPhrase}");

                    string responseBody = await serviceResponse.Content.ReadAsStringAsync();

                    if (!serviceResponse.IsSuccessStatusCode)
                        throw new FirmaRemota.Exceptions.FirmaRemotaPi3Exception(
                            ErrorDescriptions.ServiceError,
                            ErrorDescriptions.ResourceManager,
                            $"{serviceResponse.ReasonPhrase} - {responseBody}");

                    response = System.Text.Json.JsonSerializer.Deserialize<FirmaPAdESResponse>(responseBody, options);

                    if (request.FilesDaFirmare.Count == 1 && response?.FileFirmato.Count == 1)
                        response.FileFirmato[0].FileName = request.FilesDaFirmare[0].FileName;
                }

                return response;
            }
            catch (Pi3Exception)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FirmaRemota.Exceptions.FirmaRemotaPi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex);
            }
        }

        public async Task<FirmaCAdESResponse> FirmaCAdESREST(FirmaCAdESRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            Validator.ValidateObject(request, new ValidationContext(request), true);

            try
            {
                FirmaCAdESResponse? response = default;

                string apiUrl = this._options.Value.ServiceUrl + "/firmaremotap7m";
                var options = new JsonSerializerOptions();
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.PropertyNameCaseInsensitive = true;

                var clientHandler = new HttpClientHandler();

                clientHandler.ClientCertificates.Add(this._options.Value.Certificate);

                using (var client = new HttpClient(clientHandler))
                {
                    var multiPartContent = new MultipartFormDataContent();

                    request.FilesDaFirmare.ForEach(f =>
                    {
                        var imageContent = new ByteArrayContent(f.FileBase64);
                        imageContent.Headers.Add("Content-Disposition", "form-data; name=\"fileDaFirmare\"; filename=\"" + f.FileName + "\"");
                        multiPartContent.Add(imageContent, "file");
                    });

                    var alias = new StringContent(request.AliasCertificato);
                    alias.Headers.Add("Content-Disposition", "form-data; name=\"aliasCertificato\"");
                    multiPartContent.Add(alias, "aliasCertificato");

                    var pin = new StringContent(request.PinCertificato);
                    pin.Headers.Add("Content-Disposition", "form-data; name=\"pinCertificato\"");
                    multiPartContent.Add(pin, "pinCertificato");

                    var otp = new StringContent(request.OtpFirma);
                    otp.Headers.Add("Content-Disposition", "form-data; name=\"otpFirma\"");
                    multiPartContent.Add(otp, "otpFirma");

                    if (!String.IsNullOrEmpty(request.DominioCertificato))
                    {
                        var dominio = new StringContent(request.DominioCertificato);
                        dominio.Headers.Add("Content-Disposition", "form-data; name=\"dominioCertificato\"");
                        multiPartContent.Add(dominio, "dominioCertificato");
                    }
                    if ((request.MarcaTemporale.HasValue))
                    {
                        var marca = new StringContent(request.MarcaTemporale.Value ? "1" : "0");
                        marca.Headers.Add("Content-Disposition", "form-data; name=\"marcaTemporale\"");
                        multiPartContent.Add(marca, "marcaTemporale");
                    }

                    if (request.unzipOutput.HasValue)
                    {
                        var unzipOutput = new StringContent(request.unzipOutput.Value ? "1" : "0");
                        unzipOutput.Headers.Add("Content-Disposition", "form-data; name=\"unzipOutput\"");
                        multiPartContent.Add(unzipOutput, "unzipOutput");
                    }
                    if (request.FirmaParallela.HasValue)
                    {
                        var FirmaParallela = new StringContent(request.FirmaParallela.Value ? "1" : "0");
                        FirmaParallela.Headers.Add("Content-Disposition", "form-data; name=\"firmaParallela\"");
                        multiPartContent.Add(FirmaParallela, "firmaParallela");
                    }

                    client.DefaultRequestHeaders.Add("ContentType", "multipart/form-data");

                    this._logger.LogDebug("Richiesta firma CADES in corso...");

                    var serviceResponse = await client.PostAsync(apiUrl, multiPartContent);

                    this._logger.LogDebug($"Richiesta firma CADES completata. Esito: {serviceResponse.IsSuccessStatusCode} - Reason: {serviceResponse.ReasonPhrase}");

                    string responseBody = await serviceResponse.Content.ReadAsStringAsync();

                    if (!serviceResponse.IsSuccessStatusCode)
                        throw new FirmaRemota.Exceptions.FirmaRemotaPi3Exception(
                            ErrorDescriptions.ServiceError,
                            ErrorDescriptions.ResourceManager,
                            $"{serviceResponse.ReasonPhrase} - {responseBody}");

                    response = System.Text.Json.JsonSerializer.Deserialize<FirmaCAdESResponse>(responseBody, options);

                    if (request.FilesDaFirmare.Count == 1 && response?.FileFirmato.Count == 1)
                        response.FileFirmato[0].FileName = request.FilesDaFirmare[0].FileName;
                }

                return response;
            }
            catch (Pi3Exception)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FirmaRemota.Exceptions.FirmaRemotaPi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex);
            }
        }

        public async Task<VisualizzaCertificatoResponse> VisualizzaCertificatoREST(VisualizzaCertificatoRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            Validator.ValidateObject(request, new ValidationContext(request), true);

            try
            {
                VisualizzaCertificatoResponse? response = default;
                string apiUrl = this._options.Value.ServiceUrl + "/visualizzacertificato";
                var options = new JsonSerializerOptions();
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.PropertyNameCaseInsensitive = true;

                var asJson = System.Text.Json.JsonSerializer.Serialize(
                    new
                    {
                        aliasCertificato = request.AliasCertificato,
                        dominioCertificato = request.DominioCertificato,
                        userPwd = request.UserPwd
                    }, options);

                var clientHandler = new HttpClientHandler();

                clientHandler.ClientCertificates.Add(this._options.Value.Certificate);
                using (var client = new HttpClient(clientHandler))
                {
                    var content = new StringContent(asJson, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("ContentType", "application/json");

                    this._logger.LogDebug("Richiesta visualizza certificato in corso...");

                    var serviceResponse = await client.PostAsync(apiUrl, content);

                    if (!serviceResponse.IsSuccessStatusCode)
                        throw new FirmaRemota.Exceptions.FirmaRemotaPi3Exception(
                            ErrorDescriptions.ServiceError,
                            ErrorDescriptions.ResourceManager,
                            serviceResponse.ReasonPhrase ?? "");

                    string responseBody = await serviceResponse.Content.ReadAsStringAsync();


                    response = JsonSerializer.Deserialize<VisualizzaCertificatoResponse>(responseBody, options);
                }

                return response;
            }
            catch (Pi3Exception)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FirmaRemota.Exceptions.FirmaRemotaPi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex);
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FirmaRemota2Service> _logger;
        protected readonly IOptions<FirmaRemota2ServiceOptions> _options;

        #endregion
    }
}
