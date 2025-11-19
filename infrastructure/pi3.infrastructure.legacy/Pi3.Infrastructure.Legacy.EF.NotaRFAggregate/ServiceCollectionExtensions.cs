// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core;
using Pi3.Core.AggregateModels.NotaRFAggregate;
using Pi3.Core.AggregateModels.NotaRFAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaRFAggregate.Repository;

namespace Pi3.Infrastructure.Legacy.EF
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureLegacyEFNotaRFAggregate(
            this IServiceCollection services)
        {
            services.AddPi3Core();

            services.RemoveAll<INotaRFRepository>().AddScoped<INotaRFRepository, NotaRFEFRepository>();

            return services;
        }
    }
}