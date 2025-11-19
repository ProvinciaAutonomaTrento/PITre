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
using getApplicationNameRequest = Pi3.App.Legacy.WebApi.Application.Requests.getApplicationName;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getApplicationName
{

    public class getApplicationNameHandler : IRequestHandler<getApplicationNameRequest, getApplicationNameResult>
    {
        #region Public Members

        public getApplicationNameHandler(ILogger<getApplicationNameHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getApplicationNameResult> Handle(getApplicationNameRequest request, CancellationToken cancellationToken)
        {
            var output = string.Empty;

            try
            {
                var version = await this._dbContext.DocsPaEntities.OrderByDescending(d => d.SYSTEM_ID).Select(d => d.ID_VERSIONS_U).FirstOrDefaultAsync();
                if (version != null)
                    output = version;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new getApplicationNameResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getApplicationNameHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
