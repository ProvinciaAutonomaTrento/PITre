namespace DocsPAWA.SitoAccessibile.Ricerca
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DocsPAWA.DocsPaWR;
	using DocsPAWA.SitoAccessibile.Paging;
	

	/// <summary>
	///		Summary description for DocumentiGrigi.
	/// </summary>
	public class ListaDocumentiGrigi : ListaDocumenti
	{
		protected UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid grdDocumentiGrigi;
	
		private const int GRID_COL_ID_DOCUMENTO=0;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlMessage;
		private const int GRID_COL_DOC_NUMBER=1;

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
		/// Reperimento filtri di ricerca per i documenti grigi
		/// </summary>
		/// <returns></returns>
		protected override FiltroRicerca[] GetFiltriRicerca()
		{
			ArrayList list=new ArrayList(base.GetFiltriRicerca());
			
			if (RicercaDocumentiHandler.CurrentFilter.DocumentiGrigi)
			{
				// Ricerca dei documenti grigi solamente se richiesto
				FiltroRicerca filterItem=new FiltroRicerca();
				filterItem.argomento = DocsPAWA.DocsPaWR.FiltriDocumento.TIPO.ToString();
				filterItem.valore = "G";
				list.Add(filterItem);
			}

			return (FiltroRicerca[]) list.ToArray(typeof(FiltroRicerca));
		}

		/// <summary>
		/// Creazione oggetto dataset contenente i dati dei documenti da visualizzare
		/// </summary>
		/// <param name="documenti"></param>
		/// <returns></returns>
		protected override DataSet DocumentiToDataset(InfoDocumento[] documenti)
		{
			DataSet ds=new DataSet("DatasetDocumentiGrigi");
			DataTable dt=new DataTable("TableDocumentiGrigi");

			dt.Columns.Add("ID",typeof(string));
			dt.Columns.Add("DocNumber",typeof(string));
			dt.Columns.Add("Documento",typeof(string));
			dt.Columns.Add("Oggetto",typeof(string));

			foreach (DocsPAWA.DocsPaWR.InfoDocumento infoDocumento in documenti)
			{
				DataRow row=dt.NewRow();
				row["ID"]=infoDocumento.idProfile;
				row["DocNumber"]=infoDocumento.docNumber;
				row["Documento"]=SitoAccessibile.Documenti.TipiDocumento.ToString(infoDocumento);
				row["Oggetto"]=infoDocumento.oggetto;
				dt.Rows.Add(row);
			}
			
			ds.Tables.Add(dt);

			return ds;
		}

		/// <summary>
		/// Restituzione controllo datagrid
		/// </summary>
		/// <returns></returns>
		protected override DataGrid GetControlDatagrid()
		{
			return this.grdDocumentiGrigi;
		}

		/// <summary>
		/// Impostazione messaggio
		/// </summary>
		/// <param name="message"></param>
		protected override void RefreshMessage()
		{
			PagingContext pagingContext=this.GetControlPaging().GetPagingContext();

			string message=string.Empty;

			if (pagingContext.RecordCount==0)
				message="Nessun documento grigio trovato";
			else if (pagingContext.RecordCount==1)
				message="Trovato 1 documento grigio";
			else
				message="Trovati " + pagingContext.RecordCount.ToString() + " documenti grigi";

			this.pnlMessage.InnerText=message;
		}

		/// <summary>
		/// Restituzione controllo navigazione
		/// </summary>
		/// <returns></returns>
		protected override ListPagingNavigationControls GetControlPaging()
		{
			return this.FindControl("listPagingNavigation") as ListPagingNavigationControls;
		}

		protected override void OnGridItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName.Equals("SHOW_DOCUMENT"))
				this.ShowDocument(e.Item.Cells[GRID_COL_ID_DOCUMENTO].Text,e.Item.Cells[GRID_COL_DOC_NUMBER].Text);
		}
	}
}
