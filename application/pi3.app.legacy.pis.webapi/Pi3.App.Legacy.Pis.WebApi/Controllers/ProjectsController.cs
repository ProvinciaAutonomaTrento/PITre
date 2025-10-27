// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.FollowDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateFolder;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateProjectWithArchivePlan;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.EditPrjStateDiagram;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.EditProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FollowProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetArchivePlan;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetArchivePlanFilters;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectFilters;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectFolders;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectsByDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectStateDiagram;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectTemplates;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectWithArchivePlan;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.OpenCloseProject;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SearchArchivePlans;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SearchProjects;
using Pi3.App.Legacy.Pis.WebApi.Models;

namespace Pi3.App.Legacy.Pis.WebApi.Controllers
{
    public class ProjectsController : Controller
    {
        #region Public Members
        public ProjectsController(
            ILogger<ProjectsController> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        /// <summary>
        /// Servizio che restituisce la lista dei filtri applicabili alla ricerca dei fascicoli.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <returns>Lista dei filtri applicabili alla ricerca fascicoli</returns>
        /// <remarks>Metodo che restituisce la lista dei filtri applicabili alla ricerca dei fascicoli.</remarks>
        [HttpGet]
        [Route("GetProjectFilters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectFiltersCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetProjectFiltersCommandResponse>> GetProjectFilters(
            [FromHeader] string Instance,
            [FromHeader] string AuthToken)
        {

            var results = await this._mediator.Send(new GetProjectFiltersCommand());

            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento dei dati di un fascicolo
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idProject">Id del fascicolo</param>
        /// <param name="codeProject">Codice del fascicolo</param>
        /// <param name="classificationSchemeId">Id del titolario</param>
        /// <returns></returns>
        /// <remarks>Servizio per il reperimento dei dati di un fascicolo dato un codice o l’id. Obbligatorio l’id del fascicolo oppure la coppia codice e id del titolario.</remarks>
        [HttpGet]
        [Route("GetProject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetProjectCommandResponse>> GetProject(
          [FromHeader] string Instance,
          [FromHeader] string AuthToken,
          GetProjectCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio che permette la creazione di un fascicolo.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del fascicolo</returns>
        /// <remarks>Metodo che permette la creazione di un fascicolo.<br/>
        /// L'oggetto request va popolato nei seguenti campi:<br/>
        /// CodeNodeClassification (string): Obbligatorio. Nodo del titolario nel quale creare il fascicolo. <br/>
        ///ClassificationScheme (string): Obbligatorio. Id del titolario oin cui creare il fascicolo. <br/>
        ///Description (string): Obbligatorio. Descrizione del fascicolo. <br/>
        ///Template (Template): Opzionale. Tipologia del fascicolo. <br/>
        ///Paper (Boolean): Opzionale. Indica se il fascicolo è cartaceo. <br/>
        ///CollocationDate (string): Opzionale. Indica la data di collocazione del fascicolo. <br/>
        ///PhysicsCollocation (string): Opzionale. Indica la collocazione fisica del fascicolo (inserire il system id). <br/>
        ///Private (Boolean): Opzionale. Se true indica che il fascicolo è privato. <br/>
        ///Note (Note): Opzionale. Nota del fascicolo.</remarks>
        [HttpPut]
        [Route("CreateProject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateProjectCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<CreateProjectCommandResponse>> CreateProject(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] CreateProjectCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la modifica di un fascicolo
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del fascicolo modificato.</returns>
        /// <remarks>Metodo per la modifica di un fascicolo. La richiesta è nella stessa forma di CreateProject. <br/>
        /// Essendo un metodo di modifica, si consiglia di passare in ingresso un fascicolo prelevato tramite GetProject</remarks>
        [HttpPost]
        [Route("EditProject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EditProjectCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<EditProjectCommandResponse>> EditProject(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] EditProjectCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio che permette la ricerca di fascicoli.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request contenente un array di filtri, il numero della pagina ed il numero di elementi interno ad una pagina</param>
        /// <returns>Lista dei fsascicoli filtrati</returns>
        /// <remarks>Metodo per la ricerca dei fascicoli</remarks>
        [HttpPost]
        [Route("SearchProjects")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchProjectsCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<SearchProjectsCommandResponse>> SearchProjects(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] SearchProjectsCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento di tutte le tipologie di fascicoli.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <returns>Lista delle tipologie visibili all'utente</returns>
        /// <remarks>Metodo per il reperimento di tutte le tipologie di fascicoli visibili al ruolo dell'utente che esegue la richiesta.</remarks>
        [HttpGet]
        [Route("GetProjectTemplates")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectTemplatesCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetProjectTemplatesCommandResponse>> GetProjectTemplates(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken)
        {
            var results = await this._mediator.Send(new GetProjectTemplatesCommand());
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento del dettaglio di una tipologia di fascicolo
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="descriptionTemplate">Descrizione della tipologia</param>
        /// <param name="idTemplate">Id della tipologia</param>
        /// <returns>Dettaglio della tipologia</returns>
        /// <remarks>Servizio per il reperimento del dettaglio di una tipologia di fascicolo dato il nome o l’id. Obbligatorio un solo valore tra descrizione o id della tipologia.</remarks>
        [HttpGet]
        [Route("GetTemplateProject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTemplateProjectCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetTemplateProjectCommandResponse>> GetTemplateProject(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetTemplateProjectCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento dei fascicoli in cui è inserito un documento.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idDocument">Id del documento</param>
        /// <param name="signature">Segnatura di protocollo del documento</param>
        /// <returns>Lista dei fascicoli</returns>
        /// <remarks>Metodo per il reperimento dei fascicoli in cui è fascicolato un documento. Obbligatorio l’id o la segnatura del documento.</remarks>
        [HttpGet]
        [Route("GetProjectsByDocument")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectsByDocumentCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetProjectsByDocumentCommandResponse>> GetProjectsByDocument(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetProjectsByDocumentCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per modificare lo stato del diagramma associato ad un documento.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request: <br/> StateOfDiagram (string): Obbligatorio. Stato in cui avanzare il fascicolo.<br/>
        /// IdProject (string): Opzionale. Id del fascicolo<br/>
        /// CodeProject (string): Opzionale. Codice del fascicolo<br/>
        /// ClassificationSchemeId (string): Opzionale. Id del titolario</param>
        /// <returns>Messaggio di avvenuta modifica dello stato.</returns>
        /// <remarks>Metodo per modificare lo stato del diagramma associato ad un fascicolo. Obbligatorio l’id del fascicolo oppure la coppia codice e id del titolario.<br/>
        /// Il parametro stateOfDiagram deve essere popolato con la descrizione di uno stato successivo a quello attuale del fascicolo. In caso contrario viene restituito errore.</remarks>
        [HttpPost]
        [Route("EditPrjStateDiagram")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EditPrjStateDiagramCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<EditPrjStateDiagramCommandResponse>> EditPrjStateDiagram(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] EditPrjStateDiagramCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il prelievo dello stato del diagramma di un documento
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idProject">Id del fascicolo</param>
        /// <param name="codeProject">Codice del fascicolo</param>
        /// <param name="classificationSchemeId">Id del titolario</param>
        /// <returns>Dettaglio dello stato del diagramma.</returns>
        /// <remarks>Metodo per il prelievo dello stato del diagramma di un fascicolo. Obbligatorio l’id del fascicolo oppure la coppia codice e id del titolario.<br/>
        /// Se al fascicolo non è associato un diagramma di stato viene restituito errore.</remarks>
        [HttpGet]
        [Route("GetProjectStateDiagram")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectStateDiagramCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetProjectStateDiagramCommandResponse>> GetProjectStateDiagram(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetProjectStateDiagramCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per la creazione di un sottofascicolo.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del sottofascicolo creato</returns>
        /// <remarks>Metodo per la creazione di un sottofascicolo. <br/>
        /// L'oggetto request va popolato nei seguenti campi:<br/>
        /// IdProject (string): Opzionale. Id del fascicolo nel quale si vuole creare il sottofascicolo. Uno e solo uno tra IdProject e CodeProject è un parametro obbligatorio. <br/>
        /// CodeProject (string): Opzionale. Codice del fascicolo nel quale si vuole creare il sottofascicolo. Uno e solo uno tra IdProject e CodeProject è un parametro obbligatorio. <br/>
        /// ClassificationSchemeId (string): Opzionale. Id del titolario. Diviene obbligatorio in caso di immissione di CodeProject. <br/>
        /// FolderDescription (string): Obbligatorio. Descrizione del sottofascicolo. <br/>
        /// IdParentFolder (string): Opzionale. Id del fascicolo/sottofascicolo padre. Necessario per la creazione di un sottofascicolo non di primo livello.</remarks>
        [HttpPut]
        [Route("CreateFolder")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateFolderCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<CreateFolderCommandResponse>> CreateFolder(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] CreateFolderCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il prelievo dei sottofascicoli all’interno di un fascicolo.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idProject">Id del fascicolo</param>
        /// <param name="classificationSchemeId">Id del titolario</param>
        /// <param name="codeProject">Codice del fascicolo</param>
        /// <returns>Lista dei sottofascicoli</returns>
        /// <remarks>Metodo per il prelievo dei sottofascicoli all'interno di un fascicolo.</remarks>
        [HttpGet]
        [Route("GetProjectFolders")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectFoldersCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetProjectFoldersCommandResponse>> GetProjectFolders(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetProjectFoldersCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per aprire o chiudere un fascicolo.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del fascicolo modificato.</returns>
        /// <remarks>Metodo per aprire o chiudere un fascicolo.<br/>
        /// Essendo un metodo di modifica, si consiglia di passare in ingresso un fascicolo prelevato tramite GetProject</remarks>
        [HttpPost]
        [Route("OpenCloseProject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OpenCloseProjectCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<OpenCloseProjectCommandResponse>> OpenCloseProject(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] OpenCloseProjectCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio che restituisce la lista dei filtri applicabili alla ricerca dei piani di conservazione.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <returns>Lista dei filtri applicabili alla ricerca dei piani di conservazione</returns>
        /// <remarks>Metodo che restituisce la lista dei filtri applicabili alla ricerca dei piani di conservazione.</remarks>
        [HttpGet]
        [Route("GetArchivePlanFilters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetArchivePlanFiltersCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetArchivePlanFiltersCommandResponse>> GetArchivePlanFilters(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken)
        {
            var results = await this._mediator.Send(new GetArchivePlanFiltersCommand());
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio che permette la ricerca dei piani di conservazione.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request contenente un array di filtri, il numero della pagina ed il numero di elementi interno ad una pagina</param>
        /// <returns>Lista dei piani di conservazione filtrati e paginati</returns>
        /// <remarks>Metodo per la ricerca dei piani di conservazione</remarks>
        [HttpPost]
        [Route("SearchArchivePlans")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchArchivePlansCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<SearchArchivePlansCommandResponse>> SearchArchivePlans(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] SearchArchivePlansCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento dei dati di un piano di conservazione
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idArchivePlan">Id del piano di conservazione</param>
        /// <returns></returns>
        /// <remarks>Servizio per il reperimento dei dati di un piano di conservazione dato l’id.</remarks>
        [HttpGet]
        [Route("GetArchivePlan")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetArchivePlanCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetArchivePlanCommandResponse>> GetArchivePlan(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetArchivePlanCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per il reperimento dei dati di un fascicolo con le informazioni del piano di archiviazione
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="idProject">Id del fascicolo</param>
        /// <param name="codeProject">Codice del fascicolo</param>
        /// <param name="classificationSchemeId">Id del titolario</param>
        /// <returns></returns>
        /// <remarks>Servizio per il reperimento dei dati di un fascicolo dato un codice o l’id. Obbligatorio l’id del fascicolo oppure la coppia codice e id del titolario. Sono presenti le informazioni sul piano di archiviazione</remarks>
        [HttpGet]
        [Route("GetProjectWithArchivePlan")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProjectWithArchivePlanCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<GetProjectWithArchivePlanCommandResponse>> GetProjectWithArchivePlan(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  GetProjectWithArchivePlanCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio che permette la creazione di un fascicolo, con la possibilità di specificare il piano di conservazione.
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del fascicolo</returns>
        /// <remarks>Metodo che permette la creazione di un fascicolo.<br/>
        /// L'oggetto request va popolato nei seguenti campi:<br/>
        /// CodeNodeClassification (string): Obbligatorio. Nodo del titolario nel quale creare il fascicolo. <br/>
        ///ClassificationScheme (string): Obbligatorio. Id del titolario oin cui creare il fascicolo. <br/>
        ///Description (string): Obbligatorio. Descrizione del fascicolo. <br/>
        ///Template (Template): Opzionale. Tipologia del fascicolo. <br/>
        ///Paper (Boolean): Opzionale. Indica se il fascicolo è cartaceo. <br/>
        ///CollocationDate (string): Opzionale. Indica la data di collocazione del fascicolo. <br/>
        ///PhysicsCollocation (string): Opzionale. Indica la collocazione fisica del fascicolo (inserire il system id). <br/>
        ///Private (Boolean): Opzionale. Se true indica che il fascicolo è privato. <br/>
        ///Note (Note): Opzionale. Nota del fascicolo. <br/>
        ///ArchivePlan (ArchivePlan): Piano di conservazione. Obbligatorio a seconda delle impostazioni dell'amministrazione. Necessario almeno il parametro ID dell'oggetto</remarks>
        [HttpPut]
        [Route("CreateProjectWithArchivePlan")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateProjectWithArchivePlanCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<CreateProjectWithArchivePlanCommandResponse>> CreateProjectWithArchivePlan(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] CreateProjectWithArchivePlanCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }

        /// <summary>
        /// Servizio per seguire un fascicolo tramite integrazione FollowProject
        /// </summary>
        /// <param name="Instance">Istanza DB alla quale connettersi</param>
        /// <param name="AuthToken">Token di autenticazione</param>
        /// <param name="request">Oggetto request con IdObject che contiene l'id del fascicolo, e l'operazione da eseguire</param>
        /// <returns>Esito del follow</returns>
        /// <remarks>Il parametro IdObject va popolato con l'id del documento da seguire.
        /// Il parametro operation va popolato con 0 (AddFolder) per seguire un fascicolo, 3 (RemoveFolder) per smettere di seguirlo. 
        /// </remarks>
        [HttpPost]
        [Route("FollowProject")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FollowProjectCommandResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionInfo))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionInfo))]
        public async Task<ActionResult<FollowProjectCommandResponse>> FollowDocument(
                  [FromHeader] string Instance,
                  [FromHeader] string AuthToken,
                  [FromBody] FollowProjectCommand request)
        {
            var results = await this._mediator.Send(request);
            return StatusCode(StatusCodes.Status200OK, results);
        }
        #endregion

        #region Private Members

        protected readonly ILogger<ProjectsController> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}
