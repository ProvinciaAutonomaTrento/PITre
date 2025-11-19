// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isVincoloNumeroDocumentiIstanzaViolatoRequest = Pi3.App.Legacy.WebApi.Application.Requests.isVincoloNumeroDocumentiIstanzaViolato;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isVincoloNumeroDocumentiIstanzaViolato
{
    public class isVincoloNumeroDocumentiIstanzaViolatoHandler : IRequestHandler<isVincoloNumeroDocumentiIstanzaViolatoRequest, isVincoloNumeroDocumentiIstanzaViolatoResult>
    {
        #region Public Members

        public isVincoloNumeroDocumentiIstanzaViolatoHandler(ILogger<isVincoloNumeroDocumentiIstanzaViolatoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<isVincoloNumeroDocumentiIstanzaViolatoResult> Handle(isVincoloNumeroDocumentiIstanzaViolatoRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                output = request.numDocInIstanza > request.vincoloNumDocInStanza;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = false;
            }

            return new isVincoloNumeroDocumentiIstanzaViolatoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isVincoloNumeroDocumentiIstanzaViolatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}
