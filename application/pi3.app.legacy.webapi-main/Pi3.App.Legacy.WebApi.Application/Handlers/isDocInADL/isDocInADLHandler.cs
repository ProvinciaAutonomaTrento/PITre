// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isDocInADLRequest = Pi3.App.Legacy.WebApi.Application.Requests.isDocInADL;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isDocInADL
{

    public class isDocInADLHandler : IRequestHandler<isDocInADLRequest, isDocInADLResult>
    {
        #region Public Members

        public isDocInADLHandler(ILogger<isDocInADLHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<isDocInADLResult> Handle(isDocInADLRequest request, CancellationToken cancellationToken)
        {
            int output = 0;
            try
            {
                long idProfile = Convert.ToInt64(request.idProfile);
                long idPeople = Convert.ToInt64(request.idPeople);
                long idRole = Convert.ToInt64(request.idRole);

                var areaLavoroEntity = await _dbContext.AreaLavoroEntities.Where(d => d.ID_PROFILE == idProfile && d.ID_PEOPLE == idPeople && d.ID_RUOLO_IN_UO == idRole).AnyAsync();

                if (areaLavoroEntity)
                    output = 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new isDocInADLResult(output);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<isDocInADLHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        #endregion
    }

}
