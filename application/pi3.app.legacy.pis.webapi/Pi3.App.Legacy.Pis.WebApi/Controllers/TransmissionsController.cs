// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmDocModel;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmissionDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmissionProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmPrjModel;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetTransmissionModel;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetTransmissionModels;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GiveUpRights;
using Pi3.App.Legacy.Pis.WebApi.Models;

namespace Pi3.App.Legacy.Pis.WebApi.Controllers
{
    public class TransmissionsController : Controller
    {
        #region Public Members
        public TransmissionsController(
            ILogger<TransmissionsController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        /// Servizio per la trasmissione di un documento tramite un modello di trasmissione
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request da popolare nei parametri:<br/>
        /// IdModel (string): Obbligatorio. Id del modello di trasmissione<br/>
        /// DocumentId (string): Obbligatorio. Id del documento da trasmettere.</param>
        /// <returns>Messaggio di avvenuta trasmissione.</returns>
        /// <remarks>Metodo per la trasmissione di un documento tramite un modello di trasmissione.<br/>
        /// Restituisce un messaggio di avvenuta trasmissione in caso di successo.</remarks>
        [HttpPost]
        [Route("ExecuteTransmDocModel")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExecuteTransmDocModelCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<ExecuteTransmDocModelCommandResponse>> ExecuteTransmissionDocument(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken,
            [FromBody] ExecuteTransmDocModelCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per la trasmissione di un fascicolo tramite un modello di trasmissione
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request da popolare nei parametri:<br/>
        /// IdModel (string): Obbligatorio. Id del modello di trasmissione<br/>
        /// IdProject (string): Obbligatorio. Id del fascicolo da trasmettere.</param>
        /// <returns>Messaggio di avvenuta trasmissione.</returns>
        /// <remarks>Metodo per la trasmissione di un fascicolo tramite un modello di trasmissione.<br/>
        /// Restituisce un messaggio di avvenuta trasmissione in caso di successo.</remarks>
        [HttpPost]
        [Route("ExecuteTransmPrjModel")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExecuteTransmPrjModelCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<ExecuteTransmPrjModelCommandResponse>> ExecuteTransmPrjModel(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] ExecuteTransmPrjModelCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per la trasmissione singola di un documento
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto Request</param>
        /// <returns>Messaggio di avvenuta trasmissione.</returns>
        /// <remarks>Metodo per la trasmissione singola di un documento senza l’utilizzo di un modello di trasmissione.<br/>
        /// La richiesta va popolata nei campi:
        /// IdDocument (String): Obbligatorio. Id del documento che si desidera trasmettere.<br/>
        /// Receiver (Correspondent): Obbligatorio. Destinatario della trasmissione. E’ obbligatorio l’inserimento della sua proprietà Id o Code.<br/>
        /// TransmissionReason (String): Obbligatorio. Codice della ragione di trasmissione con la quale si invia il documento.<br/>
        /// CodeReg (String): Opzionale. Codice del registro. <br/>
        /// Notify (Boolean): Opzionale. Se true, notifica gli utenti della trasmissione. Se false o non inserito, non invia le notifiche.<br/>
        /// TransmissionType (String): Opzionale. Definisce il tipo di trasmissione: Se “T”, la trasmissione è di tipo “Tutti”. Qualsiasi altro valore (compreso nullo) la trasmissione è di tipo “Uno”</remarks>
        [HttpPost]
        [Route("ExecuteTransmissionDocument")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExecuteTransmissionDocumentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<ExecuteTransmissionDocumentCommandResponse>> ExecuteTransmissionDocument(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] ExecuteTransmissionDocumentCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per la trasmissione singola di un fascicolo
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto Request</param>
        /// <returns>Messaggio di avvenuta trasmissione.</returns>
        /// <remarks>Metodo per la trasmissione singola di un fascicolo senza l’utilizzo di un modello di trasmissione.<br/>
        /// La richiesta va popolata nei campi:
        /// IdProject (String): Obbligatorio. Id del fascicolo che si desidera trasmettere.<br/>
        /// Receiver (Correspondent): Obbligatorio. Destinatario della trasmissione. E’ obbligatorio l’inserimento della sua proprietà Id o Code.<br/>
        /// TransmissionReason (String): Obbligatorio. Codice della ragione di trasmissione con la quale si invia il documento.<br/>
        /// CodeReg (String): Opzionale. Codice del registro. <br/>
        /// Notify (Boolean): Opzionale. Se true, notifica gli utenti della trasmissione. Se false o non inserito, non invia le notifiche.<br/>
        /// TransmissionType (String): Opzionale. Definisce il tipo di trasmissione: Se “T”, la trasmissione è di tipo “Tutti”. Qualsiasi altro valore (compreso nullo) la trasmissione è di tipo “Uno”</remarks>
        [HttpPost]
        [Route("ExecuteTransmissionProject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExecuteTransmissionProjectCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<ExecuteTransmissionProjectCommandResponse>> ExecuteTransmissionProject(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] ExecuteTransmissionProjectCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per il reperimento del dettaglio di un modello di trasmissione
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idModel">Id del modello di trasmissione</param>
        /// <param name="codeModel">Codice del modello di trasmissione</param>
        /// <returns>Dettaglio del modello di trasmissione</returns>
        /// <remarks>Metodo per il reperimento del dettaglio di un modello di trasmissione dato il codice o l’id del modello. Obbligatorio almeno un valore tra codice e id.</remarks>
        [HttpGet]
        [Route("GetTransmissionModel")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTransmissionModelCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetTransmissionModelCommandResponse>> GetTransmissionModel(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetTransmissionModelCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per il reperimento di tutti i modelli di trasmissione per documenti o fascicoli.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto Request.</param>
        /// <returns>Lista dei modelli di trasmissione</returns>
        /// <remarks> Metodo per il reperimento di tutti i modelli di trasmissione per documenti o fascicoli.<br/>
        /// La richiesta deve essere popolata nei campi:<br/>
        /// Type (string): Obbligatorio. Inserire “D” per i modelli dei documenti, “F” per i modelli dei fascicoli.<br/>
        /// Registers (Register[]): Obbligatorio. Array di oggetti Register. Obbligatorio almeno un codice di un registro.
        /// </remarks>
        [HttpPost]
        [Route("GetTransmissionModels")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTransmissionModelsCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetTransmissionModelsCommandResponse>> GetTransmissionModels(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] GetTransmissionModelsCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per la cessione del diritto di scrittura/lettura su un oggetto
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request: <br/> 
        /// RightToKeep (string): Obbligatorio. Diritto da mantenere. Può avere 3 valori: WRITE, READ e NONE. <br/>
        /// IdObject (string): Obbligatorio. Id dell’oggetto sul quale si vogliono cedere i diritti</param>
        /// <returns>Messaggio di avvenuta cessione dei diritti.</returns>
        /// <remarks>Metodo per la cessione del diritto di scrittura/lettura su un documento o fascicolo.</remarks>
        [HttpPost]
        [Route("GiveUpRights")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GiveUpRightsCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GiveUpRightsCommandResponse>> GiveUpRights(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] GiveUpRightsCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<TransmissionsController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}
