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

namespace DocsPAWA.Archivio
{
    public partial class OpzioniArchivio : DocsPAWA.CssPage
    {
        protected DocsPAWA.UserControls.AppTitleProvider appTitleProvider;

        protected DocsPaWebService wws = new DocsPaWebService();
        protected bool focus;

        protected System.Web.UI.WebControls.RadioButtonList rb_opzioni;
        protected DocsPaWebCtrlLibrary.ImageButton btn_ricerca;
        protected System.Web.UI.WebControls.Label lbl_titolo;

        //Opzione Documenti in serie
        protected System.Web.UI.WebControls.Panel pnl_filtroSerie;
        protected System.Web.UI.WebControls.DropDownList ddl_tipologiaDoc;
        protected System.Web.UI.WebControls.Panel panel_Contenuto;
        protected System.Web.UI.WebControls.DropDownList ddl_Contatori;
        protected Table table;

        //Opzione Documenti in fascicoli generali
        protected System.Web.UI.WebControls.Panel pnl_filtroFascG;
        protected DocsPaWebCtrlLibrary.ImageButton btn_titolario;
        private DocsPAWA.DocsPaWR.FascicolazioneClassificazione m_classificazioneSelezionata;
        protected System.Web.UI.WebControls.TextBox tbAnnoProt;
        protected System.Web.UI.WebControls.TextBox TxtAnno;
        protected System.Web.UI.WebControls.Label lblAnno;
        protected System.Web.UI.WebControls.Label Llb_annoF;
        protected Microsoft.Web.UI.WebControls.TreeView Gerarchia;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        private DocsPAWA.DocsPaWR.Registro[] userRegistri;
        private DocsPAWA.DocsPaWR.Registro userReg;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (UserManager.ruoloIsAutorized(this, "GEST_ARCHIVIA"))
                {
                    this.lbl_titolo.Text = "Trasferimento in archivio corrente";
                }
                if (!Page.IsPostBack)
                {
                    Session.Remove("template");
                }
                else
                {
                    getTree();
                }
                if (Session["focus"] == null)
                {
                    Session.Add("focus", true);
                }
                if (Session["template"] != null)
                    inizializzaPanelContenuto();
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
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
            this.rb_opzioni.SelectedIndexChanged += new System.EventHandler(this.rb_opzioni_SelectedIndexChanged);
            this.ddl_tipologiaDoc.SelectedIndexChanged += new System.EventHandler(this.ddl_tipologiaDoc_SelectedIndexChanged);
            this.btn_ricerca.Click += new System.Web.UI.ImageClickEventHandler(this.btn_ricerca_Click);
            this.btn_titolario.Click += new System.Web.UI.ImageClickEventHandler(this.btn_titolario_Click);
            this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
        }
        #endregion

        private void rb_opzioni_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Session.Remove("template");
                panel_Contenuto.Controls.Clear();
                string url = "whitepage.htm";
                string funct_dx2 = "top.principale.frames[1].location='" + url + "'";
                this.Page.Response.Write("<script> " + funct_dx2 + "</script>");
                string opzione = this.rb_opzioni.SelectedValue;
                if (this.Gerarchia != null)
                    this.Gerarchia.Nodes.Clear();
                this.pnl_RFAOO.Visible = false;
                switch (opzione)
                {
                    case "fascGen":

                        this.pnl_filtroFascG.Visible = true;
                        this.pnl_filtroSerie.Visible = false;
                        this.panel_Contenuto.Visible = false;
                        caricaRegistriDisponibili();
                        break;
                    case "serie":
                        this.pnl_filtroFascG.Visible = false;
                        this.pnl_filtroSerie.Visible = true;
                        CaricaTipologia(this.ddl_tipologiaDoc);
                        ddl_tipologiaDoc.SelectedIndex = 0;
                        FascicoliManager.removeFascicoloSelezionato();
                        this.TxtAnno.Visible = false;
                        this.lblAnno.Visible = false;
                        this.TxtAnno.Text = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        private void btn_ricerca_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                string tipoOp = "";
                if (this.rb_opzioni.SelectedIndex == -1)
                {
                    Response.Write("<script>alert('Attenzione selezionare un criterio di ricerca.')</script>");
                    return;
                }
                else
                {
                    //Verifica che siano stati inseriti tutti i campi obbligatori
                    if (this.rb_opzioni.SelectedItem.Value == "fascGen")
                    {
                        if (this.Gerarchia != null && Gerarchia.Nodes.Count == 0)
                        {
                            Response.Write("<script>alert('Attenzione selezionare un fascicolo.')</script>");
                            return;
                        }
                        if (string.IsNullOrEmpty(tbAnnoProt.Text))
                        {
                            Response.Write("<script>alert('Attenzione selezionare un anno.')</script>");
                            return;
                        }
                        Session.Add("Anno", this.tbAnnoProt.Text);
                        tipoOp = "F";
                    }
                    else
                        if (this.rb_opzioni.SelectedItem.Value == "serie")
                        {
                            if (ddl_tipologiaDoc.SelectedIndex == 0)
                            {
                                Response.Write("<script>alert('Attenzione selezionare una tipologia documento.')</script>");
                                return;
                            }
                            DropDownList ddl = (DropDownList)panel_Contenuto.FindControl("ddl_Contatori");
                            if (ddl != null && ddl.SelectedIndex == -1)
                            {
                                Response.Write("<script>alert('Attenzione selezionare un contatore.')</script>");
                                return;
                            }
                            if (string.IsNullOrEmpty(this.TxtAnno.Text))
                            {
                                Response.Write("<script>alert('Attenzione selezionare un anno.')</script>");
                                return;
                            }
                            CreaCriterioRicercaSerie();
                            tipoOp = "S";
                        }
                    string url = "tabRisultatiRicArchivio.aspx?tipoOp=" + tipoOp;
                    string funct_dx2 = "top.principale.frames[1].location='" + url + "'";
                    this.Page.Response.Write("<script> " + funct_dx2 + "</script>");
                }
            }
            catch (Exception ex)
            {
                DocsPAWA.ErrorManager.redirectToErrorPage(this, ex);
            }
        }


        #region panel filtri fascicoli

        private void btn_titolario_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string amministrazione = "";
            string idTitolario = wws.getTitolarioAttivo(UserManager.getUtente(this).idAmministrazione, out amministrazione);

            if (!IsStartupScriptRegistered("apriModalDialog"))
            {
                string scriptString = "<SCRIPT>ApriTitolario('codClass=" + "" + "&idTit=" + idTitolario + "&codAmm=" + amministrazione + "','gestArchivio')</SCRIPT>";
                RegisterStartupScript("apriModalDialog", scriptString);
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

                    tbAnnoProt.Visible = true;
                    this.TxtAnno.Visible = true;
                    this.Llb_annoF.Visible = true;
                    this.lblAnno.Visible = true;
                    this.TxtAnno.Text = "";
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

        //Mette in sessione il registro selezionato
        private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (ddl_registri.SelectedIndex != -1)
            {
                if (userRegistri == null)
                    userRegistri = UserManager.getListaRegistri(this);
                UserManager.setRegistroSelezionato(this, userRegistri[this.ddl_registri.SelectedIndex]);
            }
        }

        #endregion

        #region panel serie/repertorio

        private void CreaCriterioRicercaSerie()
        {
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            int result = 0;
            Session.Add("template", template);
            DropDownList ddl = (DropDownList)panel_Contenuto.FindControl("ddl_Contatori");
            if (ddl != null)
                Session.Add("valOggetto", ddl.SelectedValue);
            else
            {
                Label lbl = (Label)panel_Contenuto.FindControl("lblContID");
                Session.Add("valOggetto", lbl.Text);
            }
            Session.Add("aoo_rf", this.ddlAooRF.SelectedValue);
            Session.Add("Anno", this.TxtAnno.Text);
        }

        //Recupera tutte le tipologie di documento che hanno almeno un contatore di repertorio che dipende dall'anno
        private void CaricaTipologia(DropDownList ddl)
        {
            DocsPaWR.Templates[] listaTemplates;
            listaTemplates = DocumentManager.getTipoAttoTrasfDeposito(this, UserManager.getInfoUtente(this).idAmministrazione, true);
            ddl.Items.Clear();
            ddl.Items.Add("");
            if (listaTemplates != null)
            {
                for (int i = 0; i < listaTemplates.Length; i++)
                {
                    ddl.Items.Add(listaTemplates[i].DESCRIZIONE);
                    ddl.Items[i + 1].Value = listaTemplates[i].SYSTEM_ID.ToString();
                }
            }
        }

        private void ddl_tipologiaDoc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(ddl_tipologiaDoc.SelectedValue))
            {
                Session.Remove("template");
                panel_Contenuto.Controls.Clear();
            }
            else
            {
                this.TxtAnno.Visible = true;
                this.lblAnno.Visible = true;
                this.TxtAnno.Text = "";
                string idTemplate = ddl_tipologiaDoc.SelectedValue;
                DocsPaWR.Templates templateInSessione = (DocsPaWR.Templates)Session["template"];
                if (!string.IsNullOrEmpty(idTemplate) && templateInSessione != null && !string.IsNullOrEmpty(templateInSessione.SYSTEM_ID.ToString()))
                {
                    if (ddl_tipologiaDoc.SelectedValue != templateInSessione.SYSTEM_ID.ToString())
                    {
                        Session.Remove("template");
                        panel_Contenuto.Controls.Clear();
                    }
                }
                if (idTemplate != "")
                {
                    DocsPaWR.Templates template = ProfilazioneDocManager.getTemplateById(idTemplate,this);
                    if (template != null)
                    {
                        Session.Add("template", template);
                        pnl_RFAOO.Visible = false;
                        ddlAooRF.Items.Clear();
                        inizializzaPanelContenuto();
                        this.TxtAnno.Visible = true;
                        this.TxtAnno.Text = "";
                        this.lblAnno.Visible = true;

                    }
                    else
                    {
                        pnl_RFAOO.Visible = false;
                    }
                }
            }
        }

        //Recupera i contatori per una scelta tipologia di documento e li inserisce nella 
        //dropdownlist ddl_Contatori
        private void inizializzaPanelContenuto()
        {
            //pnl_RFAOO.Visible = false;
            if (Session["template"] != null)
            {
                DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
                table = new Table();
                table.ID = "table_Contatori";
                TableCell cell_2 = new TableCell();
                int numContatori = 0;
                string testoUnicoContatore = "";
                string idUnicoContatore = "";
                DocsPAWA.DocsPaWR.OggettoCustom oggettoUnico = null;
                ddl_Contatori = new DropDownList();
                ddl_Contatori.ID = "ddl_Contatori";
                ddl_Contatori.Font.Size = FontUnit.Point(8);
                ddl_Contatori.CssClass = "titolo_scheda";
                ddl_Contatori.Font.Name = "Verdana";
                foreach (DocsPAWA.DocsPaWR.OggettoCustom oggetto in template.ELENCO_OGGETTI)
                {
                    if (oggetto.REPERTORIO == "1")
                    {
                        //rendo visibili i pannelli
                        if (oggetto.TIPO.DESCRIZIONE_TIPO == "Contatore" && oggetto.FORMATO_CONTATORE.ToUpper().Contains("ANNO"))
                        {
                            if (oggetto.DESCRIZIONE.Equals(""))
                            {
                                return;
                            }
                            //testoUnicoContatore e idUnicoContatore servono nel caso in cui sia presente un solo
                            //contatore, in questo caso non visualizzo la dropdownlist ma una semplice label
                            testoUnicoContatore = oggetto.DESCRIZIONE.ToString();
                            idUnicoContatore = oggetto.SYSTEM_ID.ToString();
                            oggettoUnico = oggetto;
                            ddl_Contatori.Items.Add(new ListItem(oggetto.DESCRIZIONE.ToString(), oggetto.SYSTEM_ID.ToString()));
                            numContatori++;
                        }
                    }
                }
                TableRow row = new TableRow();
                row.ID = "row_Contatori";
                TableCell cell_1 = new TableCell();
                TableCell cell_3 = new TableCell();
                if (numContatori > 1)
                {
                    cell_1.Controls.Add(ddl_Contatori);
                    ddl_Contatori.AutoPostBack = true;
                    this.ddl_Contatori.SelectedIndexChanged += new System.EventHandler(this.ddl_Contatori_SelectedIndexChanged);
                }
                else
                {
                    Label lblContatore = new Label();
                    lblContatore.ID = "lblContatore";
                    lblContatore.Font.Size = FontUnit.Point(8);
                    lblContatore.CssClass = "titolo_scheda";
                    lblContatore.Font.Name = "Verdana";
                    lblContatore.Text = testoUnicoContatore;
                    cell_1.Controls.Add(lblContatore);
                    Label lblContatoreID = new Label();
                    lblContatoreID.ID = "lblContID";
                    lblContatoreID.Text = idUnicoContatore;
                    lblContatoreID.Visible = false;
                    cell_3.Controls.Add(lblContatoreID);
                    if (ddlAooRF.SelectedIndex == -1)
                    {
                        DocsPaWR.Ruolo ruoloUtente = UserManager.getRuolo(this);
                        DocsPaWR.Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "", "");

                        switch (oggettoUnico.TIPO_CONTATORE)
                        {
                            case "T":
                                break;
                            case "A":
                                lblAooRF.Text = "&nbsp;AOO";
                                ////Aggiungo un elemento vuoto
                                ListItem it = new ListItem();
                                it.Value = "";
                                it.Text = "";
                                ddlAooRF.Items.Add(it);
                                //Distinguo se è un registro o un rf
                                for (int i = 0; i < registriRfVisibili.Length; i++)
                                {
                                    ListItem item = new ListItem();
                                    if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "0")
                                    {
                                        item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                        item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                        ddlAooRF.Items.Add(item);
                                    }
                                }
                                ddlAooRF.Width = 100;
                                this.pnl_RFAOO.Visible = true;
                                break;
                            case "R":
                                lblAooRF.Text = "&nbsp;RF";
                                ////Aggiungo un elemento vuoto
                                ListItem it_1 = new ListItem();
                                it_1.Value = "";
                                it_1.Text = "";
                                ddlAooRF.Items.Add(it_1);
                                //Distinguo se è un registro o un rf
                                for (int i = 0; i < registriRfVisibili.Length; i++)
                                {
                                    ListItem item = new ListItem();
                                    if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "1" && ((DocsPaWR.Registro)registriRfVisibili[i]).rfDisabled == "0")
                                    {
                                        item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                        item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                        ddlAooRF.Items.Add(item);
                                    }
                                }
                                ddlAooRF.Width = 100;
                                this.pnl_RFAOO.Visible = true;
                                break;
                        }


                    }
                }
                row.Cells.Add(cell_1);
                if (cell_3 != null)
                    row.Cells.Add(cell_3);
                row.Cells.Add(cell_2);
                table.Rows.Add(row);

                panel_Contenuto.Controls.Add(table);
                this.panel_Contenuto.Visible = true;
                this.btn_ricerca.Visible = true;
            }
        }

        //Se il contatore è di tipo AOO o rf recupera la lista di AOO o la lista di rf 
        //e li inserisci nella dropdownlist ddlAooRF
        private void ddl_Contatori_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ddlAooRF.Items.Clear();
            this.pnl_RFAOO.Visible = false;
            this.TxtAnno.Text = "";
            Session["aoo_rf"] = "";
            DocsPAWA.DocsPaWR.Templates template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            foreach (DocsPAWA.DocsPaWR.OggettoCustom oggetto in template.ELENCO_OGGETTI)
            {
                if (oggetto.REPERTORIO == "1")
                {
                    if (oggetto.TIPO.DESCRIZIONE_TIPO == "Contatore" && oggetto.FORMATO_CONTATORE.ToUpper().Contains("ANNO"))
                    {
                        if (oggetto.DESCRIZIONE.Equals(""))
                        {
                            return;
                        }

                        if (oggetto.SYSTEM_ID.ToString().Equals(ddl_Contatori.SelectedItem.Value))
                        {
                            DocsPaWR.Ruolo ruoloUtente = UserManager.getRuolo(this);
                            DocsPaWR.Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "", "");
                            this.pnl_RFAOO.Visible = false;
                            switch (oggetto.TIPO_CONTATORE)
                            {
                                case "T":
                                    break;
                                case "A":
                                    lblAooRF.Text = "&nbsp;AOO";
                                    ////Aggiungo un elemento vuoto
                                    ListItem it = new ListItem();
                                    it.Value = "";
                                    it.Text = "";
                                    ddlAooRF.Items.Add(it);
                                    //Distinguo se è un registro o un rf
                                    for (int i = 0; i < registriRfVisibili.Length; i++)
                                    {
                                        ListItem item = new ListItem();
                                        if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "0")
                                        {
                                            item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                            item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                            ddlAooRF.Items.Add(item);
                                        }
                                    }
                                    ddlAooRF.Width = 100;
                                    this.pnl_RFAOO.Visible = true;
                                    break;
                                case "R":
                                    lblAooRF.Text = "&nbsp;RF";
                                    ////Aggiungo un elemento vuoto
                                    ListItem it_1 = new ListItem();
                                    it_1.Value = "";
                                    it_1.Text = "";
                                    ddlAooRF.Items.Add(it_1);
                                    //Distinguo se è un registro o un rf
                                    for (int i = 0; i < registriRfVisibili.Length; i++)
                                    {
                                        ListItem item = new ListItem();
                                        if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "1" && ((DocsPaWR.Registro)registriRfVisibili[i]).rfDisabled == "0")
                                        {
                                            item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                            item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                            ddlAooRF.Items.Add(item);
                                        }
                                    }
                                    ddlAooRF.Width = 100;
                                    this.pnl_RFAOO.Visible = true;
                                    break;
                            }

                        }
                    }
                }
            }
        }
        #endregion

        #region SetFocus
        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
            RegisterStartupScript("focus", s);
        }
        #endregion SetFocus
    }
}
