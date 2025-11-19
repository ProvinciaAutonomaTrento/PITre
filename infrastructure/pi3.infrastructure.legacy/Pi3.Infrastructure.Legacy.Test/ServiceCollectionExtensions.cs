// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Test;
using Pi3.Infrastructure.Legacy.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using System.Text.Json;

namespace Pi3.Infrastructure.Legacy.Test
{
    public static class ServiceCollectionExtensions
    { 
        public static IServiceCollection AddBaseDependenciesForTest(this IServiceCollection services)
        {   
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString")!;
            var enableLogging =  Convert.ToBoolean(Environment.GetEnvironmentVariable("EnableLogging")!);

            services.AddLogging()
                   .AddScoped<IClaimsPrincipalService, UnitTestClaimsPrincipalService>()
                   .AddDistributedMemoryCache()
                   .AddInfrastructureLegacyEFOracleDbContext(opt =>
                   {
                       opt.ConnectionString = connectionString;
                       opt.EnableLogging = enableLogging;
                   });

            return services;
        }
    }
}
