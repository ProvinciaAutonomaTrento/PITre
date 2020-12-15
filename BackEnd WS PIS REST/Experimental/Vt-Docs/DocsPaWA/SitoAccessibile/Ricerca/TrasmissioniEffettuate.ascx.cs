namespace DocsPAWA.SitoAccessibile.Ricerca
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DocsPAWA.DocsPaWR;
	using SitoAccessibile.Ricerca;
	using SitoAccessibile.Paging;
	using SitoAccessibile.Trasmissioni;
	using SitoAccessibile.Documenti.Trasmissioni;

	/// <summary>
	///	User control per la gestione della visualizzazione dell'elenco 
	///	delle trasmissioni effettuate dall'utente
	/// </summary>
	public class TrasmissioniEffettuate : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlMessage;
		protected UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid grdTrasmissioniEffettuate;

		/// <summary>
		/// Costanti che indicano le colonne della griglia delle trasmissioni
		/// </summary>
		/// 
		private enum GridColumnsEnum
		{
			ID,
			IDProfile,
			DocNumber,
			IDFascicolo,
			IDRegistro,
			DataInvio,
			Mittente,
			Oggetto,
			Ruolo,
			Documento,
			Fascicolo,
			Dettagli
		}

//		private const int GRID_COL_ID=0;
//		private const int GRID_COL_ID_PROFILE=1;
//		private const int GRID_COL_DOC_NUMBER=2;
//		private const int GRID_COL_ID_FASCICOLO=3;
//		private const int GRID_COL_ID_REGISTRO=4;
//		private const int GRID_COL_DATA_INVIO=5;
//		private const int GRID_COL_MITTENTE=6;
//		private const int GRID_COL_RUOLO=7;
//		private const int GRID_COL_DOCUMENTO=8;
//		private const int GRID_COL_FASCICOLO=9;
//		private const int GRID_COL_OGGETTO=10;
//		private const int GRID_COL_SHOW_DETAILS=11;

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.GetListPagingControl().OnPageChanged += new ListPagingNavigationControls.PageChangedDelegate(this.TrasmissioniEffettuate_OnPageChanged);

			this.HideDetailsTrasmissione();
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			// Impostazione visibilità campi
			this.SetFieldsVisibility();
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
			this.grdTrasmissioniEffettuate.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdTrasmissioniEffettuate_ItemCommand);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		#region Gestione eventi 

		#region Gestione eventi paginazione

		/// <summary>
		/// Evento relativo al cambio pagina della griglia
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void TrasmissioniEffettuate_OnPageChanged(object source,Paging.ListPagingNavigationControls.PageChangedEventArgs e)
		{
			this.FetchTrasmissioni(e.PagingContext);

			((ListPagingNavigationControls) source).RefreshPaging(e.PagingContext);

			this.SetPanelMessageText(e.PagingContext);
		}

		#endregion

		#region Gestione eventi griglia

		private void grdTrasmissioniEffettuate_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName.Equals("SHOW_DETAILS"))
			{
				// Visualizzazzione dettagli trasmissione
				this.ShowDetailsTrasmissione(e.Item.Cells[(int) GridColumnsEnum.ID].Text);
			}
			else if (e.CommandName.Equals("SHOW"))
			{
				this.SetCallerContext();

				if (this.TipoRicercaTrasmissione.Equals(TipiRicercaTrasmissioniEnum.Documenti))
					this.ShowDocument(e.Item.Cells[(int) GridColumnsEnum.IDProfile].Text,e.Item.Cells[(int) GridColumnsEnum.DocNumber].Text);
				else
					this.ShowFascicolo(e.Item.Cells[(int) GridColumnsEnum.IDFascicolo].Text,e.Item.Cells[(int) GridColumnsEnum.IDRegistro].Text);


			}
		}

		#endregion

		#endregion

		public PagingContext PagingContext
		{
			get
			{
				return this.GetListPagingControl().GetPagingContext();
			}
		}

		/// <summary>
		/// Tipologia di ricerca delle trasmissioni
		/// </summary>
		private TipiRicercaTrasmissioniEnum TipoRicercaTrasmissione
		{
			get
			{
				try
				{
					return (TipiRicercaTrasmissioniEnum) Enum.Parse(typeof(TipiRicercaTrasmissioniEnum),this.ViewState["tipoRicercaTrasmissione"].ToString(),true);
				}
				catch
				{
					return TipiRicercaTrasmissioniEnum.Documenti;
				}
			}
			set
			{
				this.ViewState["tipoRicercaTrasmissione"]=value.ToString();
			}
		}

		/// <summary>
		/// Caricamento dati trasmissioni effettuate
		/// </summary>
		/// <param name="filterItem">
		/// Filtro di ricerca delle trasmissioni
		/// </param>
		public void Fetch(TrasmissioniFilterItem filterItem)
		{
			if (filterItem==null)
				throw new ApplicationException("Filtro per la ricerca delle trasmissioni non impostato");

			// Impostazione del filtro fornito in ingresso in sessione
			RicercaTrasmissioniHandler.CurrentFilter=filterItem;

			this.Fetch();
		}

		/// <summary>
		/// Caricamento dati trasmissioni effettuate
		/// </summary>
		public void Fetch()
		{
			if (RicercaTrasmissioniHandler.CurrentFilter==null)
				throw new ApplicationException("Filtro per la ricerca delle trasmissioni non impostato");
			
			// Impostazione della tipologia di ricerca delle trasmissioni effettuate per:
			// documenti o fascicoli
			if (RicercaTrasmissioniHandler.CurrentFilter.TipoOggettoTrasmissione==TipiOggettiTrasmissioniEnum.Fascicoli)
				this.TipoRicercaTrasmissione=TipiRicercaTrasmissioniEnum.Fascicoli;
			else
				this.TipoRicercaTrasmissione=TipiRicercaTrasmissioniEnum.Documenti;

			PagingContext pagingContext=new PagingContext(this.GetInitPageNumber());

			this.FetchTrasmissioni(pagingContext);

			this.GetListPagingControl().RefreshPaging(pagingContext);

			this.SetPanelMessageText(pagingContext);
		}

		/// <summary>
		/// Reperimento numero pagina iniziale
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
		/// Caricamento dati trasmissioni effettuate
		/// </summary>
		private void FetchTrasmissioni(PagingContext pagingContext)
		{	
			RicercaTrasmissioniHandler trasmissioniHandler=new RicercaTrasmissioniHandler();

			Trasmissione[] trasmissioni=trasmissioniHandler.GetTrasmissioni(TipiTrasmissioniEnum.Effettuate,pagingContext,this.GetFiltroRicerca());
			
			// Associazione dati al datagrid
			this.BindGridTrasmissioni(this.TrasmissioniEffettuateToDataset(trasmissioni));
		}

		/// <summary>
		/// Associazione dati al datagrid
		/// </summary>
		/// <param name="ds"></param>
		private void BindGridTrasmissioni(DataSet ds)
		{
			this.grdTrasmissioniEffettuate.DataSource=ds;
			this.grdTrasmissioniEffettuate.DataBind();
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
			dt.Columns.Add("IDProfile",typeof(string));
			dt.Columns.Add("DocNumber",typeof(string));
			dt.Columns.Add("IDFascicolo",typeof(string));
			dt.Columns.Add("IDRegistro",typeof(string));
			dt.Columns.Add("DataInvio",typeof(string));
			dt.Columns.Add("Mittente",typeof(string));
			dt.Columns.Add("Ruolo",typeof(string));
			dt.Columns.Add("Documento",typeof(string));
			dt.Columns.Add("Fascicolo",typeof(string));
			dt.Columns.Add("Oggetto",typeof(string));

			foreach (DocsPAWA.DocsPaWR.Trasmissione trasm in trasmissioni)
			{
				DataRow row=dt.NewRow();
				row["ID"]=trasm.systemId;
				
				if (this.TipoRicercaTrasmissione.Equals(TipiRicercaTrasmissioniEnum.Documenti))
				{
					row["IDProfile"]=trasm.infoDocumento.idProfile;
					row["DocNumber"]=trasm.infoDocumento.docNumber;
					row["Documento"]=SitoAccessibile.Documenti.TipiDocumento.ToString(trasm.infoDocumento);
					row["Oggetto"]=trasm.infoDocumento.oggetto;
				}
				else
				{
					row["IDFascicolo"]=trasm.infoFascicolo.idFascicolo;
					row["IDRegistro"]=trasm.infoFascicolo.idRegistro;
					row["Fascicolo"]=trasm.infoFascicolo.codice + Environment.NewLine + Environment.NewLine + trasm.infoFascicolo.descrizione;
				}
				
				row["DataInvio"]=trasm.dataInvio;
				row["Mittente"]=trasm.utente.descrizione;
				row["Ruolo"]=trasm.ruolo.descrizione;

				dt.Rows.Add(row);
			}
			
			ds.Tables.Add(dt);

			return ds;
		}

		/// <summary>
		/// Reperimento filtri di ricerca trasmissioni dalla sessione
		/// </summary>
		/// <returns></returns>
		private FiltroRicerca[] GetFiltroRicerca()
		{
			// Reperimento parametri di filtro di ricerca
			if (Ricerca.RicercaTrasmissioniHandler.CurrentFilter!=null)
				return Ricerca.RicercaTrasmissioniHandler.CurrentFilter.ToFiltriRicerca();
			else
				return new FiltroRicerca[1];
		}

		/// <summary>
		/// Visualizzazione dettagli trasmissione
		/// </summary>
		/// <param name="idTrasmissione"></param>
		private void ShowDetailsTrasmissione(string idTrasmissione)
		{
			DettTrasmEffettuate details=this.GetControlDetailsTrasmissione();
			details.Visible=true;
			details.Initialize(idTrasmissione);
		}

		/// <summary>
		/// Visualizzazione del documento
		/// </summary>
		/// <param name="idProfile"></param>
		/// <param name="docNumber"></param>
		private void ShowDocument(string idProfile,string docNumber)
		{
			string url=EnvironmentContext.RootPath + "Documenti/DettagliDocumento.aspx?iddoc=" + idProfile + "&docnum=" + docNumber;

			Response.Redirect(url);
		}

		/// <summary>
		/// Visualizzazione del fascicolo
		/// </summary>
		/// <param name="idFascicolo"></param>
		/// <param name="idRegistro"></param>
		private void ShowFascicolo(string idFascicolo,string idRegistro)
		{
			string url=EnvironmentContext.RootPath + "Fascicoli/Fascicolo.aspx?idFascicolo=" + idFascicolo + "&idRegistro=" + idRegistro; 

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
		/// Impostazione visibilita campi UI
		/// </summary>
		private void SetFieldsVisibility()
		{
			bool isVisible=(this.grdTrasmissioniEffettuate.Items.Count>0);

			this.grdTrasmissioniEffettuate.Visible=isVisible;
			this.GetListPagingControl().Visible=isVisible;

			bool ricercaDocumenti=(this.TipoRicercaTrasmissione.Equals(TipiRicercaTrasmissioniEnum.Documenti));

			this.grdTrasmissioniEffettuate.Columns[(int) GridColumnsEnum.Documento].Visible=ricercaDocumenti;
			this.grdTrasmissioniEffettuate.Columns[(int) GridColumnsEnum.Oggetto].Visible=ricercaDocumenti;
			this.grdTrasmissioniEffettuate.Columns[(int) GridColumnsEnum.Fascicolo].Visible=!ricercaDocumenti;

			foreach (DataGridItem item in this.grdTrasmissioniEffettuate.Items)
			{
				Button btn=item.Cells[(int) GridColumnsEnum.Dettagli].FindControl("btnShow") as Button;
				if (btn!=null)
				{
					if (ricercaDocumenti)
						btn.Text="Documento";
					else
						btn.Text="Fascicolo";
				}				
			}
		}

		/// <summary>
		/// Reperimento controllo dettagli trasmissione effettuata
		/// </summary>
		/// <returns></returns>
		private DettTrasmEffettuate GetControlDetailsTrasmissione()
		{
			return this.FindControl("DettTrasmEffettuate") as DettTrasmEffettuate;
		}

		/// <summary>
		/// Hide del controllo dettagli trasmissione effettuata
		/// </summary>
		private void HideDetailsTrasmissione()
		{
			this.GetControlDetailsTrasmissione().Visible=false;
		}

		/// <summary>
		/// Impostazione messaggio
		/// </summary>
		/// <param name="pagingContext"></param>
		private void SetPanelMessageText(PagingContext pagingContext)
		{	
			if (pagingContext.RecordCount==0)
				this.pnlMessage.InnerText="Nessuna trasmissione trovata";
			else
				this.pnlMessage.InnerText="Trovate " + pagingContext.RecordCount.ToString() + " trasmissioni";
		}

		/// <summary>
		/// Reperimento controllo navigazione
		/// </summary>
		/// <returns></returns>
		private ListPagingNavigationControls GetListPagingControl()
		{
			return this.FindControl("ListPagingNavigation") as ListPagingNavigationControls;
		}
	}
}