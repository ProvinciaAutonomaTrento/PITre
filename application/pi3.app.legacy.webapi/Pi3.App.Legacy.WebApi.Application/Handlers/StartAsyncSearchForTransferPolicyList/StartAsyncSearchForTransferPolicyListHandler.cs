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
using StartAsyncSearchForTransferPolicyListRequest = Pi3.App.Legacy.WebApi.Application.Requests.StartAsyncSearchForTransferPolicyList;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.StartAsyncSearchForTransferPolicyList
{
    public class StartAsyncSearchForTransferPolicyListHandler : IRequestHandler<StartAsyncSearchForTransferPolicyListRequest, StartAsyncSearchForTransferPolicyListResult>
    {
        #region Public Members

        public StartAsyncSearchForTransferPolicyListHandler(ILogger<StartAsyncSearchForTransferPolicyListHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<StartAsyncSearchForTransferPolicyListResult> Handle(StartAsyncSearchForTransferPolicyListRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("StartAsyncSearchForTransferPolicyList");
        }

        #endregion

        #region Private Members

        protected readonly ILogger<StartAsyncSearchForTransferPolicyListHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}