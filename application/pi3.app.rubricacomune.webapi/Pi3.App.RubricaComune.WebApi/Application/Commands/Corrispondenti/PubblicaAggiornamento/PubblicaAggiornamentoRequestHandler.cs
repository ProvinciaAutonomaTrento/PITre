// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Repositories;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.ValueObjects;
using Pi3.App.RubricaComune.Infrastructure.EF.AggregateModels.CorrispondenteAggregate.Exceptions;
using Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Pubblica;
using Pi3.Core.Services.Principal;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.PubblicaAggiornamento
{
    public class PubblicaAggiornamentoRequestHandler : IRequestHandler<PubblicaAggiornamentoRequest>
    {
        #region Public Members

        public PubblicaAggiornamentoRequestHandler(ILogger<PubblicaAggiornamentoRequestHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, ICorrispondenteRepository corrispondenteRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._corrispondenteRepository = corrispondenteRepository;

           this._mapper =  this.InitializeMapper();
        }

        public async Task Handle(PubblicaAggiornamentoRequest request, CancellationToken cancellationToken)
        {
            if (!await this._corrispondenteRepository.Exists(request.Id))
                throw new CorrispondenteNotFoundPi3Exception(request.Id);

            var aggregate = await this._corrispondenteRepository.Get(request.Id);

            aggregate.PubblicaAggiornamento(this._mapper.Map<DatiPubblicazioneAggiornamento>(request.DatiAggiornamento));

            await this._corrispondenteRepository.Update(aggregate);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<PubblicaAggiornamentoRequestHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly ICorrispondenteRepository _corrispondenteRepository;

        private readonly IMapper _mapper;

        protected IMapper InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DatiAggiornamento, Indirizzo>()
                    .ForMember(dest => dest.Recapito, dest => dest.MapFrom(src => src.DatiCorrispondente.Indirizzo));

                cfg.CreateMap<Email, EmailDaAggiornare>()
                    .ForMember(dest => dest.Email, dest => dest.MapFrom(src => src.Indirizzo));

                cfg.CreateMap<DatiAggiornamento, DatiPubblicazioneAggiornamento>()
                    .ForMember(dest => dest.Denominazione, dest => dest.MapFrom(src => src.DatiCorrispondente.Denominazione))
                    .ForMember(dest => dest.CodiceFiscale, dest => dest.MapFrom(src => src.DatiCorrispondente.CodiceFiscale))
                    .ForMember(dest => dest.PartitaIva, dest => dest.MapFrom(src => src.DatiCorrispondente.PartitaIva))
                    .ForMember(dest => dest.UrlApiInteroperabilita, dest => dest.MapFrom(src => src.DatiCorrispondente.UrlApiInteroperabilita))
                    .ForMember(dest => dest.Amministrazione, dest => dest.MapFrom(src => src.DatiCorrispondente.Amministrazione))
                    .ForMember(dest => dest.AOO, dest => dest.MapFrom(src => src.DatiCorrispondente.AOO))                    
                    .ForMember(dest => dest.Indirizzo, dest => dest.MapFrom(src => MapIndirizzo(src)));
            });

            return configuration.CreateMapper();
        }

        private static Indirizzo MapIndirizzo(DatiAggiornamento datiAggiornamento)
        {
            return new Indirizzo()
            {
                CAP = datiAggiornamento.DatiCorrispondente.CAP,
                Citta = datiAggiornamento.DatiCorrispondente.Citta,
                Fax = datiAggiornamento.DatiCorrispondente.Fax,
                Nazione = datiAggiornamento.DatiCorrispondente.Nazione,
                Provincia = datiAggiornamento.DatiCorrispondente.Provincia,
                Telefono = datiAggiornamento.DatiCorrispondente.Telefono
            };
        }

        #endregion
    }

}
