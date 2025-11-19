// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;

using MODELS = Pi3.App.Legacy.Mobile.Models;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;

public class CorrGlobaliRepository( 
    IPi3DbContext pi3DbContext ) : ICorrGlobaliRepository
{
    private readonly IPi3DbContext _pi3DbContext = pi3DbContext;

    public async Task<MODELS.SelectTemplates.Ruolo?> GetRuoloByIdCorGlobali(long idCorrGlobali, CancellationToken cancellationToken )
    {
        MODELS.SelectTemplates.Ruolo? ruoloDB = await this._pi3DbContext.CorrGlobaliEntities
            .Where(e => e.SYSTEM_ID.Equals(idCorrGlobali) 
                        && !e.DTA_FINE.HasValue
                        && "I".Equals(e.CHA_TIPO_IE) 
                        && "R".Equals(e.CHA_TIPO_URP) )
            .Select( s => new MODELS.SelectTemplates.Ruolo
            {
                SystemId = s.SYSTEM_ID,
                IdGruppo = s.ID_GRUPPO
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return ruoloDB;
    }

}
