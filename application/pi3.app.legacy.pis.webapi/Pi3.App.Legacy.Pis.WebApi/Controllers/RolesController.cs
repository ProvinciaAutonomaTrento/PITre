// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetRole;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetRoles;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetRolesForEnabledActions;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetUsersInRole;

using Pi3.App.Legacy.Pis.WebApi.Models;

namespace Pi3.App.Legacy.Pis.WebApi.Controllers
{
    public class RolesController : Controller
    {
        #region Public Members
        public RolesController(
            ILogger<RolesController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        /// Servizio per il reperimento dei dettagli di un ruolo dato il codice del ruolo o l’id.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="codeRole">Codice del ruolo</param>
        /// <param name="idRole">Id del Ruolo</param>
        /// <returns>Dettaglio del ruolo richiesto</returns>
        /// <remarks>Metodo per il prelievo dei dettagli di un ruolo dato il codice del ruolo o l’id. Almeno uno dei due parametri è obbligatorio. Restituisce il dettaglio del ruolo.
        /// </remarks>
        [HttpGet]
        [Route("GetRole")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetRoleCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetRoleCommandResponse>> GetRole(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken,
            GetRoleCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento di tutti i ruoli disponibili ad un utente.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="userId">Username dell'utente di cui si vogliono conoscere i ruoli</param>
        /// <returns>Lista dei ruoli assegnati all'utente</returns>
        /// <remarks>Metodo per il reperimento di tutti i ruoli disponibili per un utente. Restituisce la lista dei ruoli dell'utente</remarks>
        [HttpGet]
        [Route("GetRoles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetRolesCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetRolesCommandResponse>> GetRoles(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken,
            GetRolesCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il prelievo dei ruoli dato un utente ed una funzione richiesta.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="userId">Username dell'utente di cui si vogliono conoscere i ruoli</param>
        /// <param name="codeFunction">Codice della funzione secondo la quale filtrare i ruoli</param>
        /// <returns>Lista dei ruoli di un utente aventi una determinata funzione </returns>
        /// <remarks>Metodo per il prelievo dei ruoli di un utente che hanno una determinata funzione. Restituisce la lista dei ruoli.</remarks>
        [HttpGet]
        [Route("GetRolesForEnabledActions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetRolesForEnabledActionsCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetRolesForEnabledActionsCommandResponse>> GetRolesForEnabledActions(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken,
            GetRolesForEnabledActionsCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento degli utenti in un ruolo.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="codeRole">Codice del ruolo</param>
        /// <returns>Lista degli utenti presenti nel ruolo</returns>
        /// <remarks>Metodo per il prelievo degli utenti presenti in un determinato ruolo, a partire dal codice del ruolo. Restituisce la lista degli utenti.</remarks>
        [HttpGet]
        [Route("GetUsersInRole")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUsersInRoleCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetUsersInRoleCommandResponse>> GetUsersInRole(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken,
            GetUsersInRoleCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }
        #endregion

        #region Private Members

        protected readonly ILogger<RolesController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}
