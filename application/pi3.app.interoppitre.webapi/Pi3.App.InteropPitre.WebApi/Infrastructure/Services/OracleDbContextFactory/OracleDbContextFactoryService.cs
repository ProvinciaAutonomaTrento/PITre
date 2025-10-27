// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Oracle;

namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.OracleDbContextFactory
{
    public class OracleDbContextFactoryService : IOracleDbContextFactoryService
    {
        public OracleDbContextFactoryService(ILogger<OracleDbContextFactoryService> logger, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration,
            IOracleDbContextFactoryServiceOptions options, 
            IClaimsPrincipalService claimsPrincipalService,
            IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._serviceProvider = serviceProvider;
            this._configuration = configuration;
            this._options = options;
            this._claimsPrincipalService = claimsPrincipalService;
            this._httpContextAccessor = httpContextAccessor;
        }

        public OraclePi3DbContext CreateDbContext()
        {
            string instance = "";

            StringValues headerInstance = this._httpContextAccessor.HttpContext?.GetRouteValue("Instance")!.ToString() ?? string.Empty;
            if (headerInstance != StringValues.Empty)
                instance = headerInstance.FirstOrDefault() ?? String.Empty;

            if (string.IsNullOrEmpty(instance))
                instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, false);

            var connectionString = this._configuration.GetConnectionString(instance);


            this._logger.LogInformation($"InstanceName: {instance} - ConnectionString: {connectionString}");

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
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
    }
}
