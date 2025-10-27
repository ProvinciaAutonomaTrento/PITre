// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Refit;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.DistributedCache
{
    [Headers("Authorization: Basic")]
    public interface IDistributedCacheService
    {
        [Get("/DistributedCache/api/v1/Values")]
        Task<string> GetVal(FetchValParams par);

        [Put("/DistributedCache/api/v1/Values")]
        Task<string> PutVal([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> data);
    }

    public class FetchValParams
    {
        [AliasAs("key")]
        public string Key { get; set; }
    }
}
