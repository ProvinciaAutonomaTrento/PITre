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
using isRFEnabledRequest = Pi3.App.Legacy.WebApi.Application.Requests.isRFEnabled;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isRFEnabled
{
    public class isRFEnabledHandler : IRequestHandler<isRFEnabledRequest, isRFEnabledResult>
    {
        #region Public Members

        public isRFEnabledHandler(ILogger<isRFEnabledHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<isRFEnabledResult> Handle(isRFEnabledRequest request, CancellationToken cancellationToken)
        {
            return new isRFEnabledResult(true);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isRFEnabledHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}