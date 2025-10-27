// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetRagioneNotifica;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoveNotificationOfTransmissionNoWFRequest = Pi3.App.Legacy.WebApi.Application.Requests.RemoveNotificationOfTransmissionNoWF;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RemoveNotificationOfTransmissionNoWF
{
    public class RemoveNotificationOfTransmissionNoWFHandler : IRequestHandler<RemoveNotificationOfTransmissionNoWFRequest, RemoveNotificationOfTransmissionNoWFResult>
    {
        #region Public Members

        public RemoveNotificationOfTransmissionNoWFHandler(ILogger<RemoveNotificationOfTransmissionNoWFHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<RemoveNotificationOfTransmissionNoWFResult> Handle(RemoveNotificationOfTransmissionNoWFRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var idPeople = request.idPeople.AsLong();
                var idGruppo = request.idGroup.AsLong();
                var dateAsDateTime = (request.date.AsDateTime().AddDays(1)).Date;

                var notifyEntities = await this._dbContext.NotifyEntities
                    .Where(n => n.ID_PEOPLE_RECEIVER == idPeople && (n.ID_GROUP_RECEIVER == idGruppo || n.ID_GROUP_RECEIVER == 0)
                    && n.DTA_NOTIFY < dateAsDateTime && ((!n.TYPE_EVENT.StartsWith("TRASM_DOC_") && !n.TYPE_EVENT.StartsWith("TRASM_FOLDER_")) ||
                    !this._dbContext.RagioneTrasmissioneEntities
                    .Join(this._dbContext.TrasmSingolaEntities, ragione => ragione.SYSTEM_ID, trasm => trasm.ID_RAGIONE, (ragione, trasm) => new { ragione, trasm })
                    .Any(j => j.trasm.SYSTEM_ID == n.ID_SPECIALIZED_OBJECT && j.ragione.CHA_TIPO_RAGIONE == "W")))
                    .ToListAsync();

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
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new RemoveNotificationOfTransmissionNoWFResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RemoveNotificationOfTransmissionNoWFHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
