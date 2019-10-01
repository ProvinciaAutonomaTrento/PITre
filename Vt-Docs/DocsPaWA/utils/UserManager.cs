using System;
using System.Configuration;
using DocsPAWA.DocsPaWR;
using System.Web.UI;
using System.Web;
using System.Globalization;
using System.Xml;
using System.Collections;
using System.Data;
using log4net;
using System.Collections.Generic;
using DocsPAWA.AdminTool.Manager;

namespace DocsPAWA
{
    /// <summary>
    /// Summary description for UserManager.
    /// </summary>
    public class UserManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(UserManager));
        private static DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();
        # region Variabile di session per fermare il doppio caricamento della DocSalva.aspx;
        public static string getBoolDocSalva(Page page)
        {
            return (string)page.Session["docsalva.bool"];
        }

        public static void setBoolDocSalva(Page page, string salvato)
        {
            page.Session["docsalva.bool"] = salvato;
        }
        public static void removeBoolDocSalva(Page page)
        {
            page.Session.Remove("docsalva.bool");
        }
        # endregion
        public static void removeSelectedFile(Page page)
        {
            logger.Info("BEGIN");
            page.Session.Remove("docsalva.bool");
            logger.Info("END");
        }
        # region Variabile di session per il Parent Corrispondente in inserimento corrispondenti della rubrica
        public static DocsPAWA.DocsPaWR.Corrispondente getParentCorr(Page page)
        {
            return (DocsPAWA.DocsPaWR.Corrispondente)page.Session["rubricaDT.parentCorr"];
        }

        public static void setParentCorr(Page page, DocsPaWR.Corrispondente salvato)
        {
            page.Session["rubricaDT.parentCorr"] = salvato;
        }
        public static void removeParentCorr(Page page)
        {
            page.Session.Remove("rubricaDT.parentCorr");
        }
        # endregion

        # region Variabile di session per la lista dei canali
        public static DocsPAWA.DocsPaWR.Canale[] getListaCanali(Page page)
        {
            if (page.Session["rubrica.listaCanali"] == null)
            {
                DocsPaWR.Canale[] canaliDataSets = docsPaWS.AddressbookGetCanali();

                if (canaliDataSets == null)
                {
                    throw new Exception();
                }

                int arrayLength = canaliDataSets.Length;
                DocsPaWR.Canale[] canali = new DocsPAWA.DocsPaWR.Canale[arrayLength];

                for (int i = 0; i < arrayLength; i++)
                {
                    canali[i] = canaliDataSets[i];
                }

                page.Session["rubrica.listaCanali"] = canali;
            }

            return (DocsPAWA.DocsPaWR.Canale[])page.Session["rubrica.listaCanali"];
        }
        #endregion

        #region Variabile di sessione per la doppia login
        public static void setDoppiaLogin(Page page)
        {
            page.Session["userDoubleLogin"] = true;
        }
        #endregion

        #region Variabile di sessione per il num max utenti
        public static void setMaxUserReached(Page page)
        {
            page.Session["maxUserReached"] = true;
        }
        #endregion

        #region Amministrazioni


        public static DocsPAWA.DocsPaWR.Configurazione getParametroConfigurazione(Page page)
        {
            try
            {
                return docsPaWS.amministrazioneGetParametroConfigurazione();
            }
            catch (Exception) { }
            return null;
        }

        public static string getStringaConfigurazione(Page page, bool notNull)
        {
            string res = null;
            try
            {
                DocsPaWR.Configurazione conf = docsPaWS.amministrazioneGetParametroConfigurazione();
                if (conf != null)
                    res = conf.valore;
            }
            catch (Exception) { }
            if (notNull && res == null)
                return "";
            else return res;
        }

        public static DocsPAWA.DocsPaWR.Amministrazione[] getListaAmministrazioni(Page page, out string returnMsg)
        {
            returnMsg = string.Empty;

            try
            {
                return docsPaWS.amministrazioneGetAmministrazioni(out returnMsg);
            }
            catch (System.Exception e)
            {
                ErrorManager.redirectToErrorPage(page, e);
            }

            return null;
        }
        #endregion

        #region Gestione login utente

        public static Utente login(Page page, DocsPAWA.DocsPaWR.UserLogin login,
            out DocsPAWA.DocsPaWR.LoginResult loginResult, out string ipaddress)
        {
            Utente utente = null;
            loginResult = DocsPAWA.DocsPaWR.LoginResult.OK;
            ipaddress = "";

            try
            {
                loginResult = docsPaWS.Login(login, false, page.Session.SessionID, out utente, out ipaddress);
            }
            catch (Exception exception)
            {
                string msg = "Login Error";
                logger.Debug(msg, exception);

                loginResult = DocsPAWA.DocsPaWR.LoginResult.APPLICATION_ERROR;
                utente = null;
            }

            return utente;
        }


        public static Utente login(string SessionID, DocsPAWA.DocsPaWR.UserLogin login,
            out DocsPAWA.DocsPaWR.LoginResult loginResult, out string ipaddress)
        {
            Utente utente = null;
            loginResult = DocsPAWA.DocsPaWR.LoginResult.OK;
            ipaddress = "";

            try
            {
                loginResult = docsPaWS.Login(login, false, SessionID, out utente, out ipaddress);
            }
            catch (Exception exception)
            {
                string msg = "Login Error";
                logger.Debug(msg, exception);

                loginResult = DocsPAWA.DocsPaWR.LoginResult.APPLICATION_ERROR;
                utente = null;
            }

            return utente;
        }


        public static Utente LoginBatch(string SessionID, string userName, string DST,
            out DocsPAWA.DocsPaWR.LoginResult loginResult, out DocsPAWA.DocsPaWR.UserLogin userLogin)
        {
            userLogin = null;
            DocsPaWR.Utente utente = null;

            try
            {
                userLogin = docsPaWS.VerificaUtente(userName);
                if (userLogin == null) throw new Exception("Errore durante la validazione dell'utente");
                userLogin.DST = DST;
                string ipaddress = "";
                utente = login(SessionID, userLogin, out loginResult, out ipaddress);

            }
            catch (Exception e)
            {
                string msg = "Login Batch Error";
                logger.Debug(msg, e);

                loginResult = DocsPAWA.DocsPaWR.LoginResult.APPLICATION_ERROR;
                utente = null;
            }

            return utente;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="login"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public static Utente ForcedLogin(Page page, DocsPAWA.DocsPaWR.UserLogin login, out DocsPAWA.DocsPaWR.LoginResult loginResult)
        {
            Utente utente = null;
            loginResult = DocsPAWA.DocsPaWR.LoginResult.OK;
            string ipaddress;

            try
            {
                loginResult = docsPaWS.Login(login, true, page.Session.SessionID, out utente, out ipaddress);
            }
            catch (Exception exception)
            {
                string msg = "Login Error";
                logger.Debug(msg, exception);

                loginResult = DocsPAWA.DocsPaWR.LoginResult.APPLICATION_ERROR;
                utente = null;
            }

            return utente;
        }

        /// <summary>
        /// Controlla che la sessione corrisponda ad un utente connesso
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static DocsPAWA.DocsPaWR.ValidationResult ValidateLogin(string userName, string idAmm, string sessionId)
        {
            DocsPaWR.ValidationResult result = DocsPAWA.DocsPaWR.ValidationResult.APPLICATION_ERROR;

            try
            {
                result = docsPaWS.ValidateLogin(userName, idAmm, sessionId);
            }
            catch (Exception exception)
            {
                logger.Debug("Impossibile validare la sessione.", exception);
                result = DocsPAWA.DocsPaWR.ValidationResult.APPLICATION_ERROR;
            }

            return result;
        }

        public static void logoff(Page page)
        {
            //InfoUtente infoUtente = UserManager.getInfoUtente(page);
            // docsPaWS.Logoff(infoUtente);
            logoff(page.Session);
            page.Session.Abandon();
        }

        public static Utente getUtente(System.Web.SessionState.HttpSessionState session)
        {
            //ABBATANGELI GIANLUIGI - Aggiunta l'informazione che indica l'applicazione su cui sta lavorando l'utente
            Utente utente = session["userData"] as Utente;
            if (utente != null)
                utente.codWorkingApplication = (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]) ? "___" : System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]);
            return utente;
        }

        public static Utente getUtente(Page page)
        {
            return getUtente();
        }

        public static Utente getUtente()
        {
            Utente utente = getUtente(HttpContext.Current.Session);
            if(utente == null)
                throw new Exception("Utente non in sessione");

            return utente;
        }

        public static string[] getListaVisibilitaRuolo(Page page, string docNumber, string inString)
        {
            string[] result = null;
            try
            {

                result = docsPaWS.GetVisibilitaRuoli(docNumber, inString);
                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="sessionId"></param>
        public static void logoff(DocsPAWA.DocsPaWR.InfoUtente infoUtente, string sessionId)
        {
            try
            {
                string appConfigValue = ConfigSettings.getKey(ConfigSettings.KeysENUM.DISABLE_LOGOUT_CLOSE_BUTTON);
                
                if (appConfigValue == null || (!Convert.ToBoolean(appConfigValue)))
                    docsPaWS.Logoff(infoUtente.userId, infoUtente.idAmministrazione, sessionId, infoUtente.dst);
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        public static void logoff(System.Web.SessionState.HttpSessionState session)
        {
            Utente utente = (Utente)session["userData"];
            Ruolo ruolo = (Ruolo)session["userRuolo"];
            InfoUtente infoUtente = null;

            if (ruolo != null && utente != null)
                logoff(getInfoUtente(utente, ruolo), session.SessionID);
        }

        public static void logoffAdmin(DocsPaWR.InfoUtenteAmministratore adminUser)
       {
           try
           {
               docsPaWS.LogoutAmministratore(adminUser);
           }
           catch { }
       }

        public static void setUtente(Page page, Utente utente)
        {
            utente.urlWA = Utils.getHttpFullPath(page);
            page.Session["userData"] = utente;
        }

        public static Ruolo getRuolo(Page page)
        {
            return getRuolo();
        }

        public static Ruolo getRuolo()
        {
            return getRuolo(HttpContext.Current.Session);
        }

        public static Ruolo getRuolo(System.Web.SessionState.HttpSessionState session)
        {
            return session["userRuolo"] as Ruolo;
        }

        public static void setRuolo(Page page, Ruolo ruolo)
        {
            setRuolo(ruolo);
        }

        public static void setRuolo(Ruolo ruolo)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["userRuolo"] = ruolo;
        }


        public static Ruolo getRuoloDelegato()
        {
            return (Ruolo)HttpContext.Current.Session["userRuoloDelegato"];
        }

        public static void setRuoloDelegato(Ruolo ruolo)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["userRuoloDelegato"] = ruolo;
        }

        public static Utente getUtenteDelegato()
        {
            return (Utente)HttpContext.Current.Session["userDataDelegato"];
            
        }

        public static void setUtenteDelegato(Page page, Utente utente)
        {
            utente.urlWA = Utils.getHttpFullPath(page);
            page.Session["userDataDelegato"] = utente;
        }

         public static void setDelegato(InfoUtente infoUtente)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["userDelegato"] = infoUtente;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>?
        /// <returns></returns>
        public static InfoUtente getDelegato(System.Web.SessionState.HttpSessionState session)
        {
            return session["userDelegato"] as InfoUtente;
        }

        public static InfoUtente getDelegato()
        {
            return getDelegato(HttpContext.Current.Session);
        }

        public static InfoUtente getInfoUtente(Page page)
        {
            Utente utente = getUtente(page);

            Ruolo ruolo = getRuolo(page);
            return getInfoUtente(utente, ruolo);
        }

        public static InfoUtente getInfoUtente()
        {
            Utente utente = getUtente();
            Ruolo ruolo = getRuolo();
            return getInfoUtente(utente, ruolo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static InfoUtente getInfoUtente(System.Web.SessionState.HttpSessionState session)
        {
            Utente utente = getUtente(session);
            Ruolo ruolo = getRuolo(session);

            InfoUtente infoUtente = new InfoUtente();

            try
            {
                if (utente != null)
                {
                    infoUtente.idPeople = utente.idPeople;
                    infoUtente.dst = utente.dst;
                    infoUtente.idAmministrazione = utente.idAmministrazione;
                    infoUtente.userId = utente.userId;
                    infoUtente.sede = utente.sede;
                    if (utente.urlWA != null)
                        infoUtente.urlWA = utente.urlWA;
                    infoUtente.delegato = getDelegato(session);
                    
                    //ABBATANGELI GIANLUIGI - Gestione delle applicazioni esterne
                    infoUtente.extApplications = utente.extApplications;
                    //ABBATANGELI GIANLUIGI - Aggiunta l'informazione che indica l'applicazione su cui sta lavorando l'utente
                    infoUtente.codWorkingApplication = (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]) ? "___" : System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]);
                }

                if (ruolo != null)
                {
                    infoUtente.idCorrGlobali = ruolo.systemId;
                    infoUtente.idGruppo = ruolo.idGruppo;
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("Impossibile accedere alle informazioni dell'utente" + exception.ToString());
                infoUtente = null;
            }

            return infoUtente;
        }

        public static InfoUtente getInfoUtente(Utente utente, Ruolo ruolo)
        {
            return getInfoUtente(HttpContext.Current.Session);
        }

        public static InfoUtente getInfoUtente(Page page, Utente utente, Ruolo ruolo)
        {
            InfoUtente infoUtente = new InfoUtente();

            try
            {
                infoUtente.idCorrGlobali = ruolo.systemId;
                infoUtente.idPeople = utente.idPeople;
                infoUtente.idGruppo = ruolo.idGruppo;
                infoUtente.dst = utente.dst;
                infoUtente.idAmministrazione = utente.idAmministrazione;
                infoUtente.userId = utente.userId;
                infoUtente.sede = utente.sede;
                if (utente != null && utente.urlWA != null)
                    infoUtente.urlWA = utente.urlWA;

                infoUtente.delegato = getDelegato();

                //ABBATANGELI GIANLUIGI
                infoUtente.extApplications = utente.extApplications;

                infoUtente.codWorkingApplication = (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]) ? "___" : System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("Impossibile accedere alle informazioni dell'utente" + exception.ToString());
                infoUtente = null;
            }

            return infoUtente;
        }

        public static bool disabilitaButtHMDiritti(string accessRights)
        {
            bool disabilita = false;
            DocsPaVO.HMDiritti.HMdiritti HMD = new DocsPaVO.HMDiritti.HMdiritti();
            if ((Convert.ToInt32(accessRights) < HMD.HMdiritti_Write) || (Convert.ToInt32(accessRights) < HMD.HMdiritti_Eredita))
            {
                disabilita = true;
            }
            return disabilita;
        }

        public static bool disabilitaButtHMDirittiTrasmInAccettazione(string accessRights)
        {
            DocsPaVO.HMDiritti.HMdiritti HMD = new DocsPaVO.HMDiritti.HMdiritti();
            return (Convert.ToInt32(accessRights) < HMD.HMdiritti_Read);           
        }

        private static void EnablePageControls(Page page)
        {
            foreach (Control ctl in page.Controls)
            {
                EnableChildControls(ctl);
            }
        }

        private static void EnableChildControls(Control parentCtl)
        {
            foreach (Control ctl in parentCtl.Controls)
            {

                DocsPaWebCtrlLibrary.ImageButton btn = ctl as DocsPaWebCtrlLibrary.ImageButton;

                if (btn != null && btn.Tipologia != null &&
                    btn.Tipologia != string.Empty &&
                    !ruoloIsAutorized(ctl.Page, btn.Tipologia))
                    btn.Enabled = false;

                EnableChildControls(ctl);

            }
        }

        public static void disabilitaFunzNonAutorizzate(Page page)
        {
            EnablePageControls(page);
        }

        public static void disabilitaVociMenuNonAutorizzate(Page page)
        {
            docsPaMenu.DocsPaMenuWC vMenu = new docsPaMenu.DocsPaMenuWC();
            System.Web.UI.HtmlControls.HtmlForm myForm = new System.Web.UI.HtmlControls.HtmlForm();

            for (int i = 0; i < page.Controls.Count; i++)
            {
                if (page.Controls[i].GetType().Equals(myForm.GetType()))
                {
                    myForm = (System.Web.UI.HtmlControls.HtmlForm)page.Controls[i];
                    for (int j = 0; j < myForm.Controls.Count; j++)
                    {
                        if (myForm.Controls[j].GetType().Equals(vMenu.GetType()))
                        {
                            vMenu = (docsPaMenu.DocsPaMenuWC)myForm.Controls[j];
                            if (vMenu.Links != null)
                            {
                                for (int k = 0; k < vMenu.Links.Count; k++)
                                {
                                    if (vMenu.Links[k] != null && vMenu.Links[k].Type != null)
                                    {
                                        if (!vMenu.Links[k].Type.Equals(""))
                                        {
                                            if (!ruoloIsAutorized(page, vMenu.Links[k].Type))
                                            {
                                                vMenu.Links[k].Visible = false;
                                            }
                                            else
                                            {
                                                EnableButtonProtSemplificata(vMenu.Links[k]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void disabilitaVociMenuNonAutorizzate(UserControl page)
        {
            docsPaMenu.DocsPaMenuWC vMenu = new docsPaMenu.DocsPaMenuWC();
            System.Web.UI.HtmlControls.HtmlForm myForm = new System.Web.UI.HtmlControls.HtmlForm();

            for (int i = 0; i < page.Controls.Count; i++)
            {
                if (page.Controls[i].GetType().Equals(myForm.GetType()))
                {
                    myForm = (System.Web.UI.HtmlControls.HtmlForm)page.Controls[i];
                    for (int j = 0; j < myForm.Controls.Count; j++)
                    {
                        if (myForm.Controls[j].GetType().Equals(vMenu.GetType()))
                        {
                            vMenu = (docsPaMenu.DocsPaMenuWC)myForm.Controls[j];
                            if (vMenu.Links != null)
                            {
                                for (int k = 0; k < vMenu.Links.Count; k++)
                                {
                                    if (vMenu.Links[k] != null && vMenu.Links[k].Type != null)
                                    {
                                        if (!vMenu.Links[k].Type.Equals(""))
                                        {
                                            if (!ruoloIsAutorized(page, vMenu.Links[k].Type))
                                            {
                                                vMenu.Links[k].Visible = false;
                                            }
                                            else
                                            {
                                                EnableButtonProtSemplificata(vMenu.Links[k]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gestione abilitazione / disabilitazione
        /// pulsante per l'accesso alla protocollazione in ingresso
        /// in funzione del tag "PROTO_SEMPLIFICATO_ENABLED" del web.config
        /// </summary>
        private static void EnableButtonProtSemplificata(docsPaMenu.myLink link)
        {
            bool isEnabled = true;
            if ((link.Key == "PROTO_INGRESSO_SEMPLIFICATO") || 
                (link.Key == "PROTO_USCITA_SEMPLIFICATO"))
            {
                isEnabled = IsEnabledProtocollazioneSemplificata();
            }
            
            // Se la key della voce di menù è "PROTO_INGRESSO_SEMPLIFICATO"
            // e se è visibile, viene impostata la visibilità del controllo
            // in base al valore del tag del web.config.
            // Ciò significa che qualsiasi impostazione da ACL di docspa
            // viene sempre sovrascritta dalle impostazioni nel web.config.

            link.Visible = isEnabled;

            //			bool isEnabled=IsEnabledButtonProtIngresso();
            //
            //			// Se la key della voce di menù è "PROTO_INGRESSO_SEMPLIFICATO"
            //			// e se è visibile, viene impostata la visibilità del controllo
            //			// in base al valore del tag del web.config.
            //			// Ciò significa che qualsiasi impostazione da ACL di docspa
            //			// viene sempre sovrascritta dalle impostazioni nel web.config.
            //			if (link.Key=="PROTO_INGRESSO_SEMPLIFICATO" && link.Visible)
            //			{
            //				link.Visible=isEnabled;
            //			}

        }

        /// <summary>
        /// Si verifica, in base al valore del tab "PROTO_SEMPLIFICATO_ENABLED" del web.config,
        /// se deve essere abilitata la funzionalità di protocollazione in ingresso semplificata
        /// </summary>
        /// <returns></returns>
        public static bool IsEnabledProtocollazioneSemplificata()
        {
            bool isVisible = false;
            string configValue = ConfigSettings.getKey(ConfigSettings.KeysENUM.PROTO_SEMPLIFICATO_ENABLED);

            if (configValue != null && configValue != string.Empty)
                isVisible = Convert.ToBoolean(configValue);

            return isVisible;
        }

        public static bool visualBtn(Page page, string tipologia)
        {
            bool retValue = false;
            if (tipologia.Equals("DO_SMISTA"))
            {
                string test = ConfigSettings.getKey("T_" + tipologia);
                if (test != null && test.Equals("1"))
                    retValue = true;
            }
            else
            {
                retValue = true;
            }
            return retValue;
        }

        public static bool ruoloIsAutorized(Page page, string tipologia)
        {
            DocsPaWR.Ruolo userRuolo = null;
            if (page == null)
                userRuolo = getRuolo();
            else
                userRuolo = getRuolo(page);
            if (userRuolo == null || userRuolo.funzioni == null)
                return false;
            /*-*/
            bool trovato = false;
            for (int i = 0; i < userRuolo.funzioni.Length; i++)
            {
                if (userRuolo.funzioni[i].codice.Equals(tipologia))
                {
                    trovato = true;
                    break;
                }
            }
            return trovato;
        }

        public static bool ruoloIsAutorized(UserControl page, string tipologia)
        {
            DocsPaWR.Ruolo userRuolo = null;
            //if (page == null)
            //    userRuolo = getRuolo();
            //else
                userRuolo = getRuolo();
            if (userRuolo == null || userRuolo.funzioni == null)
                return false;
            /*-*/
            bool trovato = false;
            for (int i = 0; i < userRuolo.funzioni.Length; i++)
            {
                if (userRuolo.funzioni[i].codice.Equals(tipologia))
                {
                    trovato = true;
                    break;
                }
            }
            return trovato;
        }

        /// <summary>
        /// Creazione oggetto "UserLogin" a partire dai metadati dalla sessione utente corrente
        /// </summary>
        /// <param name="password">La password deve essere fornita dall'utente, in quanto non è mantenuta nella sessione</param>
        /// <returns></returns>
        public static DocsPaWR.UserLogin CreateUserLoginCurrentUser(string password)
        {
            DocsPaWR.UserLogin userLogin = null;

            // Reperimento oggetto infoutente corrente
            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();

            if (infoUtente != null)
            {
                userLogin = new DocsPAWA.DocsPaWR.UserLogin();
                userLogin.SystemID = infoUtente.idPeople;
                userLogin.UserName = infoUtente.userId;
                userLogin.Password = password;
                userLogin.IdAmministrazione = infoUtente.idAmministrazione;
                userLogin.DST = infoUtente.dst;
                userLogin.IPAddress = HttpContext.Current.Request.UserHostAddress;
            }

            return userLogin;
        }


        /// <summary>
        /// Creazione oggetto "UserLogin" a partire dai metadati dalla sessione utente corrente
        /// </summary>
        /// <param name="password">La password deve essere fornita dall'utente, in quanto non è mantenuta nella sessione</param>
        /// <returns></returns>
        public static DocsPaWR.UserLogin CreateUserLoginCurrentUser(string password, bool insertMode)
        {
            DocsPaWR.UserLogin userLogin = null;

            // Reperimento oggetto infoutente corrente
            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();

            if (infoUtente != null)
            {
                userLogin = new DocsPAWA.DocsPaWR.UserLogin();
                userLogin.SystemID = infoUtente.idPeople;
                userLogin.UserName = infoUtente.userId;
                userLogin.Password = password;
                userLogin.IdAmministrazione = "0";
                userLogin.DST = infoUtente.dst;
                userLogin.IPAddress = HttpContext.Current.Request.UserHostAddress;
            }

            return userLogin;
        }

        #endregion



        #region Gestione registri
        public static Registro getRegistroSelezionato(Page page)
        {
            //return ((Registro)page.Session["userRegistro"]);
            return (Registro)GetSessionValue("userRegistro");
        }

        public static void setRegistroSelezionato(Page page, Registro registro)
        {
            //setto la lista degli idRegistri selezionati per le ricerche dei corrispondenti...
            setListaIdRegistri(page, registro.systemId);

            //page.Session["userRegistro"] = registro;
            SetSessionValue("userRegistro", registro);
        }


        public static Registro getregistroSelezionato(Page page)
        {
            string id_registro = (string)GetSessionValue("RegistroSelezionato");
            return docsPaWS.GetRegistroBySistemId(id_registro);
        }

        public static void setregistroSelezionato(Page page, Registro registro)
        {
            setListaIdRegistri(page, registro.systemId);
            SetSessionValue("RegistroSelezionato", registro);
        }

        public static void removeRegistroSelezionato(Page page)
        {
            //page.Session.Remove("userRegistro");
            RemoveSessionValue("userRegistro");
        }


        public static string getIdRegistroSelezionato(Page page)
        {
            return getRegistroSelezionato(page).systemId;
        }

        public static Registro[] getListaRegistri(Page page)
        {
            Registro[] result = null;
            try
            {
                result = docsPaWS.UtenteGetRegistri(getInfoUtente(page).idCorrGlobali);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			} 
            catch (Exception exception)
            {
                ErrorManager.redirect(page, exception);
            }

            return result;
        }

        public static Registro[] getListaRegistriNoFiltroAOO(string IdAmministrazione, out bool filtroAOO)
        {
            filtroAOO = false;
            Registro[] result = null;
            try
            {
                result = docsPaWS.UtenteGetRegistriNoFiltroAOO(IdAmministrazione, out filtroAOO);
            }
            catch (Exception exception)
            {
                throw(exception);
            }
            return result;
        }
        public static Registro[] getListaRegistriNoFiltroAOO(Page page, out bool filtroAOO)
        {
            filtroAOO = false;
            Registro[] result = null;
            try
            {
                result = docsPaWS.UtenteGetRegistriNoFiltroAOO(getInfoUtente(page).idAmministrazione, out filtroAOO);
            }
            catch (Exception exception)
            {
                ErrorManager.redirect(page, exception);
            }
            return result;
        }

        public static bool isFiltroAooEnabled(Page page)
        {
            bool result = false;
             try
            {
                result = docsPaWS.isFiltroAooEnabled();
            }
            catch (Exception exception)
            {
                ErrorManager.redirect(page, exception);
            }
            return result;
            
        }

        public static DocsPAWA.DocsPaWR.Registro getRegistroBySistemId(Page page, string idRegistro)
        {
            DocsPaWR.Registro result = null;
            try
            {
                result = docsPaWS.GetRegistroBySistemId(idRegistro);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception exception)
            {
                ErrorManager.redirect(page, exception);
            }

            return result;
        }

        public static string getStatoRegistro(DocsPAWA.DocsPaWR.Registro reg)
        {
            // R = Rosso -  CHIUSO
            // V = Verde -  APERTO
            // G = Giallo - APERTO IN GIALLO

            string dataApertura = reg.dataApertura;

            if (!dataApertura.Equals(""))
            {

                DateTime dt_cor = DateTime.Now;

                CultureInfo ci = new CultureInfo("it-IT");

                string[] formati ={ "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };

                DateTime d_ap = DateTime.ParseExact(dataApertura, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                //aggiungo un giorno per fare il confronto con now (che comprende anche minuti e secondi)
                d_ap = d_ap.AddDays(1);

                string mydate = dt_cor.ToString(ci);

                //DateTime dt = DateTime.ParseExact(mydate,formati,ci.DateTimeFormat,DateTimeStyles.AllowWhiteSpaces);

                string StatoAperto = ConfigSettings.getKey("STATO.REG.APERTO");
                if (reg.stato.Equals(StatoAperto))
                {
                    if (dt_cor.CompareTo(d_ap) > 0)
                    {
                        //data odierna maggiore della data di apertura del registro
                        return "G";
                    }
                    else
                        return "V";
                }
            }
            return "R";

        }

        #region Gestione id registri per ricerca
        private static string[] getListaIdRegistri(Page page, Registro[] registri)
        {
            int numRegistri = registri.Length;
            string[] id = new string[numRegistri];
            for (int i = 0; i < numRegistri; i++)
                id[i] = registri[i].systemId;
            return id;
        }

        public static string[] getListaIdRegistriRuolo(Page page, DocsPAWA.DocsPaWR.Ruolo ruolo)
        {
            return getListaIdRegistri(page, ruolo.registri);
        }

        public static string[] getListaIdRegistriUtente(Page page)
        {
            return getListaIdRegistri(page, getListaRegistri(page));
        }

        public static string[] getListaIdRegistri(Page page)
        {
            return (string[])page.Session["rubrica.listaIdRegistri"];
        }

        public static void setListaIdRegistri(Page page, string[] idRegistri)
        {
            page.Session["rubrica.listaIdRegistri"] = idRegistri;
        }

        public static void removeListaIdRegistri(Page page)
        {
            page.Session.Remove("rubrica.listaIdRegistri");
        }

        public static void setListaIdRegistri(Page page, string idRegistro)
        {
            string[] idRegistri = { idRegistro };
            setListaIdRegistri(page, idRegistri);
        }
        #endregion

        #endregion

        #region Gestione Corrispondente
        public static DocsPAWA.DocsPaWR.Corrispondente GetCorrispondenteInterno(Page page, string codiceRubrica, bool fineValidita)
        {
            try
            {
                Utente utente = getUtente(page);
                AddressbookQueryCorrispondente qco = new AddressbookQueryCorrispondente();
                qco.idRegistri = getListaIdRegistri(page);
                qco.codiceRubrica = codiceRubrica;
                qco.getChildren = false;
                qco.idAmministrazione = utente.idAmministrazione;
                qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
                qco.fineValidita = fineValidita;
                DocsPaWR.Corrispondente result;
                DocsPaWR.Corrispondente[] corrispondenti = docsPaWS.AddressbookGetListaCorrispondenti(qco);

                if (corrispondenti.Length > 0)
                {
                    //result = corrispondenti[0];
                    result = getAllRuoli(corrispondenti, false)[0];
                }
                else
                {
                    result = null;
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        #region Gestione Corrispondente se vengo da Organigramma in Rubrica
        public static DocsPAWA.DocsPaWR.Corrispondente GetCorrispondenteInternoOrganigramma(Page page, string codiceRubrica, bool fineValidita)
        {
            try
            {
                Utente utente = getUtente(page);
                AddressbookQueryCorrispondente qco = new AddressbookQueryCorrispondente();
                qco.idRegistri = getListaIdRegistri(page);
                qco.codiceRubrica = codiceRubrica;
                qco.getChildren = false;
                qco.idAmministrazione = utente.idAmministrazione;
                qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
                qco.fineValidita = fineValidita;
                DocsPaWR.Corrispondente result;
                DocsPaWR.Corrispondente[] corrispondenti = docsPaWS.AddressbookGetListaCorrispondenti(qco);

                if (corrispondenti.Length > 0)
                {

                    result = getAllRuoli(corrispondenti, true)[0];
                }
                else
                {
                    result = null;
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        #endregion

        public static DocsPAWA.DocsPaWR.Corrispondente GetCorrispondenteInterno(Page page, string codiceRubrica, bool fineValidita, bool destProtoInt)
        {
            try
            {
                Utente utente = getUtente(page);
                AddressbookQueryCorrispondente qco = new AddressbookQueryCorrispondente();
                qco.idRegistri = getListaIdRegistri(page);
                qco.codiceRubrica = codiceRubrica;
                qco.getChildren = false;
                qco.idAmministrazione = utente.idAmministrazione;
                qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
                qco.fineValidita = fineValidita;
                qco.descrizioneRuolo = "";

                DocsPaWR.Corrispondente result;
                //DocsPaWR.Corrispondente[] corrispondenti = docsPaWS.AddressbookGetListaCorrispondenti(qco);
                DocsPaWR.Corrispondente[] corrispondenti = docsPaWS.AddressbookGetListaCorrispondenti_Aut(qco);

                if (corrispondenti.Length > 0)
                {
                    //result = corrispondenti[0];
                    result = getAllRuoli(corrispondenti, false)[0];
                }
                else
                {
                    result = null;
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static DocsPAWA.DocsPaWR.Corrispondente[] getAllRuoli(DocsPAWA.DocsPaWR.Corrispondente[] corrispondenti, bool cameFromRubrica)
        {
            string l_oldSystemId = "";
            System.Object[] l_objects = new System.Object[0];
            System.Object[] l_objects_ruoli = new System.Object[0];
            DocsPaWR.Ruolo[] lruolo = new DocsPAWA.DocsPaWR.Ruolo[0];
            int i = 0;
            foreach (DocsPAWA.DocsPaWR.Corrispondente t_corrispondente in corrispondenti)
            {
                string t_systemId = t_corrispondente.systemId;
                if (t_systemId != l_oldSystemId)
                {
                    l_objects = Utils.addToArray(l_objects, t_corrispondente);
                    l_oldSystemId = t_systemId;
                    i = i + 1;
                    continue;
                }
                else
                {
                    /* il corrispondente non viene aggiunto, in quanto sarebbe un duplicato 
                     * ma viene aggiunto solamente il ruolo */

                    if (t_corrispondente.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
                    {
                        if (!cameFromRubrica)
                        {
                            if ((l_objects[i - 1]).GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
                            {
                                l_objects_ruoli = ((Utils.addToArray(((DocsPAWA.DocsPaWR.Utente)(l_objects[i - 1])).ruoli, ((DocsPAWA.DocsPaWR.Utente)t_corrispondente).ruoli[0])));
                                DocsPaWR.Ruolo[] l_ruolo = new DocsPAWA.DocsPaWR.Ruolo[l_objects_ruoli.Length];
                                ((DocsPAWA.DocsPaWR.Utente)(l_objects[i - 1])).ruoli = l_ruolo;
                                l_objects_ruoli.CopyTo(((DocsPAWA.DocsPaWR.Utente)(l_objects[i - 1])).ruoli, 0);

                            }
                        }

                    }
                }

            }

            DocsPaWR.Corrispondente[] l_corrSearch = new DocsPAWA.DocsPaWR.Corrispondente[l_objects.Length];
            l_objects.CopyTo(l_corrSearch, 0);

            return l_corrSearch;
        }

        public static DocsPAWA.DocsPaWR.Ruolo[] getRuoliFiltrati(DocsPAWA.DocsPaWR.Ruolo[] ruoli)
        {
            //string l_oldSystemId="";								
            System.Object[] l_objects = new System.Object[0];

            Hashtable keysUO = new Hashtable();

            foreach (DocsPAWA.DocsPaWR.Ruolo ruolo in ruoli)
            {
                string t_systemId = ruolo.uo.systemId;

                if (!keysUO.ContainsKey(t_systemId))
                {
                    l_objects = Utils.addToArray(l_objects, ruolo);
                    keysUO.Add(ruolo.uo.systemId, null);
                }

                //				string t_systemId = ruolo.uo.systemId;						
                //				if (t_systemId!=l_oldSystemId)
                //				{
                //					l_objects=Utils.addToArray(l_objects, ruolo); 	
                //					l_oldSystemId=t_systemId;
                //				}
            }

            keysUO.Clear();
            keysUO = null;

            DocsPaWR.Ruolo[] ruoliFiltrati = new DocsPAWA.DocsPaWR.Ruolo[l_objects.Length];
            l_objects.CopyTo(ruoliFiltrati, 0);

            return ruoliFiltrati;
        }

        public static DocsPAWA.DocsPaWR.Corrispondente getCorrispondente(Page page, string codiceRubrica, bool fineValidita)
        {
            try
            {
                Utente utente = getUtente(page);
                AddressbookQueryCorrispondente qco = new AddressbookQueryCorrispondente();
                qco.idRegistri = getListaIdRegistri(page);
                qco.codiceRubrica = codiceRubrica;
                qco.getChildren = false;
                qco.idAmministrazione = utente.idAmministrazione; //ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];					
                qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE;
                //qco.fineValidita = fineValidita;
                DocsPaWR.Corrispondente result;
                DocsPaWR.Corrispondente[] corrispondenti = docsPaWS.AddressbookGetListaCorrispondenti(qco);
                if (corrispondenti.Length > 0)
                {
                    result = corrispondenti[0];
                }
                else
                {
                    result = null;
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }



        private static string getCondizioneRegistro(DocsPaWR.Registro[] listaReg)
        {
            string retValue = "";
            foreach (DocsPaWR.Registro item in listaReg)
            {
                retValue += " " + item.systemId + ",";     
            }
            if (retValue.EndsWith(","))
                retValue = retValue.Remove(retValue.LastIndexOf(","));           
            return retValue;

        }

        public static DocsPAWA.DocsPaWR.Corrispondente getCorrispondenteRubrica(Page page, string codiceRubrica, DocsPaWR.RubricaCallType callType)
        {
            try
            {
                DocsPaWR.ParametriRicercaRubrica qco = new DocsPAWA.DocsPaWR.ParametriRicercaRubrica();
                //cerco su tutti i tipi utente:
                qco.calltype = callType;
                setQueryRubricaCaller(ref qco);

              //  qco.caller.filtroRegistroPerRicerca = qco.caller.IdRegistro;

                qco.codice = codiceRubrica;
                qco.tipoIE = DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE;


                //questo serve perchè in questi casi quando si cerca un esterno all'amministrazione
                //si deve ricercare anche tra gli esterni all'amministrazione che sono creati su degli RF
                //associati al registro corrente  (nel caso di protocollo)
                if (callType == RubricaCallType.CALLTYPE_PROTO_IN
                    || callType == RubricaCallType.CALLTYPE_PROTO_IN_INT
                    || callType == RubricaCallType.CALLTYPE_PROTO_OUT
                    || callType == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                     {
                    if (qco.caller.IdRegistro != null && qco.caller.IdRegistro != string.Empty)
                    {
                        DocsPaWR.Registro[] listaReg = getListaRegistriWithRF(page, "1", qco.caller.IdRegistro);

                        //Ritorna una lista di RF concatenati da una ","
                        string condReg = getCondizioneRegistro(listaReg);

                        if (condReg != null && condReg != string.Empty)
                        {
                            //se cè almeno un RF allora aggancio anche l'id registro
                            // per ricercare tra tutti gli esterni appartenenti
                            //al mio registro e agli RF ad esso associati che posso vedere
                            condReg += ", " + qco.caller.IdRegistro;
                        }
                        else
                        {
                            condReg += qco.caller.IdRegistro;
                        }
                        qco.caller.filtroRegistroPerRicerca = condReg;
                    }
                }
                
                //in questo caso devo ricercare i corrispondenti esterni all'amministrazione
                //tra tutti i corrispondenti che sono stati creati su registi e rf a cui
                //il ruolo corrente è associato
                if (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_MANAGE
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO
                ||callType == RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE)
                {
                   
                    //nuova gestione: devo cercare in tutti i registri e RF visibili al ruolo

                    DocsPaWR.Registro[] regRuolo = getListaRegistriWithRF(qco.caller.IdRuolo, "", "");

                    string filtroRegistro = "";
                    for (int i = 0; i < regRuolo.Length; i++)
                    {
                        filtroRegistro = filtroRegistro + regRuolo[i].systemId;
                        if (i < regRuolo.Length - 1)
                        {
                            filtroRegistro = filtroRegistro + " , ";
                        }
                    }

                    qco.caller.filtroRegistroPerRicerca = filtroRegistro;
                }
                 
                qco.doRuoli = true;
                qco.doUtenti = true;
                qco.doUo = true;
                qco.doListe = false;

                // Abilita la ricerca in rubrica comune, qualora l'utente sia abilitato
                qco.doRubricaComune = (RubricaComune.Configurazioni.GetConfigurazioni(UserManager.getInfoUtente()).GestioneAbilitata);

                qco.queryCodiceEsatta = true;
                DocsPaWR.Corrispondente corrRes;
                DocsPaWR.ElementoRubrica[] elSearch = UserManager.getElementiRubrica(page, qco);

                if (elSearch != null && elSearch.Length == 1)
                {                   
                    if (elSearch[0].rubricaComune != null && elSearch[0].rubricaComune.IdRubricaComune != null)
                    {
                        corrRes = getCorrispondenteByCodRubricaRubricaComune(page, elSearch[0].codice);
                    }
                    else
                    {
                        corrRes = getCorrispondenteBySystemID(page, elSearch[0].systemId);
                    }
                    
                }
                else
                {
                    corrRes = null;
                }

                return corrRes;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        #region elisa corrispondenti interni autorizzati
        public static Corrispondente[] getListaCorrispondentiAutProtoInt(Page page, AddressbookQueryCorrispondente qco)
        {
            try
            {

                Corrispondente[] result = docsPaWS.AddressbookGetListaCorrispondenti_Aut(qco);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        #endregion		//

        public static DocsPAWA.DocsPaWR.Corrispondente getCorrispondenteReferente(Page page, string codiceRubrica, bool fineValidita)
        {
            try
            {
                Utente utente = getUtente(page);
                AddressbookQueryCorrispondente qco = new AddressbookQueryCorrispondente();
                qco.idRegistri = getListaIdRegistri(page);
                qco.codiceRubrica = codiceRubrica;
                qco.getChildren = false;
                qco.idAmministrazione = utente.idAmministrazione;
                qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
                qco.fineValidita = fineValidita;

                DocsPaWR.Corrispondente result;
                //DocsPaWR.Corrispondente[] corrispondenti = docsPaWS.AddressbookGetListaCorrispondenti_Aut(qco);
                DocsPaWR.Corrispondente[] corrispondenti = docsPaWS.AddressbookGetListaCorrispondenti(qco);
                if (corrispondenti.Length > 0)
                {
                    result = corrispondenti[0];
                }
                else
                {
                    result = null;
                }

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }
        public static Corrispondente[] getListaCorrispondenti(Page page, AddressbookQueryCorrispondente qco)
        {
            try
            {
                //qco.fineValidita = true;
                Corrispondente[] result = docsPaWS.AddressbookGetListaCorrispondenti(qco);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			} 
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static DocsPaVO.addressbook.DettagliCorrispondente getDettagliCorrispondente(Page page, string idCorrispondente)
        {
            try
            {
                Utente utente = getUtente(page);
                if (idCorrispondente == null || idCorrispondente.Equals(""))
                    return null;
                //costruzione oggetto queryCorrispondente
                AddressbookQueryCorrispondente qco = new AddressbookQueryCorrispondente();
                qco.systemId = idCorrispondente;
                qco.getChildren = false;
                qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.GLOBALE;
                qco.idAmministrazione = utente.idAmministrazione;//ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];
                qco.idRegistri = getListaIdRegistri(page);
                qco.fineValidita = true;

                System.Data.DataSet dataSet = docsPaWS.AddressbookGetDettagliCorrispondente(qco);

                DocsPaVO.addressbook.DettagliCorrispondente result = new DocsPaVO.addressbook.DettagliCorrispondente();
                DocsPaUtils.Data.TypedDataSetManager.MakeTyped(dataSet, result.Corrispondente.DataSet);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			} 
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static Corrispondente[] getListaCorrispondentiAutorizzati(Page page, AddressbookQueryCorrispondenteAutorizzato qco)
        {
            try
            {
                Corrispondente[] result = docsPaWS.AddressbookGetListaCorrispondentiAutorizzati(qco);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			} 
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }
        public static Corrispondente[] getListaRootUO(Page page, AddressbookQueryCorrispondente qco)
        {
            try
            {
                qco.fineValidita = true;
                Corrispondente[] result = docsPaWS.AddressbookGetRootUO(qco);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			} 
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static string getDecrizioneCorrispondente(Page page, Corrispondente myCorr)
        {
            string desc = "";
            if (myCorr == null)
                return "";

            if (myCorr.GetType() == typeof(Ruolo))
            {
                //				DocsPaWR.AddressbookQueryCorrispondente qco =new AddressbookQueryCorrispondente();
                //				qco.codiceRubrica=myCorr.codiceRubrica;
                //				
                //				qco.idAmministrazione=myCorr.idAmministrazione;
                //				switch (myCorr.tipoIE) 
                //				{
                //					case "I": qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO; break;
                //					case "E": qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO; break;
                //					default: qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE; break;					
                //				}

                UnitaOrganizzativa corrUO;
                string descrUO = "";
                //				DocsPaWR.Ruolo ruo=(DocsPAWA.DocsPaWR.Ruolo) UserManager.getListaCorrispondenti(page,qco)[0];
                //				if(ruo!=null)
                //				{
                Ruolo corrRuolo = (Ruolo)myCorr;
                descrUO = "";
                corrUO = corrRuolo.uo;
                while (corrUO != null)
                {
                    descrUO = descrUO + " - " + corrUO.descrizione;
                    corrUO = corrUO.parent;
                }

                desc = corrRuolo.descrizione + descrUO;
                //				} 
                //				else
                //				{ 
                //					desc = myCorr.descrizione;
                //				}
            }
            else
            {
                desc = myCorr.descrizione;
            }

            return desc;
        }

        /// <summary>
        /// da la descrizione del corr, ma senza dettaglio delle uo.
        /// </summary>
        /// <param name="myCorr"></param>
        /// <returns></returns>
        public static string getDecrizioneCorrispondenteSemplice(Corrispondente myCorr)
        {
            string desc = "";
            if (myCorr == null)
                return "";

            //			if (myCorr.GetType() == typeof(Ruolo)) 
            //			{
            //				Ruolo corrRuolo = (Ruolo) myCorr;
            //				string descrUO = "";				
            //				UnitaOrganizzativa corrUO;
            //				corrUO = corrRuolo.uo;
            //
            //				while(corrUO!=null) 
            //				{
            //					descrUO = descrUO + " - " + corrUO.descrizione;
            //					corrUO = corrUO.parent;
            //				}
            //					
            //				desc = corrRuolo.descrizione + descrUO;
            //			} 
            //			else 
            desc = myCorr.descrizione;

            return desc;
        }

        public static bool esisteCorrispondente(Corrispondente[] lista, Corrispondente corr)
        {
            if (corr.systemId != null)
            {
                if (lista != null)
                {
                    for (int i = 0; i < lista.Length; i++)
                    {
                        if (lista[i] != null && lista[i].systemId != null)
                            if (lista[i].systemId.Equals(corr.systemId))
                                return true;
                    }
                }
            }
            else
            {
                if (lista != null)
                {
                    for (int i = 0; i < lista.Length; i++)
                    {




                        if (lista[i].descrizione.ToUpper().Equals(corr.descrizione.ToUpper()))
                            return true;
                    }
                }
            }
            return false;
        }



        public static Corrispondente[] removeCorrispondente(Corrispondente[] lista, int index)
        {
            if (lista == null || lista.Length < index)
                return lista;

            if (lista.Length == 1)
                return null;

            Corrispondente[] nuovaLista = null;
            if (lista != null && lista.Length > 0)
            {
                for (int i = 0; i < lista.Length; i++)
                {
                    if (i != index)
                        nuovaLista = addCorrispondente(nuovaLista, lista[i]);
                }
            }
            return nuovaLista;
        }

        public static Corrispondente[] addCorrispondente(Corrispondente[] lista, Corrispondente corr)
        {
            Corrispondente[] nuovaLista;
            if (lista != null)
            {
                //Per le liste di ditribuzione
                if (corr.tipoCorrispondente == "L")
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    ArrayList lsCorr = UserManager.getCorrispondentiByCodLista(new Page(), corr.codiceRubrica,idAmm);
                    //					int len = lista.Length;
                    //					nuovaLista = new Corrispondente[len + lsCorr.Count];
                    //					lista.CopyTo(nuovaLista,0);
                    //					for(int i=0; i<lsCorr.Count; i++)
                    //					{
                    //						Corrispondente c = (Corrispondente) lsCorr[i];
                    //						nuovaLista[len + i] = c;						
                    //					}
                    System.Object[] l_objects = new System.Object[0];


                    for (int i = 0; i < lsCorr.Count; i++)
                    {
                        Corrispondente c = (Corrispondente)lsCorr[i];

                        if (!UserManager.esisteCorrispondente(lista, c))
                        {
                            l_objects = Utils.addToArray(l_objects, c);
                        }
                    }

                    nuovaLista = new Corrispondente[l_objects.Length + lista.Length];
                    lista.CopyTo(nuovaLista, 0);
                    l_objects.CopyTo(nuovaLista, lista.Length);

                }
                else
                {
                    int len = lista.Length;
                    nuovaLista = new Corrispondente[len + 1];
                    lista.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = corr;
                }
            }
            else
            {
                //Per le liste di ditribuzione
                if (corr.tipoCorrispondente == "L")
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    ArrayList lsCorr = UserManager.getCorrispondentiByCodLista(new Page(), corr.codiceRubrica,idAmm);

                    //					nuovaLista = new Corrispondente[lsCorr.Count];
                    //					for(int i=0; i<lsCorr.Count; i++)
                    //					{
                    //						Corrispondente c = (Corrispondente) lsCorr[i];
                    //						nuovaLista[i] = c;
                    //					}	

                    System.Object[] l_objects = new System.Object[0];

                    for (int i = 0; i < lsCorr.Count; i++)
                    {
                        Corrispondente c = (Corrispondente)lsCorr[i];

                        if (!UserManager.esisteCorrispondente(lista, c))
                        {
                            l_objects = Utils.addToArray(l_objects, c);
                        }
                    }

                    nuovaLista = new Corrispondente[lsCorr.Count];
                    l_objects.CopyTo(nuovaLista, 0);

                }
                else
                {
                    nuovaLista = new Corrispondente[1];
                    nuovaLista[0] = corr;
                }



            }

            return nuovaLista;
        }

        public static Corrispondente[] addCorrispondenteModificato(Corrispondente[] lista, Corrispondente[] listaCC, Corrispondente corr)
        {
            Corrispondente[] nuovaLista;
            if (lista != null)
            {
                //Per le liste di ditribuzione
                if (corr.tipoCorrispondente == "L")
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    ArrayList lsCorr = UserManager.getCorrispondentiByCodLista(new Page(), corr.codiceRubrica,idAmm);

                    System.Object[] l_objects = new System.Object[0];


                    for (int i = 0; i < lsCorr.Count; i++)
                    {
                        Corrispondente c = (Corrispondente)lsCorr[i];

                        if (!UserManager.esisteCorrispondente(lista, c) && (!UserManager.esisteCorrispondente(listaCC, c)))
                        {
                            l_objects = Utils.addToArray(l_objects, c);
                        }
                    }

                    nuovaLista = new Corrispondente[l_objects.Length + lista.Length];
                    lista.CopyTo(nuovaLista, 0);
                    l_objects.CopyTo(nuovaLista, lista.Length);

                }
                else
                {
                    int len = lista.Length;
                    nuovaLista = new Corrispondente[len + 1];
                    lista.CopyTo(nuovaLista, 0);
                    nuovaLista[len] = corr;
                }
            }
            else
            {
                //Per le liste di ditribuzione
                if (corr.tipoCorrispondente == "L")
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    ArrayList lsCorr = UserManager.getCorrispondentiByCodLista(new Page(), corr.codiceRubrica,idAmm);
                    System.Object[] l_objects = new System.Object[0];

                    for (int i = 0; i < lsCorr.Count; i++)
                    {
                        Corrispondente c = (Corrispondente)lsCorr[i];

                        if (!UserManager.esisteCorrispondente(lista, c) && !UserManager.esisteCorrispondente(listaCC, c))
                        {
                            l_objects = Utils.addToArray(l_objects, c);
                        }
                    }

                    nuovaLista = new Corrispondente[l_objects.Length];
                    l_objects.CopyTo(nuovaLista, 0);

                }
                else
                {
                    nuovaLista = new Corrispondente[1];
                    nuovaLista[0] = corr;
                }
            }


            return nuovaLista;
        }

        /// <summary>
        /// Usato nel caso di protocollo interno per determinare se un ruolo appartenente a una lista
        /// è autorizzato sul  registro del protocollo
        /// </summary>
        /// <param name="idRegistro">systemId del registro della scheda documento corrente</param>
        /// <returns></returns>
        public static bool VerificaAutorizzazioneRuoloSuRegistro(Page page, string idRegistro, string idRuolo)
        {
            bool result = false;
            try
            {
                if (idRegistro != null && idRegistro != "")
                {
                    DocsPaWR.Registro[] RegRuoloSel = UserManager.GetRegistriByRuolo(page, idRuolo);
                    foreach (DocsPAWA.DocsPaWR.Registro reg in RegRuoloSel)
                    {
                        if (idRegistro == reg.systemId)
                        {
                            result = true;
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
            return result;
        }


        #region Gestione corrispondente per ricerca
        public static void SetIdCorrispondenteMultiDestinatario(Page page, String idCorrispondente)
        {
            page.Session["rubrica.SetIdCorrispondenteMultiDestinatario"] = idCorrispondente;
 
        }

        public static String GetIdCorrispondenteMultiDestinatario(Page page)
        {
            object id = page.Session["rubrica.SetIdCorrispondenteMultiDestinatario"];
            return id != null ? id.ToString() : String.Empty;

        }

        public static void setCorrispondenteSelezionato(Page page, Corrispondente corrispondente)
        {
            page.Session["rubrica.corrispondenteSelezionato"] = corrispondente;
        }

        public static void setCorrispondenteSelezionatoSottoposto(Page page, Corrispondente corrispondente)
        {
            page.Session["rubrica.corrispondenteSelezionatoSottoposto"] = corrispondente;
        }

        public static void setCorrispondenteSelezionatoRuoloSottoposto(Page page, Corrispondente corrispondente)
        {
            page.Session["rubrica.corrispondenteSelezionatoRuoloSottoposto"] = corrispondente;
        }

        public static void setCorrispondenteSelezionatoRuoloAmministrazione(Page page, Corrispondente corrispondente)
        {
            page.Session["rubrica.corrispondenteSelezionatoRuoloAmministrazione"] = corrispondente;
        }

        public static void setCorrispondenteSelezionatoUOAmministrazione(Page page, Corrispondente corrispondente)
        {
            page.Session["rubrica.corrispondenteSelezionatoUOAmministrazione"] = corrispondente;
        }

        public static void setCorrispondenteReferenteSelezionato(Page page, Corrispondente corrispondente)
        {
            page.Session["rubrica.corrispondenteReferenteSelezionato"] = corrispondente;
        }

        public static Corrispondente getCorrispondenteSelezionato(Page page)
        {
            return (Corrispondente)page.Session["rubrica.corrispondenteSelezionato"];
        }

        public static Corrispondente getCorrispondenteSelezionatoSottoposto(Page page)
        {
            return (Corrispondente)page.Session["rubrica.corrispondenteSelezionatoSottoposto"];
        }

        public static Corrispondente getCorrispondenteSelezionatoRuoloSottoposto(Page page)
        {
            return (Corrispondente)page.Session["rubrica.corrispondenteSelezionatoRuoloSottoposto"];
        }

        public static Corrispondente getCorrispondenteSelezionatoRuoloAmministrazione(Page page)
        {
            return (Corrispondente)page.Session["rubrica.corrispondenteSelezionatoRuoloAmministrazione"];
        }

        public static Corrispondente getCorrispondenteSelezionatoUOAmministrazione(Page page)
        {
            return (Corrispondente)page.Session["rubrica.corrispondenteSelezionatoUOAmministrazione"];
        }

        public static Corrispondente getCorrispondenteSelezionatoSottopostoNoRubr(Page page)
        {
            return (Corrispondente)page.Session["rubrica.corrispondenteSelezionatoSottopostoNoRubr"];
        }

        public static Corrispondente getCorrispondenteReferenteSelezionato(Page page)
        {
            return (Corrispondente)page.Session["rubrica.corrispondenteReferenteSelezionato"];
        }

        public static void setCorrispondenteIntSelezionato(Page page, Corrispondente corrispondente)
        {
            page.Session["rubrica.corrispondenteIntSelezionato"] = corrispondente;
        }

        public static Corrispondente getCorrispondenteIntSelezionato(Page page)
        {
            return (Corrispondente)page.Session["rubrica.corrispondenteIntSelezionato"];
        }

        public static void setCorrispondenteSelezionatoSottopostoNoRubr(Page page, Corrispondente corrispondente)
        {
            page.Session["rubrica.corrispondenteSelezionatoSottopostoNoRubr"] = corrispondente;
        }

        public static void setCorrispondenteSelezionatoRuoloSottopostoNoRubr(Page page, Corrispondente corrispondente)
        {
            page.Session["rubrica.corrispondenteSelezionatoRuoloSottopostoNoRubr"] = corrispondente;
        }

        public static void removeCorrispondentiSelezionati(Page page)
        {
            page.Session.Remove("rubrica.corrispondenteSelezionato");
            page.Session.Remove("rubrica.corrispondenteIntSelezionato");
            page.Session.Remove("rubrica.corrispondenteReferenteSelezionato");
            page.Session.Remove("rubrica.corrispondenteSelezionatoSottoposto");
            page.Session.Remove("rubrica.corrispondenteSelezionatoSottopostoNoRubr");
            page.Session.Remove("rubrica.corrispondenteSelezionatoRuoloSottoposto");
            page.Session.Remove("rubrica.corrispondenteSelezionatoRuoloSottopostoNoRubr");
            page.Session.Remove("rubrica.corrispondenteSelezionatoRuoloAmministrazione");
            page.Session.Remove("rubrica.corrispondenteSelezionatoUOAmministrazione");
        }
        #endregion

        #endregion

        #region Report Corrispondenti
        public static DocsPAWA.DocsPaWR.FileDocumento reportCorrispondenti(Page page, string tipo, DocsPAWA.DocsPaWR.Registro registro)
        {
            try
            {
                //costruzione oggetto queryCorrispondente
                AddressbookQueryCorrispondente qco = new AddressbookQueryCorrispondente();
                qco.fineValidita = true;
                InfoUtente infoUtente = UserManager.getInfoUtente(page);
                switch (tipo)
                {
                    case "I": qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO; break;
                    case "E": qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO; break;
                    default: qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE; break;
                }
                qco.idAmministrazione = infoUtente.idAmministrazione;
                if (registro != null)
                {
                    qco.idRegistri = new String[1];
                    qco.idRegistri[0] = registro.systemId;
                }
                else
                    qco.idRegistri = getListaIdRegistri(page);

                DocsPaWR.FileDocumento result = docsPaWS.ReportCorrispondenti(qco, infoUtente);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			}
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }
        #endregion Report Corrispondenti

        #region Gestione Creatore
        public static void setCreatoreSelezionato(Page page, Corrispondente corrispondente)
        {
            page.Session["rubrica.creatoreSelezionato"] = corrispondente;
        }

        public static Corrispondente getCreatoreSelezionato(Page page)
        {
            return (Corrispondente)page.Session["rubrica.creatoreSelezionato"];
        }

        public static void removeCreatoreSelezionato(Page page)
        {
            page.Session.Remove("rubrica.creatoreSelezionato");
        }
        #endregion

        #region Gestione Proprietario
        public static void SetProprietarioSelezionato(Page page, Corrispondente corrispondente)
        {
            page.Session["rubrica.proprietarioSelezionato"] = corrispondente;
        }

        public static Corrispondente GetProprietarioSelezionato(Page page)
        {
            return (Corrispondente)page.Session["rubrica.proprietarioSelezionato"];
        }

        public static void RemoveProprietarioSelezionato(Page page)
        {
            page.Session.Remove("rubrica.proprietarioSelezionato");
        }
        #endregion
        //public static DocsPAWA.DocsPaWR.Corrispondente addressbookInsertCorrispondente(Page page, DocsPaWR.Corrispondente newCorr, DocsPaWR.Corrispondente parentCorr, InfoUtente um)
        //{
        //    try
        //    {
        //        DocsPaWR.Corrispondente result = docsPaWS.AddressbookInsertCorrispondente(newCorr, parentCorr, um);
        //        return result;
        //    }
        //    catch (Exception es)
        //    {
        //        ErrorManager.redirect(page, es);
        //    }
        //    return null;
        //}


        public static DocsPAWA.DocsPaWR.Corrispondente addressbookInsertCorrispondente(Page page, DocsPaWR.Corrispondente newCorr, DocsPaWR.Corrispondente parentCorr)
        {
            try
            {
                InfoUtente iu = getInfoUtente(page);
                DocsPaWR.Corrispondente result = docsPaWS.AddressbookInsertCorrispondente(newCorr, parentCorr, iu);
                /*
                                if(result == null)
                                {
                                    throw new Exception();
                                }
                */
                return result;
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			} 
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }


        public static DocsPAWA.DocsPaWR.Ruolo[] getListaRuoliSup(Page page, DocsPAWA.DocsPaWR.Ruolo ruolo)
        {
            //DocsPaWR.Ruolo[] ruoli;
            try
            {
                DocsPaWR.Ruolo[] result = docsPaWS.AddressbookGetRuoliSuperioriInUO(ruolo);

                if (result == null)
                {
                    throw new Exception();
                }

                return result;
            }
            //			catch(System.Web.Services.Protocols.SoapException es) 
            //			{
            //				ErrorManager.redirect(page, es);
            //			} 
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }



        public static DocsPAWA.DocsPaWR.Ruolo[] getRuoliRiferimentoAutorizzati(Page page, DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca, DocsPAWA.DocsPaWR.UnitaOrganizzativa uo)
        {
            //DocsPaWR.Ruolo[] ruoli;
            try
            {
                DocsPaWR.Ruolo[] result = docsPaWS.AddressbookGetRuoliRiferimentoAutorizzati(qca, uo);

                //				if(result == null)
                //				{
                //					throw new Exception();
                //				}

                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }

            return null;
        }

        public static Corrispondente getCorrispondenteBySystemID(Page page, string systemID)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteBySystemId(systemID);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }
        }

        public static Corrispondente getCorrispondenteBySystemIDDisabled(Page page, string systemID)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteBySystemIdDisabled(systemID);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }
        }

        // elisa ruoli utente interni

        public static DataSet GetRuoliUtenteInt(Page page, string codRubrica)
        {
            DataSet ds = new DataSet();
            try
            {
                return ds = docsPaWS.AddressbookGetRuoliUtenteInt(codRubrica);

            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }

        }

        public static ElementoRubrica[] GetRuoliUtente(Page page, string id_amm, string cod_rubrica)
        {
            try
            {
                return docsPaWS.AddressbookGetRuoliUtente(id_amm, cod_rubrica);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }

        }

        public static Ruolo[] GetRuoliUtente(Page page, string idPeople)
        {
            try
            {
                return docsPaWS.GetListaRuoliUtente(idPeople);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }

        }

        public static Ruolo[] GetRuoliUtenteByIdCorr(Page page, string idCorr)
        {
            try
            {
                return docsPaWS.GetListaRuoliUtenteByIdCorr(idCorr);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static Registro[] GetRegistriByRuolo(Page page, string idRuolo)
        {
            try
            {
                return docsPaWS.GetListaRegistriByRuolo(idRuolo);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }

        }

        #region gestione firmatari
        public static Firmatario[] removeFirmatari(Firmatario[] lista, int index)
        {
            if (lista == null || lista.Length < index)
                return lista;

            if (lista.Length == 1)
                return null;

            Firmatario[] nuovaLista = null;
            if (lista != null && lista.Length > 0)
            {
                for (int i = 0; i < lista.Length; i++)
                {
                    if (i != index)
                        nuovaLista = addFirmatario(nuovaLista, lista[i]);
                }
            }
            return nuovaLista;
        }

        public static Firmatario[] addFirmatario(Firmatario[] lista, Firmatario firmat)
        {
            Firmatario[] nuovaLista;
            if (lista != null)
            {
                int len = lista.Length;
                nuovaLista = new Firmatario[len + 1];
                lista.CopyTo(nuovaLista, 0);
                nuovaLista[len] = firmat;
            }
            else
            {
                nuovaLista = new Firmatario[1];
                nuovaLista[0] = firmat;
            }
            return nuovaLista;
        }

        public static bool esisteFirmatario(Firmatario[] lista, Firmatario firmat)
        {
            if (firmat.cognome != null)
            {
                if (lista != null)
                {
                    string f_nomeCompleto = firmat.nome.ToUpper() + "-" + firmat.cognome.ToUpper();
                    for (int i = 0; i < lista.Length; i++)
                    {
                        if (lista[i] != null)
                        {
                            string l_nomeCompleto = lista[i].nome.ToUpper() + "-" + lista[i].cognome.ToUpper();
                            if (l_nomeCompleto.Equals(f_nomeCompleto))
                                return true;
                        }
                    }
                }
            }
            return false;
        }


        #endregion


        public static ElementoRubrica[] filtra_trasmissioniPerListe(Page page, ParametriRicercaRubrica qco, DocsPAWA.DocsPaWR.ElementoRubrica[] ers)
        {
            try
            {
                return docsPaWS.filtra_trasmissioniPerListe(qco, UserManager.getInfoUtente(page), ers);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static Corrispondente[] getListaCorrispondentiSemplice(Page page)
        {
            try
            {
                return docsPaWS.addressbookGetListaCorrispondentiSemplice();
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static string[] getUtenteInternoAOO(string idPeople, string systemIdRegistro, Page page)
        {
            try
            {
                return docsPaWS.getUtenteInternoAOO(idPeople, systemIdRegistro, UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;

        }



        #region solleciti alle trasmissioni effettuate

        /// <summary>
        /// Invia solleciti alle trasmissioni effettuate
        /// </summary>
        /// <param name="trasm"></param>
        /// <param name="page"></param>
        public static void sendSollecito(DocsPAWA.DocsPaWR.Trasmissione trasm, Page page)
        {
            //DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);

            //passo la path del front end per il link del documento nella email di sollecito che sarà inviata
            string path = Utils.getHttpFullPath(page);

            try
            {
                //docsPaWS.trasmissioniSendSollecito(path, trasm, infoUtente);		
                docsPaWS.trasmissioniSendSollecito(path, trasm);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
        }

        #endregion

        #region Nuova rubrica
        public static DocsPAWA.DocsPaWR.Corrispondente getCorrispondenteByCodRubrica(Page page, string codice)
        {
            return getCorrispondenteByCodRubrica(page, codice, false);
        }
        
        public static DocsPAWA.DocsPaWR.Corrispondente getCorrispondenteByCodRubrica(Page page, string codice, bool storicizzato)
        {
            try
            {
                DocsPaWR.Registro[] regAll = UserManager.getListaRegistriWithRF(UserManager.getRuolo().systemId, "", "");
                string condRegistri = string.Empty;
                if (regAll != null && regAll.Length > 0)
                {
                    condRegistri = " and (id_registro in (";
                    foreach (DocsPaWR.Registro reg in regAll)
                        condRegistri += reg.systemId + ",";
                    condRegistri = condRegistri.Substring(0, condRegistri.Length - 1);
                    condRegistri += ") OR id_registro is null)";
                }

                return docsPaWS.AddressbookGetCorrispondenteByCodRubrica(codice, UserManager.getInfoUtente(page), condRegistri, storicizzato);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static DocsPAWA.DocsPaWR.Corrispondente getCorrispondenteByCodRubricaIE(Page page, string codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente tipoIE)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteByCodRubricaIE(codice, tipoIE, UserManager.getInfoUtente());
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }
        /// <summary>
        /// ritorna un oggetto null, se il corrispondente non osiste o è disabilitato
        /// </summary>
        /// <param name="page"></param>
        /// <param name="codice"></param>
        /// <param name="tipoIE"></param>
        /// <returns></returns>
        public static DocsPAWA.DocsPaWR.Corrispondente getCorrispondenteByCodRubricaIENotdisabled(Page page, string codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente tipoIE)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteByCodRubricaIENotDisabled(codice, tipoIE, UserManager.getInfoUtente());
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }
        public static void check_children_existence(Page page, ref ElementoRubrica[] ers)
        {
            try
            {
                docsPaWS.rubricaCheckChildrenExistence(ref ers, UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
        }

        public static void check_children_existence(Page page, ref ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti)
        {
            try
            {
                docsPaWS.rubricaCheckChildrenExistenceEx(ref ers, checkUo, checkRuoli, checkUtenti, UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
        }

        public static ElementoRubrica[] getElementiRubrica(Page page, ParametriRicercaRubrica qco)
        {
            try
            {
                return getElementiRubrica(page, qco, null);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static ElementoRubrica[] getElementiRubrica(Page page, ParametriRicercaRubrica qco, DocsPAWA.DocsPaWR.SmistamentoRubrica smistamentoRubrica)
        {
            try
            {
                return docsPaWS.rubricaGetElementiRubrica(qco, UserManager.getInfoUtente(page), smistamentoRubrica);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static ElementoRubrica getElementoRubrica(Page page, string cod, string condRegistri)
        {
            try
            {
                return getElementoRubrica(page, cod, null, condRegistri);
                //docsPaWS.rubricaGetElementoRubrica(cod, UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static ElementoRubrica getElementoRubrica(Page page, string cod)
        {
            try
            {
                return getElementoRubrica(page, cod, null, null);
                //docsPaWS.rubricaGetElementoRubrica(cod, UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }


        public static ElementoRubrica getElementoRubrica(Page page, string cod, DocsPAWA.DocsPaWR.SmistamentoRubrica smistaRubrica, string condregistri)
        {
            try
            {
                return docsPaWS.rubricaGetElementoRubrica(cod, UserManager.getInfoUtente(page), smistaRubrica, condregistri);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static ElementoRubrica getElementoRubricaSimple(Page page, string cod)
        {
            try
            {
                return docsPaWS.rubricaGetElementoRubricaSimple(cod, UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }
        public static void setQueryRubricaCaller(ref DocsPAWA.DocsPaWR.ParametriRicercaRubrica qco)
        {
            qco.caller = new DocsPAWA.DocsPaWR.RubricaCallerIdentity();
            System.Web.HttpContext ctx = System.Web.HttpContext.Current;

            if (ctx.Session["userRuolo"] != null)
            {
                qco.caller.IdRuolo = ((DocsPAWA.DocsPaWR.Ruolo)ctx.Session["userRuolo"]).systemId;
            }

            if (ctx.Session["userData"] != null)
            {
                qco.caller.IdUtente = ((Utente)ctx.Session["userData"]).systemId;

               if (!string.IsNullOrEmpty(((Utente)ctx.Session["userData"]).idPeople))
                    qco.caller.IdPeople = ((Utente)ctx.Session["userData"]).idPeople;
            }
         
            //se richiamo la rubrica Gestione Rubrica in testata320
            if (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_MANAGE
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE)
            {
              
                qco.caller.IdRegistro = null;

            }
            else
            {
                if (ctx.Session["userRegistro"] != null)
                {
                    qco.caller.IdRegistro = ((Registro)ctx.Session["userRegistro"]).systemId;
                }

            }      
        }

        public static Ruolo[] getListaRuoliInUO(Page page, DocsPaWR.UnitaOrganizzativa uo, DocsPAWA.DocsPaWR.InfoUtente utente)
        {
            try
            {
                //				throw new NotImplementedException ("getListaRuoliInUO");
                return docsPaWS.amministrazioneGetRuoliInUO(uo, utente);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception ex) { throw ex; }
            return null;
        }

        public static ElementoRubrica[] getElementiRubricaRange(string[] codici, DocsPAWA.DocsPaWR.AddressbookTipoUtente tipoIE, Page page)
        {
            try
            {
                return docsPaWS.rubricaGetElementiRubricaRange(codici, tipoIE, UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static ElementoRubrica[] getElementiRubricaRangeSysID(string[] codici, Page page)
        {
            try
            {
                return docsPaWS.rubricaGetElementiRubricaRangeCodSysID(codici, UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static ElementoRubrica[] getGerarchiaElemento(string codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente tipoIE, Page page)
        {
            try
            {
                return getGerarchiaElemento(codice, tipoIE, page, null);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static ElementoRubrica[] getGerarchiaElemento(string codice, DocsPAWA.DocsPaWR.AddressbookTipoUtente tipoIE, Page page, DocsPAWA.DocsPaWR.SmistamentoRubrica smistamentoRubrica)
        {
            try
            {
                return docsPaWS.rubricaGetGerarchiaElemento(codice, tipoIE, UserManager.getInfoUtente(page), smistamentoRubrica);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static ElementoRubrica[] getRootItems(DocsPAWA.DocsPaWR.AddressbookTipoUtente tipoIE, Page page)
        {
            try
            {
                return getRootItems(tipoIE, page, null);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static ElementoRubrica[] getRootItems(DocsPAWA.DocsPaWR.AddressbookTipoUtente tipoIE, Page page, DocsPAWA.DocsPaWR.SmistamentoRubrica smistamentoRubrica)
        {
            try
            {
                return docsPaWS.rubricaGetRootItems(tipoIE, UserManager.getInfoUtente(page), smistamentoRubrica);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }


        public static bool UoIsOnExternalReg(string cod_uo, string id_reg, string id_amm, Page page)
        {
            try
            {
                return docsPaWS.UoIsOnExternalReg(cod_uo, id_reg, id_amm, UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return false;
        }

        public static string[] GetUoInterneAoo(Page page)
        {
            try
            {
                string id_reg = UserManager.getIdRegistroSelezionato(page);
                return docsPaWS.GetUoInterneAoo(id_reg, UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }


        #endregion

        public static string getNomeRF(Page page, string codiceRF)
        {
            try
            {
                return docsPaWS.getNomeRF(codiceRF);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static string getSystemIdRF(Page page, string codiceRF)
        {
            try
            {
                return docsPaWS.getSystemIdRF(codiceRF);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        #region ListeDistribuzione
        public static string getSystemIdLista(Page page, string codiceLista)
        {
            try
            {
                return docsPaWS.getSystemIdLista(codiceLista);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static string getNomeLista(Page page, string codiceLista,string idAmm)
        {
            try
            {
                return docsPaWS.getNomeLista(codiceLista,idAmm);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static DataSet getListeDistribuzione(Page page)
        {
            try
            {
                return docsPaWS.getListeDistribuzioneUt(UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static DataSet getListePerModificaUt(Page page)
        {
            try
            {
                return docsPaWS.getListePerModificaUt(UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static DataSet getListePerRuoloUt(Page page)
        {
            try
            {
                return docsPaWS.getListePerRuoloUt(UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static void deleteListaDistribuzione(Page page, string codiceLista)
        {
            try
            {
                docsPaWS.deleteListaDistribuzione(codiceLista);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
        }

        public static string getCodiceLista(Page page, string idLista)
        {
            try
            {
                return docsPaWS.getCodiceLista(idLista);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static string getRuoloOrUserLista(Page page, string idLista)
        {
            try
            {
                return docsPaWS.getRuoloOrUserLista(idLista);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static DataSet getCorrispondentiLista(Page page, string codiceLista)
        {
            try
            {
                return docsPaWS.getCorrispondentiLista(codiceLista);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static void deleteCorrListaDistribuzione(Page page, string codiceCorr)
        {
            try
            {
                docsPaWS.deleteCorrListaDistribuzione(codiceCorr);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
        }

        public static DataSet ricercaCorrispondentiLista(Page page, string p_ricercaDescrizione)
        {
            try
            {
                return docsPaWS.ricercaCorrispondentiLista(p_ricercaDescrizione);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static void salvaLista(Page page, DataSet dsCorrLista, string nomeLista, string codiceLista, string idUtente, string idAmm, string gruppo)
        {
            try
            {
                InfoUtente infoUtente = UserManager.getInfoUtente();
                docsPaWS.salvaListaGruppo(dsCorrLista, nomeLista, codiceLista, idUtente, idAmm, gruppo, infoUtente);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
        }

        public static bool isUniqueCodLista(Page page, string codLista, string idAmm)
        {
            try
            {
                return docsPaWS.isUniqueCodLista(codLista, idAmm);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
                return false;
            }
            catch (Exception) { return false; }
        }


        public static bool isUniqueNomeLista(Page page, string nomeLista, string idAmm)
        {
            try
            {
                return docsPaWS.isUniqueNomeLista(nomeLista, idAmm);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
                return false;
            }
            catch (Exception) { return false; }
        }

        public static void modificaLista(Page page, DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista)
        {
            try
            {
                docsPaWS.modificaLista(dsCorrLista, idLista, nomeLista, codiceLista);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
        }

        public static void modificaListaUser(Page page, DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista, string idUtente)
        {
            try
            {
                docsPaWS.modificaListaUser(dsCorrLista, idLista, nomeLista, codiceLista, idUtente);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
        }

        public static void modificaListaGruppo(Page page, DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista, string idGruppo)
        {
            try
            {
                docsPaWS.modificaListaGruppo(dsCorrLista, idLista, nomeLista, codiceLista, idGruppo);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
        }

        public static void modificaLista(Page page, DataSet dsCorrLista, string idLista)
        {
            try
            {
                docsPaWS.modificaListaCorr(dsCorrLista, idLista);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
        }

        public static ArrayList getCorrispondentiByCodLista(Page page, string codiceLista, string idAmm)
        {
            try
            {
                InfoUtente infoUtente;
                try
                {
                    infoUtente = UserManager.getInfoUtente();
                }
                catch (Exception e)
                {
                    SessionManager sm = new SessionManager();
                    infoUtente = sm.getUserAmmSession();
                    string[] amministrazione = ((string)HttpContext.Current.Session["AMMDATASET"]).Split('@');
                    string codiceAmministrazione = amministrazione[0];
                    infoUtente.idAmministrazione = new DocsPaWebService().getIdAmmByCod(codiceAmministrazione);
 
                }

                return new ArrayList(docsPaWS.getCorrispondentiByCodLista(codiceLista,idAmm, infoUtente));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }
        #endregion ListeDistribuzione

        public static ArrayList getCorrispondentiByCodRF(Page page, string codiceRF)
        {
            try
            {
                return new ArrayList(docsPaWS.getCorrispondentiByCodRF(codiceRF));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }


        public static string getCodRFFromSysIdCorrGlob(string systemId)
        {
            try
            {
                return docsPaWS.getCodRFFromSysIdCorrGlob(systemId);
            }
            catch (Exception e) { }
            return null;
        }

        #region ModelliTrasmissione

        public static ArrayList getModelliUtente(Page page, DocsPAWA.DocsPaWR.Utente utente, DocsPAWA.DocsPaWR.InfoUtente infoUt, DocsPaWR.FiltroRicerca[] filtriRicerca)
        {
            try
            {
                //Veronica
                return new ArrayList(docsPaWS.getModelliUtente(utente, infoUt, filtriRicerca));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static DocsPAWA.DocsPaWR.ModelloTrasmissione getModelloByID(Page page, string idAmm, string idModello)
        {
            try
            {
                return docsPaWS.getModelloByID(idAmm, idModello);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }
        


        public static void cancellaModello(Page page, string idAmministrazione, string idModello)
        {
            try
            {
                docsPaWS.CancellaModello(idAmministrazione, idModello);
            }
            catch
            {

            }
        }

        #endregion

        #region VisibilitàDocumenti
        public static int getIdProfileByData(DocsPAWA.DocsPaWR.InfoUtente infoUtente, string numProto, string AnnoProto, string idRegistro, out string inArchivio)
        {
            try
            {
                return docsPaWS.DO_getIdProfileByData(numProto, AnnoProto, idRegistro, infoUtente, out inArchivio);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

        }
        public static string removeVisibilita(Page page, string docNumber, DocsPaWR.Corrispondente corr)
        {
            string result = null;
            try
            {

                result = docsPaWS.RemoveVisibilita(docNumber, corr, UserManager.getInfoUtente());
                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }
        #endregion

        #region MODIFICA/CANCELLAZIONE CORRISPONDENTE ESTERNO

        public static bool DeleteModifyCorrispondenteEsterno(Page page, DocsPAWA.DocsPaWR.DatiModificaCorr datiModifica, int flagListe, string action, out string message)
        {
            message = String.Empty;
            try
            {
                return docsPaWS.CorrispondentiDeleteModifyCorrispondenteEsterno(UserManager.getInfoUtente(page), datiModifica, flagListe, action, out message);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
                return false;
            }
            catch (Exception) { return false; }
        }

        public static bool DeleteModifyCorrispondenteEsterno(Page page, DocsPAWA.DocsPaWR.DatiModificaCorr datiModifica, int flagListe, string action,out string newIdCorr, out string message)
        {
            message = String.Empty;
            newIdCorr = String.Empty;
            try
            {
                return docsPaWS.CorrispondentiDeleteModifyCorrispondenteEsternoWithId(UserManager.getInfoUtente(page), datiModifica, flagListe, action, out newIdCorr, out message);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
                return false;
            }
            catch (Exception) { return false; }
        }

        #endregion

        #region gestione OGGETTI SESSIONE
        /// Impostazione valore in sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionValue"></param>
        private static void SetSessionValue(string sessionKey, object sessionValue)
        {
            System.Web.HttpContext.Current.Session[sessionKey] = sessionValue;
        }

        /// <summary>
        /// Reperimento valore da sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private static Object GetSessionValue(string sessionKey)
        {
            return System.Web.HttpContext.Current.Session[sessionKey];
        }

        /// <summary>
        /// Rimozione chiave di sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        private static void RemoveSessionValue(string sessionKey)
        {
            System.Web.HttpContext.Current.Session.Remove(sessionKey);
        }
        #endregion gestione OGGETTI SESSIONE

        /// <summary>
        /// Dato l'id dell'amministrazione, il metodo restituisce le informazioni relative all'Amministrazione 
        /// </summary>
        /// <param name="idAmm">id amministrazione</param>
        /// <returns></returns>
        public static InfoAmministrazione getInfoAmmCorrente(string idAmm)
        {
            InfoAmministrazione result = null;
            try
            {
                result = docsPaWS.AmmGetInfoAmmCorrente(idAmm);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception exception)
            {
                throw new Exception();
            }

            return result;
        }

        /// <summary>
        /// Metodo per reperire registri/RF in amministrazione
        /// </summary>
        /// <param name="codAmm"></param>
        /// <param name="codChaRF">0, se voglio solo i registri, 1 se voglio solo gli RF, "" se voglio entrambi</param>
        /// <returns></returns>
        public static OrgRegistro[] getRegistriByCodAmm(string codAmm, string codChaRF)
        {
            OrgRegistro[] result = null;
            try
            {
                result = docsPaWS.AmmGetRegistri(codAmm, codChaRF);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception exception)
            {
                throw new Exception();
            }

            return result;
        }

        /// <summary>
        /// Dato un idRuolo ritorna Registri e RF o solo RF.
        /// Se all="" allora anche idAooColl deve essere = "" e il metodo ritorna sia Reg che RF
        /// Se all="0" (e idAooColl deve essere = "" ) ritorna tutti gli RF assocaiati a un determinato ruolo
        /// Se all = "1" (idAooColl deve essere popolato con una systemId di un registro) e in
        /// tal caso il metoto ritorna tutti gli Rf associati a quel registro e visibili dal ruolo
        /// corrente
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static Registro[] getListaRegistriWithRF(Page page, string all, string idAooColl)
        {
            Registro[] result = null;
            try
            {
                result = docsPaWS.UtenteGetRegistriWithRf(getInfoUtente(page).idCorrGlobali, all, idAooColl);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception exception)
            {
                ErrorManager.redirect(page, exception);
            }

            return result;
        }

        /// <summary>
        /// Dato un idRuolo ritorna Registri e RF o solo RF.
        /// Se all="" allora anche idAooColl deve essere = "" e il metodo ritorna sia Reg che RF
        /// Se all="0" (e idAooColl deve essere = "" ) ritorna tutti gli RF assocaiati a un determinato ruolo
        /// Se all = "1" (idAooColl deve essere popolato con una systemId di un registro) e in
        /// tal caso il metoto ritorna tutti gli Rf associati a quel registro e visibili dal ruolo
        /// corrente
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static Registro getRegistroDaPec(string idProfile, Page page)
        {
            Registro result = null;
            try
            {
                result = docsPaWS.GetRegistroDaPec(idProfile);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception exception)
            {
                ErrorManager.redirect(page, exception);
            }

            return result;
        }

        /// <summary>
        /// Dato un idRuolo ritorna Registri e RF o solo RF.
        /// Se all="" allora anche idAooColl deve essere = "" e il metodo ritorna sia Reg che RF
        /// Se all="0" (e idAooColl deve essere = "" ) ritorna tutti gli RF assocaiati a un determinato ruolo
        /// Se all = "1" (idAooColl deve essere popolato con una systemId di un registro) e in
        /// tal caso il metoto ritorna tutti gli Rf associati a quel registro e visibili dal ruolo
        /// corrente
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <param name="all"></param>
        /// <param name="idAooColl"></param>
        /// <returns></returns>
        public static Registro[] getListaRegistriWithRF(string idRuolo, string all, string idAooColl)
        {
            Registro[] result = null;
            try
            {
                result = docsPaWS.UtenteGetRegistriWithRf(idRuolo, all, idAooColl);

                if (result == null)
                {
                    throw new Exception();
                }
            }
            catch (Exception exception)
            {
                throw new Exception();
            }

            return result;
        }
        /// <summary>
        /// Ritorna
        /// </summary>
        /// <param name="page"></param>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public static ElementoRubrica getElementoRubricaSimpleBySystemId(Page page, string systemId)
        {
            try
            {
                return docsPaWS.rubricaGetElementoRubricaSimpleBySystemId(systemId, UserManager.getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        #region CSS 

        public string getCssAmministrazione(string idAmm)
        {
            string result = string.Empty;
            result = docsPaWS.getCssAmministrazione(idAmm);

            return result;
        }
        #endregion

        public static DocsPAWA.DocsPaWR.Corrispondente getCorrispondenteByIdPeople(Page page, string idPeople, DocsPAWA.DocsPaWR.AddressbookTipoUtente tipoIE)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteByIdPeople(idPeople, tipoIE, UserManager.getInfoUtente());
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static DocsPAWA.DocsPaWR.Corrispondente getCorrispondenteCompletoBySystemId(Page page, string systemId, DocsPAWA.DocsPaWR.AddressbookTipoUtente tipoIE)
        {
            try
            {
                return docsPaWS.AddressbookGetCorrispondenteCompletoBySystemId(systemId, tipoIE, UserManager.getInfoUtente());
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return null;
        }

        public static string getRuoloRespUoFromUo(Page page, string id_Uo, string tipoRuolo, string idCorr)
        {
            try
            {
                return docsPaWS.AddressbookGetRuoloRespUoFromUo(id_Uo, tipoRuolo, idCorr);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return "-1";
        }

        public static bool FunzioneEsistente(Page page, string codiceFunzione)
        {
            bool retValue = false;
            try
            {
                retValue = docsPaWS.FunzioneEsistente(codiceFunzione);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
            }
            catch (Exception) { }
            return retValue;
        }

        public static ElementoRubrica[] getCorrispondenteRubricaControl(Page page, string codiceRubrica, DocsPaWR.RubricaCallType callType)
        {
            try
            {
                DocsPaWR.ParametriRicercaRubrica qco = new DocsPAWA.DocsPaWR.ParametriRicercaRubrica();
                //cerco su tutti i tipi utente:
                qco.calltype = callType;
                setQueryRubricaCaller(ref qco);

                //  qco.caller.filtroRegistroPerRicerca = qco.caller.IdRegistro;

                qco.codice = codiceRubrica;
                qco.tipoIE = DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE;


                //questo serve perchè in questi casi quando si cerca un esterno all'amministrazione
                //si deve ricercare anche tra gli esterni all'amministrazione che sono creati su degli RF
                //associati al registro corrente  (nel caso di protocollo)
                if (callType == RubricaCallType.CALLTYPE_PROTO_IN
                    || callType == RubricaCallType.CALLTYPE_PROTO_IN_INT
                    || callType == RubricaCallType.CALLTYPE_PROTO_OUT
                    || callType == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                {
                    if (qco.caller.IdRegistro != null && qco.caller.IdRegistro != string.Empty)
                    {
                        DocsPaWR.Registro[] listaReg = getListaRegistriWithRF(page, "1", qco.caller.IdRegistro);

                        //Ritorna una lista di RF concatenati da una ","
                        string condReg = getCondizioneRegistro(listaReg);

                        if (condReg != null && condReg != string.Empty)
                        {
                            //se cè almeno un RF allora aggancio anche l'id registro
                            // per ricercare tra tutti gli esterni appartenenti
                            //al mio registro e agli RF ad esso associati che posso vedere
                            condReg += ", " + qco.caller.IdRegistro;
                        }
                        else
                        {
                            condReg += qco.caller.IdRegistro;
                        }
                        qco.caller.filtroRegistroPerRicerca = condReg;
                    }
                }

                //in questo caso devo ricercare i corrispondenti esterni all'amministrazione
                //tra tutti i corrispondenti che sono stati creati su registi e rf a cui
                //il ruolo corrente è associato
                if (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_MANAGE
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO
                || callType == RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE)
                {

                    //nuova gestione: devo cercare in tutti i registri e RF visibili al ruolo

                    DocsPaWR.Registro[] regRuolo = getListaRegistriWithRF(qco.caller.IdRuolo, "", "");

                    string filtroRegistro = "";
                    for (int i = 0; i < regRuolo.Length; i++)
                    {
                        filtroRegistro = filtroRegistro + regRuolo[i].systemId;
                        if (i < regRuolo.Length - 1)
                        {
                            filtroRegistro = filtroRegistro + " , ";
                        }
                    }

                    qco.caller.filtroRegistroPerRicerca = filtroRegistro;
                }

                qco.doRuoli = true;
                qco.doUtenti = true;
                qco.doUo = true;
                qco.doListe = false;

                // Abilita la ricerca in rubrica comune, qualora l'utente sia abilitato
                qco.doRubricaComune = (RubricaComune.Configurazioni.GetConfigurazioni(UserManager.getInfoUtente()).GestioneAbilitata);

                qco.queryCodiceEsatta = true;
                //DocsPaWR.Corrispondente corrRes;
                DocsPaWR.ElementoRubrica[] elSearch = UserManager.getElementiRubrica(page, qco);

                if (elSearch != null && elSearch.Length == 1)
                {

                    //corrRes = getCorrispondenteByCodRubricaIE(page, elSearch[0].codice, elSearch[0].interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);
                }
                else
                {
                    //corrRes = null;
                }

                return elSearch;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        public static DocsPAWA.DocsPaWR.Corrispondente getCorrispondenteByCodRubricaRubricaComune(Page page, string codice)
        {
            try
            {
                return docsPaWS.GetCorrRubricaComune(codice, getInfoUtente(page));
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                throw new Exception();
            }
            catch (Exception) { }
            return null;
        }

        public static bool existRf(string idAmministrazione, Page page)
        {
            try
            {
                return docsPaWS.existRf(idAmministrazione);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
                return false;
            }
        }

        public static Ruolo getRuoloById(string idCorrGlobali, Page page)
        {
            try
            {
                return docsPaWS.getRuoloById(idCorrGlobali);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }
        }

        public static bool iscorrispondenteValid(string idCor)
        {
            bool val = false;
            try
            {
                return docsPaWS.isCorrispondenteValid(idCor);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                return val;
            }
        }


        //public static DocsPaWR.ElementoRubrica[] getElementoRubricaControl(Page page, string cod, DocsPAWA.DocsPaWR.SmistamentoRubrica smistaRubrica)
        //{
        //    try
        //    {
        //        return docsPaWS.rubricaGetElementiRubricaControl(cod, UserManager.getInfoUtente(page), smistaRubrica);
        //    }
        //    catch (System.Web.Services.Protocols.SoapException es)
        //    {
        //        ErrorManager.redirect(page, es);
        //    }
        //    return null;
        //}



        //public static DocsPAWA.DocsPaWR.Corrispondente[] getCorrispondentiForUC(Page page, string codiceRubrica, bool fineValidita)
        //{
        //    try
        //    {
        //        Utente utente = getUtente(page);
        //        AddressbookQueryCorrispondente qco = new AddressbookQueryCorrispondente();
        //        qco.idRegistri = getListaIdRegistri(page);
        //        qco.codiceRubrica = codiceRubrica;
        //        qco.getChildren = false;
        //        qco.idAmministrazione = utente.idAmministrazione; //ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];					
        //        qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE;
        //        qco.fineValidita = fineValidita;
        //        DocsPaWR.Corrispondente[] result;
        //        DocsPaWR.Corrispondente[] corrispondenti = docsPaWS.AddressbookGetListaCorrispondenti(qco);
        //        if (corrispondenti.Length > 0)
        //        {
        //            result = corrispondenti;
        //        }
        //        else
        //        {
        //            result = null;
        //        }

        //        return result;
        //    }
        //    catch (Exception es)
        //    {
        //        ErrorManager.redirect(page, es);
        //    }
        //    return null;
        //}
            


        public static Ruolo getRuoloByIdGruppo(string idGruppo, Page page)
        {
            try
            {
                return docsPaWS.getRuoloByIdGruppo(idGruppo);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                ErrorManager.redirect(page, es);
                return null;
            }
        }

        public static bool isRFEnabled()
        {
            bool result = false;
            try
            {
                result = docsPaWS.isRFEnabled();
            }
            catch (Exception exception)
            {
                return result;
            }
            return result;

        }

        /// <summary>
        /// Normalizzazione valore per una proprietà stringa
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLenght"></param>
        /// <returns></returns>
        public static string normalizeStringPropertyValue(string value)
        {

            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();
                value = value.Replace("/", string.Empty);
                value = value.Replace(System.Environment.NewLine, string.Empty);
                value = value.Replace("\n", string.Empty);
                value = value.Replace("|", string.Empty);
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objOggettoTrasmesso"></param>
        /// <param name="objRuolo"></param>
        /// <param name="cercaInf"></param>
        /// <returns></returns>
      /*  private string getCorrispondenteRuoloInferiore(DocsPaVO.trasmissione.OggettoTrasm objOggettoTrasmesso, DocsPaVO.utente.Ruolo objRuolo, bool cercaInf)
        {
            string ret = null;
            int i;
                ArrayList listaRuoliInf;

                DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();

                if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoDocumento != null)
                    listaRuoliInf = gerarchia.getGerarchiaInf(objRuolo, objOggettoTrasmesso.infoDocumento.idRegistro, null, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO);
                else if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoFascicolo != null)
                    listaRuoliInf = gerarchia.getGerarchiaInf(objRuolo, objOggettoTrasmesso.infoFascicolo.idRegistro, objOggettoTrasmesso.infoFascicolo.idClassificazione, DocsPaVO.trasmissione.TipoOggetto.FASCICOLO);
                else
                    listaRuoliInf = gerarchia.getGerarchiaInf(objRuolo, null, null, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO);

                if (listaRuoliInf == null)
                {
                    Debugger.Write("Errore nella gestione delle trasmissioni (Query - getListaRuoliInf)");
                    throw new Exception("Errore in : getListaRuoliInf");
                }
                for (i = 0; i < listaRuoliInf.Count; i++)
                {
                    ret = ret + ((DocsPaVO.utente.Ruolo)listaRuoliInf[i]).systemId;
                    if (i < listaRuoliInf.Count - 1)
                    {
                        if (i % 998 == 0 && i > 0)
                        {
                            //queryString=queryString+") OR A.ID_UO IN (";
                            ret = ret + ") OR A.ID_RUOLO_IN_UO IN (";
                        }
                        else
                        {
                            ret += ", ";
                        }
                    }
                    else
                    {
                        ret += ")";
                    }
                }

            return ret;
        }*/

        public static bool verifyRegNoAOO(DocsPAWA.DocsPaWR.SchedaDocumento schedaCorrente, DocsPaWR.Registro[] userRegistri)
        {
            bool result = false;
            if (schedaCorrente != null && schedaCorrente.registro != null & schedaCorrente.protocollo != null)
            {
                if (userRegistri != null && userRegistri.Length > 0)
                {
                    if (schedaCorrente.registro != null)
                    {
                        foreach (DocsPaWR.Registro rep in userRegistri)
                        {
                            if (rep.systemId.Equals(schedaCorrente.registro.systemId))
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    result = true;
                }
            }
            if (schedaCorrente != null && schedaCorrente.registro == null)
            {
                result = true;
            }
            return result;
        }

        public static DocsPAWA.DocsPaWR.ElementoRubrica[] getElementiRubricaMultipli(Page page, string codiceRubrica, DocsPaWR.RubricaCallType callType, bool codiceEsatto)
        {
            try
            {
                DocsPaWR.ParametriRicercaRubrica qco = new DocsPAWA.DocsPaWR.ParametriRicercaRubrica();
                //cerco su tutti i tipi utente:
                qco.calltype = callType;
                setQueryRubricaCaller(ref qco);
                string tipologiaCorr = string.Empty;
                //  qco.caller.filtroRegistroPerRicerca = qco.caller.IdRegistro;
                if (codiceRubrica.StartsWith("^^^"))
                {
                    tipologiaCorr = codiceRubrica.Substring(3, 1);
                    codiceRubrica = codiceRubrica.Substring(4);
                }
                qco.codice = codiceRubrica;

                // Intervento per evitare la chiamata di mittenti(Uffici) che siano Esterni (Tipo_IE = E).
                // I mittenti dovrebbero essere solo Interni
                if (callType == RubricaCallType.CALLTYPE_PROTO_OUT_MITT
                || callType == RubricaCallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO
                )
                {
                    qco.tipoIE = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
                }
                else
                    qco.tipoIE = DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE;
                //oldCode
                //qco.tipoIE = DocsPAWA.DocsPaWR.AddressbookTipoUtente.GLOBALE;
     


                //questo serve perchè in questi casi quando si cerca un esterno all'amministrazione
                //si deve ricercare anche tra gli esterni all'amministrazione che sono creati su degli RF
                //associati al registro corrente  (nel caso di protocollo)
                if (callType == RubricaCallType.CALLTYPE_PROTO_IN
                    || callType == RubricaCallType.CALLTYPE_PROTO_IN_INT
                    || callType == RubricaCallType.CALLTYPE_PROTO_OUT
                    || callType == RubricaCallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                {
                    if (qco.caller.IdRegistro != null && qco.caller.IdRegistro != string.Empty)
                    {
                        DocsPaWR.Registro[] listaReg = getListaRegistriWithRF(page, "1", qco.caller.IdRegistro);

                        //Ritorna una lista di RF concatenati da una ","
                        string condReg = getCondizioneRegistro(listaReg);

                        if (condReg != null && condReg != string.Empty)
                        {
                            //se cè almeno un RF allora aggancio anche l'id registro
                            // per ricercare tra tutti gli esterni appartenenti
                            //al mio registro e agli RF ad esso associati che posso vedere
                            condReg += ", " + qco.caller.IdRegistro;
                        }
                        else
                        {
                            condReg += qco.caller.IdRegistro;
                        }
                        qco.caller.filtroRegistroPerRicerca = condReg;
                    }
                }

                //in questo caso devo ricercare i corrispondenti esterni all'amministrazione
                //tra tutti i corrispondenti che sono stati creati su registi e rf a cui
                //il ruolo corrente è associato
                if (qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_MANAGE
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_ESTESA
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTDEST
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_MITTINTERMEDIO
                || qco.calltype == DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO
                || callType == RubricaCallType.CALLTYPE_LISTE_DISTRIBUZIONE
                || qco.calltype == RubricaCallType.CALLTYPE_RICERCA_CORRISPONDENTE
                || qco.calltype == RubricaCallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO)
                {

                    //nuova gestione: devo cercare in tutti i registri e RF visibili al ruolo

                    DocsPaWR.Registro[] regRuolo = getListaRegistriWithRF(qco.caller.IdRuolo, "", "");

                    string filtroRegistro = "";
                    for (int i = 0; i < regRuolo.Length; i++)
                    {
                        filtroRegistro = filtroRegistro + regRuolo[i].systemId;
                        if (i < regRuolo.Length - 1)
                        {
                            filtroRegistro = filtroRegistro + " , ";
                        }
                    }

                    qco.caller.filtroRegistroPerRicerca = filtroRegistro;
                }

                if (!string.IsNullOrEmpty(tipologiaCorr))
                {
                    qco.doRuoli = false;
                    qco.doUtenti = false;
                    qco.doUo = false;
                    qco.doListe = false;
                    switch (tipologiaCorr)
                    {
                        case "R":
                            qco.doRuoli = true;
                            break;

                        case "P":
                            qco.doUtenti = true;
                            break;

                        case "U":
                            qco.doUo = true;
                            break;

                        default:
                            qco.doRuoli = true;
                            qco.doUtenti = true;
                            qco.doUo = true;
                            break;
                    }
                }
                else
                {
                    qco.doRuoli = true;
                    qco.doUtenti = true;
                    qco.doUo = true;
                    qco.doListe = false;
                }
                // Abilita la ricerca in rubrica comune, qualora l'utente sia abilitato
                qco.doRubricaComune = (RubricaComune.Configurazioni.GetConfigurazioni(UserManager.getInfoUtente()).GestioneAbilitata);

                qco.queryCodiceEsatta = codiceEsatto;
                DocsPaWR.ElementoRubrica[] elSearch = UserManager.getElementiRubrica(page, qco);

                return elSearch;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return null;
        }

        /// <summary>
        /// Metodo per il recupero della storia di un ruolo
        /// </summary>
        /// <param name="request">Request con le informazioni sul ruolo di cui recuperare la storia</param>
        /// <returns>Storia del ruolo</returns>
        /// <exception cref="Exception">Dettaglio di un errore avvenuto nel backend</exception>
        public static RoleHistoryResponse GetRoleHistory(RoleHistoryRequest request)
        {
            try
            {
                return docsPaWS.GetRoleHistory(request);

            }
            catch (Exception e)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(e);
            }
        }

        /// <summary>
        /// Metodo per il recupero delle informazioni minimali sugli utenti di un ruolo
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>Array con le informazioni di base sugli utenti del ruolo</returns>
        public static UserMinimalInfo[] GetUsersInRoleMinimalInfo(String roleId)
        {
            try
            {
                return docsPaWS.GetUsersInRoleMinimalInfo(new GetUsersInRoleMinimalInfoRequest() { RoleId = roleId }).Users;

            }
            catch (Exception e)
            {
                throw DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(e);
            }
        }

        public static String GetRoleDescriptionByIdGroup(String idGroup)
        {
            return docsPaWS.GetRoleDescriptionByIdGroup(idGroup);
        }

        public static Utente GetUtenteByIdPeople(string idPeople)
        {
            return docsPaWS.getUtenteById(idPeople);
        }

        /// <summary>
        /// Restituisce la lista dei 'destinatari' / 'destinatari in cc' associati ad un documento con il relativo canale preferenziale
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="typeDest">Può assumere D(estrai le info sui destinatari), C(estrati le info sui destinatari in CC)</param>
        /// <returns>List</returns>
        public static List<Corrispondente> GetPrefChannelAllDest(string idProfile, string typeDest)
        {
            return new List<Corrispondente>(docsPaWS.GetPrefChannelAllDest(idProfile, typeDest));
        }

        /// <summary>
        /// Restituisce l'utente automatico per un'amministrazione
        /// </summary>
        public static DocsPAWA.DocsPaWR.Utente GetUtenteAutomatico(Page page, string idAmm, out string returnMsg)
        {
            returnMsg = string.Empty;
            try
            {
                return docsPaWS.GetUtenteAutomatico(idAmm);
            }
            catch (System.Exception e)
            {
                ErrorManager.redirectToErrorPage(page, e);
            }
            return null;
        }

        // MEV Utente Multi Amministrazione
        /// <summary>
        /// Lista Amministrazione pewr Utente
        /// </summary>
        public static DocsPAWA.DocsPaWR.Amministrazione[] getListaAmministrazioniByUser(Page page, string userId, bool controllo, out string returnMsg)
        {
            returnMsg = string.Empty;

            try
            {
                return docsPaWS.amministrazioneGetAmministrazioniByUser(userId, controllo, out returnMsg);
            }
            catch (System.Exception e)
            {
                ErrorManager.redirectToErrorPage(page, e);
            }

            return null;
        }

        public static DocsPAWA.DocsPaWR.Amministrazione[] GetDatiAmministrazioneByUserAdministrator(Page page, string userId, bool controllo, out string returnMsg)
        {
            returnMsg = string.Empty;

            try
            {
                return docsPaWS.GetDatiAmministrazioneByUserAdministrator(userId, controllo, out returnMsg);
            }
            catch (System.Exception e)
            {
                ErrorManager.redirectToErrorPage(page, e);
            }

            return null;
        }

        public static string GetPasswordUtenteMultiAmm(Page page, string userId)
        {
            string password = string.Empty;

            try
            {
                return docsPaWS.GetPasswordUtenteMultiAmm(userId);
            }
            catch (System.Exception e)
            {
                ErrorManager.redirectToErrorPage(page, e);
            }

            return password;
        }

        public static bool SetPasswordUtenteMultiAmm(Page page, string userId, string password)
        {
            bool resultValue = false;

            try
            {
                return docsPaWS.SetPasswordUtenteMultiAmm(userId, password);
            }
            catch (System.Exception e)
            {
                ErrorManager.redirectToErrorPage(page, e);
            }

            return resultValue;
        }

        public static bool ModificaPasswordUtenteMultiAmm(Page page, string userId, string idAmm)
        {
            bool resultValue = false;

            try
            {
                return docsPaWS.ModificaPasswordUtenteMultiAmm(userId, idAmm);
            }
            catch (System.Exception e)
            {
                ErrorManager.redirectToErrorPage(page, e);
            }

            return resultValue;
        }

    }
}
