using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class DocumentViewerSummeries : System.Web.UI.Page
    {
        #region Properties
        private bool IsZoom
        {
            get
            {
                return (bool)HttpContext.Current.Session["isZoom"];
            }

            set
            {
                HttpContext.Current.Session["isZoom"] = value;
            }
        }
        private bool IsPreviousVersion
        {
            get
            {
                if (HttpContext.Current.Session["IsPreviousVersion"] != null) return (bool)HttpContext.Current.Session["IsPreviousVersion"];
                return false;
            }

            set
            {
                HttpContext.Current.Session["IsPreviousVersion"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    InitializeLanguage();
                    this.frame.Attributes["src"] = "../Summaries/PDFViewer.aspx";
                    this.UpPnlDocumentData.Update();
                    this.UpPnlDocumentData.Visible = true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.DocumentViewerBtnClose.Text = Utils.Languages.GetLabelFromCode("DocumentViewerBtnClose", language);
        }

        protected void DocumentViewerBtnClose_Click(object sender, EventArgs e)
        {
            string popupId = "DocumentViewerSummeries";
            if (this.IsPreviousVersion) popupId = "VerifyPreviousViewer";
            this.IsPreviousVersion = false;

            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('" + popupId + "','up');</script></body></html>");
            Response.End();
        }
    }
}