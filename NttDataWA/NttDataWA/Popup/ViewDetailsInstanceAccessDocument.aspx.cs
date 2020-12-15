using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class ViewDetailsInstanceAccessDocument : System.Web.UI.Page
    {
        #region Properties

        private InstanceAccessDocument Document
        {
            get
            {
                InstanceAccessDocument doc = null;
                if (HttpContext.Current.Session["instanceAccessDocument"] != null)
                {
                    doc = HttpContext.Current.Session["instanceAccessDocument"] as InstanceAccessDocument;
                }
                return doc;
            }
            set
            {
                HttpContext.Current.Session["instanceAccessDocument"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeLanguage();
                InitializePage();
            }

        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.ViewDetailsInstanceDocumentBtnClose.Text = Utils.Languages.GetLabelFromCode("ViewDetailsInstanceDocumentBtnClose", language);
            this.LtrRegister.Text = Utils.Languages.GetLabelFromCode("ViewDetailsInstanceDocumentLtrRegister", language);
            this.LtrHash.Text = Utils.Languages.GetLabelFromCode("InstanceDocumentHash", language);
            this.LtrNomeFile.Text = Utils.Languages.GetLabelFromCode("InstanceDocumentFileName", language);
            this.LtrClassification.Text = Utils.Languages.GetLabelFromCode("InstanceDocumentClass", language);
            this.LtrCodeProject.Text = Utils.Languages.GetLabelFromCode("InstanceDocumentCodeProject", language);
            if(Document != null && Document.INFO_PROJECT != null && !string.IsNullOrEmpty(Document.INFO_PROJECT.ID_FASCICOLO))
                this.LtrDescriptionProject.Text = Utils.Languages.GetLabelFromCode("InstanceDocumentDescSottofascicolo", language);
            else
                this.LtrDescriptionProject.Text = Utils.Languages.GetLabelFromCode("InstanceDocumentDescProject", language);
        }

        private void InitializePage()
        {
            this.lblHashText.ReadOnly = true;
            PopulatesFields();
        }

        private void PopulatesFields()
        {
            if (Document != null)
            {
                this.lblRegisterText.Text = !string.IsNullOrEmpty(Document.INFO_DOCUMENT.COUNTER_REPERTORY) ? Document.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO : Document.INFO_DOCUMENT.REGISTER;
                this.lblHashText.Text = Document.INFO_DOCUMENT.HASH;
                this.lblNomeFileText.Text = Document.INFO_DOCUMENT.FILE_NAME;
                this.lblClassificationText.Text = Document.INFO_PROJECT != null ? Document.INFO_PROJECT.CODE_CLASSIFICATION : string.Empty;
                this.lblCodeProjectText.Text = Document.INFO_PROJECT != null ? Document.INFO_PROJECT.CODE_PROJECT : string.Empty;
                this.lblDescriptionProjectText.Text = Document.INFO_PROJECT != null ? Document.INFO_PROJECT.DESCRIPTION_PROJECT : string.Empty;
            }
        }

        protected void ViewDetailsInstanceDocumentBtnClose_Click(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Session.Remove("instanceAccessDocument");
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('ViewDetailsInstanceAccessDocument','up');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
    }
}