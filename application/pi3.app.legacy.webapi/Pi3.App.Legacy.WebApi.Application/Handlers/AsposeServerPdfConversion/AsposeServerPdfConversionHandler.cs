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
using AsposeServerPdfConversionRequest = Pi3.App.Legacy.WebApi.Application.Requests.AsposeServerPdfConversion;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AsposeServerPdfConversion
{
    public class AsposeServerPdfConversionHandler : IRequestHandler<AsposeServerPdfConversionRequest, AsposeServerPdfConversionResult>
    {
        #region Public Members

        public AsposeServerPdfConversionHandler(ILogger<AsposeServerPdfConversionHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<AsposeServerPdfConversionResult> Handle(AsposeServerPdfConversionRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("AsposeServerPdfConversion");
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AsposeServerPdfConversionHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}