// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Pi3.App.Legacy.Interop.WebApi.Infrastructure.Services.AAC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureAAC(this IServiceCollection services, string urlJWK)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.SaveToken = true;
                    opt.SetJwksOptions(new JwkOptions(urlJWK));
                    opt.Events = new JwtBearerEvents
                    {
                        OnForbidden = async (ctx) =>
                        {
                            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                            ctx.Response.ContentType = "text/plain";
                            await ctx.Response.WriteAsync("Your Role does not allow the requested operation");
                        }
                    };
                });

            return services;
        }
    }
}
