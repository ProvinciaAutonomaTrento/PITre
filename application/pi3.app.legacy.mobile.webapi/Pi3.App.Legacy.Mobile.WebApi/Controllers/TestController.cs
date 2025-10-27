// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using QUERY = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Queries;
using COMMAND = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Commands;
using DTO = Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs;
using RESULTS = Pi3.App.Legacy.Mobile.ApiHandler.Models.CommandResults;
using MODELS = Pi3.App.Legacy.Mobile.Models;
using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;

namespace Pi3.App.Legacy.Mobile.WebApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/[controller]")]
[ApiController]
public class TestController(
    IMediator mediator ) : ControllerBase
{
    readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Test()
    {
        COMMAND.RimuoviNotificaCommand command = new COMMAND.RimuoviNotificaCommand(131380670);
        RESULTS.Notifiche.RimuoviNotificaResult result = await this._mediator.Send(command);

        return Ok(result);
    }
}
