// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Oracle;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.OracleDbContextFactory
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureLegacyEFOracleDbContextFactory(
            this IServiceCollection services)
        {
            services.AddScoped<IInstanceProvider>(provider =>
            {
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();

                if (httpContextAccessor.HttpContext != null)
                    return new HttpContextAccessorInstanceProvider(httpContextAccessor);

                var claimsPrincipalService = provider.GetRequiredService<IClaimsPrincipalService>();

                if (claimsPrincipalService.Current != null)
                    return new ClaimsPrincipalInstanceProvider(claimsPrincipalService);

                return null!;
            });

            services.AddScoped<IOracleDbContextFactoryService, OracleDbContextFactoryService>();

            services.AddScoped<IPi3DbContext>(provider =>
                provider.GetRequiredService<IOracleDbContextFactoryService>().CreateDbContext());

            services.AddScoped<IPi3DbContextEntities>(provider =>
                provider.GetRequiredService<IOracleDbContextFactoryService>().CreateDbContext());

            services.AddScoped<IPi3DbContextFunctions>(provider =>
                provider.GetRequiredService<IOracleDbContextFactoryService>().CreateDbContext());

            return services;
        }
    }
}