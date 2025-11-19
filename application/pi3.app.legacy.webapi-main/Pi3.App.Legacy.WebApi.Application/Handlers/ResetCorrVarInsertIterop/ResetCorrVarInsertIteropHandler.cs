// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ResetCorrVarInsertIterop
{

    public class ResetCorrVarInsertIteropHandler : IRequestHandler<Application.Requests.ResetCorrVarInsertIterop,ResetCorrVarInsertIteropResult>
    {
        #region Public Members

        public ResetCorrVarInsertIteropHandler(ILogger<ResetCorrVarInsertIteropHandler> logger, IPi3DbContext dbContext, IMediator mediator,
            IClaimsPrincipalService claimsPrincipalService)
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<ResetCorrVarInsertIteropResult> Handle(Application.Requests.ResetCorrVarInsertIterop request, CancellationToken cancellationToken)
        {
            int rowAffected = 0;

            try
            {
                long idCorr = request.corrSystemId.AsLong();

                var toUpdate = _dbContext.CorrGlobaliEntities.SingleOrDefault(c => c.SYSTEM_ID == idCorr);

                if (toUpdate == null)
                    throw new CorrGlobaliNotFoundPi3Exception(request.corrSystemId);

                toUpdate.VAR_INSERT_BY_INTEROP = null;

                rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new ResetCorrVarInsertIteropResult(rowAffected);            
        }



        #endregion

        #region Private Members
        protected readonly ILogger<ResetCorrVarInsertIteropHandler> _logger; 
        protected readonly IPi3DbContext _dbContext; 
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        #endregion
    }

}
