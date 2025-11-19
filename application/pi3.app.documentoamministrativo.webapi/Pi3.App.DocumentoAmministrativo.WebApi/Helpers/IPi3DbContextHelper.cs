// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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

    public static async Task<IEnumerable<TrasmissioneEntity>> TrasmissioniDaCodiceDocumento(this DbSet<TrasmissioneEntity>
        trasmissioneEntities, string idAggregazione)
    {
        var trasmissioni = await trasmissioneEntities.Where(trasm =>
            trasm.ID_PROFILE == Convert.ToInt32(idAggregazione)).ToListAsync();
        return trasmissioni;
    }
}