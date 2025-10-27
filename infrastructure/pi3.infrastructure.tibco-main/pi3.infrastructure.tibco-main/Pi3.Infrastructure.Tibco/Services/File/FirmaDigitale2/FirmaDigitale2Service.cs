// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Tibco.Services.File.FirmaDigitale2.Exceptions;
using Pi3.Infrastructure.Tibco.Services.File.FirmaDigitale2.Resources;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
using Pi3.Core.Services.File.FirmaDigitale2;

namespace Pi3.Infrastructure.Tibco.Services.File.FirmaDigitale2
{
    public class FirmaDigitale2Service : IFirmaDigitale2Service
    {
        #region Public Members

        public FirmaDigitale2Service(ILogger<FirmaDigitale2Service> logger, IOptions<FirmaDigitale2ServiceOptions> options)
        {
            this._logger = logger;
            this._options = options;
        }

        public async Task<VerificaResponse> Verifica(VerificaRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            Validator.ValidateObject(request, new ValidationContext(request), true);

            try
            {
                VerificaResponse? response = default;

                string apiUrl = this._options.Value.ServiceUrl;

                var options = new JsonSerializerOptions();
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.PropertyNameCaseInsensitive = true;

                string asJson = null!;

                if (request.VerificaCompleta ?? false)
                {
                    apiUrl += "/VerifyCompleta";

                    asJson = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        FileBase64 = request.FileFirmato,
                        VerificationTime = this.FormatDataVerifica(request.DataVerifica),
                        EncapsulatedSigs = request.TipoVerifica.HasValue ? (int)request.TipoVerifica.Value : (int)TipiVerifica.Esterna,
                        ExtractOriginalFile = request.ReturnFileOriginale.HasValue ? request.ReturnFileOriginale.Value : false,
                        ExtractCompleteXML = request.ReturnXmlCompleto.HasValue ? request.ReturnXmlCompleto.Value : false
                    },
                    options);
                }
                else
                {
                    apiUrl += "/Verify";

                    asJson = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        FileBase64 = request.FileFirmato,
                        FileOriginalBase64 = request.FileOriginale,
                        VerificationTime = this.FormatDataVerifica(request.DataVerifica),
                        EncapsulatedSigs = request.TipoVerifica.HasValue ? (int)request.TipoVerifica.Value : (int)TipiVerifica.Esterna,
                        ExtractOriginalFile = request.ReturnFileOriginale.HasValue ? request.ReturnFileOriginale.Value : false,
                        ExtractCompleteXML = request.ReturnXmlCompleto.HasValue ? request.ReturnXmlCompleto.Value : false
                    },
                    options);
                }

                var clientHandler = new HttpClientHandler();

                clientHandler.ClientCertificates.Add(this._options.Value.Certificate);

                using (var client = new HttpClient(clientHandler))
                {
                    var content = new StringContent(asJson, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Add("ContentType", "application/json");

                    var serviceResponse = await client.PostAsync(apiUrl, content);

                    this._logger.LogDebug($"Richiesta verifica firma completata. Esito: {serviceResponse.IsSuccessStatusCode} - Reason: {serviceResponse.ReasonPhrase}");

                    string responseBody = await serviceResponse.Content.ReadAsStringAsync();

                    response = System.Text.Json.JsonSerializer.Deserialize<VerificaResponse>(responseBody, options);

                    if (!serviceResponse.IsSuccessStatusCode && response != null)
                        response.Warning = System.Text.Json.JsonSerializer.Deserialize<EsitoWarningVerificaFirma>(responseBody, options);
                }

                return response;
            }
            catch (Pi3Exception)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FirmaDigitalePi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex);
            }
        }

        //public async Task<VerificaResponse> VerificaREST(VerificaRequestREST request)
        //{
        //    request = request ?? throw new ArgumentNullException(nameof(request));

        //    Validator.ValidateObject(request, new ValidationContext(request), true);
           
        //    try
        //    {
        //        VerificaResponse? response = default;

        //        string apiUrl = this._options.Value.ServiceUrl + "/Verify";

        //        var options = new JsonSerializerOptions();
        //        options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        //        options.PropertyNameCaseInsensitive = true;

        //        var asJson = System.Text.Json.JsonSerializer.Serialize<VerificaRequestREST>(request, options);   

        //        var clientHandler = new HttpClientHandler();

        //        clientHandler.ClientCertificates.Add(this._options.Value.Certificate);
        //        using (var client = new HttpClient(clientHandler))
        //        {
        //            var content = new StringContent(asJson, Encoding.UTF8, "application/json");
               
        //            client.DefaultRequestHeaders.Add("ContentType", "application/json");

        //            var serviceResponse = await client.PostAsync(apiUrl, content);

        //            this._logger.LogDebug($"Richiesta verifica firma (Verify) completata. Esito: {serviceResponse.IsSuccessStatusCode} - Reason: {serviceResponse.ReasonPhrase}");

        //            if (!serviceResponse.IsSuccessStatusCode)
        //                throw new FirmaDigitaleRESTPi3Exception(
        //                    ErrorDescriptions.ServiceError,
        //                    ErrorDescriptions.ResourceManager,
        //                    serviceResponse.ReasonPhrase ?? "");

        //            string responseBody = await serviceResponse.Content.ReadAsStringAsync();

        //            responseBody = Regex.Unescape(responseBody).Replace("\"{", "{").Replace("}\"", "}");

        //            response = System.Text.Json.JsonSerializer.Deserialize<VerificaResponse>(responseBody, options);
        //        }
        //        return response;
        //    }
        //    catch (Pi3Exception)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new FirmaDigitaleRESTPi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex);
        //    }
        //}

        //public async Task<VerificaResponse> VerificaP7MREST(VerificaRequestP7MREST request)
        //{
        //    request = request ?? throw new ArgumentNullException(nameof(request));

        //    Validator.ValidateObject(request, new ValidationContext(request), true);

        //    try
        //    {
        //        VerificaResponse? response = default;

        //        string apiUrl = this._options.Value.ServiceUrl + "/VerifyCompleta";

          
        //        var options = new JsonSerializerOptions();
        //        options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        //        options.PropertyNameCaseInsensitive = true;

        //        var asJson = System.Text.Json.JsonSerializer.Serialize<VerificaRequestP7MREST>(request, options);


        //        var clientHandler = new HttpClientHandler();

        //        clientHandler.ClientCertificates.Add(this._options.Value.Certificate);
        //        using (var client = new HttpClient(clientHandler))
        //        {
        //            var content = new StringContent(asJson, Encoding.UTF8, "application/json");

        //            client.DefaultRequestHeaders.Add("ContentType", "application/json");

        //            var serviceResponse = await client.PostAsync(apiUrl, content);

        //            this._logger.LogDebug($"Richiesta verifica firma completa (VerifyCompleta) completata. Esito: {serviceResponse.IsSuccessStatusCode} - Reason: {serviceResponse.ReasonPhrase}");

        //            if (!serviceResponse.IsSuccessStatusCode)
        //                throw new FirmaDigitaleRESTPi3Exception(
        //                    ErrorDescriptions.ServiceError,
        //                    ErrorDescriptions.ResourceManager,
        //                    serviceResponse.ReasonPhrase ?? "");

        //            string responseBody = await serviceResponse.Content.ReadAsStringAsync();

        //            responseBody = Regex.Unescape(responseBody).Replace("\"{", "{").Replace("}\"", "}");

        //            response = System.Text.Json.JsonSerializer.Deserialize<VerificaResponse>(responseBody, options);
        //        }
        //        return response;
        //    }
        //    catch (Pi3Exception)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new FirmaDigitaleRESTPi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex);
        //    }
        //}

        #endregion

        #region Private Members

        protected readonly ILogger<FirmaDigitale2Service> _logger;
        protected readonly IOptions<FirmaDigitale2ServiceOptions> _options;

        public string FormatDataVerifica(DateTime? dataVerifica = null)
        {
            var data = DateTime.Now;

            if (dataVerifica.HasValue)
                data = dataVerifica.Value;

            return String.Format("{0}-{1}-{2}",
                        data.Year.ToString(),
                        data.Month.ToString().PadLeft(2, '0'),
                        data.Day.ToString().PadLeft(2, '0'));
        }

        #endregion
    }
}
