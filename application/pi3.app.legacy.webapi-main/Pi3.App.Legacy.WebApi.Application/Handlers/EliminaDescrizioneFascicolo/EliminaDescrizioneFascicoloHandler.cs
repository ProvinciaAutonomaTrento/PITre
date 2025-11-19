// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.TotalFileSizeDocument;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EliminaDescrizioneFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.EliminaDescrizioneFascicolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.EliminaDescrizioneFascicolo
{
    public class EliminaDescrizioneFascicoloHandler : IRequestHandler<EliminaDescrizioneFascicoloRequest, EliminaDescrizioneFascicoloResult>
    {
        #region Public Members

        public EliminaDescrizioneFascicoloHandler(ILogger<EliminaDescrizioneFascicoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<EliminaDescrizioneFascicoloResult> Handle(EliminaDescrizioneFascicoloRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var systemIdAsLong = request.systemId.AsLong();

                var descrizioneFascicoloEntity = await this._dbContext.DescrizioneFascEntities.FirstOrDefaultAsync(d => d.SYSTEM_ID == systemIdAsLong);

                if(descrizioneFascicoloEntity != null)
                {
                    this._dbContext.DescrizioneFascEntities.Remove(descrizioneFascicoloEntity);
                    await ((DbContext)_dbContext).SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new EliminaDescrizioneFascicoloResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<EliminaDescrizioneFascicoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
