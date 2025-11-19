// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.WebApi.Models;
using Pi3.App.Legacy.WebApi.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Services.AAC;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Pi3.App.Legacy.WebApi.Controllers
{
    [Authorize(policy: Policies.PITRE)]
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/{instance}/[Controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TemporaryUploadsController : Controller
    {
        public TemporaryUploadsController(ILogger<TemporaryUploadsController> logger,
           IClaimsPrincipalService claimsPrincipalService,
           IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._configurationService = configurationService;
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task DownloadDocumentFileStream(
                [FromBody][Required] DataPortalRequest request,
                [FromQuery] string uuid,
                [FromQuery] int? bufferSize = null
                )
        {
            var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);

            var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);

            var directory = Path.Combine(
                        repositoryRootPath,
                        tenantCode.ToUpper(),
                        "TemporaryUploads",
                        uuid).PathAsUnixPath();

            var files = Directory.GetFiles(directory);
            if (files != null && files.Count() == 1)
            {
                var filePath = files.First();
                using Stream stream = new FileStream(filePath,FileMode.Open);

                bufferSize = bufferSize ?? 1048676;

                byte[] buffer = new byte[bufferSize.Value];

                int bytesReaded = 0;

                //this.Response.Headers.Add("Content-Type", new Microsoft.Extensions.Primitives.StringValues(results.ContentType.ToString()));
                this.Response.Headers.Add("Content-Length", new Microsoft.Extensions.Primitives.StringValues(stream.Length.ToString()));

                while ((bytesReaded = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    await this.Response.Body.WriteAsync(buffer, 0, bytesReaded);
                    await this.Response.Body.FlushAsync();

                    buffer = new byte[bufferSize.Value];
                }
            }

            
        }

        protected readonly ILogger<TemporaryUploadsController> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IConfigurationService _configurationService;

    }
}
