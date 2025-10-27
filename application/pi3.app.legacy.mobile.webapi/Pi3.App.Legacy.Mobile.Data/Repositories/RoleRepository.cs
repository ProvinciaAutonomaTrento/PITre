// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;

using MODELS = Pi3.App.Legacy.Mobile.Models;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;
public class RoleRepository( IPi3DbContext pi3DbContext ) : IRoleRepository
{
    readonly IPi3DbContext _pi3DbContext = pi3DbContext;

    public async Task<IEnumerable<MODELS.SelectTemplates.RuoloUtente>> GetRoleByIdPeopleAsync(
        long id, 
        CancellationToken cancellationToken ) 
    {
        IEnumerable<MODELS.SelectTemplates.RuoloUtente> ruoli = await this._pi3DbContext.PeopleGroupEntities.AsNoTracking()
            .Join( this._pi3DbContext.CorrGlobaliEntities,
                pg => pg.GROUPS_SYSTEM_ID,
                cg => cg.ID_GRUPPO,
                ( pg, cg ) => new { pg, cg } )
            .Join( this._pi3DbContext.TipoRuoloEntities,
                pg_cg => pg_cg.cg.ID_TIPO_RUOLO,
                tr => tr.SYSTEM_ID,
                (pg_cg, tr) => new { pg_cg.pg, pg_cg.cg, tr } )
            .Where( e => e.pg.PEOPLE_SYSTEM_ID == id && e.pg.DTA_FINE == null )
            .Where(e => e.cg.DTA_FINE == null)
            .Select( s => new MODELS.SelectTemplates.RuoloUtente
            {
                Preferito = s.pg.CHA_PREFERITO,

                Id = s.cg.SYSTEM_ID,
                IdUO = s.cg.ID_UO,
                Descrizione = s.cg.VAR_DESC_CORR,
                Codice = s.cg.VAR_CODICE,
                Livello = s.tr.NUM_LIVELLO,
                IdGruppo = s.cg.ID_GRUPPO,
            })
            .OrderByDescending(o => o.Preferito != null)
            .ThenByDescending(o => o.Preferito)
            .ToListAsync(cancellationToken);

        return ruoli;
    }

    public async Task<bool> CheckFunctionExistsForRoleByCode( 
        long idRole, 
        string code, 
        CancellationToken cancellationToken )
    {
        var result = await  this._pi3DbContext.FunzioneEntities.AsNoTracking()
            .Join(this._pi3DbContext.TipoFunzioneEntities,
                  funzioni => funzioni.ID_TIPO_FUNZIONE,
                  tipoFunzione => tipoFunzione.SYSTEM_ID,
                  ( funzioni, tipoFunzione ) => new { funzioni, tipoFunzione })
            .Join(this._pi3DbContext.TipoFRuoloEntities,
                  combined => combined.tipoFunzione.SYSTEM_ID,
                  tipoFRuolo => tipoFRuolo.ID_TIPO_FUNZ,
                  ( combined, tipoFRuolo ) => new { combined.funzioni, combined.tipoFunzione, tipoFRuolo })
            .AnyAsync(combined => combined.tipoFRuolo.ID_RUOLO_IN_UO == idRole && combined.funzioni.COD_FUNZIONE == code, 
                cancellationToken);
        return result;
    }
}
