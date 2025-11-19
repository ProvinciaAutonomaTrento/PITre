// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Descrizione;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Folders;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.IdDocs.Folders.Add
{
    public class AggiungiDocumentoFascicoloCommandHandler : IRequestHandler<AggiungiDocumentoFascicoloCommand, AggiungiDocumentoFascicoloCommandResponse>
    {
        private readonly ILogger<DescrizioneCommandHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;
        private readonly IMediator _mediator;

        public AggiungiDocumentoFascicoloCommandHandler(
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

        public async Task<AggiungiDocumentoFascicoloCommandResponse> Handle(AggiungiDocumentoFascicoloCommand request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);
            if (string.IsNullOrWhiteSpace(request.Identiticativo))
                throw new IdDocNotFoundPi3Exception(request.Id);
            if (string.IsNullOrWhiteSpace(request.IdFolder))
                throw new IdFolderNotFoundPi3Exception(request.Id);

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.Id });
            request.Id = safeId.Id;

            var loadBehavior = new GetAggregatoDocumentaleLoadBehavior()
            {
                LoadFolderHierarchy = true,
                LoadDocuments = true,
            };

            var aggregate = await _aggregazioneDocumentaleRepository.Get(idTenant, request.Id, new[] { loadBehavior });

            aggregate.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc()
            {
                Identiticativo = request.Identiticativo,
            }, request.IdFolder);

            await _aggregazioneDocumentaleRepository.Update(aggregate);

            return new AggiungiDocumentoFascicoloCommandResponse();
        }
    }
}
