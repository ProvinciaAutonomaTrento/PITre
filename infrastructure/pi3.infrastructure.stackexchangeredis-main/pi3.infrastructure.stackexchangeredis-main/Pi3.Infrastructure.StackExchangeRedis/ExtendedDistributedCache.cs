// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Pi3.Infrastructure.StackExchangeRedis
{
    public class ExtendedDistributedCache : IDistributedCache
    {
        #region Public Members

        public string SerializerType { get; }
        public string Prefix { get; }
        public string KeyType { get; }
        public string HashField { get; }

        public ExtendedDistributedCache(IConnectionMultiplexer connectionMultiplexer, IOptionsSnapshot<ExtendedDistributedCacheOptions> options)
        {
            _database = connectionMultiplexer.GetDatabase();
            SerializerType = options.Value.SerializerType;
            Prefix = options.Value.Prefix;
            KeyType = options.Value.KeyType;
            HashField = options.Value.HashField;
        }

        private string GetPrefixedKey(string key) => $"{Prefix}{key}";

        public byte[] Get(string key)
        {
            key = GetPrefixedKey(key);

            return KeyType == "Hash" ? _database.HashGet(key, HashField)! : _database.StringGet(key);
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            key = GetPrefixedKey(key);

            return KeyType == "Hash" ? (byte[])await _database.HashGetAsync(key, HashField) : (byte[])await _database.StringGetAsync(key);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            key = GetPrefixedKey(key);

            if (KeyType == "Hash")
            {
                _database.HashSet(key, "data", value);
                _database.HashSet(key, "absexp", options.AbsoluteExpirationRelativeToNow.Value.Ticks);
                _database.HashSet(key, "sldexp", -1);
            }
            else
            {
                _database.StringSet(key, value, options.AbsoluteExpirationRelativeToNow);
            }
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            key = GetPrefixedKey(key);

            if (KeyType == "Hash")
            {
                await _database.HashSetAsync(key, HashField, value);
            }
            else
            {
                await _database.StringSetAsync(key, value, options.AbsoluteExpirationRelativeToNow);
            }
        }

        public void Refresh(string key)
        {
            // Not implemented
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            // Not implemented
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            key = GetPrefixedKey(key);
            _database.KeyDelete(key);
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            key = GetPrefixedKey(key);

            await _database.KeyDeleteAsync(key);
        }

        #endregion

        #region Private Members

        private readonly IDatabase _database;

        #endregion
    }
}
