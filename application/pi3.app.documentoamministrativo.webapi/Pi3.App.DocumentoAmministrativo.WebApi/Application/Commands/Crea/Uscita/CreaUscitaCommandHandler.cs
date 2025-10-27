// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.OpenApi.Extensions;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Common;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using System.ComponentModel;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Entrata
{
    public class CreaUscitaCommandHandler : IRequestHandler<CreaUscitaCommand, CreaUscitaCommandResponse>
    {
        #region Public Members

        public CreaUscitaCommandHandler(
            ILogger<CreaUscitaCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IConfigurationService configurationService,
            IMediator mediator,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IDocumentBlobRepository documentBlobRepository)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _configurationService = configurationService;
            _mediator = mediator;
            _documentoAmministrativoRepository = documentoAmministrativoRepository;
            _documentBlobRepository = documentBlobRepository;

            InitializeMapper();
        }

        public async Task<CreaUscitaCommandResponse> Handle(CreaUscitaCommand request, CancellationToken cancellationToken)
        {
            var commonRequest = this._mapper.Map<CreaCommonCommand>(request);
            commonRequest.TipoDocumento = TipologiaFlussoEnum.U;
            var response = await this._mediator.Send(commonRequest);
            return _mapper.Map<CreaUscitaCommandResponse>(response);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CreaUscitaCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IConfigurationService _configurationService;
        protected readonly IMediator _mediator;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;


        protected IMapper _mapper = null;

        protected void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OggettoDelDocumento, Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.OggettoDelDocumento>();

                cfg.CreateMap<Soggetto, Mittente>()
                    .ConstructUsing(src => new Mittente(new PG() { DenominazioneOrganizzazione = new TextValue(src.Denominazione) }, src.Id));

                cfg.CreateMap<Soggetto, Destinatario>()
                    .ConstructUsing(src => new Destinatario(new PG() { DenominazioneOrganizzazione = new TextValue(src.Denominazione) }, src.Id));

                cfg.CreateMap<Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo, CreaUscitaCommandResponse>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.CodiceRegistro, opt => opt.MapFrom(src => src.DatiRegistrazione.CodiceRegistro))
                    .ForMember(dest => dest.Numero, opt => opt.MapFrom(src =>
                            src.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo) ?
                                                    ((DatiRegistrazioneProtocollo)src.DatiRegistrazione).NumeroProtocollo :
                                                    ((DatiRegistrazioneRepertorio)src.DatiRegistrazione).NumeroRegistrazione))
                    .ForMember(dest => dest.Data, opt => opt.MapFrom(src =>
                            src.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo) ?
                                                    ((DatiRegistrazioneProtocollo)src.DatiRegistrazione).DataProtocollazione :
                                                    ((DatiRegistrazioneRepertorio)src.DatiRegistrazione).DataRegistrazione))
                    .ForMember(dest => dest.Segnatura, opt => opt.MapFrom(src => src.IdDoc != null ? src.IdDoc.Segnatura : null));

                cfg.CreateMap<CreaCommonCommandResponse, CreaUscitaCommandResponse>();
                cfg.CreateMap<CreaUscitaCommand, CreaCommonCommand>();
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
