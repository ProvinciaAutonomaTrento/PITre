// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isVincoloDimensioneIstanzaViolatoRequest = Pi3.App.Legacy.WebApi.Application.Requests.isVincoloDimensioneIstanzaViolato;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isVincoloDimensioneIstanzaViolato
{
    public class isVincoloDimensioneIstanzaViolatoHandler : IRequestHandler<isVincoloDimensioneIstanzaViolatoRequest, isVincoloDimensioneIstanzaViolatoResult>
    {
        #region Public Members

        public isVincoloDimensioneIstanzaViolatoHandler(ILogger<isVincoloDimensioneIstanzaViolatoHandler> logger
            , IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<isVincoloDimensioneIstanzaViolatoResult> Handle(isVincoloDimensioneIstanzaViolatoRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                double DimensioneMassimaConsentitaPerIstanza = request.vincoloDimensioneInIStanza - ((request.vincoloDimensioneInIStanza * request.percentualeTolleranza) / 100);

                int DimMaxConsentita = Convert.ToInt32(DimensioneMassimaConsentitaPerIstanza);

                output = request.DimensioneInIstanza > DimMaxConsentita;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = false;
            }

            return new isVincoloDimensioneIstanzaViolatoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isVincoloDimensioneIstanzaViolatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}
