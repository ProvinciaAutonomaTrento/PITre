// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RipristinaACLWithTypeRequest = Pi3.App.Legacy.WebApi.Application.Requests.RipristinaACLWithType;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RipristinaACLWithType
{
    public class RipristinaACLWithTypeHandler : IRequestHandler<RipristinaACLWithTypeRequest, RipristinaACLWithTypeResult>
    {
        #region Public Members

        public RipristinaACLWithTypeHandler(ILogger<RipristinaACLWithTypeHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<RipristinaACLWithTypeResult> Handle(RipristinaACLWithTypeRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            try
            {
                output = (await this._mediator.Send(new Requests.RipristinaACL(request.docDiritto, request.personOrGroup, request.infoUtente, request.typeObject))).output;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new RipristinaACLWithTypeResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RipristinaACLWithTypeHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}
