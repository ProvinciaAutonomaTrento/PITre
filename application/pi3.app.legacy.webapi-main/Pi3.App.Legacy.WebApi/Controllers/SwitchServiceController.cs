// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pi3.App.Legacy.WebApi.Application.Behaviors;
using Pi3.App.Legacy.WebApi.Application.Models;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Models;
using Pi3.App.Legacy.WebApi.Requests;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Services.AAC;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Pi3.App.Legacy.WebApi.Controllers
{
    [Authorize(policy: Policies.PITRE)]
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/{instance}/[Controller]")]
    public class SwitchServiceController : ControllerBase
    {
        private readonly ILogger<SwitchServiceController> _logger;
        private readonly IMediator _mediator;

        public SwitchServiceController(ILogger<SwitchServiceController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IReadOnlyList<SwitchServiceModel>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ExceptionInfo))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ExceptionInfo))]
        public async Task<IReadOnlyList<SwitchServiceModel>> Handle()
        {
            var response = await this._mediator.Send(new GetSwitchServices());

            return response.output;
        }
    }
}
