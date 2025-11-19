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
using getLastDayOfWeekRequest = Pi3.App.Legacy.WebApi.Application.Requests.getLastDayOfWeek;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getLastDayOfWeek
{

    public class getLastDayOfWeekHandler : IRequestHandler<getLastDayOfWeekRequest, getLastDayOfWeekResult>
    {
        #region Public Members

        public getLastDayOfWeekHandler(ILogger<getLastDayOfWeekHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<getLastDayOfWeekResult> Handle(getLastDayOfWeekRequest request, CancellationToken cancellationToken)
        {
            string output = string.Empty;

            var sysdate = await this._dbContext.GetSystemDateTime();

            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = sysdate.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;
            output = (sysdate.AddDays(6 - diff)).AsDateFormat();

            return new getLastDayOfWeekResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getLastDayOfWeekHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
