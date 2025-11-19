// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
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
using getLastDayOfMonthRequest = Pi3.App.Legacy.WebApi.Application.Requests.getLastDayOfMonth;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getLastDayOfMonth
{
    public class getLastDayOfMonthHandler : IRequestHandler<getLastDayOfMonthRequest, getLastDayOfMonthResult>
    {
        #region Public Members

        public getLastDayOfMonthHandler(ILogger<getLastDayOfMonthHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
               IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<getLastDayOfMonthResult> Handle(getLastDayOfMonthRequest request, CancellationToken cancellationToken)
        {
            string output = string.Empty;

            var sysdate = await this._dbContext.GetSystemDateTime();
           
            DateTime d = new DateTime(sysdate.Year, sysdate.Month, 1);
            output =  (d.AddMonths(1).AddDays(-1)).AsDateFormat();

            return new getLastDayOfMonthResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getLastDayOfMonthHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
