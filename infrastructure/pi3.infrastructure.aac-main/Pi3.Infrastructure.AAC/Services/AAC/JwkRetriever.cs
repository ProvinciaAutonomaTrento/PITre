// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Pi3.Infrastructure.Services.AAC
{
    public class JwkRetriever : IConfigurationRetriever<OpenIdConnectConfiguration>
    {
        public Task<OpenIdConnectConfiguration> GetConfigurationAsync(string address, IDocumentRetriever retriever, CancellationToken cancel)
        {
            return GetAsync(address, retriever, cancel);
        }

        public static async Task<OpenIdConnectConfiguration> GetAsync(string address, IDocumentRetriever retriever, CancellationToken cancel)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw LogHelper.LogArgumentNullException(nameof(address));

            if (retriever == null)
                throw LogHelper.LogArgumentNullException(nameof(retriever));

            var doc = await retriever.GetDocumentAsync(address, cancel).ConfigureAwait(false);

            var jwks = new JsonWebKeySet(doc);
            var openIdConnectConfiguration = new OpenIdConnectConfiguration()
            {
                JsonWebKeySet = jwks,
                JwksUri = address,
            };
            foreach (var securityKey in jwks.GetSigningKeys())
                openIdConnectConfiguration.SigningKeys.Add(securityKey);

            return openIdConnectConfiguration;
        }
    }
}
