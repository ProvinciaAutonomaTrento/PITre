// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCache
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRedisCache(this IServiceCollection services, Action<RedisCacheOptions> configure)
        {
            services.Configure(configure);
            //services.AddSingleton<IDistributedCache, RedisMessagePackCache>();

            // Configura le opzioni di Redis
            //services.Configure<RedisCacheOptions>(options =>
            //{
            //    options.Configuration = "localhost:6379";
            //    options.InstanceName = "SampleInstance";
            //    options.SerializerType = "MessagePack"; // Può essere "MessagePack" o "Json"
            //    options.KeyType = "String"; // Può essere "String" o "Hash"
            //    options.HashField = "value";
            //});

            // Registra ConnectionMultiplexer come singleton
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RedisCacheOptions>>().Value;
                var configuration = ConfigurationOptions.Parse(options.Configuration, true);
                configuration.ResolveDns = true;
                return ConnectionMultiplexer.Connect(configuration);
            });

            // Registra il custom caching provider
            services.AddSingleton<IDistributedCache, RedisMessagePackCache>();
        }
    }
}
