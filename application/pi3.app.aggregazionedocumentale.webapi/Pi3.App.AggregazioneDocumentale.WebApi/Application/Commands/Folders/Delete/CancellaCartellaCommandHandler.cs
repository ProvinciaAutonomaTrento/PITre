// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.Carica;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Folders.Delete
{
    public class CancellaCartellaCommandHandler : IRequestHandler<CancellaCartellaCommand, CancellaCartellaCommandResponse>
    {
        private readonly ILogger<CaricaAggregazioneQueryHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;
        private readonly IMediator _mediator;

        public CancellaCartellaCommandHandler(ILogger<CaricaAggregazioneQueryHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository,
            IMediator mediator)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;
            _mediator = mediator;
        }

        public async Task<CancellaCartellaCommandResponse> Handle(CancellaCartellaCommand request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);
            if (string.IsNullOrWhiteSpace(request.IdFolder))
                throw new IdFolderNotFoundPi3Exception(request.Id);

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true) ?? string.Empty;

            var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.Id });
            request.Id = safeId.Id;

            var loadBehavior = new GetAggregatoDocumentaleLoadBehavior()
            {
                LoadFolderHierarchy = true,
            };

            var aggregate = await _aggregazioneDocumentaleRepository.Get(idTenant, request.Id, new[] { loadBehavior });
            aggregate.RemoveFolder(request.IdFolder);

            await _aggregazioneDocumentaleRepository.Update(aggregate);

            return new CancellaCartellaCommandResponse();

        }
    }
}
