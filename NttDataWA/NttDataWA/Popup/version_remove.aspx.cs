using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;


namespace NttDataWA.Popup
{
    public partial class version_remove : System.Web.UI.Page
    {
        #region Fields

        protected bool _isCheckedOutDocument = false;

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (!this._isCheckedOutDocument)
                {
                    rowMessage.InnerHtml = "Sei sicuro di voler eliminare la versione?";
                }
            }
        }

        #endregion

        #region constant
        private const string Save = "save";
        private const string Close = "close";
        #endregion

        #region Methods

        /// <summary>
        /// Attachment Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AttachmentBtn_Click(object sender, EventArgs e)
        {
            string type = string.Empty;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            SchedaDocumento documentTab;
            Documento[] listDoc;
            Documento docRemove;
            if (!string.IsNullOrEmpty(DocumentManager.getSelectedAttachId()))
            {
                string docNumber = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()).docNumber;
                documentTab = DocumentManager.getDocumentDetails(this, docNumber, docNumber);
                listDoc = documentTab.documenti;
                type = "A";
            }
            else
            {
                documentTab = DocumentManager.getSelectedRecord();
                listDoc = documentTab.documenti;
                type = "D";
            }

            docRemove = (from d in listDoc where d.version.Equals(DocumentManager.getSelectedNumberVersion()) select d).FirstOrDefault();
            try
            {
                DocumentManager.RemoveVersion(docRemove, documentTab,type);
                this.CloseMask(Save);
            }
            catch (NttDataWAException ex)
            {
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AttachmentBtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            this.CloseMask(Close);
        }


        /// <summary>
        /// Render Message
        /// </summary>
        /// <param name="message"></param>
        protected virtual void RenderMessage(string message)
        {
            rowMessage.InnerHtml = message;
            rowMessage.Visible = true;
        }

        /// <summary>
        /// Close Mask
        /// </summary>
        /// <param name="versionId"></param>
        protected void CloseMask(string action)
        {
            if(action.Equals(Save))
                Response.Write( "<html><body><script type=\"text/javascript\">parent.closeAjaxModal('VersionRemove','up');</script></body></html>");
            else if(action.Equals(Close))
                Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('VersionRemove','');</script></body></html>");
        }

        #endregion
    }
}