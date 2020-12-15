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
	///		Summary description for DettagliProtocollo.
	/// </summary>
	public class DettagliProtocollo : DettaglioDocumentoUserControl
	{
		protected System.Web.UI.WebControls.ListBox lstDestinatari;
		protected System.Web.UI.WebControls.ListBox lstDestinatariCC;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divMittente;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divMittenteInterm;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divDestinatario;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divProtoMittente;
		protected System.Web.UI.WebControls.Button btnDetailsCorrispondente;
		protected System.Web.UI.WebControls.Button btnDetailsCorrispondenteCC;
		protected System.Web.UI.WebControls.Button btnDetailsMittente;
		protected System.Web.UI.WebControls.TextBox txtDataAnnullamento;
		protected System.Web.UI.WebControls.TextBox txtNoteAnnullamento;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divProtocolloAnnullato;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblAnnullato;
		protected System.Web.UI.WebControls.TextBox txtCodMitt;
		protected System.Web.UI.WebControls.TextBox txtDescMitt;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblCodMitt;
		protected System.Web.UI.WebControls.TextBox txtCodMittInterm;
		protected System.Web.UI.WebControls.TextBox txtDescMittInterm;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblCodMittInterm;
		protected System.Web.UI.WebControls.TextBox txtDescDest;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblCodDest;
		protected System.Web.UI.WebControls.TextBox txtDescDestCC;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblCodDestCC;
		protected System.Web.UI.WebControls.TextBox txtProtMitt;
		protected System.Web.UI.WebControls.TextBox txtDtaProtMitt;
		protected System.Web.UI.WebControls.TextBox txtDtaArrivo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDtaArrivo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblProtMitt;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (this.IsInitialized())
			{
				// Impostazione visibilità campi UI
				this.SetFieldsVisibility();

				// Disabilitazione campi readonly
				this.DisableReadOnlyFields();
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
			this.btnDetailsMittente.Click += new System.EventHandler(this.btnDetailsMittente_Click);
			this.btnDetailsCorrispondente.Click += new System.EventHandler(this.btnDetailsCorrispondente_Click);
			this.btnDetailsCorrispondenteCC.Click += new System.EventHandler(this.btnDetailsCorrispondenteCC_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region Gestione caricamento dati

		protected override void Fetch()
		{
			if (this.IsProtocolloInterno())
			{
				this.FetchProtocolloInterno();
			}
			else if (this.IsProtocolloUscita())
			{
				this.FetchProtocolloUscita();
			}
			else if (this.IsProtocolloIngresso())
			{
				this.FetchProtocolloIngresso();
			}
			
			// Caricamento dati protocollo annullato
			this.FetchProtocolloAnnullato();
		}

		/// <summary>
		/// Caricamento dati protocollo interno
		/// </summary>
		private void FetchProtocolloInterno()
		{
			ProtocolloInterno protocollo=this.GetSchedaDocumento().protocollo as ProtocolloInterno;

			if (protocollo!=null)
			{
				this.txtCodMitt.Text = protocollo.mittente.codiceRubrica;
				this.txtDescMitt.Text = protocollo.mittente.descrizione;
				
				this.FetchItemsDestinatari(this.lstDestinatari.Items,protocollo.destinatari);
				this.FetchItemsDestinatari(this.lstDestinatariCC.Items,protocollo.destinatariConoscenza);
			}
		}

		/// <summary>
		/// Caricamento dati protocollo uscita
		/// </summary>
		private void FetchProtocolloUscita()
		{
			ProtocolloUscita protocollo=this.GetSchedaDocumento().protocollo as ProtocolloUscita;

			if (protocollo!=null)
			{
				if (protocollo.mittente!=null)
				{
					this.txtCodMitt.Text=protocollo.mittente.codiceRubrica;
					this.txtDescMitt.Text=protocollo.mittente.descrizione;
				}
				
				this.FetchItemsDestinatari(this.lstDestinatari.Items,protocollo.destinatari);
				this.FetchItemsDestinatari(this.lstDestinatariCC.Items,protocollo.destinatariConoscenza);
			}
		}

		/// <summary>
		/// Caricamento dati dell'annullamento del protocollo
		/// </summary>
		private void FetchProtocolloAnnullato()
		{
			if (this.IsProtocolloAnnullato())
			{
				ProtocolloAnnullato protocolloAnnullato=this.GetSchedaDocumento().protocollo.protocolloAnnullato;
				this.txtDataAnnullamento.Text=protocolloAnnullato.dataAnnullamento;
				this.txtNoteAnnullamento.Text=protocolloAnnullato.autorizzazione;
			}
			else
			{
				this.txtDataAnnullamento.Text=string.Empty;
				this.txtNoteAnnullamento.Text=string.Empty;
			}
		}

		/// <summary>
		/// Caricamento dati protocollo ingresso
		/// </summary>
		private void FetchProtocolloIngresso()
		{
			ProtocolloEntrata protocollo=this.GetSchedaDocumento().protocollo as ProtocolloEntrata;

			if (protocollo!=null)
			{
				if (protocollo.mittente!=null)
				{
					this.txtCodMitt.Text=protocollo.mittente.codiceRubrica;
					this.txtDescMitt.Text=protocollo.mittente.descrizione;
				}
				this.txtDtaProtMitt.Text=protocollo.dataProtocolloMittente;
				this.txtProtMitt.Text=protocollo.descrizioneProtocolloMittente;

				if (this.MittentiIntermediEnabled() && protocollo.mittenteIntermedio!=null)
				{
					this.txtCodMittInterm.Text=protocollo.mittenteIntermedio.codiceRubrica;
					this.txtDescMittInterm.Text=protocollo.mittenteIntermedio.descrizione;
				}
			}
		}

		/// <summary>
		/// Caricamento dati controllo destinatari
		/// </summary>
		/// <param name="items"></param>
		/// <param name="destinatari"></param>
		private void FetchItemsDestinatari(ListItemCollection items,Corrispondente[] destinatari)
		{
			items.Clear();

			if (destinatari==null || destinatari.Length==0)
				// Inserimento di un elemento vuoto, in caso non siano presenti destinatari
				items.Add(this.CreateEmptyElement());
			else
				foreach (Corrispondente destinatario in destinatari)
					items.Add(new ListItem(destinatario.descrizione,destinatario.systemId));
		}

		/// <summary>
		/// Creazione di un elemento vuoto
		/// </summary>
		/// <returns></returns>
		private ListItem CreateEmptyElement()
		{
			return new ListItem(string.Empty,"-1");
		}

		#endregion

		#region Gestione visibilità campi

		/// <summary>
		/// Disabilitazione campi readonly
		/// </summary>
		private void DisableReadOnlyFields()
		{
			bool readOnlyMode=this.IsReadOnlyMode();
			
			this.txtCodMitt.ReadOnly=readOnlyMode;
			this.txtDescMitt.ReadOnly=true;
			this.txtCodMittInterm.ReadOnly=readOnlyMode;
			this.txtDescMittInterm.ReadOnly=readOnlyMode;
			this.txtDescDest.ReadOnly=readOnlyMode;
			this.txtDescDestCC.ReadOnly=readOnlyMode;
			this.txtProtMitt.ReadOnly=readOnlyMode;
			this.txtDtaProtMitt.ReadOnly=readOnlyMode;
			this.txtDtaArrivo.ReadOnly=readOnlyMode;
			this.txtDataAnnullamento.ReadOnly=readOnlyMode;
			this.txtNoteAnnullamento.ReadOnly=readOnlyMode;
		}

		/// <summary>
		/// Impostazione visibilità campi UI
		/// </summary>
		private void SetFieldsVisibility()
		{
			this.SetFieldsVisibilityDestinatari();

			this.SetFieldsVisibilityProtMittente();

			this.SetFieldsVisibilityMittenteIntermedio();

			this.SetFieldsVisibilityProtAnnullato();
		}

		/// <summary>
		/// Impostazione visibilità campi UI del protocollo annullato
		/// </summary>
		private void SetFieldsVisibilityProtAnnullato()
		{	
			this.divProtocolloAnnullato.Visible=this.IsProtocolloAnnullato();
		}

		/// <summary>
		/// Impostazione visibilità campi UI del destinatario
		/// </summary>
		private void SetFieldsVisibilityDestinatari()
		{
			this.divDestinatario.Visible=(this.IsProtocolloInterno() || this.IsProtocolloUscita());

			if (this.divDestinatario.Visible)
			{
				// Abilitazione per controlli readonly
				bool isReadOnlyMode=this.IsReadOnlyMode();
				this.txtDescDest.Visible=!isReadOnlyMode;
				this.txtDescDestCC.Visible=!isReadOnlyMode;
			}
		}

		/// <summary>
		/// Impostazione visibilità campi UI del prot mittente
		/// </summary>
		private void SetFieldsVisibilityProtMittente()
		{
			this.divProtoMittente.Visible=this.IsProtocolloIngresso();
		}

		/// <summary>
		/// Impostazione visibilità campi UI del mittente intermedio
		/// </summary>
		private void SetFieldsVisibilityMittenteIntermedio()
		{
			if (this.MittentiIntermediEnabled())
				this.divMittenteInterm.Visible=true;
		}

		#endregion
		

		#region Gestione aggiornamento dati

		/// <summary>
		/// Aggiornamento dei dati presenti nei campi della UI
		/// nei rispettivi attributi dell'oggetto schedaDocumento
		/// </summary>
		/// <param name="schedaDocumento"></param>
		protected override void OnUpdate()
		{
		}

		public override bool IsValid()
		{
			if (!this.IsReadOnlyMode())
				return false;
			else
				return true;
		}

		#endregion

		/// <summary>
		/// Associazione attributo for
		/// </summary>
		protected override void BindLabelsToFields()
		{
			this.BindLabelToField(this.lblCodMitt,this.txtCodMitt);
			this.BindLabelToField(this.lblCodMittInterm,this.txtCodMittInterm);
			this.BindLabelToField(this.lblCodDest,this.lstDestinatari);
			this.BindLabelToField(this.lblCodDestCC,this.lstDestinatariCC);
			this.BindLabelToField(this.lblProtMitt,this.txtProtMitt);
			this.BindLabelToField(this.lblDtaArrivo,this.txtDtaArrivo);
			this.BindLabelToField(this.lblAnnullato,this.txtDataAnnullamento);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool IsProtocolloInterno()
		{
			return (this.GetSchedaDocumento().tipoProto=="I" &&
					this.GetSchedaDocumento().protocollo.GetType().Equals(typeof(DocsPAWA.DocsPaWR.ProtocolloInterno)));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool IsProtocolloUscita()
		{
			return (this.GetSchedaDocumento().tipoProto=="P" &&
					this.GetSchedaDocumento().protocollo.GetType().Equals(typeof(DocsPAWA.DocsPaWR.ProtocolloUscita)));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool IsProtocolloIngresso()
		{
			return (this.GetSchedaDocumento().tipoProto=="A" &&
					this.GetSchedaDocumento().protocollo.GetType().Equals(typeof(DocsPAWA.DocsPaWR.ProtocolloEntrata)));
		}

		/// <summary>
		/// Verifica se il protocollo è annullato o meno
		/// </summary>
		/// <returns></returns>
		private bool IsProtocolloAnnullato()
		{
			DocsPaWR.ProtocolloAnnullato protocolloAnnullato=this.GetSchedaDocumento().protocollo.protocolloAnnullato;
			
			return (protocolloAnnullato!=null && protocolloAnnullato.dataAnnullamento!=string.Empty);
		}

		#region Gestione rubrica

		/// <summary>
		/// Verifica se la gestione dei mittenti intermedi sia abilitata,
		/// solo per i protocolli in ingresso
		/// </summary>
		/// <returns></returns>
		private bool MittentiIntermediEnabled()
		{
			if (!this.IsProtocolloIngresso())
			{
				string config=ConfigSettings.getKey(ConfigSettings.KeysENUM.VIEW_MITT_INTERMEDI);

				return (config!=null && config.Equals("1"));
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Visualizzazione del dettaglio del corrispondente
		/// </summary>
		/// <param name="idCorrispondente"></param>
		private void RedirectToDetailsCorrispondente(string idCorrispondente)
		{
			string redirectUrl=EnvironmentContext.RootPath + "Rubrica/DettagliCorrispondente.aspx?idCorrispondente=" + idCorrispondente + "&iddoc=" + this.GetSchedaDocumento().systemId + "&docnum=" + this.GetSchedaDocumento().docNumber;

			Response.Redirect(redirectUrl);
		}

        /// <summary>
        /// Visualizzazione dei dettagli del mittente
        /// </summary>
        private void ShowDetailsMittente()
        {
            Corrispondente mittente = null;

            if (this.IsProtocolloIngresso())
            {
                mittente = ((ProtocolloEntrata)this.GetSchedaDocumento().protocollo).mittente;
            }
            else if (this.IsProtocolloUscita())
            {
                mittente = ((ProtocolloUscita)this.GetSchedaDocumento().protocollo).mittente;
            }
            else if (this.IsProtocolloInterno())
            {
                mittente = ((ProtocolloInterno)this.GetSchedaDocumento().protocollo).mittente;
            }

            if (mittente != null)
                this.RedirectToDetailsCorrispondente(mittente.systemId);
        }

		/// <summary>
		/// Visualizzazione dei dettagli del destinatario selezionato
		/// </summary>
		private void ShowDetailsDestinatario()
		{
			if (this.lstDestinatari.SelectedItem!=null)
				this.RedirectToDetailsCorrispondente(this.lstDestinatari.SelectedItem.Value);
		}

		/// <summary>
		/// Visualizzazione dei dettagli del destinatario in CC selezionato
		/// </summary>
		private void ShowDetailsDestinatarioCC()
		{
			if (this.lstDestinatariCC.SelectedItem!=null)
				this.RedirectToDetailsCorrispondente(this.lstDestinatariCC.SelectedItem.Value);
		}

		private void btnDetailsMittente_Click(object sender, System.EventArgs e)
		{
			this.ShowDetailsMittente();
		}

		private void btnDetailsCorrispondente_Click(object sender, System.EventArgs e)
		{
			this.ShowDetailsDestinatario();
		}

		private void btnDetailsCorrispondenteCC_Click(object sender, System.EventArgs e)
		{
			this.ShowDetailsDestinatarioCC();
		}

		#endregion
	}
}