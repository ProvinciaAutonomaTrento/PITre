// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils.Date.GetFirstDayOfWeek
{
    public class GetFirstDayOfWeekCommandHandler : IRequestHandler<GetFirstDayOfWeekCommand, GetFirstDayOfWeekCommandResponse>
    {
        public GetFirstDayOfWeekCommandHandler(ILogger<GetFirstDayOfWeekCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetFirstDayOfWeekCommandResponse> Handle(GetFirstDayOfWeekCommand request, CancellationToken cancellationToken)
        {
            string output = string.Empty;

            var sysdate = await this._dbContext.GetSystemDateTime();

            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = sysdate.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;
            output = (sysdate.AddDays(-diff)).AsDateFormat();

            return new()
            {
                output = output
            };
        }
        #region Private Members

        protected readonly ILogger<GetFirstDayOfWeekCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
