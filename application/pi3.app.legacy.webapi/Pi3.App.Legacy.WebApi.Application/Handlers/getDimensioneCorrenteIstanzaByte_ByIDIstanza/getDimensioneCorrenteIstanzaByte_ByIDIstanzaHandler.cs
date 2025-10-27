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
using getDimensioneCorrenteIstanzaByte_ByIDIstanzaRequest = Pi3.App.Legacy.WebApi.Application.Requests.getDimensioneCorrenteIstanzaByte_ByIDIstanza;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getDimensioneCorrenteIstanzaByte_ByIDIstanza
{

    public class getDimensioneCorrenteIstanzaByte_ByIDIstanzaHandler : IRequestHandler<getDimensioneCorrenteIstanzaByte_ByIDIstanzaRequest, getDimensioneCorrenteIstanzaByte_ByIDIstanzaResult>
    {
        #region Public Members

        public getDimensioneCorrenteIstanzaByte_ByIDIstanzaHandler(ILogger<getDimensioneCorrenteIstanzaByte_ByIDIstanzaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getDimensioneCorrenteIstanzaByte_ByIDIstanzaResult> Handle(getDimensioneCorrenteIstanzaByte_ByIDIstanzaRequest request, CancellationToken cancellationToken)
        {
            var output = 0;

            try
            {
                var idIstanzaAsLong = request.idIstanza.AsLong();

                var sumSize = await this._dbContext.ItemConservazioneEntities.Where(c => c.ID_CONSERVAZIONE == idIstanzaAsLong).SumAsync(c => c.SIZE_ITEM);

                if(sumSize != null) 
                {
                    output = Convert.ToInt32(sumSize);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new getDimensioneCorrenteIstanzaByte_ByIDIstanzaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getDimensioneCorrenteIstanzaByte_ByIDIstanzaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
