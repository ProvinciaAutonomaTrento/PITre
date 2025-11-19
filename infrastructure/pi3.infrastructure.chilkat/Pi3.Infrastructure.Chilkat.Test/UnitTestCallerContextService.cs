// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Chilkat.Test
{
    public class UnitTestClaimsPrincipalService : IClaimsPrincipalService
    {
        private ClaimsPrincipal _current = null;

        public ClaimsPrincipal Current
        {
            get
            {
                if (_current == null)
                    _current = new ClaimsPrincipal(new ClaimsIdentity(
                        authenticationType: "Pi3Authentication",
                        claims: new List<Claim>()
                        {
                            new Claim(Pi3ClaimTypes.IdUser, "132442192"),
                            new Claim(Pi3ClaimTypes.IdGroup, "1031884"),
                            new Claim(Pi3ClaimTypes.IdTenant, "361"),
                            new Claim(Pi3ClaimTypes.Authorization, "DO_LETT")
                        }));

                return _current;
            }
        }
    }
}
