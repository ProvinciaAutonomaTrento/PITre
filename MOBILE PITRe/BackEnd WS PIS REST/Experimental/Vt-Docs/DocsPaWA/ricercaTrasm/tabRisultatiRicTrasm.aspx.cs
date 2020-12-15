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
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.ricercaTrasm
{
	/// <summary>
	/// Summary description for tabRisultatiRicTrasm.
	/// </summary>
	public class tabRisultatiRicTrasm : DocsPAWA.CssPage
	{
		#region WebControls e variabili

		protected System.Web.UI.HtmlControls.HtmlTableRow tr1;
		protected System.Web.UI.WebControls.DataGrid dt_Eff;
		protected System.Web.UI.WebControls.DataGrid dt_Ric;
		protected System.Web.UI.HtmlControls.HtmlTableRow tr2;			
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_cn;
		protected System.Web.UI.HtmlControls.HtmlTableCell TD1;
		protected System.Web.UI.HtmlControls.HtmlTableCell TD2;		
		protected System.Web.UI.WebControls.Label LabelMsg;		
		protected System.Web.UI.WebControls.Label titolo;		
		protected System.Web.UI.WebControls.Panel pnl_titolo;		
		protected System.Web.UI.WebControls.Panel pnl_dt_Eff;
		protected System.Web.UI.WebControls.Panel pnl_dt_ric;
		protected System.Web.UI.WebControls.Panel pnl_sollecito;
		protected System.Web.UI.WebControls.Button sollecita_tutti;
		protected System.Web.UI.WebControls.Button sollecita_sel;
        protected System.Web.UI.WebControls.Panel pnl_cancellaLista;
        protected System.Web.UI.HtmlControls.HtmlInputHidden hd1;
        //protected DocsPaWebCtrlLibrary.ImageButton btn_SetTxUtAsViste;
        protected DocsPaWebCtrlLibrary.ImageButton btn_smista;
        protected System.Web.UI.WebControls.ImageButton btn_stampa;
        protected System.Web.UI.WebControls.ImageButton btn_rimuoviTDL;        

		//--------------------------------------------------------------------------------------------------		
        protected DocsPAWA.dataSet.DataSetTrasmEff dataSetTrasmEff2;
        protected DocsPAWA.dataSet.DataSetTrasmRic dataSetTrasmRic2;
        protected DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento;
        protected DocsPAWA.DocsPaWR.FiltroRicerca[] filter;
        protected DocsPAWA.DocsPaWR.Trasmissione[] listaTrasmEff;
        protected DocsPAWA.DocsPaWR.Trasmissione[] listaTrasmRic;
        protected DataView dv_Eff;
        protected DataView dv_Ric;	
        protected DocsPAWA.DocsPaWR.Ruolo userRuolo;
        protected DocsPAWA.DocsPaWR.Utente userHome;
        protected Hashtable hashInfoOgg;
		protected string _tipoRicerca = string.Empty;
		protected ArrayList chk;							
		protected string indexchk = string.Empty;
        protected string _isHomepage = string.Empty;

		#endregion

		#region Page Load
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{            
			Utils.startUp(this);
			Page.Response.Expires = 0;
           	Page.Response.Buffer=false;
			
			userRuolo=UserManager.getRuolo(this);
			userHome=UserManager.getUtente(this);

			LabelMsg.Visible			=	false;	
			//this.iFrame_cn.NavigateTo	=	"RisultatiRicTrasm.aspx";
			pnl_dt_Eff.Visible			=	false;
			pnl_dt_ric.Visible			=	false;
			this.tr1.Visible			=	false;
			this.tr2.Visible			=	false;						
			pnl_titolo.Visible			=	true;

            //imposta di default i tasti presenti come NON visibili 
            //this.btn_smista.Visible     =   false;
            //this.btn_rimuoviTDL.Visible =   false;
            //this.btn_stampa.Visible     =   false;   


            if (Request.QueryString["tiporic"] != null || !Request.QueryString["tiporic"].Equals(""))
                this._tipoRicerca = Request.QueryString["tiporic"];

            switch (this._tipoRicerca)
            {
                case "E": // trasmissioni effettuate
                    this.dt_Eff.Columns[8].Visible = true;
                    this.pnl_dt_Eff.Visible = true;
                    this.pnl_dt_ric.Visible = false;
                    this.tr1.Visible = true;
                    this.tr2.Visible = true;
                    Session["tiporic"] = "E";
                    //se la segnatura di repertorio non è abilitata nascondo il campo
                    if (!utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "VIS_SEGNATURA_REPERTORI").Equals("1"))
                    {
                        foreach(DataGridColumn c in this.dt_Eff.Columns)
                        {
                            if(c.HeaderText.Equals("Repertorio"))
                            {
                                c.Visible = false;
                                break;
                            }
                        }
                    }
                    break;

                case "R": // trasmissioni ricevute                    
                    this.pnl_sollecito.Visible = false;
                    this.pnl_dt_Eff.Visible = false;
                    this.pnl_dt_ric.Visible = true;                    
                    this.tr1.Visible = true;
                    this.tr2.Visible = true;
                    Session["tiporic"] = "R";
                    //se la segnatura di repertorio non è abilitata nascondo il campo
                    if (!utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "VIS_SEGNATURA_REPERTORI").Equals("1"))
                    {
                        foreach (DataGridColumn c in this.dt_Ric.Columns)
                        {
                            if (c.HeaderText.Equals("Repertorio"))
                            {
                                c.Visible = false;
                                break;
                            }
                        }
                    }
                    break;

                default:
                    this.pnl_dt_Eff.Visible = false;
                    this.pnl_dt_ric.Visible = false;
                    this.tr1.Visible = false;
                    this.tr2.Visible = false;
                    this.pnl_sollecito.Visible = false;
                    break;
            }					

			// Reperimento filtri correnti
			this.filter=DocumentManager.getFiltroRicTrasm(this);

            if (!IsPostBack)
            {
                this.InitializePageSize();
                //verifica se la pagina è stata chiamata dalla HOMEPAGE
                if (Request.QueryString["home"] != null || !Request.QueryString["home"].Equals(""))
                    this._isHomepage = Request.QueryString["home"];                             

                this.AttatchGridPagingWaitControl();

                int currentPage = this.GetCurrentPageOnContext();

                if (this.filter != null)
                    this.LoadTrasmissioni(this._tipoRicerca, this.filter, currentPage);

                if (hashInfoOgg == null)
                    hashInfoOgg = new Hashtable();

                switch (this._tipoRicerca)
                {
                    case "R"://......................................Trasmissioni ricevute
                        
                        this.EvidenziaIdRic();
                        dt_Ric.Columns[5].Visible = false;

                        if (this._isHomepage.Equals("Y"))
                        {
                            dt_Ric.Columns[5].Visible = true;

                            if (hashInfoOgg != null && hashInfoOgg.Count > 0)
                            {
                                if (ConfigSettings.getKey(ConfigSettings.KeysENUM.T_DO_SMISTA) != null && ConfigSettings.getKey(ConfigSettings.KeysENUM.T_DO_SMISTA).Equals("1"))
                                {
                                    if (filter[0].valore.Equals("D")) btn_smista.Visible = true;
                                    if (filter[0].valore.Equals("F")) btn_smista.Visible = false;
                                }
                                else
                                    btn_smista.Visible = false;

                                if (this.btn_stampa.Visible)
                                    this.btn_stampa.Attributes.Add("onclick", "StampaRisultatoRicerca();");

                                this.btn_rimuoviTDL.Visible = true;
                                this.btn_rimuoviTDL.Attributes.Add("onclick", "svuotaTDLPage('','','" + Session["Tipo_obj"].ToString() + "','N','');");

                                // gestione dello svuotamento della TDL
                                this.isNoticeActived();
                                if (this._isActivedTDLNotice && (!this._trasmOverNoticeDays.Equals(string.Empty) && !this._trasmOverNoticeDays.Equals("0")))
                                    ClientScript.RegisterStartupScript(this.GetType(), "apreSvuotaTDL", "<script>svuotaTDLPage('" + this._noticeDays + "','" + this._trasmOverNoticeDays + "','" + Session["Tipo_obj"].ToString() + "','Y','" + this._datePost + "');</script>");
                            }
                            else
                            {
                                this.btn_smista.Visible = false;
                                this.btn_rimuoviTDL.Visible = false;
                                this.btn_stampa.Visible = false;
                            }
                        }
                        else
                        {
                            this.btn_smista.Visible = false;
                            this.btn_rimuoviTDL.Visible = false;

                            if (hashInfoOgg != null && hashInfoOgg.Count > 0)
                            {                               
                                if (this.btn_stampa.Visible)
                                    this.btn_stampa.Attributes.Add("onclick", "StampaRisultatoRicerca();");
                            }
                        }
                        
                        break;

                    case "E"://......................................Trasmissioni effettuate
                       
                        this.EvidenziaIdEff();

                        this.btn_smista.Visible = false;
                        this.btn_rimuoviTDL.Visible = false;

                        if (hashInfoOgg != null && hashInfoOgg.Count > 0)
                        {
                            this.pnl_sollecito.Visible = true;

                            if (this.btn_stampa.Visible)
                                this.btn_stampa.Attributes.Add("onclick", "StampaRisultatoRicerca();");
                        }
                        else
                        {
                            this.pnl_dt_Eff.Visible = false;
                            this.pnl_sollecito.Visible = false;
                        }                       
                                                  
                        break;
                }                

                // Impostazione della pagina corrente nel viewstate
                ViewState["currentPage"] = currentPage;
            }
            else
            {
                // Caricamento dalla sessione dell'hashtable contenente
                // gli oggetti InfoDocumenti dell'ultima ricerca
                hashInfoOgg = TrasmManager.getHashTrasmOggTrasm(this);
            }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trasm"></param>
		/// <param name="index"></param>
		/// <param name="tipoRic"></param>
		protected void CaricaDataSet(DocsPAWA.DocsPaWR.Trasmissione trasm,int index,string tipoRic)
		{
			string InfoOggTrasm="";
			string segnData="";
			string id="";
			
			InfoOggTrasm=TipoOggTrasm(trasm);
			segnData=CheckSegnODataOggTrasm(trasm);
			id=CheckCodice(trasm);

			switch(trasm.tipoOggetto)
			{
				case DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO:
				{
					hashInfoOgg.Add(index,trasm.infoFascicolo);	
					break;
				}
				case DocsPAWA.DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO:
				{
					
					hashInfoOgg.Add(index,trasm.infoDocumento);	
					break;
				}
			}
			//nuovo
			TrasmManager.setHashTrasmOggTrasm(this,hashInfoOgg);
            string descrUtente = "";
            if (string.IsNullOrEmpty(trasm.delegato))
                descrUtente = trasm.utente.descrizione;
            else
                descrUtente = trasm.delegato + "<br>Delegato da " + trasm.utente.descrizione;

            if (tipoRic.Equals("E"))
            {
                string dataInvio = trasm.dataInvio;
                string dataScadenza = trasm.trasmissioniSingole[0].dataScadenza;

                string destinatariTrasm = "";
                int i = 0;
                string mittDoc = "";

                if (trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
                {
                    if (((DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm)(trasm)).infoDocumento != null)
                    {
                        if (string.IsNullOrEmpty(InfoOggTrasm) || InfoOggTrasm.Equals("<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "</b>"))
                        {
                            mittDoc = "";
                        }
                        else
                        {
                            mittDoc = ((DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm)(trasm)).infoDocumento.mittDoc;
                        }
                    }
                }

                foreach (DocsPAWA.DocsPaWR.TrasmissioneSingola trasmS in trasm.trasmissioniSingole)
                {

                    destinatariTrasm += trasmS.corrispondenteInterno.descrizione;
                    if (i < trasm.trasmissioniSingole.Length - 1)
                    {
                        destinatariTrasm += " - ";
                    }
                    i++;


                }
                //se abilitato il campo repertorio 
                if (utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "VIS_SEGNATURA_REPERTORI").Equals("1"))
                {
                    if (trasm != null && trasm.infoDocumento != null && (!string.IsNullOrEmpty(trasm.infoDocumento.docNumber)))
                    {
                        string Repertorio = DocumentManager.getSegnaturaRepertorio(this.Page, trasm.infoDocumento.docNumber, UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice);
                        this.dataSetTrasmEff2.element1.Addelement1Row(dataInvio, descrUtente, trasm.ruolo.descrizione, dataScadenza, index, InfoOggTrasm, segnData, id, destinatariTrasm, mittDoc, Repertorio);
                    }
                    else 
                    {
                        this.dataSetTrasmEff2.element1.Addelement1Row(dataInvio, descrUtente, trasm.ruolo.descrizione, dataScadenza, index, InfoOggTrasm, segnData, id, destinatariTrasm, mittDoc, string.Empty);
                    }
                }
                else
                {
                    this.dataSetTrasmEff2.element1.Addelement1Row(dataInvio, descrUtente, trasm.ruolo.descrizione, dataScadenza, index, InfoOggTrasm, segnData, id, destinatariTrasm, mittDoc, string.Empty);

                }
            }
            else
            {
                DocsPaWR.TrasmissioneSingola trasmSing = new DocsPAWA.DocsPaWR.TrasmissioneSingola();
                DocsPaWR.TrasmissioneUtente trasmUtente = new DocsPAWA.DocsPaWR.TrasmissioneUtente();
                DocsPaWR.TrasmissioneSingola[] listaTrasmSing;

                listaTrasmSing = trasm.trasmissioniSingole;
                string ragione = "";
                string dataScad = "";
                if (listaTrasmSing.Length > 0)
                {
                    trasmSing = (DocsPAWA.DocsPaWR.TrasmissioneSingola)listaTrasmSing[0];
                    if (trasmSing.ragione != null)
                        ragione = trasmSing.ragione.descrizione;
                    else
                        ragione = "";
                    dataScad = trasmSing.dataScadenza;
                }

                string dataInvio = trasm.dataInvio;
                string dataScadenza = dataScad;
                string mittDoc = "";
                if (trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
                {
                    if (((DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm)(trasm)).infoDocumento != null)
                    {
                        if (InfoOggTrasm == null || InfoOggTrasm.Equals("<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "</b>"))
                        {
                            mittDoc = "";
                        }
                        else
                        {
                            mittDoc = ((DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm)(trasm)).infoDocumento.mittDoc;
                        }
                    }
                }
                //se abilitato il campo repertorio 
                if (utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "VIS_SEGNATURA_REPERTORI").Equals("1"))
                {
                    if (trasm != null && trasm.infoDocumento != null && (!string.IsNullOrEmpty(trasm.infoDocumento.docNumber)))
                    {
                        string Repertorio = DocumentManager.getSegnaturaRepertorio(this.Page, trasm.infoDocumento.docNumber, UserManager.getInfoAmmCorrente(UserManager.getInfoUtente(this).idAmministrazione).Codice);
                        this.dataSetTrasmRic2.element1.Addelement1Row(dataInvio, descrUtente, trasm.ruolo.descrizione, ragione, dataScadenza, index, InfoOggTrasm, segnData, id, trasm.noteGenerali, mittDoc, Repertorio);
                    }
                    else
                    {
                        this.dataSetTrasmRic2.element1.Addelement1Row(dataInvio, descrUtente, trasm.ruolo.descrizione, ragione, dataScadenza, index, InfoOggTrasm, segnData, id, trasm.noteGenerali, mittDoc, string.Empty);
                    }
                }
                else
                {
                    this.dataSetTrasmRic2.element1.Addelement1Row(dataInvio, descrUtente, trasm.ruolo.descrizione, ragione, dataScadenza, index, InfoOggTrasm, segnData, id, trasm.noteGenerali, mittDoc, string.Empty);
                }
            }
		}
		#endregion
		
		#region gestione Sollecito alle trasmissioni effettuate

        //private void btn_SetTxUtAsViste_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        //{
        //    string val = "";
        //    val = this.hd1.Value;
        //    if(val!=null && !val.Equals("")	&& val.Equals("1"))
        //    {
        //        TrasmManager.setTxUtAsViste(this);
        //        // così rifa la query è la lista si autoaggiorna dopo l'eliminazione
        //        TrasmManager.removeDocTrasmQueryRic(this);
        //        TrasmManager.removeDataTableRic(this);
        //        TrasmManager.removeHashTrasmOggTrasm(this);
				
        //        Response.Write("<script>window.top.principale.iFrame_dx.document.location = 'tabRisultatiRicTrasm.aspx?tiporic=R&home=Y';</script>");		
        //    }
        //}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sollecita_tutti_Click(object sender, System.EventArgs e)
		{					
			sendSollecito(true);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sollecita_sel_Click(object sender, System.EventArgs e)
		{
			sendSollecito(false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabRisultatiRicTrasm_PreRender(object sender, System.EventArgs e)
		{
            this.SetControlsVisibility();

			//abilitazione delle funzioni in base al ruolo
			UserManager.disabilitaFunzNonAutorizzate(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="chiave"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		public string GetCheckBoxLabel(int chiave, string flag)
		{
			//string lbl = "<input type='checkbox' class='testo_grigio'  name='chk' value='' >";
			string lbl = "";
			try 
			{
				lbl="<input type='checkbox' class='testo_grigio' id='chk" + chiave.ToString() + "' name='chk" + chiave.ToString() + "' value='" + chiave.ToString() + "' " + flag + ">";
				return lbl;
			}
			catch(Exception ex)
			
			{
				ErrorManager.redirectToErrorPage(this,ex);
				return lbl;
			}							
		}

		/// <summary>
		/// ATTENZIONE! NON UTILIZZATA !!!!!!!!!!
		/// </summary>
		/// <param name="trasm"></param>
		/// <returns></returns>
		protected bool controllasedainviare(DocsPAWA.DocsPaWR.Trasmissione trasm)
		{			
			System.DateTime	dt=Utils.formatStringToDate(trasm.dataInvio);
			
			string data=System.DateTime.Now.ToString("dd/MM/yyyy");
			System.DateTime	dt2=Utils.formatStringToDate(data);
						
			return true;
		}

		/// <summary>
		/// Invia un sollecito delle trasmissioni effettuate
		/// </summary>
		/// <param name="tutte">bool: se "true" = tutte le trasmissioni / se "false" = solo quelle selezionate</param>
		protected void sendSollecito(bool tutte)
		{
			bool passato = false;
			int indiceEffettivoChk;
			
			try
			{
				DocsPaWR.Trasmissione[] trasmlist=TrasmManager.getDocTrasmQueryEff(this);

				if(tutte)
				{					
					for(int i=0;i<trasmlist.Length;i++)
					{						
						UserManager.sendSollecito(trasmlist[i],this);
						passato = true;
					}
				}
				else
				{
					if(this.dt_Eff!=null && this.dt_Eff.Items!=null && this.dt_Eff.Items.Count>0)
					{
						chk=new ArrayList();						

						for(int i=0;i<this.dt_Eff.Items.Count;i++)
						{		
							indiceEffettivoChk=i;

							//							// gestione pagind del datagrid (8 record per pagina)
							//							if(ViewState["currentPage"] == null || (int)ViewState["currentPage"] == 1) // se ci troviamo alla prima pagina, l'indice della checkbox è lo stesso del contatore del ciclo for...
							//							{
							//								indiceEffettivoChk = i; 
							//							}
							//							else // altrimenti, dalla seconda pagina in poi, l'indice della checkbox è il num. di righe della pagina precedente più il contatore del ciclo for...
							//							{
							//								indiceEffettivoChk = ((((int)ViewState["currentPage"] - 1) * 8) + i);
							//							}

							indexchk = "chk" + indiceEffettivoChk.ToString();
							// fine gestione pagind del datagrid

							//indexchk = "chk" + i.ToString();
							if(Request.Form[indexchk] != null)
							{					
								UserManager.sendSollecito(trasmlist[i],this);	
								passato = true;
							}
						}
					}
				}				
				
				if(passato)
					Response.Write("<script>window.alert('Solleciti inviati con successo!')</script>");				
			}
			catch(Exception ex)
			{
				ErrorManager.redirectToErrorPage(this,ex);
			}
		}
		#endregion

		#region utils
		/// <summary>
		/// 
		/// </summary>
		/// <param name="msg"></param>
		protected void PrintMsg(string msg)
		{
            this.titolo.Text = msg;
		}

        ///// <summary>
        ///// 
        ///// </summary>
        //private void RefreshCountTrasmissioni()
        //{
        //    string msg = "Elenco trasmissioni";
            
        //    int countTrasmissioni=0;
        //    bool searched=false;

        //    if (this._tipoRicerca.Equals("E"))
        //    {
        //        searched = (this.listaTrasmEff != null);
        //        countTrasmissioni = this.dt_Eff.VirtualItemCount;
        //    }
        //    else
        //    {
        //        searched = (this.listaTrasmRic != null);
        //        countTrasmissioni = this.dt_Ric.VirtualItemCount;
        //    }

        //    if (searched)
        //        msg += " - Trovati " + countTrasmissioni.ToString() + " elementi.";
            
        //    this.PrintMsg(msg);
        //}

		#endregion		

		#region Sorting
		/// <summary>
		/// 
		/// </summary>
		private string DirectionSorting
		{
			get
			{
				string retValue;
				if (ViewState["directionSorting"]==null)
				{
					ViewState["directionSorting"]="ASC";
				}

				retValue=(string)ViewState["directionSorting"];
				return retValue;
			}
			set
			{
				ViewState["directionSorting"]=value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oldDirection"></param>
		private void ChangeDirectionSorting(string oldDirection)
		{
			string newValue;
			if (oldDirection!=null && oldDirection.Equals("ASC"))
			{
				newValue="DESC";
			}
			else
			{
				newValue="ASC";
			}
			DirectionSorting=newValue;
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
		
		#region InitializeComponent() commentato per backup
		//		private void InitializeComponent()
		//		{    
		//			this.dataSetTrasmEff2 = new DocsPAWA.dataSet.DataSetTrasmEff();
		//			this.dataSetTrasmRic2 = new DocsPAWA.dataSet.DataSetTrasmRic();
		//			((System.ComponentModel.ISupportInitialize)(this.dataSetTrasmEff2)).BeginInit();
		//			((System.ComponentModel.ISupportInitialize)(this.dataSetTrasmRic2)).BeginInit();
		//			this.btn_SetTxUtAsViste.Click += new System.Web.UI.ImageClickEventHandler(this.btn_SetTxUtAsViste_Click);
		//			this.btn_smista.Click += new System.Web.UI.ImageClickEventHandler(this.btn_smista_Click);
		//			this.dt_Eff.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ShowInfo);
		//			this.dt_Eff.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dt_Eff_PageIndexChanged);
		//			this.dt_Eff.PreRender += new System.EventHandler(this.dt_Eff_PreRender);
		//			this.dt_Eff.SelectedIndexChanged += new System.EventHandler(this.dt_Eff_SelectedIndexChanged);
		//			this.dt_Ric.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ShowInfo);
		//			this.dt_Ric.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dt_Ric_PageIndexChanged);
		//			this.dt_Ric.PreRender += new System.EventHandler(this.dt_Ric_PreRender);
		//			this.dt_Ric.SelectedIndexChanged += new System.EventHandler(this.dt_Ric_SelectedIndexChanged);
		//			this.sollecita_tutti.Click += new System.EventHandler(this.sollecita_tutti_Click);
		//			this.sollecita_sel.Click += new System.EventHandler(this.sollecita_sel_Click);
		//			// 
		//			// dataSetTrasmEff2
		//			// 
		//			this.dataSetTrasmEff2.DataSetName = "DataSetTrasmEff";
		//			this.dataSetTrasmEff2.Locale = new System.Globalization.CultureInfo("en-US");
		//			// 
		//			// dataSetTrasmRic2
		//			// 
		//			this.dataSetTrasmRic2.DataSetName = "DataSetTrasmRic";
		//			this.dataSetTrasmRic2.Locale = new System.Globalization.CultureInfo("en-US");
		//			this.Load += new System.EventHandler(this.Page_Load);
		//			this.PreRender += new System.EventHandler(this.tabRisultatiRicTrasm_PreRender);
		//			((System.ComponentModel.ISupportInitialize)(this.dataSetTrasmEff2)).EndInit();
		//			((System.ComponentModel.ISupportInitialize)(this.dataSetTrasmRic2)).EndInit();
		//
		//		}
		#endregion

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.dataSetTrasmEff2 = new DocsPAWA.dataSet.DataSetTrasmEff();
			this.dataSetTrasmRic2 = new DocsPAWA.dataSet.DataSetTrasmRic();
			((System.ComponentModel.ISupportInitialize)(this.dataSetTrasmEff2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataSetTrasmRic2)).BeginInit();
			this.dt_Eff.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ShowInfo);
			this.dt_Eff.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dt_Eff_PageIndexChanged);
			this.dt_Eff.PreRender += new System.EventHandler(this.dt_Eff_PreRender);
			this.dt_Eff.SelectedIndexChanged += new System.EventHandler(this.dt_Eff_SelectedIndexChanged);
			this.dt_Ric.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ShowInfo);
			this.dt_Ric.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dt_Ric_PageIndexChanged);
			this.dt_Ric.PreRender += new System.EventHandler(this.dt_Ric_PreRender);
			this.dt_Ric.SelectedIndexChanged += new System.EventHandler(this.dt_Ric_SelectedIndexChanged);
			this.sollecita_tutti.Click += new System.EventHandler(this.sollecita_tutti_Click);
			this.sollecita_sel.Click += new System.EventHandler(this.sollecita_sel_Click);
            this.btn_smista.Click += new ImageClickEventHandler(this.btn_smista_Click);
			// 
			// dataSetTrasmEff2
			// 
			this.dataSetTrasmEff2.DataSetName = "DataSetTrasmEff";
			this.dataSetTrasmEff2.Locale = new System.Globalization.CultureInfo("en-US");
			// 
			// dataSetTrasmRic2
			// 
			this.dataSetTrasmRic2.DataSetName = "DataSetTrasmRic";
			this.dataSetTrasmRic2.Locale = new System.Globalization.CultureInfo("en-US");
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.tabRisultatiRicTrasm_PreRender);
			((System.ComponentModel.ISupportInitialize)(this.dataSetTrasmEff2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataSetTrasmRic2)).EndInit();

		}
		#endregion

		#region trasmissioni EFFETTUATE
		/// <summary>
		/// 
		/// </summary>
		private void EvidenziaIdEff()
		{
            // solo per i documenti. NO per i fascicoli!
            if (dt_Eff.Columns[6].HeaderText.Equals("Oggetto<br>&#160-------<br>&#160;Mittente"))
			{
				dt_Eff.Columns[5].ItemStyle.Font.Bold = true;
				for (int i=0; i<this.dt_Eff.Items.Count; i++)
				{
					ImageButton  imgB = (ImageButton)dt_Eff.Items[i].FindControl("Imagebutton4");
					string prova = imgB.AlternateText;
					if (!imgB.AlternateText.StartsWith("Id"))
					{
						//è un protocollo
						dt_Eff.Items[i].Cells[5].ForeColor = Color.FromArgb(255,0,0);						
					}
					else
					{
						//è un doc grigio
						dt_Eff.Items[i].Cells[5].ForeColor = Color.FromArgb(130,130,130);	
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void dt_Eff_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			dv_Eff=((DataTable)TrasmManager.getDataTableEff(this)).DefaultView;	
			if(e.SortExpression.Equals("Utente"))
			{
				dv_Eff.Sort = e.SortExpression+" ASC";
			}
			if(e.SortExpression.Equals("InfoOggTrasm"))
			{
				dv_Eff.Sort=e.SortExpression+" ASC";
			}
			
			dt_Eff.DataSource=dv_Eff;
			dt_Eff.DataBind();
			EvidenziaIdEff();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void dt_Eff_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{		
			this.dt_Eff.SelectedIndex=-1;
			this.dt_Eff.CurrentPageIndex=e.NewPageIndex;

			int currentPage=e.NewPageIndex + 1;

			this.LoadTrasmissioni("E",
				this.filter,
				currentPage);

			EvidenziaIdEff();

			ViewState["currentPage"] = currentPage;

			this.SetCurrentPageOnContext(currentPage);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dt_Eff_PreRender(object sender, System.EventArgs e)
		{			
			DataGrid dg=((DataGrid) sender);

			this.SetDocumentIndexFromQueryString(dg);

			for(int i=0;i<dg.Items.Count;i++)
			{
				if(dg.Items[i].ItemIndex>=0)
				{	
					//gestione sollecito: checkbox sul datagrid delle trasmissioni effettuate
					if(chk!=null && chk.Count>0)
					{
						if(chk.Contains(((Label)this.dt_Eff.Items[i].Cells[8].Controls[1]).Text))
						{
							if(this.dt_Eff.Items[i].Cells[0].Controls[1].GetType().Equals(typeof(Label)))
							{
								Label lbc=((Label)this.dt_Eff.Items[i].Cells[0].Controls[1]);
								lbc.Text=this.GetCheckBoxLabel(Int32.Parse(((Label)this.dt_Eff.Items[i].Cells[0].Controls[1]).Text),"checked");
							}
						}						
					}
				}				
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dt_Eff_SelectedIndexChanged(object sender, System.EventArgs e)
		{			
			Label lbl=((Label)this.dt_Eff.Items[dt_Eff.SelectedIndex].Cells[0].Controls[1]);
			int key =Int32.Parse(lbl.Text);

			this.SetCurrentDocIndexOnContext(key);

			DocsPaWR.Trasmissione[] trasmlist=TrasmManager.getDocTrasmQueryEff(this);
			TrasmManager.setDocTrasmSel(this,trasmlist[key]);
            string errorMessage = string.Empty;
            //old if(Session["Tipo_obj"].Equals("D"))
            if (isRicercaPerDocumento(this.filter))
			{
                if (trasmlist[key] != null && trasmlist[key].infoDocumento != null
                    && trasmlist[key].infoDocumento.idProfile != null)
                {
                    
                    int result = DocumentManager.verificaACL("D", trasmlist[key].infoDocumento.idProfile, UserManager.getInfoUtente(), out errorMessage);
                 //   if (result == 0 || result == 1)
                 //   {
                 //       ClientScript.RegisterStartupScript(this.GetType(), "errore", "<script language=javascript>alert('" + errorMessage + "');</script>");

                 //   }
                 //   else
                        ClientScript.RegisterStartupScript(this.GetType(), "dettaglioTrasm", "<script>dettaglioTrasm('../documento/tabTrasmissioniEff.aspx');</script>");
		
                    
                }
                else
                {
                    errorMessage = "Con il ruolo corrente non è possibile visualizzare\\nil dettaglio della trasmissione selezionata.";
                    ClientScript.RegisterStartupScript(this.GetType(), "errore", "<script language=javascript>alert('" + errorMessage + "');</script>");

                }          
          	}
			else
			{
                if (trasmlist[key] != null && trasmlist[key].infoFascicolo != null
                    && trasmlist[key].infoFascicolo.idFascicolo != null)
                {

                    int result = DocumentManager.verificaACL("F", trasmlist[key].infoFascicolo.idFascicolo, UserManager.getInfoUtente(), out errorMessage);
                    if (result == 0 || result == 1)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "errore", "<script language=javascript>alert('" + errorMessage + "');</script>");

                    }
                    else
                        ClientScript.RegisterStartupScript(this.GetType(), "dettaglioTrasm", "<script>dettaglioTrasm('../fascicolo/tabTrasmissioniEffFasc.aspx?chiudi=si');</script>");
			

                }
                else 
                {
                    //this.iFrame_cn.NavigateTo="../fascicolo/tabTrasmissioniEffFasc.aspx";
                    errorMessage = "Con il ruolo corrente non è possibile visualizzare\\nil dettaglio della trasmissione selezionata.";
                    ClientScript.RegisterStartupScript(this.GetType(), "errore", "<script language=javascript>alert('" + errorMessage + "');</script>");

                }          

			}
        }

        protected void Grid_OnItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Pager)
            {
                if (e.Item.Cells.Count > 0)
                {
                    e.Item.Cells[0].Attributes.Add("colspan", e.Item.Cells[0].ColumnSpan.ToString());
                }
            }
        }

		#endregion

		#region trasmissioni RICEVUTE
		/// <summary>
		/// 
		/// </summary>
		private void EvidenziaIdRic()
		{
            // solo per i documenti. No per i fascicoli!
            if (dt_Ric.Columns[8].HeaderText.Equals("Oggetto<br>&#160-------<br>&#160;Mittente"))
			{
				dt_Ric.Columns[7].ItemStyle.Font.Bold = true;
				for (int i=0; i<this.dt_Ric.Items.Count; i++)
				{
					ImageButton  imgB = (ImageButton)dt_Ric.Items[i].FindControl("ImageButton3");
					string prova = imgB.AlternateText;
					if (!imgB.AlternateText.StartsWith("Id"))
					{
						//è un protocollo
						dt_Ric.Items[i].Cells[7].ForeColor = Color.FromArgb(255,0,0);						
					}
					else
					{						
						//è un doc grigio
						dt_Ric.Items[i].Cells[7].ForeColor = Color.FromArgb(130,130,130);	
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataGrid"></param>
		private void SetDocumentIndexFromQueryString(DataGrid dataGrid)
		{
			if (!this.IsPostBack)
			{
				string param=this.Request.QueryString["docIndex"];

				if (param!=null && param!=string.Empty)
				{
					int documentIndex=-1;
					try
					{
						documentIndex=Int32.Parse(param);
					}
					catch
					{
					}
				
					dataGrid.SelectedIndex=documentIndex;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dt_Ric_PreRender(object sender, System.EventArgs e)
		{
			this.SetDocumentIndexFromQueryString((DataGrid) sender);
		}

        /// <summary>
        /// Impostazione visibilità controlli
        /// </summary>
        private void SetControlsVisibility()
        {
            DataGrid dataGrid = null;

            if (this._tipoRicerca == "R")
            {
                dataGrid = this.dt_Ric;
            }
            else
            {
                dataGrid = this.dt_Eff;
                dataGrid.Columns[8].Visible = true;
            }            
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void dt_Ric_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			dv_Ric=((DataTable)TrasmManager.getDataTableRic(this)).DefaultView;	
			
			if(e.SortExpression.Equals("Utente"))
			{
				dv_Ric.Sort = e.SortExpression+" ASC";
				
			}
			if(e.SortExpression.Equals("InfoOggTrasm"))
			{				
				dv_Ric.Sort=e.SortExpression+" ASC";
			}
			
			dt_Ric.DataSource=dv_Ric;
			dt_Ric.DataBind();
			EvidenziaIdRic();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void dt_Ric_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.dt_Ric.SelectedIndex=-1;
			this.dt_Ric.CurrentPageIndex=e.NewPageIndex;

			int currentPage=e.NewPageIndex + 1;

			this.LoadTrasmissioni("R",
				this.filter,
				currentPage);

			EvidenziaIdRic();

			ViewState["currentPage"]=currentPage;

			this.SetCurrentPageOnContext(currentPage);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dt_Ric_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Label lbl=((Label)this.dt_Ric.Items[dt_Ric.SelectedIndex].Cells[0].Controls[1]);
			int key =Int32.Parse(lbl.Text);

			this.SetCurrentDocIndexOnContext(key);

			DocsPaWR.Trasmissione[] trasmlist=TrasmManager.getDocTrasmQueryRic(this);
			bool EsitoVerifica=false;
            int result=-1;
            string errorMessage = string.Empty;
            //old if(Session["Tipo_obj"].Equals("D"))
            if (isRicercaPerDocumento(this.filter) && trasmlist[key] != null && trasmlist[key].infoDocumento != null
                && trasmlist[key].infoDocumento.idProfile != null
                && trasmlist[key].infoDocumento.idProfile != "")
            {
                result = DocumentManager.verificaACL("D", trasmlist[key].infoDocumento.idProfile, UserManager.getInfoUtente(), out errorMessage);
            }
            else
                if (trasmlist[key] != null && trasmlist[key].infoFascicolo != null
                && trasmlist[key].infoFascicolo.idFascicolo != null
                && trasmlist[key].infoFascicolo.idFascicolo != "")
                {
                    result = DocumentManager.verificaACL("F", trasmlist[key].infoFascicolo.idFascicolo, UserManager.getInfoUtente(), out errorMessage);
                }

            if (errorMessage=="" )
            {
                //old if(Session["Tipo_obj"].Equals("D"))
                if (isRicercaPerDocumento(this.filter))
                {
                    if ((trasmlist[key].infoDocumento != null &&
                        trasmlist[key].infoDocumento.numProt != null
                        && trasmlist[key].infoDocumento.numProt != "") //PROTOCOLLATI SU UN CERTO REGISTRO
                        || (trasmlist[key].infoDocumento != null &&
                         !(trasmlist[key].infoDocumento.numProt != null
                        && trasmlist[key].infoDocumento.numProt != "") &&
                        trasmlist[key].infoDocumento.daProtocollare == "1") //PREDISPOSTI SU UN CERTO REGISTRO
                        )
                    {
                        //verifica se il ruolo selezionato ha la visibilità sul registro del documento 
                        EsitoVerifica = VerificaAutorizzazioneSuRegistro(trasmlist[key].infoDocumento.idRegistro);
                    }
                    else
                    {
                        //se il documento è grigio non occorre effettuare verifiche sui registri.
                        EsitoVerifica = true;
                    }
                }
                else
                {
                    EsitoVerifica = true;
                }
            }

            if ((trasmlist[key].tipoOggetto.ToString().Equals("DOCUMENTO") &&
                trasmlist[key].infoDocumento == null) ||
                    (trasmlist[key].tipoOggetto.ToString().
                    Equals("FASCICOLO") && string.IsNullOrEmpty(
                    trasmlist[key].infoFascicolo.idFascicolo)))
                //ELIMINATO CONTROLLO VISIBILITA' DETTAGLIO TRASMISSIONI PORCHERIA AGGIUNTO ANCHE ELSE SOTTO :)
                //EsitoVerifica = false;
                EsitoVerifica = true;
                else
                {
                    EsitoVerifica = true;
                    errorMessage = "";
                    if (result == 0)
                    {

                    }
                }

            if (EsitoVerifica && errorMessage=="")
            {
                TrasmManager.setDocTrasmSel(this, trasmlist[key]);
                //old if(Session["Tipo_obj"].Equals("D"))
                if (isRicercaPerDocumento(this.filter))
                {
                    string provenienza = Request.QueryString["home"];
                    if (provenienza.Equals("Y"))
                    {
                        Session.Add("ProvenienzaHome", "home");
                    }
                    else if (provenienza.Equals("N"))
                    {
                        Session.Add("ProvenienzaHome", "ricTrasm");
                    }

                    //this.iFrame_cn.NavigateTo="../documento/tabTrasmissioniRic.aspx";					                    
                    ClientScript.RegisterStartupScript(this.GetType(), "dettaglioTrasm", "<script>dettaglioTrasm('../documento/tabTrasmissioniRic.aspx?nomeForm=RicTrasm');</script>");
                }
                else 
                {
                    string provenienza = Request.QueryString["home"];
                    if (provenienza.Equals("Y"))
                    {
                        Session.Add("ProvenienzaHome", "home");
                    }
                    else if (provenienza.Equals("N"))
                    {
                        Session.Add("ProvenienzaHome", "ricTrasm");
                    }
                    //this.iFrame_cn.NavigateTo="../fascicolo/tabTrasmissioniRicFasc.aspx";
                    ClientScript.RegisterStartupScript(this.GetType(), "dettaglioTrasm", "<script>dettaglioTrasm('../fascicolo/tabTrasmissioniRicFasc.aspx?nomeForm=RicTrasm');</script>");
                }
            }
            else
            {
                if (errorMessage != "")
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "errore", "<script language=javascript>alert('" + errorMessage + "');</script>");
                }
                else if(!EsitoVerifica)
                {
                    errorMessage = "Con il ruolo corrente non è possibile visualizzare\\nil dettaglio della trasmissione selezionata.";
                    ClientScript.RegisterStartupScript(this.GetType(), "errore", "<script language=javascript>alert('" + errorMessage + "');</script>");
                }
               
     
            }

		}
		#endregion

		#region TRASMISSIONI
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tipoTrasm"></param>
		/// <param name="filt"></param>
		protected void queryTrasmissioni(string tipoTrasm,DocsPaWR.FiltroRicerca[] filt)
		{
			try
			{			
				DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();
				
				//chiamo web services get trasm effettuate:
			
				if (tipoTrasm.Equals("E"))
				{
					int pageCount,recordCount;
					listaTrasmEff=TrasmManager.getQueryEffettuatePaging(this,oggettoTrasm, this.userHome,this.userRuolo,filt,this.dt_Eff.CurrentPageIndex+1,out pageCount,out recordCount);
					this.dt_Eff.VirtualItemCount=recordCount;


					//listaTrasmEff =  TrasmManager.getQueryEffettuate(this,oggettoTrasm, this.userHome,this.userRuolo,filt);
					if(listaTrasmEff!=null && listaTrasmEff.Length>0)
					{
						TrasmManager.setDocTrasmQueryEff(this,listaTrasmEff);
						if(isRicercaPerDocumento(filt)) // l'export deve essere possibile solo con le ricerche per documento
						{
							this.btn_stampa.Visible=true;
						}
					}
					else
					{
						this.btn_stampa.Visible=false;
						this.pnl_sollecito.Visible=false;
						return;
					}
				}
				else
					if (tipoTrasm.Equals("R"))
				{
					//listaTrasmRic = TrasmManager.getQueryRicevute(this,oggettoTrasm, this.userHome,this.userRuolo,filt);

					int pageCount,recordCount;
					listaTrasmRic = TrasmManager.getQueryRicevutePaging(this,oggettoTrasm, this.userHome,this.userRuolo,filt,this.dt_Ric.CurrentPageIndex + 1,out pageCount,out recordCount);
					this.dt_Ric.VirtualItemCount=recordCount;

					if(listaTrasmRic!=null && listaTrasmRic.Length>0)
					{
						TrasmManager.setDocTrasmQueryRic(this,listaTrasmRic);
						if(isRicercaPerDocumento(filt)) // l'export deve essere possibile solo con le ricerche per documento
						{
							this.btn_stampa.Visible=true;
						}
					}
					else
					{
						this.btn_stampa.Visible=false;
						return;
					}
				}
			}		
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}		


		private void LoadTrasmissioni(string tipoTrasm,
			DocsPaWR.FiltroRicerca[] filt,
			int requestedPageNumber)
		{
			int totalPageCount,totalRecordCount;

			try
			{			
				DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPAWA.DocsPaWR.TrasmissioneOggettoTrasm();
				
				DocsPaWR.Trasmissione[] listaTrasmissioni=null;

				bool trasmEffettuate=(tipoTrasm=="E");

				if (trasmEffettuate)
				{
					// Trasmissioni effettuate
					//listaTrasmissioni=TrasmManager.getQueryEffettuatePaging(this,oggettoTrasm, this.userHome,this.userRuolo,filt,requestedPageNumber,out totalPageCount,out totalRecordCount);
                    listaTrasmissioni = TrasmManager.getQueryEffettuatePagingLite(this, oggettoTrasm, this.userHome, this.userRuolo, filt, requestedPageNumber,false,this.pageSize, out totalPageCount, out totalRecordCount);

					this.dt_Eff.CurrentPageIndex=requestedPageNumber - 1;
					this.dt_Eff.VirtualItemCount=totalRecordCount;
				}
				else
				{
					// Trasmissioni ricevute
					//listaTrasmissioni = TrasmManager.getQueryRicevutePaging(this,oggettoTrasm, this.userHome,this.userRuolo,filt,requestedPageNumber,out totalPageCount,out totalRecordCount);
                    listaTrasmissioni = TrasmManager.getQueryRicevuteLite(this, oggettoTrasm, this.userHome, this.userRuolo, filt, requestedPageNumber, false,this.pageSize, out totalPageCount, out totalRecordCount);

					this.dt_Ric.CurrentPageIndex=requestedPageNumber - 1;
					this.dt_Ric.VirtualItemCount=totalRecordCount;
				}	
				
				/* ABBATANGELI GIANLUIGI
                 * se viene restituito totalPageCount = -2 vuol dire che ho raggiunto 
                 * il numero massimo di riche accettate in risposta ad una ricerca */
                bool outOfMaxRowSearchable = (totalPageCount == -2);
                if (!outOfMaxRowSearchable)
                    {
				    if(listaTrasmissioni!=null && listaTrasmissioni.Length>0)
				    {
                        if (isRicercaPerDocumento(filt)) // l'export deve essere possibile solo con le ricerche per documento
                        {
                            this.btn_stampa.Visible = true;
                        }
                        else
                        {
                            this.btn_stampa.Visible = false;
                        }

					    if (hashInfoOgg==null)
					    {
						    hashInfoOgg=new Hashtable();
						    TrasmManager.setHashTrasmOggTrasm(this,hashInfoOgg);
					    }
					    else
					    {
						    hashInfoOgg.Clear();
					    }

					    for (int i=0;i<listaTrasmissioni.Length;i++)
					    {
						    DocsPaWR.Trasmissione trasm=listaTrasmissioni[i];
						
						    CaricaDataSet(trasm,i,tipoTrasm);
						    if(!trasmEffettuate && (trasm.tipoOggetto==DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO))
						    {
							    this.dt_Ric.Columns[8].Visible=false;
							    this.dt_Ric.Columns[9].Visible=true;
							
						    }
                            if (trasmEffettuate && (trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO))
                            {
                                this.dt_Eff.Columns[6].Visible = false;
                                this.dt_Eff.Columns[7].Visible = true;

                            }
						    trasm=null;
					    }

					    if (trasmEffettuate)
					    {
						    TrasmManager.setDocTrasmQueryEff(this,listaTrasmissioni);

						    this.dt_Eff.DataSource=this.dataSetTrasmEff2;
						    this.dt_Eff.DataBind();
						    TrasmManager.setDataTableEff(this,this.dataSetTrasmEff2.Tables[0]);

						    if (((DocsPAWA.DocsPaWR.Trasmissione) listaTrasmissioni[0]).tipoOggetto==DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO)
						    {
							    this.dt_Eff.Columns[6].HeaderText = "Cod.";
							    this.dt_Eff.Columns[7].HeaderText = "Descrizione";
						    }
						    else
						    {
							    this.dt_Eff.Columns[5].HeaderText = "Doc.";
                                this.dt_Eff.Columns[6].HeaderText = "Oggetto<br>&#160-------<br>&#160;Mittente";
						    }
					    }
					    else
					    {
						    TrasmManager.setDocTrasmQueryRic(this,listaTrasmissioni);

						    this.dt_Ric.DataSource=this.dataSetTrasmRic2;
						    this.dt_Ric.DataBind();
						    TrasmManager.setDataTableRic(this,this.dataSetTrasmRic2.Tables[0]);

						    if (((DocsPAWA.DocsPaWR.Trasmissione)listaTrasmissioni[0]).tipoOggetto==DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO)
						    {
							    this.dt_Ric.Columns[8].HeaderText = "Cod.";
							    this.dt_Ric.Columns[9].HeaderText = "Descrizione";
						    }
						    else
						    {
							    this.dt_Ric.Columns[7].HeaderText = "Doc.";
                                this.dt_Ric.Columns[8].HeaderText = "Oggetto<br>&#160-------<br>&#160;Mittente";
						    }
					    }

					    listaTrasmissioni=null;
				    }
				    else
				    {
					    this.btn_stampa.Visible=false;

					    if (trasmEffettuate)
						    this.pnl_sollecito.Visible=false;
				    }

                    this.SetMaskTitle(totalRecordCount.ToString());
                }
                else
                {
                    utils.AlertPostLoad.OutOfMaxRowSearchable(Page, totalRecordCount);




                    this.btn_stampa.Visible = false;

                    if (trasmEffettuate)
                        this.pnl_sollecito.Visible = false;


                     this.SetMaskTitle("0");
                }
            }
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}			
		}

		private void SetMaskTitle(string recordCountTrasmissioni)
		{
			this.titolo.Text="Elenco trasmissioni - Trovati " + recordCountTrasmissioni + " elementi."; 
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string GetUrl(int id)
		{
			string url=" ";
			if(hashInfoOgg[id]!=null)
			{
				if(hashInfoOgg[id].GetType().Equals(typeof(DocsPAWA.DocsPaWR.InfoFascicolo)))
				{
					url="../fascicolo/gestioneFasc.aspx?tab=documenti";
				}
				else
					if(((DocsPAWA.DocsPaWR.InfoDocumento)hashInfoOgg[id]).numProt!=null && !(((DocsPAWA.DocsPaWR.InfoDocumento)hashInfoOgg[id]).numProt.Equals("")))
				{
					url="../documento/gestioneDoc.aspx?tab=protocollo";
				}
				else
				{
					url="../documento/gestioneDoc.aspx?tab=profilo";
				}
				return url;
			}
			else
			{
				return url;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="txt"></param>
		/// <returns></returns>
		public string getOggetto(string txt)
		{
			string appo="";
			if (txt!=null)
				if(!txt.Equals(""))
					if(txt.Length>=10)
						appo=txt.Substring(0,10)+"...";
			return appo;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trasm"></param>
		/// <returns></returns>
		private string CheckSegnODataOggTrasm(DocsPAWA.DocsPaWR.Trasmissione trasm)
		{
			string segnTrasm=null;
			if(trasm.tipoOggetto==DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
			{
				if(trasm.infoDocumento!=null)
				{
					//è un documento
					if(!string.IsNullOrEmpty(trasm.infoDocumento.numProt))
					{
						segnTrasm=trasm.infoDocumento.segnatura;
					}
					else
					{
						//segnTrasm=trasm.infoDocumento.dataApertura;
						segnTrasm="IdDoc:"+trasm.infoDocumento.docNumber;
					}
				}
			}
			if(trasm.tipoOggetto==DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO)
			{
				if(trasm.infoFascicolo!=null)
				{
					segnTrasm=trasm.infoFascicolo.codice;
				}
			}
			return segnTrasm;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trasm"></param>
		/// <returns></returns>
		private string CheckCodice(DocsPAWA.DocsPaWR.Trasmissione trasm)
		{
			string id="";
			if(trasm.tipoOggetto==DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
			{
				if(trasm.infoDocumento!=null)
				{
					//è un documento
					if(!string.IsNullOrEmpty(trasm.infoDocumento.numProt))
					{
						//è un protocollo
						id=trasm.infoDocumento.numProt + "<br>" + trasm.infoDocumento.dataApertura;
					}
					else
					{
						//è un documento grigio
						id = trasm.infoDocumento.docNumber + "<br>" + trasm.infoDocumento.dataApertura;
					}
				}				
			}

            if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(trasm.IdSegnaturaOCodFasc))
                id = trasm.IdSegnaturaOCodFasc + "<br/>" + trasm.DataDocFasc;

			return id;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trasm"></param>
		/// <returns></returns>
		private string TipoOggTrasm(DocsPAWA.DocsPaWR.Trasmissione trasm)
		{
			string InfoOggTrasm=null;

            if (trasm.tipoOggetto == DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO) //è UN DOCUMENTO
			    if(trasm.infoDocumento!=null)
                    if (trasm.infoDocumento.docNumber != null)
                    {
                        string msg;
                        int diritti = DocumentManager.verificaACL("D", trasm.infoDocumento.idProfile, UserManager.getInfoUtente(), out msg);

                        if (diritti == 2)
                        {
                            InfoOggTrasm = trasm.infoDocumento.oggetto;
                        }
                       
                        if(diritti ==0)
                        {
                            InfoOggTrasm = "<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "</b>";
                        }
                    }
				
		
			if(trasm.tipoOggetto==DocsPaWR.TrasmissioneTipoOggetto.FASCICOLO)  //è UN FASCICOLO
                if (trasm.infoFascicolo != null)
                {
                    // Verifica diritti dell'utente sul fascicolo
                    string msg;
                    int diritti = DocumentManager.verificaACL("F", trasm.infoFascicolo.idFascicolo, UserManager.getInfoUtente(), out msg);

                    if (diritti == 2)
                    {
                        InfoOggTrasm = trasm.infoFascicolo.descrizione;
                    }
                    
                    if(diritti==0)
                    {
                      InfoOggTrasm = "<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "</b>";
                    }
                }

          //  if (string.IsNullOrEmpty(InfoOggTrasm))
           //     InfoOggTrasm = "<b>Non si possiedono i diritti per la visualizzazione delle informazioni sul " + trasm.tipoOggetto.ToString().ToLower() + "</b>";

			return InfoOggTrasm;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trasm"></param>
		/// <returns></returns>
		private string CheckTipoOggTrasm(DocsPAWA.DocsPaWR.Trasmissione trasm)
		{
			string InfoOggTrasm=null;
			if(trasm.tipoOggetto==DocsPaWR.TrasmissioneTipoOggetto.DOCUMENTO)
			{
				if(trasm.infoDocumento!=null)
				{
					//è un documento
					if(!string.IsNullOrEmpty(trasm.infoDocumento.numProt))
					{
						InfoOggTrasm="prot";
					}
					else
					{
						InfoOggTrasm="nonprot";
					}
				}
				else
				{
					// è un fascicolo
					InfoOggTrasm="fasc";
				}						
			}
			return InfoOggTrasm;
		}



		private bool VerificaAutorizzazioneSuRegistro(string idRegistro)
		{
			bool result = false;
			try
			{
                if (!UserManager.isFiltroAooEnabled(this))
                {
                    if (idRegistro != null && idRegistro != "")
                    {
                        DocsPaWR.Registro[] RegRuoloSel = ((DocsPAWA.DocsPaWR.Ruolo)UserManager.getRuolo(this)).registri;
                        foreach (DocsPAWA.DocsPaWR.Registro reg in RegRuoloSel)
                        {
                            if (idRegistro == reg.systemId)
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
                else
                    result = true;
			
			}
			catch(Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void ShowInfo(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			//			LinkButton lb=((LinkButton) e.CommandSource);
			//			string g=lb.CommandArgument.ToString();
			try
			{
				if(e.CommandName=="ShowInfo")
				{	
					
					string key=((Label)e.Item.Cells[0].Controls[1]).Text;
					int id=Int32.Parse(key);

					// Impostazione indice del documento correntemente selezionato
					this.SetCurrentDocIndexOnContext(id);

					//start modifica x errore su click link trasmissione effettuate

					DocsPaWR.Trasmissione[] trasmlist = null;

					if(this._tipoRicerca.Equals("E"))
					{
						trasmlist=TrasmManager.getDocTrasmQueryEff(this);
					}

					if(this._tipoRicerca.Equals("R"))
					{
						trasmlist=TrasmManager.getDocTrasmQueryRic(this);
					}

					if ((trasmlist!= null)&& (trasmlist.Length > 0))

						TrasmManager.setDocTrasmSel(this,trasmlist[id]);

                    bool rimosso = false;
					bool EsitoVerifica=false;
                    string errorMessage = string.Empty;
                    int result = -1;


                    if (hashInfoOgg[id] != null) 
                    {
                         
                        
                        if (hashInfoOgg[id].GetType().Equals(typeof(DocsPAWA.DocsPaWR.InfoDocumento))
                            && hashInfoOgg[id] != null && ((DocsPAWA.DocsPaWR.InfoDocumento)hashInfoOgg[id]).idProfile != null
                             && ((DocsPAWA.DocsPaWR.InfoDocumento)hashInfoOgg[id]).idProfile !="")
                        {
                            result = DocumentManager.verificaACL("D", ((DocsPAWA.DocsPaWR.InfoDocumento)hashInfoOgg[id]).idProfile, UserManager.getInfoUtente(), out errorMessage);
                        }
                        else
                             if (hashInfoOgg[id].GetType().Equals(typeof(DocsPAWA.DocsPaWR.InfoFascicolo))
                            && hashInfoOgg[id] != null && ((DocsPAWA.DocsPaWR.InfoFascicolo)hashInfoOgg[id]).idFascicolo != null
                             && ((DocsPAWA.DocsPaWR.InfoFascicolo)hashInfoOgg[id]).idFascicolo !="")
                           {
                               result = DocumentManager.verificaACL("F", ((DocsPAWA.DocsPaWR.InfoFascicolo)hashInfoOgg[id]).idFascicolo, UserManager.getInfoUtente(), out errorMessage);
                            }

                        if (result != 0 && result != 1 && result !=-1)
                        {



                            if (hashInfoOgg[id].GetType().Equals(typeof(DocsPAWA.DocsPaWR.InfoFascicolo)))
                            {
                                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
                                DocsPaWR.Fascicolo fascicolo = FascicoliManager.getFascicolo(this, infoUtente.idGruppo, infoUtente.idPeople, (DocsPAWA.DocsPaWR.InfoFascicolo)this.hashInfoOgg[id]);
                                FascicoliManager.setFascicoloSelezionato(this, fascicolo);
                                FascicoliManager.setInfoFascicolo(this, (DocsPAWA.DocsPaWR.InfoFascicolo)this.hashInfoOgg[id]);
                                //per i fascicoli non è gestita la verifica della visibilità del ruolo 
                                //sul registro associato al nodo sul quale è stato creato il fascicolo
                                EsitoVerifica = true;
                            }
                            else
                            {
                                DocsPAWA.DocsPaWR.InfoDocumento info = ((DocsPAWA.DocsPaWR.InfoDocumento)this.hashInfoOgg[id]);
                                if ((info != null &&
                                    info.numProt != null
                                    && info.numProt != "") //PROTOCOLLATI SU REGISTRI NON AUTORIZZATI
                                    ||
                                (info != null &&
                                !(info.numProt != null
                                    && info.numProt != "") &&
                                 (info.idRegistro != null && info.idRegistro != "")
                                     ))                //PREDISPOSTI SU REGISTRI NON AUTORIZZATI
                                {
                                    //verifica se il ruolo selezionato ha la visibilità sul registro del documento 
                                    EsitoVerifica = VerificaAutorizzazioneSuRegistro(((DocsPAWA.DocsPaWR.InfoDocumento)this.hashInfoOgg[id]).idRegistro);
                                }
                                else
                                {
                                    //se il documento è grigio non occorre effettuare verifiche sui registri.
                                    EsitoVerifica = true;
                                }
                                if (EsitoVerifica)
                                {
                                    DocumentManager.setRisultatoRicerca(this, (DocsPAWA.DocsPaWR.InfoDocumento)this.hashInfoOgg[id]);
                                }
                            }
                        }

                    }
                    ////caso in cui all'utente è stato rimosso il diritto di visibilità
                    //else
                    //{
                   // inutile tanto EsitoVerifica è false Diagnostics default
                    //    EsitoVerifica = false;
                        

                    //}


                    if (EsitoVerifica && errorMessage=="")
					{
						// Impostazione contesto chiamante
						this.RefreshCallerContext(this._tipoRicerca);

						Session.Remove("Tipo_obj");
						//	Response.Write("<script language='javascript'>window.open('"+this.GetUrl(id)+"','principale');</script>");
						Response.Write("<script language='javascript'>top.principale.location='"+this.GetUrl(id)+"';</script>");
					}
					else 
					{
                        if(!EsitoVerifica && errorMessage!="")
                            ClientScript.RegisterStartupScript(this.GetType(), "errore", "<script language=javascript>alert('" + errorMessage + "');</script>");
                         else
                        if(!EsitoVerifica)
                            ClientScript.RegisterStartupScript(this.GetType(), "errore", "<script language=javascript>alert('Con il ruolo corrente non si hanno i diritti per poter visualizzare\\nil documento o il fascicolo selezionato.');</script>");
                       
                        
					}

                    //Inserisco in sessione l'oggetto per lo scorrimento continuo della lista
                    DataGrid dg = ((DataGrid)source);
                    if (hashInfoOgg[id] != null)
                    {
                    if (hashInfoOgg[id].GetType().Equals(typeof(DocsPAWA.DocsPaWR.InfoDocumento)))
                    {
                        //UserControls.ScrollElementsList.ScrollManager.setInSessionNewObjScrollElementsList(dg.VirtualItemCount, dg.PageCount, dg.PageSize, id, dg.CurrentPageIndex, new ArrayList(trasmlist), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC);
                        UserControls.ScrollElementsList.ScrollManager.setInContextNewObjScrollElementsList(dg.VirtualItemCount, dg.PageCount, dg.PageSize, id, dg.CurrentPageIndex, new ArrayList(trasmlist), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_DOC);
                    }
                    if (hashInfoOgg[id].GetType().Equals(typeof(DocsPAWA.DocsPaWR.InfoFascicolo)))
                    {
                        //UserControls.ScrollElementsList.ScrollManager.setInSessionNewObjScrollElementsList(dg.VirtualItemCount, dg.PageCount, dg.PageSize, id, dg.CurrentPageIndex, new ArrayList(trasmlist), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC);
                        UserControls.ScrollElementsList.ScrollManager.setInContextNewObjScrollElementsList(dg.VirtualItemCount, dg.PageCount, dg.PageSize, id, dg.CurrentPageIndex, new ArrayList(trasmlist), UserControls.ScrollElementsList.ObjScrollElementsList.EmunSearchContext.RICERCA_TRASM_FASC);
                    }
				}
			}
			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}
		#endregion			

		#region SMISTAMENTO
		private void btn_smista_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            //DocsPAWA.smistaDoc.SmistaDocSessionManager.ReleaseSmistamentoNoteIndividuali();
            DocsPAWA.smistaDoc.SmistaDocSessionManager.ReleaseSmistaDocManager();
            if(!this.ClientScript.IsStartupScriptRegistered("apriModalDialog"))
			{                
				string scriptString = "<SCRIPT>ApriSmistamento()</SCRIPT>";
				this.ClientScript.RegisterStartupScript(this.GetType(),"apriModalDialog", scriptString);
               
			}
		}
		#endregion		

		#region TIPO RICERCA
		private bool isRicercaPerFascicolo(DocsPAWA.DocsPaWR.FiltroRicerca[] filtriImpostati)
		{
			bool isFascicolo = false;
			foreach(DocsPAWA.DocsPaWR.FiltroRicerca filtro in filtriImpostati)
			{
				if(filtro.argomento.Equals("TIPO_OGGETTO") && filtro.valore.Equals("F"))
				{
					isFascicolo = true;
					break;
				}
			}
			return isFascicolo;
		}

		private bool isRicercaPerDocumento(DocsPAWA.DocsPaWR.FiltroRicerca[] filtriImpostati)
		{
			bool isDoc = false;
			foreach(DocsPAWA.DocsPaWR.FiltroRicerca filtro in filtriImpostati)
			{
				if(filtro.argomento.Equals("TIPO_OGGETTO") && filtro.valore.Equals("D"))
				{
					isDoc = true;
					break;
				}
			}
			return isDoc;
		}

		#endregion

		#region Call Context
	
		/// <summary>
		/// Impostazione contesto chiamante
		/// </summary>
		/// <param name="tabName"></param>
		private void RefreshCallerContext(string tabName)
		{
			SiteNavigation.CallContext currentContext=SiteNavigation.CallContextStack.CurrentContext;

			if (currentContext!=null && currentContext.ContextName==SiteNavigation.NavigationKeys.RICERCA_TRASMISSIONI)
				currentContext.QueryStringParameters["verso"]=tabName;
		}

		/// <summary>
		/// Reperimento numero pagina corrente dal contesto di ricerca
		/// </summary>
		/// <returns></returns>
		private int GetCurrentPageOnContext()
		{
			SiteNavigation.CallContext currentContext=SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext != null && currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_TRASMISSIONI)
				return currentContext.PageNumber;
			else
				return 1;
		}

		/// <summary>
		/// Impostazione numero pagina corrente del contesto di ricerca
		/// </summary>
		/// <param name="currentPage"></param>
		private void SetCurrentPageOnContext(int currentPage)
		{
			SiteNavigation.CallContext currentContext=SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext != null && currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_TRASMISSIONI)
				currentContext.PageNumber=currentPage;
		}

		/// <summary>
		/// Impostazione dell'indice del documento
		/// selezionato nel contesto di ricerca
		/// </summary>
		/// <param name="documentIndex"></param>
		private void SetCurrentDocIndexOnContext(int documentIndex)
		{
			SiteNavigation.CallContext currentContext=SiteNavigation.CallContextStack.CurrentContext;
            if (currentContext != null && currentContext.ContextName == SiteNavigation.NavigationKeys.RICERCA_TRASMISSIONI)
				currentContext.QueryStringParameters["docIndex"]=documentIndex.ToString();
		}

		#endregion

        #region Wait Control
        private void AttatchGridPagingWaitControl()
		{
			if (this._tipoRicerca.Equals("E"))
				DataGridPagingWaitControl.DataGridID=this.dt_Eff.ClientID;
			else
				DataGridPagingWaitControl.DataGridID=this.dt_Ric.ClientID;

			DataGridPagingWaitControl.WaitScriptCallback="WaitDataGridCallback(eventTarget,eventArgument);";
		}

		private waiting.DataGridPagingWait DataGridPagingWaitControl
		{
			get
			{
				return this.FindControl("DataGridPagingWait1") as waiting.DataGridPagingWait;
			}
        }
        #endregion       

        /// <summary>
        /// Fornisce il separatore per il campo oggetto/mittente nel caso
        /// delle tramissioni ricevute (documenti)
        /// </summary>
        /// <param name="oggetto"></param>
        /// <param name="mittente"></param>
        /// <returns></returns>
        protected string ShowSeparator(object mittente)
        {
            string mit = String.Empty;

            if (!mittente.GetType().Equals(System.DBNull.Value.GetType()))
                mit = (string)mittente;

            if (!String.IsNullOrEmpty(mit))
                return "<br/>-------<br/>";
            else
                return String.Empty;

        }

        #region Svuotamento TDL

        protected bool _isActivedTDLNotice = false;
        protected string _noticeDays = string.Empty;
        protected string _trasmOverNoticeDays = string.Empty;
        protected string _datePost = string.Empty;

        /// <summary>
        /// verifica se è stata attivata la funzionalità di avviso per lo svuotamento della TDL.
        /// parametri di OUT:
        ///     - noticeDays: giorni impostati dall'amministratore di sistema in homepage dell'amm.ne 
        /// 
        ///     - trasmOverNoticeDays: conteggio delle trasmissioni più vecchie dei noticeDays 
        /// 
        /// Se il metodo è false, allora i parametri di OUT sono uguali a string.Empty
        /// </summary>
        /// <returns>True o False</returns>
        private bool isNoticeActived()
		{
            string tipoObjTrasm = (string)Session["Tipo_obj"];
            TodoList.TodoListManager manager = new DocsPAWA.TodoList.TodoListManager(this.userRuolo, this.userHome, tipoObjTrasm,false);
            this._isActivedTDLNotice = manager.isNoticeActived(out this._noticeDays, out this._trasmOverNoticeDays, out this._datePost);
            return this._isActivedTDLNotice;
        }
        #endregion

        protected string GetDetails(System.Data.DataRowView trasm)
        {
            return trasm["utente"].ToString() + "<br/>(" + trasm["ruolo"].ToString() + ")";
        }

        protected void InitializePageSize()
        {
            string valoreChiave;
            valoreChiave = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente(this).idAmministrazione, "FE_PAGING_ROW_TRASM");
            int tempValue = 0;
            if (!string.IsNullOrEmpty(valoreChiave))
            {
                tempValue = Convert.ToInt32(valoreChiave);
                if (tempValue >= 8 || tempValue <= 50)
                {
                    this.pageSize = tempValue;
                }
            }
        }

        /// <summary>
        /// Numero di risultati per pagina
        /// </summary>
        public int pageSize
        {
            get
            {
                int toReturn = 8;
                if (CallContextStack.CurrentContext.ContextState["PageSizeTrasm"] != null)
                    toReturn = Convert.ToInt32(CallContextStack.CurrentContext.ContextState["PageSizeTrasm"].ToString());

                return toReturn;
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["PageSizeTrasm"] = value;
            }
        }

        protected int GetPageSize()
        {
            return this.pageSize;
        }
    }
}
