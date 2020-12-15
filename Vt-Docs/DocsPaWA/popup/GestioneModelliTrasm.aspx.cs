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
using DocsPAWA.DocsPaWR;
using System.Xml.Linq;

namespace DocsPAWA.popup
{
    /// <summary>
    /// Summary description for GestioneModelliTrasm.
    /// </summary>
    public class GestioneModelliTrasm : DocsPAWA.CssPage
    {
        protected System.Web.UI.WebControls.Label lbl_titolo;
        protected System.Web.UI.WebControls.Button btn_lista_modelli;
        protected System.Web.UI.WebControls.Button btn_salvaModello;
        protected System.Web.UI.WebControls.Button btn_nuovoModello;
        //protected System.Web.UI.WebControls.Button btn_ricerca;
        //protected System.Web.UI.WebControls.TextBox txt_ricerca;
        //protected System.Web.UI.WebControls.Label lbl_ricerca;
        protected Utilities.MessageBox msg_ConfirmDel;
        protected System.Web.UI.WebControls.DataGrid dt_listaModelli;
        protected System.Web.UI.WebControls.Panel Panel_ListaModelli;
        protected System.Web.UI.WebControls.TextBox txt_nomeModello;
        protected System.Web.UI.WebControls.TextBox txt_codModello;
        protected System.Web.UI.WebControls.TextBox txt_noteGenerali;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoTrasmissione;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        protected System.Web.UI.WebControls.Panel Panel_NuovoModello;
        protected System.Web.UI.WebControls.DropDownList ddl_ragioni;
        protected System.Web.UI.WebControls.ImageButton btn_Rubrica_dest;
        protected System.Web.UI.WebControls.DataGrid dt_dest;
        protected System.Web.UI.WebControls.Panel Panel_dest;
        protected System.Web.UI.HtmlControls.HtmlGenericControl DivDGListaTemplates;
        protected System.Web.UI.HtmlControls.HtmlGenericControl Div1;
        protected System.Web.UI.WebControls.Label lbl_avviso;
        protected string idAmministrazione;
        protected ArrayList modelliTrasmissione;
        protected DocsPAWA.DocsPaWR.ModelloTrasmissione ModelloTrasmissione = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
        protected DocsPAWA.DocsPaWR.RagioneTrasmissione[] listaRagioni;
        private DocsPAWA.DocsPaWR.Registro[] userRegistri;
        protected string gerarchia_trasm;
        protected string cha_tipo_ragione;

        private Ruolo userRuolo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden txt_confirmDel;
        protected System.Web.UI.WebControls.Button btn_chiudi;
        protected System.Web.UI.WebControls.Button btn_pp_notifica;
        protected System.Web.UI.WebControls.RadioButtonList rbl_share;
        private Utente utente;
        //protected System.Web.UI.WebControls.TextBox txt_ricercaCodice;
        protected System.Web.UI.WebControls.TextBox txt_codDest;
        protected System.Web.UI.WebControls.ImageButton ibtnMoveToA;
        protected System.Web.UI.WebControls.Label lbl_codice;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;
        protected System.Web.UI.WebControls.Label lbl_stato;
        //protected System.Web.UI.HtmlControls.HtmlInputHidden hd_multimittente;
        protected System.Web.UI.WebControls.Label lbl_registri;
        protected System.Web.UI.WebControls.Label lbl_ragione;
        protected System.Web.UI.WebControls.Label lbl_registro_obb;
        //protected System.Web.UI.WebControls.CheckBox cb_ruoliDestInibitiTrasm;

        // Pannello di ricerca
        protected DocsPAWA.UserControls.PannelloRicercaModelliTrasmissione prmtRicerca;

        private void Page_Load(object sender, System.EventArgs e)
        {
            idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;
            userRuolo = UserManager.getRuolo(this);
            utente = UserManager.getUtente(this);

            if (!IsPostBack)
            {
                modelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.getInfoUtente(this), this.prmtRicerca.CreateSearchFilters());
                caricaDataGridModelli();

                ModelloTrasmissione = new ModelloTrasmissione();
                ModelloTrasmissione.ID_AMM = idAmministrazione;
                ModelloTrasmissione.SINGLE = "0";
                ModelloTrasmissione.ID_PEOPLE = utente.idPeople;
                Session.Add("Modello", ModelloTrasmissione);

                CaricaComboRegistri(ddl_registri);
            }
            if (((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]) != null && ((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MITTENTE == null)
            {
                DocsPaWR.MittDest mittente = new DocsPAWA.DocsPaWR.MittDest();
                ((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MITTENTE = new MittDest[1];
                mittente.CHA_TIPO_MITT_DEST = "M";
                mittente.VAR_COD_RUBRICA = userRuolo.codiceRubrica;
                mittente.DESCRIZIONE = userRuolo.descrizione;
                mittente.ID_CORR_GLOBALI = Convert.ToInt32(userRuolo.systemId);
                mittente.CHA_TIPO_URP = "R";//userRuolo.tipoCorrispondente;

                ((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MITTENTE[0] = mittente;
            }


            this.CaricaRagioni(idAmministrazione, false);
            if (Session["selDestDaRubrica"] != null)
            {
                addDestSelDaRubrica((ElementoRubrica[])Session["selDestDaRubrica"]);
            }

            tastoRicerca();

            // valore di ritorno della modale delle notifiche (possibili valori: "I", "U" o "")
            if (this.hd_returnValueModal.Value != null && this.hd_returnValueModal.Value != string.Empty && this.hd_returnValueModal.Value != "undefined")
            {
                if (this.hd_returnValueModal.Value.Equals("I"))
                    this.PerformSaveModel();

                this.hd_returnValueModal.Value = string.Empty;
            }

            if (Session["ClickFindAR"] != null)
            {
                Session.Remove("ClickFindAR");
                btn_lista_modelli_Click(null, null);
            }
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
            this.msg_ConfirmDel.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_ConfirmDel_GetMessageBoxResponse);
            this.btn_lista_modelli.Click += new System.EventHandler(this.btn_lista_modelli_Click);
            this.btn_salvaModello.Click += new System.EventHandler(this.btn_salvaModello_Click);
            this.btn_nuovoModello.Click += new System.EventHandler(this.btn_nuovoModello_Click);
            //this.btn_ricerca.Click += new System.EventHandler(this.btn_ricerca_Click);
            this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
            this.dt_listaModelli.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dt_listaModelli_PageIndexChanged);
            this.dt_listaModelli.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dt_listaModelli_DeleteCommand);
            this.dt_listaModelli.SelectedIndexChanged += new System.EventHandler(this.dt_listaModelli_SelectedIndexChanged);
            this.txt_nomeModello.TextChanged += new System.EventHandler(this.txt_nomeModello_TextChanged);
            this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
            this.btn_Rubrica_dest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Rubrica_dest_Click);
            this.dt_dest.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dt_dest_EditCommand);
            this.dt_dest.ItemCreated += new DataGridItemEventHandler(this.dt_dest_ItemCreated);
            this.Load += new System.EventHandler(this.Page_Load);
            this.ibtnMoveToA.Click += new System.Web.UI.ImageClickEventHandler(this.ibtnMoveToA_Click);
            this.dt_listaModelli.PreRender += new EventHandler(dt_listaModelli_PreRender);
            this.PreRender += new EventHandler(GestioneModelliTrasm_PreRender);
            this.ddl_ragioni.SelectedIndexChanged += new EventHandler(ddl_ragioni_SelectedIndexChanged);

            // Associazione evento di ricerca al pannello dei filtri
            this.prmtRicerca.Search += new EventHandler(this.btn_ricerca_Click);
        }

        void ddl_ragioni_SelectedIndexChanged(object sender, EventArgs e)
        {
            DocsPaWR.RagioneTrasmissione ragioneSel = new DocsPAWA.DocsPaWR.RagioneTrasmissione();
            ragioneSel = listaRagioni[this.ddl_ragioni.SelectedIndex];
            
            //
            // Mev Cessione Diritti - Mamtieni Scrittura
            if (!string.IsNullOrEmpty(ragioneSel.mantieniScrittura) && (ragioneSel.mantieniScrittura == "1"))
            {
                ((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MANTIENI_SCRITTURA = "1";
            }
            else
                ((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MANTIENI_SCRITTURA = "0";
            // End Mev
            //

            if (!string.IsNullOrEmpty(ragioneSel.mantieniLettura) && (ragioneSel.mantieniLettura == "1"))
            {
                ((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MANTIENI_LETTURA = "1";
            }
            else
                ((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MANTIENI_LETTURA = "0";

            Session["noRagioneTrasmNullaFE"] = true;
        }

        void GestioneModelliTrasm_PreRender(object sender, EventArgs e)
        {
            if (Session["noRagioneTrasmNullaFE"] == null)
            {
                if (dt_dest.Items.Count != 0)
                {
                    this.lbl_ragione.Visible = false;
                    ListItem item = new ListItem("", "0");
                    if (!ddl_ragioni.Items.Contains(item))
                        ddl_ragioni.Items.Add(item);
                    int index = ddl_ragioni.Items.IndexOf(ddl_ragioni.Items.FindByValue("0"));

                    this.ddl_ragioni.SelectedIndex = index;
                }
                else
                    this.lbl_ragione.Visible = true;
            }
            else
                Session.Remove("noRagioneTrasmNullaFE");
        }

        void dt_listaModelli_PreRender(object sender, EventArgs e)
        {
            if (UserManager.ruoloIsAutorized(this, "DO_DEL_MOD_TRASM"))
                this.dt_listaModelli.Columns[7].Visible = true;
            else
                this.dt_listaModelli.Columns[7].Visible = false;
        }

        protected void dt_dest_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            try
            {

            }
            catch
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Selezione del destinatario

        /// <summary>
        /// Seleziona destinatari per codice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ibtnMoveToA_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                Session["noRagioneTrasmNullaFE"] = true;
                if (this.ddl_ragioni.SelectedItem.Text.Equals(""))
                {
                    RegisterStartupScript("noresults", "<script language=\"javascript\">alert(\"Selezionare una ragione trasmissione!\");</script>");
                }
                else
                {
                    if (this.txt_codDest.Text.Trim() != "" || this.txt_codDest.Text.Trim() != string.Empty)
                    {
                        DocsPaWR.ParametriRicercaRubrica qco = new DocsPAWA.DocsPaWR.ParametriRicercaRubrica();
                        DocsPAWA.UserManager.setQueryRubricaCaller(ref qco);
                        qco.codice = this.txt_codDest.Text.Trim();
                        qco.tipoIE = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
                        //cerco su tutti i tipi utente:
                        if (System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
                            qco.doListe = true;
                        DocsPaWebService ws = new DocsPaWebService();
                        if (ws.IsEnabledRF(string.Empty))
                            qco.doRF = true;
                        qco.doRuoli = true;
                        qco.doUtenti = true;
                        qco.doUo = true;
                        //query per codice  esatta, no like.
                        qco.queryCodiceEsatta = true;

                        DocsPaWR.RagioneTrasmissione rt = TrasmManager.getRagioneSel(this);
                        this.gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0, 1);

                        switch (this.gerarchia_trasm)
                        {
                            case "T":
                                qco.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_MODELLI_TRASM_ALL;
                                break;
                            case "I":
                                qco.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_MODELLI_TRASM_INF;
                                break;
                            case "S":
                                qco.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_MODELLI_TRASM_SUP;
                                break;
                            case "P":
                                qco.calltype = DocsPAWA.DocsPaWR.RubricaCallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO;
                                break;
                        }

                        string objtype = string.Empty;
                        //if (schedaDocumento != null)
                        //{
                        //objtype = schedaDocumento.tipoProto;
                        //qco.ObjectType = objtype;

                        DocsPaWR.ElementoRubrica[] corrSearch = UserManager.getElementiRubrica(this.Page, qco);
                        if (corrSearch != null && corrSearch.Length > 0)
                        {
                            //Verifica della disabilitazione alla ricezione delle trasmissioni
                            if (corrSearch[0].tipo == "R" && corrSearch[0].disabledTrasm)
                            {
                                RegisterStartupScript("noresults", "<script language=\"javascript\">alert(\"Il ruolo risulta disabilitato alla ricezione delle trasmissioni\");</script>");
                            }
                            else
                            {
                                this.ImpostaDestinatario(corrSearch, qco);
                                this.txt_codDest.Text = "";
                            }
                        }
                        else
                        {
                            RegisterStartupScript("noresults", "<script language=\"javascript\">alert(\"Nessun destinatario presente con il codice inserito\");</script>");
                        }

                        corrSearch = null;
                        //}

                        qco = null;
                        rt = null;
                    }
                }
            }
            catch
            {
                RegisterStartupScript("ErrResults", "<script language=\"javascript\">alert(\"Attenzione! si è verificato un errore nella ricerca del destinatario\");</script>");
            }
        }

        /// <summary>
        /// Imposta Destinatario
        /// </summary>
        /// <param name="corrSearch"></param>
        /// <param name="qco"></param>
        private void ImpostaDestinatario(DocsPAWA.DocsPaWR.ElementoRubrica[] corrSearch, DocsPAWA.DocsPaWR.ParametriRicercaRubrica prr)
        {
            string t_avviso = string.Empty;
            DocsPaWR.Corrispondente corr;

            // DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoInLavorazione(this.Page);
            DocsPaWR.Trasmissione trasmissione = TrasmManager.getGestioneTrasmissione(this.Page);

            // verifica liste di distribuzione
            if (corrSearch[0].tipo.Equals("L"))
            {
                if (System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1")
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    ArrayList listaCorr = UserManager.getCorrispondentiByCodLista(this.Page, prr.codice, idAmm);
                    if (listaCorr != null && listaCorr.Count > 0)
                    {
                        DocsPaWR.Trasmissione trasmissioni = TrasmManager.getGestioneTrasmissione(this.Page);
                        if (trasmissioni == null)
                            trasmissioni = new Trasmissione();

                        ElementoRubrica[] ers = new ElementoRubrica[listaCorr.Count];
                        ArrayList el = null;

                        if (Session["selDestDaRubrica"] != null)
                            el = (ArrayList)Session["selDestDaRubrica"];

                        ArrayList nuovaListaCorr = el;

                        for (int i = 0; i < listaCorr.Count; i++)
                        {
                            DocsPaWR.ElementoRubrica er_1 = new DocsPAWA.DocsPaWR.ElementoRubrica();
                            DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i];
                            er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                            ers[i] = er_1;

                            bool daInserire = true;
                            if (el != null)
                            {
                                foreach (Corrispondente corrisp in el)
                                {
                                    if (corrisp.systemId == c.systemId)
                                        daInserire = false;
                                }
                            }
                            else
                                if (nuovaListaCorr == null)
                                    nuovaListaCorr = new ArrayList();

                            if (daInserire)
                                nuovaListaCorr.Add(c);
                        }

                        DocsPAWA.DocsPaWR.ElementoRubrica[] elRub = new ElementoRubrica[nuovaListaCorr.Count];
                        for (int a = 0; a < nuovaListaCorr.Count; a++)
                        {
                            DocsPaWR.ElementoRubrica er_1 = new DocsPAWA.DocsPaWR.ElementoRubrica();
                            DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente)nuovaListaCorr[a];
                            er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                            elRub[a] = er_1;
                        }
                        Session.Add("selDestDaRubrica", elRub);

                        //int coutStartErs = ers.Length;
                        ////si filtrano i corrispondenti della lista per verificate la loro autorizzazione
                        ////per risoluzione del bug 2285
                        //ElementoRubrica[] ers_1 = UserManager.filtra_trasmissioniPerListe(this, prr, ers);

                        //for (int i = 0; i < listaCorr.Count; i++)
                        //{
                        //    DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i];
                        //    for (int j = 0; j < ers_1.Length; j++)
                        //    {
                        //        if (c.codiceRubrica == ers_1[j].codice)
                        //        {
                        //            trasmissioni = addTrasmissioneSingola(trasmissione, c);
                        //            break;
                        //        }
                        //    }
                        //}
                        //if (trasmissione.trasmissioniSingole == null || trasmissioni.trasmissioniSingole.Length != listaCorr.Count)
                        //{
                        //    RegisterStartupScript("avviso", "<script language=\"javascript\">alert (\"AVVISO: Nella lista ci sono corrispondenti ai quali non è possibile trasmettere !\");</script>");
                        //}

                        // TrasmManager.setGestioneTrasmissione(this.Page, trasmissioni);
                    }
                }
                else
                {
                    RegisterStartupScript("checkListe", "<script language=\"javascript\">alert(\"Attenzione! le liste di distribuzione non sono state attivate in questo sistema\");</script>");
                    return;
                }
            }
            else
                if (corrSearch[0].tipo.Equals("F"))
                {
                    DocsPaWebService ws = new DocsPaWebService();
                    if (ws.IsEnabledRF(string.Empty))
                    {
                        string idAmm = UserManager.getInfoUtente().idAmministrazione;
                        ArrayList listaCorr = UserManager.getCorrispondentiByCodRF(this.Page, prr.codice);
                        if (listaCorr != null && listaCorr.Count > 0)
                        {
                            DocsPaWR.Trasmissione trasmissioni = TrasmManager.getGestioneTrasmissione(this.Page);
                            if (trasmissioni == null)
                                trasmissioni = new Trasmissione();

                            ElementoRubrica[] ers = new ElementoRubrica[listaCorr.Count];
                            ArrayList el = null;

                            if (Session["selDestDaRubrica"] != null)
                                el = (ArrayList)Session["selDestDaRubrica"];

                            ArrayList nuovaListaCorr = el;

                            for (int i = 0; i < listaCorr.Count; i++)
                            {
                                DocsPaWR.ElementoRubrica er_1 = new DocsPAWA.DocsPaWR.ElementoRubrica();
                                DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente)listaCorr[i];
                                er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                                ers[i] = er_1;

                                bool daInserire = true;
                                if (el != null)
                                {
                                    foreach (Corrispondente corrisp in el)
                                    {
                                        if (corrisp.systemId == c.systemId)
                                            daInserire = false;
                                    }
                                }
                                else
                                    if (nuovaListaCorr == null)
                                        nuovaListaCorr = new ArrayList();

                                if (daInserire)
                                    nuovaListaCorr.Add(c);
                            }

                            DocsPAWA.DocsPaWR.ElementoRubrica[] elRub = new ElementoRubrica[nuovaListaCorr.Count];
                            for (int a = 0; a < nuovaListaCorr.Count; a++)
                            {
                                DocsPaWR.ElementoRubrica er_1 = new DocsPAWA.DocsPaWR.ElementoRubrica();
                                DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente)nuovaListaCorr[a];
                                er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                                elRub[a] = er_1;
                            }
                            Session.Add("selDestDaRubrica", elRub);
                        }
                    }
                    else
                    {
                        RegisterStartupScript("checkListe", "<script language=\"javascript\">alert(\"Attenzione! gli RF non sono previsti in questa configurazione\");</script>");
                        return;
                    }
                }
                else
                {
                    DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
                    qco.codiceRubrica = prr.codice;
                    qco.getChildren = false;
                    qco.idAmministrazione = UserManager.getInfoUtente(this.Page).idAmministrazione;
                    qco.tipoUtente = DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO;
                    qco.fineValidita = true;

                    corr = UserManager.getListaCorrispondenti(this.Page, qco)[0];
                    ArrayList el = null;

                    if (Session["selDestDaRubrica"] != null)
                        el = (ArrayList)Session["selDestDaRubrica"];

                    ArrayList nuovaListaCorr = el;
                    bool daInserire = true;
                    if (el != null)
                    {
                        foreach (Corrispondente c in el)
                        {
                            if (corr.systemId == c.systemId)
                                daInserire = false;
                        }
                    }
                    else
                        if (nuovaListaCorr == null)
                            nuovaListaCorr = new ArrayList();

                    if (daInserire)
                        nuovaListaCorr.Add(corr);

                    DocsPAWA.DocsPaWR.ElementoRubrica[] elRub = new ElementoRubrica[nuovaListaCorr.Count];
                    for (int a = 0; a < nuovaListaCorr.Count; a++)
                    {
                        DocsPaWR.ElementoRubrica er_1 = new DocsPAWA.DocsPaWR.ElementoRubrica();
                        DocsPaWR.Corrispondente c = (DocsPAWA.DocsPaWR.Corrispondente)nuovaListaCorr[a];
                        er_1 = UserManager.getElementoRubrica(this, c.codiceRubrica);
                        elRub[a] = er_1;
                    }
                    Session.Add("selDestDaRubrica", elRub);

                    //trasmissione = this.addTrasmissioneSingola(trasmissione, corr);
                }


            // TrasmManager.setGestioneTrasmissione(this.Page, trasmissione);

            //if (trasm_strutture_vuote != null && trasm_strutture_vuote.Count > 0)
            //{
            //    if (t_avviso == string.Empty)
            //    {
            //        foreach (string s in trasm_strutture_vuote)
            //            t_avviso += (" - " + s + "\\n");

            //        t_avviso = String.Format("alert (\"AVVISO: Impossibile effettuare la trasmissione a questa struttura perchè priva di utenti o ruoli di riferimento:\\n{0}\");", t_avviso);
            //    }
            //}

            RegisterStartupScript("re-submit", "<script>reloadPage();</script>");
        }


        /// <summary>
        /// Aggiunge Trasmissione Singola
        /// </summary>
        /// <param name="trasmissione"></param>
        /// <param name="corr"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr)
        {
            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
                {
                    DocsPaWR.TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaWR.TrasmissioneSingola trasmissioneSingola = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = "S";
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = TrasmManager.getRagioneSel(this.Page);

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPAWA.DocsPaWR.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr);
                if (listaUtenti.Length == 0)
                    trasmissioneSingola = null;

                //ciclo per utenti se dest è gruppo o ruolo
                for (int i = 0; i < listaUtenti.Length; i++)
                {
                    DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)listaUtenti[i];
                    //trasmissioneUtente.daNotificare = true;
                    trasmissioneUtente.daNotificare = TrasmManager.getTxRuoloUtentiChecked();
                    trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                }
            }

            if (corr is DocsPAWA.DocsPaWR.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)corr;
                //trasmissioneUtente.daNotificare = true;
                trasmissioneUtente.daNotificare = TrasmManager.getTxRuoloUtentiChecked();
                trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
            }

            if (corr is DocsPAWA.DocsPaWR.UnitaOrganizzativa)
            {
                DocsPaWR.UnitaOrganizzativa theUo = (DocsPAWA.DocsPaWR.UnitaOrganizzativa)corr;
                DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = UserManager.getRuolo();

                DocsPaWR.Ruolo[] ruoli = UserManager.getRuoliRiferimentoAutorizzati(this.Page, qca, theUo);
                foreach (DocsPAWA.DocsPaWR.Ruolo r in ruoli)
                    trasmissione = addTrasmissioneSingola(trasmissione, r);

                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);
            //else
            //{
            //    // In questo caso questa trasmissione non può avvenire perché la struttura non ha utenti
            //    trasm_strutture_vuote.Add(String.Format("{0} ({1})", corr.descrizione, corr.codiceRubrica));
            //}
            return trasmissione;
        }

        /// <summary>
        /// query Utenti
        /// </summary>
        /// <param name="corr"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Corrispondente[] queryUtenti(DocsPAWA.DocsPaWR.Corrispondente corr)
        {

            //costruzione oggetto queryCorrispondente
            DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;

            qco.idAmministrazione = UserManager.getInfoUtente(this.Page).idAmministrazione;
            qco.fineValidita = true;

            //corrispondenti interni
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;

            return UserManager.getListaCorrispondenti(this.Page, qco);
        }

        #endregion

        #region DATAGRID

        public void caricaDataGridModelli()
        {
            this.prmtRicerca.EnableEsportAndFindButtons = true;
            if (modelliTrasmissione != null && modelliTrasmissione.Count != 0)
            {
                //txt_ricerca.Focus();
                DataTable dt = new DataTable();
                dt.Columns.Add("SYSTEM_ID");
                dt.Columns.Add("CODICE");
                dt.Columns.Add("MODELLO");
                dt.Columns.Add("REGISTRO");
                dt.Columns.Add("TIPO DI TRASM.");
                dt.Columns.Add("VISIBILITA'");

                foreach (ModelloTrasmissione model in modelliTrasmissione)
                {
                    if (model.NumMittenti <= 1) //visualizzo solo i modelli con un mittente
                    {
                        DocsPaWR.Registro reg = null;
                        if (model.ID_REGISTRO != "0")
                        {
                            reg = UserManager.getRegistroBySistemId(this, model.ID_REGISTRO);
                        }
                        DataRow row = dt.NewRow();

                        row[0] = model.SYSTEM_ID;
                        //row[1] = modello.CODICE;
                        row[1] = this.GetSpanElement((model).Valid, model.CODICE);
                        //row[2] = modello.NOME;
                        row[2] = this.GetSpanElement((model).Valid, model.NOME);
                        if (reg != null)
                        {
                            row[3] = reg.descrizione;
                        }
                        else
                        {
                            row[3] = "";
                        }
                        if (model.CHA_TIPO_OGGETTO == "D")
                            row[4] = "Documento";
                        if (model.CHA_TIPO_OGGETTO == "F")
                            row[4] = "Fascicolo";
                        if (!string.IsNullOrEmpty(model.ID_PEOPLE))
                            row[5] = "Solo a me stesso";
                        else
                            row[5] = "A tutto il ruolo";
                        //if (modello.MITTENTE == null)
                        //    row[5] = "Solo a me stesso";
                        //else
                        //    row[5] = "A tutto il ruolo";
                        dt.Rows.Add(row);
                    }

                }
                //lbl_ricerca.Text = "";
                this.prmtRicerca.SearchResult = String.Empty;
                dt_listaModelli.DataSource = dt;
                dt_listaModelli.DataBind();
                dt_listaModelli.Visible = true;
            }
            else
            {
                this.prmtRicerca.EnableEsportAndFindButtons = false;
                dt_listaModelli.Visible = false;
                //messaggio nessun modello risponde alla ricerca effettuata
                //lbl_ricerca.Text = "Nessun modello per questa ricerca!";
                this.prmtRicerca.SearchResult = "Nessun modello per questa ricerca!";
            }
        }

        /// <summary>
        /// Funzione per la creazione di uno span element per codice e descrizione di un modello
        /// </summary>
        /// <param name="valid">True se il modello è valido</param>
        /// <param name="text">Testo da decorare</param>
        /// <returns>Elemento span contenete il testo text</returns>
        private String GetSpanElement(bool valid, String text)
        {
            // Span da utilizzare per la decorazione di codice e descrizione del destinatario
            String retVal = "<span style=\"color: {0}; {1}\">{2}</span>";

            // Se il modello non è valido, viene colorato in rosso altrimenti in nero
            if (valid)
                retVal = String.Format(retVal, "Black", String.Empty, text);
            else
                retVal = String.Format(retVal, "Red", String.Empty, text);

            return retVal;
        }

        /// <summary>
        /// Funzione per la costruzione di un elemento span in cui inserire codice e descrizione del corrispondente
        /// </summary>
        /// <param name="dest">Destinatario da verificare</param>
        /// <param name="text">Testo da inserire nell'elemento span</param>
        /// <returns>Elemento span con stile e testo impostati</returns>
        private String GetSpanElement(MittDest dest, String text)
        {
            // Valore da restituire
            String retVal = String.Empty;

            // Span da utilizzare per la decorazione di codice e descrizione del destinatario
            String formatString = "<span style=\"color: {0}; {1}\">{2}</span>";

            // Se il ruolo è inibito, viene colorato di rosso
            if (dest.Inhibited)
                retVal = String.Format("<span style=\"color:Red;\">{0}</span>", text);

            // Se il ruolo è disabilitato, viene visualizzato nero barrato
            if (dest.Disabled)
                retVal = String.Format("<span style=\"text-decoration: line-through;\">{0}</span>", String.IsNullOrEmpty(retVal) ? text : retVal);

            if (!dest.Disabled && !dest.Inhibited)
                retVal = String.Format("<span style=\"color:Black;\">{0}</span>", text);


            return retVal;
        }

        public void caricaDataGridDest()
        {
            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            DataTable dt = new DataTable();
            dt.Columns.Add("SYSTEM_ID");
            dt.Columns.Add("RAGIONE");
            dt.Columns.Add("VAR_COD_RUBRICA");
            dt.Columns.Add("VAR_DESC_CORR");
            dt.Columns.Add("ID_RAGIONE");
            dt.Columns.Add("NASCONDI_VERSIONI_PRECEDENTI", typeof(Boolean));

            if (modello.RAGIONI_DESTINATARI != null)
            {
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {
                    for (int j = 0; j < modello.RAGIONI_DESTINATARI[i].DESTINATARI.Length; j++)
                    {
                        DataRow row = dt.NewRow();
                        DocsPaWR.RagioneDest rd = modello.RAGIONI_DESTINATARI[i];
                        if (j == 0)
                            row[1] = rd.RAGIONE;
                        DocsPaWR.MittDest mittDest = modello.RAGIONI_DESTINATARI[i].DESTINATARI[j];
                        row[0] = mittDest.SYSTEM_ID;
                        //row[2] = mittDest.VAR_COD_RUBRICA;
                        row[2] = this.GetSpanElement(mittDest, mittDest.VAR_COD_RUBRICA);
                        if (!string.IsNullOrEmpty(mittDest.DESCRIZIONE))
                        {
                            //row[3] = mittDest.DESCRIZIONE;
                            row[3] = this.GetSpanElement(mittDest, mittDest.DESCRIZIONE);
                        }
                        else
                        {
                            if (mittDest.CHA_TIPO_MITT_DEST == "UT_P")
                                row[3] = "Utente proprietario";
                            if (mittDest.CHA_TIPO_MITT_DEST == "R_P")
                                row[3] = "Ruolo proprietario";
                            if (mittDest.CHA_TIPO_MITT_DEST == "UO_P")
                                row[3] = "UO proprietaria";
                            if (mittDest.CHA_TIPO_MITT_DEST == "RSP_P")
                                row[3] = "Resp. UO proprietaria";
                        }
                        row[4] = mittDest.ID_RAGIONE;
                        row["NASCONDI_VERSIONI_PRECEDENTI"] = mittDest.NASCONDI_VERSIONI_PRECEDENTI;

                        dt.Rows.Add(row);
                    }
                }
            }
            dt_dest.DataSource = dt;
            dt_dest.DataBind();

            dt_dest.Visible = false;
            this.btn_pp_notifica.Visible = false;

            if (dt_dest.Items.Count != 0)
            {
                int k = 0;
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {
                    for (int j = 0; j < modello.RAGIONI_DESTINATARI[i].DESTINATARI.Length; j++)
                    {

                        DocsPaWR.RagioneDest rd = modello.RAGIONI_DESTINATARI[i];
                        DocsPaWR.MittDest mittDest = modello.RAGIONI_DESTINATARI[i].DESTINATARI[j];

                        string imgurl = "../images/rubrica/";
                        switch (mittDest.CHA_TIPO_URP)
                        {
                            case "U":
                                ((ImageButton)dt_dest.Items[k].Cells[2].Controls[1]).ImageUrl = imgurl + "U_NOEXP.gif";
                                ((ImageButton)dt_dest.Items[k].Cells[2].Controls[1]).ToolTip = "Ufficio";

                                break;

                            case "R":
                                ((ImageButton)dt_dest.Items[k].Cells[2].Controls[1]).ImageUrl = imgurl + "R_NOEXP.gif";
                                ((ImageButton)dt_dest.Items[k].Cells[2].Controls[1]).ToolTip = "Ruolo";
                                break;

                            case "P":
                                ((ImageButton)dt_dest.Items[k].Cells[2].Controls[1]).ImageUrl = imgurl + "P_NOEXP.gif";
                                ((ImageButton)dt_dest.Items[k].Cells[2].Controls[1]).ToolTip = "Utente";
                                break;
                        }

                        ((TextBox)dt_dest.Items[k].Cells[6].Controls[1]).Text = mittDest.VAR_NOTE_SING;
                        if (mittDest.CHA_TIPO_TRASM == "S")
                            ((DropDownList)dt_dest.Items[k].Cells[5].Controls[1]).SelectedIndex = 0;
                        if (mittDest.CHA_TIPO_TRASM == "T")
                            ((DropDownList)dt_dest.Items[k].Cells[5].Controls[1]).SelectedIndex = 1;
                        if (!modello.RAGIONI_DESTINATARI[i].CHA_TIPO_RAGIONE.Equals("W"))
                        {
                            //old: ((DropDownList)dt_dest.Items[k].Cells[5].Controls[1]).Visible = false;


                            ((TextBox)dt_dest.Items[k].Cells[7].Controls[3]).ReadOnly = true;
                            ((TextBox)dt_dest.Items[k].Cells[7].Controls[3]).BackColor = Color.Gray;
                        }
                        else
                        {
                            if (mittDest.SCADENZA != 0)
                                ((TextBox)dt_dest.Items[k].Cells[7].Controls[3]).Text = mittDest.SCADENZA.ToString();
                            else
                                ((TextBox)dt_dest.Items[k].Cells[7].Controls[3]).Text = "";
                        }
                        k++;
                    }
                }
                dt_dest.Visible = true;

                //this.gestioneTastoNotifiche(true);                
                this.gestioneTastoNotifiche((modello.SYSTEM_ID > 0));
            }
        }

        private void addDestSelDaRubrica(ElementoRubrica[] selDestDaRubrica)
        {
            if (selDestDaRubrica != null)
            {
                string ragione = this.ddl_ragioni.SelectedItem.Text;
                ArrayList destinatari = new ArrayList();
                for (int i = 0; i < selDestDaRubrica.Length; i++)
                {
                    DocsPaWR.ElementoRubrica el = (DocsPAWA.DocsPaWR.ElementoRubrica)selDestDaRubrica[i];
                    if (el.tipo.Equals("L"))
                    {
                        string idAmm = UserManager.getInfoUtente().idAmministrazione;
                        ArrayList lista = UserManager.getCorrispondentiByCodLista(this.Page, el.codice, idAmm);
                        foreach (Corrispondente corr in lista)
                        {
                            DocsPaWR.MittDest destinatario = new DocsPAWA.DocsPaWR.MittDest();
                            destinatario.CHA_TIPO_MITT_DEST = "D";
                            destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                            destinatario.VAR_COD_RUBRICA = corr.codiceRubrica;
                            destinatario.DESCRIZIONE = corr.descrizione;
                            destinatario.CHA_TIPO_URP = corr.tipoCorrispondente;

                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(corr.systemId);

                            destinatari.Add(destinatario);
                        }
                    }
                    else if (el.tipo.Equals("F"))
                    {
                        ArrayList lista = UserManager.getCorrispondentiByCodRF(this.Page, el.codice);
                        foreach (Corrispondente corr in lista)
                        {
                            DocsPaWR.MittDest destinatario = new DocsPAWA.DocsPaWR.MittDest();
                            destinatario.CHA_TIPO_MITT_DEST = "D";
                            destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                            destinatario.VAR_COD_RUBRICA = corr.codiceRubrica;
                            destinatario.DESCRIZIONE = corr.descrizione;
                            destinatario.CHA_TIPO_URP = corr.tipoCorrispondente;

                            destinatario.ID_CORR_GLOBALI = Convert.ToInt32(corr.systemId);

                            destinatari.Add(destinatario);
                        }
                    }
                    else
                    {
                        DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this, el.codice, el.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);

                        DocsPaWR.MittDest destinatario = new DocsPAWA.DocsPaWR.MittDest();
                        destinatario.CHA_TIPO_MITT_DEST = "D";
                        destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                        destinatario.VAR_COD_RUBRICA = el.codice;
                        destinatario.DESCRIZIONE = el.descrizione;
                        destinatario.CHA_TIPO_URP = el.tipo;
                        destinatario.ID_CORR_GLOBALI = Convert.ToInt32(corr.systemId);

                        destinatari.Add(destinatario);
                    }
                }
                Session.Remove("selDestDaRubrica");
                AggiornaRagioneDest();
                AddMittDest(ragione, destinatari, cha_tipo_ragione);
                caricaDataGridDest();
            }
        }

        private void AggiornaRagioneDest()
        {
            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            if (dt_dest != null && dt_dest.Items != null && dt_dest.Items.Count > 0)
            {

                int n = 0;
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {
                    for (int j = 0; j < modello.RAGIONI_DESTINATARI[i].DESTINATARI.Length; j++)
                    {
                        ((DocsPAWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]).CHA_TIPO_TRASM = ((DropDownList)dt_dest.Items[n].Cells[5].Controls[1]).SelectedValue;
                        ((DocsPAWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]).VAR_NOTE_SING = ((TextBox)dt_dest.Items[n].Cells[6].Controls[1]).Text;
                        if (((TextBox)dt_dest.Items[n].Cells[7].Controls[3]).Text != "")
                            ((DocsPAWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]).SCADENZA = Convert.ToInt32(((TextBox)dt_dest.Items[n].Cells[7].Controls[3]).Text);
                        else
                            ((DocsPAWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]).SCADENZA = 0;

                        CheckBox chkNascondiVersioniPrecedentiDocumento = dt_dest.Items[n].FindControl("chkNascondiVersioniPrecedentiDocumento") as CheckBox;
                        if (chkNascondiVersioniPrecedentiDocumento != null)
                            ((DocsPAWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]).NASCONDI_VERSIONI_PRECEDENTI = chkNascondiVersioniPrecedentiDocumento.Checked;
                        else
                            ((DocsPAWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]).NASCONDI_VERSIONI_PRECEDENTI = false;

                        n++;
                    }
                }
                Session.Add("Modello", modello);
            }

        }

        private void AddMittDest(string ragione, ArrayList destinatari, string cha_tipo_ragione)
        {
            ArrayList array_1;
            DocsPaWR.ModelloTrasmissione modello = ((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]);

            //controllo se esiste già una ragioneDest
            if (((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI != null)
            {
                //controllo se esiste per la ragione
                DocsPaWR.RagioneDest rd = null;
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {
                    if (((DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i]).RAGIONE.Equals(ragione))
                    {
                        rd = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                        break;
                    }
                }

                //se esiste già una ragione aggiungo i destinatari alla stessa
                //if(((DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i]).RAGIONE.Equals(ragione))
                if (rd != null)
                {
                    array_1 = new ArrayList(rd.DESTINATARI);
                    verificaEsistenzaDest(array_1, ref destinatari);
                    array_1.AddRange(destinatari);
                    rd.DESTINATARI = new DocsPAWA.DocsPaWR.MittDest[array_1.Count];
                    array_1.CopyTo(rd.DESTINATARI);
                }
                else
                {
                    DocsPaWR.RagioneDest rd1 = new DocsPAWA.DocsPaWR.RagioneDest();
                    rd1.RAGIONE = ragione;
                    rd1.CHA_TIPO_RAGIONE = cha_tipo_ragione;
                    array_1 = new ArrayList();
                    array_1.AddRange(destinatari);
                    rd1.DESTINATARI = new DocsPAWA.DocsPaWR.MittDest[array_1.Count];
                    array_1.CopyTo(rd1.DESTINATARI);

                    ArrayList array_2;
                    if (((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI != null)
                        array_2 = new ArrayList(((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI);
                    else
                        array_2 = new ArrayList();

                    array_2.Add(rd1);
                    ((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI = new DocsPAWA.DocsPaWR.RagioneDest[array_2.Count];
                    array_2.CopyTo(((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI);
                }

            }
            else
            {
                DocsPaWR.RagioneDest rd1 = new DocsPAWA.DocsPaWR.RagioneDest();
                rd1.RAGIONE = ragione;
                rd1.CHA_TIPO_RAGIONE = cha_tipo_ragione;
                array_1 = new ArrayList();
                array_1.AddRange(destinatari);
                rd1.DESTINATARI = new DocsPAWA.DocsPaWR.MittDest[array_1.Count];
                array_1.CopyTo(rd1.DESTINATARI);

                ArrayList array_2 = new ArrayList();
                array_2.Add(rd1);
                ((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI = new DocsPAWA.DocsPaWR.RagioneDest[array_2.Count];
                array_2.CopyTo(((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).RAGIONI_DESTINATARI);
            }

            Session.Add("Modello", modello);
        }


        private void verificaEsistenzaDest(ArrayList destVecchi, ref ArrayList destNuovi)
        {
            ArrayList elNuovi_1 = new ArrayList();
            bool result = true;
            for (int i = 0; i < destNuovi.Count; i++)
            {
                for (int j = 0; j < destVecchi.Count; j++)
                {
                    if (((DocsPAWA.DocsPaWR.MittDest)destNuovi[i]).VAR_COD_RUBRICA == ((DocsPAWA.DocsPaWR.MittDest)destVecchi[j]).VAR_COD_RUBRICA)
                        result = false;
                }
                if (result)
                {
                    elNuovi_1.Add(destNuovi[i]);
                }
                result = true;
            }
            destNuovi.Clear();
            destNuovi.AddRange(elNuovi_1);
        }

        #endregion

        #region SetFocus
        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
            RegisterStartupScript("focus", s);
        }
        #endregion SetFocus

        #region Utils

        private void pulisciCampi()
        {
            this.txt_nomeModello.Text = "";
            this.txt_noteGenerali.Text = "";
            this.ddl_ragioni.SelectedIndex = 0;
            //this.ddl_registri.SelectedIndex = 0;
            this.ddl_tipoTrasmissione.SelectedIndex = 0;
        }

        private string CheckFields()
        {
            string msg = string.Empty;

            if (this.txt_nomeModello.Text.Trim() == "")
            {
                msg = "Inserire il nome del modello.";
                SetFocus(this.txt_nomeModello);
                return msg;
            }

            if (this.dt_dest != null && this.dt_dest.Items.Count == 0)
            {
                msg = "Inserire almeno un destinatario del modello.";
                return msg;
            }

            for (int i = 0; i < dt_dest.Items.Count; i++)
            {
                if (!((TextBox)dt_dest.Items[i].Cells[7].Controls[3]).ReadOnly && ((TextBox)dt_dest.Items[i].Cells[7].Controls[3]).Text != "")
                {
                    try
                    {
                        int giorniScadenza = Convert.ToInt32(((TextBox)dt_dest.Items[i].Cells[7].Controls[3]).Text);
                    }
                    catch (Exception e)
                    {
                        msg = "I giorni di scadenza devono essere un numero intero.";
                        return msg;
                    }
                }
            }

            msg = this.checkRuoliUtentiDuplicati();
            if (msg != string.Empty)
            {
                msg = "Non è possibile inserire gli stessi destinatari con ragioni di trasmissione diverse: \\n" + msg;
                return msg;
            }

            msg = this.checkRagTrasmConCessioneDuplicate();
            if (msg != string.Empty)
            {
                msg = "Non è possibile inserire più ragioni di trasmissione che prevedono cessione: \\n" + msg;
                return msg;
            }

            msg = this.checkModUOConCessione();
            if (msg != string.Empty)
            {
                msg = "Non è possibile inserire ragioni che prevedono cessione se i destinatari sono UO: \\n" + msg;
                return msg;
            }

            string valoreChiave = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente(this).idAmministrazione, "FE_MAX_LENGTH_DESC_TRASM");
            if (this.txt_noteGenerali.Text.Length > Convert.ToInt32(valoreChiave))
                msg = "La lunghezza massima prevista per le note è di " + valoreChiave + " caratteri!";
            return msg;
        }

        private void ShowErrorMessage(string errorMessage)
        {
            this.RegisterClientScript("ErrorMessage", "alert('" + errorMessage.Replace("'", "\\'") + "')");
        }

        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        #endregion utils

        private void CaricaComboRegistri(DropDownList ddl)
        {
            userRegistri = UserManager.getListaRegistri(this);

            if (userRegistri.Length > 1)
            {
                ListItem it = new ListItem("", "");
                this.ddl_registri.Items.Add(it);

                foreach (DocsPAWA.DocsPaWR.Registro registro in userRegistri)
                {
                    ListItem item = new ListItem(registro.codRegistro, registro.systemId);
                    this.ddl_registri.Items.Add(item);
                }
            }
            else
            {
                this.ddl_registri.Visible = false;

                this.lbl_registri.Text = userRegistri[0].descrizione;
                this.lbl_registri.Visible = true;
            }
        }

        public void CaricaRagioni(string idAmm, bool all)
        {
            try
            {
                listaRagioni = DocsPAWA.AdminTool.Manager.ModelliTrasmManager.getlistRagioniTrasm(idAmm, all, this.ddl_tipoTrasmissione.Text);
                if (listaRagioni != null && listaRagioni.Length > 0)
                {
                    int selezione = ddl_ragioni.SelectedIndex;
                    ddl_ragioni.Items.Clear();
                    for (int i = 0; i < listaRagioni.Length; i++)
                    {
                        ListItem newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                        ddl_ragioni.Items.Add(newItem);
                    }

                    DocsPaWR.RagioneTrasmissione ragioneSel = new DocsPAWA.DocsPaWR.RagioneTrasmissione();
                    if (selezione >= listaRagioni.Length)
                    {
                        ListItem item = new ListItem("", "0");
                        if (!ddl_ragioni.Items.Contains(item))
                            ddl_ragioni.Items.Add(item);
                        selezione = ddl_ragioni.Items.IndexOf(ddl_ragioni.Items.FindByValue("0"));
                        ragioneSel = listaRagioni[0];
                    }
                    else
                    {
                        ragioneSel = listaRagioni[this.ddl_ragioni.SelectedIndex];
                    }

                    ddl_ragioni.SelectedIndex = selezione;

                    TrasmManager.setRagioneSel(this, ragioneSel);
                    DocsPaWR.RagioneTrasmissione rt = TrasmManager.getRagioneSel(this);
                    this.gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0, 1);
                    this.cha_tipo_ragione = rt.tipo;
                }

            }
            catch
            {
                this.ShowErrorMessage("Si è verificato un errore durante il caricamento delle ragioni di trasmissione.");
            }
        }

        private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.ddl_registri.SelectedIndex != 0)
            {
                DocsPaWebService ws = new DocsPaWebService();
                DocsPaWR.Registro reg = ws.GetRegistroBySistemId(this.ddl_registri.SelectedValue);
                if (reg.Sospeso)
                {
                    RegisterClientScript("alertRegistroSospeso", "alert('Il registro selezionato è sospeso!');");
                    this.ddl_registri.SelectedIndex = 0;
                    return;
                }
            }

            if (dt_dest.Items.Count != 0)
            {
                msg_ConfirmDel.Confirm("Attenzione. La seguente modifica comporta la perdita dei dati finora inseriti.");
                DocsPaWR.Registro reg = new Registro();
                if (Session["userRegistro"] != null)
                    reg = (DocsPAWA.DocsPaWR.Registro)Session["userRegistro"];

                reg.systemId = this.ddl_registri.SelectedValue.ToString();
                reg.codRegistro = this.ddl_registri.SelectedItem.Text.ToString();
                Session.Add("userRegistro", reg);
                return;
            }

            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            modello.ID_AMM = idAmministrazione;
            modello.NOME = this.txt_nomeModello.Text;
            modello.VAR_NOTE_GENERALI = this.txt_noteGenerali.Text;
            modello.CHA_TIPO_OGGETTO = this.ddl_tipoTrasmissione.SelectedValue;
            modello.ID_REGISTRO = this.ddl_registri.SelectedValue;
            modello.CODICE = this.txt_codModello.Text;
            Session.Add("Modello", modello);
            AggiornaRagioneDest();

            DocsPaWR.Registro registro = new Registro();
            if (Session["userRegistro"] != null)
                registro = (DocsPAWA.DocsPaWR.Registro)Session["userRegistro"];

            registro.systemId = this.ddl_registri.SelectedValue.ToString();
            registro.codRegistro = this.ddl_registri.SelectedItem.Text.ToString();
            Session.Add("userRegistro", registro);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddl_tipoTrasmissione_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.EnableColumnNascondiVersioni(this.ddl_tipoTrasmissione.SelectedValue == "D");

            this.caricaDataGridDest();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        protected void EnableColumnNascondiVersioni(bool enabled)
        {
            if (!enabled)
            {
                int index = -1;
                int i = 0;

                foreach (DataGridColumn col in this.dt_dest.Columns)
                {
                    if (col.HeaderText == "Nasc. vers.")
                    {
                        index = i;
                        break;
                    }

                    i++;
                }

                if (index > -1)
                {
                    this.dt_dest.Columns.RemoveAt(index);
                }
            }
        }

        protected void Grid_OnItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

        private void dt_listaModelli_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_confirmDel")).Value == "si")
            {
                int indx = e.Item.ItemIndex;
                string idModello = dt_listaModelli.Items[indx].Cells[0].Text;
                UserManager.cancellaModello(this, idAmministrazione, idModello);
                dt_listaModelli.CurrentPageIndex = 0;

                modelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.getInfoUtente(this), this.prmtRicerca.CreateSearchFilters());
                caricaDataGridModelli();
                ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_confirmDel")).Value = "";
            }
        }

        private void btn_nuovoModello_Click(object sender, System.EventArgs e)
        {
            pulisciCampi();
            this.Panel_ListaModelli.Visible = false;
            this.Panel_NuovoModello.Visible = true;
            this.lbl_registro_obb.Visible = true;
            this.lbl_registri.Visible = true;
            this.Panel_dest.Visible = true;
            this.btn_salvaModello.Visible = true;
            this.btn_lista_modelli.Visible = true;
            ModelloTrasmissione = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
            ModelloTrasmissione.ID_AMM = idAmministrazione;
            //DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPaWebService();
            //string systemIdModello = wws.getModelloSystemId();
            //this.txt_codModello.Text = "MT_" + systemIdModello;
            this.txt_codModello.Visible = false;
            this.lbl_codice.Visible = false;

            SetFocus(this.txt_nomeModello);
            Session.Add("Modello", ModelloTrasmissione);

            this.EnableColumnNascondiVersioni(this.ddl_tipoTrasmissione.SelectedValue == "D");

            caricaDataGridDest();

            // imposta una sessione per mantenere lo stato di "bisogna SALVARE"
            bool modelloToSave = true;
            Session.Add("modelloToSave", modelloToSave);

            // imposta una sessione per mantenere lo stato di "bisogna IMPOSTARE le notifiche"
            bool impostaNotifiche = true;
            Session.Add("impostaNotifiche", impostaNotifiche);
            this.lbl_stato.Text = "Nuovo";

            this.gestioneTastoNotifiche(false);
            // this.hd_multimittente.Value = string.Empty;


        }

        private void btn_Rubrica_dest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            userRegistri = UserManager.getListaRegistri(this);
            Session["noRagioneTrasmNullaFE"] = true;
            if (string.IsNullOrEmpty(ddl_ragioni.SelectedItem.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "No_Modello_Trasm", "alert('Attenzione, selezionare prima una ragione di trasmissione')", true);
            }
            else
            {
                RegisterClientScript("apriRubrica", "ApriRubricaTrasm('" + ddl_ragioni.SelectedItem.Text + "','A');");
            }
        }

        private void btn_lista_modelli_Click(object sender, System.EventArgs e)
        {
            dt_listaModelli.CurrentPageIndex = 0;
            modelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.getInfoUtente(this), this.prmtRicerca.CreateSearchFilters());
            caricaDataGridModelli();
            pulisciCampi();
            this.btn_salvaModello.Visible = false;
            this.Panel_NuovoModello.Visible = false;
            this.btn_lista_modelli.Visible = false;
            this.Panel_dest.Visible = false;
            this.Panel_ListaModelli.Visible = true;
            dt_listaModelli.SelectedIndex = -1;

            Session.Remove("modelloToSave");

            Session.Remove("impostaNotifiche");
        }

        private void btn_ricerca_Click(object sender, System.EventArgs e)
        {
            dt_listaModelli.CurrentPageIndex = 0;
            //if (txt_ricercaCodice.Text == string.Empty || txt_ricercaCodice.Text.StartsWith("MT_"))
            //{
            modelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.getInfoUtente(this), this.prmtRicerca.CreateSearchFilters());
            caricaDataGridModelli();
            if (Panel_ListaModelli.Visible == false)
                btn_lista_modelli_Click(null, null);
            //}
            //else
            //    RegisterClientScript("alertRicercaCodice", "<script>alert('Valore non corretto per il campo Codice');</script>");
        }

        public void tastoRicerca()
        {
            //Utils.DefaultButton(this, ref txt_ricerca, ref btn_ricerca);
        }

        private void txt_nomeModello_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void btn_salvaModello_Click(object sender, System.EventArgs e)
        {
            string msgErr = CheckFields();
            string codModello = string.Empty;
            if (msgErr != null && !msgErr.Equals(String.Empty))
            {
                this.RegisterClientScript("message", "alert('" + msgErr.Replace("'", "\\'") + "')");
            }
            else
            {
                AggiornaModello();

                //bool isModelloUnico = wws.isUniqueCodModelloTrasm((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]);
                //if (isModelloUnico)
                //{
                AggiornaRagioneDest();
                DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

                if (this.NotificheUtDaImpostate() && !this.unicaUOinModello())
                {
                    bool continua = true;
                    DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPaWebService();
                    //verifica che sia obbligatorio la selezione del registro (se previsto da chiave di configurazione)
                    string regObbl = wws.getSeRegistroObbl(UserManager.getInfoUtente().idAmministrazione, "REG_OBBL_IN_MODELLO");
                    if (!String.IsNullOrEmpty(regObbl) && regObbl.Equals("1"))
                        if (this.ddl_registri.SelectedIndex == 0)
                            continua = false;

                    if (continua)
                        RegisterClientScript("noNotifiche", "apriModaleNotifiche('INSERT');");
                    else
                        this.RegisterClientScript("message", "alert('La scelta del registro è obbligatoria per la creazione del modello di trasmissione!')");
                }
                else
                {
                    this.PerformSaveModel();

                    //DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPaWebService();
                    //DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

                    //if (rbl_share.Items[0].Selected)
                    //{?
                    //    modello.MITTENTE.ID_CORR_GLOBALI = 0;
                    //    modello.ID_PEOPLE = UserManager.getInfoUtente().idPeople;
                    //}
                    //else
                    //{
                    //    modello.ID_PEOPLE = "";?
                    //}

                    //DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
                    //wws.salvaModello(modello, infoUtente);
                    //Session.Remove("Modello");

                    //dt_listaModelli.CurrentPageIndex = 0;
                    //modelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.getInfoUtente(this), txt_ricerca.Text, txt_ricercaCodice.Text);
                    //caricaDataGridModelli();
                    //pulisciCampi();
                    //this.btn_salvaModello.Visible = false;
                    //this.btn_lista_modelli.Visible = false;
                    //this.Panel_NuovoModello.Visible = false;
                    //this.Panel_dest.Visible = false;
                    //this.Panel_ListaModelli.Visible = true;
                    //dt_listaModelli.SelectedIndex = -1;

                    //Session.Remove("modelloToSave");
                }

                //}
                //else
                //{
                //    this.RegisterClientScript("Codice Modello Esistente", "alert('Esiste già un modello trasmissione con tale codice!');");
                //}
            }
        }

        private void PerformSaveModel()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPaWebService();
            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            if (rbl_share.Items[0].Selected)
            {
                modello.MITTENTE[0].ID_CORR_GLOBALI = 0;
                modello.ID_PEOPLE = UserManager.getInfoUtente().idPeople;
            }
            else
            {
                modello.ID_PEOPLE = "";
                modello.MITTENTE[0].ID_CORR_GLOBALI = Convert.ToInt32(UserManager.getRuolo(this).systemId);// Convert.ToInt32(UserManager.getRuolo() UserManager.getInfoUtente().idCorrGlobali);
            }

            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
            wws.salvaModello(modello, infoUtente);
            Session.Remove("Modello");

            dt_listaModelli.CurrentPageIndex = 0;
            modelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.getInfoUtente(this), this.prmtRicerca.CreateSearchFilters());
            caricaDataGridModelli();
            pulisciCampi();
            this.btn_salvaModello.Visible = false;
            this.btn_lista_modelli.Visible = false;
            this.Panel_NuovoModello.Visible = false;
            this.Panel_dest.Visible = false;
            this.Panel_ListaModelli.Visible = true;
            dt_listaModelli.SelectedIndex = -1;

            Session.Remove("modelloToSave");
        }

        private void AggiornaModello()
        {
            //aggiorno i dati generali
            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            modello.ID_AMM = idAmministrazione;
            userRegistri = UserManager.getListaRegistri(this);

            if (modello.ID_REGISTRO != "0")
            {
                if (userRegistri.Length > 1)
                    modello.ID_REGISTRO = this.ddl_registri.SelectedValue;
                else
                    modello.ID_REGISTRO = userRegistri[0].systemId;
            }

            modello.NOME = this.txt_nomeModello.Text;
            modello.VAR_NOTE_GENERALI = this.txt_noteGenerali.Text;
            modello.CHA_TIPO_OGGETTO = this.ddl_tipoTrasmissione.SelectedValue;
            modello.SINGLE = "0";
            modello.ID_PEOPLE = utente.idPeople;
            modello.CODICE = this.txt_codModello.Text;

            Session.Add("Modello", modello);
        }

        private void dt_listaModelli_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //this.hd_multimittente.Value = string.Empty;
            int indx = dt_listaModelli.SelectedIndex;
            string idModello = dt_listaModelli.Items[indx].Cells[0].Text;
            DocsPaWR.ModelloTrasmissione modelloTrasmSel = UserManager.getModelloByID(this, idAmministrazione, idModello);
            Session.Add("Modello", modelloTrasmSel);
            caricaValoriGen();

            userRegistri = UserManager.getListaRegistri(this);
            if (userRegistri.Length > 1 && ddl_registri.SelectedValue == "")
            {
                if (modelloTrasmSel.ID_REGISTRO != "0")
                {
                    RegisterStartupScript("alert", "<script>alert('Con questo ruolo non è possibile modificare il modello selezionato.');</script>");
                    return;
                }
            }
            caricaDataGridDest();
            this.Panel_ListaModelli.Visible = false;
            this.btn_salvaModello.Visible = true;
            this.btn_lista_modelli.Visible = true;
            this.Panel_NuovoModello.Visible = true;
            this.Panel_dest.Visible = true;
            this.lbl_codice.Visible = true;
            this.txt_codModello.Visible = true;

            // imposta una sessione per mantenere lo stato di "bisogna IMPOSTARE le notifiche"
            bool impostaNotifiche = true;
            Session.Add("impostaNotifiche", impostaNotifiche);
            this.lbl_stato.Text = "Modifica";
            this.gestioneTastoNotifiche(true);

            //if (modelloTrasmSel.MITTENTE.Length > 1)
            //    this.hd_multimittente.Value = "1";
        }

        public void caricaValoriGen()
        {
            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            this.txt_nomeModello.Text = modello.NOME;
            this.txt_noteGenerali.Text = modello.VAR_NOTE_GENERALI;
            if (modello.CHA_TIPO_OGGETTO != null && modello.CHA_TIPO_OGGETTO.Equals("D"))
                ddl_tipoTrasmissione.SelectedIndex = 0;
            else
                ddl_tipoTrasmissione.SelectedIndex = 1;

            if (modello.CHA_TIPO_OGGETTO != null && modello.CHA_TIPO_OGGETTO.Equals("D"))
            {
                ddl_tipoTrasmissione.SelectedIndex = 0;
                this.EnableColumnNascondiVersioni(true);
            }
            else
            {
                ddl_tipoTrasmissione.SelectedIndex = 1;
                this.EnableColumnNascondiVersioni(false);
            }

            //if(modello.MITTENTE == null)
            //    rbl_share.SelectedIndex = 0;
            //else
            //    rbl_share.SelectedIndex = 1;
            if (!string.IsNullOrEmpty(modello.ID_PEOPLE))
                rbl_share.SelectedIndex = 0;
            else
                rbl_share.SelectedIndex = 1;

            if (modello.CODICE != null)
            {
                this.txt_codModello.Text = modello.CODICE;
                this.txt_codModello.Enabled = false;
            }

            CaricaRagioni(idAmministrazione, false);

            userRegistri = UserManager.getListaRegistri(this);
            if (userRegistri.Length > 1)
            {
                for (int i = 0; i < ddl_registri.Items.Count; i++)
                {
                    if (ddl_registri.Items[i].Value == modello.ID_REGISTRO)
                    {
                        ddl_registri.SelectedIndex = i;
                        DocsPaWR.Registro reg = new DocsPAWA.DocsPaWR.Registro();
                        if (Session["userRegistro"] != null)
                            reg = (DocsPAWA.DocsPaWR.Registro)Session["userRegistro"];
                        reg.systemId = this.ddl_registri.SelectedValue.ToString();
                        reg.codRegistro = this.ddl_registri.SelectedItem.Text.ToString();
                        Session.Add("userRegistro", reg);
                    }
                    if (modello.ID_REGISTRO == "0")
                    {
                        ddl_registri.SelectedIndex = 0;
                    }
                }
            }
            else
            {
                if (modello.ID_REGISTRO == "0")
                {
                    //ddl_registri.SelectedIndex = 0;
                    this.lbl_registri.Visible = false;
                    this.lbl_registro_obb.Visible = false;
                }
                else
                {
                    this.ddl_registri.Visible = false;
                    DocsPaWebService ws = new DocsPaWebService();
                    Registro reg = ws.GetRegistroBySistemId(modello.ID_REGISTRO);
                    this.lbl_registri.Text = reg.descrizione;
                    this.lbl_registri.Visible = true;
                }
            }
        }

        private void dt_dest_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_confirmDel")).Value == "si")
            {
                int indx = e.Item.ItemIndex;
                string idragione = dt_dest.Items[indx].Cells[10].Text;
                string var_cod_rubrica = dt_dest.Items[indx].Cells[3].Text;

                // Se cerchi di cancellare un destinatario cliccando sul bidoncino non succede nulla perché il codice
                // che arriva contiene anche i tag html span di formattazione.
                XElement elem = XElement.Parse(var_cod_rubrica);
                var_cod_rubrica = elem.Value;


                ((System.Web.UI.HtmlControls.HtmlInputHidden)this.FindControl("txt_confirmDel")).Value = "";
                if (var_cod_rubrica.Equals("&nbsp;"))
                    var_cod_rubrica = var_cod_rubrica.Replace("&nbsp;", "");
                CancellaDestinatario(idragione, var_cod_rubrica);
                caricaDataGridDest();
            }
        }

        private void CancellaDestinatario(string idragione, string var_cod_rubrica)
        {
            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
            {
                DocsPaWR.RagioneDest rd = modello.RAGIONI_DESTINATARI[i];
                for (int j = 0; j < modello.RAGIONI_DESTINATARI[i].DESTINATARI.Length; j++)
                {
                    DocsPaWR.MittDest dest = ((DocsPAWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]);

                    if (dest.ID_RAGIONE.ToString().Equals(idragione) && dest.VAR_COD_RUBRICA.Equals(var_cod_rubrica))
                    {

                        if (rd.DESTINATARI.Length > 1)
                        {
                            ArrayList appoggio = new ArrayList(rd.DESTINATARI);
                            appoggio.RemoveAt(j);
                            rd.DESTINATARI = new DocsPAWA.DocsPaWR.MittDest[appoggio.Count];
                            appoggio.CopyTo(rd.DESTINATARI);
                            Session.Add("Modello", modello);
                            return;
                        }
                        else
                        {
                            ArrayList appoggio = new ArrayList(modello.RAGIONI_DESTINATARI);
                            appoggio.RemoveAt(i);
                            modello.RAGIONI_DESTINATARI = new DocsPAWA.DocsPaWR.RagioneDest[appoggio.Count];
                            appoggio.CopyTo(modello.RAGIONI_DESTINATARI);
                            Session.Add("Modello", modello);
                            return;
                        }
                    }
                }
            }

        }

        private void msg_ConfirmDel_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

                modello.SINGLE = "0";

                Session.Add("Modello", modello);
                pulisciModello();
                caricaDataGridDest();

            }
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Cancel)
            {
                caricaDataGridDest();
            }
            caricaValoriGen();
        }

        private void pulisciModello()
        {
            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            modello.RAGIONI_DESTINATARI = null;
            userRegistri = UserManager.getListaRegistri(this);
            if (userRegistri.Length > 1)
                modello.ID_REGISTRO = this.ddl_registri.SelectedValue;
            else
            {
                DocsPaWebService ws = new DocsPaWebService();
                Registro reg = ws.GetRegistroBySistemId(modello.ID_REGISTRO);
                this.lbl_registri.Text = reg.descrizione;
                this.lbl_registri.Visible = true;
                this.ddl_registri.Visible = false;
            }
            Session.Add("Modello", modello);
        }

        private void btn_chiudi_Click(object sender, System.EventArgs e)
        {
            //luluciani 12/02/07
            Session.Remove("Modello");
            //
            Response.Write("<script>window.close();</script>");
        }

        private void dt_listaModelli_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            dt_listaModelli.CurrentPageIndex = e.NewPageIndex;
            modelliTrasmissione = UserManager.getModelliUtente(this, utente, UserManager.getInfoUtente(this), this.prmtRicerca.CreateSearchFilters());
            caricaDataGridModelli();
        }

        #region Notifiche trasmissione e cessione diritti

        /// <summary>
        /// Tasto Notifiche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_pp_notifica_Click(object sender, EventArgs e)
        {
            string jscript = string.Empty;
            bool modelloToSave = false;

            if (Session["modelloToSave"] != null)
                modelloToSave = (bool)Session["modelloToSave"];

            if (modelloToSave)
            {
                jscript = "<script>alert('Salvare prima il modello appena creato!');</script>";
                if (!ClientScript.IsStartupScriptRegistered("openAlert"))
                    ClientScript.RegisterStartupScript(this.GetType(), "openAlert", jscript);
            }
            else
            {
                jscript = "<script>apriModaleNotifiche('UPDATE');</script>";
                if (!ClientScript.IsStartupScriptRegistered("openModal"))
                    ClientScript.RegisterStartupScript(this.GetType(), "openModal", jscript);
            }
        }

        /// <summary>
        /// Imposta la visibilità e testo del tasto notifiche
        /// </summary>
        /// <param name="stato"></param>
        private void gestioneTastoNotifiche(bool stato)
        {
            string testo = "Gestione notifiche";
            this.btn_pp_notifica.Visible = stato;

            if (stato)
                if (this.checkUserAutorizedEditingACL())
                    this.btn_pp_notifica.Text = testo + " e Cessione diritti";
                else
                    this.btn_pp_notifica.Text = testo;
        }

        /// <summary>
        /// GESTIONE CESSIONE DIRITTI:
        /// verifica se l'utente è abilitato alla funzione ABILITA_CEDI_DIRITTI_DOC / ABILITA_CEDI_DIRITTI_FASC
        /// </summary>
        private bool checkUserAutorizedEditingACL()
        {
            string funzione = string.Empty;
            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            if (modello.CHA_TIPO_OGGETTO.Equals("D"))
                funzione = "ABILITA_CEDI_DIRITTI_DOC";
            if (modello.CHA_TIPO_OGGETTO.Equals("F"))
                funzione = "ABILITA_CEDI_DIRITTI_FASC";

            return UserManager.ruoloIsAutorized(this, funzione);
        }

        private bool NotificheUtDaImpostate()
        {
            bool retValue = false;

            if (Session["impostaNotifiche"] != null)
                retValue = (bool)Session["impostaNotifiche"];

            return retValue;
        }
        #endregion

        private string checkRuoliUtentiDuplicati()
        {
            string msg = string.Empty;
            int quanti;

            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {
                foreach (DocsPaWR.MittDest mittDest in ragDest.DESTINATARI)
                {
                    quanti = this.contaDest_perRicercaDuplicati(modello, mittDest.ID_CORR_GLOBALI);
                    if (mittDest.ID_CORR_GLOBALI != 0)
                    {
                        if (quanti > 1)
                            msg += "\\n- " + mittDest.DESCRIZIONE + " (" + ragDest.RAGIONE + ")";
                    }
                }
            }

            return msg;
        }

        private int contaDest_perRicercaDuplicati(DocsPaWR.ModelloTrasmissione modello, int idCorrGlob)
        {
            int quanti = 0;

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
                foreach (DocsPaWR.MittDest mittDest in ragDest.DESTINATARI)
                    if (mittDest.ID_CORR_GLOBALI.Equals(idCorrGlob))
                        quanti++;

            return quanti;
        }

        private string checkRagTrasmConCessioneDuplicate()
        {
            string msg = string.Empty;
            int contaRagConCessione = 0;

            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.OrgRagioneTrasmissione ragione = null;
            DocsPaWR.MittDest mittDest = null;

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {
                mittDest = ragDest.DESTINATARI[0];
                ragione = ws.AmmGetRagioneTrasmissione(Convert.ToString(mittDest.ID_RAGIONE));
                if (ragione != null)
                {
                    if (!ragione.PrevedeCessione.Equals(DocsPAWA.DocsPaWR.CedeDiritiEnum.No))
                    {
                        if (contaRagConCessione > 0)
                        {
                            msg += "\\n- " + ragDest.RAGIONE;
                        }
                        else
                        {
                            contaRagConCessione++;
                            msg += "\\n- " + ragDest.RAGIONE;
                        }
                    }
                }
            }

            if (contaRagConCessione <= 1)
                msg = string.Empty;

            return msg;
        }

        private string checkModUOConCessione()
        {
            string msg = string.Empty;

            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.OrgRagioneTrasmissione ragione = null;
            DocsPaWR.MittDest mittDest = null;

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {
                mittDest = ragDest.DESTINATARI[0];
                if (mittDest.CHA_TIPO_URP.Equals("U")) // se è una UO allora verifica se la ragione è CESSIONE
                {
                    ragione = ws.AmmGetRagioneTrasmissione(Convert.ToString(mittDest.ID_RAGIONE));
                    if (ragione != null)
                    {
                        if (!ragione.PrevedeCessione.Equals(DocsPAWA.DocsPaWR.CedeDiritiEnum.No))
                        {
                            msg += "\\n- " + mittDest.DESCRIZIONE;
                        }
                    }
                }
            }

            return msg;
        }

        private bool unicaUOinModello()
        {
            bool unicaUO = true;
            bool retValue = false;
            int contaUO = 0;
            int contaAltro = 0;
            DocsPaWR.MittDest mittDest = null;

            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

            foreach (DocsPAWA.DocsPaWR.RagioneDest ragioneDest in modello.RAGIONI_DESTINATARI)
                foreach (DocsPAWA.DocsPaWR.MittDest mDest in ragioneDest.DESTINATARI)
                    mDest.UTENTI_NOTIFICA = null;

            // quindi reperisce i dati sul db
            modello = ws.UtentiConNotificaTrasm(modello, null, null, "GET");

            Session.Remove("Modello");
            Session.Add("Modello", modello);

            foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
            {
                for (int i = 0; i < ragDest.DESTINATARI.Length; i++)
                {
                    mittDest = ragDest.DESTINATARI[i];
                    if (mittDest.CHA_TIPO_MITT_DEST == "D")
                    {
                        if (mittDest.CHA_TIPO_URP.Equals("U"))
                        {
                            contaUO++;
                        }
                        else
                        {//se c'è solo un utente per quel ruolo, viene settata automaticamente la notifica
                            if (mittDest.CHA_TIPO_URP == "R")
                            {
                                if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length == 1)
                                {
                                    mittDest.UTENTI_NOTIFICA[0].FLAG_NOTIFICA = "1";
                                    contaUO++;
                                }
                                else
                                {
                                    contaAltro++;
                                }
                            }

                            if (mittDest.CHA_TIPO_URP == "P")
                            {
                                if (mittDest.UTENTI_NOTIFICA != null && mittDest.UTENTI_NOTIFICA.Length == 1)
                                {
                                    mittDest.UTENTI_NOTIFICA[0].FLAG_NOTIFICA = "1";
                                    contaUO++;
                                }
                                else
                                {
                                    contaAltro++;
                                }
                            }
                        }
                    }
                    else
                    {
                        contaUO++;
                    }
                }
            }

            if (contaUO > 0 && contaAltro.Equals(0))
                retValue = true;

            Session.Add("Modello", modello);
            return retValue;
        }

        //private bool unicaUOinModello()
        //{
        //    bool unicaUO = true;
        //    bool retValue = false;
        //    int contaUO = 0;
        //    int contaAltro = 0;
        //    bool cessione = false;
        //    DocsPaWR.MittDest mittDest = null;
        //    DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
        //    DocsPaWR.OrgRagioneTrasmissione ragione = null;
        //    DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

        //    foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
        //    {
        //        mittDest = ragDest.DESTINATARI[0];
        //        ragione = ws.AmmGetRagioneTrasmissione(Convert.ToString(mittDest.ID_RAGIONE));
        //        if (ragione.PrevedeCessione.Equals(DocsPAWA.DocsPaWR.CedeDiritiEnum.No))
        //        {
        //            if (unicaUO)
        //            {
        //                for (int i = 0; i < ragDest.DESTINATARI.Length; i++)
        //                {
        //                    mittDest = ragDest.DESTINATARI[i];
        //                    if (mittDest.CHA_TIPO_MITT_DEST == "D")
        //                    {
        //                        if (mittDest.CHA_TIPO_URP.Equals("U"))
        //                        { // se è una UO allora verifica se la ragione è CESSIONE

        //                            contaUO++;
        //                        }
        //                        else
        //                        {
        //                            contaAltro++;
        //                            unicaUO = false;
        //                            break;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        contaUO++;
        //                    }
        //                }
        //            }
                   
        //        }
        //        else
        //        {
        //            cessione = true;
        //            contaUO++;
        //        }
        //    }

        //    if (!cessione)
        //    {
        //        return true;
        //    }

        //    if (contaUO > 0 && contaAltro.Equals(0))
        //        retValue = true;
        //    return retValue;
        //}

        public bool destinatariDaRubrica()
        {
            bool result = false;
            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            DocsPaWR.MittDest mittDest = null;
            bool destinatarioDaRubrica = false;

            if (!destinatarioDaRubrica)
            {
                foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
                {
                    for (int i = 0; i < ragDest.DESTINATARI.Length; i++)
                    {
                        mittDest = ragDest.DESTINATARI[i];
                        if (mittDest.CHA_TIPO_MITT_DEST == "D")
                        {
                            result = true;
                            destinatarioDaRubrica = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

    }
}
