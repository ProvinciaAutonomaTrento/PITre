// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Refit;
using System.Text.Json.Serialization;

namespace Pi3.App.Legacy.Mobile.WebApi.Services.AAC
{
    public interface IAacFetcherService
    {
        [Post("/oauth/token")]
        Task<AacResponse> GetToken([Body(BodySerializationMethod.UrlEncoded)] AacRequestBody data);
    }

    public class AacResponse
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }
        [JsonPropertyName("scope")]
        public string Scope { get; set; }
    }

    public class AacRequestBody
    {
        [AliasAs("grant_type")]
        public string GrantType { get; set; }
        [AliasAs("client_id")]
        public string ClientId { get; set; }
        [AliasAs("client_secret")]
        public string ClientSecret { get; set; }
        [AliasAs("scope")]
        public string Scope { get; set; }
    }
}
