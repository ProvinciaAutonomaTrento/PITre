// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isDocInADLRoleRequest = Pi3.App.Legacy.WebApi.Application.Requests.isDocInADLRole;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isDocInADLRole
{
    public class isDocInADLRoleHandler : IRequestHandler<isDocInADLRoleRequest, isDocInADLRoleResult>
    {
        #region Public Members

        public isDocInADLRoleHandler(ILogger<isDocInADLRoleHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<isDocInADLRoleResult> Handle(isDocInADLRoleRequest request, CancellationToken cancellationToken)
        {
            int output = 0;

            try
            {
                long idProfile = Convert.ToInt64(request.idProfile);
                long idRole = Convert.ToInt64(request.idRole);

                var exists = await _dbContext.AreaLavoroEntities.Where(a => a.ID_PROFILE == idProfile && a.ID_RUOLO_IN_UO == idRole && a.ID_PEOPLE == 0).AnyAsync();

                if (exists)
                    output = 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new isDocInADLRoleResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isDocInADLRoleHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
