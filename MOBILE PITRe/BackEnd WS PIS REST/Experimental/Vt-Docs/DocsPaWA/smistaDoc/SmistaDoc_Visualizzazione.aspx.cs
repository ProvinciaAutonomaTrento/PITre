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

namespace DocsPAWA.smistaDoc
{
	/// <summary>
	/// Summary description for SmistaDoc_Visualizzazione.
	/// </summary>
	public class SmistaDoc_Visualizzazione : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				this.ShowDocument();
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

		private DocsPAWA.DocsPaWR.DocumentoSmistamento GetDocumentoTrasmesso(bool content)
		{
			// Reperimento oggetto "DocumentoTrasmesso" corrente
			return SmistaDocSessionManager.GetSmistaDocManager().GetCurrentDocument(content);
		}

		public void ShowDocument()
		{
			Response.Expires = -1;
			DocsPaWR.FileDocumento immagineDocumento=this.GetDocumentoTrasmesso(true).ImmagineDocumento;

			if (immagineDocumento!=null) 
			{
				Response.ContentType = immagineDocumento.contentType;
				Response.AddHeader("content-disposition", "inline;filename=" + immagineDocumento.name);
				Response.AddHeader("content-lenght", immagineDocumento.content.Length.ToString());				
				Response.BinaryWrite(immagineDocumento.content);
			}
		}
	}
}
