// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNoticeDaysNotificationRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetNoticeDaysNotification;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetNoticeDaysNotification
{
    public class GetNoticeDaysNotificationHandler : IRequestHandler<GetNoticeDaysNotificationRequest, GetNoticeDaysNotificationResult>
    {
        #region Public Members

        public GetNoticeDaysNotificationHandler(ILogger<GetNoticeDaysNotificationHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetNoticeDaysNotificationResult> Handle(GetNoticeDaysNotificationRequest request, CancellationToken cancellationToken)
        {
            var output = string.Empty;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

                var amministraEntity = await this._dbContext.AmministraEntities.AsNoTracking().Where(a => a.SYSTEM_ID == idTenant).Select(a => new
                    {
                        a.CHA_ATTIVA_GG_PERM_TODOLIST,
                        a.NUM_GG_PERM_TODOLIST
                    })
                    .FirstAsync();
                if (amministraEntity.CHA_ATTIVA_GG_PERM_TODOLIST == "1")
                    output = amministraEntity.NUM_GG_PERM_TODOLIST?.ToString();
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new GetNoticeDaysNotificationResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetNoticeDaysNotificationHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
