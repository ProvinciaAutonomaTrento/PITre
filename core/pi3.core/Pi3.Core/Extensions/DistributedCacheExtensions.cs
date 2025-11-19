// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pi3.Core.Extensions
{
    public static class DistributedCacheExtensions
    {
        public async static Task<T> FromCache<T>(
            this IDistributedCache distributedCache, 
            Func<Task<T>> func, 
            string key, 
            DistributedCacheEntryOptions options = null, 
            CancellationToken token = default(CancellationToken)) where T : class
        {
            var cached = await distributedCache.GetAsync<T>(key, token);

            if (cached != null)
                return cached;

            cached = await func();

            await distributedCache.SetAsync<T>(cached, key, options, token);

            return cached;
        }

        private async static Task SetAsync<T>(this IDistributedCache distributedCache, T value, string key, DistributedCacheEntryOptions options = null, CancellationToken token = default(CancellationToken))
        {
            var asBytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(value));

            if (options == null)
            {
                options = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(60))
                };
            }

            await distributedCache.SetAsync(key, asBytes, options, token);
        }

        private async static Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken token = default(CancellationToken)) where T : class
        {
            var result = await distributedCache.GetAsync(key, token);

            if (result != null)
                return System.Text.Json.JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(result));
            else
                return null;
        }
    }
}
