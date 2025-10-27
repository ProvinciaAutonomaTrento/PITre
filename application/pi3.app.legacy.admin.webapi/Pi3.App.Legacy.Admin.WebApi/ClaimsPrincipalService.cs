// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.Principal;
using System.Security.Claims;

namespace Pi3.App.Legacy.Admin.WebApi;

public class ClaimsPrincipalService : IClaimsPrincipalService
{
    #region Public Members

    public ClaimsPrincipalService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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
                        new Claim(Pi3ClaimTypes.IdUser, "SUPERADMIN"),
                        new Claim(Pi3ClaimTypes.Instance, "qui se riesci a mettere l'istanza fornita nell'header, dovresti riuscirci con _httpContextAccessor")
                    });

                _current = new ClaimsPrincipal(claimsIdentity);
            }

            return _current;
        }
    }

    #endregion

    #region Private Members

    private ClaimsPrincipal _current;
    private readonly IHttpContextAccessor _httpContextAccessor;

    #endregion
}
