// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;

using MODELS = Pi3.App.Legacy.Mobile.Models;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;
public class SecurityRepository( IPi3DbContext pi3DbContext ) : ISecurityRepository
{
    private readonly IPi3DbContext _pi3DbContext = pi3DbContext;
 
    public async Task<long> GetAccessRigthByIdObjectAsync(
        long id, 
        IList<long?> thingSecurityCheck, 
        CancellationToken cancellationToken )
    {
        long? maxAccessRights = await this._pi3DbContext.SecurityEntities.AsNoTracking()
            .Where(s => s.THING == id
                && thingSecurityCheck.Contains(s.PERSONORGROUP)
            )
            .MaxAsync(s => s.ACCESSRIGHTS, cancellationToken: cancellationToken);

        return maxAccessRights ?? 0;
    }

    
}
