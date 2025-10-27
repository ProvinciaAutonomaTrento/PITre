// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.Conservazione;
using Pi3.Infrastructure.ParER.Services.DigitalPreservation;
using Pi3.Infrastructure.ParER.Services.DigitalPreservation.ValueObjects;
using Pi3.Infrastructure.ParER.Services.Versamento;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.ParER
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureParERDigitalPreservation(this IServiceCollection services,
            string uri, Action<ParEROptions> options)
        {
            services.Configure<ParEROptions>(options);
            services.AddScoped<ISIPService, ParERSIPService>();

            

            services.AddRefitClient<IVersamentoService>(new RefitSettings
            {
                ContentSerializer = new XmlContentSerializer()
            }).ConfigureHttpClient(c => c.BaseAddress = new Uri(uri));


            return services;
        }
    }
}
