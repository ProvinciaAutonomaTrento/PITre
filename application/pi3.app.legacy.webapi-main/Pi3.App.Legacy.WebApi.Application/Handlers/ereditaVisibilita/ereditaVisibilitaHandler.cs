// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using ereditaVisibilitaRequest = Pi3.App.Legacy.WebApi.Application.Requests.ereditaVisibilita;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ereditaVisibilita
{
    public class ereditaVisibilitaHandler : IRequestHandler<ereditaVisibilitaRequest, ereditaVisibilitaResult>
    {
        #region Public Members

        public ereditaVisibilitaHandler(ILogger<ereditaVisibilitaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IDistributedCache distributedCache,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            _distributedCache = distributedCache;
            _dbContext = dbContext;
        }

        public async Task<ereditaVisibilitaResult> Handle(ereditaVisibilitaRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            List<long?> idRagioni = new List<long?>();
            var idAmmAsLong = request.idAmm.AsLong();

            var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);

            var amministraEntities = await this._distributedCache.FromCache(instance!, this._dbContext.AmministraEntities);

            var amministraEntity = amministraEntities.Where(a => a.SYSTEM_ID == idAmmAsLong).First();    
            if (amministraEntity.TRASMISSIONE_AUTO_DOC == "1")
                idRagioni.AddRange( new List<long?>{ amministraEntity.ID_RAGIONE_CC, amministraEntity.ID_RAGIONE_COMPETENZA, amministraEntity.ID_RAGIONE_CONOSCENZA, amministraEntity.ID_RAGIONE_REFERENTE, amministraEntity.ID_RAGIONE_TO});

            if(!string.IsNullOrEmpty(request.idModello) && !request.idModello.Equals("null"))
            {
                var idModelloAsLong = request.idModello.AsLong();
                var idRagioneModello = await this._dbContext.ModelloMittDestEntities.Where(m => m.ID_MODELLO == idAmmAsLong).Select(r => r.ID_RAGIONE).FirstAsync();
                idRagioni.Add(idRagioneModello);
            }

            if (await this._dbContext.RagioneTrasmissioneEntities.AnyAsync(r => r.ID_AMM == idAmmAsLong && r.CHA_EREDITA == "1" && idRagioni.Contains(r.SYSTEM_ID)))
                output = true;

            return new ereditaVisibilitaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ereditaVisibilitaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
