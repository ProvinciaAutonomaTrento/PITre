// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Entities;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Repositories;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using System.Runtime.CompilerServices;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Pubblica
{
    public class PubblicaCorrispondenteHandler : IRequestHandler<PubblicaCorrispondenteRequest, PubblicaCorrispondenteResponse>
    {
        #region Public Members

        public PubblicaCorrispondenteHandler(ILogger<PubblicaCorrispondenteHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, ICorrispondenteRepository corrispondenteRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._corrispondenteRepository = corrispondenteRepository;

            this._mapper = this.InitializeMapper();
        }

        public async Task<PubblicaCorrispondenteResponse> Handle(PubblicaCorrispondenteRequest request, CancellationToken cancellationToken)
        {
            var aggregate = this._mapper.Map<Corrispondente>(request);
            
            await this._corrispondenteRepository.Add(aggregate);

            return new PubblicaCorrispondenteResponse()
            {
                Id = aggregate.Id
            };
        }

        #endregion

        #region Private Members

        protected readonly ILogger<PubblicaCorrispondenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly ICorrispondenteRepository _corrispondenteRepository;

        protected readonly IMapper _mapper;

        protected IMapper InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Pubblica.PubblicaCorrispondenteRequest, Indirizzo>()
                    .ForMember(dest => dest.Recapito, opt => opt.MapFrom(src => src.DatiCorrispondente.Indirizzo))
                    .ForMember(dest => dest.CAP, opt => opt.MapFrom(src => src.DatiCorrispondente.CAP))
                    .ForMember(dest => dest.Citta, opt => opt.MapFrom(src => src.DatiCorrispondente.Citta))
                    .ForMember(dest => dest.Fax, opt => opt.MapFrom(src => src.DatiCorrispondente.Fax))
                    .ForMember(dest => dest.Nazione, opt => opt.MapFrom(src => src.DatiCorrispondente.Nazione))
                    .ForMember(dest => dest.Provincia, opt => opt.MapFrom(src => src.DatiCorrispondente.Provincia))
                    .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.DatiCorrispondente.Telefono));

                cfg.CreateMap<Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Pubblica.Email, EmailDaAggiornare>()
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Indirizzo))
                    .ForMember(dest => dest.Preferita, opt => opt.MapFrom(src => src.Preferita))
                    .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note));

                cfg.CreateMap<Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Pubblica.PubblicaCorrispondenteRequest, DatiPubblicazione>()
                    .ForMember(dest => dest.Indirizzo, opt => opt.MapFrom(src => this._mapper.Map<Indirizzo>(src)))
                    .ForMember(dest => dest.Amministrazione, opt => opt.MapFrom(src => src.DatiCorrispondente.Amministrazione))
                    .ForMember(dest => dest.AOO, opt => opt.MapFrom(src => src.DatiCorrispondente.AOO))
                    .ForMember(dest => dest.UrlApiInteroperabilita, opt => opt.MapFrom(src => src.DatiCorrispondente.UrlApiInteroperabilita))
                    .ForMember(dest => dest.Codice, opt => opt.MapFrom(src => src.Codice))
                    .ForMember(dest => dest.CodiceFiscale, opt => opt.MapFrom(src => src.DatiCorrispondente.CodiceFiscale))
                    .ForMember(dest => dest.Denominazione, opt => opt.MapFrom(src => src.DatiCorrispondente.Denominazione))
                    .ForMember(dest => dest.PartitaIva, opt => opt.MapFrom(src => src.DatiCorrispondente.PartitaIva))
                    .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.DatiCorrispondente.Tipo))
                    .ForMember(dest => dest.Emails, opt => opt.MapFrom(src => src.Emails));

                cfg.CreateMap<PubblicaCorrispondenteRequest, Corrispondente>()
                    .ConstructUsing(src => new Corrispondente(this._mapper.Map<DatiPubblicazione>(src)))
                    .ForMember(dest => dest.Indirizzo, opt => opt.MapFrom(src => this._mapper.Map<Indirizzo>(src)))
                    .ForMember(dest => dest.Emails, opt => opt.Ignore());
            });

            return configuration.CreateMapper();
        }

        #endregion
    }
}
