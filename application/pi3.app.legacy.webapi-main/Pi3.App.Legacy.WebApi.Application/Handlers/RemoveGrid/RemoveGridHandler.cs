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
using RemoveGridRequest = Pi3.App.Legacy.WebApi.Application.Requests.RemoveGrid;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.RemoveGrid
{
    public class RemoveGridHandler : IRequestHandler<RemoveGridRequest, RemoveGridResult>
    {
        #region Private Members
        protected readonly IPi3DbContext _dbContext;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly ILogger<RemoveGridHandler> _logger;
        #endregion


        #region Public Members
        public RemoveGridHandler(
            IPi3DbContext dbContext,
            IClaimsPrincipalService claimsPrincipalService,
            ILogger<RemoveGridHandler> logger
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
        }


        public async Task<RemoveGridResult> Handle( RemoveGridRequest request, CancellationToken cancellationToken )
        {
            bool output = false;

            try
            {
                AssGridsEntity? assGridsEntity = await this._dbContext.AssGridsEntities.AsNoTracking().
                    FirstOrDefaultAsync( grid => grid.GRID_ID == request.gridBase.GridId.AsLong());
                
                if (assGridsEntity != null )
                {
                    this._dbContext.AssGridsEntities.Remove(assGridsEntity);
                    await ((DbContext)this._dbContext).SaveChangesAsync(cancellationToken);
                    output = true;
                }

                GridEntity? gridsEntity = await this._dbContext.GridEntities.AsNoTracking().
                    FirstOrDefaultAsync(grid => grid.SYSTEM_ID == request.gridBase.GridId.AsLong());

                if (gridsEntity != null )
                {
                    this._dbContext.GridEntities.Remove(gridsEntity);
                    await ((DbContext)this._dbContext).SaveChangesAsync(cancellationToken);
                    output = true;
                }

            }
            catch( Exception ex )
            {
                this._logger.LogWebMethodError(ex, "RemoveGrid");
                output = false;
            }

            return new RemoveGridResult(output);
        }
        #endregion
    }
}
