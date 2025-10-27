// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.Carica;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Services.Principal;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.IdDocs.Add
{
    public class AggiungiDocumentoCommandHandler : IRequestHandler<AggiungiDocumentoCommand, AggiungiDocumentoCommandResponse>
    {
        private readonly ILogger<AggiungiDocumentoCommandHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;
        private readonly IMediator _mediator;

        public AggiungiDocumentoCommandHandler(ILogger<AggiungiDocumentoCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository,
            IMediator mediator)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;
            _mediator = mediator;
        }

        public async Task<AggiungiDocumentoCommandResponse> Handle(AggiungiDocumentoCommand request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);
            if (string.IsNullOrWhiteSpace(request.Identiticativo))
                throw new IdDocNotFoundPi3Exception(request.Id);

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            //if (!await _aggre
            //gazioneDocumentaleRepository.Exists(idTenant, request.Id))
            //    throw new AggregazioneDocumentaleNotFoundPi3Exception(request.Id);
            var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.Id });
            request.Id = safeId.Id;

            var aggregate = await _aggregazioneDocumentaleRepository.Get(idTenant, request.Id);

            aggregate.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc()
            {
                Identiticativo = request.Identiticativo,
            });
            await _aggregazioneDocumentaleRepository.Update(aggregate);

            return new AggiungiDocumentoCommandResponse();
        }
    }
}
