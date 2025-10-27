// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils.Date.GetLastDayOfMonth
{
    public class GetLastDayOfMonthCommandHandler : IRequestHandler<GetLastDayOfMonthCommand, GetLastDayOfMonthCommandResponse>
    {
        public GetLastDayOfMonthCommandHandler(ILogger<GetLastDayOfMonthCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
               IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetLastDayOfMonthCommandResponse> Handle(GetLastDayOfMonthCommand request, CancellationToken cancellationToken)
        {
            string output = string.Empty;

            var sysdate = await this._dbContext.GetSystemDateTime();

            DateTime d = new DateTime(sysdate.Year, sysdate.Month, 1);
            output = (d.AddMonths(1).AddDays(-1)).AsDateFormat();

            return new()
            {
                output = output,
            };
        }

        #region Private Members

        protected readonly ILogger<GetLastDayOfMonthCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion

    }
}
