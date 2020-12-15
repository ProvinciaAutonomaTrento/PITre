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
using log4net;

namespace DocsPaRESTWS.Controllers
{
    [EnableCors("*", "*", "*")]
    public class MobileController : ApiController
    {
        private static ILog logger = LogManager.GetLogger(typeof(MobileController));

        /// <summary>
        /// Check Connection
        /// </summary>
        /// <returns>Connection status message</returns>
        /// <remarks>Verifica la connessione</remarks>
        [Route("CheckConnection")]
        [ResponseType(typeof(string))]
        [HttpGet]
        //[DocsPaActionFilter]
        public async Task<HttpResponseMessage> CheckConn()
        {
            HttpResponseMessage response = null;
            
            bool testX = BusinessLogic.Amministrazione.UtenteManager.Checkconnection();
            string retVal="";
            if (testX) retVal= "OK";
            else retVal= "ERROR";

            response = Request.CreateResponse<string>(HttpStatusCode.OK, retVal);
            return response;
        }

        /// <summary>
        /// Get tipi oggetto
        /// </summary>
        /// <returns>Lista dei tipi oggetto</returns>
        /// <remarks>Controllo per la connessione con il database</remarks>
        [Route("GetTipiOggetto")]
        [ResponseType(typeof(IList<string>))]
        [HttpGet]
        public async Task<HttpResponseMessage> GetTipiOggetto()
        {
            HttpResponseMessage response = null;
            IList<string> tipiOgg = null;
            //List<string> tipiOg = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTipiOggetto().Cast<string>().ToList<string>();
            tipiOgg= BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTipiOggetto().Cast<string>().ToList<string>();

            response = Request.CreateResponse(HttpStatusCode.OK, tipiOgg);
            return response;
        }

        /// <summary>
        /// Lista Istanze
        /// </summary>
        /// <returns>Lista delle istanze del PITRE</returns>
        /// <remarks>Lista delle istanze e degli endpoint da puntare a seguito della selezione da parte dell'utente</remarks>
        [Route("ListaIstanze")]
        [ResponseType(typeof(IList<DocsPaVO.Mobile.Istanza>))]
        [HttpGet]
        public async Task<HttpResponseMessage> ListaIstanze()
        {
            HttpResponseMessage response = null;
            IList<DocsPaVO.Mobile.Istanza> retval = null;
            //List<string> tipiOg = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTipiOggetto().Cast<string>().ToList<string>();
            //tipiOgg = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTipiOggetto().Cast<string>().ToList<string>();
            DocsPaVO.Mobile.Istanza tempX = new DocsPaVO.Mobile.Istanza();
            System.Collections.ArrayList istanze = new System.Collections.ArrayList();
            try
            {
                istanze = await MobileManager.ListaIstanze();
                logger.Debug(istanze == null ? "istanze null" : "Istanze not null " + istanze.Count);
            }
            catch (Exception extask)
            {
                logger.Error(extask);
                istanze = null;
            }
            if (istanze == null)
            {
                istanze = new System.Collections.ArrayList();
                // Sviluppo e Collaudo
                tempX = new DocsPaVO.Mobile.Istanza("Sviluppo", "Ambiente di sviluppo", "Sviluppo", "http://10.155.9.245/PITRERestWS/");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("Sviluppo", "Ambiente di sviluppo - visibile su internet", "Sviluppo", "http://2.113.15.90/PITRERestWS/");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("PAT_TEST - Collaudo", "Provincia Autonoma di Trento - Ambiente di collaudo", "Collaudo", "https://mobile-test.pitre.tn.it/PAT-be/");
                istanze.Add(tempX);
                // Produzione
                tempX = new DocsPaVO.Mobile.Istanza("APSS", "Azienda Provinciale per i Servizi Sanitari di Trento", "Produzione", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("COMPRENSORI", "Istanza delle comunita'", "Produzione", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("COMTN", "Comune di Trento", "Produzione", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("COMUNI", "Istanza dei comuni", "Produzione", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("CONS_COMUNI", "Consorzio dei Comuni Trentini - Consiglio delle Autonomie Locali", "Produzione", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("ENTI", "Istanza degli enti", "Produzione", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("INFOTN", "Informatica Trentina spa", "Produzione", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("PAT", "Provincia Autonoma di Trento", "Produzione", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("REGIONE", "Regione Autonoma Trentino Alto Adige/Sudtirol", "Produzione", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("SCUOLE", "Istanza delle scuole", "Produzione", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("UNITN", "Universita' degli Studi di Trento", "Produzione", "Da definire");
                istanze.Add(tempX);

                // Test
                tempX = new DocsPaVO.Mobile.Istanza("APSS_Test", "Azienda Provinciale per i Servizi Sanitari di Trento - TEST", "Test", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("COMPRENSORI_Test", "Istanza di comunità - TEST", "Test", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("COMTN_Test", "Comune di Trento - TEST", "Test", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("COMUNI_Test", "Istanza dei comuni - TEST", "Test", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("CONS_COMUNI_Test", "Consorzio dei Comuni Trentini - Consiglio delle Autonomie Locali - TEST", "Test", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("ENTI_Test", "Istanza degli enti - TEST", "Test", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("INFOTN_Test", "Informatica Trentina spa - TEST", "Test", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("PAT_Test", "Provincia Autonoma di Trento - TEST", "Test", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("REGIONE_Test", "Regione Autonoma Trentino Alto Adige/Sudtirol - TEST", "Test", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("SCUOLE_Test", "Istanza delle scuole - TEST", "Test", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("UNITN_Test", "Universita' degli Studi di Trento - TEST", "Test", "Da definire");
                istanze.Add(tempX);

                // Quality
                tempX = new DocsPaVO.Mobile.Istanza("APSS_Quality", "Azienda Provinciale per i Servizi Sanitari di Trento - QUALITY", "Quality", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("COMPRENSORI_Quality", "Istanza di comunità - QUALITY", "Quality", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("COMTN_Quality", "Comune di Trento - QUALITY", "Quality", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("COMUNI_Quality", "Istanza dei comuni - QUALITY", "Quality", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("CONS_COMUNI_Quality", "Consorzio dei Comuni Trentini - Consiglio delle Autonomie Locali - QUALITY", "Quality", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("ENTI_Quality", "Istanza degli enti - QUALITY", "Quality", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("INFOTN_Quality", "Informatica Trentina spa - QUALITY", "Quality", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("PAT_Quality", "Provincia Autonoma di Trento - QUALITY", "Quality", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("REGIONE_Quality", "Regione Autonoma Trentino Alto Adige/Sudtirol - QUALITY", "Quality", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("SCUOLE_Quality", "Istanza delle scuole - QUALITY", "Quality", "Da definire");
                istanze.Add(tempX);
                tempX = new DocsPaVO.Mobile.Istanza("UNITN_Quality", "Universita' degli Studi di Trento - QUALITY", "Quality", "Da definire");
                istanze.Add(tempX);
            }

            retval = istanze.Cast<DocsPaVO.Mobile.Istanza>().ToList<DocsPaVO.Mobile.Istanza>();
            
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="request">Informazioni di autenticazione. Obbligatori Username e password. Id Amministrazione in caso di multiamministrazione. OldPassword se necessario cambio password.</param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - User o password errati
        /// 2 - Password scaduta
        /// 3 - Utente multiamministrazione
        /// 4 - Errore generico
        /// In caso di codice 0, vengono restituite le informazioni sull'utente, se è abilitato all'OTP, e il memento del suo certificato.
        /// </returns>
        /// <remarks>Login all'applicazione</remarks>
        [Route("Login")]
        [ResponseType(typeof(LoginResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> Authenticate(AuthenticateRequest request)
        {
            HttpResponseMessage response = null;
            LoginResponse retval;

            if (!string.IsNullOrWhiteSpace(request.Otp))
                retval = await MobileManager.resetPassword(request);
            else

             if (string.IsNullOrWhiteSpace(request.OldPassword))
                retval = await MobileManager.login(request);
            else
                retval = await MobileManager.cambioPassword(request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            
            return response;
        }

        [Route("RecuperaPasswordOTP")]
        [ResponseType(typeof(LoginResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> RecuperaPasswordOTP(AuthenticateRequest request)
        {
            HttpResponseMessage response = null;
            LoginResponse retval;
            
                retval = await MobileManager.resetPasswordInviaOTP(request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);

            return response;
        }

        [Route("Update")]
        [ResponseType(typeof(VerifyUpdateResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> VerifyUpdate(VerifyUpdateRequest request)
        {
            HttpResponseMessage response = null;
            VerifyUpdateResponse retval;

            retval = await MobileManager.verifyUpdate(request);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);

            return response;
        }



        ///// <summary>
        ///// Cambio password
        ///// </summary>
        ///// <param name="request">I parametri di autenticazione, contenenti la nuova password</param>
        ///// <param name="oldPassword">la vecchia password</param>
        ///// <returns>Info su utente e ruoli</returns>
        ///// <remarks>Metodo utilizzato per il cambio della password nel caso essa sia scaduta</remarks>
        //[Route("CambioPassword")]
        //[ResponseType(typeof(LoginResponse))]
        //[HttpPost]
        //public async Task<HttpResponseMessage> cambioPassword(AuthenticateRequest request)
        //{
        //    HttpResponseMessage response = null;
        //    LoginResponse retval = await MobileManager.cambioPassword(request);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);

        //    return response;
        //}

        /// <summary>
        /// Lista amministrazioni per utente
        /// </summary>
        /// <param name="userid">Utente per il quale prelevare le amministrazioni</param>
        /// <returns>Lista delle amministrazioni sulle quali l'utente è abilitato</returns>
        /// <remarks>Metodo per il prelievo delle amministrazioni sulle quali l'utente è abilitato.</remarks>
        [Route("ListaAmmByUser")]
        [ResponseType(typeof(DocsPaVO.utente.Amministrazione[]))]
        [HttpGet]
        public async Task<HttpResponseMessage> getListaAmmByUser(string userid)
        {
            HttpResponseMessage response = null;
            DocsPaVO.utente.Amministrazione[] retval = await MobileManager.getListaAmmByUser(userid);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);

            return response;
        }

        /// <summary>
        /// LogOut
        /// </summary>
        /// <param name="dst">parametro necessario al logout dal documentale</param>
        /// <returns>Messaggio di status</returns>
        /// <remarks>LogOut dall'applicazione</remarks>
        [Route("LogOut")]
        [ResponseType(typeof(LogoutResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> LogOut(string dst)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            LogoutResponse retval = await MobileManager.logout(token, dst);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);            
            return response;
        }

        /// <summary>
        /// Get Todo List
        /// </summary>
        /// <param name="pagesize">Numero di elementi restituiti per pagina</param>
        /// <param name="requestedpage">Pagina restituita</param>
        /// <returns>Lista degli elementi del centro nofiche</returns>
        /// <remarks>Restituisce gli elementi del centro notifiche per l'utente e il ruolo con il quale si è autenticati.</remarks>
        [Route("TodoList")]
        [ResponseType(typeof(ToDoListResponse))]
        [HttpGet]
        public async Task<HttpResponseMessage> getTodoList( int pagesize, int requestedpage)
        {
            /*TodoListRequest request*/
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            //logger.Debug("getTodolist controller - Start");
            //logger.Debug("request null ? " + request == null ? "si" : "no");
            //logger.Debug("token" + token);
            //logger.DebugFormat("Request valori {0} {1}", request.PageSize, request.RequestedPage);
            //ToDoListResponse retval = await MobileManager.getTodoList(token, request.RequestedPage, request.PageSize);
            ToDoListResponse retval = await MobileManager.getTodoList(token, requestedpage, pagesize);

            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        /// <summary>
        /// Cambio Ruolo
        /// </summary>
        /// <param name="idRuolo">Id del ruolo del quale si vuole il token</param>
        /// <returns>
        /// Codice:
        /// 0 - OK
        /// 1 - User o password errati
        /// 2 - Password scaduta
        /// 3 - Utente multiamministrazione
        /// 4 - Errore generico
        /// In caso di codice 0, vengono restituite le informazioni sull'utente, se è abilitato all'OTP, e il memento del suo certificato.
        /// </returns>
        /// <remarks>Consente di ottenere il token per un ruolo differente riguardo un utente</remarks>
        [Route("CambioRuolo")]
        [ResponseType(typeof(LoginResponse))]
        [HttpPost]
        public async Task<HttpResponseMessage> cambiaruolo(string idRuolo)
        {
            HttpResponseMessage response = null;
            string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
            LoginResponse retval = await MobileManager.cambiaRuolo(token, idRuolo);
            response = Request.CreateResponse(HttpStatusCode.OK, retval);
            return response;
        }

        #region DocFascCommentato
        ///// <summary>
        ///// Prelievo informazioni documento
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[Route("DocInfo")]
        //[ResponseType(typeof(GetDocInfoResponse))]
        //[HttpGet]
        //public async Task<HttpResponseMessage> getDocInfo(GetDocInfoRequest request)
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    GetDocInfoResponse retval = await MobileManager.getDocInfo(token, request);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}

        //[Route("RimuoviNotifica")]
        //[ResponseType(typeof(bool))]
        //[HttpPost]
        //public async Task<HttpResponseMessage> rimuoviNotifica(string idEvento)
        //{
        //    HttpResponseMessage response = null;
        //    bool retval = await MobileManager.rimuoviNotifica(idEvento);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}

        //[Route("File")]
        //[ResponseType(typeof(GetFileResponse))]
        //[HttpGet]
        //public async Task<HttpResponseMessage> getFile(string idDoc)
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    GetFileResponse retval = await MobileManager.getFile(token, idDoc);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}

        //[Route("AdlAction")]
        //[ResponseType(typeof(ADLActionResponse))]
        //[HttpPost]
        //public async Task<HttpResponseMessage> AdlAction(AdlActionRequest request)
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    ADLActionResponse retval = await MobileManager.ADLAction(token, request);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}

        //[Route("FascInfo")]
        //[ResponseType(typeof(GetFascInfoResponse))]
        //[HttpGet]
        //public async Task<HttpResponseMessage> getFascInfo(GetFascInfoRequest request)
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    GetFascInfoResponse retval = await MobileManager.getFascInfo(token, request);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}
        #endregion
        #region Ricerca commentato
        //[Route("RicercheSalvate")]
        //[ResponseType(typeof(GetRicSalvateResponse))]
        //[HttpGet]
        //public async Task<HttpResponseMessage> getRicSalvate()
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    GetRicSalvateResponse retval = await MobileManager.getRicSalvate(token);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}
        #endregion
        #region deleghe commentato
        //[Route("Delega")]
        //[ResponseType(typeof(CreaDelegaResponse))]
        //[HttpPost]
        //public async Task<HttpResponseMessage> creaDelega(DocsPaVO.Mobile.Delega delega)
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    CreaDelegaResponse retval = await MobileManager.creaDelega(token, delega);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}

        //[Route("DelegaDaModello")]
        //[ResponseType(typeof(CreaDelegaDaModelloResponse))]
        //[HttpPost]
        //public async Task<HttpResponseMessage> creaDelegaDaModello(CreaDelegaDaModelloRequest request)
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    CreaDelegaDaModelloResponse retval = await MobileManager.creaDelegaDaModello(token, request);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}

        //[Route("ModelliDelega")]
        //[ResponseType(typeof(ListaModelliDelegaResponse))]
        //[HttpGet]
        //public async Task<HttpResponseMessage> getListaModelliDelega()
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    ListaModelliDelegaResponse retval = await MobileManager.getListaModelliDelega(token);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}

        //[Route("ListaTipiRuolo")]
        //[ResponseType(typeof(ListaTipiRuoloResponse))]
        //[HttpGet]
        //public async Task<HttpResponseMessage> getListaTipiRuolo()
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    ListaTipiRuoloResponse retval = await MobileManager.getListaRuoli(token);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}

        //[Route("ListaUtenti")]
        //[ResponseType(typeof(ListaUtentiResponse))]
        //[HttpGet]
        //public async Task<HttpResponseMessage> getListaTipiRuolo(string codiceTipoRuolo)
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    ListaUtentiResponse retval = await MobileManager.getListaUtenti(token, codiceTipoRuolo);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}

        //[Route("RicercaUtenti")]
        //[ResponseType(typeof(RicercaUtentiResponse))]
        //[HttpGet]
        //public async Task<HttpResponseMessage> ricercaUtenti(string descrizione, int numMaxResult)
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    RicercaUtentiResponse retval = await MobileManager.ricercaUtenti(token, descrizione, numMaxResult);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}
        #endregion
        #region codice commentato trasmissioni
        //[Route("TrasmissioneVista")]
        //[ResponseType(typeof(AccettaRifiutaTrasmResponse))]
        //[HttpPost]
        //public async Task<HttpResponseMessage> setDataVista_SP(AccettaRifiutaTrasmRequest request)
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    AccettaRifiutaTrasmResponse retval = await MobileManager.setDataVistaSP_TV(token, request);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}

        //[Route("AccettaRifiutaTrasm")]
        //[ResponseType(typeof(AccettaRifiutaTrasmResponse))]
        //[HttpPost]
        //public async Task<HttpResponseMessage> AccettaRifiutaTrasm(AccettaRifiutaTrasmRequest request)
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    AccettaRifiutaTrasmResponse retval = await MobileManager.accettaRifiutaTrasm(token, request);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}

        //[Route("ListaModelliTrasmissione")]
        //[ResponseType(typeof(ListaModelliTrasmResponse))]
        //[HttpGet]
        //public async Task<HttpResponseMessage> listaModelliTrasm(bool fascicoli)
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    ListaModelliTrasmResponse retval = await MobileManager.getListaModelliTrasm(token, fascicoli);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}

        //[Route("EseguiTrasmissione")]
        //[ResponseType(typeof(EseguiTrasmResponse))]
        //[HttpPost]
        //public async Task<HttpResponseMessage> eseguiTrasm(EseguiTrasmRequest request)
        //{
        //    HttpResponseMessage response = null;
        //    string token = Request.Headers.GetValues("AuthToken").FirstOrDefault() as String;
        //    EseguiTrasmResponse retval = await MobileManager.eseguiTrasm(token, request);
        //    response = Request.CreateResponse(HttpStatusCode.OK, retval);
        //    return response;
        //}
        #endregion


    }
}