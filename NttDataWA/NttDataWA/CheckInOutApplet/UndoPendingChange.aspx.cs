using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.CheckInOutApplet
{
    public partial class UndoPendingChange : System.Web.UI.Page
    {
        private string language;
        public string componentType = Constans.TYPE_APPLET;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {

                this.initForm();
            }
        }

        private void initForm()
        {
            string componentCall = "if (confirmAction()) parent.closeAjaxModal('UndoCheckOut','up');";
            this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();
            this.litConditionalMessage.Text = "<img src=\"" + Page.ResolveClientUrl("~/Images/Common/messager_question.gif") + "\" alt=\"\" />" + Languages.GetLabelFromCode("CheckInOutUndoConfirm", language);

            this.CheckInOutConfirmButton.Text = Languages.GetLabelFromCode("CheckInOutUndoConfirmButton", language);
            this.CheckInOutCloseButton.Text = Languages.GetLabelFromCode("CheckInOutUndoCloseButton", language);

            this.pnlAppletTag.Visible = true;

            componentType = UserManager.getComponentType(Request.UserAgent);
            if (componentType == Constans.TYPE_SOCKET)
            {
                componentCall = "confirmActionSocket(function(){parent.closeAjaxModal('UndoCheckOut','up');})";
                this.pnlAppletTag.Visible = false;
            }

            CheckInOutConfirmButton.OnClientClick = componentCall;

        }
    
        /// <summary>
		/// Reperimento percorso del file per il documento in CheckOut
		/// </summary>
		public static string CheckOutFilePath
		{
			get
			{
				string retValue=string.Empty;
                CheckInOutServices.InitializeContext();

                if (DocumentManager.getSelectedAttachId() == null)
                {
                    SchedaDocumento schedaDoc = CheckInOutServices.CurrentSchedaDocumento;
                    if (schedaDoc != null && schedaDoc.checkOutStatus != null)
                        retValue = schedaDoc.checkOutStatus.DocumentLocation;
                }
                else
                {
                        CheckOutStatus tempStatus = DocumentManager.GetCheckOutDocumentStatus(DocumentManager.GetSelectedAttachment().docNumber);
                        if (tempStatus!=null)
                            retValue = DocumentManager.GetCheckOutDocumentStatus(DocumentManager.GetSelectedAttachment().docNumber).DocumentLocation;
                }

				return retValue.Replace(@"\",@"\\");
			}
		}

        protected void CheckInOutConfirmButton_Click(object sender, EventArgs e)
        {

            //ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "confirmAJM", "return confirmAction(); parent.closeAjaxModal('CheckOutDocument','up');", true);
        }

        protected void CheckInOutCloseButton_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('UndoCheckOut','');", true);
        }
    }
}