// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using System.Security.Claims;

namespace Pi3.App.RubricaComune.WebApi.Middleware
{
    public class ClaimsPrincipalActivatorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ClaimsPrincipalActivatorMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ClaimsPrincipalActivatorMiddleware>();
        }

        public async Task Invoke(HttpContext context,
            ILogger<ClaimsPrincipalActivatorMiddleware> logger)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(Pi3ClaimTypes.IdUser, "1"));
            claims.Add(new Claim(Pi3ClaimTypes.UserName, "sa"));

            var claimsIdentity = new ClaimsIdentity(
                authenticationType: "Pi3Authentication",
                claims: claims);

            context.User.AddIdentity(claimsIdentity);

            await _next(context);
        }
    }
}
