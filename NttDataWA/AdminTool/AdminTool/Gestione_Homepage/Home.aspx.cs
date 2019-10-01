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
using System.IO;
using System.Configuration;
using SAAdminTool.DocsPaWR;

namespace Amministrazione.Gestione_Homepage
{
	/// <summary>
	/// La homepage viene disegnata in modo diverso rispetto alla tipologia di utente amministratore:
	/// 
	///		- SYSTEM ADMINISTRATOR:		gestisce tutte le amministrazioni presenti;
	///		(tipo 1)					può creare nuove amministrazioni;
	///									può eliminare le amministrazioni;			
	///									non deve inserire obbligatoriamente i seguenti campi:
	///										1. segnatura;
	///										2. fascicolatura;
	///										3. ragioni di trasmissione (TO e CC)			
	/// 
	///		------------------------------------------------------------------------------------------						
	/// 
	///		- SUPER ADMIN:				gestisce solo l'amministrazione a cui appartiene;
	///		(tipo 2)					non può creare nuove amministrazioni;
	///									non può eliminare la propria amministrazione;
	/// 
	///	    ------------------------------------------------------------------------------------------
	/// 
	///		- USER ADMIN:				non gestisce la homepage!	
	///     (tipo 3)
	///		------------------------------------------------------------------------------------------
	/// </summary>
	public class Home : System.Web.UI.Page
	{
		#region WebControls e variabili
        protected AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
		protected System.Web.UI.WebControls.Label lbl_avviso;
		protected System.Web.UI.WebControls.Label lbl_position;
		protected System.Web.UI.WebControls.DropDownList ddl_amministrazioni;
		protected System.Web.UI.WebControls.Button btn_nuova;
		protected System.Web.UI.WebControls.Panel pnl_info;
		protected System.Web.UI.WebControls.Label lbl_cod;
		protected System.Web.UI.WebControls.TextBox txt_codice;
		protected System.Web.UI.WebControls.TextBox txt_descrizione;
		protected System.Web.UI.WebControls.TextBox txt_segnatura;
        protected System.Web.UI.WebControls.TextBox txt_timbro_su_pdf;
		protected System.Web.UI.WebControls.TextBox txt_fascicolatura;
        protected System.Web.UI.WebControls.TextBox txt_from_mail;
		protected System.Web.UI.WebControls.Button btn_salva;
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.HtmlControls.HtmlTableCell lbl_ammPresenti;
		protected System.Web.UI.WebControls.Label lbl_msg;
		protected System.Web.UI.HtmlControls.HtmlTableRow COD_AMM;
		protected System.Web.UI.HtmlControls.HtmlTableRow COD_REG;
		protected System.Web.UI.HtmlControls.HtmlTableRow COD_TITOLO;
		protected System.Web.UI.HtmlControls.HtmlTableRow COD_UO;
		protected System.Web.UI.HtmlControls.HtmlTableRow DATA_ANNO;
		protected System.Web.UI.HtmlControls.HtmlTableRow DATA_COMP;
		protected System.Web.UI.HtmlControls.HtmlTableRow IN_OUT;
		protected System.Web.UI.HtmlControls.HtmlTableRow NUM_PROG;
		protected System.Web.UI.HtmlControls.HtmlTableRow NUM_PROTO;
		protected System.Web.UI.WebControls.ImageButton btn_segn;
        
		protected System.Web.UI.WebControls.Button btn_elimina;
		protected System.Web.UI.WebControls.ImageButton btn_fasc;
		protected System.Web.UI.WebControls.ImageButton btn_chiudi_pnlInfo;
		protected System.Web.UI.WebControls.Label lbl_fascSegn;
		protected System.Web.UI.WebControls.ImageButton btn_chiudi;
		protected System.Web.UI.WebControls.DataGrid dgFascSegn;
		protected System.Web.UI.WebControls.DataGrid dg_Separ;
		protected System.Web.UI.WebControls.Panel pnl_fasc_segn;
		protected System.Web.UI.WebControls.TextBox txt_dominio;
		protected System.Web.UI.WebControls.TextBox txt_smtp;
		protected System.Web.UI.WebControls.TextBox txt_portasmtp;
		protected System.Web.UI.WebControls.CheckBox chk_protoint;
		protected System.Web.UI.WebControls.DropDownList ddl_ragione_to;
		protected System.Web.UI.WebControls.DropDownList ddl_ragione_cc;
		protected System.Web.UI.WebControls.TextBox txt_userid_smtp;
		protected System.Web.UI.WebControls.TextBox txt_pwd_smtp;
        protected System.Web.UI.WebControls.TextBox txt_conferma_pwd_smtp;
		protected System.Web.UI.WebControls.Panel pnl_ddlAmm;
        protected System.Web.UI.WebControls.Panel pnlRagioniSmista;
		protected System.Web.UI.WebControls.Label lbl_titolo_pnl;
		protected System.Web.UI.WebControls.Button btn_annulla;
		protected System.Web.UI.WebControls.CheckBox chk_avviso_todolist;
		protected System.Web.UI.WebControls.TextBox txt_gg_perm_todolist;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hd_idAmm;
		protected System.Web.UI.WebControls.Label lbl_segnatura;
		protected System.Web.UI.WebControls.Label lbl_fascicolatura;		
        protected System.Web.UI.WebControls.CheckBox chkSSLSMTP;
        protected bool modSegnatura = false;
        protected bool modFascicolatura = false;
        protected bool modTimbroPdf = false;
        protected bool modProtocolloTit = false;

       	protected System.Web.UI.WebControls.DropDownList ddl_ragione_COMPETENZA;
        protected System.Web.UI.WebControls.DropDownList ddl_ragione_CONOSCENZA;
        protected System.Web.UI.WebControls.ImageButton btn_timbro_pdf;
        protected System.Web.UI.WebControls.Panel pnl_timbro;        
        protected System.Web.UI.WebControls.DropDownList CarattereList;
        protected System.Web.UI.WebControls.DropDownList ColoreList;
        protected System.Web.UI.WebControls.DropDownList PosizioneList;
        protected System.Web.UI.WebControls.DropDownList RotazioneList;
        protected System.Web.UI.WebControls.DropDownList OrientamentoList;
        protected System.Web.UI.WebControls.Label lbl_timbro_su_pdf;
        protected System.Web.UI.WebControls.Label Rotazione;
        protected SAAdminTool.DocsPaWR.InfoUtenteAmministratore _datiAmministratore = new SAAdminTool.DocsPaWR.InfoUtenteAmministratore();

        //AUTENTICAZIONE DI DOMINIO
        protected System.Web.UI.WebControls.TextBox txt_autenticazione;
        protected System.Web.UI.WebControls.ImageButton btn_aut_dominio;
        protected System.Web.UI.WebControls.DataGrid dg_aut;
        protected System.Web.UI.WebControls.DataGrid dg_sep;
        protected System.Web.UI.WebControls.Label lbl_tit_aut;
        protected System.Web.UI.WebControls.ImageButton btn_chiudi_aut;
        protected System.Web.UI.WebControls.Panel pnl_autenticazione;

        protected System.Web.UI.WebControls.Label lblClientModelProcessor;
        protected System.Web.UI.WebControls.DropDownList cboClientModelProcessor;

        protected System.Web.UI.WebControls.RadioButton rdbIsEnabledSmartClient;
        protected System.Web.UI.WebControls.RadioButton rdbIsEnabledJavaApplet;
        protected System.Web.UI.WebControls.RadioButton rdbDisableAll;

        protected System.Web.UI.WebControls.CheckBox chkSmartClientConvertPdfOnScan;
        protected System.Web.UI.WebControls.CheckBox chkSpedizioneAutomaticaDocumento;
        protected System.Web.UI.WebControls.CheckBox chkTrasmissioneAutomaticaDocumento;
        protected System.Web.UI.WebControls.CheckBox chkAvvisaSuSpedizioneDocumento;
        protected System.Web.UI.WebControls.CheckBox chkTipologiaObblig;

        //Protocollo Titolario
        protected System.Web.UI.WebControls.Panel pnl_protocolloTit_Riscontro;
        protected System.Web.UI.WebControls.Label lbl_protocolloTit_Riscontro;
        protected System.Web.UI.WebControls.TextBox txt_protocolloTit_Riscontro;
        protected System.Web.UI.WebControls.ImageButton btn_protocolloTit_Riscontro;
        protected System.Web.UI.WebControls.Button btn_test;

        //Etichette tipo protocollazione (Fabio)
        protected System.Web.UI.WebControls.ImageButton btn_etichette;
        protected System.Web.UI.WebControls.Panel pnl_etichette;
        protected System.Web.UI.WebControls.Label lbl_titolo_etichette;
        protected System.Web.UI.WebControls.ImageButton btn_chiudi_etichette;
        protected System.Web.UI.WebControls.TextBox Text1;
        protected System.Web.UI.WebControls.TextBox Text2;
        protected System.Web.UI.WebControls.TextBox Text3;
        protected System.Web.UI.WebControls.TextBox Text4;
        protected System.Web.UI.WebControls.TextBox Text5;
        protected System.Web.UI.WebControls.TextBox Descrizione1;
        protected System.Web.UI.WebControls.TextBox Descrizione2;
        protected System.Web.UI.WebControls.TextBox Descrizione3;
        protected System.Web.UI.WebControls.TextBox Descrizione4;
        protected System.Web.UI.WebControls.TextBox Descrizione5;
        protected SAAdminTool.DocsPaWR.EtichettaInfo[] etichette;

        protected System.Web.UI.WebControls.Label lbl_dettFirma;
        protected System.Web.UI.WebControls.TextBox txt_dettFirma;
        protected System.Web.UI.WebControls.Panel pnlDettFirma;


        protected System.Web.UI.WebControls.DropDownList ddlLabelPrinterType;
		#endregion		

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    			
			this.ddl_amministrazioni.SelectedIndexChanged += new System.EventHandler(this.ddl_amministrazioni_SelectedIndexChanged);
			this.btn_nuova.Click += new System.EventHandler(this.btn_nuova_Click);
			this.btn_chiudi.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudi_Click);
			this.dgFascSegn.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgFascSegn_ItemCommand);
			this.dg_Separ.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_Separ_ItemCommand);
			this.btn_segn.Click += new System.Web.UI.ImageClickEventHandler(this.btn_segn_Click);
			this.btn_fasc.Click += new System.Web.UI.ImageClickEventHandler(this.btn_fasc_Click);
            this.btn_aut_dominio.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aut_dominio_Click);
            this.btn_chiudi_aut.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aut_chiudi_Click);
            this.dg_aut.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_aut_ItemCommand);
            this.dg_sep.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dg_sep_ItemCommand);
            this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
			this.btn_elimina.Click += new System.EventHandler(this.btn_elimina_Click);
			this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
			this.Load += new System.EventHandler(this.Page_Load);
            this.btn_test.Click += new System.EventHandler(this.btn_test_Click);
            this.btn_etichette.Click += new System.Web.UI.ImageClickEventHandler(this.btn_etichette_Click);
            this.btn_chiudi_etichette.Click += new System.Web.UI.ImageClickEventHandler(this.btn_chiudi_etichette_Click);

		}
		#endregion

		#region Page Load e inizializzazione dati
		/// <summary>
		/// Page_Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
            Session["AdminBookmark"] = "Home";
            
            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------

			if(Session.IsNewSession)
			{
				Response.Redirect("../Exit.aspx?FROM=EXPIRED");
			}

            if (SAAdminTool.ConfigSettings.getKey(SAAdminTool.ConfigSettings.KeysENUM.T_DO_SMISTA) != null && SAAdminTool.ConfigSettings.getKey(SAAdminTool.ConfigSettings.KeysENUM.T_DO_SMISTA).Equals("1"))
            {

                this.pnlRagioniSmista.Visible = true;
            }
            else
            {
                this.pnlRagioniSmista.Visible = false;
            }
			
			if(!ws.CheckSession(Session.SessionID))
			{
				Response.Redirect("../Exit.aspx?FROM=ABORT");
			}
			// ---------------------------------------------------------------

			//----- GESTIONE DEL TIPO UTENTE AMMINISTRATORE ------------------
			SAAdminTool.AdminTool.Manager.SessionManager session = new SAAdminTool.AdminTool.Manager.SessionManager();
			this._datiAmministratore = session.getUserAmmSession();
			//----------------------------------------------------------------
			
			if (!IsPostBack)
			{				
				// imposta griglie della segnatura e fascicolatura
				this.ImpostaGrigliaSegnFasc();

                caricaDDLPrinter();
                // Caricamento combo dei client word processor
                this.FetchComboClientModelProcessors();

				if(Session["AMMDATASET"]!=null)
					this.ViewCurrentAmm();
				else
					this.Inizialize();

                //Inizializzazione etichette protocolli (Fabio)
                getLettereProtocolli();

                // MEV CS 1.5
                try
                {
                    string idAmm = this.ddl_amministrazioni.SelectedValue;
                    ws.Clear(idAmm);
                }
                catch (Exception ex)
                {
                }
                // end MEV CS 1.5
			}

            //Protocollo titolario / Riscontro
            setLabelPrototolloTitolarioRiscontro();

            SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            rdbIsEnabledJavaApplet.Visible = wws.EnableJavaAppletOption();
        }		
				
		private void Inizialize()
		{
			try
			{												
				// prende tutte le amm.ni disponibili
				SAAdminTool.AdminTool.Manager.AmministrazioneManager amm = new SAAdminTool.AdminTool.Manager.AmministrazioneManager();
				amm.ListaAmministrazioni();

				if(amm.getListaAmministrazioni()!=null && amm.getListaAmministrazioni().Count>0)
				{
					// gestione tipologie di utente amministratore
					if (_datiAmministratore.tipoAmministratore.Equals("2"))
					{
						this.LoadDDLAmministrazioni(amm.getListaAmministrazioni(), _datiAmministratore.idAmministrazione);
						this.onDDLAmmSelectedIndexChanged();
					}	
					else
					{
						this.LoadDDLAmministrazioni(amm.getListaAmministrazioni(),null);
						SAAdminTool.DocsPaWR.InfoAmministrazione firstTime = (SAAdminTool.DocsPaWR.InfoAmministrazione)amm.getListaAmministrazioni()[0];
						this.LoadInfoAmm(firstTime);

						this.GUI("Normal");
					}											
				}
				else
				{					
					this.GUI("Nuova");
					this.ImpostaRagioniTrasmDefault();
				}
			}
			catch
			{
				this.GUI("Errore");
			}
		}		

        public void caricaDDLPrinter() {
            DocsPaWebService wss = new DocsPaWebService();
            ddlLabelPrinterType.DataTextField = "Description";
            ddlLabelPrinterType.DataValueField = "Id";
            ddlLabelPrinterType.DataSource = wss.AmministrazioneGetDispositivoStampaEtichetta();
            ddlLabelPrinterType.DataBind();
            
        }
		public void ddl_amministrazioni_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.onDDLAmmSelectedIndexChanged(); // ..... da rivedere il doppio passaggio !!!!!!!!!!!!!!!!!!!

			//			this.ddl_amministrazioni.Items.Insert(0, new ListItem("Seleziona..."));
			//			this.ddl_amministrazioni.Items.Insert(1, new ListItem(new String( Convert.ToChar("-"), 35)));
		}

		private void onDDLAmmSelectedIndexChanged()
		{
			string idAmm = this.ddl_amministrazioni.SelectedValue;			
			
			SAAdminTool.AdminTool.Manager.AmministrazioneManager mng = new SAAdminTool.AdminTool.Manager.AmministrazioneManager();
			mng.InfoAmmCorrente(idAmm);

			this.LoadInfoAmm(mng.getCurrentAmm());

			this.GUI("Normal");

            // MEV CS 1.5
            try
            {
                ws.Clear(idAmm);
            }
            catch (Exception ex)
            {

            }
            // end MEV CS 1.5
		}

		private void ViewCurrentAmm()
		{
			// prende tutte le amm.ni disponibili
			SAAdminTool.AdminTool.Manager.AmministrazioneManager amm = new SAAdminTool.AdminTool.Manager.AmministrazioneManager();
			amm.ListaAmministrazioni();

			string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"3");
			this.LoadDDLAmministrazioni(amm.getListaAmministrazioni(),idAmm);
			this.LoadInfoAmm(this.getInfoCurrentAmm(amm.getListaAmministrazioni(),idAmm));
			this.GUI("Normal");
		}
		
		private SAAdminTool.DocsPaWR.InfoAmministrazione getInfoCurrentAmm(ArrayList lista, string idCurrentAmm)
		{			
			SAAdminTool.DocsPaWR.InfoAmministrazione currAmm = new SAAdminTool.DocsPaWR.InfoAmministrazione();
			foreach(SAAdminTool.DocsPaWR.InfoAmministrazione amm in lista)
			{
				if(amm.IDAmm.Equals(idCurrentAmm))
				{
					currAmm = amm;
					break;
				}
			}
			return currAmm;
		}

		private void LoadDDLAmministrazioni(ArrayList listaAmministrazioni, string idAmmSelected)
		{			
			this.ddl_amministrazioni.Items.Clear();

			foreach(SAAdminTool.DocsPaWR.InfoAmministrazione currAmm in listaAmministrazioni)
			{
				ListItem items = new ListItem(currAmm.Descrizione,currAmm.IDAmm);								
				this.ddl_amministrazioni.Items.Add(items);
			}			

			if(this.ddl_amministrazioni.Items.Count>1)
				this.ddl_amministrazioni.SelectedIndex = this.ddl_amministrazioni.Items.IndexOf(this.ddl_amministrazioni.Items.FindByValue(idAmmSelected));			
		}
		
		private void LoadInfoAmm(SAAdminTool.DocsPaWR.InfoAmministrazione currAmm)
		{
           
			this.hd_idAmm.Value=currAmm.IDAmm;
			this.txt_codice.Text=currAmm.Codice;
			this.txt_descrizione.Text=currAmm.Descrizione;			
			this.txt_segnatura.Text=currAmm.Segnatura;
            this.txt_timbro_su_pdf.Text=currAmm.Timbro_pdf;
			this.txt_fascicolatura.Text=currAmm.Fascicolatura;
            if(!string.IsNullOrEmpty(ws.isEnableProtocolloTitolario()) || ws.isEnableRiferimentiMittente())
                this.txt_protocolloTit_Riscontro.Text = currAmm.formatoProtTitolario;

            //MEV-Firma 1 < scrivi il dettaglio di firma se l'amminstrazione è abilitata
              pnlDettFirma.Visible = false;
              string keyDettFirma=  SAAdminTool.utils.InitConfigurationKeys.GetValue(currAmm.IDAmm, "FE_DETTAGLI_FIRMA");
                if (!String.IsNullOrEmpty (keyDettFirma))
                    if (keyDettFirma.Equals ("1"))
                    {
                        pnlDettFirma.Visible = true;
                        txt_dettFirma.Text = currAmm.DettaglioFirma;
                    }
            //>

            this.txt_dominio.Text=currAmm.Dominio;
            this.txt_autenticazione.Text = currAmm.formatoDominio;
			this.txt_smtp.Text=currAmm.ServerSMTP;
			this.txt_portasmtp.Text=currAmm.PortaSMTP;
			this.txt_userid_smtp.Text=currAmm.UserSMTP;
			this.txt_pwd_smtp.Text=currAmm.PasswordSMTP;
            this.txt_from_mail.Text = currAmm.FromEmail;
            if(!string.IsNullOrEmpty(txt_from_mail.Text))
                this.btn_test.Visible = true;
            this.chkSSLSMTP.Checked = (currAmm.SslSMTP == "1") ? true : false; 
			this.chk_avviso_todolist.Checked = true;
			if(!currAmm.AttivaGGPermanenzaTDL.Equals("1"))
				this.chk_avviso_todolist.Checked = false;
			this.txt_gg_perm_todolist.Text=currAmm.GGPermanenzaTDL;
            if (!currAmm.TipologiaDocumentoObbligatoria.Equals("1"))
                this.chkTipologiaObblig.Checked = false;
            else
                this.chkTipologiaObblig.Checked = true;

            this.rdbIsEnabledSmartClient.Checked = (currAmm.SmartClientConfigurations.ComponentsType == "2");
            this.rdbIsEnabledJavaApplet.Checked = (currAmm.SmartClientConfigurations.ComponentsType == "3");
            this.rdbDisableAll.Checked = (currAmm.SmartClientConfigurations.ComponentsType == "0" || currAmm.SmartClientConfigurations.ComponentsType == "1");

            this.chkSmartClientConvertPdfOnScan.Checked = currAmm.SmartClientConfigurations.ApplyPdfConvertionOnScan;
            //*******************************************************
            // Giordano Iacozzilli 20/09/2012 
            // Ripristino della sola trasmissione in automatico ai 
            // destinatari interni nei protocolli in uscita
            //*******************************************************
            this.chkTrasmissioneAutomaticaDocumento.Checked = currAmm.SpedizioneDocumenti.TrasmissioneAutomaticaDocumento;
            //************ FINE ***************************

            this.chkSpedizioneAutomaticaDocumento.Checked = currAmm.SpedizioneDocumenti.SpedizioneAutomaticaDocumento;
            this.chkAvvisaSuSpedizioneDocumento.Checked = currAmm.SpedizioneDocumenti.AvvisaSuSpedizioneDocumento;

            this.GetRagioniTrasm(currAmm.IDAmm, currAmm.IDRagioneTO, currAmm.IDRagioneCC, currAmm.IDRagioneCompetenza, currAmm.IDRagioneConoscenza);	
			
            this.ddlLabelPrinterType.SelectedIndex = ddlLabelPrinterType.Items.IndexOf(ddlLabelPrinterType.Items.FindByValue(currAmm.DispositivoStampa.ToString()));
			this.SetSession();
            LoadTimbro(currAmm);

            // Caricamento dati model processor
            this.SetClientModelProcessor(currAmm);
        }

        /// <summary>
        /// Carica le informazioni relative a colori e caratteri per il timbro
        /// </summary>
        /// <param name="currAmm"></param>
        private void LoadTimbro(SAAdminTool.DocsPaWR.InfoAmministrazione currAmm)
        {
            //Rendo invisibile la comboBox rotazione e la relativa label perchè attualmente non è abilitata
            this.RotazioneList.Visible = false;
            this.Rotazione.Visible = false;

            SAAdminTool.DocsPaWR.color[] colore = currAmm.Timbro.color;
            //se non ho caricato i colori ho solo l'item blank per questo deve essere < 2.
            if (ColoreList.Items.Count < 2)
            {
                for (int i = 0; i < colore.Length; i++)
                {
                    ColoreList.Items.Add(colore[i].descrizione);
                    //if (colore[i].id == currAmm.Timbro_colore)
                    //{
                    //    ColoreList.SelectedIndex = i + 1; //aggiungo 1 perchè gli id partono da 1 e non da 0.
                    //}
                }
            }
            SAAdminTool.DocsPaWR.carattere[] carat = currAmm.Timbro.carattere;
            //se non ho caricato i caratteri ho solo l'item blank per questo deve essere < 2.
            if (CarattereList.Items.Count < 2)
            {
                for (int j = 0; j < carat.Length; j++)
                {
                    CarattereList.Items.Add(carat[j].caratName + " - " + carat[j].dimensione);
                }
            }
            //imposto le comboBox al valore configurato il valore 0 corrisponde a nessuna selezione.
            //NB: il valore di default è 1 mentre 0 è nessuna selezione; a livello di back end se non sono
            //    stati selezionati dei valori allora imposta il valore di default (questo vale anche nel 
            //    caso in cui il DB non è stato allineato correttamente!!!)
            if (currAmm.Timbro_carattere != String.Empty)
            {
                try
                {
                    this.CarattereList.SelectedIndex = System.Convert.ToInt32(currAmm.Timbro_carattere);
                }
                catch { }
            }
            else
            {
                this.CarattereList.SelectedIndex = 0;
            }
            if (currAmm.Timbro_colore != String.Empty)
            {
                try
                {
                    this.ColoreList.SelectedIndex = System.Convert.ToInt32(currAmm.Timbro_colore);
                }
                catch { }
            }
            else
            {
                this.ColoreList.SelectedIndex = 0;
            }
            if (currAmm.Timbro_posizione != String.Empty)
            {
                try
                {
                    this.PosizioneList.SelectedIndex = System.Convert.ToInt32(currAmm.Timbro_posizione);
                }
                catch { }
            }
            else
            {
                this.PosizioneList.SelectedIndex = 0;
            }
            if (currAmm.Timbro_rotazione != String.Empty)
            {
                try
                {
                    this.RotazioneList.SelectedValue = currAmm.Timbro_rotazione;
                } catch {}
            }
            else
            {
                this.RotazioneList.SelectedIndex = 0;
            }
            if (currAmm.Timbro_orientamento != String.Empty )
            {
                try
                {
                    this.OrientamentoList.SelectedValue = currAmm.Timbro_orientamento.ToLower();
                }
                catch { }
            }
            else
            {
                this.OrientamentoList.SelectedIndex = 0;
            }
        }

		private SAAdminTool.DocsPaWR.InfoAmministrazione getVideoData()
		{
			SAAdminTool.DocsPaWR.InfoAmministrazione amm = new SAAdminTool.DocsPaWR.InfoAmministrazione();

			amm.IDAmm=this.ddl_amministrazioni.SelectedValue.ToString();
			amm.Codice=this.txt_codice.Text;
			amm.Descrizione=this.txt_descrizione.Text;
			amm.Segnatura=this.txt_segnatura.Text;
            amm.Timbro_pdf=this.txt_timbro_su_pdf.Text;
			amm.Fascicolatura=this.txt_fascicolatura.Text;
            if (!string.IsNullOrEmpty(ws.isEnableProtocolloTitolario()) || ws.isEnableRiferimentiMittente())
                amm.formatoProtTitolario = this.txt_protocolloTit_Riscontro.Text;
			amm.ServerSMTP=this.txt_smtp.Text;
			amm.PortaSMTP=this.txt_portasmtp.Text;
			amm.UserSMTP=this.txt_userid_smtp.Text;
			amm.PasswordSMTP=this.txt_pwd_smtp.Text;
			amm.IDRagioneTO=this.ddl_ragione_to.SelectedValue.ToString();
			amm.IDRagioneCC=this.ddl_ragione_cc.SelectedValue.ToString();

            //inserite per gestione RAGIONI nello smistamento
            amm.IDRagioneCompetenza= this.ddl_ragione_COMPETENZA.SelectedValue.ToString();
            amm.IDRagioneConoscenza = this.ddl_ragione_CONOSCENZA.SelectedValue.ToString();
            //
            amm.SslSMTP = (this.chkSSLSMTP.Checked == true) ? "1" : "0"; 
			amm.AttivaGGPermanenzaTDL="0";
			amm.GGPermanenzaTDL="0";
            amm.FromEmail = this.txt_from_mail.Text;
            //scrivo solo id del dato selezionato nell'oggetto
            amm.Timbro_carattere = this.CarattereList.SelectedIndex.ToString();
            amm.Timbro_colore = this.ColoreList.SelectedIndex.ToString();
            amm.Timbro_posizione = this.PosizioneList.SelectedIndex.ToString();
            amm.Timbro_rotazione = this.RotazioneList.SelectedValue;
            amm.Timbro_orientamento = this.OrientamentoList.SelectedValue;

            amm.SmartClientConfigurations = new SAAdminTool.DocsPaWR.SmartClientConfigurations();
            amm.SmartClientConfigurations.ComponentsType = (this.rdbIsEnabledSmartClient.Checked ? "2" : (this.rdbIsEnabledJavaApplet.Checked ? "3" : (this.rdbDisableAll.Checked? "1":"0")));
            amm.SmartClientConfigurations.ApplyPdfConvertionOnScan = this.chkSmartClientConvertPdfOnScan.Checked;

            amm.DispositivoStampa = Convert.ToInt32(this.ddlLabelPrinterType.SelectedValue.ToString());
            amm.SpedizioneDocumenti = new SAAdminTool.DocsPaWR.ConfigSpedizioneDocumento();
            //*******************************************************
            // Giordano Iacozzilli 20/09/2012 
            // Ripristino della sola trasmissione in automatico ai 
            // destinatari interni nei protocolli in uscita
            //*******************************************************
            amm.SpedizioneDocumenti.TrasmissioneAutomaticaDocumento = this.chkTrasmissioneAutomaticaDocumento.Checked;
            //****************** FINE ***************************

            amm.SpedizioneDocumenti.SpedizioneAutomaticaDocumento = this.chkSpedizioneAutomaticaDocumento.Checked;
            amm.SpedizioneDocumenti.AvvisaSuSpedizioneDocumento = this.chkAvvisaSuSpedizioneDocumento.Checked;
            
			if(this.chk_avviso_todolist.Checked)
			{
				amm.AttivaGGPermanenzaTDL="1";
				amm.GGPermanenzaTDL=this.txt_gg_perm_todolist.Text.Trim();
			}

			// gestione dominio: viene inserita la stringa |new| se è stato modificato il dominio (solo per la modifica dei dati dell'amm.ne)
			if(Session["AMMDATASET"]!=null)
			{
				if(AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"2").Equals(this.txt_dominio.Text))				
				{
					amm.Dominio=this.txt_dominio.Text;
				}
				else
				{
					if(this.txt_dominio.Text.Trim().Equals("") && (AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"2")!=null || AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"2")!=string.Empty))
					{
						//si vuole eliminare il dominio esistente
						amm.Dominio=AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"],"2") + "|del|";
					}
					else
					{
						//si vuole inserire un nuovo dominio
						amm.Dominio=this.txt_dominio.Text + "|new|";
					}
				}
			}
			else
				amm.Dominio=this.txt_dominio.Text;
            amm.formatoDominio = this.txt_autenticazione.Text;
			// gestione della libreria letta dal web.config
			//amm.LibreriaDB=System.Configuration.ConfigurationManager.AppSettings["HUMMINGBIRD"].ToString();
			amm.LibreriaDB=string.Empty;

            amm.IdClientSideModelProcessor = this.GetSelectedProcessorId();
            if (this.chkTipologiaObblig.Checked)
                amm.TipologiaDocumentoObbligatoria = "1";
            else
                amm.TipologiaDocumentoObbligatoria = "0";
            
            //MEV-Firma 1 - Aggiunto dettaglio firma
            //+++INIZIO
            amm.DettaglioFirma = txt_dettFirma.Text;
            //+++FINE



			return amm;
		}
		#endregion

		#region Interfaccia grafica

        private void setLabelPrototolloTitolarioRiscontro()
        {
            //Abilitato Riferimenti Mittente e Protocollo Titolario
            bool riffMittente = ws.isEnableRiferimentiMittente();
            string protTit = ws.isEnableProtocolloTitolario();
            if (protTit != "" && riffMittente)
            {
                pnl_protocolloTit_Riscontro.Visible = true;
                lbl_protocolloTit_Riscontro.Text = "Riscontro / " + protTit + " *";
                return;
            }

            //Abilitato solo Riferimenti Mittente
            if (string.IsNullOrEmpty(protTit) && riffMittente)
            {
                pnl_protocolloTit_Riscontro.Visible = true;
                lbl_protocolloTit_Riscontro.Text = "Riscontro *";
                return;
            }

            //Abilitato solo Protocollo Titolario
            if (protTit != "" && !riffMittente)
            {
                pnl_protocolloTit_Riscontro.Visible = true;
                lbl_protocolloTit_Riscontro.Text = protTit + " *";
                return;
            }
        }

		private void GUI(string from)
		{
			switch (from)
			{
				case "Errore":
					this.lbl_avviso.Text="Attenzione, si è verificato un errore!";
					this.pnl_ddlAmm.Visible=false;
					this.pnl_info.Visible=false;
					this.pnl_fasc_segn.Visible=	false;										
                    this.pnl_timbro.Visible = false;
                    this.pnl_autenticazione.Visible = false;
                    this.pnl_etichette.Visible = false;
					break;

				case "Normal":
					this.lbl_position.Text="&nbsp;&bull;&nbsp;Amministrazione: " + this.ddl_amministrazioni.SelectedItem;
					this.lbl_titolo_pnl.Text="Dettagli";
					
					this.txt_codice.ReadOnly=true;

					this.pnl_ddlAmm.Visible=true;
					this.pnl_info.Visible=true;
					this.pnl_fasc_segn.Visible=	false;
                    this.pnl_timbro.Visible = false;
                    this.pnl_autenticazione.Visible = false;
                    this.pnl_etichette.Visible = false;
	
					this.lbl_avviso.Text=string.Empty;
					this.lbl_msg.Text=string.Empty;
										
					this.btn_annulla.Visible=false;
					this.btn_elimina.Visible=true;
					this.btn_elimina.Attributes.Add("onclick","if (!window.confirm('Sei sicuro di voler eliminare questa amministrazione?')) {return false};");
					this.btn_salva.Text="Modifica";	
					
					// gestione tipologie di utente amministratore -------------------------------------------
					if(_datiAmministratore.tipoAmministratore.Equals("2"))
					{	// SUPER ADMIN
						this.btn_nuova.Visible = false;
						this.btn_elimina.Visible = false;
						this.ddl_amministrazioni.Enabled = false;
					}
					else					
					{	// SYSTEM ADMIN
						// rimuove gli "*" dai campi obbligatori
						if(this.lbl_segnatura.Text.EndsWith("*"))
						{
							this.lbl_segnatura.Text = this.lbl_segnatura.Text.Remove(9);
							this.lbl_fascicolatura.Text = this.lbl_fascicolatura.Text.Remove(13);
                            this.lbl_timbro_su_pdf.Text = this.lbl_timbro_su_pdf.Text.Remove(13);
                            //this.lbl_ragTrasmTO.Text = this.lbl_ragTrasmTO.Text.Remove(58);
                            //this.lbl_ragTrasmCC.Text = this.lbl_ragTrasmCC.Text.Remove(72);
						}
					}
					// ---------------------------------------------------------------------------------------

                    // Impostazione visibilità controlli relativi alla gestione del wordprocessor
                    this.SetClientModelControlsVisibility();

                    this.chkSmartClientConvertPdfOnScan.Enabled = (this.rdbIsEnabledSmartClient.Checked || this.rdbIsEnabledJavaApplet.Checked);
                    
					break;

				case "Nuova":
					this.lbl_position.Text="&nbsp;&bull;&nbsp;Amministrazione: in inserimento...";
					this.lbl_titolo_pnl.Text="Inserimento nuova amministrazione";

					this.lbl_cod.Visible=false;
					this.txt_codice.Visible=true;
					this.txt_codice.ReadOnly=false;
					this.txt_codice.Text=string.Empty;
					this.txt_descrizione.Text=string.Empty;
					this.txt_segnatura.Text=string.Empty;
                    //MEV-Firma 1 - ripulitura campo dettaglio di firma
                    this.txt_dettFirma.Text = string.Empty;
					this.txt_fascicolatura.Text=string.Empty;
                    if (!string.IsNullOrEmpty(ws.isEnableProtocolloTitolario()) || ws.isEnableRiferimentiMittente())
                        this.txt_protocolloTit_Riscontro.Text = string.Empty;
					this.txt_smtp.Text=string.Empty;
					this.txt_portasmtp.Text=string.Empty;
					this.txt_dominio.Text=string.Empty;
					this.txt_userid_smtp.Text=string.Empty;
					this.txt_pwd_smtp.Text=string.Empty;
                    this.txt_from_mail.Text = string.Empty;
                    this.txt_timbro_su_pdf.Text = string.Empty;
                    this.rdbIsEnabledSmartClient.Checked = false;
                    this.rdbIsEnabledJavaApplet.Checked = false;
                    this.rdbDisableAll.Checked = true;
                    this.chkSmartClientConvertPdfOnScan.Checked = false;
                    this.chkSmartClientConvertPdfOnScan.Enabled = false;
                    this.chkSpedizioneAutomaticaDocumento.Checked = false;
                    //*******************************************************
                    // Giordano Iacozzilli 20/09/2012 
                    // Ripristino della sola trasmissione in automatico ai 
                    // destinatari interni nei protocolli in uscita
                    //*******************************************************
                    this.chkTrasmissioneAutomaticaDocumento.Checked = false;
                    // FINE
                    //*******************************************************

                    this.chkAvvisaSuSpedizioneDocumento.Checked = false;
                    
					this.pnl_ddlAmm.Visible=false;
					this.pnl_info.Visible=true;
					this.pnl_fasc_segn.Visible=	false;
                    this.pnl_timbro.Visible = false;
                    this.pnl_autenticazione.Visible = false;
                    this.pnl_etichette.Visible = false;
					
					this.lbl_avviso.Text=string.Empty;
					this.lbl_msg.Text=string.Empty;
										
					this.btn_annulla.Visible=true;
					this.btn_elimina.Visible=false;
					this.btn_salva.Text="Salva";

					this.ddl_amministrazioni.SelectedIndex=0;

                    this.SetClientModelProcessor(null);

					SetFocus(txt_codice);

					// gestione tipologie di utente amministratore -------------------------------------------
					if(_datiAmministratore.tipoAmministratore.Equals("1"))
					{
						if(this.lbl_segnatura.Text.EndsWith("*"))
						{
							// rimuove gli "*" dai campi obbligatori
							this.lbl_segnatura.Text = this.lbl_segnatura.Text.Remove(9);
							this.lbl_fascicolatura.Text = this.lbl_fascicolatura.Text.Remove(13);
                            this.lbl_timbro_su_pdf.Text = this.lbl_timbro_su_pdf.Text.Remove(13);
                            //this.lbl_ragTrasmTO.Text = this.lbl_ragTrasmTO.Text.Remove(58);
                            //this.lbl_ragTrasmCC.Text = this.lbl_ragTrasmCC.Text.Remove(72);
						}
					}
					// ---------------------------------------------------------------------------------------								

                    // Impostazione visibilità controlli relativi alla gestione del wordprocessor
                    this.SetClientModelControlsVisibility();

					break;
			}
		}

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.ClientScript.IsStartupScriptRegistered(this.GetType(), scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), scriptKey, scriptString);
            }
        }
				
		#endregion		
				
		#region Gestione Ragioni di trasmissione

        private void GetRagioniTrasm(string idAmm, string idRagTrasmTO, string idRagTrasmCC, string IDRagTrasmComp, string IDRagTrasmCono)
		{
			SAAdminTool.AdminTool.Manager.AmministrazioneManager mng = new SAAdminTool.AdminTool.Manager.AmministrazioneManager();
			mng.ListaRagioniTrasm(idAmm);

			if(mng.getListaRagioniTrasm()!=null && mng.getListaRagioniTrasm().Count>0)
			{
				this.PulisceRagTrasm();

				foreach(SAAdminTool.DocsPaWR.OrgRagioneTrasmissione currRag in mng.getListaRagioniTrasm())
				{
					//prende solo le ragione di trasm. con destinatari "TUTTI"
					if(currRag.TipoDestinatario==SAAdminTool.DocsPaWR.TipiDestinatariTrasmissioneEnum.Tutti)
					{
						ListItem itemsTo = new ListItem(currRag.Codice,currRag.ID);	
						this.ddl_ragione_to.Items.Add(itemsTo); // TO
	
						ListItem itemsCC = new ListItem(currRag.Codice,currRag.ID);														
						this.ddl_ragione_cc.Items.Add(itemsCC); // CC						
					}

                    ListItem itemsCono = new ListItem(currRag.Codice, currRag.ID);
                    this.ddl_ragione_CONOSCENZA.Items.Add(itemsCono); // CONOSCENZA

                    ListItem itemsComp = new ListItem(currRag.Codice, currRag.ID);
                    this.ddl_ragione_COMPETENZA.Items.Add(itemsComp); // COMPETENZA
				}	
								
				this.ddl_ragione_to.SelectedIndex = this.ddl_ragione_to.Items.IndexOf(this.ddl_ragione_to.Items.FindByValue(idRagTrasmTO));				
				this.ddl_ragione_cc.SelectedIndex = this.ddl_ragione_cc.Items.IndexOf(this.ddl_ragione_cc.Items.FindByValue(idRagTrasmCC));

                this.ddl_ragione_COMPETENZA.SelectedIndex = this.ddl_ragione_COMPETENZA.Items.IndexOf(this.ddl_ragione_COMPETENZA.Items.FindByValue(IDRagTrasmComp));
                this.ddl_ragione_CONOSCENZA.SelectedIndex = this.ddl_ragione_CONOSCENZA.Items.IndexOf(this.ddl_ragione_CONOSCENZA.Items.FindByValue(IDRagTrasmCono));				
			
            }
		}
		
		private void PulisceRagTrasm()
		{
			int contaItems;

			// TO
			contaItems = this.ddl_ragione_to.Items.Count;
			contaItems--;
			for (int n=contaItems; n>1; n--)
			{
				this.ddl_ragione_to.Items.RemoveAt(n);
			}	

			// CC
			contaItems = this.ddl_ragione_cc.Items.Count;
			contaItems--;
			for (int n=contaItems; n>1; n--)
			{
				this.ddl_ragione_cc.Items.RemoveAt(n);
			}

            // COMPETENZA
            contaItems = this.ddl_ragione_COMPETENZA.Items.Count;
            contaItems--;
            for (int n = contaItems; n > 1; n--)
            {
                this.ddl_ragione_COMPETENZA.Items.RemoveAt(n);
            }

            // CONOSCENZA
            contaItems = this.ddl_ragione_CONOSCENZA.Items.Count;
            contaItems--;
            for (int n = contaItems; n > 1; n--)
            {
                this.ddl_ragione_CONOSCENZA.Items.RemoveAt(n);
            }	
            
		}

		private void ImpostaRagioniTrasmDefault()
		{
			try
			{
				SAAdminTool.AdminTool.Manager.AmministrazioneManager mng = new SAAdminTool.AdminTool.Manager.AmministrazioneManager();
				mng.ListaRagioniTrasm(null); // prende tutte quelle dell'anagrafica

				if(mng.getListaRagioniTrasm()!=null && mng.getListaRagioniTrasm().Count>0)
				{
					this.PulisceRagTrasm();

					foreach(SAAdminTool.DocsPaWR.OrgRagioneTrasmissione currRag in mng.getListaRagioniTrasm())
					{
                        ListItem items = new ListItem(currRag.Codice, currRag.ID);
                        //prende solo le ragione di trasm. con destinatari "TUTTI"
						if(currRag.TipoDestinatario==SAAdminTool.DocsPaWR.TipiDestinatariTrasmissioneEnum.Tutti)
						{
							//ListItem items = new ListItem(currRag.Codice,currRag.ID);								
							this.ddl_ragione_to.Items.Add(items); // TO
							this.ddl_ragione_cc.Items.Add(items); // CC
						}

                        this.ddl_ragione_COMPETENZA.Items.Add(items); // COMPETENZA
                        this.ddl_ragione_CONOSCENZA.Items.Add(items); // CONOSCENZA
					}					
				}
			}
			catch
			{
				this.GUI("Errore");
			}
		}

		#endregion

		#region Tasti

		private void btn_nuova_Click(object sender, System.EventArgs e)
		{
			this.GUI("Nuova");
			this.ImpostaRagioniTrasmDefault();
            btn_test.Visible = false;
		}

		private void btn_annulla_Click(object sender, System.EventArgs e)
		{			
			this.Inizialize();
		}

		private void btn_salva_Click(object sender, System.EventArgs e)
		{
            // solo per i SUPER ADMIN verifica i campi obbligatori (vedi nota intestazione classe)
            if (_datiAmministratore.tipoAmministratore.Equals("2"))
            {
                if (this.txt_segnatura.Text.Trim().Equals(string.Empty))
                {
                    this.AlertJS("Attenzione, il campo Segnatura è obbligatorio");
                    this.SetFocus(this.txt_segnatura);
                    return;
                }

                if (this.txt_timbro_su_pdf.Text.Trim().Equals(string.Empty))
                {
                    this.AlertJS("Attenzione, il campo Timbro su PDF è obbligatorio");
                    this.SetFocus(this.txt_timbro_su_pdf);
                    return;
                }

                if (this.txt_fascicolatura.Text.Trim().Equals(string.Empty))
                {
                    this.AlertJS("Attenzione, il campo Fascicolatura è obbligatorio");
                    this.SetFocus(this.txt_fascicolatura);
                    return;
                }                
            }

            if ((!string.IsNullOrEmpty(ws.isEnableProtocolloTitolario()) 
                ||
                ws.isEnableRiferimentiMittente()
                )
                && this.txt_protocolloTit_Riscontro.Text.Trim().Equals(string.Empty))
            {
                this.AlertJS("Attenzione, il campo " + lbl_protocolloTit_Riscontro.Text.Replace('*', ' ') + " è obbligatorio");
                this.SetFocus(this.txt_protocolloTit_Riscontro);
                return;
            }

            if (this.txt_pwd_smtp.Text.Trim() != string.Empty || this.txt_conferma_pwd_smtp.Text.Trim() != string.Empty)
            {
                // controllo dell'uguaglianza
                if (this.txt_pwd_smtp.Text.Trim() != this.txt_conferma_pwd_smtp.Text.Trim())
                {
                    this.AlertJS("Attenzione, la password e la conferma sono differenti");
                    this.SetFocus(this.txt_pwd_smtp);
                }
                else
                {
                    if (VerificaFormatoDominio())
                    {
                        if (btn_salva.Text.Equals("Salva"))
                        {
                            //nuova amministrazione
                            this.InsertAmm();
                        }
                        else
                        {
                            //modifica amministrazione
                            this.UpdateAmm();
                        }
                    }
                }
            }
            else
            {
                if (VerificaFormatoDominio())
                {
                    if (btn_salva.Text.Equals("Salva"))
                    {
                        //nuova amministrazione
                        this.InsertAmm();
                    }
                    else
                    {
                        //modifica amministrazione
                        this.UpdateAmm();
                    }
                }
            }
            if (this.Text1.Text != string.Empty && this.Text2.Text != string.Empty && this.Text3.Text != string.Empty && this.Text4.Text != string.Empty && this.Text5.Text != string.Empty && this.Descrizione1.Text != string.Empty && this.Descrizione2.Text != string.Empty && this.Descrizione3.Text != string.Empty && this.Descrizione4.Text != string.Empty && this.Descrizione5.Text != string.Empty)
            {

                setLettereProtocolli(this.Text1.Text, this.Text2.Text, this.Text3.Text, this.Text4.Text, this.Text5.Text, this.Descrizione1.Text, this.Descrizione2.Text, this.Descrizione3.Text, this.Descrizione4.Text, this.Descrizione5.Text);
            }
            else
            {
                this.AlertJS("Attenzione, devono essere inseriti tutte le etichette e i valori per la protocollazione");
            }
		}

        private void btn_elimina_Click(object sender, System.EventArgs e)
		{			
			this.DeleteAmm();	
		}
		#endregion		

		#region INSERT, UPDATE, DELETE

		private void InsertAmm()
		{
			try
			{
				SAAdminTool.AdminTool.Manager.AmministrazioneManager mng = new SAAdminTool.AdminTool.Manager.AmministrazioneManager();
				SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();

                SAAdminTool.DocsPaWR.InfoAmministrazione infoAmm = this.getVideoData();
                esito = mng.Insert(ref infoAmm);

                if(esito.Codice.Equals(0))
				{
                    
                    this.hd_idAmm.Value = infoAmm.IDAmm;
					this.SetSession();
					ListItem items = new ListItem(this.txt_descrizione.Text, infoAmm.IDAmm);								
					this.ddl_amministrazioni.Items.Add(items);
                    this.ddl_amministrazioni.SelectedIndex = this.ddl_amministrazioni.Items.IndexOf(this.ddl_amministrazioni.Items.FindByValue(infoAmm.IDAmm));
					this.GUI("Normal");
                    //Response.Write("<script language='javascript'>document.location.reload();</script>");	
                    Response.Redirect("Home.aspx");
				}
				else
				{
                    this.executeJS("Attenzione,\\n" + esito.Descrizione);
				}	
			}
			catch
			{
				this.GUI("Errore");
			}
		}

		private void UpdateAmm()
		{
			try
			{
                //if(this.txt_segnatura.Text)
				SAAdminTool.AdminTool.Manager.AmministrazioneManager mng = new SAAdminTool.AdminTool.Manager.AmministrazioneManager();
				SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();

                SAAdminTool.DocsPaWR.InfoAmministrazione infoAmm = this.getVideoData();
                esito = esito = mng.Update(ref infoAmm, this.modFascicolatura, this.modSegnatura, this.modTimbroPdf, this.modProtocolloTit);


				if (esito.Codice.Equals(0))
				{
					if(!this.chk_avviso_todolist.Checked)
						this.txt_gg_perm_todolist.Text="";

					this.SetSession();
					this.ddl_amministrazioni.Items[this.ddl_amministrazioni.SelectedIndex].Text=this.txt_descrizione.Text;
					this.GUI("Normal");
				}
				else
				{
					this.executeJS("Attenzione,\\n" + esito.Descrizione);
				}
			}
			catch
			{
				this.GUI("Errore");
			}
		}

		private void DeleteAmm()
		{
			try
			{
				SAAdminTool.AdminTool.Manager.AmministrazioneManager mng = new SAAdminTool.AdminTool.Manager.AmministrazioneManager();
				SAAdminTool.DocsPaWR.EsitoOperazione esito = new SAAdminTool.DocsPaWR.EsitoOperazione();

				esito = mng.Delete(this.getVideoData());

				if(esito.Codice.Equals(0))
				{
					this.Inizialize();
				}
				else
				{
					this.executeJS("Attenzione,\\n" + esito.Descrizione);	
				}
			}
			catch
			{
				this.GUI("Errore");
			}
		}		
		#endregion

		#region SEGNATURA / FASCICOLATURA / TIMBRO
		
		private void btn_chiudi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			pnl_fasc_segn.Visible=false;
            this.pnl_timbro.Visible = false;
		}

        protected void txt_segnatura_TextChanged(object sender, System.EventArgs e)
        {
            this.modSegnatura = true;
        }

        protected void txt_fascicolatura_TextChanged(object sender, System.EventArgs e)
        {
            this.modFascicolatura = true;
        }

        protected void txt_timbro_su_pdf_TextChanged(object sender, EventArgs e)
        {
            this.modTimbroPdf = true;
        }

        protected void txt_protocolloTit_TextChanged(object sender, EventArgs e)
        {
            this.modProtocolloTit = true;
        }

		/// <summary>
		/// carica i valori per la segnatura nel datagrid
		/// </summary>
		private void ImpostaGrigliaSegnFasc()
		{
			DataTable dt = new DataTable("FASC_SEGN");
			DataColumn dc;			
			dc = new DataColumn("codice");
			dt.Columns.Add(dc);
			dc = new DataColumn("descrizione");
			dt.Columns.Add(dc);
			
			DataRow row;
			row = dt.NewRow();
			row["codice"] = "COD_AMM";
			row["descrizione"] = "Codice dell'Amm.ne";
			dt.Rows.Add(row);

			row = dt.NewRow();
			row["codice"] = "COD_REG";
			row["descrizione"] = "Codice del Registro";
			dt.Rows.Add(row);

			row = dt.NewRow();
			row["codice"] = "COD_TITOLO";
			row["descrizione"] = "Codice e Titolo";
			dt.Rows.Add(row);
			
			row = dt.NewRow();
			row["codice"] = "COD_UO";
			row["descrizione"] = "Codice dell'Unità Org.va";
			dt.Rows.Add(row);
			
			row = dt.NewRow();
			row["codice"] = "DATA_ANNO";
			row["descrizione"] = "Anno";
			dt.Rows.Add(row);
			
			row = dt.NewRow();
			row["codice"] = "DATA_COMP";
			row["descrizione"] = "Data della protoc.ne";
			dt.Rows.Add(row);
			
			row = dt.NewRow();
            row["codice"] = "ORA";
            row["descrizione"] = "Ora della protoc.ne";
            dt.Rows.Add(row);
			
			row = dt.NewRow();
			row["codice"] = "IN_OUT";
            row["descrizione"] = "Tipo di registrazione";
			dt.Rows.Add(row);
			
			row = dt.NewRow();
			row["codice"] = "NUM_PROG";
			row["descrizione"] = "Numero del fascicolo";
			dt.Rows.Add(row);
			
			row = dt.NewRow();
			row["codice"] = "NUM_PROTO";
			row["descrizione"] = "Numero di protocollo";
			dt.Rows.Add(row);
			
            row = dt.NewRow();
            row["codice"] = "COD_REG";
            row["descrizione"] = "Codice AOO";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "NUM_ALLEG";
            row["descrizione"] = "Numero degli allegati";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "CLASSIFICA";
            row["descrizione"] = "Classificazioni presenti";
            dt.Rows.Add(row);

            // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
            row = dt.NewRow();
            row["codice"] = "COD_UO_PROT";
            row["descrizione"] = "Codice UO utente protocollatore";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "COD_UO_VIS";
            row["descrizione"] = "Codice UO utente corrente";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "COD_RF_PROT";
            row["descrizione"] = "Codice RF utente protocollatore";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "COD_RF_VIS";
            row["descrizione"] = "Codice RF utente corrente";
            dt.Rows.Add(row);
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            // GESTIONE PROTOCOLLO TITOLARIO ***********************************************
            row = dt.NewRow();
            row["codice"] = "CONT_TIT";
            row["descrizione"] = "Contatore nodi titolario";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "NUM_FASC";
            row["descrizione"] = "Numero fascicolo";
            dt.Rows.Add(row);
            
            row = dt.NewRow();
            row["codice"] = "PROG_DOC";
            row["descrizione"] = "Progressivo documento in fascicolo";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "DESC_TIT";
            row["descrizione"] = "Descrizione titolario";
            dt.Rows.Add(row);
            // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            
            dgFascSegn.DataSource = dt;
			dgFascSegn.DataBind();
			dgFascSegn.Visible = true;

			dt.Rows.Clear();

			row = dt.NewRow();
			row["codice"] = "|";
			row["descrizione"] = "Pipe";
			dt.Rows.Add(row);
			
			row = dt.NewRow();
			row["codice"] = ".";
			row["descrizione"] = "Punto";
			dt.Rows.Add(row);
			
			row = dt.NewRow();
			row["codice"] = "-";
			row["descrizione"] = "Meno";
			dt.Rows.Add(row);
			
			row = dt.NewRow();
			row["codice"] = "\\";
			row["descrizione"] = "Backslash";
			dt.Rows.Add(row);
			
			row = dt.NewRow();
			row["codice"] = "_";
			row["descrizione"] = "Underscore";
			dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "/";
            row["descrizione"] = "Slash";
            dt.Rows.Add(row);
			
			dg_Separ.DataSource = dt;
			dg_Separ.DataBind();
			dg_Separ.Visible = true;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_segn_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            this.pnl_autenticazione.Visible =   false;
			this.pnl_fasc_segn.Visible		=	true;
            this.pnl_timbro.Visible = false;
            this.pnl_etichette.Visible = false;
			this.lbl_fascSegn.Text			=	"Opzioni per la stringa di Segnatura";
            this.clearDgFascSegn();

			this.dgFascSegn.Items[0].Visible	=	true;
			this.dgFascSegn.Items[1].Visible	=	true;
			this.dgFascSegn.Items[3].Visible	=	true;
            this.dgFascSegn.Items[4].Visible    =   true;
            this.dgFascSegn.Items[5].Visible    =   true;
            //tag ora di protocollazione per il formato della segnatura
			this.dgFascSegn.Items[6].Visible	=	true;
			this.dgFascSegn.Items[7].Visible	=	true;
            this.dgFascSegn.Items[9].Visible    =   true;
            if (ConfigurationManager.AppSettings["ENABLE_CODBIS_SEGNATURA"] != null)
            {
                if (ConfigurationManager.AppSettings["ENABLE_CODBIS_SEGNATURA"] == "1")
                {
                    this.dgFascSegn.Items[14].Visible = false;
                    this.dgFascSegn.Items[15].Visible = true;
                    this.dgFascSegn.Items[16].Visible = false;
                }
                else
                    this.dgFascSegn.Items[15].Visible = false;
            }
            else
                this.dgFascSegn.Items[15].Visible = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btn_fasc_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            this.pnl_autenticazione.Visible =   false;
			this.pnl_fasc_segn.Visible		=	true;
            this.pnl_timbro.Visible = false;
            this.pnl_etichette.Visible = false;
			this.lbl_fascSegn.Text			=	"Opzioni per la stringa di Fascicolatura";
            this.clearDgFascSegn();

			this.dgFascSegn.Items[2].Visible	=	true;
			this.dgFascSegn.Items[8].Visible	=	true;
            this.dgFascSegn.Items[4].Visible	=	true;
            this.dgFascSegn.Items[5].Visible    =   true;

            string protTit = ws.isEnableProtocolloTitolario();
            if(!string.IsNullOrEmpty(protTit))
                this.dgFascSegn.Items[17].Visible = true;
        }

        /// <summary>
        /// Apre pagina etichette protocollazione (Inserita da Fabio)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btn_etichette_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.pnl_autenticazione.Visible = false;
            this.pnl_fasc_segn.Visible = false;
            this.pnl_timbro.Visible = false;
            this.pnl_etichette.Visible = true;
            this.lbl_titolo_etichette.Text = "Etichette per la protocollazione";
            this.clearDgFascSegn();
            getLettereProtocolli();

        }

        /// <summary>
        /// Chiude il pannello etichette protocollazione(Inserita da Fabio)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btn_chiudi_etichette_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.pnl_etichette.Visible = false;
        }

        /// <summary>
        /// Prende i dati esistenti per le etichette dei protocolli (Inserita da Fabio)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void getLettereProtocolli()
        {

            SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();

            SAAdminTool.AdminTool.Manager.SessionManager session = new SAAdminTool.AdminTool.Manager.SessionManager();
            SAAdminTool.DocsPaWR.InfoUtente infoUtente = session.getUserAmmSession();
            String idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            this.etichette = wws.getEtichetteDocumenti(infoUtente, idAmm);
            this.Text1.Text = etichette[0].Descrizione; //Valore A
            this.Descrizione1.Text = etichette[0].Etichetta;
            this.Text2.Text = etichette[1].Descrizione; //Valore P
            this.Descrizione2.Text = etichette[1].Etichetta;
            this.Text3.Text = etichette[2].Descrizione; //Valore I
            this.Descrizione3.Text = etichette[2].Etichetta;
            this.Text4.Text = etichette[3].Descrizione; //Valore G
            this.Descrizione4.Text = etichette[3].Etichetta;
            this.Text5.Text = etichette[4].Descrizione; //Valore ALL
            this.Descrizione5.Text = etichette[4].Etichetta;


        }

        private bool setLettereProtocolli(string arrivo, string partenza, string interno, string grigio, string allegato, string etiArr, string etiPart, string etiInt, string etiGrigio, string etiAll)
        {
            SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            SAAdminTool.AdminTool.Manager.SessionManager session = new SAAdminTool.AdminTool.Manager.SessionManager();
            SAAdminTool.DocsPaWR.InfoUtente infoUtente = session.getUserAmmSession();
            String idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            getLettereProtocolli();
            this.etichette[0].Descrizione = arrivo;
            this.etichette[0].Etichetta = etiArr;
            this.etichette[1].Descrizione = partenza;
            this.etichette[1].Etichetta = etiPart;
            this.etichette[2].Descrizione = interno;
            this.etichette[2].Etichetta = etiInt;
            this.etichette[3].Descrizione = grigio;
            this.etichette[3].Etichetta = etiGrigio;
            this.etichette[4].Descrizione = allegato;
            this.etichette[4].Etichetta = etiAll;

            bool success = wws.setEtichetteDocumenti(infoUtente, idAmm, etichette);
            return success;

        }

        /// <summary>
        /// Visualizza i campi da inserire nel timbro e le opzioni della relativa configurazione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_timbro_pdf_Click(object sender, ImageClickEventArgs e)
        {
            this.pnl_autenticazione.Visible = false;
            this.pnl_fasc_segn.Visible = true;
            this.pnl_timbro.Visible = true;
            this.pnl_etichette.Visible = false;
            this.lbl_fascSegn.Text = "Opzioni per la stringa di Timbro su pdf";
            this.clearDgFascSegn();

            this.dgFascSegn.Items[0].Visible = true;
            //tag codice UO inteso come UO del protocollatore
            this.dgFascSegn.Items[13].Visible = true;
            this.dgFascSegn.Items[5].Visible = true;
            //tag ora di protocollazione per il formato della segnatura
            this.dgFascSegn.Items[6].Visible = true;
            this.dgFascSegn.Items[7].Visible = true;
            this.dgFascSegn.Items[9].Visible = true;
            this.dgFascSegn.Items[10].Visible = true;
            this.dgFascSegn.Items[11].Visible = true;
            this.dgFascSegn.Items[12].Visible = true;
            //Se la chiave è abilitata visualizzo anche i campi UO_VIS, RF_PROT E RF_VIS 
            //(tenendo presente che in seguito bisognerà verificare se il cliente ha gli RF abilitati mediante chiamata ai WS)
            if (ConfigurationManager.AppSettings["ENABLE_CODBIS_TIMBRO"] != null)
            {
                if (ConfigurationManager.AppSettings["ENABLE_CODBIS_TIMBRO"] == "1")
                {
                    this.dgFascSegn.Items[14].Visible = false;
                    this.dgFascSegn.Items[15].Visible = true;
                    this.dgFascSegn.Items[16].Visible = false;
                }
            }
		}

        protected void btn_protocolloTit_Riscontro_Click(object sender, ImageClickEventArgs e)
        {
            this.pnl_autenticazione.Visible = false;
            this.pnl_fasc_segn.Visible = true;
            this.pnl_timbro.Visible = false;
            this.pnl_etichette.Visible = false;
            this.lbl_fascSegn.Text = "Opzioni per la stringa di Protocollo Titolario";
            this.clearDgFascSegn();

            //Abilitato Protocollo Titolario e Riferimenti Mittente o solo Protocollo Titolario
            bool riffMittente = ws.isEnableRiferimentiMittente();
            string protTit = ws.isEnableProtocolloTitolario();
            if ( (protTit != "" && riffMittente) || protTit != "")
            {
                this.dgFascSegn.Items[2].Visible = true;
                this.dgFascSegn.Items[4].Visible = true;
                this.dgFascSegn.Items[5].Visible = true;
                this.dgFascSegn.Items[17].Visible = true;
                this.dgFascSegn.Items[18].Visible = true;
                this.dgFascSegn.Items[19].Visible = true;
                this.dgFascSegn.Items[20].Visible = true;
                return;
            }

            //Abilitato solo Riferimenti Mittente
            if (string.IsNullOrEmpty(protTit) && riffMittente)
            {
                this.dgFascSegn.Items[2].Visible = true;
                this.dgFascSegn.Items[4].Visible = true;
                this.dgFascSegn.Items[5].Visible = true;
                this.dgFascSegn.Items[18].Visible = true;
                return;
            }
        }

        private void clearDgFascSegn()
        {
            for (int i = 0; i < this.dgFascSegn.Items.Count; i++)
            {
                this.dgFascSegn.Items[i].Visible = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void dgFascSegn_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
            switch (lbl_fascSegn.Text)
            {
                case "Opzioni per la stringa di Segnatura":
                    txt_segnatura.Text += e.Item.Cells[1].Text;
                    SetFocus(txt_segnatura);
                    break;

                case "Opzioni per la stringa di Fascicolatura":
                    txt_fascicolatura.Text += e.Item.Cells[1].Text;
                    SetFocus(txt_fascicolatura);
                    break;

                case "Opzioni per la stringa di Timbro su pdf":
                    txt_timbro_su_pdf.Text += e.Item.Cells[1].Text;
                    SetFocus(txt_timbro_su_pdf);
                    break;

                case "Opzioni per la stringa di Protocollo Titolario":
                    txt_protocolloTit_Riscontro.Text += e.Item.Cells[1].Text;
                    SetFocus(txt_protocolloTit_Riscontro);
                    break;
            }			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void dg_Separ_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
            switch (lbl_fascSegn.Text)
            {
                case "Opzioni per la stringa di Segnatura":
                    txt_segnatura.Text += e.Item.Cells[1].Text;
                    SetFocus(txt_segnatura);
                    break;

                case "Opzioni per la stringa di Fascicolatura":
                    txt_fascicolatura.Text += e.Item.Cells[1].Text;
                    SetFocus(txt_fascicolatura);
                    break;

                case "Opzioni per la stringa di Timbro su pdf":
                    txt_timbro_su_pdf.Text += e.Item.Cells[1].Text;
                    SetFocus(txt_timbro_su_pdf);
                    break;

                case "Opzioni per la stringa di Protocollo Titolario":
                    txt_protocolloTit_Riscontro.Text += e.Item.Cells[1].Text;
                    SetFocus(txt_protocolloTit_Riscontro);
                    break;
            }			
		}

		#endregion

        #region autenticazione dominio
        private void btn_aut_chiudi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.pnl_autenticazione.Visible = false;
        }

        private void btn_aut_dominio_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            this.pnl_fasc_segn.Visible = false;
            this.pnl_timbro.Visible = false;
            this.pnl_autenticazione.Visible = true;
            this.pnl_etichette.Visible = false;
            this.lbl_tit_aut.Text = "Opzioni per la stringa di Autenticazione";
            this.ImpostaGrigliaCampiAutenticazione();
            this.ImpostaGrigliaSeparatori();

        }

        private void ImpostaGrigliaCampiAutenticazione()
        {
            DataTable dt = new DataTable("CAMPI");
            DataColumn dc;
            dc = new DataColumn("codice");
            dt.Columns.Add(dc);
            dc = new DataColumn("descrizione");
            dt.Columns.Add(dc);

            DataRow row;
            row = dt.NewRow();
            row["codice"] = "NOME";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "COGNOME";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "DOMINIO";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "USERID";
            dt.Rows.Add(row);

            dg_aut.DataSource = dt;
            dg_aut.DataBind();
            dg_aut.Visible = true;
        }

        private void ImpostaGrigliaSeparatori()
        {
            DataTable dt = new DataTable("SEPARATORI");
            DataColumn dc;
            dc = new DataColumn("codice");
            dt.Columns.Add(dc);
            dc = new DataColumn("descrizione");
            dt.Columns.Add(dc);

            string separatori = "";
            if (System.Configuration.ConfigurationManager.AppSettings["SEPARATORI DOMINIO"] != null)
                separatori = System.Configuration.ConfigurationManager.AppSettings["SEPARATORI DOMINIO"];
            if (separatori != "")
            {
                string[] sepList = separatori.Split(';');
                DataRow row;
                foreach (string sep in sepList)
                {
                    row = dt.NewRow();
                    row["codice"] = sep;
                    dt.Rows.Add(row);
                }
                dg_sep.DataSource = dt;
                dg_sep.DataBind();
                dg_sep.Visible = true;
            }
        }

        private void dg_sep_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            txt_autenticazione.Text += e.Item.Cells[1].Text;
            SetFocus(txt_autenticazione);
        }

        private void dg_aut_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            txt_autenticazione.Text += e.Item.Cells[1].Text;
            SetFocus(txt_autenticazione);
        }

		
        #endregion

        #region Utils

        /// <summary>
        /// gestione controllo password
        /// </summary>
        /// <returns></returns>
        private bool VerificaPassword(string azione)
        {
            bool retValue = true;

            switch (azione)
            {
                case "Aggiungi":
                    if (this.txt_pwd_smtp.Text.Trim() != string.Empty || this.txt_conferma_pwd_smtp.Text.Trim() != string.Empty)
                    {
                        // controllo dell'uguaglianza
                        if (this.txt_pwd_smtp.Text.Trim() != this.txt_conferma_pwd_smtp.Text.Trim())
                        {
                            this.AlertJS("Attenzione, la password e la conferma sono differenti");
                            this.SetFocus(this.txt_pwd_smtp);
                            return false;
                        }
                    }


                    break;

                case "Modifica":
                    if (this.txt_pwd_smtp.Text.Trim() != string.Empty || this.txt_conferma_pwd_smtp.Text.Trim() != string.Empty)
                    {
                        // controllo dell'uguaglianza
                        if (this.txt_pwd_smtp.Text.Trim() != this.txt_conferma_pwd_smtp.Text.Trim())
                        {
                            this.AlertJS("Attenzione, la password e la conferma sono differenti");
                            this.SetFocus(this.txt_pwd_smtp);
                            return false;
                        }

                        // controllo del campo password alfanumerico
                        /*if (!AmmUtils.UtilsXml.IsAlphaNumeric(this.txt_password.Text.Trim()))
                        {
                            this.AlertJS("Attenzione, inserire valori alfanumerici nel campo PASSWORD. Ammesso l'underscore e il punto");
                            this.SetFocus(this.txt_password);
                            return false;
                        }*/

                    }
                    break;
            }
            return retValue;
        }

        private bool VerificaFormatoDominio()
        {
            bool retValue = false;
            if (txt_autenticazione.Text != "")
            {
                string separatori = "";

                //prende la lista di separatori ammessi dal webconfig
                if (System.Configuration.ConfigurationManager.AppSettings["SEPARATORI DOMINIO"] != null)
                    separatori = System.Configuration.ConfigurationManager.AppSettings["SEPARATORI DOMINIO"];
                if (separatori != "")
                {
                    char sepSelezionato = ' ';
                    string[] sepList = separatori.Split(';');
                    int unico = 0;
                    //ricerca fra i separatori della lista ammessi, quello selezionato dall'utente (escluso il punto)
                    foreach (string sep in sepList)
                    {
                        if (this.txt_autenticazione.Text.Contains(sep) && sep != ".")
                        {
                            sepSelezionato = Convert.ToChar(sep);
                            unico++;
                        }
                    }
                    switch (unico.ToString())
                    {
                        case "0":
                            //non sono stati immessi separatori
                            if (separatori.Contains("."))
                            {
                                separatori = separatori.Remove(separatori.IndexOf(";."));
                            }
                           
                            separatori = separatori.Replace(";", ",");
                            if (separatori.EndsWith(","))
                                separatori = separatori.Remove(separatori.Length - 1);
                            this.AlertJS("Attenzione, il formato del dominio deve contenere uno tra i seguenti separatori: " + @"\" + separatori);
                            this.SetFocus(this.txt_autenticazione);
                            retValue = false;
                            break;

                        case "1":
                            //è stato inserito un separatore
                            string[] formato = this.txt_autenticazione.Text.Split(sepSelezionato);
                            //non è nel formato corretto
                            if (formato.Length != 2)
                            {
                                //alert: il formato deve prevedere un separatore tra quelli presenti nel
                                //webconfig voce SEPARATORI DOMINIO e del testo prima e dopo il separatore
                                //esempio: dominio\userid oppure nome.utente@userid
                                this.AlertJS("Attenzione, formato dominio non corretto.");
                                this.SetFocus(this.txt_autenticazione);
                                retValue = false;
                            }
                            else
                            {
                                //parte1SEPparte2 con parte1 o parte2 vuoti o non alfanumerici
                                if (formato[0] != "" && AmmUtils.UtilsXml.IsAlphaNumeric(formato[0]) && formato[1] != "" && AmmUtils.UtilsXml.IsAlphaNumeric(formato[1]))
                                    return true;
                                else
                                {
                                    this.AlertJS("Attenzione, formato dominio non corretto.");
                                    this.SetFocus(this.txt_autenticazione);
                                    retValue = false;
                                }
                            }
                            break;
                        default:
                            this.AlertJS("Attenzione, il formato del dominio deve contenere un unico separatore");
                            this.SetFocus(this.txt_autenticazione);
                            retValue = false;
                            break;
                    }
                }
            }
            else
                retValue = true;
            return retValue;

        }


		private void SetFocus(System.Web.UI.Control ctrl)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
		}					
		

		private void executeJS(string message)
		{
			if(!this.Page.ClientScript.IsStartupScriptRegistered("executeJS"))
			{					
				string scriptString = "<SCRIPT>alert('" + message.Replace("'","\'") + "');</SCRIPT>";				
				//this.Page.RegisterStartupScript("executeJS", scriptString);
                this.ClientScript.RegisterStartupScript(this.GetType(), "executeJS", scriptString);
			}	
		}

        private void AlertJS(string msg)
        {
            if (!this.Page.IsStartupScriptRegistered("alertJavaScript"))
            {
                string scriptString = "<SCRIPT>alert('" + msg.Replace("'", "\\'") + "');</SCRIPT>";
                this.Page.RegisterStartupScript("alertJavaScript", scriptString);
            }
        }

		private void SetSession()
		{
			//rimuove la session attuale e l'aggiorna
			Session.Remove("AMMDATASET");
			Session["AMMDATASET"]=this.txt_codice.Text + "@" + this.txt_descrizione.Text + "@" + this.txt_dominio.Text + "@" + this.hd_idAmm.Value;
		}		
		#endregion

        #region Gestione client model processor

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetSelectedProcessorId()
        {
            if (this.cboClientModelProcessor.SelectedItem != null)
                return Convert.ToInt32(this.cboClientModelProcessor.SelectedItem.Value);
            else
                return 0;
        }

        /// <summary>
        /// Caricamento dati relativi al model processor per l'amministrazione
        /// </summary>
        /// <param name="currAmm"></param>
        private void SetClientModelProcessor(SAAdminTool.DocsPaWR.InfoAmministrazione currAmm)
        {
            if (this.IsEnabledClientModelsProcessor())
            {
                if (currAmm == null)
                    this.cboClientModelProcessor.SelectedValue = "0";
                else
                    this.cboClientModelProcessor.SelectedValue = currAmm.IdClientSideModelProcessor.ToString();
            }
        }

        /// <summary>
        /// Verifica se la creazione dei modelli è abilitata o meno
        /// </summary>
        /// <returns></returns>
        private bool IsEnabledClientModelsProcessor()
        {
            int result;
            Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["MODELLO_DOCUMENTO"], out result);
            return (result > 0);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetClientModelControlsVisibility()
        {
            this.cboClientModelProcessor.Visible = this.IsEnabledClientModelsProcessor();
            this.lblClientModelProcessor.Visible = this.cboClientModelProcessor.Visible;
        }

        /// <summary>
        /// Caricamento combo dei word processors
        /// </summary>
        private void FetchComboClientModelProcessors()
        {
            if (this.IsEnabledClientModelsProcessor())
            {
                using (SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService())
                {
                    SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();

                    foreach (SAAdminTool.DocsPaWR.ModelProcessorInfo processor in ws.GetModelProcessors(sessionManager.getUserAmmSession()))
                    {
                        this.cboClientModelProcessor.Items.Add(new ListItem(processor.name, processor.id.ToString()));
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkIsEnabledSmartClient_OnCheckedChanged(object sender, EventArgs e)
        {
            this.chkSmartClientConvertPdfOnScan.Enabled = (this.rdbIsEnabledSmartClient.Checked || this.rdbIsEnabledJavaApplet.Checked);
            this.chkSmartClientConvertPdfOnScan.Checked = this.chkSmartClientConvertPdfOnScan.Checked && this.chkSmartClientConvertPdfOnScan.Enabled;
        }

        protected void btn_test_Click(object sender, EventArgs e)
        {
           
            
            SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
            SAAdminTool.DocsPaWR.MailRegistro registro = new SAAdminTool.DocsPaWR.MailRegistro();
            SAAdminTool.AdminTool.Manager.AmministrazioneManager amm = new SAAdminTool.AdminTool.Manager.AmministrazioneManager();
            SAAdminTool.DocsPaWR.InfoAmministrazione firstTime = null;
            amm.ListaAmministrazioni();

            if (amm.getListaAmministrazioni() != null && amm.getListaAmministrazioni().Count > 0)
            {
                // gestione tipologie di utente amministratore
                if (_datiAmministratore.tipoAmministratore.Equals("2"))
                {
                    this.LoadDDLAmministrazioni(amm.getListaAmministrazioni(), _datiAmministratore.idAmministrazione);
                    string idAmm = this.ddl_amministrazioni.SelectedValue;

                    SAAdminTool.AdminTool.Manager.AmministrazioneManager mng = new SAAdminTool.AdminTool.Manager.AmministrazioneManager();
                    mng.InfoAmmCorrente(idAmm);

                    firstTime = mng.getCurrentAmm();

                }
                else
                {


                    this.LoadDDLAmministrazioni(amm.getListaAmministrazioni(), null);
                    firstTime = (SAAdminTool.DocsPaWR.InfoAmministrazione)amm.getListaAmministrazioni()[0];
                }
            }
            
            
            registro.Email = txt_from_mail.Text;
            if(firstTime != null)
                registro.PasswordSMTP = firstTime.PasswordSMTP;
            if (!string.IsNullOrEmpty(txt_portasmtp.Text))
                registro.PortaSMTP = int.Parse(txt_portasmtp.Text);
            else
                registro.PortaSMTP = 0;
            registro.ServerSMTP = txt_smtp.Text;
            registro.SMTPssl = (chkSSLSMTP.Checked)? "1" : "0";
            registro.UserSMTP = txt_userid_smtp.Text;

           // this.AlertJS("PRIMA DI CHIUDERE LA PAGINA ATTENDERE IL MESSAGGIO DI CONFERMA DEL TEST!");
            string messaggioErroreSmtp = string.Empty;
            bool smtp = ws.testConnessione(registro, "SMTP", out messaggioErroreSmtp);
            if (smtp)
            {
                Session.Add("messaggiErrorePosta", "Connessione al server SMTP Effettuata con Successo. E' stata inviata una mail di prova per la connessione.");
                string script = "<script>window.showModalDialog('../../popup/avvisoConfermaConnessioneMail.aspx"
                + "','','dialogWidth:302px;dialogHeight:147px;fullscreen:no;toolbar:no;status:no;resizable:no;scroll:auto;"
                + "center:yes;help:no;close:no');</script>";
                this.RegisterStartupScript("popupErroreMail", script);
            }
            else
            {
                Session.Add("messaggiErrorePosta", messaggioErroreSmtp);
                string script = "<script>window.showModalDialog('../../popup/avvisoErroreConnessioneMail.aspx"
                + "','','dialogWidth:302px;dialogHeight:427px;fullscreen:no;toolbar:no;status:no;resizable:no;scroll:auto;"
                + "center:yes;help:no;close:no');</script>";
                this.RegisterStartupScript("popupErroreMail", script);
            }

        }
	}
}
