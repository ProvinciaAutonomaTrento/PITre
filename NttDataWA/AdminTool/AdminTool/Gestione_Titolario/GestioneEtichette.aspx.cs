using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace SAAdminTool.AdminTool.Gestione_Titolario
{
    public partial class GestioneEtichette : System.Web.UI.Page
    {

        protected SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();
        protected SAAdminTool.DocsPaWR.OrgTitolario titolario = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            string idTitolario = Request.QueryString["idTitolario"];
            if (!string.IsNullOrEmpty(idTitolario))
            {
                titolario = wws.getTitolarioById(idTitolario);
                if(!IsPostBack && titolario != null)
                    setTxtEtichette(titolario);
            }
        }

        protected void setTxtEtichette(SAAdminTool.DocsPaWR.OrgTitolario titolario)
        {
            if (!string.IsNullOrEmpty(titolario.EtichettaTit))
                txt_etTitolario.Text = titolario.EtichettaTit;
            else
                txt_etTitolario.Text = "Titolario";

            if (!string.IsNullOrEmpty(titolario.EtichettaLiv1))
                txt_etLivello1.Text = titolario.EtichettaLiv1;
            else
                txt_etLivello1.Text = "Livello1";

            if (!string.IsNullOrEmpty(titolario.EtichettaLiv2))
                txt_etLivello2.Text = titolario.EtichettaLiv2;
            else
                txt_etLivello2.Text = "Livello2";

            if (!string.IsNullOrEmpty(titolario.EtichettaLiv3))
                txt_etLivello3.Text = titolario.EtichettaLiv3;
            else
                txt_etLivello3.Text = "Livello3";

            if (!string.IsNullOrEmpty(titolario.EtichettaLiv4))
                txt_etLivello4.Text = titolario.EtichettaLiv4;
            else
                txt_etLivello4.Text = "Livello4";

            if (!string.IsNullOrEmpty(titolario.EtichettaLiv5))
                txt_etLivello5.Text = titolario.EtichettaLiv5;
            else
                txt_etLivello5.Text = "Livello5";

            if (!string.IsNullOrEmpty(titolario.EtichettaLiv6))
                txt_etLivello6.Text = titolario.EtichettaLiv6;
            else
                txt_etLivello6.Text = "Livello6";

        }

        protected void setEtichetteTit(SAAdminTool.DocsPaWR.OrgTitolario titolario)
        {
            if (!string.IsNullOrEmpty(txt_etTitolario.Text))
                titolario.EtichettaTit = txt_etTitolario.Text;
            else
                titolario.EtichettaTit = "Titolario";

            if (!string.IsNullOrEmpty(txt_etLivello1.Text))
                titolario.EtichettaLiv1 = txt_etLivello1.Text;
            else
                titolario.EtichettaLiv1 = "Livello1";

            if (!string.IsNullOrEmpty(txt_etLivello2.Text))
                titolario.EtichettaLiv2 = txt_etLivello2.Text;
            else
                titolario.EtichettaLiv2 = "Livello2";

            if (!string.IsNullOrEmpty(txt_etLivello3.Text))
                titolario.EtichettaLiv3 = txt_etLivello3.Text;
            else
                titolario.EtichettaLiv3 = "Livello3";

            if (!string.IsNullOrEmpty(txt_etLivello4.Text))
                titolario.EtichettaLiv4 = txt_etLivello4.Text;
            else
                titolario.EtichettaLiv4 = "Livello4";

            if (!string.IsNullOrEmpty(txt_etLivello5.Text))
                titolario.EtichettaLiv5 = txt_etLivello5.Text;
            else
                titolario.EtichettaLiv5 = "Livello5";

            if (!string.IsNullOrEmpty(txt_etLivello6.Text))
                titolario.EtichettaLiv6 = txt_etLivello6.Text;
            else
                titolario.EtichettaLiv6 = "Livello6";        
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chiusura", "window.close();", true);
        }

        protected void bnt_conferma_Click(object sender, EventArgs e)
        {
            setEtichetteTit(titolario);
            wws.setEtichetteTitolario(titolario);
            ClientScript.RegisterStartupScript(this.GetType(), "chiusura", "window.close();", true);
        }
    }
}
