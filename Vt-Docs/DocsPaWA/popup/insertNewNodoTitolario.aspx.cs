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

namespace DocsPAWA.popup
{
	/// <summary>
	/// Classe inserimento nuovo nodo di titolario dall'utente DocsPA.
	/// </summary>
    public class insertNewNodoTitolario : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.TextBox txt_descrizione;
		protected System.Web.UI.WebControls.Button btn_insert;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.TextBox txt_codice_padre;
		protected System.Web.UI.WebControls.TextBox txt_codice;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_from;
		protected System.Web.UI.WebControls.TextBox txt_mesi;
		protected System.Web.UI.WebControls.Label lbl_padre;
		protected DocsPAWA.DocsPaWR.Fascicolo _fascSelezionato;
		protected System.Web.UI.WebControls.Label lbl_nota;		
		protected DocsPAWA.DocsPaWR.Registro _registro;
        protected System.Web.UI.WebControls.DropDownList ddl_tipologiaFascicoli;
        protected System.Web.UI.WebControls.Panel pnl_ProfilazioneFascicoli;
        protected System.Web.UI.WebControls.Panel pnl_protTitolario;
        protected System.Web.UI.WebControls.Label lbl_protTitolario;
        protected System.Web.UI.WebControls.TextBox txt_protTitolario;
        protected System.Web.UI.WebControls.CheckBox cb_bloccaTipoFascicoli;
        protected DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
        
	
		/// <summary>
		/// Page Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
            Page.Response.Expires = -1;

            if (!IsPostBack)
            {
                this.Inizilize();
                this.txt_protTitolario.Attributes.Add("onKeyPress", "ValidateNumericKey();");
            }

            //Profilazione dinamica fascicoli
            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
            {
                pnl_ProfilazioneFascicoli.Visible = true;
                //CaricaCombo tipologia fascicoli
                CaricaComboTipologiaFasc();
            }

            //Protocollo titolario
            string protTitolario = wws.isEnableContatoreTitolario();
            if (!string.IsNullOrEmpty(protTitolario))
            {
                pnl_protTitolario.Visible = true;
                lbl_protTitolario.Text = protTitolario + " *";
            }
		}

		/// <summary>
		/// Inizializzazione
		/// </summary>
		private void Inizilize()
		{
			Session["Titolario"] = "Y";

			this.txt_codice_padre.Text = Request.QueryString["val"].ToString();
			this.hd_from.Value = Request.QueryString["from"].ToString();

			_fascSelezionato = DocsPAWA.FascicoliManager.getFascicoloDaCodice(this,this.txt_codice_padre.Text);			
			_registro = DocsPAWA.UserManager.getRegistroSelezionato(this);

			if(_fascSelezionato!=null && _registro!=null)
				this.lbl_padre.Text = "Inserisci sotto il nodo di titolario:<br>" +
					_fascSelezionato.codice + " - " + _fascSelezionato.descrizione +
					"<br><br>" +
					"Registro:<br>" +
					_registro.codRegistro + " - " + _registro.descrizione;

            if (wws.isEnableContatoreTitolario() != "")
            {
                DocsPAWA.DocsPaWR.OrgNodoTitolario nodoTitSel = this.GetCurrentTitolario();
                txt_protTitolario.Text = wws.getContatoreProtTitolario(nodoTitSel).ToString();
            }
		}

		/// <summary>
		/// Imposta Focus() JS
		/// </summary>
		/// <param name="ctrl"></param>
		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
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
			this.btn_insert.Click += new System.EventHandler(this.btn_insert_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.ID = "insertNewNodoTitolario";
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Esecuzione JS
		/// </summary>
		/// <param name="js"></param>
		private void ExecuteJS(string js)
		{
			if(!this.Page.IsStartupScriptRegistered("executeJS"))											
				this.Page.RegisterStartupScript("executeJS", js);			
		}

		/// <summary>
		/// Tasto Chiudi
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{		
			Session.Remove("Titolario");		
			this.ExecuteJS("<SCRIPT>window.returnValue = 'N'; window.close()</SCRIPT>");			
		}

		/// <summary>
		/// Tasto Inserisci
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_insert_Click(object sender, System.EventArgs e)
		{
            if (txt_descrizione.Text.Length > 2000)
                this.ExecuteJS("<SCRIPT>alert('Il numero di caratteri inseriti per il campo descrizione e superiore al limite massimo');</SCRIPT>");
            else
            {
                if (this.VerificaCampiInseriti())
                    this.SaveTitolario();
                else
                    this.ExecuteJS("<SCRIPT>alert('I campi inseriti non sono validi');</SCRIPT>"); this.SetFocus(this.txt_codice);
            }
		}

		/// <summary>
		/// Verifica i campi obbligatori e numerici
		/// </summary>
		/// <returns></returns>
		private bool VerificaCampiInseriti()
		{
			bool retValue = true;
			
			if(this.txt_codice.Text.Trim().Equals("") || 
			    this.txt_descrizione.Text.Trim().Equals("") ||
				!DocsPAWA.Utils.isNumeric(this.txt_mesi.Text) ||
                !DocsPAWA.Utils.isNumeric(this.txt_protTitolario.Text))
					retValue = false;

			return retValue;
		}

		/// <summary>
		/// Procedura di inserimento nuovo nodo di titolario
		/// </summary>
		private void SaveTitolario()
		{
			try
			{
				DocsPaWR.OrgNodoTitolario parentTitolario=this.GetCurrentTitolario();
				if(parentTitolario!=null)
				{
					DocsPaWR.OrgNodoTitolario newNodoTitolario=this.CreateNewNodoTitolario(parentTitolario);

					if (newNodoTitolario!=null)
					{
                        //inserisce il nuovo nodo di titolario ed estende la visibilità
                        //ai soli ruoli che già "vedono" il nodo padre
                        DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                        
                        //ABBATANGELI GIANLUIGI - L'operazione di inserimento implica la copia dei diritti di visibilità 
                        //che spesso prevede tempi molto più lunghi del normale timeout
                        ws.Timeout = System.Threading.Timeout.Infinite;

                        DocsPaWR.EsitoOperazione ret = ws.AmmInsertTitolario(UserManager.getInfoUtente(), ref newNodoTitolario, UserManager.getInfoUtente().idAmministrazione);

						if (ret.Codice==0)
						{
							DocsPaWR.FascicolazioneClassificazione classificazione = new DocsPAWA.DocsPaWR.FascicolazioneClassificazione();
							classificazione.codice = newNodoTitolario.Codice;

							if(this.hd_from.Value.Equals("docClassifica"))
								DocsPAWA.DocumentManager.setClassificazioneSelezionata(this,classificazione);
							else
								FascicoliManager.setClassificazioneSelezionata(this,classificazione);

							this.ExecuteJS("<SCRIPT>window.close();</SCRIPT>");
						}
						else
						{
							this.ExecuteJS("<SCRIPT>alert('"+ret.Descrizione.Replace("'","\'")+"');</SCRIPT>");
						}
					}
				}
			}
			catch
			{
				this.ExecuteJS("<SCRIPT>alert('Attenzione,\\nsi è verificato un errore durante la creazione del nuovo nodo di titolario');</SCRIPT>");
			}
		}	
		
		/// <summary>
		/// Creazione oggetto nodo titolario
		/// </summary>
		/// <param name="parentNodoTitolario"></param>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.OrgNodoTitolario CreateNewNodoTitolario(DocsPAWA.DocsPaWR.OrgNodoTitolario parentNodoTitolario)
		{
			DocsPaWR.OrgNodoTitolario retValue=new DocsPAWA.DocsPaWR.OrgNodoTitolario();

			if (parentNodoTitolario!=null)
			{
				retValue.IDParentNodoTitolario=parentNodoTitolario.ID;
				retValue.Codice=parentNodoTitolario.Codice + "." + this.txt_codice.Text;
			}
			else
			{
				retValue.IDParentNodoTitolario="0";
				retValue.Codice=this.txt_codice.Text;
			}	

			retValue.Descrizione=this.txt_descrizione.Text;
			retValue.IDRegistroAssociato=parentNodoTitolario.IDRegistroAssociato;
			retValue.CodiceAmministrazione=parentNodoTitolario.CodiceAmministrazione;
			retValue.CreazioneFascicoliAbilitata=true;
            retValue.ID_Titolario = parentNodoTitolario.ID_Titolario;

            retValue.bloccaNodiFigli = parentNodoTitolario.bloccaNodiFigli;
            retValue.contatoreAttivo = parentNodoTitolario.contatoreAttivo;
            retValue.numProtoTit = txt_protTitolario.Text;

            int numeroMesiConservazione=0;
			if (this.txt_mesi.Text.Length>0)
				numeroMesiConservazione=Convert.ToInt32(this.txt_mesi.Text);
			retValue.NumeroMesiConservazione=numeroMesiConservazione;

			if (parentNodoTitolario!=null)
			{
				retValue.Livello=(Convert.ToInt32(parentNodoTitolario.Livello) + 1).ToString();
				retValue.CodiceLivello=this.GetCodiceLivello(parentNodoTitolario.CodiceLivello,
					retValue.Livello,
					retValue.CodiceAmministrazione);
			}
			else
			{
				retValue.Livello="1";
				retValue.CodiceLivello=this.GetCodiceLivello("",retValue.Livello,retValue.CodiceAmministrazione);
			}

            if (System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] != null && System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"] == "1")
            {
                retValue.ID_TipoFascicolo = ddl_tipologiaFascicoli.SelectedValue;
                if(cb_bloccaTipoFascicoli.Checked)
                    retValue.bloccaTipoFascicolo = "SI";
                else
                    retValue.bloccaTipoFascicolo = "NO";
            }
            else
            {
                retValue.ID_TipoFascicolo = "";
            }

            retValue.consentiClassificazione = "1";

            return retValue;
		}

		/// <summary>
		/// Creazione oggetto nodo di titolario padre
		/// </summary>
		/// <returns></returns>
		private DocsPAWA.DocsPaWR.OrgNodoTitolario GetCurrentTitolario()
		{
            /*Commentato perchè si è esposto un web method per recuperare un nodo di titolario
			DocsPaWR.OrgNodoTitolario retValue=new DocsPAWA.DocsPaWR.OrgNodoTitolario();
			
			string codAmm = string.Empty;
			_registro = DocsPAWA.UserManager.getRegistroSelezionato(this);
            if (_registro != null)
                codAmm = _registro.codAmministrazione;

            string idTitolario = Request.QueryString["idTit"].ToString();

            _fascSelezionato = DocsPAWA.FascicoliManager.getFascicoloDaCodice2(this, this.txt_codice_padre.Text, idTitolario);			

			if(_fascSelezionato!=null)
			{
				retValue.ID = _fascSelezionato.idClassificazione;
				retValue.Codice = _fascSelezionato.codice;
				retValue.IDRegistroAssociato = _fascSelezionato.idRegistroNodoTit;
				retValue.CodiceAmministrazione = codAmm;
                retValue.ID_Titolario = _fascSelezionato.idTitolario;
				
                retValue.Livello = this.getDataFromProject("NUM_LIVELLO","WHERE SYSTEM_ID="+retValue.ID);
				retValue.CodiceLivello = this.getDataFromProject("VAR_COD_LIV1","WHERE SYSTEM_ID="+retValue.ID);
			}
	        
            return retValue;
            */
            
            DocsPaWR.OrgNodoTitolario nodoTitolario = new DocsPAWA.DocsPaWR.OrgNodoTitolario();
            string idTitolario = Request.QueryString["idTit"].ToString();
            _fascSelezionato = DocsPAWA.FascicoliManager.getFascicoloDaCodice2(this, this.txt_codice_padre.Text, idTitolario);

            if (_fascSelezionato != null)
            {
                nodoTitolario = wws.getNodoTitolarioById(_fascSelezionato.idClassificazione);
            }

            return nodoTitolario;
		}

		/// <summary>
		/// Gestione del codice livello del nodo
		/// </summary>
		/// <param name="codliv_padre"></param>
		/// <param name="livello"></param>
		/// <param name="codAmm"></param>
		/// <returns></returns>
		private string GetCodiceLivello(string codliv_padre, string livello, string codAmm)
		{
			string codliv = null;

			try
			{
				AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

				codliv = ws.PrendeCodLiv(codliv_padre,livello,codAmm,"","");

				int lung = codliv.Length;
				switch(lung)
				{
					case 1:
						codliv = codliv_padre + "000" + codliv;
						break;
					case 2:
						codliv = codliv_padre + "00" + codliv;
						break;
					case 3:
						codliv = codliv_padre + "0" + codliv;
						break;	
					case 4:
						codliv =codliv_padre + codliv;
						break;	
				}

				codliv = codliv.Replace("%","");
			}
			catch
			{
				
			}
			
			return codliv;
		}

		/// <summary>
		/// Reperimento dati dalla tabella Project
		/// </summary>
		/// <param name="campo"></param>
		/// <param name="condizione"></param>
		/// <returns></returns>
		public string getDataFromProject(string campo, string condizione)
		{
			AmmUtils.WebServiceLink ws=new AmmUtils.WebServiceLink();
			return ws.GetDataFromProject(campo, condizione);
        }

        #region profilazione fascicoli
        private void CaricaComboTipologiaFasc()
        {
            if (ddl_tipologiaFascicoli.Items.Count == 0)
            {
                ArrayList listaTipiFasc = new ArrayList(ProfilazioneFascManager.getTipoFascFromRuolo(UserManager.getInfoUtente(this).idAmministrazione, UserManager.getRuolo(this).idGruppo, "2",this));
                
                ListItem item = new ListItem();
                item.Value = "";
                item.Text = "";
                ddl_tipologiaFascicoli.Items.Add(item);
                for (int i = 0; i < listaTipiFasc.Count; i++)
                {
                    DocsPAWA.DocsPaWR.Templates templates = (DocsPAWA.DocsPaWR.Templates)listaTipiFasc[i];
                    ListItem item_1 = new ListItem();
                    item_1.Value = templates.SYSTEM_ID.ToString();
                    item_1.Text = templates.DESCRIZIONE;
                    ddl_tipologiaFascicoli.Items.Add(item_1);
                }
            }
        }
        #endregion
    }
}
