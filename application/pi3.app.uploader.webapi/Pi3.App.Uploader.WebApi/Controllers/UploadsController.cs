// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Uploader.WebApi.Binders;
using Pi3.App.Uploader.WebApi.Resources;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.Uploader;
using Pi3.Infrastructure.Legacy.UploaderFS.Services.File.Uploader;
using Pi3.Infrastructure.Services.AAC;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Uploader.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize(policy: Policies.PITRE)]
    [Route("api/v{version:apiVersion}/{instance}/[controller]")]
    public class UploadsController : Controller
    {
        #region Public Members

        public UploadsController(
            ILogger<UploadsController> logger,
            IUploaderService uploaderService)
        {
            this._logger = logger;
            this._uploaderService = uploaderService;
        }

        [ResourceSwaggerOperation(nameof(InitializeUpload))]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<Guid>> InitializeUpload(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required(AllowEmptyStrings = false)] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required(AllowEmptyStrings = false)] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required(AllowEmptyStrings = false)] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromForm, ResourceSwaggerParameter("fileName")][Required(AllowEmptyStrings = false)] string fileName,
            [FromForm, ResourceSwaggerParameter("numeroParti")][Required(AllowEmptyStrings = false)] int numeroParti,
            [FromForm, ResourceSwaggerParameter("checksum")][Required(AllowEmptyStrings = false)] string checksum)
        {
            var uploadId = await this._uploaderService.InitializeUpload(fileName, numeroParti, checksum);

            return Ok(uploadId);
        }

        [ResourceSwaggerOperation(nameof(UploadPart))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{uploadId}/Part")]
        public async Task<ActionResult> UploadPart(
               [FromHeader, ResourceSwaggerParameter("idTenant")][Required(AllowEmptyStrings = false)] string tenant,
               [FromHeader, ResourceSwaggerParameter("userId")][Required(AllowEmptyStrings = false)] string userId,
               [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required(AllowEmptyStrings = false)] string groupCode,
               [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
               [FromRoute, ResourceSwaggerParameter("uploadId")] Guid uploadId,
               [FromForm, ResourceSwaggerParameter("partNumber")][Required] int partNumber,
               [FromForm, ResourceSwaggerParameter("partSize")][Required] long partSize,
               [ResourceSwaggerParameter("partContent")][Required] IFormFile partContent)
        {
            // Process the file partContent
            using (var stream = new MemoryStream())
            {
                await partContent.CopyToAsync(stream);
                var fileContent = stream.ToArray();
                // Process the file content
                await _uploaderService.UploadPart(uploadId, partNumber, partSize, fileContent);
            }

            return NoContent();
        }

        [ResourceSwaggerOperation(nameof(FinalizeUpload))]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("{uploadId}/Finalize")]
        public async Task<ActionResult> FinalizeUpload(
               [FromHeader, ResourceSwaggerParameter("idTenant")][Required(AllowEmptyStrings = false)] string tenant,
               [FromHeader, ResourceSwaggerParameter("userId")][Required(AllowEmptyStrings = false)] string userId,
               [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required(AllowEmptyStrings = false)] string groupCode,
               [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
               [FromRoute, ResourceSwaggerParameter("uploadId")] Guid uploadId)
        {
            await _uploaderService.FinalizeUpload(uploadId);

            return NoContent();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UploadsController> _logger;
        protected readonly IUploaderService _uploaderService;

        #endregion
    }
}
