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
using System.Text.RegularExpressions;
using System.Globalization;
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;

namespace DocsPAWA.ricercaDoc
{
	/// <summary>
	/// Summary description for completamento.
	/// </summary>
	public class f_Ricerca_Compl : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label lbl_initdataProt;
		//protected DocsPaWebCtrlLibrary.DateMask txt_initDataProt;
        protected DocsPAWA.UserControls.Calendar txt_initDataProt;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineDataProt;
        protected DocsPAWA.UserControls.Calendar txt_fineDataProt;
        protected System.Web.UI.WebControls.ListBox lb_reg;
		//protected System.Web.UI.WebControls.RadioButtonList rbl_documentiInCompletamento;
        protected System.Web.UI.WebControls.CheckBox cbx_Trasm;
        protected System.Web.UI.WebControls.CheckBox cbx_TrasmSenza;
        protected System.Web.UI.WebControls.DropDownList ddl_ragioneTrasm;
        //protected System.Web.UI.WebControls.CheckBoxList cbl_docInCompl;
		protected System.Web.UI.WebControls.RadioButtonList rbl_Reg;
		protected System.Web.UI.WebControls.TextBox txt_oggetto;
        protected System.Web.UI.WebControls.Label lblSearch;
		private bool isSavedSearch = false;
        //protected UserControls.Creatore Creatore;
        protected Hashtable m_hashTableRagioneTrasmissione;
        protected DocsPAWA.DocsPaWR.RagioneTrasmissione[] listaRagioni;
        protected System.Web.UI.WebControls.CheckBoxList cbl_archDoc_C;
        protected System.Web.UI.WebControls.CheckBox cb_mitt_dest_storicizzati;
        protected System.Web.UI.WebControls.RadioButtonList rbl_immagine;
        protected System.Web.UI.WebControls.RadioButtonList rbl_fascicolazione;
        protected System.Web.UI.WebControls.DropDownList ddl_tipoFileAcquisiti;
        //protected System.Web.UI.WebControls.CheckBox cb_firmato;
        //protected System.Web.UI.WebControls.CheckBox cb_nonFirmato;
        protected System.Web.UI.WebControls.CheckBox chkFirmato;
        protected System.Web.UI.WebControls.CheckBox chkNonFirmato;
        protected System.Web.UI.HtmlControls.HtmlTableRow trNumProto;
        protected System.Web.UI.WebControls.Label star;
        protected System.Web.UI.WebControls.DropDownList ddl_numProt_C;
        protected System.Web.UI.WebControls.Label lblDAnumprot_C;
        protected System.Web.UI.WebControls.TextBox txt_initNumProt_C;
        protected System.Web.UI.WebControls.Label lblAnumprot_C;
        protected System.Web.UI.WebControls.TextBox txt_fineNumProt_C;
        protected System.Web.UI.WebControls.TextBox tbAnnoProtocollo;
        protected System.Web.UI.WebControls.Panel pnl_dataProt;
        protected System.Web.UI.WebControls.RadioButtonList rb_docSpediti;
        protected DocsPAWA.UserControls.Calendar txt_dataSpedDa;
        protected DocsPAWA.UserControls.Calendar txt_dataSpedA;

        protected System.Web.UI.WebControls.ListItem opArr;
        protected System.Web.UI.WebControls.ListItem opPart;
        protected System.Web.UI.WebControls.ListItem opInt;
        protected System.Web.UI.WebControls.ListItem opGrigio;
        protected System.Web.UI.WebControls.ListItem opAll;
        protected DocsPAWA.DocsPaWR.EtichettaInfo[] etichette;
        protected System.Web.UI.WebControls.TextBox txt_codDesc;
        protected System.Web.UI.WebControls.Label l_amm_interop;
        protected System.Web.UI.WebControls.DropDownList ddl_ricevute_pec;
        protected System.Web.UI.WebControls.Label L_da_ricevute_pec;
        protected System.Web.UI.WebControls.Label L_a_ricevute_pec;
        protected System.Web.UI.WebControls.DropDownList ddl_data_ricevute_pec;
        protected DocsPAWA.UserControls.Calendar Cal_Da_pec;
        protected DocsPAWA.UserControls.Calendar Cal_A_pec;
        protected System.Web.UI.WebControls.Panel p_ricevute_pec;
        protected System.Web.UI.WebControls.RadioButtonList rblFiltriAllegati;
        protected System.Web.UI.WebControls.DropDownList ddlOrder, ddlOrderDirection;
        protected System.Web.UI.WebControls.CheckBox cbx_pec;
        protected System.Web.UI.WebControls.CheckBox cbx_pitre;
        protected System.Web.UI.WebControls.DropDownList ddl_ricevute_pitre;
        protected System.Web.UI.WebControls.DropDownList ddl_data_ricevute_pitre;
        protected DocsPAWA.UserControls.Calendar Cal_Da_pitre;
        protected DocsPAWA.UserControls.Calendar Cal_A_pitre;
        protected System.Web.UI.WebControls.Panel p_ricevute_pitre;
        protected System.Web.UI.WebControls.Label L_da_ricevute_pitre;
        protected System.Web.UI.WebControls.Label L_a_ricevute_pitre;

        protected string numResult;

        protected DocsPaWebCtrlLibrary.ImageButton btn_clear_fields;

        /// <summary>
        /// Pannello per la ricerca dei documenti consolidati
        /// </summary>
        protected UserControls.DocumentConsolidationSearchPanel documentConsolidationSearch;

        private const string KEY_SCHEDA_RICERCA="RicercaDocCompletamento";

        protected System.Web.UI.WebControls.Panel p_cod_amm;
        protected bool change_from_grid;

        protected System.Web.UI.WebControls.Button btn_modifica;

        protected DocsPAWA.UserControls.AuthorOwnerFilter aofAuthor, aofOwner;

        //private bool predisposti;
#region variabili codice
		protected DocsPAWA.DocsPaWR.Utente userHome;
		protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
		protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
		protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
		protected System.Web.UI.WebControls.Button btn_ricerca;
		protected System.Web.UI.WebControls.ImageButton btn_RubrOgget;
		protected System.Web.UI.WebControls.TextBox txt_codMit;
		protected System.Web.UI.WebControls.TextBox txt_descrMit;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_systemIdMit_Compl;
		protected System.Web.UI.WebControls.ImageButton enterKeySimulator;
        protected System.Web.UI.WebControls.ImageButton enterKeySimulator2;
        protected System.Web.UI.HtmlControls.HtmlImage btn_Rubrica;
		protected System.Web.UI.WebControls.TextBox txt_numProtMitt_C;
		protected System.Web.UI.WebControls.DropDownList ddl_dataProtMitt_C;
		protected System.Web.UI.WebControls.Label lbl_initdataProtMitt_C;
		//protected DocsPaWebCtrlLibrary.DateMask txt_initDataProtMitt_C;
        protected DocsPAWA.UserControls.Calendar txt_initDataProtMitt_C;
        protected System.Web.UI.WebControls.Label lbl_finedataProtMitt_C;
		//protected DocsPaWebCtrlLibrary.DateMask txt_fineDataProtMitt_C;
        protected DocsPAWA.UserControls.Calendar txt_fineDataProtMitt_C;
        protected System.Web.UI.WebControls.DropDownList ddl_dataProt;
		protected System.Web.UI.WebControls.Label lbl_finedataProt;
		protected System.Web.UI.WebControls.DropDownList ddl_dataCreaz;
		protected System.Web.UI.WebControls.Label lbl_initdataCreaz;
		protected System.Web.UI.WebControls.Label lbl_finedataCreaz;
		//protected DocsPaWebCtrlLibrary.DateMask txt_initDataCreaz;
        protected DocsPAWA.UserControls.Calendar txt_initDataCreaz;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineDataCreaz;
        protected DocsPAWA.UserControls.Calendar txt_fineDataCreaz;
        protected System.Web.UI.WebControls.ImageButton btn_Canc_Ric;
		protected System.Web.UI.WebControls.DropDownList ddl_Ric_Salvate;
		protected System.Web.UI.WebControls.Button btn_salva;
		protected Utilities.MessageBox mb_ConfirmDelete;
		protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
        protected System.Web.UI.WebControls.DropDownList ddl_idDocumento_C;
        protected System.Web.UI.WebControls.TextBox txt_initIdDoc_C;
        protected System.Web.UI.WebControls.TextBox txt_fineIdDoc_C;
        protected System.Web.UI.WebControls.Label lblAidDoc_C;
        protected System.Web.UI.WebControls.Label lblDAidDoc_C;
        //protected System.Web.UI.WebControls.DropDownList ddl_numProt_C;
        //protected System.Web.UI.WebControls.Label lblDAnumprot_C;
        //protected System.Web.UI.WebControls.TextBox txt_initNumProt_C;
        //protected System.Web.UI.WebControls.TextBox txt_fineNumProt_C;
        //protected System.Web.UI.WebControls.Label lblAnumprot_C;
        //protected System.Web.UI.WebControls.TextBox tbAnnoProt;
        protected System.Web.UI.WebControls.Panel pnl_timestamp;
        protected System.Web.UI.WebControls.RadioButtonList rbl_timestamp;
        protected System.Web.UI.WebControls.DropDownList ddl_timestamp;
        
        protected DocsPAWA.UserControls.Calendar date_timestamp;

        protected System.Web.UI.WebControls.Panel pnl_riferimento;

        protected System.Web.UI.WebControls.TextBox txt_rif_mittente;
#endregion	
		public SchedaRicerca schedaRicerca = null;
        protected Dictionary<string, Corrispondente> dic_Corr;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{

                this.Page.MaintainScrollPositionOnPostBack = true;

				Utils.startUp(this);

                if (!IsPostBack)
                {
                    if (Request.QueryString["gridper"] != string.Empty && Request.QueryString["gridper"] != null)
                    {
                        change_from_grid = true;
                    }
                    else
                    {
                        change_from_grid = false;
                    }
                }

                if (Request.QueryString["numRes"] != string.Empty && Request.QueryString["numRes"] != null)
                {
                    this.numResult = Request.QueryString["numRes"];
                }
                else
                {
                    this.numResult = string.Empty;
                }
                
				// Put user code to initialize the page here
				userHome=(DocsPAWA.DocsPaWR.Utente) Session["userData"];
				userRuolo = (DocsPAWA.DocsPaWR.Ruolo) Session["userRuolo"];
				//chiamo il ws della ricerca

                if (string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_NOTIFICHE_PEC)) ||
               !bool.Parse(ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_NOTIFICHE_PEC)))
                {
                    p_ricevute_pec.Visible = false;
                    cbx_pec.Visible = false;
                }
                else
                {
                    p_ricevute_pec.Visible = true;
                    cbx_pec.Visible = true;
                }

                if (string.IsNullOrEmpty(ConfigSettings.getKey(ConfigSettings.KeysENUM.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE)) ||
                    !bool.Parse(ConfigSettings.getKey(ConfigSettings.KeysENUM.ATTIVA_CODICE_DESCRIZIONE_AMMINISTRAZIONE_MITTENTE)))
                {
                    p_cod_amm.Visible = false;
                }

                if (DocsPAWA.utils.InteroperabilitaSemplificataManager.IsEnabledSimpInterop)
                {
                    p_ricevute_pitre.Visible = true;
                    cbx_pitre.Visible = true;
                }
                else
                {
                    p_ricevute_pitre.Visible = false;
                    cbx_pitre.Visible = false;
                }

                if (ddl_ricevute_pec.SelectedIndex == 0)
                {
                    ddl_data_ricevute_pec.Enabled = false;
                    Cal_Da_pec.Visible = false;
                }
                else
                {
                    ddl_data_ricevute_pec.Enabled = true;
                    Cal_Da_pec.Visible = true;
                }

                if (ddl_ricevute_pitre.SelectedIndex == 0)
                {
                    ddl_data_ricevute_pitre.Enabled = false;
                    Cal_Da_pitre.Visible = false;
                }
                else
                {
                    ddl_data_ricevute_pitre.Enabled = true;
                    Cal_Da_pitre.Visible = true;
                }

				schedaRicerca = (SchedaRicerca)Session[SchedaRicerca.SESSION_KEY];
				if (schedaRicerca==null)
				{
					//Inizializzazione della scheda di ricerca per la gestione delle 
					//ricerche salvate
                    schedaRicerca = new SchedaRicerca(KEY_SCHEDA_RICERCA,userHome, userRuolo, this);
					Session[SchedaRicerca.SESSION_KEY] = schedaRicerca;
				}
				schedaRicerca.Pagina = this;
				if (!IsPostBack)
				{
                    this.rbl_fascicolazione.Items.FindByValue("Reset").Selected = true;
                    this.rbl_immagine.Items.FindByValue("Reset").Selected = true;

                    // se ritorno alla pagina di ricerca dopo aver settato una fascicolazione
                    // rapida da un protocollo dopo il riproponi, devo annullare la variabile di 
                    // sessione relativa al template altrimenti il campo codice fascicolo viene
                    // automaticamente valorizzato
                    if (FascicoliManager.getFascicoloSelezionatoFascRapida(this) != null)
                        FascicoliManager.removeFascicoloSelezionatoFascRapida();
                    
                    //verifica se nuova ADL
                    if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1") && (!IsPostBack))
                    {
                        schedaRicerca.ElencoRicercheADL("D", false, ddl_Ric_Salvate, null);
                        isSavedSearch = PopulateField(schedaRicerca.FiltriRicerca, false);
                    }
                    else
                    {
					    schedaRicerca.ElencoRicerche("D", ddl_Ric_Salvate);
                        isSavedSearch = PopulateField(schedaRicerca.FiltriRicerca, false);
                    }
				}

                if (!IsPostBack)
                {
                    caricaRagioni(schedaRicerca.FiltriRicerca);
                    caricaComboTipoFileAcquisiti();
                    //this.ddl_numProt_C.SelectedIndex = 0;
                    //this.lblAnumprot_C.Visible = false;
                    //this.lblDAnumprot_C.Visible = false;
                    //this.txt_fineNumProt_C.Visible = false;

                    //if (schedaRicerca.FiltriRicerca != null)
                    //{
                    //    //carico il creatore, se esiste
                    //    DocsPaWR.Corrispondente creator = UserManager.getCreatoreSelezionato(this);
                    //    if (creator != null)
                    //    {
                    //        this.Creatore.RestoreCurrentFilters();
                    //    }
                    //    UserManager.removeCreatoreSelezionato(this);
                    //}

                    DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
                    
                    #region ABILITAZIONE PROTOCOLLAZIONE INTERNA
                    DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    if (!ws.IsInternalProtocolEnabled(cr.idAmministrazione)) this.cbl_archDoc_C.Items.Remove(this.cbl_archDoc_C.Items[2]); // this.rb_archDoc_E.Items.Remove(this.rb_archDoc_E.Items[2]);
                    #endregion

                    #region Abilitazione ricerca allegati
                    if (!this.IsEnabledProfilazioneAllegato)
                        this.cbl_archDoc_C.Items.Remove(this.cbl_archDoc_C.Items.FindByValue("ALL"));
                    #endregion

                    DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    if (wws.isEnableRiferimentiMittente())
                    {
                        this.pnl_riferimento.Visible = true;
                    }
                }

				if(!IsPostBack)
				{
					//set data corrente corrente al page load, ma non ap postback
					//this.txt_initDataProt.Text=Utils.getDataOdiernaDocspa();

					//viene messo il focus sul campo per la ricerca del numero di protocollo
                    //string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_oggetto.ID + "').focus();</SCRIPT>";
                    //RegisterStartupScript("focus", s);
                    if (!isSavedSearch)
                    {
                        if (!((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1")))
                            this.tbAnnoProtocollo.Text = System.DateTime.Now.Year.ToString();
                    }

					// Visualizzazione pagina di ricerca nella selezione 
					// di un criterio di ricerca salvato
					this.ddl_Ric_Salvate.Attributes.Add("onChange","OnChangeSavedFilter();");
	
					// attenzione: se vengo da un back di elemento di una ricerca salvata
					// devo comportarmi diversamente
					if (!isSavedSearch)
					{
                        this.lblDAnumprot_C.Visible = false;
                        this.lblAnumprot_C.Visible = false;
						this.GetCalendarControl("txt_fineDataProt").txt_Data.Visible=false;
                        this.GetCalendarControl("txt_fineDataProt").btn_Cal.Visible = false;
						this.lbl_finedataProt.Visible=false;
						this.lbl_initdataProt.Visible=false;
						this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Visible=false;
                        this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Visible = false;
						this.lbl_finedataCreaz.Visible=false;
						this.lbl_initdataCreaz.Visible=false;
						this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible=false;
                        this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = false;
						this.lbl_finedataProtMitt_C.Visible=false;
						this.lbl_initdataProtMitt_C.Visible=false;
                        this.lblDAnumprot_C.Visible = false;
                        this.txt_fineNumProt_C.Visible = false;
                        this.L_da_ricevute_pec.Visible = false;
                        this.L_a_ricevute_pec.Visible = false;
                        this.L_da_ricevute_pitre.Visible = false;
                        this.L_a_ricevute_pitre.Visible = false;
                    }
                    
                    setListaRegistri();
                    setFormProperties();
                }

				//carico il mittente selezionato, se esiste
				DocsPaWR.Corrispondente cor = UserManager.getCorrispondenteSelezionato(this);
				if (cor != null) 
				{				
					this.txt_codMit.Text = cor.codiceRubrica;
					this.txt_descrMit.Text = UserManager.getDecrizioneCorrispondenteSemplice(cor);
                    this.hd_systemIdMit_Compl.Value = cor.systemId;
				}
				UserManager.removeCorrispondentiSelezionati(this);
          
                tastoInvio();
                caricaTrasmissioni();
                //new ADL
                if (!IsPostBack &&
                    Request.QueryString["ricADL"] != null &&
                    Request.QueryString["ricADL"] == "1" &&
                    !SiteNavigation.CallContextStack.CurrentContext.IsBack
                    )
                {
                    lblSearch.Text = "Ricerche Salvate Area di Lavoro";
                    this.btn_ricerca_Click(null, null);
                }

                getLettereProtocolli();

                //Timestamp
                bool enableTimestamp = false;
                Boolean.TryParse(ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_TIMESTAMP_DOC), out enableTimestamp);
                if (enableTimestamp)
                        pnl_timestamp.Visible = true;

                if (Session["idRicSalvata"] != null && !string.IsNullOrEmpty(Session["idRicSalvata"].ToString()))
                {
                    ddl_Ric_Salvate.SelectedValue = Session["idRicSalvata"].ToString();
                    Session.Remove("idRicSalvata");
                    string altro = string.Empty;

                    if (!string.IsNullOrEmpty(this.numResult) && this.numResult.Equals("0"))
                    {
                        altro = "&noRic=1";
                    }

                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=completamento&tabRes=completamento" + altro + "';", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?from=completamento&tabRes=completamento" + altro + "';", true);
                    }
                }

                if (this.ddl_Ric_Salvate.SelectedIndex == 0)
                {
                    this.btn_modifica.Enabled = false;
                }
                else
                {
                    this.btn_modifica.Enabled = true;
                }

                if (!IsPostBack)
                {
                    this.btn_clear_fields.Attributes.Add("onmouseout", "this.src='" + "../images/ricerca/remove_search_filter.gif'");
                    this.btn_clear_fields.Attributes.Add("onmouseover", "this.src='" + "../images/ricerca/remove_search_filter_up.gif'");
                }

                #region Gestione filtro Allegati
                if (this.IsEnabledProfilazioneAllegato && cbl_archDoc_C.Items.FindByValue("ALL").Selected)
                    rblFiltriAllegati.Style.Add("display", "block");
                else if (this.IsEnabledProfilazioneAllegato && (!cbl_archDoc_C.Items.FindByValue("ALL").Selected))
                    rblFiltriAllegati.Style.Add("display", "none");
                if (this.IsEnabledProfilazioneAllegato)
                {
                    int countItem = -1;
                    foreach (ListItem i in cbl_archDoc_C.Items)
                    {
                        ++countItem;
                        if (i.Value.Equals("ALL"))
                        {
                            string scriptGestAll = "<script language='javascript'>" +
                                "function SetVisibilityFilterAlleg() { " +
                                "if (document.getElementById('cbl_archDoc_C_" + countItem + "') != null && " +
                                "document.getElementById('cbl_archDoc_C_" + countItem + "').checked) { " +
                                "document.getElementById('rblFiltriAllegati').style.display = 'block'; " +
                                "} " +
                                "else " +
                                "{ " +
                                " document.getElementById('rblFiltriAllegati').style.display = 'none'; " +
                                "} " +
                            "} " +
                        "</script>";
                            ClientScript.RegisterStartupScript(this.GetType(), "enable_disable_all", scriptGestAll);
                            string scriptClickAll = "<script language='javascript'> " +
                                "var cbo_all = document.getElementById('cbl_archDoc_C_" + countItem + "'); " +
                                "if(cbo_all.addEventListener){ " +
                                "cbo_all.addEventListener('clik',SetVisibilityFilterAlleg); " +
                                "} else { " +
                                "cbo_all.attachEvent('onclick', SetVisibilityFilterAlleg); " +
                                "} " +
                                "</script>";
                            ClientScript.RegisterStartupScript(this.GetType(), "event_click_all", scriptClickAll);
                            break;
                        }
                    }
                }
                #endregion
			}
			catch (System.Exception ex)
			{
				ErrorManager.redirect(this, ex);
			}
		}

        public bool IsValidYear(string strYear)
        {
            Regex onlyNumberPattern = new Regex(@"\d{4}");
            return onlyNumberPattern.IsMatch(strYear);
        }

        private void caricaTrasmissioni()
        {
            if (this.Page.IsPostBack)
            {
                if (this.cbx_Trasm.Checked)
                    this.cbx_TrasmSenza.Checked = false;

                if (this.cbx_TrasmSenza.Checked)
                    this.cbx_Trasm.Checked = false;
            }
        }

        private void caricaRagioni(DocsPAWA.DocsPaWR.FiltroRicerca[][] qV)
        {
            //listaRagioni = TrasmManager.getListaRagioni(this, null);
            //true : vengo da ricerca trasmissioni cerco tutte le ragioni
            //false : ricerca trasm con cha_vis='1'

            listaRagioni = TrasmManager.getListaRagioni(this, null, true);

            if (!Page.IsPostBack)
            {
                m_hashTableRagioneTrasmissione = new Hashtable();
                if (listaRagioni != null && listaRagioni.Length > 0)
                {
                    ddl_ragioneTrasm.Items.Add("Tutte");
                    for (int i = 0; i < listaRagioni.Length; i++)
                    {
                        m_hashTableRagioneTrasmissione.Add(i, listaRagioni[i]);

                        ListItem newItem = new ListItem(listaRagioni[i].descrizione, listaRagioni[i].systemId);
                        ddl_ragioneTrasm.Items.Add(newItem);
                    }
                    TrasmManager.setHashRagioneTrasmissione(this, m_hashTableRagioneTrasmissione);

                    this.ddl_ragioneTrasm.SelectedIndex = 0;
                }
            }
            else
            {
                m_hashTableRagioneTrasmissione = TrasmManager.getHashRagioneTrasmissione(this);
            }

            if (qV != null)
            {
                foreach (DocsPAWA.DocsPaWR.FiltroRicerca[] filterArray in qV)
                    foreach (DocsPAWA.DocsPaWR.FiltroRicerca filterItem in filterArray)
                    {
                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_CON.ToString())
                        {
                            this.ddl_ragioneTrasm.SelectedItem.Text = filterItem.valore;
                            this.cbx_Trasm.Checked = true;
                        }
                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_SENZA.ToString())
                        {
                            this.ddl_ragioneTrasm.SelectedItem.Text = filterItem.valore;
                            this.cbx_TrasmSenza.Checked = true;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString())
                        {
                            this.cbl_archDoc_C.Items.FindByValue("A").Selected = Convert.ToBoolean(filterItem.valore);
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString())
                        {
                            this.cbl_archDoc_C.Items.FindByValue("P").Selected = Convert.ToBoolean(filterItem.valore);
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString())
                        {
                            if (this.cbl_archDoc_C.Items.FindByValue("I") != null)
                                this.cbl_archDoc_C.Items.FindByValue("I").Selected = Convert.ToBoolean(filterItem.valore);
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.GRIGIO.ToString())
                        {
                            this.cbl_archDoc_C.Items.FindByValue("G").Selected = Convert.ToBoolean(filterItem.valore);
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString())
                        {
                            this.cbl_archDoc_C.Items.FindByValue("Pr").Selected = Convert.ToBoolean(filterItem.valore);
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString())
                        {
                            if (filterItem.valore.Equals("1"))
                                this.rbl_immagine.Items.FindByValue("1").Selected = true;
                            if (filterItem.valore.Equals("0"))
                                this.rbl_immagine.Items.FindByValue("0").Selected = true;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString())
                        {
                            if (filterItem.valore.Equals("1"))
                                this.rbl_fascicolazione.Items.FindByValue("1").Selected = true;
                            if (filterItem.valore.Equals("0"))
                                this.rbl_fascicolazione.Items.FindByValue("0").Selected = true;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.REGISTRO.ToString())
                        {
                            char[] sep = { ',' };
                            string[] regs = filterItem.valore.Split(sep);
                            foreach (ListItem li in lb_reg.Items)
                                li.Selected = false;
                            foreach (string reg in regs)
                            {
                                for (int i = 0; i < lb_reg.Items.Count; i++)
                                {
                                    if (lb_reg.Items[i].Value == reg)
                                        lb_reg.Items[i].Selected = true;
                                }
                            }
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.OGGETTO.ToString())
                        {
                            txt_oggetto.Text = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.MITT_DEST.ToString())
                        {
                            txt_descrMit.Text = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString())
                        {
                            DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemID(this, filterItem.valore);
                            txt_codMit.Text = corr.codiceRubrica;
                            txt_descrMit.Text = corr.descrizione;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString())
                        {
                            if (ddl_dataProt.SelectedIndex != 0)
                                ddl_dataProt.SelectedIndex = 0;
                            ddl_dataProt_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataProt").txt_Data.Text = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataProt.SelectedIndex != 1)
                                ddl_dataProt.SelectedIndex = 1;
                            ddl_dataProt_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataProt").txt_Data.Text = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataProt.SelectedIndex != 1)
                                ddl_dataProt.SelectedIndex = 1;
                            ddl_dataProt_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_fineDataProt").txt_Data.Text = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString())
                        {
                            if (ddl_dataCreaz.SelectedIndex != 0)
                                ddl_dataCreaz.SelectedIndex = 0;
                            ddl_dataCreaz_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataCreaz.SelectedIndex != 1)
                                ddl_dataCreaz.SelectedIndex = 1;
                            ddl_dataCreaz_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataCreaz.SelectedIndex != 1)
                                ddl_dataCreaz.SelectedIndex = 1;
                            ddl_dataCreaz_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.PROTOCOLLO_MITTENTE.ToString())
                        {
                            txt_numProtMitt_C.Text = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_IL.ToString())
                        {
                            if (ddl_dataProtMitt_C.SelectedIndex != 0)
                                ddl_dataProtMitt_C.SelectedIndex = 0;
                            ddl_dataProtMitt_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataProtMitt_C.SelectedIndex != 1)
                                ddl_dataProtMitt_C.SelectedIndex = 1;
                            ddl_dataProtMitt_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataProtMitt_C.SelectedIndex != 1)
                                ddl_dataProtMitt_C.SelectedIndex = 1;
                            ddl_dataProtMitt_C_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_CON.ToString())
                        {
                            this.cbx_Trasm.Checked = true;
                            if (filterItem.valore.Equals("Tutte"))
                                this.ddl_ragioneTrasm.SelectedIndex = 0;
                            else
                                this.ddl_ragioneTrasm.SelectedValue = filterItem.valore;
                        }

                        if (filterItem.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_SENZA.ToString())
                        {
                            this.cbx_TrasmSenza.Checked = true;
                            if (filterItem.valore.Equals("Tutte"))
                                this.ddl_ragioneTrasm.SelectedIndex = 0;
                            else
                                this.ddl_ragioneTrasm.SelectedValue = filterItem.valore;
                        }
                    }
            }
        }

		private void setListaRegistri()
		{
            //for (int i=0;i<userRuolo.registri.Length;i++)
            //{
            //    //lb_reg.Items.Add(userRuolo.registri[i].descrizione);
            //    lb_reg.Items.Add(userRuolo.registri[i].codRegistro);
            //    lb_reg.Items[i].Value=userRuolo.registri[i].systemId;
            //    string nomeRegCurrente="UserReg"+i;
            //    // SELEZIONA TUTTI I REGISTRI PRESENTI per DEFAULT
            //    lb_reg.Items[i].Selected = true;
            //}
            //lb_reg.Items[0].Selected=true;


            bool filtroAoo = false;
            DocsPaWR.Registro[] userRegistri = UserManager.getListaRegistriNoFiltroAOO(this, out filtroAoo);

            //DocsPaWR.Registro[] registri = UserManager.getRuolo(this).registri;
            //string[] listaReg = new string[registri.Length];
            if (userRegistri != null && filtroAoo)
            {
                ListItem itemM = new ListItem("I miei", "M");
                rbl_Reg.Items.Add(itemM);
                itemM = new ListItem("Tutti", "T");
                rbl_Reg.Items.Add(itemM);
                itemM = new ListItem("Reset", "R");
                rbl_Reg.Items.Add(itemM);
                lb_reg.Rows = 5;
            }
            else
            {
                userRegistri = UserManager.getRuolo(this).registri;
                ListItem itemM = new ListItem("Tutti", "T");
                rbl_Reg.Items.Add(itemM);
                itemM = new ListItem("Reset", "R");
                rbl_Reg.Items.Add(itemM);
                //rbl_Reg_E.SelectedIndex = 1;
            }
            rbl_Reg.SelectedIndex = 0;
            string[] id = new string[userRegistri.Length];
            for (int i = 0; i < userRegistri.Length; i++)
            {
                lb_reg.Items.Add(userRegistri[i].codRegistro);
                lb_reg.Items[i].Value = userRegistri[i].systemId;
                string nomeRegCurrente = "UserReg" + i;
                // SELEZIONA TUTTI I REGISTRI PRESENTI per DEFAULT
                if (!filtroAoo)
                {
                    if (!userRegistri[i].flag_pregresso)
                        lb_reg.Items[i].Selected = true;
                }
                else
                    if (rbl_Reg.SelectedItem.Value == "M")
                        for (int j = 0; j < UserManager.getRuolo(this).registri.Length; j++)
                        {
                            if (UserManager.getRuolo(this).registri[j].codRegistro == lb_reg.Items[i].Text)
                            {
                                if (!userRegistri[i].flag_pregresso)
                                {
                                    lb_reg.Items[i].Selected = true;
                                    break;
                                }
                            }
                        }

                id[i] = (string)userRegistri[i].systemId;
            }

		}

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }

		private void setFormProperties()
		{
			this.btn_RubrOgget.Attributes.Add("onclick","ApriOggettario('ric_CT');");
			//commentato per problema minimizzazione rubrica
			//this.btn_Rubrica.Attributes.Add("onclick","ApriRubrica('ric_CT','');");
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
			this.enterKeySimulator.Click += new System.Web.UI.ImageClickEventHandler(this.enterKeySimulator_Click);
			this.ddl_Ric_Salvate.SelectedIndexChanged += new System.EventHandler(this.ddl_Ric_Salvate_SelectedIndexChanged);
			this.btn_Canc_Ric.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Canc_Ric_Click);
			//this.rbl_documentiInCompletamento.SelectedIndexChanged += new System.EventHandler(this.rbl_documentiInCompletamento_SelectedIndexChanged);
			this.lb_reg.SelectedIndexChanged += new System.EventHandler(this.lb_reg_SelectedIndexChanged);
			this.rbl_Reg.SelectedIndexChanged += new System.EventHandler(this.rbl_Reg_SelectedIndexChanged);
			this.txt_codMit.TextChanged += new System.EventHandler(this.txt_codMit_TextChanged);
			this.ddl_dataProt.SelectedIndexChanged += new System.EventHandler(this.ddl_dataProt_SelectedIndexChanged);
			this.ddl_dataCreaz.SelectedIndexChanged += new System.EventHandler(this.ddl_dataCreaz_SelectedIndexChanged);
			this.ddl_dataProtMitt_C.SelectedIndexChanged += new System.EventHandler(this.ddl_dataProtMitt_C_SelectedIndexChanged);
			this.btn_ricerca.Click += new System.EventHandler(this.btn_ricerca_Click);
			this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
			this.mb_ConfirmDelete.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.mb_ConfirmDelete_GetMessageBoxResponse);
			this.ID = "f_Ricerca_Compl";
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.f_Ricerca_Compl_PreRender);
            this.ddl_idDocumento_C.SelectedIndexChanged += new System.EventHandler(this.ddl_idDocumento_C_SelectedIndexChanged);
            this.ddl_numProt_C.SelectedIndexChanged += new System.EventHandler(this.ddl_numProt_C_SelectedIndexChanged);
            this.ddl_ricevute_pec.SelectedIndexChanged += new System.EventHandler(this.ddl_ricevute_pec_SelectedIndexChanged);
            this.ddl_data_ricevute_pec.SelectedIndexChanged += new System.EventHandler(this.ddl_data_ricevute_pec_SelectedIndexChanged);
            this.ddl_data_ricevute_pitre.SelectedIndexChanged += new System.EventHandler(this.ddl_data_ricevute_pitre_SelectedIndexChanged);
            this.rbl_timestamp.SelectedIndexChanged += new System.EventHandler(this.rbl_timestamp_SelectedIndexChanged);
            this.ddl_timestamp.SelectedIndexChanged += new System.EventHandler(ddl_timestamp_SelectedIndexChanged);
            this.cb_mitt_dest_storicizzati.CheckedChanged += new System.EventHandler(this.cb_mitt_dest_storicizzatiCheckedChanged);
            this.btn_clear_fields.Click += new ImageClickEventHandler(this.CleanCorrFilters);
            this.rb_docSpediti.SelectedIndexChanged += new EventHandler(rb_docSpediti_SelectedIndexChanged);

            // Se è attiva l'interoperabilità semplificata, viene aggiunta una voce che consente di
            // filtrare per ricevute Interoperabilità semplificata
            if (InteroperabilitaSemplificataManager.IsEnabledSimpInterop)
                this.rblFiltriAllegati.Items.Add(
                    new ListItem(
                        InteroperabilitaSemplificataManager.SearchItemDescriprion,
                        InteroperabilitaSemplificataManager.SimplifiedInteroperabilityId));
        }

        void rb_docSpediti_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rb_docSpediti.SelectedItem.Value != "R")
            {
                if (cbx_pec.Visible)
                    cbx_pec.Checked = true;
                if (cbx_pitre.Visible)
                    cbx_pitre.Checked = true;
            }
            else
            {
                if (cbx_pec.Visible)
                    cbx_pec.Checked = false;
                if (cbx_pitre.Visible)
                    cbx_pitre.Checked = false;
            }
        }

        protected void CleanCorrFilters(object sender, EventArgs e)
        {
            this.aofOwner.ClearFilters();
            this.aofAuthor.ClearFilters();
        }
		#endregion

        //private void rbl_documentiInCompletamento_SelectedIndexChanged(object sender, System.EventArgs e)
        //{
		
        //}
        
        protected void ddl_ricevute_pec_SelectedIndexChanged(object sender, System.EventArgs e)
        {
		
        }

        protected void cb_mitt_dest_storicizzatiCheckedChanged(object sender, EventArgs e)
        {
            try
            {
                setDescMittente(this.txt_codMit.Text, "Mit");
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }

        protected void ddl_data_ricevute_pec_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_data_ricevute_pec.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("Cal_Da_pec").Visible = true;
                    this.GetCalendarControl("Cal_Da_pec").btn_Cal.Visible = true;
                    this.GetCalendarControl("Cal_Da_pec").btn_Cal.Enabled = true;
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Visible = true;
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Enabled = true;
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Text = string.Empty;
                    this.GetCalendarControl("Cal_A_pec").Visible = false;
                    this.GetCalendarControl("Cal_A_pec").btn_Cal.Visible = false;
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Visible = false;
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Text = string.Empty;
                    this.L_a_ricevute_pec.Visible = false;
                    this.L_da_ricevute_pec.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("Cal_Da_pec").Visible = true;
                    this.GetCalendarControl("Cal_Da_pec").btn_Cal.Visible = true;
                    this.GetCalendarControl("Cal_Da_pec").btn_Cal.Enabled = true;
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Visible = true;
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Enabled = true;
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Text = string.Empty;
                    this.GetCalendarControl("Cal_A_pec").Visible = true;
                    this.GetCalendarControl("Cal_A_pec").btn_Cal.Visible = true;
                    this.GetCalendarControl("Cal_A_pec").btn_Cal.Enabled = true;
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Visible = true;
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Enabled = true;
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Text = string.Empty;
                    this.L_a_ricevute_pec.Visible = true;
                    this.L_da_ricevute_pec.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("Cal_Da_pec").Visible = true;
                    this.GetCalendarControl("Cal_Da_pec").btn_Cal.Visible = true;
                    this.GetCalendarControl("Cal_Da_pec").btn_Cal.Enabled = false;
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Visible = true;
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Enabled = false;
                    this.GetCalendarControl("Cal_A_pec").Visible = false;
                    this.GetCalendarControl("Cal_A_pec").btn_Cal.Visible = false;
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Visible = false;
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Text = string.Empty;
                    this.L_a_ricevute_pec.Visible = false;
                    this.L_da_ricevute_pec.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("Cal_Da_pec").Visible = true;
                    this.GetCalendarControl("Cal_Da_pec").btn_Cal.Enabled = false;
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Enabled = false;
                    this.GetCalendarControl("Cal_A_pec").Visible = true;
                    this.GetCalendarControl("Cal_A_pec").btn_Cal.Visible = true;
                    this.GetCalendarControl("Cal_A_pec").btn_Cal.Enabled = false;
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Visible = true;
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Enabled = false;
                    this.L_a_ricevute_pec.Visible = true;
                    this.L_da_ricevute_pec.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("Cal_Da_pec").Visible = true;
                    this.GetCalendarControl("Cal_Da_pec").btn_Cal.Enabled = false;
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("Cal_Da_pec").txt_Data.Enabled = false;
                    this.GetCalendarControl("Cal_A_pec").Visible = true;
                    this.GetCalendarControl("Cal_A_pec").btn_Cal.Visible = true;
                    this.GetCalendarControl("Cal_A_pec").btn_Cal.Enabled = false;
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Visible = true;
                    this.GetCalendarControl("Cal_A_pec").txt_Data.Enabled = false;
                    this.L_a_ricevute_pec.Visible = true;
                    this.L_da_ricevute_pec.Visible = true;
                    break;
            }
        }

        protected void ddl_data_ricevute_pitre_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_data_ricevute_pitre.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("Cal_Da_pitre").Visible = true;
                    this.GetCalendarControl("Cal_Da_pitre").btn_Cal.Visible = true;
                    this.GetCalendarControl("Cal_Da_pitre").btn_Cal.Enabled = true;
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Visible = true;
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Enabled = true;
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Text = string.Empty;
                    this.GetCalendarControl("Cal_A_pitre").Visible = false;
                    this.GetCalendarControl("Cal_A_pitre").btn_Cal.Visible = false;
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Visible = false;
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Text = string.Empty;
                    this.L_a_ricevute_pitre.Visible = false;
                    this.L_da_ricevute_pitre.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("Cal_Da_pitre").Visible = true;
                    this.GetCalendarControl("Cal_Da_pitre").btn_Cal.Visible = true;
                    this.GetCalendarControl("Cal_Da_pitre").btn_Cal.Enabled = true;
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Visible = true;
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Enabled = true;
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Text = string.Empty;
                    this.GetCalendarControl("Cal_A_pitre").Visible = true;
                    this.GetCalendarControl("Cal_A_pitre").btn_Cal.Visible = true;
                    this.GetCalendarControl("Cal_A_pitre").btn_Cal.Enabled = true;
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Visible = true;
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Enabled = true;
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Text = string.Empty;
                    this.L_a_ricevute_pitre.Visible = true;
                    this.L_da_ricevute_pitre.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("Cal_Da_pitre").Visible = true;
                    this.GetCalendarControl("Cal_Da_pitre").btn_Cal.Visible = true;
                    this.GetCalendarControl("Cal_Da_pitre").btn_Cal.Enabled = false;
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Visible = true;
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Enabled = false;
                    this.GetCalendarControl("Cal_A_pitre").Visible = false;
                    this.GetCalendarControl("Cal_A_pitre").btn_Cal.Visible = false;
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Visible = false;
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Text = string.Empty;
                    this.L_a_ricevute_pitre.Visible = false;
                    this.L_da_ricevute_pitre.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("Cal_Da_pitre").Visible = true;
                    this.GetCalendarControl("Cal_Da_pitre").btn_Cal.Enabled = false;
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Enabled = false;
                    this.GetCalendarControl("Cal_A_pitre").Visible = true;
                    this.GetCalendarControl("Cal_A_pitre").btn_Cal.Visible = true;
                    this.GetCalendarControl("Cal_A_pitre").btn_Cal.Enabled = false;
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Visible = true;
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Enabled = false;
                    this.L_a_ricevute_pitre.Visible = true;
                    this.L_da_ricevute_pitre.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("Cal_Da_pitre").Visible = true;
                    this.GetCalendarControl("Cal_Da_pitre").btn_Cal.Enabled = false;
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("Cal_Da_pitre").txt_Data.Enabled = false;
                    this.GetCalendarControl("Cal_A_pitre").Visible = true;
                    this.GetCalendarControl("Cal_A_pitre").btn_Cal.Visible = true;
                    this.GetCalendarControl("Cal_A_pitre").btn_Cal.Enabled = false;
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Visible = true;
                    this.GetCalendarControl("Cal_A_pitre").txt_Data.Enabled = false;
                    this.L_a_ricevute_pitre.Visible = true;
                    this.L_da_ricevute_pitre.Visible = true;
                    break;
            }
        }

        protected void ddl_numProt_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txt_fineNumProt_C.Text = "";

            if (this.ddl_numProt_C.SelectedIndex == 0)
            {
                this.txt_fineNumProt_C.Visible = false;
                this.lblDAnumprot_C.Visible = false;
                this.lblAnumprot_C.Visible = false;
            }
            else
            {
                this.txt_fineNumProt_C.Visible = true;
                this.lblDAnumprot_C.Visible = true;
                this.lblAnumprot_C.Visible = true;
            }
        }

		private void rbl_Reg_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            //if (this.rbl_Reg.SelectedItem.Value.Equals("0")) 
            //{
            //    UserManager.removeListaIdRegistri(this);
            //    for(int h=0;h<this.lb_reg.Items.Count;h++)
            //        lb_reg.Items[h].Selected = false;
            //} 
            //else if (this.rbl_Reg.SelectedItem.Value.Equals("1")) 
            //{
            //    UserManager.setListaIdRegistri(this, UserManager.getListaIdRegistriUtente(this));
            //    for(int h=0;h<this.lb_reg.Items.Count;h++)
            //        lb_reg.Items[h].Selected = true;
            //}
            if (this.rbl_Reg.SelectedItem.Value.Equals("R"))
            {
                UserManager.removeListaIdRegistri(this);
                //for (int h = 0; h < this.lb_reg_C.Items.Count; h++)
                //    lb_reg_C.Items[h].Selected = false;
                lb_reg.ClearSelection();
            }
            if (this.rbl_Reg.SelectedItem.Value.Equals("T"))
            {
                UserManager.setListaIdRegistri(this, UserManager.getListaIdRegistriUtente(this));
                for (int h = 0; h < this.lb_reg.Items.Count; h++)
                    lb_reg.Items[h].Selected = true;
            }
            if (this.rbl_Reg.SelectedItem.Value.Equals("M"))
            {
                lb_reg.ClearSelection();
                ArrayList idList = new ArrayList();
                for (int h = 0; h < this.lb_reg.Items.Count; h++)
                {
                    for (int i = 0; i < UserManager.getRuolo(this).registri.Length; i++)
                    {
                        if (UserManager.getRuolo(this).registri[i].codRegistro == lb_reg.Items[h].Text)
                        {
                            if (UserManager.getRuolo(this).registri[i] != null && !UserManager.getRuolo(this).registri[i].flag_pregresso)
                            {
                                lb_reg.Items[h].Selected = true;
                                idList.Add(lb_reg.Items[h].Value);
                                break;
                            }
                        }
                    }

                }

                string[] id = new string[idList.Count];
                for (int i = 0; i < idList.Count; i++)
                    id[i] = (string)idList[i];
                UserManager.setListaIdRegistri(this, id);
                //UserManager.setListaIdRegistri(this, UserManager.getListaIdRegistriUtente(this));
            }
		}

        private void ddl_idDocumento_C_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txt_fineIdDoc_C.Text = "";
            if (this.ddl_idDocumento_C.SelectedIndex == 0)
            {
                this.txt_fineIdDoc_C.Visible = false;
                this.lblAidDoc_C.Visible = false;
                this.lblDAidDoc_C.Visible = false;
            }
            else
            {
                this.txt_fineIdDoc_C.Visible = true;
                this.lblAidDoc_C.Visible = true;
                this.lblDAidDoc_C.Visible = true;
            }
        }

		private void lb_reg_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.rbl_Reg.ClearSelection();
			
			ArrayList idList = new ArrayList();
			for(int h=0;h<this.lb_reg.Items.Count;h++) 
			{
				if(lb_reg.Items[h].Selected)
					idList.Add(lb_reg.Items[h].Value);					
			}
			string[] id = new string[idList.Count];
			for (int i=0; i < idList.Count; i++)
				id[i] = (string)idList[i];
			UserManager.setListaIdRegistri(this, id);
		}

        private void setDescMittente(string codiceRubrica, string tipoMit)
        {
            Session.Remove("multiCorr");
            DocsPaWR.ElementoRubrica[] listaCorr = null;
            DocsPaWR.Corrispondente corr = null;
            DocsPaWR.RubricaCallType calltype = DocsPaWR.RubricaCallType.CALLTYPE_RICERCA_COMPLETAMENTO;
            bool codiceEsatto = (cb_mitt_dest_storicizzati.Checked) ? false : true;
            if (!codiceRubrica.Equals(""))
                listaCorr = UserManager.getElementiRubricaMultipli(this, codiceRubrica, calltype, codiceEsatto);
            if (tipoMit == "Mit")
            {
                if (listaCorr != null && listaCorr.Length > 0)
                {
                    if (listaCorr.Length == 1)
                    {
                        if (!string.IsNullOrEmpty(listaCorr[0].systemId))
                        {
                            //corr = UserManager.getCorrispondenteRubrica(this.Page, listaCorr[0].codice, calltype);
                            corr = UserManager.getCorrispondenteBySystemID(this.Page, listaCorr[0].systemId);
                        }
                        else
                        {
                            corr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, listaCorr[0].codice);
                        }
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "multiCorr", "ApriFinestraMultiCorrispondenti();", true);
                        Session.Add("multiCorr", listaCorr);
                        return;
                    }
                }

                if (corr != null)
                {
                    if (!cb_mitt_dest_storicizzati.Checked && !string.IsNullOrEmpty(corr.dta_fine))
                    {
                        this.txt_codMit.Text = "";
                        this.txt_descrMit.Text = "";
                    }
                    else
                    {
                        this.txt_codMit.Text = corr.codiceRubrica;
                        this.txt_descrMit.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
                        this.hd_systemIdMit_Compl.Value = corr.systemId;
                    }
                }

                else
                {
                    this.txt_codMit.Text = "";
                    this.txt_descrMit.Text = "";
                    this.hd_systemIdMit_Compl.Value = "";
                }
            }
        }

		private void txt_codMit_TextChanged(object sender, System.EventArgs e)
		{
			try 
			{
				setDescMittente(this.txt_codMit.Text, "Mit");	
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

        /// <summary>
        /// Indica se la profilazione dell'allegato è abilitata o meno
        /// </summary>
        protected bool IsEnabledProfilazioneAllegato
        {
            get
            {
                const string VIEW_STATE_KEY = "IsEnabledProfilazioneAllegato";

                bool isEnabled = false;

                if (this.ViewState[VIEW_STATE_KEY] != null)
                {
                    isEnabled = Convert.ToBoolean(this.ViewState[VIEW_STATE_KEY]);
                }
                else
                {
                    DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                    isEnabled = ws.IsEnabledProfilazioneAllegati();
                    this.ViewState.Add(VIEW_STATE_KEY, isEnabled);
                }

                return isEnabled;
            }
        }

		private bool ricercaCompletamento()
		{
			try 
			{
				//int indexfVList=0;
				//array contenitore degli array filtro di ricerca
				qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
				qV[0]=new DocsPAWA.DocsPaWR.FiltroRicerca[1];
				
				fVList=new DocsPAWA.DocsPaWR.FiltroRicerca[0];
					
//				//filtro Documenti completamento
//				if (!this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("T"))
//				{
//					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
//					fV1.argomento=DocsPaWR.FiltriDocumento;
//					fV1.valore=this.rb_archDoc_C.SelectedItem.Value;
//					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
//				}
				
				
				//commentato il 25/10/2005 perchè tale info veniva settata anche se il filtro nn era selezionato
				//nuovo filtro per prendere solo i documenti protocollati
//				#region 
//				if (this.rbl_documentiInCompletamento.SelectedIndex<0 || (this.rbl_documentiInCompletamento.SelectedIndex>=0 && !this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("P_Prot")) )
//				{
//					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
//					fV1.argomento=DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
//					fV1.valore= "0";  //corrisponde a 'false'
//					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
//				}
//				#endregion
				
				
				#region //filtri su mancanza immagine, assegnatario, fascicolazione, etc
                //if (this.rbl_documentiInCompletamento.SelectedIndex>=0)
                //{
					//fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
                    //if (this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("M_Asg"))
                    //{
                    //    fV1.argomento=DocsPaWR.FiltriDocumento.MANCANZA_ASSEGNAZIONE.ToString(); 
                    //    fV1.valore = "0";
                    //}
                    //else if (this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("M_Img"))
                    //{
                    //    fV1.argomento=DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString();
                    //    fV1.valore = "0";
                    //}
                    //else if (this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("M_Fasc"))
                    //{
                    //    fV1.argomento=DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString();
                    //    fV1.valore = "0";
                    //}
                    //else if (this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("P_Prot"))
                    //{
                    //    fV1.argomento=DocsPaWR.FiltriDocumento.DA_PROTOCOLLARE.ToString();
                    //    fV1.valore = "1"; //corrisponde a 'true'

                    //}
                   
                
                //if ((this.cbl_docInCompl.Items.FindByValue("C_Img").Selected && !this.cbl_docInCompl.Items.FindByValue("S_Img").Selected)
                //        ||
                //      (!this.cbl_docInCompl.Items.FindByValue("C_Img").Selected && this.cbl_docInCompl.Items.FindByValue("S_Img").Selected))
                //    {
                //        if (this.cbl_docInCompl.Items.FindByValue("C_Img").Selected)
                //        {
                //            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //            fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString();
                //            fV1.valore = "0";
                //            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //        }
                //        if (this.cbl_docInCompl.Items.FindByValue("S_Img").Selected)
                //        {
                //            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //            fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString();
                //            fV1.valore = "1";
                //            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //        }
                //    }

                if (!this.rbl_immagine.Items.FindByValue("Reset").Selected)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString();
                    fV1.valore = this.rbl_immagine.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                    //if ((this.cbl_docInCompl.Items.FindByValue("C_Fasc").Selected && !this.cbl_docInCompl.Items.FindByValue("S_Fasc").Selected)
                    //    ||
                    //  (!this.cbl_docInCompl.Items.FindByValue("C_Fasc").Selected && this.cbl_docInCompl.Items.FindByValue("S_Fasc").Selected))
                    //{
                    //    if (this.cbl_docInCompl.Items.FindByValue("C_Fasc").Selected)
                    //    {
                    //        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    //        fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString();
                    //        fV1.valore = "0";
                    //        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    //    }
                    //    if (this.cbl_docInCompl.Items.FindByValue("S_Fasc").Selected)
                    //    {
                    //        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    //        fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString();
                    //        fV1.valore = "1";
                    //        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    //    }
                    //}

                if (!this.rbl_fascicolazione.Items.FindByValue("Reset").Selected)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString();
                    fV1.valore = this.rbl_fascicolazione.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                //}

				#endregion

				#region filtro Archivio  (cerco tutti i doc Arrivo, Partenza, Interno, Grigio)

				fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
				fV1.argomento=DocsPaWR.FiltriDocumento.TIPO.ToString();
				fV1.valore="tipo";
				fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);

                if (this.cbl_archDoc_C.Items.FindByValue("A") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                    if (this.cbl_archDoc_C.Items.FindByValue("A").Selected)
                        fV1.valore = "true";
                    else
                        fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                if (this.cbl_archDoc_C.Items.FindByValue("P") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                    if (this.cbl_archDoc_C.Items.FindByValue("P").Selected)
                        fV1.valore = "true";
                    else
                        fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                if (this.cbl_archDoc_C.Items.FindByValue("I") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                    if (this.cbl_archDoc_C.Items.FindByValue("I").Selected)
                        fV1.valore = "true";
                    else
                        fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                if (this.cbl_archDoc_C.Items.FindByValue("G") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
                    if (this.cbl_archDoc_C.Items.FindByValue("G").Selected)
                        fV1.valore = "true";
                    else
                        fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                if (this.cbl_archDoc_C.Items.FindByValue("Pr") != null)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                    if (this.cbl_archDoc_C.Items.FindByValue("Pr").Selected)
                        fV1.valore = "true";
                    else
                        fV1.valore = "false";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                if (this.IsEnabledProfilazioneAllegato && this.cbl_archDoc_C.Items.FindByValue("ALL").Selected)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ALLEGATO.ToString();
                    //fV1.valore = this.cbl_archDoc_C.Items.FindByValue("ALL").Selected.ToString();
                    fV1.valore = this.rblFiltriAllegati.SelectedValue.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion 

				#region filtro registro
				fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
				fV1.argomento=DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                string registri = "";
                if (this.lb_reg.Items.Count > 0)
                {
                    for (int i = 0; i < this.lb_reg.Items.Count; i++)
                    {
                        if (this.lb_reg.Items[i].Selected)
                            registri += this.lb_reg.Items[i].Value + ",";

                    }
                }
                if (!registri.Equals(""))
                {
                    //Elimino la virgola alla fine della stringa se è stato selezionato
                    //almeno un registro
                    registri = registri.Substring(0, registri.Length - 1);
                    fV1.valore = registri;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {
                    Response.Write("<script>alert('Selezionare almeno un registro');top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                    return false;

                }
				#endregion 

				#region filtro oggetto
				if (this.txt_oggetto.Text!=null && !this.txt_oggetto.Text.Equals(""))
				{
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriDocumento.OGGETTO.ToString();
					fV1.valore= Utils.DO_AdattaString(this.txt_oggetto.Text);
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				}
				#endregion 

                #region filtro mitt/dest

                if (!string.IsNullOrEmpty(this.hd_systemIdMit_Compl.Value))
                {
                    if (!this.txt_descrMit.Text.Equals(""))
                    {
                        if (!string.IsNullOrEmpty(this.txt_codMit.Text))
                        {
                            if (this.cb_mitt_dest_storicizzati.Checked)
                            {
                                // Ricerca i documenti per i mittenti / destinatari storicizzati
                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                                fV1.valore = this.txt_codMit.Text;
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
                                fV1.valore = this.cb_mitt_dest_storicizzati.Checked.ToString();
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                            else
                             {
                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                                fV1.valore = this.hd_systemIdMit_Compl.Value;
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                        
                        }
                    }
                }              
                else
                {
                    if (!this.txt_descrMit.Text.Equals(""))
                    {
                        if (!string.IsNullOrEmpty(this.txt_codMit.Text))
                        {
                            if (this.cb_mitt_dest_storicizzati.Checked)
                            {
                                // Ricerca i documenti per i mittenti / destinatari storicizzati
                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString();
                                fV1.valore = this.txt_codMit.Text;
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString();
                                fV1.valore = this.cb_mitt_dest_storicizzati.Checked.ToString();
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                            else
                            {
                                // Ricerca dell'id del corrispondente a partire dal codice
                                DocsPaWR.Corrispondente corrByCode = UserManager.getCorrispondenteByCodRubrica(this, this.txt_codMit.Text);
                                if (corrByCode != null)
                                {
                                    this.hd_systemIdMit_Compl.Value = corrByCode.systemId;

                                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString();
                                    fV1.valore = this.hd_systemIdMit_Compl.Value;
                                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                                }
                                else
                                {
                                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                                    fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                                    fV1.valore = this.txt_descrMit.Text;
                                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                                }
                            }
                        }
                        else
                        {
                            fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                            fV1.argomento = DocsPaWR.FiltriDocumento.MITT_DEST.ToString();
                            fV1.valore = this.txt_descrMit.Text;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                }

                #endregion

                #region filtro numero protocollo
                if (this.ddl_numProt_C.SelectedIndex == 0)
                {//valore singolo carico NUM_PROTOCOLLO

                    if (this.txt_initNumProt_C.Text != null && !this.txt_initNumProt_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                        fV1.valore = this.txt_initNumProt_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                    if (!this.txt_initNumProt_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                        fV1.valore = this.txt_initNumProt_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txt_fineNumProt_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                        fV1.valore = this.txt_fineNumProt_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region Anno Protocollo
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                fV1.valore = this.tbAnnoProtocollo.Text;
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

				#region filtro data protocollo 
                if (this.ddl_dataProt.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProt.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProt.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProt.SelectedIndex == 0)
				{//valore singolo carico DATA_PROTOCOLLO
					if (this.GetCalendarControl("txt_initDataProt").txt_Data.Text!=null && !this.GetCalendarControl("txt_initDataProt").txt_Data.Text.Equals(""))
					{
						if(!Utils.isDate(this.GetCalendarControl("txt_initDataProt").txt_Data.Text))
						{
							Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt").txt_Data.ID + "').focus();</SCRIPT>";
                            //RegisterStartupScript("focus", s);
							Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");						
							return false;
						}
						
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
						fV1.valore=this.GetCalendarControl("txt_initDataProt").txt_Data.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
				}
                if (this.ddl_dataProt.SelectedIndex == 1)
                {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
					if (!this.GetCalendarControl("txt_initDataProt").txt_Data.Text.Equals(""))
					{
						if(!Utils.isDate(this.GetCalendarControl("txt_initDataProt").txt_Data.Text))
						{
							Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt").txt_Data.ID + "').focus();</SCRIPT>";
                            //RegisterStartupScript("focus", s);
							Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");							
							return false;
						}
						
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
						fV1.valore=this.GetCalendarControl("txt_initDataProt").txt_Data.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
					if (!this.GetCalendarControl("txt_fineDataProt").txt_Data.Text.Equals(""))
					{
						if(!Utils.isDate(this.GetCalendarControl("txt_fineDataProt").txt_Data.Text))
						{
							Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataProt").txt_Data.ID + "').focus();</SCRIPT>";
                            //RegisterStartupScript("focus", s);
							Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");							
							return false;
						}
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
						fV1.valore=this.GetCalendarControl("txt_fineDataProt").txt_Data.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
				}
				#endregion

                //#region filtro Creatore (User Control)
                //fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                //fV1 = this.Creatore.GetFilter();
                //if (fV1 != null)
                //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                //#endregion
                #region Filtri Creatore e Proprietario

                foreach (var ownerFilter in this.aofOwner.GetFiltersList())
                    fVList = Utils.addToArrayFiltroRicerca(fVList, ownerFilter);


                foreach (var authorFilter in this.aofAuthor.GetFiltersList())
                    fVList = Utils.addToArrayFiltroRicerca(fVList, authorFilter);

                #endregion

                #region filtro data creazione
                if (this.ddl_dataCreaz.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreaz.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreaz.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreaz.SelectedIndex == 0)
				{//valore singolo carico DATA_PROTOCOLLO
					if (this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text!=null && !this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text.Equals(""))
					{
						if(!Utils.isDate(this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text))
						{
							Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt").txt_Data.ID + "').focus();</SCRIPT>";
                            //RegisterStartupScript("focus", s);
							Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");						
							return false;
						}
						
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
						fV1.valore=this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
				}
                if (this.ddl_dataCreaz.SelectedIndex == 1)
                {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
					if (!this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text.Equals(""))
					{
						if(!Utils.isDate(this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text))
						{
							Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreaz").txt_Data.ID + "').focus();</SCRIPT>";
                            //RegisterStartupScript("focus", s);
							Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");							
							return false;
						}
						
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
						fV1.valore=this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
					if (!this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text.Equals(""))
					{
						if(!Utils.isDate(this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text))
						{
							Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataCreaz").txt_Data.ID + "').focus();</SCRIPT>";
                            //RegisterStartupScript("focus", s);
							Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");							
							return false;
						}
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
						fV1.valore=this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
				}
				#endregion

				#region filtro data protocollo mittente
                if (this.ddl_dataProtMitt_C.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProtMitt_C.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProtMitt_C.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataProtMitt_C.SelectedIndex == 0)
				{//valore singolo carico DATA_PROTOCOLLO_MIT
					if (this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text!=null && !this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text.Equals(""))
					{
						if(!Utils.isDate(this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text))
						{
							Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.ID + "').focus();</SCRIPT>";
                            //RegisterStartupScript("focus", s);
							Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
							return false;
						}
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_IL.ToString();
						fV1.valore=this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
				}
                if (this.ddl_dataProtMitt_C.SelectedIndex == 1)
                {//valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
					if (!this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text.Equals(""))
					{
						if(!Utils.isDate(this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text))
						{
							Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.ID + "').focus();</SCRIPT>";
                            //RegisterStartupScript("focus", s);
							Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
							return false;
						}
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SUCCESSIVA_AL.ToString();
						fV1.valore=this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
					if (!this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Equals(""))
					{
						if(!Utils.isDate(this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text))
						{
							Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.ID + "').focus();</SCRIPT>";
                            //RegisterStartupScript("focus", s);
							Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
							return false;
						}
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_PRECEDENTE_IL.ToString();
						fV1.valore=this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
				}
				#endregion 

				#region filtro numero protocollo mittente
				if (this.txt_numProtMitt_C.Text!=null && !this.txt_numProtMitt_C.Text.Equals(""))
				{
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriDocumento.PROTOCOLLO_MITTENTE.ToString();
					fV1.valore = DocsPaUtils.Functions.Functions.ReplaceApexes(this.txt_numProtMitt_C.Text);
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				}
				#endregion 

                #region filtro RICERCA IN AREA LAVORO
                if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_IN_ADL.ToString();
                    fV1.valore = UserManager.getInfoUtente(this).idPeople.ToString() + "@" + UserManager.getRuolo(this).systemId.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro TRASMESSI_CON
                if (this.cbx_Trasm.Checked)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TRASMESSI_CON.ToString();
                    if (this.ddl_ragioneTrasm.SelectedIndex == 0)
                        fV1.valore = "Tutte";
                    else
                        fV1.valore = this.ddl_ragioneTrasm.SelectedItem.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro TRASMESSI_SENZA
                if (this.cbx_TrasmSenza.Checked)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TRASMESSI_SENZA.ToString();
                    if (this.ddl_ragioneTrasm.SelectedIndex == 0)
                        fV1.valore = "Tutte";
                    else
                        fV1.valore = this.ddl_ragioneTrasm.SelectedItem.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro docNumber
                if (this.ddl_idDocumento_C.SelectedIndex == 0)
                {
                    if (this.txt_initIdDoc_C.Text != null && !this.txt_initIdDoc_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                        fV1.valore = this.txt_initIdDoc_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {
                    if (this.txt_initIdDoc_C.Text != null && !this.txt_initIdDoc_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                        fV1.valore = this.txt_initIdDoc_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (this.txt_fineIdDoc_C.Text != null && !this.txt_fineIdDoc_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                        fV1.valore = this.txt_fineIdDoc_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion 

                #region filtro numero protocollo
                if (this.ddl_numProt_C.SelectedIndex == 0)
                {//valore singolo carico NUM_PROTOCOLLO

                    if (this.txt_initNumProt_C.Text != null && !this.txt_initNumProt_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                        fV1.valore = this.txt_initNumProt_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                    if (!this.txt_initNumProt_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                        fV1.valore = this.txt_initNumProt_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txt_fineNumProt_C.Text.Equals(""))
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                        fV1.valore = this.txt_fineNumProt_C.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro file firmati

                if (this.chkFirmato.Checked)
                {
                    //cerco documenti firmati
                    if (!chkNonFirmato.Checked)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                        fV1.valore = "1";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else //se sono entrambi selezionati cerco i documenti che abbiano un file acquisito, siano essi firmati o meno.
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                        fV1.valore = "2";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {
                    //cerco i documenti non firmati
                    if (chkNonFirmato.Checked)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.FIRMATO.ToString();
                        fV1.valore = "0";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                }

                #endregion

                #region filtro tipo file acquisito

                if (this.ddl_tipoFileAcquisiti.SelectedIndex > 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO_FILE_ACQUISITO.ToString();
                    fV1.valore = this.ddl_tipoFileAcquisiti.SelectedItem.Value;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #endregion

                #region filtro documenti spediti
                int appo = 0;
                if (cbx_pec.Visible && cbx_pec.Checked)
                    appo = 1;
                if (cbx_pitre.Visible && cbx_pitre.Checked)
                    if (appo == 0)
                        appo = 2;
                    else
                        appo = 3;

                switch (appo)
                {
                    case 0:
                        break;

                    case 1:
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOC_SPEDITI_TIPO.ToString();
                        fV1.valore = "PEC";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        break;

                    case 2:
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOC_SPEDITI_TIPO.ToString();
                        fV1.valore = "PITRE";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        break;

                    case 3:
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOC_SPEDITI_TIPO.ToString();
                        fV1.valore = "ALL";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        break;
                }

                if (!string.IsNullOrEmpty(this.txt_dataSpedDa.Text))
                {                  
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SPEDIZIONE_DA.ToString();
                    fV1.valore = this.txt_dataSpedDa.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    if (rb_docSpediti.SelectedIndex == -1 || rb_docSpediti.SelectedIndex==3)                    
                        rb_docSpediti.SelectedIndex = 2;
                }
                if (!string.IsNullOrEmpty(this.txt_dataSpedA.Text))
                {    
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SPEDIZIONE_A.ToString();
                    fV1.valore = this.txt_dataSpedA.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    if (rb_docSpediti.SelectedIndex == -1 || rb_docSpediti.SelectedIndex == 3)
                        rb_docSpediti.SelectedIndex = 2;
                }

                if (rb_docSpediti.SelectedIndex != -1 && rb_docSpediti.SelectedIndex!=3)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_SPEDITI.ToString();
                    fV1.valore = this.rb_docSpediti.SelectedValue;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region codice nome amministrazione
                if (!this.txt_codDesc.Text.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.CODICE_DESCRIZIONE_AMMINISTRAZIONE.ToString();
                    fV1.valore = this.txt_codDesc.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region DATA_TIPO_NOTIFICA_DA
                if (!string.IsNullOrEmpty(this.Cal_Da_pec.txt_Data.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_DA.ToString();
                    fV1.valore = this.Cal_Da_pec.txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region DATA_TIPO_NOTIFICA_DA_PITRE
                if (!string.IsNullOrEmpty(this.Cal_Da_pitre.txt_Data.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_DA_PITRE.ToString();
                    fV1.valore = this.Cal_Da_pitre.txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region DATA_TIPO_NOTIFICA_A
                if (!string.IsNullOrEmpty(this.Cal_A_pec.txt_Data.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_A.ToString();
                    fV1.valore = this.Cal_A_pec.txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region DATA_TIPO_NOTIFICA_A_PITRE
                if (!string.IsNullOrEmpty(this.Cal_A_pitre.txt_Data.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_A_PITRE.ToString();
                    fV1.valore = this.Cal_A_pitre.txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtri timestamp
                //Senza timestamp
                if (rbl_timestamp.SelectedValue == "1")
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.SENZA_TIMESTAMP.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                //Con timestamp
                if (rbl_timestamp.SelectedValue == "0" && ddl_timestamp.SelectedValue == "0" && string.IsNullOrEmpty(date_timestamp.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.CON_TIMESTAMP.ToString();
                    fV1.valore = "0";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                //Timestamp scaduto
                if(rbl_timestamp.SelectedValue == "0" && ddl_timestamp.SelectedValue == "1" && string.IsNullOrEmpty(date_timestamp.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIMESTAMP_SCADUTO.ToString();
                    fV1.valore = "0";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                //Timestamp scade prima di
                if(rbl_timestamp.SelectedValue == "0" && ddl_timestamp.SelectedValue == "2" && !string.IsNullOrEmpty(date_timestamp.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIMESTAMP_SCADE_PRIMA_DI.ToString();
                    fV1.valore = date_timestamp.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion filtri timestamp

                #region DATA_TIPO_NOTIFICA_TODAY
                if (!string.IsNullOrEmpty(this.Cal_Da_pec.txt_Data.Text) &&
                    string.IsNullOrEmpty(this.Cal_A_pec.txt_Data.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_TODAY.ToString();
                    fV1.valore = this.Cal_Da_pec.txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region DATA_TIPO_NOTIFICA_TODAY_PITRE
                if (!string.IsNullOrEmpty(this.Cal_Da_pitre.txt_Data.Text) &&
                    string.IsNullOrEmpty(this.Cal_A_pitre.txt_Data.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_TODAY_PITRE.ToString();
                    fV1.valore = this.Cal_Da_pitre.txt_Data.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region DATA_TIPO_NOTIFICA_NESSUNA
                if (p_ricevute_pec.Visible)
                {
                    if (this.ddl_ricevute_pec.SelectedIndex != 0)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_NESSUNA.ToString();
                        fV1.valore = ")";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region DATA_TIPO_NOTIFICA_NESSUNA_PITRE
                if (p_ricevute_pitre.Visible)
                {
                    if (this.ddl_ricevute_pitre.SelectedIndex != 0)
                    {
                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_NESSUNA_PITRE.ToString();
                        fV1.valore = ")";
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region CODICE_TIPO_NOTIFICA
                if (this.ddl_ricevute_pec.SelectedIndex != 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.CODICE_TIPO_NOTIFICA.ToString();
                    fV1.valore = ddl_ricevute_pec.SelectedValue;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region CODICE_TIPO_NOTIFICA_PITRE
                if (p_ricevute_pitre.Visible && this.ddl_ricevute_pitre.SelectedIndex != 0)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.CODICE_TIPO_NOTIFICA_PITRE.ToString();
                    fV1.valore = ddl_ricevute_pitre.SelectedValue;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region Filtro documenti consolidati

                foreach (DocsPaWR.FiltroRicerca filtroConsolidamento in documentConsolidationSearch.GetFilters())
                    fVList = Utils.addToArrayFiltroRicerca(fVList, filtroConsolidamento);
                
                //Filtro per ricordarsi il tipo di data selezionata per il consolidamento
                fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                fV1.argomento = fV1.argomento = DocsPaWR.FiltriDocumento.DATA_TIPO_CONSOLIDAMENTO.ToString();
                fV1.valore = documentConsolidationSearch.getValueDdlIntervallo();
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    

                #endregion

                #region filtro riferimento
                DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                if (wws.isEnableRiferimentiMittente() && !string.IsNullOrEmpty(txt_rif_mittente.Text))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.RIFERIMENTO_MITTENTE.ToString();
                    fV1.valore = txt_rif_mittente.Text;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                }
                #endregion

                #region Ordinamento

                // Reperimento del filtro da utilizzare per la griglia
                List<FiltroRicerca> filterList = GridManager.GetOrderFilterForDocument(this.ddlOrder.SelectedValue, this.ddlOrderDirection.SelectedValue);

                // Se la lista è valorizzata vengono aggiunti i filtri
                if (filterList != null)
                    foreach (FiltroRicerca filter in filterList)
                        fVList = Utils.addToArrayFiltroRicerca(fVList, filter);

                #endregion

                qV[0]=fVList;
				return true;
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
				return false;
			}
		}

       //protected void ddl_numProt_C_SelectedIndexChanged(object sender, System.EventArgs e)
       // {
       //     txt_fineNumProt_C.Text = "";
       //     if (this.ddl_numProt_C.SelectedIndex == 0)
       //     {
       //         this.txt_fineNumProt_C.Visible = false;
       //         this.lblDAnumprot_C.Visible = false;
       //         this.lblAnumprot_C.Visible = false;
       //     }
       //     else
       //     {
       //         this.txt_fineNumProt_C.Visible = true;
       //         this.lblDAnumprot_C.Visible = true;
       //         this.lblAnumprot_C.Visible = true;
       //     }
       // }

        public bool IsNumber(string strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                !objTwoDotPattern.IsMatch(strNumber) &&
                !objTwoMinusPattern.IsMatch(strNumber) &&
                objNumberPattern.IsMatch(strNumber);
        }

		private void btn_ricerca_Click(object sender, EventArgs e)
		{
			try
			{
				this.SetPageOnCurrentContext();

                // Salvataggio dei filtri per la ricerca proprietario e creatore
                this.aofOwner.SaveFilters();
                this.aofAuthor.SaveFilters();
                
                if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null && !Page.IsPostBack)
                {
                    GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                }

				//ddl_Ric_Salvate.SelectedIndex = 0;
                if (txt_initNumProt_C.Text != "")
                {
                    if (IsNumber(txt_initNumProt_C.Text) == false)
                    {
                        Response.Write("<script>alert('Il numero di protocollo deve essere numerico!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                if (txt_fineNumProt_C.Text != "")
                {
                    if (IsNumber(txt_fineNumProt_C.Text) == false)
                    {
                        Response.Write("<script>alert('Il numero di protocollo deve essere numerico!');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_fineNumProt_C.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                // fine controllo
                //controllo validità anno inserito
                if (tbAnnoProtocollo.Text != "")
                {
                    if (IsValidYear(tbAnnoProtocollo.Text) == false)
                    {
                        Response.Write("<script>alert('Formato anno non corretto !');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + tbAnnoProtocollo.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }

				//Controllo intervallo date
                if (this.ddl_dataProt.SelectedIndex != 0)
                {
                    if (Utils.isDate(this.GetCalendarControl("txt_initDataProt").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_fineDataProt").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataProt").txt_Data.Text, this.GetCalendarControl("txt_fineDataProt").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date Data Protocollo!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProt").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                if (this.ddl_dataCreaz.SelectedIndex != 0)
                {
                    if (Utils.isDate(this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text, this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date Data Creazione!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreaz").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                if (this.ddl_dataProtMitt_C.SelectedIndex != 0)
                {
                    if (Utils.isDate(this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text, this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date Data Protocollo Mittente!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                //Fine controllo intervallo date
                // Controllo sul tipo di ricerca richiesto (ex Tipo Protocollo)
                bool controllo = false;
                for (int i = 0; i < this.cbl_archDoc_C.Items.Count; i++)
                {
                    if (this.cbl_archDoc_C.Items[i].Selected)
                        controllo = true;
                }
                if (!controllo)
                {
                    Response.Write("<script>alert('Inserire almeno un tipo di ricerca');</script>");
                    //string s = "<SCRIPT language='javascript'>document.getElementById('" + cbl_archDoc_C.ID + "').focus();</SCRIPT>";
                    //RegisterStartupScript("focus", s);
                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                    return;
                }

                // Controllo lunghezza oggetto inserito
                if (this.txt_oggetto.Text.Trim() != string.Empty && !FullTextSearch.Configurations.CheckTextMinLenght(this.txt_oggetto.Text))
                {
                    string message = string.Format("<script>alert('Per ricercare un oggetto è necessario immettere almeno {0} caratteri');</script>", FullTextSearch.Configurations.FullTextMinTextLenght.ToString());
                    Response.Write(message);
                    this.txt_oggetto.Focus();
                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                    return;
                }

				if (ricercaCompletamento())
				{
                    int numCriteri = 0;
                    if (this.cbl_archDoc_C.Items[0].Selected && this.cbl_archDoc_C.Items[1].Selected && this.cbl_archDoc_C.Items[2].Selected && this.cbl_archDoc_C.Items[3].Selected)
                        numCriteri = 1;
                    if (qV[0] == null || qV[0].Length <= numCriteri)
					{
						Response.Write("<script>alert('Inserire almeno un criterio di ricerca');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_oggetto.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
						Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");							
						return;
					}
//					else
//					{
//						//if (!Page.IsStartupScriptRegistered("wait"))
//						//{
//						//	Page.RegisterStartupScript("wait","<script>DocsPa_FuncJS_WaitWindows();</script>");
//						//}
//					}

                    // inserisco un controllo sulle stopword nel caso in cui sia abilitata la chiave use_text_index
                    string valoreChiave = utils.InitConfigurationKeys.GetValue("0", "USE_TEXT_INDEX");
                    if (!string.IsNullOrEmpty(valoreChiave) && (valoreChiave.Equals("1") || (valoreChiave.Equals("2"))))
                    {
                        for (int i = 0; i < qV[0].Length; i++)
                        {
                            if (qV[0][i].argomento.Equals(DocsPaWR.FiltriDocumento.OGGETTO.ToString()) && !string.IsNullOrEmpty(qV[0][i].valore))
                            {
                                //string stopWord = DocumentManager.verificaStopWord(this, qV[0][i].valore);
                                //if (!string.IsNullOrEmpty(stopWord))
                                //{
                                //    string messaggio = InitMessageXml.getInstance().getMessage("STOP_WORD");
                                //    Response.Write("<script>alert('" + String.Format(messaggio, stopWord) + "');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C + "').focus();</SCRIPT>";
                                //    RegisterStartupScript("focus", s);
                                //    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                //    return;
                                //}
                                if (qV[0][i].valore.StartsWith("%"))
                                {
                                    Response.Write("<script>alert('Il parametro di ricerca non può iniziare con il carattere %');</script>");
                                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                                    RegisterStartupScript("focus", s);
                                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                    return;
                                }
                                //if (qV[0][i].valore.Contains("%&&") || qV[0][i].valore.Contains("&&%"))
                                //{
                                //    Response.Write("<script>alert('La combinazione degli operatori && e % non è supportata');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                                //    RegisterStartupScript("focus", s);
                                //    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                //    return;
                                //}
                            }
                            if (qV[0][i].argomento.Equals(DocsPaWR.FiltriDocumento.MITT_DEST.ToString()) && !string.IsNullOrEmpty(qV[0][i].valore))
                            {
                                //string stopWord = DocumentManager.verificaStopWord(this, qV[0][i].valore);
                                //if (!string.IsNullOrEmpty(stopWord))
                                //{
                                //    string messaggio = InitMessageXml.getInstance().getMessage("STOP_WORD");
                                //    Response.Write("<script>alert('" + String.Format(messaggio, stopWord) + "');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                                //    RegisterStartupScript("focus", s);
                                //    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                //    return;
                                //}
                                if (qV[0][i].valore.StartsWith("%"))
                                {
                                    Response.Write("<script>alert('Il parametro di ricerca non può iniziare con il carattere %');</script>");
                                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                                    RegisterStartupScript("focus", s);
                                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                    return;
                                }
                                //if (qV[0][i].valore.Contains("%&&") || qV[0][i].valore.Contains("&&%"))
                                //{
                                //    Response.Write("<script>alert('La combinazione degli operatori && e % non è supportata');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initNumProt_C.ID + "').focus();</SCRIPT>";
                                //    RegisterStartupScript("focus", s);
                                //    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                //    return;
                                //}
                            }
                        }
                    }
                    
                    schedaRicerca.FiltriRicerca = qV;
					DocumentManager.setFiltroRicDoc(this,qV);
					DocumentManager.removeDatagridDocumento(this);
					DocumentManager.removeListaDocProt(this);
                    Session.Remove("listInArea");
                    Session.Remove("dictionaryCorrispondente");

					//Reload del frame centrale 
					//				Response.Write("<script>parent.parent.iFrame_dx.document.location.reload();</script>");	
					//Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDoc.aspx';</script>");	
                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=completamento&tabRes=completamento';</script>");
                    else
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=completamento';</script>");

                    ViewState["new_search"] = "true";
				}
			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}

		protected void ddl_dataProt_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            switch (this.ddl_dataProt.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataProt").Visible = true;
                    this.GetCalendarControl("txt_initDataProt").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProt").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataProt").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProt").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProt").Visible = false;
                    this.GetCalendarControl("txt_fineDataProt").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProt").txt_Data.Visible = false;
                    this.lbl_finedataProt.Visible = false;
                    this.lbl_initdataProt.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataProt").Visible = true;
                    this.GetCalendarControl("txt_initDataProt").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProt").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataProt").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProt").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProt").Visible = true;
                    this.GetCalendarControl("txt_fineDataProt").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProt").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt").txt_Data.Enabled = true;
                    this.lbl_finedataProt.Visible = true;
                    this.lbl_initdataProt.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataProt").Visible = true;
                    this.GetCalendarControl("txt_initDataProt").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProt").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProt").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProt").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataProt").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt").Visible = false;
                    this.GetCalendarControl("txt_fineDataProt").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProt").txt_Data.Visible = false;
                    this.lbl_finedataProt.Visible = false;
                    this.lbl_initdataProt.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataProt").Visible = true;
                    this.GetCalendarControl("txt_initDataProt").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProt").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataProt").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt").Visible = true;
                    this.GetCalendarControl("txt_fineDataProt").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataProt").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt").txt_Data.Enabled = false;
                    this.lbl_finedataProt.Visible = true;
                    this.lbl_initdataProt.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataProt").Visible = true;
                    this.GetCalendarControl("txt_initDataProt").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProt").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataProt").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt").Visible = true;
                    this.GetCalendarControl("txt_fineDataProt").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProt").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataProt").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProt").txt_Data.Enabled = false;
                    this.lbl_finedataProt.Visible = true;
                    this.lbl_initdataProt.Visible = true;
                    break;
            }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void enterKeySimulator_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			this.btn_ricerca_Click(null, null);
		}


		private void f_Ricerca_Compl_PreRender(object sender, EventArgs e)
		{
            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
            {
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);
            }


            if (!IsPostBack)
            {
                // Caricamento delle combo con le informazioni sull'ordinamento
                GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                //visibilità filtro Allegati --> sistemi esterni
                string filterExternalSystem = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FILTRO_ALLEGATI_ESTERNI");
                if (string.IsNullOrEmpty(filterExternalSystem) || (!filterExternalSystem.Equals("1")))
                {
                    foreach (ListItem f in rblFiltriAllegati.Items)
                    {
                        if (f.Value.Equals("esterni"))
                        {
                            rblFiltriAllegati.Items.Remove(f);
                            break;
                        }
                    }
                }
            }
            DocsPAWA.DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteSelezionato(this);
            if (corr != null)
            {
                this.txt_codMit.Text = corr.codiceRubrica;
                this.txt_descrMit.Text = corr.descrizione;
            }

            if (Session["dictionaryCorrispondente"] != null)
            {
                dic_Corr = (Dictionary<string, Corrispondente>)Session["dictionaryCorrispondente"];

                if (dic_Corr != null && dic_Corr.ContainsKey("ricCompletamento") && dic_Corr["ricCompletamento"] != null)
                {
                    txt_codMit.Text = dic_Corr["ricCompletamento"].codiceRubrica;
                    this.txt_descrMit.Text = dic_Corr["ricCompletamento"].descrizione;
                    this.hd_systemIdMit_Compl.Value = dic_Corr["ricCompletamento"].systemId;
                }
                dic_Corr.Remove("ricCompletamento");
                Session.Add("dictionaryCorrispondente", dic_Corr);
            }
            
            HtmlImage btn_Rubrica = (HtmlImage)FindControl("btn_Rubrica");

			string use_new_rubrica = DocsPAWA.ConfigSettings.getKey (ConfigSettings.KeysENUM.RUBRICA_V2);
            if (!this.cb_mitt_dest_storicizzati.Checked)
                btn_Rubrica.Attributes["onClick"] = "_ApriRubrica('ric_stor');";
            else
            {
                if (use_new_rubrica != "1")
                    btn_Rubrica.Attributes["onClick"] = "ApriRubrica('ric_C','');";
                else
                    btn_Rubrica.Attributes["onClick"] = "_ApriRubrica('ric_completamento');";
            }
            string new_search = string.Empty;
            if (ViewState["new_search"] != null)
            {
                new_search = ViewState["new_search"] as string;
                ViewState["new_search"] = null;
            }

            if (change_from_grid && string.IsNullOrEmpty(new_search))
            {
              //  GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                if (ricercaCompletamento())
                {
                    schedaRicerca.FiltriRicerca = qV;
                    DocumentManager.setFiltroRicDoc(this, qV);
                    DocumentManager.removeDatagridDocumento(this);
                    DocumentManager.removeListaDocProt(this);
                    change_from_grid = false;

                    string altro = string.Empty;

                    if (!string.IsNullOrEmpty(this.numResult) && this.numResult.Equals("0"))
                    {
                        altro = "&noRic=1";
                    }


                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                    {
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=completamento&tabRes=completamento" + altro + "';</script>");
                        //       ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=estesa&tabRes=estesa" + altro + "';", true);
                    }
                    else
                    {
                        //     ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?from=estesa&tabRes=estesa" + altro + "';", true);
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?from=completamento&tabRes=completamento" + altro + "';</script>");
                    }
                }
            }
		}

		private void tastoInvio()
		{
            Utils.DefaultButton(this, ref txt_initNumProt_C, ref btn_ricerca);
            Utils.DefaultButton(this, ref txt_fineNumProt_C, ref btn_ricerca);
            Utils.DefaultButton(this, ref tbAnnoProtocollo, ref btn_ricerca);
			Utils.DefaultButton(this,ref txt_oggetto, ref btn_ricerca);
			Utils.DefaultButton(this,ref this.GetCalendarControl("txt_initDataProt").txt_Data, ref btn_ricerca);
			Utils.DefaultButton(this,ref this.GetCalendarControl("txt_fineDataProt").txt_Data, ref btn_ricerca);
		}

		protected void ddl_dataProtMitt_C_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            switch (this.ddl_dataProtMitt_C.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = false;
                    this.lbl_finedataProtMitt_C.Visible = false;
                    this.lbl_initdataProtMitt_C.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Enabled = true;
                    this.lbl_finedataProtMitt_C.Visible = true;
                    this.lbl_initdataProtMitt_C.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = false;
                    this.lbl_finedataProtMitt_C.Visible = false;
                    this.lbl_initdataProtMitt_C.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Enabled = false;
                    this.lbl_finedataProtMitt_C.Visible = true;
                    this.lbl_initdataProtMitt_C.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Enabled = false;
                    this.lbl_finedataProtMitt_C.Visible = true;
                    this.lbl_initdataProtMitt_C.Visible = true;
                    break;
            }
        }

		private void ddl_dataCreaz_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            switch (this.ddl_dataCreaz.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataCreaz").Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataCreaz").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataCreaz").Visible = false;
                    this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Visible = false;
                    this.lbl_finedataCreaz.Visible = false;
                    this.lbl_initdataCreaz.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataCreaz").Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataCreaz").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataCreaz").Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Enabled = true;
                    this.lbl_finedataCreaz.Visible = true;
                    this.lbl_initdataCreaz.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataCreaz").Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCreaz").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataCreaz").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreaz").Visible = false;
                    this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Visible = false;
                    this.lbl_finedataCreaz.Visible = false;
                    this.lbl_initdataCreaz.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataCreaz").Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataCreaz").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreaz").Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Enabled = false;
                    this.lbl_finedataCreaz.Visible = true;
                    this.lbl_initdataCreaz.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataCreaz").Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataCreaz").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreaz").Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Enabled = false;
                    this.lbl_finedataCreaz.Visible = true;
                    this.lbl_initdataCreaz.Visible = true;
                    break;
            }
		}

		private void ddl_Ric_Salvate_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            if (ddl_Ric_Salvate.SelectedIndex == 0)
            {
                if (GridManager.IsRoleEnabledToUseGrids())
                {
                    GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);
                    GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                }
                return;
            }
			try
			{
                string gridTempId = string.Empty;

                schedaRicerca.Seleziona(Int32.Parse(ddl_Ric_Salvate.SelectedValue), out gridTempId);

                if (!string.IsNullOrEmpty(gridTempId) && GridManager.IsRoleEnabledToUseGrids())
                {
                    schedaRicerca.gridId = gridTempId;
                    Grid tempGrid = GridManager.GetGridFromSearchId(schedaRicerca.gridId, GridTypeEnumeration.Document);
                    if (tempGrid != null)
                    {
                        GridManager.SelectedGrid = tempGrid;
                        GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                        // Reperimento del filtro da utilizzare per la griglia
                        List<FiltroRicerca> filterList = GridManager.GetOrderFilterForDocument(this.ddlOrder.SelectedValue, this.ddlOrderDirection.SelectedValue);

                        // Se la lista è valorizzata vengono aggiunti i filtri
                        if (filterList != null)
                        {
                            foreach (FiltroRicerca filter in filterList)
                            {
                                schedaRicerca.FiltriRicerca[0] = Utils.addToArrayFiltroRicerca(schedaRicerca.FiltriRicerca[0], filter);
                            }
                        }

                    }
                }

				qV = schedaRicerca.FiltriRicerca;
				try
				{
					if (ddl_Ric_Salvate.SelectedIndex > 0)
					{
						Session.Add("itemUsedSearch",ddl_Ric_Salvate.SelectedIndex.ToString());
					}

                    if (PopulateField(qV, true))
					{
						DocumentManager.setFiltroRicDoc(this,qV);
						DocumentManager.removeDatagridDocumento(this);
						DocumentManager.removeListaDocProt(this);

						//Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDoc.aspx';</script>");	
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?ricADL=1&from=completamento&tabRes=completamento';</script>");
                        else
                            Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=completamento';</script>");
					}
				}
				catch (Exception ex_)
				{
					string msg = ex_.Message;
					msg = msg + " Rimuovere i criteri di ricerca selezionati.";
					msg = msg.Replace("\"","\\\"");
					Response.Write("<script>alert(\""+msg+"\");window.location.href = window.location.href;</script>");
				}

                this.btn_modifica.Enabled = true;
			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				msg = msg.Replace("\"","\\\"");
				Response.Write("<script>alert(\""+msg+"\");window.location.href = window.location.href;</script>");
			}
		}

		private void btn_Canc_Ric_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (ddl_Ric_Salvate.SelectedIndex>0)
			{
				//Chiedi conferma su popup
				string id = ddl_Ric_Salvate.SelectedValue;
				DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
				DocsPaWR.SearchItem item = docspaws.RecuperaRicerca(Int32.Parse(id));
				DocsPaWR.Ruolo ruolo = null;
				if (item.owner_idGruppo!=0)
					ruolo = (DocsPAWA.DocsPaWR.Ruolo) Session["userRuolo"];
				string msg = "Il criterio di ricerca con nome '"+ddl_Ric_Salvate.SelectedItem.ToString()+"' verrà rimosso.\\n";
				msg += (ruolo!=null) ? "Attenzione! Il criterio di ricerca è condiviso con il ruolo '"+ruolo.descrizione+"'.\\n" : "";
				msg += "Confermi l'operazione?";
				msg = msg.Replace("\"","\\\"");
				if (this.Session["itemUsedSearch"]!=null)
				{
					Session.Remove("itemUsedSearch");
				}				
				mb_ConfirmDelete.Confirm(msg);
			}		
		}

		private void btn_salva_Click(object sender, EventArgs e)
		{
			if (ricercaCompletamento())
			{
                // Impostazione del filtro utilizzato
                GridManager.SetSearchFilter(this.ddlOrder.SelectedItem.Text, this.ddlOrderDirection.SelectedValue);

				schedaRicerca.FiltriRicerca = qV;
				schedaRicerca.ProprietaNuovaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca.NuovaRicerca();
                if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                    RegisterStartupScript("SalvaRicerca", "<script>apriSalvaRicercaADL();</script>");
                else
                    RegisterStartupScript("SalvaRicerca", "<script>apriSalvaRicerca();</script>");
			}
		}

		private void mb_ConfirmDelete_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
		{
			if( e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok )
			{
				try
				{
					schedaRicerca.Cancella(Int32.Parse(ddl_Ric_Salvate.SelectedValue));
					Response.Write("<script>alert(\"I criteri di ricerca sono stati rimossi\");window.location.href = window.location.href;</script>");
				}
				catch (Exception ex)
				{
					string msg = "Impossibile rimuovere i criteri di ricerca. Errore: "+ex.Message;
					msg = msg.Replace("\"","\\\"");
					Response.Write("<script>alert(\""+msg+"\");window.location.href = window.location.href;</script>");
				}
			}
		}

        private bool PopulateField(DocsPAWA.DocsPaWR.FiltroRicerca[][] qV, bool grid)
		{
			try
			{	
				if (qV==null || qV.Length==0)
					return false;

				#region pulizia campi
                #region tipo
                //for (int i = 0; i < this.cbl_archDoc_C.Items.Count; i++)
                //{
                //    this.cbl_archDoc_C.Items[i].Selected = true;
                //}
                #endregion
                #region Mancanza_immagine, Mancanza_Fascicolazione e Predisposto alla Protocollazione
                this.rbl_fascicolazione.Items.FindByValue("Reset").Selected = true;
                this.rbl_immagine.Items.FindByValue("Reset").Selected = true;
                #endregion
                #region REGISTRO
                foreach (ListItem i in lb_reg.Items)
					i.Selected = true;
				#endregion REGISTRO
                #region NUM_PROTOCOLLO
                ddl_numProt_C.SelectedIndex = 0;
                ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
                txt_initNumProt_C.Text = "";
                #endregion NUM_PROTOCOLLO
				#region OGGETTO
				txt_oggetto.Text = "";
				#endregion OGGETTO
				#region MITT_DEST
				txt_codMit.Text = "";
				txt_descrMit.Text = "";
				#endregion MITT_DEST
				#region DATA_PROT
				ddl_dataProt.SelectedIndex = 0;
				ddl_dataProt_SelectedIndexChanged(null,new System.EventArgs());
				this.GetCalendarControl("txt_initDataProt").txt_Data.Text = "";
				#endregion DATA_PROT
				#region DATA_CREAZIONE
				ddl_dataCreaz.SelectedIndex = 0;
				ddl_dataCreaz_SelectedIndexChanged(null,new System.EventArgs());
				this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = "";
				#endregion DATA_CREAZIONE
				#region PROTOCOLLO_MITTENTE
				txt_numProtMitt_C.Text = "";
				#endregion PROTOCOLLO_MITTENTE
				#region DATA_PROT_MITTENTE
				ddl_dataProtMitt_C.SelectedIndex = 0;
				ddl_dataProtMitt_C_SelectedIndexChanged(null,new System.EventArgs());
				this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = "";
				#endregion DATA_PROT_MITTENTE
                #region Trasmissione Con/Senza
                this.cbx_Trasm.Checked = false;
                this.cbx_TrasmSenza.Checked = false;
                #endregion
                #region DOCNUMBER
                ddl_idDocumento_C.SelectedIndex = 0;
                ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                txt_initIdDoc_C.Text = "";
                #endregion DOCNUMBER
                #region NUM_PROTOCOLLO
                ddl_numProt_C.SelectedIndex = 0;
                ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
                txt_initNumProt_C.Text = "";
                #endregion NUM_PROTOCOLLO
                #region ANNO_PROTOCOLLO
                tbAnnoProtocollo.Text = DateTime.Now.Year.ToString();
                #endregion ANNO_PROTOCOLLO
                #region RIFERIMENTO MITTENTE
                DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                if (wws.isEnableRiferimentiMittente())
                {
                    this.txt_rif_mittente.Text = string.Empty;
                }
                #endregion
                #endregion pulizia campi

                DocsPaWR.FiltroRicerca[] filters = qV[0];
				//array contenitore degli array filtro di ricerca
				
				if (this.Session["itemUsedSearch"]!=null)
				{
					ddl_Ric_Salvate.SelectedIndex = Convert.ToInt32(this.Session["itemUsedSearch"]);
				}

				foreach (DocsPAWA.DocsPaWR.FiltroRicerca aux in filters)
				{
					try
					{
                        //    #region MANCANZA_ASSEGNAZIONE
                        //if (aux.argomento==DocsPaWR.FiltriDocumento.MANCANZA_ASSEGNAZIONE.ToString())
                        //{
                        //    rbl_documentiInCompletamento.Items[0].Selected = true;
                        //}
                        //    #endregion MANCANZA_ASSEGNAZIONE
                        #region PROTOCOLLO_ARRIVO
                        if (aux.argomento == DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString())
                        {
                            this.cbl_archDoc_C.Items.FindByValue("A").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region PROTOCOLLO_PARTENZA
                        if (aux.argomento == DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString())
                        {
                            this.cbl_archDoc_C.Items.FindByValue("P").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region PROTOCOLLO_INTERNO
                        if (aux.argomento == DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString())
                        {
                            if (this.cbl_archDoc_C.Items.FindByValue("I") != null)
                                this.cbl_archDoc_C.Items.FindByValue("I").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region NUM_PROTOCOLLO
                        if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString())
                        {
                            if (ddl_numProt_C.SelectedIndex != 0)
                                ddl_numProt_C.SelectedIndex = 0;
                            ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_initNumProt_C.Text = aux.valore;
                        }
                        #endregion NUM_PROTOCOLLO
                        #region GRIGI
                        if (aux.argomento == DocsPaWR.FiltriDocumento.GRIGIO.ToString())
                        {
                            this.cbl_archDoc_C.Items.FindByValue("G").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region PREDISPOSTI
                        if (aux.argomento == DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString())
                        {
                            this.cbl_archDoc_C.Items.FindByValue("Pr").Selected = Convert.ToBoolean(aux.valore);
                        }
                        #endregion
                        #region MANCANZA_IMMAGINE
						if (aux.argomento==DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString())
						{
							//rbl_documentiInCompletamento.Items[1].Selected = true;
                            if (aux.valore.Equals("1"))
                                this.rbl_immagine.Items.FindByValue("1").Selected = true;
                            if (aux.valore.Equals("0"))
                                this.rbl_immagine.Items.FindByValue("0").Selected = true;
						}
							#endregion MANCANZA_IMMAGINE
						#region MANCANZA_FASCICOLAZIONE
						else if (aux.argomento==DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString())
						{
							//rbl_documentiInCompletamento.Items[3].Selected = true;
                            if (aux.valore.Equals("1"))
                                this.rbl_fascicolazione.Items.FindByValue("1").Selected = true;
                            if (aux.valore.Equals("0"))
                                this.rbl_fascicolazione.Items.FindByValue("1").Selected = true;
                        }
							#endregion MANCANZA_FASCICOLAZIONE
						#region REGISTRO
						else if (aux.argomento==DocsPaWR.FiltriDocumento.REGISTRO.ToString())
						{
							char[] sep = {','};
							string[] regs = aux.valore.Split(sep);
							foreach (ListItem li in lb_reg.Items)
								li.Selected = false;
							foreach (string reg in regs)
							{
								for (int i=0; i<lb_reg.Items.Count; i++)
								{
									if (lb_reg.Items[i].Value==reg)
										lb_reg.Items[i].Selected = true;
								}
							}
						}
							#endregion REGISTRO
						#region OGGETTO
						else if (aux.argomento==DocsPaWR.FiltriDocumento.OGGETTO.ToString())
						{
							txt_oggetto.Text = aux.valore;
						}
							#endregion OGGETTO
						#region MITT_DEST
						else if (aux.argomento==DocsPaWR.FiltriDocumento.MITT_DEST.ToString())
						{
							txt_descrMit.Text = aux.valore;
						}
							#endregion MITT_DEST
                        #region COD_MITT_DEST
                            else if (aux.argomento == DocsPaWR.FiltriDocumento.COD_MITT_DEST.ToString())
                            {
                                DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubrica(this, aux.valore);
                                txt_codMit.Text = corr.codiceRubrica;
                                txt_descrMit.Text = corr.descrizione;
                            }
                            #endregion
                        #region MITT_DEST_STORICIZZATI
                            else if (aux.argomento == DocsPaWR.FiltriDocumento.MITT_DEST_STORICIZZATI.ToString())
                            {
                                bool chkValue;
                                bool.TryParse(aux.valore, out chkValue);
                                this.cb_mitt_dest_storicizzati.Checked = chkValue;
                            }
                            #endregion
    					#region ID_MITT_DEST
						else if (aux.argomento==DocsPaWR.FiltriDocumento.ID_MITT_DEST.ToString())
						{
							DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemID(this,aux.valore);
							txt_codMit.Text = corr.codiceRubrica;
							txt_descrMit.Text = corr.descrizione;
						}
							#endregion ID_MITT_DEST
						#region DATA_PROT_IL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString())
						{
							if (ddl_dataProt.SelectedIndex!=0)
								ddl_dataProt.SelectedIndex = 0;
							ddl_dataProt_SelectedIndexChanged(null,new System.EventArgs());
							this.GetCalendarControl("txt_initDataProt").txt_Data.Text = aux.valore;
						}
							#endregion DATA_PROT_IL
						#region DATA_PROT_SUCCESSIVA_AL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString())
						{
							if (ddl_dataProt.SelectedIndex!=1)
								ddl_dataProt.SelectedIndex = 1;
							ddl_dataProt_SelectedIndexChanged(null,new System.EventArgs());
							this.GetCalendarControl("txt_initDataProt").txt_Data.Text = aux.valore;
						}
							#endregion DATA_PROT_SUCCESSIVA_AL
						#region DATA_PROT_PRECEDENTE_IL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString())
						{
							if (ddl_dataProt.SelectedIndex!=1)
								ddl_dataProt.SelectedIndex = 1;
							ddl_dataProt_SelectedIndexChanged(null,new System.EventArgs());
							this.GetCalendarControl("txt_fineDataProt").txt_Data.Text = aux.valore;
						}
							#endregion DATA_PROT_PRECEDENTE_IL
                        #region DATA_PROT_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProt.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataProt").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataProt").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProt").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineDataProt").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt").btn_Cal.Enabled = false;
                            this.lbl_finedataProt.Visible = true;
                            this.lbl_initdataProt.Visible = true;
                        }
                        #endregion
                        #region DATA_PROT_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProt.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initDataProt").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initDataProt").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProt").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt").Visible = true;
                            this.GetCalendarControl("txt_fineDataProt").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineDataProt").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataProt").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt").btn_Cal.Enabled = false;
                            this.lbl_finedataProt.Visible = true;
                            this.lbl_initdataProt.Visible = true;
                        }
                        #endregion
                        #region DATA_PROT_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProt.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initDataProt").Visible = true;
                            this.GetCalendarControl("txt_initDataProt").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initDataProt").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataProt").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProt").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataProt").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProt").Visible = false;
                            this.GetCalendarControl("txt_fineDataProt").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineDataProt").btn_Cal.Visible = false;
                            this.lbl_finedataProt.Visible = false;
                            this.lbl_initdataProt.Visible = false;
                        }
                        #endregion
                        #region DATA_CREAZIONE_IL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString())
						{
							if (ddl_dataCreaz.SelectedIndex!=0)
								ddl_dataCreaz.SelectedIndex = 0;
							ddl_dataCreaz_SelectedIndexChanged(null,new System.EventArgs());
							this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = aux.valore;
						}
							#endregion DATA_CREAZIONE_IL
						#region DATA_CREAZIONE_SUCCESSIVA_AL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString())
						{
							if (ddl_dataCreaz.SelectedIndex!=1)
								ddl_dataCreaz.SelectedIndex = 1;
							ddl_dataCreaz_SelectedIndexChanged(null,new System.EventArgs());
							this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = aux.valore;
						}
							#endregion DATA_CREAZIONE_SUCCESSIVA_AL
						#region DATA_CREAZIONE_PRECEDENTE_IL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString())
						{
							if (ddl_dataCreaz.SelectedIndex!=1)
								ddl_dataCreaz.SelectedIndex = 1;
							ddl_dataCreaz_SelectedIndexChanged(null,new System.EventArgs());
							this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text = aux.valore;
						}
							#endregion DATA_CREAZIONE_PRECEDENTE_IL
                        #region DATA_CREAZ_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataCreaz.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataCreaz").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Enabled = false;
                            this.lbl_finedataCreaz.Visible = true;
                            this.lbl_initdataCreaz.Visible = true;
                        }
                        #endregion
                        #region DATA_CREAZ_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataCreaz.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initDataCreaz").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Enabled = false;
                            this.lbl_finedataCreaz.Visible = true;
                            this.lbl_initdataCreaz.Visible = true;
                        }
                        #endregion
                        #region DATA_CREAZ_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataCreaz.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initDataCreaz").Visible = true;
                            this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initDataCreaz").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataCreaz").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataCreaz").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreaz").Visible = false;
                            this.GetCalendarControl("txt_fineDataCreaz").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineDataCreaz").btn_Cal.Visible = false;
                            this.lbl_finedataCreaz.Visible = false;
                            this.lbl_initdataCreaz.Visible = false;
                        }
                        #endregion
                        #region PROTOCOLLO_MITTENTE
						else if (aux.argomento==DocsPaWR.FiltriDocumento.PROTOCOLLO_MITTENTE.ToString())
						{
							txt_numProtMitt_C.Text = aux.valore;
						}
							#endregion PROTOCOLLO_MITTENTE
						#region DATA_PROT_MITTENTE_IL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_IL.ToString())
						{
							if (ddl_dataProtMitt_C.SelectedIndex!=0)
								ddl_dataProtMitt_C.SelectedIndex = 0;
							ddl_dataProtMitt_C_SelectedIndexChanged(null,new System.EventArgs());
							this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = aux.valore;
						}
							#endregion DATA_PROT_MITTENTE_IL
						#region DATA_PROT_MITTENTE_SUCCESSIVA_AL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SUCCESSIVA_AL.ToString())
						{
							if (ddl_dataProtMitt_C.SelectedIndex!=1)
								ddl_dataProtMitt_C.SelectedIndex = 1;
							ddl_dataProtMitt_C_SelectedIndexChanged(null,new System.EventArgs());
							this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = aux.valore;
						}
							#endregion DATA_PROT_MITTENTE_SUCCESSIVA_AL
						#region DATA_PROT_MITTENTE_PRECEDENTE_IL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_PRECEDENTE_IL.ToString())
						{
							if (ddl_dataProtMitt_C.SelectedIndex!=1)
								ddl_dataProtMitt_C.SelectedIndex = 1;
							ddl_dataProtMitt_C_SelectedIndexChanged(null,new System.EventArgs());
							this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text = aux.valore;
						}
								#endregion DATA_PROT_MITTENTE_PRECEDENTE_IL
                        #region DATA_PROT_MITTENTE_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProtMitt_C.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Enabled = false;
                            this.lbl_finedataProtMitt_C.Visible = true;
                            this.lbl_initdataProtMitt_C.Visible = true;
                        }
                        #endregion
                        #region DATA_PROT_MITTENTE_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProtMitt_C.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Enabled = false;
                            this.lbl_finedataProtMitt_C.Visible = true;
                            this.lbl_initdataProtMitt_C.Visible = true;
                        }
                        #endregion
                        #region DATA_PROT_MITTENTE_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_PROT_MITTENTE_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataProtMitt_C.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initDataProtMitt_C").Visible = true;
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataProtMitt_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").Visible = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineDataProtMitt_C").btn_Cal.Visible = false;
                            this.lbl_finedataProtMitt_C.Visible = false;
                            this.lbl_initdataProtMitt_C.Visible = false;
                        }
                        #endregion
                        #region TRASMISSIONE CON
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_CON.ToString())
                        {
                            this.cbx_Trasm.Checked = true;
                            if (aux.valore.Equals("Tutte"))
                                this.ddl_ragioneTrasm.SelectedIndex = 0;
                            else
                                this.ddl_ragioneTrasm.SelectedValue = aux.valore;
                        }
                        #endregion
                        #region DOCNUMBER
                        if (aux.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER.ToString())
                        {
                            if (ddl_idDocumento_C.SelectedIndex != 0)
                                ddl_idDocumento_C.SelectedIndex = 0;
                            ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_initIdDoc_C.Text = aux.valore;
                        }
                        #endregion DOCNUMBER
                        #region DOCNUMBER_DAL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString())
                        {
                            if (ddl_idDocumento_C.SelectedIndex != 1)
                                ddl_idDocumento_C.SelectedIndex = 1;
                            ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_initIdDoc_C.Text = aux.valore;
                        }
                        #endregion DOCNUMBER_DAL
                        #region DOCNUMBER_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString())
                        {
                            if (ddl_idDocumento_C.SelectedIndex != 1)
                                ddl_idDocumento_C.SelectedIndex = 1;
                            ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_fineIdDoc_C.Text = aux.valore;
                        }
                        #endregion DOCNUMBER_AL
                        #region TRASMISSIONE SENZA
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.TRASMESSI_SENZA.ToString())
                        {
                            this.cbx_TrasmSenza.Checked = true;
                            if (aux.valore.Equals("Tutte"))
                                this.ddl_ragioneTrasm.SelectedIndex = 0;
                            else
                                this.ddl_ragioneTrasm.SelectedValue = aux.valore;
                        }
                        #endregion
                        #region Creatore (User Control)
                        //else if (aux.argomento == DocsPaWR.FiltriDocumento.ID_PEOPLE_CREATORE.ToString()
                        //    || aux.argomento == DocsPaWR.FiltriDocumento.ID_UO_CREATORE.ToString()
                        //    || aux.argomento == DocsPaWR.FiltriDocumento.ID_RUOLO_CREATORE.ToString()
                        //    || aux.argomento == DocsPaWR.FiltriDocumento.DESC_PEOPLE_CREATORE.ToString()
                        //    || aux.argomento == DocsPaWR.FiltriDocumento.DESC_RUOLO_CREATORE.ToString()
                        //    || aux.argomento == DocsPaWR.FiltriDocumento.DESC_UO_CREATORE.ToString()
                        //    )
                        //{
                        //    this.Creatore.RestoreCurrentFilters();
                        //}

                        #endregion
                        #region NUM_PROTOCOLLO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString())
                        {
                            if (ddl_numProt_C.SelectedIndex != 0)
                                ddl_numProt_C.SelectedIndex = 0;
                            ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_initNumProt_C.Text = aux.valore;
                        }
                        #endregion NUM_PROTOCOLLO
                        #region NUM_PROTOCOLLO_DAL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString())
                        {
                            if (ddl_numProt_C.SelectedIndex != 1)
                                ddl_numProt_C.SelectedIndex = 1;
                            ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_initNumProt_C.Text = aux.valore;
                        }
                        #endregion NUM_PROTOCOLLO_DAL
                        #region NUM_PROTOCOLLO_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString())
                        {
                            if (ddl_numProt_C.SelectedIndex != 1)
                                ddl_numProt_C.SelectedIndex = 1;
                            ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
                            txt_fineNumProt_C.Text = aux.valore;
                        }
                        #endregion NUM_PROTOCOLLO_AL
                        #region ANNO_PROTOCOLLO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString())
                        {
                            tbAnnoProtocollo.Text = aux.valore;
                        }
                        #endregion ANNO_PROTOCOLLO
                        #region CODICE_DESCRIZIONE_AMMINISTRAZIONE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.CODICE_DESCRIZIONE_AMMINISTRAZIONE.ToString())
                        {
                            if(!string.IsNullOrEmpty(aux.valore))
                                txt_codDesc.Text = aux.valore;
                        }
                        #endregion
                        #region CODICE_TIPO_NOTIFICA
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.CODICE_TIPO_NOTIFICA.ToString())
                        {
                            for(int i=0;i<ddl_ricevute_pec.Items.Count;i++)
                            {
                                if (ddl_ricevute_pec.Items[i].Value.ToUpper().Equals(aux.valore.ToUpper()))
                                {
                                    ddl_ricevute_pec.SelectedIndex = i;
                                    break;
                                }
                             }
                            ddl_data_ricevute_pec.SelectedIndex = 0;
                            ddl_data_ricevute_pec_SelectedIndexChanged(null, new System.EventArgs());
                        }
                        #endregion
                        #region CODICE_TIPO_NOTIFICA_PITRE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.CODICE_TIPO_NOTIFICA_PITRE.ToString())
                        {
                            for (int i = 0; i < ddl_ricevute_pitre.Items.Count; i++)
                            {
                                if (ddl_ricevute_pitre.Items[i].Value.ToUpper().Equals(aux.valore.ToUpper()))
                                {
                                    ddl_ricevute_pitre.SelectedIndex = i;
                                    break;
                                }
                            }
                            ddl_data_ricevute_pitre.SelectedIndex = 0;
                            ddl_data_ricevute_pitre_SelectedIndexChanged(null, new System.EventArgs());
                        }
                        #endregion
                        #region DATA_TIPO_NOTIFICA_DA
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_DA.ToString())
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                            {
                                ddl_data_ricevute_pec.SelectedIndex = 0;
                                ddl_data_ricevute_pec_SelectedIndexChanged(null, new System.EventArgs());
                                this.GetCalendarControl("Cal_Da_pec").txt_Data.Text = aux.valore;
                            }
                        }
                        #endregion
                        #region DATA_TIPO_NOTIFICA_DA_PITRE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_DA_PITRE.ToString())
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                            {
                                ddl_data_ricevute_pitre.SelectedIndex = 0;
                                ddl_data_ricevute_pitre_SelectedIndexChanged(null, new System.EventArgs());
                                this.GetCalendarControl("Cal_Da_pitre").txt_Data.Text = aux.valore;
                            }
                        }
                        #endregion
                        #region DATA_TIPO_NOTIFICA_A
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_A.ToString())
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                            {
                                ddl_data_ricevute_pec.SelectedIndex = 1;
                                ddl_data_ricevute_pec_SelectedIndexChanged(null, new System.EventArgs());
                                this.GetCalendarControl("Cal_A_pec").txt_Data.Text = aux.valore;
                            }
                        }
                        #endregion
                        #region DATA_TIPO_NOTIFICA_A_PITRE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_A_PITRE.ToString())
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                            {
                                ddl_data_ricevute_pitre.SelectedIndex = 1;
                                ddl_data_ricevute_pitre_SelectedIndexChanged(null, new System.EventArgs());
                                this.GetCalendarControl("Cal_A_pitre").txt_Data.Text = aux.valore;
                            }
                        }
                        #endregion
                        #region DATA_TIPO_NOTIFICA_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_TODAY.ToString())
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                            {
                                ddl_data_ricevute_pec.SelectedIndex = 2;
                                ddl_data_ricevute_pec_SelectedIndexChanged(null, new System.EventArgs());
                                this.GetCalendarControl("Cal_DA_pec").txt_Data.Text = aux.valore;
                            }
                        }
                        #endregion
                        #region DATA_TIPO_NOTIFICA_TODAY_PITRE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_NOTIFICA_TODAY_PITRE.ToString())
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                            {
                                ddl_data_ricevute_pitre.SelectedIndex = 2;
                                ddl_data_ricevute_pitre_SelectedIndexChanged(null, new System.EventArgs());
                                this.GetCalendarControl("Cal_DA_pitre").txt_Data.Text = aux.valore;
                            }
                        }
                        #endregion
                        #region DOC_SPEDITI
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DOC_SPEDITI_TIPO.ToString())
                        {
                            switch (aux.valore)
                            {
                                case "PEC":
                                    if (cbx_pec.Visible)
                                        cbx_pec.Checked = true;
                                    if (cbx_pitre.Visible)
                                        cbx_pitre.Checked = false;
                                    break;

                                case "PITRE":
                                    if (cbx_pec.Visible)
                                        cbx_pec.Checked = false;
                                    if (cbx_pitre.Visible)
                                        cbx_pitre.Checked = true;
                                    break;

                                case "ALL":
                                    if (cbx_pec.Visible)
                                        cbx_pec.Checked = true;
                                    if (cbx_pitre.Visible)
                                        cbx_pitre.Checked = true;
                                    break;
                            }
                        }
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DOC_SPEDITI.ToString())
                        {
                            if (aux.valore == "1")
                                this.rb_docSpediti.SelectedIndex = 0;
                            if (aux.valore == "0")
                                this.rb_docSpediti.SelectedIndex = 1;
                            if (aux.valore == "T")
                                this.rb_docSpediti.SelectedIndex = 2;
                        }
                        #endregion
                        #region FILTRI TIMESTAMP
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.SENZA_TIMESTAMP.ToString())
                        {
                            rbl_timestamp.SelectedValue = "1";
                            ddl_timestamp.SelectedValue = "0";
                            date_timestamp.Text = string.Empty;
                            ddl_timestamp.Visible = false;
                            date_timestamp.Visible = false;
                        }
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.CON_TIMESTAMP.ToString())
                        {
                            rbl_timestamp.SelectedValue = "0";
                            ddl_timestamp.SelectedValue = "0";
                            date_timestamp.Text = string.Empty;
                            ddl_timestamp.Visible = true;
                            date_timestamp.Visible = false;
                        }
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.TIMESTAMP_SCADUTO.ToString())
                        {
                            rbl_timestamp.SelectedValue = "0";
                            ddl_timestamp.SelectedValue = "1";
                            date_timestamp.Text = string.Empty;
                            ddl_timestamp.Visible = true;
                            date_timestamp.Visible = false;
                        }
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.TIMESTAMP_SCADE_PRIMA_DI.ToString())
                        {
                            rbl_timestamp.SelectedValue = "0";
                            ddl_timestamp.SelectedValue = "2";
                            date_timestamp.Text = aux.valore;
                            ddl_timestamp.Visible = true;
                            date_timestamp.Visible = true;
                        }
                        #endregion FILTRI TIMESTAMP
                        #region FILTRI CONSOLIDAMENTO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.STATO_CONSOLIDAMENTO.ToString())
                        {
                            documentConsolidationSearch.setStateResaerchConsolidation(aux);
                        }
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CONSOLIDAMENTO_DA.ToString())
                        {
                            documentConsolidationSearch.setDataConsolidamentoDa(aux.valore);
                        }
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CONSOLIDAMENTO_A.ToString())
                        {
                            documentConsolidationSearch.setDataConsolidamentoA(aux.valore);
                        }
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_TIPO_CONSOLIDAMENTO.ToString())
                        {
                            documentConsolidationSearch.setStateDdlIntervallo(aux.valore);
                        }
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ID_UTENTE_CONSOLIDANTE.ToString())
                        {
                        }
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ID_RUOLO_CONSOLIDANTE.ToString())
                        {
                        }
                        #endregion
                        #region RIFERIMENTO MITTENTE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.RIFERIMENTO_MITTENTE.ToString())
                        {
                            if (wws.isEnableRiferimentiMittente())
                            {
                                txt_rif_mittente.Text = aux.valore;
                            }

                        }
                        #endregion
                        #region ORDINAMENTO ASC/DESC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ORDER_DIRECTION.ToString() && !change_from_grid && !grid)
                        {
                            if (!string.IsNullOrEmpty(aux.valore))
                            {
                                this.ddlOrderDirection.SelectedValue = aux.valore;
                            }

                        }
                        #endregion
                        #region ORDINAMENTO TIPO
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.ORACLE_FIELD_FOR_ORDER.ToString() && !change_from_grid && !grid)
                        {
                            if (!string.IsNullOrEmpty(aux.nomeCampo))
                            {
                                if (this.ddlOrder.SelectedValue.Contains(aux.nomeCampo))
                                {
                                    this.ddlOrder.SelectedValue = aux.nomeCampo;
                                }
                            }
                        }
                        #endregion
                    }
					catch (Exception)
					{
						throw new Exception("I criteri di ricerca non sono piu\' validi.");
					}
				}

                this.documentConsolidationSearch.LoadFilters(filters);

				return true;

			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
				return false;
			}
		}

		#region Gestione CallContext

		/// <summary>
		/// Impostazione numero pagina corrente del contesto di ricerca
		/// </summary>
		private void SetPageOnCurrentContext()
		{
			SiteNavigation.CallContext currentContext=SiteNavigation.CallContextStack.CurrentContext;
			currentContext.PageNumber=1;
		}

		#endregion

      protected void btn_ricerca_Click1(object sender, EventArgs e)
      {

      }
      private void caricaComboTipoFileAcquisiti()
      {
          ArrayList tipoFile = new ArrayList();
          tipoFile = DocumentManager.getExtFileAcquisiti(this);
          bool firmati = false;
          for (int i = 0; i < tipoFile.Count; i++)
          {
              //if (tipoFile[i].ToString().Contains("P7M"))
              //{
              //    if (!firmati)
              //    {
              //        ListItem item = new ListItem(tipoFile[i].ToString().Substring(tipoFile[i].ToString().Length - 3));
              //        this.ddl_tipoFileAcquisiti.Items.Add(item);
              //        firmati = true;
              //    }
              //}
              //else
              //{
              if (!tipoFile[i].ToString().Contains("P7M"))
              {
                  ListItem item = new ListItem(tipoFile[i].ToString());
                  this.ddl_tipoFileAcquisiti.Items.Add(item);
              }
              //}
          }
      }

      //Inserita da Fabio
      private void getLettereProtocolli()
      {

          DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
          DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
          string idAmm = cr.idAmministrazione;
          DocsPAWA.DocsPaWR.InfoUtente infoUtente = new DocsPAWA.DocsPaWR.InfoUtente();
          this.etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
          if ((etichette[0].Etichetta).Length > 3)
          {
              this.opArr.Text = ((etichette[0].Etichetta).Substring(0, 3)) + "."; //Valore A
          }
          else
          {
              this.opArr.Text = etichette[0].Etichetta;
          }
          if ((etichette[1].Etichetta).Length > 3)
          {
              //CASO PER INFORMATICA TRENTINA PER LASCIARE 4 CARATTERI (Part.)
              if (etichette[1].Etichetta.Equals("Partenza"))
              {
                  this.opPart.Text = "Part.";
              }
              else
              {
                  this.opPart.Text = ((etichette[1].Etichetta).Substring(0, 3)) + "."; //Valore P
              }
          }
          else
          {
              this.opPart.Text = etichette[1].Etichetta;
          }
          if ((etichette[2].Etichetta).Length > 3)
          {
              this.opInt.Text = ((etichette[2].Etichetta).Substring(0, 3)) + ".";//Valore I
          }
          else
          {
              this.opInt.Text = etichette[2].Etichetta;
          }
          if ((etichette[3].Etichetta).Length > 3)
          {
              this.opGrigio.Text = ((etichette[3].Etichetta).Substring(0, 3)) + ".";//Valore G
          }
          else
          {
              this.opGrigio.Text = etichette[3].Etichetta;
          }
          if ((etichette[4].Etichetta).Length > 3)
          {
              this.opAll.Text = ((etichette[4].Etichetta).Substring(0, 3)) + ".";//Valore ALL
          }
          else
          {
              this.opAll.Text = etichette[4].Etichetta;
          }




      }

      protected void rbl_timestamp_SelectedIndexChanged(object sender, System.EventArgs e)
      {
          if (rbl_timestamp.SelectedValue == "0")
          {
              ddl_timestamp.Visible = true;              
          }
          else
          {
              ddl_timestamp.Visible = false;
              ddl_timestamp.SelectedIndex = -1;
              date_timestamp.Visible = false;
              date_timestamp.Text = string.Empty;
          }
      }

      protected void ddl_timestamp_SelectedIndexChanged(object sender, System.EventArgs e)
      {
          if (ddl_timestamp.SelectedValue == "2")
          {
              date_timestamp.Visible = true;
          }
          else
          {
              date_timestamp.Visible = false;
              date_timestamp.Text = string.Empty;
          }
      }

      protected void ModifyRapidSearch_Click(object sender, EventArgs e)
      {
          if (ricercaCompletamento())
          {
              // Impostazione del filtro utilizzato
              GridManager.SetSearchFilter(this.ddlOrder.SelectedItem.Text, this.ddlOrderDirection.SelectedValue);

              schedaRicerca.FiltriRicerca = qV;
              schedaRicerca.ProprietaNuovaRicerca = new DocsPAWA.ricercaDoc.SchedaRicerca.NuovaRicerca();
              if (this.ddl_Ric_Salvate.SelectedIndex > 0)
              {
                  string idRicercaSalvata = this.ddl_Ric_Salvate.SelectedItem.Value.ToString();
                  ClientScript.RegisterStartupScript(this.GetType(), "modificaRicerca", "apriModificaRicerca(" + idRicercaSalvata + ");", true);
              }
              else
              {
                  ClientScript.RegisterStartupScript(this.GetType(), "alertModifica", "alert(Selezionare una ricerca salvata da modificare);", true);
              }
          }
      }

      protected void btnCleanUpField_Click(object sender, EventArgs e)
      {

          this.rbl_fascicolazione.Items.FindByValue("Reset").Selected = true;
          this.rbl_immagine.Items.FindByValue("Reset").Selected = true;
   
          foreach (ListItem i in lb_reg.Items)
              i.Selected = true;
    
          ddl_numProt_C.SelectedIndex = 0;
          ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
          txt_initNumProt_C.Text = "";
       
          txt_oggetto.Text = "";
          
          txt_codMit.Text = "";
          txt_descrMit.Text = "";

          ddl_dataProt.SelectedIndex = 0;
          ddl_dataProt_SelectedIndexChanged(null, new System.EventArgs());
          this.GetCalendarControl("txt_initDataProt").txt_Data.Text = "";
      
          ddl_dataCreaz.SelectedIndex = 0;
          ddl_dataCreaz_SelectedIndexChanged(null, new System.EventArgs());
          this.GetCalendarControl("txt_initDataCreaz").txt_Data.Text = "";
    
          txt_numProtMitt_C.Text = "";
     
          ddl_dataProtMitt_C.SelectedIndex = 0;
          ddl_dataProtMitt_C_SelectedIndexChanged(null, new System.EventArgs());
          this.GetCalendarControl("txt_initDataProtMitt_C").txt_Data.Text = "";
   
          this.cbx_Trasm.Checked = false;
          this.cbx_TrasmSenza.Checked = false;
    
          ddl_idDocumento_C.SelectedIndex = 0;
          ddl_idDocumento_C_SelectedIndexChanged(null, new System.EventArgs());
          txt_initIdDoc_C.Text = "";

          ddl_numProt_C.SelectedIndex = 0;
          ddl_numProt_C_SelectedIndexChanged(null, new System.EventArgs());
          txt_initNumProt_C.Text = "";

          tbAnnoProtocollo.Text = DateTime.Now.Year.ToString();

          this.ddl_Ric_Salvate.SelectedIndex = 0;

          if (this.cbl_archDoc_C.Items.FindByValue("A") != null)
          {
              this.cbl_archDoc_C.Items.FindByValue("A").Selected = true;
          }
          if (this.cbl_archDoc_C.Items.FindByValue("P") != null)
          {
              this.cbl_archDoc_C.Items.FindByValue("P").Selected = true;
          }
          if (this.cbl_archDoc_C.Items.FindByValue("I") != null)
          {
              this.cbl_archDoc_C.Items.FindByValue("I").Selected = true;
          }
          if (this.cbl_archDoc_C.Items.FindByValue("G") != null)
          {
              this.cbl_archDoc_C.Items.FindByValue("G").Selected = true;
          }
          if (this.cbl_archDoc_C.Items.FindByValue("Pr") != null)
          {
              this.cbl_archDoc_C.Items.FindByValue("Pr").Selected = false;
          }
          if (this.cbl_archDoc_C.Items.FindByValue("ALL") != null)
          {
              this.cbl_archDoc_C.Items.FindByValue("ALL").Selected = false;
          }

          ddl_ragioneTrasm.SelectedIndex = 0;

          //UserManager.removeCreatoreSelezionato(this.Page);
          //this.Creatore.Clear();

          ddl_tipoFileAcquisiti.SelectedIndex = 0;
          chkFirmato.Checked = false;
          chkNonFirmato.Checked = false;

          rb_docSpediti.SelectedValue = "R";

          this.GetCalendarControl("txt_dataSpedDa").txt_Data.Text = "";
          this.GetCalendarControl("txt_dataSpedA").txt_Data.Text = "";

          if (this.aofAuthor.Visible)
              this.aofAuthor.DeleteFilters();

          if (this.aofOwner.Visible)
              this.aofOwner.DeleteFilters();

          GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);

          if (p_cod_amm.Visible)
          {
              this.txt_codDesc.Text = string.Empty;
          }
      }
	}
}

		
