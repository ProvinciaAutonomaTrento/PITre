// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.StackExchangeRedis
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options = null!)
        {
            var redisCache = cache as ExtendedDistributedCache;
            byte[] data;

            if (redisCache != null && redisCache.SerializerType == "MessagePack")
            {
                data = MessagePackSerializer.Serialize(value);
            }
            else
            {
                data = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value);
            }

            await cache.SetAsync(key, data, options ?? new DistributedCacheEntryOptions());
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key)
        {
            var redisCache = cache as ExtendedDistributedCache;
            var data = await cache.GetAsync(key);

            if (data == null)
            {
                return default!;
            }

            if (redisCache != null && redisCache.SerializerType == "MessagePack")
            {
                return MessagePackSerializer.Deserialize<T>(data);
            }
            else
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(data)!;
            }
        }
    }
}
