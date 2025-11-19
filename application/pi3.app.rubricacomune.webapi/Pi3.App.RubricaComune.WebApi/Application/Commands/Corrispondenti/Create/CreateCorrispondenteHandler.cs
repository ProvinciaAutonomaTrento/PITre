// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Repositories;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.Create
{
    public class CreateCorrispondenteHandler : IRequestHandler<CreateCorrispondenteRequest, CreateCorrispondenteResponse>
    {
        #region Public Members

        public CreateCorrispondenteHandler(ILogger<CreateCorrispondenteHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, ICorrispondenteRepository corrispondenteRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._corrispondenteRepository = corrispondenteRepository;

            this._mapper = this.InitializeMapper();
        }

        public async Task<CreateCorrispondenteResponse> Handle(CreateCorrispondenteRequest request, CancellationToken cancellationToken)
        {
            if (await this._corrispondenteRepository.ExistByCodice(request.Codice))
                throw new CodiceCorrispondenteAlreadyExistsPi3Exception(request.Codice);

            var aggregate = this._mapper.Map<Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Corrispondente>(request);

            await this._corrispondenteRepository.Add(aggregate);

            return new CreateCorrispondenteResponse()
            {
                Id = aggregate.Id
            };
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CreateCorrispondenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly ICorrispondenteRepository _corrispondenteRepository;

        protected readonly IMapper _mapper;

        protected IMapper InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateCorrispondenteRequest, Indirizzo>()
                    .ForMember(dest => dest.Recapito, opt => opt.MapFrom(src => src.DatiCorrispondente.Indirizzo));

                cfg.CreateMap<CreateCorrispondenteRequest, Corrispondente>()
                    .ConstructUsing(src => MappaACorrispondente(src))
                    .ForMember(dest => dest.Indirizzo, opt => opt.Ignore());

                cfg.CreateMap<Tipi, Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum>();            
            });

            return configuration.CreateMapper();
        }


        private static Indirizzo MappaAIndirizzo(CreateCorrispondenteRequest source)
        {
            var indirizzo = new Indirizzo
            {
                Recapito = source.DatiCorrispondente.Indirizzo,
                CAP = source.DatiCorrispondente.CAP,
                Citta = source.DatiCorrispondente.Citta,
                Fax = source.DatiCorrispondente.Fax,
                Nazione = source.DatiCorrispondente.Nazione,
                Provincia = source.DatiCorrispondente.Provincia,
                Telefono = source.DatiCorrispondente.Telefono
            };
            return indirizzo;
        }

        private static Corrispondente MappaACorrispondente(CreateCorrispondenteRequest source)
        {
            var corrispondente = new Corrispondente(
                source.Codice,
                source.DatiCorrispondente.Denominazione,
                MappaTipiEnum(source.DatiCorrispondente.Tipo),
                null,
                null);

            corrispondente.ChangeIndirizzo(MappaAIndirizzo(source));
            corrispondente.ChangeCodiceFiscale(source.DatiCorrispondente.CodiceFiscale);
            corrispondente.ChangePartitaIva(source.DatiCorrispondente.PartitaIva);
            corrispondente.ChangeUrlApiInteroperabilita(source.DatiCorrispondente.UrlApiInteroperabilita);
            corrispondente.ChangeAoo(source.DatiCorrispondente.AOO);
            corrispondente.ChangeAmministrazione(source.DatiCorrispondente.Amministrazione);

            return corrispondente;
        }

        private static Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum MappaTipiEnum(Tipi tipi)
        {
            switch (tipi)
            {
                case Tipi.UnitaOrganizzativa:
                    return Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum.UnitaOrganizzativa;
                case Tipi.RaggruppamentoFunzionale:
                    return Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum.RaggruppamentoFunzionale;
                default:
                    return Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum.UnitaOrganizzativa;
            }
        }


        #endregion
    }

}
