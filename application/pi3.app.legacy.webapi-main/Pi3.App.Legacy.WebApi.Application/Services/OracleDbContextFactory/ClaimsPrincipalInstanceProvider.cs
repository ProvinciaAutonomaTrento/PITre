// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.Principal;

namespace Pi3.App.Legacy.WebApi.Application.Services.OracleDbContextFactory
{
    public class ClaimsPrincipalInstanceProvider : IInstanceProvider
    {
        protected readonly IClaimsPrincipalService _claimsPrincipalService;

        public ClaimsPrincipalInstanceProvider(IClaimsPrincipalService claimsPrincipalService)
        {
            this._claimsPrincipalService = claimsPrincipalService;
        }

        public string Instance
        {
            get
            {
                var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, false);
                
                if (instance == null)
                    throw new InstanceNotFoundPi3Exception();

                return (instance ?? string.Empty).ToString()!;
            }
        }
    }
}
