using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace SAAdminTool.AdminTool.Gestione_Homepage
{
    public partial class Home2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
            if (Session.IsNewSession)
            {
                Response.Redirect("../Exit.aspx?FROM=EXPIRED");
            }

            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            if (!ws.CheckSession(Session.SessionID))
            {
                Response.Redirect("../Exit.aspx?FROM=ABORT");
            }
            // ---------------------------------------------------------------

			SAAdminTool.AdminTool.Manager.SessionManager sessione = new SAAdminTool.AdminTool.Manager.SessionManager();
            SAAdminTool.DocsPaWR.InfoUtenteAmministratore datiAmministratore = new SAAdminTool.DocsPaWR.InfoUtenteAmministratore();
			datiAmministratore = sessione.getUserAmmSession();
			
			if(datiAmministratore!=null)
				lb_utente.Text = datiAmministratore.nome + " " + datiAmministratore.cognome;
				
            lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
        }
        				
    }
}
