// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.WebApi.Models;
using Pi3.Infrastructure.Services.AAC;
using System.Net;

namespace Pi3.App.Legacy.WebApi.Controllers
{
    [Authorize(policy: Policies.PITRE)]
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/{instance}/[Controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PerformanceController : Controller
    {
        private readonly IPerformanceLoggerService _performanceLogger;

        public PerformanceController(IPerformanceLoggerService performanceLogger)
        {
            this._performanceLogger = performanceLogger;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ExceptionInfo))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ExceptionInfo))]
        public async Task<string> Collect(
            [FromQuery] string? logName = null,
            [FromQuery] PerformanceLogOrderBy? orderBy = null,
            [FromQuery] bool? orderByDescending = false)
        {
            return await this._performanceLogger.Collect(logName, orderBy, orderByDescending);
        }
    }
}
