// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;
public class RegisterRepository( IPi3DbContext pi3DbContext ) : IRegisterRepository
{
    readonly IPi3DbContext _pi3DbContext = pi3DbContext;

    public async Task<IEnumerable<long>> GetRegistriIdByGroupIdAsync(
        long id, 
        CancellationToken cancellationToken )
    {
        return await this._pi3DbContext.RuoloRegistroEntities.AsNoTracking()
            .Join(this._pi3DbContext.RegistroEntities,
                rr => rr.ID_REGISTRO,
                r => r.SYSTEM_ID,
                (rr, r) => new { rr, r })
            .Where(e => e.rr.ID_RUOLO_IN_UO == id)
            .Where(e => e.r.CHA_RF == "0")
            .Select(s => s.r.SYSTEM_ID)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
