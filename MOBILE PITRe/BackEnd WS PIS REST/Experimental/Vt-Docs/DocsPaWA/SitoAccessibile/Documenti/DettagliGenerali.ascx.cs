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
	///		Summary description for DettagliGenerali.
	/// </summary>
	public class DettagliGenerali : DettaglioDocumentoUserControl
	{
		protected System.Web.UI.WebControls.TextBox txtRegistro;
		protected System.Web.UI.WebControls.TextBox txtTipologiaDocumento;
		protected System.Web.UI.WebControls.DropDownList cboTipologiaDocumento;
		protected System.Web.UI.WebControls.DropDownList cboRegistri;
		protected System.Web.UI.WebControls.TextBox txtNumProtocollo;
		protected System.Web.UI.WebControls.Label txtStatoRegistro;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblStatoRegistro;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerNumProt;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerRegistro;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerStatoRegistro;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerSegnatura;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerIdDocumentoBase;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerDataCreazioneBase;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerDataProtocollo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerOggetto;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerTipologiaDocumento;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerIdDocumentoExtra;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerDataCreazioneExtra;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblNumProtocollo;
		protected System.Web.UI.WebControls.TextBox txtSegnatura;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblRegistro;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblSegnatura;
		protected System.Web.UI.WebControls.TextBox txtIdDocumentoBase;
		protected System.Web.UI.WebControls.TextBox txtDtaCreazioneBase;
		protected System.Web.UI.WebControls.TextBox txtDtaProto;
		protected System.Web.UI.WebControls.TextBox txtOggetto;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblIdDocumentoBase;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDtaCreazioneBase;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDtaProto;
		protected System.Web.UI.WebControls.Button btnVisibilita;
		protected System.Web.UI.WebControls.TextBox txtIdDocumentoExtra;
		protected System.Web.UI.WebControls.TextBox txtDtaCreazioneExtra;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblFiltroTipoDoc;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblIdDocumentoExtra;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDtaCreazioneExtra;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblOggetto;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
			{
				// Impostazione visibilità campi UI
				this.SetFieldsVisibility();

				if (!this.IsReadOnlyMode())
					// Caricamento combo registri, solo se non in modalità readonly
					this.FillComboRegistri();

				if (!this.IsReadOnlyMode())
					// Caricamento combo tipi documento, solo se non in modalità readonly
					this.FillComboTipoDocumento();
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
			this.btnVisibilita.Click += new System.EventHandler(this.btn_visibilita_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		protected override void BindLabelsToFields()
		{
			bool readOnlyMode=this.IsReadOnlyMode();

			this.BindLabelToField(this.lblNumProtocollo,this.txtNumProtocollo);
			
			if (!readOnlyMode)
				this.BindLabelToField(this.lblRegistro,this.cboRegistri);
			else
				this.BindLabelToField(this.lblRegistro,this.txtRegistro);
				
			this.BindLabelToField(this.lblSegnatura,this.txtSegnatura);
			this.BindLabelToField(this.lblIdDocumentoBase,this.txtIdDocumentoBase);
			this.BindLabelToField(this.lblDtaCreazioneBase,this.txtDtaCreazioneBase);
			this.BindLabelToField(this.lblDtaProto,this.txtDtaProto);
			this.BindLabelToField(this.lblOggetto,this.txtOggetto);
			
			if (!readOnlyMode)
				this.BindLabelToField(this.lblFiltroTipoDoc,this.cboTipologiaDocumento);
			else
				this.BindLabelToField(this.lblFiltroTipoDoc,this.txtTipologiaDocumento);

			this.BindLabelToField(this.lblIdDocumentoExtra,this.txtIdDocumentoExtra);
			this.BindLabelToField(this.lblDtaCreazioneExtra,this.txtDtaCreazioneExtra);
		}

		#region Gestione caricamento dati

		/// <summary>
		/// Caricamento combo registri
		/// </summary>
		private void FillComboRegistri()
		{
			this.cboRegistri.Items.Clear();
			
			Registri.RegistroHandler handler=new Registri.RegistroHandler();

			Registro[] registri=handler.GetRegistri();
			foreach (Registro registro in registri)
				this.cboRegistri.Items.Add(new ListItem(registro.codRegistro,registro.systemId));

			if (this.cboRegistri.Items.Count==0)
				this.cboRegistri.Items.Add(new ListItem(string.Empty,string.Empty));
		}

		/// <summary>
		/// Caricamento combo tipo documento
		/// </summary>
		private void FillComboTipoDocumento()
		{
			this.cboTipologiaDocumento.Items.Clear();
			
			this.cboTipologiaDocumento.Items.Add(new ListItem("Ingresso",TipiDocumento.INGRESSO));
			this.cboTipologiaDocumento.Items.Add(new ListItem("Uscita",TipiDocumento.USCITA));
			
			if (EnvironmentContext.IsEnabledProtocolloInterno())
				this.cboTipologiaDocumento.Items.Add(new ListItem("Interno",TipiDocumento.INTERNO));

			this.cboTipologiaDocumento.Items.Add(new ListItem("Grigio",TipiDocumento.GRIGIO));

			if (this.cboTipologiaDocumento.Items.Count==0)
				this.cboTipologiaDocumento.Items.Add(new ListItem(string.Empty,string.Empty));
		}

		/// <summary>
		/// Caricamento dati
		/// </summary>
		protected override void Fetch()
		{
			string tipoDocumento=this.GetSchedaDocumento().tipoProto;

			if (tipoDocumento!=null)
			{
				// Caricamento dati campi comuni ai protocolli e documenti grigi
				this.FetchCommonFields();
				
				if (tipoDocumento!="G")
					// Caricamento dati del protocollo
					this.FetchDataProtocollo();
			}
		}

		/// <summary>
		/// Caricamento dati campi comuni ai protocolli e documenti grigi
		/// </summary>
		private void FetchCommonFields()
		{
			SchedaDocumento schedaDocumento=this.GetSchedaDocumento();

			//popolamento campo data creazione
			if(schedaDocumento.dataCreazione!=null)
			{
				this.txtDtaCreazioneExtra.Text = schedaDocumento.dataCreazione.ToString();
				this.txtDtaCreazioneBase.Text = schedaDocumento.dataCreazione.ToString();
			}
			//popolamento campo oggetto
			if(schedaDocumento.oggetto!=null && schedaDocumento.oggetto.descrizione!=null)
			{
				this.txtOggetto.Text = schedaDocumento.oggetto.descrizione.ToString();
			}
			//popolamento campo id documento
			if(schedaDocumento.systemId!=null)
			{
				this.txtIdDocumentoExtra.Text = schedaDocumento.systemId;
				this.txtIdDocumentoBase.Text = schedaDocumento.systemId;
			}

			if (this.IsReadOnlyMode())
				this.txtTipologiaDocumento.Text=TipiDocumento.GetDescrizione(schedaDocumento.tipoProto);
			else
				this.cboTipologiaDocumento.SelectedValue=schedaDocumento.tipoProto;
		}

		/// <summary>
		/// Caricamento dati relativi al protocollo
		/// </summary>
		private void FetchDataProtocollo()
		{
			DocsPaWR.SchedaDocumento schedaDocumento=this.GetSchedaDocumento();

			//popolamento campo segnatura
			if(schedaDocumento.protocollo!=null && schedaDocumento.protocollo.segnatura!=null)
			{
				this.txtSegnatura.Text = schedaDocumento.protocollo.segnatura;
				this.txtDtaProto.Text = schedaDocumento.protocollo.dataProtocollazione;
				this.txtNumProtocollo.Text = schedaDocumento.protocollo.numero;

				if (this.IsReadOnlyMode())
				{
					this.txtRegistro.Text=schedaDocumento.registro.codRegistro;
				}
				else
				{
					this.cboRegistri.SelectedValue=schedaDocumento.registro.systemId;

					// Visualizzazione dello stato del registro,
					// solamente se non si è in modalità readonly
					if (schedaDocumento.registro!=null) 
					{
						string status = UserManager.getStatoRegistro(schedaDocumento.registro);

						switch(status)
						{
							case "V":
								this.txtStatoRegistro.CssClass="regStatusOpen";
								this.txtStatoRegistro.Text="Aperto";
								break;

							case "G":
								this.txtStatoRegistro.CssClass="regStatusYellow";
								this.txtStatoRegistro.Text="Giallo";
								break;

							case "R":
								this.txtStatoRegistro.CssClass="regStatusClosed";
								this.txtStatoRegistro.Text="Chiuso";
								break;

							default :
								break;
						}
					}
				}
			}
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

		/// <summary>
		/// Validazione dati immessi
		/// </summary>
		/// <returns></returns>
		public override bool IsValid()
		{
			if (!this.IsReadOnlyMode())
				return false;
			else
				return true;
		}

		#endregion

		#region Gestione visibilità campi

		/// <summary>
		/// Disabilitazione campi readonly
		/// </summary>
		private void DisableReadOnlyFields()
		{
			bool readOnlyMode=this.IsReadOnlyMode();
			
			this.txtNumProtocollo.ReadOnly=readOnlyMode;
			this.txtRegistro.ReadOnly=readOnlyMode;
			this.txtTipologiaDocumento.ReadOnly=readOnlyMode;
			this.txtSegnatura.ReadOnly=readOnlyMode;
			this.txtIdDocumentoBase.ReadOnly=readOnlyMode;
			this.txtDtaCreazioneBase.ReadOnly=readOnlyMode;
			this.txtDtaProto.ReadOnly=readOnlyMode;
			this.txtOggetto.ReadOnly=readOnlyMode;
			this.txtTipologiaDocumento.ReadOnly=readOnlyMode;
			this.txtIdDocumentoExtra.ReadOnly=readOnlyMode;
			this.txtDtaCreazioneExtra.ReadOnly=readOnlyMode;
		}

		/// <summary>
		/// Impostazione visibilità campi
		/// </summary>
		private void SetFieldsVisibility()
		{	
			bool protocollo=(this.GetSchedaDocumento().tipoProto!="G");

			bool readOnlyMode=this.IsReadOnlyMode();

			// Visualizzazione campi protocollo
			this.containerDataProtocollo.Visible=protocollo;
			this.containerNumProt.Visible=protocollo;
			this.containerRegistro.Visible=protocollo;
			
			// Stato del registro non necessario in fase di consultazione
			this.containerStatoRegistro.Visible=(protocollo && !readOnlyMode);

			this.containerSegnatura.Visible=protocollo;

			this.containerRegistro.Visible=protocollo;
			if (this.containerRegistro.Visible)
			{
				this.txtRegistro.Visible=readOnlyMode;
				this.cboRegistri.Visible=!readOnlyMode;
			}

			this.containerIdDocumentoBase.Visible=protocollo;
			this.containerIdDocumentoExtra.Visible=protocollo;
			this.containerDataCreazioneBase.Visible=protocollo;
			this.containerDataCreazioneExtra.Visible=protocollo;

			this.containerIdDocumentoBase.Visible=!protocollo;
			this.containerDataCreazioneBase.Visible=!protocollo;

			this.txtTipologiaDocumento.Visible=readOnlyMode;
			this.cboTipologiaDocumento.Visible=!readOnlyMode;

			// Disabilitazione campi readonly
			this.DisableReadOnlyFields();
		}

		#endregion

		/// <summary>
		/// Visualizzazione pagina visibilità documento
		/// </summary>
		private void ShowVisibilitaDocumento()
		{
			SchedaDocumento schedaDocumento=this.GetSchedaDocumento();

			string redirectUrl=EnvironmentContext.RootPath + "Documenti/VisibilitaDocumento.aspx?iddoc=" + schedaDocumento.systemId + "&docnum=" + schedaDocumento.docNumber;
			Response.Redirect(redirectUrl);
		}

		private void btn_visibilita_Click(object sender, System.EventArgs e)
		{
			this.ShowVisibilitaDocumento();
		}
	}
}