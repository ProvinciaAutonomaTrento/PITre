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
using IsEnabledConversionePdfLatoServerSincronaRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsEnabledConversionePdfLatoServerSincrona;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsEnabledConversionePdfLatoServerSincrona
{
    public class IsEnabledConversionePdfLatoServerSincronaHandler : IRequestHandler<IsEnabledConversionePdfLatoServerSincronaRequest, IsEnabledConversionePdfLatoServerSincronaResult>
    {
        #region Public Members

        public IsEnabledConversionePdfLatoServerSincronaHandler(ILogger<IsEnabledConversionePdfLatoServerSincronaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<IsEnabledConversionePdfLatoServerSincronaResult> Handle(IsEnabledConversionePdfLatoServerSincronaRequest request, CancellationToken cancellationToken)
        {
            return new IsEnabledConversionePdfLatoServerSincronaResult(true);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsEnabledConversionePdfLatoServerSincronaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}