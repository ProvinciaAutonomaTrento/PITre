// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.CaricaTriasmissioniAggregato
{
    public class CaricaTrasmissioniAggregatoQueryHandler : IRequestHandler<CaricaTrasmissioniAggregatoQuery, CaricaTrasmissioniAggregatoQueryResponse>
    {

        public CaricaTrasmissioniAggregatoQueryHandler (
            ILogger<CaricaTrasmissioniAggregatoQueryHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext context
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._context = context;
            InitializeMapper();

        }

        public async Task<CaricaTrasmissioniAggregatoQueryResponse> Handle(CaricaTrasmissioniAggregatoQuery request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.Id });
            request.Id = safeId.Id;

            //var trasmissioni = await this._context.TrasmissioneEntities.Where(trasm => 
            //    trasm.ID_PROJECT == Convert.ToInt32(request.Id)).ToListAsync();

            var trasmissioni = await this._context.TrasmissioneEntities.TrasmissioniDaCodiceAggregazione(request.Id);

            var dtoTrasmissioni = new List<Trasmissione>();

            foreach (var trasmissione in trasmissioni)
            {
                var dtoTrasmissione = new Trasmissione() { 
                    DataInvio = trasmissione.DTA_INVIO,
                    Id = trasmissione.SYSTEM_ID.ToString(),
                    
                };
                dtoTrasmissioni.Add(dtoTrasmissione);
            }

            var result = new CaricaTrasmissioniAggregatoQueryResponse()
            {
                Trasmissioni = dtoTrasmissioni
            };

            return result;
        }

        #region Private Methods
        private readonly ILogger<CaricaTrasmissioniAggregatoQueryHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IMediator _mediator;
        private readonly IPi3DbContext _context;
        private IMapper _mapper;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TrasmissioneEntity, Trasmissione>()
                    .ForMember(dest => dest.DataInvio, opt => opt.MapFrom(src => src.DTA_INVIO))
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SYSTEM_ID));
;
                //cfg.CreateMap<Autore, Autore>()
                //    .ForMember(dest => dest.DataInvio, opt => opt.MapFrom(src => src.DTA_INVIO))
                //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));

                //cfg.CreateMap<Core.AggregateModels.AggregazioneDocumentaleAggregate.AggregazioneDocumentale, AggregazioneDocumentale>()
                //    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.Description.Value))
                //    .ForMember(dest => dest.IdRegistro, opt => opt.MapFrom(src => src.Registro.Id))
                //    .ForMember(dest => dest.DescrizioneRegistro, opt => opt.MapFrom(src => src.Registro.Description))
                //    .ForMember(dest => dest.Classificazioni, opt => opt.MapFrom(src => src.Classifications))
                //    .ForMember(dest => dest.Documenti, opt => opt.MapFrom(src => src.IdDocs))
                //    .ForMember(dest => dest.Profili, opt => opt.MapFrom(src => src.Profiles))
                //    //.ForMember(dest => dest.CollocazioneFisica, opt => opt.MapFrom(src => src.CollocazioneFisica))
                //    .ForMember(dest => dest.Permessi, opt => opt.MapFrom(src => src.Permissions))
                //    .ForMember(dest => dest.Profili, opt => opt.MapFrom(src => src.Profiles))
                //    .ForMember(dest => dest.Sottofascicoli, opt => opt.MapFrom(src => src.Folders))
                //    ;

            });

            _mapper = configuration.CreateMapper();

        }
        #endregion
    }
}
