// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using DocumentoGetDettaglioDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetDettaglioDocumento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetDocumentListVersions
{
    // Richiede libreria MediatR
    public class GetDocumentListVersionsHandler : IRequestHandler<Application.Requests.GetDocumentListVersions, GetDocumentListVersionsResult>
    {
        #region Public Members

        public GetDocumentListVersionsHandler(ILogger<GetDocumentListVersionsHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetDocumentListVersionsResult> Handle(Application.Requests.GetDocumentListVersions request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.SchedaDocumento schedaDocumento = null;
            long idProfile = Convert.ToInt64(request.idProfile);
            long docNumber = Convert.ToInt64(request.docNumber);

            try
            {
                var getDettaglioDocumento = await this._mediator.Send(new DocumentoGetDettaglioDocumentoRequest(request.infoutente, request.idProfile, request.docNumber));
                schedaDocumento = getDettaglioDocumento.output;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new GetDocumentListVersionsResult(schedaDocumento);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentListVersionsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
