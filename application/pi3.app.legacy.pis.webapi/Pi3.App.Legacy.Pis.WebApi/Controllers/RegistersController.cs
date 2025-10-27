// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers.GetRegisterOrRF;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers.GetRegistersOrRF;
using Pi3.App.Legacy.Pis.WebApi.Models;

namespace Pi3.App.Legacy.Pis.WebApi.Controllers
{
    public class RegistersController : Controller
    {
        #region Public Members
        public RegistersController(
            ILogger<RegistersController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        /// Servizio per il reperimento del dettaglio di un registro/RF dato il codice del registro/RF o l’id.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="codeRegister">Codice del registro</param>
        /// <param name="idRegister">Id del registro</param>
        /// <returns>Dettaglio del registro</returns>
        /// <remarks>Metodo per il prelievo dei dettagli di un registro a partire dal suo codice o dall'id. Almeno uno dei 2 parametri è obbligatorio.</remarks>
        [HttpGet]
        [Route("GetRegisterOrRF")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetRegisterOrRFCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetRegisterOrRFCommandResponse>> GetRegisterOrRF(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken,
            GetRegisterOrRFCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento di tutti gli RF o i registri disponibili per un ruolo
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="codeRole">Codice del ruolo</param>
        /// <param name="idRole">Id del ruolo</param>
        /// <param name="RegOrRF">Permette di filtrare i risultati. Popolato con REG, restituisce i soli registri; con RF, i soli RF. Se altro valore oppure omesso, restituisce entrambi</param>
        /// <returns>Lista dei registri e/o RF disponibili per un ruolo</returns>
        /// <remarks>Metodo utilizzato per il prelievo del dettaglio dei registri o RF disponibili per un ruolo. Almeno un parametro tra codeRole e idRole è obbligatorio. I risultati possono essere filtrati con il parametro RefOrRF.</remarks>
        [HttpGet]
        [Route("GetRegistersOrRF")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetRegistersOrRFCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetRegistersOrRFCommandResponse>> GetRegistersOrRF(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken,
            GetRegistersOrRFCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }
        #endregion

        #region Private Members

        protected readonly ILogger<RegistersController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}
