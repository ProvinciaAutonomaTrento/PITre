namespace DocsPAWA.SitoAccessibile.Ricerca
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using DocsPAWA.DocsPaWR;
	using SitoAccessibile.Validations;
	using SitoAccessibile.Documenti;

	/// <summary>
	///	Gestione visualizzazione filtri avanzati sui documenti
	/// </summary>
	public class FiltriDocumentiAvanzati : BaseFiltriRicercaDocumenti
	{
		protected System.Web.UI.WebControls.TextBox txtNote;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblParoleChiave;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblNote;
		protected System.Web.UI.WebControls.TextBox txtSegnatura;
		protected System.Web.UI.HtmlControls.HtmlGenericControl panelSegnatura;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblSegnatura;
		protected System.Web.UI.WebControls.TextBox txtSegnaturaMittente;
		protected System.Web.UI.HtmlControls.HtmlGenericControl panelSegnaturaMittente;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblSegnaturaMittente;
		protected System.Web.UI.WebControls.TextBox txtMittenteIntermedio;
		protected System.Web.UI.HtmlControls.HtmlGenericControl panelMittenteIntermedio;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblMittenteIntermedio;
		protected WebControls.DateMask txtDataProtocolloEmergenza;
		protected System.Web.UI.HtmlControls.HtmlGenericControl panelDataProtocolloEmergenza;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataProtocolloEmergenza;
		protected System.Web.UI.WebControls.TextBox txtProtocolloEmergenza;
		protected System.Web.UI.HtmlControls.HtmlGenericControl panelProtocolloEmergenza;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblProtocolloEmergenza;
		protected System.Web.UI.WebControls.TextBox txtCodiceFascicolo;
		protected System.Web.UI.WebControls.TextBox txtDescrizioneFascicolo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblCodiceFascicolo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDescrizioneFascicolo;
		protected System.Web.UI.WebControls.RadioButtonList listEvidenza;
		protected System.Web.UI.HtmlControls.HtmlGenericControl frameEvidenza;
		protected System.Web.UI.WebControls.RadioButtonList listStato;
		protected System.Web.UI.HtmlControls.HtmlGenericControl frameStato;
		protected WebControls.DateMask txtDataProtocolloMittFrom;
		protected WebControls.DateMask txtDataProtocolloMittTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl frameDataProtocolloMittente;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataProtocolloMittFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataProtocolloMittTo;
		protected WebControls.DateMask txtDataArrivoFrom;
		protected WebControls.DateMask txtDataArrivoTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataArrivoFrom;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lblDataArrivoTo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl framePriviDi;
		protected System.Web.UI.HtmlControls.HtmlGenericControl frameDataArrivo;
		protected System.Web.UI.WebControls.Button btnSearchFascicolo;
		protected System.Web.UI.WebControls.ListBox listParoleChiavi;
		protected System.Web.UI.WebControls.Button btnShowParoleChiavi;
		protected System.Web.UI.WebControls.Button btnRemoveParolaChiave;
		protected System.Web.UI.WebControls.CheckBox chkPriviAssegnatario;
		protected System.Web.UI.WebControls.CheckBox chkPriviFascicolazione;
		protected System.Web.UI.WebControls.CheckBox chkPriviImmagine;
		protected System.Web.UI.WebControls.Button btnShowRubricaMittenteIntermedio;

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
			this.btnShowParoleChiavi.Click += new System.EventHandler(this.btnShowParoleChiavi_Click);
			this.btnRemoveParolaChiave.Click += new System.EventHandler(this.btnRemoveParolaChiave_Click);
			this.btnShowRubricaMittenteIntermedio.Click += new System.EventHandler(this.btnShowRubricaMittenteIntermedio_Click);
			this.btnSearchFascicolo.Click += new System.EventHandler(this.btnSearchFascicolo_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		protected override void BindLabelsToFields()
		{
			this.BindLabelToField(this.lblParoleChiave,this.listParoleChiavi);
			this.BindLabelToField(this.lblNote,this.txtNote);
			this.BindLabelToField(this.lblSegnatura,this.txtSegnatura);
			this.BindLabelToField(this.lblSegnaturaMittente,this.txtSegnaturaMittente);
			this.BindLabelToField(this.lblMittenteIntermedio,this.txtMittenteIntermedio);
			this.BindLabelToField(this.lblDataProtocolloEmergenza,this.txtDataProtocolloEmergenza);
			this.BindLabelToField(this.lblCodiceFascicolo,this.txtCodiceFascicolo);
			this.BindLabelToField(this.lblDescrizioneFascicolo,this.txtDescrizioneFascicolo);
			this.BindLabelToField(this.lblDataProtocolloMittFrom,this.txtDataProtocolloMittFrom);
			this.BindLabelToField(this.lblDataProtocolloMittTo,this.txtDataProtocolloMittTo);
			this.BindLabelToField(this.lblDataArrivoFrom,this.txtDataArrivoFrom);
			this.BindLabelToField(this.lblDataArrivoTo,this.txtDataArrivoTo);
		}

		/// <summary>
		/// Caricamento dati maschera di filtri avanzati
		/// </summary>
		public override void LoadData()
		{	
			if (ParoleChiavi.OnSelectParoleChiavi)
			{
				ArrayList listParoleChiavi=new ArrayList(this.SearchProperties.Documento.ParoleChiavi);
				listParoleChiavi.AddRange(ParoleChiavi.GetParoleChiavi());
				this.SearchProperties.Documento.ParoleChiavi=(DocumentoParolaChiave[]) listParoleChiavi.ToArray(typeof(DocumentoParolaChiave));
			}

			// Caricamento parole chiavi
			this.FetchParoleChiavi();

			this.txtNote.Text=this.SearchProperties.Documento.Note;

			if (this.SearchProperties.ProtocolliArrivo || this.SearchProperties.ProtocolliPartenza)
				this.txtSegnatura.Text=this.SearchProperties.Protocollo.Segnatura;

			if (this.SearchProperties.ProtocolliArrivo || this.SearchProperties.ProtocolliPartenza)
			{
				if (this.OnSelectCorrispondente())
					this.SetCorrispondenteMittenteIntermedio();

				this.txtSegnaturaMittente.Text=this.SearchProperties.Protocollo.SegnaturaMittente;
				this.txtMittenteIntermedio.Text=this.SearchProperties.Protocollo.MittenteIntermedio.descrizione;
				this.txtDataProtocolloEmergenza.Text=this.SearchProperties.Protocollo.DataProtEmergenza;
				this.txtProtocolloEmergenza.Text=this.SearchProperties.Protocollo.ProtocolloEmergenza;
			}

			this.txtCodiceFascicolo.Text=this.SearchProperties.Documento.CodiceFascicolo;
			this.txtDescrizioneFascicolo.Text=this.SearchProperties.Documento.DescrizioneFascicolo;

			if (this.SearchProperties.ProtocolliArrivo || this.SearchProperties.ProtocolliPartenza)
			{
				this.listEvidenza.SelectedValue=this.SearchProperties.Documento.Evidenza;
				this.listStato.SelectedValue=this.SearchProperties.Protocollo.Stato;

				this.txtDataProtocolloMittFrom.Text=this.SearchProperties.Protocollo.DataProtMittenteDa;
				this.txtDataProtocolloMittTo.Text=this.SearchProperties.Protocollo.DataProtMittenteA;

				this.txtDataArrivoFrom.Text=this.SearchProperties.Protocollo.DataArrivoDa;
				this.txtDataArrivoTo.Text=this.SearchProperties.Protocollo.DataArrivoA;
			}

			if (this.SearchProperties.DocumentiGrigi)
			{
				this.chkPriviAssegnatario.Checked=this.SearchProperties.Documento.PriviDiAssegnatario;
				this.chkPriviImmagine.Checked=this.SearchProperties.Documento.PriviDiImmagine;
				this.chkPriviFascicolazione.Checked=this.SearchProperties.Documento.PriviDiFascicolazione;
			}
		}

		/// <summary>
		/// Gestione caricamento parole chiavi
		/// </summary>
		private void FetchParoleChiavi()
		{
			this.listParoleChiavi.Items.Clear();

			if (this.SearchProperties.Documento.ParoleChiavi.Length>0)
			{
				foreach (DocumentoParolaChiave parolaChiave in this.SearchProperties.Documento.ParoleChiavi)
					this.listParoleChiavi.Items.Add(new ListItem(parolaChiave.descrizione,parolaChiave.systemId));
			}
			else
			{
				this.listParoleChiavi.Items.Add(new ListItem(string.Empty,string.Empty));
			}
		}

		/// <summary>
		/// Aggiornamento dati di filtro dai campi della UI
		/// </summary>
		public override void RefreshSearchProperties()
		{
			//Legge i campi del profilo ridotto
			this.SearchProperties.Documento.Note=this.txtNote.Text;
			this.SearchProperties.Documento.Evidenza=this.listEvidenza.SelectedItem.Value;

			if (this.SearchProperties.AdvancedDocProperties)
			{	
				// Caricamento campi del profilo avanzato
				this.SearchProperties.Documento.NomeFirmatario=string.Empty; // Non gestito
				this.SearchProperties.Documento.CognomeFirmatario=string.Empty; // Non gestito
				
				this.SearchProperties.Documento.PriviDiAssegnatario=this.chkPriviAssegnatario.Checked;
				this.SearchProperties.Documento.PriviDiImmagine=this.chkPriviImmagine.Checked;
				this.SearchProperties.Documento.PriviDiFascicolazione=this.chkPriviFascicolazione.Checked;

				this.SearchProperties.Documento.CodiceFascicolo=this.txtCodiceFascicolo.Text;
				this.SearchProperties.Documento.DescrizioneFascicolo=this.txtDescrizioneFascicolo.Text;
			}

			if (this.SearchProperties.ProtocolliArrivo ||
				this.SearchProperties.ProtocolliInterni ||
				this.SearchProperties.ProtocolliPartenza)
			{	
				this.SearchProperties.Protocollo.Segnatura=this.txtSegnatura.Text;
				this.SearchProperties.Protocollo.DataArrivoDa=this.txtDataArrivoFrom.Text;
				this.SearchProperties.Protocollo.DataArrivoA=this.txtDataArrivoTo.Text;
				
				if (this.SearchProperties.AdvancedProtProperties)
				{ //Se è visibile il protocollo avanzato,
					//legge i campi del protocollo avanzato
					this.SearchProperties.Protocollo.SegnaturaMittente=this.txtSegnaturaMittente.Text;
					this.SearchProperties.Protocollo.DataProtMittenteDa=this.txtDataProtocolloMittFrom.Text;
					this.SearchProperties.Protocollo.DataProtMittenteA=this.txtDataProtocolloMittTo.Text;
					this.SearchProperties.Protocollo.Stato=this.listStato.SelectedItem.Value;
					
					if (this.txtMittenteIntermedio.Text==string.Empty)
					{
						this.SearchProperties.Protocollo.MittenteIntermedio=new Corrispondente();
					}
					else if (!this.txtMittenteIntermedio.Text.Equals(this.SearchProperties.Protocollo.MittenteIntermedio.descrizione))
					{
						this.SearchProperties.Protocollo.MittenteIntermedio=new Corrispondente();
						this.SearchProperties.Protocollo.MittenteIntermedio.descrizione=this.txtMittenteIntermedio.Text;
					}

					this.SearchProperties.Protocollo.DataProtEmergenza=this.txtDataProtocolloEmergenza.Text;
					this.SearchProperties.Protocollo.ProtocolloEmergenza=this.txtProtocolloEmergenza.Text;
				}
			}
		}

		/// <summary>
		/// Validazione filtri immessi
		/// </summary>
		public override bool ValidateFilters()
		{
			bool retValue=true;

			if (this.txtDescrizioneFascicolo.Text==string.Empty && this.txtCodiceFascicolo.Text!=string.Empty)
			{
				retValue=(this.GetDescrizioneFascicolo(this.txtCodiceFascicolo.Text)!=string.Empty);
				
				if (!retValue)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblCodiceFascicolo.ClientID,"Codice fascicolo non trovato");
			}

			if (this.panelDataProtocolloEmergenza.Visible && !Validator.ValidateDate(this.txtDataProtocolloEmergenza.Text))
				this.ValidationContainerControl.AddControlErrorMessage(this.lblDataProtocolloEmergenza.ClientID,"Data protocollo emergenza non valida");

			bool initValid,endValid,rangeValid;

			if (this.frameDataProtocolloMittente.Visible)
			{
				// Validazione data protocollo mittente
				Validator.ValidateDateRange(this.txtDataProtocolloMittFrom.Text,this.txtDataProtocolloMittTo.Text,out initValid,out endValid,out rangeValid);
				if (!initValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblDataProtocolloMittFrom.ClientID,"Data protocollo mittente iniziale non valida");
				if (!endValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblDataProtocolloMittTo.ClientID,"Data protocollo mittente finale non valida");
				if (!rangeValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblDataProtocolloMittFrom.ClientID,"Data protocollo mittente iniziale maggiore di quella finale");
			}

			if (this.frameDataArrivo.Visible)
			{
				// Validazione data arrivo
				Validator.ValidateDateRange(this.txtDataArrivoFrom.Text,this.txtDataArrivoTo.Text,out initValid,out endValid,out rangeValid);
				if (!initValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblDataArrivoFrom.ClientID,"Data arrivo iniziale non valida");
				if (!endValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblDataArrivoTo.ClientID,"Data arrivo finale non valida");
				if (!rangeValid)
					this.ValidationContainerControl.AddControlErrorMessage(this.lblDataArrivoFrom.ClientID,"Data arrivo iniziale maggiore di quella finale");
			}

			return retValue;
		}
		
		/// <summary>
		/// Impostazione visibilità campi UI
		/// </summary>
		protected override void SetFieldsVisibility()
		{
			this.panelSegnatura.Visible=(this.SearchProperties.ProtocolliArrivo ||
										this.SearchProperties.ProtocolliInterni ||
										this.SearchProperties.ProtocolliPartenza);

			this.panelSegnaturaMittente.Visible=(this.SearchProperties.ProtocolliArrivo || this.SearchProperties.ProtocolliPartenza);
			this.panelMittenteIntermedio.Visible=this.panelSegnaturaMittente.Visible;
			this.panelDataProtocolloEmergenza.Visible=this.panelSegnaturaMittente.Visible;
			this.panelProtocolloEmergenza.Visible=this.panelSegnaturaMittente.Visible;

			this.frameEvidenza.Visible=(this.SearchProperties.ProtocolliArrivo || this.SearchProperties.ProtocolliPartenza);
			this.frameStato.Visible=this.frameEvidenza.Visible;
			this.frameDataProtocolloMittente.Visible=this.frameEvidenza.Visible;
			this.frameDataArrivo.Visible=this.frameEvidenza.Visible;

			this.framePriviDi.Visible=this.SearchProperties.DocumentiGrigi;
		}

		#region Gestione parole chiavi

		private void btnShowParoleChiavi_Click(object sender, System.EventArgs e)
		{
			this.RedirectToListaParoleChiavi();
		}

		private void btnRemoveParolaChiave_Click(object sender, System.EventArgs e)
		{
			if (this.listParoleChiavi.SelectedItem!=null)
				this.RemoveParolaChiave(this.listParoleChiavi.SelectedItem.Value);
		}

		/// <summary>
		/// Rimozione di una parola chiave
		/// </summary>
		/// <param name="id"></param>
		private void RemoveParolaChiave(string id)
		{
			ArrayList list=new ArrayList(this.SearchProperties.Documento.ParoleChiavi);

			DocumentoParolaChiave itemToRemove=null;

			foreach (DocumentoParolaChiave item in list)
			{
				if (item.systemId.Equals(id))
				{	
					itemToRemove=item;
					break;
				}
			}

			if (itemToRemove!=null)
			{
				list.Remove(itemToRemove);

				this.SearchProperties.Documento.ParoleChiavi=(DocumentoParolaChiave[]) list.ToArray(typeof(DocumentoParolaChiave));
				
				this.FetchParoleChiavi();
			}
		}

		/// <summary>
		/// Redirect alla pagina di selezione delle parole chiavi
		/// </summary>
		private void RedirectToListaParoleChiavi()
		{
			Response.Redirect(EnvironmentContext.RootPath + "Documenti/ParoleChiavi.aspx");
		}

		#endregion

		#region Gestione rubrica mittente intermedio

		private void btnShowRubricaMittenteIntermedio_Click(object sender, System.EventArgs e)
		{
			this.PerformActionShowRubricaMittenteIntermedio();
		}

		/// <summary>
		/// Visualizzazione rubrica per il mittente intermedio
		/// </summary>
		private void PerformActionShowRubricaMittenteIntermedio()
		{
			Response.Redirect(EnvironmentContext.RootPath + "Rubrica/Rubrica.aspx?action=new&pgcall=DocsPAWA.SitoAccessibile.Ricerca.Documenti&field=pe_mitt_inter&urpl=urp&ie=e&capacity=one");
		}

		/// <summary>
		/// Impostazione del mittente intermedio selezionato da rubrica
		/// </summary>
		private void SetCorrispondenteMittenteIntermedio()
		{
			string field = (string) Request.Params["field"];

			if (field=="pe_mitt_inter")
				this.SearchProperties.Protocollo.MittenteIntermedio=this.GetCorrispondenteRubrica();
		}

		#endregion

		#region Gestione ricerca fascicolo inserito

		private void btnSearchFascicolo_Click(object sender, System.EventArgs e)
		{
			this.PerformSearchFascicolo();
		}

		/// <summary>
		/// Gestione ricerca fascicolo inserito
		/// </summary>
		private void PerformSearchFascicolo()
		{
			string descrizioneFasciolo=this.GetDescrizioneFascicolo(this.txtCodiceFascicolo.Text);

			if (descrizioneFasciolo==string.Empty)
			{
				this.ValidationContainerControl.AddControlErrorMessage(this.lblCodiceFascicolo.ClientID,"Codice fascicolo non trovato");
			}
			else
			{
				this.SearchProperties.Documento.CodiceFascicolo=this.txtCodiceFascicolo.Text;
				this.SearchProperties.Documento.DescrizioneFascicolo=descrizioneFasciolo;
			}

			this.txtDescrizioneFascicolo.Text=descrizioneFasciolo;
		}

		/// <summary>
		/// Reperimento descrizione del fascicolo
		/// </summary>
		/// <returns></returns>
		private string GetDescrizioneFascicolo(string codiceFascicolo)
		{
			string retValue=string.Empty;

			if (codiceFascicolo!=null && codiceFascicolo!=string.Empty)
			{
				DocsPaWR.Fascicolo fascicolo = this.GetFascicolo(codiceFascicolo);
				if (fascicolo!=null)
					retValue=fascicolo.descrizione;
			}
			
			return retValue;
		}

		/// <summary>
		/// Reperimento fascicolo
		/// </summary>
		/// <param name="codiceFascicolo"></param>
		/// <returns></returns>
		private Fascicolo GetFascicolo(string codiceFascicolo)
		{
			DocsPaWR.Fascicolo fascicolo=null;
			if (codiceFascicolo!=null && codiceFascicolo!="")
				fascicolo=FascicoliManager.getFascicoloDaCodice(this.Page,codiceFascicolo,UserManager.getInfoUtente());
			return fascicolo;
		}

		#endregion


	}
}
