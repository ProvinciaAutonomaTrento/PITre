// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetInstanceSearchFilters;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignatureProcess;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignatureProcesses;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignProcessInstance;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.InterruptSignatureProcess;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.SearchSignProcessInstances;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.StartSignatureProcess;
using Pi3.App.Legacy.Pis.WebApi.Models;

namespace Pi3.App.Legacy.Pis.WebApi.Controllers
{
    public class SignBookController : Controller
    {
        #region Public Members
        public SignBookController(
            ILogger<SignBookController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        /// Servizio per ottenere la lista dei filtri per la ricerca delle istanze di firma.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <returns>Lista dei filtri</returns>
        /// <remarks>Metodo per ottenere la lista dei filtri utilizzabile nel metodo SearchSignProcessInstances.</remarks>
        [HttpGet]
        [Route("GetInstanceSearchFilters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetInstanceSearchFiltersCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetInstanceSearchFiltersCommandResponse>> GetInstanceSearchFilters(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken)
        {

            var results = await this._mediator.Send(new GetInstanceSearchFiltersCommand());

            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per ottenere il dettaglio di un processo di firma
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idProcess">Id del processo</param>
        /// <returns>Dettaglio del processo</returns>
        /// <remarks>Servizio per ottenere un processo di firma a partire dal sui Id.</remarks>
        [HttpGet]
        [Route("GetSignatureProcess")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSignatureProcessCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetSignatureProcessCommandResponse>> GetSignatureProcess(
          [FromHeader] string Instance,
          [FromHeader] string AuthToken,
          GetSignatureProcessCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio che restituisce la lista dei processi
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <returns>Lista dei processi di firma</returns>
        /// <remarks>Metodo che restituisce la lista dei processi o dei modelli di processo visibili e quindi utilizzabili dal ruolo.</remarks>
        [HttpGet]
        [Route("GetSignatureProcesses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSignatureProcessesCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetSignatureProcessesCommandResponse>> GetSignatureProcesses(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken)
        {
            var results = await this._mediator.Send(new GetSignatureProcessesCommand());
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per ottenere una istanza dei processo di firma
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idProcessInstance">Id dell'istanza</param>
        /// <returns>Dettaglio dell'instanza</returns>
        /// <remarks>Servizio per ottenere una istanza dei processo a partire dal sui Id istanza.</remarks>
        [HttpGet]
        [Route("GetSignProcessInstance")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSignProcessInstanceCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetSignProcessInstanceCommandResponse>> GetSignProcessInstance(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetSignProcessInstanceCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la ricerca delle istanze di processo di firma
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Lista dei filtri con i quali ricercare le istanze.</param>
        /// <returns>Lista delle istanze filtrate</returns>
        /// <remarks>Metodo per ricercare e quindi monitorare lo stato delle istanze di processi  di firma visibili ad un determinato ruolo, a seconda dei filtri inseriti.</remarks>
        [HttpPost]
        [Route("SearchSignProcessInstances")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchSignProcessInstancesCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<SearchSignProcessInstancesCommandResponse>> SearchSignProcessInstances(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] SearchSignProcessInstancesCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per inserire un documento in un processo di Libro Firma
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Messaggio di avvenuto inserimento.</returns>
        /// <remarks>Servizio per inserire un documento in un processo di Libro Firma, avviando il processo stesso.
        /// La request consiste nei seguenti parametri:<br/>
        /// <br/>
        /// IdDocument (string): Obbligatorio. Id del documento che si desidera utilizzare per inserirlo nel processo del Libro firma. (id documento è ottenibile con i servizi SearchDocument o in risposta ai i servizi come CreateDocument) <br/>
        /// EndGeneratesNote (boolean): Opzionale.  Se True, Abilita la ricezione delle notifiche al ruolo passato in input con CodeRoleLogin di conclusione processo.<br/>
        /// InterruptionGeneratesNote (boolean): Opzionale. Se True, abilita la ricezione delle notifiche al ruolo passato in input  come CodeRoleLogin di interruzione del processo.<br/>
        /// Note (string): Opzionale. Permette di aggiungere delle note al processo che saranno poi visibili nel libro firma.<br/>
        /// SignatureProcess (SignatureProcess): Processo di Libro firma in cui inserire il documento <br/>
        /// EndGeneratesNote (boolean): Opzionale.  Se True, Abilita la ricezione delle notifiche al ruolo passato in input con CodeRoleLogin di conclusione processo.<br/>
        /// InterruptionGeneratesNote (boolean): Opzionale. Se True, abilita la ricezione delle notifiche al ruolo passato in input  come CodeRoleLogin di interruzione del processo.<br/>
        /// Note (string): Opzionale. Permette di aggiungere delle note al processo che saranno poi visibili nel libro firma.<br/>
        /// <br/>
        /// L'oggetto SignatureProcess è strutturato come segue:<br/>
        /// <br/>
        /// AuthorRoleId (string): Id ruolo del ruolo creatore  del Libro Firma.<br/>
        /// AuthorRoleId (string): Id people del utente creatore del Libro Firma.<br/>
        /// IdProcess (string): Id univoco del processo di firma<br/>
        /// IsProcessModel (boolean): Se true allora si tratta di un modello di processo<br/>
        /// Name (string): Nome del processo<br/>
        /// SignatureStep (SignatureStep[]): Passi del processo<br/>
        /// <br/>
        /// L'oggetto SignatureStep è strutturato come segue:<br/>
        /// <br/>
        /// IdProcess (string):  Id univoco del processo di firma<br/>
        /// IdStep (string): Id univoco del passo del processo di firma<br/>
        /// InvolvedRole (Role): Ruolo coinvolto nel passo, Ruolo che deve effettuare il passo<br/>
        /// InvolvedUser (User): Utente coinvolto nel passo, Utente che deve effettuare il passo<br/>
        /// IsModel (boolean): Indica se è un passo di modello<br/>
        /// SequenceNumber (integer): Numero di sequenza del passo<br/>
        /// Note (string): Note del passo<br/>
        /// </remarks>
        [HttpPost]
        [Route("StartSignatureProcess")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StartSignatureProcessCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<StartSignatureProcessCommandResponse>> StartSignatureProcess(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] StartSignatureProcessCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per interrompere una istanza di processo di firma
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Messaggio di avvenuta interruzione</returns>
        /// <remarks>Metodo per interrompere una istanza dei processo a partire dal sui Id istanza.<br/>
        /// Nella richiesta è possibile inserire una nota di interruzione.</remarks>
        [HttpPost]
        [Route("InterruptSignatureProcess")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InterruptSignatureProcessCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<InterruptSignatureProcessCommandResponse>> InterruptSignatureProcess(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] InterruptSignatureProcessCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }


        #endregion

        #region Private Members

        protected readonly ILogger<SignBookController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}
