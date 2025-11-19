// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Newtonsoft.Json;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Test
{
    public class UnitTestClaimsPrincipalService : IClaimsPrincipalService
    {
        private ClaimsPrincipal _current = null!;

        public ClaimsPrincipal Current
        {
            get
            {
                if (_current == null)
                {   
                    var claimsData = System.Text.Json.JsonSerializer.Deserialize<List<ClaimData>>(Pi3.Core.Test.Resources.Claims);
                    
                    var claimsIdentity = new ClaimsIdentity(
                       authenticationType: "Pi3Authentication",
                        claims: claimsData!.Select(c => new Claim(c.Type, c.Value, c.ValueType, c.Issuer, c.OriginalIssuer)));

                    _current = new ClaimsPrincipal(claimsIdentity);
                }

                return _current;
            }
        }
    }

    public class ClaimData
    {
        public string Type { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string ValueType { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string OriginalIssuer { get; set; } = null!;
    }
}