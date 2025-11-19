// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;
using MODELS = Pi3.App.Legacy.Mobile.Models;
using SERVICE_REQUESTS = Pi3.App.Legacy.Mobile.Models.ServiceRequests;
using SELECT_TEMPLATES = Pi3.App.Legacy.Mobile.Models.SelectTemplates;
using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pi3.App.Legacy.Mobile.Data.Services;
public class NotificationService(INotificationRepository notificationRepository) : INotificationService
{
    readonly INotificationRepository _notificationRepository = notificationRepository;

    public async Task<IEnumerable<SELECT_TEMPLATES.Notification>> GetNotificationByIdEventAsync(
        long idEvent,
        CancellationToken cancellationToken )
    {
        IEnumerable<SELECT_TEMPLATES.Notification> notifications 
            = await this._notificationRepository.GetNotificationByIdEventAsync(idEvent, cancellationToken);

        await this._notificationRepository.ArchiveNotificationByIdEnventAsync(idEvent, cancellationToken );

        return notifications;
    }

    public async Task<bool> RemoveNotificationByIdEventAsync(long idEvent, CancellationToken cancellationToken)
    {
        bool result = false;
        using IDbContextTransaction transaction = await this._notificationRepository.BeginTransactionAsync(cancellationToken);
        try
        {
            await this._notificationRepository.ArchiveNotificationByIdEnventAsync(idEvent, cancellationToken);
            result = await this._notificationRepository.SaveAsync(cancellationToken) > 0;
            await transaction.CommitAsync(cancellationToken);
        }
        catch( Exception )
        {
#pragma warning disable CA2016 // Forward the 'CancellationToken' parameter to methods
            await transaction.RollbackAsync();
#pragma warning restore CA2016 // Forward the 'CancellationToken' parameter to methods
            result = false;
        }
        return result;
    }
}
