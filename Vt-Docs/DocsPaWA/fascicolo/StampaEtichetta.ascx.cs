

using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;



namespace DocsPAWA.fascicolo
{
   

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

        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_fasc_classifica;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_fasc_codice;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd_fasc_desc;

  

	
		
		

        string _classifica;
        string _codice;
        string _descrizione;
        public string classifica
        {
            get { return ViewState["_classifica"].ToString(); }
            set {    _classifica = value;
                      ViewState["_classifica"] = _classifica;
                }
        }


        public string codice
        {
            get { return ViewState["_codice"].ToString(); }
            set {    _codice = value;
                     ViewState["_codice"] = _codice;
                }
        }

        public string descrizione
        {
            get { return ViewState["_descrizione"].ToString(); }
            set { _descrizione = value;
                    ViewState["_descrizione"] = _descrizione;
                }
        }

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
		public void Stampa(bool showPrintMessage)
		{
			

			this.FillCampiEtichetta();

		

			this.RegisterClientScript("PrintSignature","PrintSignature(" + showPrintMessage.ToString().ToLower() + ");");
		}

		/// <summary>
		/// Caricamento dei dati utilizzabili per la stampa dell'etichetta
		/// in un'insieme di campi testo nascosti
		/// </summary>
		/// <param name="schedaDocumento"></param>
	


        public void FillCampiEtichetta()
        {
           

            //if (ConfigSettings.getKey(ConfigSettings.KeysENUM.DISPOSITIVO_STAMPA) != null)
            //{
            //    this.hd_dispositivo.Value = ConfigSettings.getKey(ConfigSettings.KeysENUM.DISPOSITIVO_STAMPA);
            //}
            //else
            //{
            //    this.hd_dispositivo.Value = "Penna";
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


            

            this.hd_UrlIniFileDispositivo.Value = ConfigSettings.getKey(ConfigSettings.KeysENUM.URL_INIFILE_DISPOSITIVO_STAMPA_FASC);

            





            this.hd_fasc_classifica.Value = this.classifica;
            this.hd_fasc_codice.Value = this.codice;
            this.hd_fasc_desc.Value = this.descrizione;
            

          
            
                     

         
        }

		/// <summary>
		/// Reperimento del codice fascicolo a cui il documento è associato
		/// </summary>
		/// <returns>codice Fascicolo</returns>
	

	

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
