// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Infrastructure.IText.ReportGenerator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.IText.ReportGenerator
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureITextReportGenerator(this IServiceCollection services)
        {
            services.RemoveAll<IReportGeneratorService>()
                .AddScoped<IReportGeneratorService, ITextReportGeneratorService>();

            return services;
        }
    }
}
