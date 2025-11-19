// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Pi3.Infrastructure.Services.AAC
{
    public static class JwkExtension
    {
        public static void SetJwksOptions(this JwtBearerOptions options, JwkOptions jwkOptions)
        {
            var httpClient = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler())
            {
                Timeout = options.BackchannelTimeout,
                MaxResponseContentBufferSize = 1024 * 1024 * 10 // 10 MB 
            };

            options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                jwkOptions.JwksUri.OriginalString,
                new JwkRetriever(),
                new HttpDocumentRetriever(httpClient) { RequireHttps = options.RequireHttpsMetadata });
            options.TokenValidationParameters.ValidateAudience = false;
            options.TokenValidationParameters.ValidIssuer = jwkOptions.Issuer;
            options.TokenValidationParameters.ClockSkew = new TimeSpan(200);// TimeSpan( TimeSpan.Zero;
            options.TokenValidationParameters.RoleClaimType = "roles";
            options.TokenValidationParameters.ValidateLifetime = true;
            options.TokenValidationParameters.LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
            {
                return notBefore <= DateTime.UtcNow.AddSeconds(60) &&
                       expires > DateTimeUtil.Add(DateTime.UtcNow, validationParameters.ClockSkew.Negate());
            };
        }

    }
}
