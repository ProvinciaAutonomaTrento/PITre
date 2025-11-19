// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.SessionRepository
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSessionRepositoryService(this IServiceCollection services)
        {
            services.AddScoped<ISessionRepositoryService, SessionRepositoryService>();

            return services;
        }
    }
}
