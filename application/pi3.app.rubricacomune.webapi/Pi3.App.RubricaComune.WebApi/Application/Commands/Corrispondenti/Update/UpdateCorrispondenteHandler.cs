// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Repositories;
using Pi3.App.RubricaComune.Infrastructure.EF.AggregateModels.CorrispondenteAggregate.Exceptions;
using Pi3.Core.Services.Principal;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Update
{
    public class UpdateCorrispondenteHandler : IRequestHandler<UpdateCorrispondenteRequest>
    {
        #region Public Members

        public UpdateCorrispondenteHandler(ILogger<UpdateCorrispondenteHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, ICorrispondenteRepository corrispondenteRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._corrispondenteRepository = corrispondenteRepository;

            this._mapper = this.InitializeMapper();
        }

        public async Task Handle(UpdateCorrispondenteRequest request, CancellationToken cancellationToken)
        {
            if (!await this._corrispondenteRepository.Exists(request.Id))
                throw new CorrispondenteNotFoundPi3Exception(request.Id);

            var aggregate = await this._corrispondenteRepository.Get(request.Id);

            aggregate.ChangeDenominazione(request.DatiCorrispondente.Denominazione);
            aggregate.ChangeTipo(this._mapper.Map<Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum>(request.DatiCorrispondente.Tipo));
            aggregate.ChangePartitaIva(request.DatiCorrispondente.PartitaIva);
            aggregate.ChangeCodiceFiscale(request.DatiCorrispondente.CodiceFiscale);
            aggregate.ChangeUrlApiInteroperabilita(request.DatiCorrispondente.UrlApiInteroperabilita);
            aggregate.ChangeAmministrazione(request.DatiCorrispondente.Amministrazione);
            aggregate.ChangeAoo(request.DatiCorrispondente.AOO);
            aggregate.ChangeIndirizzo(this._mapper.Map<Core.AggregateModels.CorrispondenteAggregate.ValueObjects.Indirizzo>(request.DatiCorrispondente));

            await this._corrispondenteRepository.Update(aggregate);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UpdateCorrispondenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly ICorrispondenteRepository _corrispondenteRepository;

        protected IMapper _mapper;

        protected IMapper InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Tipi, Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum>();
                cfg.CreateMap<DatiCorrispondente, Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.ValueObjects.Indirizzo>()
                    .ForMember(dest => dest.Recapito, src => src.MapFrom(src => src.Indirizzo));
            });

            return configuration.CreateMapper();
        }

        #endregion
    }
}
