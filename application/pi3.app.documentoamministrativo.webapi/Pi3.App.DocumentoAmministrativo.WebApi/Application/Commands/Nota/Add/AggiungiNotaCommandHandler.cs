// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Note.Add;

public class AggiungiNotaCommandHandler : IRequestHandler<AggiungiNotaCommand, AggiungiNotaCommandResponse>
{
    private readonly ILogger<AggiungiNotaCommandHandler> _logger;
    private readonly IClaimsPrincipalService _claimsPrincipalService;
    private readonly INotaRepository _notaRepository;
    private readonly IMediator _mediator;

    public AggiungiNotaCommandHandler(
        ILogger<AggiungiNotaCommandHandler> logger,
        IClaimsPrincipalService claimsPrincipalService,
        INotaRepository notaRepository,
        IMediator mediator)
    {
        _logger = logger;
        _claimsPrincipalService = claimsPrincipalService;
        _notaRepository = notaRepository;
        _mediator = mediator;
    }

    public async Task<AggiungiNotaCommandResponse> Handle(AggiungiNotaCommand request, CancellationToken cancellationToken)
    {
        //if (!request.idOggetto.IsValidAggregateId())
        //    throw new IdAggregateNotFoundPi3Exception(request.idOggetto);

        var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
        var delegatedIdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser, false);
        var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);

        var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

        var nota = new Core.AggregateModels.NotaAggregate.Nota(idTenant, DateTime.Now, new TextValue(request.nome),
            new TextValue(request.description), new Core.AggregateModels.NotaAggregate.ValueObjects.AutoreNota()
            {
                IdRuolo = idGroup,
                IdUtente = idUser,
                IdUtenteDelegato = delegatedIdUser
            }, request.idOggetto,
                (Core.AggregateModels.NotaAggregate.ValueObjects.TipiOggettoEnum)request.tipoOggetto,
                (Core.AggregateModels.NotaAggregate.ValueObjects.TipoAccessoNotaEnum)request.tipoAccesso,
                request.idAccessoRF);

        await _notaRepository.Add(nota);

        return
            new AggiungiNotaCommandResponse { Id = nota.Id };
    }
}
