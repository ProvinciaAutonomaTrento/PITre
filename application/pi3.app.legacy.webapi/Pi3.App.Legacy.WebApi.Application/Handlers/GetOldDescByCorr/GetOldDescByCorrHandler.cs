// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
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

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetOldDescByCorr
{
    public class GetOldDescByCorrHandler : IRequestHandler<getOldDescByCorr, getOldDescByCorrResult>
    {
        public GetOldDescByCorrHandler(ILogger<GetOldDescByCorrHandler> logger, IPi3DbContext dbContext, IMediator mediator, IClaimsPrincipalService claimsPrincipalService)
        { 
            this._dbContext = dbContext;
            this._logger = logger;
            this._mediator=mediator;
            this._claimsPrincipalService=claimsPrincipalService;
        }

        public async Task<getOldDescByCorrResult> Handle(getOldDescByCorr request, CancellationToken cancellationToken)
        {
            long id = long.Parse(request.systemId);
            return new getOldDescByCorrResult(_dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == id).Select(c=>c.VAR_DESC_CORR_OLD).FirstOrDefault());

        }

        protected IPi3DbContext _dbContext;
        protected IMediator _mediator;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected ILogger<GetOldDescByCorrHandler> _logger;
    }
}
