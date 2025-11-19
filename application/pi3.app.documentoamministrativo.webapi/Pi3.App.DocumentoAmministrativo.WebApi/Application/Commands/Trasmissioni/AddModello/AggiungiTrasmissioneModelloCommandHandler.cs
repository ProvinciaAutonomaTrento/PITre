// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Entities;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Text.RegularExpressions;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.AddModello;

public class AggiungiTrasmissioneModelloCommandHandler: IRequestHandler<AggiungiTrasmissioneModelloCommand,AggiungiTrasmissioneModelloCommandResponse>
{
    private readonly ITrasmissioneRepository _repository;
    private readonly IPi3DbContext _context;
    private readonly ILogger<AggiungiTrasmissioneModelloCommandHandler> _logger;
    private readonly IClaimsPrincipalService _claimsPrincipalService;
    private readonly IMediator _mediator;
    private readonly IModelloTrasmissioneRepository _modelloTrasmissioneRepository;

    public AggiungiTrasmissioneModelloCommandHandler(
        ITrasmissioneRepository repository,
        IPi3DbContext context, ILogger<AggiungiTrasmissioneModelloCommandHandler> logger,
        IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
        IModelloTrasmissioneRepository modelloTrasmissioneRepository)

    {
        this._repository = repository;
        this._context = context;
        this._logger = logger;
        this._claimsPrincipalService = claimsPrincipalService;
        this._mediator = mediator;
        this._modelloTrasmissioneRepository = modelloTrasmissioneRepository;
    }

    public async Task<AggiungiTrasmissioneModelloCommandResponse> Handle(AggiungiTrasmissioneModelloCommand request, CancellationToken cancellationToken)
    {
        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
        //var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.IdAggregato });
        //request.IdAggregato = safeId.Id;

        //string pattern = @"^MD_\d{6}$";
        string pattern = @"^MD_\d+$";
        bool isValid = Regex.IsMatch(request.Modello, pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
        if (!isValid)
            throw new InvalidIdModelCodePi3Exception(request.Modello);

        // controllo tramite regexp che sia nel formato giusto MD_{systemId}
        var codModello = request.Modello.Substring(3);

        if (!await _modelloTrasmissioneRepository.Exists(idTenant, codModello))
            throw new ModelCodeNotFoundPi3Exception(codModello);

        var modello = await _modelloTrasmissioneRepository.Get(idTenant, codModello  );

        if (modello.TipoOggettoTrasmesso != Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects.TipiOggettiTrasmessiEnum.DocumentoAmministrativo)
            throw new InvalidModelCodePi3Exception(codModello);

        var aggregate = new Core.AggregateModels.TrasmissioneAggregate.Trasmissione(idTenant, DateTime.Now,
            request.IdAggregato,
            Core.AggregateModels.TrasmissioneAggregate.ValueObjects.TipiOggettiTrasmessiEnum.DocumentoAmministrativo);

        if (modello.NoteGenerali is not null)
            aggregate.ChangeNoteGenerali(new TextValue(modello.NoteGenerali.Value));

        var listaId = new List<string>();

        foreach (var destinatario in modello.Destinatari ) {
            if (destinatario is GruppoDestinatarioModelloTrasmissione) {
                var item = destinatario as GruppoDestinatarioModelloTrasmissione;

                //var gruppoEntity = _context.GroupEntities.Single(x => x.GROUP_ID == item.CodiceGruppo
                //    && x.DISABLED == "N");
                var gruppoEntity = await _context.GroupEntities.EntityDaCodice(item.CodiceGruppo);
                if (gruppoEntity == null)
                    throw new GruppoNotFoundPi3Exception(item.CodiceGruppo);

                var listaUtentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                foreach (var utenteNotificato in item.UtentiNotificati ?? Enumerable.Empty<DatiUtenteNotificato>())
                {
                    var utente = new DatiUtenteNotificatoTrasmissioneSingolaGruppo
                    { UserId = utenteNotificato.UserId,
                        Cognome = utenteNotificato.Cognome,
                        Nome = utenteNotificato.Nome,
                        IdUtente = utenteNotificato.IdUtente
                    };
                    listaUtentiNotificati.Add(utente);
                }

                aggregate.PrepareTrasmissioneSingolaGruppo(new DatiTrasmissioneSingolaGruppo()
                {
                    IdGruppoDestinatario = gruppoEntity.SYSTEM_ID.ToString(),
                    Tipo = (Core.AggregateModels.TrasmissioneAggregate.ValueObjects.TipiTrasmissioneSingolaEnum)item.TipoTrasmissioneSingola,
                    Note = item.NoteTrasmissioneSingola != null ? new TextValue(item.NoteTrasmissioneSingola.Value) : null,
                    IdRagioneTrasmissione = item.RagioneTrasmissione.Id.ToString(),
                    UtentiNotificati = listaUtentiNotificati
                });
            }
            else if (destinatario is UtenteDestinatarioModelloTrasmissione) {
                var item = destinatario as UtenteDestinatarioModelloTrasmissione;

                //var utenteEntity = _context.PeopleEntities.Single(x => x.SYSTEM_ID == Convert.ToInt32(item.Id)
                //    && x.DISABLED == "N");
                //var utenteEntity = await _context.PeopleEntities.EntityFromSystemId(item.Id);

                var idUtente = await _context.PeopleEntities.SystemIdFromUserId(item.UserId);

                //var idRagione = (await _context.RagioneTrasmissioneEntities.SingleAsync(x => x.VAR_DESC_RAGIONE == item.RagioneTrasmissione.Nome
                //    && x.ID_AMM == Convert.ToInt32(idTenant) && x.CHA_VIS == "1" && x.CHA_RAG_SISTEMA == "0")).SYSTEM_ID;
                var idRagione = await _context.RagioneTrasmissioneEntities.SystemIdDaRagioneTrasmissione(item.RagioneTrasmissione.Nome, idTenant);

                aggregate.PrepareTrasmissioneSingolaUtente(new DatiTrasmissioneSingolaUtente()
                {
                    IdUtente = idUtente.ToString(),
                    IdRagioneTrasmissione = idRagione.ToString(),
                    Note = new TextValue(item.NoteTrasmissioneSingola.Value),

                    DataScadenza = item.GiorniScadenza != null ?
                        DateTime.Now.AddDays(item.GiorniScadenza.Value) :
                        null
                });

            }
        }

        await _repository.Add(aggregate);

        return new AggiungiTrasmissioneModelloCommandResponse { Id = modello.Id };
    }
}
