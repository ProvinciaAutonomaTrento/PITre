// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.FirmaDigitale;
using Pi3.Core.Services.File.FirmaDigitale2;
using Pi3.Core.Services.File.FirmaRemota;
using Pi3.Core.Services.File.FirmaRemota2;
using Pi3.Core.Services.File.MarcaTemporale;
using Pi3.Core.Services.File.SigilloElettronico;
using Pi3.Core.Services.File.TextExtractors;
using Pi3.Infrastructure.Tibco.Services.File.FirmaDigitale;
using Pi3.Infrastructure.Tibco.Services.File.FirmaDigitale2;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota;
using Pi3.Infrastructure.Tibco.Services.File.FirmaRemota2;
using Pi3.Infrastructure.Tibco.Services.File.MarcaTemporale;
using Pi3.Infrastructure.Tibco.Services.File.SigilloElettronico;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Tibco
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSigilloElettronicoService(
            this IServiceCollection services,
            Action<SigilloElettronicoServiceOptions> options)
        {
            services.Configure<SigilloElettronicoServiceOptions>(options);
            services.AddScoped<ISigilloElettronicoService, SigilloElettronicoService>();

            return services;
        }

        public static IServiceCollection AddFirmaDigitaleService(
            this IServiceCollection services,
            Action<FirmaDigitaleServiceOptions> options)
        {
            services.Configure(options);
            services.AddScoped<IFirmaDigitaleService, FirmaDigitaleService>();

            return services;
        }

        public static IServiceCollection AddFirmaDigitale2Service(
            this IServiceCollection services,
            Action<FirmaDigitale2ServiceOptions> options)
        {
            services.Configure(options);
            services.AddScoped<IFirmaDigitale2Service, FirmaDigitale2Service>();

            return services;
        }

        public static IServiceCollection AddFirmaRemotaService(
            this IServiceCollection services,
            Action<FirmaRemotaServiceOptions> options)
        {
            services.Configure(options);
            services.AddScoped<IFirmaRemotaService, FirmaRemotaService>();

            return services;
        }

        public static IServiceCollection AddFirmaRemota2Service(
            this IServiceCollection services,
            Action<FirmaRemota2ServiceOptions> options)
        {
            services.Configure(options);
            services.AddScoped<IFirmaRemota2Service, FirmaRemota2Service>();

            return services;
        }

        public static IServiceCollection AddMarcaTemporaleService(
            this IServiceCollection services,
            Action<MarcaTemporaleServiceOptions> options)
        {
            services.Configure(options);
            services.AddScoped<IMarcaTemporaleService, MarcaTemporaleService>();

            return services;
        }
    }
}
