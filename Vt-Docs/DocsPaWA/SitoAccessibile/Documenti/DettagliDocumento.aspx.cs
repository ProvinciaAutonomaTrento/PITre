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

namespace DocsPAWA.SitoAccessibile.Documenti
{
	public class DettagliDocumento : SessionWebPage
	{
		//Contolli condivisi - inizio
		protected System.Web.UI.HtmlControls.HtmlForm Form1;

		protected System.Web.UI.HtmlControls.HtmlInputText txt_cod_mitt;
		protected System.Web.UI.HtmlControls.HtmlInputText txt_desc_mitt;
		protected System.Web.UI.HtmlControls.HtmlInputText txt_prot_mitt;
		protected System.Web.UI.HtmlControls.HtmlInputText dta_prot_mitt;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divMittente;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divDestinatario;
		protected System.Web.UI.HtmlControls.HtmlInputText txt_desc_dest;
		protected System.Web.UI.HtmlControls.HtmlInputText txt_desc_destCC;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divMittenteInterm;
		protected System.Web.UI.HtmlControls.HtmlInputText txt_cod_mittInterm;
		protected System.Web.UI.HtmlControls.HtmlInputText txt_desc_mittInterm;
		protected System.Web.UI.HtmlControls.HtmlGenericControl divProtoMittente;
		protected System.Web.UI.HtmlControls.HtmlTextArea dr_obj;
		protected System.Web.UI.HtmlControls.HtmlInputText txt_dta_arrivo;
		protected System.Web.UI.HtmlControls.HtmlInputCheckBox Checkbox1;
		protected System.Web.UI.WebControls.Button btnBack;
		
		protected System.Web.UI.WebControls.ListBox Listbox1;
		protected System.Web.UI.WebControls.ListBox lstDestinatari;
		protected System.Web.UI.WebControls.ListBox lstDestinatariCC;

		protected string idProfile;
		protected string docNum;

		protected DocsPAWA.DocsPaWR.SchedaDocumento _schedaDocumento=null;
		protected System.Web.UI.WebControls.Button btnShowDocumento;
		protected System.Web.UI.HtmlControls.HtmlGenericControl containerDettagliDocumento;

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

					this.InitializeControlsDettagliDocumento();
				}
				else
				{
					throw new ApplicationException("Parametri mancanti");
				}

				this.ShowButtonBackToSearchResult();

				this.ShowButtonVisualizzaDocumento();
			}
			catch(Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}

		private DettaglioDocumentoUserControl GetControlDettaglioDocumento(string id)
		{
			return this.FindControl(id) as DettaglioDocumentoUserControl;
		}

		/// <summary>
		/// Inizializzazione controllo menu documento
		/// </summary>
		private void InitializeControlMenuDocumento()
		{
			MenuDocumento menuDocumento=this.FindControl("MenuDocumento") as MenuDocumento;

			if (menuDocumento!=null)
				menuDocumento.Initialize(MenuDocumento.ItemsMenuDocumentoEnum.Documento,this._schedaDocumento);
		}

		private void InitializeControlsDettagliDocumento()
		{
			DettaglioDocumentoUserControl ctl=GetControlDettaglioDocumento("DettagliGenerali");
			ctl.Initialize(this._schedaDocumento,true);

			ctl=GetControlDettaglioDocumento("DettagliProtocollo");

			if (!this.IsDocumentoGrigio())
				ctl.Initialize(this._schedaDocumento,true);
			else
				ctl.Visible=false;

			ctl=GetControlDettaglioDocumento("DettagliProfilo");
			ctl.Initialize(this._schedaDocumento,true);
		}

		/// <summary>
		/// Gestione visualizzazione pulsante di visualizzazione del documento principale
		/// </summary>
		private void ShowButtonVisualizzaDocumento()
		{
			bool showButton=(this._schedaDocumento.documenti.Length>0);

			if (showButton)
			{
				FileRequest fileRequest=this._schedaDocumento.documenti[0];
								
				showButton=(fileRequest.fileName!=null && 
							fileRequest.fileName!=string.Empty &&
							fileRequest.fileSize!=null &&
							fileRequest.fileSize!="0");

			}

			this.btnShowDocumento.Visible=showButton;
		}

		/// <summary>
		/// Visualizzazione del documento principale
		/// </summary>
		private void ShowDocumentoPrincipale()
		{
			Versioni.VersioniHandler versioniHandler=new DocsPAWA.SitoAccessibile.Documenti.Versioni.VersioniHandler();
			
			if (this._schedaDocumento.documenti.Length>0 && versioniHandler.IsAcquired(this._schedaDocumento.documenti[0]))
			{
				string versionID= this._schedaDocumento.documenti[0].versionId;

				Response.Redirect("VisualizzaDocumento.aspx?idProfile=" + this._schedaDocumento.systemId + 
									"&docNumber=" + this._schedaDocumento.docNumber + 
									"&versionId=" + versionID + 
									"&isAllegato=false");
			}
		}

		/// <summary>
		/// Gestione visualizzazione pulsante di ritorno ai risultati di ricerca
		/// </summary>
		private void ShowButtonBackToSearchResult()
		{
			this.btnBack.Visible=this.OnSearchMode();
		}

		/// <summary>
		/// Verifica se si è in modalità di ricerca
		/// </summary>
		/// <returns></returns>
		private bool OnSearchMode()
		{
			bool searchMode=true;

//			if (Request.QueryString["searchMode"]!=null)
//			{
//				try
//				{
//					searchMode=Convert.ToBoolean(Request.QueryString["searchMode"]);
//				}
//				catch
//				{
//				}
//			}

			return searchMode;
		}

		/// <summary>
		/// Verifica se la scheda documento corrente 
		/// si riferisce ad un documento grigio
		/// </summary>
		/// <returns></returns>
		private bool IsDocumentoGrigio()
		{
			return (this._schedaDocumento.tipoProto=="G" && this._schedaDocumento.protocollo==null);
		}

		private void btnBack_Click(object sender, System.EventArgs e)
		{
			Response.Redirect(this.GetCallerUrl());
		}

		private void btnShowDocumento_Click(object sender, System.EventArgs e)
		{
			// Visualizzazione del documento principale
			this.ShowDocumentoPrincipale();
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

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			this.btnShowDocumento.Click += new System.EventHandler(this.btnShowDocumento_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

    }
}