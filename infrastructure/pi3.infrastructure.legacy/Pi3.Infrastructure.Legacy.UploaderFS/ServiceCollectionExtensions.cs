// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core.Services.File.Uploader;
using Pi3.Infrastructure.Legacy.UploaderFS.Services.File.Uploader;

namespace Pi3.Infrastructure.Legacy.UploaderFS
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureFSUploaderService(this IServiceCollection services)
        {
            services.RemoveAll<IUploaderService>().AddScoped<IUploaderService, FSUploaderService>();

            return services;
        }
    }
}
