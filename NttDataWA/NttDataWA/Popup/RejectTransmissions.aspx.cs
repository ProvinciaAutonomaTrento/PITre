using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class RejectTransmissions : System.Web.UI.Page
    {
        #region Session
        /// <summary>
        /// valore di ritorno della popup del rifiuto trasmissione
        /// </summary>
        private string ReturnValueRejectTransm
        {
            set
            {
                HttpContext.Current.Session["RejectTransmissions"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.InitializePage();
            }
            this.RefreshScript();

        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void InitializePage()
        {
            //this.ReturnValueRejectTransm = string.Empty;
            this.InitializeLanguage();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.SmistalblRejectTransmNote.Text = Utils.Languages.GetLabelFromCode("SmistalblRejectTransmNote", language);
            this.BtnRejectSave.Text = Utils.Languages.GetLabelFromCode("GenericBtnSave", language);
            this.BtnRejectClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
        }

        protected void BtnRejectClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "closeAJM", "parent.closeAjaxModal('RejectTransmissions','');", true);
        }

        protected void BtnRejectSave_Click(object sender, EventArgs e)
        {
            this.ReturnValueRejectTransm = this.txtRejectTransm.Text.Trim();
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disallow", "parent.disallowOp('Content2');", true);
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "closeAJM", "parent.closeAjaxModal('RejectTransmissions','up');", true);
        }
    }
}