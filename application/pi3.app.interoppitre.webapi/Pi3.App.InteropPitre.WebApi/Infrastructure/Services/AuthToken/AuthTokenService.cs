// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Primitives;
using Refit;

namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.AuthToken
{
    public class AuthTokenService : IAuthToken
    {
        private readonly string BearerPrefix = "Bearer ";
        private IHttpContextAccessor _httpContextAccessor;

        public AuthTokenService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }
        public string? GetAuthenticationToken()
        {
            if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            var authorizationHeader = authorizationStrings[0]!.StartsWith(BearerPrefix) ? authorizationStrings[0]!.Substring(BearerPrefix.Length) : authorizationStrings[0];

            return authorizationHeader;
        }
    }
}
