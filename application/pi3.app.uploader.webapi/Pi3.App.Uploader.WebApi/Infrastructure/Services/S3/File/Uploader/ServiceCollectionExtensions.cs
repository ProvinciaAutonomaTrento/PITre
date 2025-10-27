// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using Microsoft.Extensions.Options;
//using Pi3.App.Uploader.WebApi.Infrastructure.Services.File.Uploader;

//namespace Pi3.App.Uploader.WebApi.Infrastructure.Services.S3.File.Uploader
//{
//    public static class ServiceCollectionExtensions
//    {
//        public static IServiceCollection AddInfrastructureS3(this IServiceCollection services, Action<S3UploaderServiceOptions> options)
//        {
//            services.Configure<S3UploaderServiceOptions>(options);
//            services.RemoveAll<IUploaderService>().AddScoped<IUploaderService, S3UploaderService>();

//            return services;
//        }
//    }
//}
