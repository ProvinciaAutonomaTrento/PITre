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
using System.Configuration;


namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for paroleChiave.
	/// </summary>
    public class paroleChiave : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.ListBox ListParoleChiave;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Button btn_ok;
		protected DocsPaWebCtrlLibrary.ImageButton btn_aggiungi;
		protected System.Web.UI.HtmlControls.HtmlInputHidden h_aggiorna;

		protected string wnd;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			wnd = Request.QueryString["wnd"];
			
			if (!Page.IsPostBack)
			{			
				setListaParoleChiave();
				//bottone per l'inserimento della parola chiave
				string page = "'insParolaChiave.aspx'";
				string titolo = "'Inserimento_ParolaChiave'";
				string param = "'width=420,height=150, scrollbars=no'";
				this.btn_aggiungi.Attributes.Add("onclick","ApriFinestraGen("+ page + ',' + titolo + ',' + param + ");");
			}
			if (this.h_aggiorna.Value.Equals("S"))
			{
				setListaParoleChiave();
				this.h_aggiorna.Value = "N";
			}

			disabilitaAddParoleChiave();
		
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
			this.btn_aggiungi.Click += new System.Web.UI.ImageClickEventHandler(this.btn_aggiungi_Click);
			this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.h_aggiorna.ServerChange += new System.EventHandler(this.h_aggiorna_ServerChange);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion


		private void setListaParoleChiave()
		{
			DocsPaWR.DocumentoParolaChiave[] listaParoleChiave = DocumentManager.getListaParoleChiave(this);
			this.ListParoleChiave.Items.Clear();
			if(listaParoleChiave.Length > 0)
			{
				for (int i=0; i<listaParoleChiave.Length; i++ )
				{
					this.ListParoleChiave.Items.Add(((DocsPAWA.DocsPaWR.DocumentoParolaChiave)listaParoleChiave[i]).descrizione);	
					this.ListParoleChiave.Items[i].Value=((DocsPAWA.DocsPaWR.DocumentoParolaChiave)listaParoleChiave[i]).systemId;
				}
			}
		}

		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			wnd = Request.QueryString["wnd"];

			DocsPaWR.DocumentoParolaChiave[] listaDocParoleChiave = new DocsPAWA.DocsPaWR.DocumentoParolaChiave[0];; 
			
			for (int i=0; i< this.ListParoleChiave.Items.Count; i++)
			{
				if (this.ListParoleChiave.Items[i].Selected)
				{
					DocsPaWR.DocumentoParolaChiave docParoleChiave = new DocsPAWA.DocsPaWR.DocumentoParolaChiave();
					docParoleChiave.systemId = this.ListParoleChiave.Items[i].Value;
					docParoleChiave.descrizione = this.ListParoleChiave.Items[i].Text;
					docParoleChiave.idAmministrazione =UserManager.getInfoUtente(this).idAmministrazione;// ConfigurationManager.AppSettings["ID_AMMINISTRAZIONE"];
					listaDocParoleChiave = Utils.addToArrayParoleChiave(listaDocParoleChiave, docParoleChiave);			
				}
			}

			if (wnd.Equals("docProf"))
			{
			
				DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoInLavorazione(this);
				if (schedaDocumento != null)
				{	
					//					schedaDocumento.paroleChiave = listaDocParoleChiave;
					schedaDocumento.paroleChiave = addParoleChiaveToDoc(schedaDocumento, listaDocParoleChiave);
					schedaDocumento.daAggiornareParoleChiave = true;
					DocumentManager.setDocumentoInLavorazione(this, schedaDocumento);			
				}

                DocumentManager.setListaParoleChiaveSel(this, listaDocParoleChiave);
                
                //	Response.Write("<script>var k=window.open('../documento/docProfilo.aspx','IframeTabs'); window.close();</script>");		
				Response.Write("<script>window.opener.document.forms[0].submit(); window.close();</script>");
				
			} 
			else 
			{
				DocumentManager.setListaParoleChiaveSel(this, listaDocParoleChiave);
				
				if (wnd.Equals("RicC"))
					Response.Write("<script>window.opener.f_Ricerca_C.submit(); window.close();</script>");
				else
					if (wnd.Equals("RicG"))
					Response.Write("<script>window.opener.ricDocGrigia.submit(); window.close();</script>");

			}

		}

		private DocsPAWA.DocsPaWR.DocumentoParolaChiave[] addParoleChiaveToDoc(DocsPAWA.DocsPaWR.SchedaDocumento schedaDocumento, DocsPAWA.DocsPaWR.DocumentoParolaChiave[] listaDocParoleChiave)
		{
			DocsPaWR.DocumentoParolaChiave[] listaPC;
			listaPC = schedaDocumento.paroleChiave;
			if (listaDocParoleChiave != null)
			{
				for (int i=0; i< listaDocParoleChiave.Length; i++)
				{
					if (!listaContains(schedaDocumento.paroleChiave,listaDocParoleChiave[i]))
						listaPC = Utils.addToArrayParoleChiave(listaPC, listaDocParoleChiave[i]);
				}
			}
			return listaPC;

		}

		private bool listaContains(DocsPAWA.DocsPaWR.DocumentoParolaChiave[] lista, DocsPAWA.DocsPaWR.DocumentoParolaChiave el)
		{
			bool trovato = false;
			if(lista != null)
			{
				for (int i=0; i< lista.Length; i++)
				{
					if (lista[i].systemId.Equals(el.systemId))
					{
						trovato = true;
						break;
					}
				}
			}
			return trovato;
		}

		private void btn_aggiungi_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
		
		}

		private void h_aggiorna_ServerChange(object sender, System.EventArgs e)
		{
		
		}

		private void disabilitaAddParoleChiave()
		{
			if (UserManager.ruoloIsAutorized(this,this.btn_aggiungi.Tipologia.ToString()))
				this.btn_aggiungi.Visible = true;
			else
				this.btn_aggiungi.Visible = false;
		}

	}
}
