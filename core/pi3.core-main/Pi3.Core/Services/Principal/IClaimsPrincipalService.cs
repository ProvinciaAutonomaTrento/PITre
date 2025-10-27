// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Principal
{
    public interface IClaimsPrincipalService : IService
    {
        ClaimsPrincipal Current
        {
            get;
        }
    }
}
