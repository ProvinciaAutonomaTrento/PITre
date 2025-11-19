// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Refit;
using System.Text.Json.Serialization;

namespace Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Aac
{
    public interface IAacFetcherService
    {
        [Post("/oauth/token")]
        Task<AacResponse> GetToken([Body(BodySerializationMethod.UrlEncoded)] AacRequestBody data);
    }

    public class AacMockFetcherService : IAacFetcherService
    {
        public async Task<AacResponse> GetToken([Body(BodySerializationMethod.UrlEncoded)] AacRequestBody data)
        {
            return new AacResponse()
            {
                TokenType = "bearer",
                AccessToken = "eyJraWQiOiIxMzE4Njg",
                ExpiresIn = 14399,
                Scope = "client.roles.me"
            };
        }
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
