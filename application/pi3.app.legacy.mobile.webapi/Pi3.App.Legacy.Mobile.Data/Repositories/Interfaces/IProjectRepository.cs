// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Linq.Expressions;
using MODELS = Pi3.App.Legacy.Mobile.Models;

namespace Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
public interface IProjectRepository
{
    Task<IEnumerable<MODELS.SelectTemplates.Project>> GetProjectByIdComponentWithSecurityAsync( 
        long idComponent,
        IList<long?> thingSecurityCheck,
        CancellationToken cancellationToken );
}
