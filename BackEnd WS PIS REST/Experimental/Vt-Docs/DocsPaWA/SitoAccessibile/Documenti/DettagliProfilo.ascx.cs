namespace DocsPAWA.SitoAccessibile.Documenti
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DocsPAWA.DocsPaWR;
	
	/// <summary>
	///		Summary description for DettagliProfilo.
	/// </summary>
	public class DettagliProfilo : DettaglioDocumentoUserControl
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl divPrivato;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divParoleChiave;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divNote;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblNote;
		protected System.Web.UI.WebControls.CheckBox chkPrivato;
		protected System.Web.UI.WebControls.ListBox lstParoleChiave;
		protected System.Web.UI.WebControls.DropDownList ddlTipoDoc;
		protected System.Web.UI.WebControls.TextBox txtTipoDoc;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblPrivato;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblParoleChiave;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblTipoDoc;
		protected System.Web.UI.WebControls.TextBox txtNote;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
			{
				if (!this.IsReadOnlyMode())
					// Caricamento combo tipologia atto,
					// solo se non è in modalità readonly
					this.FillItemsTipologiaAtto(this.ddlTipoDoc.Items);

				// Impostazione visibilità campi UI
				this.SetFieldsVisibility();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region Gestione visibilità campi

		/// <summary>
		/// Disabilitazione campi readonly
		/// </summary>
		private void DisableReadOnlyFields()
		{
			bool readOnlyMode=this.IsReadOnlyMode();
			
			this.txtNote.ReadOnly=readOnlyMode;
			this.txtTipoDoc.ReadOnly=readOnlyMode;
		}

		/// <summary>
		/// Impostazione visibilità campi UI
		/// </summary>
		private void SetFieldsVisibility()
		{
			bool readOnlyMode=this.IsReadOnlyMode();

			this.chkPrivato.Visible=!readOnlyMode;

			this.txtTipoDoc.Visible=readOnlyMode;
			this.ddlTipoDoc.Visible=!readOnlyMode;

			this.DisableReadOnlyFields();
		}

		#endregion

		/// <summary>
		/// Associazione attributo for
		/// </summary>
		protected override void BindLabelsToFields()
		{
			bool readOnlyMode=this.IsReadOnlyMode();

			this.BindLabelToField(this.lblPrivato,this.chkPrivato);
			this.BindLabelToField(this.lblParoleChiave,this.lstParoleChiave);
			this.BindLabelToField(this.lblNote,this.txtNote);
			
			if (!readOnlyMode)
				this.BindLabelToField(this.lblTipoDoc,this.ddlTipoDoc);
			else
				this.BindLabelToField(this.lblTipoDoc,this.txtTipoDoc);
		}

		protected override void Fetch()
		{	
			this.FetchDocumentoGrigio();
	
			this.FetchTipologiaAtto();
		}

		/// <summary>
		/// Caricamento dati documento grigio
		/// </summary>
		private void FetchDocumentoGrigio()
		{
			// Caricamento parole chiavi
			this.FetchDocumentoGrigioParoleChiavi();

            Note.INoteManager noteManager = new Note.SchedaDocumentoNoteManager(this.GetSchedaDocumento());
            this.txtNote.Text = noteManager.GetUltimaNotaAsString();

			bool readOnlyMode=this.IsReadOnlyMode();

			//popolamento campo privato
			if(this.GetSchedaDocumento().privato!=null && this.GetSchedaDocumento().privato.Equals("1"))
			{
				if (!readOnlyMode)
					this.chkPrivato.Checked=true;
				else
					this.lblPrivato.InnerText="Documento privato";
			}
			else
			{
				this.lblPrivato.InnerText="Documento pubblico";
			}
		}

		/// <summary>
		/// Aggiornamento dei dati presenti nei campi della UI
		/// nei rispettivi attributi dell'oggetto schedaDocumento
		/// </summary>
		/// <param name="schedaDocumento"></param>
		protected override void OnUpdate()
		{
			// DA FARE
		}

		public override bool IsValid()
		{
			return false;
		}

		/// <summary>
		/// Caricamento dati documento grigio: parole chiavi
		/// </summary>
		private void FetchDocumentoGrigioParoleChiavi() 
		{
			this.lstParoleChiave.Items.Clear();

			SchedaDocumento schedaDocumento=this.GetSchedaDocumento();

			if(schedaDocumento.paroleChiave!=null)
				foreach (DocsPAWA.DocsPaWR.DocumentoParolaChiave item in schedaDocumento.paroleChiave)
					this.lstParoleChiave.Items.Add(new ListItem(item.descrizione,item.systemId));

			// Creazione elemento vuoto, in caso non siano presenti
			if (this.lstParoleChiave.Items.Count==0)
				this.lstParoleChiave.Items.Add(this.CreateEmptyElement());
		}

		private ListItem CreateEmptyElement()
		{
			return new ListItem(string.Empty,"-1");
		}

		/// <summary>
		/// Caricamento tipologie atto
		/// </summary>
		/// <param name="items"></param>
		private void FillItemsTipologiaAtto(ListItemCollection items)
		{
			TipologiaAtto[] tipologie=DocumentManager.getListaTipologiaAtto(this.Page);

			items.Clear();

			if (tipologie.Length==0)
				items.Add(this.CreateEmptyElement());
			else
				foreach (TipologiaAtto tipologia in tipologie)
					items.Add(new ListItem(tipologia.descrizione,tipologia.systemId));
		}

		/// <summary>
		/// Caricamento dati tipologia documento
		/// </summary>
		private void FetchTipologiaAtto()
		{
			SchedaDocumento schedaDocumento=this.GetSchedaDocumento();

			if (schedaDocumento.tipologiaAtto != null && 
				schedaDocumento.tipologiaAtto.descrizione != null && 
				schedaDocumento.tipologiaAtto.systemId!=null)
			{
				if (!this.IsReadOnlyMode())
					this.ddlTipoDoc.SelectedValue=schedaDocumento.tipologiaAtto.systemId;
				else
					this.txtTipoDoc.Text=schedaDocumento.tipologiaAtto.descrizione;
			}
		}
	}
}