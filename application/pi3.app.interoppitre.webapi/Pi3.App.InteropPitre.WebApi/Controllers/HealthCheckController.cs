// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Threading;

namespace Pi3.App.InteropPitre.WebApi.Controllers
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
        [Route("{instance}/startup")]
        public async Task<HealthCheckResult> Startup(string instance)
        {
            try
            {
                var dbContext = this._serviceProvider.GetRequiredService<IPi3DbContext>();
                var canConnect = await ((DbContext)dbContext).Database.CanConnectAsync();

                if (!canConnect)
                    throw new ApplicationException($"Non è stato possibile stabilire una connessione con {instance}");

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
        public async Task<HealthCheckResult> Readiness(bool? healthy)
        {
            if (healthy != null)
            {
                if (healthy == false)
                    _stateReadinessValid = false;
                else
                    _stateReadinessValid = true;
            }

            if (_stateReadinessValid)
                return HealthCheckResult.Healthy();
            else
            {
                return HealthCheckResult.Unhealthy();
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<HealthCheckController> _logger;
        protected readonly IServiceProvider _serviceProvider;
        protected static bool _stateReadinessValid = true;

        #endregion
    }
}
