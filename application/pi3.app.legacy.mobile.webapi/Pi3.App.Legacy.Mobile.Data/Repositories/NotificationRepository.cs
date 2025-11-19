// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;
using SELECT_TEMPLATES = Pi3.App.Legacy.Mobile.Models.SelectTemplates;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;

public class NotificationRepository(
    ILogger<NotificationRepository> logger,
    IPi3DbContext pi3DbContext ) : CoreRepository(pi3DbContext), INotificationRepository
{
    readonly ILogger<NotificationRepository> _logger = logger;

    public async Task<IEnumerable<SELECT_TEMPLATES.Notification>> GetNotificationByIdEventAsync( 
        long idEvent, 
        CancellationToken cancellationToken )
    {
        return await this._pi3DbContext.NotifyEntities.AsNoTracking()
            .Where(e => e.ID_EVENT == idEvent)
            .Select( s => new SELECT_TEMPLATES.Notification
            {
                SystemId = s.SYSTEM_ID,
                IdEvent = s.ID_EVENT,
                DescProducer = s.DESC_PRODUCER,
                IdPeopleReceiver = s.ID_PEOPLE_RECEIVER,
                IdGroupReceiver = s.ID_GROUP_RECEIVER,
                TypeNotify = s.TYPE_NOTIFY,
                DtaNotify = s.DTA_NOTIFY,
                Field1 = s.FIELD_1,
                Field2 = s.FIELD_2,
                Field3 = s.FIELD_3, 
                Field4 = s.FIELD_4,
                Multiplicity = s.MULTIPLICITY,
                SpecializedField = s.SPECIALIZED_FIELD,
                TypeEvent = s.TYPE_EVENT,
                DomainObject = s.DOMAINOBJECT,
                IdObject = s.ID_OBJECT,
                IdSpecializedObject = s.ID_SPECIALIZED_OBJECT,
                DtaEvent = s.DTA_EVENT,
                ReadNotification = s.READ_NOTIFICATION,
                Notes = s.NOTES
            })
            .ToListAsync(cancellationToken);
    }

    // ToDo archivia notifica by id evento : leggi notifica( per test solo la prima) e inserisci la stessa in history)
    public async Task ArchiveNotificationByIdEnventAsync(
        long idEvent,
        CancellationToken cancellationToken )
    {
        IEnumerable<NotifyEntity> notifications = await this._pi3DbContext.NotifyEntities.AsNoTracking()
            .Where(e => e.ID_EVENT == idEvent)
            .ToListAsync(cancellationToken);
        
        if(!notifications.Any())
        {
            throw new EXCEPTIONS.EntityNotFoundException(nameof(NotifyEntity), idEvent);
        }

        this._logger.LogDebug("Archivio le notifiche");
        foreach ( var notification in notifications )
        {
            this._pi3DbContext.NotifyHistoryEntities.Add(this.MapFromNotify(notification));
        }

        this._logger.LogDebug("Elimino le notifiche");
        this._pi3DbContext.NotifyEntities.RemoveRange(notifications);
    }

    private NotifyHistoryEntity MapFromNotify(NotifyEntity notify )
    {
        return new NotifyHistoryEntity ()
        {
            ID_EVENT = notify.ID_EVENT,
            DESC_PRODUCER = notify.DESC_PRODUCER,
            TYPE_NOTIFY = notify.TYPE_NOTIFY,
            DOMAINOBJECT =notify.DOMAINOBJECT,
            DTA_EVENT = notify.DTA_EVENT,
            DTA_NOTIFY = notify.DTA_NOTIFY,
            ID_SPECIALIZED_OBJECT = notify.ID_SPECIALIZED_OBJECT,
            READ_NOTIFICATION = notify.READ_NOTIFICATION,
            SPECIALIZED_FIELD = notify.SPECIALIZED_FIELD,
            FIELD_1 = notify.FIELD_1,
            FIELD_2 = notify.FIELD_2,
            FIELD_3 = notify.FIELD_3,
            FIELD_4 = notify.FIELD_4,
            ID_GROUP_RECEIVER = notify.ID_GROUP_RECEIVER,
            ID_OBJECT = notify.ID_OBJECT,
            ID_PEOPLE_RECEIVER = notify.ID_PEOPLE_RECEIVER,
            MULTIPLICITY = notify.MULTIPLICITY,
            NOTES = notify.NOTES,
            TYPE_EVENT = notify.TYPE_EVENT,
            ID_NOTIFY = notify.SYSTEM_ID
        };
    }
}
