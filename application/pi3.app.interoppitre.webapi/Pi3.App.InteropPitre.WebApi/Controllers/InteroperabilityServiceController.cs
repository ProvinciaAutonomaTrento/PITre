// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.AnalyzeDocumentDroppedOrErrorMessageProof;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.AnalyzeDocumentReceivedProof;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.ElaborateNewInteroperabilityMessage;
using Pi3.App.InteropPitre.WebApi.Models;
using Pi3.Infrastructure.Services.AAC;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Pi3.App.InteropPitre.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize(policy: Policies.PITRE)]
    [Route("api/v{version:apiVersion}/{instance}/[controller]")]
    public class InteroperabilityServiceController : Controller
    {
        #region Public Members

        public InteroperabilityServiceController(
            ILogger<InteroperabilityServiceController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        [HttpPost()]
        [Route("ElaborateNewInteroperabilityMessage")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ElaborateNewInteroperabilityMessageResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest , Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<ElaborateNewInteroperabilityMessageResponse>> ElaborateNewInteroperabilityMessage(
                [FromRoute][Required] string instance,
                [FromHeader][Required] string tenant,
                [FromBody][Required] ElaborateNewInteroperabilityMessageRequest request)
        {
            var response = await this._mediator.Send(request);

            return Ok(response);
        }

        [HttpPut()]
        [Route("AnalyzeDocumentReceivedProof")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult> AnalyzeDocumentReceivedProof(
        [FromRoute][Required] string instance,
        [FromHeader][Required] string tenant,
        [FromBody][Required] AnalyzeDocumentReceivedProofRequest request)
        {
            await this._mediator.Send(request);

            return NoContent();
        }


        [HttpPut()]
        [Route("AnalyzeDocumentDroppedOrErrorMessageProof")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult> AnalyzeDocumentDroppedOrErrorMessageProof(
        [FromRoute][Required] string instance,
        [FromHeader][Required] string tenant,
        [FromBody][Required] AnalyzeDocumentDroppedOrErrorMessageProofRequest request)
        {
            await this._mediator.Send(request);

            return NoContent();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InteroperabilityServiceController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}
