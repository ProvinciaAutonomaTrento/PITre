// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.DocumentoGetSeDocFascicolato
{

    // Richiede libreria MediatR
    public class DocumentoGetSeDocFascicolatoHandler : IRequestHandler<Application.Requests.DocumentoGetSeDocFascicolato, DocumentoGetSeDocFascicolatoResult>
    {
        #region Public Members

        public DocumentoGetSeDocFascicolatoHandler(ILogger<DocumentoGetSeDocFascicolatoHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<DocumentoGetSeDocFascicolatoResult> Handle(Application.Requests.DocumentoGetSeDocFascicolato request, CancellationToken cancellationToken)
        {
            bool result = false;
            long idDocumento = Convert.ToInt64(request.idDocumento);

            try
            {
                result = _dbContext.ProjectComponentEntities
                    .Any(x => x.LINK == idDocumento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }


            return new DocumentoGetSeDocFascicolatoResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetSeDocFascicolatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
