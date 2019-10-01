namespace ProtocollazioneIngresso
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DocsPAWA;
	using ProtocollazioneIngresso.Log;

	/// <summary>
	///		Summary description for StampaEtichetta.
	/// </summary>
	public class StampaEtichetta : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_fascicolo;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_num_proto;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_anno_proto;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_data_proto;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_classifica;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_dispositivo;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_amministrazioneEtichetta;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_descrizioneAmministrazione;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_UrlIniFileDispositivo;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_tipoProtocollazione;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_coduo_proto;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_tipo_proto;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_codreg_proto;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_descreg_proto;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_num_doc;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_signature;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_numeroAllegati;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_dataCreazione;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_codiceUoCreatore;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_modello_dispositivo;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_num_stampe;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_num_stampe_effettuate;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_ora_creazione;
		private Login.LoginMng _loginMng=null;
		private DocsPAWA.DocsPaWR.InfoDocumento _infoDocumento=null;
		protected static DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();


		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion


		/// <summary>
		/// Stampa dell'etichetta
		/// </summary>
		/// <param name="schedaDocumento"></param>
		/// <param name="showPrintMessage">se true, viene chiesta la conferma se stampare o meno l'etichetta</param>
		public void Stampa(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento, bool showPrintMessage, string txt_num_stampe)
		{
			this.CreateInfoDocumento(schedaDocumento);

			this.FillCampiEtichetta(schedaDocumento,txt_num_stampe);

			//docsPaWS.scriviLog(schedaDocumento,this._loginMng.GetInfoUtente(),this._loginMng.GetRuolo());
		
			try
			{
				// Scrittura log
				string segnatura=string.Empty;
				if (schedaDocumento.protocollo!=null)
					segnatura=schedaDocumento.protocollo.segnatura;
                //ProtocollazioneIngressoLog.WriteLogEntry(
                //    string.Format("StampaEtichetta (Segnatura: {0}",segnatura));
			}
			catch 
			{

			}
            DocsPAWA.DocsPaWR.SchedaDocumento sc = new DocsPAWA.DocsPaWR.SchedaDocumento();
            DocumentManager.setDocumentoInLavorazione(schedaDocumento);

			this.RegisterClientScript("PrintSignature","PrintSignature(" + showPrintMessage.ToString().ToLower() + ");");
		}

		/// <summary>
		/// Caricamento dei dati utilizzabili per la stampa dell'etichetta
		/// in un'insieme di campi testo nascosti
		/// </summary>
		/// <param name="schedaDocumento"></param>
		private void FillCampiEtichetta(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento, string txt_num_stampe)
		{
            string abilita_multi_stampa_etichetta = DocsPAWA.utils.InitConfigurationKeys.GetValue("0", "FE_MULTI_STAMPA_ETICHETTA");
			#region parametro Dispositivo Di Stampa
			
            //if(ConfigSettings.getKey(ConfigSettings.KeysENUM.DISPOSITIVO_STAMPA)!=null)
            //{
            //    this.hd_dispositivo.Value=ConfigSettings.getKey(ConfigSettings.KeysENUM.DISPOSITIVO_STAMPA);
            //}
            //else
            //{
            //    this.hd_dispositivo.Value="Penna";
            //}

            //this.hd_modello_dispositivo.Value = ConfigSettings.getKey(ConfigSettings.KeysENUM.MODELLO_DISPOSITIVO_STAMPA);


            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            var dispositivoStampaUtente = ws.AmmGetDispositivoStampaUtente(UserManager.getInfoUtente().idPeople);
            if (dispositivoStampaUtente != null)
            {
                this.hd_dispositivo.Value = "Etichette";
                this.hd_modello_dispositivo.Value = dispositivoStampaUtente.ToString();
            }
            else
                this.hd_dispositivo.Value = "Penna";
			#endregion parametro Dispositivo Di Stampa

			#region parametro Descrizione Amministrazione

			string descAmm = getDescAmministrazione(this.GetLoginManager().GetUtente().idAmministrazione);
			
			#endregion parametro Descrizione Amministrazione

			#region parametro Classifica Primaria

			string classificaPrimaria = String.Empty;

			string classificazioneInEtichetta = System.Configuration.ConfigurationManager.AppSettings["StampaClassificazioneInEtichetta"];
			if(classificazioneInEtichetta != null)
			{
				switch(classificazioneInEtichetta)
				{
					case "1": // stampa il codice classifica In Etichetta
						classificaPrimaria = getClassificaPrimaria();
						break;
					default:
						//massimo digregorio, non necessario se l'assegnazione avviene in dichiarazione. old: classificaPrimaria = String.Empty;
						break;
				}
			}
			this.hd_classifica.Value = classificaPrimaria;

			#endregion parametro Classifica Primaria

			#region parametro Fascicolo primario

			string fascicoloInEtichetta = System.Configuration.ConfigurationManager.AppSettings["StampaFascicoloInEtichetta"];
			if(fascicoloInEtichetta != null)
			{
				switch(fascicoloInEtichetta)
				{
					case "1": // stampa il codice fascicolo In Etichetta
						this.hd_fascicolo.Value = this.GetCodiceFascicolo();
						break;
					default:
						this.hd_fascicolo.Value = String.Empty;
						break;
				}
			}

			#endregion parametro Fascicolo primario

			#region patch per cuneo 

			string descAmministrInEtichetta = System.Configuration.ConfigurationManager.AppSettings["StampaDescrizioneAmministrazioneInEtichetta"];
			if(descAmministrInEtichetta != null)
			{
				switch(descAmministrInEtichetta)
				{
					case "1": // Stampa Descrizione Amministrazione In Etichetta
						this.hd_amministrazioneEtichetta.Value = descAmm;
						break;
					default:								
						this.hd_amministrazioneEtichetta.Value = String.Empty;
						break;
				}
			}

			//aggiuto tag Hidden "hd_desAmministrazione" per ActiveX di stampa
			/* se parametro esiste ed a 0, a hd_desAmministrazione viene assegnata la classifica
					 * se parametro non esiste o esiste <> 0, a hd_desAmministrazione viene assegnata la descrizione dell'amministrazione
					 */ 
			bool BarCodeConAmministrazione = true;
			DocsPAWA.DocsPaWR.Configurazione visualizzaClassificaSopraBarCode = UserManager.getParametroConfigurazione(this.Page);
					
			if (visualizzaClassificaSopraBarCode != null)
			{
				if (visualizzaClassificaSopraBarCode.valore.Equals("0") ) BarCodeConAmministrazione = false;
			}

			if (BarCodeConAmministrazione)					
			{
				this.hd_descrizioneAmministrazione.Value = descAmm;
			}
			else
			{
				this.hd_descrizioneAmministrazione.Value = classificaPrimaria;
			}

			#endregion patch per cuneo 

			#region parametro URL File di configurazione Dispositivo di Stampa 

			this.hd_UrlIniFileDispositivo.Value=ConfigSettings.getKey(ConfigSettings.KeysENUM.URL_INIFILE_DISPOSITIVO_STAMPA);
			
			#endregion parametro URL File di configurazione Dispositivo di Stampa 

			#region parametri scheda Documento

			this.hd_signature.Value=schedaDocumento.protocollo.segnatura;
            this.hd_num_doc.Value = schedaDocumento.docNumber;
            this.hd_dataCreazione.Value = schedaDocumento.dataCreazione;
            this.hd_codiceUoCreatore.Value = schedaDocumento.creatoreDocumento.uo_codiceCorrGlobali;

            //CAMBIA l'ETICHETTA PER LA STAMPA A SECONDA DEL VALORE SETTATO IN AMMINISTRAZIONE
            DocsPAWA.DocsPaWR.InfoUtente infoUser = new DocsPAWA.DocsPaWR.InfoUtente();
            DocsPAWA.DocsPaWR.Utente utente = DocsPAWA.UserManager.getUtente();
            DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            DocsPAWA.DocsPaWR.EtichettaInfo[] eti = wws.getEtichetteDocumenti(infoUser, utente.idAmministrazione);

            if (schedaDocumento.tipoProto.Equals("A"))
            {
                this.hd_tipo_proto.Value = eti[0].Descrizione;
            }
            else
            {
                if (schedaDocumento.tipoProto.Equals("P"))
                {
                    this.hd_tipo_proto.Value = this.hd_tipo_proto.Value = eti[1].Descrizione;
                }
                else
                {
                    this.hd_tipo_proto.Value = this.hd_tipo_proto.Value = eti[2].Descrizione;
                }
            }
			//this.hd_tipo_proto.Value = schedaDocumento.tipoProto;

			this.hd_coduo_proto.Value = String.Empty;//è gestito sul db e sull'oggetto ruolo utente attuale, ma non nell'oggetto schedaDocumento;

            if (schedaDocumento.registro != null)
            {
                this.hd_codreg_proto.Value = schedaDocumento.registro.codRegistro;
                this.hd_descreg_proto.Value = schedaDocumento.registro.descrizione;
            }

			if (schedaDocumento.protocollo != null) 
			{
				//Celeste
				//this.hd_num_proto.Value = schedaDocumento.protocollo.numero;
				this.hd_num_proto.Value = Utils.formatProtocollo(schedaDocumento.protocollo.numero);
				//Fine Celeste
				this.hd_anno_proto.Value = schedaDocumento.protocollo.anno;

                if (abilita_multi_stampa_etichetta.Equals("1"))
                    this.hd_data_proto.Value = Utils.dateLength(schedaDocumento.protocollo.dataProtocollazione);
                else if (schedaDocumento.oraCreazione != null && schedaDocumento.oraCreazione != "")
                    this.hd_data_proto.Value = Utils.dateLength(schedaDocumento.protocollo.dataProtocollazione) + " " + Utils.timeLength(schedaDocumento.oraCreazione);
                else
                    this.hd_data_proto.Value = Utils.dateLength(schedaDocumento.protocollo.dataProtocollazione);

				//massimo digregorio new:
				if(schedaDocumento.protocollatore != null)
					this.hd_coduo_proto.Value = schedaDocumento.protocollatore.uo_codiceCorrGlobali;
			}

            if (!string.IsNullOrEmpty(schedaDocumento.oraCreazione))
            {
                this.hd_ora_creazione.Value = Utils.timeLength(schedaDocumento.oraCreazione);
                this.hd_ora_creazione.Value = this.hd_ora_creazione.Value.Substring(0, 5);
            }
			#endregion parametri scheda Documento			

            #region stampa multipla etichetta
            //preparo gli attributi per il numero di stampe effettuate finora e da effettuare ora
            if (abilita_multi_stampa_etichetta.Equals("1"))
            {
                if (!string.IsNullOrEmpty(txt_num_stampe))
                    this.hd_num_stampe.Value = txt_num_stampe;
                else
                        this.hd_num_stampe.Value = "1";
                // recupero il valore di stampa corrente da inserire nella  successiva etichetta da stampare
                int num_stampe_eff;
                if (!String.IsNullOrEmpty(schedaDocumento.protocollo.stampeEffettuate))
                {
                    num_stampe_eff = Convert.ToInt32(schedaDocumento.protocollo.stampeEffettuate) + 1;
                    this.hd_num_stampe_effettuate.Value = num_stampe_eff.ToString();
                }
                else
                    this.hd_num_stampe_effettuate.Value = "1";
            }
            else
            {
                this.hd_num_stampe.Value = "1";
                this.hd_num_stampe_effettuate.Value = "1";
            }
            #endregion stampa multipla etichetta

            #region parametri Allegati (a partire dalla versione 3.5.0)

            this.hd_numeroAllegati.Value = schedaDocumento.allegati.Length.ToString();

            #endregion
        }


		/// <summary>
		/// Reperimento del codice fascicolo a cui il documento è associato
		/// </summary>
		/// <returns>codice Fascicolo</returns>
		private string GetCodiceFascicolo()
		{	
			return DocumentManager.getFascicoloDoc(this.Page,this.GetInfoDocumento());
		}

		/// <summary>
		/// Reperimento del codice classifica a cui la scheda documento è associata.
		/// </summary>
		/// <returns>codice classifica</returns>
		private string getClassificaPrimaria() 
		{
			return DocumentManager.GetClassificaDoc(this.Page,this.GetInfoDocumento().idProfile);
		}

		/// <summary>
		/// Reperimento della descrizione dell' Amministrazione attuale.
		/// </summary>
		/// <param name="IdAmministrazione"></param>
		/// <returns>restituisce la descrizione dell'amministrazione passata con il parametro di input IdAmministrazione</returns>
		private string getDescAmministrazione(string IdAmministrazione)
		{
			string descAmm = string.Empty;
			string returnMsg = string.Empty;
			DocsPAWA.DocsPaWR.Amministrazione[] amministrazioni = UserManager.getListaAmministrazioni(this.Page,out returnMsg);
			
			if(amministrazioni.Length == 1) 
			{
				descAmm = amministrazioni[0].descrizione;
			} 
			else 
			{
				bool found = false;
				int i=0;
				
				while((!found) && (i < amministrazioni.Length))
				{
					if (amministrazioni[i].systemId == IdAmministrazione )
					{
						found = true;
						descAmm = amministrazioni[i].descrizione;
					}

					i++;
				}
			}

			return descAmm;
		}

		private void CreateInfoDocumento(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento)
		{
			this._infoDocumento=DocsPAWA.DocumentManager.getInfoDocumento(schedaDocumento);
		}

		private DocsPAWA.DocsPaWR.InfoDocumento GetInfoDocumento()
		{
			return this._infoDocumento;
		}

		private Login.LoginMng GetLoginManager()
		{
			if (this._loginMng==null)
				this._loginMng=new Login.LoginMng(this.Page);

			return this._loginMng;
		}

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), scriptKey))
            {
                this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), scriptKey, scriptValue, true);
            }
        }
	}
}
