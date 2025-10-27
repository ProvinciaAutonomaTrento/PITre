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
using SendFatturaRequest = Pi3.App.Legacy.WebApi.Application.Requests.SendFattura;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SendFattura
{
    public class SendFatturaHandler : IRequestHandler<SendFatturaRequest, SendFatturaResult>
    {
        #region Public Members

        public SendFatturaHandler(ILogger<SendFatturaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<SendFatturaResult> Handle(SendFatturaRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("SendFattura");
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SendFatturaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}