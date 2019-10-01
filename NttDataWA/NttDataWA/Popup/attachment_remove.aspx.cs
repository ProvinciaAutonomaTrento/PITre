using System;
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
    public partial class attachment_remove : System.Web.UI.Page
    {

        #region Fields

        protected bool _isCheckedOutDocument = false;

        #endregion

        #region Properties

        private int PageSize
        {
            get
            {
                int toReturn = 10;
                if (HttpContext.Current.Session["PageSize"] != null) Int32.TryParse(HttpContext.Current.Session["PageSize"].ToString(), out toReturn);
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["PageSize"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 0;
                if (HttpContext.Current.Session["SelectedPage"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedPage"].ToString(), out toReturn);
                if (toReturn < 0) toReturn = 0;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedPage"] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!this.IsPostBack)
                {
                    DocsPaWR.InfoUtente info = new DocsPaWR.InfoUtente();
                    info = UserManager.GetInfoUser();

                    // Reperimento allegato selezionato
                    DocsPaWR.Allegato allegato = DocumentManager.GetSelectedAttachment();

                    if (!this._isCheckedOutDocument)
                    {
                        try
                        {
                            rowMessage.InnerHtml = "Sei sicuro di voler eliminare l'allegato " + allegato.versionLabel + "?";
                        }
                        catch (Exception ex)
                        {
                            // on postback throw n exception
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attachment Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AttachmentBtn_Click(object sender, EventArgs e)
        {
            try {
                DocsPaWR.Allegato allegato = DocumentManager.GetSelectedAttachment();
                SchedaDocumento doc = DocumentManager.getSelectedRecord();

                if (allegato != null)
                {
                    ProxyManager.GetWS().DocumentoRimuoviAllegato(allegato, UserManager.GetInfoUser(), doc);

                    // Delete attachment from document in session
                    List<DocsPaWR.Allegato> allegati = new List<DocsPaWR.Allegato>(doc.allegati);
                    allegati.Remove(allegato);
                    doc.allegati = allegati.ToArray();

                    // Update grid index
                    if (this.SelectedPage > (int)Math.Round((allegati.Count / this.PageSize) + 0.49)) this.SelectedPage = (int)Math.Round((allegati.Count / this.PageSize) + 0.49);

                    // Delete SelectedAttachment session
                    DocumentManager.setSelectedAttachId(null);

                    this.CloseMask();
                }
                else
                {
                    RenderMessage("Errore nell'eliminazione dell'allegato");
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AttachmentBtnClose_Click(object sender, EventArgs e)
        {
            try {
                this.CloseMask();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
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
        protected void CloseMask()
        {


            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('AttachmentsRemove','up');</script></body></html>");
            Response.End();
        }

        #endregion

    }
}