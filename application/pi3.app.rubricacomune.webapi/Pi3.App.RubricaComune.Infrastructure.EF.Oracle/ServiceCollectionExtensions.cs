// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.App.RubricaComune.Infrastructure.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Infrastructure.EF.Oracle
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureLegacyRubricaComuneEFOracleDbContext(this IServiceCollection services, Action<OracleRubricaComuneDbContextOptions> options)
        {
            services.AddInfrastructureLegacyRubricaComuneEF();

            services.Configure(options);
            services.AddScoped<IRubricaComuneDbContext, OracleRubricaComuneDbContext>();

            return services;
        }
    }
}
