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

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for modificaFolder.
	/// </summary>
    public class modificaFolder : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btn_salva;
		protected System.Web.UI.WebControls.Label lbl_nome;
		protected System.Web.UI.WebControls.TextBox txt_nomeFolder;
		protected System.Web.UI.WebControls.Label Label1;

		protected DocsPAWA.DocsPaWR.Folder folder;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				// Put user code to initialize the page here
				
			
				folder = FascicoliManager.getFolderSelezionato(this);
				if(!IsPostBack)
				{
					
					this.txt_nomeFolder.Text=folder.descrizione;
				}
			}
			catch(System.Exception ex)
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
			this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btn_salva_Click(object sender, System.EventArgs e)
		{
			try
			{
				
				folder.descrizione=this.txt_nomeFolder.Text;
				FascicoliManager.updateFolder(this,folder);
				FascicoliManager.setFolderSelezionato(this,folder);
				//richiama la funzione javascript che aggiorna il form chiamante
				string  funct = " window.open('../fascicolo/fascDocumenti.aspx','IframeTabs'); ";
				funct = funct + " window.close(); ";
				Response.Write("<script> " + funct + "</script>");

			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}
	}
}
