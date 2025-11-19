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
using isEnableIndiceSistematicoRequest = Pi3.App.Legacy.WebApi.Application.Requests.isEnableIndiceSistematico;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isEnableIndiceSistematico
{
    public class isEnableIndiceSistematicoHandler : IRequestHandler<isEnableIndiceSistematicoRequest, isEnableIndiceSistematicoResult>
    {
        #region Public Members

        public isEnableIndiceSistematicoHandler(ILogger<isEnableIndiceSistematicoHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<isEnableIndiceSistematicoResult> Handle(isEnableIndiceSistematicoRequest request, CancellationToken cancellationToken)
        {
            //La chiave INDICE_SISTEMATICO è sempre attiva per tutti gli enti del PiTre
            return new isEnableIndiceSistematicoResult(true);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isEnableIndiceSistematicoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}
