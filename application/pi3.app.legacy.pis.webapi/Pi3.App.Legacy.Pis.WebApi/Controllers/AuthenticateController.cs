// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Authenticate.Authenticate;
using Pi3.App.Legacy.Pis.WebApi.Models;

namespace Pi3.App.Legacy.Pis.WebApi.Controllers
{
    [ApiController]
    public class AuthenticateController : Controller
    {
        #region Public Members
        public AuthenticateController(
            ILogger<AuthenticateController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }


        /// <summary>
        /// Servizio per il prelievo del token di autenticazione
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="request">Oggetto AuthenticateRequest, contenente i seguenti parametri <br/>
        /// Username: Username dell'utente. Obbligatorio<br/>
        /// CodeAdm: Codice dell'amministrazione nella quale ci si vuole autenticare. Obbligatorio<br/>
        /// CodeRole: Ruolo con il quale ci si vuole autenticare. Se omesso, viene selezionato il ruolo preferito dell'utente. Opzionale.
        /// CodeApplication: Identifica l'integrazione che si interfaccia tramite REST </param>
        /// <returns>Token di autenticazione</returns>
        /// <remarks>Metodo per il prelievo del token di autenticazione. Fornendo lo username ed il codice amministrazione, viene restituito un token di autenticazione da inserire nelle successive chiamate, in un header "AuthToken"</remarks>
        [HttpPost]
        [Route("GetToken")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticateCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<AuthenticateCommandResponse>> Authenticate(
            [FromHeader] string Instance,
            [FromBody] AuthenticateCommand request)
        {
            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AuthenticateController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}
