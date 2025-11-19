// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.InteropPitre.WebApi.Infrastructure.Services.OracleDbContextFactory
{
    public class OracleDbContextFactoryServiceOptions : IOracleDbContextFactoryServiceOptions
    {
        public OracleDbContextFactoryServiceOptions(
            ILogger<OracleDbContextFactoryServiceOptions> logger, 
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            this._logger = logger;
            this._httpContextAccessor = httpContextAccessor;
            this._configuration = configuration;
        }

        public string InstanceName
        {
            get
            {
                return (this._httpContextAccessor.HttpContext.GetRouteValue("Instance") ?? string.Empty).ToString();
            }
        }

        public bool EnableLogging
        {
            get
            {
                return this._configuration.GetSection("OracleDbContextFactoryServiceOptions:EnableLogging").Get<bool>();
            }
        }

        public string UseOracleSQLCompatibility
        {
            get
            {
                return this._configuration.GetSection("OracleDbContextFactoryServiceOptions:UseOracleSQLCompatibility").Get<string>();
            }
        }

        protected readonly ILogger<OracleDbContextFactoryServiceOptions> _logger;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IConfiguration _configuration;
    }
}
