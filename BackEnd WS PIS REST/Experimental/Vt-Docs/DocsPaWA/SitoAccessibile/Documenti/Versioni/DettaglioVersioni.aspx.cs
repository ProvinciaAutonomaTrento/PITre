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
using UniSA.FLC.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SitoAccessibile.Documenti;

namespace DocsPAWA.SitoAccessibile.Documenti.Versioni
{
	/// <summary>
	/// Summary description for _Menu.
	/// </summary>
	public class DettaglioVersioni : SessionWebPage
	{
		protected System.Web.UI.HtmlControls.HtmlForm Form1;

		//protected System.Web.UI.WebControls.DataGrid grdVersioni;
		protected AccessibleDataGrid grdVersioni;
		protected ArrayList Dt_elem;
		protected System.Web.UI.WebControls.Label lbl_message;
		//protected System.Web.UI.WebControls.Label lbl_ADL;
		protected DocsPAWA.DocsPaWR.Documento[] ListaDocVersioni; 

		protected string idProfile;
		protected string docNum;
		protected System.Web.UI.WebControls.Button btnBack;
		protected System.Web.UI.WebControls.Panel azioniAllegati;
		protected System.Web.UI.WebControls.HyperLink docLink;
		protected DocsPAWA.DocsPaWR.SchedaDocumento _schedaDocumento=null;

		/// <summary>
		/// Costanti che identificano gli indici delle colonne del datagrid
		/// </summary>
		private const int COL_VERSIONE=0;
		private const int COL_NOTE=1;
		private const int COL_DATA=2;
		private const int COL_VERSION_ID=3;
		private const int COL_ACQUISITO=4;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlVersioniMsg;
		private const int COL_VISUALIZZA=5;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// Reperimento parametri query string
				this.idProfile=this.GetQueryStringParameter("iddoc");
				this.docNum=this.GetQueryStringParameter("docnum");
				
				if (idProfile!=string.Empty && docNum!=string.Empty)
				{
					this._schedaDocumento=DocumentManager.getDettaglioDocumento(this,this.idProfile,this.docNum);
					
					this.InitializeControlMenuDocumento();

					// Caricamento dati
					this.Fetch();

					// Impostazione visibilità campi
					this.SetFieldsVisibility();
				}
				else
				{
					throw new ApplicationException("Parametri mancanti");
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
			
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.grdVersioni.PreRender += new System.EventHandler(this.grdVersioni_PreRender);
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion


		/// <summary>
		/// Inizializzazione controllo menu documento
		/// </summary>
		private void InitializeControlMenuDocumento()
		{
			MenuDocumento menuDocumento=this.FindControl("MenuDocumento") as MenuDocumento;

			if (menuDocumento!=null)
				menuDocumento.Initialize(MenuDocumento.ItemsMenuDocumentoEnum.Versioni,this._schedaDocumento);
		}

		/// <summary>
		/// Reperimento usercontrol dei dettagli generali del documento
		/// </summary>
		/// <returns></returns>
		private DettagliGenerali GetDettagliGenerali()
		{
			return this.FindControl("DatiGeneraliDocumento") as DettagliGenerali;
		}

		/// <summary>
		/// Caricamento dati
		/// </summary>
		private void Fetch()
		{
			// Caricamento dettagli generali del documento
			this.GetDettagliGenerali().Initialize(this._schedaDocumento,true);
			
			// Caricamento dati versioni
			this.FetchVersioni();
		}

		/// <summary>
		/// Caricamento dati versioni
		/// </summary>
		private void FetchVersioni()
		{
			DataSet ds=this.VersioniToDataset(this._schedaDocumento.documenti);

			this.grdVersioni.DataSource=ds;
			this.grdVersioni.DataBind();
		}

		private DataSet VersioniToDataset(Documento[] versioni)
		{
			DataSet ds=new DataSet();
			DataTable dt=new DataTable();

			dt.Columns.Add("Versione",typeof(string));
			dt.Columns.Add("Note",typeof(string));
			dt.Columns.Add("Data",typeof(string));
			dt.Columns.Add("VersionID",typeof(string));
			dt.Columns.Add("Acquisito",typeof(bool));

			VersioniHandler versioniHandler=new VersioniHandler();

			foreach (Documento versione in versioni)
			{
				DataRow row=dt.NewRow();

				row["Versione"]=versione.version;
				row["Note"]=versione.descrizione;
				row["Data"]=versione.dataInserimento;
				row["VersionID"]=versione.versionId;

				row["Acquisito"]=versioniHandler.IsAcquired(versione);

				dt.Rows.Add(row);
			}
			
			ds.Tables.Add(dt);

			return ds;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gridItem"></param>
		private void AddLinkVisualizzaGridItem(DataGridItem gridItem)
		{
			bool acquisito=false;

			try
			{
				acquisito=Convert.ToBoolean(gridItem.Cells[COL_ACQUISITO].Text);
			}
			catch
			{
			}

			if (acquisito)
			{
				HyperLink documentLink=new HyperLink();
				documentLink.Text="Visualizza";
				documentLink.NavigateUrl="../VisualizzaDocumento.aspx?idProfile=" + this.idProfile + 
					"&docNumber=" + this.docNum + 
					"&versionId=" + gridItem.Cells[COL_VERSION_ID].Text + 
					"&isAllegato=false";

				gridItem.Cells[COL_VISUALIZZA].Controls.Add(documentLink);
			}
		}

		/// <summary>
		/// Impostazione visibilità campi UI
		/// </summary>
		private void SetFieldsVisibility()
		{
			this.grdVersioni.Visible=(this.grdVersioni.Items.Count>0);
			this.pnlVersioniMsg.Visible=!this.grdVersioni.Visible;
		}
		
		private void grdVersioni_PreRender(object sender, System.EventArgs e)
		{
			foreach (DataGridItem item in this.grdVersioni.Items)
				this.AddLinkVisualizzaGridItem(item);
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
	}
}
