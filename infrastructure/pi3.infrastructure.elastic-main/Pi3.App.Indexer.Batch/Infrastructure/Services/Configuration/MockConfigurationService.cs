// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Indexer.Infrastructure.Services.Configuration
{
    public class MockConfigurationService : Pi3.Infrastructure.Legacy.EF.Services.Configuration.ConfigurationEFService
    {
        public MockConfigurationService(ILogger<MockConfigurationService> logger, IClaimsPrincipalService claimsPrincipalService, IDistributedCache distributedCache, IPi3DbContext dbContext)
            : base(logger, claimsPrincipalService, distributedCache, dbContext)
        { }

        public async override Task<T> GetValue<T>(string key, bool? throwIfNotExists = false, T? defaultValue = default) where T : default
        {
            //if (key == "REPOSITORY_ROOT_PATH")
            //    return (T)Convert.ChangeType(@"C:\Dev\LuceneIndex\", typeof(T));
            //else
            return await base.GetValue(key, throwIfNotExists, defaultValue);
        }
    }
}
