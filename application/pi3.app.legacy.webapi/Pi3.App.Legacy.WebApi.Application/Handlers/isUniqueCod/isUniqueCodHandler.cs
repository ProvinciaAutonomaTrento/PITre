// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isUniqueCodRequest = Pi3.App.Legacy.WebApi.Application.Requests.isUniqueCod;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isUniqueCod
{
    public class isUniqueCodHandler : IRequestHandler<isUniqueCodRequest, isUniqueCodResult>
    {
        #region Public Members

        public isUniqueCodHandler(ILogger<isUniqueCodHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<isUniqueCodResult> Handle(isUniqueCodRequest request, CancellationToken cancellationToken)
        {
            var output = false;
            var codLista = request.codLista.ToUpper();
            var idAmmAsLong = request.idAmm.AsLong();

            try
            {
                output = !await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .AnyAsync(c => c.ID_AMM == idAmmAsLong && c.VAR_COD_RUBRICA.ToUpper() == codLista);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new isUniqueCodResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isUniqueCodHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
