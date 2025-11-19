// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;

using MODELS = Pi3.App.Legacy.Mobile.Models;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;
public class TrasmissionRepository(
    IPi3DbContext pi3DbContext ) : ITrasmissionRepository
{
    private readonly IPi3DbContext _pi3DbContext = pi3DbContext;

    public async Task<MODELS.SelectTemplates.TrasmissioneJoinSingolaRagioneUtente?> Get_TrasmissioneSignola_By_IdTrasmissioneBy_IdUtente_Async(
        long id,
        long peopleId,
        CancellationToken cancellationToken )
    {
        var result = await this._pi3DbContext.TrasmissioneEntities.AsNoTracking()
                .Join(_pi3DbContext.TrasmSingolaEntities,
                      t => t.SYSTEM_ID,
                      ts => ts.ID_TRASMISSIONE,
                      ( t, ts ) => new { t, ts })
                .Join(_pi3DbContext.RagioneTrasmissioneEntities,
                      tts => tts.ts.ID_RAGIONE,
                      r => r.SYSTEM_ID,
                      ( tts, r ) => new { tts, r })
                .Join(this._pi3DbContext.TrasmUtenteEntities,
                    ttsru => ttsru.tts.ts.SYSTEM_ID,
                    tu => tu.ID_TRASM_SINGOLA,
                    ( ttsru, tu) => new { ttsru, tu })
                .Where(e => e.ttsru.tts.t.SYSTEM_ID == id)
                .Where(e => e.tu.ID_PEOPLE == peopleId)
                .Select( s => new  MODELS.SelectTemplates.TrasmissioneJoinSingolaRagioneUtente
                {
                    Id = s.ttsru.tts.t.SYSTEM_ID,
                    DataInvio = s.ttsru.tts.t.DTA_INVIO,
                    NoteGenerali = s.ttsru.tts.t.VAR_NOTE_GENERALI,
                    IdPeopleDelegato = s.ttsru.tts.t.ID_PEOPLE_DELEGATO,
                    IdPeople = s.ttsru.tts.t.ID_PEOPLE,

                    NoteSingole = s.ttsru.tts.ts.VAR_NOTE_SING,
                    Ragione = s.ttsru.r.VAR_DESC_RAGIONE,

                    IdTrasmissioneUtente = s.tu.SYSTEM_ID,
                    DataAccettata = s.tu.DTA_ACCETTATA,
                    DataRifiutata = s.tu.DTA_RIFIUTATA,

                    TipoRagione = s.ttsru.r.CHA_TIPO_RAGIONE
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return result;
    }

}
