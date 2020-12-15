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
using DocsPAWA.SitoAccessibile.Documenti.Allegati;

namespace DocsPAWA.SitoAccessibile.Documenti.Allegati
{
	public class DettaglioAllegati : SessionWebPage
	{
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		
		//Importati da docspa originale - inizio
		protected ArrayList Dt_elem;
		protected DocsPAWA.DocsPaWR.Allegato[] ListaDocAllegati;
		protected DocsPaWebCtrlLibrary.ImageButton btn_rimuoviAlleg;
		protected DocsPaWebCtrlLibrary.ImageButton btn_sostituisciDocPrinc;
		protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungiAreaLav;
		protected DocsPaWebCtrlLibrary.ImageButton btn_modifAlleg;
		protected DocsPaWebCtrlLibrary.ImageButton btn_aggAlleg;
		//protected System.Web.UI.WebControls.Label lbl_message;
		//protected System.Web.UI.WebControls.DataGrid grdAllegati;
		protected AccessibleDataGrid grdAllegati;

		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlAllegatiMsg;
		protected System.Web.UI.WebControls.Button btnBack;
		protected System.Web.UI.WebControls.Panel azioniAllegati;

		protected string idProfile;
		protected string docNum;
		protected DocsPAWA.DocsPaWR.SchedaDocumento _schedaDocumento=null;

		/// <summary>
		/// Costanti che identificano gli indici delle colonne del datagrid
		/// </summary>
		private const int COL_CODICE=0;
		private const int COL_DESCRIZIONE=1;
		private const int COL_NUMERO_PAGINE=2;
		private const int COL_VERSION_ID=3;
		private const int COL_ACQUISITO=4;
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
					DocumentoHandler handler=new DocumentoHandler();
					this._schedaDocumento=handler.GetDocumento(this.idProfile,this.docNum);

					this.InitializeControlMenuDocumento();

					this.Fetch();

					// Impostazione visibilità campi UI
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
			this.grdAllegati.PreRender += new System.EventHandler(this.grdAllegati_PreRender);
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion


		/// <summary>
		/// Reperimento usercontrol dei dettagli generali del documento
		/// </summary>
		/// <returns></returns>
		private DettagliGenerali GetDettagliGenerali()
		{
			return this.FindControl("DatiGeneraliDocumento") as DettagliGenerali;
		}

		/// <summary>
		/// Inizializzazione controllo menu documento
		/// </summary>
		private void InitializeControlMenuDocumento()
		{
			MenuDocumento menuDocumento=this.FindControl("MenuDocumento") as MenuDocumento;

			if (menuDocumento!=null)
				menuDocumento.Initialize(MenuDocumento.ItemsMenuDocumentoEnum.Allegati,this._schedaDocumento);
		}

		/// <summary>
		/// Caricamento dati
		/// </summary>
		private void Fetch()
		{
			// Caricamento dettagli generali del documento
			this.GetDettagliGenerali().Initialize(this._schedaDocumento,true);

			// Caricamento dati allegati
			this.FetchAllegati();
		}

		/// <summary>
		/// Caricamento degli allegati del documento
		/// </summary>
		private void FetchAllegati()
		{
			AllegatiHandler allegatiHandler=new AllegatiHandler();
			
			// Reperimento degli allegati del documento
			Allegato[] allegati=allegatiHandler.GetAllegatiDocumento(this._schedaDocumento);

			this.grdAllegati.DataSource=this.AllegatiToDataset(allegati);
			this.grdAllegati.DataBind();	
		}

		private void grdAllegati_PreRender(object sender, System.EventArgs e)
		{
			foreach (DataGridItem item in this.grdAllegati.Items)
				this.AddLinkVisualizzaGridItem(item);
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
					"&isAllegato=true";

				gridItem.Cells[COL_VISUALIZZA].Controls.Add(documentLink);
			}
		}

		private DataSet AllegatiToDataset(Allegato[] allegati)
		{
			DataSet ds=new DataSet();
			DataTable dt=new DataTable();

			dt.Columns.Add("Codice",typeof(string));
			dt.Columns.Add("Descrizione",typeof(string));
			dt.Columns.Add("NumeroPagine",typeof(string));
			dt.Columns.Add("VersionID",typeof(string));
			dt.Columns.Add("Acquisito",typeof(bool));

			AllegatiHandler allegatiHandler=new AllegatiHandler();

			foreach (Allegato allegato in allegati)
			{
				DataRow row=dt.NewRow();

				row["Codice"]=allegato.versionLabel;
				row["Descrizione"]=allegato.descrizione;
				row["NumeroPagine"]=allegato.numeroPagine;
				row["VersionID"]=allegato.versionId;

				row["Acquisito"]=allegatiHandler.IsAcquired(allegato);

				dt.Rows.Add(row);
			}
			
			ds.Tables.Add(dt);

			return ds;
		}

		/// <summary>
		/// Impostazione visibilità campi UI
		/// </summary>
		private void SetFieldsVisibility()
		{
			this.grdAllegati.Visible=(this.grdAllegati.Items.Count>0);
			this.pnlAllegatiMsg.Visible=!this.grdAllegati.Visible;
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
