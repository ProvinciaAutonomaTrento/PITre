// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Doc.Application.Search.DocumentoAmministrativo.Commands;
using Doc.Search.DocumentoAmministrativo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Doc.Application.Search.DocumentoAmministrativo.Helpers
{
    public static class ServiceExtensions
    {
        public static async  Task AddInfrastructureDocumentoAmministrativo(this IServiceCollection services)
        {
            services.AddScoped<DocumentoAmministrativoIndexingCommandHandler>();
            services.AddMediatR((config) => { 
                config.RegisterServicesFromAssembly(typeof(DeleteDocumentoAmministrativoCommand).Assembly);
            });
            services.AddScoped<DocumentoAmministrativoIndexingService>();
            services.AddScoped<DocumentoAmministrativoSearchService>();
        }
    }
}
