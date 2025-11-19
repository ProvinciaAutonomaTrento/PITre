// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Dynamic;
using System.Security.Claims;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Infrastructure.Services.Principal
{
    public class HttpClaimsPrincipalService : IClaimsPrincipalService
    {
        #region Public Members

        public HttpClaimsPrincipalService(IHttpContextAccessor httpContextAccessor, IPi3DbContext dbContext)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal Current
        {
            get
            {
                this._httpContextAccessor.HttpContext.User.AssertPi3IdentityAuthenticated();

                return this._httpContextAccessor.HttpContext.User;
            }
        }

        #endregion

        #region Private Members

        protected readonly IHttpContextAccessor _httpContextAccessor;

        #endregion
    }
}
