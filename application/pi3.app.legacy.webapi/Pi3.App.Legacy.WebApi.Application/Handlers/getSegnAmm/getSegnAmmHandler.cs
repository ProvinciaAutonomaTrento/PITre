// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
using getSegnAmmRequest = Pi3.App.Legacy.WebApi.Application.Requests.getSegnAmm;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getSegnAmm
{
    public class getSegnAmmHandler : IRequestHandler<getSegnAmmRequest, getSegnAmmResult>
    {
        #region Public Members

        public getSegnAmmHandler(ILogger<getSegnAmmHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getSegnAmmResult> Handle(getSegnAmmRequest request, CancellationToken cancellationToken)
        {
            var idTenant = request.idAmm.AsLong();
            var output = string.Empty;

            try
            {
                var colSegn = await this._dbContext.AmministraEntities.AsNoTracking().Where(a => a.SYSTEM_ID == idTenant).Select(a => a.COL_SEGN).FirstAsync();

                if (!string.IsNullOrEmpty(colSegn) && Convert.ToInt16(colSegn) > 0)
                    output = colSegn;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new getSegnAmmResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getSegnAmmHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
