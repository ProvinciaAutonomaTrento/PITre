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

namespace RESTServices.Controllers
{
    [EnableCors("*", "*", "*")]
    public class RESTRouterController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(RESTRouterController));

        //[Route("TestRouting1")]
        //[ResponseType(typeof(HttpResponseMessage))]        
        //[HttpGet]
        //public async Task<HttpResponseMessage> TestRouting1(
        //HttpRequestMessage request = null)
        //{
        //    logger.Debug("Process request");
        //    // Call the inner handler.
        //    //var response = await base.SendAsync(request, cancellationToken);
        //    //string application_name = Request.Headers.GetValues("APPLICATION_NAME").FirstOrDefault() as String;
        //    //string action = Request.Headers.GetValues("ROUTED_ACTION").FirstOrDefault() as String;
        //    //string method= Request.Headers.GetValues("ROUTED_METHOD").FirstOrDefault() as String;
        //    //string codeAmm = Request.Headers.GetValues("CODE_ADM").FirstOrDefault() as String;
        //    logger.Debug("Process response");

        //    logger.Debug("Per test");
        //    string application_name = "APP1";
        //    string action = "GetTipiOggetto";
        //    string method = "GET";
        //    string codeAmm = "PAT";

        //    string endpoint = "http://localhost/EXP-RESTSvc/";

        //    HttpClient client = new HttpClient();
        //    client.BaseAddress = new System.Uri(endpoint);

        //    endpoint += action;

        //    HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Get, action);
        //    //request2.Content = request.Content;

        //    var httpResp = await client.SendAsync(request2);
        //    logger.Debug(httpResp.Content);
        //    logger.DebugFormat("Response returned with status code of {0}", httpResp.StatusCode);
            

        //    return httpResp;
        //}

        /// <summary>
        /// Servizio di routing
        /// </summary>
        /// <param name="request">Richiesta json formattata secondo il metodo desiderato</param>
        /// <returns>Risposta desiderata a seconda del metodo invocato</returns>
        /// <remarks>Metodo di routing. 
        /// Sono necessari gli header:<br/>
        /// ROUTED_ACTION: nome del metodo che si vuole invocare<br/>
        /// CODE_ADM: codice dell'amministrazione<br/>
        /// L'header APPLICATION_NAME viene automaticamente prelevato dal proxy e viene valorizzato con il CN del certificato con il quale è configurato il client<br/>
        /// L'header AuthToken è obbligatorio per tutti i metodi, tranne per quello di prelievo del token<br/>
        /// La richiesta da inserire è formattata secondo quella richiesta dal metodo inserito in ROUTED_ACTION<br/>
        /// La risposta sarà quella del metodo inserito in ROUTED_ACTION
        /// </remarks>
        [Route("RouteRestRequest")]
        [ResponseType(typeof(HttpResponseMessage))]
        [HttpGet]
        public async Task<HttpResponseMessage> RouteRestRequestGet(
        HttpRequestMessage request = null)
        {
            try
            {
                logger.Info("Start RouteRestRequestGet");
                // Call the inner handler.
                //var response = await base.SendAsync(request, cancellationToken);
                string application_name = "", action = "", codeAmm = "", token="";
                if (Request.Headers != null)
                {
                    if (Request.Headers.Contains("APPLICATION_NAME"))
                    {
                        application_name = Request.Headers.GetValues("APPLICATION_NAME").FirstOrDefault() as String;
                        logger.Debug("Application_name : " + application_name);
                    }
                    if (Request.Headers.Contains("ROUTED_ACTION"))
                    {
                        action = Request.Headers.GetValues("ROUTED_ACTION").FirstOrDefault() as String;
                        logger.DebugFormat("Routed_action: {0}", action);
                    }
                    if (Request.Headers.Contains("CODE_ADM"))
                    {
                        codeAmm = Request.Headers.GetValues("CODE_ADM").FirstOrDefault() as String;
                        logger.DebugFormat("Code Amm: {0}", codeAmm);
                    }
                    if (Request.Headers.Contains("AuthToken"))
                    {
                        token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
                        logger.DebugFormat("token: {0}", token);
                    }
                }
                logger.Debug("Process response");
                if (string.IsNullOrWhiteSpace(application_name))
                {
                   logger.Error("Missing APPLICATION_NAME Header");
                    HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    errorMessage.ReasonPhrase = "Missing APPLICATION_NAME Header";
                    errorMessage.Content = new StringContent("Missing APPLICATION_NAME Header");
                    return errorMessage;
                }
                if (string.IsNullOrWhiteSpace(codeAmm))
                {
                    logger.Error("Missing CODE_ADM Header");
                    HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    errorMessage.ReasonPhrase = "Missing CODE_ADM Header";
                    errorMessage.Content = new StringContent("Missing CODE_ADM Header");
                    return errorMessage;
                }
                if (string.IsNullOrWhiteSpace(action))
                {
                    logger.Error("Missing ROUTED_ACTION Header");
                    HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    errorMessage.ReasonPhrase = "Missing ROUTED_ACTION Header";
                    errorMessage.Content = new StringContent("Missing ROUTED_ACTION Header");
                    return errorMessage;
                }
                logger.InfoFormat("Richiesta di {0} su amministrazione {1}, eseguita da integrazione {2}", action, codeAmm, application_name);

                string endpoint = "http://localhost/EXP-RESTSvc/";

                endpoint = RoutingManager.GetEndpoint(codeAmm, application_name);
                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    logger.Error("Header APPLICATION_NAME inserito non valido per il codice amministrazione inserito");
                    throw new Exception("Header APPLICATION_NAME inserito non valido per il codice amministrazione inserito");
                }
                logger.DebugFormat("Richiesta di {0} su amministrazione {1}, eseguita da integrazione {2} su endpoint {3} ", action, codeAmm, application_name, endpoint);

                HttpClient client = new HttpClient();
                client.BaseAddress = new System.Uri(endpoint);

                endpoint += action;
                try
                {
                    int parGetX = 0;
                    foreach (var p1 in request.GetQueryNameValuePairs())
                    {
                        if (parGetX == 0) action += "?";
                        else action += "&";
                        action += string.Format("{0}={1}", p1.Key, p1.Value);
                        parGetX++;
                    }
                    logger.Debug(action);
                }
                catch (Exception ex1)
                {
                    logger.Error(ex1);
                }
                HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Get, action);
                //request2.Content = request.Content;
                
                request2.Headers.Add("Accept", request.Headers.Accept.ToString());
                if (!string.IsNullOrWhiteSpace(token))
                    request2.Headers.Add("AuthToken", token);
                logger.Debug("Prima della chiamata async");
                var httpResp = await client.SendAsync(request2);
                logger.DebugFormat("Response returned with status code of {0}", httpResp.StatusCode);
                logger.Info("End RouteRestRequestGet");
                return httpResp;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                HttpResponseMessage errore = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                errore.ReasonPhrase = ex.Message;
                errore.Content = new StringContent(ex.Message);
                return errore;
            }
            
        }

        /// <summary>
        /// Servizio di routing
        /// </summary>
        /// <param name="request">Richiesta json formattata secondo il metodo desiderato</param>
        /// <returns>Risposta desiderata a seconda del metodo invocato</returns>
        /// <remarks>Metodo di routing. 
        /// Sono necessari gli header:<br/>
        /// ROUTED_ACTION: nome del metodo che si vuole invocare<br/>
        /// CODE_ADM: codice dell'amministrazione<br/>
        /// L'header APPLICATION_NAME viene automaticamente prelevato dal proxy e viene valorizzato con il CN del certificato con il quale è configurato il client<br/>
        /// L'header AuthToken è obbligatorio per tutti i metodi, tranne per quello di prelievo del token<br/>
        /// La richiesta da inserire è formattata secondo quella richiesta dal metodo inserito in ROUTED_ACTION<br/>
        /// La risposta sarà quella del metodo inserito in ROUTED_ACTION
        /// </remarks>
        [Route("RouteRestRequest")]
        [ResponseType(typeof(HttpResponseMessage))]
        [HttpPut]
        public async Task<HttpResponseMessage> RouteRestRequestPut(
        HttpRequestMessage request)
        {
            logger.Info("Start RouteRestRequestPut");
                
            logger.Debug("Process request");
            // Call the inner handler.
            //var response = await base.SendAsync(request, cancellationToken);
            string application_name = "", action = "", codeAmm = "", token = "";
            if (Request.Headers != null)
            {
                if (Request.Headers.Contains("APPLICATION_NAME"))
                {
                    application_name = Request.Headers.GetValues("APPLICATION_NAME").FirstOrDefault() as String;
                    logger.Debug("Application_name : " + application_name);
                }
                if (Request.Headers.Contains("ROUTED_ACTION"))
                {
                    action = Request.Headers.GetValues("ROUTED_ACTION").FirstOrDefault() as String;
                    logger.DebugFormat("Routed_action: {0}", action);
                }
                if (Request.Headers.Contains("CODE_ADM"))
                {
                    codeAmm = Request.Headers.GetValues("CODE_ADM").FirstOrDefault() as String;
                    logger.DebugFormat("Code Amm: {0}", codeAmm);
                }
                if (Request.Headers.Contains("AuthToken"))
                {
                    token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
                    logger.DebugFormat("token: {0}", token);
                }
            }
            logger.Debug("Process response");
            if (string.IsNullOrWhiteSpace(application_name))
            {
                logger.Error("Missing APPLICATION_NAME Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing APPLICATION_NAME Header";
                errorMessage.Content = new StringContent("Missing APPLICATION_NAME Header");
                return errorMessage;
            }
            if (string.IsNullOrWhiteSpace(codeAmm))
            {
                logger.Error("Missing CODE_ADM Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing CODE_ADM Header";
                errorMessage.Content = new StringContent("Missing CODE_ADM Header");
                return errorMessage;
            }
            if (string.IsNullOrWhiteSpace(action))
            {
                logger.Error("Missing ROUTED_ACTION Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing ROUTED_ACTION Header";
                errorMessage.Content = new StringContent("Missing ROUTED_ACTION Header");
                return errorMessage;
            }
            logger.InfoFormat("Richiesta di {0} su amministrazione {1}, eseguita da integrazione {2}", action, codeAmm, application_name);

            string endpoint = "http://localhost/EXP-RESTSvc/";

            endpoint = RoutingManager.GetEndpoint(codeAmm, application_name);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                logger.Error("Header APPLICATION_NAME inserito non valido per il codice amministrazione inserito");
                throw new Exception("Header APPLICATION_NAME inserito non valido per il codice amministrazione inserito");
            }
            logger.DebugFormat("Richiesta di {0} su amministrazione {1}, eseguita da integrazione {2} su endpoint {3} ", action, codeAmm, application_name, endpoint);

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri(endpoint);

            endpoint += action;

            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Put, action);
            request2.Content = request.Content;
            request2.Headers.Add("Accept", request.Headers.Accept.ToString());
            if (!string.IsNullOrWhiteSpace(token))
                request2.Headers.Add("AuthToken", token);
                
            var httpResp = await client.SendAsync(request2);
            logger.DebugFormat("Response returned with status code of {0}", httpResp.StatusCode);
            logger.Info("End RouteRestRequestPut");
            return httpResp;
        }

        /// <summary>
        /// Servizio di routing
        /// </summary>
        /// <param name="request">Richiesta json formattata secondo il metodo desiderato</param>
        /// <returns>Risposta desiderata a seconda del metodo invocato</returns>
        /// <remarks>Metodo di routing. 
        /// Sono necessari gli header:<br/>
        /// ROUTED_ACTION: nome del metodo che si vuole invocare<br/>
        /// CODE_ADM: codice dell'amministrazione<br/>
        /// L'header APPLICATION_NAME viene automaticamente prelevato dal proxy e viene valorizzato con il CN del certificato con il quale è configurato il client<br/>
        /// L'header AuthToken è obbligatorio per tutti i metodi, tranne per quello di prelievo del token<br/>
        /// La richiesta da inserire è formattata secondo quella richiesta dal metodo inserito in ROUTED_ACTION<br/>
        /// La risposta sarà quella del metodo inserito in ROUTED_ACTION
        /// </remarks>
        [Route("RouteRestRequest")]
        [ResponseType(typeof(HttpResponseMessage))]
        [HttpPost]
        public async Task<HttpResponseMessage> RouteRestRequestPost(
        HttpRequestMessage request)
        {
            logger.Info("Start RouteRestRequestPost");
            logger.Debug("Process request");
            // Call the inner handler.
            //var response = await base.SendAsync(request, cancellationToken);
            string application_name = "", action = "", codeAmm = "", token = "";
            if (Request.Headers != null)
            {
                if (Request.Headers.Contains("APPLICATION_NAME"))
                {
                    application_name = Request.Headers.GetValues("APPLICATION_NAME").FirstOrDefault() as String;
                    logger.Debug("Application_name : " + application_name);
                }
                if (Request.Headers.Contains("ROUTED_ACTION"))
                {
                    action = Request.Headers.GetValues("ROUTED_ACTION").FirstOrDefault() as String;
                    logger.DebugFormat("Routed_action: {0}", action);
                }
                if (Request.Headers.Contains("CODE_ADM"))
                {
                    codeAmm = Request.Headers.GetValues("CODE_ADM").FirstOrDefault() as String;
                    logger.DebugFormat("Code Amm: {0}", codeAmm);
                }
                if (Request.Headers.Contains("AuthToken"))
                {
                    token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
                    logger.DebugFormat("token: {0}", token);
                }
            }
            logger.Debug("Process response");
            if (string.IsNullOrWhiteSpace(application_name))
            {
                logger.Error("Missing APPLICATION_NAME Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing APPLICATION_NAME Header";
                errorMessage.Content = new StringContent("Missing APPLICATION_NAME Header");
                return errorMessage;
            }
            if (string.IsNullOrWhiteSpace(codeAmm))
            {
                logger.Error("Missing CODE_ADM Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing CODE_ADM Header";
                errorMessage.Content = new StringContent("Missing CODE_ADM Header");
                return errorMessage;
            }
            if (string.IsNullOrWhiteSpace(action))
            {
                logger.Error("Missing ROUTED_ACTION Header");
                HttpResponseMessage errorMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                errorMessage.ReasonPhrase = "Missing ROUTED_ACTION Header";
                errorMessage.Content = new StringContent("Missing ROUTED_ACTION Header");
                return errorMessage;
            }
            logger.InfoFormat("Richiesta di {0} su amministrazione {1}, eseguita da integrazione {2}", action, codeAmm, application_name);

            string endpoint = "http://localhost/EXP-RESTSvc/";

            endpoint = RoutingManager.GetEndpoint(codeAmm, application_name);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                logger.Error("Header APPLICATION_NAME inserito non valido per il codice amministrazione inserito");
                throw new Exception("Header APPLICATION_NAME inserito non valido per il codice amministrazione inserito");
            }
            logger.DebugFormat("Richiesta di {0} su amministrazione {1}, eseguita da integrazione {2} su endpoint {3} ", action, codeAmm, application_name, endpoint);

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri(endpoint);
            
            endpoint += action;

            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Post, action);
            //request2.Content = request.Content;
            request2.Content = request.Content;
            request2.Headers.Add("Accept", request.Headers.Accept.ToString());
            if (!string.IsNullOrWhiteSpace(token))
                request2.Headers.Add("AuthToken", token);
                
            var httpResp = await client.SendAsync(request2);
            logger.DebugFormat("Response returned with status code of {0}", httpResp.StatusCode);
            logger.Info("End RouteRestRequestPost");
            return httpResp;
        }

    }
}