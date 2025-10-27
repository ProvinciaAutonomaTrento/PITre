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
using GetFatturaXMLRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetFatturaXML;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetFatturaXML
{
    public class GetFatturaXMLHandler : IRequestHandler<GetFatturaXMLRequest, GetFatturaXMLResult>
    {
        #region Public Members

        public GetFatturaXMLHandler(ILogger<GetFatturaXMLHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<GetFatturaXMLResult> Handle(GetFatturaXMLRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("GetFatturaXML");
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetFatturaXMLHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}