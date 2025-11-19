// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Pi3.App.DistributedCache.WebApi.Resources;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using StackExchange.Redis;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pi3.App.DistributedCache.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class KeysController : Controller
    {
        protected readonly ILogger<KeysController> _logger;
        protected readonly IServiceProvider _serviceProvider;

        public KeysController(ILogger<KeysController> logger, IServiceProvider serviceProvider)
        {   
            this._logger = logger;
            this._serviceProvider = serviceProvider;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<IReadOnlyList<string>>> GetKeys()
        {
            this._logger.LogInformation($"GetKeys");

            var database = this._serviceProvider.GetService<IDatabase>();
            if (database == null)
                throw new NotSupportedPi3Exception(ErrorDescriptions.OperationNotSupported, ErrorDescriptions.ResourceManager);

            var keys = new List<string>();

            foreach (var server in database.Multiplexer.GetServers())
            {
                _logger.LogInformation($"Server: {server}");
                keys.AddRange(server.Keys().Select(k => k.ToString()));
            }

            return Ok(keys.AsReadOnly());
        }
    }
}
