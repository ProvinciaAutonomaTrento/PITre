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
using isUniqueNomeListaRequest = Pi3.App.Legacy.WebApi.Application.Requests.isUniqueNomeLista;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isUniqueNomeLista
{
    public class isUniqueNomeListaHandler : IRequestHandler<isUniqueNomeListaRequest, isUniqueNomeListaResult>
    {
        #region Public Members

        public isUniqueNomeListaHandler(ILogger<isUniqueNomeListaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<isUniqueNomeListaResult> Handle(isUniqueNomeListaRequest request, CancellationToken cancellationToken)
        {
            var output = false;
            var nomeLista = request.nomeLista.ToUpper();
            var idAmmAsLong = request.idAmm.AsLong();

            try
            {
                output = !await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .AnyAsync(c => c.CHA_TIPO_URP == "L" && c.ID_AMM == idAmmAsLong && c.VAR_DESC_CORR.ToUpper() == nomeLista);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new isUniqueNomeListaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isUniqueNomeListaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }
}
