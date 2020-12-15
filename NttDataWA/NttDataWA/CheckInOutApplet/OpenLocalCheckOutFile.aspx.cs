using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.CheckInOutApplet
{
    public partial class OpenLocalCheckOutFile : System.Web.UI.Page
    {
        public static string checkOutFilePath;
        public static string errorMessage;
        public static string waitingMessage;

        private string componentType = Constans.TYPE_APPLET;

        private string language;

        protected string IsAlreadyDownloaded
        {
            get
            {
                return Session["IsAlreadyDownloaded"] as string;
            }
            set
            {
                Session["IsAlreadyDownloaded"] = value;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && this.IsAlreadyDownloaded==null)
            {
                string script = "$(function() {if (confirmAction()) parent.closeAjaxModal('OpenLocalCheckOutFile','true');});";
                checkOutFilePath = this.CheckOutFilePath();

                this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();

                string errorimg = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_error.png") + "\" alt=\"\" />";
                //string worningimg = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_warning.png") + "\" alt=\"\" />";
                waitingMessage = Utils.Languages.GetMessageFromCode("WaitingCheckOutLocalFileOpen", language);
                errorMessage = errorimg + Utils.Languages.GetMessageFromCode("ErrorCheckOutLocalFileOpen", language);

                this.CheckInOutCloseButton.Text = Utils.Languages.GetLabelFromCode("CheckInOutSaveLocalCloseButton", language);

                componentType = UserManager.getComponentType(Request.UserAgent);
                if (componentType == Constans.TYPE_SOCKET)
                {
                    script = "$(function(){confirmActionSocket(function(){parent.closeAjaxModal('OpenLocalCheckOutFile','true');});});";
                    this.pnlAppletTag.Visible = false;
                }
                ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "confirmAJM", script, true);
                this.IsAlreadyDownloaded = "true";
            }
        }

        private string CheckOutFilePath()
		{
			string retValue=string.Empty;
            CheckInOutServices.InitializeContext();

            if (DocumentManager.getSelectedAttachId() == null)
            {
                NttDataWA.DocsPaWR.SchedaDocumento schedaDoc = CheckInOutServices.CurrentSchedaDocumento;
                if (schedaDoc != null && schedaDoc.checkOutStatus != null)
                    retValue = schedaDoc.checkOutStatus.DocumentLocation;
            }
            else
            {
                NttDataWA.DocsPaWR.CheckOutStatus tempStatus = DocumentManager.GetCheckOutDocumentStatus(DocumentManager.GetSelectedAttachment().docNumber);
                if (tempStatus != null)
                    retValue = DocumentManager.GetCheckOutDocumentStatus(DocumentManager.GetSelectedAttachment().docNumber).DocumentLocation;
            }
                
			return retValue.Replace(@"\",@"\\");
		}

        protected void CheckInOutCloseButton_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('OpenLocalCheckOutFile','true');", true);
        }
    }
}