// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Conservazione.Batch.Infrastructure.Services.Principal
{
    public class ClaimsPrincipalService : IClaimsPrincipalService
    {
        public ClaimsPrincipal Current
        {
            get
            {
                if(_current == null)
                {
                    var claimsIdentity = new ClaimsIdentity(
                        authenticationType: "Pi3Authentication",
                        claims: new List<Claim>()
                        {
                            new Claim(Pi3ClaimTypes.IdUser, string.Empty),
                            new Claim(Pi3ClaimTypes.SuperAdmin, true.ToString())
                        });

                    _current = new ClaimsPrincipal(claimsIdentity);
                }

                return _current;
            }
        }

        protected ClaimsPrincipal _current = null!;
    }
}
