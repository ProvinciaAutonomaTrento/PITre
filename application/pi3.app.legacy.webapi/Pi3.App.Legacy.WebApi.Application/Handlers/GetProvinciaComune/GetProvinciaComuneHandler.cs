// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetProvinciaComuneRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetProvinciaComune;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetProvinciaComune
{
    public class GetProvinciaComuneHandler : IRequestHandler<GetProvinciaComuneRequest,GetProvinciaComuneResult>
    {

        #region Private Members


        protected readonly ILogger<GetProvinciaComuneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        #endregion


        #region Public Members
        public GetProvinciaComuneHandler(
            ILogger<GetProvinciaComuneHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;
            this._dbContext = dbContext;
        }


        public async Task<GetProvinciaComuneResult> Handle(GetProvinciaComuneRequest request, CancellationToken cancellationToken)
        {
            InfoComune cap = new InfoComune();
            try
            {
                this._logger.LogDebug("GetProvinciaComune");

                InfoComuniEntity? infoEnt = await this._dbContext.InfoComuniEntities.AsNoTracking()
                    .Where(comuneEnt => comuneEnt.VAR_COMUNE != null ? comuneEnt.VAR_COMUNE.ToUpper().Equals(request.comune.ToUpper()) : false )
                    .FirstOrDefaultAsync();

                if(infoEnt != null )
                {
                    cap.COMUNE = infoEnt.VAR_COMUNE != null ? infoEnt.VAR_COMUNE : string.Empty;
                    cap.PROVINCIA = infoEnt.VAR_PROVINCIA != null ? infoEnt.VAR_PROVINCIA : string.Empty;
                }
            }
            catch ( Exception ex )
            {
                this._logger.LogWebMethodError(ex, " Errore in GetProvinciaComune");
            }

            return new GetProvinciaComuneResult(cap);
        }

        #endregion
    }
}
