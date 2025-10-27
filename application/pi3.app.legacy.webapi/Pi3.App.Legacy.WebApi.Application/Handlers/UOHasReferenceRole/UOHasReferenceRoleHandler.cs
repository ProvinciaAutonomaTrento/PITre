// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.UOHasReferenceRole
{
    // Richiede libreria MediatR
    public class UOHasReferenceRoleHandler : IRequestHandler<Application.Requests.UOHasReferenceRole, UOHasReferenceRoleResult>
    {
        #region Public Members

        public UOHasReferenceRoleHandler(ILogger<UOHasReferenceRoleHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<UOHasReferenceRoleResult> Handle(Application.Requests.UOHasReferenceRole request, CancellationToken cancellationToken)
        {
            bool result = false;
            long idUo = Convert.ToInt64(request.idUO);

            try
            {
                result = _dbContext.CorrGlobaliEntities
                    .Any(x => x.CHA_TIPO_URP.Equals("R") && x.CHA_RIFERIMENTO.Equals("1") && x.ID_UO == idUo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new UOHasReferenceRoleResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UOHasReferenceRoleHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
