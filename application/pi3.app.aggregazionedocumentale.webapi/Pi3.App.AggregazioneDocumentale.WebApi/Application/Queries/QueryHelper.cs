// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries;

namespace Pi3.Infrastructure.Legacy.EF.Entities;
public static class QueryHelper
{
    public static IQueryable<AggregateResult> UserAggregates(this IPi3DbContext context, string? idTenant, int idRegistro, int idUtente, int idGruppo) {

        var projectsQry = context.ProjectEntities.Where(proj => proj.ID_AMM == Convert.ToInt32(idTenant)
            && proj.CHA_TIPO_PROJ == "F" && proj.ID_REGISTRO == idRegistro &&
            context.SecurityEntities.Any(sec => sec.THING == proj.SYSTEM_ID &&
                (sec.PERSONORGROUP == idUtente || sec.PERSONORGROUP == idGruppo)))
            .OrderByDescending(prj => prj.DTA_CREAZIONE)
            .Select(prj => new AggregateResult()
                {
                    Id = context.ProjectEntities.Where(prj2 => prj2.ID_PARENT == prj.SYSTEM_ID && prj2.CHA_TIPO_PROJ == "C"
                        && prj2.DESCRIPTION.ToUpper() == prj.VAR_CODICE.ToUpper()).Select(prj => prj.SYSTEM_ID).First(),
                    Codice = prj.VAR_CODICE,
                    Descrizione= prj.DESCRIPTION,
                    IdTenant= prj.ID_AMM,
                    DataCreazione = prj.DTA_CREAZIONE,
                    DataApertura = prj.DTA_APERTURA,
                    DataChiusura = prj.DTA_CHIUSURA
                });
        return projectsQry;
    }
}
