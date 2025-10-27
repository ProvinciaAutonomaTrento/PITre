// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SELECT_TEMPLATES = Pi3.App.Legacy.Mobile.Models.SelectTemplates;

namespace Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
public interface INotificationService
{
    Task<IEnumerable<SELECT_TEMPLATES.Notification>> GetNotificationByIdEventAsync(
        long idEvent,
        CancellationToken cancellationToken );

    Task<bool> RemoveNotificationByIdEventAsync( 
        long idEvent, 
        CancellationToken cancellationToken );
}
