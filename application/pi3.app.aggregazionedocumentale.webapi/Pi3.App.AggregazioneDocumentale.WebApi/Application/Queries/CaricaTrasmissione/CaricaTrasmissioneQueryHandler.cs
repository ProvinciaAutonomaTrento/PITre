// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.CaricaTrasmissione
{
    public class CaricaTrasmissioneQueryHandler : IRequestHandler<CaricaTrasmissioneQuery, CaricaTrasmissioneQueryResponse>
    {
        private readonly ILogger<CaricaTrasmissioneQueryHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly ITrasmissioneRepository _trasmissioneRepositoryaggregazioneDocumentaleRepository;
        private readonly IMediator _mediator;
        private readonly IPi3DbContext _context;
        private IMapper _mapper;

        public CaricaTrasmissioneQueryHandler(
            ILogger<CaricaTrasmissioneQueryHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            ITrasmissioneRepository trasmissioneRepositoryaggregazioneDocumentaleRepository,
            IMediator mediator,
            IPi3DbContext context
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._trasmissioneRepositoryaggregazioneDocumentaleRepository = trasmissioneRepositoryaggregazioneDocumentaleRepository;
            this._mediator = mediator;
            this._context = context;
            InitializeMapper();
        }

        public async Task<CaricaTrasmissioneQueryResponse> Handle(CaricaTrasmissioneQuery request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            var aggregate = await this._trasmissioneRepositoryaggregazioneDocumentaleRepository.Get(idTenant, request.Id);
            var trasmissione = _mapper.Map<Trasmissione>(aggregate);

            foreach (var singola in aggregate.TrasmissioniSingole) {
                if (singola is TrasmissioneSingolaGruppo)
                {
                    var trasmissioneGruppo = singola as TrasmissioneSingolaGruppo;
                    var gruppo = new GruppoDestinatario();
                    gruppo.NoteTrasmissione = trasmissioneGruppo?.Note?.Value;
                    gruppo.RagioneTrasmissione = trasmissioneGruppo.RagioneTrasmissione.Nome;
                    if (trasmissioneGruppo.DataScadenza != null)
                        gruppo.DataScadenza = trasmissioneGruppo.DataScadenza.Value.ToLongDateString();
                    gruppo.Tipo = singola.Tipo.ToString();

                    foreach (var notificato in trasmissioneGruppo.TrasmissioniUtente) {
                        if (gruppo.UtentiNotificati == null)
                            gruppo.UtentiNotificati = new List<UtenteNotificato>();

                        var notifica = new UtenteNotificato();
                        notifica.UtenteDestinatario = new UtenteDestinatario();
                        notifica.UtenteDestinatario.UserId = notificato.UtenteDestinatario.UserId;
                        notifica.UtenteDestinatario.Nome = notificato.UtenteDestinatario.Nome;
                        notifica.UtenteDestinatario.Cognome = notificato.UtenteDestinatario.Cognome;

                        if (notifica.UtenteDelegato != null) {
                            notifica.UtenteDelegato = new UtenteDestinatario();
                            notifica.UtenteDelegato.UserId = notificato.UtenteDelegato.UserId;
                            notifica.UtenteDelegato.Nome = notificato.UtenteDelegato.Nome;
                            notifica.UtenteDelegato.Cognome = notificato.UtenteDelegato.Cognome;
                        }

                        notifica.DataVista = notificato.DataVista;
                        notifica.DataAccettazione = notificato.DataAccettazione;
                        notifica.NoteAccettazione = notificato.NoteAccettazione;
                        notifica.DataRifiuto = notificato.DataRifiuto;
                        notifica.NoteRifiuto = notificato.NoteRifiuto;
                        notifica.DataRimozioneCentroNotifiche = notificato.DataRimozioneCentroNotifiche;

                        ((IList<UtenteNotificato>)gruppo.UtentiNotificati).Add(notifica);
                    }

                    var gruppoEntity = await this._context.GroupEntities.EntityDaSystemId(trasmissioneGruppo.GruppoDestinatario.Id);

                    gruppo.DescrizioneGruppo = gruppoEntity.GROUP_NAME;
                    if (trasmissione.GruppiDestinatari == null)
                        trasmissione.GruppiDestinatari = new List<GruppoDestinatario>();
                    trasmissione.GruppiDestinatari.Add(gruppo);
                }
                else if (singola is TrasmissioneSingolaUtente ) {
                    var utenteEntity = await this._context.PeopleEntities.EntityFromSystemId(singola.Id);
                    var utente = new UtenteDestinatario();
                    utente.NoteTrasmissione = singola.Note.Value;
                    utente.Nome = utenteEntity.VAR_NOME;
                    utente.Cognome = utenteEntity.VAR_COGNOME;
                    utente.UserId = utenteEntity.USER_ID;
                    utente.RagioneTrasmissione = singola?.RagioneTrasmissione.Nome;
                    utente.GiorniScadenza = (singola.DataScadenza - DateTime.Today).Value.TotalDays.ToString();

                    if (trasmissione.UtentiDestinatari == null)
                        trasmissione.UtentiDestinatari = new List<UtenteDestinatario>();
                    trasmissione.UtentiDestinatari.Add(utente);
                }
            }

            return new CaricaTrasmissioneQueryResponse { Trasmissione = trasmissione };
        }

        #region private
        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Core.AggregateModels.TrasmissioneAggregate.Trasmissione, Trasmissione>()
                    .ForMember(dest => dest.DataInvio, opt => opt.MapFrom(src => src.DataInvio.Value.ToShortDateString()))
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.NoteGenerali, opt => opt.MapFrom(src => src.NoteGenerali));
                
                cfg.CreateMap<Core.AggregateModels.TrasmissioneAggregate.ValueObjects.Autore, Autore>();
                cfg.CreateMap<Core.AggregateModels.TrasmissioneAggregate.ValueObjects.Autore, Autore>();

            });

            _mapper = configuration.CreateMapper();

        }

        #endregion
    }
}
