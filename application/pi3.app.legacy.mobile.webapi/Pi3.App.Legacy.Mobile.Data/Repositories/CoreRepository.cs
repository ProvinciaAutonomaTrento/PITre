// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;
public abstract class CoreRepository( IPi3DbContext pi3DbContext ) : ICoreRepository
{
    internal readonly DbContext? _dbContext = pi3DbContext as DbContext;
    internal readonly IPi3DbContext _pi3DbContext = pi3DbContext;

    async Task<int> ICoreRepository.SaveAsync(CancellationToken cancellationToken)
    {
        if(this._dbContext == null)
        {
            throw new Exception();
        }
        int result = await this._dbContext.SaveChangesAsync(cancellationToken);
        return result;
    }

    async Task<IDbContextTransaction> ICoreRepository.BeginTransactionAsync( CancellationToken cancellationToken )
    {
        if ( this._dbContext == null )
        {
            throw new Exception();
        }
        IDbContextTransaction transaction = await this._dbContext.Database.BeginTransactionAsync(cancellationToken);
        return transaction;
    }
}
