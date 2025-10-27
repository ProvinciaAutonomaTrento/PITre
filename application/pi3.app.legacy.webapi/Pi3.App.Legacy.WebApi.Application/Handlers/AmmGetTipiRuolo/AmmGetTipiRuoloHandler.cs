// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmmGetTipiRuoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetTipiRuolo;
using AmmGetListTipiRuoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetListTipiRuolo;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetTipiRuolo
{
    public class AmmGetTipiRuoloHandler : IRequestHandler<AmmGetTipiRuoloRequest, AmmGetTipiRuoloResult>
    {
        #region Public Members

        public AmmGetTipiRuoloHandler(ILogger<AmmGetTipiRuoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDistributedCache distributedCache)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
        }

        public async Task<AmmGetTipiRuoloResult> Handle(AmmGetTipiRuoloRequest request, CancellationToken cancellationToken)
        {
            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);
            var codiceAmm = request.codiceAmministrazione.ToUpper();

            var amministraEntities = await this._distributedCache.FromCache(instance!, this._dbContext.AmministraEntities);
            var idTenant = amministraEntities.First(a => a.VAR_CODICE_AMM.ToUpper().Equals(codiceAmm)).SYSTEM_ID;

            var getListTipiRuolo = await this._mediator.Send(new AmmGetListTipiRuoloRequest(idTenant.ToString()));

            return new AmmGetTipiRuoloResult(getListTipiRuolo.output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetTipiRuoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;

        #endregion
    }

}
