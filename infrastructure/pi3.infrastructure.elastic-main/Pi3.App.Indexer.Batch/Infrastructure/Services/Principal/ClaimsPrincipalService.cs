// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.Principal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Pi3.App.Indexer.Infrastructure.Services.Principal
{
    public class ClaimsPrincipalService : IClaimsPrincipalService
    {
        public ClaimsPrincipalService(
            IOptions<ClaimsPrincipalServiceOptions> options,
            ILogger<ClaimsPrincipalService> logger)
        {
            _options = options;
            _logger = logger;
        }

        public ClaimsPrincipal Current
        {
            get
            {
                if (_current == null)
                {
                    var claimsIdentity = new ClaimsIdentity(
                        authenticationType: "Pi3Authentication",
                        claims: new List<Claim>()
                       {
                            new Claim(Pi3ClaimTypes.IdUser, this._options.Value.IdSuperAdminUser),
                            new Claim(Pi3ClaimTypes.SuperAdmin, true.ToString())
                        });

                   _current = new ClaimsPrincipal(claimsIdentity);
                }                

                return _current;
            }
        }

        protected readonly IOptions<ClaimsPrincipalServiceOptions> _options;
        protected readonly ILogger<ClaimsPrincipalService> _logger;
        protected ClaimsPrincipal _current = null;
    }
}