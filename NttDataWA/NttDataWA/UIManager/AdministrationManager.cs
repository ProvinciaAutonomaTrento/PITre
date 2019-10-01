using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;
using System.Collections;
using NttDataWA.Utils;
using log4net;


namespace NttDataWA.UIManager
{
    public class AdministrationManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(AdministrationManager));
        

        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public void ListaAmministrazioni()
        {
            try
            {
                this.AmmGetListAmministrazioni();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static string GetNews(string idAdministration)
        {
            return docsPaWS.getNews(idAdministration);
        }

        public static string GetBanner(string idAdministration)
        {
            return docsPaWS.getBanner(idAdministration);
        }

        public ArrayList AmmGetListAmministrazioni()
        {
            try
            {
                DocsPaWR.InfoAmministrazione[] array = docsPaWS.AmmGetListAmministrazioni();

                ArrayList result = new ArrayList(array);

                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static InfoAmministrazione AmmGetInfoAmmCorrente(string idAdm)
        {
            try
            {
                return docsPaWS.AmmGetInfoAmmCorrente(idAdm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool IsEnableRF(string idAmm)
        {
            try
            {
                // DI BLASI: modifica gestione da chiave web.config backend a chiave db di amministrazione front-end
                //return docsPaWS.IsEnabledRF(idAmm);
                return (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(idAmm, DBKeys.FE_ENABLE_AMMRF.ToString())) && (Utils.InitConfigurationKeys.GetValue(idAmm, DBKeys.FE_ENABLE_AMMRF.ToString()).Equals("1")));
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static void RedirectPage(string MessageType, string param)
        {
            HttpContext.Current.Response.Redirect("~/Login.aspx?MessageType=" + MessageType + "&param=" + param);
        }

        public static void CheckSession()
        {
            //Verifica sessione scaduta
            string MessageType = String.Empty;
            if (UIManager.UserManager.GetUserInSession() == null)
            {
                MessageType = "1";
                ILog logger = LogManager.GetLogger(typeof(AdministrationManager));
                logger.Debug("Sessione scaduta");
            }
            else
            {
                if(HttpContext.Current.Session["OpenDirectLink"] != null && !string.IsNullOrEmpty(HttpContext.Current.Session["OpenDirectLink"].ToString()))
                {
                    //Accesso diretto al link del documento
                }
                else 
                {
                    //Verifica multiutenza                
                    Utente utente = UIManager.UserManager.GetUserInSession();
                    InfoUtente delegato = UserManager.getDelegato();
                    if (utente != null)
                    {
                        //if (delegato != null)
                        //{
                        //    DocsPaWR.ValidationResult resultValidationPage = UserManager.ValidateLogin(delegato.userId, delegato.idAmministrazione, utente.sessionID);

                        //    if (resultValidationPage == DocsPaWR.ValidationResult.SESSION_DROPPED)
                        //    {
                        //        MessageType = "2";
                        //        ILog logger = LogManager.GetLogger(typeof(AdministrationManager));
                        //        logger.Debug("L'utente si è connesso da un'altra postazione.");
                        //    }
                        //}
                        //else
                        //{
                            DocsPaWR.ValidationResult resultValidationPage = UserManager.ValidateLogin(utente.userId, utente.idAmministrazione, utente.sessionID);

                            if (resultValidationPage == DocsPaWR.ValidationResult.SESSION_DROPPED)
                            {
                                MessageType = "2";
                                ILog logger = LogManager.GetLogger(typeof(AdministrationManager));
                                logger.Debug("L'utente si è connesso da un'altra postazione.");
                            }
                        //}
                    }
                }
            }

            //Verifico se la pagina chiamante è una popup o una basepage
            if (!string.IsNullOrEmpty(MessageType))
            {
                string UrlCall = HttpContext.Current.Request.Url.AbsolutePath.ToString().ToLower();
                if (UrlCall.IndexOf("/popup/", 0) > 0)
                {
                    if (UrlCall.IndexOf("/popup/addkeyword", 0) > 0)
                    { RedirectPage(MessageType, "Step1"); }
                    else
                    { RedirectPage(MessageType, "Step2"); }
                }
                else
                { RedirectPage(MessageType, "StepStop"); }
            }
        }

        public static void DiagnosticError(System.Exception ex)
        {
            //CheckSession();
            Error er = new Error();

            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(ex, true);
            System.Diagnostics.StackFrame frame = st.GetFrame(0);

            er.fileName = frame.GetFileName();
            er.methodName = frame.GetMethod().Name;
            er.line = frame.GetFileLineNumber();
            er.col = frame.GetFileColumnNumber();

            string filename = er.fileName;
            string lineCol = string.Empty;

            if (er.fileName == null)
            {
                string fn = ex.StackTrace.Replace("\r\n", "");
                filename = fn.Trim();
            }

            if (er.line > 0)
                lineCol = "Linea " + er.line + " Colonna " + er.col;

            ILog logger = LogManager.GetLogger(typeof(AdministrationManager));
            logger.Error(ex.Message+" "+ex.StackTrace, ex);           

            //Verifico se la pagina chiamante è una popup o una basepage 
            string UrlCall = HttpContext.Current.Request.Url.AbsolutePath.ToString().ToLower();
            //if (UrlCall.IndexOf("/popup/", 0) > 0)
            //{
            //    HttpContext.Current.Response.Write("<script type='text/javascript'>window.open('" + Utils.utils.getHttpFullPath() + "/ErrorPage.aspx?Message=" + utils.FormatUrl(ex.Message) + "&fileName=" + utils.FormatUrl(filename) + "&methodName=" + utils.FormatUrl(er.methodName) + "&lineCol=" + utils.FormatUrl(lineCol) + "', '_parent');</script>");
            //}
            //else
            //{
                HttpContext.Current.Response.Redirect("~/ErrorPage.aspx?Message=" + utils.FormatUrl(ex.Message) + "&fileName=" + utils.FormatUrl(filename) + "&methodName=" + utils.FormatUrl(er.methodName) + "&lineCol=" + utils.FormatUrl(lineCol));
            //}
        }

        public static string ToDay()
        {
            try
            {
                return docsPaWS.toDay();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return string.Empty;
            }
        }

        public static string DayOnYearBeforeToday()
        {
            try
            {
                return docsPaWS.DayOnYearBeforeToday();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return string.Empty;
            }
        }

        public static string toFirstDayOfYear()
        {
            try
            {
                return docsPaWS.toFirstDayOfYear();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return string.Empty;
            }
        }

        public static bool isEnableIndiceSistematico()
        {
            try
            {
                return docsPaWS.isEnableIndiceSistematico();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        //ADP 9 maggio 2013
        public static DocsPaWR.OrgRegistro[] AmmGetRegistri(string codiceAmministrazione, string chaRF)
        {

            DocsPaWR.OrgRegistro[] retValue = null;

            try
            {
                retValue = docsPaWS.AmmGetRegistri(codiceAmministrazione, chaRF);
            }
            catch (Exception ex)
            {

                UIManager.AdministrationManager.DiagnosticError(ex);


            }

            return retValue;
        }

        public static string getApplicationName()
        {
            string retValue = string.Empty;
            try
            {
                retValue = docsPaWS.getApplicationName();
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
            return retValue;
        }

        public static string getLoginMessage()
        {
            string retvalue = "";
            try
            {
                retvalue = docsPaWS.GestioneMessaggioLogin("", null);
            }
            catch (Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                logger.Error(ex);
            }
            return retvalue;
        }            

        public static Disservizio GetDisservizio()
        {
            try
            {
                return docsPaWS.getInfoDisservizio();
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

    }
}
