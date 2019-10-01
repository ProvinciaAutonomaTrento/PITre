using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDataWA.UserControls;

namespace NttDataWA.Popup
{
    public partial class version_add : System.Web.UI.Page
    {
        #region constant
        private const string Save = "save";
        private const string Close = "close";
        private const string FE_MAX_LENGTH_NOTE = "FE_MAX_LENGTH_NOTE"; 
        #endregion

        #region Property
        /// <summary>
        /// Contiene il nuovo valore per la nota di versione
        /// </summary>
        private string DescriptionVersion
        {
            get
            {
                if (HttpContext.Current.Session["descriptionVersion"] != null)
                    return HttpContext.Current.Session["descriptionVersion"].ToString();
                else return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["descriptionVersion"] = value;
            }
        }
        #endregion

        #region Fields

        protected bool _isCheckedOutDocument = false;
        public int maxLength = 200;

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!this.IsPostBack)
                {
                    this.InitializeLanguage();
                    DocsPaWR.InfoUtente info = new DocsPaWR.InfoUtente();
                    info = UserManager.GetInfoUser();

                    string valoreChiave = InitConfigurationKeys.GetValue(info.idAmministrazione, FE_MAX_LENGTH_NOTE);
                    if (!string.IsNullOrEmpty(valoreChiave) && int.Parse(valoreChiave) <= maxLength)
                        maxLength = int.Parse(valoreChiave);
                    VersionDescription.MaxLength = maxLength;
                    if (this.Request.QueryString["modifyVersion"] != null && this.Request.QueryString["modifyVersion"].Equals("t"))
                    {
                        this.VersionDescription.Text = DescriptionVersion;
                    }
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

            this.VersionBtn.Text = Utils.Languages.GetLabelFromCode("GenericBtnSave", language);
            this.VersionBtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.VersionLitDescription.Text = Utils.Languages.GetLabelFromCode("VersionLitDescription", language);
            this.VersionsLitChars.Text = Utils.Languages.GetLabelFromCode("VersionLitChars", language);
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

        /// <summary>
        /// Enable Server Controls
        /// </summary>
        protected void EnableControls()
        {
            if (this._isCheckedOutDocument)
            {
                rowMessage.InnerHtml = "Il documento risulta bloccato.<br />\nPer effettuare l'operazione richiesta è necessario prima rilasciare il documento.";
                rowMessage.Visible = true;
                rowDescription.Visible = false;
                VersionBtn.Visible = false;
            }
            else
            {
                bool insertMode = string.IsNullOrEmpty(DocumentManager.getSelectedAttachId());
                rowMessage.Visible = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void VersionBtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            this.CloseMask(Close);
        }

        /// <summary>
        /// Version Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void VersionBtn_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            if (this.IsValid)
            {
                string error = string.Empty;
                    if (this.Request.QueryString["modifyVersion"] != null && this.Request.QueryString["modifyVersion"].Equals("t"))
                    {
                        error = "ErrorModifyVersion";
                        bool result = DocumentManager.ModifyVersion(this.VersionDescription.Text);
                        if (!result)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('" + error + "', 'error', '');", true);
                            return;
                        }
                        else
                        {
                            DescriptionVersion = this.VersionDescription.Text;
                            this.CloseMask(Save);
                        }
                    }
                    else
                    {
                        error = "ErrorAddVersion";
                        string version = this.SaveVersion();
                        if (version != null)
                        {
                            DocumentManager.setSelectedNumberVersion(version);
                            this.CloseMask(Save);
                        }
                    }
            }
        }

        /// <summary>
        /// Save Version
        /// </summary>
        protected virtual string SaveVersion()
        {
            // Modalità di inserimento
            Allegato a = new Allegato();
            Documento docMain = new Documento();
            FileRequest fileReq;
            //add a version attached
            if (DocumentManager.getSelectedAttachId() != null)
            {
                a.descrizione = this.VersionDescription.Text;
                a.docNumber = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()).docNumber;
                var obj = FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) as Allegato;
                if (obj != null)
                {
                    a.numeroPagine = obj.numeroPagine;
                }
                fileReq = a;
            }
            else //add a version doc main
            {
                docMain.docNumber = FileManager.GetFileRequest().docNumber;
                docMain.descrizione = this.VersionDescription.Text;
                //parte relativa alla data arrivo
                //SchedaDocumento doc = DocumentManager.getDocumentDetails(this.Page, docMain.docNumber, docMain.docNumber);
                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                docMain.dataArrivo = doc.documenti[doc.documenti.Length - 1].dataArrivo;
                fileReq = docMain;
            }
            
            try
            {
                bool toSend = true;
                fileReq = DocumentManager.AddVersion(fileReq, toSend);
                //ABBATANGELI - evita errore addVersion subito dopo upload da repository
                Session["fileDoc"] = null;
                return fileReq.version;
            }
            catch (NttDataWAException ex) 
            {
                throw new NttDataWAException(ex.Message, "");
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
        protected void CloseMask(string action)
        {
            string idPopup = string.Empty;
            if (this.Request.QueryString["modifyVersion"] != null && this.Request.QueryString["modifyVersion"].Equals("t"))
                idPopup = "ModifyVersion";
            else
                idPopup = "VersionAdd";
            if (action.Equals(Save))
                Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('" + idPopup + "','up');</script></body></html>");
            else
                Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('" + idPopup + "','');</script></body></html>");
            Response.End();
        }

    }
}
