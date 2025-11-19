// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.Carica;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Apri
{
    public class ChiudiAggregazioneCommandHandler : IRequestHandler<ChiudiAggregazioneCommand, ChiudiAggregazioneCommandResponse>
    {
        private readonly ILogger<CaricaAggregazioneQueryHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;
        private readonly IMediator _mediator;

        public ChiudiAggregazioneCommandHandler(
            ILogger<CaricaAggregazioneQueryHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository,
            IMediator mediator) {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;
            this._mediator = mediator;
        }

        public async Task<ChiudiAggregazioneCommandResponse> Handle(ChiudiAggregazioneCommand request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            //if (!await _aggregazioneDocumentaleRepository.Exists(idTenant, request.Id))
            //    throw new AggregazioneDocumentaleNotFoundPi3Exception(request.Id);
            var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.Id });
            request.Id = safeId.Id;

            var aggregate = await _aggregazioneDocumentaleRepository.Get(idTenant, request.Id);
            aggregate.Chiudi();

            await _aggregazioneDocumentaleRepository.Update(aggregate);

            return new ChiudiAggregazioneCommandResponse();
        }
    }
}
