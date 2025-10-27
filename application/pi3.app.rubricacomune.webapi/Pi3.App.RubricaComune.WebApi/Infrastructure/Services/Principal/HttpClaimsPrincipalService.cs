// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using System.Dynamic;
using System.Security.Claims;

namespace Pi3.App.RubricaComune.WebApi.Infrastructure.Services.Principal
{
    public class HttpClaimsPrincipalService : IClaimsPrincipalService
    {
        #region Public Members

        public HttpClaimsPrincipalService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal Current
        {
            get
            {
                this._httpContextAccessor.HttpContext?.User.AssertPi3IdentityAuthenticated();

                return this._httpContextAccessor.HttpContext?.User ?? throw new InvalidOperationException("Utente non autenticato o HttpContext non disponibile.");
            }
        }

        #endregion

        #region Private Members

        protected readonly IHttpContextAccessor _httpContextAccessor;

        #endregion
    }
}
