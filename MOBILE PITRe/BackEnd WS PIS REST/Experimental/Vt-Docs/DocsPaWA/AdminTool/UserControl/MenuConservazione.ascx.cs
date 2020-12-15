using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.UserControl
{
    public partial class MenuConservazione : System.Web.UI.UserControl
    {

        protected DocsPAWA.DocsPaWR.InfoUtenteAmministratore _datiAmministratore = null;

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

        protected bool GestioneMenuUserAdmin()
        {
            bool result = false;

            this.setUserSession();
            if (this._datiAmministratore.tipoAmministratore.Equals("3"))
            {
                return true;
            }

            return result;
        }

        //MEV CONS 1.3
        //funzione per la gestione dell'abilitazione/disabilitazione delle voci di menu di conservazione
        protected bool DisableAmmGestCons()
        {
            bool result = false;

            string PGU_FE_DISABLE_AMM_GEST_CONS_Value = string.Empty;
            PGU_FE_DISABLE_AMM_GEST_CONS_Value = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "PGU_FE_DISABLE_AMM_GEST_CONS");
            result = ((PGU_FE_DISABLE_AMM_GEST_CONS_Value.Equals("0") || string.IsNullOrEmpty(PGU_FE_DISABLE_AMM_GEST_CONS_Value)) ? false : true);

            return result;
        }

        protected bool DisplayMenuPARER()
        {
            bool result = false;

            string FE_WA_CONSERVAZIONE = string.Empty;
            FE_WA_CONSERVAZIONE = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_WA_CONSERVAZIONE");
            if (!string.IsNullOrEmpty(FE_WA_CONSERVAZIONE) && FE_WA_CONSERVAZIONE.Equals("1"))
                result = true;
            else
                result = false;

            return result;

        }

        private void setUserSession()
        {
            this._datiAmministratore = new DocsPAWA.DocsPaWR.InfoUtenteAmministratore();

            DocsPAWA.AdminTool.Manager.SessionManager sessionMng = new DocsPAWA.AdminTool.Manager.SessionManager();
            this._datiAmministratore = sessionMng.getUserAmmSession();
        }
    }
}