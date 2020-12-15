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
using DocsPAWA.SitoAccessibile.Trasmissioni;
using DocsPAWA.SitoAccessibile.Paging;
using DocsPAWA.SitoAccessibile.Documenti;
				
namespace DocsPAWA.SitoAccessibile.Documenti.Trasmissioni
{
	/// <summary>
	/// Summary description for Trasmissioni.
	/// </summary>
	public class DettaglioTrasmissioni : SessionWebPage
	{
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.WebControls.Button btnBack;
	
		protected System.Web.UI.WebControls.RadioButtonList RadioButtonList1;
		protected System.Web.UI.WebControls.Button btnSearch;
		protected System.Web.UI.WebControls.RadioButtonList rblSearchType;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlNoTrasm;
		protected System.Web.UI.HtmlControls.HtmlAnchor skipTrasmissioni;
		protected UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid grdTrasmissioni;
		protected UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid grdTrasmissioniRicevute;
		
		protected string idProfile;
		protected string docNum;
		protected string activeMenu;
		private DocsPAWA.DocsPaWR.SchedaDocumento _schedaDocumento=null;

		private const int GRID_EFF_COL_ID=0;
		private const int GRID_EFF_COL_DATA_INVIO=1;
		private const int GRID_EFF_COL_DATA_UTENTE=2;
		private const int GRID_EFF_COL_DATA_RUOLO=3;
		private const int GRID_EFF_COL_DATA_AZIONI=4;

		private const int GRID_RIC_COL_ID=0;

		/// <summary>
		/// Constante che indica la chiave relativa al comando 
		/// di visualizzazione dei dettagli di una trasmissione
		/// </summary>
		private const string SHOW_DETAILS_COMMAND_NAME="SHOW_DETAILS";

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				ListPagingNavigationControls ctl=this.GetPagingNavigationControls();
				ctl.OnPageChanged+=new DocsPAWA.SitoAccessibile.Paging.ListPagingNavigationControls.PageChangedDelegate(this.GridPaging_OnPageChanged);

				if (!this.IsPostBack)
				{
					// Impostazione valori di default
					this.SetDefaultValues();
				}

				// Reperimento parametri query string
				this.idProfile=this.GetQueryStringParameter("iddoc");
				this.docNum=this.GetQueryStringParameter("docnum");
				this.activeMenu=this.GetQueryStringParameter("activemenu");

				if (idProfile!=string.Empty && docNum!=string.Empty)
				{
					DocumentoHandler handler=new DocumentoHandler();
					this._schedaDocumento=handler.GetDocumento(this.idProfile,this.docNum);

					this.InitializeControlMenuDocumento();

					// Caricamento dettagli generali del documento
					this.GetDettagliGenerali().Initialize(this._schedaDocumento,true);

					if (!this.IsPostBack)
						this.Fetch();
				}
				else
				{
					throw new ApplicationException("Parametri mancanti, impossibile caricare il documento");
				}
			}
			catch(Exception ex)
			{
				ErrorManager.redirect(this,ex);
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			this.grdTrasmissioni.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdTrasmissioni_ItemCommand);
			this.grdTrasmissioniRicevute.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdTrasmissioniRicevute_ItemCommand);
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region Gestione eventi 

		#region Gestione eventi paginazione

		/// <summary>
		/// Evento relativo al cambio pagina della griglia
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GridPaging_OnPageChanged(Object sender,ListPagingNavigationControls.PageChangedEventArgs e)
		{
			this.FetchTrasmissioni(this.GetSelectedSearchType(),e.PagingContext);
		}

		#endregion

		#region Gestione eventi griglia effettuate

		private void grdTrasmissioni_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName.Equals(SHOW_DETAILS_COMMAND_NAME))
			{	
				this.ShowDetailsTrasmissioneEffettuata(e.Item.Cells[GRID_EFF_COL_ID].Text);
			}
		}

		#endregion

		#region Gestione eventi griglia ricevute

		private void grdTrasmissioniRicevute_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName.Equals(SHOW_DETAILS_COMMAND_NAME))
			{
				this.ShowDetailsTrasmissioneRicevuta(e.Item.Cells[GRID_RIC_COL_ID].Text);
			}
		}

		#endregion

		#region Gestione eventi submit buttons

		private void btnSearch_Click(object sender, System.EventArgs e)
		{	
			// Reperimento contesto di paginazione
			PagingContext pagingContext=new PagingContext(1);

			this.FetchTrasmissioni(this.GetSelectedSearchType(),pagingContext);

			this.GetPagingNavigationControls().RefreshPaging(pagingContext);
		}

		private void btnBack_Click(object sender, System.EventArgs e)
		{
			Response.Redirect(this.GetCallerUrl());
		}

		private string GetCallerUrl()
		{
			CallerContext callerContext=CallerContext.GetCallerContext();

			string retValue=string.Empty;

			if (callerContext==null)
			{
				retValue=this.BackUrl;
			}
			else
			{
				callerContext.AdditionalParameters.Add("searchPage",callerContext.PageNumber);
				retValue=callerContext.Url;
			}

			return retValue;
		}

		#endregion

		#endregion

		#region Gestione usercontrols

		/// <summary>
		/// Reperimento usercontrol dei dettagli generali del documento
		/// </summary>
		/// <returns></returns>
		private DettagliGenerali GetDettagliGenerali()
		{
			return this.FindControl("DatiGeneraliDocumento") as DettagliGenerali;
		}

		/// <summary>
		/// Reperimento controllo paginazione
		/// </summary>
		/// <returns></returns>
		private ListPagingNavigationControls GetPagingNavigationControls()
		{
			string controlID=string.Empty;
			if (this.GetSelectedSearchType()==SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum.Effettuate)
				controlID="pagingTrasmEffettuate";
			else
				controlID="pagingTrasmRicevute";

			return this.FindControl(controlID) as ListPagingNavigationControls;
		}

		/// <summary>
		/// Impostazione dei controlli di paginazione nascosti
		/// </summary>
		private void HidePagingControls()
		{
			this.FindControl("pagingTrasmEffettuate").Visible=false;
			this.FindControl("pagingTrasmRicevute").Visible=false;
		}

		/// <summary>
		/// Inizializzazione controllo menu documento
		/// </summary>
		private void InitializeControlMenuDocumento()
		{
			MenuDocumento menuDocumento=this.FindControl("MenuDocumento") as MenuDocumento;

			if (menuDocumento!=null)
				menuDocumento.Initialize(MenuDocumento.ItemsMenuDocumentoEnum.Trasmissioni,this._schedaDocumento);
		}

		#endregion

		#region Gestione dati

		/// <summary>
		/// Impostazione parametri di default
		/// </summary>
		private void SetDefaultValues()
		{
			this.rblSearchType.SelectedIndex=0;
		}

		/// <summary>
		/// Caricamento dati
		/// </summary>
		private void Fetch()
		{
			ListPagingNavigationControls ctl=this.GetPagingNavigationControls();

			// Reperimento contesto di paginazione
			PagingContext pagingContext=ctl.GetPagingContext();

			// Caricamento dati trasmissioni
			this.FetchTrasmissioni(this.GetSelectedSearchType(),pagingContext);

			ctl.RefreshPaging(pagingContext);
		}

		/// <summary>
		/// Caricamento dati trasmissioni
		/// </summary>
		/// <param name="searchType"></param>
		/// <param name="pagingContext"></param>
		private void FetchTrasmissioni(SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum tipoTrasmissione,PagingContext pagingContext)
		{
			TrasmissioniHandler trasmissioniHandler=new TrasmissioniHandler();
			
			DocsPaWR.Trasmissione[] trasmissioni=trasmissioniHandler.GetTrasmissioniDocumento(tipoTrasmissione,pagingContext,this._schedaDocumento);

			DataSet convertedDataSet=null;
			DataGrid dataGrid=null;

			if (tipoTrasmissione==SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum.Effettuate)
			{
				convertedDataSet=this.TrasmissioniEffettuateToDataset(trasmissioni);
				dataGrid=this.grdTrasmissioni;
			}
			else
			{
				convertedDataSet=this.TrasmissioniRicevuteToDataset(trasmissioni);
				dataGrid=this.grdTrasmissioniRicevute;
			}

			this.BindGridTrasmissioni(dataGrid,convertedDataSet);

			this.SetFieldsVisibility();
		}

		private void BindGridTrasmissioni(DataGrid gridTrasmissioni,DataSet ds)
		{
			gridTrasmissioni.DataSource=ds;
			gridTrasmissioni.DataBind();
		}

		/// <summary>
		/// Creazione dataset contenente i dati delle trasm effettuate
		/// </summary>
		/// <param name="trasmissioni"></param>
		/// <returns></returns>
		private DataSet TrasmissioniEffettuateToDataset(DocsPAWA.DocsPaWR.Trasmissione[] trasmissioni)
		{
			DataSet ds=new DataSet("DatasetTrasmissioniEffettuate");
			DataTable dt=new DataTable("TableTrasmissioniEffettuate");

			dt.Columns.Add("ID",typeof(string));
			dt.Columns.Add("DataInvio",typeof(string));
			dt.Columns.Add("Utente",typeof(string));
			dt.Columns.Add("Ruolo",typeof(string));

			foreach (DocsPAWA.DocsPaWR.Trasmissione trasm in trasmissioni)
			{
				DataRow row=dt.NewRow();

				row["ID"]=trasm.systemId;
				row["DataInvio"]=trasm.dataInvio;
				row["Utente"]=trasm.utente.descrizione;
				row["Ruolo"]=trasm.ruolo.descrizione;

				dt.Rows.Add(row);
			}
			
			ds.Tables.Add(dt);

			return ds;
		}

		/// <summary>
		/// Creazione dataset contenente i dati delle trasm ricevute
		/// </summary>
		/// <param name="trasmissioni"></param>
		/// <returns></returns>
		private DataSet TrasmissioniRicevuteToDataset(DocsPAWA.DocsPaWR.Trasmissione[] trasmissioni)
		{
			DataSet ds=new DataSet("DatasetTrasmissioniEffettuate");
			DataTable dt=new DataTable("TableTrasmissioniEffettuate");

			dt.Columns.Add("ID",typeof(string));
			dt.Columns.Add("DataInvio",typeof(string));
			dt.Columns.Add("Mittente",typeof(string));
			dt.Columns.Add("Ruolo",typeof(string));
			dt.Columns.Add("Ragione",typeof(string));
			dt.Columns.Add("DataScadenza",typeof(string));

			foreach (DocsPAWA.DocsPaWR.Trasmissione trasm in trasmissioni)
			{
				DataRow row=dt.NewRow();
				row["ID"]=trasm.systemId;
				row["DataInvio"]=trasm.dataInvio;
				row["Mittente"]=trasm.utente.descrizione;
				row["Ruolo"]=trasm.ruolo.descrizione;

				string ragione=string.Empty;
				string dataScadenza=string.Empty;

				if (trasm.trasmissioniSingole.Length>0)
				{
					DocsPaWR.TrasmissioneSingola trasmSingola=trasm.trasmissioniSingole[0];
					ragione=trasmSingola.ragione.descrizione;
					dataScadenza=trasmSingola.dataScadenza;
				}

				row["Ragione"]=ragione;
				row["DataScadenza"]=dataScadenza;

				dt.Rows.Add(row);
			}
			
			ds.Tables.Add(dt);

			return ds;
		}

		/// <summary>
		/// Impostazione visibilità campi successivamente alla ricerca
		/// </summary>
		/// <param name="isVisible"></param>
		private void SetFieldsVisibility()
		{
			SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum tipoTrasmissione=this.GetSelectedSearchType();
			
			this.grdTrasmissioni.Visible=(tipoTrasmissione==SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum.Effettuate);
			this.grdTrasmissioniRicevute.Visible=(tipoTrasmissione==SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum.Ricevute);
			
			DataGrid gridVisible=null;
			if (this.grdTrasmissioni.Visible)
				gridVisible=this.grdTrasmissioni;
			else
				gridVisible=this.grdTrasmissioniRicevute;

			bool isVisible=(gridVisible.Items.Count > 0);

			gridVisible.Visible=isVisible;
			this.pnlNoTrasm.Visible=!gridVisible.Visible;

			this.HidePagingControls();
			this.GetPagingNavigationControls().Visible=gridVisible.Visible;

			this.HideDetailsControls();
			if (tipoTrasmissione==SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum.Effettuate)
				this.GetDetailsTrasmissioneEffettuata().Visible=false;
			else
				this.GetDetailsTrasmissioneRicevuta().Visible=false;

			if (isVisible)
				gridVisible.Caption = "Elenco di " + gridVisible.Items.Count.ToString() + " trasmissioni trovate";
			else 
				this.skipTrasmissioni.Visible = false;
		}

		/// <summary>
		/// Reperimento tipologia ricerca trasmissioni
		/// </summary>
		/// <returns></returns>
		private SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum GetSelectedSearchType()
		{
			SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum retValue=SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum.Effettuate;

			try
			{
				retValue=(SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum) Enum.Parse(typeof(SitoAccessibile.Trasmissioni.TipiTrasmissioniEnum),this.rblSearchType.SelectedItem.Value,true);
			}
			catch
			{
			}

			return retValue;
		}

		#endregion

		#region Gestione dettagli trasmissione

		private void HideDetailsControls()
		{
			this.GetDetailsTrasmissioneEffettuata().Visible=false;
			this.GetDetailsTrasmissioneRicevuta().Visible=false;
		}

		private DettTrasmEffettuate GetDetailsTrasmissioneEffettuata()
		{
			return this.FindControl("DettTrasmEffettuate") as DettTrasmEffettuate;
		}

		private DettTrasmRicevute GetDetailsTrasmissioneRicevuta()
		{
			return this.FindControl("DettTrasmRicevute") as DettTrasmRicevute;
		}

		private void ShowDetailsTrasmissioneRicevuta(string idTrasmissione)
		{
			DettTrasmRicevute trasmissioniRicevute=this.GetDetailsTrasmissioneRicevuta();
			trasmissioniRicevute.Visible=true;
			trasmissioniRicevute.Initialize(idTrasmissione);
		}

		private void ShowDetailsTrasmissioneEffettuata(string idTrasmissione)
		{
			DettTrasmEffettuate trasmissioniEffettuate=this.GetDetailsTrasmissioneEffettuata();
			trasmissioniEffettuate.Visible=true;
			trasmissioniEffettuate.Initialize(idTrasmissione);
		}

		#endregion
	}
}
