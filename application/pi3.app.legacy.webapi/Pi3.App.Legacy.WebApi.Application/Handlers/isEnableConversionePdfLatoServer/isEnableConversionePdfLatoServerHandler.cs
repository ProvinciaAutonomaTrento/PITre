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
using isEnableConversionePdfLatoServerRequest = Pi3.App.Legacy.WebApi.Application.Requests.isEnableConversionePdfLatoServer;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isEnableConversionePdfLatoServer
{
    public class isEnableConversionePdfLatoServerHandler : IRequestHandler<isEnableConversionePdfLatoServerRequest, isEnableConversionePdfLatoServerResult>
    {
        #region Public Members

        public isEnableConversionePdfLatoServerHandler(ILogger<isEnableConversionePdfLatoServerHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
        }

        public async Task<isEnableConversionePdfLatoServerResult> Handle(isEnableConversionePdfLatoServerRequest request, CancellationToken cancellationToken)
        {
            //La chiave CONVERSIONE_PDF_LATO_SERVER è sempre attiva per tutti gli enti del PiTre
            return new isEnableConversionePdfLatoServerResult(true);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isEnableConversionePdfLatoServerHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }

}
