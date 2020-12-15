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
	/// Summary description for docSpedisci.
	/// </summary>
    public class docSpedisci : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.RadioButtonList rb_modSped_E;
		protected System.Web.UI.WebControls.Button btn_annulla;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			
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
			this.rb_modSped_E.SelectedIndexChanged += new System.EventHandler(this.rb_modSped_E_SelectedIndexChanged);
			this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion



		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			if (this.rb_modSped_E.SelectedItem.Value=="I")
				this.SendDocumentMail();
			else
				this.SendDocumentFax();

			Response.Write("<script>window.close()</script>");
		}

		public DocsPAWA.DocsPaWR.SendDocumentResponse SendDocumentMail()
		{
			DocsPaWR.SchedaDocumento schedaDocumento=DocumentManager.getDocumentoSelezionato(this);
			return DocumentManager.spedisciDoc(this, schedaDocumento);
		}

		public void SendDocumentFax()
		{
			DocsPaWR.SchedaDocumento schedaDocumento=DocumentManager.getDocumentoSelezionato(this);
			DocumentManager.spedisciFax(this, schedaDocumento);
		}

		private void btn_annulla_Click(object sender, System.EventArgs e)
		{
		    Response.Write("<script>window.close()</script>");
		}

		private void rb_modSped_E_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

	}
}
