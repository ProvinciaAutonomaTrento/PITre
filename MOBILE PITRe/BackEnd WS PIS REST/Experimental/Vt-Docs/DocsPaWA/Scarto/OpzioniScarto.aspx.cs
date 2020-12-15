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
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.Scarto
{
    public partial class OpzioniScarto : DocsPAWA.CssPage
    {
        protected DocsPaWebService wws = new DocsPaWebService();

        protected System.Web.UI.WebControls.Label lbl_titolo;
        protected DocsPaWebCtrlLibrary.ImageButton btn_filtroRicerca;
        protected DocsPaWebCtrlLibrary.ImageButton btn_Istanza;

        //Variabili per opzione ricerca
        protected System.Web.UI.WebControls.RadioButtonList rb_opzioni;
        protected System.Web.UI.WebControls.Panel pnl_filtroRicerca;
        protected System.Web.UI.WebControls.Label Llb_annoF;
        protected System.Web.UI.WebControls.TextBox tbAnnoProt;
        protected DocsPaWebCtrlLibrary.ImageButton btn_titolario;
        protected DocsPaWebCtrlLibrary.ImageButton btn_ricerca;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        private DocsPAWA.DocsPaWR.Registro[] userRegistri;
        private DocsPAWA.DocsPaWR.Registro userReg;
        private DocsPAWA.DocsPaWR.FascicolazioneClassificazione m_classificazioneSelezionata;
        protected Microsoft.Web.UI.WebControls.TreeView Gerarchia;
  
        //Variabili per opzione visibilita instanza
        protected ArrayList dataTableProt;
        protected System.Web.UI.WebControls.Panel pnl_istanza;
        protected DocsPAWA.DocsPaWR.InfoScarto[] infoScarto;
        protected int nRec;
        protected int numTotPage;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                
                FascicoliManager.removeFascicoloSelezionato(this);
                pnl_istanza.Visible = false;
                pnl_filtroRicerca.Visible = true;
                pnl_ricXCodice.Visible = false;
                caricaRegistriDisponibili();
                caricaComboTitolari();
                SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;

                if (currentContext != null &&
                    currentContext.ContextName == SiteNavigation.NavigationKeys.GESTIONE_SCARTO &&
                    currentContext.IsBack &&
                    currentContext.QueryStringParameters.ContainsKey("idFasc"))
                {
                    string url = "";
                    url = "TabListaFasc.aspx?tipoR=R";
                    string funct_dx2 = "top.principale.frames[1].location='" + url + "'";
                    this.Page.Response.Write("<script> " + funct_dx2 + "</script>");
                    return;
                }
                if (Session["operazione"] == null)
                {
                    string url = "whitepage.htm";
                    string funct_dx2 = "top.principale.frames[1].location='" + url + "'";
                    this.Page.Response.Write("<script> " + funct_dx2 + "</script>");
                    Session.Add("operazione", "ricerca");
                }
                else
                    if (Session["operazione"].ToString() == "istanza")
                    {
                        btn_Istanza_Click(null, null);
                        
                    }      
                
            }
            if (Session["operazione"].ToString() == "ricerca")
            {
                DocsPAWA.DocsPaWR.Fascicolo fascicolo;
                fascicolo = FascicoliManager.getFascicoloSelezionato(this);
                lbl_mesi.Visible = false;
                if (Session["idTitolarioSelezionato"] != null)
                {
                    ddl_titolari.SelectedValue = Session["idTitolarioSelezionato"].ToString();
                    if (!string.IsNullOrEmpty(txt_codice.Text))
                    {
                        cercaClassificazioneDaCodice();
                        
                    }
                    Session.Remove("idTitolarioSelezionato");
                }

                if (fascicolo != null && (Session["DaTit"]!=null && Session["DaTit"].ToString() == "T"))
                {
                    Session.Remove("DaTit");
                    this.txt_codice.Text = fascicolo.codice;
                    this.mesi.Text = fascicolo.numMesiConservazione;
                    this.lbl_mesi.Visible = true;
                    this.mesi.Visible = true;
                    getTree();
                }
                
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_filtroRicerca.Click += new System.Web.UI.ImageClickEventHandler(this.btn_filtroRicerca_Click);
            this.btn_Istanza.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Istanza_Click);
            this.btn_ricerca.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ricerca_Click);
            this.btn_titolario.Click += new System.Web.UI.ImageClickEventHandler(this.btn_titolario_Click);
            this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
            this.ddl_titolari.SelectedIndexChanged += new System.EventHandler(this.ddl_titolari_SelectedIndexChanged);
            this.rb_opzioni.SelectedIndexChanged += new System.EventHandler(this.rb_opzioni_SelectedIndexChanged);
            this.txt_codice.TextChanged += new System.EventHandler(this.txt_codice_TextChanged);
            this.dg_istanze.SelectedIndexChanged += new System.EventHandler(this.dg_istanze_SelectedIndexChanged);
            this.dg_istanze.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dg_istanze_ItemCreated);
        }
        #endregion

        private void rb_opzioni_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                string url = "whitepage.htm";
                string funct_dx2 = "top.principale.frames[1].location='" + url + "'";
                this.Page.Response.Write("<script> " + funct_dx2 + "</script>");
                
                string opzione = this.rb_opzioni.SelectedValue;
                if (this.Gerarchia != null)
                    this.Gerarchia.Nodes.Clear();
                FascicoliManager.removeFascicoloSelezionato();
                clear_campi_Ricerca();
                if (opzione == "tutti")
                {
                    lbl_mesi.Visible = false;
                    txt_codice.Enabled = false;
                    btn_titolario.Enabled = false;
                    this.mesi.Visible = false;
                    pnl_ricXCodice.Visible = false;
                }
                else
                {
                    pnl_ricXCodice.Visible = true;
                    lbl_mesi.Visible = false;
                    txt_codice.Enabled = true;
                    btn_titolario.Enabled = true;
                    this.mesi.Visible = false;
                }
                
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        private void getTree()
        {
            DocsPAWA.DocsPaWR.Fascicolo fascicolo;
            try
            {
                fascicolo = FascicoliManager.getFascicoloSelezionato(this);
                if (fascicolo != null)
                {
                    DocsPAWA.DocsPaWR.FascicolazioneClassifica[] FascClass;
                    FascClass = FascicoliManager.getGerarchia(this, fascicolo.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                    caricaGerarchiaFascicolazioneClassifica(FascClass);
                }
                else
                {
                    this.Gerarchia = null;
                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }

        //Costruzione del tree per la visualizzazione del titolario
        private void caricaGerarchiaFascicolazioneClassifica(DocsPAWA.DocsPaWR.FascicolazioneClassifica[] fascClass)
        {
            this.Gerarchia.Nodes.Clear();

            //Recupero il titolario di appartenenza
            if (fascClass.Length != 0)
            {
                if (fascClass[0].idTitolario != null && fascClass[0].idTitolario != "")
                {
                    DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(fascClass[0].idTitolario);
                    Microsoft.Web.UI.WebControls.TreeNode nodoTit = new Microsoft.Web.UI.WebControls.TreeNode();
                    nodoTit.Text = "<strong>" + titolario.Descrizione + "</strong>";
                    nodoTit.ID = titolario.ID;
                    this.Gerarchia.Nodes.Add(nodoTit);

                }
            }

            Microsoft.Web.UI.WebControls.TreeNode Root2 = null;
            for (int i = 0; i < fascClass.Length; i++)
            {
                Root2 = new Microsoft.Web.UI.WebControls.TreeNode();
                Root2.Text = fascClass[i].codice + "-" + fascClass[i].descrizione;
                Root2.ID = i.ToString();
                this.Gerarchia.Nodes.Add(Root2);
            }

            DocsPaWR.Fascicolo Fasc = null;
            string codiceFascicolo = fascClass[fascClass.Length - 1].codice;
            Fasc = FascicoliManager.getFascicoloDaArchiviare(this, codiceFascicolo);
            if (Fasc != null)
            {
                FascicoliManager.setFascicoloSelezionatoFascRapida(this, Fasc);
            }
        }


        private void dg_istanze_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }
            }
        }

        private void dg_istanze_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            DocsPaWR.InfoScarto itemScarto;
            DocsPaWR.InfoScarto[] listaScarto = FascicoliManager.getListaScarto(this);
            string key = ((Label)(this.dg_istanze.Items[this.dg_istanze.SelectedIndex].Cells[2].Controls[1])).Text;
            int indexScarto = Int32.Parse(key);

            itemScarto = listaScarto[indexScarto];
            FascicoliManager.setIstanzaScarto(this, itemScarto);

            string url = "TabListaIstanze.aspx";
            string funct_dx2 = "top.principale.frames[1].location='" + url + "'";
            this.Page.Response.Write("<script> " + funct_dx2 + "</script>");
        }

        private void btn_filtroRicerca_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            btn_ricerca.Visible = true;
            string url = "whitepage.htm";
            string funct_dx2 = "top.principale.frames[1].location='" + url + "'";
            this.Page.Response.Write("<script> " + funct_dx2 + "</script>");
            //Rende il pulsante "istanza" grigio
            //Rende visibilie il pannello di ricerca dei fascicoli
            pnl_istanza.Visible = false;
            pnl_filtroRicerca.Visible = true;
            clear_campi_Ricerca();
            //caricaRegistriDisponibili();
        }

        private void clear_campi_Ricerca()
        {
            ddl_titolari.SelectedIndex = 0;
            ddl_registri.SelectedIndex = 0;
            lbl_mesi.Visible = false;
            mesi.Text = "";
            txt_codice.Text = "";
        }

        private void btn_Istanza_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            btn_ricerca.Visible = false;
            if (Session["operazione"].ToString() != "istanza")
            {
                string url = "whitepage.htm";
                string funct_dx2 = "top.principale.frames[1].location='" + url + "'";
                this.Page.Response.Write("<script> " + funct_dx2 + "</script>");
            }
            //Rende visibile il pannello delle istanze
            //Carica le eventuali istanze
            pnl_filtroRicerca.Visible = false;
            pnl_istanza.Visible = true;
            FascicoliManager.removeFascicoloSelezionato(this);
            DocsPaWR.InfoScarto[] listaScarto = null;
            dataTableProt = new ArrayList();
            listaScarto = FascicoliManager.getListaScarto(this, UserManager.getInfoUtente(this), dg_istanze.CurrentPageIndex + 1, out numTotPage, out nRec);
            this.TotalRecordCount = nRec;
            this.dg_istanze.VirtualItemCount = this.TotalRecordCount;
            if (listaScarto != null)
            {
                if (listaScarto.Length > 0)
                {
                    for (int i = 0; i < listaScarto.Length; i++)
                    {
                        DocsPaWR.InfoScarto infoS = listaScarto[i];
                        dataTableProt.Add(new Cols(infoS.systemID, infoS.idAmm, infoS.idPeople, infoS.idRuoloInUo, infoS.stato, infoS.note, infoS.descrizione, i, infoS.data_Scarto));
                    }
                    this.dg_istanze.DataSource = dataTableProt;
                    this.dg_istanze.DataBind();

                }
            }
            FascicoliManager.setListaScarto(this, listaScarto);
            this.dg_istanze.Visible = (this.TotalRecordCount > 0);
         
        }

        private void btn_ricerca_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                if (rb_opzioni.SelectedIndex == -1)
                {
                    Response.Write("<script>alert('Attenzione selezionare un\\' opzione di ricerca.')</script>");
                    return;
                }
                if (rb_opzioni.SelectedIndex == 0)
                {
                    string url = "";
                    url = "TabListaFasc.aspx?tipoR=T";
                    string funct_dx2 = "top.principale.frames[1].location='" + url + "'";
                    this.Page.Response.Write("<script> " + funct_dx2 + "</script>");
                }
                if (rb_opzioni.SelectedIndex == 1)
                {
                    //Verifica che siano stati inseriti tutti i campi obbligatori
                    if (string.IsNullOrEmpty(this.txt_codice.Text))
                    {
                        Response.Write("<script>alert('Attenzione selezionare un fascicolo.')</script>");
                        return;
                    }
                    else
                    {

                        string url = "TabListaFasc.aspx?tipoR=C";
                        string funct_dx2 = "top.principale.frames[1].location='" + url + "'";
                        this.Page.Response.Write("<script> " + funct_dx2 + "</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        private void txt_codice_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                
                if (cercaClassificazioneDaCodice())
                {
                    FascicoliManager.removeClassificazioneSelezionata(this);
                }
                else
                {
                    if (!this.txt_codice.Text.Equals(""))
                    {
                        string s2 = "<script>alert('Attenzione: codice classifica non presente!');</script>";
                        if (!IsStartupScriptRegistered("NoCod"))
                            Page.RegisterStartupScript("NoCod", s2);
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_codice.ID + "').focus() </SCRIPT>";
                        RegisterStartupScript("focus", s);
                        ddl_titolari.SelectedIndex = 0;
                    }
                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
        }


        private bool cercaClassificazioneDaCodice()
        {
            bool res = false;
            //Recupero la descrizione e i mesi di conservazione in base al codice digitato
            //Se li trovo ok altrimenti avviso
            if (ddl_titolari.SelectedItem.Text == "Tutti i titolari") // || (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && listaFasc.Length == 1))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "apriSceltaTitolario", "ApriSceltaTitolario('" + txt_codice.Text + "');", true);
                FascicoliManager.removeMemoriaFiltriRicFasc(this);
                FascicoliManager.removeFiltroRicFasc(this);
                return true;
            }
            else //if (ddl_titolari.SelectedItem.Text != "Tutti i titolari" || (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && listaFasc.Length == 1))
            {
                DocsPAWA.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(getIdTitolario(null));
                //WS->cerco webmetod per ottenere la descrizione e i mesi di conservazione del fascicolo
                InfoUtente infoUtente = UserManager.getInfoUtente(this);
                FascicoliManager.removeFascicoloSelezionato();
                DocsPAWA.DocsPaWR.Fascicolo fasc = wws.FascicolazioneGetFascicoloDaCodice2(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, txt_codice.Text, UserManager.getRegistroSelezionato(this), false, false, titolario.ID);
                if (fasc != null)
                {
                    lbl_mesi.Visible = true;
                    mesi.Visible = true;
                    this.mesi.Text = fasc.numMesiConservazione;
                    FascicoliManager.setFascicoloSelezionato(this, fasc);
                    getTree();
                    res = true;
                }
                
            }

            return res;
        }

      
       

        private void btn_titolario_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            //E' necessario che sia selezionato un titolario e non la voce "tutti i titolari"
            if (ddl_titolari.Enabled && ddl_titolari.SelectedIndex == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "selezionareUnTitolario", "alert('Selezionare un titolario.');", true);
                return;
            }

            //Session["Titolario"] = "Y";
            if (!this.IsStartupScriptRegistered("apriModalDialog"))
            {
                FascicoliManager.removeFascicoloSelezionato();
                string scriptString = "<SCRIPT>ApriTitolario('codClass=" + this.txt_codice.Text + "&idTit=" + getIdTitolario(txt_codice.Text) + "','gestScarto')</SCRIPT>";
                this.RegisterStartupScript("apriModalDialog", scriptString);
            }
        }

        //Caricamento del menù a tendina per la visualizzazione dei registri
        private void caricaRegistriDisponibili()
        {
            ddl_registri.Items.Clear();
            userRegistri = UserManager.getListaRegistri(this);
            if (userRegistri != null && userRegistri.Length > 0)
            {
                for (int i = 0; i < userRegistri.Length; i++)
                {
                    this.ddl_registri.Items.Add((userRegistri[i]).codRegistro);
                    this.ddl_registri.Items[i].Value = (userRegistri[i]).systemId;
                }
                this.ddl_registri.SelectedIndex = 0;
                UserManager.setRegistroSelezionato(this, userRegistri[this.ddl_registri.SelectedIndex]);
            }
        }

        private void caricaComboTitolari()
        {
            ddl_titolari.Items.Clear();
            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(UserManager.getUtente(this).idAmministrazione));

            //Esistono dei titolari chiusi
            if (listaTitolari.Count > 1)
            {
                //Creo le voci della ddl dei titolari
                string valueTutti = string.Empty;
                foreach (DocsPaWR.OrgTitolario titolario in listaTitolari)
                {
                    ListItem it = null;
                    switch (titolario.Stato)
                    {
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                        case DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Chiuso:
                            it = new ListItem(titolario.Descrizione, titolario.ID);
                            ddl_titolari.Items.Add(it);
                            valueTutti += titolario.ID + ",";
                            break;
                    }
                }
                //Imposto la voce tutti i titolari
                valueTutti = valueTutti.Substring(0, valueTutti.Length - 1);
                if (valueTutti != string.Empty)
                {
                    if (valueTutti.IndexOf(',') == -1)
                        valueTutti = valueTutti + "," + valueTutti;

                    ListItem it = new ListItem("Tutti i titolari", valueTutti);
                    ddl_titolari.Items.Insert(0, it);
                }

                //txt_codClass.Enabled = true;
            }

            //Non esistono titolario chiusi
            if (listaTitolari.Count == 1)
            {
                DocsPaWR.OrgTitolario titolario = (DocsPaWR.OrgTitolario)listaTitolari[0];
                if (titolario.Stato != DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.InDefinizione)
                {
                    ListItem it = new ListItem(titolario.Descrizione, titolario.ID);
                    ddl_titolari.Items.Add(it);
                }
                ddl_titolari.Enabled = false;
            }
        }

        private string getIdTitolario(string codClassificazione)
        {
            if (codClassificazione != null && codClassificazione != "")
            {
                DocsPaWR.Fascicolo[] listaFasc = getFascicolo(UserManager.getRegistroSelezionato(this), codClassificazione);

                //In questo caso il metodo "GetFigliClassifica2" funzionerebbe male
                //per questo viene restituti l'idTitolario dell'unico fascicolo risolto
                if (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && listaFasc.Length == 1)
                {
                    DocsPaWR.Fascicolo fasc = (DocsPaWR.Fascicolo)listaFasc[0];
                    return fasc.idTitolario;
                }
            }

            //In tutti gli altri casi è sufficiente restituire il value degli item della
            //ddl_Titolario in quanto formati secondo le specifiche di uno o piu' titolari
            return ddl_titolari.SelectedValue;
        }


        //Questo metodo serve a verificare la ricerca tramite codice classifica i casi sono i seguenti :
        //1. Si proviene proviene da un "back" (tasto Back)si effettua la ricerca
        //2. Si proviene o meno da una selezione di un titolario, si ripristina o no il filtro ricerca per idTitolario
        //3. Selezione "Tutti i titolari" il codice restituisce un solo fascicolo OK si effettua la ricerca
        //4. Selezione <<uno specifico titolario>> OK si effettua la ricerca
        //5. Selezione "Tutti i titolari" il codice restituisce piu' di un fascicolo NO la ricerca viene bloccata e si chiede la selezione di un titolario
        private string checkRicercaFasc(string codClassificazione)
        {
            string result = "SI_RICERCA";

            DocsPaWR.Fascicolo[] listaFasc = getFascicolo(UserManager.getRegistroSelezionato(this), codClassificazione);

            //if (!this.IsPostBack && this.OnBack)
            //    return result;

            if (Session["idTitolarioSelezionato"] == null)
            {
                DocsPaWR.FiltroRicerca item = new DocsPAWA.DocsPaWR.FiltroRicerca();
                item.argomento = DocsPaWR.FiltriFascicolazione.ID_TITOLARIO.ToString();
                item.valore = ddl_titolari.SelectedValue;
                //RestoreFiltersIdTitolario(item);
            }

            if (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && listaFasc.Length > 1)
                result = "NO_RICERCA";

            if (ddl_titolari.SelectedItem.Text == "Tutti i titolari" && listaFasc.Length == 1)
                result = "SI_RICERCA";

            return result;
        }


        /// <summary>
        /// Metodo per il recupero del fascicolo da codice
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo[] getFascicolo(DocsPAWA.DocsPaWR.Registro registro, string codClassificazione)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            listaFasc = FascicoliManager.getListaFascicoliDaCodice(this, codClassificazione, registro, "R");
            if (listaFasc != null)
                return listaFasc;
            else
                return null;
        }
     

        //Mette in sessione il registro selezionato
        private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ddl_registri.SelectedIndex != -1)
            {
                clear_campi_Ricerca();
                if (userRegistri == null)
                    userRegistri = UserManager.getListaRegistri(this);
                UserManager.setRegistroSelezionato(this, userRegistri[this.ddl_registri.SelectedIndex]);
            }
        }

        private void ddl_titolari_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ddl_registri.SelectedIndex = 0;
            mesi.Text = "";
            txt_codice.Text = "";
        }

        private int TotalRecordCount
        {
            get
            {
                int count = 0;

                if (this.ViewState["TotalRecordCount"] != null)
                    Int32.TryParse(this.ViewState["TotalRecordCount"].ToString(), out count);

                return count;
            }
            set
            {
                this.ViewState["TotalRecordCount"] = value;
            }
        }

        public class Cols
        {
            private string systemID;
            private string idAmm;
            private string idPeople;
            private string idRuoloInUo;
            private string stato;
            private string note;
            private string descrizione;
            private int chiave;
            private string dataScarto;

            public Cols(string systemID, string idAmm, string idPeople, string idRuoloInUo, string stato, string note, string descrizione, int chiave, string dataScarto)
            {
                this.systemID = systemID;
                this.idAmm = idAmm;
                this.idPeople = idPeople;
                this.idRuoloInUo = idRuoloInUo;
                this.stato = stato;
                this.note = note;
                this.descrizione = descrizione;
                this.chiave = chiave;
                this.dataScarto = dataScarto;
            }

            public string SystemID { get { return systemID; } }
            public string IdAmm { get { return idAmm; } }
            public string IdPeople { get { return idPeople; } }
            public string IdRuoloInUo { get { return idRuoloInUo; } }
            public string Stato { get { return stato; } }
            public string Note { get { return note; } }
            public string Descrizione { get { return descrizione; } }
            public int Chiave { get { return chiave;  } }
            public string DataScarto { get { return dataScarto; } }
        }

    }
}
