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
using CheckConnectionRequest = Pi3.App.Legacy.WebApi.Application.Requests.CheckConnection;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckConnection
{
    public class CheckConnectionHandler : IRequestHandler<CheckConnectionRequest, CheckConnectionResult>
    {
        #region Public Members

        public CheckConnectionHandler(ILogger<CheckConnectionHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<CheckConnectionResult> Handle(CheckConnectionRequest request, CancellationToken cancellationToken)
        {
            return new CheckConnectionResult(true, null);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CheckConnectionHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}