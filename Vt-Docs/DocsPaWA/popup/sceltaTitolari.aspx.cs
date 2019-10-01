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

namespace DocsPAWA.popup
{
    public partial class sceltaTitolari : DocsPAWA.CssPage
    {
        private DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
		
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["CodClassifica"] != null)
                {
                    string codClassifica = Request.QueryString["CodClassifica"].ToString();
                    DocsPaWR.Fascicolo[] listaFasc = FascicoliManager.getListaFascicoliDaCodice(this, codClassifica, UserManager.getRegistroSelezionato(this), "R");
                    caricaDg(listaFasc);
                }
            }
        }

        protected void btn_Conferma_Click(object sender, EventArgs e)
        {
            string idTitolarioSelezionato = string.Empty;

            for (int i = 0; i < gw_Titolari.Rows.Count; i++)
            {
                if (((System.Web.UI.WebControls.CheckBox)gw_Titolari.Rows[i].Cells[2].Controls[1]).Checked)
                {
                    idTitolarioSelezionato = gw_Titolari.Rows[i].Cells[0].Text;
                    break;
                }
            }

            if(idTitolarioSelezionato != null && idTitolarioSelezionato != "")
            {
                Session.Add("idTitolarioSelezionato", idTitolarioSelezionato);
                ClientScript.RegisterStartupScript(this.GetType(), "chiudeFinestra", "window.close();", true);
            }
        }

        protected void btn_Chiudi_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chiudeFinestra", "window.close();", true);
            Session.Remove("idTitolarioSelezionato");
        }

        private void caricaDg(DocsPaWR.Fascicolo[] listaFasc)
        {
            gw_Titolari.Columns[0].Visible = true;
            DataTable dt = new DataTable();
            dt.Columns.Add("ID_TITOLARIO");
            dt.Columns.Add("TITOLARIO");
            for (int i = 0; i < listaFasc.Length; i++)
            {
                DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)listaFasc[i];
                DataRow rw = dt.NewRow();
                rw[0] = fasc.idTitolario;
                DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(fasc.idTitolario);
                rw[1] = titolario.Descrizione;
                dt.Rows.Add(rw);
            }

            dt.AcceptChanges();
            gw_Titolari.DataSource = dt;
            gw_Titolari.DataBind();
            gw_Titolari.Visible = true;
            gw_Titolari.Columns[0].Visible = false;

            //Associo l'evento alle checkBox
            for (int i = 0; i < gw_Titolari.Rows.Count; i++)
            {
                ((System.Web.UI.WebControls.CheckBox)gw_Titolari.Rows[i].Cells[2].Controls[1]).CheckedChanged += new EventHandler(cb_selezione_CheckedChanged);
            }            
        }

        protected void cb_selezione_CheckedChanged(object sender, EventArgs e)
        {
            //Gestisco la mutua esclusione delle checkbox
            CheckBox cb = (CheckBox)sender;
            GridViewRow row = (GridViewRow) cb.NamingContainer;
            int rigaSelezionata = row.RowIndex;

            for (int i = 0; i < gw_Titolari.Rows.Count; i++)
            {
                if(i!=rigaSelezionata)
                    ((System.Web.UI.WebControls.CheckBox)gw_Titolari.Rows[i].Cells[2].Controls[1]).Checked = false;
            }            
        }        
    }
}
