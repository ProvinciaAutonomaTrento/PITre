namespace Amministrazione.UserControl
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for MenuTendina.
	/// </summary>
	public class MenuTendina : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_gestione_log;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_gestione_log_amm;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_gestione_grafica;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_mezzo_spedizione;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_sblocca_doc;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_tipi_doc;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_tipi_fasc;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_diagrammi_stato;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_liste_distribuzione;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_modelli_trasm;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_formati_documenti;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_gestione_rf;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_gestione_news;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_verifica_doc;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_import_oggettario;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_gestione_asserzioni;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_info_documento;
		
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_liste_distr;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_modelli_trasm;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_diagra_stato;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_prof_dinamic;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_prof_dinamicFasc;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_formati_documenti;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_gestione_rubrica_comune;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_gestione_chiaviConfigurazione;

        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_gestione_deleghe;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_gestione_password;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_gestione_indisponibilita;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_gestione_Docum_Stato_Finale;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_gestione_cache;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_gestione_qualifiche;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_abilita_gestione_importazione;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_import_pregressi;
        // Autenticazione Sistemi Esterni R.1
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_gestione_sistemi_esterni;

        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_sblocca_doc_stato_finale;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_gestione_pregressi;


        protected DocsPAWA.DocsPaWR.InfoUtenteAmministratore _datiAmministratore = null;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{	
				#region gestione delle voci di menù tramite chiave su web.config
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamica"] == "1") 
				{
					this.hd_prof_dinamic.Value = "1";
				}
				else
				{
					this.hd_prof_dinamic.Value = "0";
				}
                if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1" )
                {
                    this.hd_prof_dinamicFasc.Value = "1";
                }
                else
                {
                    this.hd_prof_dinamicFasc.Value = "0";
                }
				if(System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["LISTE_DISTRIBUZIONE"] == "1") 
				{
					this.hd_liste_distr.Value = "1";
				}
				else
				{
					this.hd_liste_distr.Value = "0";
				}
				if(System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] != null && System.Configuration.ConfigurationManager.AppSettings["DiagrammiStato"] == "1") 
				{
					this.hd_diagra_stato.Value = "1";
				}
				else
				{
					this.hd_diagra_stato.Value = "0";
				}
                if (System.Configuration.ConfigurationManager.AppSettings["IMPORT_OGGETTARIO"] != null && System.Configuration.ConfigurationManager.AppSettings["IMPORT_OGGETTARIO"] == "1")
                    this.hd_import_oggettario.Value = "1";
                else
                    this.hd_import_oggettario.Value = "0";
                if (System.Configuration.ConfigurationManager.AppSettings["MEZZO_SPEDIZIONE"] != null && System.Configuration.ConfigurationManager.AppSettings["MEZZO_SPEDIZIONE"] == "1")
                    this.hd_mezzo_spedizione.Value = "1";
                else
                    this.hd_mezzo_spedizione.Value = "0";
                if (System.Configuration.ConfigurationManager.AppSettings["GESTIONE_ASSERZIONI"] != null && System.Configuration.ConfigurationManager.AppSettings["GESTIONE_ASSERZIONI"] == "1")
                    this.hd_gestione_asserzioni.Value = "1";
                else
                    this.hd_gestione_asserzioni.Value = "0";
                if (System.Configuration.ConfigurationManager.AppSettings["GESTIONE_GRAFICA"] != null && System.Configuration.ConfigurationManager.AppSettings["GESTIONE_GRAFICA"] == "1")
                    this.hd_abilita_gestione_grafica.Value = "1";
                else
                    this.hd_abilita_gestione_grafica.Value = "0";

                


                //Nuova gestione chiavi di configurazione

                string valoreChiave;

                // Gestione documenti in stato finale, attivabile solamente per gli utenti amministratori di amministrazioni
                valoreChiave = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_ABILITA_GEST_DOCS_ST_FINALE");
                if (!string.IsNullOrEmpty(valoreChiave) && valoreChiave.Equals("1"))
                {
                    hd_gestione_Docum_Stato_Finale.Value = valoreChiave;
                }

                if (!string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_ENABLE_VIS_INFO_DOC_AMM")) && DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_ENABLE_VIS_INFO_DOC_AMM").Equals("1"))
                    this.hd_info_documento.Value = "1";
                else
                    this.hd_info_documento.Value = "0";

                // Verifica se la gestione dei formati documenti è abilitata o meno
                if (DocsPAWA.FormatiDocumento.Configurations.SupportedFileTypesEnabled)
                    this.hd_formati_documenti.Value = "1";
                else
                    this.hd_formati_documenti.Value = "0";

              DocsPAWA.AdminTool.Manager.AmministrazioneManager am = new DocsPAWA.AdminTool.Manager.AmministrazioneManager();
                if (am.IsEnabledRF(null))
                    this.hd_abilita_gestione_rf.Value = "1";
                else
                    this.hd_abilita_gestione_rf.Value = "0";

                if (this.GestioneRubricaComuneAbilitata())

                    this.hd_gestione_rubrica_comune.Value = "1"; 
                else
                    this.hd_gestione_rubrica_comune.Value = "0";


                //--- case GESTIONE QUALIFICHE
                string chiaveQualifiche = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "GESTIONE_QUALIFICHE");
                this.hd_abilita_gestione_qualifiche.Value = "0";
                if (!string.IsNullOrEmpty(chiaveQualifiche) && chiaveQualifiche.Equals("1"))
                {
                    this.hd_abilita_gestione_qualifiche.Value = chiaveQualifiche;
                }


                this.hd_abilita_gestione_importazione.Value  = "1";

				#endregion

                
                #region gestione delle voci di menù da visualizzare               
                this.impostaLista();
                
                #endregion

                //solo il super amministratore
                //if (this.getTipoAmministratore() == "1")
                Manager.UtentiManager theManager = new Amministrazione.Manager.UtentiManager();
                if (theManager.AmmVerificaGestioneChiavi(this._datiAmministratore.idPeople))
                    this.hd_abilita_gestione_chiaviConfigurazione.Value = "1";
                else
                    this.hd_abilita_gestione_chiaviConfigurazione.Value = "0";

                 if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["USE_CACHE"])
                     && System.Configuration.ConfigurationManager.AppSettings["USE_CACHE"].ToUpper() == "TRUE")
                    this.hd_abilita_gestione_cache.Value = "1";
                else
                    this.hd_abilita_gestione_cache.Value = "0";

                //Disservizio
                string valoreChiave2;
                valoreChiave2 = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_GESTIONE_DISSERVIZIO");
                if (!string.IsNullOrEmpty(valoreChiave2) && valoreChiave2.Equals("1"))
                {
                    if (this.getTipoAmministratore() == "1")
                        this.hd_gestione_indisponibilita.Value = "1";
                    else
                        this.hd_gestione_indisponibilita.Value = "0";
                }
                else
                    this.hd_gestione_indisponibilita.Value = "0";

                //Import pregressi
                string valoreChiave3;
                valoreChiave3 = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_IMPORT_PREGRESSI");
                if (!string.IsNullOrEmpty(valoreChiave3) && valoreChiave3.Equals("1"))
                {
                    this.hd_import_pregressi.Value = "1";
                }
                else
                {
                    this.hd_import_pregressi.Value = "0";
                }

                // Autenticazione Sistemi Esterni R.1 per ora è senza chiave
                //string gestSistemiEsterni;
                //gestSistemiEsterni= DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_GEST_SYS_EXT");
                //if (!string.IsNullOrEmpty(gestSistemiEsterni) && gestSistemiEsterni.Equals("1"))
                //{
                //    this.hd_gestione_sistemi_esterni.Value = "1";
                //}
                //else
                //{
                //    this.hd_gestione_sistemi_esterni.Value = "0";
                //}

                // Autenticazione Sistemi Esterni R.1, chiave di webconfig
                //if (string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["AUTH_SYS_EXT_FE_ADMIN"]) || System.Configuration.ConfigurationManager.AppSettings["AUTH_SYS_EXT_FE_ADMIN"] == "0")
                if(string.IsNullOrEmpty(DocsPAWA.utils.InitConfigurationKeys.GetValue("0","FE_AUTH_SYS_EXT_FE_ADMIN")) || DocsPAWA.utils.InitConfigurationKeys.GetValue("0","FE_AUTH_SYS_EXT_ADMIN") =="0")
                {
                    this.hd_gestione_sistemi_esterni.Value = "0";
                }
            }
		}

        /// <summary>
        /// Indica se la gestione rubrica comune risulta abilitata per l'amministratore corrente
        /// </summary>
        /// <returns></returns>
        protected bool GestioneRubricaComuneAbilitata()
        {
            DocsPAWA.DocsPaWR.ConfigurazioniRubricaComune config = DocsPAWA.RubricaComune.Configurazioni.GetConfigurazioni(this._datiAmministratore);

            bool gestioneAbilitata = config.GestioneAmministrazioneAbilitata;

            //if (gestioneAbilitata)
            //{
            //    // Verifica se l'utente è uno UserAdministrator
            //    if (this.getTipoAmministratore() == "3")
            //        gestioneAbilitata = (this.isAttivo("RUBRICA_COMUNE") == "1");
            //}

            return gestioneAbilitata;
        }

		#region gestione tipo utente Amministratore
		
		private void impostaLista()
		{
			this.setUserSession();

			if ("3".Equals(this.getTipoAmministratore()))
				this.impostaCampiNascosti();
		}
		
		private void setUserSession()
		{
            this._datiAmministratore = new DocsPAWA.DocsPaWR.InfoUtenteAmministratore();

			DocsPAWA.AdminTool.Manager.SessionManager sessionMng = new DocsPAWA.AdminTool.Manager.SessionManager();
			this._datiAmministratore = sessionMng.getUserAmmSession();
		}

		private string getTipoAmministratore()
		{
			string retValue = string.Empty;
			retValue = this._datiAmministratore.tipoAmministratore;
			return retValue;
		}

		private DocsPAWA.DocsPaWR.Menu[] getVociMenuUserAdmin()
		{
			DocsPAWA.DocsPaWR.Menu[] lista = this._datiAmministratore.VociMenu;
			return lista;
		}

        private string isAttivo(string aVoceMenu)
        {
            DocsPAWA.DocsPaWR.Menu[] vociMenu = this.getVociMenuUserAdmin();

            foreach (DocsPAWA.DocsPaWR.Menu item in vociMenu)
            {
                if (item.Codice.ToUpper().Equals(aVoceMenu))
                {
                    if (item.Associato != null && !item.Associato.Equals(""))
                        return "1";
                    else
                        return "0";
                }
            }

            return "0";
        }

        private void impostaCampiNascosti()
        {
            /**** FUNZIONALITA' PARAMETRIZZATE DA WEB.CONFIG ****/

                    //--case "TIPI DOCUMENTO": //---------------------------------------------------------------------------
                        if (this.hd_abilita_tipi_doc.Value.Equals("1"))
                            this.hd_abilita_tipi_doc.Value = isAttivo("TIPI DOCUMENTO");
                    //--case "TIPI FASCICOLI": //---------------------------------------------------------------------------
                        if (this.hd_abilita_tipi_fasc.Value.Equals("1"))
                            this.hd_abilita_tipi_fasc.Value = isAttivo("TIPI FASCICOLI");
                    //--case "DIAGRAMMI STATO": //---------------------------------------------------------------------------
                        if (this.hd_abilita_diagrammi_stato.Value.Equals("1"))
                            this.hd_abilita_diagrammi_stato.Value = isAttivo("DIAGRAMMI STATO");
                    //--case "LISTE DI DISTRIBUZIONE": //---------------------------------------------------------------------------
                        if (this.hd_abilita_liste_distribuzione.Value.Equals("1"))
                            this.hd_abilita_liste_distribuzione.Value = isAttivo("LISTE DI DISTRIBUZIONE");
                        //--case "MEZZO SPEDIZIONE": //---------------------------------------------------------------------------
                        if (this.hd_mezzo_spedizione.Value.Equals("1"))
                            this.hd_mezzo_spedizione.Value = isAttivo("MEZZO SPEDIZIONE");
                        //--case "GESTIONE ASSERZIONI": //---------------------------------------------------------------------------
                        if (this.hd_gestione_asserzioni.Value.Equals("1"))
                            this.hd_gestione_asserzioni.Value = isAttivo("GESTIONE ASSERZIONI");
                        //--case "IMPORT OGGETTARIO": //---------------------------------------------------------------------------
                        if (this.hd_import_oggettario.Value.Equals("1"))
                            this.hd_import_oggettario.Value = isAttivo("IMPORT OGGETTARIO");
                        //--case "GESTIONE GRAFICA": //---------------------------------------------------------------------------
                        if (this.hd_abilita_gestione_grafica.Value.Equals("1"))
                            this.hd_abilita_gestione_grafica.Value = isAttivo("GESTIONE GRAFICA");
                        //--case "RUBRICA COMUNE": //---------------------------------------------------------------------------
                        if (this.hd_gestione_rubrica_comune.Value.Equals("1"))
                            this.hd_gestione_rubrica_comune.Value = isAttivo("GESTIONE RUBRICA COMUNE");

                        //--case "GESTIONE DOCS STATO FINALE"://--------------------------------------------------------------------------------------------
                        //Nuova gestione chiavi di configurazione

                        //string valoreChiave;
                        //valoreChiave = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_ABILITA_GEST_DOCS_ST_FINALE");
                        //hd_gestione_Docum_Stato_Finale.Value = valoreChiave;

            /*   altri    */

                    //--case "GESTIONE LOG": //---------------------------------------------------------------------------
                        this.hd_abilita_gestione_log.Value = this.isAttivo("GESTIONE LOG");

                    //--case "GESTIONE LOG AMM": //---------------------------------------------------------------------------
                     //   this.hd_abilita_gestione_log_amm.Value = this.isAttivo("GESTIONE LOG AMM");

                        //--case "SBLOCCA DOCUMENTI": //---------------------------------------------------------------------------
                        this.hd_abilita_sblocca_doc.Value = this.isAttivo("SBLOCCA DOCUMENTI");

                    //--case "MODELLI TRASMISSIONI": //---------------------------------------------------------------------------
                        this.hd_abilita_modelli_trasm.Value = this.isAttivo("MODELLI TRASMISSIONI");

                    //--case "FORMATO DOCUMENTI": //---------------------------------------------------------------------------            
                        this.hd_abilita_formati_documenti.Value = this.isAttivo("FORMATIDOCUMENTO");

                    //--case "GESTIONE RF": //---------------------------------------------------------------------------
                        this.hd_abilita_gestione_rf.Value = this.isAttivo("GESTIONE RF");

                        //--case "GESTIONE NEWS": //---------------------------------------------------------------------------
                        this.hd_abilita_gestione_news.Value = this.isAttivo("GESTIONE NEWS");
                        //--case "GESTIONE LOG AMM": //---------------------------------------------------------------------------
                        this.hd_abilita_gestione_log_amm.Value = this.isAttivo("GESTIONE LOG AMM");
                        //--case "GESTIONE CHIAVI CONFIG": //---------------------------------------------------------------------------
                        this.hd_abilita_gestione_chiaviConfigurazione.Value = this.isAttivo("GESTIONE CHIAVI CONFIG");
                        //--case "GESTIONE DELEGHE": //---------------------------------------------------------------------------
                        this.hd_abilita_gestione_deleghe.Value = this.isAttivo("GESTIONE DELEGHE");
                        //--case "GESTIONE PASSWORD"
                        this.hd_gestione_password.Value = this.isAttivo("GESTIONE PASSWORD");
                        //--case "GESTIONE INDISPONIBILITA"
                        this.hd_gestione_indisponibilita.Value = this.isAttivo("GESTIONE INDISPONIBILITA'");
                        
                        this.hd_gestione_indisponibilita.Value = this.isAttivo("GESTIONE DISSERVIZI");
                        //--- case CACHE
                        this.hd_abilita_gestione_cache.Value = this.isAttivo("USE_CACHE");

                        // Autenticazione Sistemi Esterni R.1
                        this.hd_gestione_sistemi_esterni.Value = this.isAttivo("GESTIONE SISTEMI ESTERNI");

                        // Verifica Doc
                        this.hd_abilita_verifica_doc.Value = this.isAttivo("VERIFICA DOC");

                        // Sblocca doc stato finale
                        this.hd_sblocca_doc_stato_finale.Value = this.isAttivo("GESTIONE DOC STATO FINALE");

                        // Gestione pregressi
                        this.hd_gestione_pregressi.Value = this.isAttivo("GESTIONE PREGRESSI");

                        this.hd_info_documento.Value = this.isAttivo("VISUALIZZA INFO DOC.");
                        
        }

		private void impostaCampiNascosti_orig()
		{
			DocsPAWA.DocsPaWR.Menu[] vociMenu = this.getVociMenuUserAdmin();

			foreach (DocsPAWA.DocsPaWR.Menu item in vociMenu)
			{
				switch (item.Codice.ToUpper())
				{
					case "GESTIONE LOG": //---------------------------------------------------------------------------
						if(item.Associato.Equals(string.Empty))	
							this.hd_abilita_gestione_log.Value = "0";
						break;

					case "SBLOCCA DOCUMENTI": //---------------------------------------------------------------------------
						if (item.Associato.Equals(string.Empty))
							this.hd_abilita_sblocca_doc.Value = "0";
						break;

					case "TIPI DOCUMENTO": //---------------------------------------------------------------------------
						if (item.Associato.Equals(string.Empty))
							this.hd_abilita_tipi_doc.Value = "0";
						break;

					case "DIAGRAMMI STATO": //---------------------------------------------------------------------------
						if (item.Associato.Equals(string.Empty))
							this.hd_abilita_diagrammi_stato.Value = "0";
						break;

					case "LISTE DI DISTRIBUZIONE": //---------------------------------------------------------------------------
						if (item.Associato.Equals(string.Empty))
							this.hd_abilita_liste_distribuzione.Value = "0";
						break;

					case "MODELLI TRASMISSIONI": //---------------------------------------------------------------------------
						if (item.Associato.Equals(string.Empty))
							this.hd_abilita_modelli_trasm.Value = "0";
						break;

                    case "GESTIONE RF": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                            this.hd_abilita_gestione_rf.Value = "0";
                        break;
                    
                    case "GESTIONE LOG AMM": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                            this.hd_abilita_gestione_log_amm.Value = "0";
                        break;

                  //  case "FE_ABILITA_GEST_DOCS_ST_FINALE"://----------------------------------------------------------------------
                case "GESTIONE CACHE": //---------------------------------------------------------------------------
                        if (item.Associato.Equals(string.Empty))
                            this.hd_abilita_gestione_cache.Value = "0";
                        break;

                // Autenticazione Sistemi Esterni R.1
                case "GESTIONE SISTEMI ESTERNI":
                        if (item.Associato.EndsWith(string.Empty))
                            this.hd_gestione_sistemi_esterni.Value = "0";
                        break;
                
                case "GESTIONE POLICY PARER":
                        //if(item.Associato.Equals(string.Empty))
                        break;

                
                }
			}
		}
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
