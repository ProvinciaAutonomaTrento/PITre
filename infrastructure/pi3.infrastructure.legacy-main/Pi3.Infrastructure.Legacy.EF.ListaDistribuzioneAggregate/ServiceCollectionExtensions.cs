// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.ListaDistribuzioneAggregate.Repository;

namespace Pi3.Infrastructure.Legacy.EF
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureLegacyEFListaDistribuzioneAggregate(
            this IServiceCollection services)
        {
            services.AddPi3Core();

            services.RemoveAll<IListaDistribuzioneRepository>().AddScoped<IListaDistribuzioneRepository, ListaDistribuzioneEFRepository>();

            return services;
        }
    }
}