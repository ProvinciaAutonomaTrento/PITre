// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.File.TextExtractors;
using Pi3.Infrastructure.BouncyCastle.Services.File.TextExtractors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core;
using Pi3.Infrastructure.BouncyCastle.Services.File.CAdES;
using Pi3.Core.Services.File.CAdES;

namespace Pi3.Infrastructure.BouncyCastle
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureBouncyCastle(this IServiceCollection services)
        {
            services.AddPi3Core();

            services.AddScoped<IFileTextExtractorService, BouncyCastleSignedFileTextExtractorService>();
            services.AddScoped<ICAdESService, BouncyCastleCAdESService>();

            return services;
        }
    }
}
