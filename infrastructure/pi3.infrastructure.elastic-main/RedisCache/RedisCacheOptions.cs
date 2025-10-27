// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCache
{
    public class RedisCacheOptions : Microsoft.Extensions.Caching.StackExchangeRedis.RedisCacheOptions
    {
        public string SerializerType { get; set; } = "MessagePack"; // Default to MessagePack
        public string Prefix { get; set; } = string.Empty; // Default to empty string
        public string KeyType { get; set; } = "String"; // Default to String
        public string HashField { get; set; } = "value"; // Default to "value"
    }
}
