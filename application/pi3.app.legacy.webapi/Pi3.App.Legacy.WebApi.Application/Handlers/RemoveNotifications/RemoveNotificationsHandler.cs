// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Notification;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckNotification;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoveNotificationsRequest = Pi3.App.Legacy.WebApi.Application.Requests.RemoveNotifications;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RemoveNotifications
{
    public class RemoveNotificationsHandler : IRequestHandler<RemoveNotificationsRequest, RemoveNotificationsResult>
    {
        #region Public Members

        public RemoveNotificationsHandler(ILogger<RemoveNotificationsHandler> logger,
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

        public async Task<RemoveNotificationsResult> Handle(RemoveNotificationsRequest request, CancellationToken cancellationToken)
        {
            bool output = true;

            try
            {
                var idPeopleDelegato = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser);

                foreach (var idNotification in request.notifications.Select(n => n.ID_NOTIFY.AsLong()))
                {
                    var notifyEntity = await this._dbContext.NotifyEntities
                                .Where(n => n.SYSTEM_ID == idNotification)
                                .FirstOrDefaultAsync();

                    if (notifyEntity != null)
                    {
                        await this._dbContext.NotifyHistoryEntities.AddAsync(new NotifyHistoryEntity()
                        {
                            ID_NOTIFY = notifyEntity.SYSTEM_ID,
                            ID_EVENT = notifyEntity.ID_EVENT,
                            DESC_PRODUCER = notifyEntity.DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER = notifyEntity.ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER = notifyEntity.ID_GROUP_RECEIVER,
                            TYPE_NOTIFY = notifyEntity.TYPE_NOTIFY,
                            DTA_NOTIFY = notifyEntity.DTA_NOTIFY,
                            FIELD_1 = notifyEntity.FIELD_1,
                            FIELD_2 = notifyEntity.FIELD_2,
                            FIELD_3 = notifyEntity.FIELD_3,
                            FIELD_4 = notifyEntity.FIELD_4,
                            MULTIPLICITY = notifyEntity.MULTIPLICITY,
                            SPECIALIZED_FIELD = notifyEntity.SPECIALIZED_FIELD,
                            TYPE_EVENT = notifyEntity.TYPE_EVENT,
                            DOMAINOBJECT = notifyEntity.DOMAINOBJECT,
                            ID_OBJECT = notifyEntity.ID_OBJECT,
                            ID_SPECIALIZED_OBJECT = notifyEntity.ID_SPECIALIZED_OBJECT,
                            DTA_EVENT = notifyEntity.DTA_EVENT,
                            READ_NOTIFICATION = notifyEntity.READ_NOTIFICATION,
                            NOTES = notifyEntity.NOTES
                        });

                        if (notifyEntity.TYPE_EVENT != null && notifyEntity.TYPE_EVENT.StartsWith("TRASM_"))
                        {
                            var trasmUtenteEntity = await this._dbContext.TrasmUtenteEntities
                                    .Where(u => u.ID_TRASM_SINGOLA == notifyEntity.ID_SPECIALIZED_OBJECT
                                        && u.ID_PEOPLE == notifyEntity.ID_PEOPLE_RECEIVER)
                                    .FirstOrDefaultAsync();

                            if (trasmUtenteEntity != null)
                            {
                                trasmUtenteEntity.DTA_RIMOZIONE_TODOLIST = await _dbContext.GetSystemDateTime();
                                trasmUtenteEntity.CHA_IN_TODOLIST = "0";

                                if (!string.IsNullOrEmpty(idPeopleDelegato))
                                {
                                    trasmUtenteEntity.CHA_RIMOZIONE_DELEGATO = "1";
                                    trasmUtenteEntity.ID_PEOPLE_DELEGATO = idPeopleDelegato.AsLong();
                                }
                            }
                        }

                        this._dbContext.NotifyEntities.Remove(notifyEntity);

                        await ((DbContext)_dbContext).SaveChangesAsync();
                    }
                }
            }
            catch (Pi3Exception pi3ex)
            {
                output = false;
                this._logger.LogError(pi3ex, null, null!);
            }

            catch (Exception ex)
            {
                output = false;
                this._logger.LogCritical(ex, null, null!);
            }

            return new RemoveNotificationsResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RemoveNotificationsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}
