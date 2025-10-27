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
using DO_GetIdAmmByCodAmmRequest = Pi3.App.Legacy.WebApi.Application.Requests.DO_GetIdAmmByCodAmm;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DO_GetIdAmmByCodAmm
{

    public class DO_GetIdAmmByCodAmmHandler : IRequestHandler<DO_GetIdAmmByCodAmmRequest, DO_GetIdAmmByCodAmmResult>
    {
        #region Public Members

        public DO_GetIdAmmByCodAmmHandler(ILogger<DO_GetIdAmmByCodAmmHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<DO_GetIdAmmByCodAmmResult> Handle(DO_GetIdAmmByCodAmmRequest request, CancellationToken cancellationToken)
        {
            int output = 0;

            var amministraEntity = await this._dbContext.AmministraEntities.Where(a => a.VAR_CODICE_AMM == request.codAmm).Select(a => new { a.SYSTEM_ID }).FirstOrDefaultAsync();
            if (amministraEntity != null)
                output = Convert.ToInt32(amministraEntity.SYSTEM_ID);

            return new DO_GetIdAmmByCodAmmResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DO_GetIdAmmByCodAmmHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
