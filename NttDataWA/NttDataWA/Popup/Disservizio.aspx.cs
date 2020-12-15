using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class Disservizio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializeLanguage();
                this.InitializePage();
            }
        }

        private void InitializePage()
        {
            NttDataWA.DocsPaWR.Disservizio diss = UIManager.AdministrationManager.GetDisservizio();
            if (diss != null && !string.IsNullOrEmpty(diss.stato) && !string.IsNullOrEmpty(diss.testo_cortesia) && !diss.stato.Equals("disattivo"))
            {
                this.txtDisservizio.Text = diss.testo_cortesia;
            }
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.DisservizioClosePage.Text = Utils.Languages.GetLabelFromCode("DisservizioClosePage", language);
        }

        protected void DisservizioClosePage_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('Disservizio','');", true);
        }
    }
}