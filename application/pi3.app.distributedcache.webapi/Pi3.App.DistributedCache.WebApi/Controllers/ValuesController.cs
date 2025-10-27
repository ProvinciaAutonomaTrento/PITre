// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Pi3.App.DistributedCache.WebApi.Resources;
using Pi3.Core.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DistributedCache.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ValuesController : Controller
    {
        protected readonly ILogger<KeysController> _logger;
        protected readonly IDistributedCache _distributedCache;
        protected const int _defaultExpirationMinutes = 30;

        public ValuesController(ILogger<KeysController> logger, IDistributedCache distributedCache)
        {
            this._logger = logger;
            this._distributedCache = distributedCache;
        }

        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult> Put(
            [FromForm][Required(AllowEmptyStrings = false)] string key,
            [FromForm][Required(AllowEmptyStrings = false)] string value,
            [FromForm] int? expiresInMinutes = null)
        {
            this._logger.LogInformation($"Put - key: {key} - Value: {value} - expiresInMinutes: {expiresInMinutes}");

            if (!expiresInMinutes.HasValue)
                expiresInMinutes = _defaultExpirationMinutes;

            await this._distributedCache.SetStringAsync(key, value, new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTimeOffset.Now.Add(TimeSpan.FromMinutes(expiresInMinutes.Value)),
            });

            return NoContent();
        }


        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<string?>> Get(
            [FromQuery][Required(AllowEmptyStrings = false)] string key)
        {
            this._logger.LogInformation($"Get - key: {key}");

            var value = await this._distributedCache.GetStringAsync(key);
            return Ok(value);
        }

        [HttpDelete()]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult> Delete(
            [FromForm][Required(AllowEmptyStrings = false)] string key)
        {
            this._logger.LogInformation($"Delete - key: {key}");

            await this._distributedCache.RemoveAsync(key);

            return NoContent();
        }
    }
}
