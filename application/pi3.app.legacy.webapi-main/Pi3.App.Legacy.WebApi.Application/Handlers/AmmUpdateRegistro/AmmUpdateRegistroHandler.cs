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
using AmmUpdateRegistroRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmUpdateRegistro;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmUpdateRegistro
{
    public class AmmUpdateRegistroHandler : IRequestHandler<AmmUpdateRegistroRequest, AmmUpdateRegistroResult>
    {
        #region Public Members

        public AmmUpdateRegistroHandler(ILogger<AmmUpdateRegistroHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<AmmUpdateRegistroResult> Handle(AmmUpdateRegistroRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("AmmUpdateRegistro");
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmUpdateRegistroHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}