// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Google.Apis.Auth.AspNetCore3;
using Microsoft.Extensions.Configuration;

namespace Pi3.App.Legacy.WebApi.Application.Services.Google
{
    public class AuthProviderFactoryService : IAuthProviderFactoryService
    {
        private readonly IConfiguration _configuration;

        public AuthProviderFactoryService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public AuthProvider Create(string instance, string codiceAmministrazione, string codiceRegistro)
        {
            var configBasePath = $"GoogleOptions:{instance}-{codiceAmministrazione}-{codiceRegistro}";

            var clientId = this._configuration[$"{configBasePath}:ClientId"];

            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentNullException(nameof(clientId));

            var clientSecret = this._configuration[$"{configBasePath}:ClientSecret"];

            if (string.IsNullOrWhiteSpace(clientSecret))
                throw new ArgumentNullException(nameof(clientSecret));

            var googleAuthProvider = new AuthProvider(clientId!, clientSecret!);
            return googleAuthProvider;
        }
    }
}
