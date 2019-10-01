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
            public void SetNodoTitolario(DocsPAWA.DocsPaWR.OrgNodoTitolario nodoTitolario)
            {
                SetViewStateItem("ID", nodoTitolario.ID);
                SetViewStateItem("Codice", nodoTitolario.Codice);
                SetViewStateItem("Descrizione", nodoTitolario.Descrizione);
                SetViewStateItem("CodiceAmministrazione", nodoTitolario.CodiceAmministrazione);
                SetViewStateItem("CodiceLivello", nodoTitolario.CodiceLivello);
                SetViewStateItem("CountChildNodiTitolario", nodoTitolario.CountChildNodiTitolario.ToString());
                SetViewStateItem("CreazioneFascicoliAbilitata", nodoTitolario.CreazioneFascicoliAbilitata.ToString());
                SetViewStateItem("ClassificazioneConsentita", nodoTitolario.consentiClassificazione.ToString());
                SetViewStateItem("ConsentiFascicolazione", nodoTitolario.consentiFascicolazione.ToString());
                SetViewStateItem("IDParentNodoTitolario", nodoTitolario.IDParentNodoTitolario);
                SetViewStateItem("IDRegistroAssociato", nodoTitolario.IDRegistroAssociato);
                SetViewStateItem("Livello", nodoTitolario.Livello);
                SetViewStateItem("NumeroMesiConservazione", nodoTitolario.NumeroMesiConservazione.ToString());
                SetViewStateItem("TipologiaFascicolo", nodoTitolario.ID_TipoFascicolo.ToString());
                SetViewStateItem("BloccaTipologiaFascicolo", nodoTitolario.bloccaTipoFascicolo.ToString());
                SetViewStateItem("BloccaNodiFigli", nodoTitolario.bloccaNodiFigli.ToString());
                SetViewStateItem("AttivaContatore", nodoTitolario.contatoreAttivo.ToString());
                SetViewStateItem("ProtocolloTitolario", nodoTitolario.numProtoTit.ToString());
                //NuovaGestioneTitolario
                SetViewStateItem("IDTitolario", nodoTitolario.ID_Titolario.ToString());
                SetViewStateItem("DataAttivazione", nodoTitolario.dataAttivazione.ToString());
                SetViewStateItem("DataCessazione", nodoTitolario.dataCessazione.ToString());
                SetViewStateItem("Stato", nodoTitolario.stato.ToString());
                SetViewStateItem("Note", nodoTitolario.note);

                SetViewStateItem("IDTemplateStruttura", nodoTitolario.IDTemplateStrutturaSottofascicoli);

                SetNodeDescription(nodoTitolario);
            }

            public DocsPAWA.DocsPaWR.OrgNodoTitolario GetNodoTitolario()
            {
                DocsPAWA.DocsPaWR.OrgNodoTitolario retValue = null;

                string itemValue = GetViewStateItem("ID");

                if (itemValue != string.Empty && itemValue != "0")
                {
                    retValue = new DocsPAWA.DocsPaWR.OrgNodoTitolario();
                    retValue.ID = itemValue;

                    retValue.Codice = GetViewStateItem("Codice");
                    retValue.Descrizione = GetViewStateItem("Descrizione");
                    retValue.CodiceAmministrazione = GetViewStateItem("CodiceAmministrazione");
                    retValue.CodiceLivello = GetViewStateItem("CodiceLivello");

                    itemValue = GetViewStateItem("CountChildNodiTitolario");
                    if (itemValue != string.Empty)
                        retValue.CountChildNodiTitolario = Convert.ToInt32(itemValue);

                    itemValue = GetViewStateItem("CreazioneFascicoliAbilitata");
                    if (itemValue != string.Empty)
                        retValue.CreazioneFascicoliAbilitata = Convert.ToBoolean(itemValue);

                    itemValue = GetViewStateItem("ClassificazioneConsentita");
                    if (itemValue != string.Empty)
                        retValue.consentiClassificazione = itemValue;

                    itemValue = this.GetViewStateItem("ConsentiFascicolazione");
                    if (itemValue != string.Empty)
                        retValue.consentiFascicolazione = itemValue;

                    retValue.IDParentNodoTitolario = GetViewStateItem("IDParentNodoTitolario");
                    retValue.IDRegistroAssociato = GetViewStateItem("IDRegistroAssociato");
                    retValue.Livello = GetViewStateItem("Livello");

                    itemValue = GetViewStateItem("NumeroMesiConservazione");
                    if (itemValue != string.Empty)
                        retValue.NumeroMesiConservazione = Convert.ToInt32(itemValue);

                    //Impostazione tipo fascicoli
                    retValue.ID_TipoFascicolo = GetViewStateItem("TipologiaFascicolo");

                    //Impostazione blocco tipo fascicolo
                    retValue.bloccaTipoFascicolo = GetViewStateItem("BloccaTipologiaFascicolo");

                    //Blocco creazione nodi figli
                    retValue.bloccaNodiFigli = GetViewStateItem("BloccaNodiFigli");

                    //Attivazione contatore
                    retValue.contatoreAttivo = GetViewStateItem("AttivaContatore");

                    //NuovaGestioneTitolario
                    retValue.ID_Titolario = GetViewStateItem("IDTitolario");
                    retValue.stato = GetViewStateItem("Stato");
                    retValue.dataAttivazione = GetViewStateItem("DataAttivazione");
                    retValue.dataCessazione = GetViewStateItem("DataCessazione");
                    retValue.note = GetViewStateItem("Note");

                    //Protocollo Titolario
                    retValue.numProtoTit = GetViewStateItem("ProtocolloTitolario");

                    // Template Struttura
                    itemValue = GetViewStateItem("IDTemplateStruttura");
                    if (itemValue != string.Empty)
                        retValue.IDTemplateStrutturaSottofascicoli = itemValue;
                }
                return retValue;
            }

            private void SetNodeDescription(DocsPAWA.DocsPaWR.OrgNodoTitolario nodoTitolario)
            {
                ID = nodoTitolario.ID;

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
                            Text = nodoTitolario.Codice + " - " + nodoTitolario.Descrizione + " - Attivo";
                            break;

                        case "D":
                            Text = nodoTitolario.Codice + " - " + nodoTitolario.Descrizione + " - In definizione";
                            break;

                        case "C":
                            Text = nodoTitolario.Codice + " - " + nodoTitolario.Descrizione + " - In vigore dal " + nodoTitolario.dataAttivazione + " al " + nodoTitolario.dataCessazione;
                            break;
                    }
                }
                else
                {
                    if (nodoTitolario.IDRegistroAssociato != null && nodoTitolario.IDRegistroAssociato != string.Empty)
                    {
                        Text = "<b>" + nodoTitolario.Codice + " - " + nodoTitolario.Descrizione + "</b>";
                    }
                    else
                    {
                        Text = nodoTitolario.Codice + " - " + nodoTitolario.Descrizione;
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
        protected DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
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

        protected System.Web.UI.WebControls.DropDownList ddl_consentiFasc;

        protected System.Web.UI.WebControls.Panel pnl_StrutturaTemplate;
        protected System.Web.UI.WebControls.Label lbStrutturaTemplate;
        protected System.Web.UI.WebControls.DropDownList ddStrutturaTemplate;
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
            ddl_registri.SelectedIndexChanged += new System.EventHandler(ddl_registri_SelectedIndexChanged);
            btnSearch.Click += new System.EventHandler(btnSearch_Click);
            trvNodiTitolario.Expand += new Microsoft.Web.UI.WebControls.ClickEventHandler(trvNodiTitolario_Expand);
            trvNodiTitolario.SelectedIndexChange += new Microsoft.Web.UI.WebControls.SelectEventHandler(trvNodiTitolario_SelectedIndexChange);
            //btnMoveUp.Click += new System.Web.UI.ImageClickEventHandler(btnMoveUp_Click);
            btnMoveDown.Click += new System.Web.UI.ImageClickEventHandler(btnMoveDown_Click);
            btn_VisibilitaRuoli.Click += new System.EventHandler(btn_VisibilitaRuoli_Click);
            btn_aggiungiNew.Click += new System.EventHandler(btn_aggiungiNew_Click);
            btn_elimina.Click += new System.EventHandler(btn_elimina_Click);
            btn_indiceSis.Click += new System.EventHandler(btn_indiceSis_Click);
            btn_salvaInfo.Click += new System.EventHandler(btn_salvaInfo_Click);
            messageBox_AttivaTitolario.GetMessageBoxResponse += new Utilities.MessageBox.Message(messageBox_AttivaTitolario_GetMessageBoxResponse);
            messageBox_EliminaTitolario.GetMessageBoxResponse += new Utilities.MessageBox.Message(messageBox_EliminaTitolario_GetMessageBoxResponse);
            messageBox_CopiaTitolario.GetMessageBoxResponse += new Utilities.MessageBox.Message(messageBox_CopiaTitolario_GetMessageBoxResponse);
            messageBox_ClassificazioneNodiFiglio.GetMessageBoxResponse += new Utilities.MessageBox.Message(messageBox_ClassificazioneNodiFiglio_GetMessageBoxResponse);
            //btn_VisibilitaTitolario.Click += new EventHandler(btn_VisibilitaTitolario_Click);
            ddl_registri.SelectedIndexChanged += new System.EventHandler(ddl_registri_SelectedIndexChanged);
            ddl_classificazione.SelectedIndexChanged += new System.EventHandler(ddl_classificazione_SelectedIndexChanged);

            Load += new System.EventHandler(Page_Load);
        }

        #endregion


        public DataTable DataStrutturaTemplate
        {
            get
            {
                return ViewState["DataStrutturaTemplate"] as DataTable;
            }
            set
            {
                ViewState["DataStrutturaTemplate"] = value;
            }
        }


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

                // Template struttura sottofascicoli
                string valorechiave = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");
                if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
                    pnl_StrutturaTemplate.Visible = true;
                    

                if (!IsPostBack)
                {
                    if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
                        CaricaComboTemplateStruttura();

                    AddControlsClientAttribute();

                    lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");

                    // Caricamento combo tipologie di ricerca
                    FillComboTipoRicerca();

                    // verifica se proviene dalla ricerca 
                    if (Request.QueryString["azione"] == "ricerca")
                    {
                        // Caricamento combo registri disponibili
                        FillComboRegistri(Request.QueryString["idregistro"]);

                        // Caricamento dati dei titolari nel treeview
                        FillTreeView(Request.QueryString["idregistro"]);

                        // Ricerca di un nodo di titolario
                        FindNodoTitolario(Request.QueryString["idrecord"],
                            Request.QueryString["idparent"],
                            Convert.ToInt32(Request.QueryString["livello"]) + 1);
                    }
                    else
                    {
                        // Caricamento combo registri disponibili
                        FillComboRegistri(null);

                        // Caricamento dati dei titolari nel treeview
                        FillTreeView(ddl_registri.SelectedValue.ToString());

                        // Selezione nodo root
                        PerformSelectionCurrentTitolario();
                    }

                    impostaVisibilitaControlli(null);
                    if (string.IsNullOrEmpty(Request.QueryString["extsysconf"]))
                        tr_backToExtSys.Visible = false;
                }

                //Verifico se provengo dalla pagina di importazione
                if (Session["titolarioSelezionato"] != null)
                {
                    Session.Remove("titolarioSelezionato");
                    //Caricamento dati dei titolari nel treeview
                    FillTreeView(ddl_registri.SelectedValue.ToString());
                    //Selezione nodo root
                    PerformSelectionCurrentTitolario();
                }
            }
            catch
            {
                ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
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
            MoveNodoTitolario("up");
        }

        /// <summary>
        /// Spostamento nodo: freccia Giù
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMoveDown_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            MoveNodoTitolario("down");
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

            DocsPAWA.DocsPaWR.OrgNodoTitolario currentTitolario = GetCurrentTitolario();
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
                                FillTreeView(getRegistroSelezionato());

                                if (trvNodiTitolario.Nodes.Count > 0)
                                    parentNode = trvNodiTitolario.Nodes[0] as TreeNodeTitolario;
                            }
                            else
                            {
                                TreeNodeTitolario treeNode = GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);
                                parentNode = treeNode.Parent as TreeNodeTitolario;
                                if (parentNode != null)
                                    FillTreeNodes(parentNode, getRegistroSelezionato());
                                treeNode = null;
                            }

                            if (parentNode != null)
                            {
                                foreach (Microsoft.Web.UI.WebControls.TreeNode node in parentNode.Nodes)
                                {
                                    if (node.ID == currentTitolario.ID)
                                    {
                                        trvNodiTitolario.SelectedNodeIndex = node.GetNodeIndex();
                                        break;
                                    }
                                }

                                PerformSelectionCurrentTitolario();
                            }

                            break;

                        case "9":
                            ShowErrorMessage("Si è verificato un errore durante lo spostamento di questo nodo");
                            break;
                    }
                }
            }
            catch
            {
                ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
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

            TreeNodeTitolario selectedNode = GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);
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
            DocsPAWA.DocsPaWR.OrgNodoTitolario currentTitolario = null;
            DocsPAWA.DocsPaWR.OrgNodoTitolario titolario = null;
            string currentRegistro = string.Empty;
            string registro = string.Empty;

            currentTitolario = GetCurrentTitolario();
            currentRegistro = currentTitolario.IDRegistroAssociato;

            string nuovoIndice = IndiceSuperioreInferiore(mode);

            TreeNodeTitolario selectedNode2 = GetNodeFromIndex(nuovoIndice);
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
            TreeNodeTitolario selectedNode = GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);
            string indice = selectedNode.GetNodeIndex();
            if (!indice.EndsWith("0"))
            {
                retValue = VerificaRegNodoSupInf("+");
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

            string nuovoIndice = IndiceSuperioreInferiore("-");

            TreeNodeTitolario selectedNode2 = GetNodeFromIndex(nuovoIndice);
            if (selectedNode2 != null)
                retValue = VerificaRegNodoSupInf("-");

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
                ddl_registri.Items.Clear();

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
                        ddl_registri.Items.Add(new ListItem("Tutti i registri", string.Empty));

                    foreach (XmlNode nodo in lista.ChildNodes)
                    {
                        descReg = nodo.SelectSingleNode("DESCRIZIONE").InnerText;
                        idReg = nodo.SelectSingleNode("IDRECORD").InnerText;

                        ListItem item = new ListItem(descReg, idReg);
                        ddl_registri.Items.Add(item);
                    }
                }
                else
                {
                    ShowErrorMessage("Attenzione, nessun registro da amministrare.");
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
                ShowErrorMessage("Si è verificato un errore durante il reperimento dati dei registri.");
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
            DocsPAWA.DocsPaWR.OrgNodoTitolario parent = new DocsPAWA.DocsPaWR.OrgNodoTitolario();
            parent = GetParentTitolario();
            string registroParent = string.Empty;
            if (parent != null)
                registroParent = parent.IDRegistroAssociato;


            ddl_registriInfo.Items.Clear();

            if (registroAssociato == null)
                registroAssociato = string.Empty;

            if (registroAssociato != string.Empty)
            {
                if (isInsertMode)
                {
                    // Inserimento:
                    // significa che il nodo è associato ad un solo registro,
                    // quindi visualizza solo il suo registro					
                    ddl_registriInfo.Items.Add(new ListItem(ddl_registri.SelectedItem.Text, ddl_registri.SelectedValue));
                }
                else
                {
                    // Modifica:
                    // significa che il nodo è associato ad un solo registro,
                    // quindi è possibile passare a tutti i registri (se esiste + di un registro)
                    if (ddl_registri.Items.Count >= 2 && (checkDueRegistriUguali() || registroParent == string.Empty))
                        ddl_registriInfo.Items.Add(new ListItem("Tutti i registri", string.Empty));
                    ddl_registriInfo.Items.Add(new ListItem(ddl_registri.SelectedItem.Text, ddl_registri.SelectedValue));
                    ddl_registriInfo.SelectedIndex = ddl_registriInfo.Items.IndexOf(ddl_registriInfo.Items.FindByValue(registroAssociato));
                }
            }
            else
            {
                if (isInsertMode)
                {
                    // Inserimento:
                    // visualizza solo il registro di gestione selezionato in alto
                    ddl_registriInfo.Items.Add(new ListItem(ddl_registri.SelectedItem.Text, ddl_registri.SelectedValue));
                }
                else
                {
                    // Modifica:
                    // significa che il nodo è comune a tutti i registri, quindi
                    // non è possibile selezionare un singolo registro
                    ddl_registriInfo.Items.Add(new ListItem("Tutti i registri", string.Empty));
                }
            }
        }

        /// <summary>
        /// Aggiornamento parametri querystring necessari
        /// per la visualizzazione della dialog "RuoliTitolario.aspx"
        /// </summary>
        private void RefreshQueryStringDialogRuoliTitolario()
        {
            DocsPAWA.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            DocsPAWA.DocsPaWR.OrgTitolario titolario = new DocsPAWA.DocsPaWR.OrgTitolario();
            titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            titolario.ID = nodo.ID;
            //DocsPAWA.DocsPaWR.FileDocumento fileDoc = wws.ExportTitolarioInExcel(titolario);

            DocsPAWA.DocsPaWR.OrgNodoTitolario currentTitolario = GetCurrentTitolario();

            if (currentTitolario != null)
            {
                string queryString = "?ID_NODO_TITOLARIO=" + currentTitolario.ID +
                                   "&ID_REGISTRO="; // +currentTitolario.IDRegistroAssociato;
                if (!ddl_registri.SelectedValue.Equals(string.Empty))
                    queryString += ddl_registri.SelectedValue;
                txtQueryStringDialogRuoliTitolario.Value = queryString;
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
            if (!Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        private void AddControlsClientAttribute()
        {
            btn_elimina.Attributes.Add("onclick", "if (!window.confirm('Eliminare il nodo di titolario?')) {return false};");
            btn_VisibilitaRuoli.Attributes.Add("onClick", "ShowDialogRuoliTitolario();");
            txtMesiConservazione.Attributes.Add("onKeyPress", "ValidateNumericKey();");
            txt_protoTitolario.Attributes.Add("onKeyPress", "ValidateNumericKey();");
        }

        /// <summary>
        /// Impostazione del focus su un controllo
        /// </summary>
        /// <param name="controlID"></param>
        private void SetControlFocus(string controlID)
        {
            RegisterClientScript("SetFocus", "SetControlFocus('" + controlID + "');");
        }

        #endregion

        #region Gestione TreeView titolario

        /// <summary>
        /// Caricamento dei nodi di titolario 
        /// </summary>
        /// <param name="idParentTitolario"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.OrgNodoTitolario[] GetNodiTitolario(string idParentTitolario, string idRegistro)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.AmmGetNodiTitolario(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"), idParentTitolario, idRegistro);
        }

        private void FillTreeView(string idRegistro)
        {
            if (ddl_registri.SelectedIndex != 0)
            {

                DocsPAWA.DocsPaWR.Registro registro = wws.GetRegistroBySistemId(idRegistro);
                if (registro.Sospeso)
                {
                    RegisterClientScript("alertRegistroSospeso", "alert('Il registro selezionato è sospeso!');");
                    ddl_registri.SelectedIndex = 0;
                    //RegisterClientScript("refresh", "document.forms[0].submit();");
                    return;
                }
            }
            if (trvNodiTitolario.Nodes.Count > 0)
                trvNodiTitolario.Nodes.Clear();

            AddRootNodes(idRegistro);
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

            FillTreeNodes((TreeNodeTitolario)rootNode, idRegistro);

            return rootNode;
        }

        private TreeNodeTitolario CreateNewTreeNodeTitolario(DocsPAWA.DocsPaWR.OrgNodoTitolario nodoTitolario)
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
            DocsPAWA.DocsPaWR.OrgNodoTitolario[] nodiTitolario = GetNodiTitolario(parentNode.ID, idRegistro);

            // Rimozione del nodo inserito per l'attesa del caricamento
            if (parentNode.Nodes.Count > 0 && parentNode.Nodes[0].ID == "WAITING_NODE")
                parentNode.Nodes.Remove(parentNode.Nodes[0]);

            foreach (DocsPAWA.DocsPaWR.OrgNodoTitolario nodoTitolario in nodiTitolario)
            {
                TreeNodeTitolario treeNodeTitolario = CreateNewTreeNodeTitolario(nodoTitolario);
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
                        parentTreeNodeIndex = ExpandNode(id, parentTreeNodeIndex);
                    }

                    // Impostazione nodo corrente
                    trvNodiTitolario.SelectedNodeIndex = parentTreeNodeIndex;

                    // Caricamento dati del titolario correntemente selezionato
                    PerformSelectionCurrentTitolario();
                }
            }
            catch
            {
                ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }
        }

        /// <summary>
        /// Reperimento, in base all'indice univoco, di un oggetto "TreeNodeTitolario"
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <returns></returns>
        private Titolario.TreeNodeTitolario GetNodeFromIndex(string nodeIndex)
        {
            return trvNodiTitolario.GetNodeFromIndex(nodeIndex) as Titolario.TreeNodeTitolario;
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
                TreeNodeTitolario parentNode = (TreeNodeTitolario)trvNodiTitolario.GetNodeFromIndex(parentTreeNodeIndex);

                foreach (TreeNodeTitolario nodeTitolario in parentNode.Nodes)
                {
                    if (nodeTitolario.GetNodoTitolario().ID == idTitolario)
                    {
                        // Reperimento indice del nodo
                        retValue = nodeTitolario.GetNodeIndex();

                        // Caricamento nodi titolario figli
                        FillTreeNodes(nodeTitolario, getRegistroSelezionato());

                        parentNode.Expanded = true;

                        break;
                    }
                }

            }
            catch
            {
                ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
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
                    TreeNodeTitolario selectedNode = GetNodeFromIndex(e.Node);
                    FillTreeNodes(selectedNode, getRegistroSelezionato());

                    impostaVisibilitaControlli(selectedNode.GetNodoTitolario());

                    // Impostazione del nodo come correntemente selezionato
                    trvNodiTitolario.SelectedNodeIndex = selectedNode.GetNodeIndex();
                    selectedNode = null;

                    // Assegnazione dei valori alla UI
                    PerformSelectionCurrentTitolario();
                }
            }
            catch
            {
                ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trvNodiTitolario_SelectedIndexChange(object sender, Microsoft.Web.UI.WebControls.TreeViewSelectEventArgs e)
        {
            PerformSelectionCurrentTitolario();
        }

        #endregion

        #region Gestione impostazione e reperimento oggetto "OrgNodoTitolario" corrente

        private DocsPAWA.DocsPaWR.OrgNodoTitolario GetParentTitolario()
        {
            DocsPAWA.DocsPaWR.OrgNodoTitolario retValue = null;

            if (trvNodiTitolario.SelectedNodeIndex != "0")
            {
                TreeNodeTitolario selectedNode = GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);
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
        private DocsPAWA.DocsPaWR.OrgNodoTitolario GetCurrentTitolario()
        {
            DocsPAWA.DocsPaWR.OrgNodoTitolario retValue = null;

            //Modifica per NuovaGestioneTitolario

            if (trvNodiTitolario.SelectedNodeIndex != "0")
            {
                TreeNodeTitolario selectedNode = GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);
                retValue = selectedNode.GetNodoTitolario();
                selectedNode = null;
                if (retValue.ID_Titolario == "0")
                    retValue = null;
            }
            return retValue;

            /*
            if (trvNodiTitolario.SelectedNodeIndex.Length > 3)
            {
                TreeNodeTitolario selectedNode = GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);
                retValue = selectedNode.GetNodoTitolario();
                selectedNode = null;
            }
            return retValue;


            if (trvNodiTitolario.SelectedNodeIndex != "0")
            {
                TreeNodeTitolario selectedNode = GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);
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
        private void SetCurrentTitolario(DocsPAWA.DocsPaWR.OrgNodoTitolario nodoTitolario)
        {
            TreeNodeTitolario selectedNode = GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);

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
            RegisterClientScript("ErrorMessage", "alert('" + errorMessage.Replace("'", "\\'") + "')");
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
                DocsPAWA.DocsPaWR.OrgNodoTitolario currentTitolario = GetCurrentTitolario();

                if (currentTitolario == null)
                {
                    // Posizionamento sul nodo root (amministrazione)

                    // Impostazione controlli UI per modalità di aggiornamento
                    SetUpdateMode();

                    /*
                    // Impostazione visibilità pannello campi UI del titolario
                    SetVisibilityPanelDetailsTitolario(false);
					
                    // Abilitazione pulsanti per l'inserimento
                    EnableButtons(true);

                    // Impostazione visibilità panel spostamento nodi
                    SetVisibilityPanelSpostaNodo(false);
                    */

                }
                else
                {
                    // Associazione dati
                    BindDataTitolario(currentTitolario);

                    // Impostazione controlli UI per modalità di aggiornamento
                    SetUpdateMode();

                    /*
                    // Impostazione visibilità pannello campi UI del titolario
                    SetVisibilityPanelDetailsTitolario(true);

                    // Impostazione visibilità panel spostamento nodi
                    SetVisibilityPanelSpostaNodo(true);
                    */
                }
            }
            catch
            {
                //SetVisibilityPanelDetailsTitolario(false);
                ShowErrorMessage("Si è verificato un errore durante il bind dei dati del titolario.");
            }
        }

        /// <summary>
        /// Associazione dati titolario ai campi della UI
        /// </summary>
        private void BindDataTitolario(DocsPAWA.DocsPaWR.OrgNodoTitolario nodoTitolario)
        {
            txt_codiceInfo.Text = nodoTitolario.Codice;
            txt_codiceInfo_provv.Text = string.Empty;
            txt_descrizioneInfo.Text = nodoTitolario.Descrizione;
            txt_noteNodo.Text = nodoTitolario.note;

            FillComboRegistroNodo(nodoTitolario.IDRegistroAssociato, false);
            //ddl_registriInfo.SelectedValue=nodoTitolario.IDRegistroAssociato;

            if (nodoTitolario.CreazioneFascicoliAbilitata)
                ddl_rw.SelectedValue = "W";
            else
                ddl_rw.SelectedValue = "R";

            //Blocco creazione nodi figli
            if (nodoTitolario.bloccaNodiFigli == "SI")
                ddl_creazioneFigli.SelectedValue = "SI";
            else
                ddl_creazioneFigli.SelectedValue = "NO";

            //Blocco classificazione
            //string valoreChiave;
            //valoreChiave = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
            //if (valoreChiave.Equals("1"))
            //{
            ddl_classificazione.SelectedIndex = 0;
            if (nodoTitolario.consentiClassificazione != null && nodoTitolario.consentiClassificazione == "1")
                ddl_classificazione.SelectedIndex = 1;
            //}

            this.ddl_consentiFasc.SelectedIndex = 0;
            if (nodoTitolario.consentiFascicolazione != null && nodoTitolario.consentiFascicolazione == "1")
                this.ddl_consentiFasc.SelectedIndex = 1;

            //Attivazione contatore
            if (nodoTitolario.contatoreAttivo == "SI")
                ddl_attivaContatore.SelectedValue = "SI";
            else
                ddl_attivaContatore.SelectedValue = "NO";

            //Protocollo titolario            
            if (!string.IsNullOrEmpty(nodoTitolario.numProtoTit))
            {
                txt_protoTitolario.Text = nodoTitolario.numProtoTit;
                lbl_protoTtitolario.Visible = true;
                txt_protoTitolario.Visible = true;
            }
            else
            {
                lbl_protoTtitolario.Visible = false;
                txt_protoTitolario.Visible = false;
                txt_protoTitolario.Text = string.Empty;
            }

            txtMesiConservazione.Text = nodoTitolario.NumeroMesiConservazione.ToString();

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
                cb_bloccaTipologia.Checked = (nodoTitolario.bloccaTipoFascicolo == "SI");
            }

            string valorechiave = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");
            if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
                ddStrutturaTemplate.SelectedValue = string.IsNullOrEmpty(nodoTitolario.IDTemplateStrutturaSottofascicoli) ?
                    "-1" : nodoTitolario.IDTemplateStrutturaSottofascicoli;

            nodoTitolario = null;

            // Aggiornamento parametri query string 
            // necessari per la visualizzazione della dialog dei ruoli
            RefreshQueryStringDialogRuoliTitolario();
        }

        /// <summary>
        /// Creazione oggetto "OrgNodoTitolario": 
        /// rappresenta il nodo di titolario correntemente selezionato
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.OrgNodoTitolario CreateNewNodoTitolario(DocsPAWA.DocsPaWR.OrgNodoTitolario parentNodoTitolario)
        {
            DocsPAWA.DocsPaWR.OrgNodoTitolario retValue = new DocsPAWA.DocsPaWR.OrgNodoTitolario();

            if (parentNodoTitolario != null)
            {
                retValue.IDParentNodoTitolario = parentNodoTitolario.ID;
                retValue.Codice = parentNodoTitolario.Codice + "." + txt_codiceInfo_provv.Text;

                //Modifica per NuovaGestioneTitolario - Aggiunto questo if
                if (parentNodoTitolario.ID_Titolario == "0")
                    retValue.ID_Titolario = parentNodoTitolario.ID;
                else
                    retValue.ID_Titolario = parentNodoTitolario.ID_Titolario;
            }
            else
            {
                //Modifica per NuovaGestioneTitolario
                //if (trvNodiTitolario.SelectedNodeIndex.Length == 3)
                if (trvNodiTitolario.SelectedNodeIndex != "0")
                {
                    TreeNodeTitolario selectedNode = GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);
                    DocsPAWA.DocsPaWR.OrgNodoTitolario nodeTit = selectedNode.GetNodoTitolario();
                    selectedNode = null;

                    retValue.IDParentNodoTitolario = nodeTit.ID;
                    retValue.Codice = txt_codiceInfo.Text;
                    if (nodeTit.ID_Titolario == "0")
                        retValue.ID_Titolario = nodeTit.ID;
                    else
                        retValue.ID_Titolario = nodeTit.ID_Titolario;
                }
                else
                {
                    retValue.IDParentNodoTitolario = "0";
                    retValue.Codice = txt_codiceInfo.Text;
                }
            }

            retValue.Descrizione = txt_descrizioneInfo.Text;
            retValue.IDRegistroAssociato = ddl_registriInfo.SelectedValue;
            retValue.CodiceAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
            retValue.CreazioneFascicoliAbilitata = (ddl_rw.SelectedValue == "W");

            retValue.note = txt_noteNodo.Text;

            int numeroMesiConservazione = 0;
            if (txtMesiConservazione.Text.Length > 0)
                numeroMesiConservazione = Convert.ToInt32(txtMesiConservazione.Text);
            retValue.NumeroMesiConservazione = numeroMesiConservazione;

            if (parentNodoTitolario != null)
            {
                retValue.Livello = (Convert.ToInt32(parentNodoTitolario.Livello) + 1).ToString();
                retValue.CodiceLivello = GetCodiceLivello(parentNodoTitolario.CodiceLivello,
                                                        retValue.Livello,
                                                        retValue.CodiceAmministrazione, retValue.ID_Titolario, retValue.IDRegistroAssociato);
            }
            else
            {
                retValue.Livello = "1";
                retValue.CodiceLivello = GetCodiceLivello("", retValue.Livello, retValue.CodiceAmministrazione, retValue.ID_Titolario, retValue.IDRegistroAssociato);
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
            if (ddl_creazioneFigli.SelectedValue == "SI")
                retValue.bloccaNodiFigli = "SI";
            else
                retValue.bloccaNodiFigli = "NO";

            //Attivazione contatore
            if (ddl_attivaContatore.SelectedValue == "SI")
                retValue.contatoreAttivo = "SI";
            else
                retValue.contatoreAttivo = "NO";

            //Protocollo Titolario
            if (txt_protoTitolario.Text.Length > 0)
                retValue.numProtoTit = txt_protoTitolario.Text;

            //Consenti classificazione
            if (ddl_classificazione.SelectedValue == "SI")
                retValue.consentiClassificazione = "1";
            else
                retValue.consentiClassificazione = "0";

            //Consenti solo fascicolazione
            if (this.ddl_consentiFasc.SelectedValue == "SI")
                retValue.consentiFascicolazione = "1";
            else
                retValue.consentiFascicolazione = "0";
            return retValue;
        }

        /// <summary>
        /// Creazione oggetto "OrgNodoTitolario"  utilizzabile per l'update
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.OrgNodoTitolario GetUpdatedNodoTitolario()
        {
            DocsPAWA.DocsPaWR.OrgNodoTitolario originalNodoTitolario = GetCurrentTitolario();

            DocsPAWA.DocsPaWR.OrgNodoTitolario updatedNodoTitolario = new DocsPAWA.DocsPaWR.OrgNodoTitolario();
            updatedNodoTitolario.ID = originalNodoTitolario.ID;
            updatedNodoTitolario.IDParentNodoTitolario = originalNodoTitolario.IDParentNodoTitolario;
            updatedNodoTitolario.Codice = originalNodoTitolario.Codice;
            updatedNodoTitolario.CodiceAmministrazione = originalNodoTitolario.CodiceAmministrazione;
            updatedNodoTitolario.CodiceLivello = originalNodoTitolario.CodiceLivello;
            updatedNodoTitolario.Livello = originalNodoTitolario.Livello;
            updatedNodoTitolario.CountChildNodiTitolario = originalNodoTitolario.CountChildNodiTitolario;

            // Aggiornamento dati nell'oggetto titolario rispetto ai dati immessi nella UI
            updatedNodoTitolario.Descrizione = txt_descrizioneInfo.Text;
            updatedNodoTitolario.IDRegistroAssociato = ddl_registriInfo.SelectedValue;
            updatedNodoTitolario.CreazioneFascicoliAbilitata = (ddl_rw.SelectedValue == "W");

            updatedNodoTitolario.note = txt_noteNodo.Text;

            if (txtMesiConservazione.Text.Length > 0)
                updatedNodoTitolario.NumeroMesiConservazione = Convert.ToInt32(txtMesiConservazione.Text);
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
            if (ddl_creazioneFigli.SelectedValue == "SI")
                updatedNodoTitolario.bloccaNodiFigli = "SI";
            else
                updatedNodoTitolario.bloccaNodiFigli = "NO";

            //Blocca classificazione
            if (ddl_classificazione.SelectedValue == "SI")
                updatedNodoTitolario.consentiClassificazione = "1";
            else
                updatedNodoTitolario.consentiClassificazione = "0";

            //Consenti fascicolazione
            if (this.ddl_consentiFasc.SelectedValue == "SI")
                updatedNodoTitolario.consentiFascicolazione = "1";
            else
                updatedNodoTitolario.consentiFascicolazione = "0";

            //Attivazione contatore
            if (ddl_attivaContatore.SelectedValue == "SI")
                updatedNodoTitolario.contatoreAttivo = "SI";
            else
                updatedNodoTitolario.contatoreAttivo = "NO";

            //Protocollo Titolario
            if (txt_protoTitolario.Text.Length > 0)
                updatedNodoTitolario.numProtoTit = txt_protoTitolario.Text;
            else
                updatedNodoTitolario.numProtoTit = string.Empty;

            //NuovaGestioneTitolario
            updatedNodoTitolario.ID_Titolario = originalNodoTitolario.ID_Titolario;
            updatedNodoTitolario.dataAttivazione = originalNodoTitolario.dataAttivazione;
            updatedNodoTitolario.dataCessazione = originalNodoTitolario.dataCessazione;
            updatedNodoTitolario.stato = originalNodoTitolario.stato;

            // Id Template struttura 
            updatedNodoTitolario.IDTemplateStrutturaSottofascicoli = 
                (ddStrutturaTemplate.SelectedValue == "-1") ? "" : ddStrutturaTemplate.SelectedValue;

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
                ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
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
                DocsPAWA.DocsPaWR.OrgNodoTitolario parentTitolario = GetCurrentTitolario();
                DocsPAWA.DocsPaWR.OrgNodoTitolario newNodoTitolario = CreateNewNodoTitolario(parentTitolario);
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

                DocsPAWA.DocsPaWR.EsitoOperazione ret = ws.AmmInsertTitolario(ref newNodoTitolario, idAmm);
                ws = null;

                if (ret.Codice == 0)
                {
                    TreeNodeTitolario parentTreeNodeTitolario = GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);
                    if (parentTreeNodeTitolario != null)
                    {
                        if (!parentTreeNodeTitolario.Expanded)
                            parentTreeNodeTitolario.Expanded = true;

                        // Creazione nuovo nodo titolario per il treeview
                        Microsoft.Web.UI.WebControls.TreeNode newTreeNodeTitolario = CreateNewTreeNodeTitolario(newNodoTitolario);
                        parentTreeNodeTitolario.Nodes.Add(newTreeNodeTitolario);

                        // Impostazione del nuovo nodo come correntemente selezionato
                        trvNodiTitolario.SelectedNodeIndex = newTreeNodeTitolario.GetNodeIndex();
                    }

                    PerformSelectionCurrentTitolario();

                    string valoreChiave;
                    valoreChiave = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
                    //BLOCCA CLASSIFICAZIONE
                    if (valoreChiave.Equals("1") && parentTitolario!=null && parentTitolario.CountChildNodiTitolario > 0)
                    {
                        DocsPAWA.DocsPaWR.OrgNodoTitolario parentNode = parentTreeNodeTitolario.GetNodoTitolario();
                        parentNode.consentiClassificazione = "0";
                        parentNode.CreazioneFascicoliAbilitata = false;
                        ((TreeNodeTitolario)parentTreeNodeTitolario).SetNodoTitolario(parentNode);

                        if (ddl_classificazione.SelectedIndex == 1)
                        {
                            // INC000000513718
                            // Rimosso il messaggio di warning
                            //validationMessage = "Non è consentita la classificazione su nodi titolario aventi nodi figli";
                            //RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");
                            //return false;
                        }
                        else
                        {
                            validationMessage = "Non sarà più possibile classificare documenti sul nodo: " + parentTitolario.Codice + " - " + parentTitolario.Descrizione;
                            RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");
                        }
                    }

                    retValue = true;
                }
                else
                {
                    // Aggiornamento non andato a buon fine, 
                    // visualizzazione messaggio di errore
                    validationMessage = "Non è stato possibile inserire il titolario: " + @"\n\n" + ret.Descrizione;
                    RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");
                    SetControlFocus("txt_descrizioneInfo");
                }
            }
            catch
            {
                ShowErrorMessage("Si è verificato un errore durante l'inserimento del titolario.");
                SetControlFocus("txt_descrizioneInfo");
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
                valoreChiave = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
                if (valoreChiave.Equals("1"))
                {
                    DocsPAWA.DocsPaWR.OrgNodoTitolario nodoTitolario = GetCurrentTitolario();
                    if (nodoTitolario.CountChildNodiTitolario > 0 && this.ddl_consentiFasc.SelectedValue.Equals("NO") && ddl_rw.SelectedIndex == 1)
                    {
                        validationMessage = "Non è stato possibile aggiornare i dati: " + @"\n\n" + "Non è possibile creare fascicoli se la classificazione sul nodo risulta bloccata.";
                        RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");
                        SetControlFocus("txt_descrizioneInfo");
                        return false;
                    }
                }

                string errorMessage;
                string firstInvalidControlID;

                // Verifica presenza dati obbligatori
                if (ContainsRequiredFields(out errorMessage, out firstInvalidControlID))
                {
                    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                    DocsPAWA.DocsPaWR.OrgNodoTitolario updatedNodoTitolario = GetUpdatedNodoTitolario();
                    DocsPAWA.DocsPaWR.EsitoOperazione ret = ws.AmmUpdateTitolario(updatedNodoTitolario);

                    if (ret.Codice == 0)
                    {
                        // Se l'update è andato a buon fine, viene aggiornato il nodo titolario corrente
                        SetCurrentTitolario(updatedNodoTitolario);

                        // Aggiornamento parametri querystring necessari
                        // per la visualizzazione della dialog "RuoliTitolario.aspx"
                        RefreshQueryStringDialogRuoliTitolario();

                        retValue = true;
                    }
                    else
                    {
                        // Aggiornamento non andato a buon fine, visualizzazione messaggio di errore
                        validationMessage = "Non è stato possibile aggiornare i dati: " + @"\n\n" + ret.Descrizione;
                        RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");
                        SetControlFocus("txt_descrizioneInfo");
                    }
                }
                else
                {
                    RegisterClientScript("ValidationMessage", "alert('" + errorMessage + "');");

                    // Impostazione focus sul primo campo non valido
                    SetControlFocus(firstInvalidControlID);
                }
            }
            catch
            {
                ShowErrorMessage("Si è verificato un errore durante la modifica del titolario.");
            }

            return retValue;
        }

        private bool DeleteTitolario()
        {
            bool retValue = false;

            try
            {
                DocsPAWA.DocsPaWR.OrgNodoTitolario titolarioToDelete = GetCurrentTitolario();

                if (titolarioToDelete != null)
                {
                    AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                    DocsPAWA.DocsPaWR.EsitoOperazione ret = ws.AmmDeleteTitolario(titolarioToDelete);

                    if (ret.Codice == 0)
                    {
                        // Rimozione del nodo
                        TreeNodeTitolario treeNodeTitolario = GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);
                        treeNodeTitolario.Remove();

                        // Assegnazione dati alla UI relativamente al nuovo nodo selezionato
                        PerformSelectionCurrentTitolario();
                    }
                    else
                    {
                        // Cancellazione non andata a buon fine, visualizzazione messaggio di errore
                        string validationMessage = "Non è stato possibile cancellare il titolario: " +
                            @"\n\n" + ret.Descrizione;

                        RegisterClientScript("ValidationMessage", "alert('" + validationMessage + "');");
                    }
                }
            }
            catch
            {
                ShowErrorMessage("Si è verificato un errore durante la cancellazione del titolario.");
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
            if (IsInsertMode())
            {
                string errorMessage;
                string firstInvalidControlID;

                if (ContainsRequiredFields(out errorMessage, out firstInvalidControlID))
                {
                    if (InsertNewTitolario())
                    {
                        // Ripristino modalità di update solo se l'inserimento è andato a buon fine
                        SetUpdateMode();
                    }
                }
                else
                {
                    RegisterClientScript("ValidationMessage", "alert('" + errorMessage + "');");
                    SetControlFocus(firstInvalidControlID);
                }
            }
            else
            {
                SetInsertMode();
            }
        }

        /// <summary>
        /// Modifica titolario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_salvaInfo_Click(object sender, System.EventArgs e)
        {
            SaveTitolario();
        }

        /// <summary>
        /// Cancellazione titolario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_elimina_Click(object sender, System.EventArgs e)
        {
            DeleteTitolario();
        }

        /// <summary>
        /// Apertura popup indice sistematico
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_indiceSis_Click(object sender, System.EventArgs e)
        {
            DocsPAWA.DocsPaWR.OrgNodoTitolario nodoSelezionato = GetNodoSelezionato();
            Session.Add("nodoSelPerIndice", nodoSelezionato);
            ClientScript.RegisterStartupScript(GetType(), "popUpIndiceSistematico", "apriPopupIndiceSistematico();", true);
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
				configValue=Convert.ToBoolean(DocsPAWA.ConfigSettings.getKey(DocsPAWA.ConfigSettings.KeysENUM.AMM_SPOSTA_NODI_TITOLARIO));

				Microsoft.Web.UI.WebControls.TreeNode selectedNode=GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);				
				bool hasSibling=(selectedNode.GetSiblingNodeCollection().Count > 1);
				selectedNode=null;
			 
				tblRowSpostaNodoTitolario.Visible=true;

				if(configValue && onNodeTitolario && hasSibling && checkDueRegistriUguali())
				{					
					btnMoveUp.Visible=CanMoveUp();
					btnMoveDown.Visible=CanMoveDown();
					tblRowSpostaNodoTitolario.Visible = (btnMoveUp.Visible || btnMoveDown.Visible);
				}
				else
				{
					tblRowSpostaNodoTitolario.Visible=false;
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
			tblDetailsTitolario.Visible=isVisibile;
			if (!isVisibile)
				ClearFieldsDetailsTitolario();
		}

		/// <summary>
		/// Gestione abilitazione / disabilitazione controlli per
		/// l'acquisizione dei dati del titolario
		/// </summary>
		/// <param name="insertMode"></param>
		private void SetVisibilityFieldsDetailsTitolario(bool insertMode,bool onAdminNode)
		{
			txt_codiceInfo.Enabled=insertMode && onAdminNode;
			lbl_codiceInfo_provv.Visible=insertMode && !onAdminNode;
            txt_codiceInfo_provv.Visible=lbl_codiceInfo_provv.Visible;
		}

		/// <summary>
		/// Gestione abilitazione / disabilitazione pulsanti
		/// </summary>
		private void EnableButtons(bool insertMode)
		{
			btn_aggiungiNew.Enabled=true;
			btn_salvaInfo.Enabled=!insertMode;
			btn_elimina.Enabled=!insertMode;
			btn_VisibilitaRuoli.Enabled=!insertMode;
        }
        */
        #endregion

        /// <summary>
        /// Impostazione controlli UI per la modalità
        /// di modifica di un titolario
        /// </summary>
        private void impostaVisibilitaControlli(DocsPAWA.DocsPaWR.OrgNodoTitolario nodoParam)
        {
            DocsPAWA.DocsPaWR.OrgNodoTitolario nodo = nodoParam;
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

                if (IsInsertMode())
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

                    txt_codiceInfo.Text = "";
                    txt_codiceInfo.Enabled = true;

                    lbl_codiceInfo_provv.Visible = false;
                    txt_codiceInfo_provv.Visible = false;
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
                DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(nodo.ID_Titolario);
                if (titolario != null)
                {
                    switch (titolario.Stato)
                    {
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            btn_aggiungiNew.Enabled = false;
                            btn_elimina.Enabled = false;
                            btn_indiceSis.Enabled = true;
                            btn_salvaInfo.Enabled = false;
                            btn_VisibilitaRuoli.Enabled = true;
                            btn_VisibilitaRuoli.Visible = true;
                            break;
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            btn_aggiungiNew.Enabled = true;
                            btn_elimina.Enabled = true;
                            btn_indiceSis.Enabled = true;
                            btn_salvaInfo.Enabled = true;
                            btn_VisibilitaRuoli.Enabled = true;
                            btn_VisibilitaRuoli.Visible = true;
                            break;
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione:
                            btn_aggiungiNew.Enabled = true;
                            btn_elimina.Enabled = true;
                            btn_indiceSis.Enabled = true;
                            btn_salvaInfo.Enabled = true;
                            btn_VisibilitaRuoli.Enabled = true;
                            btn_VisibilitaRuoli.Visible = true;
                            break;
                    }
                }

                txt_codiceInfo.Enabled = false;

                if (IsInsertMode())
                {
                    lbl_codiceInfo_provv.Visible = true;
                    txt_codiceInfo_provv.Visible = true;
                    btn_elimina.Enabled = false;
                    btn_indiceSis.Enabled = false;
                    btn_salvaInfo.Enabled = false;
                    btn_VisibilitaRuoli.Enabled = false;
                }
                else
                {
                    lbl_codiceInfo_provv.Visible = false;
                    txt_codiceInfo_provv.Visible = false;
                }

                if (nodo.Livello == titolario.MaxLivTitolario)
                    btn_aggiungiNew.Enabled = false;
            }
        }

        private void setDettagliTitolario(DocsPAWA.DocsPaWR.OrgNodoTitolario nodoParam)
        {

            txt_CommentoTit.Text = string.Empty;
            lbl_DescrizioneTitolario.Text = string.Empty;
            txt_DescrizioneTit.Text = "Titolario";

            if (nodoParam != null && pnl_DettagliTitolario.Visible)
            {
                DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(nodoParam.ID);
                if (titolario != null)
                {
                    txt_CommentoTit.Text = titolario.Commento;
                    txt_DescrizioneTit.Text = nodoParam.Descrizione;
                    lbl_DescrizioneTitolario.Text = titolario.Descrizione;
                    if (!string.IsNullOrEmpty(titolario.MaxLivTitolario))
                        ddl_LivelliTit.SelectedValue = titolario.MaxLivTitolario;

                    //switch (titolario.Stato)
                    //{
                    //    case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                    //        lbl_DescrizioneTitolario.Text = " - " + nodoParam.Descrizione + " Attivo";
                    //        break;

                    //    case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione:
                    //        lbl_DescrizioneTitolario.Text = " - " + nodoParam.Descrizione + " in definizione";
                    //        break;

                    //    case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                    //        lbl_DescrizioneTitolario.Text = " - " + nodoParam.Descrizione + " in vigore dal " + nodoParam.dataAttivazione + " al " + nodoParam.dataCessazione;
                    //        break;
                    //}
                }
            }
        }

        private DocsPAWA.DocsPaWR.OrgNodoTitolario GetNodoSelezionato()
        {
            DocsPAWA.DocsPaWR.OrgNodoTitolario retValue = null;

            if (trvNodiTitolario.SelectedNodeIndex.Length > 2)
            {
                TreeNodeTitolario selectedNode = GetNodeFromIndex(trvNodiTitolario.SelectedNodeIndex);
                retValue = selectedNode.GetNodoTitolario();
                selectedNode = null;
            }
            return retValue;
        }

        private void ClearFieldsDetailsTitolario()
        {
            txt_codiceInfo_provv.Text = string.Empty;
            txt_codiceInfo.Text = string.Empty;
            txt_descrizioneInfo.Text = string.Empty;
            txt_noteNodo.Text = string.Empty;
        }

        /// <summary>
        /// Impostazione controlli UI per la modalità
        /// di modifica di un titolario
        /// </summary>
        private void SetUpdateMode()
        {
            btn_aggiungiNew.Text = "Aggiungi nodo figlio";

            ViewState["INSERT_MODE"] = false;

            impostaVisibilitaControlli(null);

            /*
            // Abilitazione / disabilitazione pulsanti per modalità inserimento
            if(checkDueRegistriUguali())
            {
                EnableButtons(false);
            }
            else
            {
                EnableButtons(true);
            }

            /// Gestione abilitazione / disabilitazione controlli per
            /// l'acquisizione dei dati del titolario
            SetVisibilityFieldsDetailsTitolario(false,false);
            */
        }

        /// <summary>
        /// Impostazione controlli UI per la modalità
        /// di inserimento di un nuovo titolario
        /// </summary>
        private void SetInsertMode()
        {
            txt_descrizioneInfo.Text = string.Empty;
            txtMesiConservazione.Text = string.Empty;
            txt_noteNodo.Text = string.Empty;
            ddl_classificazione.SelectedIndex = 1;

            DocsPAWA.DocsPaWR.OrgNodoTitolario parentTitolario = GetCurrentTitolario();

            //Protocollo Titolario
            string contatoreTitolario = wws.isEnableContatoreTitolario();
            if (parentTitolario != null && parentTitolario.contatoreAttivo == "SI" && !string.IsNullOrEmpty(contatoreTitolario))
            {
                lbl_protoTtitolario.Visible = true;
                txt_protoTitolario.Visible = true;
                txt_protoTitolario.Text = wws.getContatoreProtTitolario(parentTitolario).ToString();
            }
            else
            {
                txt_protoTitolario.Text = string.Empty;
            }

            // Se true, inserimento di un titolario in amministrazione
            bool onAdminNode = (parentTitolario == null);

            if (!onAdminNode)
            {
                // Inserimento di un sottotitolario
                btn_aggiungiNew.Text = "Aggiungi sotto " + parentTitolario.Codice;

                // gestione combo-box registri
                FillComboRegistroNodo(parentTitolario.IDRegistroAssociato, true);
            }
            else
            {
                // gestione combo-box registri
                FillComboRegistroNodo(ddl_registri.SelectedValue.ToString(), true);
            }

            /*
			SetVisibilityPanelDetailsTitolario(true);

			/// Gestione abilitazione / disabilitazione controlli per
			/// l'acquisizione dei dati del titolario
			SetVisibilityFieldsDetailsTitolario(true,onAdminNode);

			// Abilitazione / disabilitazione pulsanti per modalità inserimento
			EnableButtons(true);
			*/

            // Flag in viewstate che determina se si è in modalità di inserimento o meno
            ViewState["INSERT_MODE"] = true;

            string focusControlName = string.Empty;

            if (onAdminNode)
                focusControlName = txt_codiceInfo.ID;
            else
                focusControlName = txt_codiceInfo_provv.ID;

            SetControlFocus(focusControlName);

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
            if (ViewState["INSERT_MODE"] != null)
                retValue = (bool)ViewState["INSERT_MODE"];

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

            if (IsInsertMode())
            {
                if (txt_codiceInfo_provv.Visible && txt_codiceInfo_provv.Text.Trim().Length == 0)
                {
                    listErrorMessage.Add("Codice titolario mancante");
                    firstInvalidControlID = txt_codiceInfo_provv.ID;
                }
                else if (txt_codiceInfo.Text.Trim().Length == 0)
                {
                    listErrorMessage.Add("Codice titolario mancante");
                    firstInvalidControlID = txt_codiceInfo.ID;
                }

                //Controllo che nei codici titolario non è presente il carattere "." 
                // e soprattutto che il codice non sia "T" lettera usata per identificare esclusivamente i titolari
                if (txt_codiceInfo.Enabled && txt_codiceInfo.Text.IndexOf(".") != -1)
                {
                    listErrorMessage.Add("Il carattere \".\" non è permesso");
                    txt_codiceInfo.Text = "";
                }
                if (txt_codiceInfo_provv.Enabled && txt_codiceInfo_provv.Text.IndexOf(".") != -1)
                {
                    listErrorMessage.Add("Il carattere \".\" non è permesso");
                    txt_codiceInfo_provv.Text = "";
                }

                if (txt_codiceInfo.Enabled && txt_codiceInfo.Text.Equals("T"))
                {
                    listErrorMessage.Add("Come codice non è possibile usare la lettera \"T\"");
                    txt_codiceInfo.Text = "";
                }
                if (txt_codiceInfo_provv.Enabled && txt_codiceInfo_provv.Text.Equals("T"))
                {
                    listErrorMessage.Add("Come codice non è possibile usare la lettera \"T\"");
                    txt_codiceInfo_provv.Text = "";
                }
            }

            if (txt_descrizioneInfo.Text.Trim().Length == 0)
            {
                listErrorMessage.Add("Descrizione titolario mancante");
                if (firstInvalidControlID == string.Empty)
                    firstInvalidControlID = txt_descrizioneInfo.ID;
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
            if (cboTipoRicerca.Items.Count > 0)
                cboTipoRicerca.Items.Clear();

            cboTipoRicerca.Items.Add(new ListItem("Codice", FILTER_TYPE_CODICE));
            cboTipoRicerca.Items.Add(new ListItem("Descrizione", FILTER_TYPE_DESCRIZIONE));
        }

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            SearchTitolario(cboTipoRicerca.SelectedItem.Value, txtFieldRicerca.Text);
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
                    string xmlStream = ws.filtroRicerca(codice, descrizione, codAmm, getRegistroSelezionato());
                    if (xmlStream == "<NewDataSet />")
                    {
                        ShowErrorMessage("Nessun risultato trovato");
                    }
                    else
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(xmlStream);

                        XmlNode lista = doc.SelectSingleNode("NewDataSet");
                        if (lista.ChildNodes.Count > 0)
                        {
                            if (!IsStartupScriptRegistered("apriRisultato"))
                            {
                                string scriptString = GeneraTesto(lista, getRegistroSelezionato());
                                RegisterStartupScript("apriRisultato", scriptString);
                            }
                        }
                    }
                }
            }
            catch
            {
                ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
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
                ShowErrorMessage("Si è verificato un errore durante l'esecuzione di questa operazione.");
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
            FillTreeView(getRegistroSelezionato());

            PerformSelectionCurrentTitolario();
        }

        /// <summary>
        /// restituisce il registro di gestione selezionato
        /// </summary>
        /// <returns></returns>
        private string getRegistroSelezionato()
        {
            string retValue = string.Empty;
            retValue = ddl_registri.SelectedValue;
            return retValue;
        }

        private bool checkDueRegistriUguali()
        {
            bool retValue = false;
            if (ddl_registri.SelectedValue.Equals(ddl_registriInfo.SelectedValue))
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

                ArrayList listaTipiFasc = new ArrayList(DocsPAWA.ProfilazioneFascManager.getTipoFasc(idAmministrazione, this));
                ListItem item = new ListItem();
                item.Value = "";
                item.Text = "";
                ddl_tipologiaFascicoli.Items.Add(item);
                for (int i = 0; i < listaTipiFasc.Count; i++)
                {
                    DocsPAWA.DocsPaWR.Templates templates = (DocsPAWA.DocsPaWR.Templates)listaTipiFasc[i];
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
            //RefreshQueryStringDialogRuoliTitolario();
            // Rimozione dati sessione utilizzati nella popup dei ruoli del titolario
            DocsPAWA.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            //DocsPAWA.DocsPaWR.OrgTitolario titolario = new DocsPAWA.DocsPaWR.OrgTitolario();
            //titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            //titolario.ID = nodo.ID;
            string queryString = "?ID_NODO_TITOLARIO=&ID_REGISTRO=";
            if (!ddl_registri.SelectedValue.Equals(string.Empty))
                queryString += ddl_registri.SelectedValue + "&ID_TITOLARIO=" + nodo.ID;
            else
                queryString += "&ID_TITOLARIO=" + nodo.ID;
            //string queryString = "?ID_NODO_TITOLARIO=&ID_REGISTRO=" + nodo.IDRegistroAssociato + "&ID_TITOLARIO=" + nodo.ID;

            txtQueryStringDialogRuoliTitolario.Value = queryString;
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
                ClientScript.RegisterStartupScript(GetType(), "DescrizioneTitObbligatoria", "alert('Inserire una descrizione per il titolario.');", true);
                return;
            }

            DocsPAWA.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();

            //Se il nodo selezionato è la radice allora si effettua un inserimento di nuovo titolario
            if (nodo == null)
            {
                DocsPAWA.DocsPaWR.OrgTitolario titolario = new DocsPAWA.DocsPaWR.OrgTitolario();
                titolario.Descrizione = txt_DescrizioneTit.Text;
                titolario.MaxLivTitolario = ddl_LivelliTit.SelectedValue;
                titolario.Commento = txt_CommentoTit.Text;
                titolario.Stato = DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione;
                titolario.Codice = "T";
                string codAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0");
                titolario.CodiceAmministrazione = codAmm;

                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();

                wws.SaveTitolario(sessionManager.getUserAmmSession(), ref titolario);

                //Ricaricamento dati dei titolari nel treeview
                FillTreeView(ddl_registri.SelectedValue.ToString());
                // Selezione nodo root
                PerformSelectionCurrentTitolario();
                impostaVisibilitaControlli(null);
                return;
            }

            //Se il nodo selezioanto è un titolario allora si vuole effettuare un aggiornamento
            if (nodo != null && nodo.Codice == "T")
            {
                DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(nodo.ID);
                titolario.Descrizione = txt_DescrizioneTit.Text;
                titolario.MaxLivTitolario = ddl_LivelliTit.SelectedValue;
                titolario.Commento = txt_CommentoTit.Text;

                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();

                wws.SaveTitolario(sessionManager.getUserAmmSession(), ref titolario);

                //Ricaricamento dati dei titolari nel treeview
                FillTreeView(ddl_registri.SelectedValue.ToString());
                // Selezione nodo root
                PerformSelectionCurrentTitolario();
                impostaVisibilitaControlli(null);
                return;
            }
        }

        protected void btn_ImportaTitolario_Click(object sender, EventArgs e)
        {
            DocsPAWA.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            DocsPAWA.DocsPaWR.OrgTitolario titolario = new DocsPAWA.DocsPaWR.OrgTitolario();
            titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            titolario.ID = nodo.ID;
            titolario.Stato = DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione;
            Session.Add("titolarioSelezionato", titolario);
            ClientScript.RegisterStartupScript(GetType(), "importaTitolario", "ApriImportaTitolario();", true);
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
                DocsPAWA.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();

                // Reperimento oggetto Titolario
                DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(nodo.ID);

                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                wws.Timeout = System.Threading.Timeout.Infinite;
                wws.attivaTitolario(sessionManager.getUserAmmSession(), titolario);

                //Ricaricamento dati dei titolari nel treeview
                FillTreeView(ddl_registri.SelectedValue.ToString());
                // Selezione nodo root
                PerformSelectionCurrentTitolario();
                impostaVisibilitaControlli(null);
            }
        }

        private void messageBox_EliminaTitolario_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                DocsPAWA.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();

                //Posso cancella un titolario solo se è in uno stato di "In Definizione"
                if (nodo.Codice == "T" && nodo.stato == "D")
                {
                    DocsPAWA.DocsPaWR.OrgTitolario titolario = new DocsPAWA.DocsPaWR.OrgTitolario();
                    titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
                    titolario.ID = nodo.ID;

                    DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                    wws.deleteTitolario(sessionManager.getUserAmmSession(), titolario);

                    //Ricaricamento dati dei titolari nel treeview
                    FillTreeView(ddl_registri.SelectedValue.ToString());
                    // Selezione nodo root
                    PerformSelectionCurrentTitolario();
                    impostaVisibilitaControlli(null);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "ImpossibileEliminareTitolario", "alert('Cacellazione impossibile.e\nIl titolario selezionato non è in definizione.');", true);
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
                ddl_classificazione.SelectedIndex = 0;
            }
        }

        private void messageBox_CopiaTitolario_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                DocsPAWA.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();

                // Reperimento nodo titolario
                DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(nodo.ID);
                titolario.Descrizione = nodo.Descrizione;

                DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                wws.Timeout = System.Threading.Timeout.Infinite;
                if (wws.copiaTitolario(sessionManager.getUserAmmSession(), titolario, ddl_registri.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(GetType(), "copiaAvvenuta", "alert('Copia avvenuta con successo.');", true);
                    //Ricaricamento dati dei titolari nel treeview
                    FillTreeView(ddl_registri.SelectedValue.ToString());
                    // Selezione nodo root
                    PerformSelectionCurrentTitolario();
                    impostaVisibilitaControlli(null);
                }
                else
                {
                    ClientScript.RegisterStartupScript(GetType(), "copiaErrata", "alert('Errore durante la copia del titolario.');", true);
                }
            }
        }

        protected void btn_EsportaTitolario_Click(object sender, EventArgs e)
        {
            DocsPAWA.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            DocsPAWA.DocsPaWR.OrgTitolario titolario = new DocsPAWA.DocsPaWR.OrgTitolario();
            titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            titolario.ID = nodo.ID;
            wws.Timeout = System.Threading.Timeout.Infinite;
            DocsPAWA.DocsPaWR.FileDocumento fileDoc = wws.ExportTitolarioInExcel(titolario, ddl_registri.SelectedValue);

            if (fileDoc != null)
            {
                DocsPAWA.exportDati.exportDatiSessionManager session = new DocsPAWA.exportDati.exportDatiSessionManager();
                session.SetSessionExportFile(fileDoc);
                ClientScript.RegisterStartupScript(GetType(), "openFile", "OpenFile();", true);
            }
        }

        protected void btn_EsportaIndice_Click(object sender, EventArgs e)
        {
            DocsPAWA.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            DocsPAWA.DocsPaWR.OrgTitolario titolario = new DocsPAWA.DocsPaWR.OrgTitolario();
            titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            titolario.ID = nodo.ID;
            DocsPAWA.DocsPaWR.FileDocumento fileDoc = wws.ExportIndiceSistematico(titolario);

            if (fileDoc != null)
            {
                DocsPAWA.exportDati.exportDatiSessionManager session = new DocsPAWA.exportDati.exportDatiSessionManager();
                session.SetSessionExportFile(fileDoc);
                ClientScript.RegisterStartupScript(GetType(), "openFile", "OpenFile();", true);
            }
        }

        protected void btn_ImportaIndice_Click(object sender, EventArgs e)
        {
            DocsPAWA.DocsPaWR.OrgNodoTitolario nodo = GetNodoSelezionato();
            DocsPAWA.DocsPaWR.OrgTitolario titolario = new DocsPAWA.DocsPaWR.OrgTitolario();
            titolario.CodiceAmministrazione = nodo.CodiceAmministrazione;
            titolario.ID = nodo.ID;
            titolario.Stato = DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione;
            Session.Add("titolarioSelezionato", titolario);
            ClientScript.RegisterStartupScript(GetType(), "importaIndice", "ApriImportaIndice();", true);
        }

        #endregion

        #region Pulsanti operazioni amministrazione
        protected void btn_gestioneEtichetteTit_Click(object sender, EventArgs e)
        {
            //DocsPAWA.DocsPaWR.OrgNodoTitolario nodoSelezionato = GetNodoSelezionato();
            //Session.Add("nodoSelPerIndice", nodoSelezionato);

            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            ArrayList titolari = new ArrayList(wws.getTitolari(idAmm));
            if (titolari != null && titolari.Count > 0)
            {
                ClientScript.RegisterStartupScript(GetType(), "popUpGestioneEtichette", "apriGestioneEtichetteTit('" + ((DocsPAWA.DocsPaWR.OrgTitolario)titolari[0]).ID + "');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "alertNessunTitolario", "alert('Nessun titolario per questa amministrazione.');", true);
            }
            }
        #endregion

        #region Strutture Sottofascicoli

        private void CaricaComboTemplateStruttura()
        {
            string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            DataTable data = new AmmUtils.WebServiceLink().GetTemplateStruttura(idAmm);
            DataRow row = data.NewRow();
            row[0] = "-1";
            row[1] = "";

            data.Rows.InsertAt(row, 0);
            DataStrutturaTemplate = data;

            ddStrutturaTemplate.DataTextField = "NAME";
            ddStrutturaTemplate.DataValueField = "SYSTEM_ID";
            ddStrutturaTemplate.DataSource = data;
            ddStrutturaTemplate.DataBind();
        }

        #endregion


        #region
        private void ddl_classificazione_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string valoreChiave;
            valoreChiave = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_BLOCCA_CLASS");
            if (valoreChiave.Equals("1"))
            {
                DocsPAWA.DocsPaWR.OrgNodoTitolario nodoTitolario = GetCurrentTitolario();
                if (nodoTitolario.CountChildNodiTitolario > 0)
                {
                    //if (ddl_classificazione.SelectedIndex == 0)
                    //{
                    //    ddl_rw.SelectedIndex = 0;
                    //}
                    if (ddl_classificazione.SelectedIndex == 1)
                    {
                        messageBox_ClassificazioneNodiFiglio.Confirm("Attenzione! E' abilitato il blocco della classificazione per i nodi padre. Procedendo con l'operazione il blocco verrà ignorato. Si desidera continuare?");
                        //RegisterClientScript("alertBloccaClass", "alert('Classificazione bloccata per i nodi padri');");
                        //ddl_classificazione.SelectedIndex = 0;
                        //return;
                    }
                }
            }
        }

        #endregion
    }
}
