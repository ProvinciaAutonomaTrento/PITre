// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.Email.Sender;
using Pi3.Infrastructure.Graph.Services.Email.BoxScanner;
using Pi3.Infrastructure.Graph.Services.Email.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Graph
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureGraph(
            this IServiceCollection services)
        {
            services.AddScoped<IEmailBoxScannerService, GraphEmailBoxScannerService>();
            services.AddScoped<IEmailSenderService, GraphEmailSenderService>();

            return services;
        }
    }
}
