using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Debugger = ConservazioneWA.Utils.Debugger;
using ConservazioneWA.DocsPaWR;

namespace ConservazioneWA.Utils
{
    public class UserManager
    {
        protected static DocsPaWR.DocsPaWebService wss = new ProxyManager().getProxyDocsPa();
        //protected static DocsPaWR.DocsPaWebService wss = new DocsPaWR.DocsPaWebService();

        public static  DocsPaWR.Utente login(Page page, DocsPaWR.UserLogin login,
            out DocsPaWR.LoginResult loginResult, out string ipaddress)
        {
            DocsPaWR.Utente utente = null;
            loginResult = DocsPaWR.LoginResult.OK;
            ipaddress = "";

            try
            {
                loginResult = wss.Login(login, false, page.Session.SessionID, out utente, out ipaddress);
            }
            catch (Exception e)
            {
                //string msg = "Login Error";
                //DocsPaUtils.LogsManagement.Debugger.Write(msg, exception);

                loginResult = DocsPaWR.LoginResult.APPLICATION_ERROR;
                utente = null;

                Debugger.Write("Errore nel login result: " + e.Message);
            }

            return utente;
        }

        public static DocsPaWR.Utente ForcedLogin(Page page, DocsPaWR.UserLogin login, out DocsPaWR.LoginResult loginResult)
        {
            DocsPaWR.Utente utente = null;
            loginResult = DocsPaWR.LoginResult.OK;
            string ipaddress;

            try
            {
                loginResult = wss.Login(login, true, page.Session.SessionID, out utente, out ipaddress);
            }
            catch (Exception ex)
            {
                loginResult = DocsPaWR.LoginResult.APPLICATION_ERROR;
                utente = null;
                Debugger.Write("Errore nella ForcedLogin: " + ex.Message);
            }

            return utente;
        }

        public static bool logOff(WSConservazioneLocale.InfoUtente infoUtente, Page page)
        {
            string userId = infoUtente.userId;
            string idAmm = infoUtente.idAmministrazione;
            string dst = infoUtente.dst;

            bool result = true;
            try
            {
                result = wss.Logoff(userId, idAmm, page.Session.SessionID, dst);
            }
            catch(Exception e)
            {
                result = false;
                Debugger.Write("Errore nel log-off: " + e.Message);
            }
            return result;
        }

        public static DocsPaWR.ValidationResult ValidateLogin(string userName, string idAmm, string sessionId)
        {
            DocsPaWR.ValidationResult result = DocsPaWR.ValidationResult.APPLICATION_ERROR;

            try
            {
                result = wss.ValidateLogin(userName, idAmm, sessionId);
            }
            catch (Exception exception)
            {
               // DocsPaUtils.LogsManagement.Debugger.Write("Impossibile validare la sessione.", exception);
                result = DocsPaWR.ValidationResult.APPLICATION_ERROR;
                Debugger.Write("Errore nel ValidateLogin: " + exception.Message);
            }

            return result;
        }

        public static void logoff(System.Web.SessionState.HttpSessionState session)
        {
            ConservazioneWA.DocsPaWR.Utente utente = (ConservazioneWA.DocsPaWR.Utente)session["userData"];
            DocsPaWR.Ruolo ruolo = (DocsPaWR.Ruolo)utente.ruoli[0];
            DocsPaWR.InfoUtente infoUtente = null;

            try
            {
                if (ruolo != null && utente != null)
                {
                    infoUtente = getInfoUtente(utente, ruolo);
                    string appConfigValue = ConfigurationManager.AppSettings["DISABLE_LOGOUT_CLOSE_BUTTON"];
                    //PER ANAS, permette login passando dal portale applicativo.
                    if (appConfigValue == null || (!Convert.ToBoolean(appConfigValue)))
                        wss.Logoff(utente.userId, utente.idAmministrazione, session.SessionID, infoUtente.dst);
                }
            }
            catch (Exception ex)
            {
                Debugger.Write("Errore nel logoff: " + ex.Message);
            }
        }

        public static DocsPaWR.InfoUtente getInfoUtente(DocsPaWR.Utente utente, DocsPaWR.Ruolo ruolo)
        {
            DocsPaWR.InfoUtente infoUtente = new ConservazioneWA.DocsPaWR.InfoUtente();

            try
            {
                if (infoUtente != null && ruolo != null)
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
                }

            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("Impossibile accedere alle informazioni dell'utente" + exception.ToString());
                infoUtente = null;
                Debugger.Write("Errore nel get info Utente: " + exception.Message);
            }

            return infoUtente;
        }

        public static Corrispondente GetCorrispondenteByCodRubricaIE(string codice, AddressbookTipoUtente tipoIE, DocsPaWR.InfoUtente infoUtente)
        {
            try
            {
                return wss.AddressbookGetCorrispondenteByCodRubricaIE(codice, tipoIE, infoUtente);
            }
            catch (System.Web.Services.Protocols.SoapException es)
            {
                Debugger.Write("Errore nel get corrispondente: " + es.Message);
            }
            catch (Exception) { }
            return null;
        }

        // MEV Utente Multi Amministrazione
        /// <summary>
        /// Lista Amministrazione pewr Utente
        /// </summary>
        public static DocsPaWR.Amministrazione[] getListaAmministrazioniByUser(Page page, string userId, bool controllo, out string returnMsg)
        {
            returnMsg = string.Empty;

            try
            {
                return wss.amministrazioneGetAmministrazioniByUser(userId, controllo, out returnMsg);
            }
            catch (System.Exception e)
            {
            }

            return null;
        }

        /// <summary>
        /// Abbatangeli - Metodo per recuperare quale tipo di componenti utilizza l'utente
        /// </summary>
        /// <param name="brObject">Il browser utilizzato dall'utente. Se non è Internet Explorer ritorna 3 (APPLET)</param>
        /// <returns>Il tipo di componenti utilizzati: 
        /// 0 - non configurati
        /// 1 - activeX
        /// 2 - Smart Client
        /// 3- Applet
        /// </returns>
        public static string getComponentType(string userAgent, DocsPaWR.InfoUtente infoUtente)
        {
            try
            {
                string TYPE_NONE = "0";
                string TYPE_ACTIVEX = "1";
                string TYPE_SMARTCLIENT = "2";
                string TYPE_APPLET = "3";

                string retval = TYPE_ACTIVEX;

                // Verifica se il browser è IE altrimenti avvia sempre le applet      
                // Necessario cercare "Trident" nella stringa per IE11+
                if (userAgent.Contains("MSIE") || userAgent.Contains("Trident"))
                {
                    //DocsPaWR.DocsPaWebService ws = NttDataWA.Utils.ProxyManager.GetWS();
                    DocsPaWR.SmartClientConfigurations smcConf = wss.GetSmartClientConfigurationsPerUser(infoUtente);
                    retval = smcConf.ComponentsType;
                }
                else
                {
                    retval = TYPE_APPLET;
                }

                return retval;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

    }
}
