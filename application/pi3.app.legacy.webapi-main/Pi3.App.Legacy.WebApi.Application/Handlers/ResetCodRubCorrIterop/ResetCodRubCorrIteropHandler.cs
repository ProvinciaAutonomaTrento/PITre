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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ResetCodRubCorrIterop
{

    public class ResetCodRubCorrIteropHandler : IRequestHandler<Application.Requests.ResetCodRubCorrIterop, ResetCodRubCorrIteropResult>
    {
        #region Public Members

        public ResetCodRubCorrIteropHandler(ILogger<ResetCodRubCorrIteropHandler> logger, IPi3DbContext dbContext, IPi3DbContextEntities dbContextEntities,
            IMediator mediator, IClaimsPrincipalService claimsPrincipalService)
        {
            _logger = logger;
            _mediator = mediator;
            _claimsPrincipalService = claimsPrincipalService;
            _dbContext = dbContext;
            _dbContextEntities = dbContextEntities;
        }

        public async Task<ResetCodRubCorrIteropResult> Handle(Application.Requests.ResetCodRubCorrIterop request, CancellationToken cancellationToken)
        {
            long idCorr = long.Parse(request.corrSystemId);

            var toUpdate = _dbContext.CorrGlobaliEntities.SingleOrDefault(c => c.SYSTEM_ID == idCorr);

            if (toUpdate == null)
                throw new CorrGlobaliNotFoundPi3Exception(request.corrSystemId);

            toUpdate.VAR_COD_RUBRICA = request.val;
            toUpdate.VAR_CODICE = request.val;

            int rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync(cancellationToken);

            return new ResetCodRubCorrIteropResult(rowAffected);
        }

        #endregion

        #region Private Members
        protected readonly ILogger<ResetCodRubCorrIteropHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IPi3DbContextEntities _dbContextEntities;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;

        #endregion
    }

}
