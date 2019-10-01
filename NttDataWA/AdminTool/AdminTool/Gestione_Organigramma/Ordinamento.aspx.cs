using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.Web.UI.WebControls;

namespace Amministrazione.Gestione_Organigramma
{
    public partial class Ordinamento : System.Web.UI.Page
    {
        protected DataSet dsOrdRuoli;
        protected DataSet dsOrdUOInf;
        protected string idUO = string.Empty;
        protected string idLivello = string.Empty;
        protected string idAmm = string.Empty;
        protected string descUO = string.Empty;
        protected int indiceDG_UO_Selezionato = 0;
        protected int indiceDG_RUOLO_Selezionato = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Buffer = true;
            Response.Expires = -1;

            if(!IsPostBack)
                this.Inizialize();
        }

        private void Inizialize()
        {
            idUO = Request.QueryString["idUo"].ToString();
            idLivello = Request.QueryString["idLivello"].ToString();
            idAmm = Request.QueryString["idAmm"].ToString();
            descUO = Request.QueryString["descUO"].ToString();

            this.lbl_desc_uo.Text = descUO;

            // RUOLI
            this.FillOrdinamentoRuoli();

            // UO INFERIORI				
            this.FillOrdinamentoUO();
        }

        private void FillOrdinamentoRuoli()
        {
            Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();            
            theManager.ListaRuoliUO(idUO);

            ArrayList lista = new ArrayList();
            lista = theManager.getListaRuoliUO();

            if (lista != null && lista.Count > 0)
            {
                InitializeDataSetOrdinamentoRuoli();
                DataRow row;
                foreach (SAAdminTool.DocsPaWR.OrgRuolo ruolo in lista)
                {
                    row = dsOrdRuoli.Tables[0].NewRow();
                    row["idCorrGlobale"] = ruolo.IDCorrGlobale;
                    row["idPeso"] = ruolo.IDPeso;
                    row["descrizione"] = ruolo.Descrizione;
                    dsOrdRuoli.Tables["ORD_RUOLI"].Rows.Add(row);
                }

                //DataView dv = dsOrdRuoli.Tables["ORD_RUOLI"].DefaultView;
                //dv.Sort = "idPeso ASC";
                //dv = OrdinaGrid(didPesov, "");
                //this.dg_ord_ruoli.DataSource = dv;
                this.dg_ord_ruoli.DataSource = dsOrdRuoli;
                this.dg_ord_ruoli.DataBind();

                this.dg_ord_ruoli.SelectedIndex = this.indiceDG_RUOLO_Selezionato;

                this.GestioneFrecceOrdRuoli();
            }
        }

        private void FillOrdinamentoUO()
        {
            int livello = Convert.ToInt32(idLivello) + 1;

            Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
            theManager.ListaUO(idUO, idLivello, idAmm);

            ArrayList lista = new ArrayList();
            lista = theManager.getListaUO();

            if (lista != null && lista.Count > 0)
            {
                InitializeDataSetOrdinamentoUO();
                DataRow row;

                foreach (SAAdminTool.DocsPaWR.OrgUO uo in lista)
                {
                    row = dsOrdUOInf.Tables[0].NewRow();
                    row["idCorrGlobale"] = uo.IDCorrGlobale;
                    row["idPeso"] = uo.IDPeso;
                    row["descrizione"] = uo.Descrizione;
                    dsOrdUOInf.Tables["ORD_UO"].Rows.Add(row);
                }

                //DataView dv = dsOrdUOInf.Tables["ORD_UO"].DefaultView;
                //dv.Sort = "idPeso ASC";
                //dv = OrdinaGrid(dv, "idPeso");
                //this.dg_ord_uo.DataSource = dv;
                this.dg_ord_uo.DataSource = dsOrdUOInf;
                this.dg_ord_uo.DataBind();

                this.dg_ord_uo.SelectedIndex = this.indiceDG_UO_Selezionato;

                this.GestioneFrecceOrdUO();
            }
        }

        private void GestioneFrecceOrdRuoli()
        {
            if (this.dg_ord_ruoli.Items.Count.Equals(1))
            {
                this.dg_ord_ruoli.Visible = false;
            }

            if (this.dg_ord_ruoli.Items.Count > 1)
            {
                this.dg_ord_ruoli.Items[0].Cells[3].Text = ""; // up
                this.dg_ord_ruoli.Items[this.dg_ord_ruoli.Items.Count - 1].Cells[4].Text = ""; // down                
            }
        }

        private void GestioneFrecceOrdUO()
        {
            if (this.dg_ord_uo.Items.Count.Equals(1))
            {
                this.dg_ord_uo.Visible = false;
            }

            if (this.dg_ord_uo.Items.Count > 1)
            {
                this.dg_ord_uo.Items[0].Cells[3].Text = ""; // up
                this.dg_ord_uo.Items[this.dg_ord_uo.Items.Count - 1].Cells[4].Text = ""; // down                
            }
        }       

        private void PerformUpDown(string tipoCorr, string mode, DataGridItem item)
        {
            DataGrid dg = new DataGrid();

            string idCorrGlobDaSpostare = string.Empty;
            string idPesoDaSpostare = string.Empty;

            string idCorrGlobSubisce = string.Empty;
            string idPesoSubisce = string.Empty;

            idCorrGlobDaSpostare = item.Cells[0].Text;
            idPesoDaSpostare = item.Cells[1].Text;

            int indiceCorrente = item.ItemIndex;
            int indiceDaSelezionare = 0;

            switch (tipoCorr)
            {
                case "RUOLO":
                    dg = this.dg_ord_ruoli;
                    break;
                case "UO":
                    dg = this.dg_ord_uo;
                    break;
            }   

            switch (mode)
            {
                case "UP":
                    indiceDaSelezionare = indiceCorrente - 1;
                    idCorrGlobSubisce = dg.Items[indiceDaSelezionare].Cells[0].Text;
                    idPesoSubisce = dg.Items[indiceDaSelezionare].Cells[1].Text;                   
                    break;
                case "DOWN":
                    indiceDaSelezionare = indiceCorrente + 1;
                    idCorrGlobSubisce = dg.Items[indiceDaSelezionare].Cells[0].Text;
                    idPesoSubisce = dg.Items[indiceDaSelezionare].Cells[1].Text;                   
                    break;
            }            

            Amministrazione.Manager.OrganigrammaManager manager = new Amministrazione.Manager.OrganigrammaManager();
            manager.PerformUpDown(idCorrGlobDaSpostare, idPesoDaSpostare, idCorrGlobSubisce, idPesoSubisce);
            
            SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();
            esito = manager.getEsitoOperazione();

            if (esito.Codice == 0)
            {
                switch (tipoCorr)
                {
                    case "RUOLO":
                        this.indiceDG_RUOLO_Selezionato = indiceDaSelezionare;
                        break;
                    case "UO":
                        this.indiceDG_UO_Selezionato = indiceDaSelezionare;
                        break;
                }   
                this.Inizialize();                
            }
            else
            {
                if (!ClientScript.IsStartupScriptRegistered("openAlert"))
                {
                    string scriptString = "<SCRIPT>alert('Attenzione, " + esito.Descrizione + "');</SCRIPT>";
                    ClientScript.RegisterStartupScript(GetType(), "openAlert", scriptString);
                }   
            }
        }


        private void InitializeDataSetOrdinamentoRuoli()
        {
            dsOrdRuoli = new DataSet();
            DataColumn dc;
            dsOrdRuoli.Tables.Add("ORD_RUOLI");
            dc = new DataColumn("idCorrGlobale");
            dsOrdRuoli.Tables["ORD_RUOLI"].Columns.Add(dc);
            dc = new DataColumn("idPeso");
            dsOrdRuoli.Tables["ORD_RUOLI"].Columns.Add(dc);
            dc = new DataColumn("descrizione");
            dsOrdRuoli.Tables["ORD_RUOLI"].Columns.Add(dc);
        }

        private void InitializeDataSetOrdinamentoUO()
        {
            dsOrdUOInf = new DataSet();
            DataColumn dc;
            dsOrdUOInf.Tables.Add("ORD_UO");
            dc = new DataColumn("idCorrGlobale");
            dsOrdUOInf.Tables["ORD_UO"].Columns.Add(dc);
            dc = new DataColumn("idPeso");
            dsOrdUOInf.Tables["ORD_UO"].Columns.Add(dc);
            dc = new DataColumn("descrizione");
            dsOrdUOInf.Tables["ORD_UO"].Columns.Add(dc);
        }

        private DataView OrdinaGrid(DataView dv, string sortColumn)
        {
            string[] words = dv.Sort.Split(' ');
            string sortMode;
            if (words.Length < 2)
            {
                sortMode = " ASC";
            }
            else
            {
                if (words[1].Equals("ASC"))
                {
                    sortMode = " DESC";
                }
                else
                {
                    sortMode = " ASC";
                }
            }
            dv.Sort = sortColumn + sortMode;
            return dv;
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            if (!ClientScript.IsStartupScriptRegistered("closeModal"))
            {
                string scriptString = "<SCRIPT>self.close();</SCRIPT>";
                ClientScript.RegisterStartupScript(GetType(), "closeModal", scriptString);
            }
        }

        protected void dg_ord_uo_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            this.PerformUpDown("UO", e.CommandName.ToUpper(), e.Item);
            //this.indiceDG_UO_Selezionato = e.Item.ItemIndex;
            //this.dg_ord_uo.SelectedIndex = this.indiceDG_UO_Selezionato;
        }

        protected void dg_ord_ruoli_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            this.PerformUpDown("RUOLO", e.CommandName.ToUpper(), e.Item);
            //this.indiceDG_RUOLO_Selezionato = e.Item.ItemIndex;
            //this.dg_ord_ruoli.SelectedIndex = this.indiceDG_RUOLO_Selezionato;
        }
    }
}
