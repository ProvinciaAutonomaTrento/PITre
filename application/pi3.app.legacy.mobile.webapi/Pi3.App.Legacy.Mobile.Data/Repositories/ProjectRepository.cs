// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Linq;
using System.Linq.Expressions;
using MODELS = Pi3.App.Legacy.Mobile.Models;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;
public class ProjectRepository( IPi3DbContext pi3DbContext ) : IProjectRepository
{
    private readonly IPi3DbContext _pi3DbContext = pi3DbContext;

    public async Task<IEnumerable<MODELS.SelectTemplates.Project>> GetProjectByIdComponentWithSecurityAsync(
        long idComponent,
        IList<long?> thingSecurityCheck,
        CancellationToken cancellationToken)
    {
        return await this._pi3DbContext.ProjectEntities.AsNoTracking()
            .Join(this._pi3DbContext.ProjectEntities.AsNoTracking(),
                p => p.SYSTEM_ID,
                pf => pf.ID_FASCICOLO,
                ( p, pf ) => new { Project = p, ProjectFascicolo = pf }
            )
            .Join(this._pi3DbContext.ProjectComponentEntities.AsNoTracking(),
                prev => prev.ProjectFascicolo.SYSTEM_ID,
                pc => pc.PROJECT_ID,
                (prev, pc) => new { prev.Project, prev.ProjectFascicolo, ProjectComponent = pc }
            )
            .Join(this._pi3DbContext.SecurityEntities.AsNoTracking(),
                prev => prev.Project.SYSTEM_ID,
                s => s.THING,
                (prev, s) => new { prev.Project, prev.ProjectFascicolo, prev.ProjectComponent, Security = s }
            )
            .Where(e => e.Project.CHA_TIPO_PROJ == "F" 
                && e.ProjectComponent.LINK == idComponent
                && thingSecurityCheck.Contains(e.Security.PERSONORGROUP)
                && e.Security.ACCESSRIGHTS > 0
            )
            .Select(s => new MODELS.SelectTemplates.Project
            {
                SystemId = s.Project.SYSTEM_ID,
                Codice = s.Project.VAR_CODICE,
                Description = s.Project.DESCRIPTION,
                TipoFascicolo = s.Project.CHA_TIPO_FASCICOLO
            })
            .Distinct()
            .OrderBy(x => x.SystemId)
            .ToListAsync(cancellationToken);
    }

}
