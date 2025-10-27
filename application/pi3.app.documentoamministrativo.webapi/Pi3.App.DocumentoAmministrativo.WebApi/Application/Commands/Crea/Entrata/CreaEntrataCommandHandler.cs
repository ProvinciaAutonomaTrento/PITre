// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Common;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Entrata
{
    public class CreaEntrataCommandHandler : IRequestHandler<CreaEntrataCommand, CreaEntrataCommandResponse>
    {
        #region Public Members

        public CreaEntrataCommandHandler(
            ILogger<CreaEntrataCommandHandler> logger,
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

        public async Task<CreaEntrataCommandResponse> Handle(CreaEntrataCommand request, CancellationToken cancellationToken)
        {
            var commonRequest = this._mapper.Map<CreaCommonCommand>(request);
            commonRequest.TipoDocumento = TipologiaFlussoEnum.E;
            var response = await this._mediator.Send(commonRequest);
            return _mapper.Map<CreaEntrataCommandResponse>(response);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CreaEntrataCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IConfigurationService _configurationService;
        protected readonly IMediator _mediator;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;

        protected IMapper _mapper = null!;

        protected void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OggettoDelDocumento, Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.OggettoDelDocumento>();

                cfg.CreateMap<Soggetto, Mittente>()
                    .ConstructUsing(src => new Mittente(new PG() { DenominazioneOrganizzazione = new TextValue(src.Denominazione) }, src.Id));

                cfg.CreateMap<ProtocolloMittente, Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloMittente>();

                cfg.CreateMap<ProtocolloEmergenza, Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloEmergenza>();
                cfg.CreateMap<Classification, Classification>();

                cfg.CreateMap<Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo, CreaEntrataCommandResponse>()
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

                cfg.CreateMap<CreaCommonCommandResponse, CreaEntrataCommandResponse>();
                cfg.CreateMap<CreaEntrataCommand, CreaCommonCommand>();

            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
