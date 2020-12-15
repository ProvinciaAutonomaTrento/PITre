namespace DocsPAWA.SitoAccessibile.Fascicoli
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DocsPAWA.DocsPaWR;
	using DocsPAWA.SitoAccessibile.Paging;
	using DocsPAWA.SitoAccessibile.Documenti;

	/// <summary>
	///		Summary description for DocumentiFascicolo.
	/// </summary>
	public class DocumentiFascicolo : System.Web.UI.UserControl
	{
		protected UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid grdDocumentiFascicolo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlMessage;
		
		private string _idFascicolo=string.Empty;
		private string _idFolder=string.Empty;
		private string _idRegistro=string.Empty;

		/// <summary>
		/// 
		/// </summary>
		public enum GridColumnsEnum
		{
			IDProfile,
			DocNumber,
			Documento,
			Registro,
			Oggetto,
			MittenteDestinatari,
			TipoDocumento,
			Azioni,
			DataAnnullamento
		}
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			this.SetControlsVisibility();

			this.ShowMessageDocuments();
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
			this.grdDocumentiFascicolo.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdDocumentiFascicolo_ItemCommand);
			this.grdDocumentiFascicolo.PreRender += new System.EventHandler(this.grdDocumentiFascicolo_PreRender);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		/// <summary>
		/// Inizializzazione controllo
		/// </summary>
		/// <param name="idFascicolo"></param>
		/// <param name="idFolder"></param>
		public void Initialize(string idFascicolo,string idFolder,string idRegistro)
		{
			this._idFascicolo=idFascicolo;
			this._idFolder=idFolder;
			this._idRegistro=idRegistro;

			// Inizializzazione eventi controllo paginazione
			this.InitializeEventsListPaging();

			if (!this.IsPostBack)
			{
				this.FetchDocumenti(new PagingContext(1));
			}
		}

		/// <summary>
		/// Caricamento documenti
		/// </summary>
		/// <param name="pagingContext"></param>
		private void FetchDocumenti(PagingContext pagingContext)
		{
			this.FetchGridDocumenti(this.GetDocumenti(pagingContext));

			this.GetListPagingControl().RefreshPaging(pagingContext);
		}

		/// <summary>
		/// Caricamento griglia documenti
		/// </summary>
		/// <param name="infoDocumenti"></param>
		private void FetchGridDocumenti(InfoDocumento[] infoDocumenti)
		{
			this.grdDocumentiFascicolo.DataSource=this.InfoDocumentiToDataset(infoDocumenti);
			this.grdDocumentiFascicolo.DataBind();
		}

		/// <summary>
		/// Creazione dataset per la visualizzazione dei documenti
		/// </summary>
		/// <param name="infoDocumenti"></param>
		/// <returns></returns>
		private DataSet InfoDocumentiToDataset(InfoDocumento[] infoDocumenti)
		{
			DataSet ds=new DataSet();
			DataTable dt=new DataTable();

			dt.Columns.Add("IDProfile",typeof(string));
			dt.Columns.Add("DocNumber",typeof(string));
			dt.Columns.Add("Documento",typeof(string));
			dt.Columns.Add("Registro",typeof(string));
			dt.Columns.Add("TipoDocumento",typeof(string));
			dt.Columns.Add("Oggetto",typeof(string));
			dt.Columns.Add("MittenteDestinatari",typeof(string));
			dt.Columns.Add("DataAnnullamento",typeof(string));

			foreach (InfoDocumento infoDocumento in infoDocumenti)
			{
				DataRow row=dt.NewRow();

				row["IDProfile"]=infoDocumento.idProfile;
				row["DocNumber"]=infoDocumento.docNumber;

				string documento=SitoAccessibile.Documenti.TipiDocumento.ToString(infoDocumento);
				if (infoDocumento.dataAnnullamento!=null && infoDocumento.dataAnnullamento!=string.Empty)
					documento+="<BR />Ann.to il " + infoDocumento.dataAnnullamento;
				row["Documento"]=documento;

				row["Registro"]=infoDocumento.codRegistro;
				row["TipoDocumento"]=TipiDocumento.GetDescrizione(infoDocumento.tipoProto);
				row["Oggetto"]=infoDocumento.oggetto;

				string mittenteDestinatari=string.Empty;
				foreach (string item in infoDocumento.mittDest)
				{
					if (mittenteDestinatari!=string.Empty)
						mittenteDestinatari+="; ";
					
					mittenteDestinatari+=item;
				}

				row["MittenteDestinatari"]=mittenteDestinatari;

				row["DataAnnullamento"]=infoDocumento.dataAnnullamento;
				
				dt.Rows.Add(row);
			}
			
			ds.Tables.Add(dt);

			return ds;
		}

		/// <summary>
		/// Reperimento documenti nel fascicolo
		/// </summary>
		/// <param name="pagingContext"></param>
		/// <returns></returns>
		private InfoDocumento[] GetDocumenti(PagingContext pagingContext)
		{
			FascicoloHandler fascicoloHandler=new FascicoloHandler();
			return fascicoloHandler.GetDocumenti(this._idFascicolo,this._idFolder,this._idRegistro,pagingContext);
		}

		/// <summary>
		/// Visualizzazione dettagli del documento
		/// </summary>
		/// <param name="idProfile"></param>
		/// <param name="docNumber"></param>
		private void ShowDetails(string idProfile,string docNumber)
		{
			// Impostazione contesto chiamante
			this.SetCallerContext();

			string url=EnvironmentContext.RootPath + "Documenti/DettagliDocumento.aspx?iddoc=" + idProfile + "&docnum=" + docNumber; // + "&caller=" + CallerKeysEnum.SearchDocumentsInFolder.ToString();

			Response.Redirect(url);
		}

		/// <summary>
		/// Impostazione contesto chiamante
		/// </summary>
		private void SetCallerContext()
		{
			// Impostazione del contesto del chiamante in sessione
			CallerContext callerContext=CallerContext.NewContext(this.Page.Request.Url.AbsoluteUri);
			callerContext.PageNumber=this.GetListPagingControl().GetPagingContext().PageNumber;
		}

		/// <summary>
		/// Impostazione visibilità controlli
		/// </summary>
		private void SetControlsVisibility()
		{
			int documentsCount=this.GetListPagingControl().GetPagingContext().RecordCount;

			this.grdDocumentiFascicolo.Visible=(documentsCount>0);
			this.GetListPagingControl().Visible=this.grdDocumentiFascicolo.Visible;
		}

		/// <summary>
		/// Visualizzazione messaggio relativo ai documenti trovati
		/// </summary>
		private void ShowMessageDocuments()
		{
			int documentsCount=this.GetListPagingControl().GetPagingContext().RecordCount;

			string message=string.Empty;

			if (documentsCount==0)
				message="Nessun documento trovato";
			else if (documentsCount==1)
				message="Trovato 1 documento";
			else
				message="Trovati " + documentsCount.ToString() + " documenti";

			this.pnlMessage.InnerText=message;
		}

		/// <summary>
		/// Aggiornamento righe relative ai protocolli annullati
		/// </summary>
		private void UpdateRowsProtocolliAnnullati()
		{
			foreach (DataGridItem item in this.grdDocumentiFascicolo.Items)
			{
				string dataAnnullamento=item.Cells[(int) GridColumnsEnum.DataAnnullamento].Text;

				if (!dataAnnullamento.Equals(string.Empty) && 
					!dataAnnullamento.Equals("&nbsp;"))
				{
					foreach (TableCell cell in item.Cells)
						// Modifica del colore del font
						cell.CssClass="invalidItem";
				}
			}
		}

		/// <summary>
		/// Inizializzazione eventi controllo paginazione
		/// </summary>
		private void InitializeEventsListPaging()
		{
			ListPagingNavigationControls listPaging=this.GetListPagingControl();
			listPaging.OnPageChanged+=new DocsPAWA.SitoAccessibile.Paging.ListPagingNavigationControls.PageChangedDelegate(this.listPaging_OnPageChanged);
		}

		/// <summary>
		/// Gestione cambio pagina griglia
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listPaging_OnPageChanged(Object sender,ListPagingNavigationControls.PageChangedEventArgs e)
		{
			this.FetchGridDocumenti(this.GetDocumenti(e.PagingContext));

			this.GetListPagingControl().RefreshPaging(e.PagingContext);
		}

		/// <summary>
		/// Reperimento controllo paginazione
		/// </summary>
		/// <returns></returns>
		private ListPagingNavigationControls GetListPagingControl()
		{
			return this.FindControl("ListPagingNavigationControl") as ListPagingNavigationControls;
		}

		private void grdDocumentiFascicolo_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName.Equals("SHOW_DOCUMENT"))
			{
				this.ShowDetails(e.Item.Cells[(int) GridColumnsEnum.IDProfile].Text,e.Item.Cells[(int) GridColumnsEnum.DocNumber].Text);
			}
		}

		private void grdDocumentiFascicolo_PreRender(object sender, System.EventArgs e)
		{
			this.UpdateRowsProtocolliAnnullati();
		}
	}
}
