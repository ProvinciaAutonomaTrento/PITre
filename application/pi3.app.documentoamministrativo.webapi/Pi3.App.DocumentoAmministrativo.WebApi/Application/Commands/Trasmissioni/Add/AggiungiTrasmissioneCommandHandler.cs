// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Add;

public class AggiungiTrasmissioneCommandHandler : IRequestHandler<AggiungiTrasmissioneCommand, AggiungiTrasmissioneCommandResponse>
{
    private readonly ITrasmissioneRepository _repository;
    private readonly IPi3DbContext _context;
    private readonly ILogger<AggiungiTrasmissioneCommandHandler> _logger;
    private readonly IClaimsPrincipalService _claimsPrincipalService;
    private readonly IMediator _mediator;

    public AggiungiTrasmissioneCommandHandler(ITrasmissioneRepository repository,
        IPi3DbContext context, ILogger<AggiungiTrasmissioneCommandHandler> logger,
        IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
    {
        this._repository = repository;
        this._context = context;
        this._logger = logger;
        this._claimsPrincipalService = claimsPrincipalService;
        this._mediator = mediator;
    }

    public async Task<AggiungiTrasmissioneCommandResponse> Handle(AggiungiTrasmissioneCommand request, CancellationToken cancellationToken)
    {
        if (request.UtentiDestinatari != null && request.UtentiDestinatari.Any() &&
            request.GruppiDestinatari != null && request.GruppiDestinatari.Any())
            throw new UsersAndGroupsDefinedPi3Exception();

        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
        //var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.IdAggregato }, CancellationToken.None);
        //request.IdAggregato = safeId.Id;

        var aggregate = new Core.AggregateModels.TrasmissioneAggregate.Trasmissione(idTenant, DateTime.Now,
            request.IdAggregato, TipiOggettiTrasmessiEnum.DocumentoAmministrativo);

        if (!string.IsNullOrWhiteSpace(request.UpdatedId) ) {
            aggregate = await _repository.Get(idTenant, request.UpdatedId);
        }

        if(!string.IsNullOrWhiteSpace(request.NoteTrasmissione))
            aggregate.ChangeNoteGenerali(new TextValue(request.NoteTrasmissione));

        if (!string.IsNullOrWhiteSpace(request.UpdatedId) &&
            (!request.Append.HasValue || !request.Append.Value)) {
            if (aggregate.TrasmissioniSingole != null && aggregate.TrasmissioniSingole.Any()) {
                    
                for (var i = aggregate.TrasmissioniSingole.Count -1; i >= 0; i--)
                {
                    var trasmissione = aggregate.TrasmissioniSingole[i];

                    if (trasmissione is TrasmissioneSingolaGruppo)
                        aggregate.RemoveTrasmissioneSingola(trasmissione.Id);
                    else if (trasmissione is TrasmissioneSingolaUtente ) 
                        aggregate.RemoveTrasmissioneUtente(trasmissione.Id);
                }
            }
        }

        foreach (var utenteDestinatario in request.UtentiDestinatari ?? Enumerable.Empty<UtenteDestinatario>()) {
            var idUtente = await _context.PeopleEntities.SystemIdFromUserId(utenteDestinatario.UserId);

            var idRagione = await _context.RagioneTrasmissioneEntities.SystemIdDaRagioneTrasmissione(
                utenteDestinatario.RagioneTrasmissione, idTenant);

            aggregate.PrepareTrasmissioneSingolaUtente(new DatiTrasmissioneSingolaUtente()
            {
                IdUtente = idUtente.ToString(),
                IdRagioneTrasmissione = idRagione.ToString(),
                Note = new TextValue(utenteDestinatario.NoteTrasmissione),

                DataScadenza = utenteDestinatario.GiorniScadenza != null ?
                    DateTime.Now.AddDays(utenteDestinatario.GiorniScadenza.Value) :
                    null
            });
        }

        foreach (var gruppoDestinatario in request.GruppiDestinatari ?? Enumerable.Empty<GruppoDestinatario>())
        {
            var idGruppo = await _context.GroupEntities.SystemIdDaCodiceGruppo(gruppoDestinatario.CodiceGruppo);

            var idRagione = await _context.RagioneTrasmissioneEntities.SystemIdDaRagioneTrasmissione(
                gruppoDestinatario.RagioneTrasmissione, idTenant);

            var tipoTrasmissione = TipiTrasmissioneSingolaEnum.Uno;
            if (gruppoDestinatario?.Tipo?.ToUpper() == "TUTTI")
                tipoTrasmissione = TipiTrasmissioneSingolaEnum.Tutti;
            else if (gruppoDestinatario?.Tipo?.ToUpper() == "UNO")
                tipoTrasmissione = TipiTrasmissioneSingolaEnum.Uno;

            List<DatiUtenteNotificatoTrasmissioneSingolaGruppo> ?listaUtentiNotificati = null;
            foreach (var userId in gruppoDestinatario.UtentiNotificati ?? Enumerable.Empty<string>()) 
            {
                if (listaUtentiNotificati == null)
                    listaUtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();

                var idUtente = await this._context.PeopleEntities.SystemIdFromUserId(userId);

                listaUtentiNotificati.Add( new DatiUtenteNotificatoTrasmissioneSingolaGruppo
                {
                    IdUtente = idUtente.ToString()
                });
            }

            aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
            {
                IdGruppoDestinatario = idGruppo.ToString(), 
                Tipo = tipoTrasmissione,
                Note = new TextValue(request.NoteTrasmissione),
                IdRagioneTrasmissione = idRagione.ToString(),// 131976805.ToString(),
                UtentiNotificati = listaUtentiNotificati
            });
        }

        if (request.Invio != null && request.Invio.Value)
        {
            aggregate.Invia();
            await this._repository.Update(aggregate);
        }
        if (!string.IsNullOrWhiteSpace(request.UpdatedId))
            await this._repository.Update(aggregate);
        else if (request.UpdatedId == null)
            await this._repository.Add(aggregate);

        return new AggiungiTrasmissioneCommandResponse()
        {
            Id = aggregate.Id
        };

    }
}

