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
using System.Xml;
using Microsoft.Web.UI.WebControls;

namespace SAAdminTool.AdminTool.Gestione_Organigramma.Gestione_Ruolo_Titolario
{
    public partial class GestRuoloTitolario : System.Web.UI.Page
    {
        #region CLASSE TreeNodeTitolario
        public class TreeNodeTitolario : Microsoft.Web.UI.WebControls.TreeNode
        {
            public void SetNodoTitolario(SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario, bool ruoloAssociato)
            {
                this.SetViewStateItem("ID", nodoTitolario.ID);
                this.SetViewStateItem("Codice", nodoTitolario.Codice);
                this.SetViewStateItem("Descrizione", nodoTitolario.Descrizione);
                this.SetViewStateItem("CodiceAmministrazione", nodoTitolario.CodiceAmministrazione);
                this.SetViewStateItem("CodiceLivello", nodoTitolario.CodiceLivello);
                this.SetViewStateItem("CountChildNodiTitolario", nodoTitolario.CountChildNodiTitolario.ToString());
                this.SetViewStateItem("IDParentNodoTitolario", nodoTitolario.IDParentNodoTitolario);
                this.SetViewStateItem("IDRegistroAssociato", nodoTitolario.IDRegistroAssociato);
                this.SetViewStateItem("Livello", nodoTitolario.Livello);                
                //NuovaGestioneTitolario
                this.SetViewStateItem("IDTitolario", nodoTitolario.ID_Titolario.ToString());
                this.SetViewStateItem("DataAttivazione", nodoTitolario.dataAttivazione.ToString());
                this.SetViewStateItem("DataCessazione", nodoTitolario.dataCessazione.ToString());
                this.SetViewStateItem("Stato", nodoTitolario.stato.ToString());
                
                this.SetNodeDescription(nodoTitolario, ruoloAssociato);
            }

            public SAAdminTool.DocsPaWR.OrgNodoTitolario GetNodoTitolario()
            {
                SAAdminTool.DocsPaWR.OrgNodoTitolario retValue = null;

                string itemValue = this.GetViewStateItem("ID");

                if (itemValue != string.Empty && itemValue != "0")
                {
                    retValue = new SAAdminTool.DocsPaWR.OrgNodoTitolario();
                    retValue.ID = itemValue;

                    retValue.Codice = this.GetViewStateItem("Codice");
                    retValue.Descrizione = this.GetViewStateItem("Descrizione");
                    retValue.CodiceAmministrazione = this.GetViewStateItem("CodiceAmministrazione");
                    retValue.CodiceLivello = this.GetViewStateItem("CodiceLivello");

                    itemValue = this.GetViewStateItem("CountChildNodiTitolario");
                    if (itemValue != string.Empty)
                        retValue.CountChildNodiTitolario = Convert.ToInt32(itemValue);
                    
                    retValue.IDParentNodoTitolario = this.GetViewStateItem("IDParentNodoTitolario");
                    retValue.IDRegistroAssociato = this.GetViewStateItem("IDRegistroAssociato");
                    retValue.Livello = this.GetViewStateItem("Livello");
                   
                    //NuovaGestioneTitolario
                    retValue.ID_Titolario = this.GetViewStateItem("IDTitolario");
                    retValue.stato = this.GetViewStateItem("Stato");
                    retValue.dataAttivazione = this.GetViewStateItem("DataAttivazione");
                    retValue.dataCessazione = this.GetViewStateItem("DataCessazione");
                }
                return retValue;
            }

            private void SetNodeDescription(SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario, bool ruoloAssociato)
            {
                string identifReg = string.Empty;

                this.ID = nodoTitolario.ID;

                //Verifico se è un titolario per aggiornare la descrizione
                //in funzione dello stato
                if (nodoTitolario.Livello == "0" &&
                    nodoTitolario.IDParentNodoTitolario == "0" &&
                    nodoTitolario.Codice == "T" &&
                    nodoTitolario.ID_Titolario == "0")
                {
                    switch (nodoTitolario.stato)
                    {
                        case "A":
                            this.Text = nodoTitolario.Codice + " - " + nodoTitolario.Descrizione + " Attivo";
                            break;

                        case "D":
                            this.Text = nodoTitolario.Codice + " - " + nodoTitolario.Descrizione + " in definizione";
                            break;

                        case "C":
                            this.Text = nodoTitolario.Codice + " - " + nodoTitolario.Descrizione + " in vigore dal " + nodoTitolario.dataAttivazione + " al " + nodoTitolario.dataCessazione;
                            break;
                    }
                }
                else
                {
                    //if (nodoTitolario.IDRegistroAssociato != null && nodoTitolario.IDRegistroAssociato != string.Empty)
                    //    identifReg = "[R] ";
                    //else
                    //    identifReg = "";

                    if(ruoloAssociato)
                    {
                        this.Text = "<b>" + nodoTitolario.Codice + " - " + identifReg + nodoTitolario.Descrizione + "</b>";
                    }
                    else
                    {
                        this.Text = nodoTitolario.Codice + " - " + identifReg + nodoTitolario.Descrizione;
                    }
                }
            }

            private void SetViewStateItem(string key, string value)
            {
                ViewState[key] = value;
            }

            private string GetViewStateItem(string key)
            {
                if (ViewState[key] != null)
                    return ViewState[key].ToString();
                else
                    return string.Empty;
            }
        }
        #endregion

        private const string FILTER_TYPE_CODICE = "CODICE";
        private const string FILTER_TYPE_DESCRIZIONE = "DESCRIZIONE";

        private const string TABLE_COL_ID = "ID_RECORD";
        private const string TABLE_COL_CODICE = "CODICE";
        private const string TABLE_COL_DESCRIZIONE = "DESCRIZIONE";
        private const string TABLE_COL_LIVELLO = "LIVELLO";
        private const string TABLE_COL_CODLIV = "CODLIV";
        private const string TABLE_COL_IDPARENT = "ID_PARENT";
        //protected System.Web.UI.WebControls.Label lbl_agg_nodo;
        //protected System.Web.UI.WebControls.Label lbl_agg_nodo_figli;
        //protected System.Web.UI.WebControls.Label lbl_elimina;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.ImpostaCampiQS();
                    //this.FillVisibAttuale();
                    this.FillComboTipoRicerca();

                    // verifica se proviene dalla ricerca 
                    if (Request.QueryString["azione"] == "ricerca")
                    {
                        this.FillComboRegistri(Request.QueryString["idregistro"]);

                        this.FillTreeView(Request.QueryString["idregistro"]);

                        this.FindNodoTitolario(Request.QueryString["idrecord"],
                                                    Request.QueryString["idparent"],
                                                    Convert.ToInt32(Request.QueryString["livello"]) + 1);
                    }
                    else
                    {
                        this.FillComboRegistri(null);
                        this.FillTreeView(this.ddl_registri.SelectedValue.ToString());
                    }
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore, impossibile procedere nella funzionalità");
            }
        }

        private void ImpostaCampiQS()
        {
            if (Request.QueryString["idCorrGlobRuolo"] != null)
            {
                this.hd_idCorrGlobRuolo.Value = Request.QueryString["idCorrGlobRuolo"].ToString();
            }

            if (Request.QueryString["idGruppo"] != null)
            {
                this.hd_idGruppo.Value = Request.QueryString["idGruppo"].ToString();
            }

            if (Request.QueryString["idAmm"] != null)
            {
                this.hd_idAmm.Value = Request.QueryString["idAmm"].ToString();
            }

            if (Request.QueryString["descRuolo"] != null)
            {
                //this.lbl_ruolo.Text = Request.QueryString["descRuolo"].ToString();
                this.lbl_ruolo_new.Text = Request.QueryString["descRuolo"].ToString();
            }

            if (Request.QueryString["codRuolo"] != null)
            {
                this.hd_codRuolo.Value = Request.QueryString["codRuolo"].ToString();
            }
        }

        private void FillVisibAttuale()
        {
            DocsPaWR.OrgNodoTitolario[] nodiAttuali = this.GetNodiAttualiAssociati();
            DataSet ds = this.ConvertToDataSet(nodiAttuali);

            this.dg_visibAttuale.DataSource = ds;
            this.dg_visibAttuale.DataBind();

            ds.Dispose();
            ds = null;

            nodiAttuali = null;		
        }

        private DataSet ConvertToDataSet(SAAdminTool.DocsPaWR.OrgNodoTitolario[] nodi)
        {
            DataSet ds = this.CreateGridDataSet();
            DataTable dt = ds.Tables["VisibAttuale"];

            foreach (SAAdminTool.DocsPaWR.OrgNodoTitolario nodo in nodi)
            {
                DataRow row = dt.NewRow();

                row[TABLE_COL_ID] = nodo.ID;
                row[TABLE_COL_CODICE] = nodo.Codice;
                row[TABLE_COL_DESCRIZIONE] = "<b>" + nodo.Codice + "</b><br>" + nodo.Descrizione;
                row[TABLE_COL_LIVELLO] = nodo.Livello;
                row[TABLE_COL_CODLIV] = nodo.CodiceLivello;
                row[TABLE_COL_IDPARENT] = nodo.IDParentNodoTitolario;

                dt.Rows.Add(row);
            }

            return ds;
        }

        private DataSet CreateGridDataSet()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("VisibAttuale");

            dt.Columns.Add(new DataColumn(TABLE_COL_ID));
            dt.Columns.Add(new DataColumn(TABLE_COL_CODICE));
            dt.Columns.Add(new DataColumn(TABLE_COL_DESCRIZIONE));
            dt.Columns.Add(new DataColumn(TABLE_COL_LIVELLO));
            dt.Columns.Add(new DataColumn(TABLE_COL_CODLIV));
            dt.Columns.Add(new DataColumn(TABLE_COL_IDPARENT));

            ds.Tables.Add(dt);
            return ds;
        }

        private void FillTreeView(string idRegistro)
        {
            if (this.trvNodiTitolario.Nodes.Count > 0)
                this.trvNodiTitolario.Nodes.Clear();

            this.AddRootNodes(idRegistro);
        }

        private Microsoft.Web.UI.WebControls.TreeNode AddRootNodes(string idRegistro)
        {
            Microsoft.Web.UI.WebControls.TreeNode rootNode = new TreeNodeTitolario();
            rootNode.ID = "0";
            rootNode.Text = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
            rootNode.Expanded = true;
            trvNodiTitolario.Nodes.Add(rootNode);

            this.FillTreeNodes((TreeNodeTitolario)rootNode, idRegistro);

            return rootNode;
        }

        private void FillTreeNodes(TreeNodeTitolario parentNode, string idRegistro)
        {
            if (parentNode.Nodes.Count > 0)
                parentNode.Nodes.Clear();

            // Reperimento dei nodi di titolario 
            SAAdminTool.DocsPaWR.OrgNodoTitolario[] nodiTitolario = this.GetNodiTitolario(parentNode.ID, idRegistro);

            // Rimozione del nodo inserito per l'attesa del caricamento
            if (parentNode.Nodes.Count > 0 && parentNode.Nodes[0].ID == "WAITING_NODE")
                parentNode.Nodes.Remove(parentNode.Nodes[0]);

            foreach (SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario in nodiTitolario)
            {
                TreeNodeTitolario treeNodeTitolario = this.CreateNewTreeNodeTitolario(nodoTitolario);
                parentNode.Nodes.Add(treeNodeTitolario);

                if (nodoTitolario.CountChildNodiTitolario > 0)
                {
                    // Nodo immesso per l'attesa del caricamento
                    Microsoft.Web.UI.WebControls.TreeNode childNodeTitolario = new TreeNodeTitolario();
                    childNodeTitolario.ID = "WAITING_NODE";
                    childNodeTitolario.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                    treeNodeTitolario.Nodes.Add(childNodeTitolario);
                }
            }

            nodiTitolario = null;
        }

        private TreeNodeTitolario CreateNewTreeNodeTitolario(SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario)
        {
            TreeNodeTitolario retValue = new TreeNodeTitolario();
            DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();

            bool ruoloAssociato = (ws.AmmGetVisibNodoTit_InRuolo(nodoTitolario.ID, this.hd_idGruppo.Value) != null);

            ((TreeNodeTitolario)retValue).SetNodoTitolario(nodoTitolario, ruoloAssociato);

            return retValue;
        }

        private SAAdminTool.DocsPaWR.OrgNodoTitolario[] GetNodiTitolario(string idParentTitolario, string idRegistro)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.AmmGetNodiTitolario(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"), idParentTitolario, idRegistro);
        }

        private void FillComboTipoRicerca()
        {
            if (this.cboTipoRicerca.Items.Count > 0)
                this.cboTipoRicerca.Items.Clear();

            this.cboTipoRicerca.Items.Add(new ListItem("Codice", FILTER_TYPE_CODICE));
            this.cboTipoRicerca.Items.Add(new ListItem("Descrizione", FILTER_TYPE_DESCRIZIONE));
        }

        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!ClientScript.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
            }
        }

        private void ShowErrorMessage(string errorMessage)
        {
            this.RegisterClientScript("ErrorMessage", "alert('" + errorMessage.Replace("'", " ") + "')");
        }

        private SAAdminTool.DocsPaWR.OrgNodoTitolario GetCurrentTitolario()
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario retValue = null;

            if (this.trvNodiTitolario.SelectedNodeIndex.Length > 3)
            {
                TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
                retValue = selectedNode.GetNodoTitolario();
                selectedNode = null;
            }
            return retValue;            
        }
        
      
        private SAAdminTool.DocsPaWR.OrgNodoTitolario GetParentTitolario()
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario retValue = null;

            if (this.trvNodiTitolario.SelectedNodeIndex != "0")
            {
                TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
                TreeNodeTitolario parentNode = (TreeNodeTitolario)selectedNode.Parent;

                if (parentNode != null)
                    retValue = parentNode.GetNodoTitolario();
            }

            return retValue;
        }

        private GestRuoloTitolario.TreeNodeTitolario GetNodeFromIndex(string nodeIndex)
        {
            return this.trvNodiTitolario.GetNodeFromIndex(nodeIndex) as GestRuoloTitolario.TreeNodeTitolario;
        }

        private string getRegistroSelezionato()
        {
            string retValue = string.Empty;
            retValue = this.ddl_registri.SelectedValue;
            return retValue;
        }        

        public void FillComboRegistri(string idRegistro)
        {
            try
            {
                this.ddl_registri.Items.Clear();

                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                ArrayList listaRegistri = new ArrayList();
                string descReg;
                string idReg;
                
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.ListaRegistriAssRuolo(this.hd_idAmm.Value, this.hd_idCorrGlobRuolo.Value);
                int totRegistri = theManager.getListaRegistri().Count;
                if (totRegistri > 0)
                {        
                    if(totRegistri > 1)
                        this.ddl_registri.Items.Add(new ListItem("Tutti i registri", string.Empty));

                    foreach (SAAdminTool.DocsPaWR.OrgRegistro registro in theManager.getListaRegistri())
                    {
                        descReg = registro.Descrizione;
                        idReg = registro.IDRegistro;

                        ListItem item = new ListItem(descReg, idReg);
                        this.ddl_registri.Items.Add(item);
                    }
                }
                else
                {
                    this.ShowErrorMessage("Attenzione, nessun registro da amministrare.");
                    return;
                }

                if (idRegistro != null)
                {
                    ddl_registri.SelectedIndex = ddl_registri.Items.IndexOf(ddl_registri.Items.FindByValue(idRegistro));
                }
                else
                {
                    if (totRegistri == 1)
                    {
                        ddl_registri.SelectedIndex = 1;
                    }
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante il reperimento dati dei registri.");
            }
        }

        public DocsPaWR.OrgNodoTitolario[] GetNodiAttualiAssociati()
        {
            DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            return ws.AmmGetNodiTitolario_InRuolo(this.hd_idGruppo.Value);
        }

        protected void trvNodiTitolario_Expand1(object sender, TreeViewClickEventArgs e)
        {
            try
            {
                if (e.Node != "0")
                {
                    TreeNodeTitolario selectedNode = this.GetNodeFromIndex(e.Node);
                    
                    this.FillTreeNodes(selectedNode, this.getRegistroSelezionato());

                    this.trvNodiTitolario.SelectedNodeIndex = selectedNode.GetNodeIndex();

                    this.abilitazioneTasti();

                    selectedNode = null;
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }
        }

        protected void btn_add_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.trvNodiTitolario.SelectedNodeIndex != "")
                {
                    TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
                    if (selectedNode != null)
                    {
                        if (this.ExtendToNode(selectedNode.ID, this.hd_idGruppo.Value, true, true, false))
                            this.setBoldNode(true);

                        //this.FillVisibAttuale();
                    }
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore, impossibile procedere nella funzionalità");
            }
        }

        protected void btn_del_Click(object sender, EventArgs e)
        {
            bool AllTitolario = false;
            try
            {
                if (this.trvNodiTitolario.SelectedNodeIndex != "")
                {
                    TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
                    if (selectedNode != null)
                    {
                        SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario = selectedNode.GetNodoTitolario();
                        if (nodoTitolario.Livello.Equals("0") && nodoTitolario.Codice.Equals("T") && nodoTitolario.IDParentNodoTitolario.Equals("0"))
                            AllTitolario = true;
                        
                        if (this.ExtendToNode(selectedNode.ID, this.hd_idGruppo.Value, false, true, AllTitolario))
                            this.setBoldNode(false);

                        //this.FillVisibAttuale(); 
                    }
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore, impossibile procedere nella funzionalità");
            }
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            string key = "chiudiModal";
            string script = "<script>self.close()</script>";
            ClientScript.RegisterStartupScript(this.GetType(), key, script);            
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.SearchTitolario(this.cboTipoRicerca.SelectedItem.Value, this.txtFieldRicerca.Text);
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore, impossibile procedere nella funzionalità");
            }
        }       

        /// <summary>
        /// Ricerca del titolario in base ai parametri di filtro immessi
        /// </summary>
        /// <param name="searchType"></param>
        /// <param name="valueToSearch"></param>
        private void SearchTitolario(string searchType, string valueToSearch)
        {
            string myHtml = string.Empty;

            try
            {
                if (valueToSearch != string.Empty)
                {
                    valueToSearch = valueToSearch.Trim();

                    string codice = string.Empty;
                    string descrizione = string.Empty;

                    if (searchType == FILTER_TYPE_CODICE)
                        codice = valueToSearch;
                    else
                        descrizione = valueToSearch;

                    string codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");

                    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                    string xmlStream = ws.filtroRicerca(codice, descrizione, codAmm, this.getRegistroSelezionato());
                    //string xmlStream = ws.filtroRicerca(codice, descrizione, codAmm, null);
                    if (xmlStream == "<NewDataSet />")
                    {
                        this.ShowErrorMessage("Nessun risultato trovato");
                    }
                    else
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(xmlStream);

                        XmlNode lista = doc.SelectSingleNode("NewDataSet");
                        if (lista.ChildNodes.Count > 0)
                        {                           
                            foreach (XmlNode nodo in lista.ChildNodes)
                            {
                                myHtml += "<TR bgColor=#FAFAFA>";
                                myHtml += "<TD width='15%' class=testo_grigio_scuro><a href='GestRuoloTitolario.aspx?azione=ricerca&idrecord=" + nodo.SelectSingleNode("ID").InnerText + "&idparent=" + nodo.SelectSingleNode("IDPARENT").InnerText + "&idregistro=" + this.getRegistroSelezionato() + "&livello=" + nodo.SelectSingleNode("LIVELLO").InnerText + "&idCorrGlobRuolo=" + this.hd_idCorrGlobRuolo.Value + "&idGruppo=" + this.hd_idGruppo.Value + "&idAmm=" + this.hd_idAmm.Value + "&descRuolo=" + this.lbl_ruolo_new.Text + "&codRuolo=" + this.hd_codRuolo.Value + "' class='testo_grigio_scuro'>" + nodo.SelectSingleNode("CODICE").InnerText + "</a></TD>";
                                myHtml += "<TD width='80%' class=testo_grigio_scuro><a href='GestRuoloTitolario.aspx?azione=ricerca&idrecord=" + nodo.SelectSingleNode("ID").InnerText + "&idparent=" + nodo.SelectSingleNode("IDPARENT").InnerText + "&idregistro=" + this.getRegistroSelezionato() + "&livello=" + nodo.SelectSingleNode("LIVELLO").InnerText + "&idCorrGlobRuolo=" + this.hd_idCorrGlobRuolo.Value + "&idGruppo=" + this.hd_idGruppo.Value + "&idAmm=" + this.hd_idAmm.Value + "&descRuolo=" + this.lbl_ruolo_new.Text + "&codRuolo=" + this.hd_codRuolo.Value + "' class='testo_grigio_scuro'>" + nodo.SelectSingleNode("DESCRIZIONE").InnerText.Replace("'", "&#39;").Replace("\"", "&quot;") + "</a></TD>";
                                myHtml += "<TD width='5%' align=center class=testo_grigio_scuro><a href='GestRuoloTitolario.aspx?azione=ricerca&idrecord=" + nodo.SelectSingleNode("ID").InnerText + "&idparent=" + nodo.SelectSingleNode("IDPARENT").InnerText + "&idregistro=" + this.getRegistroSelezionato() + "&livello=" + nodo.SelectSingleNode("LIVELLO").InnerText + "&idCorrGlobRuolo=" + this.hd_idCorrGlobRuolo.Value + "&idGruppo=" + this.hd_idGruppo.Value + "&idAmm=" + this.hd_idAmm.Value + "&descRuolo=" + this.lbl_ruolo_new.Text + "&codRuolo=" + this.hd_codRuolo.Value + "' class='testo_grigio_scuro'>" + nodo.SelectSingleNode("LIVELLO").InnerText + "</a></TD>";
                                myHtml += "</TR>";
                            }
                            lbl_td.Text = myHtml;
                        }
                    }
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }
        }
        

        public void FindNodoTitolario(string idTitolario,
                                      string idParentTitolario,
                                      int livello)
        {
            try
            {
                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                string xmlStream = ws.RicercaNodoRoot(idTitolario, idParentTitolario, livello);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlStream);

                XmlNode lista = doc.SelectSingleNode("NewDataSet");

                if (lista.ChildNodes.Count > 0)
                {
                    string parentTreeNodeIndex = "0";

                    for (int n = 1; n <= livello; n++)
                    {
                        XmlNode liv = doc.SelectSingleNode(".//livello[text()='" + n.ToString() + "']");
                        XmlNode root = liv.ParentNode;
                        string id = root.ChildNodes.Item(0).InnerText;

                        // Caricamento nodi titolario figli e reperimento 
                        // dell'indice del nodo del treeview
                        parentTreeNodeIndex = this.ExpandNode(id, parentTreeNodeIndex);
                    }

                    // Impostazione nodo corrente
                    trvNodiTitolario.SelectedNodeIndex = parentTreeNodeIndex;
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }
        }

        private string ExpandNode(string idTitolario, string parentTreeNodeIndex)
        {
            string retValue = string.Empty;

            try
            {
                TreeNodeTitolario parentNode = (TreeNodeTitolario)this.trvNodiTitolario.GetNodeFromIndex(parentTreeNodeIndex);

                foreach (TreeNodeTitolario nodeTitolario in parentNode.Nodes)
                {
                    if (nodeTitolario.GetNodoTitolario().ID == idTitolario)
                    {
                        // Reperimento indice del nodo
                        retValue = nodeTitolario.GetNodeIndex();

                        // Caricamento nodi titolario figli
                        this.FillTreeNodes(nodeTitolario, this.getRegistroSelezionato());

                        parentNode.Expanded = true;

                        break;
                    }
                }

            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }

            return retValue;
        }

        protected void dg_visibAttuale_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            string idrecord = e.Item.Cells[0].Text;
            string livello = e.Item.Cells[4].Text;
            string idparent = e.Item.Cells[6].Text;

            this.FillTreeView(this.ddl_registri.SelectedValue.ToString());
            this.FindNodoTitolario(idrecord,idparent,Convert.ToInt32(livello) + 1);
        }

        private bool isNodePresentInDataGrid(string id)
        {
            bool retValue = false;
            if (this.dg_visibAttuale.Items.Count > 0)
            {
                foreach (DataGridItem item in this.dg_visibAttuale.Items)
                {
                    if (id.Equals(item.Cells[0].Text))
                    {
                        retValue = true;
                        break;
                    }
                }
            }
            return retValue;
        }

        protected void btn_add_figli_Click(object sender, EventArgs e)
        {
            bool AllTitolario = false;
            try
            {
                if (this.trvNodiTitolario.SelectedNodeIndex != "")
                {
                    TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
                    if (selectedNode != null)
                    {
                        SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario = selectedNode.GetNodoTitolario();
                        if (nodoTitolario.Livello.Equals("0") && nodoTitolario.Codice.Equals("T") && nodoTitolario.IDParentNodoTitolario.Equals("0"))
                            AllTitolario = true;
                        //{
                        //    SAAdminTool.DocsPaWR.EsitoOperazione[] retValue = null;
                        //    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                        //    ArrayList ruoli = new ArrayList();
                        //    ruoli.Add(
                        //    retValue = ws.AmmUpdateRuoliTitolario(nodoTitolario.ID_Titolario, idAmm, true, changedRuoliTitolario, changedRuoliTitolarioDisattivati, nodoTitolario.IDRegistroAssociato);
                        //}
                        //else
                        //{
                            if (this.ExtendToNode(selectedNode.ID, this.hd_idGruppo.Value, true, true, AllTitolario))
                                if(!AllTitolario)
                                    this.ExtendToChildNodes(selectedNode.ID, this.hd_idGruppo.Value);
                        //}
                        //this.FillVisibAttuale();
                    }
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore, impossibile procedere nella funzionalità");
            }
        }

        private bool ExtendToNode(string idNodoTitolario, string idRuolo, bool isInsert, bool showMsgSuccessed, bool AllTitolario)
        {
            bool retValue = true;
            string validationMessage = string.Empty;

            SAAdminTool.DocsPaWR.EsitoOperazione[] esiti = this.ApplyChanges(idNodoTitolario, isInsert, AllTitolario);

            if (esiti != null)
            {
                foreach (SAAdminTool.DocsPaWR.EsitoOperazione es in esiti)
                {
                    if (es.Codice != 0)
                {
                    //    if (showMsgSuccessed)
                    //        this.RegisterClientScript("alertExtend", "alert('Visibilità " + (isInsert ? "estesa" : "eliminata") + " con successo');");
                    //}
                    //else
                    //{
                    // Aggiornamento non andato a buon fine, 
                    // visualizzazione messaggio di errore

                        //foreach (SAAdminTool.DocsPaWR.EsitoOperazione esito in esiti)
                        //{
                        if (validationMessage != string.Empty)
                            validationMessage += @"\n";

                            validationMessage += " - " + es.Descrizione;
                        //}

                    }
                }

                if (validationMessage != string.Empty)
                {
                    validationMessage = "Risultato modifica visibilità: " + validationMessage;

                    Session.Add("mex", validationMessage);

                    string javascript = "window.showModalDialog(\'../../../popup/VisibilitaTitolario.aspx\',\'Visibilità Titolario\',\'dialogWidth:600px;dialogHeight:300px;status:no;resizable:no;scroll:no;center:yes;help:no;\')";
                    
                    ClientScript.RegisterStartupScript(this.GetType(), "ValidationMessage", javascript, true);
                    
                    this.setBoldNode(!isInsert);

                    retValue = false;
                }
                else
                {
                    this.setBoldNode(isInsert);
                    if (showMsgSuccessed)
                        this.RegisterClientScript("alertExtend", "alert('Visibilità " + (isInsert ? "estesa" : "eliminata") + " con successo');");
                    retValue = true;
                }
            }

            return retValue;
        }

        private bool ExtendToChildNodes(string idNodoTitolario, string idRuolo)
        {
            bool retValue = true;

            SAAdminTool.DocsPaWR.OrgRuoloTitolario ruoloTitolario = new SAAdminTool.DocsPaWR.OrgRuoloTitolario();
            ruoloTitolario.ID = idRuolo;
            ruoloTitolario.Codice = this.hd_codRuolo.Value;
            ruoloTitolario.Associato = true;

            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            
            string idAmm = this.hd_idAmm.Value;
            string idRegistro = this.getRegistroSelezionato();

            SAAdminTool.DocsPaWR.EsitoOperazione esito = ws.AmmExtendToChildNodes(idNodoTitolario, ruoloTitolario, idAmm, idRegistro, true);

            if (esito.Codice > 0)
            {
                this.RegisterClientScript("alertExtend", "alert('" + esito.Descrizione.Replace("'", "\\'") + "');");
                retValue = false;
            }
            else
            {
                this.RegisterClientScript("alertExtend", "alert('Visibilità estesa con successo');");
            }

            this.setBoldNode(retValue);

            return retValue;
        }

        private SAAdminTool.DocsPaWR.EsitoOperazione[] ApplyChanges(string idNodo, bool isInsert, bool AllTitolario)
        {
            // crea un array perchè il metodo accetta array di oggetti "OrgRuoloTitolario"
            ArrayList arrayRuoli = new ArrayList();
            string idRegistro = this.getRegistroSelezionato();

            SAAdminTool.DocsPaWR.OrgRuoloTitolario ruoloTitolario = new SAAdminTool.DocsPaWR.OrgRuoloTitolario();
            ruoloTitolario.ID = this.hd_idGruppo.Value;
            ruoloTitolario.Codice = this.hd_codRuolo.Value;
            ruoloTitolario.Associato = isInsert;

            arrayRuoli.Add(ruoloTitolario);

            SAAdminTool.DocsPaWR.OrgRuoloTitolario[] ruoliTitolario = new SAAdminTool.DocsPaWR.OrgRuoloTitolario[arrayRuoli.Count];
            arrayRuoli.CopyTo(ruoliTitolario);

            string idAmm = this.hd_idAmm.Value;

            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            SAAdminTool.DocsPaWR.EsitoOperazione[] retValue = null;
            if(isInsert)
                retValue = ws.AmmUpdateRuoliTitolario(idNodo, idAmm, AllTitolario, ruoliTitolario, null, idRegistro);
            else
                retValue = ws.AmmUpdateRuoliTitolario(idNodo, idAmm, AllTitolario, null, ruoliTitolario, idRegistro);
          
            return retValue;
        }

        private void setBoldNode(bool isBold)
        {
            TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
            if (selectedNode != null)
            {
                selectedNode.Expanded = false;
                selectedNode.Text = (isBold ? "<b>" + selectedNode.Text + "</b>" : selectedNode.Text.Replace("<b>","").Replace("</b>",""));
            }
        }

        protected void ddl_registri_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.FillTreeView(this.getRegistroSelezionato());
                this.abilitazioneTasti();
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore, impossibile procedere nella funzionalità");
            }
        }

        private void abilitazioneTasti()
        {
             TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
             if (selectedNode != null)
             {
                 SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario = selectedNode.GetNodoTitolario();

                 if (nodoTitolario == null)
                 {
                     this.impostaStatoTasti(false, false, false);
                     return;
                 }

                 if (nodoTitolario.Livello == "0" && nodoTitolario.Codice.Equals("T") && nodoTitolario.IDParentNodoTitolario.Equals("0"))
                 {
                     this.lbl_agg_nodo.Visible = false;
                     this.lbl_agg_nodo_figli.Text = "Aggiungi visibilità per tutto il titolario";
                     this.lbl_elimina.Text = "Elimina visibilità di tutto il titolario";
                     this.impostaStatoTasti(false, true, true);
                     this.btn_add.Visible = false;
                 }
                 else
                 {
                     this.lbl_agg_nodo.Visible = true;
                     this.lbl_agg_nodo_figli.Text = "Aggiungi visibilità al nodo e a tutti i sotto-nodi";
                     this.lbl_elimina.Text = "Elimina visibilità";
                     this.btn_add.Visible = true;
                     if (nodoTitolario.Livello != "0" &&
                        nodoTitolario.IDParentNodoTitolario != "0" &&
                        nodoTitolario.Codice != "T" &&
                        nodoTitolario.ID_Titolario != "0")
                     {
                         this.impostaStatoTasti(!selectedNode.Text.Contains("<b>"), (!selectedNode.Text.Contains("<b>") && nodoTitolario.CountChildNodiTitolario > 0), selectedNode.Text.Contains("<b>"));
                     }
                     else
                     {
                         this.impostaStatoTasti(false, false, false);
                     }
                 }
             }
        }

        private void impostaStatoTasti(bool enableADD, bool enableADD_F, bool enableDEL)
        {
            this.btn_add.Enabled = enableADD;
            this.btn_add_figli.Enabled = enableADD_F;

            this.btn_del.Enabled = enableDEL;
        }

        protected void trvNodiTitolario_SelectedIndexChange(object sender, TreeViewSelectEventArgs e)
        {
            this.abilitazioneTasti();
        }
    }
}
