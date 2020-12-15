namespace DocsPAWA.SitoAccessibile.Trasmissioni
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DocsPAWA.DocsPaWR;

	/// <summary>
	///	Dettaglio di una trasmissione ricevuta
	/// </summary>
	public class DettTrasmRicevute : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl detailsContainer;
		protected System.Web.UI.WebControls.Button btnAccetta;
		protected System.Web.UI.WebControls.Button btnRifiuta;
		protected System.Web.UI.WebControls.TextBox txtNoteAccRif;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlAccettazioneRifiuto;

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.BindLabelsToFields();
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
			this.btnAccetta.Click += new System.EventHandler(this.btnAccetta_Click);
			this.btnRifiuta.Click += new System.EventHandler(this.btnRifiuta_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// ID della trasmissione ricevuta
		/// </summary>
		private string IDTrasmissione
		{
			get
			{
				if (this.ViewState["IDTrasmissione"]!=null)
					return this.ViewState["IDTrasmissione"].ToString();
				else
					return string.Empty;
			}
			set
			{
				this.ViewState["IDTrasmissione"]=value;
			}
		}

		/// <summary>
		/// Caricamento dati
		/// </summary>
		/// <param name="idTrasmissione"></param>
		public void Initialize(string idTrasmissione)
		{
			this.IDTrasmissione=idTrasmissione;

			this.Initialize(this.GetTrasmissione());
		}

		/// <summary>
		/// Caricamento dati
		/// </summary>
		/// <param name="trasmissione"></param>
		public void Initialize(Trasmissione trasmissione)
		{
			this.Fetch(trasmissione);
			
			TrasmissioniHandler handler=new TrasmissioniHandler();
			this.SetVisibilityPanelAccRif(handler.IsRequiredAccettazioneRifiuto(trasmissione));
		}

		/// <summary>
		/// Reperimento trasmissione ricevuta
		/// </summary>
		/// <returns></returns>
		private Trasmissione GetTrasmissione()
		{
			SitoAccessibile.Trasmissioni.TrasmissioniHandler trasmissioniHandler=new SitoAccessibile.Trasmissioni.TrasmissioniHandler();
			return trasmissioniHandler.GetTrasmissione(SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum.Ricevute,this.IDTrasmissione);
		}

		/// <summary>
		/// Caricamento dati dettagli trasmissione
		/// </summary>
		/// <param name="trasmissione"></param>
		private void Fetch(Trasmissione trasmissione)
		{
			this.FetchTableTrasmissione(trasmissione);

			this.txtNoteAccRif.Text=string.Empty;
		}

		/// <summary>
		/// Visualizzazione pannello accettazione / rifiuto della trasmissione
		/// </summary>
		/// <param name="isVisible"></param>
		public void SetVisibilityPanelAccRif(bool isVisible)
		{
			this.pnlAccettazioneRifiuto.Visible=isVisible;
		}

		/// <summary>
		/// Associazione attributo for per i campi label richiesti
		/// </summary>
		private void BindLabelsToFields()
		{
		}

		/// <summary>
		/// Associazione attributo "for"
		/// </summary>
		/// <param name="labelControl"></param>
		/// <param name="controlToBind"></param>
		private void BindLabelToField(HtmlGenericControl labelControl,System.Web.UI.Control controlToBind)
		{
			labelControl.Attributes.Add("for",controlToBind.ClientID);
		}

		#region Gestione dati

		#region Creazione tabella dettagli

		/// <summary>
		/// Creazione tabella trasmissioni
		/// </summary>
		/// <param name="trasmissione"></param>
		/// <returns></returns>
		private void FetchTableTrasmissione(Trasmissione trasmissione)
		{
			Table table=new Table();
			table.ID="tableTrasmissione_" + trasmissione.systemId;
			table.Attributes.Add("summary","Dati generali trasmissione");

			TableRow row=new TableRow();
			this.AppendHeaderTableCell(row,"Note generali");
			table.Rows.Add(row);

			row=new TableRow();
			this.AppendStandardTableCell(row,trasmissione.noteGenerali);
			table.Rows.Add(row);

			this.detailsContainer.Controls.Add(table);

			if (trasmissione.trasmissioniSingole.Length>0)
				this.FetchTableTrasmissioneSingola(trasmissione.trasmissioniSingole[0]);
		}

		/// <summary>
		/// Creazione tabella trasmissioni singole
		/// </summary>
		/// <param name="viewNoteSingole"></param>
		private void FetchTableTrasmissioneSingola(TrasmissioneSingola trasmissioneSingola)
		{
			Table table=new Table();
			table.ID="tableTrasmissioneSingola_" + trasmissioneSingola.systemId;
			table.Attributes.Add("summary","Dettagli trasmissione singola");

			TableRow row=new TableRow();
			this.AppendHeaderTableCell(row,"Note individuali");
			table.Rows.Add(row);

			row=new TableRow();
			this.AppendStandardTableCell(row,trasmissioneSingola.noteSingole);
			table.Rows.Add(row);

			this.detailsContainer.Controls.Add(table);

			if (trasmissioneSingola.trasmissioneUtente.Length>0)
				this.FetchTableTrasmissioniUtente(trasmissioneSingola.trasmissioneUtente[0]);
		}

		/// <summary>
		/// Creazione tabella trasmissioni utente
		/// </summary>
		/// <param name="trasmissioneUtente"></param>
		/// <returns></returns>
		private void FetchTableTrasmissioniUtente(TrasmissioneUtente trasmissioneUtente)
		{
			Table table=new Table();
			table.ID="tableTrasmissioniUtente_" + trasmissioneUtente.systemId;

			table.Attributes.Add("summary","Dettagli trasmissione utente");

			TableRow row=new TableRow();
			this.AppendHeaderTableCell(row,"Data vista");
			this.AppendHeaderTableCell(row,"Data risposta");
			this.AppendHeaderTableCell(row,"Data accettazione");
			this.AppendHeaderTableCell(row,"Data rifiuto");
			this.AppendHeaderTableCell(row,"Note accettazione / rifiuto");
			table.Rows.Add(row);

			row=new TableRow();
			this.AppendStandardTableCell(row,trasmissioneUtente.dataVista);
			this.AppendStandardTableCell(row,trasmissioneUtente.dataRisposta);

			if (trasmissioneUtente.valida!=null && trasmissioneUtente.valida.Equals("1"))
				this.AppendStandardTableCell(row,trasmissioneUtente.dataAccettata);
			else
				this.AppendStandardTableCell(row,new string('-',15));
			
			this.AppendStandardTableCell(row,trasmissioneUtente.dataRifiutata);
			this.AppendStandardTableCell(row,trasmissioneUtente.noteAccettazione + Environment.NewLine + trasmissioneUtente.noteRifiuto);
			
			table.Rows.Add(row);

			this.detailsContainer.Controls.Add(table);
		}

		private void AppendStandardTableCell(TableRow row,string text)
		{
			TableCell cell=new TableCell();
			//cell.HorizontalAlign=HorizontalAlign.Center;
			cell.Text=this.GetTableValue(text);
			row.Cells.Add(cell);
			cell=null;
		}

		private void AppendHeaderTableCell(TableRow row,string text)
		{
			TableHeaderCell cell=new TableHeaderCell();
			cell.Text=this.GetTableValue(text);
			cell.Attributes.Add("scope", "col");
			row.Cells.Add(cell);
			cell=null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		private string GetTableValue(string val)
		{
			string retValue=val;

			if (retValue==null || retValue==string.Empty)
				retValue="&nbsp;";

			return retValue;
		}

		/// <summary>
		/// verifica se la trasmissione è stata effettuata 
		/// dall'utente correntemente connesso
		/// </summary>
		/// <returns></returns>
		private bool CheckTrasmEffettuataDaUtenteCorrente(Trasmissione trasmissione)
		{	
			return trasmissione.utente.idPeople.Equals(UserManager.getInfoUtente().idPeople);
		}

		#endregion

		#endregion

		/// <summary>
		/// Reperimento trasmissione utente visualizzata
		/// </summary>
		/// <returns></returns>
		private TrasmissioneUtente GetTrasmissioneUtente()
		{
			TrasmissioneUtente trasmUtente=null;
			Trasmissione trasmissione=this.GetTrasmissione();

			if (trasmissione.trasmissioniSingole.Length>0)
			{
				TrasmissioneSingola trasmSingola=trasmissione.trasmissioniSingole[0];
				if (trasmSingola.trasmissioneUtente.Length>0)
					trasmUtente=trasmSingola.trasmissioneUtente[0];
			}

			return trasmUtente;
		}

		private void btnAccetta_Click(object sender, System.EventArgs e)
		{
			TrasmissioniHandler handler=new TrasmissioniHandler();
			handler.AccettaTrasmissione(this.GetTrasmissioneUtente(),this.txtNoteAccRif.Text, this.GetTrasmissione());
		}

		private void btnRifiuta_Click(object sender, System.EventArgs e)
		{
			TrasmissioniHandler handler=new TrasmissioniHandler();
			handler.RifiutaTrasmissione(this.GetTrasmissioneUtente(),this.txtNoteAccRif.Text, this.GetTrasmissione());	
		}
	}
}