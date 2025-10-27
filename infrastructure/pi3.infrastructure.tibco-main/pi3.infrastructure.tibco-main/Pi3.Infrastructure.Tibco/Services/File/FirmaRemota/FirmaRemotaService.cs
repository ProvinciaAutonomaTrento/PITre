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
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota.Exceptions;
//using static Pi3.Infrastructure.Tibco.Services.File.FirmaDigitale.FirmaDigitaleService;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Xml;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota.Proxy;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota.Resources;
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
using Pi3.Core.Services.File.FirmaRemota;

namespace Pi3.Infrastructure.Tibco.Services.File.FirmaRemota
{
    public class FirmaRemotaService : IFirmaRemotaService
    {
        #region Public Members

        public FirmaRemotaService(ILogger<FirmaRemotaService> logger, IOptions<FirmaRemotaServiceOptions> options)
        {
            this._logger = logger;
            this._options = options;

            this.InitializeMapper();
        }

        public async Task<RichiestaOtpResponse> RichiestaOtp(RichiestaOtpRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            Validator.ValidateObject(request, new ValidationContext(request), true);            

            try
            {
                using (var client = this.CreateClient())
                {
                    try
                    {                       
                        var serviceResponse = await client.RichiestaOTPAsync(request.AliasCertificato, request.DominioCertificato);

                        return this._mapper.Map<RichiestaOtpResponse>(serviceResponse);
                    }
                    catch (FaultException fe)
                    {
                        MessageFault msgFault = fe.CreateMessageFault();
                        XmlElement elm = msgFault.GetDetail<XmlElement>();
                        var exceptionDetails = elm.InnerText;

                        throw new FirmaRemotaPi3Exception(ErrorDescriptions.ServiceError, ErrorDescriptions.ResourceManager, fe, fe.Message);
                    }
                    catch (Exception ex)
                    {
                        throw new FirmaRemotaPi3Exception(ErrorDescriptions.ServiceError, ErrorDescriptions.ResourceManager, ex, ex.Message);
                    }
                }
            }
            catch (Pi3Exception)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw  new FirmaRemotaPi3Exception(ErrorDescriptions.UnhandledError, ErrorDescriptions.ResourceManager, ex);
            }
        }

        public async Task<FirmaPAdESResponse> FirmaPAdES(FirmaPAdESRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            Validator.ValidateObject(request, new ValidationContext(request), true);

            using (var client = this.CreateClient())
            {
                try
                {
                    var serviceResponse = await client.FirmaRemotaPDFAsync(
                        request.fileDaFirmare, 
                        request.AliasCertificato, 
                        request.DominioCertificato, 
                        request.PinCertificato, 
                        request.OtpFirma, 
                        (request.MarcaTemporale.HasValue && request.MarcaTemporale.Value) 
                            ? (sbyte)1 : (sbyte)0);

                    return this._mapper.Map<FirmaPAdESResponse>(serviceResponse);

                }
                catch (Exception ex)
                {
                    throw new FirmaRemotaPi3Exception(ErrorDescriptions.ServiceError, ErrorDescriptions.ResourceManager, ex, ex.Message);
                }
            }
        }

        public async Task<FirmaCAdESResponse> FirmaCAdES(FirmaCAdESRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            Validator.ValidateObject(request, new ValidationContext(request), true);

            using (var client = this.CreateClient())
            {
                try
                {
                    var serviceResponse = await client.FirmaRemotaP7MAsync(
                        request.fileDaFirmare, 
                        request.AliasCertificato, 
                        request.DominioCertificato, 
                        request.PinCertificato, 
                        request.OtpFirma, 
                        (request.MarcaTemporale.HasValue && request.MarcaTemporale.Value) 
                        ? (sbyte)1 : (sbyte)0, 
                        (request.FirmaParallela.HasValue && request.FirmaParallela.Value) 
                        ? (sbyte)1 : (sbyte)0);

                    return this._mapper.Map<FirmaCAdESResponse>(serviceResponse);

                }
                catch (Exception ex)
                {
                    throw new FirmaRemotaPi3Exception(ErrorDescriptions.ServiceError, ErrorDescriptions.ResourceManager, ex, ex.Message);
                }
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FirmaRemotaService> _logger;
        protected readonly IOptions<FirmaRemotaServiceOptions> _options;

        protected IMapper _mapper = null!;

        protected void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Proxy.RichiestaOTPResponse, RichiestaOtpResponse>();
                cfg.CreateMap<Proxy.FirmaRemotaPDFResponse, FirmaPAdESResponse>();
                cfg.CreateMap<Proxy.FirmaRemotaP7MResponse, FirmaCAdESResponse>();
                cfg.CreateMap<Proxy.fileFirmato, FileFirmato>();
            });

            this._mapper = configuration.CreateMapper();
        }


        protected virtual FirmaRemotaPortTypeClient CreateClient()
        {
            var binding = new CustomBinding();
            binding.Name = "FirmaRemotaPortTypeEndpoint2BindingMIO";

            XmlDictionaryReaderQuotas readquota = new XmlDictionaryReaderQuotas
            {
                MaxStringContentLength = 100000000,
                MaxArrayLength = 100000000,
                MaxBytesPerRead = 524288
            };

            MtomMessageEncodingBindingElement mtomEconding = new MtomMessageEncodingBindingElement
            {
                MaxReadPoolSize = 640,
                MaxWritePoolSize = 160,
                MessageVersion = MessageVersion.Soap12,
                MaxBufferSize = 2147483647,
            };

            mtomEconding.ReaderQuotas.MaxArrayLength = 100000000;
            mtomEconding.ReaderQuotas.MaxBytesPerRead = 524288;
            mtomEconding.ReaderQuotas.MaxStringContentLength = 5242880;

            HttpsTransportBindingElement httpsTransposport = new HttpsTransportBindingElement
            {
                ManualAddressing = false,
                MaxBufferPoolSize = 524288,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                AllowCookies = false,
                AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous,
                BypassProxyOnLocal = false,
                HostNameComparisonMode = System.ServiceModel.HostNameComparisonMode.StrongWildcard,
                KeepAliveEnabled = true,
                ProxyAuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous,
                Realm = "",
                TransferMode = System.ServiceModel.TransferMode.Buffered,
                UnsafeConnectionNtlmAuthentication = false,
                UseDefaultWebProxy = false,
                RequireClientCertificate = true
            };
            binding.Elements.Add(mtomEconding);
            binding.Elements.Add(httpsTransposport);

            EndpointAddress epAddre = new EndpointAddress(this._options.Value.ServiceUrl);
            ContractDescription cd = new ContractDescription("FirmaRemota.FirmaRemotaPortType");

            var client = new Proxy.FirmaRemotaPortTypeClient(binding, epAddre);

            client.ClientCredentials.ClientCertificate.Certificate = this._options.Value.Certificate;

            //System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            //      (sender, cert, chain, error) =>
            //      {
            //          return true;
            //      };

            client.Endpoint.EndpointBehaviors.Add(new MaxFaultSizeBehavior(2147483647));

            return client;
        }

        public class MaxFaultSizeBehavior : IEndpointBehavior
        {
            private int _size;

            public MaxFaultSizeBehavior(int size)
            {
                _size = size;
            }

            public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.MaxFaultSize = _size;
            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {
            }

            public void Validate(ServiceEndpoint endpoint)
            {
            }
        }

        #endregion
    }
}
