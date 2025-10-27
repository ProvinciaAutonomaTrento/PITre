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
using Pi3.App.StampaRepertori.Batch.Infrastructure.Services.OracleDbContextFactory;

namespace Pi3.App.StampaRepertori.Batch.Services.Principal
{
    public class ClaimsPrincipalService : IClaimsPrincipalService
    {
        public ClaimsPrincipalService(IInstanceProvider instanceProvider)
        {
            this._instanceProvider = instanceProvider;
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
                            new Claim(Pi3ClaimTypes.Instance, this._instanceProvider.Instance)
                        });

                    _current = new ClaimsPrincipal(claimsIdentity);
                }

                return _current;
            }
        }

        protected readonly IInstanceProvider _instanceProvider;
        protected ClaimsPrincipal _current = null!;
    }
}