// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Trasmissioni.Invio;

public class InvioTrasmissioneCommandHandler : IRequestHandler<InvioTrasmissioneCommand, InvioTrasmissioneCommandResponse>
{
    private readonly ILogger<InvioTrasmissioneCommandHandler> _logger;
    private readonly IClaimsPrincipalService _claimsPrincipalService;
    private readonly ITrasmissioneRepository _trasmissioneRepository;
    private readonly IMediator _mediator;
    private readonly IPi3DbContext _context;

    public InvioTrasmissioneCommandHandler(
        ILogger<InvioTrasmissioneCommandHandler> logger,
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

    public async Task<InvioTrasmissioneCommandResponse> Handle(InvioTrasmissioneCommand request, CancellationToken cancellationToken)
    {
        if (!request.Id.IsValidAggregateId())
            throw new TrasmissioneNotFoundPi3Exception(request.Id);

        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

        if (!await this._trasmissioneRepository.Exists(idTenant!, request.Id!))
            throw new TrasmissioneNotFoundPi3Exception(request.Id);

        var aggregate = await this._trasmissioneRepository.Get(idTenant!, request.Id);
        if (aggregate == null)
            throw new TrasmissioneNotFoundPi3Exception(request.Id);

        aggregate.Invia(request.DataInvio);

        await this._trasmissioneRepository.Update(aggregate);

        return new InvioTrasmissioneCommandResponse();
    }
}
