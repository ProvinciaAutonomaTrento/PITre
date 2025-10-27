// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pi3.App.RubricaComune.Infrastructure.EF.Entities;
using System.Threading;

namespace Pi3.App.RubricaComune.WebApi.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HealthCheckController : Controller
    {
        #region Public Members

        public HealthCheckController(
            ILogger<HealthCheckController> logger,
            IServiceProvider serviceProvider)
        {
            this._logger = logger;
            this._serviceProvider = serviceProvider;
        }

        [HttpGet()]
        [Route("healthz")]
        public async Task<HealthCheckResult> Healthz()
        {
            return HealthCheckResult.Healthy($"Healthz OK");
        }

        [HttpGet()]
        [Route("startup")]
        public async Task<HealthCheckResult> Startup()
        {
            try
            {
                var dbContext = this._serviceProvider.GetRequiredService<IRubricaComuneDbContext>();
                var canConnect = await ((DbContext)dbContext).Database.CanConnectAsync();

                if (!canConnect)
                    throw new ApplicationException($"Non è stato possibile stabilire una connessione con la base dati di RubricaComune");

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, ex.Message);

                return HealthCheckResult.Unhealthy();
            }
        }

        [HttpGet]
        [Route("readiness")]
        public async Task<HealthCheckResult> Readiness()
        {
            return HealthCheckResult.Healthy($"Readiness OK");
        }

        #endregion

        #region Private Members

        protected readonly ILogger<HealthCheckController> _logger;
        protected readonly IServiceProvider _serviceProvider;

        #endregion
    }
}
