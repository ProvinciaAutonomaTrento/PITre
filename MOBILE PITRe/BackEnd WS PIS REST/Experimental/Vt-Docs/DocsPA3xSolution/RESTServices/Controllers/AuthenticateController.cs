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
using RESTServices.Manager;
using log4net;

namespace RESTServices.Controllers
{
    [EnableCors("*", "*", "*")]
    public class AuthenticateController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(AuthenticateController));

        /// <summary>
        /// Servizio per il prelievo del token di autenticazione
        /// </summary>
        /// <param name="request">Oggetto AuthenticateRequest, contenente i seguenti parametri <br/>
        /// Username: Username dell'utente. Obbligatorio<br/>
        /// CodeAdm: Codice dell'amministrazione nella quale ci si vuole autenticare. Obbligatorio<br/>
        /// CodeRole: Ruolo con il quale ci si vuole autenticare. Se omesso, viene selezionato il ruolo preferito dell'utente. Opzionale.
        /// CodeApplication: Identifica l'integrazione che si interfaccia tramite REST </param>
        /// <returns>Token di autenticazione</returns>
        /// <remarks>Metodo per il prelievo del token di autenticazione. Fornendo lo username ed il codice amministrazione, viene restituito un token di autenticazione da inserire nelle successive chiamate, in un header "AuthToken"</remarks>
        [Route("GetToken")]
        [ResponseType(typeof(RESTServices.Model.Responses.AuthenticateResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> Authenticate(RESTServices.Model.Requests.AuthenticateRequest request)
        {
            HttpResponseMessage response = null;
            RESTServices.Model.Responses.AuthenticateResponse retval = await RESTServices.Manager.AuthenticateManager.GetAuthenticationToken(request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);

            return response;
        }

        /// <summary>
        /// Servizio di test
        /// </summary>
        /// <returns>Lista dei tipi oggetto</returns>
        /// <remarks>Metodo di test per verificare la connessione al DB</remarks>
        [Route("GetTipiOggetto")]
        [ResponseType(typeof(IList<string>))]
        [HttpGet]
        public async Task<HttpResponseMessage> GetTipiOggetto()
        {
            HttpResponseMessage response = null;
            IList<string> tipiOgg = null;
            logger.Debug(Request.Method.ToString());

            //List<string> tipiOg = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTipiOggetto().Cast<string>().ToList<string>();
            tipiOgg = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTipiOggetto().Cast<string>().ToList<string>();

            response = Request.CreateResponse(HttpStatusCode.OK, tipiOgg);
            return response;
        }
    }
}