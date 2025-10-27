// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
using UpdateNoteNotificationRequest = Pi3.App.Legacy.WebApi.Application.Requests.UpdateNoteNotification;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateNoteNotification
{

    // Richiede libreria MediatR
    public class UpdateNoteNotificationHandler : IRequestHandler<UpdateNoteNotificationRequest, UpdateNoteNotificationResult>
    {
        #region Public Members

        public UpdateNoteNotificationHandler(ILogger<UpdateNoteNotificationHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<UpdateNoteNotificationResult> Handle(UpdateNoteNotificationRequest request, CancellationToken cancellationToken)
        {
            var output = false;

            try
            {
                var idNotifyAsLong = request.idNotify.AsLong();

                var notifyEntity = await this._dbContext.NotifyEntities.Where(n => n.SYSTEM_ID == idNotifyAsLong).FirstAsync();
                notifyEntity.NOTES = request.note;

                await ((DbContext)_dbContext).SaveChangesAsync();

                output = true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new UpdateNoteNotificationResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UpdateNoteNotificationHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
