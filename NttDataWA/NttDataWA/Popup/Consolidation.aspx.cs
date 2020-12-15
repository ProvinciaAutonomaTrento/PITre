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
    public partial class Consolidation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializeLanguage();
                    this.InitializePage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitializePage()
        {
            this.ConsolidationBtnOk.Enabled = false;
            SchedaDocumento doc = DocumentManager.getSelectedRecord();
            bool consolidationSetp1 = UserManager.IsAuthorizedFunctions("DO_CONSOLIDAMENTO");
            bool consolidationSetp2 = UserManager.IsAuthorizedFunctions("DO_CONSOLIDAMENTO_METADATI");

            if (doc != null && !string.IsNullOrEmpty(doc.systemId))
            {
                //Documento non consolidato
                if (doc.ConsolidationState == null || (doc.ConsolidationState != null && doc.ConsolidationState.State == DocsPaWR.DocumentConsolidationStateEnum.None))
                {
                    if (consolidationSetp1 && consolidationSetp2)
                    {
                        this.PlcBothConsolidation.Visible = true;
                        this.PnlSelectedConsolidation.Visible = false;
                    }
                    else
                    {
                        this.PnlSelectedConsolidation.Visible = true;
                        this.PlcBothConsolidation.Visible = false;
                        if (consolidationSetp1)
                        {
                            this.ConsolidationLblTitleOneChoise.Text = this.ConsolidationLblTitleStep1.Text;
                            this.ConsolidationLblOneChoise.Text = this.ConsolidationLblStep1.Text;
                            this.ConsolidationBtnOk.Enabled = true;
                            this.Step1.Checked = true;
                        }
                        else
                        {
                            this.ConsolidationLblTitleOneChoise.Text = this.ConsolidationLblTitleStep2.Text;
                            this.ConsolidationLblOneChoise.Text = this.ConsolidationLblStep2.Text;
                            this.ConsolidationBtnOk.Enabled = true;
                            this.Step2.Checked = true;
                        }
                    }
                }
                else
                {
                    if (doc.ConsolidationState.State == DocumentConsolidationStateEnum.Step1 && consolidationSetp2)
                    {
                        string language = UIManager.UserManager.GetUserLanguage();
                        this.ConsolidationLblTitleOneChoise.Text = Utils.Languages.GetLabelFromCode("ConsolidationLblTitleStep2b", language);
                        this.ConsolidationLblOneChoise.Text = Utils.Languages.GetLabelFromCode("ConsolidationLblStep2b", language);
                        this.ConsolidationBtnOk.Enabled = true;
                        this.Step2.Checked = true;
                        this.PnlSelectedConsolidation.Visible = true;
                    }
                }
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.ConsolidationBtnOk.Text = Utils.Languages.GetLabelFromCode("ConsolidationBtnOk", language);
            this.ConsolidationBtnChiudi.Text = Utils.Languages.GetLabelFromCode("ConsolidationBtnChiudi", language);
            this.ConsolidationLblStep1.Text = Utils.Languages.GetLabelFromCode("ConsolidationLblStep1", language);
            this.ConsolidationLblStep2.Text = Utils.Languages.GetLabelFromCode("ConsolidationLblStep2", language);
            this.ConsolidationLblTitleStep1.Text = Utils.Languages.GetLabelFromCode("ConsolidationLblTitleStep1", language);
            this.ConsolidationLblTitleStep2.Text = Utils.Languages.GetLabelFromCode("ConsolidationLblTitleStep2", language);
            this.messager.Text = Utils.Languages.GetLabelFromCode("ConsolidationMessanger", language);
        }

        protected void ConsolidationBtnOk_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                //Consolidazione
                if (this.Step1.Checked)
                {
                    this.ConsolidateDocument(DocsPaWR.DocumentConsolidationStateEnum.Step1);
                }
                else
                {
                    this.ConsolidateDocument(DocsPaWR.DocumentConsolidationStateEnum.Step2);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ConsolidateDocument(DocsPaWR.DocumentConsolidationStateEnum toState)
        {
            DocumentManager.ConsolidateDocument(DocumentManager.getSelectedRecord(), toState, UserManager.GetInfoUser());
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "UpBtnOk", "parent.closeAjaxModal('Consolidation','up');", true);
        }

        protected void ConsolidationBtnChiudi_Click(object sender, EventArgs e)
        {
            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('Consolidation', '');</script></body></html>");
            Response.End();
        }

        protected void Step1_CheckedChanged(object sender, EventArgs e)
        {
            try {
                this.ConsolidationBtnOk.Enabled = true;
                UpBtnOk.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void Step2_CheckedChanged(object sender, EventArgs e)
        {
            try {
                this.ConsolidationBtnOk.Enabled = true;
                UpBtnOk.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }
    }
}