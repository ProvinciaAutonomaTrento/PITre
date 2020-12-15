using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;
using ConservazioneWA.Utils;
using Debugger = ConservazioneWA.Utils.Debugger;

namespace ConservazioneWA
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            /*
           * GESTIONE DELLA SESSIONE:
           * -----------------------------------------------------------------------------
           * sia il tool di amministrazione sia Docspa si trovano sotto lo stesso progetto 
           * quindi hanno in comune il presente Global.asax .
           * 
           * Esiste una sessione denominata "AppWA" che all'accesso del tool di amm.ne 
           * viene impostata a "ADMIN"; all'accesso di Docspa viene impostata a "DOCSPA".
           */
            try
            {
                UserManager.logoff(this.Session);
            }
            catch (Exception ex)
            {
                Debugger.Write("Errore nel log off: " + ex.Message);
            }
            

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}