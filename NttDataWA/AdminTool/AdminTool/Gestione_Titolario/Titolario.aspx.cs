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


namespace Amministrazione.Gestione_Titolario
{
    /// <summary>
    /// Summary description for Titolario.
    /// </summary>
    public class Titolario : System.Web.UI.Page
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
                this.SetViewStateItem("ClassificazioneConsentita", nodoTitolario.consentiClassificazione.ToString());
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

                    itemValue = this.GetViewStateItem("ClassificazioneConsentita");
                    if (itemValue != string.Empty)
                        retValue.consentiClassificazione = itemValue;


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

        #region WebControls e costanti
        protected System.Web.UI.WebControls.DropDownList ddl_tipologiaFascicoli;
        protected System.Web.UI.WebControls.Label lbl_position;
        protected System.Web.UI.HtmlControls.HtmlForm Form1;
        //protected HtmlTable tblDetailsTitolario;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txtQueryStringDialogRuoliTitolario;
        protected System.Web.UI.WebControls.TextBox txt_livello_hd;
        protected System.Web.UI.WebControls.Panel pnl_info;
        protected System.Web.UI.HtmlControls.HtmlTable tblButtons;
        protected System.Web.UI.WebControls.Label lblTipoRicerca;
        protected System.Web.UI.WebControls.DropDownList cboTipoRicerca;
        protected System.Web.UI.WebControls.TextBox txtFieldRicerca;
        protected System.Web.UI.WebControls.Button btnSearch;
        protected Microsoft.Web.UI.WebControls.TreeView trvNodiTitolario;
        protected System.Web.UI.WebControls.Label lblSpostaNodo;
        //protected System.Web.UI.WebControls.ImageButton btnMoveUp;
        protected System.Web.UI.WebControls.ImageButton btnMoveDown;
        protected System.Web.UI.WebControls.Button btn_aggiungiNew;
        protected System.Web.UI.WebControls.Button btn_salvaInfo;
        protected System.Web.UI.WebControls.Button btn_elimina;
        protected System.Web.UI.WebControls.Button btn_indiceSis;
        protected System.Web.UI.WebControls.Button btn_VisibilitaRuoli;
        protected System.Web.UI.WebControls.Label lbl_codiceInfo;
        protected System.Web.UI.WebControls.TextBox txt_codiceInfo;
        protected System.Web.UI.WebControls.TextBox txt_noteNodo;
        protected System.Web.UI.WebControls.Label lbl_codiceInfo_provv;
        protected System.Web.UI.WebControls.TextBox txt_codiceInfo_provv;
        protected System.Web.UI.WebControls.Label lbl_descrizioneInfo;
        protected System.Web.UI.WebControls.TextBox txt_descrizioneInfo;
        protected System.Web.UI.WebControls.Label lbl_registroInfo;
        protected System.Web.UI.WebControls.DropDownList ddl_registriInfo;
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.DropDownList ddl_rw;
        protected System.Web.UI.WebControls.Label lblNumeroMesiConservazione;
        protected System.Web.UI.WebControls.TextBox txtMesiConservazione;
        //protected System.Web.UI.HtmlControls.HtmlTableRow tblRowSpostaNodoTitolario;
        protected System.Web.UI.WebControls.ImageButton btnFrecciaSu;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        protected System.Web.UI.WebControls.ImageButton btnFrecciaGiu;
        protected SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();
        protected System.Web.UI.WebControls.Panel pnl_ProfilazioneFascicoli;
        protected System.Web.UI.WebControls.Label lbl_bloccaTipologia;
        protected System.Web.UI.WebControls.Label lbl_attivaContatore;
        protected System.Web.UI.WebControls.DropDownList ddl_attivaContatore;
        protected System.Web.UI.WebControls.DropDownList ddl_creazioneFigli;

        protected System.Web.UI.WebControls.CheckBox cb_bloccaTipologia;
        protected System.Web.UI.WebControls.Panel pnl_DettagliNodo;
        protected System.Web.UI.WebControls.Panel pnl_PulsantiNodo;
        protected System.Web.UI.WebControls.Panel pnl_DettagliTitolario;
        protected System.Web.UI.WebControls.Panel pnl_PulsantiTitolario;
        protected System.Web.UI.WebControls.Button btn_NuovoTitolario;
        protected System.Web.UI.WebControls.Button btn_gestioneEtichetteTit;
        protected System.Web.UI.WebControls.Button btn_SalvaTitolario;
        protected System.Web.UI.WebControls.Button btn_ImportaTitolario;
        protected System.Web.UI.WebControls.Button btn_AttivaTitolario;
        protected System.Web.UI.WebControls.Button btn_EliminaTitolario;
        protected System.Web.UI.WebControls.Button btn_CopiaTitolario;
        protected System.Web.UI.WebControls.Button btn_EsportaTitolario;
        protected System.Web.UI.WebControls.TextBox txt_CommentoTit;
        protected System.Web.UI.WebControls.Label lbl_DescrizioneTitolario;
        protected System.Web.UI.WebControls.Panel pnl_RicercaNodo;
        protected Utilities.MessageBox messageBox_AttivaTitolario;
        protected Utilities.MessageBox messageBox_EliminaTitolario;
        protected Utilities.MessageBox messageBox_CopiaTitolario;
        protected Utilities.MessageBox messageBox_ClassificazioneNodiFiglio;
        protected System.Web.UI.WebControls.Button btn_EsportaIndice;
        protected System.Web.UI.WebControls.Button btn_ImportaIndice;
        protected System.Web.UI.WebControls.Button btn_VisibilitaTitolario;
        protected System.Web.UI.WebControls.TextBox txt_DescrizioneTit;
        protected System.Web.UI.WebControls.DropDownList ddl_LivelliTit;
        protected System.Web.UI.WebControls.Label lbl_protoTtitolario;
        protected System.Web.UI.WebControls.TextBox txt_protoTitolario;
        protected System.Web.UI.WebControls.Panel pnl_ProtocolloTitolario;
        protected System.Web.UI.WebControls.Label lbl_classificazione;
        protected System.Web.UI.WebControls.DropDownList ddl_classificazione;

        protected System.Web.UI.WebControls.Button btn_toExtSys;
        protected System.Web.UI.HtmlControls.HtmlTableRow tr_backToExtSys;


        #endregion

        #region Web Form Designer generated code

        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.trvNodiTitolario.Expand += new Microsoft.Web.UI.WebControls.ClickEventHandler(this.trvNodiTitolario_Expand);
            this.trvNodiTitolario.SelectedIndexChange += new Microsoft.Web.UI.WebControls.SelectEventHandler(this.trvNodiTitolario_SelectedIndexChange);
            //this.btnMoveUp.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveUp_Click);
            this.btnMoveDown.Click += new System.Web.UI.ImageClickEventHandler(this.btnMoveDown_Click);
            this.btn_VisibilitaRuoli.Click += new System.EventHandler(this.btn_VisibilitaRuoli_Click);
            this.btn_aggiungiNew.Click += new System.EventHandler(this.btn_aggiungiNew_Click);
            this.btn_elimina.Click += new System.EventHandler(this.btn_elimina_Click);
            this.btn_indiceSis.Click += new System.EventHandler(this.btn_indiceSis_Click);
            this.btn_salvaInfo.Click += new System.EventHandler(this.btn_salvaInfo_Click);
            this.messageBox_AttivaTitolario.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.messageBox_AttivaTitolario_GetMessageBoxResponse);
            this.messageBox_EliminaTitolario.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.messageBox_EliminaTitolario_GetMessageBoxResponse);
            this.messageBox_CopiaTitolario.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.messageBox_CopiaTitolario_GetMessageBoxResponse);
            this.messageBox_ClassificazioneNodiFiglio.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.messageBox_ClassificazioneNodiFiglio_GetMessageBoxResponse);
            //this.btn_VisibilitaTitolario.Click += new EventHandler(btn_VisibilitaTitolario_Click);
            this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
            this.ddl_classificazione.SelectedIndexChanged += new System.EventHandler(this.ddl_classificazione_SelectedIndexChanged);

            this.Load += new System.EventHandler(this.Page_Load);
        }

        #endregion

        #region Gestione eventi pagina

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Session["AdminBookmark"] = "Titolario";

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

                //Profilazione dinamica fascicoli
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                {
                    pnl_ProfilazioneFascicoli.Visible = true;
                    //CaricaCombo tipologia fascicoli
                    CaricaComboTipologiaFasc();
                }

                //Protocollo titolario
                string textProtoTitolario = ws.isEnableContatoreTitolario();
                if (textProtoTitolario != "")
                {
                    pnl_ProtocolloTitolario.Visible = true;
                    lbl_protoTtitolario.Text = textProtoTitolario;
                }

                if (!IsPostBack)
                {
                    this.AddControlsClientAttribute();

                    this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");

                    // Caricamento combo tipologie di ricerca
                    this.FillComboTipoRicerca();

                    // verifica se proviene dalla ricerca 
                    if (Request.QueryString["azione"] == "ricerca")
                    {
                        // Caricamento combo registri disponibili
                        this.FillComboRegistri(Request.QueryString["idregistro"]);

                        // Caricamento dati dei titolari nel treeview
                        this.FillTreeView(Request.QueryString["idregistro"]);

                        // Ricerca di un nodo di titolario
                        this.FindNodoTitolario(Request.QueryString["idrecord"],
                            Request.QueryString["idparent"],
                            Convert.ToInt32(Request.QueryString["livello"]) + 1);
                    }
                    else
                    {
                        // Caricamento combo registri disponibili
                        this.FillComboRegistri(null);

                        // Caricamento dati dei titolari nel treeview
                        this.FillTreeView(this.ddl_registri.SelectedValue.ToString());

                        // Selezione nodo root
                        this.PerformSelectionCurrentTitolario();
                    }

                    impostaVisibilitaControlli(null);
                    if (string.IsNullOrEmpty(Request.QueryString["extsysconf"]))
                        this.tr_backToExtSys.Visible = false;
                }

                //Verifico se provengo dalla pagina di importazione
                if (Session["titolarioSelezionato"] != null)
                {
                    Session.Remove("titolarioSelezionato");
                    //Caricamento dati dei titolari nel treeview
                    this.FillTreeView(this.ddl_registri.SelectedValue.ToString());
                    //Selezione nodo root
                    this.PerformSelectionCurrentTitolario();
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }
        }

        #endregion

        #region Gestione spostamento nodi titolario

        #region Codice azioni pulsanti moveUp / moveDown commentato
        /// <summary>
        /// Spostamento nodo: freccia SU
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMoveUp_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.MoveNodoTitolario("up");
        }

        /// <summary>
        /// Spostamento nodo: freccia Giù
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMoveDown_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.MoveNodoTitolario("down");
        }

        /// <summary>
        /// sposta il nodo su o giù
        /// </summary>
        /// <param name="azione">possibili valori: "up" o "down"</param>
        private void MoveNodoTitolario(string azione)
        {
            string currentCodLivNoChild;
            string newCodLiv = "";
            int lung;

            SAAdminTool.DocsPaWR.OrgNodoTitolario currentTitolario = this.GetCurrentTitolario();
            string livello = currentTitolario.Livello;
            string currentCodLiv = currentTitolario.CodiceLivello;
            string registro = currentTitolario.IDRegistroAssociato;

            try
            {
                // prende il codice livello senza il figlio
                if (Convert.ToInt32(livello) > 1)
                {
                    currentCodLivNoChild = currentCodLiv.Remove((4 * (Convert.ToInt32(livello) - 1)), 4);
                }
                else
                {
                    currentCodLivNoChild = "";
                }

                // prende il codice livello figlio
                string currentCodLivChild = currentCodLiv.Remove(0, ((Convert.ToInt32(livello) * 4) - 4));

                // elimina gli "0"				
                string currentCodLivChildNoZero = int.Parse(currentCodLivChild).ToString();

                // converte in intero
                int codliv = int.Parse(currentCodLivChildNoZero);

                if ((codliv > 1 && azione.Equals("up")) || (codliv > 0 && azione.Equals("down")))
                {
                    //lung = Convert.ToString(((int.Parse(currentCodLivChildNoZero))- 1)).Length;

                    switch (azione)
                    {
                        case "up":
                            lung = Convert.ToString(((int.Parse(currentCodLivChildNoZero)) - 1)).Length;
                            switch (lung)
                            {
                                case 1:
                                    newCodLiv = currentCodLivNoChild + "000" + Convert.ToString(codliv - 1);
                                    break;
                                case 2:
                                    newCodLiv = currentCodLivNoChild + "00" + Convert.ToString(codliv - 1);
                                    break;
                                case 3:
                                    newCodLiv = currentCodLivNoChild + "0" + Convert.ToString(codliv - 1);
                                    break;
                                case 4:
                                    newCodLiv = Convert.ToString(Convert.ToInt32(currentCodLivChild) - 1);
                                    break;
                            }
                            break;

                        case "down":
                            lung = Convert.ToString(((int.Parse(currentCodLivChildNoZero)) + 1)).Length;
                            switch (lung)
                            {
                                case 1:
                                    newCodLiv = currentCodLivNoChild + "000" + Convert.ToString(codliv + 1);
                                    break;
                                case 2:
                                    newCodLiv = currentCodLivNoChild + "00" + Convert.ToString(codliv + 1);
                                    break;
                                case 3:
                                    newCodLiv = currentCodLivNoChild + "0" + Convert.ToString(codliv + 1);
                                    break;
                                case 4:
                                    newCodLiv = Convert.ToString(Convert.ToInt32(currentCodLivChild) + 1);
                                    break;
                            }
                            break;
                    }

                    string codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");

                    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                    string result = ws.spostaNodo(currentCodLiv, newCodLiv, codAmm, registro);

                    /*
                    * possibili valori di ritorno:
                    * "0" = ok
                    * "9" = errore generico							
                    */
                    switch (result)
                    {
                        case "0":
                            TreeNodeTitolario parentNode = null;

                            if (currentTitolario.IDParentNodoTitolario == "0")
                            {
                                this.FillTreeView(this.getRegistroSelezionato());

                                if (this.trvNodiTitolario.Nodes.Count > 0)
                                    parentNode = this.trvNodiTitolario.Nodes[0] as TreeNodeTitolario;
                            }
                            else
                            {
                                TreeNodeTitolario treeNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
                                parentNode = treeNode.Parent as TreeNodeTitolario;
                                if (parentNode != null)
                                    this.FillTreeNodes(parentNode, this.getRegistroSelezionato());
                                treeNode = null;
                            }

                            if (parentNode != null)
                            {
                                foreach (Microsoft.Web.UI.WebControls.TreeNode node in parentNode.Nodes)
                                {
                                    if (node.ID == currentTitolario.ID)
                                    {
                                        this.trvNodiTitolario.SelectedNodeIndex = node.GetNodeIndex();
                                        break;
                                    }
                                }

                                this.PerformSelectionCurrentTitolario();
                            }

                            break;

                        case "9":
                            this.ShowErrorMessage("Si è verificato un errore durante lo spostamento di questo nodo");
                            break;
                    }
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }
        }
        #endregion

        /// <summary>
        /// Restituisce l'indice del nodo superiore o inferiore al nodo corrente
        /// </summary>
        /// <param name="mode">Possibili valori: "+" = superiore, "-" = inferiore</param>
        /// <returns></returns>
        private string IndiceSuperioreInferiore(string mode)
        {
            string indice = string.Empty;
            string[] indiceSplittato = null;
            string ultimo = string.Empty;
            string suffisso = string.Empty;

            TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
            indice = selectedNode.GetNodeIndex();
            if (indice.Length >= 3)
            {
                indiceSplittato = indice.Split('.');
                ultimo = indiceSplittato[indiceSplittato.Length - 1];
                for (int i = 0; i < indiceSplittato.Length - 1; i++)
                {
                    suffisso += indiceSplittato[i] + ".";
                }
            }

            switch (mode)
            {
                case "+":
                    indice = string.Concat(suffisso, Convert.ToString(Convert.ToInt32(ultimo) - 1));
                    break;
                case "-":
                    indice = string.Concat(suffisso, Convert.ToString(Convert.ToInt32(ultimo) + 1));
                    break;
            }

            return indice;
        }

        /// <summary>
        /// Verifica se il nodo prima del corrente ha lo stesso registro associato
        /// </summary>
        /// <param name="mode">Possibili valori: "+" = superiore, "-" = inferiore</param>
        /// <returns></returns>
        private bool VerificaRegNodoSupInf(string mode)
        {
            bool retValue = false;
            SAAdminTool.DocsPaWR.OrgNodoTitolario currentTitolario = null;
            SAAdminTool.DocsPaWR.OrgNodoTitolario titolario = null;
            string currentRegistro = string.Empty;
            string registro = string.Empty;

            currentTitolario = this.GetCurrentTitolario();
            currentRegistro = currentTitolario.IDRegistroAssociato;

            string nuovoIndice = this.IndiceSuperioreInferiore(mode);

            TreeNodeTitolario selectedNode2 = this.GetNodeFromIndex(nuovoIndice);
            if (selectedNode2 != null)
            {
                titolario = selectedNode2.GetNodoTitolario();
                registro = titolario.IDRegistroAssociato;

                if (registro.Equals(currentRegistro))
                    retValue = true;
            }

            return retValue;
        }

        /// <summary>
        /// Imposta la visibilità del tasto sposta nodo in SU'
        /// </summary>
        /// <returns></returns>
        private bool CanMoveUp()
        {
            bool retValue = false;
            TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
            string indice = selectedNode.GetNodeIndex();
            if (!indice.EndsWith("0"))
            {
                retValue = this.VerificaRegNodoSupInf("+");
            }
            return retValue;
        }

        /// <summary>
        /// Imposta la visibilità del tasto sposta nodo in GIU'
        /// </summary>
        /// <returns></returns>
        private bool CanMoveDown()
        {
            bool retValue = false;

            string nuovoIndice = this.IndiceSuperioreInferiore("-");

            TreeNodeTitolario selectedNode2 = this.GetNodeFromIndex(nuovoIndice);
            if (selectedNode2 != null)
                retValue = this.VerificaRegNodoSupInf("-");

            return retValue;
        }
        #endregion

        #region Gestione associazione Registro / Ruolo / Nodo di titolario

        private void btn_VisibilitaRuoli_Click(object sender, System.EventArgs e)
        {
            // Rimozione dati sessione utilizzati nella popup dei ruoli del titolario
            RuoliTitolario.RuoliTitolarioSessionManager.RemoveRuoliTitolario();
        }

        /// <summary>
        /// Caricamento combo registri disponibili
        /// </summary>
        public void FillComboRegistri(string idRegistro)
        {
            try
            {
                this.ddl_registri.Items.Clear();

                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                string xmlStream;
                string descReg;
                string idReg;

                // ************************************
                //		GESTIONE RESTRICTED AREA
                // ************************************
                if (Session["Restricted"] != null)
                    xmlStream = ws.RegistriInAmmRestricted(Session.SessionID);
                else
                    xmlStream = ws.RegistriInAmm(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"));

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlStream);

                XmlNode lista = doc.SelectSingleNode("NewDataSet");

                if (lista != null && lista.ChildNodes.Count > 0)
                {
                    // ************************************
                    //		GESTIONE RESTRICTED AREA
                    // ************************************
                    if (Session["Restricted"] == null)
                        this.ddl_registri.Items.Add(new ListItem("Tutti i registri", string.Empty));

                    foreach (XmlNode nodo in lista.ChildNodes)
                    {
                        descReg = nodo.SelectSingleNode("DESCRIZIONE").InnerText;
                        idReg = nodo.SelectSingleNode("IDRECORD").InnerText;

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
                    if (lista != null && lista.ChildNodes.Count == 1)
                    {
                        // se esiste UN SOLO registro nell'amministrazione,
                        // allora imposta per default questo registro
                        ddl_registri.SelectedIndex = 1;
                    }
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante il reperimento dati dei registri.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registroAssociato"></param>
        /// <param name="isInsertMode"></param>
        private void FillComboRegistroNodo(string registroAssociato, bool isInsertMode)
        {
            // prende il registro associato al padre
            SAAdminTool.DocsPaWR.OrgNodoTitolario parent = new SAAdminTool.DocsPaWR.OrgNodoTitolario();
            parent = this.GetParentTitolario();
            string registroParent = string.Empty;
            if (parent != null)
                registroParent = parent.IDRegistroAssociato;


            this.ddl_registriInfo.Items.Clear();

            if (registroAssociato == null)
                registroAssociato = string.Empty;

            if (registroAssociato != string.Empty)
            {
                if (isInsertMode)
                {
                    // Inserimento:
                    // significa che il nodo è associato ad un solo registro,
                    // quindi visualizza solo il suo registro					
                    this.ddl_registriInfo.Items.Add(new ListItem(this.ddl_registri.SelectedItem.Text, this.ddl_registri.SelectedValue));
                }
                else
                {
                    // Modifica:
                    // significa che il nodo è associato ad un solo registro,
                    // quindi è possibile passare a tutti i registri (se esiste + di un registro)
                    if (this.ddl_registri.Items.Count >= 2 && (this.checkDueRegistriUguali() || registroParent == string.Empty))
                        this.ddl_registriInfo.Items.Add(new ListItem("Tutti i registri", string.Empty));
                    this.ddl_registriInfo.Items.Add(new ListItem(this.ddl_registri.SelectedItem.Text, this.ddl_registri.SelectedValue));
                    ddl_registriInfo.SelectedIndex = ddl_registriInfo.Items.IndexOf(ddl_registriInfo.Items.FindByValue(registroAssociato));
                }
            }
            else
            {
                if (isInsertMode)
                {
                    // Inserimento:
                    // visualizza solo il registro di gestione selezionato in alto
                    this.ddl_registriInfo.Items.Add(new ListItem(this.ddl_registri.SelectedItem.Text, this.ddl_registri.SelectedValue));
                }
                else
                {
                    // Modifica:
                    // significa che il nodo è comune a tutti i registri, quindi
                    // non è possibile selezionare un singolo registro
                    this.ddl_registriInfo.Items.Add(new ListItem("Tutti i registri", string.Empty));
                }
            }
        }

        /// <summary>
        /// Aggiornamento parametri querystring necessari
        /// per la visualizzazione della dialog "RuoliTitolario.aspx"
        /// </summary>
        private void RefreshQueryStringDialogRuoliTitolario()
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            SAAdminTool.DocsPaWR.OrgTitolario titolario = new SAAdminTool.DocsPaWR.OrgTitolario();
            titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            titolario.ID = nodo.ID;
            //SAAdminTool.DocsPaWR.FileDocumento fileDoc = wws.ExportTitolarioInExcel(titolario);

            SAAdminTool.DocsPaWR.OrgNodoTitolario currentTitolario = this.GetCurrentTitolario();

            if (currentTitolario != null)
            {
                string queryString = "?ID_NODO_TITOLARIO=" + currentTitolario.ID +
                                   "&ID_REGISTRO="; // +currentTitolario.IDRegistroAssociato;
                if (!this.ddl_registri.SelectedValue.Equals(string.Empty))
                    queryString += this.ddl_registri.SelectedValue;
                this.txtQueryStringDialogRuoliTitolario.Value = queryString;
            }
        }

        #endregion

        #region Gestione JavaScript

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        private void AddControlsClientAttribute()
        {
            this.btn_elimina.Attributes.Add("onclick", "if (!window.confirm('Eliminare il nodo di titolario?')) {return false};");
            this.btn_VisibilitaRuoli.Attributes.Add("onClick", "ShowDialogRuoliTitolario();");
            this.txtMesiConservazione.Attributes.Add("onKeyPress", "ValidateNumericKey();");
            this.txt_protoTitolario.Attributes.Add("onKeyPress", "ValidateNumericKey();");
        }

        /// <summary>
        /// Impostazione del focus su un controllo
        /// </summary>
        /// <param name="controlID"></param>
        private void SetControlFocus(string controlID)
        {
            this.RegisterClientScript("SetFocus", "SetControlFocus('" + controlID + "');");
        }

        #endregion

        #region Gestione TreeView titolario

        /// <summary>
        /// Caricamento dei nodi di titolario 
        /// </summary>
        /// <param name="idParentTitolario"></param>
        /// <returns></returns>
        private SAAdminTool.DocsPaWR.OrgNodoTitolario[] GetNodiTitolario(string idParentTitolario, string idRegistro)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.AmmGetNodiTitolario(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"), idParentTitolario, idRegistro);
        }

        private void FillTreeView(string idRegistro)
        {
            if (this.ddl_registri.SelectedIndex != 0)
            {

                SAAdminTool.DocsPaWR.Registro registro = wws.GetRegistroBySistemId(idRegistro);
                if (registro.Sospeso)
                {
                    RegisterClientScript("alertRegistroSospeso", "alert('Il registro selezionato è sospeso!');");
                    this.ddl_registri.SelectedIndex = 0;
                    //RegisterClientScript("refresh", "document.forms[0].submit();");
                    return;
                }
            }
            if (this.trvNodiTitolario.Nodes.Count > 0)
                this.trvNodiTitolario.Nodes.Clear();

            this.AddRootNodes(idRegistro);
        }

        /// <summary>
        /// Inserimento nodo amministrazione corrente
        /// </summary>
        /// <returns></returns>
        private Microsoft.Web.UI.WebControls.TreeNode AddRootNodes(string idRegistro)
        {
            Microsoft.Web.UI.WebControls.TreeNode rootNode = new TreeNodeTitolario();
            rootNode.ID = "0";
            rootNode.Text = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
            //rootNode.NavigateUrl = "Titolario.aspx?from=TI&azione=AddNewNodeToRoot";
            rootNode.Expanded = true;
            trvNodiTitolario.Nodes.Add(rootNode);

            this.FillTreeNodes((TreeNodeTitolario)rootNode, idRegistro);

            return rootNode;
        }

        private TreeNodeTitolario CreateNewTreeNodeTitolario(SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario)
        {
            TreeNodeTitolario retValue = new TreeNodeTitolario();
            ((TreeNodeTitolario)retValue).SetNodoTitolario(nodoTitolario);
            return retValue;
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

        /// <summary>
        /// Ricerca di un nodo di titolario
        /// </summary>
        /// <param name="idTitolario"></param>
        /// <param name="idParentTitolario"></param>
        /// <param name="livello"></param>
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

                    // Caricamento dati del titolario correntemente selezionato
                    this.PerformSelectionCurrentTitolario();
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }
        }

        /// <summary>
        /// Reperimento, in base all'indice univoco, di un oggetto "TreeNodeTitolario"
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <returns></returns>
        private Titolario.TreeNodeTitolario GetNodeFromIndex(string nodeIndex)
        {
            return this.trvNodiTitolario.GetNodeFromIndex(nodeIndex) as Titolario.TreeNodeTitolario;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idrecord"></param>
        /// <param name="indiceCiclo"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trvNodiTitolario_Expand(object sender, Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e)
        {
            try
            {
                if (e.Node != "0")
                {
                    TreeNodeTitolario selectedNode = this.GetNodeFromIndex(e.Node);
                    this.FillTreeNodes(selectedNode, this.getRegistroSelezionato());

                    impostaVisibilitaControlli(selectedNode.GetNodoTitolario());

                    // Impostazione del nodo come correntemente selezionato
                    this.trvNodiTitolario.SelectedNodeIndex = selectedNode.GetNodeIndex();
                    selectedNode = null;

                    // Assegnazione dei valori alla UI
                    this.PerformSelectionCurrentTitolario();
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trvNodiTitolario_SelectedIndexChange(object sender, Microsoft.Web.UI.WebControls.TreeViewSelectEventArgs e)
        {
            this.PerformSelectionCurrentTitolario();
        }

        #endregion

        #region Gestione impostazione e reperimento oggetto "OrgNodoTitolario" corrente

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

        /// <summary>
        /// Reperimento oggetto "OrgNodoTitolario" relativo al nodo correntemente selezionato
        /// </summary>
        /// <returns></returns>
        private SAAdminTool.DocsPaWR.OrgNodoTitolario GetCurrentTitolario()
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario retValue = null;

            //Modifica per NuovaGestioneTitolario

            if (this.trvNodiTitolario.SelectedNodeIndex != "0")
            {
                TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
                retValue = selectedNode.GetNodoTitolario();
                selectedNode = null;
                if (retValue.ID_Titolario == "0")
                    retValue = null;
            }
            return retValue;

            /*
            if (this.trvNodiTitolario.SelectedNodeIndex.Length > 3)
            {
                TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
                retValue = selectedNode.GetNodoTitolario();
                selectedNode = null;
            }
            return retValue;


            if (this.trvNodiTitolario.SelectedNodeIndex != "0")
            {
                TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
                retValue = selectedNode.GetNodoTitolario();
                selectedNode = null;
            }

            return retValue;
            */
        }

        /// <summary>
        /// Impostazione oggetto "OrgNodoTitolario" relativo al nodo correntemente selezionato
        /// </summary>
        /// <param name="nodoTitolario"></param>
        private void SetCurrentTitolario(SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario)
        {
            TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);

            if (selectedNode != null)
            {
                selectedNode.SetNodoTitolario(nodoTitolario);
                selectedNode = null;
            }
        }

        #endregion

        #region Gestione assegnazione alla UI dei dati del titolario

        /// <summary>
        /// Impostazione messaggio di errore
        /// </summary>
        /// <param name="errorMessage"></param>
        private void ShowErrorMessage(string errorMessage)
        {
            this.RegisterClientScript("ErrorMessage", "alert('" + errorMessage.Replace("'", "\\'") + "')");
        }

        /// <summary>
        /// Gestione assegnazione dati alla UI del titolario 
        /// correntemente selezionato nel treeview
        /// </summary>
        private void PerformSelectionCurrentTitolario()
        {
            try
            {
                // Reperimento titolario correntemente selezionato
                SAAdminTool.DocsPaWR.OrgNodoTitolario currentTitolario = this.GetCurrentTitolario();

                if (currentTitolario == null)
                {
                    // Posizionamento sul nodo root (amministrazione)

                    // Impostazione controlli UI per modalità di aggiornamento
                    this.SetUpdateMode();

                    /*
                    // Impostazione visibilità pannello campi UI del titolario
                    this.SetVisibilityPanelDetailsTitolario(false);
					
                    // Abilitazione pulsanti per l'inserimento
                    this.EnableButtons(true);

                    // Impostazione visibilità panel spostamento nodi
                    this.SetVisibilityPanelSpostaNodo(false);
                    */

                }
                else
                {
                    // Associazione dati
                    this.BindDataTitolario(currentTitolario);

                    // Impostazione controlli UI per modalità di aggiornamento
                    this.SetUpdateMode();

                    /*
                    // Impostazione visibilità pannello campi UI del titolario
                    this.SetVisibilityPanelDetailsTitolario(true);

                    // Impostazione visibilità panel spostamento nodi
                    this.SetVisibilityPanelSpostaNodo(true);
                    */
                }
            }
            catch
            {
                //this.SetVisibilityPanelDetailsTitolario(false);
                this.ShowErrorMessage("Si è verificato un errore durante il bind dei dati del titolario.");
            }
        }

        /// <summary>
        /// Associazione dati titolario ai campi della UI
        /// </summary>
        private void BindDataTitolario(SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario)
        {
            this.txt_codiceInfo.Text = nodoTitolario.Codice;
            this.txt_codiceInfo_provv.Text = string.Empty;
            this.txt_descrizioneInfo.Text = nodoTitolario.Descrizione;
            this.txt_noteNodo.Text = nodoTitolario.note;

            this.FillComboRegistroNodo(nodoTitolario.IDRegistroAssociato, false);
            //this.ddl_registriInfo.SelectedValue=nodoTitolario.IDRegistroAssociato;

            if (nodoTitolario.CreazioneFascicoliAbilitata)
                this.ddl_rw.SelectedValue = "W";
            else
                this.ddl_rw.SelectedValue = "R";

            //Blocco creazione nodi figli
            if (nodoTitolario.bloccaNodiFigli == "SI")
                this.ddl_creazioneFigli.SelectedValue = "SI";
            else
                this.ddl_creazioneFigli.SelectedValue = "NO";

            //Blocco classificazione
            //string valoreChiave;
            //valoreChiave = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
            //if (valoreChiave.Equals("1"))
            //{
            this.ddl_classificazione.SelectedIndex = 0;
            if (nodoTitolario.consentiClassificazione != null && nodoTitolario.consentiClassificazione == "1")
                this.ddl_classificazione.SelectedIndex = 1;
            //}

            //Attivazione contatore
            if (nodoTitolario.contatoreAttivo == "SI")
                this.ddl_attivaContatore.SelectedValue = "SI";
            else
                this.ddl_attivaContatore.SelectedValue = "NO";

            //Protocollo titolario            
            if (!string.IsNullOrEmpty(nodoTitolario.numProtoTit))
            {
                this.txt_protoTitolario.Text = nodoTitolario.numProtoTit;
                this.lbl_protoTtitolario.Visible = true;
                this.txt_protoTitolario.Visible = true;
            }
            else
            {
                this.lbl_protoTtitolario.Visible = false;
                this.txt_protoTitolario.Visible = false;
                this.txt_protoTitolario.Text = string.Empty;
            }

            this.txtMesiConservazione.Text = nodoTitolario.NumeroMesiConservazione.ToString();

            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
            {
                //Impostazione tipologia fascicoli
                if (nodoTitolario.ID_TipoFascicolo != "" && nodoTitolario.ID_TipoFascicolo != "0")
                {
                    for (int i = 0; i < ddl_tipologiaFascicoli.Items.Count; i++)
                    {
                        if (ddl_tipologiaFascicoli.Items[i].Value == nodoTitolario.ID_TipoFascicolo.ToString())
                            ddl_tipologiaFascicoli.SelectedValue = nodoTitolario.ID_TipoFascicolo.ToString();
                    }
                }
                else
                {
                    ddl_tipologiaFascicoli.SelectedIndex = 0;
                }

                //Impostazione blocco tipo fascicolo
                if (nodoTitolario.bloccaTipoFascicolo == "SI")
                    cb_bloccaTipologia.Checked = true;
                else
                    cb_bloccaTipologia.Checked = false;
            }

            nodoTitolario = null;

            // Aggiornamento parametri query string 
            // necessari per la visualizzazione della dialog dei ruoli
            this.RefreshQueryStringDialogRuoliTitolario();
        }

        /// <summary>
        /// Creazione oggetto "OrgNodoTitolario": 
        /// rappresenta il nodo di titolario correntemente selezionato
        /// </summary>
        /// <returns></returns>
        private SAAdminTool.DocsPaWR.OrgNodoTitolario CreateNewNodoTitolario(SAAdminTool.DocsPaWR.OrgNodoTitolario parentNodoTitolario)
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario retValue = new SAAdminTool.DocsPaWR.OrgNodoTitolario();

            if (parentNodoTitolario != null)
            {
                retValue.IDParentNodoTitolario = parentNodoTitolario.ID;
                retValue.Codice = parentNodoTitolario.Codice + "." + this.txt_codiceInfo_provv.Text;

                //Modifica per NuovaGestioneTitolario - Aggiunto questo if
                if (parentNodoTitolario.ID_Titolario == "0")
                    retValue.ID_Titolario = parentNodoTitolario.ID;
                else
                    retValue.ID_Titolario = parentNodoTitolario.ID_Titolario;
            }
            else
            {
                //Modifica per NuovaGestioneTitolario
                //if (this.trvNodiTitolario.SelectedNodeIndex.Length == 3)
                if (this.trvNodiTitolario.SelectedNodeIndex != "0")
                {
                    TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
                    SAAdminTool.DocsPaWR.OrgNodoTitolario nodeTit = selectedNode.GetNodoTitolario();
                    selectedNode = null;

                    retValue.IDParentNodoTitolario = nodeTit.ID;
                    retValue.Codice = this.txt_codiceInfo.Text;
                    if (nodeTit.ID_Titolario == "0")
                        retValue.ID_Titolario = nodeTit.ID;
                    else
                        retValue.ID_Titolario = nodeTit.ID_Titolario;
                }
                else
                {
                    retValue.IDParentNodoTitolario = "0";
                    retValue.Codice = this.txt_codiceInfo.Text;
                }
            }

            retValue.Descrizione = this.txt_descrizioneInfo.Text;
            retValue.IDRegistroAssociato = this.ddl_registriInfo.SelectedValue;
            retValue.CodiceAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
            retValue.CreazioneFascicoliAbilitata = (this.ddl_rw.SelectedValue == "W");

            retValue.note = this.txt_noteNodo.Text;

            int numeroMesiConservazione = 0;
            if (this.txtMesiConservazione.Text.Length > 0)
                numeroMesiConservazione = Convert.ToInt32(this.txtMesiConservazione.Text);
            retValue.NumeroMesiConservazione = numeroMesiConservazione;

            if (parentNodoTitolario != null)
            {
                retValue.Livello = (Convert.ToInt32(parentNodoTitolario.Livello) + 1).ToString();
                retValue.CodiceLivello = this.GetCodiceLivello(parentNodoTitolario.CodiceLivello,
                                                        retValue.Livello,
                                                        retValue.CodiceAmministrazione, retValue.ID_Titolario, retValue.IDRegistroAssociato);
            }
            else
            {
                retValue.Livello = "1";
                retValue.CodiceLivello = this.GetCodiceLivello("", retValue.Livello, retValue.CodiceAmministrazione, retValue.ID_Titolario, retValue.IDRegistroAssociato);
            }

            //Impostazione tipologia fascicoli per il nuovo nodo
            if (ddl_tipologiaFascicoli.SelectedIndex != 0)
                retValue.ID_TipoFascicolo = ddl_tipologiaFascicoli.SelectedValue;

            //Impostazione blocco tipo fascicolo
            if (cb_bloccaTipologia.Checked)
                retValue.bloccaTipoFascicolo = "SI";
            else
                retValue.bloccaTipoFascicolo = "NO";

            //Blocco creazione nodi figli
            if (this.ddl_creazioneFigli.SelectedValue == "SI")
                retValue.bloccaNodiFigli = "SI";
            else
                retValue.bloccaNodiFigli = "NO";

            //Attivazione contatore
            if (this.ddl_attivaContatore.SelectedValue == "SI")
                retValue.contatoreAttivo = "SI";
            else
                retValue.contatoreAttivo = "NO";

            //Protocollo Titolario
            if (this.txt_protoTitolario.Text.Length > 0)
                retValue.numProtoTit = txt_protoTitolario.Text;

            //Consenti classificazione
            if (this.ddl_classificazione.SelectedValue == "SI")
                retValue.consentiClassificazione = "1";
            else
                retValue.consentiClassificazione = "0";

            return retValue;
        }

        /// <summary>
        /// Creazione oggetto "OrgNodoTitolario"  utilizzabile per l'update
        /// </summary>
        /// <returns></returns>
        private SAAdminTool.DocsPaWR.OrgNodoTitolario GetUpdatedNodoTitolario()
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario originalNodoTitolario = this.GetCurrentTitolario();

            SAAdminTool.DocsPaWR.OrgNodoTitolario updatedNodoTitolario = new SAAdminTool.DocsPaWR.OrgNodoTitolario();
            updatedNodoTitolario.ID = originalNodoTitolario.ID;
            updatedNodoTitolario.IDParentNodoTitolario = originalNodoTitolario.IDParentNodoTitolario;
            updatedNodoTitolario.Codice = originalNodoTitolario.Codice;
            updatedNodoTitolario.CodiceAmministrazione = originalNodoTitolario.CodiceAmministrazione;
            updatedNodoTitolario.CodiceLivello = originalNodoTitolario.CodiceLivello;
            updatedNodoTitolario.Livello = originalNodoTitolario.Livello;
            updatedNodoTitolario.CountChildNodiTitolario = originalNodoTitolario.CountChildNodiTitolario;

            // Aggiornamento dati nell'oggetto titolario rispetto ai dati immessi nella UI
            updatedNodoTitolario.Descrizione = this.txt_descrizioneInfo.Text;
            updatedNodoTitolario.IDRegistroAssociato = this.ddl_registriInfo.SelectedValue;
            updatedNodoTitolario.CreazioneFascicoliAbilitata = (this.ddl_rw.SelectedValue == "W");

            updatedNodoTitolario.note = this.txt_noteNodo.Text;

            if (this.txtMesiConservazione.Text.Length > 0)
                updatedNodoTitolario.NumeroMesiConservazione = Convert.ToInt32(this.txtMesiConservazione.Text);
            else
                updatedNodoTitolario.NumeroMesiConservazione = 0;

            //Impostazione tipologia fascicoli per il nuovo nodo
            updatedNodoTitolario.ID_TipoFascicolo = ddl_tipologiaFascicoli.SelectedValue;

            //Impostazione blocco tipo fascicolo
            if (cb_bloccaTipologia.Checked)
                updatedNodoTitolario.bloccaTipoFascicolo = "SI";
            else
                updatedNodoTitolario.bloccaTipoFascicolo = "NO";

            //Blocca creazione nodi figli
            if (this.ddl_creazioneFigli.SelectedValue == "SI")
                updatedNodoTitolario.bloccaNodiFigli = "SI";
            else
                updatedNodoTitolario.bloccaNodiFigli = "NO";

            //Blocca classificazione
            if (this.ddl_classificazione.SelectedValue == "SI")
                updatedNodoTitolario.consentiClassificazione = "1";
            else
                updatedNodoTitolario.consentiClassificazione = "0";

            //Attivazione contatore
            if (this.ddl_attivaContatore.SelectedValue == "SI")
                updatedNodoTitolario.contatoreAttivo = "SI";
            else
                updatedNodoTitolario.contatoreAttivo = "NO";

            //Protocollo Titolario
            if (this.txt_protoTitolario.Text.Length > 0)
                updatedNodoTitolario.numProtoTit = txt_protoTitolario.Text;
            else
                updatedNodoTitolario.numProtoTit = string.Empty;

            //NuovaGestioneTitolario
            updatedNodoTitolario.ID_Titolario = originalNodoTitolario.ID_Titolario;
            updatedNodoTitolario.dataAttivazione = originalNodoTitolario.dataAttivazione;
            updatedNodoTitolario.dataCessazione = originalNodoTitolario.dataCessazione;
            updatedNodoTitolario.stato = originalNodoTitolario.stato;

            originalNodoTitolario = null;

            return updatedNodoTitolario;
        }

        private string GetCodiceLivello(string codliv_padre, string livello, string codAmm, string idTitolario, string idRegistro)
        {
            string codliv = null;

            try
            {
                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

                codliv = ws.PrendeCodLiv(codliv_padre, livello, codAmm, idTitolario, idRegistro);

                int lung = codliv.Length;
                switch (lung)
                {
                    case 1:
                        codliv = codliv_padre + "000" + codliv;
                        break;
                    case 2:
                        codliv = codliv_padre + "00" + codliv;
                        break;
                    case 3:
                        codliv = codliv_padre + "0" + codliv;
                        break;
                    case 4:
                        codliv = codliv_padre + codliv;
                        break;
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }

            codliv = codliv.Replace("%", "");
            return codliv;
        }

        #endregion

        #region Gestione inserimento, cancellazione e modifica titolario

        /// <summary>
        /// Inserimento di un nuovo titolario
        /// </summary>
        private bool InsertNewTitolario()
        {
            bool retValue = false;
            string validationMessage;

            try
            {
                SAAdminTool.DocsPaWR.OrgNodoTitolario parentTitolario = this.GetCurrentTitolario();
                SAAdminTool.DocsPaWR.OrgNodoTitolario newNodoTitolario = this.CreateNewNodoTitolario(parentTitolario);
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

                SAAdminTool.DocsPaWR.EsitoOperazione ret = ws.AmmInsertTitolario(ref newNodoTitolario, idAmm);
                ws = null;

                if (ret.Codice == 0)
                {


                    TreeNodeTitolario parentTreeNodeTitolario = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);



                    if (parentTreeNodeTitolario != null)
                    {
                        if (!parentTreeNodeTitolario.Expanded)
                            parentTreeNodeTitolario.Expanded = true;

                        // Creazione nuovo nodo titolario per il treeview
                        Microsoft.Web.UI.WebControls.TreeNode newTreeNodeTitolario = this.CreateNewTreeNodeTitolario(newNodoTitolario);
                        parentTreeNodeTitolario.Nodes.Add(newTreeNodeTitolario);

                        // Impostazione del nuovo nodo come correntemente selezionato
                        this.trvNodiTitolario.SelectedNodeIndex = newTreeNodeTitolario.GetNodeIndex();
                    }

                    this.PerformSelectionCurrentTitolario();

                    string valoreChiave;
                    valoreChiave = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
                    //BLOCCA CLASSIFICAZIONE
                    if (valoreChiave.Equals("1") && parentTitolario!=null && parentTitolario.CountChildNodiTitolario > 0)
                    {
                        SAAdminTool.DocsPaWR.OrgNodoTitolario parentNode = parentTreeNodeTitolario.GetNodoTitolario();
                        parentNode.consentiClassificazione = "0";
                        parentNode.CreazioneFascicoliAbilitata = false;
                        ((TreeNodeTitolario)parentTreeNodeTitolario).SetNodoTitolario(parentNode);


                        if (ddl_classificazione.SelectedIndex == 1)
                        {
                            // INC000000513718
                            // Rimosso il messaggio di warning
                            //validationMessage = "Non è consentita la classificazione su nodi titolario aventi nodi figli";
                            //this.RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");
                            //return false;
                        }
                        else
                        {
                            validationMessage = "Non sarà più possibile classificare documenti sul nodo: " + parentTitolario.Codice + " - " + parentTitolario.Descrizione;
                            this.RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");
                        }

                    }

                    retValue = true;
                }
                else
                {
                    // Aggiornamento non andato a buon fine, 
                    // visualizzazione messaggio di errore
                    validationMessage = "Non è stato possibile inserire il titolario: " + @"\n\n" + ret.Descrizione;

                    this.RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");

                    this.SetControlFocus("txt_descrizioneInfo");
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante l'inserimento del titolario.");

                this.SetControlFocus("txt_descrizioneInfo");
            }

            return retValue;
        }

        private bool SaveTitolario()
        {
            bool retValue = false;
            string validationMessage;
            try
            {
                string valoreChiave;
                valoreChiave = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
                if (valoreChiave.Equals("1"))
                {
                    SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario = this.GetCurrentTitolario();
                    if (nodoTitolario.CountChildNodiTitolario > 0 && ddl_classificazione.SelectedValue.Equals("NO") && ddl_rw.SelectedIndex == 1)
                    {
                        validationMessage = "Non è stato possibile aggiornare i dati: " + @"\n\n" + "Non è possibile creare fascicoli se la classificazione sul nodo risulta bloccata.";

                        this.RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");

                        this.SetControlFocus("txt_descrizioneInfo");
                        return false;
                    }
                }

                string errorMessage;
                string firstInvalidControlID;

                // Verifica presenza dati obbligatori
                if (this.ContainsRequiredFields(out errorMessage, out firstInvalidControlID))
                {
                    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

                    SAAdminTool.DocsPaWR.OrgNodoTitolario updatedNodoTitolario = this.GetUpdatedNodoTitolario();

                    SAAdminTool.DocsPaWR.EsitoOperazione ret = ws.AmmUpdateTitolario(updatedNodoTitolario);

                    if (ret.Codice == 0)
                    {
                        // Se l'update è andato a buon fine, viene aggiornato il nodo titolario corrente
                        this.SetCurrentTitolario(updatedNodoTitolario);

                        // Aggiornamento parametri querystring necessari
                        // per la visualizzazione della dialog "RuoliTitolario.aspx"
                        this.RefreshQueryStringDialogRuoliTitolario();

                        retValue = true;
                    }
                    else
                    {
                        // Aggiornamento non andato a buon fine, visualizzazione messaggio di errore
                        validationMessage = "Non è stato possibile aggiornare i dati: " + @"\n\n" + ret.Descrizione;

                        this.RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");

                        this.SetControlFocus("txt_descrizioneInfo");
                    }
                }
                else
                {
                    this.RegisterClientScript("ValidationMessage", "alert('" + errorMessage + "');");

                    // Impostazione focus sul primo campo non valido
                    this.SetControlFocus(firstInvalidControlID);
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante la modifica del titolario.");
            }

            return retValue;
        }

        private bool DeleteTitolario()
        {
            bool retValue = false;

            try
            {
                SAAdminTool.DocsPaWR.OrgNodoTitolario titolarioToDelete = this.GetCurrentTitolario();

                if (titolarioToDelete != null)
                {
                    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

                    SAAdminTool.DocsPaWR.EsitoOperazione ret = ws.AmmDeleteTitolario(titolarioToDelete);

                    if (ret.Codice == 0)
                    {
                        // Rimozione del nodo
                        TreeNodeTitolario treeNodeTitolario = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
                        treeNodeTitolario.Remove();

                        // Assegnazione dati alla UI relativamente al nuovo nodo selezionato
                        this.PerformSelectionCurrentTitolario();
                    }
                    else
                    {
                        // Cancellazione non andata a buon fine, visualizzazione messaggio di errore
                        string validationMessage = "Non è stato possibile cancellare il titolario: " +
                            @"\n\n" + ret.Descrizione;

                        this.RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");
                    }
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante la cancellazione del titolario.");
            }

            return retValue;
        }

        /// <summary>
        /// Inserimento titolario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_aggiungiNew_Click(object sender, System.EventArgs e)
        {
            if (this.IsInsertMode())
            {
                string errorMessage;
                string firstInvalidControlID;

                if (this.ContainsRequiredFields(out errorMessage, out firstInvalidControlID))
                {
                    if (this.InsertNewTitolario())
                    {
                        // Ripristino modalità di update solo se l'inserimento è andato a buon fine
                        this.SetUpdateMode();
                    }
                }
                else
                {
                    this.RegisterClientScript("ValidationMessage", "alert('" + errorMessage + "');");
                    this.SetControlFocus(firstInvalidControlID);
                }
            }
            else
            {
                this.SetInsertMode();
            }
        }

        /// <summary>
        /// Modifica titolario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_salvaInfo_Click(object sender, System.EventArgs e)
        {
            this.SaveTitolario();
        }

        /// <summary>
        /// Cancellazione titolario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_elimina_Click(object sender, System.EventArgs e)
        {
            this.DeleteTitolario();
        }

        /// <summary>
        /// Apertura popup indice sistematico
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_indiceSis_Click(object sender, System.EventArgs e)
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario nodoSelezionato = GetNodoSelezionato();
            Session.Add("nodoSelPerIndice", nodoSelezionato);
            ClientScript.RegisterStartupScript(this.GetType(), "popUpIndiceSistematico", "apriPopupIndiceSistematico();", true);
        }

        #endregion

        #region Gestione visibilità e abilitazione controlli UI

        #region Codice gestione visibilità commentato

        /*
        /// <summary>
		/// Gestione vibibilità panel controlli per lo spostamento dei nodi titolario.
		/// Lo spostamento dei nodi è consentito solo se:
		///	- il nodo correntemente selezionato ha nodi dello stesso livello
		///	- si è posizionati su un nodo di titolario nell'albero
		///	- la chiave del web.config "AMM_SPOSTA_NODI_TITOLARIO" (che consente l'utilizzo della funzione) è impostata a "true"
		/// </summary>
		/// <param name="onNodeTitolario">se "true" si è attualmente posizionati su un nodo di titolario nell'albero</param>
		private void SetVisibilityPanelSpostaNodo(bool onNodeTitolario)
		{
			bool configValue=false;
			try
			{
				configValue=Convert.ToBoolean(SAAdminTool.ConfigSettings.getKey(SAAdminTool.ConfigSettings.KeysENUM.AMM_SPOSTA_NODI_TITOLARIO));

				Microsoft.Web.UI.WebControls.TreeNode selectedNode=this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);				
				bool hasSibling=(selectedNode.GetSiblingNodeCollection().Count > 1);
				selectedNode=null;
			 
				this.tblRowSpostaNodoTitolario.Visible=true;

				if(configValue && onNodeTitolario && hasSibling && this.checkDueRegistriUguali())
				{					
					this.btnMoveUp.Visible=this.CanMoveUp();
					this.btnMoveDown.Visible=this.CanMoveDown();
					this.tblRowSpostaNodoTitolario.Visible = (this.btnMoveUp.Visible || this.btnMoveDown.Visible);
				}
				else
				{
					this.tblRowSpostaNodoTitolario.Visible=false;
				}
			}
			catch
			{

			}			
		}

		/// <summary>
		/// Gestione visibilità panel controlli dati del titolario
		/// </summary>
		/// <param name="isVisibile"></param>
		private void SetVisibilityPanelDetailsTitolario(bool isVisibile)
		{
			this.tblDetailsTitolario.Visible=isVisibile;
			if (!isVisibile)
				this.ClearFieldsDetailsTitolario();
		}

		/// <summary>
		/// Gestione abilitazione / disabilitazione controlli per
		/// l'acquisizione dei dati del titolario
		/// </summary>
		/// <param name="insertMode"></param>
		private void SetVisibilityFieldsDetailsTitolario(bool insertMode,bool onAdminNode)
		{
			this.txt_codiceInfo.Enabled=insertMode && onAdminNode;
			this.lbl_codiceInfo_provv.Visible=insertMode && !onAdminNode;
            this.txt_codiceInfo_provv.Visible=this.lbl_codiceInfo_provv.Visible;
		}

		/// <summary>
		/// Gestione abilitazione / disabilitazione pulsanti
		/// </summary>
		private void EnableButtons(bool insertMode)
		{
			this.btn_aggiungiNew.Enabled=true;
			this.btn_salvaInfo.Enabled=!insertMode;
			this.btn_elimina.Enabled=!insertMode;
			this.btn_VisibilitaRuoli.Enabled=!insertMode;
        }
        */
        #endregion

        /// <summary>
        /// Impostazione controlli UI per la modalità
        /// di modifica di un titolario
        /// </summary>
        private void impostaVisibilitaControlli(SAAdminTool.DocsPaWR.OrgNodoTitolario nodoParam)
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario nodo = nodoParam;
            if (nodo == null)
                nodo = GetNodoSelezionato();

            //I pannelli di operazioni su titolario e nodo sono sempre visilbil
            //sono i pulsanti che vengon abilitati/disablitati a seconda delle operazioni  possibili
            pnl_PulsantiNodo.Visible = true;
            pnl_PulsantiTitolario.Visible = true;

            //Abilitazione indice sistematico
            if (!wws.isEnableIndiceSistematico())
                btn_indiceSis.Visible = false;

            //CASO INIZIALE LA SELEZIONE E' SULL'AMMINISTRAZIONE
            if (nodo == null)
            {
                //Pannelli
                pnl_DettagliNodo.Visible = false;
                pnl_DettagliTitolario.Visible = false;

                //Op Titolario
                //Questo è l'unico caso in cui viene abilitato il pulsante nuovo titolario
                //Prima di farlo bisogna controllare se non esiste gia' un titolario in definizione
                if (wws.existTitolarioInDef(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0")))
                {
                    btn_NuovoTitolario.Enabled = false;
                    btn_gestioneEtichetteTit.Enabled = true;
                }
                else
                {
                    btn_NuovoTitolario.Enabled = true;
                    btn_gestioneEtichetteTit.Enabled = true;
                }
                btn_AttivaTitolario.Enabled = false;
                btn_EliminaTitolario.Enabled = false;
                btn_ImportaTitolario.Enabled = false;
                btn_SalvaTitolario.Enabled = false;
                btn_CopiaTitolario.Enabled = false;
                btn_EsportaTitolario.Enabled = false;
                btn_VisibilitaTitolario.Visible = false;
                btn_EsportaIndice.Enabled = false;
                btn_ImportaIndice.Enabled = false;

                //Indice sistematico 
                if (wws.isEnableIndiceSistematico())
                {
                    btn_EsportaIndice.Visible = true;
                    btn_ImportaIndice.Visible = true;
                }
                else
                {

                    btn_EsportaIndice.Visible = false;
                    btn_ImportaIndice.Visible = false;
                }

                //OP NODO
                btn_aggiungiNew.Enabled = false;
                btn_elimina.Enabled = false;
                btn_indiceSis.Enabled = false;
                btn_salvaInfo.Enabled = false;
                btn_VisibilitaRuoli.Visible = false;
                return;
            }

            //E' STATO SELEZIONATO UN TITOLARIO
            if (nodo.Codice == "T")
            {
                pnl_DettagliNodo.Visible = false;
                pnl_DettagliTitolario.Visible = true;
                //Solo in questo caso viene chiamato il metodo seguente
                //per impostare i campi di dettagli titolario
                setDettagliTitolario(nodo);

                //Op Titolario
                //Operazioni consentite solo se selezionato un titolario in definizione
                switch (nodo.stato)
                {
                    case "D":
                        btn_NuovoTitolario.Enabled = false;
                        btn_gestioneEtichetteTit.Enabled = false;
                        btn_ImportaTitolario.Enabled = true;
                        btn_AttivaTitolario.Enabled = true;
                        btn_EliminaTitolario.Enabled = true;
                        btn_SalvaTitolario.Enabled = true;
                        //Op Nodo
                        btn_aggiungiNew.Enabled = true;
                        break;
                    case "C":
                        btn_NuovoTitolario.Enabled = false;
                        btn_gestioneEtichetteTit.Enabled = false;
                        btn_ImportaTitolario.Enabled = false;
                        btn_AttivaTitolario.Enabled = false;
                        btn_EliminaTitolario.Enabled = false;
                        btn_SalvaTitolario.Enabled = false;
                        //Op Nodo
                        btn_aggiungiNew.Enabled = false;
                        break;
                    case "A":
                        btn_NuovoTitolario.Enabled = false;
                        btn_gestioneEtichetteTit.Enabled = false;
                        btn_ImportaTitolario.Enabled = true;
                        btn_AttivaTitolario.Enabled = false;
                        btn_EliminaTitolario.Enabled = false;
                        btn_SalvaTitolario.Enabled = true;
                        //Op Nodo
                        btn_aggiungiNew.Enabled = true;
                        break;
                }

                //Controllo se esiste un titolario in definizione per abilitare o meno la copia titolario
                if (wws.existTitolarioInDef(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0")))
                {
                    btn_CopiaTitolario.Enabled = false;
                }
                else
                {
                    btn_CopiaTitolario.Enabled = true;
                }

                //L'esportazione di un titolario è sempre disponibile quando si seleziona un titolario
                btn_EsportaTitolario.Enabled = true;
                btn_VisibilitaTitolario.Enabled = true;
                btn_VisibilitaTitolario.Visible = true;

                //Indice sistematico 
                if (wws.isEnableIndiceSistematico())
                {
                    btn_EsportaIndice.Enabled = true;
                    btn_ImportaIndice.Enabled = true;
                    btn_EsportaIndice.Visible = true;
                    btn_ImportaIndice.Visible = true;
                }
                else
                {
                    btn_EsportaIndice.Enabled = false;
                    btn_ImportaIndice.Enabled = false;
                    btn_EsportaIndice.Visible = false;
                    btn_ImportaIndice.Visible = false;
                }

                //Op Nodo
                btn_elimina.Enabled = false;
                btn_indiceSis.Enabled = false;
                btn_salvaInfo.Enabled = false;
                btn_VisibilitaRuoli.Visible = false;

                if (this.IsInsertMode())
                {
                    pnl_DettagliNodo.Visible = true;
                    pnl_DettagliTitolario.Visible = false;
                    //Op Titolario
                    btn_NuovoTitolario.Enabled = false;
                    btn_gestioneEtichetteTit.Enabled = false;
                    btn_ImportaTitolario.Enabled = false;
                    btn_AttivaTitolario.Enabled = false;
                    btn_EliminaTitolario.Enabled = false;
                    btn_SalvaTitolario.Enabled = false;
                    btn_CopiaTitolario.Enabled = false;
                    btn_EsportaTitolario.Enabled = false;
                    btn_VisibilitaTitolario.Visible = false;
                    btn_EsportaIndice.Enabled = false;
                    btn_ImportaIndice.Enabled = false;

                    //Op Nodo
                    btn_aggiungiNew.Enabled = true;
                    btn_elimina.Enabled = false;
                    btn_indiceSis.Enabled = false;
                    btn_salvaInfo.Enabled = false;
                    btn_VisibilitaRuoli.Visible = false;

                    this.txt_codiceInfo.Text = "";
                    this.txt_codiceInfo.Enabled = true;

                    this.lbl_codiceInfo_provv.Visible = false;
                    this.txt_codiceInfo_provv.Visible = false;
                }
            }

            //E' STATO SELEZIONATO UN NODO DI TITOLARIO
            if (nodo.Codice != "T")
            {
                pnl_DettagliNodo.Visible = true;
                pnl_DettagliTitolario.Visible = false;
                //Op Titolario
                btn_NuovoTitolario.Enabled = false;
                btn_gestioneEtichetteTit.Enabled = false;
                btn_ImportaTitolario.Enabled = false;
                btn_AttivaTitolario.Enabled = false;
                btn_EliminaTitolario.Enabled = false;
                btn_SalvaTitolario.Enabled = false;
                btn_CopiaTitolario.Enabled = false;
                btn_EsportaTitolario.Enabled = false;
                btn_VisibilitaTitolario.Visible = false;
                btn_EsportaIndice.Enabled = false;
                btn_ImportaIndice.Enabled = false;

                //Op Nodo
                SAAdminTool.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(nodo.ID_Titolario);
                if (titolario != null)
                {
                    switch (titolario.Stato)
                    {
                        case SAAdminTool.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            btn_aggiungiNew.Enabled = false;
                            btn_elimina.Enabled = false;
                            btn_indiceSis.Enabled = true;
                            btn_salvaInfo.Enabled = false;
                            btn_VisibilitaRuoli.Enabled = true;
                            btn_VisibilitaRuoli.Visible = true;
                            break;
                        case SAAdminTool.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            btn_aggiungiNew.Enabled = true;
                            btn_elimina.Enabled = true;
                            btn_indiceSis.Enabled = true;
                            btn_salvaInfo.Enabled = true;
                            btn_VisibilitaRuoli.Enabled = true;
                            btn_VisibilitaRuoli.Visible = true;
                            break;
                        case SAAdminTool.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione:
                            btn_aggiungiNew.Enabled = true;
                            btn_elimina.Enabled = true;
                            btn_indiceSis.Enabled = true;
                            btn_salvaInfo.Enabled = true;
                            btn_VisibilitaRuoli.Enabled = true;
                            btn_VisibilitaRuoli.Visible = true;
                            break;
                    }
                }

                this.txt_codiceInfo.Enabled = false;

                if (this.IsInsertMode())
                {
                    this.lbl_codiceInfo_provv.Visible = true;
                    this.txt_codiceInfo_provv.Visible = true;
                    btn_elimina.Enabled = false;
                    btn_indiceSis.Enabled = false;
                    btn_salvaInfo.Enabled = false;
                    btn_VisibilitaRuoli.Enabled = false;
                }
                else
                {
                    this.lbl_codiceInfo_provv.Visible = false;
                    this.txt_codiceInfo_provv.Visible = false;
                }

                if (nodo.Livello == titolario.MaxLivTitolario)
                    btn_aggiungiNew.Enabled = false;
            }
        }

        private void setDettagliTitolario(SAAdminTool.DocsPaWR.OrgNodoTitolario nodoParam)
        {

            txt_CommentoTit.Text = string.Empty;
            lbl_DescrizioneTitolario.Text = string.Empty;
            txt_DescrizioneTit.Text = "Titolario";

            if (nodoParam != null && pnl_DettagliTitolario.Visible)
            {
                SAAdminTool.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(nodoParam.ID);
                if (titolario != null)
                {
                    txt_CommentoTit.Text = titolario.Commento;
                    txt_DescrizioneTit.Text = nodoParam.Descrizione;
                    lbl_DescrizioneTitolario.Text = titolario.Descrizione;
                    if (!string.IsNullOrEmpty(titolario.MaxLivTitolario))
                        ddl_LivelliTit.SelectedValue = titolario.MaxLivTitolario;

                    //switch (titolario.Stato)
                    //{
                    //    case SAAdminTool.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                    //        lbl_DescrizioneTitolario.Text = " - " + nodoParam.Descrizione + " Attivo";
                    //        break;

                    //    case SAAdminTool.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione:
                    //        lbl_DescrizioneTitolario.Text = " - " + nodoParam.Descrizione + " in definizione";
                    //        break;

                    //    case SAAdminTool.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                    //        lbl_DescrizioneTitolario.Text = " - " + nodoParam.Descrizione + " in vigore dal " + nodoParam.dataAttivazione + " al " + nodoParam.dataCessazione;
                    //        break;
                    //}
                }
            }
        }

        private SAAdminTool.DocsPaWR.OrgNodoTitolario GetNodoSelezionato()
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario retValue = null;

            if (this.trvNodiTitolario.SelectedNodeIndex.Length > 2)
            {
                TreeNodeTitolario selectedNode = this.GetNodeFromIndex(this.trvNodiTitolario.SelectedNodeIndex);
                retValue = selectedNode.GetNodoTitolario();
                selectedNode = null;
            }
            return retValue;
        }

        private void ClearFieldsDetailsTitolario()
        {
            this.txt_codiceInfo_provv.Text = string.Empty;
            this.txt_codiceInfo.Text = string.Empty;
            this.txt_descrizioneInfo.Text = string.Empty;
            this.txt_noteNodo.Text = string.Empty;
        }

        /// <summary>
        /// Impostazione controlli UI per la modalità
        /// di modifica di un titolario
        /// </summary>
        private void SetUpdateMode()
        {
            this.btn_aggiungiNew.Text = "Aggiungi nodo figlio";

            this.ViewState["INSERT_MODE"] = false;

            impostaVisibilitaControlli(null);

            /*
            // Abilitazione / disabilitazione pulsanti per modalità inserimento
            if(this.checkDueRegistriUguali())
            {
                this.EnableButtons(false);
            }
            else
            {
                this.EnableButtons(true);
            }

            /// Gestione abilitazione / disabilitazione controlli per
            /// l'acquisizione dei dati del titolario
            this.SetVisibilityFieldsDetailsTitolario(false,false);
            */
        }

        /// <summary>
        /// Impostazione controlli UI per la modalità
        /// di inserimento di un nuovo titolario
        /// </summary>
        private void SetInsertMode()
        {
            this.txt_descrizioneInfo.Text = string.Empty;
            this.txtMesiConservazione.Text = string.Empty;
            this.txt_noteNodo.Text = string.Empty;
            this.ddl_classificazione.SelectedIndex = 1;

            SAAdminTool.DocsPaWR.OrgNodoTitolario parentTitolario = this.GetCurrentTitolario();

            //Protocollo Titolario
            string contatoreTitolario = wws.isEnableContatoreTitolario();
            if (parentTitolario != null && parentTitolario.contatoreAttivo == "SI" && !string.IsNullOrEmpty(contatoreTitolario))
            {
                lbl_protoTtitolario.Visible = true;
                txt_protoTitolario.Visible = true;
                this.txt_protoTitolario.Text = wws.getContatoreProtTitolario(parentTitolario).ToString();
            }
            else
            {
                this.txt_protoTitolario.Text = string.Empty;
            }

            // Se true, inserimento di un titolario in amministrazione
            bool onAdminNode = (parentTitolario == null);

            if (!onAdminNode)
            {
                // Inserimento di un sottotitolario
                this.btn_aggiungiNew.Text = "Aggiungi sotto " + parentTitolario.Codice;

                // gestione combo-box registri
                this.FillComboRegistroNodo(parentTitolario.IDRegistroAssociato, true);
            }
            else
            {
                // gestione combo-box registri
                this.FillComboRegistroNodo(this.ddl_registri.SelectedValue.ToString(), true);
            }

            /*
			this.SetVisibilityPanelDetailsTitolario(true);

			/// Gestione abilitazione / disabilitazione controlli per
			/// l'acquisizione dei dati del titolario
			this.SetVisibilityFieldsDetailsTitolario(true,onAdminNode);

			// Abilitazione / disabilitazione pulsanti per modalità inserimento
			this.EnableButtons(true);
			*/

            // Flag in viewstate che determina se si è in modalità di inserimento o meno
            this.ViewState["INSERT_MODE"] = true;

            string focusControlName = string.Empty;

            if (onAdminNode)
                focusControlName = this.txt_codiceInfo.ID;
            else
                focusControlName = this.txt_codiceInfo_provv.ID;

            this.SetControlFocus(focusControlName);

            impostaVisibilitaControlli(null);
        }

        /// <summary>
        /// Verifica se si è in modalità di inserimento di un nuovo titolario
        /// </summary>
        /// <returns></returns>
        private bool IsInsertMode()
        {
            bool retValue = false;

            // Flag in viewstate che determina se si è in modalità di inserimento o meno
            if (this.ViewState["INSERT_MODE"] != null)
                retValue = (bool)this.ViewState["INSERT_MODE"];

            return retValue;
        }

        /// <summary>
        /// Verifica se sono stati immessi i dati obbligatori
        /// </summary>
        /// <returns></returns>
        private bool ContainsRequiredFields(out string errorMessage, out string firstInvalidControlID)
        {
            errorMessage = string.Empty;
            firstInvalidControlID = string.Empty;

            ArrayList listErrorMessage = new ArrayList();

            if (this.IsInsertMode())
            {
                if (this.txt_codiceInfo_provv.Visible && this.txt_codiceInfo_provv.Text.Trim().Length == 0)
                {
                    listErrorMessage.Add("Codice titolario mancante");
                    firstInvalidControlID = this.txt_codiceInfo_provv.ID;
                }
                else if (this.txt_codiceInfo.Text.Trim().Length == 0)
                {
                    listErrorMessage.Add("Codice titolario mancante");
                    firstInvalidControlID = this.txt_codiceInfo.ID;
                }

                //Controllo che nei codici titolario non è presente il carattere "." 
                // e soprattutto che il codice non sia "T" lettera usata per identificare esclusivamente i titolari
                if (this.txt_codiceInfo.Enabled && this.txt_codiceInfo.Text.IndexOf(".") != -1)
                {
                    listErrorMessage.Add("Il carattere \".\" non è permesso");
                    this.txt_codiceInfo.Text = "";
                }
                if (this.txt_codiceInfo_provv.Enabled && this.txt_codiceInfo_provv.Text.IndexOf(".") != -1)
                {
                    listErrorMessage.Add("Il carattere \".\" non è permesso");
                    this.txt_codiceInfo_provv.Text = "";
                }

                if (this.txt_codiceInfo.Enabled && this.txt_codiceInfo.Text.Equals("T"))
                {
                    listErrorMessage.Add("Come codice non è possibile usare la lettera \"T\"");
                    this.txt_codiceInfo.Text = "";
                }
                if (this.txt_codiceInfo_provv.Enabled && this.txt_codiceInfo_provv.Text.Equals("T"))
                {
                    listErrorMessage.Add("Come codice non è possibile usare la lettera \"T\"");
                    this.txt_codiceInfo_provv.Text = "";
                }
            }

            if (this.txt_descrizioneInfo.Text.Trim().Length == 0)
            {
                listErrorMessage.Add("Descrizione titolario mancante");
                if (firstInvalidControlID == string.Empty)
                    firstInvalidControlID = this.txt_descrizioneInfo.ID;
            }

            foreach (string item in listErrorMessage)
            {
                if (errorMessage != string.Empty)
                    errorMessage += "\\n";
                else
                    errorMessage = "Sono state riscontrate le seguenti anomalie:\\n\\n";

                errorMessage += " - " + item;
            }

            return (listErrorMessage.Count == 0);
        }

        #endregion

        #region Gestione filtri ricerca titolario

        private const string FILTER_TYPE_CODICE = "CODICE";
        private const string FILTER_TYPE_DESCRIZIONE = "DESCRIZIONE";

        private void FillComboTipoRicerca()
        {
            if (this.cboTipoRicerca.Items.Count > 0)
                this.cboTipoRicerca.Items.Clear();

            this.cboTipoRicerca.Items.Add(new ListItem("Codice", FILTER_TYPE_CODICE));
            this.cboTipoRicerca.Items.Add(new ListItem("Descrizione", FILTER_TYPE_DESCRIZIONE));
        }

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            this.SearchTitolario(this.cboTipoRicerca.SelectedItem.Value, this.txtFieldRicerca.Text);
        }

        /// <summary>
        /// Ricerca del titolario in base ai parametri di filtro immessi
        /// </summary>
        /// <param name="searchType"></param>
        /// <param name="valueToSearch"></param>
        private void SearchTitolario(string searchType, string valueToSearch)
        {
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
                            if (!this.IsStartupScriptRegistered("apriRisultato"))
                            {
                                string scriptString = GeneraTesto(lista, this.getRegistroSelezionato());
                                this.RegisterStartupScript("apriRisultato", scriptString);
                            }
                        }
                    }
                }
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }
        }

        private string GeneraTesto(XmlNode lista, string idRegistro)
        {
            string scriptString = null;

            try
            {
                scriptString = "<script language=JavaScript>";
                scriptString += "wndricerca = window.open('','RisultatoRicerca','width=700,height=450,scrollbars=YES');";
                scriptString += "wndricerca.document.write(\"<HTML>";
                scriptString += "<HEAD><TITLE>DOCSPA - Amministrazione - Risultato della ricerca sul titolario</TITLE>";
                scriptString += "</HEAD>";
                scriptString += "<LINK href=../CSS/AmmStyle.css type=text/css rel=stylesheet>";
                scriptString += "<BODY bgColor=#f6f4f4>";
                scriptString += "<TABLE border=0 cellpadding=1 cellspacing=3 width=100%>";
                scriptString += "<TR><TD colspan=3 class=testo_grigio_scuro>Risultato della ricerca:</TD></TR>";
                scriptString += "<TR bgColor=#810d06><TD class=testo_bianco>Codice</TD><TD class=testo_bianco>Descrizione</TD><TD class=testo_bianco>Livello</TD></TR>";

                foreach (XmlNode nodo in lista.ChildNodes)
                {
                    scriptString += "<TR bgColor=#ffefd5>";
                    scriptString += "<TD class=testo_grigio_scuro><a href=\\'javascript:void(0)\\' class=testo_grigio_scuro onclick=window.opener.location.href=\\'Titolario.aspx?from=TI&azione=ricerca&idrecord=" + nodo.SelectSingleNode("ID").InnerText + "&idparent=" + nodo.SelectSingleNode("IDPARENT").InnerText + "&idregistro=" + idRegistro + "&livello=" + nodo.SelectSingleNode("LIVELLO").InnerText + "&idTitolario=" + nodo.SelectSingleNode("IDTITOLARIO").InnerText + "\\';>" + nodo.SelectSingleNode("CODICE").InnerText + "</a></TD>";
                    scriptString += "<TD class=testo>" + nodo.SelectSingleNode("DESCRIZIONE").InnerText.Replace("'", "&#39;").Replace("\"", "&quot;") + "</TD>";
                    scriptString += "<TD align=center class=testo_grigio_scuro>" + nodo.SelectSingleNode("LIVELLO").InnerText + "</TD>";
                    scriptString += "</TR>";
                }

                scriptString += "</TABLE>";
                scriptString += "</BODY></HTML>\");";
                scriptString += "</script>";
            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }

            return scriptString;
        }

        #endregion

        #region Gestione Registri

        /// <summary>
        /// gestione selezione registro di gestione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // Caricamento dati dei titolari nel treeview 
            this.FillTreeView(this.getRegistroSelezionato());

            this.PerformSelectionCurrentTitolario();
        }

        /// <summary>
        /// restituisce il registro di gestione selezionato
        /// </summary>
        /// <returns></returns>
        private string getRegistroSelezionato()
        {
            string retValue = string.Empty;
            retValue = this.ddl_registri.SelectedValue;
            return retValue;
        }

        private bool checkDueRegistriUguali()
        {
            bool retValue = false;
            if (this.ddl_registri.SelectedValue.Equals(this.ddl_registriInfo.SelectedValue))
                retValue = true;
            return retValue;
        }
        #endregion

        #region Tipologia fascioli
        private void CaricaComboTipologiaFasc()
        {
            if (ddl_tipologiaFascicoli.Items.Count == 0)
            {
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                string idAmministrazione = wws.getIdAmmByCod(codiceAmministrazione);

                ArrayList listaTipiFasc = new ArrayList(SAAdminTool.ProfilazioneFascManager.getTipoFasc(idAmministrazione, this));
                ListItem item = new ListItem();
                item.Value = "";
                item.Text = "";
                ddl_tipologiaFascicoli.Items.Add(item);
                for (int i = 0; i < listaTipiFasc.Count; i++)
                {
                    SAAdminTool.DocsPaWR.Templates templates = (SAAdminTool.DocsPaWR.Templates)listaTipiFasc[i];
                    ListItem item_1 = new ListItem();
                    item_1.Value = templates.SYSTEM_ID.ToString();
                    item_1.Text = templates.DESCRIZIONE;
                    ddl_tipologiaFascicoli.Items.Add(item_1);
                }
            }
        }
        #endregion

        #region Pulsanti operazioni titolario

        protected void btn_VisibilitaTitolario_Click(object sender, EventArgs e)
        {
            //this.RefreshQueryStringDialogRuoliTitolario();
            // Rimozione dati sessione utilizzati nella popup dei ruoli del titolario
            SAAdminTool.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            //SAAdminTool.DocsPaWR.OrgTitolario titolario = new SAAdminTool.DocsPaWR.OrgTitolario();
            //titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            //titolario.ID = nodo.ID;
            string queryString = "?ID_NODO_TITOLARIO=&ID_REGISTRO=";
            if (!this.ddl_registri.SelectedValue.Equals(string.Empty))
                queryString += this.ddl_registri.SelectedValue + "&ID_TITOLARIO=" + nodo.ID;
            else
                queryString += "&ID_TITOLARIO=" + nodo.ID;
            //string queryString = "?ID_NODO_TITOLARIO=&ID_REGISTRO=" + nodo.IDRegistroAssociato + "&ID_TITOLARIO=" + nodo.ID;

            this.txtQueryStringDialogRuoliTitolario.Value = queryString;
            RuoliTitolario.RuoliTitolarioSessionManager.RemoveRuoliTitolario();
            RegisterClientScript("ruoliTitolario", "ShowDialogRuoliTitolario();");
        }

        protected void btn_NuovoTitolario_Click(object sender, EventArgs e)
        {
            pnl_DettagliTitolario.Visible = true;
            setDettagliTitolario(null);
            btn_SalvaTitolario.Enabled = true;
            btn_NuovoTitolario.Enabled = false;
            btn_gestioneEtichetteTit.Enabled = false;
        }

        protected void btn_SalvaTitolario_Click(object sender, EventArgs e)
        {
            //Controllo che i campi obbligatori siano stati inseriti
            if (string.IsNullOrEmpty(txt_DescrizioneTit.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "DescrizioneTitObbligatoria", "alert('Inserire una descrizione per il titolario.');", true);
                return;
            }

            SAAdminTool.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();

            //Se il nodo selezionato è la radice allora si effettua un inserimento di nuovo titolario
            if (nodo == null)
            {
                SAAdminTool.DocsPaWR.OrgTitolario titolario = new SAAdminTool.DocsPaWR.OrgTitolario();
                titolario.Descrizione = txt_DescrizioneTit.Text;
                titolario.MaxLivTitolario = ddl_LivelliTit.SelectedValue;
                titolario.Commento = txt_CommentoTit.Text;
                titolario.Stato = SAAdminTool.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione;
                titolario.Codice = "T";
                string codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
                titolario.CodiceAmministrazione = codAmm;

                SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();

                wws.SaveTitolario(sessionManager.getUserAmmSession(), ref titolario);

                //Ricaricamento dati dei titolari nel treeview
                this.FillTreeView(this.ddl_registri.SelectedValue.ToString());
                // Selezione nodo root
                this.PerformSelectionCurrentTitolario();
                impostaVisibilitaControlli(null);
                return;
            }

            //Se il nodo selezioanto è un titolario allora si vuole effettuare un aggiornamento
            if (nodo != null && nodo.Codice == "T")
            {
                SAAdminTool.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(nodo.ID);
                titolario.Descrizione = txt_DescrizioneTit.Text;
                titolario.MaxLivTitolario = ddl_LivelliTit.SelectedValue;
                titolario.Commento = txt_CommentoTit.Text;

                SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();

                wws.SaveTitolario(sessionManager.getUserAmmSession(), ref titolario);

                //Ricaricamento dati dei titolari nel treeview
                this.FillTreeView(this.ddl_registri.SelectedValue.ToString());
                // Selezione nodo root
                this.PerformSelectionCurrentTitolario();
                impostaVisibilitaControlli(null);
                return;
            }
        }

        protected void btn_ImportaTitolario_Click(object sender, EventArgs e)
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            SAAdminTool.DocsPaWR.OrgTitolario titolario = new SAAdminTool.DocsPaWR.OrgTitolario();
            titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            titolario.ID = nodo.ID;
            titolario.Stato = SAAdminTool.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione;
            Session.Add("titolarioSelezionato", titolario);
            ClientScript.RegisterStartupScript(this.GetType(), "importaTitolario", "ApriImportaTitolario();", true);
        }

        protected void btn_AttivaTitolario_Click(object sender, EventArgs e)
        {
            messageBox_AttivaTitolario.Confirm("L'attuale titolario attivo verrà chiuso. Confermare ?");
        }

        protected void btn_EliminaTitolario_Click(object sender, EventArgs e)
        {
            messageBox_EliminaTitolario.Confirm("Si desidera effettivamente eliminare il titolario in definizione ?");
        }

        private void messageBox_AttivaTitolario_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                SAAdminTool.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();

                // Reperimento oggetto Titolario
                SAAdminTool.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(nodo.ID);

                SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();
                wws.Timeout = System.Threading.Timeout.Infinite;
                wws.attivaTitolario(sessionManager.getUserAmmSession(), titolario);

                //Ricaricamento dati dei titolari nel treeview
                this.FillTreeView(this.ddl_registri.SelectedValue.ToString());
                // Selezione nodo root
                this.PerformSelectionCurrentTitolario();
                impostaVisibilitaControlli(null);
            }
        }

        private void messageBox_EliminaTitolario_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                SAAdminTool.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();

                //Posso cancella un titolario solo se è in uno stato di "In Definizione"
                if (nodo.Codice == "T" && nodo.stato == "D")
                {
                    SAAdminTool.DocsPaWR.OrgTitolario titolario = new SAAdminTool.DocsPaWR.OrgTitolario();
                    titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
                    titolario.ID = nodo.ID;

                    SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();
                    wws.deleteTitolario(sessionManager.getUserAmmSession(), titolario);

                    //Ricaricamento dati dei titolari nel treeview
                    this.FillTreeView(this.ddl_registri.SelectedValue.ToString());
                    // Selezione nodo root
                    this.PerformSelectionCurrentTitolario();
                    impostaVisibilitaControlli(null);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ImpossibileEliminareTitolario", "alert('Cacellazione impossibile.e\nIl titolario selezionato non è in definizione.');", true);
                }
            }
        }

        protected void btn_CopiaTitolario_Click(object sender, EventArgs e)
        {
            messageBox_CopiaTitolario.Confirm("Verrà creato un titolario \"In Definizione\" copia di quello selezionato. Confermare ?");
        }
        //

        private void messageBox_ClassificazioneNodiFiglio_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed != Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                this.ddl_classificazione.SelectedIndex = 0;
            }
        }

        private void messageBox_CopiaTitolario_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                SAAdminTool.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();

                // Reperimento nodo titolario
                SAAdminTool.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(nodo.ID);
                titolario.Descrizione = nodo.Descrizione;

                SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();
                wws.Timeout = System.Threading.Timeout.Infinite;
                if (wws.copiaTitolario(sessionManager.getUserAmmSession(), titolario, ddl_registri.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "copiaAvvenuta", "alert('Copia avvenuta con successo.');", true);
                    //Ricaricamento dati dei titolari nel treeview
                    this.FillTreeView(this.ddl_registri.SelectedValue.ToString());
                    // Selezione nodo root
                    this.PerformSelectionCurrentTitolario();
                    impostaVisibilitaControlli(null);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "copiaErrata", "alert('Errore durante la copia del titolario.');", true);
                }
            }
        }

        protected void btn_EsportaTitolario_Click(object sender, EventArgs e)
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            SAAdminTool.DocsPaWR.OrgTitolario titolario = new SAAdminTool.DocsPaWR.OrgTitolario();
            titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            titolario.ID = nodo.ID;
            wws.Timeout = System.Threading.Timeout.Infinite;
            SAAdminTool.DocsPaWR.FileDocumento fileDoc = wws.ExportTitolarioInExcel(titolario, ddl_registri.SelectedValue);

            if (fileDoc != null)
            {
                SAAdminTool.exportDati.exportDatiSessionManager session = new SAAdminTool.exportDati.exportDatiSessionManager();
                session.SetSessionExportFile(fileDoc);
                ClientScript.RegisterStartupScript(this.GetType(), "openFile", "OpenFile();", true);
            }
        }

        protected void btn_EsportaIndice_Click(object sender, EventArgs e)
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            SAAdminTool.DocsPaWR.OrgTitolario titolario = new SAAdminTool.DocsPaWR.OrgTitolario();
            titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            titolario.ID = nodo.ID;
            SAAdminTool.DocsPaWR.FileDocumento fileDoc = wws.ExportIndiceSistematico(titolario);

            if (fileDoc != null)
            {
                SAAdminTool.exportDati.exportDatiSessionManager session = new SAAdminTool.exportDati.exportDatiSessionManager();
                session.SetSessionExportFile(fileDoc);
                ClientScript.RegisterStartupScript(this.GetType(), "openFile", "OpenFile();", true);
            }
        }

        protected void btn_ImportaIndice_Click(object sender, EventArgs e)
        {
            SAAdminTool.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            SAAdminTool.DocsPaWR.OrgTitolario titolario = new SAAdminTool.DocsPaWR.OrgTitolario();
            titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            titolario.ID = nodo.ID;
            titolario.Stato = SAAdminTool.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione;
            Session.Add("titolarioSelezionato", titolario);
            ClientScript.RegisterStartupScript(this.GetType(), "importaIndice", "ApriImportaIndice();", true);
        }

        #endregion

        #region Pulsanti operazioni amministrazione
        protected void btn_gestioneEtichetteTit_Click(object sender, EventArgs e)
        {
            //SAAdminTool.DocsPaWR.OrgNodoTitolario nodoSelezionato = GetNodoSelezionato();
            //Session.Add("nodoSelPerIndice", nodoSelezionato);

            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            ArrayList titolari = new ArrayList(wws.getTitolari(idAmm));
            if (titolari != null && titolari.Count > 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "popUpGestioneEtichette", "apriGestioneEtichetteTit('" + ((SAAdminTool.DocsPaWR.OrgTitolario)titolari[0]).ID + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alertNessunTitolario", "alert('Nessun titolario per questa amministrazione.');", true);
            }
        }
        #endregion


        #region
        private void ddl_classificazione_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string valoreChiave;
            valoreChiave = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
            if (valoreChiave.Equals("1"))
            {
                SAAdminTool.DocsPaWR.OrgNodoTitolario nodoTitolario = this.GetCurrentTitolario();
                if (nodoTitolario.CountChildNodiTitolario > 0)
                {
                    if (ddl_classificazione.SelectedIndex == 0)
                    {
                        ddl_rw.SelectedIndex = 0;
                    }
                    if (ddl_classificazione.SelectedIndex == 1)
                    {
                        messageBox_ClassificazioneNodiFiglio.Confirm("Attenzione! E' abilitato il blocco della classificazione per i nodi padre. Procedendo con l'operazione il blocco verrà ignorato. Si desidera continuare?");
                        //RegisterClientScript("alertBloccaClass", "alert('Classificazione bloccata per i nodi padri');");
                        //this.ddl_classificazione.SelectedIndex = 0;
                        //return;
                    }
                }
            }
        }
        #endregion
    }
}
