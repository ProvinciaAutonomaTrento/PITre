// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogoffRequest = Pi3.App.Legacy.WebApi.Application.Requests.Logoff;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.Logoff
{
    public class LogoffHandler : IRequestHandler<LogoffRequest, LogoffResult>
    {
        #region Public Members

        public LogoffHandler(ILogger<LogoffHandler> logger, 
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

        public async Task<LogoffResult> Handle(LogoffRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);

                if (string.IsNullOrWhiteSpace(userId))
                    return new LogoffResult(output);

                var queryable = this._dbContext.LoginEntities.AsNoTracking().Where(l => l.USER_ID.ToUpper() == userId.ToUpper());

                if (!string.IsNullOrEmpty(request.idAmm))
                    queryable = queryable.Where(l => l.ID_AMM == request.idAmm.AsLong());

                if (!string.IsNullOrEmpty(request.sessionId))
                    queryable = queryable.Where(l => l.SESSION_ID == request.sessionId);

                var loginEntity = await queryable.FirstOrDefaultAsync();
                if (loginEntity != null)
                    this._dbContext.LoginEntities.Remove(loginEntity);

                loginEntity = await this._dbContext.LoginEntities.FirstOrDefaultAsync(x => x.DST == request.dst);
                if (loginEntity != null)
                    this._dbContext.LoginEntities.Remove(loginEntity);

                await this._webMethodLoggerService.LogOK("LOGOFF");

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("LOGOFF", request.sessionId, null, null);
                output = false;
            }

            return new LogoffResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<LogoffHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}
