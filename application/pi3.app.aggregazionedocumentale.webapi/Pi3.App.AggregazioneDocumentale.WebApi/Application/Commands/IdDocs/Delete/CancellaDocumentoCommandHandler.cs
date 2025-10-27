// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Descrizione;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Services.Principal;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.IdDocs.Delete
{
    public class CancellaDocumentoCommandHandler : IRequestHandler<CancellaDocumentoCommand, CancellaDocumentoCommandResponse>
    {
        private readonly ILogger<DescrizioneCommandHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;
        private readonly IMediator _mediator;

        public CancellaDocumentoCommandHandler(
            ILogger<DescrizioneCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository,
            IMediator mediator)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;
            _mediator = mediator;
        }

        public async Task<CancellaDocumentoCommandResponse> Handle(CancellaDocumentoCommand request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);
            if (string.IsNullOrWhiteSpace(request.Identiticativo))
                throw new IdDocNotFoundPi3Exception(request.Id);

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true) ?? string.Empty;

            var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.Id });
            request.Id = safeId.Id;

            //var loadBehavior = new GetAggregatoDocumentaleLoadBehavior()
            //{
            //    LoadDocuments = true,
            //};

            var aggregate = await _aggregazioneDocumentaleRepository.Get(idTenant, request.Id);
            aggregate.RemoveIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc()
            {
                Identiticativo = request.Identiticativo,
            });


            //var cancellato = false;
            //var aggregate = await _aggregazioneDocumentaleRepository.Get(idTenant, request.Id, new[] { loadBehavior });
            //for (int i = aggregate.IdDocs.Count-1; i >= 0;  i--) {
            //    var idDoc = aggregate.IdDocs[i];
            //    if (idDoc.Identiticativo == request.IdDocumento) { 
            //        aggregate.RemoveIdDoc(idDoc);
            //        cancellato = true;
            //    }
            //}
            //if (!cancellato) {
            //    throw new IdDocNotFoundPi3Exception(request.IdDocumento);
            //}

            await _aggregazioneDocumentaleRepository.Update(aggregate);

            return new CancellaDocumentoCommandResponse();
        }
    }
}
