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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmmListaIDParentRicercaRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmListaIDParentRicerca;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmListaIDParentRicerca
{
    public class AmmListaIDParentRicercaHandler : IRequestHandler<AmmListaIDParentRicercaRequest, AmmListaIDParentRicercaResult>
    {
        #region Public Members

        public AmmListaIDParentRicercaHandler(ILogger<AmmListaIDParentRicercaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AmmListaIDParentRicercaResult> Handle(AmmListaIDParentRicercaRequest request, CancellationToken cancellationToken)
        {
            ArrayList output = new ArrayList();
            var idParent = request.IDPartenza.AsLong();
            var tipo = request.tipo;

            output.Add(idParent);

            while(idParent > 0)
            {
                if(tipo.Equals("U") || tipo.Equals("R"))
                {
                    idParent = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idParent).Select(c => c.ID_PARENT ?? 0).FirstOrDefaultAsync();
                }
                else
                {
                    idParent = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idParent).Select(c => c.ID_UO ?? 0).FirstOrDefaultAsync();
                    tipo = "U";
                }

                if(idParent > 0)
                    output.Add(idParent);
            }

            return new AmmListaIDParentRicercaResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmListaIDParentRicercaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
