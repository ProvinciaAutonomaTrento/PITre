using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class VerificaConnettoreSocket : System.Web.UI.Page
    {
        # region Constants
        private const string LINUX_OS = "Linux";
        private const string MACINTOSH_OS = "Macintosh";
        private const string appName = "NOAPPLET"; 
        private const string appNameLinux = "NOAPPLET_LINUX";
        private const string VERSIONE_NON_RILEVATA = "0.0.0";
        # endregion
        # region Property
        private string Version
        {
            get
            {
                String version = String.IsNullOrEmpty(Request.QueryString["version"]) ? "0.0.0" : Request.QueryString["version"];
                String sessionVersion = HttpContext.Current.Session["socketVersion"] as String;
                if (!String.IsNullOrEmpty(sessionVersion) && version.Equals("0.0.0"))
                {
                    version = sessionVersion;
                }else{
                    HttpContext.Current.Session["socketVersion"] = version;
                }
                return version;
            }
        }


        protected DesktopApp desktopApp
        {
            get
            {
                DesktopApp result = null;
                if (HttpContext.Current.Session["desktopAppNoApplet"] != null)
                {
                    result = HttpContext.Current.Session["desktopAppNoApplet"] as DesktopApp;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["desktopAppNoApplet"] = value;
            }
        }


        private bool IsCheckVersion
        {
            get
            {
                return !String.IsNullOrEmpty(Request.QueryString["from"]) && Request.QueryString["from"].Equals("index") ? true : false;
            }
        }

        # endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeLanguage();
                this.lblLastVesion.Text = this.desktopApp != null && !String.IsNullOrEmpty(this.desktopApp.Versione) ? this.desktopApp.Versione : "";
                if (this.Version.Equals(VERSIONE_NON_RILEVATA))
                {
                    string language = UIManager.UserManager.GetUserLanguage();
                    this.PnlVersion.Visible = false;
                    this.messager.Text = Utils.Languages.GetMessageFromCode("VerificaConnettoreSocketMessageVersioneNonRilevata", language);
                }
                else
                {
                    this.lblVersion.Text = this.Version;
                }
                //this.desktopApp != null && !String.IsNullOrEmpty(this.desktopApp.Path) ? this.desktopApp.Path : "";
                if (IsCheckVersion)
                {
                    this.CheckVersion();
                }
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.VerificaConnettoreSocketBtnCancel.Text = (UIManager.UserManager.IsAuthorizedFunctions(DBKeys.FE_CHECK_DISABLE_CONTROL_REMOVE_NOTIFY.ToString())) ?
                Utils.Languages.GetLabelFromCode("NotificationNoticeDaysBtnClose", language) :
                Utils.Languages.GetLabelFromCode("NotificationNoticeDaysBtnCancel", language);
            this.messager.Text = Utils.Languages.GetMessageFromCode("VerificaConnettoreSocketMessage", language);
            this.ltlVersion.Text = Utils.Languages.GetLabelFromCode("VerificaConnettoreSocketVersion", language);
            this.ltlLastVersion.Text = Utils.Languages.GetLabelFromCode("VerificaConnettoreSocketLastVersion", language);
            //this.ltlPath.Text = Utils.Languages.GetLabelFromCode("VerificaConnettoreSocketPath", language);
            this.LblPath.Text = Utils.Languages.GetLabelFromCode("VerificaConnettoreSocketScarica", language);
            this.LblPassi.Text = Utils.Languages.GetLabelFromCode("VerificaConnettoreSocketPassi", language);
            this.LblDownload.Text = Utils.Languages.GetLabelFromCode("VerificaConnettoreSocketScarica", language);
            this.LblDisinstalla.Text = Utils.Languages.GetLabelFromCode("VerificaConnettoreSocketDisinstalla", language);
            this.LblRiavvia.Text = Utils.Languages.GetLabelFromCode("VerificaConnettoreSocketRiavvia", language);
            this.LblInstalla.Text = Utils.Languages.GetLabelFromCode("VerificaConnettoreSocketInstalla", language);
            this.LblRiavvia1.Text = Utils.Languages.GetLabelFromCode("VerificaConnettoreSocketRiavvia", language);
        }

        private void CheckVersion()
        {
            NttDataWA.DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();
            InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();
            if ((Request.UserAgent.IndexOf(LINUX_OS) > 0 || Request.UserAgent.IndexOf(MACINTOSH_OS) > 0))
                this.desktopApp = ws.GetDesktopApp(appNameLinux, infoUtente);
            else
                this.desktopApp = ws.GetDesktopApp(appName, infoUtente);
            if (
                !String.IsNullOrEmpty(this.Version) &&
                this.desktopApp != null &&
                !String.IsNullOrEmpty(this.desktopApp.Versione) &&
                !this.desktopApp.Versione.Equals(this.Version)
                )
            {
                Response.StatusCode=500;
                Response.StatusDescription = "Installare componente no applet";
            }

        }

        protected void Path_Click(object sender, EventArgs e)
        {
            //if(this.desktopApp != null && !String.IsNullOrEmpty( this.desktopApp.Path))
            //    Response.Redirect(this.desktopApp.Path);

            Response.Redirect("../WebClientHTML5/DownloadConnector.ashx");
        }

        protected void VerificaConnettoreSocketBtnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('VerificaConnettoreSocket','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
    }
}