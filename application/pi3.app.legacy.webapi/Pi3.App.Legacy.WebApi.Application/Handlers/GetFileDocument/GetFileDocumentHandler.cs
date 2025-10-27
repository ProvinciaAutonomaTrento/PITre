// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetFileDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetFileDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetFileDocument
{
    public class GetFileDocumentHandler : IRequestHandler<GetFileDocumentRequest, GetFileDocumentResult>
    {
        #region Public Members

        public GetFileDocumentHandler(ILogger<GetFileDocumentHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            ISessionRepositoryService sessionRepositoryService,
            IDocumentBlobRepository documentBlobRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._sessionRepositoryService = sessionRepositoryService;
            this._documentBlobRepository = documentBlobRepository;

        }

        public async Task<GetFileDocumentResult> Handle(GetFileDocumentRequest request, CancellationToken cancellationToken)
        {
            FileDocumento fileDocumento = new FileDocumento();
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            if (request.request.repositoryContext != null)
            {
                fileDocumento = await _sessionRepositoryService.GetFile(request.request.repositoryContext, request.request);
            }
            else
            {
                fileDocumento = (await this._mediator.Send(new Application.Requests.DocumentoGetInfoFile(request.request, request.infoUtente))).output;

                var path = fileDocumento.path;
                if (string.IsNullOrEmpty(path))
                    path = await GetFileName(request.request.versionId.AsLong());

                DocumentBlob blob = await this._documentBlobRepository.Get(idTenant, path);
                using (var memoryStream = new MemoryStream())
                {
                    blob.Stream.CopyTo(memoryStream);
                    fileDocumento.content = memoryStream.ToArray();
                }
            }

            return new GetFileDocumentResult(fileDocumento);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetFileDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly ISessionRepositoryService _sessionRepositoryService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected const int _defaultBufferSize = 1048576;

        private async Task<string> GetFileName(long versionId)
        {
            return await this._dbContext.ComponentEntities.Where(x => x.VERSION_ID == versionId).Select(x => x.PATH).FirstOrDefaultAsync() ?? string.Empty;
        }

        #endregion
    }
}
