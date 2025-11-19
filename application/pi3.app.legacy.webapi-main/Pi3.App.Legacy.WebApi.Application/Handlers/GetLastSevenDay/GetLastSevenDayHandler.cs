// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetLastSevenDayRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetLastSevenDay;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetLastSevenDay
{
    public class GetLastSevenDayHandler : IRequestHandler<GetLastSevenDayRequest, GetLastSevenDayResult>
    {
        #region Public Members

        public GetLastSevenDayHandler(ILogger<GetLastSevenDayHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetLastSevenDayResult> Handle(GetLastSevenDayRequest request, CancellationToken cancellationToken)
        {
            string output = string.Empty;

            var sysdate = await this._dbContext.GetSystemDateTime();

            output = (sysdate.AddDays(-7)).AsDateFormat();

            return new GetLastSevenDayResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetLastSevenDayHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
