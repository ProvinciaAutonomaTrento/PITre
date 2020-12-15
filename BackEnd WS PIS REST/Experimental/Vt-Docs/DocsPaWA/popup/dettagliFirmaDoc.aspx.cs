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

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for dettagliFirmaDoc.
	/// </summary>
    public class dettagliFirmaDoc : DocsPAWA.CssPage
	{	
		private void Page_Load(object sender, System.EventArgs e)
		{
            // Se il querystring contiene documentId, viene prima prelevato il dettaglio
            // sul documento richiesto
            if (!String.IsNullOrEmpty(Request["documentId"]))
            {
                // Recupero della scheda documento
                SchedaDocumento currentDocument = DocumentManager.getDettaglioDocumento(
                    this, 
                    Request["documentId"], 
                    String.Empty);
                DocumentManager.setDocumentoSelezionato(this.Page, currentDocument);
                Page.Session["FileManager.selectedFile"] = currentDocument.documenti[0];
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
