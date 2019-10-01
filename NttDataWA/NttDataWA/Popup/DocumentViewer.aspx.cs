using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class DocumentViewer : System.Web.UI.Page
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
                if (HttpContext.Current.Session["IsPreviousVersion"]!=null) return (bool)HttpContext.Current.Session["IsPreviousVersion"];
                return false;
            }

            set
            {
                HttpContext.Current.Session["IsPreviousVersion"] = value;
            }
        }

        private bool CloseDocumentViewer
        {
            get
            {
                return (!String.IsNullOrEmpty(OnClose));
            }
        }

        protected String OnClose
        {
            get
            {
                return onClose.Value;
            }
            set
            {
                onClose.Value = value;
            }
        }

        public static bool OpenDocumentViewer
        {
            get
            {
                if (HttpContext.Current.Session["OpenDocumentViewer"] != null)
                    return (bool)HttpContext.Current.Session["OpenDocumentViewer"];
                else return false;
            }
            set
            {
                HttpContext.Current.Session["OpenDocumentViewer"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (CloseDocumentViewer || !OpenDocumentViewer)
                {
                    OnClose = null;
                    this.ViewDocument.Dispose();
                    return;
                }
                    

                if (!IsPostBack)
                {
                    InitializeLanguage();
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
            OpenDocumentViewer = false;
            string popupId = "DocumentViewer";
            if (this.IsPreviousVersion) popupId = "VerifyPreviousViewer";
            this.IsPreviousVersion = false;

            if (Session["CallFromSignDetails"]!=null)
                Session["CallFromSignDetails"] = null;

            if (Session["isPopupOfPopup"] == null)
            {
                Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('" + popupId + "','up');</script></body></html>");
                Response.End();
            }
            else
            {
                Session["isPopupOfPopup"] = null;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('" + popupId + "','up', parent);", true);
            }
        }
    }
}