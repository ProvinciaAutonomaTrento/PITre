// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Pi3.Infrastructure.DocumentFormat.OpenXml.Services.ReportGenerator;
using Pi3.Infrastructure.DocumentFormat.OpenXml.Services.Spreadsheet;

namespace Pi3.Infrastructure.DocumentFormat.OpenXml
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureOpenXmlSpreadsheetService(this IServiceCollection services)
        {
            services.AddPi3Core();

            services.RemoveAll<ISpreadsheetService>()
                .AddScoped<ISpreadsheetService, OpenXmlShreadsheetService>();

            return services;
        }

        public static IServiceCollection AddInfrastructureOpenXmlReportGeneratorService(this IServiceCollection services)
        {
            services.AddPi3Core();

            services.RemoveAll<IReportGeneratorService>()
                .AddScoped<IReportGeneratorService, OpenXmlReportGeneratorService>();

            return services;
        }
    }
}
