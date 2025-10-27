// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Options;
using System.Collections;

namespace Pi3.App.DistributedCache.WebApi.Infrastructure.Services
{
    public class BasicAuthService : IAuthService
    {
        private readonly ILogger<BasicAuthService> _logger;
        private readonly IOptions<BasicAuthServiceOptions> _options;

        public BasicAuthService(
            ILogger<BasicAuthService> logger,
            IOptions<BasicAuthServiceOptions> options)
        {
            this._logger = logger;
            this._options = options;
        }

        public async Task<bool> Authenticate(string userName, string password)
        {
            this._logger.LogInformation($"UserName: {userName} - UserName server: {this._options.Value.UserName}");

            return userName.Equals(this._options.Value.UserName, StringComparison.OrdinalIgnoreCase)
                    && password.Equals(this._options.Value.Password, StringComparison.Ordinal);
        }
    }
}
