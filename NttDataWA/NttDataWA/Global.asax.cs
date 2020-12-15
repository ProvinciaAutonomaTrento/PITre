using System;
using System.Web;
using System.Web.Security;
using log4net;
using log4net.Config;
using NttDataWA.UIManager;

namespace NttDataWA
{
    public class Global : HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Global));

        /// <summary>
        /// Code that runs on application startup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Start(object sender, EventArgs e)
        {
            // Questo serve per far partire log4net, manca la classe che va a leggere la chiave di Backend
            // per ora possiamo utilizzare il logFileAppender per leggere alcuni log in test.
            XmlConfigurator.Configure();
        }

        private void Application_End(object sender, EventArgs e)
        {
        }

        private void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            var ex = Server.GetLastError();
            Log.Error(ex);
            if (ex.InnerException == null) 
                return;

            Log.Error(ex.InnerException);
        }

        private void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started           
        }

        /// <summary>
        /// GESTIONE DELLA SESSIONE:
        /// -----------------------------------------------------------------------------
        /// sia il tool di amministrazione sia Docspa si trovano sotto lo stesso progetto 
        /// quindi hanno in comune il presente Global.asax .
        /// 
        /// Esiste una sessione denominata "AppWA" che all'accesso del tool di amm.ne 
        /// viene impostata a "ADMIN"; all'accesso di Docspa viene impostata a "DOCSPA".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Session_End(object sender, EventArgs e)
        {
            try
            {
                // Reperimento infoutente dalla sessione
                var infoUtente = UserManager.getInfoUtente(Session);
                if (infoUtente != null)
                    if (infoUtente.delegato != null)
                        if (DelegheManager.DismettiDelega(infoUtente))
                            infoUtente = infoUtente.delegato;

                //ABBATANGELI - Cancello anche eventuali cookie creati da asp.net
                if (Response.Cookies.Count > 0)
                    foreach (var lName in Response.Cookies.AllKeys)
                        if (lName == FormsAuthentication.FormsCookieName || lName.ToLower() == "asp.net_sessionid")
                            Response.Cookies[lName].Expires = DateTime.Today.AddDays(-1);

                if(infoUtente!= null)
                    LoginManager.LogOut2(
                        infoUtente.userId, 
                        infoUtente.idAmministrazione, 
                        Session.SessionID, 
                        infoUtente.dst);
            }
            catch (Exception)
            {
                // Ignored
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (Response.Cookies.Count <= 0)
                return;

            foreach (var lName in Response.Cookies.AllKeys)
                if (lName == FormsAuthentication.FormsCookieName || lName.ToLower() == "asp.net_sessionid")
                    Response.Cookies[lName].HttpOnly = false;
        }
    }
}