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
    public partial class AddNewProcess : System.Web.UI.Page
    {
        #region Properties

        private ProcessoFirma ProcessoDiFirmaSelected
        {
            get
            {
                if (HttpContext.Current.Session["ProcessoDiFirmaSelected"] != null)
                    return (ProcessoFirma)HttpContext.Current.Session["ProcessoDiFirmaSelected"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ProcessoDiFirmaSelected"] = value;
            }
        }

        #endregion

        #region Standard Method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
        }

        private void InitializePage()
        {
            InitializeLanguage();
            this.txt_processName.Text = this.ProcessoDiFirmaSelected.nome;
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.AddNewProcessClose.Text = Utils.Languages.GetLabelFromCode("AddNewProcessClose", language);
            this.AddNewProcessSave.Text = Utils.Languages.GetLabelFromCode("AddNewProcessSave", language);
            this.ProcessName.Text = Utils.Languages.GetLabelFromCode("AddNewProcessName", language);
            this.cbxCopiaVisibilita.Text = Utils.Languages.GetLabelFromCode("AddNewProcessCbxCopiaVisibilita", language);
        }
        #endregion

        #region Event Buttons

        protected void AddNewProcessSave_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = string.Empty;
                if (string.IsNullOrEmpty(this.txt_processName.Text))
                {
                    msg = "WarningRequiredFieldNameProcess";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    return;
                }
                ResultProcessoFirma result = ResultProcessoFirma.OK;
                ProcessoFirma processo = SignatureProcessesManager.DuplicaProcessoFirma(this.ProcessoDiFirmaSelected.idProcesso, this.txt_processName.Text, this.cbxCopiaVisibilita.Checked, out result);
                switch (result)
                { 
                    case ResultProcessoFirma.OK:
                        ProcessoDiFirmaSelected = processo;
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('AddNewProcess','up');", true);
                        break;
                    case ResultProcessoFirma.KO:
                        msg = "ErrorCreationProcess";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                        break;
                    case ResultProcessoFirma.EXISTING_PROCESS_NAME:
                         msg = "WarningSignatureProcessUniqueProcessName";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        break;
                }
            }
            catch (Exception ex)
            {
                string msg = "ErrorSignatureProcess";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'error', '');}", true);
                return;
            }
        }

        protected void AddNewProcessClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeAJM", "parent.closeAjaxModal('AddNewProcess','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        #endregion
    }
}