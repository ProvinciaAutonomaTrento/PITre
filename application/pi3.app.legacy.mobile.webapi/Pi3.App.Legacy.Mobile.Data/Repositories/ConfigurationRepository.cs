// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Linq;

namespace Pi3.App.Legacy.Mobile.Data.Repositories;
public class ConfigurationRepository( 
    IPi3DbContext pi3DbContext,
    IConfigurationService pi3ConfigurationService,
    IClaimsPrincipalService claimsPrincipalService ) : IConfigurationRepository
{
    readonly IPi3DbContext _pi3DbContext = pi3DbContext;
    readonly IConfigurationService _pi3ConfigurationService = pi3ConfigurationService;
    readonly IClaimsPrincipalService _claimsPrincipalService = claimsPrincipalService;

    public async Task<string?> GetChiaveByIdAmmAsync(string chiave, long idAmministrazione, CancellationToken cancellationToken)
    {
        return await this._pi3DbContext.ChiaviConfigurazioneEntities.AsNoTracking()
            .Where(c => c.VAR_CODICE == chiave && c.ID_AMM == idAmministrazione)
            .Select(c => c.VAR_VALORE)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<string> GetConfigurationValue(string key)
    {
        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
        return await this._pi3ConfigurationService.GetValue<string>(idTenant.ToString(), key);
    }
}
