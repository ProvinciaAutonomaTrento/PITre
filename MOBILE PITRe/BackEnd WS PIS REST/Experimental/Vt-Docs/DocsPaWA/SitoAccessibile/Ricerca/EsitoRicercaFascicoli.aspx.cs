using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Paging;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// 
	/// </summary>
	public class EsitoRicercaFascicoli : SessionWebPage
	{
		protected UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid grdEsitoRicercaFascicoli;
		protected System.Web.UI.WebControls.Button btnBack;
	
		/// <summary>
		/// Costanti che identificano le colonne del datagrid
		/// </summary>
		private const int COL_ID=0;
		private const int COL_TIPO=1;
		private const int COL_CODICE=2;
		private const int COL_DESCRIZIONE=3;
		private const int COL_DATA_APERTURA=4;
		private const int COL_DATA_CHIUSURA=5;
		private const int COL_DETTAGLI=6;

		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlMessage;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// Inizializzazione eventi controllo paginazione
				this.InitializeEventsListPaging();

				if (!this.IsPostBack)
				{
					// Caricamento dati
					this.Fetch();
				}
			}
			catch (Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			this.SetControlsVisibility();

			this.ShowMessage();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.grdEsitoRicercaFascicoli.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdEsitoRicercaFascicoli_ItemCommand);
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		/// <summary>
		/// Reperimento numero pagina inizialie
		/// </summary>
		/// <returns></returns>
		private int GetInitPageNumber()
		{
			int pageNumber=1;

			string searchPage=Request.QueryString["searchPage"];
			
			if (searchPage!=null && searchPage!=string.Empty)
			{
				try
				{
					pageNumber=Convert.ToInt32(searchPage);
				}
				catch
				{
				}
			}

			return pageNumber;
		}

		/// <summary>
		/// Caricamento dati ricerca fascicoli
		/// </summary>
		private void Fetch()
		{
			PagingContext pagingContext=new PagingContext(this.GetInitPageNumber());

			this.FetchGridFascicoli(this.Search(pagingContext));

			this.GetListPagingControl().RefreshPaging(pagingContext);
		}

		/// <summary>
		/// Caricamento griglia fascicoli
		/// </summary>
		/// <param name="fascicoli"></param>
		private void FetchGridFascicoli(Fascicolo[] fascicoli)
		{
			this.grdEsitoRicercaFascicoli.DataSource=this.FascicoliToDataset(fascicoli);
			this.grdEsitoRicercaFascicoli.DataBind();
		}

		/// <summary>
		/// Creazione dataset per la visualizzazione dei fascicoli
		/// </summary>
		/// <param name="fascicoli"></param>
		/// <returns></returns>
		private DataSet FascicoliToDataset(Fascicolo[] fascicoli)
		{
			DataSet ds=new DataSet();
			DataTable dt=new DataTable();

			dt.Columns.Add("ID",typeof(string));
			dt.Columns.Add("Tipo",typeof(string));
			dt.Columns.Add("Codice",typeof(string));
			dt.Columns.Add("Descrizione",typeof(string));
			dt.Columns.Add("DataApertura",typeof(string));
			dt.Columns.Add("DataChiusura",typeof(string));

			foreach (Fascicolo fascicolo in fascicoli)
			{
				DataRow row=dt.NewRow();

				row["ID"]=fascicolo.systemID;
				row["Tipo"]=SitoAccessibile.Fascicoli.TipiFascicolo.GetDescrizione(fascicolo.tipo);
				row["Codice"]=fascicolo.codice;
				row["Descrizione"]=fascicolo.descrizione;
				row["DataApertura"]=fascicolo.apertura;
				row["DataChiusura"]=fascicolo.chiusura;

				dt.Rows.Add(row);
			}
			
			ds.Tables.Add(dt);

			return ds;
		}

		/// <summary>
		/// Avvio ricerca fascicoli
		/// </summary>
		/// <param name="pagingContext"></param>
		/// <returns></returns>
		private Fascicolo[] Search(PagingContext pagingContext)
		{
			return RicercaFascicoliHandler.SearchFascicoli(RicercaFascicoliHandler.CurrentFilter,pagingContext);
		}

		/// <summary>
		/// Visualizzazione dettagli del fascicolo
		/// </summary>
		/// <param name="idFascicolo"></param>
		/// <param name="idRegistro"></param>
		private void ShowDetails(string idFascicolo)
		{
			this.SetCallerContext();

			int pageNumber=this.GetListPagingControl().GetPagingContext().PageNumber;

			string idRegistro=string.Empty;
			if (RicercaFascicoliHandler.CurrentFilter!=null)
				idRegistro=RicercaFascicoliHandler.CurrentFilter.IDRegistro;

			Response.Redirect(EnvironmentContext.RootPath + "Fascicoli/Fascicolo.aspx?idFascicolo=" + idFascicolo + "&idRegistro=" + idRegistro);
		}

		/// <summary>
		/// Impostazione del contesto del chiamante
		/// </summary>
		private void SetCallerContext()
		{
			CallerContext context=CallerContext.NewContext(this.Page.Request.Url.AbsoluteUri);
			context.PageNumber=this.GetListPagingControl().GetPagingContext().PageNumber;
		}

		/// <summary>
		/// Inizializzazione eventi controllo paginazione
		/// </summary>
		private void InitializeEventsListPaging()
		{
			ListPagingNavigationControls listPaging=this.GetListPagingControl();
			listPaging.OnPageChanged+=new DocsPAWA.SitoAccessibile.Paging.ListPagingNavigationControls.PageChangedDelegate(listPaging_OnPageChanged);
		}

		/// <summary>
		/// Impostazione visibilità controlli
		/// </summary>
		private void SetControlsVisibility()
		{
			int documentsCount=this.GetListPagingControl().GetPagingContext().RecordCount;

			this.grdEsitoRicercaFascicoli.Visible=(documentsCount>0);
			this.GetListPagingControl().Visible=this.grdEsitoRicercaFascicoli.Visible;
		}

		/// <summary>
		/// Visualizzazione messaggio relativo ai documenti trovati
		/// </summary>
		private void ShowMessage()
		{
			int documentsCount=this.GetListPagingControl().GetPagingContext().RecordCount;

			string message=string.Empty;

			if (documentsCount==0)
				message="Nessun fascicolo trovato";
			else if (documentsCount==1)
				message="Trovato 1 fascicolo";
			else
				message="Trovati " + documentsCount.ToString() + " fascicoli";

			this.pnlMessage.InnerText=message;
		}

		/// <summary>
		/// Gestione cambio pagina griglia
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listPaging_OnPageChanged(Object sender,ListPagingNavigationControls.PageChangedEventArgs e)
		{
			this.FetchGridFascicoli(this.Search(e.PagingContext));

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
	
		private void grdEsitoRicercaFascicoli_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName.Equals("SHOW_DETAILS"))
			{
				this.ShowDetails(e.Item.Cells[COL_ID].Text);
			}
		}

		private void btnBack_Click(object sender, System.EventArgs e)
		{
			Response.Redirect(EnvironmentContext.RootPath + "Ricerca/Fascicoli.aspx");
		}
	}
}
