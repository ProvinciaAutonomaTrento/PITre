using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class AddressBook_import : System.Web.UI.Page
    {

        #region Fields

        protected int corrInseriti = 0;
        protected int corrAggiornati = 0;
        protected int corrRimossi = 0;
        protected int corrNonInseriti = 0;
        protected int corrNonAggiornati = 0;
        protected int corrNonRimossi = 0;

        #endregion

        #region Properties

        private bool EnableDistributionLists
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableDistributionLists"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableDistributionLists"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableDistributionLists"] = value;
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
            }
            else
            {
                this.ReadRetValueFromPopup();
            }
            this.RefreshScript();
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask(true);
        }

        protected void BtnConfirm_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);

            //Controllo la selezione file
            if (string.IsNullOrEmpty(this.uploadFile.Value))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialog", "ajaxDialogModal('ErrorAddressBookImportFileInvalid', 'error');", true);
                return;
            }

            //Controllo che sia un file Excel
            if (this.uploadFile.Value != "")
            {
                if (this.uploadFile.Value != null)
                {
                    string[] path = this.uploadFile.Value.Split('.');
                    if (path.Length != 0 && path[path.Length - 1] != "xls")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialog", "ajaxDialogModal('ErrorAddressBookImportXlsInvalid', 'error');", true);
                        return;
                    }
                }
            }

            //Inizio importazione
            HttpPostedFile p = this.uploadFile.PostedFile;
            Stream fs = p.InputStream;
            byte[] dati = new byte[fs.Length];
            fs.Read(dati, 0, (int)fs.Length);
            fs.Close();
            bool esitoImport = true;

            esitoImport = AddressBookManager.ImportaRubrica(dati, this.EnableDistributionLists, ref corrInseriti, ref corrAggiornati, ref corrRimossi, ref corrNonInseriti, ref corrNonAggiornati, ref corrNonRimossi);
            if (!esitoImport)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialog", "ajaxDialogModal('ErrorAddressBookImportTemplateInvalid', 'error');", true);
                this.litMessage.Text = this.GetLabel("AddressBookImportTemplateInvalid");
            }
            else
            {
                this.litMessage.Text = String.Format(this.GetLabel("AddressBookImportResult"), corrInseriti, corrAggiornati, corrRimossi, corrNonInseriti, corrNonAggiornati, corrNonRimossi);
            }
            this.plcMessage.Visible = true;

            this.BtnLog.Visible = true;
        }

        protected void BtnLog_Click(object sender, EventArgs e)
        {
            string msg = "";
            foreach (string log in AddressBookManager.getLogImportRubrica())
                msg += log + "<br />\n";
            this.litMessage.Text = msg;

            this.plcFile.Visible = false;
            
            this.BtnLog.Enabled = false;
            this.BtnConfirm.Enabled = false;
        }

        #endregion

        #region Methods

        private void ReadRetValueFromPopup()
        {
            //if (!string.IsNullOrEmpty(this.MassiveReport.ReturnValue))
            //{
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveReport','');", true);
            //}
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnConfirm.Text = Utils.Languages.GetLabelFromCode("AddressBookImportBtnConfirm", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("AddressBookImportBtnClose", language);
            this.BtnLog.Text = Utils.Languages.GetLabelFromCode("AddressBookImportBtnLog", language);
            this.litFile.Text = Utils.Languages.GetLabelFromCode("AddressBookImportFile", language);
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('ImportDati', '" + retValue + "', parent);", true);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        #endregion

    }
}