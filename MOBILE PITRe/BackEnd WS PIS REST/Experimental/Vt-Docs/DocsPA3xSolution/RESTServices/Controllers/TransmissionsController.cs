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
    public class TransmissionsController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(TransmissionsController));

        /// <summary>
        /// Servizio per la trasmissione di un documento tramite un modello di trasmissione
        /// </summary>
        /// <param name="request">Oggetto request da popolare nei parametri:<br/>
        /// IdModel (string): Obbligatorio. Id del modello di trasmissione<br/>
        /// DocumentId (string): Obbligatorio. Id del documento da trasmettere.</param>
        /// <returns>Messaggio di avvenuta trasmissione.</returns>
        /// <remarks>Metodo per la trasmissione di un documento tramite un modello di trasmissione.<br/>
        /// Restituisce un messaggio di avvenuta trasmissione in caso di successo.</remarks>
        [Route("ExecuteTransmDocModel")]
        [ResponseType(typeof(TransmissionResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> executeTransmDocModel(ExecuteTransmDocModelRequest request)
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
            TransmissionResponse retval = await TransmissionsManager.executeTransmDocModel(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la trasmissione di un fascicolo tramite un modello di trasmissione
        /// </summary>
        /// <param name="request">Oggetto request da popolare nei parametri:<br/>
        /// IdModel (string): Obbligatorio. Id del modello di trasmissione<br/>
        /// IdProject (string): Obbligatorio. Id del fascicolo da trasmettere.</param>
        /// <returns>Messaggio di avvenuta trasmissione.</returns>
        /// <remarks>Metodo per la trasmissione di un fascicolo tramite un modello di trasmissione.<br/>
        /// Restituisce un messaggio di avvenuta trasmissione in caso di successo.</remarks>
        [Route("ExecuteTransmPrjModel")]
        [ResponseType(typeof(TransmissionResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> executeTransmPrjModel(ExecuteTransmPrjModelRequest request)
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
            TransmissionResponse retval = await TransmissionsManager.executeTransmPrjModel(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la trasmissione singola di un documento
        /// </summary>
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
        [Route("ExecuteTransmissionDocument")]
        [ResponseType(typeof(TransmissionResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> executeTransmissionDocument(ExecuteTransmissionDocumentRequest request)
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
            TransmissionResponse retval = await TransmissionsManager.executeTransmissionDocument(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la trasmissione singola di un fascicolo
        /// </summary>
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
        [Route("ExecuteTransmissionProject")]
        [ResponseType(typeof(TransmissionResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> executeTransmissionProject(ExecuteTransmissionProjectRequest request)
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
            TransmissionResponse retval = await TransmissionsManager.executeTransmissionProject(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento del dettaglio di un modello di trasmissione
        /// </summary>
        /// <param name="idModel">Id del modello di trasmissione</param>
        /// <param name="codeModel">Codice del modello di trasmissione</param>
        /// <returns>Dettaglio del modello di trasmissione</returns>
        /// <remarks>Metodo per il reperimento del dettaglio di un modello di trasmissione dato il codice o l’id del modello. Obbligatorio almeno un valore tra codice e id.</remarks>
        [Route("GetTransmissionModel")]
        [ResponseType(typeof(GetTransmModelResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getTransmissionModel(string idModel = "", string codeModel = "")
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
            GetTransmModelResponse retval = await TransmissionsManager.getTransmissionModel(token, idModel, codeModel);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento di tutti i modelli di trasmissione per documenti o fascicoli.
        /// </summary>
        /// <param name="request">Oggetto Request.</param>
        /// <returns>Lista dei modelli di trasmissione</returns>
        /// <remarks> Metodo per il reperimento di tutti i modelli di trasmissione per documenti o fascicoli.<br/>
        /// La richiesta deve essere popolata nei campi:<br/>
        /// Type (string): Obbligatorio. Inserire “D” per i modelli dei documenti, “F” per i modelli dei fascicoli.<br/>
        /// Registers (Register[]): Obbligatorio. Array di oggetti Register. Obbligatorio almeno un codice di un registro.
        /// </remarks>
        [Route("GetTransmissionModels")]
        [ResponseType(typeof(GetTransmModelsResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> getTransmissionModels(GetTransmModelsRequest request)
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
            GetTransmModelsResponse retval = await TransmissionsManager.getTransmissionModels(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la cessione del diritto di scrittura/lettura su un oggetto
        /// </summary>
        /// <param name="request">Oggetto request: <br/> 
        /// RightToKeep (string): Obbligatorio. Diritto da mantenere. Può avere 3 valori: WRITE, READ e NONE. <br/>
        /// IdObject (string): Obbligatorio. Id dell’oggetto sul quale si vogliono cedere i diritti</param>
        /// <returns>Messaggio di avvenuta cessione dei diritti.</returns>
        /// <remarks>Metodo per la cessione del diritto di scrittura/lettura su un documento o fascicolo.</remarks>
        [Route("GiveUpRights")]
        [ResponseType(typeof(MessageResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> giveUpRights(GiveUpRightsRequest request)
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
            MessageResponse retval = await TransmissionsManager.giveUpRights(token, request.RightToKeep, request.IdObject);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }
    }
}