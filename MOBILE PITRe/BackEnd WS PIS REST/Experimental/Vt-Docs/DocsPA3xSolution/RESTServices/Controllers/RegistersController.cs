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
    public class RegistersController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(RegistersController));

        /// <summary>
        /// Servizio per il reperimento del dettaglio di un registro/RF dato il codice del registro/RF o l’id.
        /// </summary>
        /// <param name="codeRegister">Codice del registro</param>
        /// <param name="idRegister">Id del registro</param>
        /// <returns>Dettaglio del registro</returns>
        /// <remarks>Metodo per il prelievo dei dettagli di un registro a partire dal suo codice o dall'id. Almeno uno dei 2 parametri è obbligatorio.</remarks>
        [Route("GetRegisterOrRF")]
        [ResponseType(typeof(GetRegisterOrRFResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getRegisterOrRFResponse(string codeRegister = "", string idRegister = "")
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
            GetRegisterOrRFResponse retval = await RegistersManager.getRegisterOrRF(token, codeRegister, idRegister);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento di tutti gli RF o i registri disponibili per un ruolo
        /// </summary>
        /// <param name="codeRole">Codice del ruolo</param>
        /// <param name="idRole">Id del ruolo</param>
        /// <param name="RegOrRF">Permette di filtrare i risultati. Popolato con REG, restituisce i soli registri; con RF, i soli RF. Se altro valore oppure omesso, restituisce entrambi</param>
        /// <returns>Lista dei registri e/o RF disponibili per un ruolo</returns>
        /// <remarks>Metodo utilizzato per il prelievo del dettaglio dei registri o RF disponibili per un ruolo. Almeno un parametro tra codeRole e idRole è obbligatorio. I risultati possono essere filtrati con il parametro RefOrRF.</remarks>
        [Route("GetRegistersOrRF")]
        [ResponseType(typeof(GetRegistersOrRFResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getRegistersOrRFResponse(string codeRole = "", string idRole = "", string RegOrRF = "")
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
            GetRegistersOrRFResponse retval = await RegistersManager.getRegistersOrRF(token, codeRole, idRole, RegOrRF);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }


    }
}