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
using Microsoft.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.Gestione_Deleghe
{
    public partial class GestioneDeleghe : System.Web.UI.Page
    {
        private string idAmministrazione;
        private DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
        private string HttpFullPath;
        protected System.Web.UI.WebControls.Label lbl_stato;

        #region class myTreeNode
        /// <summary>
        /// Summary description for myTreeNode
        /// </summary>
        public class myTreeNode : Microsoft.Web.UI.WebControls.TreeNode
        {
            // Tipo Nodo [Possibili Valori: U=(Unità organizz.), R=(Ruolo), U=(Utente) ]
            public string getTipoNodo()
            {
                return ViewState["TipoNodo"].ToString();
            }
            public void setTipoNodo(string id)
            {
                ViewState["TipoNodo"] = id;
            }

            // IDCorrGlobale
            public string getIDCorrGlobale()
            {
                return ViewState["IDCorrGlobale"].ToString();
            }
            public void setIDCorrGlobale(string id)
            {
                ViewState["IDCorrGlobale"] = id;
            }

            // Codice
            public string getCodice()
            {
                return ViewState["Codice"].ToString();
            }
            public void setCodice(string id)
            {
                ViewState["Codice"] = id;
            }

            // CodiceRubrica
            public string getCodiceRubrica()
            {
                return ViewState["CodiceRubrica"].ToString();
            }
            public void setCodiceRubrica(string id)
            {
                ViewState["CodiceRubrica"] = id;
            }

            // Descrizione
            public string getDescrizione()
            {
                return ViewState["Descrizione"].ToString();
            }
            public void setDescrizione(string id)
            {
                ViewState["Descrizione"] = id;
            }

            // Livello
            public string getLivello()
            {
                return ViewState["Livello"].ToString();
            }
            public void setLivello(string id)
            {
                ViewState["Livello"] = id;
            }

            // Amministrazione
            public string getIDAmministrazione()
            {
                return ViewState["IDAmministrazione"].ToString();
            }
            public void setIDAmministrazione(string id)
            {
                ViewState["IDAmministrazione"] = id;
            }

            // AOO interoperabilità
            public string getCodRegInterop()
            {
                return ViewState["CodRegInterop"].ToString();
            }
            public void setCodRegInterop(string id)
            {
                ViewState["CodRegInterop"] = id;
            }

            // Tipo ruolo
            public string getIDTipoRuolo()
            {
                return ViewState["IDTipoRuolo"].ToString();
            }
            public void setIDTipoRuolo(string id)
            {
                ViewState["IDTipoRuolo"] = id;
            }

            // ID Groups
            public string getIDGruppo()
            {
                return ViewState["IDGruppo"].ToString();
            }
            public void setIDGruppo(string id)
            {
                ViewState["IDGruppo"] = id;
            }

            // ID People
            public string getIDPeople()
            {
                return ViewState["IDPeople"].ToString();
            }
            public void setIDPeople(string id)
            {
                ViewState["IDPeople"] = id;
            }

            // Ruolo Di riferimento
            public string getDiRiferimento()
            {
                return ViewState["DiRiferimento"].ToString();
            }
            public void setDiRiferimento(string id)
            {
                ViewState["DiRiferimento"] = id;
            }

            // Percorso
            public string getPercorso()
            {
                return ViewState["Percorso"].ToString();
            }
            public void setPercorso(string id)
            {
                ViewState["Percorso"] = id;
            }

            //Ruolo Responsabile
            public string getResponsabile()
            {
                return ViewState["Responsabile"].ToString();
            }
            public void setResponsabile(string id)
            {
                ViewState["Responsabile"] = id;
            }

            public string getRuoliUO()
            {
                return ViewState["RuoliUO"].ToString();
            }
            public void setRuoliUO(string id)
            {
                ViewState["RuoliUO"] = id;
            }

            public string getSottoUO()
            {
                return ViewState["SottoUO"].ToString();
            }
            public void setSottoUO(string id)
            {
                ViewState["SottoUO"] = id;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            setDefaultButton();

            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
            Session["AdminBookmark"] = "GestioneDeleghe";
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

            //btn_anteprima.Attributes.Add("onclick", "javascript: apriPopupAnteprima();");

            if (Session["AMMDATASET"] == null)
            {
                RegisterStartupScript("NoProfilazione", "<script>alert('Attenzione selezionare un\\'amministrazione !'); document.location = '../Gestione_Homepage/Home.aspx';</script>");
                return;
            }
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            idAmministrazione = wws.getIdAmmByCod(codiceAmministrazione);

            this.GetCalendarControl("txt_dta_scadenza").fromUrl = "../../UserControls/DialogCalendar.aspx";
            this.GetCalendarControl("txt_dta_decorrenza").fromUrl = "../../UserControls/DialogCalendar.aspx";

            //Immagini per il tree
            HttpFullPath = DocsPAWA.Utils.getHttpFullPath(this);
            this.treeViewUO.SystemImagesPath = HttpFullPath + "/AdminTool/Images/treeimages/";

            if (!Page.IsPostBack)
            {
                ddl_stato.SelectedIndex = 3;
                BindGridAndSelect(ddl_stato.SelectedValue);
                this.SetFocus(this.btn_nuova);
            }
            else
            {
                this.SetFocus(this.txt_ricDesc);
               
                // gestione del valore di ritorno della modal Dialog (ricerca)
                if (this.hd_returnValueModal.Value != null && this.hd_returnValueModal.Value != string.Empty && this.hd_returnValueModal.Value != "undefined")
                {
                    this.RicercaNodo(this.hd_returnValueModal.Value, "");
                    this.SetFocus(this.btn_conferma);
                }
            }

            if (pnl_nuovaDelega.Visible)
                abilita_pulsanti(false);
            else
                abilita_pulsanti(true);

            AdminTool.UserControl.ScrollKeeper skDgTemplate = new AdminTool.UserControl.ScrollKeeper();
            skDgTemplate.WebControl = "DivDGList";
            this.deleghe.Controls.Add(skDgTemplate);

           
        }

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
            this.ddl_stato.SelectedIndexChanged += new EventHandler(this.ddl_stato_SelectedIndexChanged);
            this.btn_nuova.Click += new EventHandler(btn_nuova_Click);
            this.btn_revoca.Click += new EventHandler(btn_revoca_Click);
            this.btn_modifica.Click += new EventHandler(btn_modifica_Click);
            this.msg_revoca.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_revoca_GetMessageBoxResponse);
            this.btn_conferma.Click += new EventHandler(btn_conferma_Click);
            this.btn_chiudiPnl.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudiPnl_Click);
            this.msg_chiudi.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_chiudi_GetMessageBoxResponse);
            this.msg_DelegaPermanente.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_DelegaPermanente_GetMessageBoxResponse);
            this.chklst_utente.SelectedIndexChanged += new System.EventHandler(this.chklst_utente_SelectedIndexChanged);
            this.chkTutti.CheckedChanged += new EventHandler(chkTutti_CheckedChanged);
            this.ddl_ricTipo.SelectedIndexChanged += new System.EventHandler(this.ddl_ricTipo_SelectedIndexChanged);
            this.Load += new System.EventHandler(this.Page_Load);
            this.treeViewUO.Expand += new Microsoft.Web.UI.WebControls.ClickEventHandler(this.treeViewUO_Expand);
            this.treeViewUO.SelectedIndexChange += new Microsoft.Web.UI.WebControls.SelectEventHandler(this.treeViewUO_SelectedIndexChange);
            this.treeViewUO.Collapse += new Microsoft.Web.UI.WebControls.ClickEventHandler(this.treeViewUO_Collapse);
            this.msg_conferma.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_conferma_GetMessageBoxResponse);

        }
        #endregion


        #region pulsanti
        private void visualizza_pulsanti()
        {
            btn_nuova.Visible = true;
            btn_nuova.Enabled = true;
            btn_revoca.Visible = true;
            btn_revoca.Enabled = false;
            btn_modifica.Visible = true;
            btn_modifica.Enabled = false;

            if (this.dgDeleghe.Items.Count == 1)
            {
                CheckBox chkBox = dgDeleghe.Items[0].Cells[0].FindControl("cbSel") as CheckBox;
                chkBox.Enabled = true;
                chkBox.Checked = true;
                btn_revoca.Enabled = true;
                btn_modifica.Enabled = true;
            }
            else
            {
                btn_revoca.Enabled = false;
                btn_modifica.Enabled = false;
            }

            //Deleghe assegnate scadute
            //if (ddl_stato.SelectedValue.Equals("S"))
            //{
            //    this.dgDeleghe.Columns[0].Visible = false;
            //    btn_revoca.Enabled = false;
            //    btn_modifica.Enabled = false;
            //}
        }

        private void abilita_pulsanti(bool valore)
        {
            if (valore)
            {
                visualizza_pulsanti();
            }
            else
            {
                btn_nuova.Enabled = false;
                btn_revoca.Enabled = false;
                btn_modifica.Enabled = false;

            }
        }

        private void btn_nuova_Click(object sender, EventArgs e)
        {
            //inizializza la grafica per il pannello nuova delega
            abilita_pulsanti(false);
            ddl_ricTipo.SelectedIndex = 0;
            ddl_ricTipo_SelectedIndexChanged(null, null);
            this.InizializeTree();
            lbl_messaggio.Visible = false;
            lbl_operazione.Text = "Nuova delega";
            this.pnl_nuovaDelega.Visible = true;
            this.chkTutti.Checked = false;
            this.lbl_ruolo_delegante.Visible = true;
            this.chklst_ruoli.Visible = true;
            this.chklst_ruoli.Items.Clear();
            //this.txt_ruolo_delegante.Visible = true;
            //this.txt_ruolo_delegante.Text = "";
            this.txt_delegante.Text = "";
            this.txt_delegato.Text = "";
            this.ddl_ricTipo.SelectedIndex = 0;
            this.txt_ricCod.Text = "";
            this.txt_ricDesc.Text = "";
            btn_conferma.ToolTip = "Conferma creazione nuova delega";
            //setInfoRuolo();
            chklst_utente.SelectedIndex = 0;
            this.treeViewUO.AutoPostBack = true;
            this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text = DateTime.Now.Date.ToShortDateString().ToString();
            this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text = "";
            ddl_ora_decorrenza.SelectedIndex = DateTime.Now.Hour;
            ddl_ora_scadenza.SelectedIndex = 0;
            DocsPAWA.DocsPaWR.InfoDelega delega = new DocsPAWA.DocsPaWR.InfoDelega();
            Session.Add("delega", delega);
            
        }

        private void btn_revoca_Click(object sender, EventArgs e)
        {
            //messaggio di conferma a seguito del quale il sistema revoca immediatamente le deleghe selezionate
            string messaggio = InitMessageXml.getInstance().getMessage("DELEGA_REVOCA");
            msg_revoca.Confirm(messaggio);
        }

        private void btn_modifica_Click(object sender, EventArgs e)
        {
            this.SetFocus(this.btn_conferma);
            int posizione = verificaSelezioneDeleghe();
            if (posizione >= 0)
            {
                //imposto interfaccia grafica
                abilita_pulsanti(false);
                lbl_messaggio.Visible = false;
                lbl_operazione.Text = "Modifica delega";
                btn_conferma.ToolTip = "Conferma modifica delega";
                this.pnl_nuovaDelega.Visible = true;
                this.ddl_ricTipo.SelectedIndex = 0;
                this.txt_ricCod.Text = "";
                this.txt_ricDesc.Text = "";
                InizializeTree();

                //recupero le informazioni sulla delega che si vuole modificare

                DocsPAWA.DocsPaWR.InfoDelega delega = (DocsPAWA.DocsPaWR.InfoDelega)this.ListaDeleghe[posizione];
                Session.Add("delega", delega);
                this.txt_delegante.Text = delega.cod_utente_delegante;
                setInfoRuolo(delega.id_utente_delegante, delega.cod_ruolo_delegante);
                //this.txt_ruolo_delegante.Text = delega.cod_ruolo_delegante;
                if (delega.cod_ruolo_delegante.Equals("TUTTI"))
                {
                    this.chkTutti.Checked = true;
                    this.lbl_ruolo_delegante.Visible = false;
                    this.chklst_ruoli.Visible = false;
                }
                else
                {
                    this.chkTutti.Checked = false;
                    this.lbl_ruolo_delegante.Visible = true;
                    this.chklst_ruoli.Visible = true;
                }
                this.txt_delegato.Text = delega.cod_utente_delegato;

                //impostazione data decorrenza 
                this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text = (Convert.ToDateTime(delega.dataDecorrenza).ToShortDateString()).ToString();
                string oraDecorrenza = Convert.ToDateTime(delega.dataDecorrenza).ToShortTimeString();
                if (!string.IsNullOrEmpty(oraDecorrenza))
                {
                    oraDecorrenza = oraDecorrenza.Substring(0, 2);
                    if (oraDecorrenza.EndsWith("."))
                        oraDecorrenza = "0" + oraDecorrenza.Substring(0, 1);
                    ddl_ora_decorrenza.SelectedIndex = Convert.ToInt32(oraDecorrenza);
                }
                else
                    ddl_ora_decorrenza.SelectedIndex = 0;

                //impostazione data scadenza
                if (string.IsNullOrEmpty(delega.dataScadenza))
                {
                    this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text = string.Empty;
                    ddl_ora_scadenza.SelectedIndex = 0;
                }
                else
                {
                    this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text = (Convert.ToDateTime(delega.dataScadenza).ToShortDateString()).ToString();
                    string oraScadenza = "";
                    if (!string.IsNullOrEmpty(delega.dataScadenza))
                    {
                        oraScadenza = Convert.ToDateTime(delega.dataScadenza).ToShortTimeString();
                    }
                    if (!string.IsNullOrEmpty(oraScadenza))
                    {
                        oraScadenza = oraScadenza.Substring(0, 2);
                        if (oraScadenza.EndsWith("."))
                            oraScadenza = "0" + oraScadenza.Substring(0, 1);
                        ddl_ora_scadenza.SelectedIndex = Convert.ToInt32(oraScadenza) + 1;
                    }
                    else
                        ddl_ora_scadenza.SelectedIndex = 0;
                }
                //delega attiva (non è possibile modificare la data di decorrenza di una delega attiva)
                if (Convert.ToDateTime(delega.dataDecorrenza) < DateTime.Now && (string.IsNullOrEmpty(delega.dataScadenza) || Convert.ToDateTime(delega.dataScadenza) > DateTime.Now))
                {
                    txt_dta_decorrenza.EnableBtnCal = false;
                }

            }
            else
            {
                Response.Write("<script>alert('Nessuna delega selezionata!');</script>");
                return;
            }
        }

        private void ddl_stato_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                this.pnl_nuovaDelega.Visible = false;
                BindGridAndSelect(ddl_stato.SelectedValue);
                visualizza_pulsanti();
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        private void Revoca()
        {
            //Crea la lista delle deleghe da revocare, dopo averle revocate si ricarica
            //l'elenco delle deleghe assegnate
            ArrayList listaDeleghe = new ArrayList();
            for (int i = 0; i < dgDeleghe.Items.Count; i++)
            {
                CheckBox chkSelection = dgDeleghe.Items[i].Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                {
                    DocsPAWA.DocsPaWR.InfoDelega delega = (DocsPAWA.DocsPaWR.InfoDelega)this.ListaDeleghe[i];
                    listaDeleghe.Add(delega);
                }
            }
            string msg = string.Empty;
            if (DelegheManager.RevocaDelegaAmm(this, (DocsPAWA.DocsPaWR.InfoDelega[])listaDeleghe.ToArray(typeof(DocsPAWA.DocsPaWR.InfoDelega)), out msg))
                BindGridAndSelect(ddl_stato.SelectedValue);
            else
            {
                string messaggio = string.Empty;
                if (string.IsNullOrEmpty(msg))
                    messaggio = "Impossibile revocare le deleghe selezionate";
                else
                    messaggio = msg;
                disabilitaCheckDataGrid();
                Response.Write("<script>alert('" + messaggio + "');</script>");
                return;
            }

        }
        #endregion


        #region DATAGRID
        protected void BindGridAndSelect(string statoDelega)
        {
            DataGridItem item;
            int numDeleghe = 0;
            string tipo = "tutte";
            //Recupera l'elenco delle deleghe
            this.ListaDeleghe = DelegheManager.GetListaDeleghe(this, tipo, ddl_stato.SelectedValue, idAmministrazione, out numDeleghe);
            DocsPaWR.InfoDelega[] data = this.ListaDeleghe;
            this.dgDeleghe.DataSource = data;
            this.dgDeleghe.DataBind();
            if (numDeleghe > 0)
            {
                this.dgDeleghe.Visible = true;

                this.lbl_messaggio.Visible = false;
            }
            else
            {
                this.dgDeleghe.Visible = false;
                this.lbl_messaggio.Visible = true;
                this.lbl_messaggio.Text = "Nessuna delega presente";
            }
            for (int i = 0; i < numDeleghe; i++)
            {
                this.dgDeleghe.Items[i].Cells[4].Text = data[i].cod_utente_delegato + "<br>(" + data[i].cod_ruolo_delegato + ")";
                this.dgDeleghe.Items[i].Cells[8].Text = data[i].cod_utente_delegante + "<br>(" + data[i].cod_ruolo_delegante + ")";

                //data decorrenza
                this.dgDeleghe.Items[i].Cells[1].Text = Convert.ToDateTime(data[i].dataDecorrenza).ToShortDateString()
                                                        + "<br>" + Convert.ToDateTime(data[i].dataDecorrenza).ToShortTimeString();
                //data scadenza 
                if (!string.IsNullOrEmpty(data[i].dataScadenza))
                {
                    if (Convert.ToDateTime(data[i].dataScadenza) > DateTime.Now)
                        this.dgDeleghe.Items[i].Font.Bold = true;

                    this.dgDeleghe.Items[i].Cells[2].Text = Convert.ToDateTime(data[i].dataScadenza).ToShortDateString()
                                                            + "<br>" + Convert.ToDateTime(data[i].dataScadenza).ToShortTimeString();
                }
                else
                    this.dgDeleghe.Items[i].Font.Bold = true;

                if (data[i].utDeleganteDismesso.Equals("1") || data[i].utDelegatoDismesso.Equals("1"))
                {
                    this.dgDeleghe.Items[i].Font.Strikeout = true;
                    if (data[i].utDeleganteDismesso.Equals("1"))
                        this.dgDeleghe.Items[i].ToolTip = "Utente delegante dismesso";
                    else
                        this.dgDeleghe.Items[i].ToolTip = "Utente delegato dismesso";
                }

                statoDelega = "I";
                if (Convert.ToDateTime(data[i].dataDecorrenza) < DateTime.Now && (string.IsNullOrEmpty(data[i].dataScadenza) || Convert.ToDateTime(data[i].dataScadenza) > DateTime.Now))
                {
                    statoDelega = "A";
                }
                if (!string.IsNullOrEmpty(data[i].dataScadenza) && Convert.ToDateTime(data[i].dataScadenza) < DateTime.Now)
                {
                    statoDelega = "S";
                }
                this.dgDeleghe.Items[i].Cells[15].Text = statoDelega;
            }
        }

        protected DocsPaWR.InfoDelega[] ListaDeleghe
        {
            get
            {
                if (this.ViewState["ListaDeleghe"] != null)
                    return (DocsPaWR.InfoDelega[])this.ViewState["ListaDeleghe"];
                else
                    return new DocsPaWR.InfoDelega[0];
            }
            set
            {
                this.ViewState["ListaDeleghe"] = value;
            }
        }

        //Controllo se sono state selezionate alcune deleghe dall'elenco
        //Nel caso in cui sia stata selezionata solo una delega memorizzo la sua posizione
        //nell'elenco
        private int verificaSelezioneDeleghe()
        {
            int posizione = -1;
            int numDelegheSel = 0;

            for (int i = 0; i < dgDeleghe.Items.Count; i++)
            {
                DataGridItem item = dgDeleghe.Items[i];
                CheckBox chkSelection = item.Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                {
                    posizione = i;
                    numDelegheSel++;
                }
            }
            if (numDelegheSel != 1)
                posizione = -1;
            else
                ViewState.Add("posizione", posizione);
            return posizione;
        }

        protected void cbSel_CheckedChanged(object sender, EventArgs e)
        {
            abilita_pulsanti(true);
            pnl_nuovaDelega.Visible = false;
            //Se è stata selezionata almeno una delega non ancora scaduta sono attivi 
            //i pulsanti esercita e revoca, il pulsante modifica è invece attivo solo se si seleziona 
            //una e una sola delega attiva
            btn_modifica.Enabled = false;
            btn_revoca.Enabled = false;
            int elemSelModificabili = 0;
            int elemSelezionati = 0;
            for (int i = 0; i < dgDeleghe.Items.Count; i++)
            {
                DataGridItem item = dgDeleghe.Items[i];
                CheckBox chkSelection = item.Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                {
                    elemSelezionati++;
                    if (string.IsNullOrEmpty(ListaDeleghe[i].dataScadenza))
                        elemSelModificabili++;
                    else
                        if ((Convert.ToDateTime(ListaDeleghe[i].dataScadenza)).CompareTo(DateTime.Now) > 0)
                            elemSelModificabili++;
                }
            }
            if (elemSelModificabili > 0)
            {
                btn_revoca.Enabled = true;
            }
            if (elemSelezionati == 1)
            {
                btn_modifica.Enabled = true;
            }
        }

        private void disabilitaCheckDataGrid()
        {
            foreach (DataGridItem item in this.dgDeleghe.Items)
            {
                CheckBox chkSelection = item.Cells[0].FindControl("cbSel") as CheckBox;
                if (chkSelection != null && chkSelection.Checked)
                {
                    chkSelection.Checked = false;
                }
            }
        }

        private void setInfoRuolo(string idPeople, string descRuolo)
        {
            int pos = 0;
            int i = 0;
            ListItem item;
            chklst_ruoli.Items.Clear();
            DocsPAWA.DocsPaWR.Ruolo[] ruoli = UserManager.GetRuoliUtente(this, idPeople);
            if (ruoli != null)
            {
                if (ruoli.Length > 1)
                {
                    //Inserimento voce "Tutti"
                    item = new ListItem("TUTTI");
                    this.chklst_ruoli.Items.Add(item);
                    i++;
                }

                foreach (DocsPAWA.DocsPaWR.Ruolo ruolo in ruoli)
                {
                    item = new ListItem(ruolo.descrizione.ToString(), ruolo.systemId.ToString() + "_" + ruolo.uo.systemId);
                    this.chklst_ruoli.Items.Add(item);
                    if (ruolo.descrizione == descRuolo)
                        pos = i;
                    i++;
                }
                if (this.chkTutti.Checked || descRuolo == "TUTTI")
                    chklst_ruoli.SelectedIndex = 0;
                else
                    chklst_ruoli.SelectedIndex = pos;
            }
        }
        #endregion

        #region MessageBox
        private void msg_revoca_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                Revoca();
            }
        }

        private void msg_DelegaPermanente_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                DocsPAWA.DocsPaWR.InfoDelega delega = (DocsPAWA.DocsPaWR.InfoDelega)(Session["delega"]);

                if (DelegheManager.CreaNuovaDelegaAmm(this, delega))
                {
                    pnl_nuovaDelega.Visible = false;
                    BindGridAndSelect(ddl_stato.SelectedValue);
                    abilita_pulsanti(true);
                }
            }
        }

        private void msg_conferma_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                Modifica();
            }
        }

        private void msg_chiudi_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Cancel)
            {
                disabilitaCheckDataGrid();
                pnl_nuovaDelega.Visible = false;
                visualizza_pulsanti();
                SetFocus(btn_nuova);
                if (dgDeleghe.Items.Count == 0)
                    lbl_messaggio.Visible = true;
            }
        }
        #endregion

        #region NUOVA E MODIFICA DELEGA

        private void chklst_utente_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ddl_ricTipo.SelectedIndex = 0;
            ddl_ricTipo_SelectedIndexChanged(null, null);
            //this.RicercaNodo(chklst_ruoli.SelectedValue, "");
        }

        private void chkTutti_CheckedChanged(object sender, System.EventArgs e)
        {
            if (chkTutti.Checked)
            {
                lbl_ruolo_delegante.Visible = false;
                chklst_ruoli.Visible = false;
            }
            else
            {
                lbl_ruolo_delegante.Visible = true;
                chklst_ruoli.Visible = true;
            }
        }

        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            DocsPAWA.UserControls.Calendar calendar = (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
            calendar.fromUrl = "../../UserControls/DialogCalendar.aspx";
            return calendar;
        }

        private void btn_conferma_Click(object sender, EventArgs e)
        {
            lbl_avviso.Text = "";
            if (lbl_operazione.Text == "Nuova delega")
            {
                confermaNuova();
            }
            if (lbl_operazione.Text == "Modifica delega")
            {
                confermaModifica();
            }

        }

        private void confermaNuova()
        {
            try
            {
                string dta_scadenza = this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text;

                if (!string.IsNullOrEmpty(dta_scadenza) && dta_scadenza.Length <= 10)
                {
                    if (!string.IsNullOrEmpty(ddl_ora_scadenza.SelectedValue))
                    {
                        string ora = ddl_ora_scadenza.SelectedValue;
                        if (ddl_ora_scadenza.SelectedValue.Length == 1)
                            ora = "0" + ddl_ora_scadenza.SelectedValue;
                        dta_scadenza = dta_scadenza + " " + ora + ".00.00";
                    }
                    else
                        dta_scadenza = dta_scadenza + " " + System.DateTime.Now.Hour + ".00.00";

                }

                //Verifica Date
                if (string.IsNullOrEmpty(dta_scadenza))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare formato data decorrenza!');</script>");
                        return;
                    }
                }
                else
                    if (!Utils.isDate(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text)
                        || !Utils.isDate(dta_scadenza)
                        || !Utils.verificaIntervalloDate(dta_scadenza, this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date!');</script>");
                        return;
                    }
                string dataDecorrenza;
                if (!string.IsNullOrEmpty(ddl_ora_decorrenza.SelectedValue))
                {
                    string ora = ddl_ora_decorrenza.SelectedValue;
                    if (ddl_ora_decorrenza.SelectedValue.Length == 1)
                        ora = "0" + ddl_ora_decorrenza.SelectedValue;
                    string minuti = (DateTime.Now.Minute + 1).ToString();
                    if (minuti.Length == 1)
                        minuti = "0" + minuti;

                    //string secondi = (DateTime.Now.Second).ToString();
                    //if (secondi.Length == 1)
                    //    secondi = "0" + minuti;

                    string[] temp = this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text.Split(' ');
                    //dataDecorrenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + "." + minuti + "." + DateTime.Now.Second.ToString();
                    dataDecorrenza = temp[0] + " " + ora + ".00.00";
                }
                else
                    dataDecorrenza = this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text;


                if (!Utils.verificaIntervalloDate(dataDecorrenza, System.DateTime.Now.ToShortDateString()))
                {
                    Response.Write("<script>alert('La data di decorrenza non deve essere inferiore alla data corrente!');</script>");
                    return;
                }
                if (string.IsNullOrEmpty(txt_delegante.Text))
                {
                    Response.Write("<script>alert('Selezionare il delegante!');</script>");
                    return;
                }
                if (string.IsNullOrEmpty(txt_delegato.Text))
                {
                    Response.Write("<script>alert('Selezionare il delegato!');</script>");
                    return;
                }
                if (txt_delegato.Text.Equals(txt_delegante.Text))
                {
                    Response.Write("<script>alert('Attenzione il delegante e il delegato non possono essere la stessa persona!');</script>");
                    return;
                }
                CreaDelega();
            }
            catch (System.Exception ex)
            {
                //Gestione errore
            }
        }

        //Creazione di una nuova delega
        private void CreaDelega()
        {
            DocsPAWA.DocsPaWR.InfoDelega delega = (DocsPAWA.DocsPaWR.InfoDelega)(Session["delega"]);
            if (this.chkTutti.Checked)
            {
                delega.id_ruolo_delegante = "0";
                delega.cod_ruolo_delegante = "TUTTI";
            }
            string ora;
            // Gabriele Melini 28-04-2014
            // bug salvataggio ora delega
            //if (ddl_ora_decorrenza.SelectedIndex == 0)
            //    ora = System.DateTime.Now.Hour.ToString();
            //else
            //    ora = ddl_ora_decorrenza.SelectedValue;
            ora = ddl_ora_decorrenza.SelectedValue;
            if (ora.Length == 1)
                ora = "0" + ddl_ora_decorrenza.SelectedValue;

            if (Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString().Equals(DateTime.Now.ToShortDateString())
                             && ora.Equals(DateTime.Now.Hour.ToString()))
                delega.dataDecorrenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + "." + (DateTime.Now.Minute).ToString() + ".00";
            else
                delega.dataDecorrenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + ".00.00";

            
            if (ddl_ora_scadenza.SelectedIndex == 0)
                ora = System.DateTime.Now.Hour.ToString();
            else
                ora = ddl_ora_scadenza.SelectedValue;
            if (ora.Length == 1)
                ora = "0" + ddl_ora_scadenza.SelectedValue;

            if (!string.IsNullOrEmpty(this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text))
                delega.dataScadenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + ".00.00";
            else
                delega.dataScadenza = string.Empty;
            //Verifica che non sia stata già creata una delega nello stesso periodo (univocità dell'assegnazione di responsabilità)
            if (DelegheManager.VerificaUnicaDelegaAmministrazione(this, delega))
            {
                if (string.IsNullOrEmpty(this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text))
                {
                    Session["delega"] = delega;
                    string messaggio = InitMessageXml.getInstance().getMessage("DELEGA_PERMANENTE");
                    msg_DelegaPermanente.Confirm(messaggio);
                }
                else
                {
                    if (DelegheManager.CreaNuovaDelegaAmm(this, delega))
                    {
                        pnl_nuovaDelega.Visible = false;
                        BindGridAndSelect(ddl_stato.SelectedValue);
                        abilita_pulsanti(true);
                    }
                    else
                    {
                        Response.Write("<script>alert('Attenzione, impossibile creare la delega. Riprovare più tardi.');</script>");
                        return;
                    }
                }
            }
            else
            {
                Response.Write("<script>alert('Attenzione, impossibile assegnare più deleghe in periodi temporali sovrapposti per lo stesso ruolo!');</script>");
                return;
            }
        }

        private void confermaModifica()
        {
            try
            {
                string dta_scadenza = this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text;

                if (!string.IsNullOrEmpty(dta_scadenza) && dta_scadenza.Length <= 10)
                {
                    if (!string.IsNullOrEmpty(ddl_ora_scadenza.SelectedValue))
                    {
                        string ora = ddl_ora_scadenza.SelectedValue;
                        if (ddl_ora_scadenza.SelectedValue.Length == 1)
                            ora = "0" + ddl_ora_scadenza.SelectedValue;
                        dta_scadenza = dta_scadenza + " " + ora + ".00.00";
                    }
                    else
                        dta_scadenza = dta_scadenza + " " + System.DateTime.Now.Hour + ".00.00";

                }

                //Verifica Date
                if (string.IsNullOrEmpty(dta_scadenza))
                {
                    if (!Utils.isDate(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare formato data decorrenza!');</script>");
                        return;
                    }
                }
                else
                    if (!Utils.isDate(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text)
                        || !Utils.isDate(dta_scadenza)
                        || !Utils.verificaIntervalloDate(dta_scadenza, this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date!');</script>");
                        return;
                    }

                if (string.IsNullOrEmpty(txt_delegante.Text))
                {
                    Response.Write("<script>alert('Selezionare il delegante!');</script>");
                    return;
                }
                if (string.IsNullOrEmpty(txt_delegato.Text))
                {
                    Response.Write("<script>alert('Selezionare il delegato!');</script>");
                    return;
                }
                if (txt_delegato.Text.Equals(txt_delegante.Text))
                {
                    Response.Write("<script>alert('Attenzione il delegante e il delegato non possono essere la stessa persona!');</script>");
                    return;
                }

                if (string.IsNullOrEmpty(txt_dta_scadenza.Text))
                {
                    string messaggio = InitMessageXml.getInstance().getMessage("DELEGA_PERMANENTE");
                    msg_conferma.Confirm(messaggio);
                }
                else
                    Modifica();

            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        private void Modifica()
        {
            string idUtenteOld = "";
            string idRuoloOld = "";
            string tipoDelega = "";
            string dataScadenzaOld = "";
            string dataDecorrenzaOld = "";

            //recupero le informazioni sulla delega che si vuole modificare
            int posizione = verificaSelezioneDeleghe();
            DocsPAWA.DocsPaWR.InfoDelega delegaOld = (DocsPAWA.DocsPaWR.InfoDelega)this.ListaDeleghe[posizione];
            DocsPAWA.DocsPaWR.InfoDelega delegaNew = (DocsPAWA.DocsPaWR.InfoDelega)Session["delega"];
            if (delegaNew == null)
                delegaNew = delegaOld;
            //Delega attiva o impostata o scaduta?
            tipoDelega = "I";
            if (Convert.ToDateTime(delegaOld.dataDecorrenza) < DateTime.Now && (string.IsNullOrEmpty(delegaOld.dataScadenza) || Convert.ToDateTime(delegaOld.dataScadenza) > DateTime.Now))
            {
                tipoDelega = "A";
            }
            if (!string.IsNullOrEmpty(delegaOld.dataScadenza) && Convert.ToDateTime(delegaOld.dataScadenza) < DateTime.Now)
            {
                tipoDelega = "S";
            }
            dataScadenzaOld = delegaOld.dataScadenza;
            dataDecorrenzaOld = delegaOld.dataDecorrenza;

            string ora;
            // Gabriele Melini 28-04-2014
            // bug salvataggio ora delega
            //if (ddl_ora_decorrenza.SelectedIndex == 0)
            //    ora = System.DateTime.Now.Hour.ToString();
            //else
            //    ora = ddl_ora_decorrenza.SelectedValue;
            ora = ddl_ora_decorrenza.SelectedValue;
            if (ora.Length == 1)
                ora = "0" + ddl_ora_decorrenza.SelectedValue;
            //delegaNew.dataDecorrenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + "." + (DateTime.Now.Minute).ToString() + "." + DateTime.Now.Second.ToString();

            delegaNew.dataDecorrenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + ".00.00";        
            
            if (ddl_ora_scadenza.SelectedIndex == 0)
                ora = System.DateTime.Now.Hour.ToString();
            else
                ora = ddl_ora_scadenza.SelectedValue;
            if (ora.Length == 1)
                ora = "0" + ddl_ora_scadenza.SelectedValue;
            //delegaNew.dataScadenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + ".00.00";
            if (!string.IsNullOrEmpty(this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text))
                delegaNew.dataScadenza = Convert.ToDateTime(this.GetCalendarControl("txt_dta_scadenza").txt_Data.Text).Date.ToShortDateString() + " " + ora + ".00.00";
            else
                delegaNew.dataScadenza = string.Empty;
            if (this.chkTutti.Checked)
            {
                delegaNew.id_ruolo_delegante = "0";
                delegaNew.cod_ruolo_delegante = "TUTTI";
            }

            if (DelegheManager.ModificaDelegaAmm(this, delegaOld, delegaNew, tipoDelega, dataScadenzaOld, dataDecorrenzaOld))
            {
                //chiudo il pannello e carico il datagrid delle deleghe assegnate
                pnl_nuovaDelega.Visible = false;
                //NUOVO STATO DELEGHE
                BindGridAndSelect(ddl_stato.SelectedValue);
                btn_nuova.Enabled = true;

            }
            else
            {
                Response.Write("<script>alert('Attenzione, impossibile modificare la delega selezionata!');</script>");
                return;
            }

        }

        private void btn_chiudiPnl_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            lbl_avviso.Text = "";
            myTreeNode TreeNodo;
            TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);
            if (TreeNodo.getTipoNodo() == "P" && Utils.isDate(this.GetCalendarControl("txt_dta_decorrenza").txt_Data.Text))
            {
                string messaggio = InitMessageXml.getInstance().getMessage("DELEGA_CHIUDINUOVA");
                msg_chiudi.Confirm(messaggio);
            }
            else
            {
                disabilitaCheckDataGrid();
                pnl_nuovaDelega.Visible = false;
                SetFocus(btn_nuova);
                //abilita_pulsanti(true);
                visualizza_pulsanti();
                if (dgDeleghe.Items.Count == 0)
                    lbl_messaggio.Visible = true;
            }


        }

        #endregion

        #region Organigramma

        private void InizializeTree()
        {
            try
            {
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                if (!string.IsNullOrEmpty(idAmministrazione))
                {
                    theManager.ListaUOLivelloZero(idAmministrazione);
                    if (theManager.getListaUO() != null && theManager.getListaUO().Count > 0)
                    {
                        this.btn_find.Attributes.Add("onclick", "ApriRisultRic('" + idAmministrazione + "');");
                        this.LoadTreeviewLivelloZero(theManager.getListaUO());
                        
                    }
                }
                else
                {
                    this.lbl_avviso.Text = "Attenzione! l'amministrazione corrente non risulta essere presente nel database.<br><br>Effettuare il Chiudi e trasmetti.";
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        private void LoadTreeviewLivelloZero(ArrayList listaUO)
        {
            try
            {
                treeViewUO.Nodes.Clear();
                Microsoft.Web.UI.WebControls.TreeNode treenode = new Microsoft.Web.UI.WebControls.TreeNode();
                treenode.Text = "Organigramma";
                treeViewUO.Nodes.Add(treenode);

                Microsoft.Web.UI.WebControls.TreeNode tNode = new Microsoft.Web.UI.WebControls.TreeNode();
                tNode = treeViewUO.Nodes[0];

                myTreeNode nodoT;
                myTreeNode nodoFiglio;

                foreach (DocsPAWA.DocsPaWR.OrgUO uo in listaUO)
                {
                    nodoT = new myTreeNode();

                    nodoT.ID = uo.IDCorrGlobale;
                    nodoT.Text = uo.CodiceRubrica + " - " + uo.Descrizione;

                    nodoT.ImageUrl = HttpFullPath + "/AdminTool/Images/uo.gif";

                    tNode.Nodes.Add(nodoT);

                    nodoT.setTipoNodo("U");
                    nodoT.setIDCorrGlobale(uo.IDCorrGlobale);
                    nodoT.setCodice(uo.Codice);
                    nodoT.setCodiceRubrica(uo.CodiceRubrica);
                    nodoT.setDescrizione(uo.Descrizione);
                    nodoT.setLivello(uo.Livello);
                    nodoT.setIDAmministrazione(uo.IDAmministrazione);
                    nodoT.setCodRegInterop(uo.CodiceRegistroInterop);
                    nodoT.setPercorso(uo.Descrizione + " &gt; ");

                    if ((!uo.Ruoli.Equals("0")) || (!uo.SottoUo.Equals("0")))
                    {
                        nodoFiglio = new myTreeNode();
                        nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                        nodoT.Nodes.Add(nodoFiglio);
                    }
                    else
                    {
                        nodoT.Text = uo.CodiceRubrica + " - " + uo.Descrizione;
                    }
                }
                tNode.Expanded = true;
                this.SelezionaPrimo();
                this.LoadTreeViewLivelloFigli("0.0", "U");
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        private void RicercaNodo(string returnValue, string tipo)
        {
            try
            {
                //this.hd_returnValueModal.Value = "";
                DocsPAWA.DocsPaWR.InfoDelega delega = (DocsPAWA.DocsPaWR.InfoDelega)(Session["delega"]);

                string[] appo = returnValue.Split('_');

                string idCorrGlobale = appo[0];
                string idParent = appo[1];

                if (string.IsNullOrEmpty(tipo))
                    tipo = this.ddl_ricTipo.SelectedValue;

                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.ListaUOLivelloZero(idAmministrazione);
                this.LoadTreeviewLivelloZero(theManager.getListaUO());
                theManager.ListaIDParentRicerca(idParent, tipo);
                if (theManager.getListaIDParentRicerca() != null && theManager.getListaIDParentRicerca().Count > 0)
                {
                    ArrayList lista = new ArrayList();
                    theManager.getListaIDParentRicerca().Reverse();
                    lista = theManager.getListaIDParentRicerca();

                    lista.Add(Convert.ToInt32(idCorrGlobale));

                    for (int n = 1; n <= lista.Count - 1; n++)
                    {
                        myTreeNode TreeNodo;
                        TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(treeViewUO.SelectedNodeIndex);

                        foreach (myTreeNode nodo in TreeNodo.Nodes)
                        {
                            if (nodo.ID.Equals(lista[n].ToString()) && nodo.ID != idCorrGlobale)
                            {
                                if (nodo.getTipoNodo().Equals("U"))
                                {
                                    this.LoadTreeViewLivelloFigli(nodo.GetNodeIndex(), nodo.getTipoNodo());
                                }
                                else
                                {
                                    nodo.Expanded = true;
                                }
                                treeViewUO.SelectedNodeIndex = nodo.GetNodeIndex();
                                break;
                            }
                            if (nodo.ID.Equals(lista[n].ToString()) && nodo.ID.Equals(idCorrGlobale))
                            {
                                treeViewUO.SelectedNodeIndex = nodo.GetNodeIndex();
                                if (nodo.getIDCorrGlobale() == idCorrGlobale)
                                {
                                    string nomeUtente = nodo.Text.Substring(nodo.getCodiceRubrica().Length + 3, nodo.Text.Length - (nodo.getCodiceRubrica().Length + 3));

                                    //delegante
                                    if (((myTreeNode)nodo).getTipoNodo().ToString() == "P")
                                    {
                                        if (chklst_utente.SelectedIndex == 0)
                                        {

                                            txt_delegante.Text = nomeUtente;
                                            delega.id_utente_delegante = nodo.getIDPeople();
                                            delega.cod_utente_delegante = nodo.getCodiceRubrica();
                                            setInfoRuolo(delega.id_utente_delegante, ((myTreeNode)nodo.Parent).getDescrizione());
                                            //txt_ruolo_delegante.Text = ((myTreeNode)nodo.Parent).getDescrizione();

                                            if (this.chkTutti.Checked)
                                            {
                                                delega.id_ruolo_delegante = "0";
                                                delega.cod_ruolo_delegante = "TUTTI";
                                            }
                                            else
                                            {
                                                delega.id_ruolo_delegante = nodo.Parent.ToString();
                                                string ruoloDelegante = ((myTreeNode)((TreeBase)(nodo.Parent))).Text;
                                                string[] codRuoloDelegante = ruoloDelegante.Split('-');
                                                delega.cod_ruolo_delegante = codRuoloDelegante[1].Substring(1, codRuoloDelegante[1].Length - 1);
                                            }
                                        }

                                        else
                                        {
                                            txt_delegato.Text = nomeUtente;
                                            delega.id_utente_delegato = nodo.getIDPeople();

                                            delega.id_ruolo_delegato = nodo.Parent.ToString();
                                            delega.id_uo_delegato = ((TreeBase)(nodo.Parent)).Parent.ToString();
                                            string ruoloDelegato = ((myTreeNode)((TreeBase)(nodo.Parent))).Text;
                                            string[] codRuolo = ruoloDelegato.Split('-');
                                            delega.cod_ruolo_delegato = codRuolo[1].Substring(1, codRuolo[1].Length - 1);
                                            delega.id_utente_delegato = nodo.getIDPeople();
                                            delega.cod_utente_delegato = nodo.getCodiceRubrica();
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                this.hd_returnValueModal.Value = "";
                Session["delega"] = delega;
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        private void SelezionaPrimo()
        {
            try
            {
                treeViewUO.SelectedNodeIndex = "0.0";
                myTreeNode TreeNodo;
                TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex("0.0");
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        private void LoadTreeViewLivelloFigli(string indice, string tipoNodo)
        {
            try
            {
                treeViewUO.SelectedNodeIndex = indice;
                myTreeNode TreeNodo;
                TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(indice);
                TreeNodo.Expanded = true;
                if (TreeNodo.Nodes.Count > 0)
                    TreeNodo.Nodes.RemoveAt(0);
                myTreeNode nodoRuoli;
                myTreeNode nodoUtenti;
                myTreeNode nodoUO;
                myTreeNode nodoFiglio;
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.ListaRuoliUO(TreeNodo.getIDCorrGlobale());
                ArrayList lista = new ArrayList();
                lista = theManager.getListaRuoliUO();
                // ... ruoli
                if (lista != null && lista.Count > 0)
                {
                    foreach (DocsPAWA.DocsPaWR.OrgRuolo ruolo in lista)
                    {
                        nodoRuoli = new myTreeNode();
                        nodoRuoli.ID = ruolo.IDCorrGlobale;
                        nodoRuoli.Text = ruolo.CodiceRubrica + " - " + ruolo.Descrizione;
                        nodoRuoli.ImageUrl = HttpFullPath + "/AdminTool/Images/ruolo.gif";

                        TreeNodo.Nodes.Add(nodoRuoli);

                        nodoRuoli.setTipoNodo("R");
                        nodoRuoli.setIDCorrGlobale(ruolo.IDCorrGlobale);
                        nodoRuoli.setIDTipoRuolo(ruolo.IDTipoRuolo);
                        nodoRuoli.setIDGruppo(ruolo.IDGruppo);
                        nodoRuoli.setCodice(ruolo.Codice);
                        nodoRuoli.setCodiceRubrica(ruolo.CodiceRubrica);
                        nodoRuoli.setDescrizione(ruolo.Descrizione);
                        nodoRuoli.setDiRiferimento(ruolo.DiRiferimento);
                        nodoRuoli.setIDAmministrazione(ruolo.IDAmministrazione);
                        //nodoRuoli.setPercorso(TreeNodo.getPercorso() + "<a href=\"javascript:void(0)\" onclick=\"Ricarica('"+nodoRuoli.GetNodeIndex()+"');\">" + ruolo.Descrizione + "</a> &gt; ");
                        nodoRuoli.setPercorso(TreeNodo.getPercorso() + ruolo.Descrizione + " &gt; ");
                        nodoRuoli.setResponsabile(ruolo.Responsabile);
                        // ... utenti
                        if (ruolo.Utenti.Length > 0)
                        {
                            foreach (DocsPAWA.DocsPaWR.OrgUtente utente in ruolo.Utenti)
                            {
                                nodoUtenti = new myTreeNode();

                                nodoUtenti.ID = utente.IDCorrGlobale;
                                nodoUtenti.Text = utente.CodiceRubrica + " - " + utente.Cognome + " " + utente.Nome;
                                nodoUtenti.ImageUrl = HttpFullPath + "/AdminTool/Images/utente.gif";

                                nodoRuoli.Nodes.Add(nodoUtenti);

                                nodoUtenti.setTipoNodo("P");
                                nodoUtenti.setIDCorrGlobale(utente.IDCorrGlobale);
                                nodoUtenti.setIDPeople(utente.IDPeople);
                                nodoUtenti.setCodice(utente.Codice);
                                nodoUtenti.setCodiceRubrica(utente.CodiceRubrica);
                                nodoUtenti.setIDAmministrazione(utente.IDAmministrazione);
                            }
                        } // fine inserimento utenti	
                        else
                        {
                            nodoRuoli.Text = ruolo.CodiceRubrica + " - " + ruolo.Descrizione;

                        }
                    } // fine inserimento ruoli 						
                }

                // ... uo sottostanti				
                int livello = Convert.ToInt32(TreeNodo.getLivello()) + 1;

                theManager.ListaUO(TreeNodo.getIDCorrGlobale(), livello.ToString(), TreeNodo.getIDAmministrazione());
                lista = theManager.getListaUO();

                if (lista != null && lista.Count > 0)
                {
                    foreach (DocsPAWA.DocsPaWR.OrgUO sub_uo in lista)
                    {
                        nodoUO = new myTreeNode();

                        nodoUO.ID = sub_uo.IDCorrGlobale;
                        nodoUO.Text = sub_uo.CodiceRubrica + " - " + sub_uo.Descrizione;
                        nodoUO.ImageUrl = HttpFullPath + "/AdminTool/Images/uo.gif";

                        TreeNodo.Nodes.Add(nodoUO);

                        nodoUO.setTipoNodo("U");
                        nodoUO.setIDCorrGlobale(sub_uo.IDCorrGlobale);
                        nodoUO.setCodice(sub_uo.Codice);
                        nodoUO.setCodiceRubrica(sub_uo.CodiceRubrica);
                        nodoUO.setDescrizione(sub_uo.Descrizione);
                        nodoUO.setLivello(sub_uo.Livello);
                        nodoUO.setIDAmministrazione(sub_uo.IDAmministrazione);
                        nodoUO.setCodRegInterop(sub_uo.CodiceRegistroInterop);
                        //nodoUO.setPercorso(TreeNodo.getPercorso() + "<a href=\"javascript:void(0)\" onclick=\"Ricarica('"+nodoUO.GetNodeIndex()+"');\">" + sub_uo.Descrizione + "</a> &gt; ");
                        nodoUO.setPercorso(TreeNodo.getPercorso() + sub_uo.Descrizione + " &gt; ");

                        if ((!sub_uo.Ruoli.Equals("0")) || (!sub_uo.SottoUo.Equals("0")))
                        {
                            nodoFiglio = new myTreeNode();
                            nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                            //TODO VERO if (this.chklst_utente.SelectedIndex == 0)

                            nodoUO.Nodes.Add(nodoFiglio);
                        }
                        else
                        {
                            nodoUO.Text = sub_uo.CodiceRubrica + " - " + sub_uo.Descrizione;
                        }
                    } // fine inserimento uo sottostanti
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        private void treeViewUO_SelectedIndexChange(object sender, Microsoft.Web.UI.WebControls.TreeViewSelectEventArgs e)
        {
            try
            {
                if (!e.NewNode.Equals("0"))
                {
                    myTreeNode TreeNodo;
                    TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(e.NewNode);
                    if (TreeNodo.getTipoNodo() == "P")
                    {

                        DocsPAWA.DocsPaWR.InfoDelega delega = (DocsPAWA.DocsPaWR.InfoDelega)(Session["delega"]);
                        string nomeUtente = TreeNodo.Text.Substring(TreeNodo.getCodiceRubrica().Length + 3, TreeNodo.Text.Length - (TreeNodo.getCodiceRubrica().Length + 3));

                        if (((myTreeNode)TreeNodo).getTipoNodo().ToString() == "P")
                        {
                            //delegante
                            if (chklst_utente.SelectedIndex == 0)
                            {
                                txt_delegante.Text = nomeUtente;
                                setInfoRuolo(TreeNodo.getIDPeople(), ((myTreeNode)TreeNodo.Parent).getDescrizione());
                                //txt_ruolo_delegante.Text = ((myTreeNode)TreeNodo.Parent).getDescrizione();

                                delega.id_utente_delegante = TreeNodo.getIDPeople();
                                delega.cod_utente_delegante = TreeNodo.getCodiceRubrica();

                                if (this.chkTutti.Checked)
                                {
                                    delega.id_ruolo_delegante = "0";
                                    delega.cod_ruolo_delegante = "TUTTI";
                                }
                                else
                                {
                                    delega.id_ruolo_delegante = TreeNodo.Parent.ToString();
                                    string ruoloDelegante = ((myTreeNode)((TreeBase)(TreeNodo.Parent))).Text;
                                    string[] codRuoloDelegante = ruoloDelegante.Split('-');
                                    delega.cod_ruolo_delegante = codRuoloDelegante[1].Substring(1, codRuoloDelegante[1].Length - 1);
                                }
                            }
                            //delegato
                            else
                            {
                                txt_delegato.Text = nomeUtente;
                                delega.id_ruolo_delegato = TreeNodo.Parent.ToString();
                                delega.id_uo_delegato = ((TreeBase)(TreeNodo.Parent)).Parent.ToString();
                                string ruoloDelegato = ((myTreeNode)((TreeBase)(TreeNodo.Parent))).Text;
                                string[] codRuolo = ruoloDelegato.Split('-');
                                delega.cod_ruolo_delegato = codRuolo[1].Substring(1, codRuolo[1].Length - 1);
                                delega.id_utente_delegato = TreeNodo.getIDPeople();
                                delega.cod_utente_delegato = TreeNodo.getCodiceRubrica();
                            }
                            Session["delega"] = delega;
                        }
                    }
                }
            }
            catch
            {
                Response.Write("<SCRIPT>('Attenzione, si è verificato un errore di sistema');</SCRIPT>");
            }
        }

        private void treeViewUO_Expand(object sender, Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e)
        {
            try
            {
                if (e.Node != "0")
                {
                    myTreeNode TreeNodo;
                    TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(e.Node);
                    if (TreeNodo.getTipoNodo().Equals("U"))
                    {
                        if (TreeNodo.Nodes.Count > 0)
                            TreeNodo.Nodes.Clear();
                        this.LoadTreeViewLivelloFigli(e.Node, TreeNodo.getTipoNodo());
                    }
                    treeViewUO.SelectedNodeIndex = e.Node;
                }
                else
                {
                    this.InizializeTree();
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        private void treeViewUO_Collapse(object sender, Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e)
        {
            try
            {
                if (e.Node != "0")
                {
                    myTreeNode TreeNodo;
                    TreeNodo = (myTreeNode)treeViewUO.GetNodeFromIndex(e.Node);
                    Microsoft.Web.UI.WebControls.TreeNode nodoFiglio;
                    if (TreeNodo.getTipoNodo().Equals("U"))
                    {
                        if (TreeNodo.Nodes.Count > 0)
                            TreeNodo.Nodes.Clear();

                        nodoFiglio = new Microsoft.Web.UI.WebControls.TreeNode();
                        nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                        TreeNodo.Nodes.Add(nodoFiglio);
                    }
                    treeViewUO.SelectedNodeIndex = e.Node;
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        #region Tipologia di ricerca
        /// <summary>
        /// DDL per impostare la tipologia della ricerca in organigramma
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddl_ricTipo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.restoreSearchDefault();
            switch (this.ddl_ricTipo.SelectedValue)
            {
                case "U":
                    this.td_descRicerca.InnerHtml = "Nome UO:";
                    break;
                case "R":
                    this.td_descRicerca.InnerHtml = "Nome ruolo:";
                    break;
                case "PN":
                    this.td_descRicerca.InnerHtml = "Nome utente:";
                    break;
                case "PC":
                    this.td_descRicerca.InnerHtml = "Cognome utente:";
                    break;
            }
        }

        /// <summary>
        /// re-imposta la GUI di default per la tipologia di ricerca
        /// </summary>
        private void restoreSearchDefault()
        {
            this.txt_ricCod.Visible = true;
            this.txt_ricCod.Text = string.Empty;
            this.td_descRicerca.Visible = true;
            this.txt_ricDesc.Visible = true;
            this.txt_ricDesc.Text = string.Empty;
            this.btn_find.Visible = true;
        }
        #endregion
        #endregion

        private void setDefaultButton()
        {
            DocsPAWA.Utils.DefaultButton(this, ref txt_delegante, ref btn_conferma);
            DocsPAWA.Utils.DefaultButton(this, ref txt_delegato, ref btn_conferma);
            DocsPAWA.Utils.DefaultButton(this, ref txt_ricDesc, ref btn_find);
            DocsPAWA.Utils.DefaultButton(this, ref txt_ricCod, ref btn_find);
        }
    }
}
