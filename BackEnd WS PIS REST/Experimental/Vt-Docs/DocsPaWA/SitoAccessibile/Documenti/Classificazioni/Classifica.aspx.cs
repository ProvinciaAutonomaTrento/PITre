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
using DocsPAWA.SitoAccessibile.Documenti;

namespace DocsPAWA.SitoAccessibile.Documenti.Classificazioni
{
	public class Classifica : SessionWebPage
	{
		//Contolli condivisi - inizio
		protected System.Web.UI.HtmlControls.HtmlForm Form1;

		protected System.Web.UI.WebControls.Button btnBack;
		protected UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid grdFascicoli;

		protected string idProfile;
		protected string docNum;

		//protected System.Web.UI.WebControls.Label lblClassificazioniNotFound;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlClassMsg;
		protected System.Web.UI.WebControls.LinkButton LinkButton1;
		protected DocsPAWA.DocsPaWR.SchedaDocumento _schedaDocumento=null;

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
				
					// Caricamento dettagli generali del documento
					this.GetDettagliGenerali().Initialize(this._schedaDocumento,true);

					this.InitializeControlMenuDocumento();

					if (!this.IsPostBack)
						this.Fetch();
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
			this.grdFascicoli.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.grdFascicoli_ItemCommand);
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
		/// Caricamento dati
		/// </summary>
		private void Fetch()
		{
			idProfile=this._schedaDocumento.systemId;

			ClassificaHandler classificaHandler=new ClassificaHandler();
			DocsPaWR.Fascicolo[] fascicoli=classificaHandler.GetFascicoliDocumento(idProfile);

			this.grdFascicoli.DataSource=FascicoloGridBindableObject.GetListFascicoli(fascicoli);
			this.grdFascicoli.DataBind();

			this.SetVisibilityDataGrid(fascicoli.Length>0);
		}

		/// <summary>
		/// Inizializzazione controllo menu documento
		/// </summary>
		private void InitializeControlMenuDocumento()
		{
			MenuDocumento menuDocumento=this.FindControl("MenuDocumento") as MenuDocumento;

			if (menuDocumento!=null)
				menuDocumento.Initialize(MenuDocumento.ItemsMenuDocumentoEnum.Classifica,this._schedaDocumento);
		}

		/// <summary>
		/// Impostazione visibilità datagrid
		/// </summary>
		/// <param name="isVisible"></param>
		private void SetVisibilityDataGrid(bool isVisible)
		{
			this.grdFascicoli.Visible=isVisible;
			//this.lblClassificazioniNotFound.Visible=!this.grdFascicoli.Visible;
			this.pnlClassMsg.Visible=!this.grdFascicoli.Visible;

		}

		/// <summary>
		/// Visualizzazione del fascicolo in cui è classificato il documento
		/// </summary>
		/// <param name="idFascicolo"></param>
		/// <param name="idRegistro"></param>
		private void ShowFascicolo(string idFascicolo,string idRegistro)
		{
			this.SetCallerContext();

			string navigateUrl=EnvironmentContext.RootPath + "Fascicoli/Fascicolo.aspx?idFascicolo=" + idFascicolo + "&idRegistro=" + idRegistro;

			Response.Redirect(navigateUrl);
		}

		/// <summary>
		/// Impostazione contesto chiamante
		/// </summary>
		private void SetCallerContext()
		{
			// Impostazione del contesto del chiamante in sessione
			CallerContext.NewContext(this.Page.Request.Url.AbsoluteUri);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void grdFascicoli_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName=="SHOW_FASCICOLO")
			{
				string idFascicolo=e.Item.Cells[0].Text;
				string idRegistro=e.Item.Cells[1].Text;

				this.ShowFascicolo(idFascicolo,idRegistro);
			}
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

		/// <summary>
		/// Classe necessaria per la visualizzazione del fascicolo in griglia
		/// </summary>
		private class FascicoloGridBindableObject
		{
			private string _id=string.Empty;
			private string _codice=string.Empty;
			private string _descrizione=string.Empty;
			private string _idRegistro=string.Empty;
			private string _registro=string.Empty;

			public static ArrayList GetListFascicoli(DocsPAWA.DocsPaWR.Fascicolo[] listFascicoli)
			{
				ArrayList retValue=new ArrayList();
				foreach (DocsPAWA.DocsPaWR.Fascicolo fascicolo in listFascicoli)
					retValue.Add(new FascicoloGridBindableObject(fascicolo));
				return retValue;
			}

			public FascicoloGridBindableObject(DocsPAWA.DocsPaWR.Fascicolo fascicolo)
			{
				this._id=fascicolo.systemID;
				this._codice=fascicolo.codice;
				this._descrizione=fascicolo.descrizione;
				this._idRegistro=fascicolo.idRegistroNodoTit;
				this._registro=fascicolo.codiceRegistroNodoTit;
			}

			public string ID
			{
				get
				{
					return this._id;
				}
			}

			public string Codice
			{
				get
				{
					return this._codice;
				}
			}

			public string Descrizione
			{
				get
				{
					return this._descrizione;
				}
			}

			public string IDRegistro
			{
				get
				{
					return this._idRegistro;
				}
			}

			public string Registro
			{
				get
				{
					return this._registro;
				}
			}
		}
	}
}