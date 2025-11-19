// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.DependencyInjection;
using Pi3.Core.Services.File.Decorators;
using Pi3.Infrastructure.IText.Decorator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.IText.Decorator
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureITextFileDecorator(this IServiceCollection services)
        {
            services.AddScoped<IFileDecoratorService, ITextFileDecoratorService>();

            return services;
        }
    }
}
