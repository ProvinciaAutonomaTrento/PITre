using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.Gestione_Organigramma
{
    public partial class AvvisoUtenteConTrasm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.btnRimuovi.Attributes.Add("onclick", "window.returnValue = 'Y'; window.close();");
            this.btn_Annulla.Attributes.Add("onclick", "window.returnValue = 'N'; window.close();");
        }

        #region EXPORT
        protected void btnExport_Click(object sender, EventArgs e)
        {
            string idPeople = this.Request.QueryString["idPeople"].ToString();
            string idCorrGlobali = this.Request.QueryString["idCorrGlobali"].ToString();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenExport", "OpenExport('" + idPeople + "','" + idCorrGlobali + "');", true);
        }
        #endregion
    }
}