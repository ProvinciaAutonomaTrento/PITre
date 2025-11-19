// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.Data.Services
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
