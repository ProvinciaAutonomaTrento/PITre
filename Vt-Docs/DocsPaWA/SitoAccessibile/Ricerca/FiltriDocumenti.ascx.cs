namespace DocsPAWA.SitoAccessibile.Ricerca
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DocsPAWA.DocsPaWR;
	using SitoAccessibile.Validations;

	/// <summary>
	///	UserControl per la gestione dei filtri di base dei documenti
	/// </summary>
	public class FiltriDocumenti : BaseFiltriRicercaDocumenti
	{
		protected WebControls.DateMask txtDataProtocolloFrom;
		protected WebControls.DateMask txtDataProtocolloTo;
		protected System.Web.UI.WebControls.TextBox txtNumProtocolloFrom;
		protected System.Web.UI.WebControls.TextBox txtNumProtocolloTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl frameDataProtocollo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataProtocolloFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataProtocolloTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl frameNumeroProtocollo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblNumProtocolloFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblNumProtocolloTo;
		protected System.Web.UI.WebControls.TextBox txtOggetto;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblOggetto;
		protected System.Web.UI.WebControls.TextBox txtMittenteDestinatario;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblMittenteDestinatario;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlMittenteDestinatario;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataCreazioneFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataCreazioneTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl frameDataCreazione;
		protected System.Web.UI.HtmlControls.HtmlGenericControl frameIdDocumento;
		protected WebControls.DateMask txtDataCreazioneFrom;
		protected WebControls.DateMask txtDataCreazioneTo;
		protected System.Web.UI.WebControls.TextBox txtIdDocumentoFrom;
		protected System.Web.UI.WebControls.TextBox txtIdDocumentoTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblIdDocumentoFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblIdDocumentoTo;
		protected System.Web.UI.WebControls.ListBox lstRegistri;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblRegistro;
		protected System.Web.UI.WebControls.DropDownList cboTipologiaDocumento;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblTipologiaDocumento;
		protected System.Web.UI.WebControls.Button btnShowRubricaMittDest;
		protected System.Web.UI.WebControls.Button btnSelectAllRegistri;
		protected System.Web.UI.WebControls.Button btnUnSelectAllRegistri;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblAnnoProtocollo;
		protected System.Web.UI.WebControls.TextBox txtAnnoProtocollo;

		private void Page_Load(object sender, System.EventArgs e)
		{
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
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
			this.btnShowRubricaMittDest.Click += new System.EventHandler(this.btnShowRubricaMittDest_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);
			this.btnSelectAllRegistri.Click += new EventHandler(this.btnSelectAllRegistri_Click);
			this.btnUnSelectAllRegistri.Click += new EventHandler(this.btnUnSelectAllRegistri_Click);

		}
		#endregion

		protected override void BindLabelsToFields()
		{
			this.BindLabelToField(this.lblDataProtocolloFrom,this.txtDataProtocolloFrom);
			this.BindLabelToField(this.lblDataProtocolloTo,this.txtDataProtocolloTo);
			this.BindLabelToField(this.lblAnnoProtocollo,this.txtAnnoProtocollo);
			this.BindLabelToField(this.lblNumProtocolloFrom,this.txtNumProtocolloFrom);
			this.BindLabelToField(this.lblNumProtocolloTo,this.txtNumProtocolloTo);
			this.BindLabelToField(this.lblOggetto,this.txtOggetto);
			this.BindLabelToField(this.lblMittenteDestinatario,this.txtMittenteDestinatario);
			this.BindLabelToField(this.lblDataCreazioneFrom,this.txtDataCreazioneFrom);
			this.BindLabelToField(this.lblDataCreazioneTo,this.txtDataCreazioneTo);
			this.BindLabelToField(this.lblIdDocumentoFrom,this.txtIdDocumentoFrom);
			this.BindLabelToField(this.lblIdDocumentoTo,this.txtIdDocumentoTo);
			this.BindLabelToField(this.lblTipologiaDocumento,this.cboTipologiaDocumento);
		}

		/// <summary>
		/// Caricamento dati maschera di filtri avanzati
		/// </summary>
		public override void LoadData()
		{
			if (!this.IsPostBack)
			{
				this.FillRegistri();

				this.FillTipologieDocumenti();
			}

			this.txtDataProtocolloFrom.Text=this.SearchProperties.Protocollo.DataProtocolloDa;
			this.txtDataProtocolloTo.Text=this.SearchProperties.Protocollo.DataProtocolloA;
			this.txtAnnoProtocollo.Text=this.SearchProperties.Protocollo.AnnoProtocollo;
			this.txtNumProtocolloFrom.Text=this.SearchProperties.Protocollo.NumProtocolloDa;
			this.txtNumProtocolloTo.Text=this.SearchProperties.Protocollo.NumProtocolloA;
			this.txtOggetto.Text=this.SearchProperties.Documento.Oggetto;

			if (this.OnSelectCorrispondente())
				this.SetCorrispondenteMittenteDestinatario();

			if (this.SearchProperties.Protocollo.Corrispondente!=null)
				this.txtMittenteDestinatario.Text=this.SearchProperties.Protocollo.Corrispondente.descrizione;

			this.txtDataCreazioneFrom.Text=this.SearchProperties.Documento.DataDocumentoDa;
			this.txtDataCreazioneTo.Text=this.SearchProperties.Documento.DataDocumentoA;

			this.txtIdDocumentoFrom.Text=this.SearchProperties.Documento.IdDocumentoDa;
			this.txtIdDocumentoTo.Text=this.SearchProperties.Documento.IdDocumentoA;

			foreach (SearchRegistry registro in this.SearchProperties.Protocollo.Registri)
			{
				ListItem item=this.lstRegistri.Items.FindByValue(registro.Id);

				if (item!=null)
					item.Selected=registro.Selezionato;
			}

			this.cboTipologiaDocumento.SelectedValue=this.SearchProperties.Documento.Tipologia;
		}

		/// <summary>
		/// Impostazione visibilità campi UI
		/// </summary>
		protected override void SetFieldsVisibility()
		{
			this.frameDataProtocollo.Visible=(this.SearchProperties.ProtocolliArrivo || this.SearchProperties.ProtocolliPartenza);
			this.frameNumeroProtocollo.Visible=this.frameDataProtocollo.Visible;

			this.pnlMittenteDestinatario.Visible=(this.SearchProperties.ProtocolliArrivo || this.SearchProperties.ProtocolliPartenza);

			this.frameDataCreazione.Visible=this.SearchProperties.DocumentiGrigi;
			this.frameIdDocumento.Visible=this.frameDataCreazione.Visible;
		}

		/// <summary>
		/// Aggiornamento dati di filtro dai campi della UI
		/// </summary>
		public override void RefreshSearchProperties()
		{
			this.SearchProperties.Documento.Oggetto = this.txtOggetto.Text;
			this.SearchProperties.Documento.Tipologia = this.cboTipologiaDocumento.SelectedItem.Value;
			
			if (this.SearchProperties.DocumentiGrigi)
			{
				// Filtri relativi ai documenti grigi
				this.SearchProperties.Documento.DataDocumentoDa = this.txtDataCreazioneFrom.Text;
				this.SearchProperties.Documento.DataDocumentoA = this.txtDataCreazioneTo.Text;
				this.SearchProperties.Documento.IdDocumentoDa = this.txtIdDocumentoFrom.Text;
				this.SearchProperties.Documento.IdDocumentoA = this.txtIdDocumentoTo.Text;
			}
			
			if (this.SearchProperties.ProtocolliArrivo ||
				this.SearchProperties.ProtocolliInterni ||
				this.SearchProperties.ProtocolliPartenza)
			{ 
				// Filtri relativi ai protocolli
				if (this.txtMittenteDestinatario.Text==string.Empty)
				{
					this.SearchProperties.Protocollo.Corrispondente=new Corrispondente();
				}
				else if (!this.txtMittenteDestinatario.Text.Equals(this.SearchProperties.Protocollo.Corrispondente.descrizione))
				{
					this.SearchProperties.Protocollo.Corrispondente=new Corrispondente();
					this.SearchProperties.Protocollo.Corrispondente.descrizione=this.txtMittenteDestinatario.Text;
				}

				foreach (ListItem item in this.lstRegistri.Items)
					this.SearchProperties.Protocollo.SelectRegistryWithId(item.Value,item.Selected);

				this.SearchProperties.Protocollo.DataProtocolloDa =this.txtDataProtocolloFrom.Text;
				this.SearchProperties.Protocollo.DataProtocolloA = this.txtDataProtocolloTo.Text;
				this.SearchProperties.Protocollo.AnnoProtocollo=this.txtAnnoProtocollo.Text;
				this.SearchProperties.Protocollo.NumProtocolloDa = this.txtNumProtocolloFrom.Text;
				this.SearchProperties.Protocollo.NumProtocolloA = this.txtNumProtocolloTo.Text;
			}
		}

		/// <summary>
		/// Validazione filtri immessi
		/// </summary>
		public override bool ValidateFilters()
		{
			bool retValue=true;
			
			bool initValid,endValid,rangeValid;

			if (this.frameNumeroProtocollo.Visible)
			{
				// Validazione numero protocollo
				Validator.ValidateNumericRange(this.txtNumProtocolloFrom.Text,this.txtNumProtocolloTo.Text,out initValid,out endValid,out rangeValid);
				if (!initValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblNumProtocolloFrom.ClientID,"Numero protocollo iniziale non valido");
				if (!endValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblNumProtocolloTo.ClientID,"Numero protocollo finale non valido");
				if (!rangeValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblNumProtocolloFrom.ClientID,"Numero protocollo iniziale maggiore di quello finale");
			}

			if (this.frameDataProtocollo.Visible)
			{
				// Validazione data protocollo
				Validator.ValidateDateRange(this.txtDataProtocolloFrom.Text,this.txtDataProtocolloTo.Text,out initValid,out endValid,out rangeValid);
				if (!initValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblDataProtocolloFrom.ClientID,"Data protocollo iniziale non valida");
				if (!endValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblDataProtocolloTo.ClientID,"Data protocollo finale non valida");
				if (!rangeValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblDataProtocolloFrom.ClientID,"Data protocollo iniziale maggiore di quella finale");

				// Validazione anno protocollo
				if (!Validator.ValidateNumeric(this.txtAnnoProtocollo.Text))
					this.ValidationContainerControl.AddControlErrorMessage(this.lblAnnoProtocollo.ClientID,"Anno protocollo non valido");
			}

			if (this.frameIdDocumento.Visible)
			{
				// Validazione ID documento
				Validator.ValidateNumericRange(this.txtIdDocumentoFrom.Text,this.txtIdDocumentoTo.Text,out initValid,out endValid,out rangeValid);
				if (!initValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblIdDocumentoFrom.ClientID,"ID documento iniziale non valido");
				if (!endValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblIdDocumentoTo.ClientID,"ID documento finale non valido");
				if (!rangeValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.txtIdDocumentoFrom.ClientID,"ID documento iniziale maggiore di quello finale");
			}

			if (this.frameDataCreazione.Visible)
			{
				// Validazione data creazione
				Validator.ValidateDateRange(this.txtDataCreazioneFrom.Text,this.txtDataCreazioneTo.Text,out initValid,out endValid,out rangeValid);
				if (!initValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblDataCreazioneFrom.ClientID,"Data creazione iniziale non valida");
				if (!endValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblDataCreazioneTo.ClientID,"Data creazione finale non valida");
				if (!rangeValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblDataCreazioneFrom.ClientID,"Data creazione iniziale maggiore di quella finale");
			}

			return retValue;
		}

		/// <summary>
		/// Caricamento registri
		/// </summary>
		private void FillRegistri()
		{
			foreach (SearchRegistry item in this.SearchProperties.Protocollo.Registri)
				this.lstRegistri.Items.Add(new ListItem(item.Codice,item.Id));
		}

		/// <summary>
		/// Caricamento tipologie documenti
		/// </summary>
		private void FillTipologieDocumenti()
		{
			foreach (DocsPAWA.DocsPaWR.TipologiaAtto item in this.SearchProperties.TipiDocumento)
				this.cboTipologiaDocumento.Items.Add(new ListItem(item.descrizione,item.systemId));
		}

		private void btnSelectAllRegistri_Click(object sender,EventArgs e)
		{
			this.SelectUnselectAllItems(this.lstRegistri.Items,true);
		}

		private void btnUnSelectAllRegistri_Click(object sender,EventArgs e)
		{
			this.SelectUnselectAllItems(this.lstRegistri.Items,false);
		}

		private void SelectUnselectAllItems(ListItemCollection items,bool selectAll)
		{
			foreach (ListItem item in items)
				item.Selected=selectAll;
		}

		private void SelectAllRegistries()
		{
			this.SearchProperties.Protocollo.SelectAllRegistries();
		}

		#region Gestione rubrica mittente / destinatario

		private void btnShowRubricaMittDest_Click(object sender, System.EventArgs e)
		{
			this.PerformActionShowRubricaMittenteIntermedio();
		}

		/// <summary>
		/// Visualizzazione rubrica per il mittente intermedio
		/// </summary>
		private void PerformActionShowRubricaMittenteIntermedio()
		{
			Response.Redirect(EnvironmentContext.RootPath + "Rubrica/Rubrica.aspx?action=new&pgcall=DocsPAWA.SitoAccessibile.Ricerca.Documenti&field=pr_mitt_dest&urpl=urp&ie=ie&capacity=one");
		}

		/// <summary>
		/// Impostazione del mittente intermedio selezionato da rubrica
		/// </summary>
		private void SetCorrispondenteMittenteDestinatario()
		{
			string field=(string)Request.Params["field"];
			if (field=="pr_mitt_dest")
				this.SearchProperties.Protocollo.Corrispondente=this.GetCorrispondenteRubrica();
		}

		#endregion
	}
}
