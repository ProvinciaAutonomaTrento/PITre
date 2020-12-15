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
	///	Dettaglio di una trasmissione effettuata
	/// </summary>
	public class DettTrasmEffettuate : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.TextBox txt_destinatario;
		protected System.Web.UI.WebControls.TextBox txt_ragione;
		protected System.Web.UI.WebControls.TextBox txt_utente;
		protected System.Web.UI.WebControls.TextBox txt_datavis;
		protected System.Web.UI.WebControls.TextBox txt_dataccett;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_destinatario;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_ragione;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_utente;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_datavis;
		protected System.Web.UI.WebControls.TextBox txt_tipo;
		protected System.Web.UI.WebControls.TextBox txt_noteind;
		protected System.Web.UI.WebControls.TextBox txt_scadenza;
		protected System.Web.UI.WebControls.TextBox txt_datarif;
		protected System.Web.UI.WebControls.TextBox txt_info;
		protected System.Web.UI.WebControls.TextBox txt_datarisp;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_tipo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_noteind;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_scadenza;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_datarif;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_info;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_datarisp;
		protected System.Web.UI.HtmlControls.HtmlGenericControl detailsContainer;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_dataccett;

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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

		/// <summary>
		/// Caricamento dati trasmissione
		/// </summary>
		/// <param name="idTrasmissione"></param>
		/// <param name="enableAccettazioneRifiuto"></param>
		public void Initialize(string idTrasmissione)
		{
			this.Fetch(this.GetTrasmissione(idTrasmissione));
		}

		/// <summary>
		/// Caricamento dati trasmissione
		/// </summary>
		/// <param name="trasmissione"></param>
		public void Initialize(Trasmissione trasmissione)
		{
			this.Fetch(trasmissione);
		}

		/// <summary>
		/// Reperimento trasmissione
		/// </summary>
		/// <param name="idTrasmissione"></param>
		/// <returns></returns>
		private Trasmissione GetTrasmissione(string idTrasmissione)
		{
			SitoAccessibile.Trasmissioni.TrasmissioniHandler trasmissioniHandler=new SitoAccessibile.Trasmissioni.TrasmissioniHandler();

			return trasmissioniHandler.GetTrasmissione(SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum.Effettuate,idTrasmissione);
		}

		/// <summary>
		/// Caricamento dati dettagli trasmissione
		/// </summary>
		/// <param name="trasmissione"></param>
		private void Fetch(Trasmissione trasmissione)
		{
			this.FetchTableTrasmissione(trasmissione);
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
			table.Style.Add("WIDTH","100%");

			TableRow row=new TableRow();
			this.AppendHeaderTableCell(row,"Note generali");
			table.Rows.Add(row);

			row=new TableRow();
			this.AppendStandardTableCell(row,trasmissione.noteGenerali);
			table.Rows.Add(row);

			this.detailsContainer.Controls.Add(table);

			foreach (TrasmissioneSingola trasmissioneSingola in trasmissione.trasmissioniSingole)
				this.FetchTableTrasmissioneSingola(trasmissioneSingola,this.CheckTrasmEffettuataDaUtenteCorrente(trasmissione));
		}

		/// <summary>
		/// Creazione tabella trasmissioni singole
		/// </summary>
		/// <param name="trasmissioneSingola"></param>
		/// <param name="viewNoteSingole"></param>
		private void FetchTableTrasmissioneSingola(TrasmissioneSingola trasmissioneSingola,bool viewNoteSingole)
		{
			Table table=new Table();
			table.ID="tableTrasmissioneSingola_" + trasmissioneSingola.systemId;
			table.Attributes.Add("summary","Dettagli trasmissione singola");
			table.Style.Add("WIDTH","95%");

			TableRow row=new TableRow();
			this.AppendHeaderTableCell(row,"Destinatario");
			this.AppendHeaderTableCell(row,"Ragione");
			this.AppendHeaderTableCell(row,"Tipo");
			this.AppendHeaderTableCell(row,"Note individuali");
			this.AppendHeaderTableCell(row,"Scade il");
			table.Rows.Add(row);

			row=new TableRow();
			this.AppendStandardTableCell(row,trasmissioneSingola.corrispondenteInterno.descrizione);
			this.AppendStandardTableCell(row,trasmissioneSingola.ragione.descrizione);

			string tipoTrasmissione=string.Empty;
			if(trasmissioneSingola.tipoTrasm.Equals("T")) 
				tipoTrasmissione="TUTTI";
			else if(trasmissioneSingola.tipoTrasm.Equals("S")) 
				tipoTrasmissione="UNO";

			this.AppendStandardTableCell(row,tipoTrasmissione);

			// visualizzazione del testo relativo alla nota singola
			// solamente se l'utente che ha effettuato la trasmissione
			// sia lo stesso di quello correntemente connesso
			if (viewNoteSingole)
				this.AppendStandardTableCell(row,trasmissioneSingola.noteSingole);
			else
				this.AppendStandardTableCell(row,new string('-',15));

			this.AppendStandardTableCell(row,trasmissioneSingola.dataScadenza);
			table.Rows.Add(row);

			this.detailsContainer.Controls.Add(table);

			this.FetchTableTrasmissioniUtente(trasmissioneSingola);
		}

		/// <summary>
		/// Creazione tabella trasmissioni utente
		/// </summary>
		/// <param name="trasmissioneUtente"></param>
		/// <returns></returns>
		private void FetchTableTrasmissioniUtente(TrasmissioneSingola trasmissioneSingola)
		{
			Table table=new Table();
			table.ID="tableTrasmissioniUtente_" + trasmissioneSingola.systemId;
			table.Style.Add("WIDTH","90%");

			table.Attributes.Add("summary","Dettagli trasmissione utente");

			TableRow row=new TableRow();
			this.AppendHeaderTableCell(row,"Utente");
			this.AppendHeaderTableCell(row,"Vista il");
			this.AppendHeaderTableCell(row,"Accettata il");
			this.AppendHeaderTableCell(row,"Data rifiuto");
			this.AppendHeaderTableCell(row,"Info accettazione / Info rifiuto");
			this.AppendHeaderTableCell(row,"Riposta il");
			table.Rows.Add(row);
		
			foreach (TrasmissioneUtente trasmissioneUtente in trasmissioneSingola.trasmissioneUtente)
			{
				row=new TableRow();
				this.AppendStandardTableCell(row,trasmissioneUtente.utente.descrizione);
				this.AppendStandardTableCell(row,trasmissioneUtente.dataVista);

				if (trasmissioneUtente.valida!=null && trasmissioneUtente.valida.Equals("1"))
					this.AppendStandardTableCell(row,trasmissioneUtente.dataAccettata);
				else
					this.AppendStandardTableCell(row,new string('-',15));
				
				this.AppendStandardTableCell(row,trasmissioneUtente.dataRifiutata);
				this.AppendStandardTableCell(row,trasmissioneUtente.noteAccettazione + Environment.NewLine + trasmissioneUtente.noteRifiuto);
				this.AppendStandardTableCell(row,trasmissioneUtente.dataRisposta);
				table.Rows.Add(row);
			}

			this.detailsContainer.Controls.Add(table);
		}

		private void AppendStandardTableCell(TableRow row,string text)
		{
			TableCell cell=new TableCell();
			cell.HorizontalAlign=HorizontalAlign.Center;
			cell.Text=this.GetTableValue(text);
			row.Cells.Add(cell);
			cell=null;
		}

		private void AppendHeaderTableCell(TableRow row,string text)
		{
			TableHeaderCell cell=new TableHeaderCell();
			cell.Text=this.GetTableValue(text);
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
	}
}