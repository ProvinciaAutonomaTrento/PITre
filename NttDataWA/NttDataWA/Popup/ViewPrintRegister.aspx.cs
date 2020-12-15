using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class ViewPrintRegister : System.Web.UI.Page
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
            this.BtnViewPrintRegisterClose.Text = Utils.Languages.GetLabelFromCode("BtnViewPrintRegisterClose", language);
        }

        protected void BtnViewPrintRegisterClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "reallowOp", "$('iframe').hide(); reallowOp();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('ViewPrintRegister','');", true);
        }
    }
}