// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.Decorators;
using Pi3.Infrastructure.Adobe.Services.ConverterService;

namespace Pi3.Infrastructure.Adobe
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureAdobeFileConverter(this IServiceCollection services, Action<AdobePdfFileConverterServiceOptions> options)
        {
            services.Configure<AdobePdfFileConverterServiceOptions>(options);
            services.AddScoped<IFileConverterService, AdobePdfFileConverterService>();

            return services;
        }
    }
}