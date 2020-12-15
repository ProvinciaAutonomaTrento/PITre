namespace DocsPAWA.SitoAccessibile.Documenti
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using DocsPAWA.DocsPaWR;

	/// <summary>
	///		Summary description for VisibilitaDocumento.
	/// </summary>
	public class DettagliVisibilitaDocumento : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label lblUsers;
		protected System.Web.UI.WebControls.Label lblUsersPrefix;
		protected UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid grdVisibilita;

		private const int GRID_COL_CODICE_RUBRICA=0;
		private const int GRID_COL_RUOLO_UTENTE=1;
		private const int GRID_COL_DIRITTO=2;
		private const int GRID_COL_TIPO=3;
		private const int GRID_COL_DETTAGLI=4;

		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlListUsers;
		
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			this.pnlListUsers.Visible = false;
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			// Impostazione visibilità controlli UI
			this.SetControlsVisibility();
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
			this.grdVisibilita.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdVisibilita_ItemCommand);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		/// <summary>
		/// Impostazione visibilità controlli UI
		/// </summary>
		private void SetControlsVisibility()
		{
			foreach (DataGridItem item in this.grdVisibilita.Items)
			{
				Button btn=item.Cells[GRID_COL_TIPO].FindControl("btnShowDetails") as Button;

				if (btn!=null)
					btn.Visible=(item.Cells[GRID_COL_TIPO].Text!="UTENTE");
			}
		}

		private void grdVisibilita_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName=="SHOW_DETAILS")
			{	
				this.FetchUsers(e.Item.Cells[GRID_COL_CODICE_RUBRICA].Text);

				this.ShowUsers();
			}
		}

		#region Gestione dati

		public void Fetch(string idProfile)
		{
			if (!this.IsPostBack)
			{
				DettagliVisibilitaDocumentoHandler dettagliHandler=new DettagliVisibilitaDocumentoHandler();
				
				DocumentoDiritto[] list=dettagliHandler.GetDettagliVisibilita(idProfile);

				this.BindGridVisibilita(this.DirittiDocumentoToDataset(list));
			}
		}

		/// <summary>
		/// Reperimento utenti facenti parte del ruolo selezionato
		/// </summary>
		/// <param name="codiceRubrica"></param>
		private void FetchUsers(string codiceRubrica)
		{
			DettagliVisibilitaDocumentoHandler dettagliHandler=new DettagliVisibilitaDocumentoHandler();
			ArrayList listCorrispondenti=dettagliHandler.GetUserList(codiceRubrica);

			string users=string.Empty;

			foreach (string corrispondente in listCorrispondenti)
			{
				if (users!=string.Empty)
					users+=",&nbsp;";

				users+=corrispondente;
			}

			this.lblUsers.Text=users;

			this.pnlListUsers.Visible=(listCorrispondenti.Count>0);
			this.lblUsersPrefix.Visible=this.pnlListUsers.Visible;
			this.lblUsers.Visible=this.lblUsersPrefix.Visible;
		}

		private void ShowUsers()
		{
			this.pnlListUsers.Visible=true;
		}

		/// <summary>
		/// Associazione dati griglia
		/// </summary>
		/// <param name="ds"></param>
		private void BindGridVisibilita(DataSet ds)
		{
			this.grdVisibilita.DataSource=ds;
			this.grdVisibilita.DataBind();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="documentiDiritti"></param>
		/// <returns></returns>
		private DataSet DirittiDocumentoToDataset(DocumentoDiritto[] documentiDiritti)
		{
			DataSet ds=new DataSet("DatasetDirittiVisibilita");
			DataTable dt=new DataTable("TableDirittiVisibilita");

			dt.Columns.Add("CodiceRubrica",typeof(string));
			dt.Columns.Add("RuoloUtente",typeof(string));
			dt.Columns.Add("Diritto",typeof(string));
			dt.Columns.Add("Tipo",typeof(string));

			foreach (DocumentoDiritto diritto in documentiDiritti)
			{
				DataRow row=dt.NewRow();

				row["CodiceRubrica"]=diritto.soggetto.codiceRubrica;
				row["RuoloUtente"]=diritto.soggetto.descrizione;
				row["Diritto"]=DettagliVisibilitaDocumentoHandler.GetTipoDiritto(diritto);
				row["Tipo"]=DettagliVisibilitaDocumentoHandler.GetTipoCorrispondente(diritto);

				dt.Rows.Add(row);
			}
			
			ds.Tables.Add(dt);

			return ds;
		}

		#endregion
	}
}