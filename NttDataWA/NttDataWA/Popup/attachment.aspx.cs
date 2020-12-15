using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.Popup
{
    public partial class attachment : System.Web.UI.Page
    {

        #region Fields
        
        protected bool _isCheckedOutDocument = false;
        public int maxLength = 2000;
        
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


        private string VersionIdAttachSelected
        {
            get
            {
                if (HttpContext.Current.Session["versionIdAttachSelected"] != null)
                    return HttpContext.Current.Session["versionIdAttachSelected"].ToString();
                else return string.Empty;
            }
        }

        private bool isNewAttachment
        {
            get
            {
                if (Request.QueryString["t"] == null)
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!this.IsPostBack)
                {
                    this.InitializeLanguage();

                    DocsPaWR.InfoUtente info = new DocsPaWR.InfoUtente();
                    info = UserManager.GetInfoUser();

                    string valoreChiave = InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_DESC_ALLEGATO");
                    if (!string.IsNullOrEmpty(valoreChiave)) maxLength = int.Parse(valoreChiave);
                    AttachmentDescription.MaxLength = maxLength;

                    string versionId = String.Empty;

                    // Reperimento allegato selezionato
                    if (Request.QueryString["t"] == null)
                    {
                        versionId = DocumentManager.getSelectedAttachId();
                        DocumentManager.setSelectedAttachId(null);
                    }
                    else
                        DocumentManager.setSelectedAttachId(VersionIdAttachSelected);
                    Allegato allegato = DocumentManager.GetSelectedAttachment();

                    if (!string.IsNullOrEmpty(versionId))
                        DocumentManager.setSelectedAttachId(versionId);

                    if (!this._isCheckedOutDocument)
                    {
                        if (allegato == null)
                        {
                            // Modalità di inserimento
                            AttachmentCode.Text = string.Empty;
                            AttachmentDescription.Text = string.Empty;
                            AttachmentPagesCount.Text = string.Empty;
                        }
                        else
                        {
                            // Modalità di modifica dati
                            AttachmentCode.Text = allegato.versionLabel;
                            AttachmentDescription.Text = allegato.descrizione;
                            AttachmentPagesCount.Text = allegato.numeroPagine.ToString();
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

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            try {
                if (!this.IsPostBack) this.EnableControls();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion

        #region Methods

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.AttachmentLitDescription.Text = Utils.Languages.GetLabelFromCode("AttachmentsLblModifyDescr", language);
            this.AttachmentLitPagesNr.Text = Utils.Languages.GetLabelFromCode("AttachmentsLblPages", language);
            this.AttachmentsLitAvChars.Text = Utils.Languages.GetLabelFromCode("AttachmentsLitAvChars", language);
            this.AttachmentBtn.Text = Utils.Languages.GetLabelFromCode("GenericBtnSave", language);
            this.AttachmentBtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
        }

        /// <summary>
        /// Enable Server Controls
        /// </summary>
        protected void EnableControls()
        {
            if (this._isCheckedOutDocument)
            {
                rowMessage.InnerHtml = "Il documento risulta bloccato.<br />\nPer effettuare l'operazione richiesta è necessario prima rilasciare il documento.";
                rowMessage.Visible = true;
                rowCode.Visible = false;
                rowDescription.Visible = false;
                rowPagesCount.Visible = false;
                AttachmentBtn.Visible = false;
            }
            else
            {
                //bool insertMode = string.IsNullOrEmpty(DocumentManager.getSelectedAttachId());
                bool insertMode = isNewAttachment;
                rowMessage.Visible = false;
                rowCode.Visible = !insertMode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AttachmentBtnClose_Click(object sender, EventArgs e)
        {
            //try {
                this.CloseMask(true);
            //}
            /*catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }*/
        }

        /// <summary>
        /// Attachment Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AttachmentBtn_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            if(isNewAttachment)
                DocumentManager.setSelectedAttachId(null);
            if (this.IsValid)
            {

                DocsPaWR.Allegato allegato = this.SaveAttachment();
                if (allegato != null)
                {
                    this.CloseMask(false);
                }
            }
        }

        /// <summary>
        /// Save Attachment
        /// </summary>
        protected virtual DocsPaWR.Allegato SaveAttachment()
        {
            Allegato attachment = DocumentManager.GetSelectedAttachment();
            SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
            List<DocsPaWR.Allegato> attachments = new List<DocsPaWR.Allegato>(documentTab.allegati);
            if (attachment == null)
            {
                // Modalità di inserimento
                attachment = new DocsPaWR.Allegato();
                attachment.descrizione = AttachmentDescription.Text;
                attachment.numeroPagine = AttachmentPagesCount.Text.Length>0 ? Convert.ToInt32(AttachmentPagesCount.Text) : 0;
                attachment.docNumber = documentTab.docNumber;
                attachment.dataInserimento = DateTime.Today.ToString();
                attachment.version = "1";
                attachment.TypeAttachment = 1;
                string idPeopleDelegato = "0";
                if (UserManager.GetInfoUser().delegato != null) idPeopleDelegato = UserManager.GetInfoUser().delegato.idPeople;
                attachment.idPeopleDelegato = idPeopleDelegato;

                // Impostazione del repositoryContext associato al documento
                attachment.repositoryContext = documentTab.repositoryContext;
                attachment.position = (documentTab.allegati.Length + 1);
                if (attachment.version != null && attachment.version.Equals("0"))
                {
                    attachment.version = "1";
                }
                try
                {
                    attachment = DocumentManager.AddAttachment(attachment);
                }
                 catch (NttDataWAException ex)
                {
                    throw new NttDataWAException(ex.Message, "");
                }
                //calcolo a runtime il versionLabel dell'allegato appena creato 
                attachment.versionLabel = string.Format("A{0:0#}", DocumentManager.getSelectedRecord().allegati.Count() + 1);
                // Inserimento dell'allegato nella scheda documento
                attachments.Add(attachment);
                documentTab.allegati = attachments.ToArray();
                DocumentManager.setSelectedRecord(documentTab);
                // Aggiornamento grid index
                this.SelectedPage = (int)Math.Round((attachments.Count / this.PageSize) + 0.49);
            }
            else
            {
                // Modifica dati
                attachment.descrizione = AttachmentDescription.Text;
                attachment.numeroPagine = AttachmentPagesCount.Text.Length > 0 ? Convert.ToInt32(AttachmentPagesCount.Text) : 0;
                documentTab.allegati = attachments.ToArray();
                ProxyManager.GetWS().DocumentoModificaAllegato(UserManager.GetInfoUser(), attachment, documentTab.docNumber);
                if (documentTab != null && documentTab.systemId != null && !(documentTab.systemId.Equals(documentTab.systemId))) FileManager.removeSelectedFile();
            }

            return attachment;
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
        protected void CloseMask(bool close)
        {
            if (Request.QueryString["t"] != null && Request.QueryString["t"] == "swap")
            {
                if (close)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "addAtt", "parent.closeAjaxModal('AttachmentsSwap', '');", true);
                else
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "addAtt", "parent.closeAjaxModal('AttachmentsSwap', 'up');", true);
            }
            else
            {
                //if (string.IsNullOrEmpty(DocumentManager.getSelectedAttachId()))
                if(isNewAttachment)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "addAtt", "parent.closeAjaxModal('AttachmentsAdd', 'up');", true);
                //Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('AttachmentsAdd', 'up');</script></body></html>");
                else
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "addAtt", "parent.closeAjaxModal('AttachmentsModify', 'up');", true);
                //Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('AttachmentsModify', 'up');</script></body></html>");
                // Response.End();
            }
        }

        #endregion

    }
}
