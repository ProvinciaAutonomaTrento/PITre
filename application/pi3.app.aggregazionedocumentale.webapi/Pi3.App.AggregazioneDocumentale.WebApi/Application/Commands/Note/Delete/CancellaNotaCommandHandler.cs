// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.Services.Principal;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Note.Delete
{
    public class CancellaNotaCommandHandler : IRequestHandler<CancellaNotaCommand, CancellaNotaCommandResponse>
    {
        private readonly ILogger<CancellaNotaCommandHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly INotaRepository _notaRepository;
        private readonly IMediator _mediator;

        public CancellaNotaCommandHandler(
            ILogger<CancellaNotaCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            INotaRepository notaRepository,
            IMediator mediator
            )
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _notaRepository = notaRepository;
            _mediator = mediator;
        }

        public async Task<CancellaNotaCommandResponse> Handle(CancellaNotaCommand request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.Id });
            request.Id = safeId.Id;

            var nota = await _notaRepository.Get(idTenant, request.IdNota);
            await _notaRepository.Delete(nota);

            return new CancellaNotaCommandResponse();
        }
    }
}
