// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Exceptions;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.Carica;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.IdAggregatoSafe;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands.Folders.Hierarchy
{
    public class CaricaCartelleCommandHandler : IRequestHandler<CaricaCartelleCommand, CaricaCartelleCommandResponse>
    {
        private readonly ILogger<CaricaAggregazioneQueryHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IAggregazioneDocumentaleRepository _repository;
        private readonly IMediator _mediator;

        public CaricaCartelleCommandHandler(
            ILogger<CaricaAggregazioneQueryHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository,
            IMediator mediator)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _repository = aggregazioneDocumentaleRepository;
            _mediator = mediator;
        }

        public async Task<CaricaCartelleCommandResponse> Handle(CaricaCartelleCommand request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new IdAggregateNotFoundPi3Exception(request.Id);
            if (request.Sottofascicoli == null || !request.Sottofascicoli.Sottofascicoli.Any())
                throw new FolderHierarchyNotFoundPi3Exception(request.Id);

            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            var safeId = await _mediator.Send(new IdAggregatoSafeQuery() { Id = request.Id });
            request.Id = safeId.Id;


            var loadBehavior = new GetAggregatoDocumentaleLoadBehavior()
            {
                LoadFolderHierarchy = true,
                LoadDocuments = true,
            };

            var aggregate = await _repository.Get(idTenant, request.Id, new[] { loadBehavior });

            aggregate.CreateFolderHierarchy(AggregateHelpers.FolderHierarcy2ValueObject(request.Sottofascicoli));

            await _repository.Update(aggregate);

            return new CaricaCartelleCommandResponse();

            //Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.FolderHierarcy
            //    FolderHierarcy2ValueObject(FolderHierarcy element)
            //{
            //    var docs = new List<Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc>();
            //    var folders = new List<Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.FolderHierarcy>();

            //    foreach (var doc in element.Documenti ?? Enumerable.Empty<IdDoc>())
            //    {
            //        docs.Add(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc()
            //        {
            //            Identiticativo = doc.Identiticativo
            //        });
            //    }

            //    foreach (var folder in element.Sottofascicoli ?? Enumerable.Empty<FolderHierarcy>())
            //    {
            //        folders.Add(FolderHierarcy2ValueObject(folder));
            //    }

            //    var result = new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.FolderHierarcy()
            //    {
            //        Name = new TextValue(element.Nome),
            //        IdDocs = docs,
            //        Folders = folders
            //    };
            //    return result;
            //}

        }
    }
}
