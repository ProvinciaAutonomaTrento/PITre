// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.ParER.Services.Principal
{
    public static class ServiceExtensions
    {
        public static void SetClaim(this ClaimsPrincipal principal, string type, string? value)
        {
            var identity = principal.GetPi3AuthenticationIdentity();

            if (identity is null) return;

            var claim = principal.FindFirst(type);

            if (claim is not null) principal.RemovePi3Claim(type);

            identity.AddClaim(new Claim(type, value));
        }

        private static ClaimsIdentity GetPi3AuthenticationIdentity(this ClaimsPrincipal principal)
        {
            return principal.Identities.Where(x => x.AuthenticationType == "Pi3Authentication").FirstOrDefault();
        }
    }
}
