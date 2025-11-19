// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsFascicoloGeneraleRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsFascicoloGenerale;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsFascicoloGenerale
{
    public class IsFascicoloGeneraleHandler : IRequestHandler<IsFascicoloGeneraleRequest, IsFascicoloGeneraleResult>
    {
        protected readonly ILogger<IsFascicoloGeneraleHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        public IsFascicoloGeneraleHandler(
            ILogger<IsFascicoloGeneraleHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext

            )
        {
            this._dbContext = dbContext;
            this._claimsPrincipalService = claimsPrincipalService;      
            this._mediator = mediator;
            this._logger = logger;
        }

        public async Task<IsFascicoloGeneraleResult> Handle(IsFascicoloGeneraleRequest request, CancellationToken cancellation)
        {
            bool retVal = false;

            try
            {
                string? field = await this._dbContext.ProjectEntities.AsNoTracking().
                    Where(project => project.SYSTEM_ID == request.idFascicolo.AsLong()).
                    Select( project => project.CHA_TIPO_FASCICOLO).
                    FirstOrDefaultAsync();

                if(field != null )
                {
                    retVal = field.Equals("G");
                }

            }catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex, "IsFascicoloGenerale");
            }

            return new IsFascicoloGeneraleResult(retVal);
        }

    }
}
