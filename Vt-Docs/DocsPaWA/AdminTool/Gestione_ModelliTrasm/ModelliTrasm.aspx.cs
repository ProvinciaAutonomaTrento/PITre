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
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;
using Amministrazione.Manager;
using System.Configuration;
using DocsPAWA.SiteNavigation;
using DocsPAWA.utils;
using System.Xml.Linq;

namespace DocsPAWA.AdminTool.Gestione_ModelliTrasm
{
	/// <summary>
	/// Summary description for ModelliTrasm.
	/// </summary>
	public class ModelliTrasm : System.Web.UI.Page
	{
        //protected System.Web.UI.WebControls.TextBox txt_ricerca;
        //protected System.Web.UI.WebControls.Button btn_find;
		protected System.Web.UI.WebControls.Label lbl_position;
		protected System.Web.UI.WebControls.Label lbl_avviso;
		protected System.Web.UI.WebControls.Label lbl_titolo;
        //protected System.Web.UI.WebControls.Label lbl_ricerca;
		protected System.Web.UI.WebControls.Button btn_nuovoModello;
		protected System.Web.UI.WebControls.Panel Panel_ListaModelli;
		protected System.Web.UI.WebControls.Panel Panel_NuovoModello;
		protected System.Web.UI.WebControls.ImageButton btn_Rubrica_mitt;
		protected System.Web.UI.WebControls.DropDownList ddl_registri;
		protected System.Web.UI.HtmlControls.HtmlGenericControl DivDGListaTemplates;
		protected DocsPAWA.DocsPaWR.RagioneTrasmissione[] listaRagioni;
		protected System.Web.UI.WebControls.TextBox txt_nomeModello;
        protected System.Web.UI.WebControls.TextBox txt_codModello;
        //protected System.Web.UI.WebControls.TextBox txt_ricercaCodice;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoTrasmissione;
		protected System.Web.UI.WebControls.RadioButtonList rb_tipo_mittente;
		protected System.Web.UI.WebControls.DropDownList ddl_ragioni;
		protected System.Web.UI.WebControls.ImageButton btn_Rubrica_dest;
		protected System.Web.UI.WebControls.DataGrid dt_mitt;
		protected string gerarchia_trasm;
		protected string cha_tipo_ragione;
		protected System.Web.UI.WebControls.DataGrid dt_dest;
		protected System.Web.UI.WebControls.Panel Panel_mitt;
		protected System.Web.UI.WebControls.Panel Panel_dest;
		protected System.Web.UI.WebControls.Button btn_salvaModello;
		protected System.Web.UI.WebControls.TextBox txt_noteGenerali;
		protected string idAmministrazione;
		protected DocsPAWA.DocsPaWR.ModelloTrasmissione ModelloTrasmissione = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
		protected ArrayList modelliTrasmissione;
		protected System.Web.UI.WebControls.DataGrid dt_listaModelli;
		protected Utilities.MessageBox msg_ConfirmDel;
		protected Utilities.MessageBox msg_ConfirmCambioMitt;
		protected System.Web.UI.WebControls.Button btn_lista_modelli;
        protected System.Web.UI.WebControls.Button btn_pp_notifica;
		protected System.Web.UI.HtmlControls.HtmlInputHidden txt_confirmDel;
		protected System.Web.UI.HtmlControls.HtmlGenericControl Div1;
        //protected System.Web.UI.WebControls.DropDownList ddl_ricerca;
        //protected System.Web.UI.WebControls.TextBox txt_search;
		
		private DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
		private int numTotModelli = 0;
        protected System.Web.UI.WebControls.TextBox txt_codDest;
        protected System.Web.UI.WebControls.ImageButton ibtnMoveToA;
        protected System.Web.UI.WebControls.Label lbl_codice;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_returnValueModal;
        protected System.Web.UI.WebControls.Label lbl_stato;
        protected System.Web.UI.WebControls.Button btn_utente_proprietario;
        protected System.Web.UI.WebControls.Button btn_ruolo_prop;
        protected System.Web.UI.WebControls.Button btn_Uo_prop;
        protected System.Web.UI.WebControls.Button btn_resp_uo_prop;
        protected System.Web.UI.WebControls.Button btn_ruolo_segretario;
        protected System.Web.UI.WebControls.Button btn_resp_uo_mitt;
        protected System.Web.UI.WebControls.Button btn_segr_uo_mitt;
        protected System.Web.UI.WebControls.Label lbl_ragione;
        protected System.Web.UI.WebControls.CheckBox ckb_notify;
        //protected System.Web.UI.WebControls.CheckBox cb_ruoliDestInibitiTrasm;

        // Pannello dei filtri di ricerca
        protected DocsPAWA.UserControls.PannelloRicercaModelliTrasmissione prmtPannelloRicerca;

        protected System.Web.UI.WebControls.Button btn_toExtSys;
        protected System.Web.UI.HtmlControls.HtmlTableRow tr_backToExtSys;

		private void Page_Load(object sender, System.EventArgs e)
		{
            
            // Compilazione dei controlli con codice e descrizione se da impostare
            // se è presente un sessione CodeTextBox
            if (Session["CodeTextBox"] == null)
                this.ContinueWithPageLoad();

            if (Session["ClickFindAR"] != null)
            {
                Session.Remove("ClickFindAR");
                btn_lista_modelli_Click(null, null);
            }
            if (!Page.IsPostBack)
            {
                if (string.IsNullOrEmpty(Request.QueryString["extsysconf"]))
                    this.tr_backToExtSys.Visible = false;
            }
        }

        private void ContinueWithPageLoad()
        {
            Session["AdminBookmark"] = "ModelliTrasmissione";

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
            if (Session["AMMDATASET"] == null)
            {
                RegisterStartupScript("NoModTrasm", "<script>alert('Attenzione selezionare un\\'amministrazione !'); document.location = '../Gestione_Homepage/Home.aspx';</script>");
                return;
            }
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            idAmministrazione = wws.getIdAmmByCod(codiceAmministrazione);


            if (Session["PageIndexChanged"] == null && !IsPostBack)
            {
                modelliTrasmissione = new ArrayList(wws.getModelliByDdlAmmPaging(idAmministrazione, 0, this.prmtPannelloRicerca.CreateSearchFilters(), out numTotModelli));
                caricaDataGridModelli();
            }
            else
            {
                Session.Add("PageIndexChanged", null);
            }


            if (!IsPostBack)
            {
                lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");

                ModelloTrasmissione = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
                ModelloTrasmissione.ID_AMM = idAmministrazione;
                if(Session["Modello"] == null)
                    Session.Add("Modello", ModelloTrasmissione);

                // Caricamento combo registri disponibili
                this.CaricaComboRegistri(idAmministrazione);
                // Caricamento combo ragioni trasmissione

                DocsPaWR.Utente ut = new DocsPAWA.DocsPaWR.Utente();
                ut.codiceAmm = codiceAmministrazione;
                ut.idAmministrazione = idAmministrazione;
                ut.tipoIE = "I";
                ut.idRegistro = this.ddl_registri.SelectedValue.ToString();
                Session.Add("userData", ut);

                DocsPaWR.Ruolo rl = new DocsPAWA.DocsPaWR.Ruolo();
                rl.codiceAmm = codiceAmministrazione;
                rl.idAmministrazione = idAmministrazione;
                rl.tipoIE = "I";
                rl.idRegistro = this.ddl_registri.SelectedValue.ToString();

                rl.systemId = idAmministrazione;
                rl.uo = new DocsPAWA.DocsPaWR.UnitaOrganizzativa();
                rl.uo.codiceRubrica = codiceAmministrazione;

                Session.Add("userRuolo", rl);

                DocsPaWR.Registro reg = new DocsPAWA.DocsPaWR.Registro();
                reg.codAmministrazione = codiceAmministrazione;
                reg.idAmministrazione = idAmministrazione;
                reg.systemId = this.ddl_registri.SelectedValue.ToString();
                reg.codRegistro = this.ddl_registri.SelectedItem.Text.ToString();
                Session.Add("userRegistro", reg);
            }
            if (rb_tipo_mittente.SelectedValue.ToString().Equals("R"))
            {
                this.CaricaRagioni(idAmministrazione, false);
            }
            else
            {
                this.CaricaRagioni(idAmministrazione, true);
            }
            //carico i mittenti che mi ritornano da rubrica
            if (Session["selMittDaRubrica"] != null)
            {
                addMittSelDaRubrica((DocsPAWA.DocsPaWR.ElementoRubrica[])Session["selMittDaRubrica"]);
            }
            //
            //carico i destinatari che mi ritornano da rubrica
            if (Session["selDestDaRubrica"] != null)
            {
                addDestSelDaRubrica((DocsPAWA.DocsPaWR.ElementoRubrica[])Session["selDestDaRubrica"]);
            }

            // valore di ritorno della modale delle notifiche (possibili valori: "I", "U" o "")
            if (this.hd_returnValueModal.Value != null && this.hd_returnValueModal.Value != string.Empty && this.hd_returnValueModal.Value != "undefined")
            {
                if (this.hd_returnValueModal.Value.Equals("I"))
                    this.PerformSaveModel();

                this.hd_returnValueModal.Value = string.Empty;
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
			this.msg_ConfirmCambioMitt.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_ConfirmCambioMitt_GetMessageBoxResponse);
			this.msg_ConfirmDel.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.msg_ConfirmDel_GetMessageBoxResponse);
			this.btn_lista_modelli.Click += new System.EventHandler(this.btn_lista_modelli_Click);
			this.btn_salvaModello.Click += new System.EventHandler(this.btn_salvaModello_Click);
			this.btn_nuovoModello.Click += new System.EventHandler(this.btn_nuovoModello_Click);
			this.dt_listaModelli.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dt_listaModelli_PageIndexChanged);
			this.dt_listaModelli.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dt_listaModelli_DeleteCommand);
			this.dt_listaModelli.SelectedIndexChanged += new System.EventHandler(this.dt_listaModelli_SelectedIndexChanged);
			this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
			this.rb_tipo_mittente.SelectedIndexChanged += new System.EventHandler(this.rb_tipo_mittente_SelectedIndexChanged);
			this.btn_Rubrica_mitt.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Rubrica_mitt_Click);
			this.dt_mitt.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dt_mitt_EditCommand);
			this.ddl_ragioni.SelectedIndexChanged += new System.EventHandler(this.ddl_ragioni_SelectedIndexChanged);
			this.btn_Rubrica_dest.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Rubrica_dest_Click);
			this.dt_dest.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dt_dest_EditCommand);
            //this.btn_find.Click += new System.EventHandler(this.btn_find_Click);
			this.Load += new System.EventHandler(this.Page_Load);
            this.ibtnMoveToA.Click += new System.Web.UI.ImageClickEventHandler(this.ibtnMoveToA_Click);
            this.btn_utente_proprietario.Click += new System.EventHandler(this.btn_utente_proprietario_Click);
            this.btn_ruolo_prop.Click += new System.EventHandler(this.btn_ruolo_prop_Click);
            this.btn_Uo_prop.Click += new System.EventHandler(this.btn_Uo_prop_Click);
            this.btn_resp_uo_prop.Click += new System.EventHandler(this.btn_resp_uo_prop_Click);
            this.btn_ruolo_segretario.Click += new System.EventHandler(this.btn_ruolo_segretario_Click);
            this.btn_resp_uo_mitt.Click += new System.EventHandler(this.btn_resp_uo_mitt_Click);
            this.btn_segr_uo_mitt.Click += new System.EventHandler(this.btn_segr_uo_mitt_Click);
            this.PreRender += new EventHandler(ModelliTrasm_PreRender);

            // Registrazione evento per il search
            this.prmtPannelloRicerca.Search += new EventHandler(this.btn_find_Click);
        }

        void ModelliTrasm_PreRender(object sender, EventArgs e)
        {
            if (Session["noRagioneTrasmNulla"] == null)
            {
                if (dt_mitt.Items.Count != 0 || dt_dest.Items.Count != 0)
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
                Session.Remove("noRagioneTrasmNulla");
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
                Session["noRagioneTrasmNulla"] = true;
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
                            trasmissioni = new DocsPaWR.Trasmissione();

                        DocsPaWR.ElementoRubrica[] ers = new DocsPaWR.ElementoRubrica[listaCorr.Count];
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
                                foreach (DocsPaWR.Corrispondente corrisp in el)
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

                        DocsPAWA.DocsPaWR.ElementoRubrica[] elRub = new DocsPaWR.ElementoRubrica[nuovaListaCorr.Count];
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
                    foreach (DocsPaWR.Corrispondente c in el)
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

                DocsPAWA.DocsPaWR.ElementoRubrica[] elRub = new DocsPaWR.ElementoRubrica[nuovaListaCorr.Count];
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

        #endregion

        private void addDestSelDaRubrica(DocsPAWA.DocsPaWR.ElementoRubrica[] selDestDaRubrica)
		{
			if(selDestDaRubrica!=null)
			{
				string ragione= this.ddl_ragioni.SelectedItem.Text;
				ArrayList destinatari = new ArrayList();
				for(int i=0;i<selDestDaRubrica.Length;i++)
				{
					DocsPaWR.ElementoRubrica el		= (DocsPAWA.DocsPaWR.ElementoRubrica) selDestDaRubrica[i];
                    if (el.tipo.Equals("L"))
                    {
                        string idAmm = UserManager.getInfoUtente().idAmministrazione;                        
                        ArrayList lista = UserManager.getCorrispondentiByCodLista(this.Page, el.codice, idAmm);
                        foreach (DocsPaWR.Corrispondente corr in lista)
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
				AddMittDest(ragione,destinatari,cha_tipo_ragione);    
				caricaDataGridDest();
			}
		}
        
		private void btn_nuovoModello_Click(object sender, System.EventArgs e)
		{

			pulisciCampi();
            this.btn_salvaModello.Enabled = true;
			this.Panel_ListaModelli.Visible = false;
			this.Panel_NuovoModello.Visible  = true;
			this.Panel_mitt.Visible = true;
			this.Panel_dest.Visible = true;
			this.btn_salvaModello.Visible = true;
			this.btn_lista_modelli.Visible = true;
			SetFocus(this.txt_nomeModello);
			ModelloTrasmissione = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
			ModelloTrasmissione.ID_AMM = idAmministrazione;
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            string codModello = wws.getModelloSystemId();
            this.txt_codModello.Text = "MT_" + codModello;
            this.txt_codModello.Visible = false;
            this.lbl_codice.Visible = false;
            Session.Add("Modello", ModelloTrasmissione);

            this.EnableColumnNascondiVersioni(this.ddl_tipoTrasmissione.SelectedValue == "D");
			caricaDataGridDest();
			caricaDataGridMitt();

            // imposta una sessione per mantenere lo stato di "bisogna SALVARE"
            bool modelloToSave = true;
            Session.Add("modelloToSave", modelloToSave);

            // imposta una sessione per mantenere lo stato di "bisogna IMPOSTARE le notifiche"
            bool impostaNotifiche = true;
            Session.Add("impostaNotifiche", impostaNotifiche);
            this.lbl_stato.Text = "Nuovo";

            this.gestioneTastoNotifiche(false);
		}

		public void CaricaComboRegistri(string idAmministrazione)
		{
			try
			{
				this.ddl_registri.Items.Clear();
				
				Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.ListaRegistriRF(idAmministrazione, null, "");		
				
				ListItem it = new ListItem("","");
				this.ddl_registri.Items.Add(it);
					
				if(theManager.getListaRegistri().Count>0)
				{
                    foreach (DocsPAWA.DocsPaWR.OrgRegistro registro in theManager.getListaRegistri())
                    {
                        ListItem item = new ListItem(registro.Codice, registro.IDRegistro);
                        this.ddl_registri.Items.Add(item);
                    }         
				}
			}
			catch
			{
				this.ShowErrorMessage("Si è verificato un errore durante il reperimento dati dei registri.");
			}            
		}
	
		public void CaricaRagioni(string idAmm,bool all)
		{
			try
			{
				listaRagioni = Manager.ModelliTrasmManager.getlistRagioniTrasm(idAmm,all,string.Empty,true);				
				if (listaRagioni!=null && listaRagioni.Length>0)
				{
					int selezione = ddl_ragioni.SelectedIndex;
					ddl_ragioni.Items.Clear();
					for (int i=0; i<listaRagioni.Length; i++)
					{
						ListItem newItem=new ListItem(listaRagioni[i].descrizione,listaRagioni[i].systemId);
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

					TrasmManager.setRagioneSel(this,ragioneSel);
					DocsPaWR.RagioneTrasmissione rt = TrasmManager.getRagioneSel(this);
					this.gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0,1);
					this.cha_tipo_ragione = rt.tipo;
				}
			}
			catch
			{
				this.ShowErrorMessage("Si è verificato un errore durante il caricamento delle ragioni di trasmissione.");
			}
		}

		#region dt_dest
		
		public void caricaValoriGen()
		{
			DocsPaWR.ModelloTrasmissione modello =  (DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"];
            if (modello != null)
            {
                this.txt_nomeModello.Text = modello.NOME;
                this.txt_noteGenerali.Text = modello.VAR_NOTE_GENERALI;
                this.txt_codModello.Text = modello.CODICE;
                this.txt_codModello.Enabled = false;
                if (string.IsNullOrEmpty(modello.NO_NOTIFY) || modello.NO_NOTIFY.Equals("0"))
                    ckb_notify.Checked = false;
                else
                    ckb_notify.Checked = true;

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

                if (modello.SINGLE == "0") //selezionato un ruolo
                {
                    rb_tipo_mittente.SelectedIndex = 1;
                    btn_Rubrica_mitt.Visible = true;
                    CaricaRagioni(idAmministrazione, false);
                }
                else // tutta la AOO
                {
                    rb_tipo_mittente.SelectedIndex = 0;
                    btn_Rubrica_mitt.Visible = false;
                    CaricaRagioni(idAmministrazione, true);
                }

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
                }
            }
		}

		public void caricaDataGridMitt()
		{
			DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"];
			DataTable dt = new DataTable();
			dt.Columns.Add("SYSTEM_ID");
			dt.Columns.Add("VAR_DESC_CORR");
            dt.Columns.Add("VAR_COD_RUBRICA");
            this.btn_salvaModello.Enabled = true;
			if(modello.MITTENTE != null)
			{
                for (int j = 0; j < modello.MITTENTE.Length; j++)
                {
				    DataRow row = dt.NewRow();
				    //row[0] = corr.systemId;
                        row[1] = ((DocsPAWA.DocsPaWR.MittDest)modello.MITTENTE[j]).DESCRIZIONE;
                        row[2] = ((DocsPAWA.DocsPaWR.MittDest)modello.MITTENTE[j]).VAR_COD_RUBRICA;
                        if (modello.MITTENTE[j].ID_CORR_GLOBALI == 0)
                        {
                            Corrispondente corr = UserManager.getCorrispondenteByIdPeople(this, modello.ID_PEOPLE, AddressbookTipoUtente.INTERNO);
                            if (corr != null)
                            {
                                ListItem item = new ListItem();
                                item.Value = "P";
                                item.Text = "Persona";
                                item.Selected = true;
                                if (!this.rb_tipo_mittente.Items.Contains(item))
                                    this.rb_tipo_mittente.Items.Add(item);
                                else
                                    this.rb_tipo_mittente.SelectedValue.Equals("P");
                                this.btn_salvaModello.Enabled = false;
                                //this.rb_tipo_mittente.Items[2].Selected = true;
                                row[1] = corr.descrizione;
                                row[2] = corr.codiceRubrica;
                            }
                        }
				    dt.Rows.Add(row);
				    dt.AcceptChanges();
				    dt_mitt.DataSource = dt;
				    dt_mitt.DataBind();		
    			
				    dt_mitt.Visible = true;
			    }
                if (modello.MITTENTE.Length == 0)
                {
                    dt_mitt.DataSource = dt;
                    dt_mitt.DataBind();
                    dt_mitt.Visible = false;
                }
			}
			else
			{
				dt_mitt.DataSource = dt;
				dt_mitt.DataBind();		
				dt_mitt.Visible = false;
			}
		
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
                retVal = retVal = String.Format("<span style=\"color:Black;\">{0}</span>", text);

            return retVal;
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

		public void caricaDataGridDest()
		{
			DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"];
			
			DataTable dt = new DataTable();
			dt.Columns.Add("SYSTEM_ID");
			dt.Columns.Add("RAGIONE");
			dt.Columns.Add("VAR_COD_RUBRICA");
			dt.Columns.Add("VAR_DESC_CORR");
			dt.Columns.Add("ID_RAGIONE");
            dt.Columns.Add("NASCONDI_VERSIONI_PRECEDENTI", typeof(Boolean));
			
			if( modello.RAGIONI_DESTINATARI != null)
			{
				for(int i=0; i<modello.RAGIONI_DESTINATARI.Length; i++)
				{				
					for(int j=0; j<modello.RAGIONI_DESTINATARI[i].DESTINATARI.Length; j++)
					{
						DataRow row = dt.NewRow();

						DocsPaWR.RagioneDest rd = modello.RAGIONI_DESTINATARI[i];
						if(j==0)
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
                                row[3] = this.btn_utente_proprietario.Text;
                            if (mittDest.CHA_TIPO_MITT_DEST == "R_P")
                                row[3] = this.btn_ruolo_prop.Text;
                            if (mittDest.CHA_TIPO_MITT_DEST == "UO_P")
                                row[3] = this.btn_Uo_prop.Text;
                            if (mittDest.CHA_TIPO_MITT_DEST == "RSP_P")
                                row[3] = this.btn_resp_uo_prop.Text;
                            if (mittDest.CHA_TIPO_MITT_DEST == "R_S")
                                row[3] = this.btn_ruolo_segretario.Text;
                            if (mittDest.CHA_TIPO_MITT_DEST == "RSP_M")
                                row[3] = this.btn_resp_uo_mitt.Text;
                            if (mittDest.CHA_TIPO_MITT_DEST == "S_M")
                                row[3] = this.btn_segr_uo_mitt.Text;
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
            this.gestioneTastoNotifiche(false); 

			if(dt_dest.Items.Count!=0)
			{
				int k=0;
				for(int i=0; i<modello.RAGIONI_DESTINATARI.Length; i++)
				{				
					for(int j=0; j<modello.RAGIONI_DESTINATARI[i].DESTINATARI.Length; j++)
					{

						DocsPaWR.RagioneDest rd = modello.RAGIONI_DESTINATARI[i];
						DocsPaWR.MittDest mittDest = modello.RAGIONI_DESTINATARI[i].DESTINATARI[j];
						
						string imgurl="../../images/rubrica/";
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
						if(mittDest.CHA_TIPO_TRASM == "S")
							((DropDownList)dt_dest.Items[k].Cells[5].Controls[1]).SelectedIndex = 0;
						if(mittDest.CHA_TIPO_TRASM == "T")
							((DropDownList)dt_dest.Items[k].Cells[5].Controls[1]).SelectedIndex = 1;
                        if (!modello.RAGIONI_DESTINATARI[i].CHA_TIPO_RAGIONE.Equals("W"))
                        {
                           //OLD: ((DropDownList)dt_dest.Items[k].Cells[5].Controls[1]).Visible = false;
                            
                            ((TextBox)dt_dest.Items[k].Cells[7].Controls[3]).ReadOnly = true;
                            ((TextBox)dt_dest.Items[k].Cells[7].Controls[3]).BackColor = Color.Gray;
                        }
                        else
                        {
                            if(mittDest.SCADENZA != 0)
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

		public void caricaDataGridModelli()
		{
            this.prmtPannelloRicerca.EnableEsportAndFindButtons = true;
            //txt_ricerca.Focus();
            //this.txtCodice.Focus();
            if(modelliTrasmissione.Count != 0)
			{
				DataTable dt = new DataTable();
				dt.Columns.Add("SYSTEM_ID");
                dt.Columns.Add("CODICE");
                dt.Columns.Add("MODELLO");
				dt.Columns.Add("REGISTRO");
				dt.Columns.Add("TIPO DI TRASM.");
				dt.Columns.Add("VISIBILITA'");
                dt.Columns.Add("COD. REG.");

				for(int i=0; i<modelliTrasmissione.Count; i++)
				{
                   	DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione) modelliTrasmissione[i];
                    DocsPaWR.Registro reg = null;
                    if (modello.ID_REGISTRO != "0")
                    {
                        reg = UserManager.getRegistroBySistemId(this, modello.ID_REGISTRO);
                    }
					//DocsPaWR.Registro reg = UserManager.getRegistroBySistemId(this,modello.ID_REGISTRO);
                    DataRow row = dt.NewRow();
                   
                    row[0] = modello.SYSTEM_ID;
                    row[1] = this.GetSpanElement(modello.Valid, modello.CODICE); //modello.CODICE;
                    row[2] = this.GetSpanElement(modello.Valid, modello.NOME); //modello.NOME;
                    if (reg != null)
                    {
                        row[3] = reg.descrizione;
                        row[6] = reg.codRegistro; ////
                    }
                    else
                    {
                        row[3] = "";
                        row[6] = ""; ////
                    }
                    //row[3] = reg.descrizione;
                    if (modello.CHA_TIPO_OGGETTO == "D")
                        row[4] = "Documento";
                    if (modello.CHA_TIPO_OGGETTO == "F")
                        row[4] = "Fascicolo";
                    if (modello.SINGLE == "1")
                        if (reg != null)
                        {
                            row[5] = "Tutta la AOO" + " - " + reg.codRegistro;
                        }
                        else {
                            row[5] = "Tutta la AOO";
                        }
                        else
                        {
                            string visibilità = string.Empty;
                            for (int j = 0; j < modello.MITTENTE.Length; j++)
                            {
                                if (j != modello.MITTENTE.Length - 1)
                                    visibilità = visibilità + modello.MITTENTE[j].DESCRIZIONE + ", ";
                                else
                                    visibilità = visibilità + modello.MITTENTE[j].DESCRIZIONE;
                            }
                            row[5] = visibilità;
                        }
                    dt.Rows.Add(row);
                }
                dt_listaModelli.DataSource = dt;
                dt_listaModelli.VirtualItemCount = numTotModelli;
                if (dt_listaModelli.VirtualItemCount <= dt_listaModelli.PageSize)
                    dt_listaModelli.CurrentPageIndex = 0;
                dt_listaModelli.DataBind();
                dt_listaModelli.Visible = true;

                //lbl_ricerca.Text = "";
                this.prmtPannelloRicerca.SearchResult = String.Empty;
            }
			else
			{
				dt_listaModelli.Visible = false;
                //lbl_ricerca.Text = "Nessun modello per questa ricerca!";
                this.prmtPannelloRicerca.SearchResult = "Nessun modello per questa ricerca!";
                this.prmtPannelloRicerca.EnableEsportAndFindButtons = false;

			}
		}

		private void AggiornaRagioneDest()
		{
			DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"];

			if(dt_dest!=null && dt_dest.Items!=null && dt_dest.Items.Count>0)
			{
                if (modello != null && modello.RAGIONI_DESTINATARI != null)
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
			
		}

		#endregion dt_dest

		#region Gestione Modello in Session

		private void AggiornaModello()
		{
			//aggiorno i dati generali
			DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"];
			modello.ID_AMM = idAmministrazione;
			modello.ID_REGISTRO=this.ddl_registri.SelectedValue;
			modello.NOME = this.txt_nomeModello.Text;
			modello.VAR_NOTE_GENERALI = this.txt_noteGenerali.Text;
			modello.CHA_TIPO_OGGETTO = this.ddl_tipoTrasmissione.SelectedValue;

            if (modello.CHA_TIPO_OGGETTO == "D")
            {
                // Solamente per le trasmissioni ai documenti, impostazione del flag nascondi versioni
                //////////modello.NASCONDI_VERSIONI_PRECEDENTI = this.chkNascondiVersioniDocumento.Checked;
            }
            else
            {
                // Per i fascicoli, non esistono versioni
                //////////modello.NASCONDI_VERSIONI_PRECEDENTI = false;
            }

            modello.CODICE = this.txt_codModello.Text;
			if(rb_tipo_mittente.SelectedValue.Equals("AOO"))
				modello.SINGLE="1";
			else
				modello.SINGLE="0";

            modello.NO_NOTIFY = ckb_notify.Checked ? "1" : "0";

			Session.Add("Modello",modello);

			//aggiorno la session del registro per la rubrica
			DocsPaWR.Registro reg =(DocsPAWA.DocsPaWR.Registro) Session["userRegistro"];
			reg.systemId = this.ddl_registri.SelectedValue.ToString();
			reg.codRegistro = this.ddl_registri.SelectedItem.Text.ToString();
			Session.Add("userRegistro",reg);
		}

		private void addMittSelDaRubrica(DocsPAWA.DocsPaWR.ElementoRubrica[] selMittDaRubrica)
		{
			if(selMittDaRubrica!= null)
			{
                //((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MITTENTE = new DocsPAWA.DocsPaWR.MittDest[selMittDaRubrica.Length];
                ArrayList mittenti = new ArrayList();
                for (int i = 0; i < selMittDaRubrica.Length; i++)
                {
                    DocsPaWR.ElementoRubrica el = (DocsPAWA.DocsPaWR.ElementoRubrica)selMittDaRubrica[i];
				
				    DocsPaWR.Corrispondente corr	= UserManager.getCorrispondenteByCodRubricaIE(this,el.codice,el.interno ? DocsPAWA.DocsPaWR.AddressbookTipoUtente.INTERNO : DocsPAWA.DocsPaWR.AddressbookTipoUtente.ESTERNO);
				
				    DocsPaWR.Ruolo rl = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
				    rl.systemId = ((DocsPAWA.DocsPaWR.Ruolo)corr).systemId;
				    rl.uo = new DocsPAWA.DocsPaWR.UnitaOrganizzativa();
				    rl.uo.codiceRubrica = ((DocsPAWA.DocsPaWR.Ruolo)corr).uo.codiceRubrica;
				    Session.Add("userRuolo",rl);

				    //inserisco le informazione del mittente 
				    DocsPaWR.MittDest mittente = new DocsPAWA.DocsPaWR.MittDest();
				    mittente.CHA_TIPO_MITT_DEST = "M";
				    mittente.VAR_COD_RUBRICA = corr.codiceRubrica;
				    mittente.DESCRIZIONE =corr.descrizione;
				    mittente.ID_CORR_GLOBALI = Convert.ToInt32(corr.systemId);
                    //((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]).MITTENTE[i] = mittente;
				    ((DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"]).SINGLE ="0";
				    mittente.CHA_TIPO_URP = corr.tipoCorrispondente;
                    mittenti.Add(mittente);
                }
				
				Session.Remove("selMittDaRubrica");				
                addMitt(mittenti);
                caricaDataGridMitt();
			}		
		}


		private void AddMittDest(string ragione,ArrayList destinatari,string cha_tipo_ragione)
		{
			ArrayList array_1;
			DocsPaWR.ModelloTrasmissione modello = ((DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"]);
				
			//controllo se esiste già una ragioneDest
			if(((DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"]).RAGIONI_DESTINATARI!=null)
			{
				//controllo se esiste per la ragione
				DocsPaWR.RagioneDest rd = null;
				for(int i=0;i<modello.RAGIONI_DESTINATARI.Length;i++)
				{
					if(((DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i]).RAGIONE.Equals(ragione))
					{
						rd = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
						break;
					}
				}
					
					//se esiste già una ragione aggiungo i destinatari alla stessa
					//if(((DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i]).RAGIONE.Equals(ragione))
					if(rd != null)
					{						
						array_1 = new ArrayList(rd.DESTINATARI);
						verificaEsistenzaDest(array_1,ref destinatari);
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
						if(((DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"]).RAGIONI_DESTINATARI != null)
							array_2 = new ArrayList( ((DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"]).RAGIONI_DESTINATARI );
						else
							array_2 = new ArrayList();

						array_2.Add(rd1);
						((DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"]).RAGIONI_DESTINATARI = new DocsPAWA.DocsPaWR.RagioneDest[array_2.Count];
						array_2.CopyTo( ((DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"]).RAGIONI_DESTINATARI );
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
				((DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"]).RAGIONI_DESTINATARI = new DocsPAWA.DocsPaWR.RagioneDest[array_2.Count];
				array_2.CopyTo(((DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"]).RAGIONI_DESTINATARI);
			}

			Session.Add("Modello",modello);
		}


		private void verificaEsistenzaDest(ArrayList destVecchi, ref ArrayList destNuovi)
		{
			ArrayList elNuovi_1 = new ArrayList();
			bool result = true;
			for(int i=0; i<destNuovi.Count; i++)
			{
				for(int j=0; j<destVecchi.Count; j++)
				{
                    if (((DocsPAWA.DocsPaWR.MittDest)destNuovi[i]).VAR_COD_RUBRICA == ((DocsPAWA.DocsPaWR.MittDest)destVecchi[j]).VAR_COD_RUBRICA && ((DocsPAWA.DocsPaWR.MittDest)destNuovi[i]).CHA_TIPO_MITT_DEST == "D")
                    {
                        result = false;
                    }
                    else 
                    {
                        if (((DocsPAWA.DocsPaWR.MittDest)destNuovi[i]).CHA_TIPO_MITT_DEST != "D" && ((DocsPAWA.DocsPaWR.MittDest)destNuovi[i]).CHA_TIPO_MITT_DEST == ((DocsPAWA.DocsPaWR.MittDest)destVecchi[j]).CHA_TIPO_MITT_DEST)
                        {
                            result = false;
                        }
                    }

				}
				if(result)
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
			this.txt_nomeModello.Text="";
			this.txt_noteGenerali.Text = "";
			this.ddl_ragioni.SelectedIndex = 0;
			this.ddl_registri.SelectedIndex = 0;
			this.ddl_tipoTrasmissione.SelectedIndex = 0;
			this.rb_tipo_mittente.SelectedIndex = 1;
			this.btn_Rubrica_mitt.Visible = true;
            this.ckb_notify.Checked = false;
		}

		private string CheckFields() 
		{
			string msg = string.Empty;

			if(this.txt_nomeModello.Text.Trim()=="")
			{
				msg="Inserire il nome del modello.";
				SetFocus(this.txt_nomeModello);
				return msg;
			}
			if(this.dt_mitt!=null  && this.dt_mitt.Items.Count==0 && rb_tipo_mittente.SelectedValue.Equals("R"))
			{
				msg="Inserire il mittente del modello.";
				return msg;
			}
			if(this.dt_dest!=null && this.dt_dest.Items.Count==0)
			{
				msg="Inserire almeno un destinatario del modello.";
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

			return msg;
		}

		private void ShowErrorMessage(string errorMessage)
		{
			this.RegisterClientScript("ErrorMessage","alert('" + errorMessage.Replace("'","\\'") + "')");
		}

		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				//this.Page.RegisterStartupScript(scriptKey, scriptString);
                this.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
			}
		}
		
		#endregion utils
		
		private void rb_tipo_mittente_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            //if(ddl_registri.SelectedValue == "")
            //{
            //    RegisterStartupScript("alert","<script>alert('Attenzione! Selezionare un registro!');</script>");
            //    return;
            //}

			DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"];
			modello.ID_AMM = idAmministrazione;
			modello.ID_REGISTRO=this.ddl_registri.SelectedValue;
			modello.NOME = this.txt_nomeModello.Text;
			modello.VAR_NOTE_GENERALI = this.txt_noteGenerali.Text;
			modello.CHA_TIPO_OGGETTO = this.ddl_tipoTrasmissione.SelectedValue;
			Session.Add("Modello",modello);

			AggiornaRagioneDest();

			if(dt_mitt.Items.Count != 0 || dt_dest.Items.Count != 0)
				msg_ConfirmDel.Confirm("Attenzione. La seguente modifica comporta la perdita dei dati finora inseriti.");
			
			if(rb_tipo_mittente.SelectedValue.ToString().Equals("R"))
			{
				this.btn_Rubrica_mitt.Visible=true;
				CaricaRagioni(((DocsPAWA.DocsPaWR.Registro) Session["userRegistro"]).idAmministrazione.ToString(),false);
			}
			else
			{
				this.btn_Rubrica_mitt.Visible=false;
				CaricaRagioni(((DocsPAWA.DocsPaWR.Registro) Session["userRegistro"]).idAmministrazione.ToString(),true);					
			}
		}

		private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            if (this.ddl_registri.SelectedIndex != 0)
            {
                DocsPAWA.DocsPaWR.Registro registro = wws.GetRegistroBySistemId(this.ddl_registri.SelectedValue);
                if (registro.Sospeso)
                {
                    RegisterClientScript("alertRegistroSospeso", "alert('Il registro selezionato è sospeso!');");
                    this.ddl_registri.SelectedIndex = 0;
                    //RegisterClientScript("refresh", "document.forms[0].submit();");
                    return;
                }
            }

            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
			modello.ID_AMM = idAmministrazione;
			modello.NOME = this.txt_nomeModello.Text;
			modello.VAR_NOTE_GENERALI = this.txt_noteGenerali.Text;
			modello.CHA_TIPO_OGGETTO = this.ddl_tipoTrasmissione.SelectedValue;
			Session.Add("Modello",modello);

			AggiornaRagioneDest();

			if(dt_mitt.Items.Count != 0 || dt_dest.Items.Count != 0)
				msg_ConfirmDel.Confirm("Attenzione. La seguente modifica comporta la perdita dei dati finora inseriti.");

			DocsPaWR.Registro reg =(DocsPAWA.DocsPaWR.Registro) Session["userRegistro"];
			
			reg.systemId = this.ddl_registri.SelectedValue.ToString();
			reg.codRegistro = this.ddl_registri.SelectedItem.Text.ToString();
			Session.Add("userRegistro",reg);
		}

		private void dt_mitt_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			AggiornaModello();
			AggiornaRagioneDest();
            int index = e.Item.ItemIndex;
            string codiceRubrica = dt_mitt.Items[index].Cells[2].Text;
            Session.Add("codiceMittDaEliminare", codiceRubrica);
            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];

			if(dt_dest.Items.Count != 0 && modello.MITTENTE.Length==1)
			{
				msg_ConfirmDel.Confirm("Attenzione. La seguente modifica comporta la perdita dei dati finora inseriti.");
			}
			else
			{                
				//pulisciModello();
                cancellaMittente(codiceRubrica);
				caricaDataGridMitt();
			}
		}

		private void ddl_ragioni_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DocsPaWR.RagioneTrasmissione ragioneSel = listaRagioni[this.ddl_ragioni.SelectedIndex];
			TrasmManager.setRagioneSel(this,ragioneSel);
			DocsPaWR.RagioneTrasmissione rt = TrasmManager.getRagioneSel(this);
			this.gerarchia_trasm = rt.tipoDestinatario.ToString("g").Substring(0,1);
            Session["noRagioneTrasmNulla"] = true;
		}

		private void btn_modelli_Click(object sender, System.EventArgs e)
		{
		
		}

        private void btn_find_Click(object sender, System.EventArgs e)
        {
            //lbl_ricerca.Text = "";
            if (Panel_ListaModelli.Visible == false)
                btn_lista_modelli_Click(null, null);
            else
                // Altrimenti si effettua la ricerca seocndo i filtri impostati
                this.Search();
            Session.Add("PageIndexChanged", null);
        }

		private void btn_salvaModello_Click(object sender, System.EventArgs e)
		{
			string msgErr = CheckFields();
            string codModello = string.Empty;
            if (msgErr != null && !msgErr.Equals(String.Empty))
			{
				this.RegisterClientScript("message","alert('" + msgErr.Replace("'","\\'") + "')");
				//Response.Write("<script>alert('" + msgErr + "');</script>");
				
			}
			else
			{
				AggiornaModello();

                // univocità del codice Modello Trasmissione
                // commentata in quanto si è scelto di mettere automatcamente il codice come "MT_" + system_id

                //bool isModelloUnico = wws.isUniqueCodModelloTrasm((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]);
                //if (isModelloUnico)
                //{
                    AggiornaRagioneDest();

                    if (this.NotificheUtDaImpostate() && !this.unicaUOinModello())
                    {
                        bool continua = true;
                        //verifica che sia obbligatorio la selezione del registro (se previsto da chiave di configurazione)
                        string regObbl = wws.getSeRegistroObbl(UserManager.getInfoUtente().idAmministrazione, "REG_OBBL_IN_MODELLO");
                        if (!String.IsNullOrEmpty(regObbl) && regObbl.Equals("1"))
                            if(this.ddl_registri.SelectedIndex == 0)
                                continua = false;

                        if (continua)
                            RegisterClientScript("noNotifiche", "apriModaleNotifiche('INSERT');");
                        else
                            this.RegisterClientScript("message", "alert('La scelta del registro è obbligatoria per la creazione del modello di trasmissione!')");
                    }
                    else
                    {
                        this.PerformSaveModel();

                        //wws.salvaModello((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"], UserManager.getInfoUtente());
                        //RegisterClientScript("Modifica effettuata", "alert('Operazione effettuata con successo!');");

                        //Session.Remove("Modello");

                        //dt_listaModelli.CurrentPageIndex = 0;
                        //modelliTrasmissione = new ArrayList(wws.getModelliByAmmPaging(idAmministrazione, 0, txt_ricerca.Text, txt_ricercaCodice.Text, out numTotModelli));
                        //caricaDataGridModelli();
                        //pulisciCampi();
                        //this.btn_salvaModello.Visible = false;
                        //this.btn_lista_modelli.Visible = false;
                        //this.Panel_NuovoModello.Visible = false;
                        //this.Panel_mitt.Visible = false;
                        //this.Panel_dest.Visible = false;
                        //this.Panel_ListaModelli.Visible = true;
                        //dt_listaModelli.SelectedIndex = -1;
                        //Session.Remove("modelloToSave");
                    }                    
                    
                //}
                //else
                //{
                //    RegisterClientScript("Codice Modello Esistente", "alert('Esiste già un modello trasmissione con tale codice!');");
                //}
			}								
		}

        private void PerformSaveModel()
        {
            bool continua = true;
            //verifica che sia obbligatorio la selezione del registro (se previsto da chiave di configurazione)
            string regObbl = wws.getSeRegistroObbl(UserManager.getInfoUtente().idAmministrazione, "REG_OBBL_IN_MODELLO");
            if (!String.IsNullOrEmpty(regObbl) && regObbl.Equals("1"))
                if(this.ddl_registri.SelectedIndex == 0)
                    continua = false;

            if (continua)
                {
                    string codModello = wws.salvaModello((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"], UserManager.getInfoUtente());
                    string[] cod = codModello.Split('^');
                    string alertMsg = "Operazione effettuata con successo!";
                    if (cod[0] == "I")
                        alertMsg += "\\n Inserito modello con codice MT_" + cod[1];
                    else
                        alertMsg += "\\n Modificato modello con codice MT_" + cod[1];
                    RegisterClientScript("Modifica effettuata", "alert('" + alertMsg + "');");

                    Session.Remove("Modello");

                    dt_listaModelli.CurrentPageIndex = 0;

                    modelliTrasmissione = new ArrayList(wws.getModelliByDdlAmmPaging(idAmministrazione, 0, this.prmtPannelloRicerca.CreateSearchFilters(), out numTotModelli));

                    caricaDataGridModelli();
                    pulisciCampi();
                    this.btn_salvaModello.Visible = false;
                    this.btn_lista_modelli.Visible = false;
                    this.Panel_NuovoModello.Visible = false;
                    this.Panel_mitt.Visible = false;
                    this.Panel_dest.Visible = false;
                    this.Panel_ListaModelli.Visible = true;
                    dt_listaModelli.SelectedIndex = -1;
                    Session.Remove("modelloToSave");
                }
                else
                    this.RegisterClientScript("message", "alert('La scelta del registro è obbligatoria per la creazione del modello di trasmissione!')");
        }

		private void dt_listaModelli_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int indx = dt_listaModelli.SelectedIndex;
			string idModello = dt_listaModelli.Items[indx].Cells[0].Text;
			DocsPaWR.ModelloTrasmissione modelloTrasmSel = wws.getModelloByID(idAmministrazione,idModello);
			Session.Add("Modello",modelloTrasmSel);
			caricaValoriGen();
			if(modelloTrasmSel.SINGLE.Equals("0"))
			{
                try
                {
                    DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this, modelloTrasmSel.MITTENTE[0].VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
                    //DocsPaWR.Ruolo rl = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
                    //if (rl != null && !string.IsNullOrEmpty(((DocsPAWA.DocsPaWR.Ruolo)corr).systemId))
                    //{
                    //    rl.systemId = ((DocsPAWA.DocsPaWR.Ruolo)corr).systemId;
                    //    rl.uo = new DocsPAWA.DocsPaWR.UnitaOrganizzativa();
                    //    rl.uo.codiceRubrica = ((DocsPAWA.DocsPaWR.Ruolo)corr).uo.codiceRubrica;
                    //    Session.Add("userRuolo", rl);
                    //}
                }
                catch
                {
                    // Viene visualizzato un alert per avvisare l'utente che non è stato possibile caricare informazioni
                    // sul mittente
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "noSender", "alert('Si è verificato un errore durante il caricamento delle informazioni sulla visibilità del modello.');", true);
                }

				
			}
			caricaDataGridMitt();
			caricaDataGridDest();
			this.Panel_ListaModelli.Visible=false;
			this.btn_salvaModello.Visible = true;
			this.btn_lista_modelli.Visible = true;
			this.Panel_NuovoModello.Visible=true;
			this.Panel_mitt.Visible=true;
			this.Panel_dest.Visible=true;
            this.lbl_codice.Visible = true;
            this.txt_codModello.Visible = true;

            // imposta una sessione per mantenere lo stato di "bisogna IMPOSTARE le notifiche"
            bool impostaNotifiche = true;
            Session.Add("impostaNotifiche", impostaNotifiche);
            this.lbl_stato.Text = "Modifica";

            this.gestioneTastoNotifiche(true);
		}

		private void dt_listaModelli_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if(((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmDel")).Value == "si")
			{
				int indx = e.Item.ItemIndex;
				string idModello = dt_listaModelli.Items[indx].Cells[0].Text;
				wws.CancellaModello(idAmministrazione,idModello);
				dt_listaModelli.CurrentPageIndex = 0;
                
                modelliTrasmissione = new ArrayList(wws.getModelliByDdlAmmPaging(idAmministrazione, dt_listaModelli.CurrentPageIndex, this.prmtPannelloRicerca.CreateSearchFilters(), out numTotModelli));
                
                caricaDataGridModelli();

				if(this.dt_listaModelli.Items.Count -1 == 0 && this.dt_listaModelli.CurrentPageIndex > 0)
					this.dt_listaModelli.CurrentPageIndex = this.dt_listaModelli.CurrentPageIndex - 1;
				
				((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmDel")).Value = "";
			}
		}

		private void msg_ConfirmDel_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
		{
			
			if(e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
			{
				DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"];
				if(rb_tipo_mittente.SelectedValue.Equals("AOO"))
				{
					modello.SINGLE="1";
				}
				else
				{
					modello.SINGLE="0";
				}
				Session.Add("Modello",modello);
				pulisciModello();
                //cancellaMittente(Session["codiceMittDaEliminare"].ToString());
				caricaDataGridDest();
				caricaDataGridMitt();
			}
			if(e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Cancel)
			{
				
				caricaDataGridMitt();
				caricaDataGridDest();
				
			}
			caricaValoriGen();			
		}

		private void msg_ConfirmCambioMitt_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
		{
			
		
			if(e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
			{
				pulisciModello();
				caricaDataGridDest();
				caricaDataGridMitt();
				RegisterClientScript("apriRubrica","_ApriRubrica();");
			}
			if(e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Cancel)
			{
				
				caricaDataGridMitt();
				caricaDataGridDest();
				
			}
			caricaValoriGen();
		}
		
		private void btn_Rubrica_dest_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            //if(ddl_registri.SelectedValue == "")
            //{
            //    RegisterStartupScript("alert","<script>alert('Attenzione! Selezionare un registro!');</script>");
            //    return;
            //}

            Session["noRagioneTrasmNulla"] = true;

            // bug inps: nel caso in cui da amministrazione non si seleziona alcun registro,
            // in inps capitava che i risultati della ricerca da rubrica erano diversi tra amministrazione
            // e front end; per ovviare a questo problema abbiamo dovuto far in modo che, da amministrazione, 
            // anche selezionando un ruolo mittente, se il registro è vuoto, la ricerca si comporti come se 
            // fosse selezionata tutta la AOO come mittente
            //if (this.ddl_registri.SelectedIndex == 0)
            //    this.rb_tipo_mittente.SelectedIndex = 0;

            if(this.dt_mitt!=null  && this.dt_mitt.Items.Count==0 && rb_tipo_mittente.SelectedValue.Equals("R"))
			{
				msg_ConfirmDel.Alert("Inserire il mittente del modello.");
			}
			else
			{
                if (!this.ddl_ragioni.SelectedItem.Text.Equals(""))
                {
                    if (rb_tipo_mittente.SelectedValue.ToString().Equals("R") && this.ddl_registri.SelectedIndex != 0)
                    {
                        if (dt_mitt.Items.Count > 1)
                        {
                            RegisterClientScript("apriRubrica", "ApriRubricaAoo()");
                        }
                        else
                        {
                            RegisterClientScript("apriRubrica", "ApriRubricaTrasm('" + ddl_ragioni.SelectedItem.Text + "','A');");
                        }
                    }
                    else
                    {
                        RegisterClientScript("apriRubrica", "ApriRubricaAoo()");
                    }
                }
                else
                {
                    msg_ConfirmDel.Alert("Selezionare la ragione trasmissione.");
                }
			}
		}

		private void btn_Rubrica_mitt_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            bool rfEnabled = false;

            try
            {
                rfEnabled = wws.existRf(idAmministrazione);
            }
            catch (Exception ex)
            { 
            }

            if(rfEnabled && this.ddl_registri.SelectedValue == "")
            //if(ddl_registri.SelectedValue == "")
            {
                RegisterStartupScript("alert","<script>alert('Attenzione! Selezionare un registro!');</script>");
                return;
            }

            Session["noRagioneTrasmNulla"] = true;
            AggiornaModello();
			AggiornaRagioneDest();
            //if(dt_mitt.Items.Count != 0 || dt_dest.Items.Count != 0)
            //{	
            //    msg_ConfirmCambioMitt.Confirm("Attenzione. La seguente modifica comporta la perdita dei dati finora inseriti.");
            //}
            //else
            //{
				RegisterClientScript("apriRubrica","_ApriRubrica();");	
			//}
		}
		
		private void pulisciModello()
		{

			DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"];				
			//modello.RAGIONI_DESTINATARI = null;
			modello.MITTENTE = null;
			modello.ID_REGISTRO=this.ddl_registri.SelectedValue;
            Session.Add("Modello", modello);				
		}

		private void btn_lista_modelli_Click(object sender, System.EventArgs e)
		{
			dt_listaModelli.CurrentPageIndex = 0;
            modelliTrasmissione = new ArrayList(wws.getModelliByDdlAmmPaging(idAmministrazione, 0, this.prmtPannelloRicerca.CreateSearchFilters(), out numTotModelli));
            caricaDataGridModelli();
            pulisciCampi();
			this.btn_salvaModello.Visible = false;
			this.Panel_NuovoModello.Visible=false;
			this.btn_lista_modelli.Visible = false;
			this.Panel_mitt.Visible=false;
			this.Panel_dest.Visible=false;
			this.Panel_ListaModelli.Visible=true;
			dt_listaModelli.SelectedIndex = -1;

            Session.Remove("modelloToSave");

            Session.Remove("impostaNotifiche");
		}

		private void dt_dest_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if(((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmDel")).Value == "si")
			{
				int indx = e.Item.ItemIndex;
				string idragione = dt_dest.Items[indx].Cells[8].Text;
				string var_cod_rubrica = dt_dest.Items[indx].Cells[3].Text;
					((System.Web.UI.HtmlControls.HtmlInputHidden) this.FindControl("txt_confirmDel")).Value = "";
                    if (var_cod_rubrica.Equals("&nbsp;"))
                        var_cod_rubrica = var_cod_rubrica.Replace("&nbsp;", "");
				CancellaDestinatario(idragione,var_cod_rubrica);
				caricaDataGridDest();
			}


		}

		private void CancellaDestinatario(string idragione,string var_cod_rubrica)
		{
			DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione) Session["Modello"];
			
            // Se cerchi di cancellare un destinatario cliccando sul bidoncino non succede nulla perché il codice
            // che arriva contiene anche i tag html span di formattazione.
            XElement elem = XElement.Parse(var_cod_rubrica);
            var_cod_rubrica = elem.Value;

            if (modello != null && modello.RAGIONI_DESTINATARI != null)
            {
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {
                    DocsPaWR.RagioneDest rd = modello.RAGIONI_DESTINATARI[i];
                    for (int j = 0; j < modello.RAGIONI_DESTINATARI[i].DESTINATARI.Length; j++)
                    {
                        DocsPaWR.MittDest dest = ((DocsPAWA.DocsPaWR.MittDest)modello.RAGIONI_DESTINATARI[i].DESTINATARI[j]);

                        if (dest.ID_RAGIONE.ToString().Equals(idragione) && dest.VAR_COD_RUBRICA.Equals(var_cod_rubrica.Trim()))
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
		}

		private void dt_listaModelli_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			dt_listaModelli.CurrentPageIndex = e.NewPageIndex;
            
            modelliTrasmissione = new ArrayList(wws.getModelliByDdlAmmPaging(idAmministrazione, dt_listaModelli.CurrentPageIndex, this.prmtPannelloRicerca.CreateSearchFilters(), out numTotModelli));
            
            caricaDataGridModelli();
			dt_listaModelli.SelectedIndex = -1;
			Session.Add("PageIndexChanged", true);
		}

        protected void dt_listaModelli_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }
        }

        protected void dt_mitt_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }
        }

        protected void dt_dest_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {

                if (e.Item.Cells.Count > 0)
                {

                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());

                }

            }
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

            if(Session["modelloToSave"]!=null)
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
            // Gestione notifiche e Cessione diritti
            string testo = "Gestione notifiche";

            if (isCessioneAbilitata())
                testo += " e Cessione diritti";

            this.btn_pp_notifica.Text = testo;

            this.btn_pp_notifica.Visible = stato;            
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

            if (modello != null && modello.RAGIONI_DESTINATARI != null)
            {
                foreach (DocsPaWR.RagioneDest ragDest in modello.RAGIONI_DESTINATARI)
                {
                    foreach (DocsPaWR.MittDest mittDest in ragDest.DESTINATARI)
                    {
                        quanti = this.contaDest_perRicercaDuplicati(modello, mittDest.ID_CORR_GLOBALI);
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
                    if (mittDest.ID_CORR_GLOBALI.Equals(idCorrGlob) && idCorrGlob!=0)
                        quanti ++;                    
                
            return quanti;
        }

        private bool isCessioneAbilitata()
        {
            return (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["GEST_RAG_TRASM_CESSIONE"]) && System.Configuration.ConfigurationManager.AppSettings["GEST_RAG_TRASM_CESSIONE"].Equals("1"));
        }

        private string checkRagTrasmConCessioneDuplicate()
        {
            string msg = string.Empty;
            int contaRagConCessione = 0;

            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPaWR.OrgRagioneTrasmissione ragione = null;
            DocsPaWR.MittDest mittDest = null;

            if (modello != null && modello.RAGIONI_DESTINATARI != null)
            {
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

            if (modello != null && modello.RAGIONI_DESTINATARI != null)
            {
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

            if (modello != null && modello.RAGIONI_DESTINATARI != null)
            {
                foreach (DocsPAWA.DocsPaWR.RagioneDest ragioneDest in modello.RAGIONI_DESTINATARI)
                    foreach (DocsPAWA.DocsPaWR.MittDest mDest in ragioneDest.DESTINATARI)
                        mDest.UTENTI_NOTIFICA = null;
            }
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

        protected void btn_utente_proprietario_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dt_dest.Items.Count; i++)
            {
                string ragione = dt_dest.Items[i].Cells[1].Text;
                if (ragione.ToUpper().Equals("CESSIONE") && ddl_ragioni.SelectedItem.Text == "CESSIONE")
                {
                    RegisterStartupScript("alert", "<script>alert('Impossibile inserire destinatario, il modello contiene una ragione di trasmissione cessione');</script>");
                    return;
                }
            }
                //if (ddl_registri.SelectedValue == "")
                //{
                //    RegisterStartupScript("alert", "<script>alert('Attenzione! Selezionare un registro!');</script>");
                //    return;
                //}

            if (this.dt_mitt != null && this.dt_mitt.Items.Count == 0 && rb_tipo_mittente.SelectedValue.Equals("R"))
            {
                msg_ConfirmDel.Alert("Inserire il mittente del modello.");
            }
            else
            {
                string ragione = this.ddl_ragioni.SelectedItem.Text;
                ArrayList destinatari = new ArrayList();
                DocsPaWR.MittDest destinatario = new DocsPAWA.DocsPaWR.MittDest();
                destinatario.CHA_TIPO_MITT_DEST = "UT_P";
                destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                destinatario.VAR_COD_RUBRICA = "";
                destinatario.DESCRIZIONE = this.btn_utente_proprietario.Text;
                destinatario.CHA_TIPO_URP = "P";
                destinatario.ID_CORR_GLOBALI =Convert.ToInt32("0");
                destinatari.Add(destinatario);
                AggiornaRagioneDest();
                AddMittDest(ragione, destinatari, cha_tipo_ragione);
                caricaDataGridDest();

            }
        }

        protected void btn_ruolo_prop_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dt_dest.Items.Count; i++)
            {
                string ragione = dt_dest.Items[i].Cells[1].Text;
                if (ragione.ToUpper().Equals("CESSIONE") && ddl_ragioni.SelectedItem.Text == "CESSIONE")
                {
                    RegisterStartupScript("alert", "<script>alert('Impossibile inserire destinatario, il modello contiene una ragione di trasmissione cessione');</script>");
                    return;
                }
            }
            //if (ddl_registri.SelectedValue == "")
            //{
            //    RegisterStartupScript("alert", "<script>alert('Attenzione! Selezionare un registro!');</script>");
            //    return;
            //}

            if (this.dt_mitt != null && this.dt_mitt.Items.Count == 0 && rb_tipo_mittente.SelectedValue.Equals("R"))
            {
                msg_ConfirmDel.Alert("Inserire il mittente del modello.");
            }
            else
            {
                string ragione = this.ddl_ragioni.SelectedItem.Text;
                ArrayList destinatari = new ArrayList();
                DocsPaWR.MittDest destinatario = new DocsPAWA.DocsPaWR.MittDest();
                destinatario.CHA_TIPO_MITT_DEST = "R_P";
                destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                destinatario.VAR_COD_RUBRICA = "";
                destinatario.DESCRIZIONE = this.btn_ruolo_prop.Text;
                destinatario.CHA_TIPO_URP = "R";
                destinatario.ID_CORR_GLOBALI = Convert.ToInt32("0");
               
                destinatari.Add(destinatario);
                AggiornaRagioneDest();
                AddMittDest(ragione, destinatari, cha_tipo_ragione);
                caricaDataGridDest();

            }
        }

        protected void btn_Uo_prop_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dt_dest.Items.Count; i++)
            {
                string ragione = dt_dest.Items[i].Cells[1].Text;
                if (ragione.ToUpper().Equals("CESSIONE") && ddl_ragioni.SelectedItem.Text == "CESSIONE")
                {
                    RegisterStartupScript("alert", "<script>alert('Impossibile inserire destinatario, il modello contiene una ragione di trasmissione cessione');</script>");
                    return;
                }
            }
            //if (ddl_registri.SelectedValue == "")
            //{
            //    RegisterStartupScript("alert", "<script>alert('Attenzione! Selezionare un registro!');</script>");
            //    return;
            //}

            if (this.dt_mitt != null && this.dt_mitt.Items.Count == 0 && rb_tipo_mittente.SelectedValue.Equals("R"))
            {
                msg_ConfirmDel.Alert("Inserire il mittente del modello.");
            }
            else
            {
                string ragione = this.ddl_ragioni.SelectedItem.Text;
                ArrayList destinatari = new ArrayList();
                DocsPaWR.MittDest destinatario = new DocsPAWA.DocsPaWR.MittDest();
                destinatario.CHA_TIPO_MITT_DEST = "UO_P";
                destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                destinatario.VAR_COD_RUBRICA = "";
                destinatario.DESCRIZIONE = this.btn_Uo_prop.Text;
                destinatario.CHA_TIPO_URP = "U";
                destinatario.ID_CORR_GLOBALI = Convert.ToInt32("0");
                destinatari.Add(destinatario);
                AggiornaRagioneDest();
                AddMittDest(ragione, destinatari, cha_tipo_ragione);
                caricaDataGridDest();

            }
        }

        protected void btn_resp_uo_prop_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dt_dest.Items.Count; i++)
            {
                string ragione = dt_dest.Items[i].Cells[1].Text;
                if (ragione.ToUpper().Equals("CESSIONE") && ddl_ragioni.SelectedItem.Text == "CESSIONE")
                {
                    RegisterStartupScript("alert", "<script>alert('Impossibile inserire destinatario, il modello contiene una ragione di trasmissione cessione');</script>");
                    return;
                }
            }
            //if (ddl_registri.SelectedValue == "")
            //{
            //    RegisterStartupScript("alert", "<script>alert('Attenzione! Selezionare un registro!');</script>");
            //    return;
            //}

            if (this.dt_mitt != null && this.dt_mitt.Items.Count == 0 && rb_tipo_mittente.SelectedValue.Equals("R"))
            {
                msg_ConfirmDel.Alert("Inserire il mittente del modello.");
            }
            else
            {
                string ragione = this.ddl_ragioni.SelectedItem.Text;
                ArrayList destinatari = new ArrayList();
                DocsPaWR.MittDest destinatario = new DocsPAWA.DocsPaWR.MittDest();
                destinatario.CHA_TIPO_MITT_DEST = "RSP_P";
                destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                destinatario.VAR_COD_RUBRICA = "";
                destinatario.DESCRIZIONE = this.btn_resp_uo_prop.Text;
                destinatario.CHA_TIPO_URP = "R";
                destinatario.ID_CORR_GLOBALI = Convert.ToInt32("0");
                destinatari.Add(destinatario);
                AggiornaRagioneDest();
                AddMittDest(ragione, destinatari, cha_tipo_ragione);
                caricaDataGridDest();

            }
        }
        private void cancellaMittente(string codiceRubrica)
        {
            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
            if(modello.MITTENTE.Length==1)
                modello.RAGIONI_DESTINATARI = null;
            modello.ID_REGISTRO = this.ddl_registri.SelectedValue;
            for (int i = 0; i < modello.MITTENTE.Length; i++)
            {                
                if (((DocsPaWR.MittDest)modello.MITTENTE[i]).VAR_COD_RUBRICA.Equals(codiceRubrica))
                {
                    ArrayList appoggio = new ArrayList(modello.MITTENTE);
                    appoggio.RemoveAt(i);
                    modello.MITTENTE = new DocsPaWR.MittDest[appoggio.Count];
                    appoggio.CopyTo(modello.MITTENTE);
                    break;
                }
            }
            Session.Add("Modello", modello);
        }
        
        private void addMitt(ArrayList mittenti)
        {
            ArrayList array_1;
            DocsPaWR.ModelloTrasmissione modello = ((DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"]);
            //se già ci sono dei destinatari li mantengo aggiungendo quelli nuovi selezionati da rubrica
           
                if (modello.MITTENTE!=null && modello.MITTENTE.Length>0)
                {
                    array_1 = new ArrayList(modello.MITTENTE);
                    verificaEsistenzaMitt(array_1, ref mittenti);
                    array_1.AddRange(mittenti);
                    modello.MITTENTE = new DocsPAWA.DocsPaWR.MittDest[array_1.Count];
                    array_1.CopyTo(modello.MITTENTE);
                }
                else
                {
                    modello.MITTENTE = new DocsPAWA.DocsPaWR.MittDest[mittenti.Count];
                    mittenti.CopyTo(modello.MITTENTE);
                }

            Session.Add("Modello", modello);
        }

        private void verificaEsistenzaMitt(ArrayList mittVecchi, ref ArrayList mittNuovi)
        {
            ArrayList elNuovi_1 = new ArrayList();
            bool result = true;
            for (int i = 0; i < mittNuovi.Count; i++)
            {
                for (int j = 0; j < mittVecchi.Count; j++)
                {
                    if (((DocsPAWA.DocsPaWR.MittDest)mittNuovi[i]).VAR_COD_RUBRICA == ((DocsPAWA.DocsPaWR.MittDest)mittVecchi[j]).VAR_COD_RUBRICA && ((DocsPAWA.DocsPaWR.MittDest)mittNuovi[i]).CHA_TIPO_MITT_DEST == "M")
                    {
                        result = false;
                    }                    
                }
                if (result)
                {
                    elNuovi_1.Add(mittNuovi[i]);
                }
                result = true;
            }
            mittNuovi.Clear();
            mittNuovi.AddRange(elNuovi_1);
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
                this.dt_dest.Columns.RemoveAt(this.dt_dest.Columns.Count - 2);
        }

        protected void btn_ruolo_segretario_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dt_dest.Items.Count; i++)
            {
                string ragione = dt_dest.Items[i].Cells[1].Text;
                if (ragione.ToUpper().Equals("CESSIONE") && ddl_ragioni.SelectedItem.Text == "CESSIONE")
                {
                    RegisterStartupScript("alert", "<script>alert('Impossibile inserire destinatario, il modello contiene una ragione di trasmissione cessione');</script>");
                    return;
                }
            }
            //if (ddl_registri.SelectedValue == "")
            //{
            //    RegisterStartupScript("alert", "<script>alert('Attenzione! Selezionare un registro!');</script>");
            //    return;
            //}

            if (this.dt_mitt != null && this.dt_mitt.Items.Count == 0 && rb_tipo_mittente.SelectedValue.Equals("R"))
            {
                msg_ConfirmDel.Alert("Inserire il mittente del modello.");
            }
            else
            {
                string ragione = this.ddl_ragioni.SelectedItem.Text;
                ArrayList destinatari = new ArrayList();
                DocsPaWR.MittDest destinatario = new DocsPAWA.DocsPaWR.MittDest();
                destinatario.CHA_TIPO_MITT_DEST = "R_S";
                destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                destinatario.VAR_COD_RUBRICA = "";
                destinatario.DESCRIZIONE = this.btn_ruolo_segretario.Text;
                destinatario.CHA_TIPO_URP = "R";
                destinatario.ID_CORR_GLOBALI = Convert.ToInt32("0");
                destinatari.Add(destinatario);
                AggiornaRagioneDest();
                AddMittDest(ragione, destinatari, cha_tipo_ragione);
                caricaDataGridDest();

            }
        }



        protected void btn_resp_uo_mitt_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dt_dest.Items.Count; i++)
            {
                string ragione = dt_dest.Items[i].Cells[1].Text;
                if (ragione.ToUpper().Equals("CESSIONE") && ddl_ragioni.SelectedItem.Text == "CESSIONE")
                {
                    RegisterStartupScript("alert", "<script>alert('Impossibile inserire destinatario, il modello contiene una ragione di trasmissione cessione');</script>");
                    return;
                }
            }
            //if (ddl_registri.SelectedValue == "")
            //{
            //    RegisterStartupScript("alert", "<script>alert('Attenzione! Selezionare un registro!');</script>");
            //    return;
            //}

            if (this.dt_mitt != null && this.dt_mitt.Items.Count == 0 && rb_tipo_mittente.SelectedValue.Equals("R"))
            {
                msg_ConfirmDel.Alert("Inserire il mittente del modello.");
            }
            else
            {
                string ragione = this.ddl_ragioni.SelectedItem.Text;
                ArrayList destinatari = new ArrayList();
                DocsPaWR.MittDest destinatario = new DocsPAWA.DocsPaWR.MittDest();
                destinatario.CHA_TIPO_MITT_DEST = "RSP_M";
                destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                destinatario.VAR_COD_RUBRICA = "";
                destinatario.DESCRIZIONE = this.btn_resp_uo_mitt.Text;
                destinatario.CHA_TIPO_URP = "R";
                destinatario.ID_CORR_GLOBALI = Convert.ToInt32("0");
                destinatari.Add(destinatario);
                AggiornaRagioneDest();
                AddMittDest(ragione, destinatari, cha_tipo_ragione);
                caricaDataGridDest();

            }
        }

        protected void btn_segr_uo_mitt_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dt_dest.Items.Count; i++)
            {
                string ragione = dt_dest.Items[i].Cells[1].Text;
                if (ragione.ToUpper().Equals("CESSIONE") && ddl_ragioni.SelectedItem.Text == "CESSIONE")
                {
                    RegisterStartupScript("alert", "<script>alert('Impossibile inserire destinatario, il modello contiene una ragione di trasmissione cessione');</script>");
                    return;
                }
            }
            //if (ddl_registri.SelectedValue == "")
            //{
            //    RegisterStartupScript("alert", "<script>alert('Attenzione! Selezionare un registro!');</script>");
            //    return;
            //}

            if (this.dt_mitt != null && this.dt_mitt.Items.Count == 0 && rb_tipo_mittente.SelectedValue.Equals("R"))
            {
                msg_ConfirmDel.Alert("Inserire il mittente del modello.");
            }
            else
            {
                string ragione = this.ddl_ragioni.SelectedItem.Text;
                ArrayList destinatari = new ArrayList();
                DocsPaWR.MittDest destinatario = new DocsPAWA.DocsPaWR.MittDest();
                destinatario.CHA_TIPO_MITT_DEST = "S_M";
                destinatario.ID_RAGIONE = Convert.ToInt32(this.ddl_ragioni.SelectedValue);
                destinatario.VAR_COD_RUBRICA = "";
                destinatario.DESCRIZIONE = this.btn_segr_uo_mitt.Text;
                destinatario.CHA_TIPO_URP = "R";
                destinatario.ID_CORR_GLOBALI = Convert.ToInt32("0");
                destinatari.Add(destinatario);
                AggiornaRagioneDest();
                AddMittDest(ragione, destinatari, cha_tipo_ragione);
                caricaDataGridDest();

            }
        }

        /// <summary>
        /// Funzione per la ricerca e la visualizzazione di modelli che rispettasno i criteri impostati
        /// attraverso i filtri di ricerca
        /// </summary>
        private void Search()
        {
            modelliTrasmissione = new ArrayList(wws.getModelliByDdlAmmPaging(idAmministrazione, 0, this.prmtPannelloRicerca.CreateSearchFilters(), out numTotModelli));
            caricaDataGridModelli();
        }



    }
}
