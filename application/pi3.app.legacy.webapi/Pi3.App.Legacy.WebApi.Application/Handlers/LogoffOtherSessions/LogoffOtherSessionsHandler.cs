// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
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
using LogoffOtherSessionsRequest = Pi3.App.Legacy.WebApi.Application.Requests.LogoffOtherSessions;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.LogoffOtherSessions
{
    public class LogoffOtherSessionsHandler : IRequestHandler<LogoffOtherSessionsRequest, LogoffOtherSessionsResult>
    {
        #region Public Members

        public LogoffOtherSessionsHandler(ILogger<LogoffOtherSessionsHandler> logger, 
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

        public async Task<LogoffOtherSessionsResult> Handle(LogoffOtherSessionsRequest request, CancellationToken cancellationToken)
        {
            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var output = true;
            try
            {
                var queryable = this._dbContext.LoginEntities.AsNoTracking().Where(x => x.ID_AMM == idTenantAsLong && x.USER_ID.ToUpper() == request.userId.ToUpper());
                if (!string.IsNullOrEmpty(request.sessionId))
                    queryable = queryable.Where(l => l.SESSION_ID != request.sessionId);

                var loginEntity = await queryable.FirstOrDefaultAsync();
                if (loginEntity != null)
                    this._dbContext.LoginEntities.Remove(loginEntity);

                await this._webMethodLoggerService.LogOK("LOGOFF");

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO(null, request.sessionId, null, null);
                output = false;
            }

            return new LogoffOtherSessionsResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<LogoffOtherSessionsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}
