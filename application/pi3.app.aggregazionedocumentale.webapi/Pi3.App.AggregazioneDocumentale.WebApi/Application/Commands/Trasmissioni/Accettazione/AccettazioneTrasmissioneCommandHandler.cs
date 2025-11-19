// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Trasmissioni.Accettazione;

public class AccettazioneTrasmissioneCommandHandler : IRequestHandler<AccettazioneTrasmissioneCommand, AccettazioneTrasmissioneCommandResponse>
{
    private readonly ILogger<AccettazioneTrasmissioneCommandHandler> _logger;
    private readonly IClaimsPrincipalService _claimsPrincipalService;
    private readonly ITrasmissioneRepository _trasmissioneRepository;
    private readonly IMediator _mediator;
    private readonly IPi3DbContext _context;

    public AccettazioneTrasmissioneCommandHandler(
        ILogger<AccettazioneTrasmissioneCommandHandler> logger,
        IClaimsPrincipalService claimsPrincipalService,
        ITrasmissioneRepository trasmissioneRepository,
        IMediator mediator,
        IPi3DbContext context
        )
    {
        this._logger = logger;
        this._claimsPrincipalService = claimsPrincipalService;
        this._trasmissioneRepository = trasmissioneRepository;
        this._mediator = mediator;
        this._context = context;
    }

    public async Task<AccettazioneTrasmissioneCommandResponse> Handle(AccettazioneTrasmissioneCommand request, CancellationToken cancellationToken)
    {
        if (!request.Id.IsValidAggregateId())
            throw new IdAggregateNotFoundPi3Exception(request.Id);

        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
        var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId, true);
        var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);

        var aggregate = await this._trasmissioneRepository.Get(idTenant, request.Id);

        var idUtente = _context.PeopleEntities.SystemIdFromUserId(userId);

        aggregate.Accetta(idGroup.ToString(), idUtente.ToString(), new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.Accetta { 
           IdDelegato = idUtente.ToString(),
           UserIdDelegato = userId
        });
        await this._trasmissioneRepository.Update(aggregate);

        return new AccettazioneTrasmissioneCommandResponse();
    }
}
