// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.Extensions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pi3.Infrastructure.Legacy.EF.Entities.Extensions
{
    public static class DistributedCacheExtensions
    {
        public async static Task<IReadOnlyList<E>> FromCache<E>(
             this IDistributedCache distributedCache,
             string instance,
             DbSet<E> dbSet,
             Expression<Func<E, bool>>? predicate = null,
             DistributedCacheEntryOptions? options = null,
             CancellationToken token = default(CancellationToken)) where E : class
        {
            distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            instance = instance ?? throw new ArgumentNullException(nameof(instance));
            dbSet = dbSet ?? throw new ArgumentNullException(nameof(dbSet));

            string? key = null;
            IQueryable<E>? queryable = null;

            if (predicate != null)
                queryable = dbSet.Where(predicate).AsNoTracking();
            else
                queryable = dbSet.AsNoTracking().AsQueryable();

            key = $"{instance}_{queryable.ToQueryString()}";

            var hashedKey = Encoding.UTF8.GetBytes(key).ComputeHashAsSha256String();

            return await distributedCache.FromCache<IReadOnlyList<E>>(
                async () =>
                {
                    return (await queryable.ToListAsync()).AsReadOnly();
                },
                hashedKey,
                options,
                token);
        }
    }
}
