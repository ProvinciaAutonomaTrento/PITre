// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using Pi3.Infrastructure.Legacy.EF.Services.Services.Configuration.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Services.Configuration
{
    public class ConfigurationEFService : IConfigurationService
    {
        #region Public Members

        public ConfigurationEFService(ILogger<ConfigurationEFService> logger, IClaimsPrincipalService claimsPrincipalService, IDistributedCache distributedCache, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._distributedCache = distributedCache;
            this._dbContext = dbContext;
        }

        public virtual async Task<(T?, bool)> TryGetValue<T>(string idTenant, string key)
        {
            idTenant = idTenant ?? throw new ArgumentNullException(nameof(idTenant));
            key = key ?? throw new ArgumentNullException(nameof(key));

            var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);

            var configurations = await this._distributedCache.FromCache<ChiaveConfigurazioneEntity>(
                        instance!,
                        this._dbContext.ChiaviConfigurazioneEntities,
                        c => c.ID_AMM == idTenant.AsLong());

            var configuration = configurations.FirstOrDefault(c => c.VAR_CODICE.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            if (configuration == null)
                return (default(T), false);
            else
                return ((T)Convert.ChangeType(configuration.VAR_VALORE, typeof(T)), true);
        }

        public virtual async Task<T> GetValue<T>(string idTenant, string key, bool? throwIfNotExists = false, T? defaultValue = default)
        {
            idTenant = idTenant ?? throw new ArgumentNullException(nameof(idTenant));
            key = key ?? throw new ArgumentNullException(nameof(key));

            var tryReturn = await this.TryGetValue<T>(idTenant, key);

            if (!tryReturn.Item2)
            {
                if (throwIfNotExists ?? false)
                    throw new ConfigurationKeyNotFoundPi3Exception(key);
                else
                    return defaultValue!;
            }

            return tryReturn.Item1!;
        }

        public virtual async Task<(T?, bool)> TryGetValue<T>(string key)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));

            var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);

            var configurations = await this._distributedCache.FromCache<ChiaveConfigurazioneEntity>(
                        instance!,
                        this._dbContext.ChiaviConfigurazioneEntities,
                        c => c.ID_AMM == 0);

            var configuration = configurations.FirstOrDefault(c => c.VAR_CODICE.Equals(key, StringComparison.InvariantCultureIgnoreCase));

            if (configuration == null)
                return (default(T), false);
            else
                return ((T)Convert.ChangeType(configuration.VAR_VALORE, typeof(T)), true);
        }

        public virtual async Task<T> GetValue<T>(string key, bool? throwIfNotExists = false, T? defaultValue = default)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));

            var tryReturn = await this.TryGetValue<T>(key);
            
            if (!tryReturn.Item2)
            {
                if (throwIfNotExists ?? false)
                    throw new ConfigurationKeyNotFoundPi3Exception(key);
                else
                    return defaultValue!;
            }

            return tryReturn.Item1!;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ConfigurationEFService> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
