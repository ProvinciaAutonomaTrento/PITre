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

namespace DocsPAWA.organigramma
{
	/// <summary>
	/// Summary description for VisualPdfToPrint.
	/// </summary>
	public class VisualPdfToPrint : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{			
			try
			{				
				DocsPaWR.FileDocumento filePdf = new DocsPAWA.DocsPaWR.FileDocumento();
				Amministrazione.Manager.OrganigrammaManager manager = new Amministrazione.Manager.OrganigrammaManager();
				filePdf = manager.getSessionFilePDF();
				manager.releaseSessionFilePDF();

				if(filePdf.content.Length>0)
				{
					Response.ContentType = filePdf.contentType;					
					Response.AddHeader("content-disposition", "inline;filename=" + filePdf.fullName);
					Response.AddHeader("content-lenght", filePdf.content.Length.ToString());				
					Response.BinaryWrite(filePdf.content);						
				}
				else
				{
					this.executeJS("<SCRIPT>alert('Si è verificato un errore nella generazione del file PDF da stampare');self.close();</SCRIPT>");
				}				
			}
			catch(Exception ex)
			{
				this.executeJS("<SCRIPT>alert('Errore di sistema: " + ex.Message.Replace("'","\\'") + "');self.close();</SCRIPT>");
			}
		}

		private void executeJS(string key)
		{
			if(!this.Page.IsStartupScriptRegistered("theJS"))			
				this.Page.RegisterStartupScript("theJS", key);
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
