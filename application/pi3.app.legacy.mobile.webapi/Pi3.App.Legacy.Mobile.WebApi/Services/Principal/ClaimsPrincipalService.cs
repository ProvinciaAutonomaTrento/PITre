// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.Principal;
using System.Security.Claims;

namespace Pi3.App.Legacy.Mobile.WebApi.Services.Principal
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
