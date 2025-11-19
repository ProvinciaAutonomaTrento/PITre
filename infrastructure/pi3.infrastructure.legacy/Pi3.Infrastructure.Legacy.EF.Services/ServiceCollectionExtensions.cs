// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger;
using Pi3.Core.Services.Configuration;
using Pi3.Infrastructure.Legacy.EF.Services.Configuration;
using Pi3.Core;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Infrastructure.Legacy.EF.Services.FileValidator;

namespace Pi3.Infrastructure.Legacy.EF
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureLegacyEFServices(
            this IServiceCollection services)
        {
            services.AddPi3Core();

            services.RemoveAll<IFileValidatorService>().AddScoped<IFileValidatorService, FileValidatorEFService>();
            services.RemoveAll<IWebMethodLoggerService>().AddScoped<IWebMethodLoggerService, WebMethodLoggerEFService>();
            services.RemoveAll<IConfigurationService>().AddScoped<IConfigurationService, ConfigurationEFService>();

            return services;
        }
    }
}