// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetActiveClassificationScheme;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetAllClassificationSchemes;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes.GetClassificationSchemeById;
using Pi3.App.Legacy.Pis.WebApi.Models;

namespace Pi3.App.Legacy.Pis.WebApi.Controllers
{
    public class ClassificationSchemesController : Controller
    {
        #region Public Members
        public ClassificationSchemesController(
            ILogger<ClassificationSchemesController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }


        /// <summary>
        /// Servizio per il reperimento del titolario attivo.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <returns>Il dettaglio del titolario attivo</returns>
        /// <remarks>Metodo per il prelievo del titolario attivo, all'interno del quale possono essere creati i nuovi fascicoli</remarks>
        [HttpGet]
        [Route("GetActiveClassificationScheme")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetActiveClassificationSchemeCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetActiveClassificationSchemeCommandResponse>> GetActiveClassificationScheme(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken)
        {

            var results = await this._mediator.Send(new GetActiveClassificationSchemeCommand());

            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento di tutti i titolari.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <returns>Lista dei titolari.</returns>
        /// <remarks>Metodo per il prelievo del dettaglio di tutti i titolari</remarks>
        [HttpGet]
        [Route("GetAllClassificationSchemes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllClassificationSchemesCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetAllClassificationSchemesCommandResponse>> GetAllClassificationSchemes(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken)
        {

            var results = await this._mediator.Send(new GetAllClassificationSchemesCommand());

            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento dei dettagli di un titolario dato l’id.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idClassificationScheme">Id del titolario cercato</param>
        /// <returns>Dettaglio del titolario</returns>
        /// <remarks>Metodo per il reperimento del dettaglio di un titolario dato l'id dello stesso</remarks>
        [HttpGet]
        [Route("GetClassificationSchemeById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetClassificationSchemeByIdCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetClassificationSchemeByIdCommandResponse>> GetClassificationSchemeById(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken,
            GetClassificationSchemeByIdCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        #endregion

        #region Private Members

        protected readonly ILogger<ClassificationSchemesController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }

}
