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
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Validations;
using DocsPAWA.SitoAccessibile.Trasmissioni;
using DocsPAWA.SitoAccessibile.Documenti.Trasmissioni;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Pagina filtri di ricerca trasmissione
	/// </summary>
	public class Trasmissioni : SessionWebPage
	{
		protected System.Web.UI.WebControls.Button btnSearch;
		protected System.Web.UI.WebControls.RadioButtonList listTipiTrasmissioni;
		protected System.Web.UI.WebControls.RadioButtonList listTipiOggetto;
		protected System.Web.UI.WebControls.TextBox txtMittente;
		protected WebControls.DateMask txtDataTrasmissioneFrom;
		protected WebControls.DateMask txtDataTrasmissioneTo;
		protected System.Web.UI.WebControls.Button btnSearchMittente;
		protected System.Web.UI.WebControls.Button btnSearchDestinatario;
		protected System.Web.UI.WebControls.Button btnSelectTipoTrasmissione;
		protected System.Web.UI.HtmlControls.HtmlGenericControl frameMittente;
		protected System.Web.UI.HtmlControls.HtmlGenericControl frameDestinatario;
		protected System.Web.UI.WebControls.CheckBox chkVisualizzaTrasmSottoposti;
		protected WebControls.DateMask txtDataAccRifFrom;
		protected WebControls.DateMask txtDataAccRifTo;
		protected WebControls.DateMask txtDataRispostaFrom;
		protected WebControls.DateMask txtDataRispostaTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblNoteIndividuali;
		protected System.Web.UI.WebControls.TextBox txtNoteIndividuali;
		protected System.Web.UI.WebControls.TextBox txtNoteGenerali;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerNoteIndividuali;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblNoteGenerali;
		protected System.Web.UI.WebControls.DropDownList cboRagioniTrasmissione;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerRagioneTrasmissione;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblRagioniTrasmissione;
		protected System.Web.UI.WebControls.TextBox txtDestinatario;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblMittente;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDestinatario;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataTrasmissioneFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataTrasmissioneTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataAccRifFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataAccRifTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataRispostaFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataRispostaTo;
		protected System.Web.UI.WebControls.Button btnClearFilters;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{	
				if (!this.IsPostBack)
				{
					// Inizializzazione filtri
					this.InitializeFilters();

					// Ripristino dati di filtro
					this.RestoreFilterItems();

					// Inizializzazione controllo di validazione
					this.InitializeValidationControl();

					// Registrazione della classe necessaria 
					// per il rilascio delle risorse relative ai filtri fascicoli
					ResourceReleaser.Register(new TrasmissioniFilterResourceReleaser());
				}
			}
			catch (Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			// Impostazione visibilità campi UI
			this.SetFieldsVisibility();
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
			this.btnSelectTipoTrasmissione.Click += new System.EventHandler(this.btnSelectTipoTrasmissione_Click);
			this.btnSearchMittente.Click += new System.EventHandler(this.btnSearchMittente_Click);
			this.btnSearchDestinatario.Click += new System.EventHandler(this.btnSearchDestinatario_Click);
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			this.btnClearFilters.Click += new System.EventHandler(this.btnClearFilters_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		/// <summary>
		/// Inizializzazione dati maschera di ricerca
		/// </summary>
		private void InitializeFilters()
		{
			// Caricamento delle ragioni trasmissione
			this.FetchRagioniTrasmissione();

//			// Caricamento combo filtri generici
//			this.FetchGenericFilters(this.cboGenericFilters_1.Items);
//			this.FetchGenericFilters(this.cboGenericFilters_2.Items);
		}

		/// <summary>
		/// Caricamento delle ragioni trasmissione
		/// </summary>
		private void FetchRagioniTrasmissione()
		{
			SitoAccessibile.Ricerca.RicercaTrasmissioniHandler handler=new RicercaTrasmissioniHandler();
			RagioneTrasmissione[] ragioni=handler.GetRagioniTrasmissione();

			this.cboRagioniTrasmissione.Items.Clear();
			this.cboRagioniTrasmissione.Items.Add(this.GetEmptyElement());

			foreach (RagioneTrasmissione ragione in ragioni)
				this.cboRagioniTrasmissione.Items.Add(new ListItem(ragione.descrizione,ragione.systemId));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="items"></param>
		private void FetchGenericFilters(ListItemCollection items)
		{
			RicercaTrasmissioniHandler handler=new RicercaTrasmissioniHandler();
			
			items.Clear();
			items.Add(this.GetEmptyElement());

			foreach (string item in handler.GetFiltriTrasmissione())
				items.Add(new ListItem(item));
		}

		private ListItem GetEmptyElement()
		{
			return new ListItem(string.Empty,string.Empty);
		}

		/// <summary>
		/// Gestione validazione filtri immessi
		/// </summary>
		/// <returns></returns>
		private bool ValidateFilters()
		{
			ValidationContainer ctl=this.GetControlValidation();

			bool initValid,endValid,rangeValid;

			// Validazione data trasmissione
			Validator.ValidateDateRange(this.txtDataTrasmissioneFrom.Text,this.txtDataTrasmissioneTo.Text,out initValid,out endValid,out rangeValid);
			if (!initValid)
				ctl.AddControlErrorMessage(this.lblDataTrasmissioneFrom.ClientID,"Data trasmissione iniziale non valida");
			if (!endValid)
				ctl.AddControlErrorMessage(this.lblDataTrasmissioneTo.ClientID,"Data trasmissione finale non valida");
			if (!rangeValid)
				ctl.AddControlErrorMessage(this.lblDataTrasmissioneFrom.ClientID,"Data trasmissione iniziale maggiore di quella finale");

			// Validazione data accettazione/rifiuto
			Validator.ValidateDateRange(this.txtDataAccRifFrom.Text,this.txtDataAccRifTo.Text,out initValid,out endValid,out rangeValid);
			if (!initValid)
				ctl.AddControlErrorMessage(this.lblDataAccRifFrom.ClientID,"Data Acc./Rif. iniziale non valida");
			if (!endValid)
				ctl.AddControlErrorMessage(this.lblDataAccRifTo.ClientID,"Data Acc./Rif. finale non valida");
			if (!rangeValid)
				ctl.AddControlErrorMessage(this.lblDataAccRifFrom.ClientID,"Data Acc./Rif. iniziale maggiore di quella finale");

			// Validazione data risposta
			Validator.ValidateDateRange(this.txtDataRispostaFrom.Text,this.txtDataRispostaTo.Text,out initValid,out endValid,out rangeValid);
			if (!initValid)
				ctl.AddControlErrorMessage(this.lblDataRispostaFrom.ClientID,"Data risposta iniziale non valida");
			if (!endValid)
				ctl.AddControlErrorMessage(this.lblDataRispostaTo.ClientID,"Data risposta finale non valida");
			if (!rangeValid)
				ctl.AddControlErrorMessage(this.lblDataRispostaFrom.ClientID,"Data risposta iniziale maggiore di quella finale");

			return ctl.IsValid();
		}

		/// <summary>
		/// Avvio ricerca trasmissioni
		/// </summary>
		private void Search()
		{
			// Impostazione filtro corrente
			TrasmissioniFilterItem filterItem=this.GetTrasmissioniFilterItem();
			this.FetchTrasmissioniFilterItem(filterItem);
			
			if (this.ValidateFilters())
			{
				string url=EnvironmentContext.RootPath + "Ricerca/";

				if (this.GetTipoTrasmissione().Equals(TipiTrasmissioniEnum.Ricevute))
					url+="EsitoRicercaTrasmissioniRicevute.aspx";
				else
					url+="EsitoRicercaTrasmissioniEffettuate.aspx";

				Response.Redirect(url);
			}
		}

		/// <summary>
		/// Reperimento tipologia di filtro trasmissione corrente.
		/// Può essere: Ricevuta o Effettuata
		/// </summary>
		/// <returns></returns>
		private SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum GetTipoTrasmissione()
		{
			return (SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum)
				Enum.Parse(typeof(SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum),this.listTipiTrasmissioni.SelectedItem.Value,true);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private TrasmissioniFilterItem CreateTrasmissioniFilterItem()
		{
			TipiTrasmissioniEnum searchType=this.GetTipoTrasmissione();

			if (searchType.Equals(TipiTrasmissioniEnum.Effettuate))
				return new TrasmissioniEffettuateFilterItem();
			else 
				return new TrasmissioniRicevuteFilterItem();
		}

		/// <summary>
		/// Reperimento elemento di filtro trasmissioni
		/// </summary>
		/// <returns></returns>
		private TrasmissioniFilterItem GetTrasmissioniFilterItem()
		{
			if (RicercaTrasmissioniHandler.CurrentFilter==null)
				RicercaTrasmissioniHandler.CurrentFilter=this.CreateTrasmissioniFilterItem();

			return RicercaTrasmissioniHandler.CurrentFilter;
		}

		/// <summary>
		/// Caricamento oggetto contenente i dati di filtro delle trasmissioni
		/// </summary>
		/// <returns></returns>
		private void FetchTrasmissioniFilterItem(TrasmissioniFilterItem filterItem)
		{
			filterItem.TipoOggettoTrasmissione=(TipiOggettiTrasmissioniEnum) 
				Enum.Parse(typeof(TipiOggettiTrasmissioniEnum),this.listTipiOggetto.SelectedValue,true);

			filterItem.DataTrasmissione=this.GetRangeDateFilter("Data Trasmissione",this.txtDataTrasmissioneFrom.Text,this.txtDataTrasmissioneTo.Text);

			filterItem.DataAccRif=this.GetRangeDateFilter("Data Acc/Rif",this.txtDataAccRifFrom.Text,this.txtDataAccRifTo.Text);

			filterItem.DataRisposta=this.GetRangeDateFilter("Data Risposta",this.txtDataRispostaFrom.Text,this.txtDataRispostaTo.Text);

			filterItem.RagioneTrasmissione=this.cboRagioniTrasmissione.SelectedItem.Text;

			filterItem.NoteIndividuali=this.txtNoteIndividuali.Text;

			filterItem.NoteGenerali=this.txtNoteGenerali.Text;

//			// Caricamento filtri aggiuntivi
//			ArrayList additionalFilters=new ArrayList();
//			if (this.cboGenericFilters_1.SelectedItem.Value!=string.Empty)
//				additionalFilters.Add(new TrasmissioniAdditionalFilterItem(this.cboGenericFilters_1.SelectedItem.Value,this.txtGenericFilter_1.Text));
//			if (this.cboGenericFilters_2.SelectedItem.Value!=string.Empty)
//				additionalFilters.Add(new TrasmissioniAdditionalFilterItem(this.cboGenericFilters_2.SelectedItem.Value,this.txtGenericFilter_2.Text));
//			filterItem.AdditionalFilters=(TrasmissioniAdditionalFilterItem[]) additionalFilters.ToArray(typeof(TrasmissioniAdditionalFilterItem));

			// Caricamento filtri specifici per la tipologia di trasmissione selezionata
			if (filterItem.GetType().Equals(typeof(TrasmissioniRicevuteFilterItem)))
				this.FetchTrasmissioniRicevuteFilterItem((TrasmissioniRicevuteFilterItem) filterItem);
			else
				this.FetchTrasmissioniEffettuateFilterItem((TrasmissioniEffettuateFilterItem) filterItem);
		}

		/// <summary>
		/// Caricamento dati di filtro relativi alle trasmissioni ricevute
		/// </summary>
		/// <param name="filterItem"></param>
		private void FetchTrasmissioniRicevuteFilterItem(TrasmissioniRicevuteFilterItem filterItem)
		{
			if (this.txtMittente.Text.Equals(string.Empty) ||
				(filterItem.Mittente!=null && !filterItem.Mittente.descrizione.Equals(this.txtMittente.Text)))
				filterItem.Mittente=null;
		}

		/// <summary>
		/// Caricamento dati di filtro relativi alle trasmissioni effettuate
		/// </summary>
		/// <param name="filterItem"></param>
		private void FetchTrasmissioniEffettuateFilterItem(TrasmissioniEffettuateFilterItem filterItem)
		{
			if (this.txtDestinatario.Text.Equals(string.Empty) ||
				(filterItem.Destinatario!=null && !filterItem.Destinatario.descrizione.Equals(this.txtDestinatario.Text)))
				filterItem.Destinatario=null;

			filterItem.VisualizzaTrasmissioniSottoposti=this.chkVisualizzaTrasmSottoposti.Checked;
		}

		/// <summary>
		/// Ripristiono dati di filtro nella UI relativamente
		/// ai campi di ricerca delle trasmissioni ricevute 
		/// </summary>
		private void RestoreFilterItemsRicevute()
		{
			TrasmissioniRicevuteFilterItem trasmRicevuteFilterItem=(TrasmissioniRicevuteFilterItem) this.GetTrasmissioniFilterItem();

			// Ripristino filtri relativi alle trasmissioni ricevute
			this.listTipiTrasmissioni.SelectedValue=TipiTrasmissioniEnum.Ricevute.ToString();

			// Reperimento del mittente selezionato da rubrica
			if (this.OnSelectCorrispondente())
				trasmRicevuteFilterItem.Mittente=this.GetCorrispondenteRubrica();

			// Impostazione mittente
			if (trasmRicevuteFilterItem.Mittente!=null)
				this.txtMittente.Text=trasmRicevuteFilterItem.Mittente.descrizione;
		}

		/// <summary>
		/// Ripristiono dati di filtro nella UI relativamente
		/// ai campi di ricerca delle trasmissioni effettuate
		/// </summary>
		private void RestoreFilterItemsEffettuate()
		{
			TrasmissioniEffettuateFilterItem trasmEffettuateFilterItem=(TrasmissioniEffettuateFilterItem) this.GetTrasmissioniFilterItem();

			// Ripristino filtri relativi alle trasmissioni effettuate
			this.listTipiTrasmissioni.SelectedValue=TipiTrasmissioniEnum.Effettuate.ToString();

			// Reperimento del mittente selezionato da rubrica
			if (this.OnSelectCorrispondente())
				trasmEffettuateFilterItem.Destinatario=this.GetCorrispondenteRubrica();

			// Impostazione destinatario
			if (trasmEffettuateFilterItem.Destinatario!=null)
				this.txtDestinatario.Text=trasmEffettuateFilterItem.Destinatario.descrizione;

			// Impostazione check visualizza trasmissioni sottoposti
			this.chkVisualizzaTrasmSottoposti.Checked=trasmEffettuateFilterItem.VisualizzaTrasmissioniSottoposti;
		}

		/// <summary>
		/// Ripristino elementi di filtro trasmissioni nella UI
		/// </summary>
		private void RestoreFilterItems()
		{
			TrasmissioniFilterItem filterItem=this.GetTrasmissioniFilterItem();

			bool trasmissioniRicevute=filterItem.GetType().Equals(typeof(TrasmissioniRicevuteFilterItem));

			this.listTipiOggetto.SelectedValue=filterItem.TipoOggettoTrasmissione.ToString();

			this.RestoreRangeDateFilter(filterItem.DataTrasmissione,this.txtDataTrasmissioneFrom,this.txtDataTrasmissioneTo);

			this.RestoreRangeDateFilter(filterItem.DataAccRif,this.txtDataAccRifFrom,this.txtDataAccRifTo);

			this.RestoreRangeDateFilter(filterItem.DataRisposta,this.txtDataRispostaFrom,this.txtDataRispostaTo);

			if (filterItem.RagioneTrasmissione!=null && filterItem.RagioneTrasmissione!=string.Empty)
			{
				foreach (ListItem item in this.cboRagioniTrasmissione.Items)
				{
					if (item.Text==filterItem.RagioneTrasmissione)
					{
						item.Selected=true;
						break;
					}
				}
			}

			this.txtNoteIndividuali.Text=filterItem.NoteIndividuali;

			this.txtNoteGenerali.Text=filterItem.NoteGenerali;

//			for (int i=0;i<filterItem.AdditionalFilters.Length;i++)
//			{
//				TrasmissioniAdditionalFilterItem additionalFilter=filterItem.AdditionalFilters[i];
//
//				DropDownList dropDown=(DropDownList) this.FindControl("cboGenericFilters_" + (i + 1).ToString());
//				TextBox textBox=(TextBox) this.FindControl("txtGenericFilter_" + (i + 1).ToString());
//
//				dropDown.SelectedValue=additionalFilter.FilterName;
//				textBox.Text=additionalFilter.FilterValue;
//			}

			// Ripristino campi specifici per la tipologia di filtro corrente
			if (trasmissioniRicevute)
				this.RestoreFilterItemsRicevute();
			else
				this.RestoreFilterItemsEffettuate();
		}

		/// <summary>
		/// Reperimento valore filtro per intervallo di date
		/// </summary>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		/// <returns></returns>
		private RangeDateFilter GetRangeDateFilter(string fieldName,string fromDate,string toDate)
		{
			RangeDateFilter retValue=null;

			if (fromDate!=string.Empty)
				retValue=RangeDateFilter.Create(fieldName,fromDate,toDate);

			return retValue;
		}

		/// <summary>
		/// Gestione selezione tipologia di filtro trasmissione
		/// </summary>
		private void PerformSelectionTipoTrasmissione()
		{	
			if (RicercaTrasmissioniHandler.CurrentFilter!=null)
				RicercaTrasmissioniHandler.CurrentFilter=null;
			
			RicercaTrasmissioniHandler.CurrentFilter=this.CreateTrasmissioniFilterItem();;
		}

		/// <summary>
		/// Impostazione visibilità campi UI
		/// </summary>
		private void SetFieldsVisibility()
		{
			this.frameMittente.Visible=false;
			this.frameDestinatario.Visible=false;
			this.chkVisualizzaTrasmSottoposti.Visible=false;

			bool trasmRicevute=this.GetTipoTrasmissione().Equals(TipiTrasmissioniEnum.Ricevute);
			
			this.frameMittente.Visible=trasmRicevute;
			this.frameDestinatario.Visible=!trasmRicevute;
			this.chkVisualizzaTrasmSottoposti.Visible=!trasmRicevute;
		}

		/// <summary>
		/// Ripristino elementi di filtro per data
		/// </summary>
		/// <param name="rangeFilter"></param>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		private void RestoreRangeDateFilter(RangeDateFilter rangeFilter,WebControls.DateMask fromDate,WebControls.DateMask toDate)
		{
			if (rangeFilter!=null)
			{
				fromDate.Text=rangeFilter.InitDateString;

				if (!rangeFilter.SearchInitDate)
					toDate.Text=rangeFilter.EndDateString;
			}
		}

		private void btnSelectTipoTrasmissione_Click(object sender, System.EventArgs e)
		{
			// Azione di selezione tipologia di filtro
			this.PerformSelectionTipoTrasmissione();
		}

		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			this.Search();
		}

		#region Gestione rubrica

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool OnSelectCorrispondente()
		{
			return (Request.UrlReferrer.AbsoluteUri.IndexOf("Rubrica.aspx")>-1 && Rubrica.Rubrica.AddressbookResult!=null);
		}

		/// <summary>
		/// Reperimento del corrispondente relativo alla collocazione fisica del documento
		/// </summary>
		/// <returns></returns>
		private Corrispondente GetCorrispondenteRubrica()
		{
			Corrispondente retValue=null;

			SitoAccessibile.Rubrica.Rubrica.AddressbookContainer results=null;
			
			if (Rubrica.Rubrica.AddressbookResult!=null)
			{
				results=Rubrica.Rubrica.AddressbookResult;
				Rubrica.Rubrica.AddressbookResult=null;
			}

			if (results!=null && results.GlobalRecipient!=null && results.GlobalRecipient.Length>0)
				retValue=results.GlobalRecipient[0];

			return retValue;
		}

		#region Gestione impostazione / reperimento mittente

		private void ShowRubricaMittente()
		{
			string url=EnvironmentContext.RootPath + "Rubrica/Rubrica.aspx?action=new&pgcall=DocsPAWA.SitoAccessibile.Ricerca.Trasmissioni&field=pe_mitt_inter&urpl=urp&ie=i&capacity=one";
			Response.Redirect(url);
		}

		private void btnSearchMittente_Click(object sender, System.EventArgs e)
		{
			this.ShowRubricaMittente();
		}

		#endregion

		#region Gestione impostazione / reperimento destinatario

		private void ShowRubricaDestinatario()
		{
			string url=EnvironmentContext.RootPath + "Rubrica/Rubrica.aspx?action=new&pgcall=DocsPAWA.SitoAccessibile.Ricerca.Trasmissioni&field=pe_mitt_inter&urpl=urp&ie=i&capacity=one";
			Response.Redirect(url);
		}

		private void btnSearchDestinatario_Click(object sender, System.EventArgs e)
		{
			this.ShowRubricaDestinatario();
		}

		#endregion

		#endregion

		/// <summary>
		/// Rimozione filtro correntemente attivo
		/// </summary>
		private void DisposeCurrentFilter()
		{
			RicercaFascicoliHandler.CurrentFilter=null;
		}

		/// <summary>
		/// Rimozione filtri dai campi della UI
		/// </summary>
		private void ClearMask()
		{
			this.listTipiTrasmissioni.SelectedValue=TipiTrasmissioniEnum.Ricevute.ToString();
			this.listTipiOggetto.SelectedValue=TipiOggettiTrasmissioniEnum.Tutti.ToString();
			this.txtMittente.Text=string.Empty;
			this.txtDestinatario.Text=string.Empty;
			this.txtDataTrasmissioneFrom.Text=string.Empty;
			this.txtDataTrasmissioneTo.Text=string.Empty;
			this.txtDataAccRifFrom.Text=string.Empty;
			this.txtDataAccRifTo.Text=string.Empty;
			this.txtDataRispostaFrom.Text=string.Empty;
			this.txtDataRispostaTo.Text=string.Empty;
			this.cboRagioniTrasmissione.SelectedValue=string.Empty;
			this.txtNoteIndividuali.Text=string.Empty;
			this.txtNoteGenerali.Text=string.Empty;
//			this.cboGenericFilters_1.SelectedValue=string.Empty;
//			this.txtGenericFilter_1.Text=string.Empty;
//			this.cboGenericFilters_2.SelectedValue=string.Empty;
//			this.txtGenericFilter_2.Text=string.Empty;
			this.chkVisualizzaTrasmSottoposti.Checked=false;
		}
		
		private void btnClearFilters_Click(object sender, System.EventArgs e)
		{
			this.DisposeCurrentFilter();

			this.ClearMask();
		}

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
	}
}