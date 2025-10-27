// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
using IsVersionWithSegnatureRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsVersionWithSegnature;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsVersionWithSegnature
{
    public class IsVersionWithSegnatureHandler : IRequestHandler<IsVersionWithSegnatureRequest, IsVersionWithSegnatureResult>
    {
        #region Public Members

        public IsVersionWithSegnatureHandler(ILogger<IsVersionWithSegnatureHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<IsVersionWithSegnatureResult> Handle(IsVersionWithSegnatureRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                if (!string.IsNullOrEmpty(request.versionId))
                {
                    long versionId = Convert.ToInt64(request.versionId);
                    output = await this._dbContext.VersionEntities.AnyAsync(v => v.CHA_SEGNATURA == "1" && v.VERSION_ID == versionId);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new IsVersionWithSegnatureResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsVersionWithSegnatureHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }

}
