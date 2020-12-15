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

namespace DocsPAWA.popup
{
    public partial class sceltaNodoTitolario : DocsPAWA.CssPage
    {
        private DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
		
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string queryString = this.Server.UrlDecode(Request.QueryString[0]);
                if (queryString != null && queryString != "")
                {
                    string[] parametri = queryString.Split('&');
                    string tipoChiamata = (parametri[parametri.Length-1].Split('='))[1];

                    switch (tipoChiamata)
                    {
                        case"IndiceSistematico":
                            string indice = (parametri[0].Split('='))[1];
                            string idTitolarioIndice = (parametri[1].Split('='))[1];

                            //Recupero la lista dei nodi associati all'indice selezionato
                            ArrayList listaNodiIndice = new ArrayList(wws.getCodNodiByIndice(indice, idTitolarioIndice));
                            caricaDg(listaNodiIndice,tipoChiamata);
                            break;

                        case"ProtocolloTitolario":
                            string numProtTit = (parametri[0].Split('='))[1];
                            string idTitolarioNumProtTit = (parametri[1].Split('='))[1];

                            //Recupero la lista dei nodi associati al numero di protocollo titolario
                            ArrayList listaNodiNumProtTit = new ArrayList(wws.getNodiFromProtoTit(UserManager.getRegistroSelezionato(this), UserManager.getUtente(this).idAmministrazione, numProtTit, idTitolarioNumProtTit));
                            caricaDg(listaNodiNumProtTit, tipoChiamata);
                            break;
                    }
                }               
            }
        }

        private void caricaDg(ArrayList listaNodi, string tipoChiamata)
        {
            gw_Nodi.Columns[0].Visible = true;
            
            DataTable dt = new DataTable();
            dt.Columns.Add("ID_TITOLARIO");
            dt.Columns.Add("TITOLARIO");
            dt.Columns.Add("CODICE_NODO");
            dt.Columns.Add("NODO");
            dt.Columns.Add("REGISTRO");
            dt.Columns.Add("NUM_PROTO_TIT");

            switch (tipoChiamata)
            {
                case"IndiceSistematico":
                    
                    for (int i = 0; i < listaNodi.Count; i++)
                    {
                        DocsPaWR.VoceIndiceSistematico voceIndice = (DocsPaWR.VoceIndiceSistematico)listaNodi[i];
                        DataRow rw = dt.NewRow();
                        rw["ID_TITOLARIO"] = voceIndice.idTitolario;
                        DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(voceIndice.idTitolario);
                        rw["TITOLARIO"] = titolario.DescrizioneLite;
                        rw["CODICE_NODO"] = voceIndice.codiceNodo;
                        rw["NODO"] = voceIndice.descrizioneNodo;
                        rw["REGISTRO"] = string.Empty;
                        rw["NUM_PROTO_TIT"] = string.Empty;
                        dt.Rows.Add(rw);
                    }

                    dt.AcceptChanges();
                    gw_Nodi.DataSource = dt;
                    gw_Nodi.DataBind();
                    gw_Nodi.Visible = true;
                    gw_Nodi.Columns[0].Visible = false;
                    gw_Nodi.Columns[4].Visible = false;
                    gw_Nodi.Columns[5].Visible = false;
                    break;

                case"ProtocolloTitolario":
                    for (int i = 0; i < listaNodi.Count; i++)
                    {
                        DocsPaWR.OrgNodoTitolario nodoTitolario = (DocsPaWR.OrgNodoTitolario)listaNodi[i];
                        DataRow rw = dt.NewRow();
                        rw["ID_TITOLARIO"] = nodoTitolario.ID_Titolario;
                        DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(nodoTitolario.ID_Titolario);
                        rw["TITOLARIO"] = titolario.DescrizioneLite;
                        rw["CODICE_NODO"] = nodoTitolario.Codice;
                        rw["NODO"] = nodoTitolario.Descrizione;
                        if (!string.IsNullOrEmpty(nodoTitolario.IDRegistroAssociato))
                        {
                            DocsPAWA.DocsPaWR.Registro reg = UserManager.getRegistroBySistemId(this, nodoTitolario.IDRegistroAssociato);
                            if (reg != null)
                                rw["REGISTRO"] = reg.descrizione;
                            else
                                rw["REGISTRO"] = string.Empty;
                        }
                        else
                        {
                            rw["REGISTRO"] = string.Empty;
                        }
                        rw["NUM_PROTO_TIT"] = nodoTitolario.numProtoTit;
                        string stringContatoreTitolario = wws.isEnableContatoreTitolario();
                        if (!string.IsNullOrEmpty(stringContatoreTitolario))
                            gw_Nodi.Columns[5].HeaderText = stringContatoreTitolario;
                        else
                            gw_Nodi.Columns[5].HeaderText = string.Empty;
                        dt.Rows.Add(rw);
                    }

                    dt.AcceptChanges();
                    gw_Nodi.DataSource = dt;
                    gw_Nodi.DataBind();
                    gw_Nodi.Visible = true;
                    gw_Nodi.Columns[0].Visible = false;
                    break;
            }            

            //Associo l'evento alle checkBox
            for (int i = 0; i < gw_Nodi.Rows.Count; i++)
            {
                ((System.Web.UI.WebControls.CheckBox)gw_Nodi.Rows[i].Cells[6].Controls[1]).CheckedChanged += new EventHandler(cb_selezione_CheckedChanged);
            }
        }

        protected void btn_Conferma_Click(object sender, EventArgs e)
        {
            string idTitolarioSelezionato = string.Empty;
            string codiceNodoSelezionato = string.Empty;

            for (int i = 0; i < gw_Nodi.Rows.Count; i++)
            {
                if (((System.Web.UI.WebControls.CheckBox)gw_Nodi.Rows[i].Cells[6].Controls[1]).Checked)
                {
                    idTitolarioSelezionato = gw_Nodi.Rows[i].Cells[0].Text;
                    codiceNodoSelezionato = gw_Nodi.Rows[i].Cells[2].Text;
                    break;
                }
            }

            if (idTitolarioSelezionato != null && idTitolarioSelezionato != "" && codiceNodoSelezionato != null && codiceNodoSelezionato != "")
            {
                Session.Add("idTitolarioSelezionato", idTitolarioSelezionato);
                Session.Add("codiceNodoSelezionato", codiceNodoSelezionato);
                ClientScript.RegisterStartupScript(this.GetType(), "chiudeFinestra", "window.close();", true);
            }
        }

        protected void btn_Chiudi_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chiudeFinestra", "window.close();", true);
            Session.Remove("idTitolarioSelezionato");
            Session.Remove("codiceNodoSelezionato");
        }

        protected void cb_selezione_CheckedChanged(object sender, EventArgs e)
        {
            //Gestisco la mutua esclusione delle checkbox
            CheckBox cb = (CheckBox)sender;
            GridViewRow row = (GridViewRow)cb.NamingContainer;
            int rigaSelezionata = row.RowIndex;

            for (int i = 0; i < gw_Nodi.Rows.Count; i++)
            {
                if (i != rigaSelezionata)
                    ((System.Web.UI.WebControls.CheckBox)gw_Nodi.Rows[i].Cells[6].Controls[1]).Checked = false;
            }
        }    

    }
}
