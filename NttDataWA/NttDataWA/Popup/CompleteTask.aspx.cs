using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class CompleteTask : System.Web.UI.Page
    {

        #region Properties

        private string NoteCompleteTask
        {
            set
            {
                HttpContext.Current.Session["NoteCompleteTask"] = value;
            }
        }


        private DocsPaWR.Task TaskSelected
        {
            get
            {
                if (HttpContext.Current.Session["Task"] != null)
                {
                    return HttpContext.Current.Session["Task"] as DocsPaWR.Task;
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session["Task"] = value;
            }
        }
        #endregion

        public int maxLength = 250;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.InitializePage();
                }
                this.RefreshScript();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.TxtCompleteTask.MaxLength = 250;
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("it-IT");
            string today = DateTime.Now.ToString(ci);
            if (!string.IsNullOrEmpty(Request.QueryString["from"]) && Request.QueryString["from"].Equals("ReopenTask") 
                && !string.IsNullOrEmpty(this.TaskSelected.STATO_TASK.DATA_SCADENZA)
                && Utils.utils.verificaIntervalloDate(today, this.TaskSelected.STATO_TASK.DATA_SCADENZA)
                )
            {
                this.pnlDataScadenza.Visible = true;
            }
            else
            {
                this.pnlDataScadenza.Visible = false;
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            if(!string.IsNullOrEmpty(Request.QueryString["from"]) && Request.QueryString["from"].Equals("ReopenTask"))
                this.lblNoteCompleteTask.Text = Utils.Languages.GetLabelFromCode("CompleteTaskLblNoteReopenTask", language);
            else
                this.lblNoteCompleteTask.Text = Utils.Languages.GetLabelFromCode("CompleteTaskLblNoteCompleteTask", language);
            this.LblDataScadenza.Text = Utils.Languages.GetLabelFromCode("CompleteTaskLblDataScadenza", language);
            this.CompleteTaskOk.Text = Utils.Languages.GetLabelFromCode("AbortRecordBtnOk", language);
            this.CompleteTaskClose.Text = Utils.Languages.GetLabelFromCode("AbortRecordBtnClose", language);
            this.ltrTextCompleteTask.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language) + " ";
        }

        protected void CompleteTaskClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            string popupId = Request.QueryString["from"];
            this.TaskSelected = null;
            if (string.IsNullOrEmpty(popupId))
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('CompleteTask', '');} else {parent.closeAjaxModal('CompleteTask', '', parent);};", true);
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('" + popupId + "', '');} else {parent.closeAjaxModal('" + popupId + "', '', parent);};", true);
        }

        protected void CompleteTaskOk_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            string popupId = Request.QueryString["from"];
            if (!string.IsNullOrEmpty(popupId) && popupId.Equals("ReopenTask"))
            {
                this.TaskSelected.STATO_TASK.NOTE_RIAPERTURA = this.TxtCompleteTask.Text;
                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("it-IT");
                string today = DateTime.Now.ToString(ci);
                if (!string.IsNullOrEmpty(this.TaskSelected.STATO_TASK.DATA_SCADENZA) && Utils.utils.verificaIntervalloDate(today, this.TaskSelected.STATO_TASK.DATA_SCADENZA))
                {
                    if (!string.IsNullOrEmpty(this.txt_dataScadenza.Text))
                    {
                        //Verifico se la data si scadenza è superiore o uguale alla data corrente
                        if (Utils.utils.verificaIntervalloDateSenzaOra(Utils.dateformat.ConvertToDate(txt_dataScadenza.Text).ToShortDateString(), Utils.dateformat.ConvertToDate(today).ToShortDateString()))
                        {
                            this.TaskSelected.STATO_TASK.DATA_SCADENZA = this.txt_dataScadenza.Text;
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningCompleteTaskExpireDateValid', 'warning', '','',null,null,'')", true);
                            return;
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('WarningCompleteTaskExpireDateRequired', 'warning', '','',null,null,'')", true);
                        return;
                    }
                }
            }
            else
            {
                NoteCompleteTask = this.TxtCompleteTask.Text;
            }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            try
            {
                if(string.IsNullOrEmpty(popupId))
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('CompleteTask', 'up');} else {parent.closeAjaxModal('CompleteTask', 'up', parent);};", true);
                else
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('" + popupId + "', 'up');} else {parent.closeAjaxModal('" + popupId + "', 'up', parent);};", true);
            }
            catch (Exception ex)
            {
            }
        }
    }
}