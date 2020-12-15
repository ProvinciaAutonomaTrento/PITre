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
using DocsPAWA.utils;
using DocsPAWA.DocsPaWR;
using System.Collections.Generic;

namespace DocsPAWA.ricercaDoc
{
	/// <summary>
	/// Summary description for ricDocStampaReg.
	/// </summary>
	public class ricDocStampaReg : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.ListBox lbxRegistro;
		protected System.Web.UI.WebControls.Button butt_ricerca;
	
		protected DocsPAWA.DocsPaWR.FiltroRicerca[][] qV;
		protected DocsPAWA.DocsPaWR.FiltroRicerca fV1;
		protected System.Web.UI.WebControls.Label lblNumProtocollo;
		protected System.Web.UI.WebControls.Label lblInitNumProtocollo;
		protected System.Web.UI.WebControls.TextBox txtInitNumProtocollo;
		protected System.Web.UI.WebControls.Label lblEndNumProtocollo;
		protected System.Web.UI.WebControls.TextBox txtEndNumProtocollo;
		protected System.Web.UI.WebControls.Label lblAnnoProtocollo;
		protected System.Web.UI.WebControls.TextBox txtAnnoProtocollo;
		protected System.Web.UI.HtmlControls.HtmlTable Table1;
		protected System.Web.UI.WebControls.DropDownList cboFilterTypeNumProtocollo;
		protected System.Web.UI.WebControls.Label lblDataStampa;
		protected System.Web.UI.WebControls.DropDownList cboFilterTypeDataStampa;
		protected System.Web.UI.WebControls.Label lblInitDataStampa;
		// protected DocsPaWebCtrlLibrary.DateMask txtInitDataStampa;
        protected DocsPAWA.UserControls.Calendar txtInitDataStampa;
		// protected DocsPaWebCtrlLibrary.DateMask txtEndDataStampa;
        protected DocsPAWA.UserControls.Calendar txtEndDataStampa;
		protected System.Web.UI.HtmlControls.HtmlTable tblNumProtocollo;
		protected System.Web.UI.HtmlControls.HtmlTable tblStampa;
		protected System.Web.UI.WebControls.Label lblEndDataStampa;
		protected DocsPAWA.DocsPaWR.FiltroRicerca[] fVList;

        protected bool change_from_grid;
        protected string numResult;
        public SchedaRicerca schedaRicerca = null;
        private const string KEY_SCHEDA_RICERCA = "StampaReg";

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if(!Page.IsPostBack)
			{
				this.AddControlsClientAttribute();

				this.setListaRegistri();

				this.FillComboFilterTypes();
			}

            if (GridManager.SelectedGrid == null || GridManager.SelectedGrid.GridType != GridTypeEnumeration.Document)
            {
                GridManager.SelectedGrid = GridManager.getUserGrid(GridTypeEnumeration.Document);
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["gridper"] != string.Empty && Request.QueryString["gridper"] != null)
                {
                    change_from_grid = true;
                }
                else
                {
                    change_from_grid = false;
                }
            }

            if (Request.QueryString["numRes"] != string.Empty && Request.QueryString["numRes"] != null)
            {
                this.numResult = Request.QueryString["numRes"];
            }
            else
            {
                this.numResult = string.Empty;
            }

			this.InitRangeFilterItems();

			this.EnableRangeFilterControls(this.cboFilterTypeNumProtocollo);
			this.EnableRangeFilterControls(this.cboFilterTypeDataStampa);

			tastoInvio();

            schedaRicerca = (SchedaRicerca)Session[SchedaRicerca.SESSION_KEY];
            if (schedaRicerca == null)
            {
                DocsPAWA.DocsPaWR.Utente userHome = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
                DocsPAWA.DocsPaWR.Ruolo userRuolo = (DocsPAWA.DocsPaWR.Ruolo)Session["userRuolo"];
                schedaRicerca = new SchedaRicerca(KEY_SCHEDA_RICERCA, userHome, userRuolo, this);
                Session[SchedaRicerca.SESSION_KEY] = schedaRicerca;
            }

            schedaRicerca.Pagina = this;

            if (change_from_grid)
            {
                if (schedaRicerca != null && schedaRicerca.FiltriRicerca != null)
                {
                    PopulateField(schedaRicerca.FiltriRicerca);
                }
                if (Ricerca())
                {

                    int numCriteri = 0;

                    if (qV[0] == null || qV[0].Length <= numCriteri)
                    {
                        Response.Write("<script>alert('Selezionare un registro');top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }

                    string altro = string.Empty;

                    if (!string.IsNullOrEmpty(this.numResult) && this.numResult.Equals("0"))
                    {
                        altro = "&noRic=1";
                    }

                    DocumentManager.setFiltroRicDoc(this, qV);
                    DocumentManager.removeDatagridDocumento(this);
                    DocumentManager.removeListaNonDocProt(this);
                    //	Response.Write("<script>parent.parent.iFrame_dx.document.location = 'tabRisultatiRicDocStampe.aspx';</script>");

                    ClientScript.RegisterStartupScript(this.GetType(), "regresh_dx", "top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?from=StampaReg&tabRes=StampaReg" + altro + "';", true);

                }
            }
		}

		private void setListaRegistri()
		{
			DocsPaWR.Registro[] registri = UserManager.getRuolo(this).registri;
			for (int i=0; i < registri.Length; i++)
			{
				//lbxRegistro.Items.Add(registri[i].descrizione);
				this.lbxRegistro.Items.Add(registri[i].codRegistro);
				lbxRegistro.Items[i].Value = registri[i].systemId;
			}
			//se uno solo auto selec
			if(lbxRegistro.Items.Count==1 )
					lbxRegistro.SelectedIndex=0;

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
			this.cboFilterTypeNumProtocollo.SelectedIndexChanged += new System.EventHandler(this.cboFilterType_SelectedIndexChanged);
			this.cboFilterTypeDataStampa.SelectedIndexChanged += new System.EventHandler(this.cboFilterType_SelectedIndexChanged);
			this.butt_ricerca.Click += new System.EventHandler(this.butt_ricerca_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region Gestione javascript
		
		/// <summary>
		/// Associazione funzioni javascript agli eventi client dei controlli
		/// </summary>
		private void AddControlsClientAttribute()
		{
			this.txtInitNumProtocollo.Attributes.Add("onKeyPress","ValidateNumericKey();");
			this.txtEndNumProtocollo.Attributes.Add("onKeyPress","ValidateNumericKey();");
			this.txtAnnoProtocollo.Attributes.Add("onKeyPress","ValidateNumericKey();");
		}

		/// <summary>
		/// Impostazione del focus su un controllo
		/// </summary>
		/// <param name="controlID"></param>
		private void SetControlFocus(string controlID)
		{
			this.RegisterClientScript("SetFocus","document.frmFiltriRicercaDocumenti." + controlID + ".focus();");
		}

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}

		#endregion

		#region Validazione dati filtro immessi

		private bool IsValidData(out ArrayList validationItems,
								 out string firstInvalidControlID)
		{
			validationItems=new ArrayList();
			firstInvalidControlID=string.Empty;

			// Validazione numero protocollo
			this.ValidateNumericRange("Numero protocollo",this.txtInitNumProtocollo,this.txtEndNumProtocollo,validationItems,ref firstInvalidControlID);

			// Validazione anno immesso
			if (this.txtAnnoProtocollo.Text!=string.Empty && 
				!this.IsValidNumber(this.txtAnnoProtocollo))
				validationItems.Add("Anno protocollo non valido");

			// Validazione data stampa
			this.ValidateDateRange("Data stampa",this.GetCalendarControl("txtInitDataStampa").txt_Data,this.GetCalendarControl("txtEndDataStampa").txt_Data,validationItems,ref firstInvalidControlID);
			
			return (validationItems.Count==0);
		}

		/// <summary>
		/// Validazione range di dati numerici
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="initText"></param>
		/// <param name="endText"></param>
		/// <param name="validationItems"></param>
		/// <param name="firstInvalidControlID"></param>
		private void ValidateNumericRange( 	string fieldName,
											TextBox initText,
											TextBox endText,
											ArrayList validationItems,
											ref string firstInvalidControlID)
		{
			bool isValidInitNumber=false;
			bool isValidEndNumber=false;

			if (initText.Text.Length>0)
			{
				isValidInitNumber=this.IsValidNumber(initText);
				
				if (!isValidInitNumber)
				{
					validationItems.Add(fieldName + " non valido");
					
					if (firstInvalidControlID==string.Empty)
						firstInvalidControlID=initText.ID;				
				}
			}

			if (endText.Visible && endText.Text.Length>0)
			{
				if (endText.Visible)
				{
					isValidEndNumber=this.IsValidNumber(endText);

					if (!isValidEndNumber)
					{
						validationItems.Add(fieldName + " non valido");
				
						if (firstInvalidControlID==string.Empty)
							firstInvalidControlID=endText.ID;
					}
				}
			}

			if (initText.Text.Length>0 || (endText.Visible && endText.Text.Length>0))
			{
				if (initText.Text.Equals(string.Empty))
					validationItems.Add(fieldName + " iniziale mancante");

				else if (endText.Visible && endText.Text.Equals(string.Empty))
					validationItems.Add(fieldName + " finale mancante");
			}

			// Validazione range di dati
			if (isValidInitNumber && isValidEndNumber && 
				int.Parse(initText.Text) > int.Parse(endText.Text))
			{
				validationItems.Add(fieldName + " iniziale maggiore di quello finale");
					
				if (firstInvalidControlID==string.Empty)
					firstInvalidControlID=endText.ID;
			}
			
		}

		/// <summary>
		/// Validazione range di date
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="initDate"></param>
		/// <param name="endDate"></param>
		/// <param name="validationItems"></param>
		/// <param name="firstInvalidControlID"></param>
		private void ValidateDateRange(	string fieldName,
										DocsPaWebCtrlLibrary.DateMask initDate,
										DocsPaWebCtrlLibrary.DateMask endDate,
										ArrayList validationItems,
										ref string firstInvalidControlID)
		{
			bool isValidInitDate=false;
			bool isValidEndDate=false;

			if (initDate.Text.Length>0)
			{
				// Validazione data iniziale
				isValidInitDate=this.IsValidDate(initDate);
			
				if (!isValidInitDate)
				{
					validationItems.Add(fieldName + " iniziale non valida");
					
					if (firstInvalidControlID==string.Empty)
						firstInvalidControlID=initDate.ID;
				}
			}

			if (endDate.Visible && endDate.Text.Length>0)
			{
				// Validazione data finale
				isValidEndDate=this.IsValidDate(endDate);

				if (!isValidEndDate)
				{
					validationItems.Add(fieldName + " finale non valida");
					
					if (firstInvalidControlID==string.Empty)
						firstInvalidControlID=endDate.ID;
				}
			}
			
			if (initDate.Text.Length>0 || (endDate.Visible && endDate.Text.Length>0))
			{
				if (initDate.Text.Equals(string.Empty))
					validationItems.Add(fieldName + " iniziale mancante");

				else if (endDate.Visible && endDate.Text.Equals(string.Empty))
					validationItems.Add(fieldName + " finale mancante");
			}

			// Validazione range di dati
			if (isValidInitDate && isValidEndDate && 
				DateTime.Parse(initDate.Text) > DateTime.Parse(endDate.Text))
			{
				validationItems.Add(fieldName + " iniziale maggiore di quella finale");
				
				if (firstInvalidControlID==string.Empty)
					firstInvalidControlID=endDate.ID;
			}
		}

		/// <summary>
		/// Validazione singola data
		/// </summary>
		/// <param name="dateMask"></param>
		/// <returns></returns>
		private bool IsValidDate(DocsPaWebCtrlLibrary.DateMask dateMask)
		{
			bool retValue=false;
			
			if (dateMask.Text.Length>0)
				retValue=DocsPAWA.Utils.isDate(dateMask.Text);
			
			return retValue;
		}

		/// <summary>
		/// Validazione valore numerico
		/// </summary>
		/// <param name="numberText"></param>
		/// <returns></returns>
		private bool IsValidNumber(TextBox numberText)
		{
			bool retValue=true;
			
			try
			{
				int.Parse(numberText.Text);
			}
			catch 
			{
				retValue=false;
			}

			return retValue;
		}

		#endregion

		protected bool Ricerca()
		{

			try
			{
				//array contenitore degli array filtro di ricerca
				qV = new DocsPAWA.DocsPaWR.FiltroRicerca[1][];
				qV[0]=new DocsPAWA.DocsPaWR.FiltroRicerca[1];
				
				fVList=new DocsPAWA.DocsPaWR.FiltroRicerca[0];
					

				#region filtro registro
				fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
				fV1.argomento=DocsPaWR.FiltriDocumento.REGISTRO.ToString();
				if (this.lbxRegistro.Items.Count > 0) 
				{
					if (this.lbxRegistro.SelectedIndex>=0)
					{	
						fV1.valore=this.lbxRegistro.Items[lbxRegistro.SelectedIndex].Value;
						fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
					}
					else
					{
						Response.Write("<script>alert('Selezionare un registro');top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
						return false;
					}
				}
				else
					return false;
				#endregion 

				#region filtro Archivio  (cerco tutti i doc del tipo R)

				fV1= new DocsPAWA.DocsPaWR.FiltroRicerca();
				fV1.argomento=DocsPaWR.FiltriDocumento.TIPO.ToString();
				fV1.valore="R";
				fVList=Utils.addToArrayFiltroRicerca(fVList,fV1);
				#endregion 

				#region Filtro per numero e anno protocollo

				this.AddFilterNumProtocollo(ref fVList);

				#endregion

				#region Filtro per data stampa registro

				this.AddFilterDataStampa(ref fVList);

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
                List<FiltroRicerca> filterList;

                if (GridManager.SelectedGrid.FieldForOrder != null)
                {
                    Field field = GridManager.SelectedGrid.FieldForOrder;
                    filterList = GridManager.GetOrderFilterForDocument(
                        field.FieldId,
                        GridManager.SelectedGrid.OrderDirection.ToString());
                }
                else
                    filterList = GridManager.GetOrderFilterForDocument(String.Empty, "DESC");

                // Se la lista è valorizzata vengono aggiunti i filtri
                if (filterList != null)
                    foreach (FiltroRicerca filter in filterList)
                        fVList = Utils.addToArrayFiltroRicerca(fVList, filter);

                #endregion

				qV[0]=fVList;

                DocumentManager.setFiltroRicDoc(this, qV);
                DocumentManager.removeDatagridDocumento(this);
                DocumentManager.removeListaDocProt(this);	

				return true;
			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
				return false;
			}

		}


		private void butt_ricerca_Click(object sender, EventArgs e)
		{
			try
			{
				//Controllo intervallo date
                if (this.cboFilterTypeDataStampa.SelectedIndex != 0)
                {
                    if (Utils.isDate(this.GetCalendarControl("txtInitDataStampa").txt_Data.Text) && Utils.isDate(this.GetCalendarControl("txtEndDataStampa").txt_Data.Text) && Utils.verificaIntervalloDate(this.GetCalendarControl("txtInitDataStampa").txt_Data.Text, this.GetCalendarControl("txtEndDataStampa").txt_Data.Text))
                    {
                        Response.Write("<script>alert('Verificare intervallo date Data Stampa!');</script>");
                        //string s = "<SCRIPT language='javascript'>document.getElementById('" + this.GetCalendarControl("txtInitDataStampa").txt_Data.ID + "').focus();</SCRIPT>";
                        //RegisterStartupScript("focus", s);

                      //  Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDoc.aspx';</script>");
                      Response.Write("<script>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
                        return;
                    }
                }
				//Fine controllo intervallo date

				ArrayList validationItems=null;
				string firstInvalidControlID=string.Empty;

				if (this.IsValidData(out validationItems,out firstInvalidControlID))
				{
					if (Ricerca())
					{
					
						int numCriteri = 0;

						if (qV[0] == null || qV[0].Length <= numCriteri)
						{
							Response.Write("<script>alert('Selezionare un registro');top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
							return;
						}
						else
						{
							//if (!Page.IsStartupScriptRegistered("wait"))
							//{
							//	Page.RegisterStartupScript("wait","<script>DocsPa_FuncJS_WaitWindows();</script>");
							//		}
						}

                        schedaRicerca.FiltriRicerca = qV;
						DocumentManager.setFiltroRicDoc(this,qV);
						DocumentManager.removeDatagridDocumento(this);
						DocumentManager.removeListaNonDocProt(this);
						//	Response.Write("<script>parent.parent.iFrame_dx.document.location = 'tabRisultatiRicDocStampe.aspx';</script>");	
                      
                        //modifica del 14/05/2009
                        // Response.Write("<script language='javascript'>top.principale.iFrame_dx.document.location = 'tabRisultatiRicDocStampe.aspx';</script>");
                        Response.Write("<script  language='javascript'>top.principale.iFrame_dx.document.location = 'NewTabSearchResult.aspx?tabRes=StampaReg';</script>");
                       //fine modifica del 14/05/2009
                        
					}
				}
				else
				{
					string validationMessage=string.Empty;

					foreach (string item in validationItems)
					{
						if (validationMessage!=string.Empty)
							validationMessage += @"\n";

						validationMessage += " - " + item;
					}

					if (validationMessage!=string.Empty)
						validationMessage="Sono state rilevate le seguenti incongruenze: " +  
							@"\n" +  @"\n" + validationMessage;

					this.RegisterClientScript("ValidationMessage","alert('" + validationMessage + "');");

					// impostazione del focus sul primo controllo non valido
					this.SetControlFocus(firstInvalidControlID);	
	
					Response.Write("<script language='javascript'>top.principale.document.iFrame_dx.location='../blank_page.htm';</script>");
				}
			}
			catch(System.Exception ex)
			{
				
				ErrorManager.redirect(this,ex);
			}
		}

		public void tastoInvio()
		{
			Utils.DefaultButton(this,ref txtInitNumProtocollo, ref butt_ricerca);
			Utils.DefaultButton(this,ref txtEndNumProtocollo, ref butt_ricerca);
			Utils.DefaultButton(this,ref txtAnnoProtocollo, ref butt_ricerca);
			Utils.DefaultButton(this,ref this.GetCalendarControl("txtInitDataStampa").txt_Data, ref butt_ricerca);
			Utils.DefaultButton(this,ref this.GetCalendarControl("txtEndDataStampa").txt_Data, ref butt_ricerca);			
		}


		#region Gestione filtri stampa registro

		// Costanti che definiscono le tipologie di filtro disponibili
		private const string RANGE_FILTER_TYPE_INTERVAL="I";
		private const string RANGE_FILTER_TYPE_SINGLE="S";

		private ListItem[] GetListItemsTipiSelezione()
		{
			ListItem[] items=new ListItem[2];
			items[0]=new ListItem("Valore singolo",RANGE_FILTER_TYPE_SINGLE);
			items[1]=new ListItem("Intervallo",RANGE_FILTER_TYPE_INTERVAL);
			return items;
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


		/// <summary>
		/// Caricamento dati combo tipologie filtri
		/// </summary>
		private void FillComboFilterTypes()
		{
			this.cboFilterTypeNumProtocollo.Items.AddRange(this.GetListItemsTipiSelezione());
			this.cboFilterTypeDataStampa.Items.AddRange(this.GetListItemsTipiSelezione());
		}

		private Hashtable _rangeFilterItems=null;

		private void InitRangeFilterItems()
		{
			this._rangeFilterItems=new Hashtable();

			this._rangeFilterItems.Add(this.cboFilterTypeNumProtocollo,
				this.CreateRangeFilterInnerHT(	this.lblInitNumProtocollo,
												this.txtInitNumProtocollo,
												this.lblEndNumProtocollo,
												this.txtEndNumProtocollo));


			this._rangeFilterItems.Add(this.cboFilterTypeDataStampa,
				this.CreateRangeFilterInnerHT(	this.lblInitDataStampa,
												this.GetCalendarControl("txtInitDataStampa").txt_Data,
												this.lblEndDataStampa,
												this.GetCalendarControl("txtEndDataStampa").txt_Data));
		}

		private void DisposeRangeFilterItems()
		{
			this._rangeFilterItems.Clear();
			this._rangeFilterItems=null;
		}

		private Hashtable CreateRangeFilterInnerHT(Label initLabel,
													TextBox initText,
													Label endLabel,
													TextBox endText)
		{
			Hashtable retValue=new Hashtable();
			retValue.Add("INIT_LABEL",initLabel);
			retValue.Add("INIT_TEXT",initText);
			retValue.Add("END_LABEL",endLabel);
			retValue.Add("END_TEXT",endText);
			return retValue;
		}

		private void EnableRangeFilterControls(DropDownList cboFilterType)
		{
			bool intervalFilterEnabled=(cboFilterType.SelectedValue==RANGE_FILTER_TYPE_INTERVAL);
			Hashtable innerHT=(Hashtable) this._rangeFilterItems[cboFilterType];

			Label initLabel=(Label) innerHT["INIT_LABEL"];
			TextBox initText=(TextBox) innerHT["INIT_TEXT"];
			Label endLabel=(Label) innerHT["END_LABEL"];
			TextBox endText=(TextBox) innerHT["END_TEXT"];

			initLabel.Visible=intervalFilterEnabled;
			initText.Visible=true;
			endLabel.Visible=intervalFilterEnabled;
			endText.Visible=intervalFilterEnabled;			
		}

		private void cboFilterType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			EnableRangeFilterControls((DropDownList) sender);

            if (this.cboFilterTypeDataStampa.SelectedIndex == 0)
            {
                this.GetCalendarControl("txtEndDataStampa").Visible = false;
                this.GetCalendarControl("txtEndDataStampa").btn_Cal.Visible = false;
                this.GetCalendarControl("txtEndDataStampa").txt_Data.Visible = false;
            }
            else
            {
                this.GetCalendarControl("txtEndDataStampa").Visible = true;
                this.GetCalendarControl("txtEndDataStampa").btn_Cal.Visible = true;
                this.GetCalendarControl("txtEndDataStampa").txt_Data.Visible = true;
            }



		}


		/// <summary>
		/// Creazione oggetti di filtro per numero protocollo
		/// </summary>
		/// <param name="filterItems"></param>
		private void AddFilterNumProtocollo(ref DocsPAWA.DocsPaWR.FiltroRicerca[] filterItems)
		{
			bool rangeFilterInterval=(this.cboFilterTypeNumProtocollo.SelectedValue==RANGE_FILTER_TYPE_INTERVAL);

			DocsPaWR.FiltroRicerca filterItem=null;

			if (this.txtInitNumProtocollo.Text.Length>0)
			{
				filterItem=new DocsPAWA.DocsPaWR.FiltroRicerca();

				if (rangeFilterInterval)
                    filterItem.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_DAL.ToString();
				else
                    filterItem.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA.ToString();

				filterItem.valore=this.txtInitNumProtocollo.Text;
				filterItems=Utils.addToArrayFiltroRicerca(filterItems,filterItem);
				filterItem=null;
			}

			if (rangeFilterInterval && this.txtEndNumProtocollo.Text.Length>0)
			{
				filterItem=new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_AL.ToString();
				filterItem.valore=this.txtEndNumProtocollo.Text;
				filterItems=Utils.addToArrayFiltroRicerca(filterItems,filterItem);
				filterItem=null;
			}

			if (this.txtAnnoProtocollo.Text.Length>0)
			{
				filterItem=new DocsPAWA.DocsPaWR.FiltroRicerca();
                filterItem.argomento = DocsPaWR.FiltriStampaRegistro.ANNO_PROTOCOLLO_STAMPA.ToString();
				filterItem.valore=this.txtAnnoProtocollo.Text;
				filterItems=Utils.addToArrayFiltroRicerca(filterItems,filterItem);
				filterItem=null;
			}
		}

		/// <summary>
		/// Creazione oggetti di filtro per data stampa
		/// </summary>
		/// <param name="filterItems"></param>
		private void AddFilterDataStampa(ref DocsPAWA.DocsPaWR.FiltroRicerca[] filterItems)
		{
			bool rangeFilterInterval=(this.cboFilterTypeDataStampa.SelectedValue==RANGE_FILTER_TYPE_INTERVAL);

			DocsPaWR.FiltroRicerca filterItem=null;

			if (this.GetCalendarControl("txtInitDataStampa").txt_Data.Text.Length>0)
			{
				filterItem=new DocsPAWA.DocsPaWR.FiltroRicerca();

				if (rangeFilterInterval)
					filterItem.argomento=DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_DAL.ToString();
				else
					filterItem.argomento=DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO.ToString();

				filterItem.valore=this.GetCalendarControl("txtInitDataStampa").txt_Data.Text;
				filterItems=Utils.addToArrayFiltroRicerca(filterItems,filterItem);
				filterItem=null;
			}

			if (rangeFilterInterval && this.GetCalendarControl("txtEndDataStampa").txt_Data.Text.Length>0)
			{
				filterItem=new DocsPAWA.DocsPaWR.FiltroRicerca();
				filterItem.argomento=DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_AL.ToString();
				filterItem.valore=this.GetCalendarControl("txtEndDataStampa").txt_Data.Text;
				filterItems=Utils.addToArrayFiltroRicerca(filterItems,filterItem);
				filterItem=null;
			}
		}

        protected void PopulateField(DocsPAWA.DocsPaWR.FiltroRicerca[][] qV)
        {
            try
            {
                if (qV != null || qV.Length>0)
                {
                    DocsPaWR.FiltroRicerca[] filters = qV[0];

                    foreach (DocsPAWA.DocsPaWR.FiltroRicerca aux in filters)
                    {
                        if (aux.argomento == DocsPaWR.FiltriDocumento.REGISTRO.ToString())
                        {
                            this.lbxRegistro.SelectedValue = aux.valore;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_DAL.ToString())
                        {
                            this.cboFilterTypeNumProtocollo.SelectedValue = RANGE_FILTER_TYPE_INTERVAL;
                            this.txtInitNumProtocollo.Text = aux.valore;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA.ToString())
                        {
                            this.txtInitNumProtocollo.Text = aux.valore;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.NUM_PROTOCOLLO_STAMPA_AL.ToString())
                        {
                            this.cboFilterTypeNumProtocollo.SelectedValue = RANGE_FILTER_TYPE_INTERVAL;
                            this.txtEndNumProtocollo.Text = aux.valore;
                            this.txtEndNumProtocollo.Visible = true;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.ANNO_PROTOCOLLO_STAMPA.ToString())
                        {
                            this.txtAnnoProtocollo.Text = aux.valore;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_DAL.ToString())
                        {
                            this.cboFilterTypeDataStampa.SelectedValue = RANGE_FILTER_TYPE_INTERVAL;
                            this.GetCalendarControl("txtInitDataStampa").txt_Data.Text = aux.valore;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO.ToString())
                        {
                            this.GetCalendarControl("txtInitDataStampa").txt_Data.Text = aux.valore;
                        }

                        if (aux.argomento == DocsPaWR.FiltriStampaRegistro.DATA_STAMPA_REGISTRO_AL.ToString())
                        {
                            this.cboFilterTypeDataStampa.SelectedValue = RANGE_FILTER_TYPE_INTERVAL;
                            this.GetCalendarControl("txtEndDataStampa").txt_Data.Text = aux.valore;
                            this.GetCalendarControl("txtEndDataStampa").Visible = true;
                        }
                    }
                }
            }

            catch (System.Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }


		#endregion

	}
}
