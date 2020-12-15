using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAAdminTool.AdminTool.UserControl
{
    public partial class MenuConservazione : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                GestioneGrafica();
            }
        }

        protected void GestioneGrafica()
        {
            this.linkPolicyDocumenti.Attributes.Add("onmouseover", "this.src='" + "../Images/up_admin_menu.gif'");
            this.linkPolicyDocumenti.Attributes.Add("onmouseout", "this.src='" + "../Images/down_admin_menu.gif'");
            this.linkPolicyFascicoli.Attributes.Add("onmouseover", "this.src='" + "../Images/up_admin_menu.gif'");
            this.linkPolicyFascicoli.Attributes.Add("onmouseout", "this.src='" + "../Images/down_admin_menu.gif'");
        }

        //MEV CONS 1.3
        //funzione per la gestione dell'abilitazione/disabilitazione delle voci di menu di conservazione
        protected bool DisableAmmGestCons()
        {
            bool result = false;

            string PGU_FE_DISABLE_AMM_GEST_CONS_Value = string.Empty;
            PGU_FE_DISABLE_AMM_GEST_CONS_Value = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "PGU_FE_DISABLE_AMM_GEST_CONS");
            result = ((PGU_FE_DISABLE_AMM_GEST_CONS_Value.Equals("0") || string.IsNullOrEmpty(PGU_FE_DISABLE_AMM_GEST_CONS_Value)) ? false : true);

            return result;
        }
    }
}