// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.InstanceAccess.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Threading;

namespace Pi3.App.Legacy.Pis.WebApi.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HealthCheckController : Controller
    {
        #region Public Members

        public HealthCheckController(
            ILogger<HealthCheckController> logger,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._serviceProvider = serviceProvider;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet()]
        [Route("healthz")]
        public async Task<HealthCheckResult> Healthz()
        {
            return HealthCheckResult.Healthy();
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

                this._httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

                return HealthCheckResult.Unhealthy();
            }
        }

        [HttpGet]
        [Route("readiness")]
        public async Task<HealthCheckResult> Readiness(bool? throwError)
        {
            if (throwError != null)
            {
                if (throwError == false)
                    this._stateReadinessValid = false;
                else
                    this._stateReadinessValid = true;
            }

            if (this._stateReadinessValid)
                return HealthCheckResult.Healthy();
            else
            {
                this._httpContextAccessor.HttpContext!.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

                return HealthCheckResult.Unhealthy();
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<HealthCheckController> _logger;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected bool _stateReadinessValid = true;

        #endregion
    }
}
