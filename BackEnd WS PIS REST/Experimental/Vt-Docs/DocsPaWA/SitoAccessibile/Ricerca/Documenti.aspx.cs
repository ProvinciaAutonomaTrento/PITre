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
using DocsPAWA.SitoAccessibile.Validations;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Gestione ricerca fascicoli
	/// </summary>
	public class Documenti : SessionWebPage
	{
		protected System.Web.UI.WebControls.Button btnSearch;
		protected System.Web.UI.WebControls.Button btnClearFilters;
		protected System.Web.UI.WebControls.CheckBox chkProtocolloArrivo;
		protected System.Web.UI.WebControls.CheckBox chkProtocolloPartenza;
		protected System.Web.UI.WebControls.CheckBox chkProtocolloInterno;
		protected System.Web.UI.WebControls.CheckBox chkDocumentoGrigio;
		protected System.Web.UI.WebControls.Button btnSelectTipoDocumento;
		protected System.Web.UI.WebControls.CheckBoxList CheckBoxList1;
		protected System.Web.UI.WebControls.Button btnAdvancedFilters;

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			this.SetFieldsVisibility();

			this.RefreshButtonAdvancedFilters(this.AdvancedFilterPanelVisible);
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (RicercaDocumentiHandler.CurrentFilter==null)
				RicercaDocumentiHandler.CurrentFilter=new SearchProperties();
			
			this.GetControlFiltriDocumenti().Initialize(this.GetControlValidation());

			this.GetControlFiltriAvanzati().Initialize(this.GetControlValidation());

			if (!this.IsPostBack)
			{
				ResourceReleaser.Register(new DocumentiFilterResourceReleaser());

				this.AdvancedFilterPanelVisible=this.Mask.AdvancedDocProperties || 
					this.Mask.AdvancedProtProperties;

				this.InitializeValidationControl();

				this.Fetch();
			}
		}

		#region Gestione dati

		/// <summary>
		/// Momentaneo
		/// </summary>
		protected SearchProperties Mask
		{
			get
			{
				return RicercaDocumentiHandler.CurrentFilter;
			}
		}

		/// <summary>
		/// Caricamento dati filtri
		/// </summary>
		/// <param name="searchProperties"></param>
		private void Fetch()
		{	
			this.FetchTipiDocumento();

			this.GetControlFiltriDocumenti().LoadData();

			if (this.AdvancedFilterPanelVisible)
				this.GetControlFiltriAvanzati().LoadData();
		}

		/// <summary>
		/// Caricamento dati tipologie di documento
		/// </summary>
		private void FetchTipiDocumento()
		{
			this.chkProtocolloArrivo.Checked=this.Mask.ProtocolliArrivo;
			this.chkProtocolloPartenza.Checked=this.Mask.ProtocolliPartenza;
			this.chkProtocolloInterno.Checked=this.Mask.ProtocolliInterni;
			this.chkDocumentoGrigio.Checked=this.Mask.DocumentiGrigi;
		}

		/// <summary>
		/// Caricamento dati di filtro immessi nell'oggetto "SearchProperties"
		/// </summary>
		private void FetchSearchProperties()
		{
			this.GetControlFiltriDocumenti().RefreshSearchProperties();

			if (this.AdvancedFilterPanelVisible)
				this.GetControlFiltriAvanzati().RefreshSearchProperties();
		}

		#endregion

		#region Gestione validazione dati immessi

		/// <summary>
		/// Reperimento controllo di validazione
		/// </summary>
		/// <returns></returns>
		private ValidationContainer GetControlValidation()
		{
			return this.FindControl("validationContainer") as ValidationContainer;
		}

		/// <summary>
		/// Inizializzazione controllo di validazione
		/// </summary>
		private void InitializeValidationControl()
		{
			ValidationContainer ctl=this.GetControlValidation();
			ctl.HeaderText="Sono state rilevate le seguenti incongruenze nei filtri immessi:";
		}

		#endregion

		#region Azioni

		/// <summary>
		/// Impostazione visibilità campi UI
		/// </summary>
		private void SetFieldsVisibility()
		{
			this.chkProtocolloInterno.Visible=EnvironmentContext.IsEnabledProtocolloInterno();

			// Impostazione visibilità campi UI dipendenti dalle 
			/// tipologie di documento selezionate
			this.SetTipoDocumentoFieldsVisibility();
		}

		/// <summary>
		/// Impostazione visibilità campi UI dipendenti dalle 
		/// tipologie di documento selezionate
		/// </summary>
		private void SetTipoDocumentoFieldsVisibility()
		{			
			// Impostazione visibilità filtri avanzati
			this.GetControlFiltriAvanzati().Visible=this.AdvancedFilterPanelVisible;
		}


		/// <summary>
		/// Rimozione dei filtri correntemente visualizzati
		/// </summary>
		private void ClearFilters()
		{
			if (RicercaDocumentiHandler.CurrentFilter!=null)
				RicercaDocumentiHandler.CurrentFilter=null;

			RicercaDocumentiHandler.CurrentFilter=new SearchProperties();
			
			this.FetchTipiDocumento();

			this.AdvancedFilterPanelVisible=false;

			this.GetControlFiltriDocumenti().LoadData();

			this.GetControlFiltriAvanzati().LoadData();
		}

		/// <summary>
		/// Gestione validazione filtri
		/// </summary>
		private void ValidateFilters()
		{
			this.ValidateFiltersTipiDocumento();

			this.GetControlFiltriDocumenti().ValidateFilters();

			if (this.AdvancedFilterPanelVisible)
				// Validazione filtri avanzati immessi, se visibili
				this.GetControlFiltriAvanzati().ValidateFilters();
		}

		/// <summary>
		/// Validazione dei checkbox tipi documento: 
		/// verifica se almeno un tipo di documento è stato selezionato
		/// </summary>
		private bool ValidateFiltersTipiDocumento()
		{
			bool valid=this.chkDocumentoGrigio.Checked ||
				this.chkProtocolloArrivo.Checked ||
				this.chkProtocolloInterno.Checked ||
				this.chkProtocolloPartenza.Checked;

			if (!valid)
				this.GetControlValidation().AddControlErrorMessage("","Selezionare almeno un tipo di documento");

			return valid;
		}

		/// <summary>
		/// Ricerca documenti con i filtri immessi
		/// </summary>
		private void PerformActionSearch()
		{
			// Validazione filtri
			this.ValidateFilters();

			// Caricamento filtri immessi
			this.FetchSearchProperties();

			if (this.GetControlValidation().IsValid())
				this.Context.Response.Redirect("./EsitoRicercaDocumenti.aspx?action=new",true);
		}
	
		/// <summary>
		/// Azione di selezione del tipo di documento
		/// </summary>
		private void PerformActionSelectTipoDocumento()
		{
			if (this.ValidateFiltersTipiDocumento())
			{
				this.Mask.ProtocolliArrivo=this.chkProtocolloArrivo.Checked;
				this.Mask.ProtocolliPartenza=this.chkProtocolloPartenza.Checked;
				this.Mask.ProtocolliInterni=this.chkProtocolloInterno.Checked;
				this.Mask.DocumentiGrigi=this.chkDocumentoGrigio.Checked;

//				this.Mask.AdvancedDocProperties=this.chkDocumentoGrigio.Checked;
//				this.Mask.AdvancedProtProperties=this.chkProtocolloArrivo.Checked ||
//					this.chkProtocolloInterno.Checked ||
//					this.chkProtocolloPartenza.Checked;
			}
		}


		#region Gestione rubrica

		private void ShowRubrica()
		{
			string pg=EnvironmentContext.RootPath + "Rubrica/Rubrica.aspx?action=new&pgcall=DocsPAWA.SitoAccessibile.Ricerca.Documenti&field=pr_mitt_dest&urpl=urp&ie=ie&capacity=one";
			this.Context.Response.Redirect(pg,true);
		}

		private void AddressbookResult()
		{
			string field = (string)this.Context.Request.Params["field"];
			SitoAccessibile.Rubrica.Rubrica.AddressbookContainer results = null;
			if (Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookContainer"]!=null)
			{
				results = (SitoAccessibile.Rubrica.Rubrica.AddressbookContainer)Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookContainer"];
				Session.Remove("DocsPAWA.SitoAccessibile.Rubrica.Rubrica.AddressbookContainer");
			}

			switch (field)
			{
				case "pr_mitt_dest":
					this.Mask.Protocollo.Corrispondente = (results.GlobalRecipient!=null && results.GlobalRecipient.Length!=0) ? results.GlobalRecipient[0] : new DocsPAWA.DocsPaWR.Corrispondente();
					break;
				case "pe_mitt_inter":
					this.Mask.Protocollo.MittenteIntermedio = (results.GlobalRecipient!=null && results.GlobalRecipient.Length!=0) ? results.GlobalRecipient[0] : new DocsPAWA.DocsPaWR.Corrispondente();
					break;
				default:
					break;
			}
		}

		private void OpenAddressBookForMittInter()
		{
			this.FetchSearchProperties();

			string pg = "../Rubrica/Rubrica.aspx?action=new&pgcall=DocsPAWA.SitoAccessibile.Ricerca.Documenti&field=pe_mitt_inter&urpl=urp&ie=e&capacity=one";
			this.Context.Response.Redirect(pg,true);
		}

		#endregion

		#region Gestione filtri avanzati

		/// <summary>
		/// Azione di selezione dei filtri avanzati
		/// </summary>
		private void PerformActionSelectAdvancedFilters()
		{
			this.AdvancedFilterPanelVisible=!this.AdvancedFilterPanelVisible;

			this.Mask.AdvancedDocProperties=this.AdvancedFilterPanelVisible && 
				this.chkDocumentoGrigio.Checked;
			
			this.Mask.AdvancedProtProperties=this.AdvancedFilterPanelVisible &&
				(this.chkProtocolloArrivo.Checked ||
				this.chkProtocolloInterno.Checked ||
				this.chkProtocolloPartenza.Checked);

			// Caricamento dati
			this.GetControlFiltriAvanzati().LoadData();
		}

		/// <summary>
		/// Aggiornamento testo pulsante filtri avanzati
		/// </summary>
		private void RefreshButtonAdvancedFilters(bool advancedFilterPanelVisible)
		{
			if (advancedFilterPanelVisible)
				this.btnAdvancedFilters.Text="Mostra opzioni base";
			else
				this.btnAdvancedFilters.Text="Mostra opzioni avanzate";
		}

		/// <summary>
		/// Reperimento controllo filtri
		/// </summary>
		/// <returns></returns>
		private BaseFiltriRicercaDocumenti GetControlFiltriDocumenti()
		{
			return this.FindControl("filtriDocumenti") as BaseFiltriRicercaDocumenti;
		}

		/// <summary>
		/// Reperimento controllo filtri avanzati
		/// </summary>
		/// <returns></returns>
		private BaseFiltriRicercaDocumenti GetControlFiltriAvanzati()
		{
			return this.FindControl("filtriDocumentiAvanzati") as BaseFiltriRicercaDocumenti;
		}

		/// <summary>
		/// Flag che gestisce se i filtri avanzati sono visibili o meno
		/// </summary>
		private bool AdvancedFilterPanelVisible
		{
			get
			{
				bool retValue=false;
				if (this.ViewState["AdvancedFilterPanelVisible"]!=null)
					retValue=Convert.ToBoolean(this.ViewState["AdvancedFilterPanelVisible"]);

				return retValue;
			}
			set
			{
				this.ViewState["AdvancedFilterPanelVisible"]=value;
			}
		}

		#endregion

		#endregion

		#region Gestione eventi controlli UI

		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			this.PerformActionSearch();
		}

		private void btnClearFilters_Click(object sender, System.EventArgs e)
		{
			this.ClearFilters();
		}

		private void btnAdvancedFilters_Click(object sender, System.EventArgs e)
		{
			this.PerformActionSelectAdvancedFilters();
		}

		private void btnSelectTipoDocumento_Click(object sender, System.EventArgs e)
		{
			this.PerformActionSelectTipoDocumento();
		}

		private void btnSelectAllRegistri_Click(object sender, System.EventArgs e)
		{
		
		}

		private void btnShowRubricaMittDest_Click(object sender, System.EventArgs e)
		{
			this.ShowRubrica();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnSelectTipoDocumento.Click += new System.EventHandler(this.btnSelectTipoDocumento_Click);
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			this.btnClearFilters.Click += new System.EventHandler(this.btnClearFilters_Click);
			this.btnAdvancedFilters.Click += new System.EventHandler(this.btnAdvancedFilters_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);
		}
		#endregion
	}
}