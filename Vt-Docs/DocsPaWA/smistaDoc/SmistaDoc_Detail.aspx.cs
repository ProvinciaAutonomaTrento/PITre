using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DocsPAWA.utils;



namespace DocsPAWA.smistaDoc
{
	/// <summary>
	/// Summary description for SmistaDoc_Detail.
	/// </summary>
	public class SmistaDoc_Detail : DocsPAWA.CssPage
	{

        //Andrea
        private ArrayList listaExceptionTrasmissioni = new ArrayList();
        private string messError = "";
        //End Andrea
        protected static string separatore = "----------------";

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
			this.btn_first.Click += new System.Web.UI.ImageClickEventHandler(this.btn_first_Click);
			this.btn_previous.Click += new System.Web.UI.ImageClickEventHandler(this.btn_previous_Click);
			this.btn_next.Click += new System.Web.UI.ImageClickEventHandler(this.btn_next_Click);
			this.btn_last.Click += new System.Web.UI.ImageClickEventHandler(this.btn_last_Click);
			this.btn_clearFlag.Click += new System.Web.UI.ImageClickEventHandler(this.btn_clearFlag_Click);
            this.grdUOApp.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.OnGridAppItemBounded);
            this.grdUOInf.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.OnGridInfItemBounded);
			this.chk_showDoc.CheckedChanged += new System.EventHandler(this.chk_showDoc_CheckedChanged);
			this.chk_mantieniSel.CheckedChanged += new System.EventHandler(this.chk_mantieniSel_CheckedChanged);
			this.btn_zoom.Click += new System.EventHandler(this.btn_zoom_Click);
			this.btn_rifiuta.Click += new System.EventHandler(this.btn_rifiuta_Click);
            this.btn_scarta.Click += new System.EventHandler(this.btn_scarta_Click);
			this.btn_smista.Click += new System.EventHandler(this.btn_smista_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);
            this.btn_dettFirma.Click += new System.EventHandler(this.btn_dettFirma_Click);
            this.grdUOApp.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdUOApp_ItemCommand);
            this.grdUOInf.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdUOInf_ItemCommand);
            this.MessageBox1.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.MessageBox1_GetMessageBoxResponse);
		}
		#endregion

		#region Dichiarazioni, costruttori

        protected System.Web.UI.WebControls.Panel pnlContainer;
        protected System.Web.UI.WebControls.Panel pnlContainerUoInferiori;
        protected System.Web.UI.WebControls.Panel pnlContainerUoAppartenenza;
        
        protected System.Web.UI.ScriptManager scriptManager;
        protected System.Web.UI.UpdatePanel updatePanelUOApp;
        protected System.Web.UI.UpdatePanel updatePanelUOInf;
		protected System.Web.UI.WebControls.Label lbl_dataCreazione;
		protected System.Web.UI.WebControls.Label lbl_segnatura;
		protected System.Web.UI.WebControls.Label lbl_oggetto;
        protected System.Web.UI.WebControls.Label lbl_mitt_trasm;
		protected System.Web.UI.WebControls.Label lbl_mittente;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.Label lbl_contatore;
		protected System.Web.UI.WebControls.Button btn_smista;
		protected System.Web.UI.WebControls.DataGrid grdUOApp;
		protected System.Web.UI.WebControls.DataGrid grdUOInf;
		protected System.Web.UI.WebControls.ImageButton btn_previous;
		protected System.Web.UI.WebControls.ImageButton btn_next;
		protected System.Web.UI.WebControls.ImageButton btn_last;
		protected System.Web.UI.WebControls.ImageButton btn_first;
		protected System.Web.UI.WebControls.Button btn_dettFirma;
		protected System.Web.UI.WebControls.CheckBox chk_showDoc;
		protected System.Web.UI.WebControls.Panel pnl_navigationButtons;
		protected System.Web.UI.WebControls.Button btn_rifiuta;
		protected System.Web.UI.WebControls.TextBox GetIdTrasmissioneUtente;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_msg_rifiuto;
        protected System.Web.UI.WebControls.HiddenField txtNoteInd;
        protected System.Web.UI.WebControls.HiddenField hdDescrCompetenza;
        protected System.Web.UI.WebControls.HiddenField hdDescrConoscenza;
		protected System.Web.UI.WebControls.CheckBox chk_mantieniSel;
		protected System.Web.UI.WebControls.ImageButton btn_clearFlag;
		protected System.Web.UI.WebControls.Button btn_scarta;
		protected System.Web.UI.WebControls.Button btn_zoom;
        protected System.Web.UI.WebControls.Button btn_AdL;        
		protected System.Web.UI.WebControls.Label lbl_versioni;
		protected System.Web.UI.WebControls.Label lbl_allegati;
		protected System.Web.UI.WebControls.Label lbl_destinatario;
        protected System.Web.UI.WebControls.TextBox txtNoteGen;
        protected System.Web.UI.WebControls.TextBox txtAreaNoteInd;
        protected System.Web.UI.WebControls.HiddenField hdCheckCompetenza;
        protected System.Web.UI.WebControls.HiddenField hdCheckConoscenza;
        protected System.Web.UI.WebControls.HiddenField hdTipoDestCompetenza;
        protected System.Web.UI.WebControls.HiddenField hdTipoDestConoscenza;
        protected System.Web.UI.WebControls.HiddenField hdCheckedUtenti;
        protected System.Web.UI.WebControls.HiddenField hdUOapp;
        protected System.Web.UI.WebControls.HiddenField hdIsEnabledNavigaUO;
        protected System.Web.UI.WebControls.Label lbl_descRagTrasm;
        protected System.Web.UI.WebControls.TextBox txt_CodFascicolo;
        protected System.Web.UI.WebControls.TextBox txt_DescFascicolo;
        protected System.Web.UI.WebControls.Panel pnl_fasc_rapida;
        protected string codClassifica = string.Empty;
        protected System.Web.UI.WebControls.HiddenField hdCountNavigaDown;
        protected System.Web.UI.WebControls.HiddenField hdSelSmista;
        protected System.Web.UI.WebControls.DropDownList ddl_trasm_rapida;
        protected System.Web.UI.WebControls.Panel pnl_trasm_rapida;
        protected System.Web.UI.WebControls.ImageButton btn_selezioniSmistamento;
        protected System.Web.UI.HtmlControls.HtmlTable idTable;
        protected int heightUoApp;
        protected int heightUoInf;
        protected Utilities.MessageBox MessageBox1;

        protected System.Web.UI.WebControls.Label lbl_segn_repertorio;
        protected System.Web.UI.HtmlControls.HtmlControl segnatura;
        protected System.Web.UI.HtmlControls.HtmlControl segnatura_val;
        protected System.Web.UI.HtmlControls.HtmlControl segn_repert;
        protected System.Web.UI.HtmlControls.HtmlControl segn_repert_val;
        protected System.Web.UI.WebControls.Label lblTipology;
        protected System.Web.UI.HtmlControls.HtmlTableCell trTipologyTitle, tdTipology;
		#endregion

		#region Inizializzazione dati

		private void Page_Load(object sender, System.EventArgs e)
		{         
            //se non è abilitata la visualizzazione della segnatura di repertorio, devo modificare la tabella per la visualizzazione
            if (!utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "VIS_SEGNATURA_REPERTORI").Equals("1"))
            { 
                //rendo non visibile le colonne dedicate alla segnatura di repertorio ed alla tipologia documentale
                segn_repert.Attributes["style"] = "display:none;";
                segn_repert_val.Attributes["style"] = "display:none;";

                trTipologyTitle.Visible = false;
                tdTipology.Visible = false;
            }

            // verifica se esistono le ragioni di trasmissione utili allo smistamento
			//if(this.CheckExistRagTrasm())
            DocsPaWR.RagioneTrasmissione[] listaRagioniSmistamento =null;
            
            if (this.CheckExistenzaRagioniSmistamento(out listaRagioniSmistamento))
			{
				if (!IsPostBack)
				{
                    // Gestione navigazione UO (imposta campo nascosto)
                    this.ImpostaHiddenNavigaUO();

                    //settaggio campo invisibili per la gestione della nuova chiave di web.config
                    //UT_TX_RUOLI_CHECKED, checked decide se glu utenti di un ruolo devono comparire checked di default o no.
                    setCampoNascostoGestioneCheckUtenti();
			
                    //settaggio campi radioButton invisibili per la gestione della data di scadenza
                    setTipoRagioneSmistamento(listaRagioniSmistamento);

                    // Gestione script client per il check visualizza documento
					this.RegisterClientScriptCheckVisualizza();
                   
					// Associazione dati UI
					this.FillDataDocumentoTrasmesso();

					// Visualizzione pannello pulsanti di navigazione
					this.ShowPanelNavigationButtons();

                    //Settaggio flag Visualizza Documento
                    this.SetFlagVisualizzaDocumento();
				}			

				// Gestione script client per i pulsanti di navigazione
				this.RegisterClientScriptNavigationButtons();

                // gestione fascicolaziione rapida
               // this.GestioneFascicolazione(); spostato nell prerender
			}
			else
			{
				if(!this.Page.ClientScript.IsStartupScriptRegistered("AlertModalDialog"))
				{
					string msg = "Attenzione, non esistono le ragioni di trasmissione COMPETENZA e CONOSCENZA utili allo smistamento.";
					string scriptString = "<SCRIPT>alert('" + msg + "'); window.close();</SCRIPT>";
					//this.Page.RegisterStartupScript("AlertModalDialog", scriptString);
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "AlertModalDialog", scriptString);
				}
			}

            //Nodo titolario scelto
            DocsPAWA.DocsPaWR.FascicolazioneClassificazione classificazione = DocumentManager.getClassificazioneSelezionata(this);
            if (classificazione != null)
            {
                this.txt_CodFascicolo.Text = classificazione.codice;
                //this.txt_DescFascicolo.Text = classificazione.descrizione;
                DocsPAWA.DocsPaWR.Fascicolo Fasc = getFascicolo();
                FascicoliManager.setFascicoloSelezionatoFascRapida(Fasc);
                DocumentManager.setClassificazioneSelezionata(this, null);
            }
            this.gestisciPopUpSelezioneSmistamento();
		}

        private bool cfg_Smista_Abilita_Trasm_Rapida()
        {
            string valoreChiaveDB = DocsPAWA.utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_SMISTA_ABILITA_TRASM_RAPIDA");
            // Se il valore è diverso da null oppure è "1" allora ritorna true, altrimenti torna false;
            return (valoreChiaveDB != null && valoreChiaveDB.Equals("1")) ? true : false;
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            this.GestioneFascicolazione();

            //abilito pannello per la trasmissione rapida
            this.pnl_trasm_rapida.Visible = false;
            if (cfg_Smista_Abilita_Trasm_Rapida())
            {
                this.pnl_trasm_rapida.Visible = true;
            }

            //controllo abilitazione bottonre per visualizzare le selezioni effettuate per lo smistamento
            this.btn_selezioniSmistamento.Visible = false;
            if (System.Configuration.ConfigurationManager.AppSettings["SMISTA_VISUALIZZA_SELEZIONI"] != null && System.Configuration.ConfigurationManager.AppSettings["SMISTA_VISUALIZZA_SELEZIONI"].ToString().Equals("1"))
            {
                this.btn_selezioniSmistamento.Visible = true;
            }

            smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();
            
            if (docManager.GetDocumentCount() > 0)
            {
                DocsPaWR.DocumentoSmistamento docSmistamento = docManager.GetCurrentDocument(false);
                if (docManager.IsTrasmissioneConWorkflow(docManager.GetCurrentDocumentPosition() - 1))
                {
                    this.btn_rifiuta.Enabled = true;
                    this.btn_scarta.Text = "Accetta";
                    this.btn_scarta.ToolTip = "Accetta la trasmissione, imposta il documento come VISTO e lo toglie dalla lista delle COSE DA FARE";
                }
                else
                {
                    this.btn_rifiuta.Enabled = false;
                    this.btn_scarta.Text = "Visto";
                    this.btn_scarta.ToolTip = "Imposta il documento come VISTO e lo toglie dalla lista delle COSE DA FARE";
                }

                try
                {
                    int isInAdl = DocumentManager.isDocInADL(docSmistamento.IDDocumento, this);
                    if (isInAdl == 1)
                    {
                        this.btn_AdL.Text = "ADL **";
                        this.btn_AdL.Enabled = false;
                        this.btn_AdL.ToolTip = "Documento già presente in Area di Lavoro";
                    }
                    else
                    {
                        this.btn_AdL.Text = "ADL";
                        this.btn_AdL.Enabled = true;
                        this.btn_AdL.ToolTip = "Inserisce il documento in AdL";
                    }
                }
                catch (Exception excp)
                {
                    string s = "<SCRIPT language='javascript'>alert('Errore nel reperimento dati del documento!');</SCRIPT>";
                    Page.RegisterStartupScript("focus", s);
                }
            }

            if (Session["NoteGenSmista"] != null)
            {
                this.txtNoteGen.Text = Session["NoteGenSmista"].ToString();
                Session.Remove("NoteGenSmista");
            }

            //Andrea
            if (Session["MessError"] != null)
            {
                messError = Session["MessError"].ToString();
                //Response.Write("<script language=\"javascript\">alert('Trasmissioni avvenute con successo per tutti i destinatari ad eccezione delle seguenti: \\n" + messError + "');</script>");
                Response.Write("<script language=\"javascript\">alert('Trasmissioni con esito negativo: \\n" + messError + "');</script>");
                Session.Remove("MessError");
            }
            //End Andrea
        }

        public void setCampoNascostoGestioneCheckUtenti()
        {
            if(TrasmManager.getTxRuoloUtentiChecked())
                hdCheckedUtenti.Value = "true";
            else
                hdCheckedUtenti.Value = "false";
        
        }
        public void setTipoRagioneSmistamento (DocsPaWR.RagioneTrasmissione[] listaRagSmistamento)
        {
            hdCheckCompetenza.Value = listaRagSmistamento[0].tipo.ToString(); // tipo_ragione della COMPETENZA

            hdCheckConoscenza.Value = listaRagSmistamento[1].tipo.ToString();  // tipo_ragione della CONOSCENZA

            hdTipoDestCompetenza.Value = setTipoDestRagione((DocsPaWR.TramissioneTipoGerarchia)(listaRagSmistamento[0].tipoDestinatario));

            hdTipoDestConoscenza.Value = setTipoDestRagione((DocsPaWR.TramissioneTipoGerarchia)(listaRagSmistamento[1].tipoDestinatario));

            hdDescrCompetenza.Value = listaRagSmistamento[0].descrizione.ToString(); // descrizione rgione COMPETENZA

            hdDescrConoscenza.Value = listaRagSmistamento[1].descrizione.ToString(); // descrizione ragione CONOSCENZA           
        }

        private string setTipoDestRagione(DocsPaWR.TramissioneTipoGerarchia tipoDirittoRagione)
        {
            string tipoDest = String.Empty;

            switch (tipoDirittoRagione)
            {
                case DocsPaWR.TramissioneTipoGerarchia.TUTTI:
                    tipoDest = "";
                    break;

                case DocsPaWR.TramissioneTipoGerarchia.INFERIORE:
                    tipoDest = "0";
                    break;

                case DocsPaWR.TramissioneTipoGerarchia.SUPERIORE:
                    tipoDest = "1";
                    break;

                case DocsPaWR.TramissioneTipoGerarchia.PARILIVELLO:
                    tipoDest = "2";
                    break;
            }

            return tipoDest;
        }

		public smistaDoc.SmistaDocManager GetSmistaDocManager()
		{
			string idDocumento=Request.QueryString["DOC_NUMBER"];

			// Reperimento oggetto "DocumentoTrasmesso" corrente
			return SmistaDocSessionManager.GetSmistaDocManager(idDocumento);
		}

		/// <summary>
		/// verifica l'esistenza delle ragioni di trasmissione: COMPETENZA e CONOSCENZA
		/// </summary>
		/// <returns>TRUE: esistono le ragioni di trasmissione per lo smistamento; FALSE: non esistono</returns>
		private bool CheckExistRagTrasm()
		{		
			bool retValue = false;	
			if(DocsPAWA.smistaDoc.SmistaDocSessionManager.ExistSessionRagTrasm())
			{
				retValue = true;
			}
			else
			{						
				DocsPAWA.smistaDoc.SmistaDocManager docManager = new SmistaDocManager();
				retValue = docManager.CheckExistRagTrasm(UserManager.getInfoUtente(this));	
				if(retValue) 
					DocsPAWA.smistaDoc.SmistaDocSessionManager.SetSessionRagTrasm();
			}
			return retValue;
		}

        private bool CheckExistenzaRagioniSmistamento(out DocsPaWR.RagioneTrasmissione[] listaRagioniSmistamento)
        {
            listaRagioniSmistamento = null;
            bool retValue = false;
            if (DocsPAWA.smistaDoc.SmistaDocSessionManager.ExistSessionRagTrasm())
            {
                retValue = true;
            }
            else
            {
                DocsPAWA.smistaDoc.SmistaDocManager docManager = new SmistaDocManager();
                listaRagioniSmistamento = docManager.GetListaRagioniSmistamento(UserManager.getInfoUtente(this));
                if (listaRagioniSmistamento != null && listaRagioniSmistamento.Length == 2)
                {
                    retValue = true;
                }
                if (retValue)
                    DocsPAWA.smistaDoc.SmistaDocSessionManager.SetSessionRagTrasm();
            }
            return retValue;
        }


		#endregion

		#region Associazione dati alla UI

		private void FillDataDocumentoTrasmesso()
		{			
			smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();

			if(docManager.GetDocumentCount()>0)
			{
				DocsPaWR.DocumentoSmistamento docSmistamento=docManager.GetCurrentDocument(false);

                //se abilitata la segnatura di repertorio, inizializzo il il campo
                if (utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "VIS_SEGNATURA_REPERTORI").Equals("1"))
                {
                    if (!string.IsNullOrEmpty(docSmistamento.DocNumber))
                    {
                        lbl_segn_repertorio.Text = DocumentManager.getSegnaturaRepertorio(this.Page, docSmistamento.DocNumber, UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice);

                        // Recupero dell'eventuale tipologia associata al documento / fascicolo
                        this.lblTipology.Text = docSmistamento.TipologyDescription;
                    }
                }

				lbl_dataCreazione.Text	= docSmistamento.DataCreazione;
                
				if(docSmistamento.Segnatura==null || docSmistamento.Segnatura.Equals(string.Empty))
				{
					lbl_segnatura.CssClass = "testo_grigio_scuro";
					lbl_segnatura.Text	= docSmistamento.IDDocumento;
				}
				else
				{
					lbl_segnatura.CssClass = "testo_red";
					lbl_segnatura.Text	= docSmistamento.Segnatura;
				}

                if (docSmistamento.Oggetto.Length > 60)
                {
                    lbl_oggetto.Text = docSmistamento.Oggetto.Substring(0, 60) + "...";
                    lbl_oggetto.ToolTip = docSmistamento.Oggetto;
                }
                else
                {
				    lbl_oggetto.Text = docSmistamento.Oggetto;
                    lbl_oggetto.ToolTip = string.Empty;
				}

                if(docSmistamento.MittenteDocumento!=null && docSmistamento.MittenteDocumento!=string.Empty && docSmistamento.MittenteDocumento!="")
					lbl_mittente.Text	= docSmistamento.MittenteDocumento;
				else
					lbl_mittente.Text	= "---";

                int docIndex = docManager.GetCurrentDocumentPosition();
                string mittTrasm =  docManager.getMittenteTrasmissione(docIndex - 1);
                this.lbl_mitt_trasm.Text = mittTrasm;
                
				string listaDest = this.GetDestinatari(docSmistamento);
				if(listaDest!=null && listaDest!="")
				{
                    if (listaDest.Length > 60)
                    {
                        lbl_destinatario.Text = listaDest.Substring(0, 60) + "...";
                        lbl_destinatario.ToolTip = listaDest;
                    }
                    else
                        lbl_destinatario.Text = listaDest;
				}
				else
				{
					lbl_destinatario.Text = "---";
				}

				this.lbl_versioni.Text = docSmistamento.Versioni;
				this.lbl_allegati.Text = docSmistamento.Allegati;
                this.lbl_descRagTrasm.Text = docManager.GetDescRagioneTrasm(docManager.GetCurrentDocumentPosition() - 1);
               
				docSmistamento=null;
                                
                int config=0;
                if (cfg_Smista_Abilita_Trasm_Rapida())
                    config = 1;

                if (config == 1)
                {
                    bool mantieniSelezione = this.chk_mantieniSel.Checked;

                    string idModello = string.Empty;

                    if (mantieniSelezione)
                        idModello = this.ddl_trasm_rapida.SelectedValue;

                    // Caricamento modelli trasmissione rapida per il documento
                    this.caricaModelliTrasmRapida();

                    if (mantieniSelezione)
                        // Ripristino modello trasmissione rapida (se presente)
                        this.ddl_trasm_rapida.SelectedValue = idModello;
                }
                

                // Rimozione contenuto documento firmato dalla sessione
                this.SetSignedDocumentOnSession(null);

				// Caricamento griglie destinatari
				this.FillGridDestinatari();

				// Aggiornamento indice documento corrente
				this.RefreshDocumentCounter();			

				// Gestione abilitazione / disabilitazione pulsanti di navigazione
				this.EnabledNavigationButtons();

				// Gestione abilitazione / disabilitazione pulsante dettagli firma
				this.EnableButtonDettagliFirma();
		
				// Gestione del mantenimento dei flag selezionati e dei tasti di default
				this.ChkMantieniSelezione();

                //Viene autopopolato il campo relativo alle note generali
                this.txtNoteGen.Text = docManager.GetNoteGenerali(docManager.GetCurrentDocumentPosition() - 1);

                //Viene autopopolato il campo relativo alle note individuali e reso non editabile
                this.txtAreaNoteInd.Text = docManager.GetNoteIndividuali(docManager.GetCurrentDocumentPosition() - 1);

                // Visualizzazione file
				this.ShowDocumentFile(false);			

				// pulsanti di default
				this.EnableButtonsDefault();

				// pulsante rifiuta
				this.EnableButtonRifiuta();

                //registro il focus sul campo relativo alle note generali della trasmissione
                SetFocus(this.txtNoteGen);
			}
			else
			{
				if (!this.Page.IsStartupScriptRegistered("AlertModalDialog"))
				{
					string msg = "Nessun documento da smistare.";
					string scriptString = "<SCRIPT>alert('" + msg + "'); window.close();</SCRIPT>";
					//this.Page.RegisterStartupScript("AlertModalDialog", scriptString);
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "AlertModalDialog", scriptString); 
				}
			}
		}

        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
            Page.RegisterStartupScript("focus", s);
        }

		/// <summary>
		/// Gestione della spunta Mantieni selezione
		/// </summary>
		private void ChkMantieniSelezione()
		{
			if (!chk_mantieniSel.Checked)
			{
                this.ResetDetail();

                // gestione naviga UO
                if (this.IsEnabledNavigazioneUO())
                {
                    if (this.grdUOApp.Visible && this.grdUOApp.Items.Count > 0)
                    {
                        if (!this.GetId(this.grdUOApp.Items[0]).Equals(this.hdUOapp.Value))
                            this.FillGridDestinatariDefault();
                    }
                    else
                        this.FillGridDestinatariDefault();
                }    
			}

            // gestione fascicolazione rapida
            if (this.IsEnabledFascicolazione())
                this.ResetFascicolazione();
		}

		/// <summary>
		/// Abilita i tasti di default (Smista e Visto)
		/// </summary>
		private void EnableButtonsDefault()
		{
			this.btn_smista.Enabled=true;
			this.btn_scarta.Enabled=true;
            this.btn_AdL.Enabled = true;

            // Se il documento è stato ricevuto per interoperabilità semplificata, il pulsante 
            // "Visto", deve essere attivo solo nel caso in cui il documento sia protocollato
            if (this.btn_scarta.Text.ToLower() == "visto")
            {
                SmistaDocManager docManager = this.GetSmistaDocManager();
                DocsPaWR.DocumentoSmistamento docSmistamento = docManager.GetCurrentDocument(false);
                if (InteroperabilitaSemplificataManager.IsDocumentReceivedWithIS(docSmistamento.IDDocumento) &&
                    String.IsNullOrEmpty(docSmistamento.Segnatura))
                    this.btn_scarta.Enabled = false;

            }

		}

		/// <summary>
		/// Gestione del tasto di rifiuto
		/// </summary>
		private void EnableButtonRifiuta()
		{
			smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();
			
            if (docManager.IsTrasmissioneConWorkflow(docManager.GetCurrentDocumentPosition()-1))
			{
                // La trasmissione prevede workflow
                this.btn_rifiuta.Enabled = true;

                this.RegisterClientScriptBtnRifiuta();
			}
			else
			{
				this.btn_rifiuta.Enabled=false;
			}
		}

		/// <summary>
		/// Visualizzazione file
		/// </summary>
		private void ShowDocumentFile(bool content)
		{
			smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();
			DocsPaWR.DocumentoSmistamento docSmistamento=docManager.GetCurrentDocument(content);	

			if (!this.chk_showDoc.Checked)
			{
				btn_zoom.Enabled=false;
				Response.Write("<SCRIPT>top.left.location='SmistaDoc_DocNoVisualizzato.htm';</SCRIPT>");
			}
			else 
			{
                // Gestione abilitazione / disabilitazione pulsante dettagli firma
                this.EnableButtonDettagliFirma();

				if (docSmistamento.ImmagineDocumento!=null)
				{
                    btn_zoom.Enabled = true;
                    if ((System.Configuration.ConfigurationManager.AppSettings["VIS_UNIFICATA"] != null) && (System.Configuration.ConfigurationManager.AppSettings["VIS_UNIFICATA"] == "1"))
                    {
                        //mod: gestione vis_unificata // mod: non setto data vista solo da smistamento
                        DocsPAWA.DocsPaWR.SchedaDocumento sd = DocumentManager.getDettaglioDocumentoNoDataVista(this, docSmistamento.IDDocumento, docSmistamento.DocNumber);
                        
                        DocumentManager.setDocumentoSelezionato(this, sd);
                        FileManager.setSelectedFile(this, sd.documenti[0], false);

                        Response.Write("<SCRIPT>top.left.location='../documento/docVisualizzaFrame.aspx?id=" + Session.SessionID + "';</SCRIPT>");								
                    }
                    else Response.Write("<SCRIPT>top.left.location='SmistaDoc_Visualizzazione.aspx';</SCRIPT>");	
				}
				else
				{
					btn_zoom.Enabled=false;
                    //Page.RegisterClientScriptBlock("daAcquisire", "<SCRIPT>top.left.location='../documento/statoDocDaAcquisire.aspx';</SCRIPT>");
					Response.Write("<SCRIPT>top.left.location='../documento/statoDocDaAcquisire.aspx';</SCRIPT>");
				}
			}			
		}

		/// <summary>
		/// Formattazione dei destinatari del documento
		/// </summary>
		/// <param name="docSmistamento"></param>
		/// <returns></returns>
		private string GetDestinatari(DocsPAWA.DocsPaWR.DocumentoSmistamento docSmistamento)
		{
			string retValue=string.Empty;

			foreach (string dest in docSmistamento.DestinatariDocumento)
			{
				if (retValue!=string.Empty)
					retValue += "; ";

				retValue += dest;
			}

			return retValue;
		}

		/// <summary>
		/// Persistenza del valore del check visualizza documento nei cookie
		/// </summary>
		private void PersistChkShowDocumentValue()
		{
			//			this.Response.Cookies["SHOW_DOCUMENT_ACTIVE"].Value=chk_showDoc.Checked.ToString();		
		}

		/// <summary>
		/// Ripristino del valore del check visualizza documento dai cookie
		/// </summary>
		private void RestoreChkShowDocumentValue()
		{
			//			HttpCookie cookie=this.Request.Cookies["SHOW_DOCUMENT_ACTIVE"];
			//		
			//			if (cookie!=null)
			//			{
			//				if (cookie.Value!=null &&!cookie.Value.Equals(string.Empty))
			//					chk_showDoc.Checked=Convert.ToBoolean(cookie.Value);
			//				
			//				cookie=null;
			//			}
		}

		#endregion

		#region Smistamento documento

		/// <summary>
		/// Rimozione delle selezioni effettuate nei datagrid correntemente visualizzate
		/// </summary>
		private void ResetDetail()
		{
            // Rimozione delle selezioni effettuate negli oggetti dello smistamento
            this.ClearSelections();

			foreach (DataGridItem grdItem in this.grdUOApp.Items)
			{	
				CheckBox chkComp=this.GetGridItemControl<CheckBox>(grdItem, "chkComp");
                CheckBox chkCC = this.GetGridItemControl<CheckBox>(grdItem, "chkCC");
				CheckBox chkNotifica=this.GetGridItemControl<CheckBox>(grdItem, "chkNotifica");

				chkComp.Checked = false;
				chkCC.Checked = false;
				chkNotifica.Checked = false;

                if (grdItem.Cells[1].Text == "R")
                    this.RefreshItemUoAppartenenza(grdItem, false);
            }

			foreach (DataGridItem grdItem in this.grdUOInf.Items)
			{
                CheckBox chkComp = this.GetGridItemControl<CheckBox>(grdItem, "chkComp");
                CheckBox chkCC = this.GetGridItemControl<CheckBox>(grdItem, "chkCC");

				chkComp.Checked = false;
				chkCC.Checked = false;

                ImageButton imgNote = this.GetGridItemControl<ImageButton>(grdItem, "imgNote");
                imgNote.Visible = (chkComp.Checked || chkCC.Checked);
			}

            // gestione fascicolazione rapida
            if (this.IsEnabledFascicolazione())
                this.ResetFascicolazione();

            if (this.pnl_trasm_rapida.Visible)
            {
                this.ddl_trasm_rapida.SelectedIndex = -1;
                this.GetSmistaDocManager().GetUOAppartenenza().UoSmistaTrasAutomatica = null;
            }

            this.ResetNoteGenerali();
		}

        /// <summary>
        /// 
        /// </summary>
        private void ResetNoteGenerali()
        {
            this.txtNoteGen.Text = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uoApp"></param>
        /// <param name="selezioneUtenteRuolo"></param>
        private void UpadateFlagUtentiRuoloSelezionato(DocsPaWR.UOSmistamento uoApp, out bool selezioneUtenteRuolo)
        {
            //bool retValue = false;
            selezioneUtenteRuolo = false;

            if (this.grdUOApp.Visible && this.grdUOApp.Items.Count > 0)
            {
                smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();

                // gestione naviga UO
                if (!this.grdUOApp.Items[0].Cells[0].Text.Equals(this.hdUOapp.Value))
                    docManager.FillCurrentUO_NavigaUO(this.grdUOApp.Items[0].Cells[0].Text, UserManager.getRuolo(this.Page), UserManager.getUtente(this.Page), UserManager.getInfoUtente(this));

               // DocsPaWR.UOSmistamento uoApp = docManager.GetUOAppartenenza();

                //Controllo se per i ruoli selezionati delle UO di Appartenenza sia selezionato almeno un utente
                //Caso che si puo' verificare in quanto adesso la notifica puo' essere abilitata o meno da web.config
                foreach (DocsPaWR.RuoloSmistamento ruolo in uoApp.Ruoli)
                {
                    if (ruolo.FlagCompetenza || ruolo.FlagConoscenza)
                    {
                        int i = 0;
                        for (i = 0; i < ruolo.Utenti.Length; i++)
                        {
                            DocsPaWR.UtenteSmistamento utente = (DocsPaWR.UtenteSmistamento)ruolo.Utenti[i];
                            if (utente.FlagCompetenza || utente.FlagConoscenza)
                                break;
                        }
                        if (i == ruolo.Utenti.Length)
                        {
                            selezioneUtenteRuolo = true;
                            break;
                        }
                    }
                }
            }

          //  return retValue;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uoApp"></param>
        /// <param name="selezioneUoApp"></param>
		private void UpdateFlagUOAppartenenza(DocsPaWR.UOSmistamento uoApp, out bool selezioneUoApp)
		{
			//bool retValue = false;
            selezioneUoApp = false;

            if (this.grdUOApp.Visible && this.grdUOApp.Items.Count > 0)
			{
				smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();

                // gestione naviga UO                
                if (!this.grdUOApp.Items[0].Cells[0].Text.Equals(this.hdUOapp.Value))
                    docManager.FillCurrentUO_NavigaUO(this.grdUOApp.Items[0].Cells[0].Text, UserManager.getRuolo(this.Page), UserManager.getUtente(this.Page), UserManager.getInfoUtente(this));
              
				string itemID = string.Empty;
				string type = string.Empty;
				bool flagComp = false;
				bool flagCC = false;
                bool flagNotifica = false;
          
                foreach (DataGridItem grdItem in this.grdUOApp.Items)
				{
                    this.LoadItemValues(grdItem, out itemID, out type, out flagComp, out flagCC, out flagNotifica);   

					if ((type.Equals("R") || type.Equals("P")))
					{						
						this.SetFlagUO(true, uoApp, type, itemID, flagComp, flagCC, flagNotifica, grdItem.DataSetIndex);
						selezioneUoApp = true; 
					}
				}                   
			}
		}

        private void UpdateFlagUOInferiori(DocsPaWR.UOSmistamento[] uoInferiori, out bool selezioneUoInf)
        {
            selezioneUoInf = false;
            
            if (this.grdUOInf.Visible && this.grdUOInf.Items.Count > 0)
            {
                smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();

                // gestione naviga UO
                if (this.grdUOApp.Visible && this.grdUOApp.Items.Count > 0)
                    if (!this.grdUOApp.Items[0].Cells[0].Text.Equals(this.hdUOapp.Value))
                        docManager.FillUOInf_NavigaUO(this.grdUOApp.Items[0].Cells[0].Text, UserManager.getRuolo(this.Page), UserManager.getUtente(this.Page), UserManager.getInfoUtente(this));

                string itemID = string.Empty;
                string type = string.Empty;
                bool flagComp = false;
                bool flagCC = false;
                bool flagNotifica = false;

                foreach (DataGridItem grdItem in this.grdUOInf.Items)
                {
                    this.LoadItemValues(grdItem, out itemID, out type, out flagComp, out flagCC, out flagNotifica);

                    if (type.Equals("U"))
                    {
                        foreach (DocsPAWA.DocsPaWR.UOSmistamento currentUO in uoInferiori)
                        {
                            bool uoChanged = false;

                            if (!currentUO.Selezionata)
                            {
                                if (currentUO.ID.Equals(itemID))
                                {
                                    uoChanged = (flagComp != currentUO.FlagCompetenza || flagCC != currentUO.FlagConoscenza);

                                    currentUO.FlagCompetenza = flagComp;
                                    currentUO.FlagConoscenza = flagCC;

                                    foreach (DocsPAWA.DocsPaWR.RuoloSmistamento ruolo in currentUO.Ruoli)
                                    {
                                        // Smistamento del documento solamente ai ruoli di riferimento
                                        if (
                                            (ruolo.RuoloRiferimento) &&
                                                (
                                                    (flagComp && (this.hdTipoDestCompetenza.Value == string.Empty || ruolo.Gerarchia == this.hdTipoDestCompetenza.Value)) ||
                                                    (flagCC && (this.hdTipoDestConoscenza.Value == string.Empty || ruolo.Gerarchia == this.hdTipoDestConoscenza.Value))
                                                )
                                           )
                                        {
                                            ruolo.FlagCompetenza = flagComp;
                                            ruolo.FlagConoscenza = flagCC;

                                            SetFlagUtentiRuolo(ruolo, flagComp, flagCC);

                                            selezioneUoInf = true;
                                        }
                                        else
                                        {
                                            ruolo.FlagCompetenza = false;
                                            ruolo.FlagConoscenza = false;

                                            SetFlagUtentiRuolo(ruolo, false, false);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (currentUO.ID.Equals(itemID))
                                {
                                    uoChanged = (flagComp != currentUO.FlagCompetenza || flagCC != currentUO.FlagConoscenza);

                                    currentUO.FlagCompetenza = flagComp;
                                    currentUO.FlagConoscenza = flagCC;

                                    foreach (DocsPaWR.RuoloSmistamento ruolo in currentUO.Ruoli)
                                    {
                                        if (ruolo.RuoloRiferimento)
                                        {
                                            if (uoChanged || (!ruolo.FlagCompetenza && !ruolo.FlagConoscenza))
                                            {
                                                ruolo.FlagCompetenza = flagComp;
                                                ruolo.FlagConoscenza = flagCC;

                                                foreach (DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                                                {
                                                    if (uoChanged || (!utente.FlagCompetenza && !utente.FlagConoscenza))
                                                    {
                                                        utente.FlagCompetenza = flagComp;
                                                        utente.FlagConoscenza = flagCC;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

		private void SetFlagUO(bool isUOAppartenenza,
                                DocsPaWR.UOSmistamento uo,
                                string type,
                                string id,
                                bool flagComp,
                                bool flagCC,
                                bool flagNotifica, 
                                int indice)
		{
			switch (type)
			{
				case "U": // ----------------------- Unità organizzative -----------------
					uo.FlagCompetenza=flagComp;
					uo.FlagConoscenza=flagCC;
					
					// Impostazione flag per i ruoli della UO
					this.SetFlagRuoliUO(isUOAppartenenza,uo,flagComp,flagCC);
					break;

				case "R": // ----------------------- Ruoli -------------------------------
					foreach (DocsPAWA.DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
					{	
						if(ruolo.ID.Equals(id))
						{
							ruolo.FlagCompetenza=flagComp;
							ruolo.FlagConoscenza=flagCC;

                            //ruolo.datiAggiuntiviSmistamento.NoteIndividuali = smistaDoc.SmistaDocSessionManager.GetNoteIndividuali(indice, "grdUOApp");
                            //ruolo.datiAggiuntiviSmistamento.dtaScadenza = smistaDoc.SmistaDocSessionManager.GetDataScadenza(indice, "grdUOApp");
                            //ruolo.datiAggiuntiviSmistamento.tipoTrasm = smistaDoc.SmistaDocSessionManager.GetTipoTrasm(indice, "grdUOApp");

							return;
						}
					}
					break;
				
				case "P": // ----------------------- Utenti ------------------------------
                    string uniqueID = string.Empty;

					foreach (DocsPAWA.DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
					{			
						foreach (DocsPAWA.DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
						{
							uniqueID = ruolo.ID + "_" + utente.ID;

							//if(utente.ID.Equals(id))
                            if (uniqueID.Equals(id))
                            {
                                if (!flagComp && !flagCC && flagNotifica)
                                {
                                    // Se per l'utente risulta selezionata solamente l'opzione di notifica,
                                    // vengono impostati gli stessi valori dei flag del ruolo di appartenenza
                                    // NB: è il caso in cui lo smistamento è a ruolo
                                    utente.FlagCompetenza = ruolo.FlagCompetenza;
                                    utente.FlagConoscenza = ruolo.FlagConoscenza;
                                }
                                else
                                {
                                    // Selezione esplicita dell'utente
                                    // NB: è il caso in cui lo smistamento è direttamente ad utente
                                    utente.FlagCompetenza = flagComp;
                                    utente.FlagConoscenza = flagCC;
                                }

                                //utente.datiAggiuntiviSmistamento.NoteIndividuali = smistaDoc.SmistaDocSessionManager.GetNoteIndividuali(indice, "grdUOApp");
                                //utente.datiAggiuntiviSmistamento.dtaScadenza = smistaDoc.SmistaDocSessionManager.GetDataScadenza(indice, "grdUOApp");
                                return;
							}
						}
					}
					break;
			}
		}

		/// <summary>
		/// Impostazione flag per i ruoli e utenti della uo
		/// </summary>
		/// <param name="uo"></param>
		/// <param name="flagComp"></param>
		/// <param name="flagCC"></param>
		private void SetFlagRuoliUO(bool isUOAppartenenza,DocsPaWR.UOSmistamento uo,bool flagComp,bool flagCC)
		{
			foreach (DocsPAWA.DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
			{
				if (ruolo.RuoloRiferimento && isUOAppartenenza)
				{
					ruolo.FlagCompetenza=flagComp;
					ruolo.FlagConoscenza=flagCC;

					this.SetFlagUtentiRuolo(ruolo,flagComp,flagCC);
				}
			}
		}

		/// <summary>
		/// Impostazione flag per gli utenti di un ruolo
		/// </summary>
		/// <param name="ruolo"></param>
		/// <param name="flagComp"></param>
		/// <param name="flagCC"></param>
		private void SetFlagUtentiRuolo(DocsPAWA.DocsPaWR.RuoloSmistamento ruolo,bool flagComp,bool flagCC)
		{
			foreach (DocsPAWA.DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
			{
                utente.FlagCompetenza = flagComp;
                utente.FlagConoscenza = flagCC;
			}
		}


		private void LoadItemValues(DataGridItem grdItem,
			                        out string id,
			                        out string type,
			                        out bool flagComp,
			                        out bool flagCC,
                                    out bool flagNotifica)
		{
			id = grdItem.Cells[0].Text;
			type = grdItem.Cells[1].Text;
			flagComp = false;
			flagCC = false;
            flagNotifica = false;

			CheckBox chkComp=grdItem.Cells[3].FindControl("chkComp") as CheckBox;
			CheckBox chkCC=grdItem.Cells[3].FindControl("chkCC") as CheckBox;
            CheckBox chkNotifica = grdItem.Cells[3].FindControl("chkNotifica") as CheckBox;
						
			if (chkComp!=null && chkCC!=null && chkComp.Visible && chkCC.Visible)
			{
				flagComp=chkComp.Checked;
				flagCC=chkCC.Checked;

                // NB: condizione su null necessaria in quanto la griglia uo inferiori non ha il flag di notifica
                flagNotifica = (chkNotifica != null && chkNotifica.Checked);
			}            
		}

		#endregion

		#region Caricamento griglie destinatari

		/// <summary>
		/// // Caricamento griglie destinatari
		/// </summary>
		private void FillGridDestinatari()
		{
            this.heightUoApp = 0;
            this.heightUoApp = 37;
			if (!IsPostBack)
			{
				smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();

				DocsPaWR.UOSmistamento uoApp=docManager.GetUOAppartenenza();

                if (uoApp!=null && this.hdUOapp.Value.Equals(""))
                    this.hdUOapp.Value = uoApp.ID;

                this.indxUoSel = this.hdUOapp.Value;

				// Caricamento griglia uo di appartenenza
				if(uoApp!=null && uoApp.Ruoli.Length>0)
				{					          
					this.FillDataGridUOApp(uoApp);
				}

				// Caricamento griglia uo inferiori
				DocsPaWR.UOSmistamento[] uoInferiori=docManager.GetUOInferiori();

				if (uoInferiori!=null && uoInferiori.Length>0)
				{
                    this.grdUOInf.Visible = true;
                    this.heightUoInf = 25;

                    foreach (DocsPaWR.UOSmistamento tempUoInf in uoInferiori)
                    {
                        if (tempUoInf.Descrizione.Length > 38)
                        {
                            this.heightUoInf += 36;
                        }
                        else
                        {
                            this.heightUoInf += 25;
                        }
                    }

					this.FillDataGridUOInferiori(uoInferiori);
                    if (heightUoApp >= 180)
                    //if (heightUoApp >= 190)
                    {
                        this.pnlContainerUoAppartenenza.Height = 180;
                        //this.pnlContainerUoAppartenenza.Height = 190;
                    }
                    else
                    {
                        this.pnlContainerUoAppartenenza.Height = this.heightUoApp;
                        this.pnlContainerUoInferiori.Height = 170 + (170 - this.heightUoApp);
                       // this.pnlContainerUoInferiori.Height = 180 + (180 - this.heightUoApp);
                    }

                    if (heightUoInf < 170 && heightUoApp >= 180)
                    //if (heightUoInf < 180 && heightUoApp >= 190)
                    {
                        this.pnlContainerUoAppartenenza.Height = Convert.ToInt32(this.pnlContainerUoAppartenenza.Height.Value) + (160 - this.heightUoInf);
                        //this.pnlContainerUoAppartenenza.Height = Convert.ToInt32(this.pnlContainerUoAppartenenza.Height.Value) + (170 - this.heightUoInf);
                        this.pnlContainerUoInferiori.Height = heightUoInf;
                    }
                    else
                    {
                        if (heightUoApp >= 180)
                        //if (heightUoApp >= 190)
                        {
                            this.pnlContainerUoInferiori.Height = 160;
                            //this.pnlContainerUoInferiori.Height = 170;
                        }
                    }
                }
                else
                {
                    this.pnlContainerUoInferiori.Height = 0;
                    this.pnlContainerUoAppartenenza.Height = 330;
                    //this.pnlContainerUoAppartenenza.Height = 340;
                }	
			}

            // gestione naviga UO
            this.GestVisibilitaFrecceNavigaUO();
		}

		/// <summary>
		/// Verifica se esiste almeno un destinatario (o ruoli della UO di appartenenza o uo inferiori)  
		/// </summary>
		/// <returns></returns>
		private bool CheckExistGridDestinatari()
		{
			bool retValue = false;
			bool existUOApp = false;
			bool existUOInf = false;

			smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();
			// uo di appartenenza
			DocsPaWR.UOSmistamento uoApp=docManager.GetUOAppartenenza();
			if(uoApp!=null && uoApp.Ruoli.Length>0)
			{					
				existUOApp = true;
				// uo inferiori
				DocsPaWR.UOSmistamento[] uoInferiori=docManager.GetUOInferiori();
				if (uoInferiori!=null && uoInferiori.Length>0)
				{
					existUOInf = true;
				}
			}		
	
			if(existUOApp || existUOInf) retValue = true;

			return retValue;
		}

		
        /// <summary>
        /// Caricamento dataset utilizzato per le griglie
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="uo"></param>
        private void FillGridDataSet(DataSet ds, DocsPaWR.UOSmistamento uo, bool isUOAppartenenza)
        {
            DataTable dt = ds.Tables["GRID_TABLE"];

            this.AppendDataRow(dt, "U", uo.ID, uo.Descrizione, "", "", false);

            if (isUOAppartenenza)
            {
                foreach (DocsPAWA.DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                {
                    if (uo.Descrizione.Length < 38)
                    {
                        this.heightUoApp += 24;
                    }
                    else
                    {
                        this.heightUoApp += 34;
                    }

                    if (ruolo.Utenti != null && ruolo.Utenti.Length > 0)
                    {
                        this.AppendDataRow(dt, "R", ruolo.ID, ruolo.Descrizione, ruolo.ID, ruolo.Gerarchia, ruolo.disabledTrasm);

                        if (ruolo.Descrizione.Length > 38)
                        {
                            this.heightUoApp += 34;
                        }
                        else
                        {
                            this.heightUoApp += 24;
                        }

                        foreach (DocsPAWA.DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                        {
                            this.heightUoApp += 24;
                            this.AppendDataRow(dt, "P", ruolo.ID + "_" + utente.ID, utente.Denominazione, ruolo.ID, ruolo.Gerarchia, ruolo.disabledTrasm);
                        }
                    }
                }
            }
        }

		private DataSet CreateGridDataSet()
		{
			DataSet retValue=new DataSet();

			DataTable dt=new DataTable("GRID_TABLE");
			dt.Columns.Add("ID",typeof(string));
			dt.Columns.Add("TYPE",typeof(string));
			dt.Columns.Add("DESCRIZIONE",typeof(string));
            dt.Columns.Add("DESCR",typeof(string));
            dt.Columns.Add("PARENT", typeof(string));
            dt.Columns.Add("ROWINDEX",typeof(Int32));
            dt.Columns.Add("GERARCHIA", typeof(string));
            dt.Columns.Add("DISABLED_TRASM", typeof(string));
			retValue.Tables.Add(dt);

			return retValue;
		}
		
		private string GetImage(string rowType)
		{
			string retValue=string.Empty;
			string spaceIndent=string.Empty;

			switch (rowType)
			{
				case "U":
					retValue="uo_noexp";				
					break;

				case "R":
					retValue="ruolo_noexp";
					spaceIndent="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
					break;

				case "P":
					retValue="utente_noexp";
					spaceIndent="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
					break;
			}

			retValue=spaceIndent + "<img src='../images/smistamento/" + retValue + ".gif' border='0'>";

			return retValue;
		}

	    private void AppendDataRow(DataTable dt, string type, string id, string descrizione, string padre, string superiore, bool disabledTrasm)
        {            
            DataRow row = dt.NewRow();
            row["ID"] = id;
            row["TYPE"] = type;
            row["DESCRIZIONE"] = this.GetImage(type) + " " + descrizione;
            row["DESCR"] = descrizione;
            row["PARENT"] = padre;
            row["GERARCHIA"] = superiore;
            row["DISABLED_TRASM"] = disabledTrasm ? "1" : "0";
            dt.Rows.Add(row);
            
            row = null;
        }

        /// <summary>
        /// Impostazione visibilità controlli di un elemento del datagrid (sia appartenenza che inferiori)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="visible"></param>
        private void SetRowControlsVisibility(DataGridItem item, bool visible)
        {
            CheckBox radio = this.GetGridItemControl<CheckBox>(item, "chkComp");
            if (radio != null)
                radio.Visible = visible;

            radio = this.GetGridItemControl<CheckBox>(item, "chkCC");
            if (radio != null)
                radio.Visible = visible;

            radio = this.GetGridItemControl<CheckBox>(item, "chkNotifica");
            if (radio != null)
                radio.Visible = visible;

            ImageButton imgNote = this.GetGridItemControl<ImageButton>(item, "imgNote");

            if (imgNote != null)
                imgNote.Visible = visible;
        }

		#region Griglia UO appartenenza
		
		/// <summary>
		/// Caricamento griglia uo di appartenenza
		/// </summary>
		/// <param name="uoApp"></param>
		private void FillDataGridUOApp(DocsPAWA.DocsPaWR.UOSmistamento uoApp)
		{
			DataSet ds=this.CreateGridDataSet();

			this.FillGridDataSet(ds,uoApp,true);

			this.grdUOApp.DataSource=ds;
			this.grdUOApp.DataBind();

            this.SetRowControlsVisibility(this.grdUOApp.Items[0], false);

            // Gestione abilitazione / disabilitazione checkbox 
            // in funzione delle regole di gerarchia delle ragioni trasmissione
            this.RefreshOptionsEnabledPerGerarchia(this.grdUOApp);
		}

        /// <summary>
        /// Reperimento tipo elemento griglia
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetTipoURP(DataGridItem item)
        {
            return item.Cells[1].Text;
        }

        /// <summary>
        /// Reperimento ID elemento griglia
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetId(DataGridItem item)
        {
            string tipo = this.GetTipoURP(item);

            if (tipo == "R" || tipo == "U")
                return item.Cells[0].Text;
            else if (tipo == "P")
                return item.Cells[0].Text.Split('_')[1];
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected T GetGridItemControl<T>(DataGridItem item, string controlId) where T : Control
        {
            Control ctrl = item.FindControl(controlId);

            if (ctrl == null)
                return default(T);
            else
                return (T)ctrl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGridAppItemBounded(Object sender, DataGridItemEventArgs e)
        {
            string type = e.Item.Cells[1].Text;

            if (type.Equals("U"))
            {
                this.SetRowControlsVisibility(e.Item, false);
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CheckBox chkComp = this.GetGridItemControl<CheckBox>(e.Item, "chkComp");
                CheckBox chkCC = this.GetGridItemControl<CheckBox>(e.Item, "chkCC");
                CheckBox chkNotifica = this.GetGridItemControl<CheckBox>(e.Item, "chkNotifica");

                if (!type.Equals("U"))
                {
                    string id = string.Empty;
                    string[] ids = e.Item.Cells[0].Text.Split('_');

                    if (type.Equals("R"))
                    {
                        id = ids[0];
                    }
                    else if (type.Equals("P"))
                    {
                        id = ids[1];
                    }

                    // Impostazione script client per il pulsante di inserimento dati aggiuntivi trasmissione
                    ImageButton imgNote = this.GetGridItemControl<ImageButton>(e.Item, "imgNote");
                    imgNote.OnClientClick = string.Format("ApriFinestraNoteSmistamento('{0}', '{1}', '{2}', '{3}');",
                        e.Item.ItemIndex, type, id, this.grdUOApp.ID);
                }

                if (chkComp != null)
                    chkComp.ToolTip = hdDescrCompetenza.Value;

                if (chkCC != null)
                    chkCC.ToolTip = hdDescrConoscenza.Value;
            }

            // gestione naviga UO
            if (this.IsEnabledNavigazioneUO())
            {
                if (type.Equals("U"))
                {
                    e.Item.Cells[3].Visible = true;
                    if (this.hdUOapp.Value.Equals(e.Item.Cells[0].Text))
                    {
                        e.Item.Cells[3].Text = "";
                    }
                }
                else
                    e.Item.Cells[3].Text = "";
            }
            else
                e.Item.Cells[3].Visible = false;
        }        

		#endregion

		#region Griglia UO inferiori

		/// <summary>
		/// Caricamento griglia UO destinatari
		/// </summary>
		/// <param name="uoInferiori"></param>
		private void FillDataGridUOInferiori(DocsPAWA.DocsPaWR.UOSmistamento[] uoInferiori)
		{
			DataSet ds=this.CreateGridDataSet();

			foreach (DocsPAWA.DocsPaWR.UOSmistamento uo in uoInferiori)
				this.FillGridDataSet(ds,uo,false);
            
			this.grdUOInf.DataSource=ds;
			this.grdUOInf.DataBind();
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OnGridInfItemBounded(Object sender, DataGridItemEventArgs e)
		{
            DataGridItem item = e.Item;

            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            {
                string id = item.Cells[0].Text;
                string type = item.Cells[1].Text;

                if (type.Equals("R") || type.Equals("P"))
                {
                    // Per le UO inferiori, rende invisibili i controlli per il ruolo e per gli utenti
                    this.SetRowControlsVisibility(item, false);
                }
                else if (type.Equals("U"))
                {
                    // Tipologia UO
                    DocsPaWR.UOSmistamento uo = this.getUoSelezionata(this.GetSmistaDocManager().GetUOAppartenenza(), id);

                    if (uo.Ruoli.Length == 0)
                        // Nessun ruolo di riferimento presente nell'UO, vengono resi invisibili i controlli
                        this.SetRowControlsVisibility(item, false);
                    else
                    {
                        // Determina se il livello di gerarchia per tutti i ruoli di riferimento contenuti 
                        // nell'UO ve ne sia almeno uno tale per cui è possibile trasmettere in base alla
                        // ragione trasmissione impostata in amministrazione
                        bool flagComp = this.hdTipoDestCompetenza.Value == string.Empty ||
                                        (uo.Ruoli.Count(itm => itm.Gerarchia == this.hdTipoDestCompetenza.Value) > 0);
                        bool flagCC = this.hdTipoDestConoscenza.Value == string.Empty ||
                                            (uo.Ruoli.Count(itm => itm.Gerarchia == this.hdTipoDestConoscenza.Value) > 0);

                        CheckBox chkComp = this.GetGridItemControl<CheckBox>(item, "chkComp");
                        CheckBox chkCC = this.GetGridItemControl<CheckBox>(item, "chkCC");

                        chkComp.Enabled = flagComp;
                        chkCC.Enabled = flagCC;
                    }

                    if (!this.IsEnabledNavigazioneUO())
                        // Pulsante di navigazione visibile
                        item.Cells[3].Visible = true;
                }

                // Impostazione script client per il pulsante di inserimento dati aggiuntivi trasmissione
                ImageButton imgNote = this.GetGridItemControl<ImageButton>(e.Item, "imgNote");
                imgNote.OnClientClick = string.Format("ApriFinestraNoteSmistamento('{0}', '{1}', '{2}', '{3}');",
                    e.Item.ItemIndex, type, id, this.grdUOInf.ID);
            }
		}

		#endregion

		#endregion
        
		#region Gestione Pulsanti

        /// <summary>
        /// Verifica se l'opzione competenza di un elemento della griglia è abilitata o meno
        /// in funzione delle regole di gerarchia delle ragioni di trasmissione
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool IsEnabledOptionCompPerGerarchia(DataGridItem item)
        {
            bool isEnabled = true;

            CheckBox chkComp = this.GetGridItemControl<CheckBox>(item, "chkComp");

            if (chkComp != null)
            {
                if (!string.IsNullOrEmpty(this.hdTipoDestCompetenza.Value))
                {
                    //prendo la gerarchia dell'elemento corrente presente in ultima
                    //posizione nel datagrid in ultima colonna
                    Label hdGerarchia = this.GetGridItemControl<Label>(item, "hdGerarchia");

                    isEnabled = (string.IsNullOrEmpty(hdGerarchia.Text) || 
                                 this.hdTipoDestCompetenza.Value == hdGerarchia.Text);
                }
            }

            return isEnabled;
        }

        /// <summary>
        /// Verifica se l'opzione conoscenza di un elemento della griglia è abilitata o meno
        /// in funzione delle regole di gerarchia delle ragioni di trasmissione 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool IsEnabledOptionCCPerGerarchia(DataGridItem item)
        {
            bool isEnabled = true;

            CheckBox chkCC = this.GetGridItemControl<CheckBox>(item, "chkCC");

            if (chkCC != null)
            {
                if (!string.IsNullOrEmpty(this.hdTipoDestConoscenza.Value))
                {
                    //prendo la gerarchia dell'elemento corrente presente in ultima
                    //posizione nel datagrid in ultima colonna
                    Label hdGerarchia = this.GetGridItemControl<Label>(item, "hdGerarchia");

                    isEnabled = (string.IsNullOrEmpty(hdGerarchia.Text) ||
                                 this.hdTipoDestConoscenza.Value == hdGerarchia.Text);
                }
            }

            return isEnabled;
        }

        /// <summary>
        /// Gestione abilitazione / disabilitazione opzioni in funzione
        /// delle regole di gerarchia delle ragioni trasmissione
        /// </summary>
        /// <param name="dataGrid"></param>
        protected void RefreshOptionsEnabledPerGerarchia(DataGrid dataGrid)
        {
            foreach (DataGridItem itm in dataGrid.Items)
            {
                this.RefreshOptionsEnabledPerGerarchia(itm);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itm"></param>
        protected virtual void RefreshOptionsEnabledPerGerarchia(DataGridItem itm)
        {
            CheckBox chkComp = this.GetGridItemControl<CheckBox>(itm, "chkComp");
            chkComp.Enabled = this.IsEnabledOptionCompPerGerarchia(itm);

            CheckBox chkCC = this.GetGridItemControl<CheckBox>(itm, "chkCC");
            chkCC.Enabled = this.IsEnabledOptionCCPerGerarchia(itm);
        }

        /// <summary>
        /// Handler evento di selezione delle opzioni delle uo inferiori
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OpzioniUoInferiori_CheckedChanged(object sender, EventArgs e)
        {
            this.PerformActionCheckOpzioniUoInferiori((CheckBox)sender);
        }

        /// <summary>
        /// Handler evento di selezione delle opzioni del ruolo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OpzioniUoAppartenenza_CheckedChanged(object sender, EventArgs e)
        {
            this.PerformActionCheckOpzioniUoAppartenenza((CheckBox)sender);
        }

        /// <summary>
        /// Logica di gestione dell'azione di selezione di uno dei CheckBox relativi alle UO inferiori
        /// </summary>
        /// <param name="sender"></param>
        protected void PerformActionCheckOpzioniUoInferiori(CheckBox sender)
        {
            // Ricerca del container di tipo DataGridItem
            Control parent = sender.Parent;
            do
            {
                parent = parent.Parent;
            }
            while (parent != null && parent.GetType() != typeof(DataGridItem));

            DataGridItem item = (DataGridItem)parent;

            CheckBox chkComp = this.GetGridItemControl<CheckBox>(item, "chkComp");
            CheckBox chkCC = this.GetGridItemControl<CheckBox>(item, "chkCC");
            ImageButton imgNote = this.GetGridItemControl<ImageButton>(item, "imgNote");
            imgNote.Visible = (chkComp.Checked || chkCC.Checked);

            if (sender == chkComp)
            {
                if (chkComp.Checked)
                    chkCC.Checked = false;
            }
            else if (sender == chkCC)
            {
                if (chkCC.Checked)
                    chkComp.Checked = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="chkComp"></param>
        /// <param name="chkCC"></param>
        /// <param name="chkNotifica"></param>
        /// <param name="imgNote"></param>
        /// <param name="onCheckNotificaTutti"></param>
        protected void RefreshItemUoAppartenenza(string id, string type, CheckBox chkComp, CheckBox chkCC, CheckBox chkNotifica, ImageButton imgNote, bool onCheckNotificaTutti, Label hd_disablendTrasm)
        {
            // Determina, tramite il valore del campo nascosto "hdCheckedUtenti",
            // se in fase di selezione di un ruolo deve o meno selezionare in automatico
            // la notifica su tutti gli utenti dello stesso
            bool checkNotificaUtenti;
            bool.TryParse(this.hdCheckedUtenti.Value, out checkNotificaUtenti);

            if (!onCheckNotificaTutti)
            {
                chkNotifica.Enabled = (chkComp.Checked || chkCC.Checked);
                chkNotifica.Checked = checkNotificaUtenti && chkNotifica.Enabled;
            }

            if (type == "R")
            {
                // Selezione ruolo
                imgNote.Visible = (chkComp.Checked || chkCC.Checked);

                if (!imgNote.Visible)
                {
                    // TODO: Annullamento note smistamento per il ruolo
                }

                bool utentiRuolo = false;

                //Disabilitazione dei cb in quanto il ruolo risulata disabilitato alla ricezione delle trasmissioni
                if (hd_disablendTrasm != null && hd_disablendTrasm.Text == "1")
                {
                    chkComp.Checked = false;
                    chkComp.Enabled = false;
                    chkCC.Checked = false;
                    chkCC.Enabled = false;
                    chkNotifica.Checked = false;
                    chkNotifica.Enabled = false;
                }

                /* MAC 3981 - INPS (MCaropreso)
                 * contatore:           conta quanti utenti ci sono per ruolo
                 * lastUtenteNotifica:  puntatore all'ultimo utente esaminato
                 */
                int contatore = 0;
                CheckBox lastUtenteNotifica = null;

                foreach (DataGridItem itemUtente in this.grdUOApp.Items)
                {
                    if (utentiRuolo)
                    {
                        string typeUtente = this.GetTipoURP(itemUtente);

                        if (typeUtente == "P")
                        {
                            CheckBox chkCompUtente = this.GetGridItemControl<CheckBox>(itemUtente, "chkComp");
                            CheckBox chkCCUtente = this.GetGridItemControl<CheckBox>(itemUtente, "chkCC");
                            CheckBox chkNotificaUtente = this.GetGridItemControl<CheckBox>(itemUtente, "chkNotifica");

                            /* MAC 3981 - INPS (MCaropreso) 
                             * Aggiorna variabili di stato della ricerca utente
                             */
                            contatore++;
                            lastUtenteNotifica = chkNotificaUtente;

                            ImageButton imgNoteUtente = this.GetGridItemControl<ImageButton>(itemUtente, "imgNote");

                            /* MAC 3981 - INPS (MCaropreso)
                             * Verifica se l'utente è l'unico per il ruolo selezionato
                             */
                            //var conteggio = ds.Tables["GRID_TABLE"].AsEnumerable().Count(r => r.Field<string>("PARENT") == id);

                            if (onCheckNotificaTutti)
                            {
                                chkNotificaUtente.Checked = chkNotifica.Checked;
                            }
                            /*
                             else if (conteggio == 2)
                            {
                                chkNotificaUtente.Checked = true;
                            }
                             */
                            else
                            {
                                chkCompUtente.Enabled = this.IsEnabledOptionCompPerGerarchia(itemUtente) && !chkComp.Checked && !chkCC.Checked;
                                chkCompUtente.Checked = false;

                                chkCCUtente.Enabled = this.IsEnabledOptionCCPerGerarchia(itemUtente) && !chkComp.Checked && !chkCC.Checked;
                                chkCCUtente.Checked = false;

                                chkNotificaUtente.Enabled = (chkComp.Checked || chkCC.Checked);
                                chkNotificaUtente.Checked = checkNotificaUtenti && chkNotifica.Checked;

                                imgNoteUtente.Visible = false;

                                // TODO: Annullamento note smistamento per l'utente
                            }
                        }
                        else
                        {
                            utentiRuolo = false;

                            /* MAC 3981 - INPS (MCaropreso)
                             * Se abbiamo trovato un unico utente, aggiorna la notifica
                             */
                            if (contatore == 1)
                            {
                                lastUtenteNotifica.Checked = chkComp.Checked || chkCC.Checked;
                            }
                            break;
                        }
                    }
                    else if (this.GetId(itemUtente) == id)
                    {
                        utentiRuolo = true;
                    }
                }
                /* MAC 3981 - INPS (MCaropreso)
                 * Questa modifica è necessario se l'utente è l'ultimo elemento del datagrid
                 * Aggiorna il checkbox notifica se l'utente è l'unico per il ruolo
                 */
                if (utentiRuolo && contatore == 1)
                {
                    lastUtenteNotifica.Checked = chkComp.Checked || chkCC.Checked;
                }
            }
            else if (type == "P")
            {
                // Selezione riga utente
                if ((chkComp.Checked || chkCC.Checked) && chkComp.Enabled && chkCC.Enabled)
                    // Il check è disabilitato nel caso in cui venga marcato direttamente
                    // competenza o conoscenza per l'utente
                    chkNotifica.Enabled = false;
                else
                    // Il check è abilitato se i flag competenza e conoscenza sono disabilitati e non marcati
                    chkNotifica.Enabled = (!chkComp.Enabled && !chkCC.Enabled);

                imgNote.Visible = (chkComp.Checked || chkCC.Checked);

                if (!imgNote.Visible)
                {
                    // TODO: Annullamento note smistamento per l'utente

                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="onCheckNotificaTutti"></param>
        protected void RefreshItemUoAppartenenza(DataGridItem item, bool onCheckNotificaTutti)
        {
            string id = this.GetId(item);
            string type = this.GetTipoURP(item);
            CheckBox chkComp = this.GetGridItemControl<CheckBox>(item, "chkComp");
            CheckBox chkCC = this.GetGridItemControl<CheckBox>(item, "chkCC");
            CheckBox chkNotifica = this.GetGridItemControl<CheckBox>(item, "chkNotifica");
            ImageButton imgNote = this.GetGridItemControl<ImageButton>(item, "imgNote");
            Label hd_disablendTrasm = this.GetGridItemControl<Label>(item, "hd_disablendTrasm");

            this.RefreshItemUoAppartenenza(id, type, chkComp, chkCC, chkNotifica, imgNote, onCheckNotificaTutti, hd_disablendTrasm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected void PerformActionCheckOpzioniUoAppartenenza(CheckBox sender)
        {
            // Ricerca del container di tipo DataGridItem
            Control parent = sender.Parent;

            do
            {
                parent = parent.Parent;
            }
            while (parent != null && parent.GetType() != typeof(DataGridItem));

            DataGridItem item = (DataGridItem)parent;

            CheckBox chkComp = this.GetGridItemControl<CheckBox>(item, "chkComp");
            CheckBox chkCC = this.GetGridItemControl<CheckBox>(item, "chkCC");
            CheckBox chkNotifica = this.GetGridItemControl<CheckBox>(item, "chkNotifica");
            ImageButton imgNote = this.GetGridItemControl<ImageButton>(item, "imgNote");

            // Indica, se true, che il sender dell'evento è il checkBox di selezione tutti / nessuno 
            // del checkbox di notifica agli utenti del ruolo
            bool performSelectNotificaTutti = false;

            if (sender == chkComp)
            {
                if (chkComp.Checked)
                    chkCC.Checked = false;
            }
            else if (sender == chkCC)
            {
                if (chkCC.Checked)
                    chkComp.Checked = false;
            }
            else if (sender == chkNotifica)
            {
                // Il sender è il flag di seleziona / deseleziona tutti
                performSelectNotificaTutti = true;
            }

            // Se si è effettuata una selezione tra comp, cc e notifica di un utente o ruolo,
            // disabilita automaticamente le selezioni dell'uo di appartenenza
            MarkUoAsChanged(this.GetId(item), this.GetSmistaDocManager().GetUOAppartenenza());

            this.RefreshItemUoAppartenenza(item, performSelectNotificaTutti);
        }

        /// <summary>
        /// Rimuove le selezioni per competenza / conoscenza dell'uo di appartenenza
        /// dell'id utente o ruolo 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uo"></param>
        private bool MarkUoAsChanged(string id, DocsPaWR.UOSmistamento uo)
        {
            bool found = false;

          /*  if (uo.Ruoli.Count(e => e.ID == id) > 0)
            {
                uo.FlagCompetenza = false;
                uo.FlagConoscenza = false;

                found = true;
            }
            else
            {*/
                foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                {
                        if (ruolo.Utenti.Count(e => e.ID == id) > 0)
                        {
                            uo.FlagCompetenza = false;
                            uo.FlagConoscenza = false;
                        
                            found = true;
                           // break;
                        }
                  //  }
             //   }

                if (!found)
                {
                    if (uo.UoInferiori != null)
                    {
                        // Ricerca id tra le uo inferiori
                        foreach (DocsPaWR.UOSmistamento uoInferiore in uo.UoInferiori)
                        {
                            found = MarkUoAsChanged(id, uoInferiore);

                            if (found)
                            {
                                uoInferiore.FlagCompetenza = false;
                                uoInferiore.FlagConoscenza = false;
                                //    break;
                            }
                        }
                    }
                }
            }

            return found;
        }

		/// <summary>
		/// Pulsante chiudi finestra web
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			smistaDoc.SmistaDocSessionManager.ReleaseSmistaDocManager();
            if (Session["noDocSuccessivo"] != null)
                Session.Remove("noDocSuccessivo");

			if (!this.Page.ClientScript.IsStartupScriptRegistered(this.GetType(), "chiudiModalDialog"))
			{
                string scriptString = "<SCRIPT>window.returnValue = 'Y';window.close();</SCRIPT>";				
				//this.Page.RegisterStartupScript("chiudiModalDialog", scriptString);
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "chiudiModalDialog", scriptString);
			}			
		}

		#region pulsanti navigazione
		private void btn_first_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();
            //smistaDoc.SmistaDocSessionManager.ReleaseSmistamentoNoteIndividuali();

            if (docManager.MoveFirstDocument())
            {
                if (!this.chk_mantieniSel.Checked)
                {
                    // In navigazione tra documenti, 
                    // rimozione delle selezioni effettuate dall'utente
                    // qualora non sia attivo il flag mantieni selezione
                    this.ClearSelections();
                }

                //Remove note correnti
                if (Session["datiAggiuntivi"] != null)
                {

                    DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                    foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                    {
                        appoggioDatiAggiuntivi.NoteIndividuali = "";
                        appoggioDatiAggiuntivi.dtaScadenza = "";
                        appoggioDatiAggiuntivi.tipoTrasm = "";
                    }
                }

                this.FillDataDocumentoTrasmesso();
            }
		}

		private void btn_previous_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();
            //smistaDoc.SmistaDocSessionManager.ReleaseSmistamentoNoteIndividuali();
            
            if (docManager.MovePreviousDocument())
            {
                if (!this.chk_mantieniSel.Checked)
                {
                    // In navigazione tra documenti, 
                    // rimozione delle selezioni effettuate dall'utente
                    // qualora non sia attivo il flag mantieni selezione
                    this.ClearSelections();
                }

                //Remove note correnti
                if (Session["datiAggiuntivi"] != null)
                {

                    DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                    foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                    {
                        appoggioDatiAggiuntivi.NoteIndividuali = "";
                        appoggioDatiAggiuntivi.dtaScadenza = "";
                        appoggioDatiAggiuntivi.tipoTrasm = "";
                    }
                }

                this.FillDataDocumentoTrasmesso();
            }
		}

		private void btn_next_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();
            //smistaDoc.SmistaDocSessionManager.ReleaseSmistamentoNoteIndividuali();

            if (docManager.MoveNextDocument())
            {
                if (!this.chk_mantieniSel.Checked)
                {
                    // In navigazione tra documenti, 
                    // rimozione delle selezioni effettuate dall'utente
                    // qualora non sia attivo il flag mantieni selezione
                    this.ClearSelections();
                }

                //Remove note correnti
                if (Session["datiAggiuntivi"] != null)
                {

                    DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                    foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                    {
                        appoggioDatiAggiuntivi.NoteIndividuali = "";
                        appoggioDatiAggiuntivi.dtaScadenza = "";
                        appoggioDatiAggiuntivi.tipoTrasm = "";
                    }
                }

                this.FillDataDocumentoTrasmesso();
            }
		}

		private void btn_last_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();
            //smistaDoc.SmistaDocSessionManager.ReleaseSmistamentoNoteIndividuali();

            if (docManager.MoveLastDocument())
            {
                if (!this.chk_mantieniSel.Checked)
                {
                    // In navigazione tra documenti, 
                    // rimozione delle selezioni effettuate dall'utente
                    // qualora non sia attivo il flag mantieni selezione
                    this.ClearSelections();
                }

                //Remove note correnti
                if (Session["datiAggiuntivi"] != null)
                {

                    DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                    foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                    {
                        appoggioDatiAggiuntivi.NoteIndividuali = "";
                        appoggioDatiAggiuntivi.dtaScadenza = "";
                        appoggioDatiAggiuntivi.tipoTrasm = "";
                    }
                }

                this.FillDataDocumentoTrasmesso();
            }
		}
		#endregion

		/// <summary>
		/// TASTO SMISTA DOCUMENTO
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_smista_Click(object sender, System.EventArgs e)
		{            
			/*			Ragioni di trasmissione per lo smistamento:
			 *			
			 *			-------------------------------------------------------------------------
			 *			NOME:					COMPETENZA			|		CONOSCENZA	
			 *			-------------------------------------------------------------------------
			 *			VISIBILITA':				NO				|			NO
			 *			TIPO:					WORKFLOW			|		SENZA WORKFLOW
			 *			DIRITTI:				LETURA / SCRITTURA	|		LETTURA
			 *			DESTINATARI:			SOLO SOTTOPOSTI		|		SOLO SOTTOPOSTI
			 *			E' UNA RISPOSTA:			NO				|			NO
			 *			PREVEDE RISPOSTA:			NO				|			NO
			 *			EREDITA':					NO				|			NO
			 *			-------------------------------------------------------------------------
			 * */
            this.smistaDocumento();

            //Remove note correnti
            if (Session["datiAggiuntivi"] != null)
            {

                DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                {
                    appoggioDatiAggiuntivi.NoteIndividuali = "";
                    appoggioDatiAggiuntivi.dtaScadenza = "";
                    appoggioDatiAggiuntivi.tipoTrasm = "";
                }
            }

		}

		private void RegisterClientMessageError(string errorMessage)
		{
            if (!ClientScript.IsStartupScriptRegistered("errorModalDialog"))
			{
				string scriptString = "<SCRIPT>alert('" + errorMessage + "');</SCRIPT>";				
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "errorModalDialog", scriptString); 
			}
		}

		/// <summary>
		/// TASTO SCARTA DOCUMENTO
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_scarta_Click(object sender, System.EventArgs e)
		{
			try
			{
               // rimuovo le note individuali
                //smistaDoc.SmistaDocSessionManager.ReleaseSmistamentoNoteIndividuali();

                //Remove note correnti
                if (Session["datiAggiuntivi"] != null)
                {

                    DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                    foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                    {
                        appoggioDatiAggiuntivi.NoteIndividuali = "";
                        appoggioDatiAggiuntivi.dtaScadenza = "";
                        appoggioDatiAggiuntivi.tipoTrasm = "";
                    }
                }
                

                smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();	
				if(docManager.ScartaDoc())
				{
                    this.FascicolaDocumento();
                    this.verificaDiagrammiDiStato();
					this.ProgressStatus();	
				}
				else
				{
                    if (!ClientScript.IsStartupScriptRegistered("errorModalDialog"))
					{
						string scriptString = "<SCRIPT>alert('Errore nella funzionalità di impostazione del documento come VISTO.');</SCRIPT>";						
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "errorModalDialog", scriptString);
					}
				}
			}
			catch
			{
                if (!ClientScript.IsStartupScriptRegistered("errorModalDialog"))
				{
					string scriptString = "<SCRIPT>alert('Errore nella funzionalità di impostazione del documento come VISTO.');</SCRIPT>";                    
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "errorModalDialog", scriptString);
				}
			}
            finally
            {
                this.GestVisibilitaFrecceNavigaUO();
            }
		}

        /// <summary>
        /// Evento visualizzazione dettagli di firma
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_dettFirma_Click(object sender, EventArgs e)
        {
            DocsPaWR.DocumentoSmistamento docSmistamento = this.GetSmistaDocManager().GetCurrentDocument(true);

            if (this.btn_dettFirma.Enabled)
            {
                this.SetSignedDocumentOnSession(docSmistamento.ImmagineDocumento);

                if (!this.ClientScript.IsClientScriptBlockRegistered("ShowDettagliFirma"))
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ShowDettagliFirma", "<script>ShowMaskDettagliFirma();</script>");
            }
            else
            {
                this.SetSignedDocumentOnSession(null);
            }

            // gestione naviga UO
            this.GestVisibilitaFrecceNavigaUO();
        }

		/// <summary>
		/// TASTO RIFIUTA DOCUMENTO
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_rifiuta_Click(object sender, System.EventArgs e)
		{
			try
			{
                SmistaDocManager docManager = this.GetSmistaDocManager();

                // Reperimento dello stato della trasmissione utente
                // per controllo ulteriore allo scopo di impedire il rifiuto di un documento già accettato / rifiutato
                DocsPaWR.StatoTrasmissioneUtente statoTrasmissione = docManager.GetStatoTrasmissioneCorrente();

                if (statoTrasmissione.Accettata || statoTrasmissione.Rifiutata)
                {
                    this.RegisterClientMessageError(string.Format("Impossibile rifiutare il documento in quanto la trasmissione risulta già {0}", (statoTrasmissione.Accettata ? "accettata" : "rifiutata")));
                }
                else
                {
                    //Remove note correnti
                    if (Session["datiAggiuntivi"] != null)
                    {

                        DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                        foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                        {
                            appoggioDatiAggiuntivi.NoteIndividuali = "";
                            appoggioDatiAggiuntivi.dtaScadenza = "";
                            appoggioDatiAggiuntivi.tipoTrasm = "";
                        }
                    }

                    // rimuovo l'oggetto creato per memorizzare le note individuali
                    //smistaDoc.SmistaDocSessionManager.ReleaseSmistamentoNoteIndividuali();

                    if (this.hd_msg_rifiuto.Value.Trim() != "" && this.hd_msg_rifiuto.Value != "null" && this.hd_msg_rifiuto.Value.Length > 0)
                    {
                        if (docManager.RifiutaDoc(this.hd_msg_rifiuto.Value))
                        {
                            this.ProgressStatus();
                        }
                        else
                        {
                            if (!ClientScript.IsStartupScriptRegistered("errorModalDialog"))
                            {
                                string scriptString = "<SCRIPT>alert('Errore nella funzionalità di rifiuto documento!');</SCRIPT>";
                                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "errorModalDialog", scriptString);
                            }
                        }
                    }
                }
			}
			catch
			{
                if (!ClientScript.IsStartupScriptRegistered("errorModalDialog"))
				{
					string scriptString = "<SCRIPT>alert('Errore nella funzionalità di rifiuto documento!');</SCRIPT>";					
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(),"errorModalDialog", scriptString);
				}
			}
            finally
            {
                this.GestVisibilitaFrecceNavigaUO();
            }
		}

        /// <summary>
        /// inserisce il documento in ADL e se la trasmissione lo prevede l'accetta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_AdL_Click(object sender, System.EventArgs e)
        {
            smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();
            DocsPaWR.InfoDocumento info= new DocsPAWA.DocsPaWR.InfoDocumento();
            DocsPaWR.DocumentoSmistamento docsmista= docManager.GetCurrentDocument(false);
            //DocumentManager.getListaAreaLavoro(this, docsmista.TipoDocumento, "0", 
                
            bool inAdLOk = false;
            try
            {
                info.docNumber=docsmista.DocNumber;
                info.idProfile=docsmista.IDDocumento;
                info.idRegistro=docsmista.IDRegistro;
               // info.acquisitaImmagine=(docsmista.ImmagineDocumento!=null ? "1":"0");
                info.segnatura=docsmista.Segnatura;
                info.tipoProto=docsmista.TipoDocumento;

                try
                {
                    DocumentManager.addAreaLavoro(this, info);
                    inAdLOk = true;
                }
                catch 
                { 
                    throw new Exception("errore durante l'operazione di inserimendo in AdL ."); 
                }

                if (inAdLOk)
                {
                    this.btn_AdL.Text = "ADL **";    
                    //if (docManager.ScartaDoc())
                    //{
                    //    this.FascicolaDocumento();
                    //    this.ProgressStatus();
                    //}
                    //else 
                    //    throw new Exception("errore durante l'operazione di inserimendo in AdL .");                   
                    Session["noDocSuccessivo"] = true;
                }

                //Remove note correnti
                if (Session["datiAggiuntivi"] != null)
                {

                    DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = (DocsPaWR.datiAggiuntiviSmistamento[])Session["datiAggiuntivi"];

                    foreach (DocsPaWR.datiAggiuntiviSmistamento appoggioDatiAggiuntivi in datiAggiuntivi)
                    {
                        appoggioDatiAggiuntivi.NoteIndividuali = "";
                        appoggioDatiAggiuntivi.dtaScadenza = "";
                        appoggioDatiAggiuntivi.tipoTrasm = "";
                    }
                }               
            }
            catch
            {
                if (!ClientScript.IsStartupScriptRegistered("errorModalDialog"))
                {
                    string scriptString = "<SCRIPT>alert('Errore nella funzionalità inserimento in AdL documento!');</SCRIPT>";                    
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "errorModalDialog", scriptString);
                }
            }
            finally
            {
                if(Session["noDocSuccessivo"] == null)
                    this.GestVisibilitaFrecceNavigaUO();
            }
        }
		/// <summary>
		/// Tasto ZOOM visualizzazione documento
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_zoom_Click(object sender, System.EventArgs e)
		{
			string scriptString = null;
            //chk_showDoc.Checked = false;
			//btn_zoom.Enabled=false;

            //if (!ClientScript.IsStartupScriptRegistered("DocNoVisualizzato"))
            //{					
            //    scriptString = "<SCRIPT>top.left.location='SmistaDoc_DocNoVisualizzato.htm';</SCRIPT>";					
            //    //this.Page.RegisterStartupScript("DocNoVisualizzato", scriptString);
            //    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "DocNoVisualizzato", scriptString);
            //}	
			
			smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();
			DocsPaWR.DocumentoSmistamento docSmistamento=docManager.GetCurrentDocument(false);	// false perchè ora la visualizzazione è interna nel visualizzatore. 
		
			if (docSmistamento.ImmagineDocumento!=null)
			{
                if (!this.Page.IsStartupScriptRegistered("VisualizzazioneZoom"))
                {

                    if ((System.Configuration.ConfigurationManager.AppSettings["VIS_UNIFICATA"] != null) && (System.Configuration.ConfigurationManager.AppSettings["VIS_UNIFICATA"] == "1"))
                    {
                        //mod: gestione vis_unificata
                        DocsPAWA.DocsPaWR.SchedaDocumento sd = DocumentManager.getDettaglioDocumentoNoDataVista(this, docSmistamento.IDDocumento, docSmistamento.DocNumber);
                        DocumentManager.setDocumentoSelezionato(sd);
                        string targetName = "zoom";
                        scriptString = "<script language=JavaScript>%1%</script>";
                        string scriptBody = null;
                        scriptBody = "ApriFinestraZoom('../documento/docVisualizzaFrame.aspx?id=" + Session.SessionID + "&idprofile=" + docSmistamento.IDDocumento + "','" + targetName + "')";
                        scriptString = scriptString.Replace("%1%", scriptBody);

                        //scriptString = "<SCRIPT>dialogArguments.window.open('../documento/docVisualizzaFrame.aspx?id=" + Session.SessionID + "','','height=600,width=800,status=no,toolbar=no,menubar=no,location=no,resizable=yes,scrollbars=no,top=0,left=0,center=yes');</SCRIPT>";		
                    }
                    else scriptString = "<SCRIPT>dialogArguments.window.open('SmistaDoc_Visualizzazione.aspx','','height=600,width=800,status=no,toolbar=no,menubar=no,location=no,resizable=yes,scrollbars=no');</SCRIPT>";

                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "VisualizzazioneZoom", scriptString);
                    // this.Page.ClientScript.RegisterStartupScript(this.GetType(), "RitornaDaZoom", "top.left.location='../documento/docVisualizzaFrame.aspx?id=" + Session.SessionID + "&idprofile=" + docSmistamento.IDDocumento + "','""';", true);
                    //Response.Write("<SCRIPT>top.left.location='../documento/docVisualizzaFrame.aspx?id=" + Session.SessionID + "';</SCRIPT>");
                }
			}

            // gestione naviga UO
            this.GestVisibilitaFrecceNavigaUO();
		}

		/// <summary>
		/// Avanzamento navigazione
		/// </summary>
		private void ProgressStatus()
		{
			smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();
			
			if(docManager.GetDocumentCount() == 1)
			{
				smistaDoc.SmistaDocSessionManager.ReleaseSmistaDocManager();

                if (!ClientScript.IsStartupScriptRegistered("chiudiModalDialog"))
				{
                    string scriptString = "<SCRIPT>alert('Documenti da smistare terminati');window.returnValue = 'Y';window.close();</SCRIPT>";
                    this.ClientScript.RegisterStartupScript(this.GetType(), "chiudiModalDialog", scriptString);              
				}									
			}
			else
			{
				docManager.RemoveDocument();

				if(docManager.GetDocumentCount() == 1 || (docManager.GetCurrentDocumentPosition() > docManager.GetDocumentCount()))
				{
					docManager.MoveFirstDocument();
				}					
				else
				{
					docManager.MoveAbsoluteDocument(docManager.GetCurrentDocumentPosition());							
				}

				this.FillDataDocumentoTrasmesso();

				this.RefreshDocumentCounter();
			}           
		}


		/// <summary>
		/// Gestione script client per il check visualizza documento
		/// </summary>
		private void RegisterClientScriptCheckVisualizza()
		{
			string script="javascript:top.left.location='SmistaDoc_Waiting.htm';"; 
			
            if (Session["noDocSuccessivo"] != null && (bool)Session["noDocSuccessivo"])
                script = string.Empty;
			this.chk_showDoc.Attributes.Add("onclick",script);
		}

		/// <summary>
		/// Gestione script client per i pulsanti di navigazione
		/// </summary>
		private void RegisterClientScriptNavigationButtons()
		{
			string destinationPage=string.Empty;

			if (this.chk_showDoc.Checked)
				destinationPage="SmistaDoc_Waiting.htm";
			else
				destinationPage="SmistaDoc_DocNoVisualizzato.htm";

			string script="javascript:top.left.location='" + destinationPage + "';"; 

			this.btn_first.Attributes.Add("onclick",script);
			this.btn_previous.Attributes.Add("onclick",script);
			this.btn_next.Attributes.Add("onclick",script);
			this.btn_last.Attributes.Add("onclick",script);
		}


		/// <summary>
		/// Gestione script client per il pulsante di rifiuta
		/// </summary>
		private void RegisterClientScriptBtnRifiuta()
		{
            string script = "GetCommentoRifiuto()";

            smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();

            // Reperimento dello stato della trasmissione corrente
            // per impedire di rifiutare il documento qualora sia già stato accettato / rifiutato
            DocsPaWR.StatoTrasmissioneUtente statoTrasmissione = docManager.GetStatoTrasmissioneCorrente();

            if (statoTrasmissione.Accettata || statoTrasmissione.Rifiutata)
                script = string.Format("alert('Impossibile rifiutare il documento in quanto la trasmissione risulta già {0}'); return false;", (statoTrasmissione.Accettata ? "accettata" : "rifiutata"));
            else
                script = "GetCommentoRifiuto()";

			this.btn_rifiuta.Attributes.Add("onclick",script);
		}

		/// <summary>
		/// 
		/// </summary>
		private void RefreshDocumentCounter()
		{
			smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();
			lbl_contatore.Text = Convert.ToString(docManager.GetCurrentDocumentPosition()) + " / " + Convert.ToString(docManager.GetDocumentCount()); 
		}

		/// <summary>
		/// 
		/// </summary>
		private void EnabledNavigationButtons()
		{	
			smistaDoc.SmistaDocManager docManager=this.GetSmistaDocManager();
			int docIndex=docManager.GetCurrentDocumentPosition();
			int docLastIndex=docManager.GetDocumentCount();
				
			this.btn_previous.Enabled=(docIndex > 1);
			this.btn_next.Enabled=(docIndex < docLastIndex);
			this.btn_first.Enabled=this.btn_previous.Enabled;
			this.btn_last.Enabled=this.btn_next.Enabled;	
		
			if(this.btn_first.Enabled)	
			{
				this.btn_first.ImageUrl="../images/smistamento/FIRST.bmp";
				this.btn_previous.ImageUrl="../images/smistamento/PREV.bmp";
			}
			else
			{
				this.btn_first.ImageUrl="../images/smistamento/FIRST_disabled.bmp";
				this.btn_previous.ImageUrl="../images/smistamento/PREV_disabled.bmp";
			}
			
			if(this.btn_last.Enabled)	
			{
				this.btn_last.ImageUrl="../images/smistamento/LAST.bmp";
				this.btn_next.ImageUrl="../images/smistamento/NEXT.bmp";
			}
			else
			{
				this.btn_last.ImageUrl="../images/smistamento/LAST_disabled.bmp";
				this.btn_next.ImageUrl="../images/smistamento/NEXT_disabled.bmp";
			}
		}	

		/// <summary>
		/// Gestione visualizzione pannello pulsanti di navigazione
		/// </summary>
		private void ShowPanelNavigationButtons()
		{
			string docNumber=Request.QueryString["DOC_NUMBER"];
			pnl_navigationButtons.Visible=(docNumber==null || docNumber.Equals(string.Empty));
		}
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chk_mantieniSel_CheckedChanged(object sender, System.EventArgs e)
		{
		}

		/// <summary>
		/// Tasto pulisce selezioni effettuate
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_clearFlag_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            // Rimozione di tutte le selezioni effettuate nelle UO visualizzate
            this.ResetDetail();

            this.GestVisibilitaFrecceNavigaUO();
		}

		#endregion		

		#region Firma digitale

        /// <summary>
        /// Gestione abilitazione / disabilitazione pulsante per 
        /// la visualizzaione dei dettagli di firma del documento
        /// </summary>
		private void EnableButtonDettagliFirma()
		{
			DocsPaWR.DocumentoSmistamento docSmistamento=this.GetSmistaDocManager().GetCurrentDocument(false);

            bool buttonEnabled=false;

            if (docSmistamento.ImmagineDocumento != null)
            {
                if (docSmistamento.ImmagineDocumento.signatureResult != null)
                    // Se sono presenti i dettagli di firma, abilita il pulsante
                    buttonEnabled = true;

                else if (docSmistamento.ImmagineDocumento.name.ToLower().EndsWith(".p7m"))
                    // Se non sono presenti i dettagli di firma, si verifica l'estensione del
                    // nome del file, se ".p7m" abilita il pulsante
                    buttonEnabled = true;
            }

            this.btn_dettFirma.Enabled = buttonEnabled;
		}


		/// <summary>
		/// Impostazione contenuto file firmato digitalmente in sessione
		/// </summary>
		/// <param name="fileDocumento"></param>
		private void SetSignedDocumentOnSession(DocsPAWA.DocsPaWR.FileDocumento fileDocumento)
		{
			if (fileDocumento!=null)
				DocumentManager.SetSignedDocument(fileDocumento);
			else
				DocumentManager.RemoveSignedDocument();
		}

		private void chk_showDoc_CheckedChanged(object sender, System.EventArgs e)
		{
			this.ShowDocumentFile(false);

            // gestione naviga UO
            this.GestVisibilitaFrecceNavigaUO();
		}
		#endregion							      

        #region Gestione navigazione UO
         
        /// <summary>
        /// Imposta nell'inizializzazione dei dati il campo nascosto che indica
        ///  se è attiva la funzione di navigazione UO
        /// </summary>
        private void ImpostaHiddenNavigaUO()
        {
            if (IsEnabledNavigazioneUO())
                this.hdIsEnabledNavigaUO.Value = "1";
            else
                this.hdIsEnabledNavigaUO.Value = "0";
        }

        /// <summary>
        /// Verifica se è attiva la chiave SMISTA_NAVIGA_UO
        /// </summary>
        /// <returns></returns>
        private bool IsEnabledNavigazioneUO()
        {
            DocsPAWA.smistaDoc.SmistaDocManager docManager = new SmistaDocManager();
            return docManager.IsEnabledNavigazioneUO();
        }        

        private void grdUOApp_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            bool selezioneUoApp = false;
            bool selezioneUoInf = false;
            bool selezioneUtenteRuolo = false;
            switch (e.CommandName)
            {
                case "navigaUO_up":
                    string id = this.GetId(e.Item);

                    this.UpdateFlagUo(id, out selezioneUoApp, out selezioneUoInf, out selezioneUtenteRuolo);
                    this.navigaUO_up(id);

                    this.setFlagDataGrid();
                    
                    // Impostazione id UO parent
                    this.indxUoSel = this.getUOPadre(id);

                    break;              
            }
        }

        private void grdUOInf_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            bool selezioneUoApp = false;
            bool selezioneUoInf = false;
            bool selezioneUtenteRuolo = false;

            switch (e.CommandName)
            {
                case "navigaUO_down":
                    string id = this.GetId(e.Item);

                    //devo memorizzare gli utenti/ruoli selezionati della uo padre che ho selezionato
                    smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();
                    DocsPaWR.UOSmistamento uoAppartenenza = docManager.GetUOAppartenenza();                    
                    string idUOPadre = this.getUOPadre(id);
                    DocsPaWR.UOSmistamento uoSelezionata = this.getUoSelezionata(uoAppartenenza, id);

                    CheckBox chkComp = this.GetGridItemControl<CheckBox>(e.Item, "chkComp");
                    CheckBox chkCC = this.GetGridItemControl<CheckBox>(e.Item, "chkCC");

                    if (uoSelezionata.FlagConoscenza.Equals(chkCC.Checked) && 
                        uoSelezionata.FlagCompetenza.Equals(chkComp.Checked))
                        uoSelezionata.Selezionata = true;                         
                    else
                        uoSelezionata.Selezionata = false;                          
                    
                    DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    if(uoSelezionata.UoInferiori==null || uoSelezionata.UoInferiori.Length==0)
                        uoSelezionata.UoInferiori = ws.GetUOInferiori(id, docManager.getMittenteSmistamento());

                    this.UpdateFlagUo(idUOPadre, out selezioneUoApp, out selezioneUoInf, out selezioneUtenteRuolo);
                    this.navigaUO_down(id);
                    this.setFlagDataGrid();
                    this.indxUoSel = id;

                    if (this.hdCountNavigaDown.Value == "0")
                    {
                        this.hdCountNavigaDown.Value = "1";
                        uoSelezionata.Selezionata = true;                        
                    }

                    break;
            }
        }

        private void FillUOApp_NavigaUO(string idUO)
        {
            this.heightUoApp += 37;
            smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();
            docManager.FillCurrentUO_NavigaUO(idUO, UserManager.getRuolo(this.Page), UserManager.getUtente(this.Page), UserManager.getInfoUtente(this));
        
            DocsPaWR.UOSmistamento uoApp = docManager.GetUOAppartenenza();
            
            DocsPaWR.UOSmistamento uoSelezionata = this.getUoSelezionata(uoApp, idUO);

            if (uoSelezionata != null) // && uoSelezionata.Ruoli.Length > 0)
            {
                //luciani 3.10.3
                //smistaDoc.SmistaDocSessionManager.SetSessionUoApp(uoApp);

                this.grdUOApp.Visible = true;
                this.FillDataGridUOApp(uoSelezionata);
            }
            Session["UoSelezionata"] = uoSelezionata;
        }

        private bool FillUOInf_NavigaUO(string idUO)
        {
            bool retValue = false;
            smistaDoc.SmistaDocManager docManager = new SmistaDocManager();
            
            docManager.FillUOInf_NavigaUO(idUO, UserManager.getRuolo(this.Page), UserManager.getUtente(this.Page), UserManager.getInfoUtente(this));
           // DocsPaWR.UOSmistamento[] uoInferiori = docManager.GetUOInferiori();

            DocsPaWR.UOSmistamento uoSel = (DocsPaWR.UOSmistamento)Session["UoSelezionata"];
            DocsPaWR.UOSmistamento[] uoInferiori = uoSel.UoInferiori;

            if (uoInferiori != null && uoInferiori.Length > 0)
            {
                //luciani 3.10.3
                //smistaDoc.SmistaDocSessionManager.SetSessionUoInf(uoInferiori);
                this.heightUoInf = 25;
                this.grdUOInf.Visible = true;
                foreach (DocsPaWR.UOSmistamento tempUoInf in uoInferiori)
                {
                    if (tempUoInf.Descrizione.Length > 38)
                    {
                        this.heightUoInf += 36;
                    }
                    else
                    {
                        this.heightUoInf += 25;
                    }
                }
                
                this.FillDataGridUOInferiori(uoInferiori);
                retValue = true;
                if (heightUoApp >= 190)
                {
                    this.pnlContainerUoAppartenenza.Height = 190;
                }
                else
                {
                    this.pnlContainerUoAppartenenza.Height = this.heightUoApp;
                    this.pnlContainerUoInferiori.Height = 180 + (180 - this.heightUoApp);
                }

                if (heightUoInf < 180 && heightUoApp >= 190)
                {
                    this.pnlContainerUoAppartenenza.Height = Convert.ToInt32(this.pnlContainerUoAppartenenza.Height.Value) + (170 - this.heightUoInf);
                    this.pnlContainerUoInferiori.Height = heightUoInf;
                }
                else
                {
                    if (heightUoApp >= 190)
                    {
                        this.pnlContainerUoInferiori.Height = 170;
                    }
                }

            }
            else
            {
                this.pnlContainerUoInferiori.Height = 0;
                this.pnlContainerUoAppartenenza.Height = 340;
                this.grdUOInf.Visible = false;
            }	  
           
            return retValue;
        }

        private void navigaUO_down(string idUO)
        {
            // carica dati UO corrente
            this.FillUOApp_NavigaUO(idUO);
            // carica dati UO inferiori
            this.FillUOInf_NavigaUO(idUO);            
        }

        private void navigaUO_up(string idUO)
        {
            // carica dati UO corrente
            string idUOPadre = this.getUOPadre(idUO);
            this.FillUOApp_NavigaUO(idUOPadre);            
            // carica dati UO inferiori
            this.FillUOInf_NavigaUO(idUOPadre);                                   
        }

        private string getUOPadre(string idUO)
        {
            string id_Uo_padre = string.Empty;
            smistaDoc.SmistaDocManager docManager = new SmistaDocManager();
            ArrayList lista = new ArrayList();
            lista = docManager.ListaIDUOParent(idUO);
            if (lista.Count > 0)
                id_Uo_padre = lista[1].ToString();

            return id_Uo_padre;
        }

        private void GestVisibilitaFrecceNavigaUO()
        {
            this.GestVisibilitaFrecceNavigaUOApp();
            this.GestVisibilitaFrecceNavigaUOInf();
        }

        private void GestVisibilitaFrecceNavigaUOApp()
        {
            if (this.grdUOApp.Visible && this.grdUOApp.Items.Count > 0)
            {
                for (int i = 0; i < this.grdUOApp.Items.Count; i++)
                {                    
                    if (this.IsEnabledNavigazioneUO())
                    {
                        string tipo = this.GetTipoURP(this.grdUOApp.Items[i]);

                        if (tipo.Equals("U"))
                        {
                            this.grdUOApp.Items[i].Cells[3].Visible = true;
                            if (this.hdUOapp.Value.Equals(this.grdUOApp.Items[i].Cells[0].Text))
                                this.grdUOApp.Items[i].Cells[3].Text = "";                          
                        }
                        else
                            this.grdUOApp.Items[i].Cells[3].Text = "";
                    }
                    else
                        this.grdUOApp.Items[i].Cells[3].Visible = false;                    
                }
            }
        }

        private void GestVisibilitaFrecceNavigaUOInf()
        {
            if (this.grdUOInf.Visible && this.grdUOInf.Items.Count > 0)
            {
                for (int i = 0; i < this.grdUOInf.Items.Count; i++)
                {
                    if (!this.IsEnabledNavigazioneUO())
                        this.grdUOInf.Items[i].Cells[3].Visible = false;
                }
            }
        }

        private void FillGridDestinatariDefault()
        {
            this.heightUoApp = 0;
            this.heightUoApp = 37;

            smistaDoc.SmistaDocManager docManager = new SmistaDocManager();
            docManager.FillDestinatariDefault(UserManager.getRuolo(this.Page), UserManager.getUtente(this.Page), UserManager.getInfoUtente(this));

            DocsPaWR.UOSmistamento uoApp = docManager.GetUOAppartenenza();

            // Caricamento griglia uo di appartenenza
            if (uoApp != null) // && uoApp.Ruoli.Length > 0)
            {
                //luciani 3.10.3
                smistaDoc.SmistaDocSessionManager.SetSessionUoApp(uoApp);
                this.grdUOApp.Visible = true;
                this.FillDataGridUOApp(uoApp);

                // Impostazione dell'indice dell'UO di appartenenza
                this.indxUoSel = uoApp.ID;
            }
            else
            {
                this.grdUOApp.Visible = false;

                this.indxUoSel = string.Empty;
            }

            // Caricamento griglia uo inferiori
            DocsPaWR.UOSmistamento[] uoInferiori = docManager.GetUOInferiori();

            if (uoInferiori != null && uoInferiori.Length > 0)
            {
                this.grdUOInf.Visible = true;
                this.heightUoInf = 25;

                foreach (DocsPaWR.UOSmistamento tempUoInf in uoInferiori)
                {
                    if (tempUoInf.Descrizione.Length > 38)
                    {
                        this.heightUoInf += 36;
                    }
                    else
                    {
                        this.heightUoInf += 25;
                    }
                }

                this.FillDataGridUOInferiori(uoInferiori);
                if (heightUoApp >= 190)
                {
                    this.pnlContainerUoAppartenenza.Height = 190;
                }
                else
                {
                    this.pnlContainerUoAppartenenza.Height = this.heightUoApp;
                    this.pnlContainerUoInferiori.Height = 180 + (180 - this.heightUoApp);
                }

                if (heightUoInf < 180 && heightUoApp >= 190)
                {
                    this.pnlContainerUoAppartenenza.Height = Convert.ToInt32(this.pnlContainerUoAppartenenza.Height.Value) + (170 - this.heightUoInf);
                    this.pnlContainerUoInferiori.Height = heightUoInf;
                }
                else
                {
                    if (heightUoApp >= 190)
                    {
                        this.pnlContainerUoInferiori.Height = 170;
                    }
                }
            }
            else
            {
                this.pnlContainerUoInferiori.Height = 0;
                this.pnlContainerUoAppartenenza.Height = 340;
            }	
        }

        private bool existNavigaUOInf(string idUO)
        {
            bool retValue = false;

            try
            {
                int intID = Convert.ToInt32(idUO);
            
                smistaDoc.SmistaDocManager docManager = new SmistaDocManager();

                #region prima versione (risultata lenta)
                //docManager.FillUOInf_NavigaUO(idUO, UserManager.getRuolo(this.Page), UserManager.getUtente(this.Page), UserManager.getInfoUtente(this));
                //DocsPaWR.UOSmistamento[] uoInferiori = docManager.GetUOInferiori();
                //retValue = (uoInferiori != null && uoInferiori.Length > 0);    
                #endregion

                // seconda versione
                retValue = docManager.ExistUoInf(idUO, UserManager.getRuolo(this.Page), UserManager.getUtente(this.Page), UserManager.getInfoUtente(this));
            }
            catch
            {
                retValue = false;
            }

            return retValue;
        }

        private void AlertNoUOInf()
        {
            ClientScript.RegisterStartupScript(this.GetType(), "openAlertNoUOInf", "<script>alert('Impossibile passare alla UO inferiore')</script>");            
        }

        private void RipristinaFrecce_UOApp()
        {
            string type = string.Empty;

            foreach (DataGridItem grdItem in this.grdUOApp.Items)
            {
                type = this.GetTipoURP(grdItem);

                if (type.Equals("U"))
                {
                    grdItem.Cells[3].Visible = true;
                    if (this.hdUOapp.Value.Equals(this.GetId(grdItem)))
                        grdItem.Cells[3].Text = "";
                }
                else
                    grdItem.Cells[3].Text = "";              
            }     
        }

        #endregion               

        #region Fascicolazione rapida

        private void GestioneFascicolazione()
        {
            this.pnl_fasc_rapida.Visible = false;

            if (this.IsEnabledFascicolazione())
            {
                this.pnl_fasc_rapida.Visible = true;
                this.ImpostaCodDescFascicolazione();
                this.GestVisibilitaFrecceNavigaUO();
            }
        }

        private bool IsEnabledFascicolazione()
        {
            return UserManager.ruoloIsAutorized(this, "FASC_SMISTA");
        }

        private void ResetFascicolazione()
        {
            FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
            this.txt_CodFascicolo.Text = "";
            this.txt_DescFascicolo.Text = "";
        }

        private void ImpostaCodDescFascicolazione()
        {

           DocsPaWR.Fascicolo fascRap = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
           string codiceFascRapida = FascicoliManager.getCodiceFascRapida(this);
           string descrFascRapida = FascicoliManager.getDescrizioneFascRapida(this);

           if (fascRap != null)
           {
              if (fascRap.folderSelezionato != null && codiceFascRapida != string.Empty && descrFascRapida != string.Empty)
              {
                 //this.txt_CodFascicolo.Text = fascRap.codice + "//" + fascRap.folderSelezionato.descrizione;
                 //this.txt_DescFascicolo.Text = fascRap.folderSelezionato.descrizione;
                 this.txt_CodFascicolo.Text = codiceFascRapida;
                 this.txt_DescFascicolo.Text = descrFascRapida;
              }
              else
              {
                 this.txt_CodFascicolo.Text = fascRap.codice;
                 this.txt_DescFascicolo.Text = fascRap.descrizione;
              }
           }
           else
           {

             // this.txt_CodFascicolo.Text = "";
              this.txt_DescFascicolo.Text = "";
           }

           //setto la tooltip del fascicolo
           this.txt_DescFascicolo.ToolTip = txt_DescFascicolo.Text;


            //DocsPAWA.DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
            //if (fasc != null)
            //{
            //    this.txt_CodFascicolo.Text = fasc.codice;
            //    this.txt_DescFascicolo.Text = fasc.descrizione;
            //}
        }

        protected void txt_CodFascicolo_TextChanged(object sender, EventArgs e)
        {
            //inizialmente svuoto il campo e pulisco la sessione
            FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
            this.txt_DescFascicolo.Text = "";

            if (this.txt_CodFascicolo.Text.Trim().Equals(""))
            {
                txt_DescFascicolo.Text = "";
                return;
            }

            DocsPaWR.Registro registro = this.GetRegistroCorrente();

            if (registro != null && !string.IsNullOrEmpty(registro.systemId))
            {
                //Fascicolazione rapida in sottofascicoli
                if (this.txt_CodFascicolo.Text.IndexOf("//") > -1)
                {
                    #region FASCICOLAZIONE IN SOTTOFASCICOLI
                    string codice = string.Empty;
                    string descrizione = string.Empty;
                    DocsPaWR.Fascicolo SottoFascicolo = getFolder(UserManager.getRegistroSelezionato(this), ref codice, ref descrizione);
                    if (SottoFascicolo != null)
                    {

                        if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                        {
                            txt_DescFascicolo.Text = descrizione;
                            txt_CodFascicolo.Text = codice;
                            FascicoliManager.setCodiceFascRapida(this, codice);
                            FascicoliManager.setDescrizioneFascRapida(this, descrizione);
                            FascicoliManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);

                        }
                        else
                        {
                            //caso 0: al codice digitato non corrisponde alcun fascicolo

                            Session["validCodeFasc"] = "false";
                            Page.RegisterStartupScript("", "<script>alert('Attenzione, sottofascicolo non presente')</script>");
                            this.txt_DescFascicolo.Text = "";
                            this.txt_CodFascicolo.Text = "";

                        }

                    }
                    else
                    {
                        Session["validCodeFasc"] = "false";
                        Page.RegisterStartupScript("", "<script>alert('Attenzione, sottofascicolo non presente')</script>");
                        this.txt_DescFascicolo.Text = "";
                        this.txt_CodFascicolo.Text = "";
                    }


                    #endregion
                }
                else
                {
                    #region fascicolazione rapida in fascicoli
                    // registro noto: doc. protocollato
                    DocsPAWA.DocsPaWR.Fascicolo[] listaFasc = FascicoliManager.getListaFascicoliDaCodice(this, this.txt_CodFascicolo.Text.Trim(), registro, "I");

                    if (listaFasc != null)
                    {
                        if (listaFasc.Length > 0)
                        {
                            //caso 1: al codice digitato corrisponde un solo fascicolo
                            if (listaFasc.Length == 1)
                            {
                                txt_DescFascicolo.Text = listaFasc[0].descrizione;
                                if (listaFasc[0].tipo.Equals("G"))
                                {
                                    codClassifica = listaFasc[0].codice;
                                }
                                else
                                {
                                    //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                                    DocsPAWA.DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                    string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                    codClassifica = codiceGerarchia;
                                }
                                FascicoliManager.setFascicoloSelezionatoFascRapida(this, listaFasc[0]);
                            }
                            else
                            {
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
                                    DocsPAWA.DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, listaFasc[0].idClassificazione, UserManager.getUtente(this).idAmministrazione);
                                    string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                                    codClassifica = codiceGerarchia;
                                }
                                ClientScript.RegisterStartupScript(this.GetType(), "openModale", "<script>ApriRicercaFascicoli('" + codClassifica + "', 'Y')</script>");
                                return;
                            }
                        }
                        else
                        {
                            this.AlertFascicoloNonPresente();
                        }
                    }
                    #endregion
                }
            }
            else
            {
                if (this.txt_CodFascicolo.Text.IndexOf("//") > -1)
                {
                    #region fascicolazione rapida in sottofascicoli
                    string codice = string.Empty;
                    string descrizione = string.Empty;

                    DocsPaWR.Fascicolo SottoFascicolo = getFolder(null, ref codice, ref descrizione);
                    if (SottoFascicolo != null)
                    {
                        if (SottoFascicolo.folderSelezionato != null && codice != string.Empty && descrizione != string.Empty)
                        {
                            txt_DescFascicolo.Text = descrizione;
                            txt_CodFascicolo.Text = codice;
                            FascicoliManager.setCodiceFascRapida(this, codice);
                            FascicoliManager.setDescrizioneFascRapida(this, descrizione);
                            FascicoliManager.setFascicoloSelezionatoFascRapida(this, SottoFascicolo);

                        }
                        else
                        {
                            //caso 0: al codice digitato non corrisponde alcun fascicolo

                            Session["validCodeFasc"] = "false";
                            Page.RegisterStartupScript("", "<script>alert('Attenzione, sottofascicolo non presente')</script>");
                            this.txt_DescFascicolo.Text = "";
                            this.txt_CodFascicolo.Text = "";

                        }
                    }
                    else
                    {
                        Session["validCodeFasc"] = "false";
                        Page.RegisterStartupScript("", "<script>alert('Attenzione, sottofascicolo non presente')</script>");
                        this.txt_DescFascicolo.Text = "";
                        this.txt_CodFascicolo.Text = "";
                    }
                    #endregion
                }
                else
                {
                    // senza registro: doc. grigio
                    //old: DocsPAWA.DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicoloDaCodice(this, this.txt_CodFascicolo.Text.Trim());
                    DocsPAWA.DocsPaWR.Fascicolo fascicolo = null;
                    DocsPAWA.DocsPaWR.Fascicolo[] listaFasc = FascicoliManager.getListaFascicoliDaCodice(this, this.txt_CodFascicolo.Text.Trim(), registro, "I");
                    if (listaFasc != null && listaFasc.Length > 0 && listaFasc[0] != null)
                    {
                        fascicolo = (DocsPAWA.DocsPaWR.Fascicolo)listaFasc[0];
                    }
                    if (fascicolo != null)
                    {
                        FascicoliManager.setFascicoloSelezionatoFascRapida(this, fascicolo);
                        this.ImpostaCodDescFascicolazione();
                    }
                    else
                    {
                        this.AlertFascicoloNonPresente();
                    }
                }
            }
        }

        /// <summary>
        /// Metodo per il recupero del fascicolo da codice
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo getFascicolo()
        {
            DocsPaWR.Fascicolo fascicoloSelezionato = null;
            string codiceFascicolo = string.Empty;
            string descrizione = string.Empty;
            DocsPaWR.Registro registro = this.GetRegistroCorrente();
            if (!this.txt_CodFascicolo.Text.Equals(""))
            {
                if (this.txt_CodFascicolo.Text.IndexOf("//") > -1)
                {
                    fascicoloSelezionato = getFolder(UserManager.getRegistroSelezionato(this), ref codiceFascicolo, ref descrizione);
                }
                else
                {
                    codiceFascicolo = txt_CodFascicolo.Text;

                    //OLD: fascicoloSelezionato = FascicoliManager.getFascicoloDaCodice(this, codiceFascicolo);

                    DocsPaWR.Fascicolo[] FascS = FascicoliManager.getListaFascicoliDaCodice(this, codiceFascicolo, registro, "I");

                    if (FascS != null && FascS.Length > 0 && FascS[0] != null)
                    {
                        fascicoloSelezionato = (DocsPaWR.Fascicolo)FascS[0];
                    }
                }
            }
            if (fascicoloSelezionato != null)
            {
                return fascicoloSelezionato;
            }
            else
            {
                return null;
            }
        }

        protected void btn_cerca_fasc_Click(object sender, ImageClickEventArgs e)
        {
            if (this.txt_CodFascicolo.Text.Trim() != "")
            {
               // DocsPAWA.DocsPaWR.Fascicolo fascSel = FascicoliManager.getFascicoloDaCodice(this, this.txt_CodFascicolo.Text.Trim());
               DocsPAWA.DocsPaWR.Fascicolo fascSel = getFascicolo();
                if (fascSel != null)
                {
                    FascicoliManager.setFascicoloSelezionatoFascRapida(this, fascSel);
                    if (fascSel.tipo.Equals("G"))
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "openModale", "<script>ApriRicercaFascicoli('" + fascSel.codice + "')</script>");
                    }
                    else
                    {
                        //se il fascicolo è procedimentale, ricerco la classifica a cui appartiene
                        DocsPAWA.DocsPaWR.FascicolazioneClassifica[] gerClassifica = FascicoliManager.getGerarchia(this, fascSel.idClassificazione, UserManager.getUtente(this).idAmministrazione);
                        string codiceGerarchia = gerClassifica[gerClassifica.Length - 1].codice;
                        ClientScript.RegisterStartupScript(this.GetType(), "openModale", "<script>ApriRicercaFascicoli('" + codiceGerarchia + "')</script>");
                    }
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "openModale", "<script>ApriRicercaFascicoli('" + txt_CodFascicolo.Text.Trim() + "')</script>");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DocsPaWR.Registro GetRegistroCorrente()
        {
            smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();
            DocsPaWR.DocumentoSmistamento docsmista= docManager.GetCurrentDocument(false);

            if (docsmista.TipoDocumento.Equals("A") || docsmista.TipoDocumento.Equals("P"))
            {
                return new DocsPAWA.DocsPaWR.Registro
                {
                    systemId = docsmista.IDRegistro
                };
            }
            else
                return null;
        }

        private void AlertFascicoloNonPresente()
        {
            FascicoliManager.removeFascicoloSelezionatoFascRapida(this);
            ClientScript.RegisterStartupScript(this.GetType(), "openAlertNoFound", "<script>alert('Attenzione, il codice fascicolo non è presente')</script>");
            this.txt_DescFascicolo.Text = "";
            this.txt_CodFascicolo.Text = "";            
        }

        private void AlertErroreFascicolazione()
        {
            ClientScript.RegisterStartupScript(this.GetType(), "openAlertErrFasc", "<script>alert('Attenzione, il documento risulta già classificato nel fascicolo selezionato')</script>");            
        }

        private void FascicolaDocumento()
        {
            if (this.IsEnabledFascicolazione())
            {
                smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();
                DocsPaWR.DocumentoSmistamento docsmista = docManager.GetCurrentDocument(false);
                DocsPAWA.DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicoloSelezionatoFascRapida(this);
                if (fascicolo != null)
                {
                    DocsPAWA.DocsPaWR.Folder folder= null;
                    if(fascicolo.folderSelezionato!=null)
                        folder = fascicolo.folderSelezionato;
                    else
                        folder = FascicoliManager.getFolder(this, fascicolo);

                    if (folder != null)
                    {
                        string idFolder = folder.systemID;
                        string idDoc = docsmista.IDDocumento;
                        bool isInFolder = false;
                        String message = String.Empty;
                        bool docInserted = DocumentManager.addDocumentoInFolder(this, idDoc, idFolder, false, out isInFolder, out message);

                        if (!docInserted)
                        this.AlertErroreFascicolazione();
                    }                    
                }
            }
        }       

        /// <summary>
        /// Metodo per il recupero del sottofascicolo da codice fascicolo e descrizione sottofascicolo
        /// </summary>
        /// <param name="registro"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.Fascicolo getFolder(DocsPAWA.DocsPaWR.Registro registro, ref string codice, ref string descrizione)
        {
           DocsPaWR.Folder[] listaFolder = null;
           DocsPaWR.Fascicolo fasc = null;
           string separatore = "//";
           int posSep = this.txt_CodFascicolo.Text.IndexOf("//");
           if (this.txt_CodFascicolo.Text != string.Empty && posSep > -1)
           {

              string codiceFascicolo = txt_CodFascicolo.Text.Substring(0, posSep);
              string descrFolder = txt_CodFascicolo.Text.Substring(posSep + separatore.Length);

              listaFolder = FascicoliManager.getListaFolderDaCodiceFascicolo(this, codiceFascicolo, descrFolder, registro);
              if (listaFolder != null && listaFolder.Length > 0)
              {

                 //calcolo fascicolazionerapida
                 fasc = FascicoliManager.getFascicoloById(this, listaFolder[0].idFascicolo);

                 if (fasc != null)
                 {
                    //folder selezionato è l'ultimo
                    fasc.folderSelezionato = listaFolder[listaFolder.Length - 1];
                 }
                 codice = fasc.codice + separatore;
                 descrizione = fasc.descrizione + separatore;
                 for (int i = 0; i < listaFolder.Length; i++)
                 {
                    codice += listaFolder[i].descrizione + "/";
                    descrizione += listaFolder[i].descrizione + "/";
                 }
                 codice = codice.Substring(0, codice.Length - 1);
                 descrizione = descrizione.Substring(0, descrizione.Length - 1);

              }

           }
           if (fasc != null)
           {

              return fasc;

           }
           else
           {
              return null;
           }
        }

        protected void btn_titolario_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ////E' necessario che sia selezionato un titolario e non la voce "tutti i titolari"
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            ArrayList listaTitolari = new ArrayList(wws.getTitolariUtilizzabili(UserManager.getUtente(this).idAmministrazione));
            string idTitolario = this.getIdTitolario("", listaTitolari);
            //FascicoliManager.removeFascicoloSelezionatoFascRapida();
            if (!string.IsNullOrEmpty(idTitolario))
            {
                if (!this.IsStartupScriptRegistered("apriModalDialog"))
                {
                    //string scriptString = "<SCRIPT>ApriTitolario('codClass=" + txt_codClass.Text + "&idTit=" + ddl_titolari.SelectedValue + "','gestClass')</SCRIPT>";
                    string scriptString = "<SCRIPT>ApriTitolario2('codClass=" + string.Empty + "&idTit=" + idTitolario + "','gestTodolist')</SCRIPT>";
                    this.RegisterStartupScript("apriModalDialog", scriptString);
                }
            }
            else
            {
                string script = "<SCRIPT>alert('Non è presente nessun titolario attivo');</SCRIPT>";
                this.RegisterStartupScript("alert", script);
            }
        }

        private string getIdTitolario(string codClassificazione, ArrayList titolari)
        {
            string result = string.Empty;
            foreach (DocsPaWR.OrgTitolario titolario in titolari)
            {
                if (titolario.Stato == DocsPAWA.DocsPaWR.OrgStatiTitolarioEnum.Attivo)
                {
                    result = titolario.ID;
                }
            }
            return result;
        }
        #endregion

        private DocsPaWR.UOSmistamento getUoSelezionata(DocsPaWR.UOSmistamento uo, string id)
        {
            DocsPaWR.UOSmistamento uoSel = new DocsPAWA.DocsPaWR.UOSmistamento();
            if (uo.ID == id)
            {
                uoSel = uo;
            }
            else
            {
                if (uo.UoInferiori != null && uo.UoInferiori.Length > 0)
                {
                    foreach (DocsPaWR.UOSmistamento uoInf in uo.UoInferiori)
                    {
                        if (string.IsNullOrEmpty(uoSel.ID))
                        {
                            uoSel = getUoSelezionata(uoInf, id);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return uoSel;
        }

        private void UpdateFlagUo(string idUo, out bool selezioneUoApp, out  bool selezioneUoInf, out bool selezioneUtenteRuolo)
        {
            smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();

            DocsPaWR.UOSmistamento uo = this.getUoSelezionata(docManager.GetUOAppartenenza(), idUo);

            this.UpdateFlagUOAppartenenza(uo, out selezioneUoApp);

            this.UpdateFlagUOInferiori(uo.UoInferiori, out selezioneUoInf);

            this.UpadateFlagUtentiRuoloSelezionato(uo, out selezioneUtenteRuolo);
            
        }

        /// <summary>
        /// 
        /// </summary>
        private void setFlagDataGrid()
        {
            DocsPaWR.UOSmistamento uoSelezionata = (DocsPaWR.UOSmistamento)Session["UoSelezionata"];

            int i = 0;

            if (uoSelezionata.Ruoli != null && uoSelezionata.Ruoli.Length > 0)
            {
                foreach (DocsPaWR.RuoloSmistamento ruolo in uoSelezionata.Ruoli)
                {
                    if (ruolo.Utenti != null && ruolo.Utenti.Length > 0)
                    {
                        i++;

                        CheckBox chkComp = this.GetGridItemControl<CheckBox>(this.grdUOApp.Items[i], "chkComp");
                        CheckBox chkCC = this.GetGridItemControl<CheckBox>(this.grdUOApp.Items[i], "chkCC");
                        CheckBox chkNotifica = this.GetGridItemControl<CheckBox>(this.grdUOApp.Items[i], "chkNotifica");
                        ImageButton imgNote = this.GetGridItemControl<ImageButton>(this.grdUOApp.Items[i], "imgNote");

                        chkComp.Checked = ruolo.FlagCompetenza;
                        chkCC.Checked = ruolo.FlagConoscenza;
                        
                        chkNotifica.Checked = false;
                        chkNotifica.Enabled = (ruolo.FlagCompetenza || ruolo.FlagConoscenza);
                        imgNote.Visible = (ruolo.FlagCompetenza || ruolo.FlagConoscenza);
                        
                        foreach (DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                        {
                            i++;

                            bool compEnabledPerGerarchia = this.IsEnabledOptionCompPerGerarchia(this.grdUOApp.Items[i]);
                            bool ccEnabledPerGerarchia = this.IsEnabledOptionCCPerGerarchia(this.grdUOApp.Items[i]);

                            chkComp = this.GetGridItemControl<CheckBox>(this.grdUOApp.Items[i], "chkComp");
                            chkCC = this.GetGridItemControl<CheckBox>(this.grdUOApp.Items[i], "chkCC");
                            chkNotifica = this.GetGridItemControl<CheckBox>(this.grdUOApp.Items[i], "chkNotifica");
                            imgNote = this.GetGridItemControl<ImageButton>(this.grdUOApp.Items[i], "imgNote");

                            if (ruolo.FlagCompetenza || ruolo.FlagConoscenza)
                                // Se è stata fatta una selezione a livello di ruolo,
                                // il check per l'utente non è impostato
                                chkComp.Checked = false;
                            else
                                chkComp.Checked = utente.FlagCompetenza;

                            chkComp.Enabled = compEnabledPerGerarchia && (!ruolo.FlagCompetenza && !ruolo.FlagConoscenza);

                            if (ruolo.FlagCompetenza || ruolo.FlagConoscenza)
                                // Se è stata fatta una selezione a livello di ruolo,
                                // il check per l'utente non è impostato
                                chkCC.Checked = false;
                            else                                
                                chkCC.Checked = utente.FlagConoscenza;

                            chkCC.Enabled = ccEnabledPerGerarchia && (!ruolo.FlagCompetenza && !ruolo.FlagConoscenza);

                            // Flag di notifica checked solo se per l'utente è stata 
                            // effettuata una selezione
                            chkNotifica.Checked = (utente.FlagCompetenza || utente.FlagConoscenza);
                            chkNotifica.Enabled = (ruolo.FlagCompetenza || ruolo.FlagConoscenza);

                            imgNote.Visible = (utente.FlagCompetenza || utente.FlagConoscenza) &&
                                              (!ruolo.FlagCompetenza && !ruolo.FlagConoscenza);
                        }
                    }
                }
            }

            i = 0;

            if (uoSelezionata.UoInferiori != null && uoSelezionata.UoInferiori.Length > 0)
            {
                foreach (DocsPaWR.UOSmistamento uo in uoSelezionata.UoInferiori)
                {
                    CheckBox chkComp = this.GetGridItemControl<CheckBox>(this.grdUOInf.Items[i], "chkComp");
                    CheckBox chkCC = this.GetGridItemControl<CheckBox>(this.grdUOInf.Items[i], "chkCC");
                    ImageButton imgNote = this.GetGridItemControl<ImageButton>(this.grdUOInf.Items[i], "imgNote");

                    chkComp.Checked = uo.FlagCompetenza;
                    chkCC.Checked = uo.FlagConoscenza;
                    imgNote.Visible = (uo.FlagCompetenza || uo.FlagConoscenza);

                    i++;
                }
            }
        }

        private void SetFlagVisualizzaDocumento()
        {
            string valoreChiave;
            valoreChiave = utils.InitConfigurationKeys.GetValue("0", "FE_VISUAL_DOC_SMISTAMENTO");
            if (valoreChiave.Equals("1"))
            {
                this.chk_showDoc.Checked = true;
                this.chk_showDoc_CheckedChanged(this.chk_showDoc, new EventArgs());
            }
        }

        private void navigaUoSelezionata(string idUo)
        {
            bool selezioneUoApp = false;
            bool selezioneUoInf = false;
            bool selezioneUtenteRuolo = false;

            smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();
            DocsPaWR.UOSmistamento uoAppartenenza = docManager.GetUOAppartenenza();
            string idUOPadre = this.getUOPadre(idUo);
            DocsPaWR.UOSmistamento uoSelezionata = this.getUoSelezionata(uoAppartenenza, idUo);

            uoSelezionata.Selezionata = true;
            
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            if (uoSelezionata.UoInferiori == null || uoSelezionata.UoInferiori.Length == 0)
                uoSelezionata.UoInferiori = ws.GetUOInferiori(idUo, docManager.getMittenteSmistamento());
            if (uoSelezionata.ID == uoAppartenenza.ID)
                idUOPadre = uoSelezionata.ID;
            this.UpdateFlagUo(idUOPadre, out selezioneUoApp, out selezioneUoInf, out selezioneUtenteRuolo);
            this.navigaUO_down(idUo);
            this.setFlagDataGrid();
            this.indxUoSel = idUo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_selezioniSmistamento_Click(object sender, ImageClickEventArgs e)
        {
            bool selezioneUoApp = false;
            bool selezioneUoInf = false;
            bool selezioneUtenteRuolo = false;

            this.UpdateFlagUo(this.indxUoSel, out selezioneUoApp, out selezioneUoInf, out selezioneUtenteRuolo);

            if (!this.ClientScript.IsStartupScriptRegistered(this.GetType(), "selezioniSmistamento"))
                this.ClientScript.RegisterStartupScript(this.GetType(), "selezioniSmistamento", "ApriSelezioneSmistamento();", true);
        }

        //private void execTrasmRapida()
        //{
        //    int indexSel = ddl_trasm_rapida.Items.IndexOf(ddl_trasm_rapida.Items.FindByValue(separatore));
        //    DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato();
        //    DocsPaWR.DocsPaWebService wws = new DocsPaWR.DocsPaWebService();

        //    if (this.ddl_trasm_rapida.SelectedIndex > 0 && ddl_trasm_rapida.SelectedIndex < indexSel)
        //    {
        //        if (Session["Modello"] != null)
        //        {
        //            DocsPaWR.ModelloTrasmissione modello = (DocsPAWA.DocsPaWR.ModelloTrasmissione)Session["Modello"];
        //            DocsPaWR.Trasmissione trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();

        //            //Parametri della trasmissione
        //            trasmissione.noteGenerali = modello.VAR_NOTE_GENERALI;
        //            trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO;
        //            trasmissione.infoDocumento = DocumentManager.getInfoDocumento(schedaDocumento);
        //            trasmissione.utente = UserManager.getUtente(this);
        //            trasmissione.ruolo = UserManager.getRuolo(this);

        //            if (modello != null)
        //                trasmissione.NO_NOTIFY = modello.NO_NOTIFY;
        //            //Parametri delle trasmissioni singole
        //            for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
        //            {
        //                DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
        //                ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
        //                for (int j = 0; j < destinatari.Count; j++)
        //                {
        //                    DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
        //                    DocsPaWR.Corrispondente corr = new DocsPaWR.Corrispondente();
        //                    //old: ritoranva anche i corr diasbilitati DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteByCodRubricaIE(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
        //                    if (mittDest.CHA_TIPO_MITT_DEST == "D")
        //                        corr = UserManager.getCorrispondenteByCodRubricaIENotdisabled(this, mittDest.VAR_COD_RUBRICA, DocsPaWR.AddressbookTipoUtente.INTERNO);
        //                    else
        //                        corr = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, schedaDocumento, null, this);

        //                    if (corr != null)
        //                    {   //il corr è null se non esiste o se è stato disabiltato.    
        //                        DocsPaWR.RagioneTrasmissione ragione =  wws.getRagioneById(mittDest.ID_RAGIONE.ToString());

        //                        //Andrea - try - catch
        //                        try
        //                        {
        //                            trasmissione = TrasmManager.addTrasmissioneSingola(trasmissione, corr, ragione, mittDest.VAR_NOTE_SING, mittDest.CHA_TIPO_TRASM, mittDest.SCADENZA, mittDest.NASCONDI_VERSIONI_PRECEDENTI, this);
        //                        }
        //                        catch (ExceptionTrasmissioni e)
        //                        {
        //                            listaExceptionTrasmissioni.Add(e.Messaggio);
        //                        }
        //                        //End Andrea
        //                    }
        //                }
        //            }
        //            //Andrea
        //            foreach (string s in listaExceptionTrasmissioni)
        //                messError = messError + s + "\\n";

        //            if (messError != "")
        //                Session.Add("MessError", messError);
        //            //End Andrea

        //            DocsPaWR.Trasmissione t_rs = null;
        //            if (trasmissione.trasmissioniSingole != null && trasmissione.trasmissioniSingole.Length > 0)
        //            {
        //                trasmissione = this.impostaNotificheUtentiDaModello(trasmissione);

        //                if (estendiVisibilita.Value == "false")
        //                {
        //                    TrasmissioneSingola[] appoTrasmSingole = new TrasmissioneSingola[trasmissione.trasmissioniSingole.Length];
        //                    for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
        //                    {
        //                        TrasmissioneSingola trasmSing = new TrasmissioneSingola();
        //                        trasmSing = trasmissione.trasmissioniSingole[i];
        //                        trasmSing.ragione.eredita = "0";
        //                        appoTrasmSingole[i] = trasmSing;
        //                    }
        //                    trasmissione.trasmissioniSingole = appoTrasmSingole;
        //                }

        //                DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
        //                if (infoUtente.delegato != null)
        //                    trasmissione.delegato = ((DocsPAWA.DocsPaWR.InfoUtente)(infoUtente.delegato)).idPeople;

        //                //Nuovo metodo saveExecuteTrasm

        //                //Modifica per cessione diritti

        //                if (this.cessioneDirittiAbilitato(trasmissione, modello))
        //                {
        //                    bool aperturaPopUpSceltaNuovoProprietario = false;

        //                    // verifica se esistono ruoli tra i destinatari
        //                    this.verificaRuoliDestInTrasmissione(trasmissione);

        //                    switch (this.numeroRuoliDestInTrasmissione)
        //                    {
        //                        case 0:
        //                            // non ci sono ruoli tra i destinatari! avvisa...
        //                            this.inviaMsgNoRuoli();
        //                            return;
        //                            break;

        //                        case 1:
        //                            // ce n'è 1, verifica se un solo utente del ruolo ha la notifica...
        //                            this.utentiConNotifica(trasmissione);
        //                            if (this.numeroUtentiConNotifica > 1)
        //                                aperturaPopUpSceltaNuovoProprietario = true;
        //                            else
        //                            {
        //                                // 1 solo utente con notifica, il sistema ha già memorizzato il suo id_people...
        //                                trasmissione.cessione.idPeopleNewPropr = this.idPeopleNewOwner;
        //                                trasmissione.cessione.idRuoloNewPropr = ((DocsPAWA.DocsPaWR.Ruolo)trasmissione.trasmissioniSingole[0].corrispondenteInterno).idGruppo;

        //                                modello.CEDE_DIRITTI = "1";
        //                                modello.ID_PEOPLE_NEW_OWNER = trasmissione.cessione.idPeopleNewPropr;
        //                                modello.ID_GROUP_NEW_OWNER = trasmissione.cessione.idRuoloNewPropr;
        //                            }
        //                            break;

        //                        default:
        //                            // ce ne sono + di 1, quindi ne fa scegliere uno...                                    
        //                            aperturaPopUpSceltaNuovoProprietario = true;
        //                            break;
        //                    }
        //                }

        //                if (this.verificaNotificheUtentiDaModello(trasmissione))
        //                    t_rs = TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
        //                else
        //                    Response.Write("<script>window.alert('Trasmissione non effettuata in quanto il modello di trasmissione utilizzato prevede dei ruoli per i quali non sono presenti utenti con notifica. \\n Ricontrollare il modello.');</script>");
        //                //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
        //                //t_rs = TrasmManager.executeTrasm(this, trasmissione);
        //            }
        //            if (t_rs != null && t_rs.ErrorSendingEmails)
        //                //Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");
        //                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "No_inoltro", "window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');", true);

        //            //Salvataggio del system_id della trasm per il cambio di stato automatico
        //            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
        //            {
        //                DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
        //                bool trasmWF = false;
        //                if (trasmissione != null && trasmissione.trasmissioniSingole != null)
        //                    for (int i = 0; i < trasmissione.trasmissioniSingole.Length; i++)
        //                    {
        //                        DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
        //                        if (trasmSing.ragione.tipo == "W")
        //                            trasmWF = true;
        //                    }
        //                if (stato != null &&
        //                    DiagrammiManager.IsRuoloAssociatoStatoDia(stato.ID_DIAGRAMMA.ToString(), UserManager.getRuolo().idGruppo, stato.SYSTEM_ID.ToString()) &&
        //                    trasmWF)
        //                    DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammi(trasmissione.systemId, schedaDocumento.docNumber, Convert.ToString(stato.SYSTEM_ID), this);
        //            }
        //            return;
        //        }
        //    }
        //    //FINE MODELLI TRASMISSIONE NUOVI
        //    DocsPaWR.Trasmissione trasmEff = null;

        //    try
        //    {
        //        if (this.ddl_tmpl.SelectedIndex > 0 && ddl_tmpl.SelectedIndex > indexSel)
        //        {
        //            trasmEff = creaTrasmissione();

        //            if (trasmEff == null)
        //                //Response.Write("<script language='javascript'>alert('Si è verificato un errore nella creazione della trasmissione da template');</script>");
        //                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Err_template", "alert('Si è verificato un errore nella creazione della trasmissione da template');", true);
        //            else
        //            {

        //                if (estendiVisibilita.Value == "false")
        //                {
        //                    TrasmissioneSingola[] appoTrasmSingole = new TrasmissioneSingola[trasmEff.trasmissioniSingole.Length];
        //                    for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
        //                    {
        //                        TrasmissioneSingola trasmSing = new TrasmissioneSingola();
        //                        trasmSing = trasmEff.trasmissioniSingole[i];
        //                        trasmSing.ragione.eredita = "0";
        //                        appoTrasmSingole[i] = trasmSing;
        //                    }
        //                    trasmEff.trasmissioniSingole = appoTrasmSingole;
        //                }

        //                DocsPaWR.Trasmissione t_rs = TrasmManager.executeTrasm(this, trasmEff);

        //                if (t_rs != null && t_rs.ErrorSendingEmails)
        //                    //Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");
        //                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Err_mail", "window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');", true);

        //                //resetto il template della trasmissione
        //                this.ddl_tmpl.SelectedIndex = 0;

        //                //Salvataggio del system_id della trasm per il cambio di stato automatico
        //                if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
        //                {
        //                    DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
        //                    bool trasmWF = false;
        //                    for (int i = 0; i < trasmEff.trasmissioniSingole.Length; i++)
        //                    {
        //                        DocsPaWR.TrasmissioneSingola trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmEff.trasmissioniSingole[i];
        //                        if (trasmSing.ragione.tipo == "W")
        //                            trasmWF = true;
        //                    }
        //                    if (stato != null &&
        //                        DiagrammiManager.IsRuoloAssociatoStatoDia(stato.ID_DIAGRAMMA.ToString(), UserManager.getRuolo().idGruppo, stato.SYSTEM_ID.ToString()) &&
        //                        trasmWF)
        //                        DocsPAWA.DiagrammiManager.salvaStoricoTrasmDiagrammi(trasmEff.systemId, schedaDocumento.docNumber, Convert.ToString(stato.SYSTEM_ID), this);
        //                }
        //            }
        //        }
        //    }
        //    catch (System.Web.Services.Protocols.SoapException es)
        //    {
        //        ErrorManager.redirect(this, es);
        //    }
        //    logger.Info("END");
        //}

        /// <summary>
        /// 
        /// </summary>
        private void smistaDocumento()
        {
            try
            {
                bool selezioneUoApp = false;
                bool selezioneUoInf = false;
                bool selezioneUtenteRuolo = false;

                this.UpdateFlagUo(this.indxUoSel, out selezioneUoApp, out selezioneUoInf, out selezioneUtenteRuolo);

                string alertMessage = string.Empty;

                //Se per un ruolo selezionato non è stato anche selezionato almeno un utente scatta l'alert
                if (selezioneUtenteRuolo)
                {
                    string msg = "Effettuare almeno una selezione per Competenza o Conoscenza di un utente.";
                    string scriptString = "<SCRIPT>alert('" + msg + "');</SCRIPT>";
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "AlertModalDialog", scriptString);
                    return;
                }

                // NB: 
                // IL CONTROLLO SULLE SELEZIONI VIENE COMMENTATO LATO FRONTEND IN 
                // QUANTO VIENE EFFETTUATO CORRETTAMENTE LATO BACKEND, IL QUALE CORRETTAMENTE
                // NON SI FIDA DELL'INPUT CHE ARRIVA DALL'ESTERNO E RICONTROLLA I DATI.
                // BENCHE' SAREBBE COSA BUONA E GIUSTA FARE TALI CONTROLLI ANCHE DA FRONTEND, 
                // MA ATTUALMENTE LA FUNZIONE DI CONTROLLO NON è UTILIZZABILE ED E' RIFARE
                
                    smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();

                    //Gestione NoteGenerali se è popolato il campo di testo
                    
                    //Eliminato controllo per ticket aperto
                   // if (this.txtNoteGen.Text != "")
                   // {
                        docManager.setNoteGenarali(txtNoteGen.Text);
                  //  }

                    
                    DocsPAWA.DocsPaWR.EsitoSmistamentoDocumento[] retValue = docManager.SmistaDocumento();
                    
                    bool notaSmistamento = false;   // codice 99
                    string descNota = "Avviso\\n\\nil documento [" + this.lbl_segnatura.Text + "] non è stato trasmesso ai seguenti ruoli poichè operano su registro differente:\\n\\n";
                    
                    bool erroreSmistamento = false; // codice > 0
                    string descrizioneErroreSmistamento = string.Empty;

                    // lettura valori di ritorno dello smistamento del documento corrente
                    foreach (DocsPAWA.DocsPaWR.EsitoSmistamentoDocumento item in retValue)
                    {
                        if (item.CodiceEsitoSmistamento.Equals(-1))
                        {
                            // Codice -1: 
                            // nessun destinatario selezionato per lo smistamento
                            erroreSmistamento = true;
                            descrizioneErroreSmistamento = item.DescrizioneEsitoSmistamento.Replace("'", @"\'");
                        }

                        if (!item.CodiceEsitoSmistamento.Equals(0) && !item.CodiceEsitoSmistamento.Equals(99) && !item.CodiceEsitoSmistamento.Equals(999))
                            erroreSmistamento = true;

                        if (!item.CodiceEsitoSmistamento.Equals(0) && item.CodiceEsitoSmistamento.Equals(99))
                        {
                            notaSmistamento = true;
                            descNota += " - smistamento a " + item.DenominazioneDestinatario.Replace("'", "\\'") + ":\\n" + item.DescrizioneEsitoSmistamento + "\\n\\n";
                        }


                        //ABBa
                        if (!item.CodiceEsitoSmistamento.Equals(0) && item.CodiceEsitoSmistamento.Equals(999))
                        {
                            listaExceptionTrasmissioni.Add(item.DescrizioneEsitoSmistamento.ToString());

                            //if (listaExceptionTrasmissioni.Count > 0)
                            //{
                            //    foreach (string s in listaExceptionTrasmissioni)
                            //    {
                            //        messError = messError + s + "\\n";
                            //    }
                            //}

                            erroreSmistamento = true;
                            notaSmistamento = true;
                            //descNota +="Trasmissioni con esito negativo: " + messError;
                        }

                        /* Andrea
                        if (item.CodiceEsitoSmistamento.Equals(999))
                        {

                            //listaExceptionTrasmissioni.Add("Il destinatario: "
                            //                                + item.DenominazioneDestinatario
                            //                                + " è inesistente");
                            listaExceptionTrasmissioni.Add(item.DescrizioneEsitoSmistamento.ToString());
                            
                            //Prova 
                            //erroreSmistamento = false;
                            //notaSmistamento = false;
                            //End Prova

                        }
                        //End Andrea
                         */

                    }
                    

                    //Andrea
                
                    if (listaExceptionTrasmissioni.Count > 0)
                    {
                        foreach (string s in listaExceptionTrasmissioni)
                        {
                            messError = messError + s + "\\n";
                        }
                        Session.Add("MessError", messError);
                    }
                    
                //End Andrea 
                    

                    // movenext del documento da smistare o uscita dallo smistamento
                    if (!erroreSmistamento && !notaSmistamento)
                    {
                        this.FascicolaDocumento();
                        this.verificaDiagrammiDiStato();
                        this.ProgressStatus();

                        if (!this.chk_mantieniSel.Checked)
                        {
                            // rimozione delle selezioni effettuate dall'utente qualora non sia attivo il flag mantieni selezione
                            this.ClearSelections();
                        }
                    }
                    else
                    {
                        if (notaSmistamento)
                        {
                            this.FascicolaDocumento();
                            this.ProgressStatus();
                            this.RegisterClientMessageError(descNota);

                            if (!this.chk_mantieniSel.Checked)
                            {
                                // rimozione delle selezioni effettuate dall'utente qualora non sia attivo il flag mantieni selezione
                                this.ClearSelections();
                            }
                        }
                        else
                        {
                            if (descrizioneErroreSmistamento == string.Empty)
                                descrizioneErroreSmistamento = "Errore nello smistamento";

                            this.RegisterClientMessageError(descrizioneErroreSmistamento);
                        }
                    }
                //}
                //else
                //{
                //    if (!ClientScript.IsStartupScriptRegistered("AlertModalDialog"))
                //    {
                //        string msg = "Effettuare almeno una selezione per Competenza o Conoscenza.";
                //        string scriptString = "<SCRIPT>alert('" + msg + "');</SCRIPT>";
                //        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "AlertModalDialog", scriptString);
                //    }
                //}
            }
            catch (Exception ex)
            {
                this.RegisterClientMessageError("Errore nello smistamento");
                string ecc = ex.Message;
            }
            finally
            {
                //rilascio le risorse allocate per il settaggio e la gestione dei dati aggiuntivi
                //dello smistamento (note individuali e data scadenza)
                //DocsPAWA.smistaDoc.SmistaDocSessionManager.ReleaseSmistamentoNoteIndividuali();

                this.GestVisibilitaFrecceNavigaUO();
            }
        }

        /// <summary>
        /// Indice dell'UO correntemente selezionata come di appartenenza
        /// </summary>
        /// <remarks>
        /// Nella navigazione UO inferiore / superiore, l'indice cambia e 
        /// diventa quello dell'UO su cui si clicca il pulsante "Naviga Inferiori"
        /// o quello dell'UO parent quando si clicca sul pulsante "Naviga Superiori"
        /// </remarks>
        protected string indxUoSel
        {
            get
            {
                if (this.ViewState["indxUoSel"] != null)
                    return this.ViewState["indxUoSel"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["indxUoSel"] = value;
            }
        }

        private void gestisciPopUpSelezioneSmistamento()
        {
            if (!string.IsNullOrEmpty(this.hdSelSmista.Value) && this.hdSelSmista.Value == "smista")
            {
                this.smistaDocumento();
            }
            else
            {
                 if (!string.IsNullOrEmpty(this.hdSelSmista.Value) && this.hdSelSmista.Value != "undefined")
                 {
                     this.navigaUoSelezionata(this.hdSelSmista.Value);
                 }

                 this.GestVisibilitaFrecceNavigaUO();
            }
            this.hdSelSmista.Value = "";
        }

        /// <summary>
        /// 
        /// </summary>
        private void caricaModelliTrasmRapida()
        {
            this.ddl_trasm_rapida.Items.Clear();

            string idAmm = UserManager.getInfoUtente(this).idAmministrazione;
            string idPeople = UserManager.getInfoUtente(this).idPeople;
            string idCorrGlobali = UserManager.getInfoUtente(this).idCorrGlobali;
            string idRuoloUtente = UserManager.getInfoUtente(this).idGruppo;
            string idTipoDoc = "";
            string idDiagramma = "";
            string idStato = "";

            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();
            DocsPaWR.DocumentoSmistamento docCorrente = docManager.GetCurrentDocument(false);

            DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDettaglioDocumentoNoDataVista(this, docCorrente.IDDocumento, docCorrente.DocNumber);
            DocumentManager.setDocumentoSelezionato(this, schedaDocumento);

            string settingValue = System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"];

            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                if (schedaDocumento.tipologiaAtto != null)
                {
                    if (schedaDocumento.template != null && schedaDocumento.template.SYSTEM_ID.ToString() != "")
                    {
                        idTipoDoc = schedaDocumento.template.SYSTEM_ID.ToString();

                        if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                        {
                            DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoDoc(schedaDocumento.tipologiaAtto.systemId, idAmm, this);
                            if (dg != null)
                            {
                                idDiagramma = dg.SYSTEM_ID.ToString();
                                DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(schedaDocumento.docNumber, this);
                                if (stato != null)
                                    idStato = stato.SYSTEM_ID.ToString();
                            }
                        }
                    }
                }
            }
            
            DocsPAWA.DocsPaWR.Registro[] registri = null;
            if (docCorrente.TipoDocumento == "G")
            {
                registri = ((DocsPAWA.DocsPaWR.Ruolo)UserManager.getRuolo(this)).registri;
            }
            else
            {
                registri = new DocsPAWA.DocsPaWR.Registro[1];
                registri[0] = schedaDocumento.registro;
            }
            //ArrayList idModelli = new ArrayList(ws.getModelliPerTrasm(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, "D", schedaDocumento.systemId, idRuoloUtente, false, schedaDocumento.accessRights));
            ArrayList idModelli = new ArrayList(ws.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, "D", schedaDocumento.systemId, idRuoloUtente, false, schedaDocumento.accessRights));            
            
            if (this.ddl_trasm_rapida.Items.Count == 0)
            {
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                li.Text = " ";
                this.ddl_trasm_rapida.Items.Add(li);
            }

            for (int i = 0; i < idModelli.Count; i++)
            {
                DocsPaWR.ModelloTrasmissione mod = (DocsPAWA.DocsPaWR.ModelloTrasmissione)idModelli[i];
                System.Web.UI.WebControls.ListItem li = new System.Web.UI.WebControls.ListItem();
                li.Value = mod.SYSTEM_ID.ToString();
                li.Text = mod.NOME;
                if (System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CODICE_MODELLI_TRASM"] != null && System.Configuration.ConfigurationManager.AppSettings["VISUALIZZA_CODICE_MODELLI_TRASM"] == "1")
                    li.Text += " (" + mod.CODICE + ")";
                this.ddl_trasm_rapida.Items.Add(li);
            }
        }

        protected void ddl_trasm_rapida_SelectedIndexChanged(object sender, EventArgs e)
        {
            smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();
            DocsPaWR.UOSmistamento uoAppartenenza = docManager.GetUOAppartenenza();
            uoAppartenenza.UoSmistaTrasAutomatica = null;
            DocsPaWR.ModelloTrasmissione modello = new DocsPAWA.DocsPaWR.ModelloTrasmissione();
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            ArrayList uoTrasm = new ArrayList();
            List<string> uoRiferimento = new List<string>();
            if(!string.IsNullOrEmpty(this.txtNoteGen.Text))
                Session.Add("NoteGenSmista", this.txtNoteGen.Text);

            int indexSel = ddl_trasm_rapida.SelectedIndex;
            if (indexSel > 0)
            {
                modello = ws.getModelloByID(UserManager.getInfoUtente(this).idAmministrazione, ddl_trasm_rapida.SelectedValue);
                this.txtNoteGen.Text = modello.VAR_NOTE_GENERALI;
                for (int i = 0; i < modello.RAGIONI_DESTINATARI.Length; i++)
                {
                    DocsPaWR.RagioneDest ragDest = (DocsPAWA.DocsPaWR.RagioneDest)modello.RAGIONI_DESTINATARI[i];
                    ArrayList destinatari = new ArrayList(ragDest.DESTINATARI);
                    for (int j = 0; j < destinatari.Count; j++)
                    {
                        DocsPaWR.UOSmistamento uo = new DocsPAWA.DocsPaWR.UOSmistamento();
                        DocsPaWR.RuoloSmistamento ruolo = new DocsPAWA.DocsPaWR.RuoloSmistamento();
                        DocsPaWR.MittDest mittDest = (DocsPAWA.DocsPaWR.MittDest)destinatari[j];
                        switch (mittDest.CHA_TIPO_MITT_DEST)
                        {
                            case "D":
                                if (mittDest.CHA_TIPO_URP == "R")
                                {
                                    bool isOneUtenteNotifica = false;
                                    ruolo.ID = Convert.ToString(mittDest.ID_CORR_GLOBALI);
                                    ruolo.Descrizione = mittDest.DESCRIZIONE;
                                    ruolo.Codice = mittDest.VAR_COD_RUBRICA;
                                    ruolo.ragioneTrasmRapida = ragDest.RAGIONE;
                                    ruolo.Registri = ws.SmistaGetRegistriRuolo(ruolo.ID);
                                    ruolo.chaTipoTrasm = mittDest.CHA_TIPO_TRASM;
                                    ruolo.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
                                    ruolo.datiAggiuntiviSmistamento.NoteIndividuali = mittDest.VAR_NOTE_SING;
                                    ruolo.datiAggiuntiviSmistamento.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                                    ArrayList utentiConNotifica = new ArrayList();
                                    for (int k = 0; k < mittDest.UTENTI_NOTIFICA.Length; k++)
                                    {
                                        DocsPaWR.UtentiConNotificaTrasm utenteNotifica = (DocsPaWR.UtentiConNotificaTrasm)mittDest.UTENTI_NOTIFICA[k];
                                        if (utenteNotifica.FLAG_NOTIFICA == "1")
                                        {
                                            DocsPaWR.UtenteSmistamento utenteSmistamento = new DocsPAWA.DocsPaWR.UtenteSmistamento();
                                            utenteSmistamento.Denominazione = utenteNotifica.NOME_COGNOME_UTENTE;
                                            utenteSmistamento.ID = utenteNotifica.ID_PEOPLE;
                                            utenteSmistamento.UserID = utenteNotifica.CODICE_UTENTE;
                                            this.getDatiAggiuntiviUtenteSmistamento(ref utenteSmistamento, utenteSmistamento.ID);
                                            utentiConNotifica.Add(utenteSmistamento);
                                            isOneUtenteNotifica = true;
                                        }
                                    }
                                    if (isOneUtenteNotifica)
                                    {
                                        DocsPaWR.UtenteSmistamento[] utenti = new DocsPAWA.DocsPaWR.UtenteSmistamento[utentiConNotifica.Count];
                                        utentiConNotifica.CopyTo(utenti);
                                        ruolo.Utenti = utenti;
                                        DocsPaWR.RuoloSmistamento[] ruoli = { ruolo };
                                        uo.Ruoli = ruoli;
                                        uoTrasm.Add(uo);
                                    }
                                }
                                if (mittDest.CHA_TIPO_URP == "U")
                                {
                                    uo.ragioneTrasmRapida = ragDest.RAGIONE;
                                    uo.ID = Convert.ToString(mittDest.ID_CORR_GLOBALI);
                                    uo.Codice = mittDest.VAR_COD_RUBRICA;
                                    uo.Descrizione = mittDest.DESCRIZIONE;
                                    uo.Ruoli = ws.getRuoliUoSmistamento(uo.ID);
                                    if (uo.Ruoli.Length > 0)
                                    {
                                        foreach (DocsPaWR.RuoloSmistamento role in uo.Ruoli)
                                        {
                                            role.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
                                            role.datiAggiuntiviSmistamento.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                                            role.datiAggiuntiviSmistamento.NoteIndividuali = mittDest.VAR_NOTE_SING;
                                        }
                                        uoTrasm.Add(uo);
                                    }
                                    else
                                    {
                                        uoRiferimento.Add("Manca un ruolo di riferimento per la UO: "
                                                    + uo.Codice + " (" + uo.Descrizione+ ")"
                                                    + ".");
                                    }
                                }
                                if (mittDest.CHA_TIPO_URP == "P")
                                {
                                    DocsPaWR.UtenteSmistamento ut = new DocsPAWA.DocsPaWR.UtenteSmistamento();
                                    //OLD:  ut.ID = Convert.ToString(mittDest.ID_CORR_GLOBALI);
                                    //LULUCIANI
                                    DocsPaWR.UtentiConNotificaTrasm utenteNotifica = (DocsPaWR.UtentiConNotificaTrasm)mittDest.UTENTI_NOTIFICA[0];
                                    if (utenteNotifica != null && !string.IsNullOrEmpty(utenteNotifica.ID_PEOPLE))
                                        ut.ID = Convert.ToString(utenteNotifica.ID_PEOPLE);
                                    else throw new Exception("Errore impossibile trovare l'utente con notifica con ID: " + mittDest.ID_CORR_GLOBALI);
                                    //END LULUCIANI                                   
                                    ut.ragioneTrasmRapida = ragDest.RAGIONE;
                                    ut.UserID = mittDest.VAR_COD_RUBRICA;
                                    ut.Denominazione = mittDest.DESCRIZIONE;
                                    ut.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
                                    ut.datiAggiuntiviSmistamento.NoteIndividuali = mittDest.VAR_NOTE_SING;
                                    ut.datiAggiuntiviSmistamento.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                                    this.getDatiAggiuntiviUtenteSmistamento(ref ut, ut.ID);
                                    ruolo.Utenti = new DocsPAWA.DocsPaWR.UtenteSmistamento[] { ut };
                                    uo.Ruoli = new DocsPAWA.DocsPaWR.RuoloSmistamento[] { ruolo };
                                    uoTrasm.Add(uo);
                                }
                                break;
                            case "UT_P":
                                DocsPaWR.Corrispondente corrispondente = new DocsPaWR.Corrispondente();
                                corrispondente = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, DocumentManager.getDocumentoSelezionato(), null, this);
                                DocsPaWR.UtenteSmistamento[] utentiNot = ws.getUtentiRuoloSmistamento(corrispondente.systemId, "U");
                                utentiNot[0].ragioneTrasmRapida = ragDest.RAGIONE;
                                ruolo.Utenti = utentiNot;
                                DocsPaWR.RuoloSmistamento[] ruoliSmistamento = { ruolo };
                                uo.Ruoli = ruoliSmistamento;
                                foreach (DocsPaWR.RuoloSmistamento role in uo.Ruoli)
                                {
                                    role.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
                                    role.datiAggiuntiviSmistamento.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                                    role.datiAggiuntiviSmistamento.NoteIndividuali = mittDest.VAR_NOTE_SING;
                                }
                                uoTrasm.Add(uo);
                                break;
                            case "UO_P":
                                DocsPaWR.Corrispondente corrUo = new DocsPaWR.Corrispondente();
                                corrUo = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, DocumentManager.getDocumentoSelezionato(), null, this);
                                uo.ragioneTrasmRapida = ragDest.RAGIONE;
                                uo.ID = corrUo.systemId;
                                uo.Codice = corrUo.codiceCorrispondente;
                                uo.Descrizione = corrUo.descrizione;
                                //prendo dolo i ruoli di riferimento come avviene nella trasmissione da modello
                                uo.Ruoli = ws.getRuoliUoSmistamento(uo.ID);
                                if (uo.Ruoli.Length > 0)
                                {
                                    foreach (DocsPaWR.RuoloSmistamento role in uo.Ruoli)
                                    {
                                        role.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
                                        role.datiAggiuntiviSmistamento.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                                        role.datiAggiuntiviSmistamento.NoteIndividuali = mittDest.VAR_NOTE_SING;
                                    }
                                    uoTrasm.Add(uo);
                                }
                                break;
                            default:
                                DocsPaWR.Corrispondente corr = new DocsPaWR.Corrispondente();
                                corr = TrasmManager.getCorrispondenti(mittDest.CHA_TIPO_MITT_DEST, DocumentManager.getDocumentoSelezionato(), null, this);
                                if (corr != null)
                                {
                                    ruolo.ID = corr.systemId;
                                    ruolo.Descrizione = corr.descrizione;
                                    ruolo.Codice = corr.codiceCorrispondente;
                                    ruolo.ragioneTrasmRapida = ragDest.RAGIONE;
                                    ruolo.Registri = ws.SmistaGetRegistriRuolo(ruolo.ID);
                                    DocsPaWR.UtenteSmistamento[] utentiNotifica = ws.getUtentiRuoloSmistamento(ruolo.ID, "R");
                                    ruolo.Utenti = utentiNotifica;
                                    DocsPaWR.RuoloSmistamento[] ruoliSmista = { ruolo };
                                    uo.Ruoli = ruoliSmista;
                                    if (utentiNotifica.Length > 0)
                                    {
                                        foreach (DocsPaWR.RuoloSmistamento role in uo.Ruoli)
                                        {
                                            role.datiAggiuntiviSmistamento = new DocsPaWR.datiAggiuntiviSmistamento();
                                            role.datiAggiuntiviSmistamento.tipoTrasm = mittDest.CHA_TIPO_TRASM;
                                            role.datiAggiuntiviSmistamento.NoteIndividuali = mittDest.VAR_NOTE_SING;
                                        }
                                        uoTrasm.Add(uo);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            if (uoTrasm.Count > 0)
            {
                DocsPaWR.UOSmistamento[] uoTrasmRapida = new DocsPAWA.DocsPaWR.UOSmistamento[uoTrasm.Count];
                uoTrasm.CopyTo(uoTrasmRapida);                
                uoAppartenenza.UoSmistaTrasAutomatica = uoTrasmRapida;
                if (!string.IsNullOrEmpty(modello.NO_NOTIFY) && modello.NO_NOTIFY.Equals("1"))
                    uoAppartenenza.modelloNoNotify = true;
                else
                    uoAppartenenza.modelloNoNotify = false;
            }

            if (uoRiferimento != null && uoRiferimento.Count != 0)
            {
                string msgError = string.Empty;
                foreach (string s in uoRiferimento)
                {
                    msgError = msgError + s + "\\n";
                }
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "AlertModalDialog", "<script>alert('" + msgError + "');</script>");
                uoRiferimento = new List<string>();
            }
        }

        private void getDatiAggiuntiviUtenteSmistamento(ref DocsPaWR.UtenteSmistamento utenteSmistamento, string idUtente)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            ws.getDatiAggiuntiviUtenteSmistamento(ref utenteSmistamento, idUtente);
        }

        /// <summary>
        /// Rimozione di tutte le selezioni effettuate nelle UO visualizzate
        /// </summary>
        private void ClearSelections()
        {
            DocsPaWR.UOSmistamento uoAppartenenza = this.GetSmistaDocManager().GetUOAppartenenza();

            this.ClearSelectionsUO(uoAppartenenza);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uo"></param>
        private void ClearSelectionsUO(DocsPaWR.UOSmistamento uo)
        {
            uo.FlagCompetenza = false;
            uo.FlagConoscenza = false;

            if (uo.Ruoli != null)
                foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                    this.ClearSelectionsRuoli(ruolo);

            if (uo.UoInferiori != null)
                foreach (DocsPaWR.UOSmistamento uoInf in uo.UoInferiori)
                    this.ClearSelectionsUO(uoInf);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruolo"></param>
        private void ClearSelectionsRuoli(DocsPaWR.RuoloSmistamento ruolo)
        {
            ruolo.FlagCompetenza = false;
            ruolo.FlagConoscenza = false;
            ruolo.datiAggiuntiviSmistamento = new DocsPAWA.DocsPaWR.datiAggiuntiviSmistamento();

            if (ruolo.Utenti != null)
                foreach (DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                    this.ClearSelectionsUtenti(utente);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="utente"></param>
        private void ClearSelectionsUtenti(DocsPaWR.UtenteSmistamento utente)
        {
            utente.FlagCompetenza = false;
            utente.FlagConoscenza = false;
            utente.datiAggiuntiviSmistamento = new DocsPAWA.DocsPaWR.datiAggiuntiviSmistamento();
        }

        private void verificaDiagrammiDiStato()
        {
            //Verifico l'abilitazione dei diagrammi di stato
            if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
            {
                // DocsPaWR.Trasmissione trasmissione = (DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this);

                //if (trasmissione.tipoOggetto.ToString() == "DOCUMENTO")

                SmistaDocManager mng = this.GetSmistaDocManager();
                string idTrasmissione = mng.GetIdTrasmissione(mng.GetCurrentDocumentPosition() - 1);


                //DocsPaWR.InfoDocumento infoDocumento = ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoDocumento;
                DocsPaWR.DocumentoSmistamento infoDocumento = mng.GetCurrentDocument(false);

                //E' importante che l'accettazione della trasmiossione corrente sia fatta prima di questo tipo di verifica
                if (DocsPAWA.DiagrammiManager.isUltimaDaAccettare(idTrasmissione, this))
                {

                    DocsPaWR.Stato statoSucc = DocsPAWA.DiagrammiManager.getStatoSuccessivoAutomatico(infoDocumento.DocNumber, this);
                    DocsPaWR.Stato statoCorr = DocsPAWA.DiagrammiManager.getStatoDoc(infoDocumento.DocNumber, this);
                    if (statoSucc != null)
                    {
                        if (statoSucc.STATO_FINALE)
                        {
                            //Controllo se non è bloccato il documento principale o un suo allegato
                            if (CheckInOut.CheckInOutServices.IsCheckedOutDocument(infoDocumento.IDDocumento, infoDocumento.DocNumber, UserManager.getInfoUtente(this), true))
                            {
                                RegisterStartupScript("docInCheckOut", "<script>alert('Attenzione non è possibile passare in uno stato finale. Documento o allegati bloccati !');</script>");
                                return;
                            }

                            //Scatta l'alert
                            MessageBox1.Confirm("Si sta portando il documento in uno stato finale.\\nIl documento diventerà di sola lettura.\\nConfermi ?");
                            return;
                        }
                        else
                        {
                            //Cambio stato
                            DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA), this);
                            DocsPAWA.DiagrammiManager.salvaModificaStato(infoDocumento.DocNumber, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);

                            //Cancellazione storico trasmissioni
                            DocsPAWA.DiagrammiManager.deleteStoricoTrasmDiagrammi(infoDocumento.DocNumber, Convert.ToString(statoCorr.SYSTEM_ID), this);
                            //Verifico se il nuovo stato ha delle trasmissioni automatiche
                            DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(infoDocumento.DocNumber, this);
                            string idTipoDoc = ProfilazioneDocManager.getIdTemplate(infoDocumento.DocNumber, this);
                            if (idTipoDoc != "")
                            {
                                ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAuto(UserManager.getInfoUtente(this).idAmministrazione, Convert.ToString(stato.SYSTEM_ID), idTipoDoc, this));
                                for (int i = 0; i < modelli.Count; i++)
                                {
                                    DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                    if (mod.SINGLE == "1")
                                    {
                                        DocsPaWR.InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(infoDocumento.IDDocumento, infoDocumento.DocNumber, this);
                                        //TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoDocumento, this);
                                        TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), infoDoc, this);
                                    }
                                    else
                                    {
                                        for (int k = 0; k < mod.MITTENTE.Length; k++)
                                        {
                                            if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                            {
                                                DocsPaWR.InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(infoDocumento.IDDocumento, infoDocumento.DocNumber, this);
                                                //TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoDocumento, this);
                                                TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), infoDoc, this);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void MessageBox1_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                //DocsPaWR.InfoDocumento infoDocumento = ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoDocumento;
                SmistaDocManager mng = this.GetSmistaDocManager();
                DocsPaWR.DocumentoSmistamento infoDocumento = mng.GetCurrentDocument(false);

                DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();

                DocsPaWR.Stato statoSucc = DocsPAWA.DiagrammiManager.getStatoSuccessivoAutomatico(infoDocumento.DocNumber, this);
                DocsPaWR.Stato statoCorr = DocsPAWA.DiagrammiManager.getStatoDoc(infoDocumento.DocNumber, this);

                //Cambio stato
                DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDiagrammaById(Convert.ToString(statoSucc.ID_DIAGRAMMA), this);
                DocsPAWA.DiagrammiManager.salvaModificaStato(infoDocumento.DocNumber, Convert.ToString(statoSucc.SYSTEM_ID), dg, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);

                //Cancellazione storico trasmissioni
                DocsPAWA.DiagrammiManager.deleteStoricoTrasmDiagrammi(infoDocumento.DocNumber, Convert.ToString(statoCorr.SYSTEM_ID), this);

                //Verifico se il nuovo stato ha delle trasmissioni automatiche
                DocsPaWR.Stato stato = DocsPAWA.DiagrammiManager.getStatoDoc(infoDocumento.DocNumber, this);
                string idTemplate = ProfilazioneDocManager.getIdTemplate(infoDocumento.DocNumber, this);
                if (idTemplate != "")
                {
                    ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAuto(UserManager.getInfoUtente(this).idAmministrazione, Convert.ToString(stato.SYSTEM_ID), idTemplate, this));
                    for (int i = 0; i < modelli.Count; i++)
                    {
                        DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                        if (mod.SINGLE == "1")
                        {
                            DocsPaWR.InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(infoDocumento.IDDocumento, infoDocumento.DocNumber, this);
                            //TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoDocumento, this);
                            TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), infoDoc, this);
                        }
                        else
                        {
                            for (int k = 0; k < mod.MITTENTE.Length; k++)
                            {
                                if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                {
                                    DocsPaWR.InfoDocumento infoDoc = DocumentManager.GetInfoDocumento(infoDocumento.IDDocumento, infoDocumento.DocNumber, this);
                                    //TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), ((DocsPAWA.DocsPaWR.Trasmissione)TrasmManager.getDocTrasmSel(this)).infoDocumento, this);
                                    TrasmManager.effettuaTrasmissioneDocDaModello(mod, Convert.ToString(stato.SYSTEM_ID), infoDoc, this);
                                    break;
                                }
                            }
                        }
                    }
                }

                //Metto il documento in sola lettura
                DocsPaVO.HMDiritti.HMdiritti diritti = new DocsPaVO.HMDiritti.HMdiritti();
                wws.cambiaDirittiDocumenti(diritti.HMdiritti_Read, infoDocumento.IDDocumento);
                //schedaDocumento.accessRights = "45";

                //bool result = accettaRifiuta(DocsPAWA.DocsPaWR.TrasmissioneTipoRisposta.ACCETTAZIONE);

                //this.btn_accetta.Visible	= false;
                //this.btn_rifiuta.Visible	= false;
                //this.txt_noteAccRif.Visible = false;
                /*
                if (result)
                {
                    this.isEnabledAccRif(false);
                }
                */
            }
        }
    }
}
    	
