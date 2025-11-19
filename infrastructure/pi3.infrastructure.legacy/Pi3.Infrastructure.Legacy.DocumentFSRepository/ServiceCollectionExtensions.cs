// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Infrastructure.Legacy.DocumentFSRepository.AggregateModels.DocumentBlobAggregate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;

namespace Pi3.Infrastructure.Legacy.DocumentFSRepository
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureLegacyDocumentBlobFileSystemRepository(this IServiceCollection services)
        {
            services.AddPi3Core();

            services.RemoveAll<IDocumentBlobRepository>().AddScoped<IDocumentBlobRepository, DocumentBlobFileSystemRepository>();

            return services;
        }
    }
}
