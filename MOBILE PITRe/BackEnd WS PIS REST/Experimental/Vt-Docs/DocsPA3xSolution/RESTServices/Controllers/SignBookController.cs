using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SignBookController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(SignBookController));

        /// <summary>
        /// Servizio per ottenere la lista dei filtri per la ricerca delle istanze di firma.
        /// </summary>
        /// <returns>Lista dei filtri</returns>
        /// <remarks>Metodo per ottenere la lista dei filtri utilizzabile nel metodo SearchSignProcessInstances.</remarks>
        [Route("GetInstanceSearchFilters")]
        [ResponseType(typeof(GetFiltersResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getInstanceSearchFilters()
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
            GetFiltersResponse retval = await LibroFirmaManager.getInstanceSearchFilters(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per ottenere il dettaglio di un processo di firma
        /// </summary>
        /// <param name="idProcess">Id del processo</param>
        /// <returns>Dettaglio del processo</returns>
        /// <remarks>Servizio per ottenere un processo di firma a partire dal sui Id.</remarks>
        [Route("GetSignatureProcess")]
        [ResponseType(typeof(GetSignatureProcessResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getSignatureProcess(string idProcess)
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
            GetSignatureProcessResponse retval = await LibroFirmaManager.getSignatureProcess(token, idProcess);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio che restituisce la lista dei processi
        /// </summary>
        /// <returns>Lista dei processi di firma</returns>
        /// <remarks>Metodo che restituisce la lista dei processi o dei modelli di processo visibili e quindi utilizzabili dal ruolo.</remarks>
        [Route("GetSignatureProcesses")]
        [ResponseType(typeof(GetSignatureProcessesResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getSignatureProcesses()
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
            GetSignatureProcessesResponse retval = await LibroFirmaManager.getSignatureProcesses(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per ottenere una istanza dei processo di firma
        /// </summary>
        /// <param name="idProcessInstance">Id dell'istanza</param>
        /// <returns>Dettaglio dell'instanza</returns>
        /// <remarks>Servizio per ottenere una istanza dei processo a partire dal sui Id istanza.</remarks>
        [Route("GetSignProcessInstance")]
        [ResponseType(typeof(GetSignProcessInstanceResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getSignProcessInstance(string idProcessInstance)
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
            GetSignProcessInstanceResponse retval = await LibroFirmaManager.getSignProcessInstance(token, idProcessInstance);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la ricerca delle istanze di processo di firma
        /// </summary>
        /// <param name="request">Lista dei filtri con i quali ricercare le istanze.</param>
        /// <returns>Lista delle istanze filtrate</returns>
        /// <remarks>Metodo per ricercare e quindi monitorare lo stato delle istanze di processi  di firma visibili ad un determinato ruolo, a seconda dei filtri inseriti.</remarks>
        [Route("SearchSignProcessInstances")]
        [ResponseType(typeof(GetSignProcessInstancesResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> searchSignProcessInstances(SearchRequest request)
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
            GetSignProcessInstancesResponse retval = await LibroFirmaManager.searchSignProcessInstances(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per inserire un documento in un processo di Libro Firma
        /// </summary>
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
        [Route("StartSignatureProcess")]
        [ResponseType(typeof(MessageResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> startSignatureProcess(StartSignatureProcessRequest request)
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
            MessageResponse retval = await LibroFirmaManager.startSignatureProcess(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per interrompere una istanza di processo di firma
        /// </summary>
        /// <param name="request">Oggetto request</param>
        /// <returns>Messaggio di avvenuta interruzione</returns>
        /// <remarks>Metodo per interrompere una istanza dei processo a partire dal sui Id istanza.<br/>
        /// Nella richiesta è possibile inserire una nota di interruzione.</remarks>
        [Route("InterruptSignatureProcess")]
        [ResponseType(typeof(MessageResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> interruptSignatureProcess(InterruptSignatureProcessRequest request)
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
            MessageResponse retval = await LibroFirmaManager.interruptSignatureProcess(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

    }
}