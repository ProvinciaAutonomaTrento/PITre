using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class ProjectSearchSubset : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitLanguage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnSearch.Text = Utils.Languages.GetLabelFromCode("GenericBtnSearch", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnCancel", language);
            this.lbl_search.Text = Utils.Languages.GetLabelFromCode("ProjectSearchSubsetText", language);
        }

        private string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                this.CloseMask(string.Empty);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            try {
                if (!string.IsNullOrEmpty(this.FolderSearch.Text))
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                    this.CloseMask(this.FolderSearch.Text);
                }
                else
                {
                    this.rowMessage.InnerHtml = this.GetLabel("ProjectSearchSubsetTextObligatory");
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void CloseMask(string returnValue)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "parent.closeAjaxModal('SearchSubset', '" + returnValue + "', parent);", true);
        }

    }
}