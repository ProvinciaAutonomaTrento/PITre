// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.CommandHandlers.Notifications;
public class RemoveNotificationByIdEventHandler(
    INotificationService notificationService
    ) : IRequestHandler<COMMAND.RimuoviNotificaCommand, RESULT.Notifiche.RimuoviNotificaResult>
{
    readonly INotificationService _notificationService = notificationService;
    public async Task<RESULT.Notifiche.RimuoviNotificaResult> Handle( COMMAND.RimuoviNotificaCommand request, CancellationToken cancellationToken )
    {
        bool result = await this._notificationService.RemoveNotificationByIdEventAsync(request.IdEvento, cancellationToken);
        return new() { Result = result };
    }
}
