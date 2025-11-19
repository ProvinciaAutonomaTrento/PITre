// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.Carica
{
    public class CaricaAggregazioneQueryHandler : IRequestHandler<CaricaAggregazioneQuery, CaricaAggregazioneQueryResult>
    {

        public CaricaAggregazioneQueryHandler(
            ILogger<CaricaAggregazioneQueryHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository,
            IMediator mediator
            )
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._claimsPrincipalService = claimsPrincipalService ?? throw new ArgumentNullException(nameof(claimsPrincipalService));
            this._aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository ?? throw new ArgumentNullException(nameof(aggregazioneDocumentaleRepository));
            this._mediator = mediator;
            InitializeMapper();
        }

        public async Task<CaricaAggregazioneQueryResult> Handle(CaricaAggregazioneQuery request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.Id});
            request.Id = safeId.Id;

            var loadBehavior = new GetAggregatoDocumentaleLoadBehavior()
            {
                LoadDocuments = request.LoadDocuments,
                DocumentsPagination = request.DocumentsPagination != null ? new Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories.Pagination() {
                    Skip = request.DocumentsPagination.Salta,
                    Take = request.DocumentsPagination.Prendi,
                } : null,
                LoadFolderHierarchy = request.LoadFolderHierarchy,
                LoadNote = request.LoadNote,
                LoadPermissions = request.LoadPermissions,
                LoadProfiles = request.LoadProfiles,
                FoldersPagination = request.FoldersPagination != null ?new Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories.Pagination()
                {
                    Skip = request.FoldersPagination.Salta,
                    Take = request.FoldersPagination.Prendi,
                } : null
            };
            var aggregate = await _aggregazioneDocumentaleRepository.Get(idTenant!, request.Id, new [] { loadBehavior } );

            var result = new CaricaAggregazioneQueryResult()
            {
                AggregazioneDocumentale = _mapper.Map<AggregazioneDocumentale>(aggregate)
            };

            return result;
        }

        #region Private Members
        protected readonly ILogger<CaricaAggregazioneQueryHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;
        protected IMapper _mapper = null;
        private readonly IMediator _mediator;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Core.AggregateModels.AggregazioneDocumentaleAggregate.AggregazioneDocumentale, AggregazioneDocumentale>()
                    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.Description.Value))
                    .ForMember(dest => dest.IdRegistro, opt => opt.MapFrom(src => src.DatiRegistrazione.IdRegistro))
                    .ForMember(dest => dest.Classificazioni, opt => opt.MapFrom(src => src.Classifications))
                    .ForMember(dest => dest.Documenti, opt => opt.MapFrom(src => src.IdDocs))
                    .ForMember(dest => dest.Profili, opt => opt.MapFrom(src => src.Profiles))
                    .ForMember(dest => dest.Permessi, opt => opt.MapFrom(src => src.Permissions))
                    .ForMember(dest => dest.Profili, opt => opt.MapFrom(src => src.Profiles))
                    .ForMember(dest => dest.Sottofascicoli, opt => opt.MapFrom(src => src.Folders))
                    ;

                cfg.CreateMap<Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.CollocazioneFisica, CollocazioneFisica>()
                    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.Descrizione));

                cfg.CreateMap<Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc, IdDoc>();

                cfg.CreateMap<Core.AggregateModels.ContentElementAggregate.ValueObjects.ContentElementPermission, Permission>()
                    .ForMember(dest => dest.IdMembro, opt => opt.MapFrom(src => src.IdMember))
                    .ForMember(dest => dest.NomeMembro, opt => opt.MapFrom(src => src.MemberName))
                    .ForMember(dest => dest.TipoPermesso, opt => opt.MapFrom(src => src.PermissionType))
                    .ForMember(dest => dest.TipoMembro, opt => opt.MapFrom(src => src.MemberType))
                    .ForMember(dest => dest.TipoPermesso, opt => opt.MapFrom(src => src.PermissionType));

                cfg.CreateMap<Core.AggregateModels.ContentElementAggregate.Entities.ContentElementClassification, Classification>()
                    .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Name))
                ;

                cfg.CreateMap<Core.AggregateModels.AggregazioneDocumentaleAggregate.Entities.Folder, FolderHierarcy>()
                    .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.Sottofascicoli, opt => opt.MapFrom(src => src.Folders))
                    .ForMember(dest => dest.Documenti, opt => opt.MapFrom(src => src.IdDocs))
                ;

                cfg.CreateMap<Core.AggregateModels.ElementAggregate.Entities.ElementProfile, Profile>()
                    .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Name.Value))
                    .ForMember(dest => dest.Campi, opt => opt.MapFrom(src => src.Fields));

                cfg.CreateMap<Core.AggregateModels.AggregazioneDocumentaleAggregate.Entities.Nota, Nota>()
                    .ForMember(dest => dest.TipologiaVisibilita, opt => opt.MapFrom(src => src.TipologiaVisibilita.ToString()))
                    .ForMember(dest => dest.Testo, opt => opt.MapFrom(src => src.Testo.Value))
                    ;

            });

            _mapper = configuration.CreateMapper();
        }

        #endregion

    }
}
