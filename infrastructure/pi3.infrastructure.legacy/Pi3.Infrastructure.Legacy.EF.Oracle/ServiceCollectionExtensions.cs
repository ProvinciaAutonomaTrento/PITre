// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Oracle
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureLegacyEFOracleDbContext(this IServiceCollection services, Action<OraclePi3DbContextOptions> options)
        {
            services.AddInfrastructureLegacyEFOracleDbContext<OraclePi3DbContext>(options);

            return services;
        }

        public static IServiceCollection AddInfrastructureLegacyEFOracleDbContext<T>(this IServiceCollection services, Action<OraclePi3DbContextOptions> options) 
            where T : OraclePi3DbContext
        {
            services.Configure(options);
            services.AddScoped<IPi3DbContext, T>();
            services.AddScoped<IPi3DbContextEntities, T>();
            services.AddScoped<IPi3DbContextFunctions, T>();

            //services.AddDbContextPool<T>(c =>
            //{
                
            //})

            return services;
        }
    }
}
