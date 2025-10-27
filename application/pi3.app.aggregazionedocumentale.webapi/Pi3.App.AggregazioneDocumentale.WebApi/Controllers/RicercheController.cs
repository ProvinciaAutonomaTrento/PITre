// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.AggregazioneDocumentale.WebApi.Application.Queries.YearFilter;
using Pi3.App.AggregazioneDocumentale.WebApi.Binders;
using Pi3.App.AggregazioneDocumentale.WebApi.Resources;
using Pi3.Infrastructure.Services.AAC;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize(policy: Policies.PITRE)]
    [Route("api/v{version:apiVersion}/{instance}/[controller]")]
    public class RicercheController: Controller
    {
        private readonly ILogger<RicercheController> _logger;
        private readonly IMediator _mediator;

        public RicercheController(
            ILogger<RicercheController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        [ResourceSwaggerOperation(nameof(Files.Anno))]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(YearFilterQueryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route("RicercaPerRegistro")]
        public async Task<ActionResult> Anno(
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.idTenant))][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.userId))][Required] string userId,
            [FromHeader, ResourceSwaggerParameter(nameof(Documentation.codiceGruppo))][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter(nameof(Documentation.idIstanza))] string instance,
            [FromQuery, ResourceSwaggerParameter(nameof(Documentation.AnnoRicerca))] string anno,
            [FromQuery, ResourceSwaggerParameter(nameof(Documentation.CodiceRegistro))] string codRegistro,
            [FromQuery, ResourceSwaggerParameter(nameof(Documentation.PaginazioneRicerca))][FromJsonQuery] Pagination? paginazione)
        {
            var results = await this._mediator.Send(new YearFilterQuery()
            {
                CodiceRegistro = codRegistro,
                Paginazione = paginazione,
                Anno = Convert.ToInt32(anno)
            });
            results.Links = Helpers.GetLinksUrl(instance);

            return StatusCode(StatusCodes.Status200OK, results);
        }


    }
}
