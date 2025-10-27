// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Mobile.Shared.Exceptions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.Data.Context;
public class OracleDbContextFactoryService( 
    ILogger<OracleDbContextFactoryService> logger, 
    IServiceProvider serviceProvider, 
    IConfiguration configuration, 
    IHttpContextAccessor httpContextAccessor,
    IClaimsPrincipalService claimsPrincipalService,
    IOptions<OracleDbContextFactoryServiceOptions> options ) : IOracleDbContextFactoryService
{
    protected readonly ILogger<OracleDbContextFactoryService> _logger = logger;
    protected readonly IServiceProvider _serviceProvider = serviceProvider;
    protected readonly IConfiguration _configuration = configuration;
    protected readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    protected readonly IOptions<OracleDbContextFactoryServiceOptions> _options = options;
    protected readonly IClaimsPrincipalService _claimsPrincipalService = claimsPrincipalService;

    public OraclePi3DbContext CreateDbContext()
    {
        string instance = "";
        StringValues headerInstance = this._httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("Instance", out StringValues value) ?? false ? value : StringValues.Empty;
        if ( headerInstance != StringValues.Empty )
            instance = headerInstance.FirstOrDefault() ?? String.Empty;

        if (string.IsNullOrEmpty(instance))
            instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, false);

        string connectionString = this._configuration.GetConnectionString(instance) ?? throw new ConnectionStringNotFoundPi3Exception();

        this._logger.LogDebug("Instance: {Instance} - ConnectionString: {Connection}", instance, connectionString);

        return new OraclePi3DbContext(
            this._serviceProvider.GetService<ILogger<OraclePi3DbContext>>()!,
            Options.Create<OraclePi3DbContextOptions>(new OraclePi3DbContextOptions()
            {
                ConnectionString = connectionString,
                EnableLogging = this._options.Value.EnableLogging,
                UseOracleSQLCompatibility = this._options.Value.UseOracleSQLCompatibility
            }));
    }
}
