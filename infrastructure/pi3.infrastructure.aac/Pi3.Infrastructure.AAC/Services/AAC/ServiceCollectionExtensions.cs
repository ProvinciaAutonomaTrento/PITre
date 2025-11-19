// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Pi3.Infrastructure.Services.AAC
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
                            await ctx.Response.WriteAsync(ErrorDescriptions.AccessDenied);
                        }
                    };
                });

            services.AddAuthorization(options =>
                {
                    options.AddPolicy(Policies.PITRE, policy =>
                      policy.AddAuthenticationSchemes("Bearer")
                      .RequireAuthenticatedUser()
                      .RequireRole(new[] { "pitre:ROLE_PITRE_BACKEND" })
                      .Build()
                    );
                });

            return services;
        }
    }
}
