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
using DocsPAWA.SitoAccessibile.Registri;
using DocsPAWA.SitoAccessibile.Fascicoli;
using DocsPAWA.SitoAccessibile.Validations;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Gestione ricerca fascicoli
	/// </summary>
	public class Fascicoli : SessionWebPage,IDisposable
	{
		protected System.Web.UI.WebControls.Button btnSearch;
		protected System.Web.UI.WebControls.DropDownList cboRegistri;
		protected System.Web.UI.WebControls.TextBox txtCodiceTitolario;
		protected WebControls.DateMask txtDataAperturaFrom;
		protected WebControls.DateMask txtDataAperturaTo;
		protected WebControls.DateMask txtDataChiusuraFrom;
		protected WebControls.DateMask txtDataChiusuraTo;
		protected WebControls.DateMask txtDataCreazioneFrom;
		protected WebControls.DateMask txtDataCreazioneTo;
		protected System.Web.UI.WebControls.TextBox txtNumero;
		protected System.Web.UI.WebControls.TextBox txtAnno;
		protected System.Web.UI.WebControls.DropDownList cboStato;
		protected System.Web.UI.WebControls.DropDownList cboTipo;
		protected System.Web.UI.WebControls.TextBox txtDescrizione;
		protected WebControls.DateMask txtDataCollocazioneFrom;
		protected WebControls.DateMask txtDataCollocazioneTo;
		protected System.Web.UI.WebControls.TextBox txtCollocazioneFisica;
		protected System.Web.UI.WebControls.Button btnShowRubrica;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblCodiceTitolario;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblAnno;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblNumero;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataAperturaFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataAperturaTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataCollocazioneFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataCollocazioneTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataChiusuraFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataChiusuraTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataCreazioneFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataCreazioneTo;
		protected System.Web.UI.WebControls.Button btnClearFilters;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (!this.IsPostBack)
				{
					this.InitializeValidationControl();

					// Inizializzazione dati maschera di ricerca
					this.InitializeFilters();

					if (this.OnSelectCorrispondente())
						this.GetFascicoliFilterItem().CorrispondenteLocazioneFisica=this.GetCorrispondenteRubrica();

					// Ripristino elementi di filtro dalla sessione
					this.RestoreFilterItems();

					// Registrazione della classe necessaria 
					// per il rilascio delle risorse relative ai filtri fascicoli
					ResourceReleaser.Register(new FascicoliFilterResourceReleaser());
				}
			}
			catch(Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
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
			this.btnShowRubrica.Click += new System.EventHandler(this.btnShowRubrica_Click);
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			this.btnClearFilters.Click += new System.EventHandler(this.btnClearFilters_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Inizializzazione dati maschera di ricerca
		/// </summary>
		private void InitializeFilters()
		{
			this.FetchRegistri();

			this.FetchStatiFascicolo();

			this.FetchTipiFascicolo();
		}

		/// <summary>
		/// Caricamento registri disponibili
		/// </summary>
		private void FetchRegistri()
		{
			RegistroHandler registroHandler=new RegistroHandler();
			Registro[] registri=registroHandler.GetRegistri();
			
			this.cboRegistri.Items.Clear();
			if (registri.Length==0)
				this.cboRegistri.Items.Add(this.CreateEmptyListItem());
			else
				foreach (Registro registro in registri)
					this.cboRegistri.Items.Add(new ListItem(registro.codRegistro,registro.systemId));
		}

		/// <summary>
		/// Caricamento stati del fascicolo
		/// </summary>
		private void FetchStatiFascicolo()
		{
			this.cboStato.Items.Clear();
			this.cboStato.Items.Add(this.CreateEmptyListItem());
			this.cboStato.Items.Add(new ListItem("Aperto",StatiFascicolo.APERTO));
			this.cboStato.Items.Add(new ListItem("Chiuso",StatiFascicolo.CHIUSO));
		}

		/// <summary>
		/// Caricamento tipi del fascicolo
		/// </summary>
		private void FetchTipiFascicolo()
		{
			this.cboTipo.Items.Clear();
			this.cboTipo.Items.Add(this.CreateEmptyListItem());
			this.cboTipo.Items.Add(new ListItem("Generale",TipiFascicolo.GENERALE));
			this.cboTipo.Items.Add(new ListItem("Procedimentale",TipiFascicolo.PROCEDIMENTALE));
		}

		/// <summary>
		/// Creazione elemento vuoto
		/// </summary>
		/// <returns></returns>
		private ListItem CreateEmptyListItem()
		{
			return new ListItem(string.Empty,string.Empty);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsValidClassifica()
        {
            bool isValid = false;

            if (!string.IsNullOrEmpty(this.txtCodiceTitolario.Text))
            {
                Utente user = UserManager.getUtente();
                InfoUtente infoUtente = UserManager.getInfoUtente();

                Registro registro = null;
                foreach (Registro item in UserManager.getRuolo().registri)
                {
                    if (item.systemId.Equals(this.cboRegistri.SelectedValue))
                    {
                        registro = item;
                        break;
                    }
                }

                FascicolazioneClassificazione classificazione = null;

                if (this.txtCodiceTitolario.Text != string.Empty)
                {
                    SitoAccessibile.Documenti.Classificazioni.ClassificaHandler classificaHandler = new SitoAccessibile.Documenti.Classificazioni.ClassificaHandler();

                    isValid = (classificaHandler.GetClassificazione(this.txtCodiceTitolario.Text, registro) != null);

                }
            }
            else
                isValid = true;

            return isValid;
        }

		/// <summary>
		/// Gestione validazione filtri immessi
		/// </summary>
		/// <returns></returns>
		private bool ValidateFilters()
		{
			ValidationContainer ctl=this.GetControlValidation();

            if (!this.IsValidClassifica())
                ctl.AddControlErrorMessage(this.lblAnno.ClientID, "Codice classifica non trovato");

			if (!Validator.ValidateNumeric(this.txtAnno.Text))
				ctl.AddControlErrorMessage(this.lblAnno.ClientID,"Anno fascicolo non valido");

			if (!Validator.ValidateNumeric(this.txtNumero.Text))
				ctl.AddControlErrorMessage(this.lblNumero.ClientID,"Numero fascicolo non valido");

			bool initValid,endValid,rangeValid;

			// Validazione data chiusura
			Validator.ValidateDateRange(this.txtDataChiusuraFrom.Text,this.txtDataChiusuraTo.Text,out initValid,out endValid,out rangeValid);
			if (!initValid)
				ctl.AddControlErrorMessage(this.lblDataChiusuraFrom.ClientID,"Data chiusura iniziale non valida");
			if (!endValid)
				ctl.AddControlErrorMessage(this.lblDataChiusuraTo.ClientID,"Data chiusura finale non valida");
			if (!rangeValid)
				ctl.AddControlErrorMessage(this.lblDataChiusuraFrom.ClientID,"Data chiusura iniziale maggiore di quella finale");

			// Validazione data collocazione
			Validator.ValidateDateRange(this.txtDataCollocazioneFrom.Text,this.txtDataCollocazioneTo.Text,out initValid,out endValid,out rangeValid);
			if (!initValid)
				ctl.AddControlErrorMessage(this.lblDataCollocazioneFrom.ClientID,"Data collocazione iniziale non valida");
			if (!endValid)
				ctl.AddControlErrorMessage(this.lblDataCollocazioneTo.ClientID,"Data collocazione finale non valida");
			if (!rangeValid)
				ctl.AddControlErrorMessage(this.lblDataCollocazioneFrom.ClientID,"Data collocazione iniziale maggiore di quella finale");

			// Validazione data apertura
			Validator.ValidateDateRange(this.txtDataAperturaFrom.Text,this.txtDataAperturaTo.Text,out initValid,out endValid,out rangeValid);
			if (!initValid)
				ctl.AddControlErrorMessage(this.lblDataAperturaFrom.ClientID,"Data apertura iniziale non valida");
			if (!endValid)
				ctl.AddControlErrorMessage(this.lblDataAperturaTo.ClientID,"Data apertura finale non valida");
			if (!rangeValid)
				ctl.AddControlErrorMessage(this.lblDataAperturaFrom.ClientID,"Data apertura iniziale maggiore di quella finale");

			// Validazione data creazione
			Validator.ValidateDateRange(this.txtDataCreazioneFrom.Text,this.txtDataCreazioneTo.Text,out initValid,out endValid,out rangeValid);
			if (!initValid)
				ctl.AddControlErrorMessage(this.lblDataCreazioneFrom.ClientID,"Data creazione iniziale non valida");
			if (!endValid)
				ctl.AddControlErrorMessage(this.lblDataCreazioneTo.ClientID,"Data creazione finale non valida");
			if (!rangeValid)
				ctl.AddControlErrorMessage(this.lblDataCreazioneFrom.ClientID,"Data creazione iniziale maggiore di quella finale");

			return ctl.IsValid();
		}

		/// <summary>
		/// Avvio ricerca fascicoli in base ai dati immessi dall'utente
		/// </summary>
		private void Search()
		{
			if (this.ValidateFilters())
			{
				// Impostazione filtro corrente
				FascicoliFilterItem filterItem=this.GetFascicoliFilterItem();
				this.FetchFascicoliFilterItem(filterItem);

				Response.Redirect(EnvironmentContext.RootPath + "Ricerca/EsitoRicercaFascicoli.aspx");
			}
		}

		/// <summary>
		/// Rimozione filtro correntemente attivo
		/// </summary>
		private void DisposeCurrentFilter()
		{
			RicercaFascicoliHandler.CurrentFilter=null;
		}

		/// <summary>
		/// Rimozione filtri
		/// </summary>
		private void ClearMask()
		{
			this.txtCodiceTitolario.Text=string.Empty;
			this.cboRegistri.SelectedIndex=0;
			
			this.txtDataAperturaFrom.Text=string.Empty;
			this.txtDataAperturaTo.Text=string.Empty;

			this.txtDataChiusuraFrom.Text=string.Empty;
			this.txtDataChiusuraTo.Text=string.Empty;

			this.txtDataCreazioneFrom.Text=string.Empty;
			this.txtDataCreazioneTo.Text=string.Empty;

			this.txtNumero.Text=string.Empty;
			this.txtAnno.Text=string.Empty;

			this.cboStato.SelectedIndex=0;
			this.cboTipo.SelectedIndex=0;

			this.txtDescrizione.Text=string.Empty;

			this.txtDataCollocazioneFrom.Text=string.Empty;
			this.txtDataCollocazioneTo.Text=string.Empty;
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
		/// Reperimento valore filtro numero
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private int GetNumericFilter(string data)
		{
			int retValue=0;

			if (data!=string.Empty)
			{
				try
				{
					retValue=Convert.ToInt32(data);
				}
				catch
				{
				}
			}

			return retValue;
		}

		/// <summary>
		/// Reperimento oggetto elemento di filtro fascicoli
		/// </summary>
		/// <returns></returns>
		private FascicoliFilterItem GetFascicoliFilterItem()
		{
			if (RicercaFascicoliHandler.CurrentFilter==null)
				RicercaFascicoliHandler.CurrentFilter=new FascicoliFilterItem();

			return RicercaFascicoliHandler.CurrentFilter;
		}

		/// <summary>
		/// Creazione oggetto contenente i dati di filtro dei fascicoli
		/// </summary>
		/// <returns></returns>
		private void FetchFascicoliFilterItem(FascicoliFilterItem filterItem)
		{
			if (this.txtCollocazioneFisica.Text==string.Empty ||
				(filterItem.CorrispondenteLocazioneFisica!=null && !filterItem.CorrispondenteLocazioneFisica.descrizione.Equals(this.txtCollocazioneFisica.Text)))
				filterItem.CorrispondenteLocazioneFisica=null;

			filterItem.CodiceNodoTitolario=this.txtCodiceTitolario.Text;
			filterItem.IDRegistro=this.cboRegistri.SelectedItem.Value;

			filterItem.DataApertura=this.GetRangeDateFilter("Data apertura",this.txtDataAperturaFrom.Text,this.txtDataAperturaTo.Text);
			filterItem.DataChiusura=this.GetRangeDateFilter("Data chiusura",this.txtDataChiusuraFrom.Text,this.txtDataChiusuraTo.Text);
			filterItem.DataCreazione=this.GetRangeDateFilter("Data creazione",this.txtDataCreazioneFrom.Text,this.txtDataCreazioneTo.Text);

			filterItem.Numero=this.GetNumericFilter(this.txtNumero.Text);
			filterItem.Anno=this.GetNumericFilter(this.txtAnno.Text);
			
			filterItem.Stato=this.cboStato.SelectedItem.Value;
			filterItem.Tipo=this.cboTipo.SelectedItem.Value;
			filterItem.Descrizione=this.txtDescrizione.Text;

			filterItem.DataCollocazione=this.GetRangeDateFilter("Data collocazione",this.txtDataCollocazioneFrom.Text,this.txtDataCollocazioneTo.Text);
		}

		/// <summary>
		/// Ripristino elementi di filtro in sessione nei campi della UI
		/// </summary>
		private void RestoreFilterItems()
		{
			FascicoliFilterItem filterItem=this.GetFascicoliFilterItem();

			this.txtCodiceTitolario.Text=filterItem.CodiceNodoTitolario;
			
			if (filterItem.IDRegistro!=string.Empty)
				this.cboRegistri.SelectedValue=filterItem.IDRegistro;

			this.cboStato.SelectedValue=filterItem.Stato;

			this.cboTipo.SelectedValue=filterItem.Tipo;
			
			if (filterItem.Numero>0)
				this.txtNumero.Text=filterItem.Numero.ToString();	
			
			if (filterItem.Anno>0)
				this.txtAnno.Text=filterItem.Anno.ToString();

			this.RestoreRangeDateFilter(filterItem.DataApertura,this.txtDataAperturaFrom,this.txtDataAperturaTo);

			this.RestoreRangeDateFilter(filterItem.DataChiusura,this.txtDataChiusuraFrom,this.txtDataChiusuraTo);

			this.RestoreRangeDateFilter(filterItem.DataCreazione,this.txtDataCreazioneFrom,this.txtDataCreazioneTo);

			this.RestoreRangeDateFilter(filterItem.DataCollocazione,this.txtDataCollocazioneFrom,this.txtDataCollocazioneTo);
			
			this.txtDescrizione.Text=filterItem.Descrizione;

			if (filterItem.CorrispondenteLocazioneFisica!=null)
				this.txtCollocazioneFisica.Text=filterItem.CorrispondenteLocazioneFisica.descrizione;
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

		/// <summary>
		/// Visualizzazione rubrica per la selezione di un corrispondente
		/// </summary>
		private void ShowRubricaCollocazioneFisica()
		{
			string url=EnvironmentContext.RootPath + "Rubrica/Rubrica.aspx?action=new&pgcall=DocsPAWA.SitoAccessibile.Ricerca.Fascicoli&field=pe_mitt_inter&urpl=u&ie=i&capacity=one";
			Response.Redirect(url);
		}

		/// <summary>
		/// Verifica se si è in fase di selezione di un corrispondente da rubrica
		/// </summary>
		/// <returns></returns>
		protected virtual bool OnSelectCorrispondente()
		{
			return (Request.UrlReferrer.AbsoluteUri.IndexOf("Rubrica.aspx")>-1 && Rubrica.Rubrica.AddressbookResult!=null);
		}

		/// <summary>
		/// Reperimento del corrispondente selezionato dalla rubrica
		/// </summary>
		/// <returns></returns>
		protected virtual Corrispondente GetCorrispondenteRubrica()
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

		private void btnShowRubrica_Click(object sender, System.EventArgs e)
		{
			this.ShowRubricaCollocazioneFisica();
		}

		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			this.Search();
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