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
    public class RolesController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(RolesController));

        /// <summary>
        /// Servizio per il reperimento dei dettagli di un ruolo dato il codice del ruolo o l’id.
        /// </summary>
        /// <param name="codeRole">Codice del ruolo</param>
        /// <param name="idRole">Id del Ruolo</param>
        /// <returns>Dettaglio del ruolo richiesto</returns>
        /// <remarks>Metodo per il prelievo dei dettagli di un ruolo dato il codice del ruolo o l’id. Almeno uno dei due parametri è obbligatorio. Restituisce il dettaglio del ruolo.
        /// </remarks>
        [Route("GetRole")]
        [ResponseType(typeof(GetRoleResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getRole(string codeRole = "", string idRole = "")
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
            GetRoleResponse retval = await RolesManager.getRole(token, codeRole, idRole);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento di tutti i ruoli disponibili ad un utente.
        /// </summary>
        /// <param name="userId">Username dell'utente di cui si vogliono conoscere i ruoli</param>
        /// <returns>Lista dei ruoli assegnati all'utente</returns>
        /// <remarks>Metodo per il reperimento di tutti i ruoli disponibili per un utente. Restituisce la lista dei ruoli dell'utente</remarks>
        [Route("GetRoles")]
        [ResponseType(typeof(GetRolesResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getRoles(string userId)
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
            GetRolesResponse retval = await RolesManager.getRoles(token, userId);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il prelievo dei ruoli dato un utente ed una funzione richiesta.
        /// </summary>
        /// <param name="userId">Username dell'utente di cui si vogliono conoscere i ruoli</param>
        /// <param name="codeFunction">Codice della funzione secondo la quale filtrare i ruoli</param>
        /// <returns>Lista dei ruoli di un utente aventi una determinata funzione </returns>
        /// <remarks>Metodo per il prelievo dei ruoli di un utente che hanno una determinata funzione. Restituisce la lista dei ruoli.</remarks>
        [Route("GetRolesForEnabledActions")]
        [ResponseType(typeof(GetRolesResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getRolesForEnabledActions(string userId, string codeFunction)
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
            GetRolesResponse retval = await RolesManager.getRolesForEnabledActions(token, userId, codeFunction);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento degli utenti in un ruolo.
        /// </summary>
        /// <param name="codeRole">Codice del ruolo</param>
        /// <returns>Lista degli utenti presenti nel ruolo</returns>
        /// <remarks>Metodo per il prelievo degli utenti presenti in un determinato ruolo, a partire dal codice del ruolo. Restituisce la lista degli utenti.</remarks>
        [Route("GetUsersInRole")]
        [ResponseType(typeof(GetUsersResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getUsersInRole(string codeRole)
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
            GetUsersResponse retval = await RolesManager.getUsersInRole(token, codeRole);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

    }
}