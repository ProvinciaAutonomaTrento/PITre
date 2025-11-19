// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetTrasmissione;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.RicercaDocumenti;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.RicercaTrasmissioni;
using Pi3.App.DocumentoAmministrativo.WebApi.Binders;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Services.AAC;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Authorize(policy: Policies.PITRE)]
    [Route("api/v{version:apiVersion}/{instance}/[controller]")]
    [Produces("application/json")]
    public class RicercheController: Controller
    {
        #region private members

        private readonly ILogger<RicercheController> _logger;
        private readonly IMediator _mediator;

        private ObjectResult AddLinks(RicercaDocumentiQueryResponse results , string instance) {
            results.Links = Helpers.GetLinksUrl(instance);

            return StatusCode(StatusCodes.Status200OK, results);
        }

        #endregion

        public RicercheController(
            ILogger<RicercheController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        [ResourceSwaggerOperation("RicercaNonProtocollati")]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RicercaDocumentiQueryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route(nameof(NonProtocollati))]
        public async Task<ActionResult> NonProtocollati(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromQuery, Required, ResourceSwaggerParameter("annoCreazioneRicerca")] int annoCreazione,
            [FromQuery, ResourceSwaggerParameter("ignora")] int? ignora = 0,
            [FromQuery, ResourceSwaggerParameter("prendi")] int? prendi = 10
            )
        {
            var results = await this._mediator.Send(new RicercaDocumentiQuery()
            {
                Paginazione = new Paginazione()
                {
                    Ignora = ignora!.Value,
                    Prendi = prendi!.Value
                },
                Anno = annoCreazione,
                TipoRicerca = TipiRicercheEnum.NonProtocollati
            });

            return AddLinks(results, instance);
        }

        [ResourceSwaggerOperation("RicercaProtocollati")]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RicercaDocumentiQueryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route(nameof(Protocollati))]
        public async Task<ActionResult> Protocollati(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromQuery, Required, ResourceSwaggerParameter("annoProtocollazioneRicerca")] int annoProtocollazione,
           [FromQuery, Required, ResourceSwaggerParameter("registroRicerca")] string registro,
           [FromQuery, ResourceSwaggerParameter("tipologiaFlussoRicerca")] string? tipologiaFlusso = null,
           [FromQuery, ResourceSwaggerParameter("numeroProtocolloRicerca")] int? numeroProtocollo = null,
           [FromQuery, ResourceSwaggerParameter("numeroProtocolloFinaleRicerca")] int? numeroProtocolloFinale = null,
           [FromQuery, ResourceSwaggerParameter("ignora")] int? ignora = 0,
           [FromQuery, ResourceSwaggerParameter("prendi")] int? prendi = 10
           )
        {
            var results = await this._mediator.Send(new RicercaDocumentiQuery()
            {
                Paginazione = new Paginazione()
                {
                    Ignora = ignora!.Value,
                    Prendi = prendi!.Value
                },
                CodiceRegistro = registro,
                Anno = annoProtocollazione,
                TipoRicerca = TipiRicercheEnum.Protocollati,
                TipologiaFlusso = tipologiaFlusso switch
                {
                    "E" => TipologieFlussiRicercheEnum.E,
                    "U" => TipologieFlussiRicercheEnum.U,
                    "I" => TipologieFlussiRicercheEnum.I,
                    null => null,
                    _ => throw new Pi3.Core.SeedWork.BadRequestPi3Exception()
                },
                NumeroProtocollo = numeroProtocollo,
                NumeroProtocolloFinale = numeroProtocolloFinale
            });

            return AddLinks(results, instance);
        }

        [ResourceSwaggerOperation("RicercaPredisposti")]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RicercaDocumentiQueryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route(nameof(Predisposti))]
        public async Task<ActionResult> Predisposti(
           [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
           [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
           [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
           [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
           [FromQuery, Required, ResourceSwaggerParameter("annoCreazioneRicerca")] int annoCreazione,
           [FromQuery, Required, ResourceSwaggerParameter("registroRicerca")] string registro,
           [FromQuery, ResourceSwaggerParameter("tipologiaFlussoRicerca")] string? tipologiaFlusso = null,
           [FromQuery, ResourceSwaggerParameter("ignora")] int? ignora = 0,
           [FromQuery, ResourceSwaggerParameter("prendi")] int? prendi = 10
           )
        {
            var results = await this._mediator.Send(new RicercaDocumentiQuery()
            {
                Paginazione = new Paginazione()
                {
                    Ignora = ignora!.Value,
                    Prendi = prendi!.Value
                },
                CodiceRegistro = registro,
                Anno = annoCreazione,
                TipoRicerca = TipiRicercheEnum.Predisposti,
                TipologiaFlusso = tipologiaFlusso switch
                {
                    "E" => TipologieFlussiRicercheEnum.E,
                    "U" => TipologieFlussiRicercheEnum.U,
                    "I" => TipologieFlussiRicercheEnum.I,
                    null => null,
                    _ => throw new Pi3.Core.SeedWork.BadRequestPi3Exception()
                },
            });

            return AddLinks(results, instance);
        }

        [ResourceSwaggerOperation("RicercaTrasmissioni")]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RicercaTrasmissioniQueryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        [Route(nameof(Trasmissioni))]
        public async Task<ActionResult> Trasmissioni(
            [FromHeader, ResourceSwaggerParameter("idTenant")][Required] string tenant,
            [FromHeader, ResourceSwaggerParameter("userId")][Required] string userId,
            [FromHeader, ResourceSwaggerParameter("codiceGruppo")][Required] string groupCode,
            [FromRoute, ResourceSwaggerParameter("idIstanza")] string instance,
            [FromQuery, ResourceSwaggerParameter("IdDocumentoTrasmissioni")] string idDocumento,
            [FromQuery, ResourceSwaggerParameter("ignora")] int? ignora = 0,
            [FromQuery, ResourceSwaggerParameter("prendi")] int? prendi = 10)
        {
            var results = await this._mediator.Send(new RicercaTrasmissioniQuery
            {
                IdDocumento = idDocumento,
                Ignora = ignora!.Value,
                Prendi = prendi!.Value
            });

            results.Links = Helpers.GetLinksUrl(instance, idDocumento);

            return StatusCode(StatusCodes.Status200OK, results);
        }
    }
}
