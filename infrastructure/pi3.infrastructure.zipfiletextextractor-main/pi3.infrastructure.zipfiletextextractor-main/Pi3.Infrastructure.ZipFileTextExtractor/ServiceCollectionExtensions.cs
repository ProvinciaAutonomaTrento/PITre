// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.File.TextExtractors;
using Pi3.Infrastructure.ZipFileTextExtractor.Services.File;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core;

namespace Pi3.Infrastructure.ZipFileTextExtractor
{
    public class ZipFileTextExtractorOptions
    {
        public string SpoolFolder { get; set; }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddZipFileTextExtractor(this IServiceCollection services, Action<ZipFileTextExtractorOptions> options)
        {
            services.AddPi3Core();
            services.Configure<ZipFileTextExtractorOptions>(options);
            services.AddScoped<IFileTextExtractorService, ZipFileTextExtractorService>();

            return services;
        }
    }
}
