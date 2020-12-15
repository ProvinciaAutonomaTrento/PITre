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
using DocsPaVO.Mobile.Responses;
using DocsPaRESTWS.Model;
using DocsPaRESTWS.Manager;

namespace DocsPaRESTWS.Controllers
{
    [EnableCors("*", "*", "*")]
    public class DocFascController : ApiController
    {
        /// <summary>
        /// Informazioni documento
        /// </summary>
        /// <param name="idDoc">Id del documento</param>
        /// <param name="idEvento">Id dell'evento di log</param>
        /// <param name="idTrasm">Id della trasmissione</param>
        /// <returns>Informazioni sul documento</returns>
        /// <remarks>Metodo per il prelievo delle informazioni di un documento. Il parametro idDoc è obbligatorio per identificare il documento.
        /// Il parametro idTrasm consente di prelevare le informazioni per la trasmissione.
        /// Il parametro idEvento, in caso di assenza del parametro idTrasm, consente di rimuovere la notifica della TodoList.
        /// Code restituiti:
        /// 0: OK
        /// 1: Errore generico
        /// </remarks>
        [Route("DocInfo")]
        [ResponseType(typeof(GetDocInfoResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getDocInfo(string idDoc, string idEvento = "", string idTrasm= "")
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            GetDocInfoRequest request = new GetDocInfoRequest();
            request.idDoc= idDoc;
            request.IdEvento = idEvento;
            request.IdTrasm= idTrasm;
            GetDocInfoResponse retval = await MobileManager.getDocInfo(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Rimuovi notifica
        /// </summary>
        /// <param name="idEvento">Id dell'evento dal quale deriva la notifica</param>
        /// <returns>Valore booleano che comunica l'avvenuta rimozione</returns>
        /// <remarks>Metodo utilizzato per la rimozione di una notifica dalla todoList.</remarks>
        [Route("RimuoviNotifica")]
        [ResponseType(typeof(bool))]
        [HttpPost]
        public async Task<HttpResponseMessage> rimuoviNotifica(string idEvento)
        {
            HttpResponseMessage response = null;
            bool retval = await MobileManager.rimuoviNotifica(idEvento);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Get File
        /// </summary>
        /// <param name="idDoc">Id del documento relativo al file.</param>
        /// <param name="getSignedFile">Se popolato con 1, restituisce il file con busta di firma se presente</param>
        /// <returns>Oggetto FileInfo contenente le informazioni del file ed il contenuto binario</returns>
        /// <remarks>Metodo utilizzato per il prelievo di un file, sia il contenuto binario che le informazioni riguardanti.
        /// Code restituiti:
        /// 0: OK
        /// 1: Errore generico
        /// </remarks>
        [Route("File")]
        [ResponseType(typeof(GetFileResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getFile(string idDoc, string getSignedFile = "0")
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            GetFileResponse retval = await MobileManager.getFile(token, idDoc, getSignedFile);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Gestione area di lavoro
        /// </summary>
        /// <param name="request">
        /// Oggetto AdlActionRequest, campi da popolare:
        /// ADLAction (string): Aggiunta o rimozione in Area di Lavoro. Valori ammessi "ADD" o "REMOVE"
        /// IdElemento (string): Id dell'elemento sul quale eseguire l'azione
        /// TipoElemento (string): se popolato con "F", indica che si sta lavorando su un fascicolo
        /// </param>
        /// <returns>Codice di azione eseguita correttamente</returns>
        /// <remarks>
        /// Metodo per la gestione di documenti e fascicoli in area di lavoro.
        /// La richiesta contiene i seguenti parametri:
        /// IdElemento: id dell'elemento sul quale si vuole eseguire l'azione
        /// TipoElemento: se popolato con "F", indica che l'elemento è un fascicolo
        /// ADLAction: azione da eseguire sull'elemento. Se popolato con "ADD", inserisce in area di lavoro. Con "REMOVE", rimuove dall'area di lavoro.
        /// Code restituiti:
        /// 0 - OK
        /// 1 - Errore generico
        /// </remarks>
        [Route("AdlAction")]
        [ResponseType(typeof(ADLActionResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> AdlAction(AdlActionRequest request)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            ADLActionResponse retval = await MobileManager.ADLAction(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Informazioni fascicolo
        /// </summary>
        /// <param name="idFasc">Id del fascicolo</param>
        /// <param name="idTrasm">Id della trasmissione</param>
        /// <returns>Informazioni sul fascicolo</returns>
        /// <remarks>Metodo per il prelievo delle informazioni di un fascicolo. Il parametro idFasc è obbligatorio per identificare il fascicolo.
        /// Il parametro idTrasm consente di prelevare le informazioni per la trasmissione.
        /// Code restituiti:
        /// 0: OK
        /// 1: Errore generico</remarks>
        [Route("FascInfo")]
        [ResponseType(typeof(GetFascInfoResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getFascInfo(string idFasc, string idTrasm="")
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            GetFascInfoRequest request = new GetFascInfoRequest();
            request.IdFasc = idFasc;
            request.IdTrasm = idTrasm;
            GetFascInfoResponse retval = await MobileManager.getFascInfo(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Condividi documento
        /// </summary>
        /// <param name="idpeople">Id people della persona verso la quale condividere il documento</param>
        /// <param name="idDocumento">Id del documento da condividere</param>
        /// <returns>Stringa contenente la chiave di condivisione. "ERRORE" altrimenti.</returns>
        /// <remarks>Metodo utilizzato per la creazione di una chiave di condivisione. Questa chiave riguarda un utente, un documento ed è limitata temporalmente a 72 ore</remarks>
        [Route("Condividi")]
        [ResponseType(typeof(string))]
        [HttpGet]
        public async Task<HttpResponseMessage> condividiDocumento(string idpeople, string idDocumento)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            string retval = await MobileManager.getTokenCondivisioneDocumento(token, idpeople, idDocumento);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }
        
        /// <summary>
        /// Documento condiviso
        /// </summary>
        /// <param name="chiaveDoc">Chiave di condivisione del documento</param>
        /// <returns>Codice:
        /// 0 - OK
        /// 1 - Utente errato
        /// 2 - Token scaduto
        /// 3 - Errore generico
        /// In caso di codice zero, restituisce le informazioni riguardanti il documento</returns>
        /// <remarks>Metodo utilizzato per prelevare le informazioni di un documento condiviso.</remarks>
        [Route("DocumentoCondiviso")]
        [ResponseType(typeof(DocCondivisoResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> docCondiviso(string chiaveDoc)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            DocCondivisoResponse retval = await MobileManager.ctrlTokenCondivisioneDoc(token, chiaveDoc);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }
    }
}
