// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.File.PAdES;
using Pi3.Infrastructure.Chilkat.Services.Email.BoxScanner;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;
using Pi3.Infrastructure.Chilkat.Services.File.PAdES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Chilkat
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureChilkat(
            this IServiceCollection services,
            Action<ChilkatOptions> options)
        {
            services.AddPi3Core();

            services.Configure(options);

            services.AddScoped<IEmailSenderService, ChilkatEmailSenderService>();
            services.AddScoped<IEmailBoxScannerService, ChilkatEmailBoxScannerService>();
            services.AddScoped<IPAdESService, ChilkatPAdESService>();

            return services;
        }
    }
}
