// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Apri;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Descrizione
{
    public class DescrizioneCommandHandler : IRequestHandler<DescrizioneCommand, DescrizioneCommandResponse>
    {
        private readonly ILogger<DescrizioneCommandHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;
        private readonly IMediator _mediator;

        public DescrizioneCommandHandler(
            ILogger<DescrizioneCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository,
            IMediator mediator) {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;
            this._mediator = mediator;
        }

        public async Task<DescrizioneCommandResponse> Handle(DescrizioneCommand request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);
            if (string.IsNullOrWhiteSpace(request.Descrizione))
                throw new AggregateDescriptionNotFoundPi3Exception(request.Id);

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true) ?? string.Empty;

            if (!await _aggregazioneDocumentaleRepository.Exists(idTenant, request.Id))
                throw new AggregazioneDocumentaleNotFoundPi3Exception(request.Id);

            var aggregate = await _aggregazioneDocumentaleRepository.Get(idTenant, request.Id);
            aggregate.ChangeDescription(new TextValue(request.Descrizione));

            await _aggregazioneDocumentaleRepository.Update(aggregate);

            return new DescrizioneCommandResponse();
        }
    }
}
