// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.Principal;
using System.Security.Claims;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal
{
    public class ClaimsPrincipalService : IClaimsPrincipalService
    {
        public ClaimsPrincipal Current
        {
            get;
            set;
        }
    }
}
