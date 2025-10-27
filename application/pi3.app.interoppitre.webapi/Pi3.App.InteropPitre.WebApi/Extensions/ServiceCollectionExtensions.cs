// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Pi3.Infrastructure.Services.AAC;
using System.IdentityModel.Tokens.Jwt;

namespace Pi3.App.InteropPitre.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureAACFake(this IServiceCollection services, string urlJWK)
        {


            return services;
        }
    }
}
