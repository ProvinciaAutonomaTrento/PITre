// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.FirmaDigitale;
using Pi3.Infrastructure.Tibco.Services.File.FirmaDigitale.Exceptions;
using Pi3.Infrastructure.Tibco.Services.File.FirmaDigitale.Proxy;
using Pi3.Infrastructure.Tibco.Services.File.FirmaDigitale.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Pi3.Infrastructure.Tibco.Services.File.FirmaDigitale
{
    public class FirmaDigitaleService : IFirmaDigitaleService
    {
        #region Public Members

        public FirmaDigitaleService(ILogger<FirmaDigitaleService> logger, IOptions<FirmaDigitaleServiceOptions> options)
        {
            this._logger = logger;
            this._options = options;

            this.InitializeMapper();
        }

        public async Task<VerificaResponse> Verifica(VerificaRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            
            Validator.ValidateObject(request, new ValidationContext(request), true);

            try
            {
                using (var client = this.CreateClient())
                {
                    try
                    {
                        var serviceResponse = await client.VerificaFirmaAsync(
                            new VerificaFirmaRequest(request.FileFirmato,
                                request.FirmaSha1WithRSA.HasValue && request.FirmaSha1WithRSA.Value ? (sbyte)1 : (sbyte)0,
                                request.MarcaSha1WithRSA.HasValue && request.MarcaSha1WithRSA.Value ? (sbyte)1 : (sbyte)0,
                                this.DateTimeAsServiceCompliantString(request.DataVerifica),
                                request.ControlloFirmeAnnidate.HasValue && request.ControlloFirmeAnnidate.Value ? (sbyte)1 : (sbyte)0));

                        return this._mapper.Map<VerificaResponse>(serviceResponse);
                    }
                    catch (Exception ex)
                    {
                        throw new FirmaDigitalePi3Exception(ErrorDescriptions.ServiceError, ErrorDescriptions.ResourceManager, ex, ex.Message);
                    }
                }
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

        #endregion

        #region Private Members

        protected readonly ILogger<FirmaDigitaleService> _logger;
        protected readonly IOptions<FirmaDigitaleServiceOptions> _options;

        protected IMapper _mapper = null;

        protected void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Proxy.VerificaFirmaResponse, VerificaResponse>();

                cfg.CreateMap<Proxy.DocumentoType, Documento>()
                    .ForMember(dest => dest.Contenuto, opt => opt.MapFrom(src => src.fileOriginale));

                cfg.CreateMap<Proxy.DettaglioFirmaDigitaleType, EsitoVerificaFirma>()
                    .ForMember(dest => dest.Firmatari, opt => opt.MapFrom(src => src.datiFirmatari));

                cfg.CreateMap<Proxy.FirmatarioType, Firmatario>()
                    .ForMember(dest => dest.DatiFirma, opt => opt.MapFrom(src => src.firmatario));

                cfg.CreateMap<Proxy.DatiFirmaType, DatiFirma>();
                cfg.CreateMap<Proxy.MarcaType, Marca>();
                cfg.CreateMap<Proxy.FirmatarioTypeControfirmatario, FirmatarioControfirmatario>()
                    .ForMember(dest => dest.DatiFirma, opt => opt.MapFrom(src => src.firma));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected virtual string DateTimeAsServiceCompliantString(DateTime? dataVerifica = null)
        {
            var data = DateTime.MinValue;

            if (dataVerifica.HasValue)
                data = dataVerifica.Value;

            return String.Format("{0}{1}{2}{3}{4}{5}",
                data.Year.ToString().Remove(0, 2).PadLeft(2, '0'),
                data.Month.ToString().PadLeft(2, '0'),
                data.Day.ToString().PadLeft(2, '0'),
                data.Hour.ToString().PadLeft(2, '0'),
                data.Minute.ToString().PadLeft(2, '0'),
                data.Second.ToString().PadLeft(2, '0'));
        }

        protected virtual FirmaDigitalePortTypeClient CreateClient()
        {
            var binding = new CustomBinding();
            binding.Name = "FirmaDigitalePortTypeEndpoint2BindingMIO";

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
            ContractDescription cd = new ContractDescription("FirmaDigitale.FirmaDigitalePortType");

            var client = new Proxy.FirmaDigitalePortTypeClient(binding, epAddre);

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
