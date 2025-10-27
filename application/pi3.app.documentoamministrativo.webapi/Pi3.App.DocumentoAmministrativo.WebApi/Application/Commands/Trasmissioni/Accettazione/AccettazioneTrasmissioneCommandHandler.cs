// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Accettazione;

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
        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
        var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
        var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);
        var delegatedIdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser, false);
        var delegatedUserId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserId, false);

        if (!await this._trasmissioneRepository.Exists(idTenant!, request.Id!))
            throw new TrasmissioneNotFoundPi3Exception(request.Id);

        var aggregate = await this._trasmissioneRepository.Get(idTenant!, request.Id);

        aggregate.Accetta(
            idGroup!, 
            idUser!, 
            new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.Accetta { 
               IdDelegato = delegatedIdUser,
               UserIdDelegato = delegatedUserId
            });
        await this._trasmissioneRepository.Update(aggregate);

        return new AccettazioneTrasmissioneCommandResponse();
    }
}
