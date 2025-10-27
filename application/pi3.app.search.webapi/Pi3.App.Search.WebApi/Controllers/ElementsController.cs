// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Search.WebApi.Application.Queries.Elements.GetElements;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Pi3.App.Search.WebApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace Pi3.App.Search.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize()]
    [Route("api/v{version:apiVersion}/{instance}/[controller]")]
    public class ElementsController : Controller
    {
        #region Public Members

        public ElementsController(
            ILogger<ElementsController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }


        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetElementsQueryResults))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetElementsQueryResults>> GetElements(
                [FromRoute][Required] string instance,
                [FromHeader][Required] string tenant,
                [FromHeader][Required] string userId,
                [FromHeader][Required] string groupCode,         
                [FromQuery][Required] IReadOnlyList<TypesEnum> type,
                [FromQuery] int? skip = 0,
                [FromQuery] int? take = 50,
                [FromQuery] string? query = null)
        {
            var results = await this._mediator.Send(new GetElementsQuery()
            {
                Types = type,
                Skip = skip,
                Take = take,
                Query = query
            });

            return results;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ElementsController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}
