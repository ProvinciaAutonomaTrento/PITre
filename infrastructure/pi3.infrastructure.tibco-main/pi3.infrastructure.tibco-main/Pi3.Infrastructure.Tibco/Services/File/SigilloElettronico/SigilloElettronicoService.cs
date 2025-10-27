// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.SigilloElettronico;
using Pi3.Infrastructure.Tibco.Services.File.SigilloElettronico.Exceptions;
using Pi3.Infrastructure.Tibco.Services.File.SigilloElettronico.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Pi3.Infrastructure.Tibco.Services.File.SigilloElettronico
{
    public class SigilloElettronicoService : ISigilloElettronicoService
    {
        #region Public Members

        public SigilloElettronicoService(ILogger<SigilloElettronicoService> logger, 
            IOptions<SigilloElettronicoServiceOptions> options)
        {
            this._logger = logger;
            this._options = options;
        }

        public async Task<SignResponsePDFType> SignPdf(SignPDFType signPDFType)
        {
            return await this.InternalSign<SignPDFType, SignResponsePDFType>("/pdfsignature", signPDFType);
        }

        public async Task<SignResponseXMLType> SignXml(SignXMLType signXMLType)
        {
            return await this.InternalSign<SignXMLType, SignResponseXMLType>("/xmlsignature", signXMLType);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SigilloElettronicoService> _logger;
        protected readonly IOptions<SigilloElettronicoServiceOptions> _options;

        protected virtual async Task<TResponse> InternalSign<TRequest, TResponse>(string serviceEndpoint, TRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            Validator.ValidateObject(request, new ValidationContext(request), true);
            
            try
            {
                string apiUrl = this._options.Value.ServiceUrl + serviceEndpoint;

                TResponse? response = default;

                var options = new JsonSerializerOptions();
                options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.PropertyNameCaseInsensitive = true;

                var asJson = System.Text.Json.JsonSerializer.Serialize<TRequest>(request, options);

                this._logger.LogDebug("request to RestService pdfSign -> " + asJson);

                var clientHandler = new HttpClientHandler();

                clientHandler.ClientCertificates.Add(this._options.Value.Certificate);

                using (var client = new HttpClient(clientHandler))
                {
                    var content = new StringContent(asJson, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Add("ContentType", "application/json");

                    client.DefaultRequestHeaders.Add("Authorization",
                        "Basic " + System.Convert.ToBase64String(
                            System.Text.Encoding.UTF8.GetBytes(
                                this._options.Value.UserId + ":" + this._options.Value.UserPassword)));

                    this._logger.LogDebug("Richiesta apposizione sigillo elettronico in corso...");

                    var serviceResponse = await client.PostAsync(apiUrl, content);
                    while(!serviceResponse.IsSuccessStatusCode && this._options.Value.NumberOfRetries > 0)
                    {
                        this._logger.LogDebug($"Richiesta apposizione sigillo elettronico in corso...Tentativo numero: {this._options.Value.NumberOfRetries}");
                        this._options.Value.NumberOfRetries--;
                        Thread.Sleep(this._options.Value.RetryDelayMilliseconds);
                        serviceResponse = await client.PostAsync(apiUrl, content);
                    }

                    this._logger.LogDebug($"Richiesta apposizione sigillo elettronico completata. Esito: {serviceResponse.IsSuccessStatusCode} - Reason: {serviceResponse.ReasonPhrase}");

                    if (!serviceResponse.IsSuccessStatusCode)
                        throw new SigilloElettronicoPi3Exception(
                            ErrorDescriptions.ServiceError,
                            ErrorDescriptions.ResourceManager,
                            serviceResponse.ReasonPhrase ?? "");

                    string responseBody = await serviceResponse.Content.ReadAsStringAsync();

                    response = System.Text.Json.JsonSerializer.Deserialize<TResponse>(responseBody, options);
                }

                return response;
            }
            catch (Pi3Exception pi3Ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SigilloElettronicoPi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex);
            }
        }

        #endregion
    }
}
