// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getNumeroDocIstanza_ByIDIstanzaRequest = Pi3.App.Legacy.WebApi.Application.Requests.getNumeroDocIstanza_ByIDIstanza;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getNumeroDocIstanza_ByIDIstanza
{
    public class getNumeroDocIstanza_ByIDIstanzaHandler : IRequestHandler<getNumeroDocIstanza_ByIDIstanzaRequest, getNumeroDocIstanza_ByIDIstanzaResult>
    {
        #region Public Members

        public getNumeroDocIstanza_ByIDIstanzaHandler(ILogger<getNumeroDocIstanza_ByIDIstanzaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getNumeroDocIstanza_ByIDIstanzaResult> Handle(getNumeroDocIstanza_ByIDIstanzaRequest request, CancellationToken cancellationToken)
        {
            var output = 0;

            try
            {
                var idIstanzaAsLong = request.idIstanza.AsLong();

                output = await this._dbContext.ItemConservazioneEntities.CountAsync(c => c.ID_CONSERVAZIONE == idIstanzaAsLong);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new getNumeroDocIstanza_ByIDIstanzaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getNumeroDocIstanza_ByIDIstanzaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
