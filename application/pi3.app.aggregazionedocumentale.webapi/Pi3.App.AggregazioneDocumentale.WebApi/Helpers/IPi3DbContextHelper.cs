// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Microsoft.EntityFrameworkCore;

public static class IPi3DbContextHelper
{
    public static async Task<long> SystemIdFromUserId(this DbSet<PeopleEntity> peopleEntities, string userId)
    {
        var result = (await peopleEntities.SingleAsync(x => x.USER_ID.ToUpper() == userId.ToUpper()
            && x.DISABLED == "N")).SYSTEM_ID;
        return result;
    }

    public static async Task<PeopleEntity> EntityFromSystemId(this DbSet<PeopleEntity> peopleEntities, string systemId) {
        var result = await peopleEntities.SingleAsync(x => x.SYSTEM_ID == Convert.ToInt32(systemId)
                                && x.DISABLED == "N");
        return result;
    }

    public static async Task<CorrGlobaliEntity> PianoConservazioneCollocazioneDaCodice ( this 
        DbSet<CorrGlobaliEntity> pianoConservazioneEntities, string codice)
    {
        var result = await pianoConservazioneEntities
                    .AsNoTracking()
                    .Where(entity => entity.VAR_CODICE.ToUpper() == codice.ToUpper()
                        && entity.CHA_TIPO_URP == "U"
                        && entity.DTA_FINE == null)
                    .SingleAsync();
        return result;
    }

    public static async Task<PianoConservazioneEntity> GetPianoConservazioneClassificazione(this
        DbSet<PianoConservazioneEntity> pianoConservazioneEntities, string tipologiaFascicolo, string codiceClassificazione,
            string idTenant, long? idRegistro) {
        var result = await pianoConservazioneEntities
            .AsNoTracking()
            .Where(entity =>
                   entity.TIPOLOGIA_FASCICOLO.ToUpper() == tipologiaFascicolo.ToUpper()
                && entity.CODICE_CLASSIFICAZIONE.ToUpper() == codiceClassificazione.ToUpper()
                && entity.ID_AMM == Convert.ToInt32(idTenant)
                && entity.ID_REGISTRO == idRegistro

                && entity.DTA_FINE == null)
            .SingleAsync();
        return result;
    }

    public static async Task<long> SystemIdDaCodiceRegistro( this DbSet<RegistroEntity> registroEntities, string codiceRegistro) {
        var result = await registroEntities
        .AsNoTracking()
            .Where(entity => entity.VAR_CODICE.ToUpper() == codiceRegistro.ToUpper())
            .Select(entity => entity.SYSTEM_ID).FirstOrDefaultAsync();
        return result;
    }

    public static async Task<long> SystemIdDaRagioneTrasmissione( this DbSet<RagioneTrasmissioneEntity> ragioneTrasmissioneEntities,
        string ragioneTrasmissione, string idTenant) {
        var result = (await ragioneTrasmissioneEntities.SingleAsync(x => x.VAR_DESC_RAGIONE.ToUpper() == ragioneTrasmissione.ToUpper()
            && x.ID_AMM == Convert.ToInt32(idTenant) && x.CHA_VIS == "1" && x.CHA_RAG_SISTEMA == "0")).SYSTEM_ID;
        return result;
    }

    public static async Task<long> SystemIdDaCodiceGruppo(this DbSet<GroupEntity> groupEntities, string codiceGruppo) {
        var result = (await groupEntities.SingleAsync(x => x.GROUP_ID.ToUpper() == codiceGruppo.ToUpper()
            && x.DISABLED == "N")).SYSTEM_ID;
        return result;
    }

    public static async Task<GroupEntity> EntityDaCodice(this DbSet<GroupEntity> groupEntity, string codiceGruppo) {
        var result = await groupEntity.SingleAsync(x => x.GROUP_ID.ToUpper() == codiceGruppo.ToUpper()
            && x.DISABLED == "N");
        return result;
    }

    public static async Task<GroupEntity> EntityDaSystemId(this DbSet<GroupEntity> groupEntity, string systemId)
    {
        var result = await groupEntity.SingleAsync(grp => grp.SYSTEM_ID == Convert.ToInt32(systemId)
            && grp.DISABLED == "N");
        return result;
    }

    public static async Task<IEnumerable<TrasmissioneEntity>> TrasmissioniDaCodiceAggregazione(this DbSet<TrasmissioneEntity>
        trasmissioneEntities, string idAggregazione)
    {
        var trasmissioni = await trasmissioneEntities.Where(trasm =>
            trasm.ID_PROJECT == Convert.ToInt32(idAggregazione)).ToListAsync();
        return trasmissioni;
    }

    public static async Task<ProjectEntity> EntityFromSystemId(this DbSet<ProjectEntity> projectEntities, string idTenant, string systemId) {
        var result = await projectEntities.FirstOrDefaultAsync(prj => prj.ID_AMM == Convert.ToInt32(idTenant)
            && prj.SYSTEM_ID == Convert.ToInt32(systemId));
        return result;
    }

    public static async Task<ProjectEntity> EntityAltFromSystemId(this DbSet<ProjectEntity> projectEntities, string idTenant, string systemId) {
        var result = await projectEntities.FirstOrDefaultAsync(prj => prj.ID_AMM == Convert.ToInt32(idTenant)
            && prj.ID_PARENT == Convert.ToInt32(systemId) && prj.CHA_TIPO_PROJ == "C");
        return result;
    }

    public static async Task<TipoFascEntity> EntityFromNome(this DbSet<TipoFascEntity> tipoFascEntity, string nome) {
        var tipoFascicoloEntity = await tipoFascEntity
         .AsNoTracking()
         .Where(ta => ta.VAR_DESC_FASC.ToUpper() == nome.ToUpper()
            && ta.IN_ESERCIZIO == "SI")
         .FirstOrDefaultAsync();

        return tipoFascicoloEntity;
    }
}