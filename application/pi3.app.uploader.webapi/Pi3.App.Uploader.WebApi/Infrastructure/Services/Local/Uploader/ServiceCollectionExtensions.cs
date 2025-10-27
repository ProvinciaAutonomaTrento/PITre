// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Pi3.App.Uploader.WebApi.Infrastructure.Services.File.Uploader;
using Pi3.App.Uploader.WebApi.Infrastructure.Services.Local.Uploader;

namespace Pi3.App.Uploader.WebApi.Infrastructure.Services.Local.File.Uploader
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureLocalUpload(this IServiceCollection services)
        {
            services.RemoveAll<IUploaderService>().AddScoped<IUploaderService, FileUploaderService>();

            return services;
        }
    }
}
