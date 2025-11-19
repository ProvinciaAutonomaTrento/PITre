// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddCorrespondent;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddCorrespondentAdvanced;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.EditCorrespondent;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.EditCorrespondentAdvanced;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrespondent;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrespondentAdvanced;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrespondentFilters;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetUserFilters;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchCorrespondents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchCorrespondentsAdvanced;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchUsers;
using Pi3.App.Legacy.Pis.WebApi.Models;

namespace Pi3.App.Legacy.Pis.WebApi.Controllers
{
    public class AddressBookController : Controller
    {
        #region Public Members
        public AddressBookController(
            ILogger<AddressBookController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        /// Servizio che permette il reperimento del dettaglio di un corrispondente dato l’id.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="IdCorrespondent">Id del corrispondente cercato</param>
        /// <returns>Dettaglio del corrispondente</returns>
        /// <remarks>Metodo per il prelievo dei dettagli di un corrispondente a partire dal suo id.</remarks>
        [HttpGet]
        [Route("GetCorrespondent")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCorrespondentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetCorrespondentCommandResponse>> GetCorrespondent(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken,
            GetCorrespondentCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per la creazione di un nuovo corrispondente esterno.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto Correspondent.</param>
        /// <returns>Dettaglio del corrispondente creato</returns>
        /// <remarks>Metodo per l'aggiunta di un corrispondente esterno in rubrica.
        /// La richiesta può essere popolata nei seguenti campi
        /// Description (string): Obbligatorio. Descrizione del corrispondente.<br/>
        /// Code (string): Obbligatorio. Codice del corrispondente ricercabile da rubrica.<br/>
        /// CorrespondentType (string): Obbligatorio. Tipologia del corrispondente. Può assumere i valori “U” nel caso di Unità organizzativa e “P” nel caso di persona.<br/>
        /// City (string): Opzionale. Città del corrispondente.<br/>
        /// Province (string): Opzionale. Provincia del corrispondente.<br/>
        /// Location (string): Opzionale. Località del corrispondente.<br/>
        /// Nation (string): Opzionale. Nazione del corrispondente.<br/>
        /// PhoneNumber (string): Opzionale. Numero di telefono del corrispondente.<br/>
        /// PhoneNumber2 (string): Opzionale. Secondo numero di telefono del corrispondente.<br/>
        /// Fax (string): Opzionale. Numero del fax del corrispondente.<br/>
        /// NationalIdentificationNumber (string): Opzionale.<br/>
        /// Codice fiscale del corrispondente.<br/>
        /// Email (string): Opzionale. Mail principale del corrispondente.<br/>
        /// OtherEmails (string array): Opzionale. Mail secondarie del corrispondente.<br/>
        /// AOOCode (string): Opzionale. Codice AOO.<br/>
        /// AdmCode (string): Opzionale. Codice amministrazione.<br/>
        /// Note (string): Opzionale. Note 
        /// Address (string): Opzionale. Indirizzo del corrispondente.<br/>
        /// Cap (string): Opzionale. Cap del corrispondente.<br/>
        /// CodeRegisterOrRF (string): Opzionale. Se il corrispondente deve essere disponibile soltanto per un Registro/RF valorizzare con il codice del Registro/RF.<br/>
        /// PreferredChannel (string): Opzionale. Canale preferenziale del corrispondente.<br/>
        /// </remarks>
        [HttpPut]
        [Route("AddCorrespondent")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddCorrespondentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<AddCorrespondentCommandResponse>> AddCorrespondent(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] AddCorrespondentCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per la modifica di un corrispondente.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto Correspondent.</param>
        /// <returns>Dettaglio del corrispondente modificato</returns>
        /// <remarks>Metodo per l'aggiunta di un corrispondente esterno in rubrica.
        /// La richiesta può essere popolata nei seguenti campi
        /// Description (string): Obbligatorio. Descrizione del corrispondente.<br/>
        /// Code (string): Obbligatorio. Codice del corrispondente ricercabile da rubrica.<br/>
        /// CorrespondentType (string): Obbligatorio. Tipologia del corrispondente. Può assumere i valori “U” nel caso di Unità organizzativa e “P” nel caso di persona.<br/>
        /// City (string): Opzionale. Città del corrispondente.<br/>
        /// Province (string): Opzionale. Provincia del corrispondente.<br/>
        /// Location (string): Opzionale. Località del corrispondente.<br/>
        /// Nation (string): Opzionale. Nazione del corrispondente.<br/>
        /// PhoneNumber (string): Opzionale. Numero di telefono del corrispondente.<br/>
        /// PhoneNumber2 (string): Opzionale. Secondo numero di telefono del corrispondente.<br/>
        /// Fax (string): Opzionale. Numero del fax del corrispondente.<br/>
        /// NationalIdentificationNumber (string): Opzionale.<br/>
        /// Codice fiscale del corrispondente.<br/>
        /// Email (string): Opzionale. Mail del corrispondente.<br/>
        /// OtherEmails (string array): Opzionale. Mail secondarie del corrispondente.<br/>
        /// AOOCode (string): Opzionale. Codice AOO.<br/>
        /// AdmCode (string): Opzionale. Codice amministrazione.<br/>
        /// Note (string): Opzionale. Note 
        /// Address (string): Opzionale. Indirizzo del corrispondente.<br/>
        /// Cap (string): Opzionale. Cap del corrispondente.<br/>
        /// CodeRegisterOrRF (string): Opzionale. Se il corrispondente deve essere disponibile soltanto per un Registro/RF valorizzare con il codice del Registro/RF.<br/>
        /// PreferredChannel (string): Opzionale. Canale preferenziale del corrispondente.<br/>
        /// </remarks>
        [HttpPost]
        [Route("EditCorrespondent")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EditCorrespondentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<EditCorrespondentCommandResponse>> EditCorrespondent(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] EditCorrespondentCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio che restituisce la lista dei filtri applicabili alla ricerca corrispondenti.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <returns>Lista dei filtri disponibili per la ricerca corrispondenti.</returns>
        /// <remarks>Metodo che restituisce la lista dei filtri applicabili alla ricerca corrispondenti.</remarks>
        [HttpGet]
        [Route("GetCorrespondentFilters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCorrespondentFiltersCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetCorrespondentFiltersCommandResponse>> GetCorrespondentFilters(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken)
        {

            var results = await this._mediator.Send(new GetCorrespondentFiltersCommand());

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio che restituisce la lista dei filtri applicabili alla ricerca utenti.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <returns>Lista dei filtri applicabili alla ricerca utenti</returns>
        /// <remarks>Metodo che restituisce la lista dei filtri applicabili alla ricerca utenti.</remarks>
        [HttpGet]
        [Route("GetUserFilters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserFiltersCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetUserFiltersCommandResponse>> GetUserFilters(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken)
        {

            var results = await this._mediator.Send(new GetUserFiltersCommand());

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per la ricerca di corrispondenti.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Array di filtri di ricerca.</param>
        /// <returns>Lista dei corrispondenti discriminati secondo i filtri inseriti nella richiesta.</returns>
        /// <remarks>Metodo per la ricerca dei corrispondenti, inserendo nella richiesta i filtri disponibili tramite il metodo GetCorrespondentFilters</remarks>
        [HttpPost]
        [Route("SearchCorrespondents")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchCorrespondentsCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<SearchCorrespondentsCommandResponse>> SearchCorrespondents(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] SearchCorrespondentsCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per il reperimento di utenti interni all’applicazione.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Array di filtri di ricerca.</param>
        /// <returns>Lista degli utenti discriminati secondo i filtri inseriti nella richiesta.</returns>
        /// <remarks>Metodo per la ricerca degli utenti, inserendo nella richiesta i filtri disponibili tramite il metodo GetUserFilters</remarks>
        [HttpPost]
        [Route("SearchUsers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchUsersCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<SearchUsersCommandResponse>> SearchUsers(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] SearchUsersCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio che permette il reperimento del dettaglio di un corrispondente dato l’id. Restituisce informazioni aggiuntive.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="IdCorrespondent">Id del corrispondente cercato</param>
        /// <returns>Dettaglio del corrispondente</returns>
        /// <remarks>Metodo per il prelievo dei dettagli di un corrispondente a partire dal suo id.</remarks>
        [HttpGet]
        [Route("GetCorrespondentAdvanced")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCorrespondentAdvancedCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetCorrespondentAdvancedCommandResponse>> GetCorrespondentAdvanced(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetCorrespondentAdvancedCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per la creazione di un nuovo corrispondente esterno. Permette l'inserimento di informazioni aggiuntive.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto CorrespondentAdvanced.</param>
        /// <returns>Dettaglio del corrispondente creato</returns>
        /// <remarks>Metodo per l'aggiunta di un corrispondente esterno in rubrica.
        /// La richiesta può essere popolata nei seguenti campi
        /// Description (string): Obbligatorio. Descrizione del corrispondente.<br/>
        /// Code (string): Obbligatorio. Codice del corrispondente ricercabile da rubrica.<br/>
        /// CorrespondentType (string): Obbligatorio. Tipologia del corrispondente. Può assumere i valori “U” nel caso di Unità organizzativa e “P” nel caso di persona.<br/>
        /// City (string): Opzionale. Città del corrispondente.<br/>
        /// Province (string): Opzionale. Provincia del corrispondente.<br/>
        /// Location (string): Opzionale. Località del corrispondente.<br/>
        /// Nation (string): Opzionale. Nazione del corrispondente.<br/>
        /// PhoneNumber (string): Opzionale. Numero di telefono del corrispondente.<br/>
        /// PhoneNumber2 (string): Opzionale. Secondo numero di telefono del corrispondente.<br/>
        /// Fax (string): Opzionale. Numero del fax del corrispondente.<br/>
        /// NationalIdentificationNumber (string): Opzionale.<br/>
        /// Codice fiscale del corrispondente.<br/>
        /// Email (string): Opzionale. Mail principale del corrispondente.<br/>
        /// OtherEmails (string array): Opzionale. Mail secondarie del corrispondente.<br/>
        /// EmailsDetailed (CorrespondentEmail array): Opzionale. Lista delle mail associate al corrispondente, comprensive di note e definizione se principale. La mail indicata con Main = 1 deve essere presente nel campo Email del correspondent.<br/>
        /// AOOCode (string): Opzionale. Codice AOO.<br/>
        /// AdmCode (string): Opzionale. Codice amministrazione.<br/>
        /// Note (string): Opzionale. Note 
        /// Address (string): Opzionale. Indirizzo del corrispondente.<br/>
        /// Cap (string): Opzionale. Cap del corrispondente.<br/>
        /// CodeRegisterOrRF (string): Opzionale. Se il corrispondente deve essere disponibile soltanto per un Registro/RF valorizzare con il codice del Registro/RF.<br/>
        /// PreferredChannel (string): Opzionale. Canale preferenziale del corrispondente.<br/>
        /// </remarks>
        [HttpPut]
        [Route("AddCorrespondentAdvanced")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddCorrespondentAdvancedCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<AddCorrespondentAdvancedCommandResponse>> AddCorrespondentAdvanced(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] AddCorrespondentAdvancedCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per la modifica di un corrispondente. Permette l'inserimento di informazioni aggiuntive.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto CorrespondentAdvanced.</param>
        /// <returns>Dettaglio del corrispondente modificato</returns>
        /// <remarks>Metodo per l'aggiunta di un corrispondente esterno in rubrica.
        /// La richiesta può essere popolata nei seguenti campi
        /// Description (string): Obbligatorio. Descrizione del corrispondente.<br/>
        /// Code (string): Obbligatorio. Codice del corrispondente ricercabile da rubrica.<br/>
        /// CorrespondentType (string): Obbligatorio. Tipologia del corrispondente. Può assumere i valori “U” nel caso di Unità organizzativa e “P” nel caso di persona.<br/>
        /// City (string): Opzionale. Città del corrispondente.<br/>
        /// Province (string): Opzionale. Provincia del corrispondente.<br/>
        /// Location (string): Opzionale. Località del corrispondente.<br/>
        /// Nation (string): Opzionale. Nazione del corrispondente.<br/>
        /// PhoneNumber (string): Opzionale. Numero di telefono del corrispondente.<br/>
        /// PhoneNumber2 (string): Opzionale. Secondo numero di telefono del corrispondente.<br/>
        /// Fax (string): Opzionale. Numero del fax del corrispondente.<br/>
        /// NationalIdentificationNumber (string): Opzionale.<br/>
        /// Codice fiscale del corrispondente.<br/>
        /// Email (string): Opzionale. Mail del corrispondente.<br/>
        /// OtherEmails (string array): Opzionale. Mail secondarie del corrispondente.<br/>
        /// EmailsDetailed (CorrespondentEmail array): Opzionale. Lista delle mail associate al corrispondente, comprensive di note e definizione se principale. La mail indicata con Main = 1 deve essere presente nel campo Email del correspondent.<br/>
        /// AOOCode (string): Opzionale. Codice AOO.<br/>
        /// AdmCode (string): Opzionale. Codice amministrazione.<br/>
        /// Note (string): Opzionale. Note 
        /// Address (string): Opzionale. Indirizzo del corrispondente.<br/>
        /// Cap (string): Opzionale. Cap del corrispondente.<br/>
        /// CodeRegisterOrRF (string): Opzionale. Se il corrispondente deve essere disponibile soltanto per un Registro/RF valorizzare con il codice del Registro/RF.<br/>
        /// PreferredChannel (string): Opzionale. Canale preferenziale del corrispondente.<br/>
        /// </remarks>
        [HttpPost]
        [Route("EditCorrespondentAdvanced")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EditCorrespondentAdvancedCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<EditCorrespondentAdvancedCommandResponse>> EditCorrespondentAdvanced(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] EditCorrespondentAdvancedCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        /// <summary>
        /// Servizio per la ricerca di corrispondenti. Permette la restituzione di informazioni aggiuntive.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Array di filtri di ricerca.</param>
        /// <returns>Lista dei corrispondenti discriminati secondo i filtri inseriti nella richiesta.</returns>
        /// <remarks>Metodo per la ricerca dei corrispondenti, inserendo nella richiesta i filtri disponibili tramite il metodo GetCorrespondentFilters</remarks>
        [HttpPost]
        [Route("SearchCorrespondentsAdvanced")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchCorrespondentsAdvancedCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<SearchCorrespondentsAdvancedCommandResponse>> SearchCorrespondentsAdvanced(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] SearchCorrespondentsAdvancedCommand request)
        {

            var results = await this._mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, results);
        }


        #endregion

        #region Private Members

        protected readonly ILogger<AddressBookController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}
