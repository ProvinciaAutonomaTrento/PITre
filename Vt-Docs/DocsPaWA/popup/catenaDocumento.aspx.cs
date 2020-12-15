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
using Microsoft.Web.UI.WebControls;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for catenaDocumento.
	/// </summary>
	public class catenaDocumento : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.Label Label1;
		protected Microsoft.Web.UI.WebControls.TreeView catenaDoc;
		protected DocsPAWA.DocsPaWR.AnelloDocumentale albero;
	
		#region template pagina
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				//recupera i parametri dalla session o vengono
				//costruiti appositamente in base ai parametri
				//ricevuti.
				getParametri();
				if(!IsPostBack)
					caricaCatenaDoc();
			
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private void getParametri()
		{
			//recupera i parametri dalla session o vengono
			//costruiti appositamente in base ai parametri
			//ricevuti.
			try
			{

				if (!IsPostBack)
				{
					//primo caricamento pagina:
					//creo i dati che dovrà gestire la pagina
					buildParametriPagina();
				}
				else
				{
					//post back:
					//recupero eventuali dati già creati per la 
					//pagina e memorizzati in session			
				}
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}



		private void buildParametriPagina()
		{
			//creo i dati che dovrà gestire la pagina
			try
			{
		        //caricamento dei dati di sessione
				DocsPaWR.InfoUtente infoUser=UserManager.getInfoUtente(this);
				DocsPaWR.InfoDocumento infoDoc=DocumentManager.getInfoDocumento(DocumentManager.getDocumentoSelezionato(this));
				
				//caricamento dei dati dell'albero
				albero=DocumentManager.getCatenaDoc(infoUser.idGruppo, infoUser.idPeople, infoDoc.idProfile, this);


			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}

		private void setParametriPaginaInSession()
		{
			//memorizzo i parametri che dovranno essere
			//disponibili alla pagina tra le varie 
			//sequenze di postback
			try
			{

			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			} 
		}



		

#endregion

		#region gestione caricamento TgestioneDoc
		private Microsoft.Web.UI.WebControls.TreeNode addDocumentoNode(Microsoft.Web.UI.WebControls.TreeNode parentNode,DocsPaWR.AnelloDocumentale anelloDoc)
		{
			try
			{
				Microsoft.Web.UI.WebControls.TreeNode node=new Microsoft.Web.UI.WebControls.TreeNode();

				node.Text=anelloDoc.infoDoc.segnatura;
				//node.ID=m_nodeIndex.ToString();
				

				if (parentNode!=null)
				{
					//aggiungo il nuovo nodo al nodo padre
					parentNode.Nodes.Add(node);
				}
				else
				{
					catenaDoc.Nodes.Add(node);
				}

				return node;
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			}
			return null;
		}

		public void createTree(Microsoft.Web.UI.WebControls.TreeNode parentNode,DocsPaWR.AnelloDocumentale anello)
		{	
			try
			{
				Microsoft.Web.UI.WebControls.TreeNode newAddedNode=addDocumentoNode(parentNode,anello);
				//m_nodeIndex++;

				int g=anello.children.Length;
				for(int j=0;j<g;j++)
				{
					
					//richiama la funzione ricorsivamente
					createTree(newAddedNode,anello.children[j]);
				}
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			}
		}

		private void caricaCatenaDoc()
		{
			//esegue il caricamento della catena
			try
			{
                catenaDoc.Nodes.Clear();
			    this.createTree(null,albero);
			}
			catch(System.Exception es) 
			{
				ErrorManager.redirect(this, es);
			}
		}

		#endregion

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
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			Response.Write("<script>window.close();</script>");
		}

	}
}
