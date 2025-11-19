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
using getCheckInteropFromSysIdCorrGlobRequest = Pi3.App.Legacy.WebApi.Application.Requests.getCheckInteropFromSysIdCorrGlob;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getCheckInteropFromSysIdCorrGlob
{
    public class GetCheckInteropFromSysIdCorrGlobHandler : IRequestHandler<getCheckInteropFromSysIdCorrGlobRequest, getCheckInteropFromSysIdCorrGlobResult>
    {
        #region Public Members

        public GetCheckInteropFromSysIdCorrGlobHandler(ILogger<GetCheckInteropFromSysIdCorrGlobHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<getCheckInteropFromSysIdCorrGlobResult> Handle(getCheckInteropFromSysIdCorrGlobRequest request, CancellationToken cancellationToken)
        {
            string output = string.Empty;
            try
            {
                long idCorrGlobali = long.Parse(request.systemId);
                output = _dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == idCorrGlobali).Select(c => c.VAR_INSERT_BY_INTEROP).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }
            return new getCheckInteropFromSysIdCorrGlobResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetCheckInteropFromSysIdCorrGlobHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
