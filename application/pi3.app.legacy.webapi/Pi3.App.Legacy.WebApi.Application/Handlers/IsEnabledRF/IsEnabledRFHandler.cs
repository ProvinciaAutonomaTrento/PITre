// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsEnabledRFRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsEnabledRF;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsEnabledRF
{
    public class IsEnabledRFHandler : IRequestHandler<IsEnabledRFRequest, IsEnabledRFResult>
    {
        #region Public Members

        public IsEnabledRFHandler(ILogger<IsEnabledRFHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
        }

        public async Task<IsEnabledRFResult> Handle(IsEnabledRFRequest request, CancellationToken cancellationToken)
        {
            //In PiTre la chiave ENABLE_RF è attiva per tutti
            return new IsEnabledRFResult(true);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsEnabledRFHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }

}
