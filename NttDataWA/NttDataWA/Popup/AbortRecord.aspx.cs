using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class AbortRecord : System.Web.UI.Page
    {
        public int maxLength = 250;
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializePage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.TxtTextAbortRecord.MaxLength = 250;
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.AbortRecordLliDesc.Text = Utils.Languages.GetLabelFromCode("AbortRecordLliDesc", language);
            this.AbortRecordBtnOk.Text = Utils.Languages.GetLabelFromCode("AbortRecordBtnOk", language);
            this.AbortRecordBtnClose.Text = Utils.Languages.GetLabelFromCode("AbortRecordBtnClose", language);
            this.ltrTextAbortRecord.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language) + " ";
        }

        protected void AbortRecordBtnClose_Click(object sender, EventArgs e)
        {
            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('AbortRecord', '');</script></body></html>");
            Response.End();
        }

        protected void AbortRecordBtnOk_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            if (!String.IsNullOrEmpty(this.TxtTextAbortRecord.Text.Trim()))
            {
                SchedaDocumento docReturn = DocumentManager.AbortRecord(this.TxtTextAbortRecord.Text);
                DocumentManager.setSelectedRecord(docReturn);
                Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('AbortRecord', 'up');</script></body></html>");
                Response.End();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('La motivazione è obbligatoria.');", true);
            }
        }
    }
}