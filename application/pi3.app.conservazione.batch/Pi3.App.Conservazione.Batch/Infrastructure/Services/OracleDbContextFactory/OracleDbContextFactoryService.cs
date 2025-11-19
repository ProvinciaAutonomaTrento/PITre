// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.Infrastructure.Legacy.EF.Oracle;

namespace Pi3.App.Conservazione.Batch.Infrastructure.Services.OracleDbContextFactory
{
    public class OracleDbContextFactoryService : IOracleDbContextFactoryService
    {
        public OracleDbContextFactoryService(ILogger<OracleDbContextFactoryService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IOptions<OracleDbContextFactoryServiceOptions> options)
        {
            this._logger = logger;
            this._serviceProvider = serviceProvider;
            this._configuration = configuration;
            this._options = options;
        }
        public OraclePi3DbContext CreateDbContext()
        {
            var instanceProvicer = this._serviceProvider.GetRequiredService<IInstanceProvider>();

            var instance = instanceProvicer.Instance;

            var connectionString = this._configuration.GetConnectionString(instance);

            this._logger.LogDebug($"Instance: {instance} - ConnectionString: {connectionString}");

            return new OraclePi3DbContext(
                this._serviceProvider.GetService<ILogger<OraclePi3DbContext>>(),
                Options.Create<OraclePi3DbContextOptions>(new OraclePi3DbContextOptions()
                {
                    ConnectionString = connectionString,
                    EnableLogging = this._options.Value.EnableLogging,
                    UseOracleSQLCompatibility = this._options.Value.UseOracleSQLCompatibility
                }));
        }

        protected readonly ILogger<OracleDbContextFactoryService> _logger;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IConfiguration _configuration;
        protected readonly IOptions<OracleDbContextFactoryServiceOptions> _options;
    }
}
