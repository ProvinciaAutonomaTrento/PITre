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
    public class ClassificationSchemesController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(ClassificationSchemesController));

        /// <summary>
        /// Servizio per il reperimento del titolario attivo.
        /// </summary>
        /// <returns>Il dettaglio del titolario attivo</returns>
        /// <remarks>Metodo per il prelievo del titolario attivo, all'interno del quale possono essere creati i nuovi fascicoli</remarks>
        [Route("GetActiveClassificationScheme")]
        [ResponseType(typeof(GetClassificationSchemeResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getActiveClassificationScheme()
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
            GetClassificationSchemeResponse retval = await ClassificationSchemesManager.getActiveClassificationScheme(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento di tutti i titolari.
        /// </summary>
        /// <returns>Lista dei titolari.</returns>
        /// <remarks>Metodo per il prelievo del dettaglio di tutti i titolari</remarks>
        [Route("GetAllClassificationSchemes")]
        [ResponseType(typeof(GetAllClassificationSchemesResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getAllClassificationSchemes()
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
            GetAllClassificationSchemesResponse retval = await ClassificationSchemesManager.getAllClassificationSchemes(token);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei dettagli di un titolario dato l’id.
        /// </summary>
        /// <param name="idClassificationScheme">Id del titolario cercato</param>
        /// <returns>Dettaglio del titolario</returns>
        /// <remarks>Metodo per il reperimento del dettaglio di un titolario dato l'id dello stesso</remarks>
        [Route("GetClassificationSchemeById")]
        [ResponseType(typeof(GetClassificationSchemeResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getClassificationSchemeById(string idClassificationScheme)
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
            GetClassificationSchemeResponse retval = await ClassificationSchemesManager.getClassificationSchemeById(token, idClassificationScheme);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }
    }
}