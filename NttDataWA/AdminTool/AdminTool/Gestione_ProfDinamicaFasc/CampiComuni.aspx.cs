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

namespace SAAdminTool.AdminTool.Gestione_ProfDinamicaFasc
{
    public partial class CampiComuni : System.Web.UI.Page
    {
        protected SAAdminTool.DocsPaWR.Templates modelloSelezionato;
        protected SAAdminTool.DocsPaWR.Templates modelloIperfascicolo;
        //private SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();
		
        protected void Page_Load(object sender, EventArgs e) 
        {
            modelloSelezionato = (SAAdminTool.DocsPaWR.Templates)Session["template"];
            lbl_titolo.Text = "Campi Comuni - " + modelloSelezionato.DESCRIZIONE;

            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione,this);
            ArrayList listaTemplates = new ArrayList(ProfilazioneFascManager.getTemplatesFasc(idAmministrazione,this));

            for (int i = 0; i < listaTemplates.Count; i++)
            {
                SAAdminTool.DocsPaWR.Templates  modello = (SAAdminTool.DocsPaWR.Templates)listaTemplates[i]; 
                if (modello.IPER_FASC_DOC == "1")
                {
                    modelloIperfascicolo = ProfilazioneFascManager.getTemplateFascById(modello.SYSTEM_ID.ToString(),this);
                    break;
                }
            }

            if (!IsPostBack)
            {
                caricaDg();
                impostaSelezioneCampiComuni();    
            }
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chiudi", "window.close();", true);
        }

        protected void btn_conferma_Click(object sender, EventArgs e)
        {
            ArrayList campiComuniSelezionati = new ArrayList();
            
            for (int i = 0; i < gw_CampiComuni.Rows.Count; i++)
            {
                if (((System.Web.UI.WebControls.CheckBox)gw_CampiComuni.Rows[i].Cells[2].Controls[1]).Checked)
                {
                    SAAdminTool.DocsPaWR.OggettoCustom oggCustomDaAssociare = (SAAdminTool.DocsPaWR.OggettoCustom)modelloIperfascicolo.ELENCO_OGGETTI[i];
                    campiComuniSelezionati.Add(oggCustomDaAssociare);
                }
            }

            SAAdminTool.DocsPaWR.OggettoCustom[] campiComuniSelezionati_1 = new SAAdminTool.DocsPaWR.OggettoCustom[campiComuniSelezionati.Count];
            campiComuniSelezionati.CopyTo(campiComuniSelezionati_1);

            SAAdminTool.DocsPaWR.Templates template = ProfilazioneFascManager.impostaCampiComuniFasc(modelloSelezionato, campiComuniSelezionati_1,this);
            if (template != null)
            {
                Session.Add("selezioneCampiComuni", true);
                Session["template"] = template;
                ClientScript.RegisterStartupScript(this.GetType(), "chiudiPopup", "window.close();", true);
            }
        }

        private void caricaDg()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID_CAMPO");
            dt.Columns.Add("DESCRIZIONE");
            for (int i = 0; i < modelloIperfascicolo.ELENCO_OGGETTI.Length; i++)
            {
                SAAdminTool.DocsPaWR.OggettoCustom oggettoCustom = (SAAdminTool.DocsPaWR.OggettoCustom) modelloIperfascicolo.ELENCO_OGGETTI[i];
                DataRow rw = dt.NewRow();
                rw[0] = oggettoCustom.SYSTEM_ID;
                rw[1] = oggettoCustom.DESCRIZIONE;
                dt.Rows.Add(rw);
            }

            dt.AcceptChanges();
            gw_CampiComuni.DataSource = dt;
            gw_CampiComuni.DataBind();
            gw_CampiComuni.Visible = true;
            gw_CampiComuni.Columns[0].Visible = false;

            ////Associo l'evento alle checkBox
            //for (int i = 0; i < gw_CampiComuni.Rows.Count; i++)
            //{
            //    ((System.Web.UI.WebControls.CheckBox)gw_CampiComuni.Rows[i].Cells[2].Controls[1]).CheckedChanged += new EventHandler(cb_selezione_CheckedChanged);
            //}            
        }

        protected void cb_selezione_CheckedChanged(object sender, EventArgs e)
        {
            //Gestisco la mutua esclusione delle checkbox
            CheckBox cb = (CheckBox)sender;
            GridViewRow row = (GridViewRow)cb.NamingContainer;
            int rigaSelezionata = row.RowIndex;

            for (int i = 0; i < gw_CampiComuni.Rows.Count; i++)
            {
                if (i != rigaSelezionata)
                    ((System.Web.UI.WebControls.CheckBox)gw_CampiComuni.Rows[i].Cells[2].Controls[1]).Checked = false;
            }           
        }

        private void impostaSelezioneCampiComuni()
        {
            for (int i = 0; i < modelloSelezionato.ELENCO_OGGETTI.Length; i++)
            {
                SAAdminTool.DocsPaWR.OggettoCustom oggModelloSelezionato = (SAAdminTool.DocsPaWR.OggettoCustom)modelloSelezionato.ELENCO_OGGETTI[i];
                if (oggModelloSelezionato.CAMPO_COMUNE == "1")
                {
                    for (int j = 0; j < modelloIperfascicolo.ELENCO_OGGETTI.Length; j++)
                    {
                        SAAdminTool.DocsPaWR.OggettoCustom oggModelloIperfascicolo = (SAAdminTool.DocsPaWR.OggettoCustom)modelloIperfascicolo.ELENCO_OGGETTI[j];
                        if (oggModelloIperfascicolo.SYSTEM_ID == oggModelloSelezionato.SYSTEM_ID)
                        {
                            ((System.Web.UI.WebControls.CheckBox)gw_CampiComuni.Rows[j].Cells[2].Controls[1]).Checked = true;
                        }
                    }
                }
            }
        }
    }
}
