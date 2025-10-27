// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.Infrastructure.Services.AAC
{
    public class JwkOptions
    {
        public string Issuer { get; private set; }
        public Uri JwksUri { get; private set; }
        public TimeSpan KeepFor { get; private set; }

        public JwkOptions(string jwksUri)
        {
            JwksUri = new Uri(jwksUri);
            Issuer = $"{JwksUri.Scheme}://{JwksUri.Authority}";
            KeepFor = TimeSpan.FromMinutes(15);
        }

        public JwkOptions(string jwksUri, TimeSpan cacheTime)
        {
            JwksUri = new Uri(jwksUri);
            Issuer = $"{JwksUri.Scheme}://{JwksUri.Authority}";
            KeepFor = cacheTime;
        }

        public JwkOptions(string jwksUri, string issuer, TimeSpan cacheTime)
        {
            JwksUri = new Uri(jwksUri);
            Issuer = issuer;
            KeepFor = cacheTime;
        }
    }
}
