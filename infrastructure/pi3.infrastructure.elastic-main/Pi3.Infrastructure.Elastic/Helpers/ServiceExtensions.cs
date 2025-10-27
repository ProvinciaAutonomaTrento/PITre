// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Doc.Application.Search.DocumentoAmministrativo.Commands;
using Doc.Search.DocumentoAmministrativo;
using Microsoft.Extensions.DependencyInjection;
using pi3.Core.Contracts.DocumentoAmministrativo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Elastic.Helpers
{
    public static class ServiceExtensions
    {
        public static async Task AddInfrastructureElasticSearch(this IServiceCollection services)
        {
            services.AddScoped<IIndexingService, ElasticIndexerService>();
            services.AddScoped<ISearchService, ElasticQueryService>();
            services.AddScoped<ElasticQueryService>();
            //services.AddMediatR((config) => {
            //    config.RegisterServicesFromAssembly(typeof(DeleteDocumentoAmministrativoCommand).Assembly);
            //});
        }

    }
}
