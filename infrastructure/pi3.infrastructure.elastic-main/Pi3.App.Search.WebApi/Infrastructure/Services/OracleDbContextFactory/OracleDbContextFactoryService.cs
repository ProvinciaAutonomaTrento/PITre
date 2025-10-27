// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.Infrastructure.Legacy.EF.Oracle;

namespace Pi3.App.Search.WebApi.Infrastructure.Services.OracleDbContextFactory
{
    public class OracleDbContextFactoryService : IOracleDbContextFactoryService
    {
        public OracleDbContextFactoryService(ILogger<OracleDbContextFactoryService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IOracleDbContextFactoryServiceOptions options)
        {
            this._logger = logger;
            this._serviceProvider = serviceProvider;
            this._configuration = configuration;
            this._options = options;
        }

        public OraclePi3DbContext CreateDbContext()
        {
            var connectionString = this._configuration.GetConnectionString(this._options.InstanceName);

            this._logger.LogInformation($"InstanceName: {this._options.InstanceName} - ConnectionString: {connectionString}");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ConnectionStringNotFoundPi3Exception(this._options.InstanceName);

            return new OraclePi3DbContext(
                this._serviceProvider.GetService<ILogger<OraclePi3DbContext>>(),
                Options.Create<OraclePi3DbContextOptions>(new OraclePi3DbContextOptions()
                {
                    ConnectionString = connectionString,
                    EnableLogging = this._options.EnableLogging,
                    UseOracleSQLCompatibility = this._options.UseOracleSQLCompatibility
                }));
        }

        protected readonly ILogger<OracleDbContextFactoryService> _logger;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IConfiguration _configuration;
        protected readonly IOracleDbContextFactoryServiceOptions _options;
    }
}
