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
using getIdAmmByCodRequest = Pi3.App.Legacy.WebApi.Application.Requests.getIdAmmByCod;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getIdAmmByCod
{
    public class getIdAmmByCodHandler : IRequestHandler<getIdAmmByCodRequest, getIdAmmByCodResult>
    {
        #region Public Members

        public getIdAmmByCodHandler(ILogger<getIdAmmByCodHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getIdAmmByCodResult> Handle(getIdAmmByCodRequest request, CancellationToken cancellationToken)
        {
            var output = await this._dbContext.AmministraEntities.AsNoTracking()
                .Where(a => a.VAR_CODICE_AMM.ToUpper() == request.codiceAmministrazione.ToUpper())
                .Select(a => a.SYSTEM_ID)
                .FirstOrDefaultAsync();

            return new getIdAmmByCodResult(output.ToString());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getIdAmmByCodHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
