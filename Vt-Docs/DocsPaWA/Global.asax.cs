using System;
using System.Web;
using DocsPAWA.AdminTool.Manager;
using DocsPAWA.DocsPaWR;
using log4net;

namespace DocsPAWA
{
    /// <summary>
    /// Summary description for Global.
    /// </summary>
    public class Global : HttpApplication
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Global));

        public Global()
        {
            InitializeComponent();
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            // Initialize log4net.
            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Session is Available here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            try
            {
                Utente ut = HttpContext.Current.Session["userData"] as Utente;
                if (ut != null && ut.userId != null)
                    LogicalThreadContext.Properties["userId"] = ut.userId.ToUpper();
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            var lastException = Server.GetLastError();
            _logger.Error(lastException);
            if (lastException.InnerException != null)
            {
                Session["ErrorManager.error"] = lastException.InnerException.Message;
                _logger.Error(lastException.InnerException.Message);
                _logger.ErrorFormat("Stacktrace : \r\n{0}", lastException.InnerException.StackTrace);
            }

            Server.ClearError();
            Server.Transfer("~/ErrorPage.aspx");
        }

        /// <summary>
        /// GESTIONE DELLA SESSIONE:
        ///  -----------------------------------------------------------------------------
        ///  sia il tool di amministrazione sia Docspa si trovano sotto lo stesso progetto 
        ///  quindi hanno in comune il presente Global.asax .
        ///  
        ///  Esiste una sessione denominata "AppWA" che all'accesso del tool di amm.ne 
        ///  viene impostata a "ADMIN"; all'accesso di Docspa viene impostata a "DOCSPA".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_End(object sender, EventArgs e)
        {
            try
            {
                // Reperimento infoutente dalla sessione
                var infoUtente = UserManager.getInfoUtente(Session);
                if (infoUtente != null)
                    if (infoUtente.delegato != null)
                        if (DelegheManager.DismettiDelega(infoUtente))
                            infoUtente = infoUtente.delegato;

                var appKey = string.Empty;

                if (Session["AppWA"] != null)
                    appKey = Session["AppWA"].ToString();

                if (string.IsNullOrEmpty(appKey)) 
                    return;

                switch (appKey)
                {
                    case "DOCSPA":
                        UserManager.logoff(infoUtente, Session.SessionID);
                        break;
                    case "ADMIN":
                        var userdata = new SessionManager().getUserAmmSession();
                        if(userdata != null)
                            UserManager.logoffAdmin(userdata);
                        break;
                    default:
                        UserManager.logoff(Session);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.ErrorFormat("Stacktrace :\r\n{0}", ex.StackTrace);

                if (ex.InnerException != null)
                {
                    _logger.Error(ex.InnerException.Message);
                    _logger.ErrorFormat("Stacktrace :\r\n{0}", ex.InnerException.StackTrace);
                }
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }

        #region Web Form Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion
    }
}