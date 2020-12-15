using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using RESTServices.Manager;
using log4net;
using RESTServices.Model.Responses;
using RESTServices.Model.Requests;

namespace RESTServices.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ProjectsController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(ProjectsController));

        /// <summary>
        /// Servizio che restituisce la lista dei filtri applicabili alla ricerca dei fascicoli.
        /// </summary>
        /// <returns>Lista dei filtri applicabili alla ricerca fascicoli</returns>
        /// <remarks>Metodo che restituisce la lista dei filtri applicabili alla ricerca dei fascicoli.</remarks>
        [Route("GetProjectFilters")]
        [ResponseType(typeof(GetFiltersResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getDocumentFilters()
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header"; return errorMessage;
            }
            GetFiltersResponse retval = await ProjectsManager.getProjectFilters(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei dati di un fascicolo
        /// </summary>
        /// <param name="idProject">Id del fascicolo</param>
        /// <param name="codeProject">Codice del fascicolo</param>
        /// <param name="classificationSchemeId">Id del titolario</param>
        /// <returns></returns>
        /// <remarks>Servizio per il reperimento dei dati di un fascicolo dato un codice o l’id. Obbligatorio l’id del fascicolo oppure la coppia codice e id del titolario.</remarks>
        [Route("GetProject")]
        [ResponseType(typeof(GetProjectResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getProject(string idProject = "", string codeProject = "", string classificationSchemeId = "")
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header";
                return errorMessage;
            }
            GetProjectResponse retval = await ProjectsManager.getProject(token, idProject, codeProject, classificationSchemeId);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio che permette la creazione di un fascicolo.
        /// </summary>
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
        [Route("CreateProject")]
        [ResponseType(typeof(GetProjectResponse))]
        [HttpPut]
        public async Task<HttpResponseMessage> createProject(CreateProjectRequest request)
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header"; return errorMessage;
            }
            GetProjectResponse retval = await ProjectsManager.createProject(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la modifica di un fascicolo
        /// </summary>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del fascicolo modificato.</returns>
        /// <remarks>Metodo per la modifica di un fascicolo. La richiesta è nella stessa forma di CreateProject. <br/>
        /// Essendo un metodo di modifica, si consiglia di passare in ingresso un fascicolo prelevato tramite GetProject</remarks>
        [Route("EditProject")]
        [ResponseType(typeof(GetProjectResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> editProject(CreateProjectRequest request)
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header"; return errorMessage;
            }
            GetProjectResponse retval = await ProjectsManager.editProject(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }


        /// <summary>
        /// Servizio che permette la ricerca di fascicoli.
        /// </summary>
        /// <param name="request">Oggetto request contenente un array di filtri, il numero della pagina ed il numero di elementi interno ad una pagina</param>
        /// <returns>Lista dei fsascicoli filtrati</returns>
        /// <remarks>Metodo per la ricerca dei fascicoli</remarks>
        [Route("SearchProjects")]
        [ResponseType(typeof(SearchProjectsResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> searchProjects(SearchRequest request)
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header"; return errorMessage;
            }
            SearchProjectsResponse retval = await ProjectsManager.searchProjects(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento di tutte le tipologie di fascicoli.
        /// </summary>
        /// <returns>Lista delle tipologie visibili all'utente</returns>
        /// <remarks>Metodo per il reperimento di tutte le tipologie di fascicoli visibili al ruolo dell'utente che esegue la richiesta.</remarks>
        [Route("GetProjectTemplates")]
        [ResponseType(typeof(GetTemplatesResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getProjectTemplates()
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header"; return errorMessage;
            }
            GetTemplatesResponse retval = await ProjectsManager.getProjectTemplates(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento del dettaglio di una tipologia di fascicolo
        /// </summary>
        /// <param name="descriptionTemplate">Descrizione della tipologia</param>
        /// <param name="idTemplate">Id della tipologia</param>
        /// <returns>Dettaglio della tipologia</returns>
        /// <remarks>Servizio per il reperimento del dettaglio di una tipologia di fascicolo dato il nome o l’id. Obbligatorio un solo valore tra descrizione o id della tipologia.</remarks>
        [Route("GetTemplateProject")]
        [ResponseType(typeof(GetTemplateResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getTemplateProject(string descriptionTemplate = "", string idTemplate = "")
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header"; return errorMessage;
            }
            GetTemplateResponse retval = await ProjectsManager.getTemplateProject(token, descriptionTemplate, idTemplate);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei fascicoli in cui è inserito un documento.
        /// </summary>
        /// <param name="idDocument">Id del documento</param>
        /// <param name="signature">Segnatura di protocollo del documento</param>
        /// <returns>Lista dei fascicoli</returns>
        /// <remarks>Metodo per il reperimento dei fascicoli in cui è fascicolato un documento. Obbligatorio l’id o la segnatura del documento.</remarks>
        [Route("GetProjectsByDocument")]
        [ResponseType(typeof(SearchProjectsResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getProjectsByDocument(string idDocument = "", string signature = "")
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header"; return errorMessage;
            }
            SearchProjectsResponse retval = await ProjectsManager.getProjectsByDocument(token, idDocument, signature);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per modificare lo stato del diagramma associato ad un documento.
        /// </summary>
        /// <param name="request">Oggetto request: <br/> StateOfDiagram (string): Obbligatorio. Stato in cui avanzare il fascicolo.<br/>
        /// IdProject (string): Opzionale. Id del fascicolo<br/>
        /// CodeProject (string): Opzionale. Codice del fascicolo<br/>
        /// ClassificationSchemeId (string): Opzionale. Id del titolario</param>
        /// <returns>Messaggio di avvenuta modifica dello stato.</returns>
        /// <remarks>Metodo per modificare lo stato del diagramma associato ad un fascicolo. Obbligatorio l’id del fascicolo oppure la coppia codice e id del titolario.<br/>
        /// Il parametro stateOfDiagram deve essere popolato con la descrizione di uno stato successivo a quello attuale del fascicolo. In caso contrario viene restituito errore.</remarks>
        [Route("EditPrjStateDiagram")]
        [ResponseType(typeof(MessageResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> editPrjStateDiagram(EditPrjStateDiagramRequest request)
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header"; return errorMessage;
            }
            MessageResponse retval = await ProjectsManager.editPrjStateDiagram(token, request.StateOfDiagram, request.IdProject, request.ClassificationSchemeId, request.CodeProject);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il prelievo dello stato del diagramma di un documento
        /// </summary>
        /// <param name="idProject">Id del fascicolo</param>
        /// <param name="codeProject">Codice del fascicolo</param>
        /// <param name="classificationSchemeId">Id del titolario</param>
        /// <returns>Dettaglio dello stato del diagramma.</returns>
        /// <remarks>Metodo per il prelievo dello stato del diagramma di un fascicolo. Obbligatorio l’id del fascicolo oppure la coppia codice e id del titolario.<br/>
        /// Se al fascicolo non è associato un diagramma di stato viene restituito errore.</remarks>
        [Route("GetProjectStateDiagram")]
        [ResponseType(typeof(GetStateOfDiagramResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getProjectStateDiagram(string idProject = "", string classificationSchemeId = "", string codeProject = "")
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header"; return errorMessage;
            }
            GetStateOfDiagramResponse retval = await ProjectsManager.getProjectStateDiagram(token, idProject, classificationSchemeId, codeProject);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la creazione di un sottofascicolo.
        /// </summary>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del sottofascicolo creato</returns>
        /// <remarks>Metodo per la creazione di un sottofascicolo. <br/>
        /// L'oggetto request va popolato nei seguenti campi:<br/>
        /// IdProject (string): Opzionale. Id del fascicolo nel quale si vuole creare il sottofascicolo. Uno e solo uno tra IdProject e CodeProject è un parametro obbligatorio. <br/>
        /// CodeProject (string): Opzionale. Codice del fascicolo nel quale si vuole creare il sottofascicolo. Uno e solo uno tra IdProject e CodeProject è un parametro obbligatorio. <br/>
        /// ClassificationSchemeId (string): Opzionale. Id del titolario. Diviene obbligatorio in caso di immissione di CodeProject. <br/>
        /// FolderDescription (string): Obbligatorio. Descrizione del sottofascicolo. <br/>
        /// IdParentFolder (string): Opzionale. Id del fascicolo/sottofascicolo padre. Necessario per la creazione di un sottofascicolo non di primo livello.</remarks>
        [Route("CreateFolder")]
        [ResponseType(typeof(GetFolderResponse))]
        [HttpPut]
        public async Task<HttpResponseMessage> createFolder(CreateFolderRequest request)
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header"; return errorMessage;
            }
            GetFolderResponse retval = await ProjectsManager.createFolder(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il prelievo dei sottofascicoli all’interno di un fascicolo.
        /// </summary>
        /// <param name="idProject">Id del fascicolo</param>
        /// <param name="classificationSchemeId">Id del titolario</param>
        /// <param name="codeProject">Codice del fascicolo</param>
        /// <returns>Lista dei sottofascicoli</returns>
        /// <remarks>Metodo per il prelievo dei sottofascicoli all'interno di un fascicolo.</remarks>
        [Route("GetProjectFolders")]
        [ResponseType(typeof(GetProjectFoldersResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getProjectFolders(string idProject = "", string classificationSchemeId = "", string codeProject = "")
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header"; return errorMessage;
            }
            GetProjectFoldersResponse retval = await ProjectsManager.getProjectFolders(token, idProject, classificationSchemeId, codeProject);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per aprire o chiudere un fascicolo.
        /// </summary>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del fascicolo modificato.</returns>
        /// <remarks>Metodo per aprire o chiudere un fascicolo.<br/>
        /// Essendo un metodo di modifica, si consiglia di passare in ingresso un fascicolo prelevato tramite GetProject</remarks>
        [Route("OpenCloseProject")]
        [ResponseType(typeof(GetProjectResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> openCloseProject(OpenCloseProjectRequest request)
        {
            HttpResponseMessage response = null;
            string token = "";
            if (Request.Headers.Contains("AuthToken"))
            {
                token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            }
            else
            {
                logger.Error("Missing AuthToken Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing AuthToken Header"; return errorMessage;
            }
            GetProjectResponse retval = await ProjectsManager.openCloseProject(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

    }
}