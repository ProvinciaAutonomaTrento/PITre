// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

using QUERY = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Queries;
using DTO = Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs;
using Pi3.Infrastructure.Services.AAC;
using Microsoft.AspNetCore.Authorization;

namespace Pi3.App.Legacy.Mobile.WebApi.Controllers;

//[Authorize(policy: Policies.PITRE)]
[Authorize()]
[Route("api/[controller]")]
[ApiController]
public class AuthenticateController(
    ILogger<AuthenticateController> logger,
    IMediator mediator ) : ControllerBase
{
    readonly ILogger<AuthenticateController> _logger = logger;
    readonly IMediator _mediator = mediator;

    [HttpPost("Login")]
    [Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
    [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
    [SwaggerOperation( OperationId = "Authenticate_Login", Tags = ["Authenticate"] )]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(DTO.Authentication.AuthenticateUserDTO), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    public async Task<ActionResult<DTO.Authentication.AuthenticateUserDTO>> Login( [FromBody]QUERY.AuthenticateUserQuery request)
    {
        this._logger.LogInformation("Richiesta procedura di Login");
        // ToDo gestire cambio password
        DTO.Authentication.AuthenticateUserDTO result = await this._mediator.Send(request);

        return Ok(result);
    }
}
