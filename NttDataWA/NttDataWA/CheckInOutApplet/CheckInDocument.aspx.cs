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
    public partial class CheckInDocument : System.Web.UI.Page
    {
        private string language;
        private string componentType = Constans.TYPE_APPLET;
        public const int MAX_RELEASE_LENTGTH = 200;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {

                this.initForm();
            }
        }

        private void initForm()
        {
            string confirmScript = "if (confirmAction()) parent.closeAjaxModal('CheckInDocument','up');";
            this.language = NttDataWA.UIManager.UserManager.GetUserLanguage();
            this.optDeleteFile.Text = Languages.GetLabelFromCode("CheckInOutDeleteLocalFile", language);
            this.optPreserveFile.Text = Languages.GetLabelFromCode("CheckInOutPreserveLocalFile", language);

            this.CheckInOutConfirmButton.Text = Languages.GetLabelFromCode("CheckInOutUndoConfirmButton", language);
            this.CheckInOutCloseButton.Text = Languages.GetLabelFromCode("CheckInOutUndoCloseButton", language);
            this.componentType = UIManager.UserManager.getComponentType(Request.UserAgent);
            if (this.componentType.Equals(Constans.TYPE_SOCKET))
            {
                this.pnlAppletTag.Visible = false;
                confirmScript = "disallowOp('Content1'); confirmActionSocket(function(close) { reallowOp(); if(close){ parent.closeAjaxModal('CheckInDocument','up');} });";
            }
            this.CheckInOutConfirmButton.OnClientClick = confirmScript;
            //ScriptManager.RegisterStartupScript(this.Page,this.Page.GetType(),"initKeyPressRelease", "initKeyPressRelease();", true);
        }

        /// <summary>
        /// Reperimento percorso del file per il documento in CheckOut
        /// </summary>
        public static string CheckOutFilePath
        {
            get
            {
                string retValue = string.Empty;
                CheckInOutServices.InitializeContext();

                SchedaDocumento schedaDoc = CheckInOutServices.CurrentSchedaDocumento;

                if (schedaDoc != null && schedaDoc.checkOutStatus != null)
                    retValue = schedaDoc.checkOutStatus.DocumentLocation;
                else
                {
                    if (DocumentManager.getSelectedAttachId() != null)
                    {
                        CheckOutStatus tempStatus = DocumentManager.GetCheckOutDocumentStatus(DocumentManager.GetSelectedAttachment().docNumber);
                        if (tempStatus != null)
                            retValue = DocumentManager.GetCheckOutDocumentStatus(DocumentManager.GetSelectedAttachment().docNumber).DocumentLocation;
                    }
                }


                return retValue.Replace(@"\", @"\\");
            }
        }

        public static string httpFullPath
        {
            get
            {
                return utils.getHttpFullPath();
            }
        }

        protected void rblListCheckInOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.hdnOptSelected.Value = this.rblListCheckInOption.SelectedValue;
            //this.pnlOpt.Update();
        }

        protected void CheckInOutConfirmButton_Click(object sender, EventArgs e)
        {
            // Impostazione dei commenti nell'oggetto di contesto del checkout
            if (CheckOutAppletContext.Current != null && this.txtComments.Text.Length < MAX_RELEASE_LENTGTH)
                CheckOutAppletContext.Current.CheckInComments = this.txtComments.Text;

        }

        protected void CheckInOutCloseButton_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this.UpUpdateButtons, this.UpUpdateButtons.GetType(), "closeAJM", "parent.closeAjaxModal('CheckInDocument','');", true);
        }
    }
}