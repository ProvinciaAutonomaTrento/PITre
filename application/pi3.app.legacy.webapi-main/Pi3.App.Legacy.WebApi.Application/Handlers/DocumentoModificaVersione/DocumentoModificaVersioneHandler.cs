// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoModificaVersione
{

    public class DocumentoModificaVersioneHandler : IRequestHandler<Requests.DocumentoModificaVersione, DocumentoModificaVersioneResult>
    {
        #region Public Members

        public DocumentoModificaVersioneHandler(ILogger<DocumentoModificaVersioneHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, 
            IPi3DbContext dbContext,
            IDistributedCache distributedCache, 
            IConfiguration configuration,
            IDocumentoAmministrativoRepository documentRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
            this._documentRepository = documentRepository;
        }

        public async Task<DocumentoModificaVersioneResult> Handle(Requests.DocumentoModificaVersione request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                var aggregate = await this._documentRepository.Get(idTenant, request.fileReq.docNumber, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = true,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = false
                    }
                }); 
                aggregate.ChangeVersionName(request.fileReq.versionId, new TextValue(request.fileReq.descrizione));

                await this._documentRepository.Update(aggregate);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new DocumentoModificaVersioneResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoModificaVersioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IDocumentoAmministrativoRepository _documentRepository;

        #endregion
    }

}
