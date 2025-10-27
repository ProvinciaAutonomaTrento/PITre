// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils.Date.GetLastThirtyOneDay
{
    public class GetLastThirtyOneDayCommandHandler : IRequestHandler<GetLastThirtyOneDayCommand, GetLastThirtyOneDayCommandResponse>
    {
        public GetLastThirtyOneDayCommandHandler(ILogger<GetLastThirtyOneDayCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
               IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetLastThirtyOneDayCommandResponse> Handle(GetLastThirtyOneDayCommand request, CancellationToken cancellationToken)
        {
            string output = string.Empty;

            var sysdate = await this._dbContext.GetSystemDateTime();

            output = (sysdate.AddDays(-31)).AsDateFormat();

            return new()
            { 
                output = output
            };
        }

        #region Private Members

        protected readonly ILogger<GetLastThirtyOneDayCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
