using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace SAAdminTool.AdminTool.Gestione_ProfDinamicaFasc
{
    public partial class StrutturaSottofascicoli : System.Web.UI.Page
    {

        #region CLASSE TreeNodeTitolario
        public class TreeNodeTitolario : Microsoft.Web.UI.WebControls.TreeNode
        {
            public void SetNodoTitolario(SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario)
            {
                this.SetViewStateItem("ID", nodoTitolario.ID);
                this.SetViewStateItem("Codice", nodoTitolario.Codice);
                this.SetViewStateItem("Descrizione", nodoTitolario.Descrizione);
                this.SetViewStateItem("CodiceAmministrazione", nodoTitolario.CodiceAmministrazione);
                this.SetViewStateItem("CodiceLivello", nodoTitolario.CodiceLivello);
                this.SetViewStateItem("CountChildNodiTitolario", nodoTitolario.CountChildNodiTitolario.ToString());
                this.SetViewStateItem("CreazioneFascicoliAbilitata", nodoTitolario.CreazioneFascicoliAbilitata.ToString());
                this.SetViewStateItem("IDParentNodoTitolario", nodoTitolario.IDParentNodoTitolario);
                this.SetViewStateItem("IDRegistroAssociato", nodoTitolario.IDRegistroAssociato);
                this.SetViewStateItem("Livello", nodoTitolario.Livello);
                this.SetViewStateItem("NumeroMesiConservazione", nodoTitolario.NumeroMesiConservazione.ToString());
                this.SetViewStateItem("TipologiaFascicolo", nodoTitolario.ID_TipoFascicolo.ToString());
                this.SetViewStateItem("BloccaTipologiaFascicolo", nodoTitolario.bloccaTipoFascicolo.ToString());
                this.SetViewStateItem("BloccaNodiFigli", nodoTitolario.bloccaNodiFigli.ToString());
                this.SetViewStateItem("AttivaContatore", nodoTitolario.contatoreAttivo.ToString());
                this.SetViewStateItem("ProtocolloTitolario", nodoTitolario.numProtoTit.ToString());
                //NuovaGestioneTitolario
                this.SetViewStateItem("IDTitolario", nodoTitolario.ID_Titolario.ToString());
                this.SetViewStateItem("DataAttivazione", nodoTitolario.dataAttivazione.ToString());
                this.SetViewStateItem("DataCessazione", nodoTitolario.dataCessazione.ToString());
                this.SetViewStateItem("Stato", nodoTitolario.stato.ToString());
                this.SetViewStateItem("Note", nodoTitolario.note);

                this.SetNodeDescription(nodoTitolario);
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

                    itemValue = this.GetViewStateItem("CreazioneFascicoliAbilitata");
                    if (itemValue != string.Empty)
                        retValue.CreazioneFascicoliAbilitata = Convert.ToBoolean(itemValue);

                    retValue.IDParentNodoTitolario = this.GetViewStateItem("IDParentNodoTitolario");
                    retValue.IDRegistroAssociato = this.GetViewStateItem("IDRegistroAssociato");
                    retValue.Livello = this.GetViewStateItem("Livello");

                    itemValue = this.GetViewStateItem("NumeroMesiConservazione");
                    if (itemValue != string.Empty)
                        retValue.NumeroMesiConservazione = Convert.ToInt32(itemValue);

                    //Impostazione tipo fascicoli
                    retValue.ID_TipoFascicolo = this.GetViewStateItem("TipologiaFascicolo");

                    //Impostazione blocco tipo fascicolo
                    retValue.bloccaTipoFascicolo = this.GetViewStateItem("BloccaTipologiaFascicolo");

                    //Blocco creazione nodi figli
                    retValue.bloccaNodiFigli = this.GetViewStateItem("BloccaNodiFigli");

                    //Attivazione contatore
                    retValue.contatoreAttivo = this.GetViewStateItem("AttivaContatore");

                    //NuovaGestioneTitolario
                    retValue.ID_Titolario = this.GetViewStateItem("IDTitolario");
                    retValue.stato = this.GetViewStateItem("Stato");
                    retValue.dataAttivazione = this.GetViewStateItem("DataAttivazione");
                    retValue.dataCessazione = this.GetViewStateItem("DataCessazione");
                    retValue.note = this.GetViewStateItem("Note");

                    //Protocollo Titolario
                    retValue.numProtoTit = this.GetViewStateItem("ProtocolloTitolario");
                }
                return retValue;
            }

            private void SetNodeDescription(SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario)
            {
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
                            this.Text = nodoTitolario.Codice + " - " + nodoTitolario.Descrizione + " - Attivo";
                            break;

                        case "D":
                            this.Text = nodoTitolario.Codice + " - " + nodoTitolario.Descrizione + " - In definizione";
                            break;

                        case "C":
                            this.Text = nodoTitolario.Codice + " - " + nodoTitolario.Descrizione + " - In vigore dal " + nodoTitolario.dataAttivazione + " al " + nodoTitolario.dataCessazione;
                            break;
                    }
                }
                else
                {
                    if (nodoTitolario.IDRegistroAssociato != null && nodoTitolario.IDRegistroAssociato != string.Empty)
                    {
                        this.Text = "<b>" + nodoTitolario.Codice + " - " + nodoTitolario.Descrizione + "</b>";
                    }
                    else
                    {
                        this.Text = nodoTitolario.Codice + " - " + nodoTitolario.Descrizione;
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

        protected SAAdminTool.DocsPaWR.Templates modelloSelezionato;
        protected Microsoft.Web.UI.WebControls.TreeView trvSottoFasc;
        

        protected void Page_Load(object sender, EventArgs e)
        {
            modelloSelezionato = (SAAdminTool.DocsPaWR.Templates)Session["template"];
            lbl_titolo.Text = "Struttura sottofascicoli - " + modelloSelezionato.DESCRIZIONE;

            if (!Page.IsPostBack)
            {
                //Caricamento dati dei sottofascicoli nel treeview
                FillTreeView();
            }
        }


        #region Gestione TreeView
        private void FillTreeView()
        {
            if (this.trvSottoFasc.Nodes.Count > 0)
                this.trvSottoFasc.Nodes.Clear();

            this.AddRootNodes();
        }

        /// <summary>
        /// Inserimento nodo amministrazione corrente
        /// </summary>
        /// <returns></returns>
        private Microsoft.Web.UI.WebControls.TreeNode AddRootNodes()
        {
            Microsoft.Web.UI.WebControls.TreeNode rootNode = new TreeNodeTitolario();
            rootNode.ID = "0";
            rootNode.Text = "Fascicolo";
            rootNode.Expanded = true;
            trvSottoFasc.Nodes.Add(rootNode);

            this.FillTreeNodes((TreeNodeTitolario)rootNode);

            return rootNode;
        }

        private void FillTreeNodes(TreeNodeTitolario parentNode)
        { }

        #endregion
        #region pulsanti
        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chiudi", "window.close();", true);
        }

        protected void btn_conferma_Click(object sender, EventArgs e)
        { 
        
        }

        protected void btn_aggiungi_Click(object sender, System.EventArgs e)
        {
        }
        #endregion
    }
}
