// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isFascInADLRoleRequest = Pi3.App.Legacy.WebApi.Application.Requests.isFascInADLRole;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isFascInADLRole
{
    public class isFascInADLRoleHandler : IRequestHandler<isFascInADLRoleRequest, isFascInADLRoleResult>
    {
        #region Public Members

        public isFascInADLRoleHandler(ILogger<isFascInADLRoleHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<isFascInADLRoleResult> Handle(isFascInADLRoleRequest request, CancellationToken cancellationToken)
        {
            int output = 0;

            try
            {
                long idProject = request.idProject.AsLong();
                long idRole = request.idRole.AsLong();

                var exists = await _dbContext.AreaLavoroEntities.AnyAsync(a => a.ID_PROJECT == idProject && a.ID_RUOLO_IN_UO == idRole && a.ID_PEOPLE == 0);

                if (exists)
                    output = 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new isFascInADLRoleResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isFascInADLRoleHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
