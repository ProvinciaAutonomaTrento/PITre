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
using System.Configuration;
using DocsPAWA.DocsPaWR;
using DocsPaWA.UserControls;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for modificaFascicolo.
	/// </summary>
    public class modificaFascicolo : DocsPAWA.CssPage
    {
        #region Sezione variabili
        protected DocsPAWA.DocsPaWR.Fascicolo fasc;
		protected System.Web.UI.WebControls.Label lbl_note;
		protected System.Web.UI.WebControls.Button btn_salva;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label lbl_descrizione;
        protected Note.DettaglioNota dettaglioNota;
		protected System.Web.UI.WebControls.TextBox txt_descFascicolo;
		protected System.Web.UI.WebControls.TextBox txt_LFCod;
		protected System.Web.UI.WebControls.TextBox txt_LFDesc;
        protected DocsPAWA.UserControls.Calendar txt_LFDTA;
		protected System.Web.UI.WebControls.Label lbl_LFDTA;
		protected System.Web.UI.WebControls.ImageButton btn_rubrica;
		protected System.Web.UI.WebControls.TextBox txt_cod_uff_ref;
		protected System.Web.UI.WebControls.TextBox txt_desc_uff_ref;
		protected System.Web.UI.WebControls.Panel pnl_uff_ref;
		protected DocsPAWA.DocsPaWR.InfoUtente userInfo;
		protected System.Web.UI.WebControls.ImageButton btn_rubrica_ref;
        protected System.Web.UI.WebControls.CheckBox chkFascicoloCartaceo;
		protected System.Web.UI.HtmlControls.HtmlInputButton btn_annulla;
		protected bool enableUfficioRef;
		protected DocsPAWA.DocsPaWR.Corrispondente corrRef;
		protected DocsPAWA.DocsPaWR.Utente utente;
		protected DocsPAWA.DocsPaWR.Ruolo ruolo;
		protected System.Web.UI.WebControls.Panel pnl_locFis;
		protected DocsPAWA.DocsPaWR.Trasmissione trasmissione;
        protected System.Web.UI.WebControls.Panel panel_profDinamica;
        protected System.Web.UI.WebControls.DropDownList ddl_tipologiaFasc;
        protected System.Web.UI.WebControls.Panel panel_Contenuto;
        protected Table table;
        protected DocsPaWR.Templates template;
        protected System.Web.UI.WebControls.Panel Panel_DiagrammiStato;
        protected System.Web.UI.WebControls.DropDownList ddl_statiSuccessivi;
        protected Utilities.MessageBox msg_StatoAutomatico;
        protected Utilities.MessageBox msg_StatoFinale;
        protected ArrayList dirittiCampiRuolo;
        protected System.Web.UI.WebControls.Label lblFascicoloControllato;
        protected System.Web.UI.WebControls.CheckBox chkFascicoloControllato;
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
			this.txt_LFCod.TextChanged += new System.EventHandler(this.txt_LFCod_TextChanged);
            this.ddl_tipologiaFasc.SelectedIndexChanged += new EventHandler(this.ddl_tipologiaFasc_SelectedIndexChanged);            
            this.txt_cod_uff_ref.TextChanged += new System.EventHandler(this.txt_cod_uff_ref_TextChanged);
			this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
			this.btn_annulla.ServerClick += new System.EventHandler(this.btn_annulla_ServerClick);
			this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_PreRender(object sender, EventArgs e)
        {
            // Gestione abilitazione pulsante salva
            this.EnableButtonSalva();
        }

        private void Page_Load(object sender, System.EventArgs e)
        {

            try
            {
                Page.Response.Expires = -1;
                // Put user code to initialize the page here
                userInfo = UserManager.getInfoUtente(this);
                enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null
                    && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
                TrasmManager.removeGestioneTrasmissione(this);

                if (!this.IsPostBack)
                {
                    // Creazione di una copia locale del fascicolo, utilizzata solo
                    // per apportare modifiche nell'ambito del dettaglio
                    this.FascicoloInLavorazione = this.CloneFascicolo(FascicoliManager.getFascicoloSelezionato(this));
                    
                    // Al dettaglio delle note viene impostata la chiave di sessione del fascicolo in lavorazione
                    this.dettaglioNota.ContainerSessionKey = FASCICOLO_IN_LAVORAZIONE_SESSION_KEY;
                    
                    // Disabilitazione di tutti i controlli della maschera se in modalità readonly
                    this.DisableControlsIfReadOnlyMode();
                }

                this.fasc = this.FascicoloInLavorazione;

                corrRef = FascicoliManager.getUoReferenteSelezionato(this);

                if (!IsPostBack)
                {
                    //Profilazione dinamica fascicoli
                    if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1" && fasc.tipo.Equals("P"))
                    {
                        Session.Remove("template");                        
                        panel_profDinamica.Visible = true;
                        CaricaComboTipologiaFasc();
                    }
                    //Fine profilazione dinamica fascicoli

                    this.txt_descFascicolo.Text = fasc.descrizione;
                    
                    // Caricamento dati dettaglio fascicolo
                    //this.dettaglioNota.AttatchPulsanteConferma(this.btn_salva.ClientID);
                    this.dettaglioNota.Fetch();

                    // Impostazione valore checkbox fascicolo cartaceo
                    this.chkFascicoloCartaceo.Checked = fasc.cartaceo;
                    // Se il fascicolo è cartaceo, il check è disabilitato
                    this.chkFascicoloCartaceo.Enabled = !fasc.cartaceo;

                    //Impostazione valore checkbox fascicolo controllato
                    if (fasc.controllato == "1")
                        this.chkFascicoloControllato.Checked = true;
                    if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["GEST_FASC_CONTROLLATO"]) && System.Configuration.ConfigurationManager.AppSettings["GEST_FASC_CONTROLLATO"].Equals("1"))
                    {
                        this.chkFascicoloControllato.Visible = true;
                        lblFascicoloControllato.Visible = true;
                    }
                    else
                    {
                        this.chkFascicoloControllato.Visible = false;
                        this.lblFascicoloControllato.Visible = false;
                    }
                    this.chkFascicoloControllato.Enabled = false;
                    if (UserManager.ruoloIsAutorized(this, "FASC_CONTROLLATO"))
                    {
                        this.chkFascicoloControllato.Enabled = true;
                    }

                    if (!fasc.tipo.Equals("G"))
                    {
                        if (fasc.idUoLF != null)
                        {
                            DocsPaVO.LocazioneFisica.LocazioneFisica _lf = new DocsPaVO.LocazioneFisica.LocazioneFisica();
                            this.txt_LFCod.Text = fasc.varCodiceRubricaLF;
                            _lf.CodiceRubrica = fasc.varCodiceRubricaLF;
                            _lf.UO_ID = fasc.idUoLF;
                            if (fasc.dtaLF != null && fasc.dtaLF != "")
                            {
                                this.GetCalendarControl("txt_LFDTA").txt_Data.Text = fasc.dtaLF.Substring(0, 10);
                                _lf.Data = fasc.dtaLF.Substring(0, 10);
                            }
                            else
                            {
                                _lf.Data = (string)DateTime.Now.ToShortDateString();
                            }
                            this.txt_LFDesc.Text = fasc.descrizioneUOLF;
                            _lf.Descrizione = fasc.descrizioneUOLF;

                            FascicoliManager.DO_SetLocazioneFisica(_lf);
                        }
                    }
                    if (enableUfficioRef)
                    {
                        if (!fasc.tipo.Equals("G") && fasc.ufficioReferente != null)
                        {
                            this.txt_cod_uff_ref.Text = fasc.ufficioReferente.codiceRubrica;
                            this.txt_desc_uff_ref.Text = fasc.ufficioReferente.descrizione;
                        }
                    }
                }
                else
                {

                    if (FascicoliManager.DO_VerifyFlagLF())
                    {
                        //FascicoliManager.DO_RemoveFlagLF();

                        DocsPaVO.LocazioneFisica.LocazioneFisica LF = FascicoliManager.DO_GetLocazioneFisica();
                        //						FascicoliManager.DO_RemoveLocazioneFisica();
                        if (LF != null)
                        {
                            txt_LFDesc.Text = LF.Descrizione;

                            if (LF.CodiceRubrica != null)
                            {
                                this.txt_LFCod.Text = LF.CodiceRubrica;

                                if (this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString().Trim() != "")
                                {
                                    LF.Data = this.GetCalendarControl("txt_LFDTA").txt_Data.Text;
                                }
                                else
                                {
                                    LF.Data = "";
                                }

                            }
                        }
                    }

                    if (enableUfficioRef)
                    {
                        if (FascicoliManager.DO_VerifyFlagUR())
                        {
                            FascicoliManager.DO_RemoveFlagUR();
                            if (!fasc.tipo.Equals("G") && corrRef != null)
                            {
                                this.txt_cod_uff_ref.Text = corrRef.codiceRubrica;
                                this.txt_desc_uff_ref.Text = corrRef.descrizione;
                            }
                        }
                    }

                    //Profilazione dinamica fascicoli
                    if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    {
                        template = (DocsPaWR.Templates)Session["template"];
                        if (template != null && ddl_tipologiaFasc.SelectedValue == template.SYSTEM_ID.ToString())
                        {
                            inserisciComponenti("NO", template);
                        }
                    }
                    //Fine profilazione dinamica fascicoli
                }

                // Inizializzazione condizionale link rubrica
                string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);
                if (use_new_rubrica != "1")
                    this.btn_rubrica.Attributes["onClick"] = "ApriRubrica('fascLF','U');";
                else
                    this.btn_rubrica.Attributes["onClick"] = "_ApriRubrica('ef_locfisica');";

                if (enableUfficioRef && !fasc.tipo.Equals("G"))
                {
                    this.pnl_uff_ref.Visible = true;
                    if (UserManager.ruoloIsAutorized(this, "DO_PROT_MODIFICA_UFF_REF"))
                    {
                        this.btn_rubrica_ref.Enabled = true;
                        this.txt_cod_uff_ref.ReadOnly = false;

                        // Inizializzazione condizionale link rubrica
                        if (use_new_rubrica != "1")
                            this.btn_rubrica_ref.Attributes["onClick"] = "ApriRubrica('fascUffRefMod','U');";
                        else
                            this.btn_rubrica_ref.Attributes["onClick"] = "_ApriRubrica('ef_uffref');";

                    }
                }
                if (!fasc.tipo.Equals("G"))//se il fascicolo è generale non deve essere gestita la locazione fisica
                {
                    this.pnl_locFis.Visible = true;
                }
                else
                {
                    this.pnl_locFis.Visible = false;
                }
                //luluciani: per il fascicolo generale non deve poter essere modificabile la descrizione.
                if (fasc != null && fasc.tipo.Equals("G"))
                {
                    this.txt_descFascicolo.ReadOnly = true;
                }                
            }
            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
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

		private void txt_LFCod_TextChanged(object sender, System.EventArgs e)
		{
			try 
			{
				txt_LFDesc.Text = "";
				if(txt_LFCod.Text != "")
				{
					setDescCorrispondente(this.txt_LFCod.Text,true); 
				}
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void setDescCorrispondente(string codiceRubrica, bool fineValidita) 
		{						
			string msg = "Codice rubrica non esistente";
			DocsPaWR.Corrispondente corr = null;
			try
			{
				if(!codiceRubrica.Equals(""))
					corr = UserManager.GetCorrispondenteInterno(this, codiceRubrica, fineValidita);
				if((corr != null && corr.descrizione != "") && corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)))
				{
					txt_LFDesc.Text = corr.descrizione;
					DocsPaVO.LocazioneFisica.LocazioneFisica LF = new DocsPaVO.LocazioneFisica.LocazioneFisica();
					LF.CodiceRubrica = corr.codiceRubrica;
					LF.Descrizione = corr.descrizione;
					LF.UO_ID = corr.systemId;
					if(this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString().Trim()!="")
					{
						LF.Data = this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString();
					}
					else
					{
						LF.Data = System.DateTime.Now.ToShortDateString();
					}
					
					//metto la LF in session
					FascicoliManager.DO_SetLocazioneFisica(LF);
					//					FascicoliManager.DO_SetFlagLF();
				}
				else
				{
					if(!codiceRubrica.Equals(""))
					{
						RegisterStartupScript("alert","<script language='javascript'>alert('" + msg + "');</script>");
						string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_LFCod.ID + "').focus() </SCRIPT>";
						RegisterStartupScript("focus", s);
					}
					this.txt_LFDesc.Text = "";

				}
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}		
		}

		private void txt_cod_uff_ref_TextChanged(object sender, System.EventArgs e)
		{
			try 
			{
				setDescUffRefInModificaFascicolo(this.txt_cod_uff_ref.Text);	
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}
	
		/// <summary>
		/// CheckUoReferenteFascicoli:
		/// Verifica se la UO specificata come Ufficio Referente possiede dei ruoli di riferimento
		/// prima di procedere al salvataggio dei dati
		/// </summary>
		/// <returns></returns>
		private bool CheckUoReferenteFascicoli()
		{
			bool result = true;
			try 
			{
				DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
				DocsPaWR.Corrispondente corrRef = FascicoliManager.getUoReferenteSelezionato(this);
				if(corrRef != null)
				{
					if(!docsPaWS.UOHasReferenceRole(corrRef.systemId))
					{
						result = false;
					}
				}
				else
				{
					//necessario per controllare il caso in cui si salvi la modifica di un fascicolo
					//specificando la stessa UO di prima(non è detto che in questo momento possieda Ruoli di Riferimento)
					if(!docsPaWS.UOHasReferenceRole(fasc.ufficioReferente.systemId))
					{
						result = false;
					}
				}
			}
			catch (Exception ex)
			{
				ErrorManager.redirect(this, ex);
			}
			return result;
		}

		/// <summary>
		/// setDescUffRefInModificaFascicolo:
		/// Setta la descrizione dell'Ufficio Referente nel relativo campo della form,
		/// qualora il corrispondente selezionato sia una UO, altrimenti verrà lanciato un 
		/// messaggio d'errore
		/// </summary>
		/// string codiceRubrica: codice rubrica del corrispondente
		/// <returns></returns>
		private void setDescUffRefInModificaFascicolo(string codiceRubrica) 
		{								
			DocsPaWR.Corrispondente corr = null;
			string msg = "Codice rubrica non valido per l\\'Ufficio referente!";
			if(!codiceRubrica.Equals(""))
				corr = UserManager.getCorrispondenteReferente(this, codiceRubrica,false);
			if (corr != null && (corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))))
			{
				this.txt_desc_uff_ref.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
				FascicoliManager.setUoReferenteSelezionato(this.Page,corr);
			} 
			else
			{
				this.txt_desc_uff_ref.Text = "";
				if(!codiceRubrica.Equals(""))
				{
					RegisterStartupScript("alert","<script language='javascript'>alert('" + msg + "');</script>");
					string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_uff_ref.ID + "').focus() </SCRIPT>";
					RegisterStartupScript("focus", s);
				}
			}

		}

		/// <summary>
		/// ChechInput:
		/// Verifica se il campo Ufficio Referente è stato inserito prima di procedere
		/// al salvataggio dei dati
		/// </summary>
		/// <returns></returns>
		private string ChechInput()
		{
			string msg ="";
			if ( (this.txt_cod_uff_ref.Text.Equals("") || this.txt_cod_uff_ref.Text == null) 
				|| (this.txt_desc_uff_ref.Text.Equals("") || this.txt_desc_uff_ref.Text== null)) 
			{		
				msg = "Inserire il valore: Ufficio Referente";						
			}
			return msg;
		}

		private string setCorrispondentiTrasmissione() 
		{
			string esito="";
			try
			{
				
				DocsPaWR.Trasmissione trasmissione = TrasmManager.getGestioneTrasmissione(this);
				//creo l'oggetto qca in caso di trasmissioni a UO
				DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();
				qco.fineValidita = true;
				DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qca = setQCA(qco);
				DocsPaWR.Corrispondente corrRef = FascicoliManager.getUoReferenteSelezionato(this);	
				if (corrRef != null)
				{
					// se siamo di fronte ad una UO, ne individuiamo i ruoli e trasmettiamo ai relativi utenti
					DocsPaWR.Ruolo[] listaRuoli = UserManager.getRuoliRiferimentoAutorizzati(this,qca,(DocsPAWA.DocsPaWR.UnitaOrganizzativa)corrRef);
					if (listaRuoli!=null && listaRuoli.Length>0)
					{
						for (int index=0; index < listaRuoli.Length; index++)
							trasmissione = addTrasmissioneSingola(trasmissione, (DocsPAWA.DocsPaWR.Ruolo) listaRuoli[index]);
					}
					else
					{
						if (esito.Equals("")) 
							esito += "Ruoli di riferimento non trovati o non autorizzati nella: ";
						esito += "\\nUO: " + corrRef.descrizione;
					}
				}
		
				TrasmManager.setGestioneTrasmissione(this, trasmissione);
	
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}	
			return esito;
		}

		//prende la ragione di trasmissione per l'ufficio referente
		private bool getRagTrasmissioneUfficioReferente()
		{
			bool retValue = true;
			bool verificaRagioni;
			trasmissione = TrasmManager.getGestioneTrasmissione(this);
			utente = UserManager.getUtente(this);
			ruolo = UserManager.getRuolo(this);

			//se è null la trasmissione è nuova altrimenti è in modifica
			if (trasmissione == null)
			{
				trasmissione = new DocsPAWA.DocsPaWR.Trasmissione();
				trasmissione.systemId = null;
				trasmissione.ruolo = ruolo;
				trasmissione.utente = utente;
				trasmissione.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;
				trasmissione.infoDocumento = null;
				trasmissione.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(FascicoliManager.getFascicoloSelezionato(this),this);
				TrasmManager.setGestioneTrasmissione(this, trasmissione);
			}
	
			DocsPaWR.RagioneTrasmissione ragTrasm = null;
	
			ragTrasm = FascicoliManager.TrasmettiFascicoloToUoReferente(ruolo, out verificaRagioni); 
			
			if(ragTrasm == null && !verificaRagioni)
			{
				retValue = false;	
			}	
			else
			{
				TrasmManager.setRagioneSel(this,ragTrasm);
			}
			return retValue;
		}

		private DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato setQCA(DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente qco)
		{
			DocsPaWR.AddressbookQueryCorrispondenteAutorizzato qcAut = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato();
			qcAut.tipoOggetto = DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO;			
			qcAut.idNodoTitolario = FascicoliManager.getFascicoloSelezionato(this).idClassificazione;
			qcAut.idRegistro = FascicoliManager.getFascicoloSelezionato(this).idRegistroNodoTit;
			if (qcAut.idRegistro != null && qcAut.idRegistro.Equals(""))
					qcAut.idRegistro = null;
			//cerco la ragione in base all'id che ho nella querystring
			qcAut.ragione = TrasmManager.getRagioneSel(this);
			if (TrasmManager.getGestioneTrasmissione(this) != null)
			{
				qcAut.ruolo = TrasmManager.getGestioneTrasmissione(this).ruolo;
			}
			qcAut.queryCorrispondente = qco;	
			return qcAut;
		}

		private void btn_annulla_ServerClick(object sender, System.EventArgs e)
		{
			Response.Write("<script>window.close();</script>");
			FascicoliManager.removeUoReferenteSelezionato(this);
			FascicoliManager.DO_RemoveLocazioneFisica();
		}

		public DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr) 
		{
			
			if (trasmissione.trasmissioniSingole != null)
			{
				// controllo se esiste la trasmissione singola associata a corrispondente selezionato
				for(int i = 0; i < trasmissione.trasmissioniSingole.Length; i++) 
				{
					DocsPaWR.TrasmissioneSingola ts = (DocsPAWA.DocsPaWR.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
					if (ts.corrispondenteInterno.systemId.Equals(corr.systemId)) 
					{
						if(ts.daEliminare) 
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
			trasmissioneSingola.ragione = TrasmManager.getRagioneSel(this);
			
			// Aggiungo la lista di trasmissioniUtente
			if( corr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo)) 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
				DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr.codiceRubrica);
				if (listaUtenti == null || listaUtenti.Length == 0)
					return trasmissione;
				//ciclo per utenti se dest è gruppo o ruolo
				for(int i= 0; i < listaUtenti.Length; i++) 
				{
					DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
					trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente) listaUtenti[i];
					if(TrasmManager.getRagioneSel(this).descrizione.Equals("RISPOSTA"))
						trasmissioneUtente.idTrasmRispSing=trasmissioneSingola.systemId;
					trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
				}
			}
			else 
			{
				trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
				DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
				trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente) corr;
				trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
			}
			trasmissione.trasmissioniSingole = TrasmManager.addTrasmissioneSingola(trasmissione.trasmissioniSingole, trasmissioneSingola);

			return trasmissione;

		}

		private DocsPAWA.DocsPaWR.Corrispondente[] queryUtenti(string codiceRubrica) 
		{
			
			//costruzione oggetto queryCorrispondente
			DocsPaWR.AddressbookQueryCorrispondente qco = new DocsPAWA.DocsPaWR.AddressbookQueryCorrispondente();

			qco.codiceRubrica = codiceRubrica;
			qco.getChildren = true;
			qco.fineValidita = true;
			
			qco.idAmministrazione=UserManager.getInfoUtente(this).idAmministrazione;// ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];
			
			//corrispondenti interni
			qco.tipoUtente=DocsPaWR.AddressbookTipoUtente.INTERNO;			
			
			DocsPaWR.Corrispondente[] l_corrispondenti=UserManager.getListaCorrispondenti(this.Page,qco);
			
			return pf_getCorrispondentiFiltrati(l_corrispondenti);
	
		}

		private DocsPAWA.DocsPaWR.Corrispondente[] pf_getCorrispondentiFiltrati(DocsPAWA.DocsPaWR.Corrispondente[] corrispondenti)
		{
			string l_oldSystemId="";								
			System.Object[] l_objects=new System.Object[0];
			System.Object[] l_objects_ruoli = new System.Object[0];
			DocsPaWR.Ruolo[] lruolo = new DocsPAWA.DocsPaWR.Ruolo[0];
			int i = 0;
			foreach(DocsPAWA.DocsPaWR.Corrispondente t_corrispondente in corrispondenti)
			{
				string t_systemId=t_corrispondente.systemId;						
				if (t_systemId!=l_oldSystemId)
				{
					l_objects=Utils.addToArray(l_objects,t_corrispondente); 	
					l_oldSystemId=t_systemId;
					i = i + 1 ;
					continue;
				}
				else
				{
					/* il corrispondente non viene aggiunto, in quanto sarebbe un duplicato 
					 * ma viene aggiunto solamente il ruolo */
					
					if(t_corrispondente.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
					{
						if ((l_objects[i - 1]).GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
						{
							l_objects_ruoli = ((Utils.addToArray(((DocsPAWA.DocsPaWR.Utente)(l_objects[i -1])).ruoli, ((DocsPAWA.DocsPaWR.Utente)t_corrispondente).ruoli[0])));			
							DocsPaWR.Ruolo[] l_ruolo=new DocsPAWA.DocsPaWR.Ruolo[l_objects_ruoli.Length];
							((DocsPAWA.DocsPaWR.Utente)(l_objects[i -1])).ruoli = l_ruolo;
							l_objects_ruoli.CopyTo(((DocsPAWA.DocsPaWR.Utente)(l_objects[i -1])).ruoli, 0);
						}
						
					}
				}
				
			}
			
			DocsPaWR.Corrispondente[] l_corrSearch=new DocsPAWA.DocsPaWR.Corrispondente[l_objects.Length];	
			l_objects.CopyTo(l_corrSearch,0);

			return l_corrSearch;
		}

		private void btn_salva_Click(object sender, System.EventArgs e)
		{
			try
			{
                if (this.FascicoloInLavorazione.tipo == "G" ||
                    (this.FascicoloInLavorazione.tipo == "P" && 
                        this.dettaglioNota.ReadOnly && this.dettaglioNota.IsDirty))
                {
                    // Aggiornamento in modalità batch delle sole note solo se:
                    // - il fascicolo è generale
                    // - il fascicolo è procedimentale ma in sola lettura
                    this.UpdateNote();
                    FascicoliManager.setFascicoloSelezionato(this.FascicoloInLavorazione);
                }
                else
                {
                    if (enableUfficioRef && !fasc.tipo.Equals("G"))
                    {
                        string msg = ChechInput();
                        if (!msg.Equals(""))
                        {
                            msg = msg.Replace("'", "\\'");
                            Response.Write("<script>alert('" + msg + "');</script>");
                            string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_uff_ref.ID + "').focus() </SCRIPT>";
                            RegisterStartupScript("focus", s);
                            return;
                        }
                        else
                        {
                            if (!CheckUoReferenteFascicoli())
                            {
                                msg = "Il salvataggio dei dati non può essere effettuato.";
                                msg = msg + "\\nL\\'Ufficio Referente non possiede ruoli di riferimento.";
                                Response.Write("<script>alert('" + msg + "');</script>");
                                string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_uff_ref.ID + "').focus() </SCRIPT>";
                                RegisterStartupScript("focus", s);
                                return;
                            }

                            if (corrRef != null)
                            {
                                if (((DocsPAWA.DocsPaWR.UnitaOrganizzativa)corrRef).systemId != fasc.ufficioReferente.systemId)
                                {
                                    fasc.ufficioReferente = corrRef;
                                    fasc.daAggiornareUfficioReferente = true;
                                }
                                else
                                {
                                    fasc.daAggiornareUfficioReferente = false;
                                }

                            }
                            else
                            {
                                fasc.daAggiornareUfficioReferente = false;
                            }
                        }
                    }
                    if (this.GetCalendarControl("txt_LFDTA").txt_Data.Text != "" && !Utils.isDate(this.GetCalendarControl("txt_LFDTA").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Il formato della data non è valido.');</script>");
                        string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_LFDTA").txt_Data.ID + "').focus();</SCRIPT>";
                        RegisterStartupScript("focus", s);
                        return;
                    }

                    if (this.txt_descFascicolo.Text.Trim() == "")
                    {
                        string errore = "La modifica del fascicolo non può essere effettuata.\\nCi sono dei campi obbligatori non valorizzati.";
                        Response.Write("<script>alert(" + "'" + errore + "'" + ");</script>");
                        return;
                    }

                    //Controllo i campi obbligatori della profilazione dinamica
                    if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    {
                        if (controllaCampiObbligatori())
                        {
                            string errore = "La modifica del fascicolo non può essere effettuata.\\nCi sono dei campi obbligatori non valorizzati.";
                            Response.Write("<script>alert(" + "'" + errore + "'" + ");</script>");
                            return;
                        }
                        else
                        {
                            string messag = ProfilazioneFascManager.verificaOkContatoreFasc((DocsPAWA.DocsPaWR.Templates)Session["template"]);
                            if (messag != string.Empty)
                            {
                                messag = messag.Replace("'", "\\'");
                                Response.Write("<script>alert('" + messag + "');</script>");
                                return;
                            }

                            fasc.template = template;
                        }

                        //Diagrammi di stato
                        if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                        {
                            DocsPaWR.Stato statoAttuale = DocsPAWA.DiagrammiManager.getStatoFasc(FascicoliManager.getFascicoloSelezionato(this).systemID, this);
                            DocsPAWA.DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"];
                            //Stato iniziale
                            if (statoAttuale == null && dg != null) 
                            {
                                DocsPAWA.DiagrammiManager.salvaModificaStatoFasc(fasc.systemID, ddl_statiSuccessivi.SelectedValue, dg, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);
                            }
                            
                            //Stato qualsiasi
                            if(statoAttuale != null && dg != null)
                            {
                                if (ddl_statiSuccessivi.SelectedValue != null && ddl_statiSuccessivi.SelectedValue != "" && dg != null)
                                {
                                    bool statoQualsiasi = true;
                                    //Controllo se lo stato selezionato è sia automatico che finale
                                    if (DocsPAWA.DiagrammiManager.isStatoAuto(ddl_statiSuccessivi.SelectedValue, dg.SYSTEM_ID.ToString(), this) && controllaStatoFinale())
                                    {
                                        msg_StatoFinale.Confirm("Si sta portando il fascicolo in uno stato finale.\\nIl fascicolo verrà chiuso.\\nConfermi il salvataggio ?");
                                        statoQualsiasi = false;
                                    }

                                    //Controllo se lo stato selezioanto è uno stato automatico
                                    if (DocsPAWA.DiagrammiManager.isStatoAuto(ddl_statiSuccessivi.SelectedValue, dg.SYSTEM_ID.ToString(), this))
                                    {
                                        msg_StatoAutomatico.Confirm("Lo stato selezionato è uno stato automatico.\\nConfermi il salvataggio ?");
                                        statoQualsiasi = false;
                                    }

                                    //Controllo se lo stato selezionato è uno stato finale
                                    if (controllaStatoFinale())
                                    {
                                        msg_StatoFinale.Confirm("Si sta portando il fascicolo in uno stato finale.\\nIl fascicolo verrà chiuso.\\nConfermi il salvataggio ?");
                                        statoQualsiasi = false;
                                    }

                                    if (statoQualsiasi)
                                    {
                                        DocsPAWA.DiagrammiManager.salvaModificaStatoFasc(fasc.systemID, ddl_statiSuccessivi.SelectedValue, dg, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);
                                   
                                        //Verifico se effettuare una tramsissione automatica assegnata allo stato
                                        if (fasc.template != null && fasc.template.SYSTEM_ID != 0 && Panel_DiagrammiStato.Visible)
                                        {
                                            ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAutoFasc(UserManager.getInfoUtente(this).idAmministrazione, ddl_statiSuccessivi.SelectedItem.Value, fasc.template.SYSTEM_ID.ToString(), this));
                                            for (int i = 0; i < modelli.Count; i++)
                                            {
                                                DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                                if (mod.SINGLE == "1")
                                                {
                                                    DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, fasc, this);
                                                }
                                                else
                                                {
                                                    for (int j = 0; j < mod.MITTENTE.Length; j++)
                                                    {
                                                        if (mod.MITTENTE[j].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                                        {
                                                            DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, fasc, this);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            ////Se il fascicolo non è in uno stato finale
                            //if (statoAttuale != null && !statoAttuale.STATO_FINALE)
                            //{
                            //    //DocsPAWA.DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"];
                            //    if (ddl_statiSuccessivi.SelectedValue != null && ddl_statiSuccessivi.SelectedValue != "" && dg != null)
                            //    {
                            //        //Controllo se lo stato selezionato è sia automatico che finale
                            //        if (DocsPAWA.DiagrammiManager.isStatoAuto(ddl_statiSuccessivi.SelectedValue, dg.SYSTEM_ID.ToString(), this) && controllaStatoFinale())
                            //        {
                            //            msg_StatoFinale.Confirm("Si sta portando il fascicolo in uno stato finale.\\nIl fascicolo verrà chiuso.\\nConfermi il salvataggio ?");
                            //        }
                            //        else
                            //        {
                            //            //Controllo se lo stato selezioanto è uno stato automatico
                            //            if (DocsPAWA.DiagrammiManager.isStatoAuto(ddl_statiSuccessivi.SelectedValue, dg.SYSTEM_ID.ToString(), this))
                            //            {
                            //                msg_StatoAutomatico.Confirm("Lo stato selezionato è uno stato automatico.\\nConfermi il salvataggio ?");
                            //            }

                            //            //Controllo se lo stato selezionato è uno stato finale
                            //            if (controllaStatoFinale())
                            //            {
                            //                msg_StatoFinale.Confirm("Si sta portando il fascicolo in uno stato finale.\\nIl fascicolo verrà chiuso.\\nConfermi il salvataggio ?");
                            //            }
                            //        }
                            //    }
                            //}
                        }
                    }


                    fasc.descrizione = this.txt_descFascicolo.Text;
                    // Salvataggio dettagli nota
                    this.dettaglioNota.Save();
                    fasc.cartaceo = this.chkFascicoloCartaceo.Checked;


                    fasc.controllato = "0";
                    if (this.chkFascicoloControllato.Visible && this.chkFascicoloControllato.Checked)
                        fasc.controllato = "1";

                    DocsPaVO.LocazioneFisica.LocazioneFisica lf = FascicoliManager.DO_GetLocazioneFisica();
                    if (lf != null)
                    {
                        fasc.idUoLF = lf.UO_ID;
                        fasc.descrizioneUOLF = lf.Descrizione;
                        fasc.varCodiceRubricaLF = lf.CodiceRubrica;

                        if (this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString().Trim() != "")
                        {
                            fasc.dtaLF = this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString();
                        }
                        else
                        {
                            fasc.dtaLF = null;
                        }
                    }

                    FascicoliManager.DO_RemoveLocazioneFisica();
                    FascicoliManager.setFascicolo(this, ref fasc);
                    FascicoliManager.setFascicoloSelezionato(this, fasc);

                    //ufficio referente
                    if (Page.IsPostBack && enableUfficioRef)
                    {

                        if (fasc.daAggiornareUfficioReferente == true)
                        {
                            //l'ufficio referente è stato cambiato quindi viene creata la trasmissione
                            if (!getRagTrasmissioneUfficioReferente())//si ricava la riagione di trasmissione
                            {
                                string theAlert = "<script>alert('Attenzione! Ragione di trasmissione assente per l\\'ufficio referente.";
                                theAlert = theAlert + "\\nLa trasmissione non è stata effettuata.');</script>";
                                Response.Write(theAlert);
                            }
                            else
                            {
                                //Si invia la trasmissione ai ruoli di riferimento autorizzati dell'Ufficio Referente
                                string esito = setCorrispondentiTrasmissione();
                                if (!esito.Equals(""))
                                {
                                    esito = esito.Replace("'", "\\'");
                                    Page.RegisterStartupScript("chiudi", "<script>alert('" + esito + "')</script>");
                                    esito = "";
                                }
                                else
                                {
                                    //richiamo il metodo che salva la trasmissione
                                    DocsPAWA.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();
                                    if (infoUtente.delegato != null)
                                        trasmissione.delegato = ((DocsPAWA.DocsPaWR.InfoUtente)(infoUtente.delegato)).idPeople;

                                    //Nuovo metodo salvaExecuteTrasm
                                    trasmissione.daAggiornare = false;
                                    DocsPaWR.Trasmissione trasm_res = TrasmManager.saveExecuteTrasm(this, trasmissione, infoUtente);
                                    //trasmissione = TrasmManager.saveTrasm(this, trasmissione);
                                    //trasmissione.daAggiornare = false;
                                    //DocsPaWR.Trasmissione trasm_res = TrasmManager.executeTrasm(this, trasmissione);
                                    if (trasm_res != null && trasm_res.ErrorSendingEmails)
                                        Response.Write("<script>window.alert('Non è stato possibile inoltrare una o più e-mail. \\nContattare l\\'amministratore per risolvere il problema.');</script>");
                                }
                            }

                            //rimozione variabili di sessione
                            TrasmManager.removeGestioneTrasmissione(this);
                            TrasmManager.removeRagioneSel(this);
                            fasc.daAggiornareUfficioReferente = false;
                        }

                    }

                    FascicoliManager.removeUoReferenteSelezionato(this);
                }

				Page.RegisterStartupScript("returnY","<script>window.close();</script>");
			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
        }

        #region Profilazione Dinamica

        private void CaricaComboTipologiaFasc()
        {
            if (fasc.template != null)
            {
                ListItem item = new ListItem();
                item.Value = ((DocsPaWR.Templates)fasc.template).SYSTEM_ID.ToString();
                item.Text = ((DocsPaWR.Templates)fasc.template).DESCRIZIONE;
                ddl_tipologiaFasc.Items.Add(item);
                ddl_tipologiaFasc.Enabled = false;
                template = fasc.template;
                Session.Add("template", template);
                inserisciComponenti("NO", template);                  
            }
            else
            {
                ArrayList listaTipiFasc = new ArrayList(ProfilazioneFascManager.getTipoFascFromRuolo(UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "2",this));
                ListItem item = new ListItem();
                item.Value = "";
                item.Text = "";
                ddl_tipologiaFasc.Items.Add(item);
                for (int i = 0; i < listaTipiFasc.Count; i++)
                {
                    DocsPaWR.Templates templates = (DocsPaWR.Templates)listaTipiFasc[i];
                    if (templates.IPER_FASC_DOC != "1")
                    {
                        ListItem item_1 = new ListItem();
                        item_1.Value = templates.SYSTEM_ID.ToString();
                        item_1.Text = templates.DESCRIZIONE;
                        ddl_tipologiaFasc.Items.Add(item_1);
                    }
                }
                
                //Blocco eventualmente la tipologia di fascicolo
                DocsPaWR.FascicolazioneClassificazione classificazione = FascicoliManager.getClassificazioneSelezionata(this);
                if (classificazione != null && classificazione.bloccaTipoFascicolo != null && classificazione.bloccaTipoFascicolo.Equals("SI"))
                    ddl_tipologiaFasc.Enabled = false;
                else
                    ddl_tipologiaFasc.Enabled = true;                
            }

            //DIAGRAMMI DI STATO 
            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                {
                    if (ddl_tipologiaFasc.SelectedValue != null && ddl_tipologiaFasc.SelectedValue != "")
                    {
                        //Verifico se esiste un diagramma di stato associato al tipo di documento
                        DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoFasc(ddl_tipologiaFasc.SelectedValue, (UserManager.getInfoUtente(this)).idAmministrazione,this);
                        //Session.Add("DiagrammaSelezionato", dg);

                        //Popolo la comboBox degli stati
                        if (dg != null)
                        {
                            DocsPaWR.Stato statoAttuale = DocsPAWA.DiagrammiManager.getStatoFasc(FascicoliManager.getFascicoloSelezionato(this).systemID,this);
                            if(statoAttuale != null)
                                popolaComboBoxStatiSuccessivi(statoAttuale, dg);
                            else
                                popolaComboBoxStatiSuccessivi(null, dg);
                            Panel_DiagrammiStato.Visible = true;
                            Session.Add("DiagrammaSelezionato", dg);
                        }
                    }
                }
            }
            //FINE DIAGRAMMI DI STATO 
        }

        private void inserisciComponenti(string readOnly, DocsPaWR.Templates template)
        {
            table = new Table();
            table.Width = Unit.Percentage(100);
            dirittiCampiRuolo = ProfilazioneFascManager.getDirittiCampiTipologiaFasc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), this);
            for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
                ProfilazioneFascManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                {
                    case "CampoDiTesto":
                        inserisciCampoDiTesto(oggettoCustom, readOnly);
                        break;
                    case "CasellaDiSelezione":
                        inserisciCasellaDiSelezione(oggettoCustom, readOnly);
                        break;
                    case "MenuATendina":
                        inserisciMenuATendina(oggettoCustom, readOnly);
                        break;
                    case "SelezioneEsclusiva":
                        inserisciSelezioneEsclusiva(oggettoCustom, readOnly);
                        break;
                    case "Contatore":
                        inserisciContatore(oggettoCustom);
                        break;
                    case "Data":
                        inserisciData(oggettoCustom, readOnly);
                        break;
                    case "Corrispondente":
                        inserisciCorrispondente(oggettoCustom, readOnly);
                        break;
                    case "Link":
                        inserisciLink(oggettoCustom, readOnly);
                        break;
                    case "OggettoEsterno":
                        inserisciOggettoEsterno(oggettoCustom, readOnly);
                        break;

                }
            }
            panel_Contenuto.Controls.Add(table);
        }

        private void ddl_tipologiaFasc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string idTemplate = ddl_tipologiaFasc.SelectedValue;

            if (idTemplate != "")
            {
                Session.Remove("template");
                template = ProfilazioneFascManager.getTemplateFascById(idTemplate,this);
                Session.Add("template", template);
                panel_Contenuto.Controls.Clear();
                inserisciComponenti("NO", template);
            }
            else
            {
                Session.Remove("template");
            }

            //DIAGRAMMI DI STATO 
            string settingValue = ConfigurationManager.AppSettings["ProfilazioneDinamica"];
            if (!string.IsNullOrEmpty(settingValue) && settingValue == "1")
            {
                if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                {
                    if (ddl_tipologiaFasc.SelectedValue != null && ddl_tipologiaFasc.SelectedValue != "")
                    {
                        //Verifico se esiste un diagramma di stato associato al tipo di documento
                        DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoFasc(ddl_tipologiaFasc.SelectedValue, (UserManager.getInfoUtente(this)).idAmministrazione,this);
                        //Session.Add("DiagrammaSelezionato", dg);

                        //Popolo la comboBox degli stati
                        if (dg != null)
                        {
                            DocsPaWR.Stato statoAttuale = DocsPAWA.DiagrammiManager.getStatoFasc(FascicoliManager.getFascicoloSelezionato(this).systemID,this);
                            if (statoAttuale != null)
                                popolaComboBoxStatiSuccessivi(statoAttuale, dg);
                            else
                                popolaComboBoxStatiSuccessivi(null, dg);
                            popolaComboBoxStatiSuccessivi(null, dg);
                            Panel_DiagrammiStato.Visible = true;
                            Session.Add("DiagrammaSelezionato", dg);
                        }                        
                    }
                }
            }
            //FINE DIAGRAMMI DI STATO 
        }

        #region InserisciCampoDiTesto
        public void inserisciCampoDiTesto(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaCampoDiTesto = new Label();
            TextBox txt_CampoDiTesto = new TextBox();
            if (oggettoCustom.MULTILINEA.Equals("SI"))
            {
                if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE + " *";
                }
                else
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;
                }

                etichettaCampoDiTesto.CssClass = "titolo_scheda";

                //txt_CampoDiTesto.Width = 450;
                txt_CampoDiTesto.Width = Unit.Percentage(100);
				txt_CampoDiTesto.TextMode = TextBoxMode.MultiLine;

                if (oggettoCustom.NUMERO_DI_LINEE.Equals(""))
                {
                    txt_CampoDiTesto.Height = 55;
                }
                else
                {
                    txt_CampoDiTesto.Rows = Convert.ToInt32(oggettoCustom.NUMERO_DI_LINEE);
                }

                if (oggettoCustom.NUMERO_DI_CARATTERI.Equals(""))
                {
                    txt_CampoDiTesto.MaxLength = 150;
                }
                else
                {
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }

                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;

                TableRow row_1 = new TableRow();
                TableCell cell_1 = new TableCell();
                cell_1.Controls.Add(etichettaCampoDiTesto);
                cell_1.ColumnSpan = 2;
                row_1.Cells.Add(cell_1);
                table.Rows.Add(row_1);

                TableRow row_2 = new TableRow();
                TableCell cell_2 = new TableCell();
                cell_2.Controls.Add(txt_CampoDiTesto);
                cell_2.ColumnSpan = 2;
                row_2.Cells.Add(cell_2);
                table.Rows.Add(row_2);
            }
            else
            {
                if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE + " *";
                }
                else
                {
                    etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;
                }

                etichettaCampoDiTesto.CssClass = "titolo_scheda";

                if (!oggettoCustom.NUMERO_DI_CARATTERI.Equals(""))
                {
                    //ATTENZIONE : La lunghezza della textBox non è speculare al numero massimo di
                    //caratteri che l'utente inserisce.
                    txt_CampoDiTesto.Width = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6;
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }
                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.CssClass = "testo_grigio";

                TableRow row = new TableRow();
                TableCell cell_1 = new TableCell();
                cell_1.Controls.Add(etichettaCampoDiTesto);
                row.Cells.Add(cell_1);
                TableCell cell_2 = new TableCell();
                cell_2.Controls.Add(txt_CampoDiTesto);
                row.Cells.Add(cell_2);
                table.Rows.Add(row);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaCampoDiTesto, txt_CampoDiTesto, oggettoCustom, template);    
        }
        #endregion inserisciCampoDiTesto

        #region InserisciCasellaDiSelezione
        public void inserisciCasellaDiSelezione(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaCasellaSelezione = new Label();
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE;
            }

            //etichettaCasellaSelezione.Width = 430;
            etichettaCasellaSelezione.Width = Unit.Percentage(100);
			etichettaCasellaSelezione.CssClass = "titolo_scheda";

            CheckBoxList casellaSelezione = new CheckBoxList();
            casellaSelezione.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreElenco = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                if (i < oggettoCustom.VALORI_SELEZIONATI.Length)
                {
                    string valoreSelezionato = (string)(oggettoCustom.VALORI_SELEZIONATI[i]);
                    if (valoreElenco.ABILITATO == 1 || (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato)))
                    {
                        //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                        if (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato))
                            valoreElenco.ABILITATO = 1;

                        casellaSelezione.Items.Add(new ListItem(valoreElenco.VALORE, valoreElenco.VALORE));
                        //Valore di default
                        if (valoreElenco.VALORE_DI_DEFAULT.Equals("SI"))
                        {
                            valoreDiDefault = i;
                        }
                    }
                }
            } 
            casellaSelezione.CssClass = "testo_grigio";
            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Vertical;
            }
            if (valoreDiDefault != -1)
            {
                casellaSelezione.SelectedIndex = valoreDiDefault;
            }

            if (oggettoCustom.VALORI_SELEZIONATI != null)
            {
                impostaSelezioneCaselleDiSelezione(oggettoCustom, casellaSelezione);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaCasellaSelezione, casellaSelezione, oggettoCustom, template);

            TableRow row_1 = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaCasellaSelezione);
            cell_1.ColumnSpan = 2;
            row_1.Cells.Add(cell_1);
            table.Rows.Add(row_1);

            TableRow row_2 = new TableRow();
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(casellaSelezione);
            cell_2.ColumnSpan = 2;
            row_2.Cells.Add(cell_2);
            table.Rows.Add(row_2);
        }
        #endregion inserisciCasellaDiSelezione

        #region InserisciMenuATendina
        public void inserisciMenuATendina(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaMenuATendina = new Label();
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE;
            }

            etichettaMenuATendina.CssClass = "titolo_scheda";

            DropDownList menuATendina = new DropDownList();
            menuATendina.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    menuATendina.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    {
                        valoreDiDefault = i;
                    }
                }
            }
            menuATendina.CssClass = "testo_grigio";
            if (valoreDiDefault != -1)
            {
                menuATendina.SelectedIndex = valoreDiDefault;
            }
            if (!(valoreDiDefault != -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
            {
                menuATendina.Items.Insert(0, "");
            }
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                //menuATendina.SelectedIndex = Convert.ToInt32(oggettoCustom.VALORE_DATABASE);
                menuATendina.SelectedIndex = impostaSelezioneMenuATendina(oggettoCustom.VALORE_DATABASE, menuATendina);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaMenuATendina, menuATendina, oggettoCustom, template);

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaMenuATendina);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(menuATendina);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
        }
        #endregion inserisciMenuATendina

        #region InserisciSelezioneEsclusiva
        public void inserisciSelezioneEsclusiva(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            TableRow row_1 = new TableRow();
            TableCell cell_1 = new TableCell();

            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaSelezioneEsclusiva = new Label();
            HtmlAnchor cancella_selezioneEsclusiva = new HtmlAnchor();

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;

                cancella_selezioneEsclusiva.HRef = "javascript:clearSelezioneEsclusiva(" + oggettoCustom.SYSTEM_ID.ToString() + "," + oggettoCustom.ELENCO_VALORI.Length + ");";
                cancella_selezioneEsclusiva.InnerHtml = "<img src=\"../images/cancella.gif\" width=\"10\" height=\"10\" border=\"0\" alt=\"Cacella selezione esclusiva\" class=\"resettaSelezioneEsclusiva\">";
                cell_1.Controls.Add(cancella_selezioneEsclusiva);
            }

            //etichettaSelezioneEsclusiva.Width = 400;
            etichettaSelezioneEsclusiva.Width = Unit.Percentage(90);
			etichettaSelezioneEsclusiva.CssClass = "titolo_scheda";

            RadioButtonList selezioneEsclusiva = new RadioButtonList();
            selezioneEsclusiva.AutoPostBack = false;
            selezioneEsclusiva.ID = oggettoCustom.SYSTEM_ID.ToString();
            int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPAWA.DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    selezioneEsclusiva.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    {
                        valoreDiDefault = i;
                    }
                }
            }
            selezioneEsclusiva.CssClass = "testo_grigio";
            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Vertical;
            }
            if (valoreDiDefault != -1)
            {
                selezioneEsclusiva.SelectedIndex = valoreDiDefault;
            }
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                //selezioneEsclusiva.SelectedIndex = Convert.ToInt32(oggettoCustom.VALORE_DATABASE);
                selezioneEsclusiva.SelectedIndex = impostaSelezioneEsclusiva(oggettoCustom.VALORE_DATABASE, selezioneEsclusiva);
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSelezioneEsclusiva(etichettaSelezioneEsclusiva, selezioneEsclusiva, cancella_selezioneEsclusiva, oggettoCustom, template);

            cell_1.Controls.Add(etichettaSelezioneEsclusiva);
            cell_1.ColumnSpan = 2;
            row_1.Cells.Add(cell_1);
            table.Rows.Add(row_1);

            TableRow row_2 = new TableRow();
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(selezioneEsclusiva);
            cell_2.ColumnSpan = 2;
            row_2.Cells.Add(cell_2);
            table.Rows.Add(row_2);
        }
        #endregion inserisciSelezioneEsclusiva

        #region InserisciContatore
        public void inserisciContatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaContatore = new Label();
            etichettaContatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaContatore.CssClass = "titolo_scheda";

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaContatore);
            row.Cells.Add(cell_1);

            TableCell cell_2 = new TableCell();
            
            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            DropDownList ddl = new DropDownList();

            if (oggettoCustom.VALORE_DATABASE == null || oggettoCustom.VALORE_DATABASE == "")
            {
                DocsPaWR.Ruolo ruoloUtente = UserManager.getRuolo(this);
                DocsPaWR.Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, "", "");
                
                switch (oggettoCustom.TIPO_CONTATORE)
                {
                    case "T":
                        break;
                    case "A":
                        etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                        etichettaDDL.CssClass = "titolo_scheda";
                        etichettaDDL.Width = 30;
                        cell_2.Controls.Add(etichettaDDL);
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "comp_profilazione_anteprima";
                        //Distinguere se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "0")
                            {
                                ListItem item = new ListItem();
                                item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = "";
                            it.Value = "";
                            ddl.Items.Add(it);
                            ddl.SelectedValue = "";
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;

                        ddl.Width = 100;
                        cell_2.Controls.Add(ddl);
                        /*
                        Label etichettaAoo = new Label();
                        etichettaAoo.Text = "&nbsp;AOO&nbsp;";
                        etichettaAoo.CssClass = "titolo_scheda";
                        etichettaAoo.Width = 30;
                        cell_2.Controls.Add(etichettaAoo);
                        DropDownList ddlAoo = new DropDownList();
                        ddlAoo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddlAoo.CssClass = "comp_profilazione_anteprima";
                        //Distinguere se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "0")
                            {
                                ListItem item = new ListItem();
                                item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                ddlAoo.Items.Add(item);
                            }
                        }
                        if (ddlAoo.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = "";
                            it.Value = "";
                            ddlAoo.Items.Add(it);
                            ddlAoo.SelectedValue = "";
                        }
                        else
                            ddlAoo.SelectedValue = oggettoCustom.ID_AOO_RF;

                        ddlAoo.Width = 100;
                        cell_2.Controls.Add(ddlAoo);
                        */
                        break;
                    case "R":
                        etichettaDDL.Text = "&nbsp;RF&nbsp;";
                        etichettaDDL.CssClass = "titolo_scheda";
                        etichettaDDL.Width = 34;
                        cell_2.Controls.Add(etichettaDDL);
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "comp_profilazione_anteprima";
                        //Distinguere se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "1" && ((DocsPaWR.Registro)registriRfVisibili[i]).rfDisabled == "0")
                            {
                                ListItem item = new ListItem();
                                item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }
                        if (ddl.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = "";
                            it.Value = "";
                            ddl.Items.Add(it);
                            ddl.SelectedValue = "";
                        }
                        else
                            ddl.SelectedValue = oggettoCustom.ID_AOO_RF;

                        ddl.Width = 100;
                        cell_2.Controls.Add(ddl);
                        /*
                        Label etichettaRf = new Label();
                        etichettaRf.Text = "&nbsp;RF&nbsp;";
                        etichettaRf.CssClass = "titolo_scheda";
                        etichettaRf.Width = 34;
                        cell_2.Controls.Add(etichettaRf);
                        DropDownList ddlRf = new DropDownList();
                        ddlRf.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddlRf.CssClass = "comp_profilazione_anteprima";
                        //Distinguere se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            if (((DocsPaWR.Registro)registriRfVisibili[i]).chaRF == "1" && ((DocsPaWR.Registro)registriRfVisibili[i]).rfDisabled == "0")
                            {
                                ListItem item = new ListItem();
                                item.Value = ((DocsPaWR.Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((DocsPaWR.Registro)registriRfVisibili[i]).codRegistro;
                                ddlRf.Items.Add(item);
                            }
                        }
                        if (ddlRf.Items.Count > 1)
                        {
                            ListItem it = new ListItem();
                            it.Text = "";
                            it.Value = "";
                            ddlRf.Items.Add(it);
                            ddlRf.SelectedValue = "";
                        }
                        else
                            ddlRf.SelectedValue = oggettoCustom.ID_AOO_RF;

                        ddlRf.Width = 100;
                        cell_2.Controls.Add(ddlRf);
                        */
                        break;
                }

                
            }

            TextBox contatore = new TextBox();
            contatore.ID = oggettoCustom.SYSTEM_ID.ToString();
            if (oggettoCustom.FORMATO_CONTATORE != "")
            {
                contatore.Text = oggettoCustom.FORMATO_CONTATORE;
                if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                {
                    contatore.Text = contatore.Text.Replace("ANNO", oggettoCustom.ANNO);
                    contatore.Text = contatore.Text.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);
                    string codiceAmministrazione = UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice;
                    contatore.Text = contatore.Text.Replace("COD_AMM", codiceAmministrazione);
                    contatore.Text = contatore.Text.Replace("COD_UO", oggettoCustom.CODICE_DB);
                    contatore.Text = contatore.Text.Replace("gg/mm/aaaa", oggettoCustom.DATA_INSERIMENTO);
                    if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
                    {
                        DocsPAWA.DocsPaWR.Registro reg = UserManager.getRegistroBySistemId(this,oggettoCustom.ID_AOO_RF);
                        if (reg != null)
                        {
                            contatore.Text = contatore.Text.Replace("RF", reg.codRegistro);
                            contatore.Text = contatore.Text.Replace("AOO", reg.codRegistro);
                        }
                    }
                }
                else
                {
                    contatore.Text = "";
                    //contatore.Text = contatore.Text.Replace("ANNO", "");
                    //contatore.Text = contatore.Text.Replace("CONTATORE", "");
                    //contatore.Text = contatore.Text.Replace("RF", "");
                    //contatore.Text = contatore.Text.Replace("AOO", "");
                    //contatore.Text = contatore.Text.Replace("COD_AMM", "");
                    //contatore.Text = contatore.Text.Replace("COD_UO", "");
                    //contatore.Text = contatore.Text.Replace("gg/mm/aaaa", "");
                }
            }
            else
            {
                contatore.Text = oggettoCustom.VALORE_DATABASE;
            }

            CheckBox cbContaDopo = new CheckBox();

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloContatore(etichettaContatore, contatore, cbContaDopo, etichettaDDL, ddl, oggettoCustom, template);

            contatore.Width = Unit.Percentage(60);
            contatore.Enabled = false;
            contatore.BackColor = System.Drawing.Color.WhiteSmoke;
            contatore.CssClass = "testo_grigio";
            contatore.Style.Add("TEXT-ALIGN", "right");
            cell_2.Controls.Add(contatore);
            row.Cells.Add(cell_2);

            //Inserisco il cb per il conta dopo
            if (oggettoCustom.CONTA_DOPO == "1")
            {
                cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
                cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";
                cell_2.Controls.Add(cbContaDopo);
            }

            table.Rows.Add(row);
        }
        #endregion inserisciContatore

        #region InserisciData
        public void inserisciData(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            //Per il momento questo tipo di campo è stato implementato con tre semplici textBox
            //Sarebbe opportuno creare un oggetto personalizzato, che espone le stesse funzionalità
            //della textBox, ma che mi permette di gestire la data con i tre campi separati.
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichettaData = new Label();
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaData.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaData.Text = oggettoCustom.DESCRIZIONE;
            }
            etichettaData.CssClass = "titolo_scheda";

            DocsPAWA.UserControls.Calendar data = (DocsPAWA.UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data.fromUrl = "../UserControls/DialogCalendar.aspx";
            data.CSS = "testo_grigio";
            data.ID = oggettoCustom.SYSTEM_ID.ToString();
            data.VisibleTimeMode = ProfilazioneFascManager.getVisibleTimeMode(oggettoCustom);

            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
                //data.txt_Data.Text = oggettoCustom.VALORE_DATABASE;
                data.Text = oggettoCustom.VALORE_DATABASE;

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichettaData, data, oggettoCustom, template);

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichettaData);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(data);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);   	
        }
        #endregion inserisciData

        #region inserisciCorrispondente
        public void inserisciCorrispondente(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }

            Label etichetta = new Label();
            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            //etichetta.Font.Size = FontUnit.Point(8);
            //etichetta.Font.Bold = true;
            //etichetta.Font.Name = "Verdana";
            etichetta.CssClass = "titolo_scheda";

            DocsPAWA.UserControls.Corrispondente corrispondente = (DocsPAWA.UserControls.Corrispondente)this.LoadControl("../UserControls/Corrispondente.ascx");
            corrispondente.CSS_CODICE = "testo_grigio";
            corrispondente.CSS_DESCRIZIONE = "testo_grigio";
            corrispondente.DESCRIZIONE_READ_ONLY = true;
            corrispondente.TIPO_CORRISPONDENTE = oggettoCustom.TIPO_RICERCA_CORR;
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();

            //Da amministrazione è stato impostato un ruolo di default per questo campo.
            if (oggettoCustom.ID_RUOLO_DEFAULT != null && oggettoCustom.ID_RUOLO_DEFAULT != "" && oggettoCustom.ID_RUOLO_DEFAULT != "0")
            {
                DocsPaWR.Ruolo ruolo = (DocsPaWR.Ruolo) UserManager.getRuoloById(oggettoCustom.ID_RUOLO_DEFAULT,this);
                if (ruolo != null)
                {
                    corrispondente.SYSTEM_ID_CORR = ruolo.systemId;
                    corrispondente.CODICE_TEXT = ruolo.codiceRubrica;
                    corrispondente.DESCRIZIONE_TEXT = ruolo.descrizione;
                }
                oggettoCustom.ID_RUOLO_DEFAULT = "0";
            }
            //Il campo è valorizzato.
            if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
            {
                DocsPAWA.DocsPaWR.Corrispondente corr_1 = (DocsPAWA.DocsPaWR.Corrispondente) UserManager.getCorrispondenteBySystemID(this,oggettoCustom.VALORE_DATABASE);
                if (corr_1 != null)
                {
                    corrispondente.SYSTEM_ID_CORR = corr_1.systemId;
                    corrispondente.CODICE_TEXT = corr_1.codiceRubrica;
                    corrispondente.DESCRIZIONE_TEXT = corr_1.descrizione;
                }
                oggettoCustom.VALORE_DATABASE = "";
            }
            //E' stato selezionato un corrispondente da rubrica.
            if (Session["rubrica.campoCorrispondente"] != null)
            {
                DocsPAWA.DocsPaWR.Corrispondente corr_3 = (DocsPAWA.DocsPaWR.Corrispondente)Session["rubrica.campoCorrispondente"];
                if (corr_3 != null)
                {
                    //Verifico che l'id del campo sia quello che mi interessa.
                    //Questo id viene messo in sessione dallo UserControl e serve a 
                    //distinguere i diversi campi corrispondete che una popup di profilazione puo' contenere
                    if (Session["rubrica.idCampoCorrispondente"] != null && Session["rubrica.idCampoCorrispondente"].ToString() == corrispondente.ID)
                    {
                        corrispondente.SYSTEM_ID_CORR = corr_3.systemId;
                        corrispondente.CODICE_TEXT = corr_3.codiceRubrica;
                        corrispondente.DESCRIZIONE_TEXT = corr_3.descrizione;
                        Session.Remove("rubrica.campoCorrispondente");
                        Session.Remove("rubrica.idCampoCorrispondente");
                    }
                }
            }

            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, corrispondente, oggettoCustom, template);
            
            if (readOnly == "SI")
                corrispondente.CODICE_READ_ONLY = true;

            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichetta);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            corrispondente.WIDTH_CODICE = 60;
            corrispondente.WIDTH_DESCRIZIONE = 200;
            cell_2.Controls.Add(corrispondente);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
        }
        #endregion inserisciCorrispondente

        #region inserisciLink
        public void inserisciLink(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichetta = new Label();
            if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            etichetta.CssClass = "titolo_scheda";
            DocsPAWA.UserControls.LinkDocFasc link = (DocsPAWA.UserControls.LinkDocFasc)this.LoadControl("../UserControls/LinkDocFasc.ascx");
            link.ID = oggettoCustom.SYSTEM_ID.ToString();
            link.TextCssClass = "testo_grigio";
            link.IsEsterno = (oggettoCustom.TIPO_LINK.Equals("ESTERNO"));
            link.IsFascicolo = ("FASCICOLO".Equals(oggettoCustom.TIPO_OBJ_LINK));
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, link, oggettoCustom, template);
            link.HideLink = true;
            link.Value = oggettoCustom.VALORE_DATABASE;
            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichetta);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(link);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
        }
        #endregion inserisciLink

        #region inserisciOggettoEsterno
        public void inserisciOggettoEsterno(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
        {
            if (oggettoCustom.DESCRIZIONE.Equals(""))
            {
                return;
            }
            Label etichetta = new Label();
            if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            etichetta.CssClass = "titolo_scheda";
            DocsPaWA.UserControls.IntegrationAdapter intAd = (DocsPaWA.UserControls.IntegrationAdapter)this.LoadControl("../UserControls/IntegrationAdapter.ascx");
            intAd.ID = oggettoCustom.SYSTEM_ID.ToString();
            intAd.View = DocsPaWA.UserControls.IntegrationAdapterView.INSERT_MODIFY;
            intAd.CssClass = "testo_grigio";
            intAd.ManualInsertCssClass = "testo_red";
            intAd.IsFasc = true;
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, intAd, oggettoCustom, template);
            intAd.ConfigurationValue = oggettoCustom.CONFIG_OBJ_EST;
            IntegrationAdapterValue value = new IntegrationAdapterValue(oggettoCustom.CODICE_DB, oggettoCustom.VALORE_DATABASE, oggettoCustom.MANUAL_INSERT);
            TableRow row = new TableRow();
            TableCell cell_1 = new TableCell();
            cell_1.Controls.Add(etichetta);
            row.Cells.Add(cell_1);
            TableCell cell_2 = new TableCell();
            cell_2.Controls.Add(intAd);
            row.Cells.Add(cell_2);
            table.Rows.Add(row);
        }
        #endregion inserisciOggettoEsterno

        #region Utility
        private bool controllaCampiObbligatori()
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione.

            DocsPaWR.Templates template = (DocsPaWR.Templates)Session["template"];
            if (template != null)
            {
                for (int j = 0; j < template.ELENCO_OGGETTI.Length; j++)
                {
                    DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[j];

                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            TextBox textBox = (TextBox)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            if (textBox.Text.Equals("") && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            {
                                return true;
                            }
                            oggettoCustom.VALORE_DATABASE = textBox.Text;
                            break;
                        case "CasellaDiSelezione":
                            CheckBoxList casellaSelezione = (CheckBoxList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            if (casellaSelezione.SelectedIndex == -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            {
                                for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                                    oggettoCustom.VALORI_SELEZIONATI[i] = null;
                                return true;
                            }

                            //Controllo eventuali selezioni
                            oggettoCustom.VALORI_SELEZIONATI = new string[oggettoCustom.ELENCO_VALORI.Length];
                            oggettoCustom.VALORE_DATABASE = "";

                            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
                            {
                                DocsPaWR.ValoreOggetto valoreOggetto = (DocsPaWR.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i];
                                foreach (ListItem valoreSelezionato in casellaSelezione.Items)
                                {
                                    if (valoreOggetto.VALORE == valoreSelezionato.Text && valoreSelezionato.Selected)
                                        oggettoCustom.VALORI_SELEZIONATI[i] = valoreSelezionato.Text;
                                }
                            }
                            break;
                        case "MenuATendina":
                            DropDownList dropDwonList = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            if (dropDwonList.SelectedItem.Text.Equals("") && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            {
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }
                            oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                            break;
                        case "SelezioneEsclusiva":
                            RadioButtonList radioButtonList = (RadioButtonList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            if ((oggettoCustom.VALORE_DATABASE == "-1" && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
                            {
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }
                            if (oggettoCustom.VALORE_DATABASE == "-1")
                            {
                                oggettoCustom.VALORE_DATABASE = "";
                            }
                            else
                            {
                                if (radioButtonList.SelectedItem != null)
                                    oggettoCustom.VALORE_DATABASE = radioButtonList.SelectedItem.Text;
                            }
                            break;
                        case "Data":
                            UserControls.Calendar data = (UserControls.Calendar)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            //if (data.txt_Data.Text.Equals("") && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            if (data.Text.Equals("") && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                            {
                                SetFocus(data.txt_Data);
                                return true;
                            }
                            //if (data.txt_Data.Text.Equals(""))
                            if (data.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            else
                                //oggettoCustom.VALORE_DATABASE = data.txt_Data.Text;
                                oggettoCustom.VALORE_DATABASE = data.Text;
                            break;
                        case "Corrispondente":
                            UserControls.Corrispondente corr = (UserControls.Corrispondente)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            DocsPaWR.Corrispondente corrispondente = UserManager.getCorrispondenteByCodRubrica(this,corr.CODICE_TEXT);

                            //Correzione fatta quando un corrispondente non è visibile al destinatario di una trasmissione
                            if (corrispondente == null)
                                corrispondente = UserManager.getCorrispondenteBySystemIDDisabled(this, corr.SYSTEM_ID_CORR);
                            //Fine Correzione

                            if (    (corr.CODICE_TEXT == "" && 
                                    corr.DESCRIZIONE_TEXT == "" && 
                                    oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                                    ||
                                    (corrispondente == null && 
                                    oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                                )
                            {
                                return true;
                            }
                            if (corrispondente != null)
                                oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                            else
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                        case "Contatore":
                            if (oggettoCustom.VALORE_DATABASE == null || oggettoCustom.VALORE_DATABASE == "")
                            {
                                switch (oggettoCustom.TIPO_CONTATORE)
                                {
                                    case "A":
                                        DropDownList ddlAoo = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                                        oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                                        break;
                                    case "R":
                                        DropDownList ddlRf = (DropDownList)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                                        oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                                        break;
                                }
                                if (oggettoCustom.CONTA_DOPO == "1")
                                {
                                    CheckBox cbContaDopo = (CheckBox)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo");
                                    oggettoCustom.CONTATORE_DA_FAR_SCATTARE = cbContaDopo.Checked;
                                    //Verifico che il calcolo del contatore si abilitato
                                    //In caso affermativo metto in sessione un valore che mi permetterà poi di riaprire o meno
                                    //la popup di profilazione, per far verificare il numero generato.
                                    if (cbContaDopo.Checked)
                                    {
                                        Session.Add("contaDopoChecked", true);
                                    }
                                }
                                else
                                {
                                    oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                }
                            }
                            break;
                        case "Link":
                            UserControls.LinkDocFasc link = (UserControls.LinkDocFasc)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            string value = link.Value;
                            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI") && string.IsNullOrEmpty(value)) return true;
                            if (string.IsNullOrEmpty(value))
                            {
                                oggettoCustom.VALORE_DATABASE = "";
                            }
                            else
                            {
                                oggettoCustom.VALORE_DATABASE = value;
                            }
                            break;
                        case "OggettoEsterno":
                            IntegrationAdapter intAd = (IntegrationAdapter)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                            IntegrationAdapterValue val = intAd.Value;
                            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI") && val == null) return true;
                            if (val == null)
                            {
                                oggettoCustom.VALORE_DATABASE = "";
                                oggettoCustom.CODICE_DB = "";
                                oggettoCustom.MANUAL_INSERT = false;
                            }
                            else
                            {
                                oggettoCustom.VALORE_DATABASE = val.Descrizione;
                                oggettoCustom.CODICE_DB = val.Codice;
                                oggettoCustom.MANUAL_INSERT = val.ManualInsert;
                            }
                            break;
                    }
                }
            }
            return false;
        }

        private void impostaSelezioneCaselleDiSelezione(DocsPAWA.DocsPaWR.OggettoCustom objCustom, CheckBoxList cbl)
        {
            for (int i = 0; i < objCustom.VALORI_SELEZIONATI.Length; i++)
            {
                for (int j = 0; j < cbl.Items.Count; j++)
                {
                    if ((string)objCustom.VALORI_SELEZIONATI[i] == cbl.Items[j].Text)
                    {
                        cbl.Items[j].Selected = true;
                    }
                }
            }
        }

        private int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        private int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
        {
            for (int i = 0; i < rbl.Items.Count; i++)
            {
                if (rbl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        private void popolaComboBoxStatiSuccessivi(DocsPAWA.DocsPaWR.Stato stato, DocsPAWA.DocsPaWR.DiagrammaStato diagramma)
        {
            //Inizializzazione
            ddl_statiSuccessivi.Items.Clear();
            ListItem itemEmpty = new ListItem();
            ddl_statiSuccessivi.Items.Add(itemEmpty);

            //Popola la combo con gli stati iniziali del diagramma
            if (stato == null)
            {
                for (int i = 0; i < diagramma.STATI.Length; i++)
                {
                    DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)diagramma.STATI[i];
                    if (st.STATO_INIZIALE)
                    {
                        ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                        ddl_statiSuccessivi.Items.Add(item);
                    }
                }
                if (ddl_statiSuccessivi.Items.Count == 2)
                    ddl_statiSuccessivi.SelectedIndex = 1;
            }
            //Popola la combo con i possibili stati, successivi a quello passato
            else
            {
                for (int i = 0; i < diagramma.PASSI.Length; i++)
                {
                    DocsPaWR.Passo step = (DocsPAWA.DocsPaWR.Passo)diagramma.PASSI[i];
                    if (step.STATO_PADRE.SYSTEM_ID == stato.SYSTEM_ID)
                    {
                        for (int j = 0; j < step.SUCCESSIVI.Length; j++)
                        {
                            DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)step.SUCCESSIVI[j];
                            ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                            ddl_statiSuccessivi.Items.Add(item);
                        }
                    }
                }
                //Controllo che non sia uno stato finale
                if (stato.STATO_FINALE)
                {
                    ddl_statiSuccessivi.Items.Clear();
                    ListItem item = new ListItem(stato.DESCRIZIONE, Convert.ToString(stato.SYSTEM_ID));
                    ddl_statiSuccessivi.Items.Add(item);
                    ddl_statiSuccessivi.Enabled = false;
                }
            }
        }

        protected void msg_StatoAutomatico_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            DocsPAWA.DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"];
            fasc = FascicoliManager.getFascicoloSelezionato();
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                DocsPAWA.DiagrammiManager.salvaModificaStatoFasc(fasc.systemID, ddl_statiSuccessivi.SelectedValue, dg, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);

                //Verifico se effettuare una tramsissione automatica assegnata allo stato
                if (fasc.template != null && fasc.template.SYSTEM_ID != null && fasc.template.SYSTEM_ID != 0 && Panel_DiagrammiStato.Visible)
                {
                    ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAutoFasc(UserManager.getInfoUtente(this).idAmministrazione, ddl_statiSuccessivi.SelectedItem.Value, fasc.template.SYSTEM_ID.ToString(), this));
                    for (int i = 0; i < modelli.Count; i++)
                    {
                        DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                        //Nel caso vengo da toDoList il registro non è impostato quindi l'id lo recupero dal ruolo
                        if (mod.SINGLE == "1")
                        {
                            DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, fasc, this);
                        }
                        else
                        {
                            for (int k = 0; k < mod.MITTENTE.Length; k++)
                            {
                                if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                {
                                    DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, fasc, this);
                                    break;
                                }
                            }
                        }
                    }
                }

                ClientScript.RegisterStartupScript(this.GetType(), "closePage", "window.close();", true);
            }
        }

        protected void msg_StatoFinale_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            DocsPAWA.DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"];
            fasc = FascicoliManager.getFascicoloSelezionato();
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                DocsPAWA.DiagrammiManager.salvaModificaStatoFasc(fasc.systemID, ddl_statiSuccessivi.SelectedValue, dg, UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), "", this);
                DateTime ora = DateTime.Now;
                fasc.chiusura = ora.ToString("dd/MM/yyyy");
                fasc.stato = "C";
                FascicoliManager.setFascicolo(this, ref fasc);
                FascicoliManager.setFascicoloSelezionato(this, fasc);

                //Verifico se effettuare una tramsissione automatica assegnata allo stato
                if (fasc.template != null && fasc.template.SYSTEM_ID != null && fasc.template.SYSTEM_ID != 0 && Panel_DiagrammiStato.Visible)
                {
                    ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAutoFasc(UserManager.getInfoUtente(this).idAmministrazione, ddl_statiSuccessivi.SelectedItem.Value, fasc.template.SYSTEM_ID.ToString(), this));
                    for (int i = 0; i < modelli.Count; i++)
                    {
                        DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                        //Nel caso vengo da toDoList il registro non è impostato quindi l'id lo recupero dal ruolo
                        if (mod.SINGLE == "1")
                        {
                            DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, fasc, this);
                        }
                        else
                        {
                            for (int k = 0; k < mod.MITTENTE.Length; k++)
                            {
                                if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                {
                                    DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, fasc, this);
                                    break;
                                }
                            }
                        }
                    }
                }

                ClientScript.RegisterStartupScript(this.GetType(), "closePage", "window.close();", true);
            }
        }

        private bool controllaStatoFinale()
        {
            DocsPaWR.DiagrammaStato dg = (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"];
            for (int i = 0; i < dg.STATI.Length; i++)
            {
                DocsPaWR.Stato st = (DocsPAWA.DocsPaWR.Stato)dg.STATI[i];
                if (st.SYSTEM_ID.ToString() == ddl_statiSuccessivi.SelectedValue && st.STATO_FINALE)
                    return true;
            }
            return false;
        }

        #endregion        
        
        #region imposta diritti ruolo sul campo
        public void impostaDirittiRuoloSulCampo(Object etichetta, Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneFascManager.getDirittiCampoTipologiaFasc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1")
                            {
                                ((System.Web.UI.WebControls.TextBox)campo).ReadOnly = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.TextBox)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "CasellaDiSelezione":
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1")
                            {
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Enabled = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "MenuATendina":
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1")
                            {
                                ((System.Web.UI.WebControls.DropDownList)campo).Enabled = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.DropDownList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "SelezioneEsclusiva":
                            //Per la selezione esclusiva è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Contatore":
                            //Per il contatore è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Data":
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1")
                            {
                                ((DocsPAWA.UserControls.Calendar)campo).ReadOnly = true;
                                ((DocsPAWA.UserControls.Calendar)campo).btn_Cal.Enabled = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPAWA.UserControls.Calendar)campo).Visible = false;
                                ((DocsPAWA.UserControls.Calendar)campo).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                                ((DocsPAWA.UserControls.Calendar)campo).btn_Cal.Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Corrispondente":
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1")
                            {
                                ((DocsPAWA.UserControls.Corrispondente)campo).CODICE_READ_ONLY = true;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPAWA.UserControls.Corrispondente)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Link":
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1")
                            {
                                ((DocsPAWA.UserControls.LinkDocFasc)campo).IsInsertModify = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            else
                            {
                                ((DocsPAWA.UserControls.LinkDocFasc)campo).IsInsertModify = true;
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPAWA.UserControls.LinkDocFasc)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "OggettoEsterno":
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1")
                            {
                                ((DocsPaWA.UserControls.IntegrationAdapter)campo).View = DocsPaWA.UserControls.IntegrationAdapterView.READ_ONLY;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            else
                            {
                                ((DocsPaWA.UserControls.IntegrationAdapter)campo).View = DocsPaWA.UserControls.IntegrationAdapterView.INSERT_MODIFY;
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPaWA.UserControls.IntegrationAdapter)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                    }
                }
            }
        }

        public void impostaDirittiRuoloSelezioneEsclusiva(Object etichetta, Object campo, Object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneFascManager.getDirittiCampoTipologiaFasc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1")
                    {
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Enabled = false;
                        ((System.Web.UI.HtmlControls.HtmlAnchor)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Visible = false;
                        ((System.Web.UI.HtmlControls.HtmlAnchor)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                }
            }
        }

        public void impostaDirittiRuoloContatore(Object etichettaContatore, Object campo, Object checkBox, Object etichettaDDL, Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template)
        {
            //DocsPaWR.AssDocFascRuoli assDocFascRuoli = ProfilazioneFascManager.getDirittiCampoTipologiaFasc(UserManager.getRuolo(this).idGruppo, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString(), this);

            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "1")
                    {
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Enabled = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Enabled = false;

                        //Se il contatore è solo visibile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                    if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatore).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((System.Web.UI.WebControls.TextBox)campo).Visible = false;
                        ((System.Web.UI.WebControls.CheckBox)checkBox).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;

                        //Se il contatore non è nè visibile nè modificabile deve comunque scattare se :
                        //1. Contatore di tipologia senza conta dopo
                        //2. Contatore di AOO senza conta dopo e con una sola scelta
                        //3. Contatore di RF senza conta dopo e con una sola scelta
                        if (
                            (oggettoCustom.TIPO_CONTATORE == "T" && oggettoCustom.CONTA_DOPO == "0")
                            ||
                            (oggettoCustom.TIPO_CONTATORE == "A" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            ||
                           (oggettoCustom.TIPO_CONTATORE == "R" && oggettoCustom.CONTA_DOPO == "0" && ((System.Web.UI.WebControls.DropDownList)ddl).Items.Count == 1)
                            )
                        {
                            oggettoCustom.CONTA_DOPO = "0";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        else
                        {
                            oggettoCustom.CONTA_DOPO = "1";
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                        }
                    }
                }
            }
        }
        #endregion 

        #endregion        
        
        #region Gestione sessione fascicolo in lavorazione

        /// <summary>
        /// Il fascicolo correntemente selezionato viene clonato
        /// per creare una nuova copia in memoria "lavorabile"
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        protected DocsPaWR.Fascicolo CloneFascicolo(DocsPaWR.Fascicolo fascicolo)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(DocsPaWR.Fascicolo));

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                serializer.Serialize(stream, fascicolo);
                stream.Position = 0;
                return (DocsPaWR.Fascicolo)serializer.Deserialize(stream);
            }
        }

        /// <summary>
        /// Chiave di sessione utilizzata per mantenere una copia in memoria
        /// del fascicolo correntemente in modifica
        /// </summary>
        private const string FASCICOLO_IN_LAVORAZIONE_SESSION_KEY = "modificaFascicolo.fascicoloInLavorazione";

        /// <summary>
        /// Impostazione / reperimento del fascicolo in lavorazione per il dettaglio corrente
        /// </summary>
        /// <param name="fascicolo"></param>
        protected DocsPaWR.Fascicolo FascicoloInLavorazione
        {
            get
            {
                if (this.Session[FASCICOLO_IN_LAVORAZIONE_SESSION_KEY] != null)
                    return (DocsPaWR.Fascicolo)this.Session[FASCICOLO_IN_LAVORAZIONE_SESSION_KEY];
                else
                    return null;
            }
            set
            {
                this.Session[FASCICOLO_IN_LAVORAZIONE_SESSION_KEY] = value;
            }
        }

        #endregion

        #region Gestione del fascicolo in modalità readonly

        /// <summary>
        /// Disabilitazione di tutti i controlli grafici
        /// del dettaglio se il fascicolo è in sola lettura
        /// </summary>
        private void DisableControlsIfReadOnlyMode()
        {
            // Verifica se il fascicolo in lavorazione è in modalità readonly
            bool readOnly = (this.FascicoloInLavorazione.accessRights == "45");

            foreach (Control control in this.Form.Controls)
            {
                if (control is WebControl)
                {
                    ((WebControl) control).Enabled = !readOnly;
                }
            }
        }

        /// <summary>
        /// Gestione abilitazione / disabilitazione pulsante salva
        /// </summary>
        private void EnableButtonSalva()
        {   
            // Il pulsante è abilitato solo se il controllo del dettaglio della nota 
            // risulta abilitato
            this.btn_salva.Enabled = this.dettaglioNota.Enabled;
        }


        /// <summary>
        /// Aggiornamento in batch delle sole note
        /// </summary>
        protected virtual void UpdateNote()
        {
            DocsPaWR.Fascicolo fascicolo = this.FascicoloInLavorazione;

            DocsPaWR.AssociazioneNota oggettoAssociato = new DocsPaWR.AssociazioneNota();
            oggettoAssociato.TipoOggetto = DocsPaWR.OggettiAssociazioniNotaEnum.Fascicolo;
            oggettoAssociato.Id = fascicolo.systemID;

            // Inserimento della nota creata
            this.dettaglioNota.Save();

            // Aggiornamento delle note sul backend
            fascicolo.noteFascicolo = Note.NoteManager.Update(oggettoAssociato, fascicolo.noteFascicolo);

            // Disabilitazione del dettaglio nota e del pulsante salva
            this.dettaglioNota.Enabled = false;
            this.btn_salva.Enabled = false;
        }

        #endregion
    }
}
