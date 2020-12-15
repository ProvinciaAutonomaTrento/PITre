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
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;
using System.Collections.Generic;

namespace DocsPAWA.ricercaDoc
{
	/// <summary>
	/// Summary description for ricDocGrigia.
	/// </summary>
	public class ricDocGrigia : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.DropDownList ddl_tipoAtto;
		protected System.Web.UI.WebControls.ListBox ListParoleChiave;
		protected System.Web.UI.WebControls.TextBox txt_oggetto;
		protected System.Web.UI.WebControls.Button btn_ricerca;
		protected DocsPAWA.DocsPaWR.InfoUtente Safe;		
		protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
		protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
		protected System.Web.UI.WebControls.ImageButton btn_selezionaParoleChiave;
		protected System.Web.UI.WebControls.ImageButton btn_RubrOgget;
		protected System.Web.UI.WebControls.ImageButton btn_oggettario;
		protected System.Web.UI.WebControls.DropDownList ddl_idDocumento_C;
		protected System.Web.UI.WebControls.DropDownList ddl_dataCreazione_C;
        protected System.Web.UI.WebControls.DropDownList ddl_dataScadenza_G;
        protected System.Web.UI.WebControls.Label lbl_da_dtaScadenza_G;
        protected System.Web.UI.WebControls.Label lbl_a_dtaScadenza_G;
        //protected DocsPaWebCtrlLibrary.DateMask txt_initScadenza_G;
        protected DocsPAWA.UserControls.Calendar txt_initScadenza_G;
        //protected DocsPaWebCtrlLibrary.DateMask txt_fineScadenza_G;
        protected DocsPAWA.UserControls.Calendar txt_fineScadenza_G;
		//protected DocsPaWebCtrlLibrary.DateMask txt_fineDataCreaz_C;
        protected DocsPAWA.UserControls.Calendar txt_fineDataCreaz_C;
		protected System.Web.UI.WebControls.Label lblAdataCreaz_C;
		//protected DocsPaWebCtrlLibrary.DateMask txt_initDataCreaz_C;
        protected DocsPAWA.UserControls.Calendar txt_initDataCreaz_C;
        protected System.Web.UI.WebControls.Label lblDAdataCreaz_C;
		protected System.Web.UI.WebControls.TextBox txt_fineIdDoc_C;
		protected System.Web.UI.WebControls.Label lblAidDoc_C;
		protected System.Web.UI.WebControls.TextBox txt_initIdDoc_C;
		protected System.Web.UI.WebControls.Label lblDAidDoc_C;
		protected System.Web.UI.WebControls.TextBox txt_numOggetto;
		protected System.Web.UI.WebControls.TextBox txt_commRef;
		protected System.Web.UI.WebControls.Panel panel_numOgg_commRef;
        protected UserControls.RicercaNote rn_note;
		protected System.Web.UI.WebControls.RadioButtonList rbl_documentiInCompletamento;
		protected System.Web.UI.WebControls.ImageButton enterKeySimulator;
		protected System.Web.UI.WebControls.TextBox txt_CodFascicolo;
		protected System.Web.UI.WebControls.TextBox txt_DescFascicolo;
		protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;
		protected System.Web.UI.WebControls.ImageButton btn_CampiPersonalizzati;
		protected System.Web.UI.WebControls.DropDownList ddl_statiDoc;
		protected System.Web.UI.WebControls.Panel Panel_StatiDocumento;
		protected System.Web.UI.WebControls.ImageButton btn_Canc_Ric;
		protected System.Web.UI.WebControls.DropDownList ddl_Ric_Salvate;
		protected System.Web.UI.WebControls.Button btn_salva;
		protected Utilities.MessageBox mb_ConfirmDelete;
        protected DocsPaWebCtrlLibrary.ImageButton imgFasc;
        protected string codClassifica = "";
        //protected UserControls.Creatore Creatore;
        protected System.Web.UI.WebControls.Label lblSearch;
        protected System.Web.UI.HtmlControls.HtmlTableRow trFiltroTipiDocumento;
        protected System.Web.UI.WebControls.CheckBoxList cblTipiDocumento;

        protected System.Web.UI.WebControls.DropDownList ddlOrder, ddlOrderDirection;

        private const string KEY_SCHEDA_RICERCA = "RicercaDocGrigia";

		public SchedaRicerca schedaRicerca = null;
		private bool isSavedSearch = false;

        protected DocsPAWA.UserControls.AuthorOwnerFilter aofAuthor, aofOwner;

        //private void Page_Unload(object sender, System.EventArgs e)
        //{
        //    FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
        //}

        private void ricDocGrigia_PreRender(object sender, System.EventArgs e)
        {
            // Caricamento della griglia per la ricerca documenti
            if (ddl_Ric_Salvate.SelectedIndex == 0)
                GridManager.SelectedGrid = GridManager.GetStandardGridForUser(GridTypeEnumeration.Document);
            else
                GridManager.SelectedGrid = GridManager.GetGridFromSearchId(this.ddl_Ric_Salvate.SelectedValue, GridTypeEnumeration.Document);

            if (!IsPostBack)
                // Caricamento delle combo con le informazioni sull'ordinamento
                GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);


            DocsPAWA.DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
            if (fasc != null)
            {
                this.txt_CodFascicolo.Text = fasc.codice;
                this.txt_DescFascicolo.Text = fasc.descrizione;
            }

            if (DocsPAWA.popup.ricercaFascicoli.RicercaFascicoliSessionMng.IsLoaded(this))
            {
                // DocsPAWA.popup.ricercaFascicoli.RicercaFascicoliSessionMng.SetAsNotLoaded(this);
                DocsPAWA.popup.ricercaFascicoli.RicercaFascicoliSessionMng.ClearSessionData(this);
            }
        
        }

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			Utils.startUp(this);
			setFormProperties();

			schedaRicerca = (SchedaRicerca)Session[SchedaRicerca.SESSION_KEY];
			if (schedaRicerca==null)
			{
				//Inizializzazione della scheda di ricerca per la gestione delle 
				//ricerche salvate
				DocsPaWR.Utente utente =(DocsPAWA.DocsPaWR.Utente) Session["userData"];
				DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo) Session["userRuolo"];

				schedaRicerca = new SchedaRicerca(KEY_SCHEDA_RICERCA, utente,ruolo,this);
				Session[SchedaRicerca.SESSION_KEY] = schedaRicerca;
			}
			schedaRicerca.Pagina = this;

            if (!IsPostBack)
            {
                this.GetCalendarControl("txt_fineDataCreaz_C").Visible = false;
                this.GetCalendarControl("txt_fineScadenza_G").Visible = false;

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
            }

			if (!IsPostBack)
			{
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
                    isSavedSearch = PopulateField(schedaRicerca.FiltriRicerca);
                }
                else
                {
                    //Scheda Ricerca
                    schedaRicerca.ElencoRicerche("D", ddl_Ric_Salvate);
                    isSavedSearch = PopulateField(schedaRicerca.FiltriRicerca);
                }
				// Visualizzazione pagina di ricerca nella selezione 
				// di un criterio di ricerca salvato
				this.ddl_Ric_Salvate.Attributes.Add("onChange","OnChangeSavedFilter();");

				// attenzione: se vengo da un back di elemento di una ricerca salvata
				// devo comportarmi diversamente
				if (!isSavedSearch)
				{
					//set data corrente corrente al page load, ma non ap postback
					this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text=Utils.getDataOdiernaDocspa();
				}

				string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initIdDoc_C.ID + "').focus();</SCRIPT>";
				RegisterStartupScript("focus", s);
			}

			//PROFILAZIONE DINAMICA
			if(System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1") 
			{
				verificaCampiPersonalizzati();				
			}
			else
			{
				btn_CampiPersonalizzati.Visible = false;
			}
			//FINE PROFILAZIONE DINAMICA

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

			tastoInvio();
		}

		private void setFormProperties()
		{
			if(ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_OGGETTO_COMM_REF)!=null 
				&& ConfigSettings.getKey(ConfigSettings.KeysENUM.VISUALIZZA_OGGETTO_COMM_REF).Equals("1"))
			{
				this.panel_numOgg_commRef.Visible=true;
			}
			if(!Page.IsPostBack)
			{
				CaricaComboTipologiaAtto(ddl_tipoAtto);
				this.btn_selezionaParoleChiave.Attributes.Add("onclick","ApriFinestraParoleChiave('RicG');");
				this.btn_oggettario.Attributes.Add("onclick","ApriOggettario('ric_G');");
			}
			setListaParoleChiave();

            // Visibilità filtro su ricerca documenti / allegati
            this.trFiltroTipiDocumento.Visible = this.IsEnabledProfilazioneAllegato;
		}

		private void CaricaComboTipologiaAtto(DropDownList ddl)
		{
			DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
			if(System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null) 
			{
				//listaTipologiaAtto=DocumentManager.getListaTipologiaAtto(this,UserManager.getInfoUtente(this).idAmministrazione);
                listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "1");
			}
			else
			{
				listaTipologiaAtto=DocumentManager.getListaTipologiaAtto(this);			
			}
			
			//aggiunge una riga vuota alla combo
			ddl.Items.Add("");
            if (listaTipologiaAtto != null)
            {
                for (int i = 0; i < listaTipologiaAtto.Length; i++)
                {
                    ddl.Items.Add(listaTipologiaAtto[i].descrizione);
                    ddl.Items[i + 1].Value = listaTipologiaAtto[i].systemId;
                }
            }		
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }


		private void setListaParoleChiave()
		{
			//DocsPaWR.DocumentoParolaChiave[] listaParoleChiave = DocumentManager.getListaParoleChiave(this);
			DocsPaWR.DocumentoParolaChiave[] listaParoleChiave = DocumentManager.getListaParoleChiaveSel(this);
			if(listaParoleChiave == null)
				return;

			this.ListParoleChiave.Items.Clear();
			
			if(listaParoleChiave.Length > 0)
			{
				for (int i=0; i<listaParoleChiave.Length; i++ )
				{
					this.ListParoleChiave.Items.Add(((DocsPAWA.DocsPaWR.DocumentoParolaChiave)listaParoleChiave[i]).descrizione);	
					this.ListParoleChiave.Items[i].Value=((DocsPAWA.DocsPaWR.DocumentoParolaChiave)listaParoleChiave[i]).systemId;
				}
			}

			DocumentManager.removeListaParoleChiaveSel(this);
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
			this.ddl_dataCreazione_C.SelectedIndexChanged += new System.EventHandler(this.ddl_dataCreazione_C_SelectedIndexChanged);
			this.ddl_idDocumento_C.SelectedIndexChanged += new System.EventHandler(this.ddl_idDocumento_C_SelectedIndexChanged);
            this.ddl_dataScadenza_G.SelectedIndexChanged += new System.EventHandler(this.ddl_dataScadenza_SelectedIndexChanged);
			this.ddl_tipoAtto.SelectedIndexChanged += new System.EventHandler(this.ddl_tipoAtto_SelectedIndexChanged);
			this.btn_CampiPersonalizzati.Click += new System.Web.UI.ImageClickEventHandler(this.btn_CampiPersonalizzati_Click);
			this.txt_CodFascicolo.TextChanged += new System.EventHandler(this.txt_CodFascicolo_TextChanged);
			this.btn_ricerca.Click += new System.EventHandler(this.btn_ricerca_Click);
			this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
			this.mb_ConfirmDelete.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.mb_ConfirmDelete_GetMessageBoxResponse);
            this.imgFasc.Click += new System.Web.UI.ImageClickEventHandler(this.imgFasc_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.ricDocGrigia_PreRender);
            //this.Unload += new System.EventHandler(this.Page_Unload);

		}
		#endregion

		
		private bool ricercaGrigia()
		{
			try 
			{
				//int indexfVList=0;
				//array contenitore degli array filtro di ricerca
				qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
				qV[0]=new DocsPAWA.DocsPaWR.FiltroRicerca[1];
				
				fVList=new DocsPAWA.DocsPaWR.FiltroRicerca[0];

              
                #region filtro sulla tipologia documento (grigio e allegato)

                if (this.IsEnabledProfilazioneAllegato)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
                    fV1.valore = this.cblTipiDocumento.Items.FindByValue("G").Selected.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.ALLEGATO.ToString();
                    fV1.valore = this.cblTipiDocumento.Items.FindByValue("ALL").Selected.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                else
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.GRIGIO.ToString();
                    fV1.valore = "true";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #endregion
                #region filtro dataScadenza
                if (this.ddl_dataScadenza_G.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCAD_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataScadenza_G.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCAD_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataScadenza_G.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCAD_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataScadenza_G.SelectedIndex == 0)
                {
                    if (this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text != null && !this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initScadenza_G").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCADENZA_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                if (this.ddl_dataScadenza_G.SelectedIndex == 1)
                {
                    if (this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text != null && !this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initScadenza_G").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCADENZA_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Text != null && !this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Text.Equals(""))
                    {
                        if (!Utils.isDate(this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Text))
                        {
                            Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineScadenza_G").txt_Data.ID + "').focus();</SCRIPT>";
                            RegisterStartupScript("focus", s);
                            Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                            return false;
                        }

                        fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_SCADENZA_PRECEDENTE_IL.ToString();
                        fV1.valore = this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Text;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
				#endregion
                #region filtro date creazione
                if (this.ddl_dataCreazione_C.SelectedIndex == 2)
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreazione_C.SelectedIndex == 3)
                {
                    // siamo nel caso di Settimana corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreazione_C.SelectedIndex == 4)
                {
                    // siamo nel caso di Mese corrente
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString();
                    fV1.valore = "1";
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                if (this.ddl_dataCreazione_C.SelectedIndex == 0)
				{
					if (this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text!=null && !this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text.Equals(""))
					{
						if(!Utils.isDate(this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text))
						{
							Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
							string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.ID + "').focus();</SCRIPT>";
							RegisterStartupScript("focus", s);
							Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
							return false;
						}
					
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
						fV1.valore=this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
				}
                if (this.ddl_dataCreazione_C.SelectedIndex == 1)
                {
					if (this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text!=null && !this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text.Equals(""))
					{
						if(!Utils.isDate(this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text))
						{
							Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
							string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.ID + "').focus();</SCRIPT>";
							RegisterStartupScript("focus", s);
							Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
							return false;
						}
					
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
						fV1.valore=this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
					if (this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Text!=null && !this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Text.Equals(""))
					{
						if(!Utils.isDate(this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Text))
						{
							Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
							string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.ID + "').focus();</SCRIPT>";
							RegisterStartupScript("focus", s);
							Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
							return false;
						}
					
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
						fV1.valore=this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
				}
				#endregion
				#region filtro docNumber
				if(this.ddl_idDocumento_C.SelectedIndex==0)
				{
					if (this.txt_initIdDoc_C.Text!=null && !this.txt_initIdDoc_C.Text.Equals(""))
					{
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
						fV1.valore=this.txt_initIdDoc_C.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
				}
				else
				{
					if (this.txt_initIdDoc_C.Text!=null && !this.txt_initIdDoc_C.Text.Equals(""))
					{
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
						fV1.valore=this.txt_initIdDoc_C.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
					if (this.txt_fineIdDoc_C.Text!=null && !this.txt_fineIdDoc_C.Text.Equals(""))
					{
						fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
						fV1.argomento=DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
						fV1.valore=this.txt_fineIdDoc_C.Text;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
				
				
				
				}
				#endregion
				#region //filtri su mancanza immagine, assegnatario, fascicolazione
				if (this.rbl_documentiInCompletamento.SelectedIndex>=0)
				{
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					if (this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("M_Asg"))
					{
						fV1.argomento=DocsPaWR.FiltriDocumento.MANCANZA_ASSEGNAZIONE.ToString();
						fV1.valore = "0";
					}
					else if (this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("M_Img"))
					{
						fV1.argomento=DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString();
						fV1.valore = "0";
					}
					else if (this.rbl_documentiInCompletamento.SelectedItem.Value.Equals("M_Fasc"))
					{
						fV1.argomento=DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString();
						fV1.valore = "0";
					}
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				}
				#endregion
				#region filtro oggetto
				if (this.txt_oggetto.Text!=null && !this.txt_oggetto.Text.Equals(""))
				{
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriDocumento.OGGETTO.ToString();
					fV1.valore=Utils.DO_AdattaString( this.txt_oggetto.Text);
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				}
				#endregion 
				#region filtro Tipologia Documento
				if (this.ddl_tipoAtto.SelectedIndex > 0)
				{
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString();
					fV1.valore=this.ddl_tipoAtto.SelectedItem.Value;
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				}
				#endregion
				#region parole chiave

				//creo tanti filtri quante sono le parole chiave (condizione di AND)
				for(int i = 0; i<this.ListParoleChiave.Items.Count; i++)
				{
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriDocumento.PAROLE_CHIAVE.ToString();
					fV1.valore=this.ListParoleChiave.Items[i].Value;
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);

				}

				#endregion
				#region filtro firmatari
				//				if (this.txt_nomeFirma.Text!=null && !this.txt_nomeFirma.Text.Equals(""))
				//				{
				//					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
				//					fV1.argomento=DocsPaWR.FiltriDocumento.FIRMATARIO_NOME.ToString();
				//					fV1.valore=this.txt_nomeFirma.Text;
				//					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				//				}
				//				if (this.txt_cognomeFirma.Text!=null && !this.txt_cognomeFirma.Text.Equals(""))
				//				{
				//					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
				//					fV1.argomento=DocsPaWR.FiltriDocumento.FIRMATARIO_COGNOME.ToString();
				//					fV1.valore=this.txt_cognomeFirma.Text;
				//					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				//				}
				#endregion 
				#region filtro numero oggetto
				if(this.txt_numOggetto.Text!=null && !this.txt_numOggetto.Text.Equals(""))
				{
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriDocumento.NUM_OGGETTO.ToString();
					fV1.valore=this.txt_numOggetto.Text;
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				}
				#endregion
				#region filtro commissione referente
				if(this.txt_commRef.Text!=null && !this.txt_commRef.Text.Equals(""))
				{
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento=DocsPaWR.FiltriDocumento.COMMISSIONE_REF.ToString();
					fV1.valore=this.txt_commRef.Text;
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				}
				#endregion
                #region filtro note
                //                if (this.txt_note.Text != null && !this.txt_note.Text.Equals(""))
                if (this.rn_note.Testo != null && !this.rn_note.Testo.Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.NOTE.ToString();
                    string[] rf;
                    string rfsel = "0";
                    if (Session["RFNote"] != null && !string.IsNullOrEmpty(Session["RFNote"].ToString()))
                    {
                        rf = Session["RFNote"].ToString().Split('^');
                        rfsel = rf[1];
                    }


                    fV1.valore = DocsPaUtils.Functions.Functions.ReplaceApexes(this.rn_note.Testo) + "@-@" + this.rn_note.TipoRicerca + "@-@" + rfsel;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
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

                #region filtro CODICE FASCICOLO
                if (!this.txt_CodFascicolo.Text.Equals(""))
                {
                    #region costruzione condizione IN per valorizzare il filtro di ricerca IN_CHILD_RIC_ESTESA
                    //Viene ricavato il root folder(con tutti i sottofascicoli) da ogni fascicolo trovato
                    // ArrayList listaFascicoli = FascicoliManager.getFascicoliSelezionati(this);
                    ArrayList listaFascicoli = getFascicoli();
                    string inSubFolder = "IN (";
                    for (int k = 0; k < listaFascicoli.Count; k++)
                    {
                        DocsPaWR.Folder folder = FascicoliManager.getFolder(this, (DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[k]);
                        inSubFolder += folder.systemID;
                        if (folder.childs != null && folder.childs.Length > 0)
                        {
                            for (int i = 0; i < folder.childs.Length; i++)
                            {
                                inSubFolder += ", " + folder.childs[i].systemID;
                                inSubFolder = getInStringChild(folder.childs[i], inSubFolder);
                            }
                        }
                        inSubFolder += ",";
                    }
                    inSubFolder = inSubFolder.Substring(0, inSubFolder.Length - 1) + ")";

                    #endregion

                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.IN_CHILD_RIC_ESTESA.ToString();
                    fV1.valore = inSubFolder;
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion 
				#region filtro PROFILAZIONE_DINAMICA
				//fV1 = (DocsPAWA.DocsPaWR.FiltroRicerca) Session["filtroProfilazioneDinamica"];
				fV1 = schedaRicerca.GetFiltro(DocsPAWA.DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString());
				if(fV1 != null)
				{
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);				
				}
				#endregion filtro PROFILAZIONE_DINAMICA
				#region filtro DIAGRAMMI DI STATO
				if(ddl_statiDoc.Visible && ddl_statiDoc.SelectedIndex != 0)
				{
					fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
					fV1.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.DIAGRAMMA_STATO_DOC.ToString();
					string	cond = " AND (DPA_DIAGRAMMI.DOC_NUMBER = A.DOCNUMBER AND DPA_DIAGRAMMI.ID_STATO = "+ddl_statiDoc.SelectedValue+") ";
					fV1.valore = cond;
					fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				}
				#endregion filtro DIAGRAMMI DI STATO
                #region filtro RICERCA IN AREA LAVORO
                if ((Request.QueryString["ricADL"] != null) && (Request.QueryString["ricADL"] == "1"))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.DOC_IN_ADL.ToString();
                    fV1.valore = UserManager.getInfoUtente(this).idPeople.ToString() + "@" + UserManager.getRuolo(this).systemId.ToString();
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                //ABBATANGELI GIANLUIGI - Filtro per nascondere doc di altre applicazioni
                if (System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"] != null && !System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"].Equals(""))
                {
                    fV1 = new DocsPAWA.DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.COD_EXT_APP.ToString();
                    fV1.valore = (System.Configuration.ConfigurationManager.AppSettings["FILTRO_APPLICAZIONE"]);
                    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                }

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

        /// <summary>
        /// pop up per la selezione dei fascicoli
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgFasc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
            if (fasc != null)
            {
                if (this.txt_CodFascicolo.Text != "" && this.txt_DescFascicolo.Text != "")
                {
                    if (fasc.tipo.Equals("G"))
                    {
                        Session.Add("FascSelezFascRap", fasc);
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + fasc.codice + "', 'N')</script>");
                    }
                    else
                    {
                        ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fasc.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "', 'N')</script>");

                    }
                }

            }
            else
            {
                if (this.txt_CodFascicolo.Text != "")
                {
                    DocsPaWR.Fascicolo[] listaFasc = getFascicolo(null);

                    if (listaFasc != null)
                    {
                        Session.Add("listaFascFascRapida", listaFasc);

                        switch (listaFasc.Length)//il codice corrisponde a un solo fascicolo
                        {
                            case 0:
                                {
                                    RegisterStartupScript("AlertNoFasc", "<script>alert('Attenzione, codice fascicolo non presente');</script>");
                                    this.txt_DescFascicolo.Text = "";
                                    this.txt_CodFascicolo.Text = "";
                                }
                                break;
                            case 1:
                                {
                                    if (listaFasc[0].tipo.Equals("G"))
                                    {
                                        Session.Add("FascSelezFascRap", listaFasc[0]);
                                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + listaFasc[0].codice + "', 'N')</script>");
                                    }
                                    else
                                    {
                                        ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "', 'N')</script>");

                                    }
                                }
                                break;
                            default:
                                {
                                    if (listaFasc[0].tipo.Equals("G"))
                                    {
                                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + listaFasc[0].codice + "', 'Y')</script>");
                                    }
                                    else
                                    {
                                        ///se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                        DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "', 'Y')</script>");

                                    }
                                }
                                break;
                        }
                    }

                }
                else
                {
                    if (!(Session["validCodeFasc"] != null && Session["validCodeFasc"].ToString() == "false"))
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + txt_CodFascicolo.Text + "', 'N')</script>");
                }
                //RegisterStartupScript("openModale","<script>ApriRicercaFascicoli('"+txt_CodFascicolo.Text+"', 'N')</script>");
            }
        }



		/// <summary>
		/// Verifica se il dato passato è numerico
		/// </summary>
		/// <param name="strNumber"></param>
		/// <returns></returns>
		public bool IsNumber(string strNumber)
		{
			Regex objNotNumberPattern=new Regex("[^0-9.-]");
			Regex objTwoDotPattern=new Regex("[0-9]*[.][0-9]*[.][0-9]*");
			Regex objTwoMinusPattern=new Regex("[0-9]*[-][0-9]*[-][0-9]*");
			String strValidRealPattern="^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
			String strValidIntegerPattern="^([-]|[0-9])[0-9]*$";
			Regex objNumberPattern =new Regex("(" + strValidRealPattern +")|(" + strValidIntegerPattern + ")");

			return !objNotNumberPattern.IsMatch(strNumber) &&
				!objTwoDotPattern.IsMatch(strNumber) &&
				!objTwoMinusPattern.IsMatch(strNumber) &&
				objNumberPattern.IsMatch(strNumber);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_ricerca_Click(object sender, EventArgs e)
		{
			try
			{
                this.SetPageOnCurrentContext();

                // Salvataggio dei filtri per la ricerca proprietario e creatore
                this.aofOwner.SaveFilters();
                this.aofAuthor.SaveFilters();

				ddl_Ric_Salvate.SelectedIndex = 0;

				// controllo dei campi NUM. PROTOCOLLO numerici				
				if(txt_initIdDoc_C.Text!="")
				{
					if(IsNumber(txt_initIdDoc_C.Text)==false)
					{
						Response.Write("<script>alert('Il numero di ID documento deve essere numerico!');</SCRIPT>");
						string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initIdDoc_C.ID + "').focus();</SCRIPT>";
						RegisterStartupScript("focus", s);
						Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
						return;
					}
				}
				if(txt_fineIdDoc_C.Text!="")
				{
					if(IsNumber(txt_fineIdDoc_C.Text)==false)
					{
						Response.Write("<script>alert('Il numero di ID documento deve essere numerico!');</script>");
						string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_fineIdDoc_C.ID + "').focus();</SCRIPT>";
						RegisterStartupScript("focus", s);
						Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
						return;
					}
				}

				//Controllo intervallo date
                if (this.ddl_dataCreazione_C.SelectedIndex == 1)
                {
                    if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text, this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date di Data Creazione!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
				//Fine controllo intervallo date

                //Controllo intervallo data scadenza
                if (this.ddl_dataScadenza_G.SelectedIndex == 1)
                {
                    if (Utils.verificaIntervalloDate(this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text, this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date di Data Scadenza!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_initScadenza_G").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);
                        Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
                //Fine controllo intervallo data scadenza

                // Controllo lunghezza oggetto inserito
                if (this.txt_oggetto.Text.Trim() != string.Empty && !FullTextSearch.Configurations.CheckTextMinLenght(this.txt_oggetto.Text))
                {
                    string message = string.Format("<script>alert('Per ricercare un oggetto è necessario immettere almeno {0} caratteri');</script>", FullTextSearch.Configurations.FullTextMinTextLenght.ToString());
                    Response.Write(message);
                    this.txt_oggetto.Focus();
                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                    return;
                }

				if (ricercaGrigia())
				{
                    // E' obbligatorio impostare almeno un filtro di ricerca sul tipo documento
                    // (grigio, allegato o entrambi).
					if ((this.IsEnabledProfilazioneAllegato && this.cblTipiDocumento.SelectedIndex == -1) || qV[0] == null || qV[0].Length <=1)
					{
						Response.Write("<script>alert('Inserire almeno un criterio di ricerca');</script>");
						string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initIdDoc_C.ID + "').focus();</SCRIPT>";
						RegisterStartupScript("focus", s);
						Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
						return;
					}
					else
					{
						//if (!Page.IsStartupScriptRegistered("wait"))
					//	{
					//		Page.RegisterStartupScript("wait","<script>DocsPa_FuncJS_WaitWindows();</script>");
					//	}
					}

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
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initIdDoc_C.ID + "').focus();</SCRIPT>";
                                //    RegisterStartupScript("focus", s);
                                //    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                //    return;
                                //}
                                if (qV[0][i].valore.StartsWith("%"))
                                {
                                    Response.Write("<script>alert('Il parametro di ricerca non può iniziare con il carattere %');</script>");
                                    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initIdDoc_C.ID + "').focus();</SCRIPT>";
                                    RegisterStartupScript("focus", s);
                                    Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                                    return;
                                }
                                //if (qV[0][i].valore.Contains("%&&") || qV[0][i].valore.Contains("&&%"))
                                //{
                                //    Response.Write("<script>alert('La combinazione degli operatori && e % non è supportata');</script>");
                                //    string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_initIdDoc_C.ID + "').focus();</SCRIPT>";
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
					DocumentManager.removeListaNonDocProt(this);
					Session.Remove("listInArea");
				
					//Reload del frame centrale 
					//	Response.Write("<script>parent.parent.iFrame_dx.document.location = 'tabRisultatiRicDocGrigia.aspx';</script>");	
					//Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDocGrigia.aspx';</script>");	
                    if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDocGrigia.aspx?ricADL=1&from=Grigia';</script>");
                    else
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDocGrigia.aspx';</script>");
                    
					//	
			
					
				}
			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}

        private void ddl_dataScadenza_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (this.ddl_dataScadenza_G.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initScadenza_G").Visible = true;
                    this.GetCalendarControl("txt_initScadenza_G").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initScadenza_G").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initScadenza_G").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initScadenza_G").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineScadenza_G").Visible = false;
                    this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Visible = false;
                    this.lbl_a_dtaScadenza_G.Visible = false;
                    this.lbl_da_dtaScadenza_G.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initScadenza_G").Visible = true;
                    this.GetCalendarControl("txt_initScadenza_G").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initScadenza_G").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initScadenza_G").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initScadenza_G").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineScadenza_G").Visible = true;
                    this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Enabled = true;
                    this.lbl_a_dtaScadenza_G.Visible = true;
                    this.lbl_da_dtaScadenza_G.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initScadenza_G").Visible = true;
                    this.GetCalendarControl("txt_initScadenza_G").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initScadenza_G").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initScadenza_G").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initScadenza_G").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineScadenza_G").Visible = false;
                    this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Visible = false;
                    this.lbl_a_dtaScadenza_G.Visible = false;
                    this.lbl_da_dtaScadenza_G.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initScadenza_G").Visible = true;
                    this.GetCalendarControl("txt_initScadenza_G").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initScadenza_G").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineScadenza_G").Visible = true;
                    this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Enabled = false;
                    this.lbl_a_dtaScadenza_G.Visible = true;
                    this.lbl_da_dtaScadenza_G.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initScadenza_G").Visible = true;
                    this.GetCalendarControl("txt_initScadenza_G").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initScadenza_G").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineScadenza_G").Visible = true;
                    this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Enabled = false;
                    this.lbl_a_dtaScadenza_G.Visible = true;
                    this.lbl_da_dtaScadenza_G.Visible = true;
                    break;
            }
        }

		private void ddl_dataCreazione_C_SelectedIndexChanged(object sender, System.EventArgs e)
		{
            switch (this.ddl_dataCreazione_C.SelectedIndex)
            {
                case 0:
                    this.GetCalendarControl("txt_initDataCreaz_C").Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataCreaz_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Visible = false;
                    this.lblAdataCreaz_C.Visible = false;
                    this.lblDAdataCreaz_C.Visible = false;
                    break;

                case 1:
                    this.GetCalendarControl("txt_initDataCreaz_C").Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Enabled = true;
                    this.GetCalendarControl("txt_fineDataCreaz_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Enabled = true;
                    this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Enabled = true;
                    this.lblAdataCreaz_C.Visible = true;
                    this.lblDAdataCreaz_C.Visible = true;
                    break;

                case 2:
                    this.GetCalendarControl("txt_initDataCreaz_C").Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                    this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreaz_C").Visible = false;
                    this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Visible = false;
                    this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Visible = false;
                    this.lblAdataCreaz_C.Visible = false;
                    this.lblDAdataCreaz_C.Visible = false;
                    break;

                case 3:
                    this.GetCalendarControl("txt_initDataCreaz_C").Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                    this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreaz_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                    this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Enabled = false;
                    this.lblAdataCreaz_C.Visible = true;
                    this.lblDAdataCreaz_C.Visible = true;
                    break;

                case 4:
                    this.GetCalendarControl("txt_initDataCreaz_C").Visible = true;
                    this.GetCalendarControl("txt_initDataCreaz_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                    this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreaz_C").Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Enabled = false;
                    this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                    this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Visible = true;
                    this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Enabled = false;
                    this.lblAdataCreaz_C.Visible = true;
                    this.lblDAdataCreaz_C.Visible = true;
                    break;
            }
		}

		private void ddl_idDocumento_C_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			txt_fineIdDoc_C.Text="";
			if(this.ddl_idDocumento_C.SelectedIndex==0)
			{
				this.txt_fineIdDoc_C.Visible=false;
				this.lblAidDoc_C.Visible=false;
				this.lblDAidDoc_C.Visible=false;
					
			}
			else
			{
				this.txt_fineIdDoc_C.Visible=true;
				this.lblAidDoc_C.Visible=true;
				this.lblDAidDoc_C.Visible=true;
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

		#region gestione Fascicolo

        private void txt_CodFascicolo_TextChanged(object sender, System.EventArgs e)
        {
            Session["validCodeFasc"] = "true";
            //inizialmente svuoto il campo e pulisco la sessione
            FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
            this.txt_DescFascicolo.Text = "";
            //
            if (this.txt_CodFascicolo.Text.Equals(""))
            {
                txt_DescFascicolo.Text = "";
                return;
            }

            DocsPaWR.Fascicolo[] listaFasc = getFascicolo(null);

            if (listaFasc != null)
            {
                if (listaFasc.Length > 0)
                {
                    //caso 1: al codice digitato corrisponde un solo fascicolo
                    if (listaFasc.Length == 1)
                    {
                        txt_DescFascicolo.Text = listaFasc[0].descrizione;
                        //txt_CodFascicolo.Text = listaFasc[0].codice;
                        //metto il fascicolo in sessione
                        if (listaFasc[0].tipo.Equals("G"))
                        {
                            codClassifica = listaFasc[0].codice;
                        }
                        else
                        {
                            //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                            DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                            string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                            codClassifica = codiceGerarchia;
                        }
                        FascicoliManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                        //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                        //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'N');");
                    }
                    else
                    {
                        //Hashtable hashRegistriNodi = getRegistriNodi(listaFasc);
                        //caso 2: al codice digitato corrispondono piu fascicoli
                        Session.Add("listaFascFascRapida", listaFasc);
                        codClassifica = this.txt_CodFascicolo.Text;
                        if (listaFasc[0].tipo.Equals("G"))
                        {
                            codClassifica = codClassifica;
                        }
                        else
                        {
                            //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                            DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                            string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                            codClassifica = codiceGerarchia;
                        }
                        //Page.RegisterStartupScript("openListaFasc","<SCRIPT>ApriSceltaFascicolo();</SCRIPT>");
                        //Session.Add("hasRegistriNodi",hasRegistriNodi);

                        //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                        //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', 'Y');");
                        RegisterStartupScript("openModale", "<script>ApriRicercaFascicoli('" + codClassifica + "', 'Y')</script>");

                        return;
                    }
                }
                else
                {
                    //caso 0: al codice digitato non corrisponde alcun fascicolo
                    if (listaFasc.Length == 0)
                    {
                        Session["validCodeFasc"] = "false";
                        Page.RegisterStartupScript("", "<script>alert('Attenzione, codice fascicolo non presente')</script>");
                        this.txt_DescFascicolo.Text = "";
                        this.txt_CodFascicolo.Text = "";
                    }
                    //IL SECONDO PARAMETRO INDICA SE IL NODO è PRESENTE SU PIU REGISTRI
                    //this.imgFasc.Attributes.Add("onclick","ApriRicercaFascicoli('"+codClassifica+"', '');");
                }
            }
        }


		/// <summary>
        /// Metodo per recuperare i fascicoli dal codice digitato dall'utente
        /// </summary>
        /// <returns></returns>
        private ArrayList getFascicoli()
        {
            ArrayList listaFascicoli = new ArrayList();

            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                string codiceFascicolo = txt_CodFascicolo.Text;
                listaFascicoli = FascicoliManager.getFascicoloDaCodice3(this, codiceFascicolo);
                FascicoliManager.setFascicoliSelezionati(this, listaFascicoli);
            }
            if (listaFascicoli.Count != 0)
            {
                //txt_DescFascicolo.Text = "";
                return listaFascicoli;
            }
            else
            {
                txt_DescFascicolo.Text = "";
                return null;
            }
        }

        /// <summary>
        /// Metodo per il recupero del fascicolo da codice
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo[] getFascicolo(DocsPAWA.DocsPaWR.Registro registro)
        {
            DocsPaWR.Fascicolo[] listaFasc = null;
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                string codiceFascicolo = txt_CodFascicolo.Text;
                listaFasc = FascicoliManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "R");
            }
            if (listaFasc != null)
            {
                return listaFasc;
            }
            else
            {
                return null;
            }
        }

		//Metodo per la costruzione ricorsiva della condizione IN
		private string getInStringChild(DocsPAWA.DocsPaWR.Folder folder, string inSubFolder)
		{
			if(folder.childs != null && folder.childs.Length > 0) 
			{
				for (int i=0; i < folder.childs.Length; i++) 
				{
					inSubFolder += ", " + folder.childs[i].systemID;
					inSubFolder= getInStringChild(folder.childs[i],inSubFolder);
				}
			}
			return inSubFolder;
		}
		#endregion

		#region ProfilazioneDinamica
		private void verificaCampiPersonalizzati()
		{
			DocsPaWR.Templates template = new DocsPAWA.DocsPaWR.Templates();
            if(!ddl_tipoAtto.SelectedValue.Equals(""))
			{
				template = (DocsPAWA.DocsPaWR.Templates) Session["templateRicerca"];
    			if(Session["templateRicerca"] == null)
				{
					template = ProfilazioneDocManager.getTemplatePerRicerca( (UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoAtto.SelectedItem.Text,this);				
					Session.Add("templateRicerca",template);				
				}
				if( template != null && !(ddl_tipoAtto.SelectedItem.Text.ToUpper()).Equals(template.DESCRIZIONE.ToUpper()))
				{
					template = ProfilazioneDocManager.getTemplatePerRicerca( (UserManager.getInfoUtente(this)).idAmministrazione, ddl_tipoAtto.SelectedItem.Text,this);				
					Session.Add("templateRicerca",template);								
				}
			}
			if(template != null && template.SYSTEM_ID == 0)
			{
				btn_CampiPersonalizzati.Visible = false;
			}
			else
			{
				if(template != null && template.ELENCO_OGGETTI.Length != 0)
				{
					btn_CampiPersonalizzati.Visible = true;				
				}
				else
				{
					btn_CampiPersonalizzati.Visible = false;	
				}
			}
		}

		private void attivaProfilazioneDinamica()
		{
			//PROFILAZIONE DINAMICA
			if(System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1" && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null)
			{ 
				//DIAGRAMMI DI STATO
				if(System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
				{
					//Verifico se esiste un diagramma di stato associato al tipo di documento
                    DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoDoc(ddl_tipoAtto.SelectedValue, (UserManager.getInfoUtente(this)).idAmministrazione,this);
					Session.Add("DiagrammaSelezionato",dg);
					if(ddl_tipoAtto.SelectedValue != "" && dg != null)
					{
						Panel_StatiDocumento.Visible = true;

						//Inizializzazione comboBox
						ddl_statiDoc.Items.Clear();
						ListItem itemEmpty = new ListItem();
						ddl_statiDoc.Items.Add(itemEmpty);
						for(int i=0; i<dg.STATI.Length; i++)
						{
							DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato) dg.STATI[i];
							ListItem item = new ListItem(st.DESCRIZIONE,Convert.ToString(st.SYSTEM_ID));
							ddl_statiDoc.Items.Add(item);
						}
					}
					else
					{
						Panel_StatiDocumento.Visible = false;
					}
				}
				//FINE DIAGRAMMI STATO
			}
			//FINE PROFILAZIONE DINAMICA
		}

		private void ddl_tipoAtto_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Session.Remove("templateRicerca");
//			Session.Remove("filtroProfilazioneDinamica");
			schedaRicerca.RimuoviFiltro(DocsPAWA.DocsPaWR.FiltriDocumento.PROFILAZIONE_DINAMICA.ToString());

			attivaProfilazioneDinamica();
		}

		#endregion ProfilazioneDinamica

		private void btn_CampiPersonalizzati_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			ricercaGrigia();
			schedaRicerca.FiltriRicerca = qV;
			RegisterStartupScript("Apri","<script>apriPopupAnteprima();</script>");
		}

		private void tastoInvio()
		{
			Utils.DefaultButton(this,ref this.GetCalendarControl("txt_initDataCreaz_C").txt_Data,ref btn_ricerca);
			Utils.DefaultButton(this,ref this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data,ref btn_ricerca);
			Utils.DefaultButton(this,ref txt_initIdDoc_C,ref btn_ricerca);
			Utils.DefaultButton(this,ref txt_fineIdDoc_C,ref btn_ricerca);
			Utils.DefaultButton(this,ref txt_oggetto,ref btn_ricerca);
			Utils.DefaultButton(this,ref txt_numOggetto,ref btn_ricerca);
			Utils.DefaultButton(this,ref txt_commRef,ref btn_ricerca);
            TextBox note = rn_note.getTextBox();
            Utils.DefaultButton(this, ref note, ref btn_ricerca);
						
		}

		private void ddl_Ric_Salvate_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (ddl_Ric_Salvate.SelectedIndex==0)
				return;
			try
			{
                // Caricamento della griglia relativa alla ricerca salvata
                GridManager.SelectedGrid = GridManager.GetGridFromSearchId(this.ddl_Ric_Salvate.SelectedValue, GridTypeEnumeration.Document);
                // Compilazione delle combo con le informazioni sull'ordinamento
                GridManager.CompileDdlOrderAndSetOrderFilterDocuments(GridManager.SelectedGrid, this.ddlOrder, this.ddlOrderDirection);
                string gridTempId = string.Empty;
                schedaRicerca.Seleziona(Int32.Parse(ddl_Ric_Salvate.SelectedValue), out gridTempId);
				qV = schedaRicerca.FiltriRicerca;
				try
				{
					if (ddl_Ric_Salvate.SelectedIndex > 0)
					{
						Session.Add("itemUsedSearch",ddl_Ric_Salvate.SelectedIndex.ToString());
					}

					if (PopulateField(qV))
					{
						DocumentManager.setFiltroRicDoc(this,qV);
						DocumentManager.removeDatagridDocumento(this);
						DocumentManager.removeListaDocProt(this);
						//Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDocGrigia.aspx';</script>");	
                        if (Request.QueryString["ricADL"] != string.Empty && Request.QueryString["ricADL"] != null)
                            Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDocGrigia.aspx?ricADL=1&from=grigia';</script>");
                        else
                            Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDocGrigia.aspx';</script>");
					}
				}
				catch (Exception ex_)
				{
					string msg = ex_.Message;
					msg = msg + " Rimuovere i criteri di ricerca selezionati.";
					msg = msg.Replace("\"","\\\"");
					Response.Write("<script>alert(\""+msg+"\");window.location.href = window.location.href;</script>");
				}
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
			if (ricercaGrigia())
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

		private bool PopulateField(DocsPAWA.DocsPaWR.FiltroRicerca[][] qV)
		{
			try
			{	
				if (qV==null || qV.Length==0)
					return false;
				
				#region pulizia campi
				#region DOCNUMBER
				ddl_idDocumento_C.SelectedIndex = 0;
				ddl_idDocumento_C_SelectedIndexChanged(null,new System.EventArgs());
				txt_initIdDoc_C.Text = "";
				#endregion DOCNUMBER
                #region DATA_SCADENZA
                ddl_dataScadenza_G.SelectedIndex = 0;
                ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
                this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text = "";
                #endregion DATA_SCADENZA
				#region DATA_CREAZIONE
				ddl_dataCreazione_C.SelectedIndex = 0;
				ddl_dataCreazione_C_SelectedIndexChanged(null,new System.EventArgs());
				this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text = "";
				#endregion DATA_CREAZIONE
				#region MANCANZA_ASSEGNAZIONE
				rbl_documentiInCompletamento.Items[0].Selected = false;
				#endregion MANCANZA_ASSEGNAZIONE
				#region MANCANZA_IMMAGINE
				rbl_documentiInCompletamento.Items[1].Selected = false;
				#endregion MANCANZA_IMMAGINE
				#region MANCANZA_FASCICOLAZIONE
				rbl_documentiInCompletamento.Items[2].Selected = false;
				#endregion MANCANZA_FASCICOLAZIONE
				#region OGGETTO
				txt_oggetto.Text = "";
				#endregion OGGETTO
				#region PAROLE_CHIAVE
				ListParoleChiave.Items.Clear();
				#endregion PAROLE_CHIAVE
				#region NUM_OGGETTO
				txt_numOggetto.Text = "";
				#endregion NUM_OGGETTO
				#region COMMISSIONE_REF
				txt_commRef.Text = "";
				#endregion COMMISSIONE_REF
				#region TIPO_ATTO
				ddl_tipoAtto.SelectedIndex = 0;
				#endregion TIPO_ATTO
				#region NOTE
				rn_note.Testo = "";
				#endregion NOTE
                /*
				#region DIAGRAMMA_STATO_DOC
                ddl_statiDoc.SelectedIndex = 0;
				#endregion DIAGRAMMA_STATO_DOC
                */
				#region FASCICOLO
				txt_CodFascicolo.Text = "";
				txt_DescFascicolo.Text = "";
				FascicoliManager.setFascicoliSelezionati(this,null);
				#endregion FASCICOLO
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
                        #region TIPOLOGIA DOCUMENTO

                        if (this.IsEnabledProfilazioneAllegato)
                        {
                            if (aux.argomento.Equals(DocsPaWR.FiltriDocumento.GRIGIO.ToString()))
                                this.cblTipiDocumento.Items.FindByValue("G").Selected = Convert.ToBoolean(aux.valore);
                            else if (aux.argomento.Equals(DocsPaWR.FiltriDocumento.ALLEGATO.ToString()))
                                this.cblTipiDocumento.Items.FindByValue("ALL").Selected = Convert.ToBoolean(aux.valore);
                        }

                        #endregion
                        #region DOCNUMBER
                        if (aux.argomento==DocsPaWR.FiltriDocumento.DOCNUMBER.ToString())
						{
							if (ddl_idDocumento_C.SelectedIndex!=0)
								ddl_idDocumento_C.SelectedIndex = 0;
							ddl_idDocumento_C_SelectedIndexChanged(null,new System.EventArgs());
							txt_initIdDoc_C.Text = aux.valore;
						}
							#endregion DOCNUMBER
						#region DOCNUMBER_DAL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString())
						{
							if (ddl_idDocumento_C.SelectedIndex!=1)
								ddl_idDocumento_C.SelectedIndex = 1;
							ddl_idDocumento_C_SelectedIndexChanged(null,new System.EventArgs());
							txt_initIdDoc_C.Text = aux.valore;
						}
							#endregion DOCNUMBER_DAL
						#region DOCNUMBER_AL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString())
						{
							if (ddl_idDocumento_C.SelectedIndex!=1)
								ddl_idDocumento_C.SelectedIndex = 1;
							ddl_idDocumento_C_SelectedIndexChanged(null,new System.EventArgs());
							txt_fineIdDoc_C.Text = aux.valore;
						}
							#endregion DOCNUMBER_AL
                        #region DATA_SCADENZA_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCADENZA_IL.ToString())
                        {
                            if (ddl_dataScadenza_G.SelectedIndex != 0)
                                ddl_dataScadenza_G.SelectedIndex = 0;
                            ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_SCADENZA_IL
                        #region DATA_SCADENZA_SUCCESSIVA_AL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCADENZA_SUCCESSIVA_AL.ToString())
                        {
                            if (ddl_dataScadenza_G.SelectedIndex != 1)
                                ddl_dataScadenza_G.SelectedIndex = 1;
                            ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_SCADENZA_SUCCESSIVA_AL
                        #region DATA_SCADENZA_PRECEDENTE_IL
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCADENZA_PRECEDENTE_IL.ToString())
                        {
                            if (ddl_dataScadenza_G.SelectedIndex != 1)
                                ddl_dataScadenza_G.SelectedIndex = 1;
                            ddl_dataScadenza_SelectedIndexChanged(null, new System.EventArgs());
                            this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Text = aux.valore;
                        }
                        #endregion DATA_SCADENZA_PRECEDENTE_IL
                        #region DATA_SCAD_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCAD_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataScadenza_G.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initScadenza_G").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initScadenza_G").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Enabled = false;
                            this.lbl_a_dtaScadenza_G.Visible = true;
                            this.lbl_da_dtaScadenza_G.Visible = true;
                        }
                        #endregion
                        #region DATA_SCAD_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCAD_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataScadenza_G.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initScadenza_G").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initScadenza_G").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Enabled = false;
                            this.lbl_a_dtaScadenza_G.Visible = true;
                            this.lbl_da_dtaScadenza_G.Visible = true;
                        }
                        #endregion
                        #region DATA_SCAD_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_SCAD_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataScadenza_G.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initScadenza_G").Visible = true;
                            this.GetCalendarControl("txt_initScadenza_G").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initScadenza_G").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initScadenza_G").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initScadenza_G").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initScadenza_G").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineScadenza_G").Visible = false;
                            this.GetCalendarControl("txt_fineScadenza_G").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineScadenza_G").btn_Cal.Visible = false;
                            this.lbl_a_dtaScadenza_G.Visible = false;
                            this.lbl_da_dtaScadenza_G.Visible = false;
                        }
                        #endregion
                        #region DATA_CREAZIONE_IL
                        else if (aux.argomento==DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString())
						{
							if (ddl_dataCreazione_C.SelectedIndex!=0)
								ddl_dataCreazione_C.SelectedIndex = 0;
							ddl_dataCreazione_C_SelectedIndexChanged(null,new System.EventArgs());
							this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text = aux.valore;
						}
						#endregion DATA_CREAZIONE_IL
						#region DATA_CREAZIONE_SUCCESSIVA_AL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString())
						{
							if (ddl_dataCreazione_C.SelectedIndex!=1)
								ddl_dataCreazione_C.SelectedIndex = 1;
							ddl_dataCreazione_C_SelectedIndexChanged(null,new System.EventArgs());
							this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text = aux.valore;
						}
						#endregion DATA_CREAZIONE_SUCCESSIVA_AL
						#region DATA_CREAZIONE_PRECEDENTE_IL
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString())
						{
							if (ddl_dataCreazione_C.SelectedIndex!=1)
								ddl_dataCreazione_C.SelectedIndex = 1;
							ddl_dataCreazione_C_SelectedIndexChanged(null,new System.EventArgs());
							this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Text = aux.valore;
						}
						#endregion DATA_CREAZIONE_PRECEDENTE_IL
                        #region DATA_CREAZ_SC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_SC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataCreazione_C.SelectedIndex = 3;
                            this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfWeek();
                            this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataCreaz_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfWeek();
                            this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Enabled = false;
                            this.lblAdataCreaz_C.Visible = true;
                            this.lblDAdataCreaz_C.Visible = true;
                        }
                        #endregion
                        #region DATA_CREAZ_MC
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_MC.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataCreazione_C.SelectedIndex = 4;
                            this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text = DocsPAWA.DocumentManager.getFirstDayOfMonth();
                            this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataCreaz_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Text = DocsPAWA.DocumentManager.getLastDayOfMonth();
                            this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Enabled = false;
                            this.lblAdataCreaz_C.Visible = true;
                            this.lblDAdataCreaz_C.Visible = true;
                        }
                        #endregion
                        #region DATA_CREAZ_TODAY
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.DATA_CREAZ_TODAY.ToString() && aux.valore == "1")
                        {
                            this.ddl_dataCreazione_C.SelectedIndex = 2;
                            this.GetCalendarControl("txt_initDataCreaz_C").Visible = true;
                            this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Text = DocsPAWA.DocumentManager.toDay();
                            this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Visible = true;
                            this.GetCalendarControl("txt_initDataCreaz_C").txt_Data.Enabled = false;
                            this.GetCalendarControl("txt_initDataCreaz_C").btn_Cal.Visible = true;
                            this.GetCalendarControl("txt_initDataCreaz_C").btn_Cal.Enabled = false;
                            this.GetCalendarControl("txt_fineDataCreaz_C").Visible = false;
                            this.GetCalendarControl("txt_fineDataCreaz_C").txt_Data.Visible = false;
                            this.GetCalendarControl("txt_fineDataCreaz_C").btn_Cal.Visible = false;
                            this.lblAdataCreaz_C.Visible = false;
                            this.lblDAdataCreaz_C.Visible = false;
                        }
                        #endregion
                        #region MANCANZA_ASSEGNAZIONE
						else if (aux.argomento==DocsPaWR.FiltriDocumento.MANCANZA_ASSEGNAZIONE.ToString())
						{
							rbl_documentiInCompletamento.Items[0].Selected = true;
						}
							#endregion MANCANZA_ASSEGNAZIONE
						#region MANCANZA_IMMAGINE
						else if (aux.argomento==DocsPaWR.FiltriDocumento.MANCANZA_IMMAGINE.ToString())
						{
							rbl_documentiInCompletamento.Items[1].Selected = true;
						}
							#endregion MANCANZA_IMMAGINE
						#region MANCANZA_FASCICOLAZIONE
						else if (aux.argomento==DocsPaWR.FiltriDocumento.MANCANZA_FASCICOLAZIONE.ToString())
						{
							rbl_documentiInCompletamento.Items[2].Selected = true;
						}
							#endregion MANCANZA_FASCICOLAZIONE
						#region OGGETTO
						else if (aux.argomento==DocsPaWR.FiltriDocumento.OGGETTO.ToString())
						{
							txt_oggetto.Text = aux.valore;
						}
							#endregion OGGETTO
						#region PAROLE_CHIAVE
						else if (aux.argomento==DocsPaWR.FiltriDocumento.PAROLE_CHIAVE.ToString())
						{
							DocumentoParolaChiave[] pk = DocumentManager.getListaParoleChiave(this);
							bool found = false;
							for (int i=0; !found && i<pk.Length; i++)
							{
								if (pk[i].systemId == aux.valore)
								{
									ListItem li = new ListItem(pk[i].descrizione,pk[i].systemId);
									this.ListParoleChiave.Items.Add(li);	
									found = true;
								}
							}
						}
							#endregion PAROLE_CHIAVE
						#region NUM_OGGETTO
						else if (aux.argomento==DocsPaWR.FiltriDocumento.NUM_OGGETTO.ToString())
						{
							txt_numOggetto.Text = aux.valore;
						}
							#endregion NUM_OGGETTO
						#region COMMISSIONE_REF
						else if (aux.argomento==DocsPaWR.FiltriDocumento.COMMISSIONE_REF.ToString())
						{
							txt_commRef.Text = aux.valore;
						}
							#endregion COMMISSIONE_REF
						#region TIPO_ATTO
						else if (aux.argomento==DocsPaWR.FiltriDocumento.TIPO_ATTO.ToString())
						{
							ddl_tipoAtto.SelectedValue = aux.valore;
							attivaProfilazioneDinamica();
//							ddl_tipoAtto_SelectedIndexChanged(null, new System.EventArgs());
						}								
							#endregion TIPO_ATTO
						#region DIAGRAMMA_STATO_DOC
						else if (aux.argomento==DocsPaWR.FiltriDocumento.DIAGRAMMA_STATO_DOC.ToString())
						{
                            ddl_statiDoc.Visible = true;
                            ddl_statiDoc.SelectedIndex = 0;
							string val = null;
							try
							{
								string s = "DPA_DIAGRAMMI.ID_STATO";
								s = aux.valore.Substring(aux.valore.LastIndexOf(s)).Trim();
								s = s.Substring(0,s.Length-1);
								s = s.Replace(" ","");
								char[] sep = {'='};
								string[] tks = s.Split(sep);
								val = tks[1];
							} 
							catch {}
							
							if (val!=null)
							{
								bool found = false;
								for (int i=0; !found && i<ddl_statiDoc.Items.Count; i++)
								{
									if (ddl_statiDoc.Items[i].Value==val)
									{
										ddl_statiDoc.SelectedIndex = i;
										found = true;
									}
								}
							}
						}							
							#endregion DIAGRAMMA_STATO_DOC
                        #region NOTE
                        else if (aux.argomento == DocsPaWR.FiltriDocumento.NOTE.ToString())
                        {
                            string[] infoRic = Utils.splittaStringaRicercaNote(aux.valore);
                            rn_note.Testo = infoRic[0];
                            rn_note.TipoRicerca = (infoRic[1])[0];
                        }
                        #endregion NOTE
                        //#region Creatore (User Control)
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

                        //#endregion
						#region FASCICOLO
						else if (aux.argomento==DocsPaWR.FiltriFascicolazione.IN_CHILD_RIC_ESTESA.ToString())
						{
							string val = aux.valore.Trim();
							val = val.Substring("IN".Length).Trim();
							val = val.Substring("(".Length).Trim();
							val = val.Substring(0,val.LastIndexOf(")")).Trim();
							char[] sep = {','};
							string[] ids = val.Split(sep);
							if (ids!=null && ids.Length>0)
							{
                                DocsPaWR.Folder folder = FascicoliManager.getFolder(this, ids[0].Trim());
                                DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicolo(this, folder.idFascicolo);
                                if (fasc != null)
                                {
                                    ArrayList listaFascicoli = FascicoliManager.getFascicoloDaCodice3(this, fasc.codice);
                                    if (listaFascicoli != null)
                                    {
                                        FascicoliManager.setFascicoliSelezionati(this, listaFascicoli);
                                        txt_CodFascicolo.Text = fasc.codice;
                                        if (listaFascicoli.Count == 1)
                                        {
                                            txt_DescFascicolo.Text = fasc.descrizione;
                                        }
                                        else
                                        {
                                            if (((DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[0]).descrizione == ((DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[1]).descrizione)
                                                txt_DescFascicolo.Text = ((DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[0]).descrizione;
                                        }
                                    }
/*                                    if (listaFascicoli != null && listaFascicoli.Count == 2)
                                    {
                                        if (((DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[0]).descrizione == ((DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[1]).descrizione)
                                            txt_DescFascicolo.Text = ((DocsPAWA.DocsPaWR.Fascicolo)listaFascicoli[0]).descrizione;
                                    }
                                    else
                                    {
                                        txt_DescFascicolo.Text = "";
                                    }
*/                                }
							}
						}
								#endregion FASCICOLO
					}
					catch (Exception)
					{
						throw new Exception("I criteri di ricerca non sono piu\' validi.");
					}
				}
				
				if(System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1") 
					verificaCampiPersonalizzati();
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
            SiteNavigation.CallContext currentContext = SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext != null)
                currentContext.PageNumber = 1;
        }

		#endregion

        #region Gestione ricerca Grigio / Allegato 

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

        #endregion
    }
}
