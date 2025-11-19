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
using AmmUpdateMailRegistroRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmUpdateMailRegistro;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmUpdateMailRegistro
{
    public class AmmUpdateMailRegistroHandler : IRequestHandler<AmmUpdateMailRegistroRequest, AmmUpdateMailRegistroResult>
    {
        #region Public Members

        public AmmUpdateMailRegistroHandler(ILogger<AmmUpdateMailRegistroHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<AmmUpdateMailRegistroResult> Handle(AmmUpdateMailRegistroRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("AmmUpdateMailRegistro");
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmUpdateMailRegistroHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}