// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MODELS = Pi3.App.Legacy.Mobile.Models;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;
public class NoteRepository( IPi3DbContext pi3DbContext ) : INoteRepository
{
    private readonly IPi3DbContext _pi3DbContext = pi3DbContext;

    public async Task<MODELS.SelectTemplates.Nota?> GetUltimaNota(long idOggettoAssociato, long idPeople, long? idGroup, long? idCorrGlobali, CancellationToken cancellationToken )
    {
        var subQueryRegistro = this._pi3DbContext.RuoloRegistroEntities.AsNoTracking()
            .Where(rr => rr.ID_RUOLO_IN_UO == idCorrGlobali)
            .Select(rr => rr.ID_REGISTRO);

        var query = this._pi3DbContext.NoteEntities.AsNoTracking()
            .Where(n => n.TIPOOGGETTOASSOCIATO == "D" && n.IDOGGETTOASSOCIATO == idOggettoAssociato)
            .Where(n => n.TIPOVISIBILITA == "T"
                        || (n.TIPOVISIBILITA == "F" && subQueryRegistro.Contains(n.IDRFASSOCIATO))
                        || (n.TIPOVISIBILITA == "P" && n.IDUTENTECREATORE == idPeople)
                        || (n.TIPOVISIBILITA == "R" && n.IDRUOLOCREATORE == idGroup))
            .OrderByDescending(n => n.DATACREAZIONE)
            .Select(n => new MODELS.SelectTemplates.Nota
            {
                SystemId = n.SYSTEM_ID,
                Testo = n.TESTO,
                DataCreazione = n.DATACREAZIONE,
                IdUtenteCreatore = n.IDUTENTECREATORE,
                IdRuoloCreatore = n.IDRUOLOCREATORE,
                TipoVisibilita = n.TIPOVISIBILITA,
                TipoOggettoAssociato = n.TIPOOGGETTOASSOCIATO,
                IdOggettoAssociato = n.IDOGGETTOASSOCIATO,
                IdFascicoloAssociato = n.IDRFASSOCIATO
            });

        MODELS.SelectTemplates.Nota? risultato = await query.FirstOrDefaultAsync(cancellationToken);
        return risultato;
    }
}
