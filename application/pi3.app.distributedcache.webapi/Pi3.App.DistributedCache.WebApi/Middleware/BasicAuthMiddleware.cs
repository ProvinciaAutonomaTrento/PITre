// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.DistributedCache.WebApi.Infrastructure.Services;
using Pi3.App.DistributedCache.WebApi.Resources;
using Pi3.Core.SeedWork;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Claims;
using System.Text;

namespace Pi3.App.DistributedCache.WebApi.Middleware
{
    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public BasicAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthService authService,
            ILogger<BasicAuthMiddleware> logger)
        {
            var authHeaderValue = context.Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(authHeaderValue))
                throw new BadRequestPi3Exception(ErrorDescriptions.MissingAuthorizationParameter, ErrorDescriptions.ResourceManager, "Authorization");

            if (AuthenticationHeaderValue.TryParse(authHeaderValue, out AuthenticationHeaderValue? authHeader))
            {
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                var username = credentials[0];
                var password = credentials[1];

                if (await authService.Authenticate(username, password))
                {
                    context.User = new System.Security.Claims.ClaimsPrincipal(
                        new ClaimsIdentity(
                            authenticationType: "SimpleIdentity",
                            claims: new Claim[1]
                            {
                        new Claim("User", username)
                            }));
                    
                    await _next(context);
                }
                else
                {
                    throw new Pi3.Core.SeedWork.UnauthorizedPi3Exception(ErrorDescriptions.UnauthorizedUser, ErrorDescriptions.ResourceManager);
                }
            }
        }
    }
}
