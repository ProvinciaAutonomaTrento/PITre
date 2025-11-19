// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.TextExtractors;
using Pi3.Core.Services.Factory;

namespace Pi3.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPi3Core(this IServiceCollection services)
        {
            services.RemoveAll<IEventPublisher>().AddScoped<IEventPublisher, Pi3.Core.AggregateModels.SyncEventPublisher>();
            services.AddScoped<IFactoryService, ServiceProviderFactoryService>();
            services.RemoveAll<IFileConverterFactory>().AddScoped<IFileConverterFactory, FileConverterServiceProviderFactory>();
            services.RemoveAll<IFileTextExtractorFactory>().AddScoped<IFileTextExtractorFactory, FileTextExtractorServiceProviderFactory>();
            
            return services;
        }
    }
}

