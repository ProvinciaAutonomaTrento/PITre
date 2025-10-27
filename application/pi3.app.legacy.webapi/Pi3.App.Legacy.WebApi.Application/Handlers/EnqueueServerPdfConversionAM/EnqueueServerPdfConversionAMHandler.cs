// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnqueueServerPdfConversionAMRequest = Pi3.App.Legacy.WebApi.Application.Requests.EnqueueServerPdfConversionAM;
using EnqueueServerPdfConversionRequest = Pi3.App.Legacy.WebApi.Application.Requests.EnqueueServerPdfConversion;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.EnqueueServerPdfConversionAM
{
    public class EnqueueServerPdfConversionAMHandler : IRequestHandler<EnqueueServerPdfConversionAMRequest, EnqueueServerPdfConversionAMResult>
    {
        #region Public Members

        public EnqueueServerPdfConversionAMHandler(ILogger<EnqueueServerPdfConversionAMHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<EnqueueServerPdfConversionAMResult> Handle(EnqueueServerPdfConversionAMRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await this._mediator.Send(new EnqueueServerPdfConversionRequest(request.infoUtente, request.objServerPdfConversion));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new EnqueueServerPdfConversionAMResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<EnqueueServerPdfConversionAMHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
