// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.StampaRegistri.Batch.Infrastructure.Services.Configuration
{
    public class MockConfigurationService : Pi3.Infrastructure.Legacy.EF.Services.Configuration.ConfigurationEFService
    {
        public MockConfigurationService(ILogger<MockConfigurationService> logger, IClaimsPrincipalService claimsPrincipalService, IDistributedCache distributedCache, IPi3DbContext dbContext)
            : base(logger, claimsPrincipalService, distributedCache, dbContext)
        { }

        public async override Task<T> GetValue<T>(string key, bool? throwIfNotExists = false, T? defaultValue = default) where T : default
        {
            switch (key)
            {
                case "REPOSITORY_ROOT_PATH":
                    return (T)Convert.ChangeType(@"C:\Lavoro\Logs\PiTre\DOC_PAT\DocServer", typeof(T));
                case "BE_TEMP_PATH":
                    return (T)Convert.ChangeType(@"C:\Lavoro\Logs\", typeof(T));
                default:
                    return await base.GetValue(key, throwIfNotExists, defaultValue);
            }
        }
    }
}
