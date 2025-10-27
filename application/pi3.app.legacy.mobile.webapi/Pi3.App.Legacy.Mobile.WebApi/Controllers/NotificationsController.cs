// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Swashbuckle.AspNetCore.Annotations;
using Pi3.Infrastructure.Services.AAC;
using Microsoft.AspNetCore.Authorization;

using COMMAND = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Commands;
using RESULT = Pi3.App.Legacy.Mobile.ApiHandler.Models.CommandResults;
using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;

namespace Pi3.App.Legacy.Mobile.WebApi.Controllers;

//[Authorize(policy: Policies.PITRE)]
[Authorize()]
[Route("api/[controller]")]
[ApiController]
public class NotificationsController(
    ILogger<NotificationsController> logger,
    IMediator mediator ) : ControllerBase
{
    readonly ILogger<NotificationsController> _logger = logger;
    readonly IMediator _mediator = mediator;


    [HttpPost("Remove")]
    [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
    [SwaggerOperation(OperationId = "Notifications_Remove", Tags = ["Notifications"])]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(bool), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails), ContentTypes = [System.Net.Mime.MediaTypeNames.Application.Json])]
    public async Task<IActionResult> Remove( [FromQuery(Name = "idEvento")] string parap_IdEvento )
    {
        if ( !long.TryParse(parap_IdEvento, out long idEvento) || idEvento == 0 )
        {
            this._logger.LogError("Conversione del parametro fallito: {Param}", parap_IdEvento);
            throw new EXCEPTIONS.RequestParamNotFoundException("idEvento");
        }

        COMMAND.RimuoviNotificaCommand command = new(idEvento);
        RESULT.Notifiche.RimuoviNotificaResult result = await this._mediator.Send(command);

        return Ok(result.Result);
    }

}
