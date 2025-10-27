// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;

using SELECT_TEMPLATES = Pi3.App.Legacy.Mobile.Models.SelectTemplates;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;
public class AdministrationRepository( IPi3DbContext pi3DbContext ) : IAdministrationRepository
{
    readonly IPi3DbContext _pi3DbContext = pi3DbContext;
    
    public async Task<SELECT_TEMPLATES.AdministrationPasswordSettings?> GetAdministrationPasswordSettingsAsync(
        long id, 
        CancellationToken cancellationToken)
    {
        SELECT_TEMPLATES.AdministrationPasswordSettings? settings = await this._pi3DbContext.AmministraEntities.AsNoTracking()
            .Where(e => e.SYSTEM_ID == id)
            .Select(s => new SELECT_TEMPLATES.AdministrationPasswordSettings
            {
                IsPasswordExpirationEnabled = s.ENABLE_PASSWORD_EXPIRATION,
                PasswordExpirationDays = s.PASSWORD_EXPIRATION_DAYS,
            })
            .FirstOrDefaultAsync(cancellationToken);
        return settings;
    }
}
