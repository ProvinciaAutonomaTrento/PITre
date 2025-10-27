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
using Do_GetVarDescAmmByIdAmmRequest = Pi3.App.Legacy.WebApi.Application.Requests.Do_GetVarDescAmmByIdAmm;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.Do_GetVarDescAmmByIdAmm
{
    public class Do_GetVarDescAmmByIdAmmHandler : IRequestHandler<Do_GetVarDescAmmByIdAmmRequest, Do_GetVarDescAmmByIdAmmResult>
    {
        #region Public Members

        public Do_GetVarDescAmmByIdAmmHandler(ILogger<Do_GetVarDescAmmByIdAmmHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<Do_GetVarDescAmmByIdAmmResult> Handle(Do_GetVarDescAmmByIdAmmRequest request, CancellationToken cancellationToken)
        {
            var idAmmAsLong = Convert.ToInt64(request.idAmm);

            var output = await this._dbContext.AmministraEntities.AsNoTracking().Where(a => a.SYSTEM_ID == idAmmAsLong).Select(a => a.VAR_DESC_AMM).FirstAsync();

            return new Do_GetVarDescAmmByIdAmmResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<Do_GetVarDescAmmByIdAmmHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
