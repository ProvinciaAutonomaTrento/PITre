// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Invoices.FatturaEsitoNotifica;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Invoices.NuovaFattura;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Invoices.NuovaFatturaAttiva;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Invoices.NuovoLotto;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Invoices.NuovoLottoAttivo;
using Pi3.App.Legacy.Pis.WebApi.Models;

namespace Pi3.App.Legacy.Pis.WebApi.Controllers
{
    public class InvoicesController : Controller
    {
        /*
        #region Public Members
        public InvoicesController(
            ILogger<InvoicesController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        /// Servizio specifico per l'aggiornamento dell'esito della fattura passiva.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto richiesta</param>
        /// <returns>Messaggio di esito</returns>
        /// <remarks>Il metodo permette di aggiornare il campo esito notifica nella tipologia Fattura Elettronica</remarks>
        [HttpPost]
        [Route("FatturaEsitoNotifica")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FatturaEsitoNotificaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<FatturaEsitoNotificaCommandResponse>> FatturaEsitoNotifica(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken,
            [FromBody] FatturaEsitoNotificaCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la creazione della fattura passiva
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto richiesta</param>
        /// <returns>Dettaglio della fattura creata</returns>
        /// <remarks>Metodo per la creazione della fattura passiva. I funzionamenti interni sono ad uso esclusivo dell'integrazione con TIBCO.</remarks>
        [HttpPut]
        [Route("NuovaFattura")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NuovaFatturaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<NuovaFatturaCommandResponse>> NuovaFattura(
          [FromHeader] string Instance,
          [FromHeader] string AuthToken,
          [FromBody] NuovaFatturaCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la creazione di un lotto di fatture passive
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto richiesta</param>
        /// <returns>Dettaglio del lotto creato</returns>
        /// <remarks>Metodo per la creazione di un lotto di fatture passive. I funzionamenti interni sono ad uso esclusivo dell'integrazione con TIBCO.</remarks>
        [HttpPut]
        [Route("NuovoLotto")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NuovoLottoCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<NuovoLottoCommandResponse>> NuovoLotto(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] NuovoLottoCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la creazione della fattura attiva
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto richiesta</param>
        /// <returns>Dettaglio della fattura creata</returns>
        /// <remarks>Metodo per la creazione della fattura attiva. La tipologia documentale viene popolata a partire dal file xml passato come documento principale. Dallo stesso vengono inoltre estratti gli allegati.</remarks>
        [HttpPut]
        [Route("NuovaFatturaAttiva")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NuovaFatturaAttivaCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<NuovaFatturaAttivaCommandResponse>> NuovaFatturaAttiva(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] NuovaFatturaAttivaCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la creazione di un lotto di fatture attive
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto richiesta</param>
        /// <returns>Dettaglio del lotto creato</returns>
        /// <remarks>Metodo per la creazione di un lotto di fatture attive. La tipologia documentale viene popolata a partire dal file xml passato come documento principale.</remarks>
        [HttpPut]
        [Route("NuovoLottoAttivo")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NuovoLottoAttivoCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<NuovoLottoAttivoCommandResponse>> NuovoLottoAttivo(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] NuovoLottoAttivoCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }
        #endregion

        #region Private Members

        protected readonly ILogger<InvoicesController> _logger;
        protected readonly IMediator _mediator;

        #endregion
        */
    }
}
