// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Grid;
using DocsPaVO.Notification;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
using CheckNotificationRequest = Pi3.App.Legacy.WebApi.Application.Requests.CheckNotification;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckNotification
{
    public class CheckNotificationHandler : IRequestHandler<CheckNotificationRequest, CheckNotificationResult>
    {
        #region Public Members

        public CheckNotificationHandler(ILogger<CheckNotificationHandler> logger, 
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

        public async Task<CheckNotificationResult> Handle(CheckNotificationRequest request, CancellationToken cancellationToken)
        {
            bool output = true;

            try
            {
                var idEventAsLong = request.notification.ID_EVENT.AsLong();

                var queryable = this._dbContext.NotifyEntities.Where(n => n.ID_EVENT == idEventAsLong);

                if(!string.IsNullOrEmpty(request.notification.ID_GROUP))
                {
                    var idGruppo = request.notification.ID_GROUP.AsLong();
                    queryable = queryable.Where(n => n.ID_GROUP_RECEIVER == idGruppo);
                }

                if(!request.notification.MULTIPLICITY.Equals(Multiplicity.ONE))
                {
                    var idPeople = request.notification.ID_PEOPLE.AsLong();
                    queryable = queryable.Where(n => n.ID_PEOPLE_RECEIVER == idPeople);
                }

                var notifyEntities = await queryable.ToListAsync();

                if (notifyEntities != null && notifyEntities.Any())
                {
                    foreach (var n in notifyEntities)
                    {
                        var notifyHistoryEntity = new NotifyHistoryEntity()
                        {
                            ID_NOTIFY = n.SYSTEM_ID,
                            ID_EVENT = n.ID_EVENT,
                            DESC_PRODUCER = n.DESC_PRODUCER,
                            ID_PEOPLE_RECEIVER = n.ID_PEOPLE_RECEIVER,
                            ID_GROUP_RECEIVER = n.ID_GROUP_RECEIVER,
                            TYPE_NOTIFY = n.TYPE_NOTIFY,
                            DTA_NOTIFY = n.DTA_NOTIFY,
                            FIELD_1 = n.FIELD_1,
                            FIELD_2 = n.FIELD_2,
                            FIELD_3 = n.FIELD_3,
                            FIELD_4 = n.FIELD_4,
                            MULTIPLICITY = n.MULTIPLICITY,
                            SPECIALIZED_FIELD = n.SPECIALIZED_FIELD,
                            TYPE_EVENT = n.TYPE_EVENT,
                            DOMAINOBJECT = n.DOMAINOBJECT,
                            ID_OBJECT = n.ID_OBJECT,
                            ID_SPECIALIZED_OBJECT = n.ID_SPECIALIZED_OBJECT,
                            DTA_EVENT = n.DTA_EVENT,
                            READ_NOTIFICATION = n.READ_NOTIFICATION,
                            NOTES = n.NOTES
                        };

                        await this._dbContext.NotifyHistoryEntities.AddAsync(notifyHistoryEntity);
                    }

                    this._dbContext.NotifyEntities.RemoveRange(notifyEntities);

                    await ((DbContext)_dbContext).SaveChangesAsync();
                }
                
                if (request.notification.TYPE_EVENT.Equals("REJECT_TRASM_DOCUMENT") || request.notification.TYPE_EVENT.Equals("REJECT_TRASM_FOLDER"))
                {
                    var method = string.Empty;
                    var objectDescription = string.Empty;

                    if(request.notification.DOMAINOBJECT.Equals("DOCUMENTO"))
                    {
                        method = "CHECK_REJECT_DOCUMENT";
                        if(string.IsNullOrEmpty(request.notification.ITEMS.ITEM2))
                        {
                            objectDescription = string.Format(Resources.LogVistataNotificaDocumentoGrigio, request.notification.ID_OBJECT);
                        }
                        else
                        {
                            var idProfileAsLong = request.notification.ID_OBJECT.AsLong();
                            objectDescription = await this._dbContext.ProfileEntities.Where(p => p.SYSTEM_ID == idProfileAsLong).Select(p => p.VAR_SEGNATURA).FirstOrDefaultAsync();
                        }
                    }
                    if(request.notification.DOMAINOBJECT.Equals("FASCICOLO"))
                    {
                        method = "CHECK_REJECT_FOLDER";
                        objectDescription = string.Format(Resources.LogVistataNotificaFascicolo, request.notification.ID_OBJECT);
                    }

                    await this._webMethodLoggerService.LogOK(method, request.notification.ID_OBJECT, objectDescription, request.notification.ID_SPECIALIZED_OBJECT);
                }
            }
            catch (Exception ex)
            {
                output = false;
                this._logger.LogError(ex, null, null);
            }

            return new CheckNotificationResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CheckNotificationHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}
