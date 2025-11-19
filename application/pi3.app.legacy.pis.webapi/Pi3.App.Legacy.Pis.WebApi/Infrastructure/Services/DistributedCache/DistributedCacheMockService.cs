// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Refit;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.DistributedCache
{
    public class DistributedCacheMockService : IDistributedCacheService
    {
        public async Task<string> GetVal(FetchValParams par)
        {
            return null!;
        }

        public async Task<string> PutVal([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> data)
        {
            return null!;
        }
    }
}
