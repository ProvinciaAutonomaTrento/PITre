// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.Principal;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetDocumentStream
{
    public class GetDocumentStreamQueryHandler : IRequestHandler<GetDocumentStreamQuery, GetDocumentStreamQueryResults>
    {
        #region Public Members

        public GetDocumentStreamQueryHandler(
            ILogger<GetDocumentStreamQueryHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IDocumentBlobRepository documentBlobRepository)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _documentoAmministrativoRepository = documentoAmministrativoRepository;
            _documentBlobRepository = documentBlobRepository;

            InitializeMapper();
        }

        public async virtual Task<GetDocumentStreamQueryResults> Handle(GetDocumentStreamQuery request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            if (!await _documentoAmministrativoRepository.Exists(idTenant, request.IdDocument))
                throw new DocumentoAmministrativoNotFoundPi3Exception(request.IdDocument);

            var documentAggregate = await _documentoAmministrativoRepository.Get(idTenant, request.IdDocument);

            Pi3.Core.AggregateModels.DocumentAggregate.Entities.DocumentVersion version = null;
            if (!string.IsNullOrWhiteSpace(request.IdVersion))
                version = documentAggregate.Versions.First(v => v.Id.Equals(request.IdVersion, StringComparison.InvariantCultureIgnoreCase));
            else
                version = documentAggregate.CurrentVersion;

            if (version.DocumentBlobRef == null)
                throw new DocumentoAmministrativoPi3Exception(ErrorDescriptions.FileNonAcquisito, ErrorDescriptions.ResourceManager, documentAggregate.Id);

            var idBlob = version.DocumentBlobRef.IdBlob;
            this._logger.LogDebug($"idBlob: {idBlob}");

            if (!await _documentBlobRepository.Exists(idTenant, idBlob))
                throw new DocumentBlobNotFoundPi3Exception(idBlob);

            var documentBlobAggregate = await _documentBlobRepository.Get(idTenant, idBlob);

            return this._mapper.Map<GetDocumentStreamQueryResults>(documentBlobAggregate);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentStreamQueryHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DocumentBlob, GetDocumentStreamQueryResults>();
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
