// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Http;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

namespace Pi3.App.Legacy.Mobile.Data.Services;
public class HttpClaimsPrincipalService( IHttpContextAccessor httpContextAccessor ) : IClaimsPrincipalService
{
    protected readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public ClaimsPrincipal Current
    {
        get
        {
            this._httpContextAccessor.HttpContext?.User.AssertPi3IdentityAuthenticated();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return this._httpContextAccessor.HttpContext.User;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }

}
