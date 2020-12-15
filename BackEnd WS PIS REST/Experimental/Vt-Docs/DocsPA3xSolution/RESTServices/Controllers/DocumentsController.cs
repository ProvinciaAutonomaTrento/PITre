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
    public class DocumentsController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocumentsController));


        /// <summary>
        /// Servizio che restituisce la lista dei filtri applicabili alla ricerca documenti.
        /// </summary>
        /// <returns>Lista dei filtri applicabili ai documenti</returns>
        /// <remarks>Metodo che restituisce la lista dei filtri applicabili alla ricerca documenti.</remarks>
        [Route("GetDocumentFilters")]
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
                errorMessage.ReasonPhrase = "Missing AuthToken Header";
                return errorMessage;
            }
            GetFiltersResponse retval = await DocumentsManager.getDocumentsFilters(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento del dettaglio di un documento.
        /// </summary>
        /// <param name="request">Oggetto GetDocumentRequest contenente i seguenti campi
        /// IdDocument (string): id del documento
        /// Signature (string): segnatura di protocollo del documento, da inserire in alternativa a IdDocument
        /// GetFile (bool): se vero, preleva i file associati al documento, sia principale che allegati
        /// GetFileWithSignature (string): se impostato a 1, preleva i file con la busta di firma
        /// </param>
        /// <returns>Dettaglio del documento.</returns>
        /// <remarks>Metodo per il reperimento del dettaglio di un documento data la segnatura o l’id.
        /// Obbligatorio un valore tra id del documento o segnatura.</remarks>
        [Route("GetDocument")]
        [ResponseType(typeof(GetDocumentResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> getDocument(GetDocumentRequest request)
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
            GetDocumentResponse retval = await DocumentsManager.getDocument(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la creazione di un documento
        /// </summary>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del documento creato</returns>
        /// <remarks>Metodo per la creazione di un documento non protocollato, protocollato in arrivo, in uscita, interno oppure un predisposto in arrivo, in uscita oppure interno.<br/>
        /// La richiesta consiste nei parametri:<br/>
        /// Document (Document): Obbligatorio. Documento che deve essere creato. Vedere le tabelle successive per le varie tipologie di documento. <br/>
        /// CodeRegister (String): Opzionale. Codice del registro, obbligatorio nel caso di documenti protocollati. <br/>
        /// CodeRF (String): Opzionale. Codice dell’RF per la segnatura. <br/><br/>
        /// L'oggetto document consiste nei seguenti parametri: (esempio creazione Documento non protocollato)
        /// DocumentType (String): Obbligatorio. Indica la tipologia del documento. Inserire il valore “G”. <br/>
        /// Object (String): Obbligatorio. Oggetto del documento. <br/>
        /// Note (Note[]): Opzionale. Possibilità di inserire una o più note nel documento. <br/>
        /// Template (Template): Opzionale. Tipologia del documento che si vuole associare al documento con i relativi valori. <br/>
        /// MainDocument (File): Opzionale. Documento principale.
        /// </remarks>
        [Route("CreateDocument")]
        [ResponseType(typeof(CreateDocumentResponse))]
        [HttpPut]
        public async Task<HttpResponseMessage> createDocument(CreateDocumentRequest request)
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
            CreateDocumentResponse retval = await DocumentsManager.createDocument(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio che crea un nuovo documento e lo inserisce nel fascicolo indicato
        /// </summary>
        /// <param name="request">Oggetto request</param>
        /// <returns>Dettaglio del documento creato</returns>
        /// <remarks>Metodo per la creazione di un documento non protocollato, protocollato in arrivo, in uscita, interno oppure un predisposto in arrivo, in uscita oppure interno.<br/>
        /// Il metodo procede quindi ad inserirlo nel fascicolo indicato.<br/>
        /// La richiesta consiste nei parametri:<br/>
        /// Document (Document): Obbligatorio. Documento che deve essere creato. Vedere le tabelle successive per le varie tipologie di documento. <br/>
        /// CodeRegister (String): Opzionale. Codice del registro, obbligatorio nel caso di documenti protocollati. <br/>
        /// CodeRF (String): Opzionale. Codice dell’RF per la segnatura. <br/>
        /// CodeProject (string): Opzionale. Codice del fascicolo nel quale fascicolare il documento, il codice prende soltanto i fascicoli nei titolari attivi. Da utilizzare in coppia con ClassificationSchemaId. <br/>
        /// IdProject (string): Opzionale. Id del fascicolo nel quale fascicolare il documento. Alternativo alla coppia CodeProject/ClassificationSchemeId <br/>
        /// ClassificationSchemeId (string): Opzionale. Id del titolario.<br/><br/>
        /// L'oggetto document consiste nei seguenti parametri: (esempio creazione Documento non protocollato)
        /// DocumentType (String): Obbligatorio. Indica la tipologia del documento. Inserire il valore “G”. <br/>
        /// Object (String): Obbligatorio. Oggetto del documento. <br/>
        /// Note (Note[]): Opzionale. Possibilità di inserire una o più note nel documento. <br/>
        /// Template (Template): Opzionale. Tipologia del documento che si vuole associare al documento con i relativi valori. <br/>
        /// MainDocument (File): Opzionale. Documento principale.
        /// </remarks>
        [Route("CreateDocumentAndAddInProject")]
        [ResponseType(typeof(CreateDocumentResponse))]
        [HttpPut]
        public async Task<HttpResponseMessage> createDocumentAndAddInPrj(CreateDocAndAddInPrjRequest request)
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
            CreateDocumentResponse retval = await DocumentsManager.createDocAndAddInPrj(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il caricamento di un file e/o allegato in un documento
        /// </summary>
        /// <param name="request">Oggetto request</param>
        /// <returns>Messaggio di avvenuta creazione allegato o versione</returns>
        /// <remarks>Metodo per aggiungere un file ad un documento, per creare un allegato o per aggiungere una nuova versione del file.<br/>
        /// L'oggetto Request va popolato nei seguenti campi:<br/>
        /// IdDocument (String): Obbligatorio. DocNumber del documento. <br/>
        /// File (File): Obbligatorio. File da acquisire. <br/>
        /// CreateAttachment (Boolean): Opzionale. Se a true, indica che deve essere creato il documento come nuovo allegato. <br/>
        /// Description (String): Obbligatorio. Descrizione del file da acquisire. <br/>
        /// CovertToPDFA (Boolean): Opzionale. Se a true converte il file in PDF/A. <br/>
        /// AttachmentType (String): Opzionale. Indica il tipo dell’allegato creato quando la proprietà CreateAttachment è true. Se “E”, l’allegato è di tipo esterno, se assente o “U”, l’allegato è di tipo utente. <br/>
        /// HashFile (String): Opzionale. Hash del file da acquisire. Obbligatorio in presenza della configurazione “HASH_OBBLIGATORIO”.
        /// <br/><br/>
        /// L'oggetto File va popolato nei campi:<br/>
        /// Content (byte[]): Obbligatorio. Contenuto binario del file.
        /// <br/>Name (string): Obbligatorio. Nome del file, con estensione. Il nome deve contenere solo caratteri accettati da file system, e deve avere un'estensione ammessa in amministrazione.</remarks>
        [Route("UploadFileToDocument")]
        [ResponseType(typeof(UploadFileToDocumentResponse))]
        [HttpPut]
        public async Task<HttpResponseMessage> uploadFileToDocument(UploadFileToDocumentRequest request)
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
            UploadFileToDocumentResponse retval = await DocumentsManager.uploadFileToDocument(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il prelievo di un file
        /// </summary>
        /// <param name="IdDocument">Id del documento.</param>
        /// <param name="VersionId">Numero della versione. Inserendo la stringa "SIGNED" permette di ottenere l'ultima versione comprensiva di busta di firma digitale.</param>
        /// <returns>Oggetto file con contenuto binario</returns>
        /// <remarks>MEtodo per il reperimento di un file dato un id di un documento/allegato e opzionalmente il numero di versione.</remarks>
        [Route("GetFileDocumentById")]
        [ResponseType(typeof(GetFileDocumentByIdResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getFileDocumentById(string IdDocument, string VersionId = "")
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
            GetFileDocumentByIdResponse retval = await DocumentsManager.getFileDocumentById(token, IdDocument, VersionId);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento di un file con l’aggiunta della segnatura o del timbro
        /// </summary>
        /// <param name="idDocument">Id del documento</param>
        /// <param name="signature">Segnatura di protocollo del documento</param>
        /// <param name="signOrStamp">Se popolato con STAMP, imprime il timbro sul PDF, altrimenti la segnatura </param>
        /// <returns></returns>
        /// <remarks>Metodo per il reperimento di un file con l’aggiunta della segnatura o del timbro. Etichetta PDF o timbro sopraimpressi. Obbligatorio un valore tra id del documento e segnatura.
        /// Il parametro SignOrStamp va popolato con "STAMP" se si desidera il timbro.</remarks>
        [Route("GetFileWithSignatureOrStamp")]
        [ResponseType(typeof(GetFileDocumentByIdResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getFileWithSignatureOrStamp(string idDocument = "", string signature = "", string signOrStamp = "")
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
            GetFileDocumentByIdResponse retval = await DocumentsManager.getFileWithSignatureOrStamp(token, idDocument, signature, signOrStamp);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la modifica di un documento
        /// </summary>
        /// <param name="request">Oggetto Request</param>
        /// <returns>Dettaglio del documento modificato</returns>
        /// <remarks>Metodo per la modifica di un documento o un protocollo. La richiesta è nella stessa forma di CreateDocument. <br/>
        /// Essendo un metodo di modifica, si consiglia di passare in ingresso un documento prelevato tramite GetDocument</remarks>
        [Route("EditDocument")]
        [ResponseType(typeof(GetDocumentResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> editDocument(CreateDocumentRequest request)
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
            GetDocumentResponse retval = await DocumentsManager.editDocument(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la protocollazione di un documento predisposto
        /// </summary>
        /// <param name="request">Oggetto Request</param>
        /// <returns>Dettaglio del documento protocollato</returns>
        /// <remarks>Metodo per la protocollazione di un documento predisposto senza modificare ulteriori dati. La richiesta richiede l'id del documento predisposto, il registro di protocollazione e l'eventuale RF. <br/>
        /// </remarks>
        [Route("ProtocolPredisposed")]
        [ResponseType(typeof(GetDocumentResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> protocolPredisposed(ProtocolPredisposedRequest request)
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
            GetDocumentResponse retval = await DocumentsManager.protocolPredisposed(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la ricerca dei documenti
        /// </summary>
        /// <param name="request">Oggetto request contenente un array di filtri, il numero della pagina ed il numero di elementi interno ad una pagina</param>
        /// <returns>Lista dei documenti filtrati</returns>
        /// <remarks>Metodo per la ricerca dei documenti</remarks>
        [Route("SearchDocuments")]
        [ResponseType(typeof(SearchDocumentsResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> searchDocuments(SearchRequest request)
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
            SearchDocumentsResponse retval = await DocumentsManager.searchDocuments(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il prelievo dei diritti di accesso ad un documento
        /// </summary>
        /// <param name="IdDocument">Id del documento</param>
        /// <returns>Lista di oggetti ObjectAccessRights, che contengono le informazioni su chi e quali diritti ha sul documento.</returns>
        /// <remarks>Metodo per il prelievo dei diritti di accesso ad un documento.<br/>
        /// L'oggetto ObjectAccessRights ha questi parametri:<br/>
        /// iIdObject (string): Id del documento o fascicolo<br/>
        /// AccessRights (string): Diritti posseduti sul documento. Possibili valori: 63, Scrittura; 45, Lettura; 20, in attesa di accettazione; 0, Utente proprietario; 255, Ruolo proprietario<br/>
        /// AccessRightsType (string): Tipologia dei diritti.<br/>
        /// SubjectDescription (string): Descrizione di chi possiede i diritti (utente o ruolo)<br/>
        /// SubjectCode (string): Codice di chi possiede i diritti (utente o ruolo)<br/>
        /// SubjectType (string): Tipo di chi possiede i diritti (utente o ruolo). "P" per utente, "R" per ruolo.<br/>
        /// SubjectId (string): Id di chi possiede i diritti (utente o ruolo)<br/>
        /// AccessDate (string): Data di acquisizione dei diritti<br/>
        /// Note (string): Note di acquisizione dei diritti. Non sempre presenti<br/></remarks>
        [Route("GetDocAccessRights")]
        [ResponseType(typeof(GetDocAccessRightsResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getDocAccessRights(string IdDocument)
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
            GetDocAccessRightsResponse retval = await DocumentsManager.getDocAccessRights(token, IdDocument);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento degli eventi e/o modifiche riguardanti un documento
        /// </summary>
        /// <param name="IdDocument">Id documento</param>
        /// <param name="AllEvents">Variabile che se impostata a false, restituisce i soli eventi di modifica su di un documento</param>
        /// <returns>Lista degli eventi</returns>
        /// <remarks>Metodo per il reperimento degli eventi e/o modifiche riguardanti un documento. Obbligatorio l'id del documento. <br/>
        /// Il parametro AllEvents da la possibilità di ottenere tutti gli eventi, o di filtrare solo gli eventi che generano modifiche del documento (metadati o file).</remarks>
        [Route("GetDocumentEvents")]
        [ResponseType(typeof(GetDocumentEventsResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getDocumentEvents(string IdDocument, bool AllEvents)
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
            GetDocumentEventsResponse retval = await DocumentsManager.getDocumentEvents(token, IdDocument, AllEvents);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il prelievo degli id dei documenti modificati in un intervallo di date.
        /// </summary>
        /// <param name="dateFrom">Inizio intervallo di date</param>
        /// <param name="dateTo">Fine intervallo di date</param>
        /// <param name="modifiedOnly">Solo documenti modificati nell'intervallo specificato, altrimenti comprende anche quelli creati.</param>
        /// <param name="security">Permette la visualizzazione di tutti i documenti, a prescindere che siano visibili al ruolo. Funzionalità consentita solo previa autorizzazione.</param>
        /// <param name="allEvents">Tutti i documenti che hanno subito qualsiasi evento, altrimenti restituisce i soli documenti che hanno subito modifiche. Di default è "false".</param>
        /// <returns></returns>
        /// <remarks>Metodo per il prelievo degli id dei documenti modificati in un intervallo di date. Le date vanno inserite nel formato dd/mm/yyyy. <br/>
        /// Il parametro modifiedOnly permette di ottenere i soli documenti modificati nell'intervallo di date, non quelli creati.<br/>
        /// Il parametro security permette di ottenere tutti i documenti modificati nell'intervallo di date, a prescindere se siano visibili o meno al ruolo.<br/>
        /// Il parametro allEvents permette di ottenere i documenti che hanno subito qualsiasi evento, oppure solo quelli che hanno subito modifiche ai metadati o file.</remarks>
        [Route("GetModifiedDocuments")]
        [ResponseType(typeof(SearchDocumentsResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getModifiedDocuments(string dateFrom, string dateTo, bool modifiedOnly, bool allEvents, string security = "")
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
            SearchDocumentsResponse retval = await DocumentsManager.getModifiedDocuments(token, dateFrom, dateTo, allEvents, modifiedOnly, security);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il caricamento di un documento pregresso
        /// </summary>
        /// <param name="request">OggettoRequest</param>
        /// <returns>Dettaglio del documento creato.</returns>
        /// <remarks>Metodo che permette l'importazione di un documento pregresso.<br/>
        /// La richiesta consiste nei parametri:<br/>
        /// Document (Document): Obbligatorio. Documento che deve essere creato. Vedere le tabelle successive per le varie tipologie di documento. <br/>
        /// CodeRegister (String): Opzionale. Codice del registro, obbligatorio nel caso di documenti protocollati. <br/>
        /// CodeRF (String): Opzionale. Codice dell’RF per la segnatura. <br/><br/>
        /// L'oggetto document consiste nei seguenti parametri: (esempio creazione Documento non protocollato)
        /// DocumentType (String): Obbligatorio. Indica la tipologia del documento. Inserire il valore “G”. <br/>
        /// Object (String): Obbligatorio. Oggetto del documento. <br/>
        /// Note (Note[]): Opzionale. Possibilità di inserire una o più note nel documento. <br/>
        /// Template (Template): Opzionale. Tipologia del documento che si vuole associare al documento con i relativi valori. <br/>
        /// MainDocument (File): Opzionale. Documento principale.
        /// ProtocolNumber (string): Opzionale. Numero di protocollo del documento pregresso.<br/>
        /// ProtocolYear (string): Opzionale. Anno di protocollo del documento pregresso<br/>
        /// ProtocolDate (string): Opzionale. Data di protocollo del documento pregresso<br/> 
        /// </remarks>
        [Route("ImportPreviousDocument")]
        [ResponseType(typeof(CreateDocumentResponse))]
        [HttpPut]
        public async Task<HttpResponseMessage> importPreviousDocument(CreateDocumentRequest request)
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
            CreateDocumentResponse retval = await DocumentsManager.importPreviousDocument(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento del dettaglio di una tipologia di documento
        /// </summary>
        /// <param name="descriptionTemplate">Descrizione della tipologia</param>
        /// <param name="idTemplate">Id della tipologia</param>
        /// <returns>Dettaglio della tipologia</returns>
        /// <remarks>Servizio per il reperimento del dettaglio di una tipologia di documento dato il nome o l’id. Obbligatorio un valore tra descrizione o id della tipologia.</remarks>
        [Route("GetDocumentTemplate")]
        [ResponseType(typeof(GetTemplateResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getTemplateDoc(string descriptionTemplate = "", string idTemplate = "")
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
            GetTemplateResponse retval = await DocumentsManager.getTemplateDoc(token, descriptionTemplate, idTemplate);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento di tutte le tipologie di documenti.
        /// </summary>
        /// <returns>Lista delle tipologie visibili all'utente</returns>
        /// <remarks>Metodo per il reperimento di tutte le tipologie di documenti visibili al ruolo dell'utente che esegue la richiesta.</remarks>
        [Route("GetDocumentTemplates")]
        [ResponseType(typeof(GetTemplatesResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getDocumentTemplates()
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
            GetTemplatesResponse retval = await DocumentsManager.getDocumentTemplates(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per aggiungere un documento in un fascicolo.
        /// </summary>
        /// <param name="request">Oggetto request:<br/>
        /// IdDocument (string): Obbligatorio. Id del documento.<br/>
        /// IdProject (string): Opzionale. Id del fascicolo<br/>
        /// CodeProject (string): Opzionale. Codice del fascicolo</param>
        /// <returns>Messaggio che conferma l'avvenuta aggiunta</returns>
        /// <remarks>Metodo per l'inserimento di un documento in un fascicolo. E' necessario inserire un solo parametro tra idProject e CodeProject. Per effettuare l’inserimento del documento in un sottofascicolo, bisogna inserire nella richiesta nel parametro in ingresso CodeProject il codice del fascicolo, inserendo quindi i
        /// caratteri separatori “//”, e di seguito la descrizione del sottofascicolo. Un esempio di stringa accettabile può essere “6.2-2014//SottoFasc1”.Per inserire il documento in un sottofascicolo non di primo livello bisogna inserire nel parametro tutto il percorso per arrivare alla cartella desiderata. Esempio “6.2-2014//SottoFasc1//SottoFasc2”.</remarks>
        [Route("AddDocInProject")]
        [ResponseType(typeof(MessageResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> addDocInProject(AddDocInProjectRequest request)
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
            MessageResponse retval = await DocumentsManager.addDocInProject(token, request.IdDocument, request.IdProject, request.CodeProject);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei documenti contenuti in un fascicolo.
        /// </summary>
        /// <param name="request">Oggetto request</param>
        /// <returns>Lista dei documenti presenti in un fascicolo</returns>
        /// <remarks>Metodo per il reperimento dei documenti contenuti in un fascicolo. Obbligatorio l’id del fascicolo oppure la coppia codice e id del titolario.<br/>
        /// L'oggetto request ha i seguenti parametri:<br/>
        /// CodeProject (string): Opzionale. Codice del fascicolo.<br/>
        /// ClassificationSchemeId (string): Opzionale. Id del titolario.<br/>
        /// PageNumber (string): Opzionale. Numero di pagina da visualizzare.<br/>
        /// ElementsInPage (string): Opzionale. Numero di elementi per pagina.<br/>
        /// IdProject (string): Opzionale. Id del fascicolo.</remarks>
        [Route("GetDocumentsInProject")]
        [ResponseType(typeof(SearchDocumentsResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> getDocumentsInProject(GetDocumentsInProjectRequest request)
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
            SearchDocumentsResponse retval = await DocumentsManager.getDocumentsInProject(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per modificare lo stato del diagramma associato ad un documento.
        /// </summary>
        /// <param name="request">Oggetto request: <br/>StateOfDiagram (string): Obbligatorio. Stato in cui avanzare il documento. <br/>
        /// IdDocument (string): Opzionale. Id del documento<br/>
        /// Signature (string): Opzionale. Segnatura del documento</param>
        /// <returns>Messaggio di avvenuta modifica dello stato</returns>
        /// <remarks>Metodo per modificare lo stato del diagramma associato ad un documento. Obbligatorio un valore tra id del documento e segnatura.<br/>
        /// Il parametro stateOfDiagram deve essere popolato con la descrizione di uno stato successivo a quello attuale del documento. In caso contrario viene restituito errore.</remarks>
        [Route("EditDocStateDiagram")]
        [ResponseType(typeof(MessageResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> editDocStateDiagram(EditDocStateDiagramRequest request)
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
            MessageResponse retval = await DocumentsManager.editDocStateDiagram(token, request.IdDocument, request.Signature, request.StateOfDiagram);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il prelievo dello stato del diagramma di un documento
        /// </summary>
        /// <param name="idDocument">Id del documento</param>
        /// <param name="signature">Segnatura del documento</param>
        /// <returns>Dettaglio dello stato del diagramma.</returns>
        /// <remarks>Metodo per il prelievo dello stato del diagramma di un documento. Solo un parametro obbligatorio tra idDocument e signature.<br/>
        /// Se al documento non è associato un diagramma di stato viene restituito errore.</remarks>
        [Route("GetDocStateDiagram")]
        [ResponseType(typeof(GetStateOfDiagramResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getDocumentStateDiagram(string idDocument = "", string signature = "")
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
            GetStateOfDiagramResponse retval = await DocumentsManager.getDocumentStateDiagram(token, idDocument, signature);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la spedizione di un documento.
        /// </summary>
        /// <param name="request">Oggetto request.</param>
        /// <returns>Messaggio di avvenuta spedizione.</returns>
        /// <remarks>Metodo per la spedizione di un documento verso i destinatari esterni del protocollo. Obbligatorio un valore tra id del documento e segnatura. <br/>
        /// Il metodo utilizza la mail di default del registro visibile al ruolo dell'utente.</remarks>
        [Route("SendDocument")]
        [ResponseType(typeof(MessageResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> sendDocument(SendDocRequest request)
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
            MessageResponse retval = await DocumentsManager.sendDocument(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la spedizione avazata di un documento.
        /// </summary>
        /// <param name="request">Oggetto request</param>
        /// <returns>Lista degli esiti della spedizione per ogni corrispondente.</returns>
        /// <remarks>Metodo per la spedizione di un documento permettendo la scelta del registro, della mail dalla quale inviare e dei destinatari.<br/>
        /// Restituisce la lista degli esiti spedizione per ogni destinatario.<br/>
        /// L'oggetto request va popolato con i seguenti parametri: <br/>
        /// IdDocument (string): Opzionale. Id del documento da cancellare. E’ obbligatorio inserire uno tra idDocument e Signature. <br/>
        /// Signature (string): Opzionale. Segnatura del documento. E’ obbligatorio inserire uno tra idDocument e Signature. <br/>
        /// CodeRegister (string): Opzionale. Codice del registro dal quale si spedisce. E’ obbligatorio inserire uno tra CodeRegister e IdRegister. <br/>
        /// IdRegister (string): Opzionale. Id del registro dal quale si spedisce. E’ obbligatorio inserire uno tra CodeRegister e IdRegister. <br/>
        /// SenderMail (string): Opzionale. Mail dalla quale si spedisce: se omessa utilizza la mail default del registro. <br/>
        /// Recipients (Correspondent[]): Opzionale. Array dei corrispondenti da inserire come destinatari della spedizione. Se omesso spedisce a tutti i destinatari.
        /// </remarks>
        [Route("SendDocumentAdvanced")]
        [ResponseType(typeof(SendDocAdvResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> sendDocumentAdvanced(SendDocAdvRequest request)
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
            SendDocAdvResponse retval = await DocumentsManager.sendDocAdvanced(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei dati di segnatura e timbro del documento
        /// </summary>
        /// <param name="idDocument">Id del documento</param>
        /// <param name="signature">Segnatura del documento</param>
        /// <returns>Dettaglio dei dati di segnatura e timbro.</returns>
        /// <remarks>Metodo per il reperimento dei dati di segnatura e timbro del documento. Solo un parametro obbligatorio tra idDocument e signature.<br/>
        /// </remarks>
        [Route("GetStampAndSignature")]
        [ResponseType(typeof(GetStampResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getStampAndSignature(string idDocument = "", string signature = "")
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
            GetStampResponse retval = await DocumentsManager.getStampAndSignature(token, idDocument, signature);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

    }
}