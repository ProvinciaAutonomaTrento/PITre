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
using DocsPAWA.LocazioneFisica;
using System.Configuration;
using DocsPAWA.DocsPaWR;
using DocsPaWA.UserControls;
using DocsPaWA;
using System.Collections.Generic;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for fascNewFascicolo.
	/// </summary>
    public class fascNewFascicolo : DocsPAWA.CssPage
    {
        #region Sezione variabili
        protected DocsPaWR.DocsPaWebService wws = new DocsPaWR.DocsPaWebService();
        protected Table table;
        protected DocsPaWR.Templates template;
        protected System.Web.UI.WebControls.Label lbl_note;
		protected System.Web.UI.WebControls.TextBox txt_descFascicolo;
		protected System.Web.UI.WebControls.Button Button1;
		protected System.Web.UI.WebControls.PlaceHolder placeScriptCalls;
        protected System.Web.UI.WebControls.CheckBox chkFascicoloCartaceo;
        protected System.Web.UI.WebControls.CheckBox chkPrivato;
        protected System.Web.UI.WebControls.CheckBox chkControllato;
        protected DocsPAWA.DocsPaWR.Utente utente;
		protected DocsPAWA.DocsPaWR.Ruolo ruolo;
		protected DocsPAWA.DocsPaWR.Trasmissione trasmissione;
        protected System.Web.UI.WebControls.Panel panel_profDinamica;
        protected System.Web.UI.WebControls.DropDownList ddl_tipologiaFasc;
        protected System.Web.UI.WebControls.Panel panel_Contenuto;
        protected string val;
        protected string idTitolario;
        protected System.Web.UI.WebControls.TextBox txt_codice;
        //protected DocsPaWebCtrlLibrary.DateMask txt_LFDTA;
        protected DocsPAWA.UserControls.Calendar txt_LFDTA;
        protected System.Web.UI.WebControls.TextBox txt_LFCod;
        protected System.Web.UI.WebControls.TextBox txt_LFDesc;
        protected System.Web.UI.WebControls.Label lbl_lf;
        protected System.Web.UI.WebControls.Button btn_salva;
        protected System.Web.UI.WebControls.Button btn_annulla;
        protected System.Web.UI.WebControls.TextBox txt_cod_uffRef;
        protected System.Web.UI.WebControls.TextBox txt_desc_UffRef;
        protected System.Web.UI.WebControls.Panel pnl_uff_ref;
        protected System.Web.UI.WebControls.Label lbl_registro;
        protected System.Web.UI.WebControls.DropDownList ddl_registri;
        protected System.Web.UI.WebControls.Panel pnl_registri;
        protected System.Web.UI.WebControls.Image btnRubricaF;
        protected System.Web.UI.WebControls.Image btn_Rubrica_ref;
        protected System.Web.UI.WebControls.Label lbl_codice;
        protected System.Web.UI.WebControls.Label lbl_nota;
        protected System.Web.UI.WebControls.Label lbl_dta_LF;
        protected System.Web.UI.WebControls.Button btn_salva_disabled;
        protected Note.DettaglioNota dettaglioNota;
        protected Dictionary<string, Corrispondente> dic_Corr; // = new Dictionary<string,Corrispondente>();
        
        private int indexScript = 0;
        private bool enableUfficioRef = (ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.ENABLE_UFFICIO_REF).Equals("1"));
        private ArrayList storedScriptCall = new ArrayList();
        private string nameScript = "script";
        private DocsPAWA.DocsPaWR.FascicolazioneClassificazione fascicolazioneSelezionata;
        private string loadCombo = ConfigSettings.getKey("ALLOW_FASC_MULTI_REG");
        protected System.Web.UI.WebControls.Panel Panel_DiagrammiStato;
        protected System.Web.UI.WebControls.Panel Panel_DataScadenza;
        protected System.Web.UI.WebControls.DropDownList ddl_statiSuccessivi;
        protected DocsPaWebCtrlLibrary.DateMask txt_dataScadenza;

        protected System.Web.UI.WebControls.HiddenField estendiVisibilita;
        protected System.Web.UI.WebControls.HiddenField abilitaModaleVis;
        protected System.Web.UI.WebControls.HiddenField isCodFascFree;
        bool codFascFree = (System.Configuration.ConfigurationManager.AppSettings["COD_FASC_FREE"] != null && System.Configuration.ConfigurationManager.AppSettings["COD_FASC_FREE"].Equals("1"));
        protected ArrayList dirittiCampiRuolo;
        protected System.Web.UI.HtmlControls.HtmlInputControl clTesto;
        protected  int caratteriDisponibili = 2000;
        #endregion

		#region Sezione gestione script JavaScript lato client	
		//è necessario che alla fine delal sezione HTML della pagina
		//sia presente il seguente codice:
		//
		//				<script language="javascript">
		//					esecuzioneScriptUtente();
		//				</script>
		//
		//nella page_Load:		
		//				if (!IsPostBack)
		//				{
		//					generaFunctionChiamataScript();
		//				}
		//chiamare addScript() per aggiungere gli script 
		//da eseguire alla fine della routine che ne richiede
		//l'esecuzione. Infine, eseguire generaFunctionChiamataScript()
		
		private void addScript(string scriptBody)
		{
			indexScript++;
			string newScriptName=nameScript+indexScript.ToString();
			creaScript(newScriptName,scriptBody);
		}

		private void creaScript(string nameScript,string scriptBody)
		{
			try
			{
				//crea funxione script
				string script="<script language=\"javascript\">"+
					"function "+nameScript+"(){"+scriptBody+"}</script>";
				Response.Write(script);
				
				//crea chiamata alla funzione
				storedScriptCall.Add(nameScript);			
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private void generaFunctionChiamataScript()
		{
			try
			{
				string script = "<script language=\"javascript\">" +
					"function esecuzioneScriptUtente()" +
					"{";

				for (int i=0;i<storedScriptCall.Count;i++)
					script += ((string) storedScriptCall[i] + "();");

				script += "}</script>";

				//if (!IsClientScriptBlockRegistered ("script_esecuzioneScriptUtente"))
				//RegisterClientScriptBlock ("script_esecuzioneScriptUtente", script);
               
                Response.Write (script);
                
                
			}
			catch(Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}
		#endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            if (!this.DesignMode)
            {
                if (Context != null && Context.Session != null && Session != null)
                {
                    InitializeComponent();
                    base.OnInit(e);
                }
            }
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ddl_registri.SelectedIndexChanged += new System.EventHandler(this.ddl_registri_SelectedIndexChanged);
            this.ddl_tipologiaFasc.SelectedIndexChanged += new EventHandler(this.ddl_tipologiaFasc_SelectedIndexChanged);
            this.txt_LFCod.TextChanged += new System.EventHandler(this.txt_LFCod_TextChancged);
            this.txt_cod_uffRef.TextChanged += new System.EventHandler(this.txt_cod_uffRef_TextChanged);
            this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
            this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.fascNewFascicolo_PreRender);
        }
        #endregion

        private void Page_Load(object sender, System.EventArgs e)
        {

            try
            {
                Page.Response.Expires = -1;

                this.RegisterClientScript("nascondi", "nascondi();");

                if (!IsPostBack)
                {
                    DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
                    info = UserManager.getInfoUtente(this.Page);
                    string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_DESC_FASC");
                    if (!string.IsNullOrEmpty(valoreChiave))
                        caratteriDisponibili = int.Parse(valoreChiave);

                    txt_descFascicolo.MaxLength = caratteriDisponibili;
                    clTesto.Value = caratteriDisponibili.ToString();
                    txt_descFascicolo.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','DESCRIZIONE'," + clTesto.ClientID + ")");
                    txt_descFascicolo.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','DESCRIZIONE'," + clTesto.ClientID + ")");
                    FascicoliManager.setFascicoloSelezionato(new DocsPAWA.DocsPaWR.Fascicolo());

                    generaFunctionChiamataScript();

                    Session.Remove("dictionaryCorrispondente");
                }

                //recupera i parametri dalla session
                getParametri();
                //gestioneParametri();
                //per abilitare la scrittura nel campo codice:
                DocsPaWR.Corrispondente corrRef = new DocsPAWA.DocsPaWR.Corrispondente();
                string use_new_rubrica = DocsPAWA.ConfigSettings.getKey(ConfigSettings.KeysENUM.RUBRICA_V2);

                if (enableUfficioRef)
                {
                    if (use_new_rubrica != "1")
                        this.btn_Rubrica_ref.Attributes.Add("onClick", "ApriRubrica('fascUffRef','U');");
                    else
                        this.btn_Rubrica_ref.Attributes.Add("onClick", "_ApriRubrica('nf_uffref');");

                    if (string.IsNullOrEmpty(abilitaModaleVis.Value))
                    {
                        string idAmm = UserManager.getInfoUtente(this).idAmministrazione;
                        if (wws.ereditaVisibilita(idAmm, "null"))
                        {
                            abilitaModaleVis.Value = "true";
                            //ClientScript.RegisterStartupScript(this.GetType(), "openAvvisoVisibilita", "AvvisoVisibilita();", true);
                        }
                    }
                }

                //Modifica per aggiungere la locazione Fisica
                if (!IsPostBack)
                {
                    //modifica per la nuova fascicolazione
                    settaModalitaFascicolazione();

                    caricaCodiceInizialeFascicolo();

                    DocsPaVO.LocazioneFisica.LocazioneFisica _lf = Do_PopolaLF();
                    FascicoliManager.DO_SetLocazioneFisica(_lf);
                    DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoSelezionato(this);

                    //Modifiche AF
                    HttpContext ctx = HttpContext.Current;
                    var s = ctx.Session["ProtocollazioneIngresso.ProtocolloCorrente"];
                    string page_from = "";
                    if (!string.IsNullOrEmpty(Request.QueryString["from"]))
                        page_from = Request.QueryString["from"].ToString();


                    if (page_from.Equals("protoSempl") && s != null && ((DocsPAWA.DocsPaWR.SchedaDocumento)s).oggetto != null)
                        txt_descFascicolo.Text = ((DocsPAWA.DocsPaWR.SchedaDocumento)s).oggetto.descrizione;
                    else
                        if (schedaDoc != null && schedaDoc.oggetto != null && !string.IsNullOrEmpty(schedaDoc.oggetto.descrizione) && (Request.QueryString["isDoc"].ToString() == "1") && utils.InitConfigurationKeys.GetValue("0", "FE_NUOVO_FASC_DIRECT") != null && utils.InitConfigurationKeys.GetValue("0", "FE_NUOVO_FASC_DIRECT").Equals("1"))
                            txt_descFascicolo.Text = schedaDoc.oggetto.descrizione;
                    //else
                    //    if (s != null && !string.IsNullOrEmpty(Request.QueryString["from"]) && Request.QueryString["from"].ToString().Equals("protoSempl"))
                    //        txt_descFascicolo.Text = ((DocsPAWA.DocsPaWR.SchedaDocumento)s).oggetto.descrizione;

                    txt_LFCod.Text = _lf.CodiceRubrica;
                    txt_LFDesc.Text = _lf.Descrizione;
                    this.GetCalendarControl("txt_LFDTA").txt_Data.Text = _lf.Data;

                    //this.dettaglioNota.AttatchPulsanteConferma(this.btn_salva.ClientID);
                    if (!this.IsPostBack)
                        this.dettaglioNota.Fetch();

                    //gestione per la visualizzazione dell'ufficio referente
                    if (enableUfficioRef)
                    {
                        if (!IsPostBack)
                        {
                            TrasmManager.removeGestioneTrasmissione(this);
                            FascicoliManager.removeFascicoloSelezionato(this);
                            pnl_uff_ref.Visible = true;
                            corrRef = setdefaultUoRef();
                            FascicoliManager.setUoReferenteSelezionato(this.Page, corrRef);
                        }
                    }
                }
                else
                {
                    //locazione fisica
                    if (FascicoliManager.DO_VerifyFlagLF())
                    {
                        //FascicoliManager.DO_RemoveFlagLF();

                        DocsPaVO.LocazioneFisica.LocazioneFisica LF = FascicoliManager.DO_GetLocazioneFisica();

                        if (LF != null)
                        {
                            txt_LFDesc.Text = LF.Descrizione;
                            if (LF.CodiceRubrica != null)
                            {
                                this.txt_LFCod.Text = LF.CodiceRubrica;
                                if (LF.Data != null)
                                {
                                    this.GetCalendarControl("txt_LFDTA").txt_Data.Text = LF.Data;
                                }
                                else
                                {
                                    if (this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString() != "")
                                    {
                                        this.GetCalendarControl("txt_LFDTA").txt_Data.Text = this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString();
                                    }
                                    else
                                    {
                                        this.GetCalendarControl("txt_LFDTA").txt_Data.Text = System.DateTime.Now.ToShortDateString();
                                    }
                                }
                            }
                        }
                    }

                    if (enableUfficioRef)
                    {
                        if (FascicoliManager.DO_VerifyFlagUR())
                        {
                            //FascicoliManager.DO_RemoveFlagUR();
                            DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato(this);
                            if (fasc != null)
                            {
                                if (fasc.ufficioReferente != null)
                                    corrRef = fasc.ufficioReferente;
                            }
                            else
                            {
                                corrRef = FascicoliManager.getUoReferenteSelezionato(this.Page);
                            }
                            txt_cod_uffRef.Text = corrRef.codiceRubrica;
                            txt_desc_UffRef.Text = corrRef.descrizione;
                        }
                    }
                }

                this.lbl_codice.Visible = codFascFree;
                this.txt_codice.Visible = codFascFree;
                this.txt_codice.ReadOnly = !codFascFree;
                if (codFascFree)
                    this.isCodFascFree.Value = "true";
                else
                    this.isCodFascFree.Value = "false";

                if (use_new_rubrica != "1")
                    this.btnRubricaF.Attributes.Add("onClick", "ApriRubrica('fascLF','U');");
                else
                    this.btnRubricaF.Attributes.Add("onClick", "_ApriRubrica('nf_locfisica');");

                if (!IsPostBack)
                {
                    //Profilazione dinamica fascicoli
                    if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                    {
                        Session.Remove("template");
                        CaricaComboTipologiaFasc();
                        panel_profDinamica.Visible = true;
                    }
                    //Fine profilazione dinamica fascicoli
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
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            }
            generaFunctionChiamataScript();
        }

        private void caricaCodiceInizialeFascicolo()
        {
            try
            {
                if (codFascFree) //se il codice fascicolo è editabile
                {

                    if (loadCombo != null && loadCombo.ToString().Equals("1"))
                    {
                        // nuova gestione 
                        this.txt_codice.Text = FascicoliManager.getNumeroFascicolo(this, fascicolazioneSelezionata.systemID, ddl_registri.SelectedValue);
                    }
                    else
                    {
                        // vecchia gestione
                        this.txt_codice.Text = FascicoliManager.getNumeroFascicolo(this, fascicolazioneSelezionata.systemID, fascicolazioneSelezionata.idRegistroNodoTit);

                    }
                }
                else //se il codice fascicolo non è editabile
                {
                    this.txt_codice.Text = "";
                }
            }
            catch (System.Exception es)
            {
                ErrorManager.redirect(this, es);
            } 
		}
        
		private void caricaCodiceInizialeFascicoloaSetteCifre()
		{
			try
			{
				//Popolo il campo della PopUp contenente il cod_ultimo
				this.txt_codice.Text=Utils.formatFascSetteCifre(fascicolazioneSelezionata.codUltimo);
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}
		
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}
        		
		private void getParametri()
		{
			//recupera i parametri dalla session o vengono
			//costruiti appositamente in base ai parametri
			//ricevuti.
			try
			{
				/* Sulla bese dei parametri della query string viene 
				 * viene calcolata la fascicolazione selezionata e settata in sessione */
				getParametriQueryString();
				
				//Recupero dati per la pagina memorizzati in sessione.
				getParametriPaginaInSession();

			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
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

		private void getParametriPaginaInSession()
		{
			try
			{
				/* Recupero fascicolazione selezionata memorizzata nella sessione */
				fascicolazioneSelezionata=FascicoliManager.getClassificazioneSelezionata(this);
				
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private void getParametriQueryString()
		{
			//recupera i parametri dalla query string
			try
			{
				if( Request.QueryString["val"] !=null && 
                    !Request.QueryString["val"].ToString().Equals("") &&
                    Request.QueryString["idTit"] != null &&
                    !Request.QueryString["idTit"].ToString().Equals(""))
				{
					val=Request.QueryString["val"].ToString();
                    idTitolario = Request.QueryString["idTit"].ToString();
					if(val!=null && !val.Equals(""))
					{
						//Modifica per la gestione dei titolario multipli
						//DocsPaWR.FascicolazioneClassificazione[] titolario=FascicoliManager.fascicolazioneGetTitolario(this,val,false);
                        DocsPaWR.FascicolazioneClassificazione[] titolario = FascicoliManager.fascicolazioneGetTitolario2(this, val, false,idTitolario);
						if (titolario == null || titolario.Length<1)
						{
							Response.Write("<script>window.alert('Codice classifica non valido'); window.close();</script>");
						}

						FascicoliManager.setClassificazioneSelezionata(this,titolario[0]);	
					}
					else
					{
						Response.Write("<script>window.alert('selezionare una classifica.')</script>");
					}
					
				}
						
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private void scaricaVariabiliSessione()
		{
			//scarica Variabili Sessione
			try
			{
			
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private void scaricaPagina()
		{
			//esegue la pulizia della session
			//e scarica la pagina
			try
			{
                //scaricaVariabiliSessione();
                //string scriptToExecute="window.opener.location='../ricercaFascicoli/ricFasc.aspx'; window.close();";

                //string scriptToExecute="createFascicolo(); window.close();";
                //addScript(scriptToExecute);

                // Response.Write("<script>window.open('../ricercaFascicoli/tabRisultatiRicFasc.aspx?newFasc=1','iFrame_dx');</script>");
                //Response.Write("<script>window.close();</script>");

                //modifica x risoluzione anomalia codice fascicolo del 06/09/2011 afiordi

                if (Request.QueryString["from"].ToString().Equals("ricercaFascicoli") || Request.QueryString["from"].ToString().Equals("docClassifica"))
                    ClientScript.RegisterStartupScript(this.GetType(), "apri_ric_fasc", "window.open('../ricercaFascicoli/NewTabSearchResult.aspx?newFasc=1','iFrame_dx');", true);
                ClientScript.RegisterStartupScript(this.GetType(), "chiudi_window", "window.close();", true);
            
            }
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}
		
    	private bool verificaCodice(string codice,out string errorMessage)
		{

			string errore="";
			bool retValue;
			//restituisce true se la descrizione che 
			//riceve come parametro è corretta
			if (codice=="")
			{
				errore="Il codice del fascicolo non può essere vuoto!";
				retValue=false;
			}
			else
			{
				if (Utils.isNumeric(codice))
				{
					retValue=true;
				}
				else
				{
					errore="Il codice fascicolo deve essere un numerico!";
					retValue=false;
				}
				//				string pattern="\\D";
				//				string newCod=codice;
				//
				//				Regex regExp=new Regex(pattern);
				//				Match match=regExp.Match(codice);
				//
				//				if (match.Length==0)
				//				{
				//					retValue=true;
				//				}
				//				else
				//				{
				//					errore="Il codice fascicolo deve essere un numerico!";
				//					retValue=false;
				//				}
			}

			errorMessage=errore;
			return retValue;
		}

		private bool verificaDescrizione(string descrizione,out string errorMessage)
		{
			string errore="";
			bool retValue;
			
			//restituisce true se la descrizione che 
			//riceve come parametro è corretta
			if (descrizione.Trim()=="")
			{
				errore="La descrizione del fascicolo è obbligatoria!";
				retValue=false;
			}
			else
			{
				// controllo sulla lunghezza max dell'oggetto (500 car.)
				if (this.txt_descFascicolo.Text.Length > 2000)
				{
					errore = "Consentita lunghezza massima di 2000 caratteri per il campo descrizione!";
					retValue=false;
				}
				else
				{
					retValue=true;
				}
				
				
			}


			errorMessage=errore;
			return retValue;
		}

        /// <summary>
        /// Inizializzazione controllo per la gestione delle note del documento
        /// </summary>
        private void InitializeControlListaNote()
        {

        }

		private bool confermaSelezioneUtente()
		{
			//conferma le selezioni effettuate dall'utente
			DocsPaWR.ResultCreazioneFascicolo resultCreazione;
			try
			{
				bool retValue=false;
				string errore="";
				string bodyScript="";
				
				retValue=verificaDescrizione(this.txt_descFascicolo.Text.ToString(),out errore);
				if (retValue)
				{
                    if (codFascFree)
                    {
                        retValue = verificaCodice(this.txt_codice.Text.ToString(), out errore);
                    }
                    else
                    {
                        // nuova gestione(se il codice non è editabile a mano sarà proposto da DocsaPa)
                        // quindi di default sarà passato vuoto dal frontend e sarà calcolato da backend
                        // leggendo il valore dalla tabella DPA_REG_FASC
                        retValue = true;
                    }
                    if (retValue)
                    {
                        DocsPaWR.Fascicolo newFascicolo = FascicoliManager.getFascicoloSelezionato();

                        //luluciani 30/01/2009
                        if (newFascicolo == null)
                        {
                            newFascicolo = new Fascicolo();

                            FascicoliManager.setFascicoloSelezionato(newFascicolo);
                        }
                        //fine

                        newFascicolo.isFascConsentita = "1";
                        newFascicolo.descrizione = this.txt_descFascicolo.Text.Replace(Environment.NewLine, " ");
                        newFascicolo.codUltimo = this.txt_codice.Text.ToString();
                        newFascicolo.cartaceo = this.chkFascicoloCartaceo.Checked;
                        newFascicolo.controllato = "0";
                        if (this.chkControllato.Visible && !this.chkControllato.Checked)
                            newFascicolo.controllato = "1";

                        if (this.chkPrivato.Checked)
                            newFascicolo.privato = "1";
                        else newFascicolo.privato = "0";
                        //string loadCombo = ConfigSettings.getKey("ALLOW_FASC_MULTI_REG");

                        //inserito campo note per fascicoli procedimentali
                        this.dettaglioNota.Save();

                        newFascicolo.apertura = System.DateTime.Today.ToString("dd/MM/yyyy");
                        newFascicolo.idRegistroNodoTit = FascicoliManager.getClassificazioneSelezionata(this).idRegistroNodoTit;
                        if (loadCombo != null && loadCombo.ToString().Equals("1"))
                        {
                            newFascicolo.idRegistro = ddl_registri.SelectedValue.ToString();
                        }
                        else
                        {
                            newFascicolo.idRegistro = newFascicolo.idRegistroNodoTit;
                        }

                        newFascicolo.ufficioReferente = FascicoliManager.getUoReferenteSelezionato(this);
						DocsPaVO.LocazioneFisica.LocazioneFisica lf = FascicoliManager.DO_GetLocazioneFisica();
						if(lf!=null)
						{
							newFascicolo.idUoLF= lf.UO_ID;
							newFascicolo.descrizioneUOLF =lf.Descrizione;
							newFascicolo.varCodiceRubricaLF = lf.CodiceRubrica;
							
							#region commentato per risoluzione bug 2078
							//OLD commentato per risoluzione bug 2078
//							if(lf.Data!=null)
//							{
//								newFascicolo.dtaLF= lf.Data;
//							}
//							else
//							{
//								if(this.txt_LFDTA.Text!=null && this.txt_LFDTA.Text!="")
//								{
//									newFascicolo.dtaLF=this.txt_LFDTA.Text;
//								}
//								else
//								{
//									newFascicolo.dtaLF=System.DateTime.Now.ToShortDateString();
//								}
//							}
							//fine commentO
							#endregion

							if(this.GetCalendarControl("txt_LFDTA").txt_Data.Text!=null)
							{
								if (this.GetCalendarControl("txt_LFDTA").txt_Data.Text=="")
								{
									lf.Data = null;
								}
								else
								{
									lf.Data = this.GetCalendarControl("txt_LFDTA").txt_Data.Text;		
								}
							}
							else
							{
								lf.Data = null;	
							}
							newFascicolo.dtaLF = lf.Data;
						}

                        //Profilazione Dinamica fascicoli
                        if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                        {
                            if (template != null)
                                newFascicolo.template = template;
                        }
                        //Fine profilazione dinamica fascicoli


                        FascicoliManager.DO_RemoveLocazioneFisica();
                        FascicoliManager.setNewFascicolo(this, newFascicolo);

                        if (val != null && (Request.QueryString["from"] != null && (Request.QueryString["from"].ToString().Equals("ricercaFascicoli") || Request.QueryString["from"].ToString().Equals("docClassifica") || Request.QueryString["from"].ToString().Equals("docProtocollo") || Request.QueryString["from"].ToString().Equals("docProfilo") || Request.QueryString["from"].ToString().Equals("protoSempl"))))
						{
							retValue = false;
							newFascicolo=FascicoliManager.newFascicolo(this,FascicoliManager.getClassificazioneSelezionata(this),newFascicolo,out resultCreazione);					
							if (newFascicolo == null || newFascicolo.systemID == null || newFascicolo.systemID.Equals(""))
							{
//								errore = "Errore nella creazione del fascicolo";
//								bodyScript="alert('"+errore+"');";

								//Response.Write("<SCRIPT>window.close();<SCRIPT>");
//								Response.Write("<SCRIPT>window.close();<SCRIPT>");
								string message = "";
								switch(resultCreazione)
								{
									case DocsPAWA.DocsPaWR.ResultCreazioneFascicolo.GENERIC_ERROR:
									{
										message = "In questo momento non è stato possibile creare il fascicolo: riprovare più tardi.";
									}
										break;
									case DocsPAWA.DocsPaWR.ResultCreazioneFascicolo.FASCICOLO_GIA_PRESENTE:
									{
										message = "Codice fascicolo già presente";
									}
										break;
                                    case DocsPAWA.DocsPaWR.ResultCreazioneFascicolo.FORMATO_FASCICOLATURA_NON_PRESENTE:
                                        {
                                            message = "Formato della Fascicolatura non impostato. Contattare l'Amministratore";
                                        }
                                        break;
                                    default:
                                        message = "Errore nella creazione del fascicolo";
                                        break;

                                }
                                throw new Exception(message);
                                //throw new Exception("In questo momento non è stato possibile creare il fascicolo: riprovare più tardi.");

                            }
                            else
                            {
                                retValue = true;
                                newFascicolo.idRegistroNodoTit = FascicoliManager.getClassificazioneSelezionata(this).idRegistroNodoTit;
                                FascicoliManager.setFascicoloSelezionato(this, newFascicolo);
                                FascicoliManager.setNewFascicolo(this, newFascicolo);
                                //DIAGRAMMI DI STATO
                                //Essendo andata a buon fine la creazione del fascicolo verifico se è necessario salvarne anche lo stato
                                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                                {
                                    if (System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                                    {
                                        //Salvo lo stato del fascicolo
                                        if (Panel_DiagrammiStato.Visible)
                                            DocsPAWA.DiagrammiManager.salvaModificaStatoFasc(newFascicolo.systemID, ddl_statiSuccessivi.SelectedItem.Value, (DocsPAWA.DocsPaWR.DiagrammaStato)Session["DiagrammaSelezionato"], UserManager.getInfoUtente(this).userId, UserManager.getInfoUtente(this), txt_dataScadenza.Text, this);

                                        if (Panel_DataScadenza.Visible && txt_dataScadenza.Text != null && txt_dataScadenza.Text != "" && newFascicolo.template != null && newFascicolo.template.SYSTEM_ID != null)
                                        {
                                            DocsPAWA.DiagrammiManager.salvaDataScadenzaFasc(newFascicolo.systemID, txt_dataScadenza.Text, newFascicolo.template.SYSTEM_ID.ToString(), this);
                                            newFascicolo.dtaScadenza = txt_dataScadenza.Text;
                                            FascicoliManager.setNewFascicolo(this, newFascicolo);
                                        }
                                        Session.Remove("DiagrammaSelezionato");

                                        //Verifico se effettuare una tramsissione automatica assegnata allo stato
                                        if (newFascicolo.template != null && newFascicolo.template.SYSTEM_ID != 0 && Panel_DiagrammiStato.Visible)
                                        {
                                            ArrayList modelli = new ArrayList(DocsPAWA.DiagrammiManager.isStatoTrasmAutoFasc(UserManager.getInfoUtente(this).idAmministrazione, ddl_statiSuccessivi.SelectedItem.Value, newFascicolo.template.SYSTEM_ID.ToString(), this));
                                            for (int i = 0; i < modelli.Count; i++)
                                            {
                                                DocsPaWR.ModelloTrasmissione mod = (DocsPaWR.ModelloTrasmissione)modelli[i];
                                                if (mod.SINGLE == "1")
                                                {
                                                    DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, newFascicolo, this);
                                                }
                                                else
                                                {
                                                    for (int k = 0; k < mod.MITTENTE.Length; k++)
                                                    {
                                                        if (mod.MITTENTE[k].ID_CORR_GLOBALI.ToString() == UserManager.getRuolo(this).systemId)
                                                        {
                                                            DocsPAWA.TrasmManager.effettuaTrasmissioneFascDaModello(mod, ddl_statiSuccessivi.SelectedItem.Value, newFascicolo, this);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                //FINE DIAGRAMMI DI STATO 
							}
						}
						
					}
					else
					{
						bodyScript="alert('"+errore+"');";
					}
				}
				else
				{
					bodyScript="alert('"+errore+"');";
				}

				if (retValue==false)
				{
					addScript(bodyScript);
				}
               
				return retValue;
			}
			catch(System.Exception es) 
			{
				throw new Exception(es.Message);
//				string avvisoModale = "creazione nuovo fascicolo";
//				ErrorManager.redirect(this,es,avvisoModale);
				
			} 
			//return false;
		}

		private void setDescUffRefInNuovaFascicolo(string codiceRubrica) 
		{								
			DocsPaWR.Corrispondente corr = null;
			string msg = "Codice rubrica non valido per l\\'Ufficio referente!";
			if(!codiceRubrica.Equals(""))
				corr = UserManager.getCorrispondenteReferente(this, codiceRubrica,false);
			if (corr != null && (corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa))))
			{
				this.txt_desc_UffRef.Text = UserManager.getDecrizioneCorrispondenteSemplice(corr);
				FascicoliManager.setUoReferenteSelezionato(this.Page,corr);
			} 
			else
			{
				//this.txt_cod_uffRef.Text = "";
				if(!codiceRubrica.Equals(""))
				{
					RegisterStartupScript("alert","<script language='javascript'>alert('" + msg + "');</script>");
					string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_uffRef.ID + "').focus() </SCRIPT>";
					RegisterStartupScript("focus", s);
				}
				this.txt_desc_UffRef.Text = "";
			}

		}

		private void ripristinaValoriIniziali()
		{
			//annulla le selezioni effettuate dall'utente
			try
			{
				this.txt_descFascicolo.Text="";
				//this.ddl_tipoFascicolo.SelectedIndex=-1;
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private void btn_salva_Click(object sender, System.EventArgs e)
		{
			try
			{
				Session["OggettoDellaTrasm"]="FASC";
				//controllo campi
				string errore="";
				if (!this.verificaDescrizione(this.txt_descFascicolo.Text, out errore))
				{
					if (!errore.Equals(""))
						Response.Write("<script>alert("+"'"+errore+ "'" +");</script>");	
					generaFunctionChiamataScript();
					return;
				}
				if(this.GetCalendarControl("txt_LFDTA").txt_Data.Text!="" && !Utils.isDate(this.GetCalendarControl("txt_LFDTA").txt_Data.Text))
				{
					Response.Write("<script>alert('Il formato della data non è valido. \\nIl formato richiesto è gg/mm/aaaa');</script>");
					string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txt_LFDTA").txt_Data.ID + "').focus();</SCRIPT>";
					RegisterStartupScript("focus", s);
					return;
				}
                if(codFascFree)
                {
                    if (!this.verificaCodice(this.txt_codice.Text, out errore))
                    {
                        if (!errore.Equals(""))
                            Response.Write("<script>alert(" + "'" + errore + "'" + ");</script>");
                        generaFunctionChiamataScript();
                        return;
                    }
                }
			
				if(enableUfficioRef)//se è abilitato l'ufficio referente
				{
					if ( (this.txt_cod_uffRef.Text.Equals("") || this.txt_cod_uffRef.Text == null) 
						|| (this.txt_desc_UffRef.Text.Equals("") || this.txt_desc_UffRef.Text== null)) 
					{
					
						errore = "Inserire l'ufficio referente!";
						string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_uffRef.ID + "').focus();</SCRIPT>";
						Response.Write("<script>alert("+"'"+errore+ "'" +");</script>");
						RegisterStartupScript("focus", s);
						return;
							
					}
					if(!CheckUoReferente())
					{
						errore = "La creazione del fascicolo non può essere effettuata.\\nL\\'ufficio referente non possiede ruoli di riferimento.";
						string s = "<SCRIPT language='javascript'>document.getElementById('" + txt_cod_uffRef.ID + "').focus();</SCRIPT>";
						Response.Write("<script>alert("+"'"+errore+ "'" +");</script>");
						RegisterStartupScript("focus", s);
						return;
					}

                    //se è abilitato l'ufficio referente e il fascicolo è privato devo lanciare la modale
                    if (!string.IsNullOrEmpty(estendiVisibilita.Value))
                    {
                        Session.Add("EreditaSI_NO", estendiVisibilita.Value);
                    }

                }

                //Controllo i campi obbligatori della profilazione dinamica
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
                {
                    if (controllaCampiObbligatori())
                    {
                        errore = "La creazione del fascicolo non può essere effettuata.\\nCi sono dei campi obbligatori non valorizzati.";
                        Response.Write("<script>alert(" + "'" + errore + "'" + ");</script>");
                        return;
                    }

                    string messag = ProfilazioneFascManager.verificaOkContatoreFasc((DocsPAWA.DocsPaWR.Templates)Session["template"]);
                    if (messag != string.Empty)
                    {
                        Response.Write("<script>alert('" + messag + "');</script>");
                        return;
                    }

                    //Controllo la validità di una eventuale data scadenza per la tipologia di documento
                    if (Panel_DataScadenza.Visible && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1")
                    {
                        try
                        {
                            DateTime dataInserita = Convert.ToDateTime(txt_dataScadenza.Text);
                            DateTime dataOdierna = System.DateTime.Now;
                            if ((dataInserita < dataOdierna) && txt_dataScadenza.Enabled)
                            {
                                errore = "La data di scadenza deve essere successiva a quella odierna.";
                                Response.Write("<script>alert(" + "'" + errore + "'" + ");</script>");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            errore = "Inserire una data valida";
                            Response.Write("<script>alert(" + "'" + errore + "'" + ");</script>");
                            return;
                        }
                    }

                    if (dic_Corr != null)
                        Session["dictionaryCorrispondente"] = dic_Corr;
                }               
			
				bool retValue=confermaSelezioneUtente();
			
				if (retValue == false)
				{
					generaFunctionChiamataScript();
					return;
				}

                if (!(val != null && !val.Equals("")) || (Request.QueryString["from"] != null && (Request.QueryString["from"].ToString().Equals("ricercaFascicoli") || Request.QueryString["from"].ToString().Equals("docProtocollo") || Request.QueryString["from"].ToString().Equals("docProfilo") || Request.QueryString["from"].ToString().Equals("protoSempl"))))
				{
					if (retValue)
					{
						scaricaPagina();
					}
				}
				else
				{
					//FascicoliManager.removeClassificazioneSelezionata(this);
					//var k=window.open('../documento/docClassifica.aspx','IframeTabs');
					string codClassifica = FascicoliManager.getClassificazioneSelezionata(this).codice;
					FascicoliManager.removeNewFascicolo(this);
					Session.Remove("OggettoDellaTrasm");

                    DocsPaWR.Fascicolo fasc = FascicoliManager.getFascicoloSelezionato(this);
					//Response.Write("<script>var k=window.open('../documento/listaFascicoli.aspx?newFasc=1','iFrame_dx');  window.close();</script>");
                    Response.Write("<script> var k=window.open('../fascicolo/fascDettagliFasc.aspx?newFasc=1','iFrame_dx'); window.returnValue = '" + Server.UrlEncode(fasc.codice) + "';  window.close();</script>");
                }

                //impongo nella maschera di protocollazione/salva doc da cui proviene la chiamata il nuovo fascicolo come fascicolo di fascicolazione rapida
                DocsPaWR.Fascicolo fascSel = FascicoliManager.getFascicoloSelezionato(this);
                FascicoliManager.setFascicoloSelezionatoFascRapida(this, fascSel);

                if (enableUfficioRef)
                {
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
                }

                // MAC 3891
                /* Annulla la precedente classificazione. 
                 * In docProfilo.Page_Load, se esiste una classificazione, viene forzato l'evento txt_CodFascicolo_TextChanged
                 * che va a copiare in sessione la classificazione esistente. Ciò sovrascrive la nuova classificazione.
                 */
                DocumentManager.removeClassificazioneSelezionata(this);

				generaFunctionChiamataScript();

                //if (Session["rubrica.campoCorrispondente"] != null)
                //    Session.Remove("rubrica.campoCorrispondente");
            }
			catch(Exception ex)
			{
				
				string avvisoModale = "creazione nuovo fascicolo";
				ErrorManager.OpenErrorPage(Page,ex,avvisoModale,false);
                
			}
            
		}
	
		private bool CheckUoReferente()
		{
			bool result = true;
			try 
			{
				DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
				DocsPaWR.Corrispondente corrRef = FascicoliManager.getUoReferenteSelezionato(this);		
				if(!docsPaWS.UOHasReferenceRole(corrRef.systemId))
				{
					result = false;
				}
			}
			catch (Exception ex)
			{
				ErrorManager.redirect(this, ex);
			}
			return result;
		}

		private void btn_annulla_Click(object sender, System.EventArgs e)
		{
			//annulla le selezioni effettuate dall'utente
			try
			{
				if(!(val!=null && !val.Equals("")))
				{
					ripristinaValoriIniziali();
					scaricaPagina();
				} 
				else
				{
					FascicoliManager.removeNewFascicolo(this);
                    Response.Write("<script>window.returnValue=false; window.close();</script>");
				}
				generaFunctionChiamataScript();
                Session.Remove("template");               
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}		

		private DocsPaVO.LocazioneFisica.LocazioneFisica Do_PopolaLF()
		{
			DocsPaVO.LocazioneFisica.LocazioneFisica _lf = new DocsPaVO.LocazioneFisica.LocazioneFisica();
			try
			{
				DocsPaWR.UnitaOrganizzativa _uo = ((DocsPAWA.DocsPaWR.Ruolo)UserManager.getRuolo(this)).uo;
				_lf.CodiceRubrica = _uo.codiceRubrica;
				_lf.UO_ID = _uo.systemId;
				_lf.Descrizione = _uo.descrizione;
				_lf.Data = (string)DateTime.Now.ToShortDateString();
			}
			catch(Exception ex)
			{
				throw ex;
			}
			return _lf;
		}

		private DocsPAWA.DocsPaWR.UnitaOrganizzativa setdefaultUoRef()
		{
			DocsPaWR.UnitaOrganizzativa _uo = ((DocsPAWA.DocsPaWR.Ruolo)UserManager.getRuolo(this)).uo;
				
			try
			{
				txt_cod_uffRef.Text = _uo.codiceRubrica;
				txt_desc_UffRef.Text = _uo.descrizione;
			}
			catch(System.Exception es)
			{
				ErrorManager.redirect(this, es);
			}

			return _uo;	
		}

		private void txt_LFCod_TextChancged(object sender, System.EventArgs e)
		{
			try 
			{ 
//				txt_LFDesc.Text = "";
//				if(txt_LFCod.Text != "")
//				{
					setDescCorrispondente(txt_LFCod.Text,true);	
//				}

			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
		}

		private void setDescCorrispondente(string codiceRubrica, bool fineValidita) 
		{						
			string msg = "Codice rubrica non esistente";
			DocsPaVO.LocazioneFisica.LocazioneFisica LF = new DocsPaVO.LocazioneFisica.LocazioneFisica();
			DocsPaWR.Corrispondente corr = null;
			try
			{
				if(!codiceRubrica.Equals(""))
					corr = UserManager.GetCorrispondenteInterno(this, codiceRubrica, fineValidita);
				if((corr != null && corr.descrizione != "") && corr.GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)))
				{
					txt_LFDesc.Text = corr.descrizione;
					//DocsPaVO.LocazioneFisica.LocazioneFisica LF = new DocsPaVO.LocazioneFisica.LocazioneFisica();
					LF.CodiceRubrica = corr.codiceRubrica;
					LF.Descrizione = corr.descrizione;
					LF.UO_ID = corr.systemId;
					if(this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString().Trim()!="")
					{
						LF.Data = this.GetCalendarControl("txt_LFDTA").txt_Data.Text.ToString();
					}
					else
					{
						LF.Data = null;
					}
//					else
//					{
//						LF.Data = System.DateTime.Now.ToShortDateString();
//					}
					
					//metto la LF in session
					
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
					LF.CodiceRubrica = "";
					LF.Descrizione = "";
					LF.UO_ID = "";
					LF.Data = this.GetCalendarControl("txt_LFDTA").txt_Data.Text;

				}
				FascicoliManager.DO_SetLocazioneFisica(LF);
			}
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}		
		}

		private void txt_cod_uffRef_TextChanged(object sender, System.EventArgs e)
		{
			try 
			{
				setDescUffRefInNuovaFascicolo(this.txt_cod_uffRef.Text);	
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}
			
		}

		private void fascNewFascicolo_PreRender(object sender, EventArgs e)
		{

           

			if(enableUfficioRef && IsPostBack && FascicoliManager.DO_VerifyFlagUR())
				FascicoliManager.DO_RemoveFlagUR();

			if(IsPostBack && FascicoliManager.DO_VerifyFlagLF())
				FascicoliManager.DO_RemoveFlagLF();

			btnRubricaF.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + btnRubricaF.ClientID + "');";
			btnRubricaF.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + btnRubricaF.ClientID + "');";

			if (enableUfficioRef) 
			{
				btn_Rubrica_ref.Attributes["onMouseOver"] = "javascript:ImpostaCursore (1,'" + btn_Rubrica_ref.ClientID + "');";
				btn_Rubrica_ref.Attributes["onMouseOut"] = "javascript:ImpostaCursore (0,'" + btn_Rubrica_ref.ClientID + "');";
			}

			if (!this.Page.IsClientScriptBlockRegistered ("imposta_cursore")) 
			{
				this.Page.RegisterClientScriptBlock ("imposta_cursore",
					"<script language=\"javascript\">\n" +
					"function ImpostaCursore (t, ctl)\n{\n" +
					"document.getElementById(ctl).style.cursor = (t == 0) ? 'default' : 'hand';\n" +
					"}\n</script>\n");
			}
             // personalizzazzione label data collocazione fisica da web.config.
            if (Utils.label_data_Loc_fisica.Trim() != "")
                this.lbl_dta_LF.Text = Utils.label_data_Loc_fisica;
            else
                this.lbl_dta_LF.Text = "Data collocazione";

            //Controllo se il ruolo utente è autorizzato a creare documenti privati
            if (!UserManager.ruoloIsAutorized(this, "DO_FASC_PRIVATO"))
            {
                this.chkPrivato.Visible = false;
            }
            else
            {
                this.chkPrivato.Visible = true;
            }

            //Controllo che sia possibile la gestione di fascicoli controllati
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["GEST_FASC_CONTROLLATO"]) && System.Configuration.ConfigurationManager.AppSettings["GEST_FASC_CONTROLLATO"].Equals("1"))
                this.chkControllato.Visible = true;
            else
                this.chkControllato.Visible = false;

            this.chkControllato.Enabled = false;
            if (UserManager.ruoloIsAutorized(this, "FASC_CONTROLLATO"))
            {
                this.chkControllato.Enabled = true;
            }

            //if (template != null && template.ELENCO_OGGETTI != null)
            //{
            //    for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
            //    {
            //        DocsPaWR.OggettoCustom oggettoCustom = (DocsPAWA.DocsPaWR.OggettoCustom)template.ELENCO_OGGETTI[i];
            //        salvaValoreCampo(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString());
            //    }
            //    Session.Add("template", template);
            //}
		}

        private void salvaValoreCampo(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
            //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"

            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto":
                    TextBox textBox = (TextBox)panel_Contenuto.FindControl(idOggetto);
                    oggettoCustom.VALORE_DATABASE = textBox.Text;
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList casellaSelezione = (CheckBoxList)panel_Contenuto.FindControl(idOggetto);
                    //Nessuna selezione
                    if (casellaSelezione.SelectedIndex == -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                    {
                        for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                            oggettoCustom.VALORI_SELEZIONATI[i] = null;
                        return;
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
                    DropDownList dropDwonList = (DropDownList)panel_Contenuto.FindControl(idOggetto);
                    oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)panel_Contenuto.FindControl(idOggetto);
                    if ((oggettoCustom.VALORE_DATABASE == "-1" && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        return;
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
                        oggettoCustom.VALORE_DATABASE = "";
                        return;
                    }
                    //if (data.txt_Data.Text.Equals(""))
                    if (data.Text.Equals(""))
                        oggettoCustom.VALORE_DATABASE = "";
                    else
                        //oggettoCustom.VALORE_DATABASE = data.txt_Data.Text;
                        oggettoCustom.VALORE_DATABASE = data.Text;
                    break;
                case "Corrispondente":
                    UserControls.Corrispondente corr = null;
                    DocsPaWR.Corrispondente corrispondente = null;
                    //Controllo se è una selezione da rubrica 
                    if (Session["rubrica.campoCorrispondente"] != null)
                    {
                        corr = (UserControls.Corrispondente)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                        corrispondente = (DocsPAWA.DocsPaWR.Corrispondente)Session["rubrica.campoCorrispondente"];
                        if (corrispondente != null)
                        {
                            if (Session["rubrica.idCampoCorrispondente"] != null && Session["rubrica.idCampoCorrispondente"].ToString() == corr.ID)
                            {
                                corr.CODICE_TEXT = corrispondente.codiceRubrica;
                                corr.DESCRIZIONE_TEXT = corrispondente.descrizione;
                                oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                                //Session.Remove("rubrica.campoCorrispondente");
                                //Session.Remove("rubrica.idCampoCorrispondente");
                                return;
                            }
                        }
                    }

                    //Controllo se è stato digitato un codice
                    corr = (UserControls.Corrispondente)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);
                    //Correzione fatta quando un corrispondente non è visibile al destinatario di una trasmissione
                    if (corrispondente == null && !string.IsNullOrEmpty(corr.CODICE_TEXT))
                        corrispondente = UserManager.getCorrispondenteBySystemIDDisabled(this, corr.SYSTEM_ID_CORR);
                    //Fine Correzione
                    if (corrispondente != null)
                    {
                        oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                        corr.CODICE_TEXT = corrispondente.codiceRubrica;
                        corr.DESCRIZIONE_TEXT = corrispondente.descrizione;
                        return;
                    }

                    //Eventualmente resetto i valori
                    if (corr.CODICE_TEXT == "")
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        corr.CODICE_TEXT = "";
                        corr.DESCRIZIONE_TEXT = "";
                        return;
                    }
                    break;
                case "Contatore":
                case "ContatoreSottocontatore":
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
                        }
                        else
                        {
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                        if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE) && oggettoCustom.FORMATO_CONTATORE.LastIndexOf("COD_UO") != -1)
                        {
                            if (UserManager.getRuolo(this) != null && UserManager.getRuolo(this).uo != null)
                                oggettoCustom.CODICE_DB = UserManager.getRuolo(this).uo.codice;
                        }
                    }
                    break;
                case "OggettoEsterno":
                    IntegrationAdapter intAd = (IntegrationAdapter)panel_Contenuto.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    IntegrationAdapterValue value = intAd.Value;
                    if (value != null)
                    {
                        oggettoCustom.VALORE_DATABASE = value.Descrizione;
                        oggettoCustom.CODICE_DB = value.Codice;
                        oggettoCustom.MANUAL_INSERT = value.ManualInsert;
                    }
                    else
                    {
                        oggettoCustom.VALORE_DATABASE = "";
                        oggettoCustom.CODICE_DB = "";
                        oggettoCustom.MANUAL_INSERT = false;
                    }
                    break;
            }
            //MODIFICA GIORDANO 27/03/2012
            //Modifica relativa alla rubrica ajax, rimuovendo ora la 
            //session CountCorr la pagina non sa più quanti controlli
            //custom "Corrispondenti" ci sono, quindi non scrive i relativi JS.
            //Session.Remove("CountCorr");
            //FINE MODIFICA
            Session.Remove("whichCorr");
        }

        private void CaricaComboRegistri(DropDownList ddl, DocsPaWR.Registro[] userRegistri)
        {
            if (userRegistri != null)
            {
                for (int i = 0; i < userRegistri.Length; i++)
                {
                    ddl.Items.Add(userRegistri[i].codRegistro);
                    ddl.Items[i].Value = userRegistri[i].systemId;
                }
            }
        }

        private void ddl_registri_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            caricaCodiceInizialeFascicolo();
        }

        /// <summary>
        /// Questo metodo caricherà la combo dei registri in maniere diversa, a seconda del valore specificato nella chiave
        /// ALLOW_FASC_MULTI_REG contenuta nel web.config della WA.
        /// Se il valore della chiave è :
        /// - 0: allora la fascicolazione dovrà funzionare come prima. Ovvero se si crea un fascicolo
        /// su un nodo con id_registro NULL allora il fascicolo sarà sempre creato su null, quindi la combo
        /// dei registri sarà invisibile;
        /// - 1:  nuovo caso, la combo dei registri sarà popolata con tutti i registri su cui il ruolo corrente ha visibilità, ma
        /// di default viene presentato il registro preferito per il ruolo dell’utente.
        /// </summary>
        private void settaModalitaFascicolazione()
        {
            int allowFascMultiReg = 0; //default ALLOW_FASC_MULTI_REG = 0 (la fascicolazione funziona come prima)
            // string loadCombo = ConfigSettings.getKey("ALLOW_FASC_MULTI_REG");
            this.pnl_registri.Visible = false;
            DocsPaWR.FascicolazioneClassificazione fascClass = FascicoliManager.getClassificazioneSelezionata(this);
            if (loadCombo != null)
            {
                allowFascMultiReg = Convert.ToInt32(loadCombo.ToString());

                switch (allowFascMultiReg)
                {
                    case 0:
                        //rendo il pannello invisibile
                        this.pnl_registri.Visible = false;

                        break;
                    case 1:
                        //prendo il registro selezionato
                        DocsPaWR.Registro reg = UserManager.getRegistroSelezionato(this);
                        DocsPaWR.Registro[] listaReg = new DocsPaWR.Registro[1];

                        listaReg[0] = reg;


                        this.pnl_registri.Visible = true;

                        CaricaComboRegistri(this.ddl_registri, listaReg);
                        this.ddl_registri.Enabled = false;

                        ////se vengo da docClassifica e il nodo ha registro NULL
                        //if (fascClass != null && (fascClass.idRegistroNodoTit == null || fascClass.idRegistroNodoTit == String.Empty))
                        //{
                        //    if ((Request.QueryString["from"] != null
                        //        && Request.QueryString["from"].ToString().Equals("docClassifica")))
                        //    {
                        //        DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoSelezionato(this);
                        //        if (schedaDoc != null && schedaDoc.systemId != null)
                        //        {
                        //            //se il doc è PROTOCOLLATO, la combo dei registri deve riportare
                        //            //solamente il registro del protocollo e il registro NULL	
                        //            if (schedaDoc.tipoProto != null &&
                        //                (schedaDoc.tipoProto.Equals("A")
                        //                || schedaDoc.tipoProto.Equals("P") || schedaDoc.tipoProto.Equals("I")))
                        //            {
                        //                DocsPaWR.Registro[] protoReg = new DocsPaWR.Registro[1];
                        //                protoReg[0] = schedaDoc.registro;
                        //                listaReg = protoReg;
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        // vengo da ricerca fascicoli
                        //    }

                        //    CaricaComboRegistri(this.ddl_registri, reg);
                        //}
                        //else
                        //{
                        //    //se il nodo è associato ad un determinato registro disabilito la combo
                        //    //autoselezionata con il registro del nodo
                        //    ListItem item3 = new ListItem(fascClass.codiceRegistroNodoTit, fascClass.idRegistroNodoTit);
                        //    this.ddl_registri.Items.Add(item3);
                        //    this.ddl_registri.Enabled = false;
                        //}
                        this.ddl_registri.SelectedIndex = 0;
                        break;
                }

            }

        }

        #region Profilazione Dinamica

        private void CaricaComboTipologiaFasc()
        {
            ArrayList listaTipiFasc = new ArrayList(ProfilazioneFascManager.getTipoFascFromRuolo(UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "2", this));

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

            DocsPaWR.FascicolazioneClassificazione classificazione = FascicoliManager.getClassificazioneSelezionata(this);
            if (classificazione != null && classificazione.idTipoFascicolo != null && classificazione.idTipoFascicolo != "")
            {
                ddl_tipologiaFasc.SelectedValue = classificazione.idTipoFascicolo;
                string idTemplate = ddl_tipologiaFasc.SelectedValue;
                Session.Remove("template");
                template = ProfilazioneFascManager.getTemplateFascById(idTemplate, this);
                Session.Add("template", template);
                //panel_Contenuto.Controls.Clear();
            }
            //Blocco eventualmente la tipologia di fascicolo
            if (classificazione != null && classificazione.bloccaTipoFascicolo != null && classificazione.bloccaTipoFascicolo.Equals("SI"))
                ddl_tipologiaFasc.Enabled = false;
            else
                ddl_tipologiaFasc.Enabled = true;
        }

        private void ddl_tipologiaFasc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string idTemplate = ddl_tipologiaFasc.SelectedValue;
            Panel_DiagrammiStato.Visible = false;
            Panel_DataScadenza.Visible = false;

            if (idTemplate != "")
            {
                Session.Remove("template");
                template = ProfilazioneFascManager.getTemplateFascById(idTemplate, this);
                Session.Add("template", template);
                //panel_Contenuto.Controls.Clear();
                if (template != null && template.PRIVATO != null && template.PRIVATO == "1")
                {
                    chkPrivato.Checked = true;
                }
                else
                    if (chkPrivato.Enabled == false)
                    {
                        chkPrivato.Checked = false;
                    }
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
                        DocsPaWR.DiagrammaStato dg = DocsPAWA.DiagrammiManager.getDgByIdTipoFasc(ddl_tipologiaFasc.SelectedValue, (UserManager.getInfoUtente(this)).idAmministrazione, this);
                        //Session.Add("DiagrammaSelezionato", dg);

                        //Popolo la comboBox degli stati
                        if (dg != null)
                        {
                            popolaComboBoxStatiSuccessivi(null, dg);
                            Panel_DiagrammiStato.Visible = true;
                            Session.Add("DiagrammaSelezionato", dg);
                        }

                        //Imposto la data di scadenza
                        if (Session["template"] != null)
                        {
                            DocsPaWR.Templates template = (DocsPaWR.Templates)Session["template"];
                            if (template.SCADENZA != null && template.SCADENZA != "" && template.SCADENZA != "0")
                            {
                                try
                                {
                                    DateTime dataOdierna = System.DateTime.Now;
                                    int scadenza = Convert.ToInt32(template.SCADENZA);
                                    DateTime dataCalcolata = dataOdierna.AddDays(scadenza);
                                    txt_dataScadenza.Text = Utils.formatDataDocsPa(dataCalcolata);
                                    if (FascicoliManager.getFascicoloSelezionato(this) != null)
                                        FascicoliManager.getFascicoloSelezionato(this).dtaScadenza = Utils.formatDataDocsPa(dataCalcolata);
                                    Panel_DataScadenza.Visible = true;
                                }
                                catch (Exception ex) { }
                            }
                            else
                            {
                                txt_dataScadenza.Text = "";
                                Panel_DataScadenza.Visible = false;
                                if (FascicoliManager.getFascicoloSelezionato(this) != null)
                                    FascicoliManager.getFascicoloSelezionato(this).dtaScadenza = "";
                            }
                        }
                    }
                }
            }
            //FINE DIAGRAMMI DI STATO 
        }

        private void inserisciComponenti(string readOnly, DocsPaWR.Templates template)
        {
            panel_Contenuto.Controls.Clear();
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
                        inserisciContatore(oggettoCustom, readOnly);
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
        public void inserisciContatore(DocsPAWA.DocsPaWR.OggettoCustom oggettoCustom, string readOnly)
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

                        if (readOnly == "SI")
                            ddl.Enabled = false;
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

                        if (readOnly == "SI")
                            ddlAoo.Enabled = false;
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

                        if (readOnly == "SI")
                            ddl.Enabled = false;
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

                        if (readOnly == "SI")
                            ddlRf.Enabled = false;
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
                    if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
                    {
                        int fine = oggettoCustom.DATA_INSERIMENTO.LastIndexOf(".");
                        contatore.Text = contatore.Text.Replace("gg/mm/aaaa hh:mm", oggettoCustom.DATA_INSERIMENTO.Substring(0, fine));
                        contatore.Text = contatore.Text.Replace("gg/mm/aaaa", oggettoCustom.DATA_INSERIMENTO.Substring(0, 10));
                    }

                    if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && oggettoCustom.ID_AOO_RF != "0")
                    {
                        DocsPAWA.DocsPaWR.Registro reg = UserManager.getRegistroBySistemId(this, oggettoCustom.ID_AOO_RF);
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

            contatore.Width = Unit.Percentage(100);
            contatore.ReadOnly = true;
            contatore.CssClass = "testo_segnatura";
            contatore.Style.Add("TEXT-ALIGN", "left");
            cell_2.Controls.Add(contatore);
            row.Cells.Add(cell_2);

            //Inserisco il cb per il conta dopo
            if (oggettoCustom.CONTA_DOPO == "1")
            {
                cbContaDopo.ID = oggettoCustom.SYSTEM_ID.ToString() + "_contaDopo";
                cbContaDopo.Checked = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                cbContaDopo.ToolTip = "Attiva / Disattiva incremento del contatore al salvataggio dei dati.";

                if (readOnly == "SI")
                    cbContaDopo.Enabled = false;

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
            data.paginaChiamante = "NuovoFascicolo";
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
            etichetta.CssClass = "titolo_scheda";

            DocsPAWA.UserControls.Corrispondente corrispondente = (DocsPAWA.UserControls.Corrispondente)this.LoadControl("../UserControls/Corrispondente.ascx");
            corrispondente.CSS_CODICE = "testo_grigio";
            corrispondente.CSS_DESCRIZIONE = "testo_grigio";
            corrispondente.DESCRIZIONE_READ_ONLY = false;
            corrispondente.TIPO_CORRISPONDENTE = oggettoCustom.TIPO_RICERCA_CORR;
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();
            corrispondente.RICERCA_AJAX = false;
            
            if(Session["dictionaryCorrispondente"] != null)
                dic_Corr = (Dictionary<string, Corrispondente>) Session["dictionaryCorrispondente"];

            if (dic_Corr != null && dic_Corr.ContainsKey(corrispondente.ID) && dic_Corr[corrispondente.ID] != null)
            {
                corrispondente.SYSTEM_ID_CORR = dic_Corr[corrispondente.ID].systemId;
                corrispondente.CODICE_TEXT = dic_Corr[corrispondente.ID].codiceRubrica;
                corrispondente.DESCRIZIONE_TEXT = dic_Corr[corrispondente.ID].descrizione;
                oggettoCustom.VALORE_DATABASE = dic_Corr[corrispondente.ID].systemId;
            }
            else
            {
                //Da amministrazione è stato impostato un ruolo di default per questo campo.
                if (oggettoCustom.ID_RUOLO_DEFAULT != null && oggettoCustom.ID_RUOLO_DEFAULT != "" && oggettoCustom.ID_RUOLO_DEFAULT != "0")
                {
                    DocsPaWR.Ruolo ruolo = (DocsPaWR.Ruolo)UserManager.getRuoloById(oggettoCustom.ID_RUOLO_DEFAULT, this);
                    if (ruolo != null)
                    {
                        corrispondente.SYSTEM_ID_CORR = ruolo.systemId;
                        corrispondente.CODICE_TEXT = ruolo.codiceRubrica;
                        corrispondente.DESCRIZIONE_TEXT = ruolo.descrizione;
                    }
                    oggettoCustom.ID_RUOLO_DEFAULT = "0";
                }

                //if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                //{
                //    //oggettoCustom.VALORE_DATABASE = "";
                //    DocsPAWA.DocsPaWR.Corrispondente corr_1 = (DocsPAWA.DocsPaWR.Corrispondente)UserManager.getCorrispondenteBySystemIDDisabled(this, oggettoCustom.VALORE_DATABASE);
                //    if (corr_1 != null)
                //    {
                //        corrispondente.SYSTEM_ID_CORR = corr_1.systemId;
                //        corrispondente.CODICE_TEXT = corr_1.codiceRubrica.ToString();
                //        corrispondente.DESCRIZIONE_TEXT = corr_1.descrizione.ToString();
                //        oggettoCustom.VALORE_DATABASE = corr_1.systemId;
                //        if (dic_Corr == null)
                //            dic_Corr = new Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>();
                //        dic_Corr[corrispondente.ID] = corr_1;
                //        Session["dictionaryCorrispondente"] = dic_Corr;
                //    }
                //}
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
                            if (dic_Corr == null)
                                dic_Corr = new Dictionary<string, DocsPAWA.DocsPaWR.Corrispondente>();
                            dic_Corr[corrispondente.ID] = corr_3;
                            oggettoCustom.VALORE_DATABASE = corr_3.systemId;
                            Session.Remove("rubrica.campoCorrispondente");
                            Session.Remove("rubrica.idCampoCorrispondente");
                            Session["noRicercaCodice"] = true;
                            Session["noRicercaDesc"] = true;
                            Session["dictionaryCorrispondente"] = dic_Corr;
                        }
                    }
                }

                //E' stato selezionato un corrispondente dalla popup dei corrispondenti multipli.
                //if (Session["CorrSelezionatoDaMulti"] != null)
                //{
                //    DocsPAWA.DocsPaWR.Corrispondente corr_4 = (DocsPAWA.DocsPaWR.Corrispondente)Session["CorrSelezionatoDaMulti"];
                //    int idCorrMulti = 0;
                //    if (Session["idCorrMulti"] != null)
                //        idCorrMulti = (int)Session["idCorrMulti"];

                //    if (corr_4 != null && idCorrMulti.ToString().Equals(corrispondente.ID))
                //    {
                //        corrispondente.CODICE_TEXT = corr_4.codiceRubrica;
                //        corrispondente.DESCRIZIONE_TEXT = corr_4.descrizione;
                //        corrispondente.SYSTEM_ID_CORR = corr_4.systemId;
                //        if (dic_Corr == null)
                //            dic_Corr = new Dictionary<string, Corrispondente>();
                //        dic_Corr[corrispondente.ID] = corr_4;
                //        oggettoCustom.VALORE_DATABASE = corr_4.systemId;
                //        Session.Remove("CorrSelezionatoDaMulti");
                //        Session.Remove("noDoppiaRicerca");
                //        Session["dictionaryCorrispondente"] = dic_Corr;
                //        Session.Remove("idCorrMulti");
                //    }
                //}
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
            link.HideLink = true;
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, link, oggettoCustom, template);
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
                                //SetFocus(data.txt_Data);
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
                            //DocsPaWR.Corrispondente corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);
                            DocsPaWR.Corrispondente corrispondente = new Corrispondente();

                            if (Session["dictionaryCorrispondente"] != null)
                            {
                                dic_Corr = (Dictionary<string, DocsPaWR.Corrispondente>)Session["dictionaryCorrispondente"];
                                if (dic_Corr != null && dic_Corr.ContainsKey(corr.ID) && dic_Corr[corr.ID] != null)
                                    corrispondente = dic_Corr[corr.ID];
                            }
                            else
                            {
                                //if (Session["CorrSelezionatoDaMulti"] != null)
                                //{
                                //    DocsPaWR.Corrispondente corr1 = (DocsPaWR.Corrispondente)Session["CorrSelezionatoDaMulti"];
                                //    oggettoCustom.VALORE_DATABASE = corr1.systemId;
                                //}
                                //else
                                //{
                                    if (Session["rubrica.campoCorrispondente"] != null)
                                    {
                                        DocsPAWA.DocsPaWR.Corrispondente corr_3 = (DocsPAWA.DocsPaWR.Corrispondente)Session["rubrica.campoCorrispondente"];
                                        oggettoCustom.VALORE_DATABASE = corr_3.systemId;
                                    }
                                    else
                                    {
                                        //Correzione fatta quando un corrispondente non è visibile al destinatario di una trasmissione
                                        //if (corrispondente == null && !string.IsNullOrEmpty(corr.CODICE_TEXT))
                                            if (!string.IsNullOrEmpty(corr.SYSTEM_ID_CORR))
                                                corrispondente = UserManager.getCorrispondenteBySystemIDDisabled(this, corr.SYSTEM_ID_CORR);
                                            else
                                                corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);
                                        //Fine Correzione

                                        if ((corr.CODICE_TEXT == "" &&
                                                corr.DESCRIZIONE_TEXT == "" &&
                                                oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                                                ||
                                                (corrispondente == null &&
                                                oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
                                            )
                                        {
                                            return true;
                                        }
                                    }
                                //}
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
                                if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_CONTATORE) && oggettoCustom.FORMATO_CONTATORE.LastIndexOf("COD_UO") != -1)
                                {
                                    if (UserManager.getRuolo(this) != null && UserManager.getRuolo(this).uo != null)
                                        oggettoCustom.CODICE_DB = UserManager.getRuolo(this).uo.codice;
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
            }
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
                                ((DocsPAWA.UserControls.Calendar)campo).txt_Data.ReadOnly = true;
                                ((DocsPAWA.UserControls.Calendar)campo).btn_Cal.Enabled = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            if (assDocFascRuoli != null && assDocFascRuoli.INS_MOD_OGG_CUSTOM == "0" && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((DocsPAWA.UserControls.Calendar)campo).txt_Data.Visible = false;
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
                trasmissione.infoFascicolo = FascicoliManager.getInfoFascicoloDaFascicolo(FascicoliManager.getFascicoloSelezionato(this), this);
                TrasmManager.setGestioneTrasmissione(this, trasmissione);
            }

            DocsPaWR.RagioneTrasmissione ragTrasm = null;

            ragTrasm = FascicoliManager.TrasmettiFascicoloToUoReferente(ruolo, out verificaRagioni);

            if (ragTrasm == null && !verificaRagioni)
            {
                retValue = false;
            }
            else
            {
                TrasmManager.setRagioneSel(this, ragTrasm);
            }
            return retValue;
        }

        private string setCorrispondentiTrasmissione()
        {
            string esito = "";
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
                    DocsPaWR.Ruolo[] listaRuoli = UserManager.getRuoliRiferimentoAutorizzati(this, qca, (DocsPAWA.DocsPaWR.UnitaOrganizzativa)corrRef);
                    if (listaRuoli != null && listaRuoli.Length > 0)
                    {
                        for (int index = 0; index < listaRuoli.Length; index++)
                            trasmissione = addTrasmissioneSingola(trasmissione, (DocsPAWA.DocsPaWR.Ruolo)listaRuoli[index]);
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
            catch (Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
            return esito;
        }

        public DocsPAWA.DocsPaWR.Trasmissione addTrasmissioneSingola(DocsPAWA.DocsPaWR.Trasmissione trasmissione, DocsPAWA.DocsPaWR.Corrispondente corr)
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
            trasmissioneSingola.ragione = TrasmManager.getRagioneSel(this);

            // Aggiungo la lista di trasmissioniUtente
            if (corr.GetType() == typeof(DocsPAWA.DocsPaWR.Ruolo))
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.RUOLO;
                DocsPaWR.Corrispondente[] listaUtenti = queryUtenti(corr.codiceRubrica);
                if (listaUtenti == null || listaUtenti.Length == 0)
                    return trasmissione;
                //ciclo per utenti se dest è gruppo o ruolo
                for (int i = 0; i < listaUtenti.Length; i++)
                {
                    DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)listaUtenti[i];
                    if (TrasmManager.getRagioneSel(this).descrizione.Equals("RISPOSTA"))
                        trasmissioneUtente.idTrasmRispSing = trasmissioneSingola.systemId;
                    trasmissioneSingola.trasmissioneUtente = TrasmManager.addTrasmissioneUtente(trasmissioneSingola.trasmissioneUtente, trasmissioneUtente);
                }
            }
            else
            {
                trasmissioneSingola.tipoDest = DocsPAWA.DocsPaWR.TrasmissioneTipoDestinatario.UTENTE;
                DocsPaWR.TrasmissioneUtente trasmissioneUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPAWA.DocsPaWR.Utente)corr;
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

            qco.idAmministrazione = UserManager.getInfoUtente(this).idAmministrazione;// ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];

            //corrispondenti interni
            qco.tipoUtente = DocsPaWR.AddressbookTipoUtente.INTERNO;

            DocsPaWR.Corrispondente[] l_corrispondenti = UserManager.getListaCorrispondenti(this.Page, qco);

            return pf_getCorrispondentiFiltrati(l_corrispondenti);

        }

        private DocsPAWA.DocsPaWR.Corrispondente[] pf_getCorrispondentiFiltrati(DocsPAWA.DocsPaWR.Corrispondente[] corrispondenti)
        {
            string l_oldSystemId = "";
            System.Object[] l_objects = new System.Object[0];
            System.Object[] l_objects_ruoli = new System.Object[0];
            DocsPaWR.Ruolo[] lruolo = new DocsPAWA.DocsPaWR.Ruolo[0];
            int i = 0;
            foreach (DocsPAWA.DocsPaWR.Corrispondente t_corrispondente in corrispondenti)
            {
                string t_systemId = t_corrispondente.systemId;
                if (t_systemId != l_oldSystemId)
                {
                    l_objects = Utils.addToArray(l_objects, t_corrispondente);
                    l_oldSystemId = t_systemId;
                    i = i + 1;
                    continue;
                }
                else
                {
                    /* il corrispondente non viene aggiunto, in quanto sarebbe un duplicato 
                     * ma viene aggiunto solamente il ruolo */

                    if (t_corrispondente.GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
                    {
                        if ((l_objects[i - 1]).GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
                        {
                            l_objects_ruoli = ((Utils.addToArray(((DocsPAWA.DocsPaWR.Utente)(l_objects[i - 1])).ruoli, ((DocsPAWA.DocsPaWR.Utente)t_corrispondente).ruoli[0])));
                            DocsPaWR.Ruolo[] l_ruolo = new DocsPAWA.DocsPaWR.Ruolo[l_objects_ruoli.Length];
                            ((DocsPAWA.DocsPaWR.Utente)(l_objects[i - 1])).ruoli = l_ruolo;
                            l_objects_ruoli.CopyTo(((DocsPAWA.DocsPaWR.Utente)(l_objects[i - 1])).ruoli, 0);
                        }

                    }
                }

            }

            DocsPaWR.Corrispondente[] l_corrSearch = new DocsPAWA.DocsPaWR.Corrispondente[l_objects.Length];
            l_objects.CopyTo(l_corrSearch, 0);

            return l_corrSearch;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qco"></param>
        /// <returns>DocsPAWA.DocsPaWR.AddressbookQueryCorrispondenteAutorizzato</returns>
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

    }
}
