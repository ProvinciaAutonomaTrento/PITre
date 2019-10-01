using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class visualReport_iframe : System.Web.UI.Page
    {
        public static string componentType = Constans.TYPE_SMARTCLIENT;


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                componentType = UserManager.getComponentType(Request.UserAgent);

                this.InitApplet();


                this.InitLanguage();
                string tipologia = "rtf";
                Response.Expires = -1;

                if (!IsPostBack && !this.AlreadyDownloaded)
                {
                    DocsPaWR.FileDocumento file = new DocsPaWR.FileDocumento();

                    exportDatiSessionManager sessioneManager = new exportDatiSessionManager();
                    file = sessioneManager.GetSessionExportFile();


                    if (file == null || file.content == null || file.content.Length == 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('InfoExportDatiNoneFound', 'info', '');", true);
                        if (componentType == Constans.TYPE_APPLET)
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "openFile", "OpenFileApplet('" + tipologia + "');", true);
                        else
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "openFile", "OpenFileActiveX('" + tipologia + "');", true);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(file.name) && FileManager.getEstensioneIntoP7M(file.name).ToUpper().Equals("PDF"))
                            tipologia = "PDF";
                        String script = "OpenFileApplet('" + tipologia + "');";
                        switch (componentType)
                        {
                            case Constans.TYPE_ACTIVEX:
                            case Constans.TYPE_SMARTCLIENT:
                                script = "OpenFileActiveX('" + tipologia + "');";
                                break;
                            case Constans.TYPE_SOCKET:
                                script = "OpenFileSocket('" + tipologia + "');";
                                break;
                            default:
                                script = "OpenFileApplet('" + tipologia + "');";
                                break;
                        }

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "openFile", script, true);
                    }
                }

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
       
        private void InitApplet()
        {
            if (componentType == Constans.TYPE_APPLET)
                this.plcApplet.Visible = true;
            else
            {
                Control ShellWrapper = Page.LoadControl("../ActivexWrappers/ShellWrapper.ascx");
                this.plcActiveX.Controls.Add(ShellWrapper);

                Control AdoStreamWrapper = Page.LoadControl("../ActivexWrappers/AdoStreamWrapper.ascx");
                this.plcActiveX.Controls.Add(AdoStreamWrapper);

                Control FsoWrapper = Page.LoadControl("../ActivexWrappers/FsoWrapper.ascx");
                this.plcActiveX.Controls.Add(FsoWrapper);

                this.plcActiveX.Visible = true;
            }
        }

        private bool AlreadyDownloaded
        {
            get
            {
                if (HttpContext.Current.Session["visualReportAlreadyDownloaded" + Session.SessionID] != null)
                    return (bool)HttpContext.Current.Session["visualReportAlreadyDownloaded" + Session.SessionID];
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["visualReportAlreadyDownloaded" + Session.SessionID] = value;
            }
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.litMessage.Text = Utils.Languages.GetMessageFromCode("InfoPrintCreated", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
        }

        public static string httpFullPath
        {
            get
            {
                return utils.getHttpFullPath();
            }
        }

        private bool IsApplet
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.QueryString["isapplet"]))
                    return true;
                else
                    return false;
            }
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.CloseMask();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CloseMask()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["fr"]) && Request.QueryString["fr"].ToString().Equals("P"))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('visualReport_iframe', 'true');} else {parent.closeAjaxModal('visualReport_iframe', 'true');};", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('Prints', 'true');} else {parent.closeAjaxModal('Prints', 'true');};", true);
            }
        }

    }
}

