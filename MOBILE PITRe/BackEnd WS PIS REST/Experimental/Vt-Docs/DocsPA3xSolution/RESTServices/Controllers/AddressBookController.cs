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
    public class AddressBookController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(AddressBookController));

        /// <summary>
        /// Servizio che permette il reperimento del dettaglio di un corrispondente dato l’id.
        /// </summary>
        /// <param name="IdCorrespondent">Id del corrispondente cercato</param>
        /// <returns>Dettaglio del corrispondente</returns>
        /// <remarks>Metodo per il prelievo dei dettagli di un corrispondente a partire dal suo id.</remarks>
        [Route("GetCorrespondent")]
        [ResponseType(typeof(GetCorrespondentResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getCorrespondent(string IdCorrespondent)
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
            GetCorrespondentResponse retval = await AddressBookManager.getCorrespondent(token, IdCorrespondent);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la creazione di un nuovo corrispondente esterno.
        /// </summary>
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
        /// Email (string): Opzionale. Mail del corrispondente.<br/>
        /// AOOCode (string): Opzionale. Codice AOO.<br/>
        /// AdmCode (string): Opzionale. Codice amministrazione.<br/>
        /// Note (string): Opzionale. Note 
        /// Address (string): Opzionale. Indirizzo del corrispondente.<br/>
        /// Cap (string): Opzionale. Cap del corrispondente.<br/>
        /// CodeRegisterOrRF (string): Opzionale. Se il corrispondente deve essere disponibile soltanto per un Registro/RF valorizzare con il codice del Registro/RF.<br/>
        /// PreferredChannel (string): Opzionale. Canale preferenziale del corrispondente.<br/>
        /// </remarks>
        [Route("AddCorrespondent")]
        [ResponseType(typeof(GetCorrespondentResponse))]
        [HttpPut]
        public async Task<HttpResponseMessage> addCorrespondent(AddCorrespondentRequest request)
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
            GetCorrespondentResponse retval = await AddressBookManager.addCorrespondent(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la modifica di un nuovo corrispondente.
        /// </summary>
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
        /// AOOCode (string): Opzionale. Codice AOO.<br/>
        /// AdmCode (string): Opzionale. Codice amministrazione.<br/>
        /// Note (string): Opzionale. Note 
        /// Address (string): Opzionale. Indirizzo del corrispondente.<br/>
        /// Cap (string): Opzionale. Cap del corrispondente.<br/>
        /// CodeRegisterOrRF (string): Opzionale. Se il corrispondente deve essere disponibile soltanto per un Registro/RF valorizzare con il codice del Registro/RF.<br/>
        /// PreferredChannel (string): Opzionale. Canale preferenziale del corrispondente.<br/>
        /// </remarks>
        [Route("EditCorrespondent")]
        [ResponseType(typeof(GetCorrespondentResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> editCorrespondent(AddCorrespondentRequest request)
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
            GetCorrespondentResponse retval = await AddressBookManager.editCorrespondent(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio che restituisce la lista dei filtri applicabili alla ricerca corrispondenti.
        /// </summary>
        /// <returns>Lista dei filtri disponibili per la ricerca corrispondenti.</returns>
        /// <remarks>Metodo che restituisce la lista dei filtri applicabili alla ricerca corrispondenti.</remarks>
        [Route("GetCorrespondentFilters")]
        [ResponseType(typeof(GetFiltersResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getCorrespondentFilters()
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
            GetFiltersResponse retval = await AddressBookManager.getCorrespondentFilters(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio che restituisce la lista dei filtri applicabili alla ricerca utenti.
        /// </summary>
        /// <returns>Lista dei filtri applicabili alla ricerca utenti</returns>
        /// <remarks>Metodo che restituisce la lista dei filtri applicabili alla ricerca utenti.</remarks>
        [Route("GetUserFilters")]
        [ResponseType(typeof(GetFiltersResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getUserFilters()
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
            GetFiltersResponse retval = await AddressBookManager.getUserFilters(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per la ricerca di corrispondenti.
        /// </summary>
        /// <param name="request">Array di filtri di ricerca.</param>
        /// <returns>Lista dei corrispondenti discriminati secondo i filtri inseriti nella richiesta.</returns>
        /// <remarks>Metodo per la ricerca dei corrispondenti, inserendo nella richiesta i filtri disponibili tramite il metodo GetCorrespondentFilters</remarks>
        [Route("SearchCorrespondents")]
        [ResponseType(typeof(SearchCorrespondentsResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> searchCorrespondents(SearchFiltersOnlyRequest request)
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
            SearchCorrespondentsResponse retval = await AddressBookManager.searchCorrespondents(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento di utenti interni all’applicazione.
        /// </summary>
        /// <param name="request">Array di filtri di ricerca.</param>
        /// <returns>Lista degli utenti discriminati secondo i filtri inseriti nella richiesta.</returns>
        /// <remarks>Metodo per la ricerca degli utenti, inserendo nella richiesta i filtri disponibili tramite il metodo GetUserFilters</remarks>
        [Route("SearchUsers")]
        [ResponseType(typeof(GetUsersResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> searchUsers(SearchFiltersOnlyRequest request)
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
            GetUsersResponse retval = await AddressBookManager.searchUsers(token, request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }
    }
}