namespace DocsPAWA.SitoAccessibile.Ricerca
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DocsPAWA.DocsPaWR;
	using SitoAccessibile.Validations;

	/// <summary>
	///	UserControl (classe base astratta) per la gestione dei controlli grafici
	///	relativi ai filtri di ricerca dei documenti
	/// </summary>
	public abstract class BaseFiltriRicercaDocumenti : System.Web.UI.UserControl
	{
		private SearchProperties _searchProperties=null;
		private ValidationContainer _validationContainer=null;

		protected override void OnLoad(EventArgs e)
		{
			// Associazione label ai campi della UI
			this.BindLabelsToFields();

			base.OnLoad (e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.SetFieldsVisibility();

			base.OnPreRender (e);
		}


		/// <summary>
		/// Filtro di ricerca documenti corrente
		/// </summary>
		protected SearchProperties SearchProperties
		{
			get
			{
				if (this._searchProperties==null)
					this._searchProperties=RicercaDocumentiHandler.CurrentFilter;

				return this._searchProperties;
			}
		}

		/// <summary>
		/// Reperimento controllo validazione
		/// </summary>
		protected ValidationContainer ValidationContainerControl
		{
			get
			{
				return this._validationContainer;
			}
		}

		/// <summary>
		/// Inizializzazione maschera di ricerca
		/// </summary>
		/// <param name="validationContainer"></param>
		public void Initialize(ValidationContainer validationContainer)
		{
			this._validationContainer=validationContainer;
			if (this._validationContainer==null)
				throw new ApplicationException("Parametro 'validationContainer' non fornito");
		}

		/// <summary>
		/// Caricamento dati maschera di filtri avanzati
		/// </summary>
		public abstract void LoadData();

		/// <summary>
		/// Aggiornamento dati di filtro dai campi della UI
		/// </summary>
		public abstract void RefreshSearchProperties();

		/// <summary>
		/// Validazione filtri immessi
		/// </summary>
		public abstract bool ValidateFilters();

		/// <summary>
		/// Impostazione visibilità campi UI
		/// </summary>
		protected abstract void SetFieldsVisibility();

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

		/// <summary>
		/// Associazione attributo for per i campi label richiesti
		/// </summary>
		protected virtual void BindLabelsToFields()
		{
		}

		/// <summary>
		/// Associazione attributo "for"
		/// </summary>
		/// <param name="labelControl"></param>
		/// <param name="controlToBind"></param>
		protected virtual void BindLabelToField(HtmlGenericControl labelControl,System.Web.UI.Control controlToBind)
		{
			labelControl.Attributes.Add("for",controlToBind.ClientID);
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
		}
		#endregion
	}
}
