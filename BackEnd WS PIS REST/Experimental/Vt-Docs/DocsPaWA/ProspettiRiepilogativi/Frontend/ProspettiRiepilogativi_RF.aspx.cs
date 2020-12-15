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

namespace ProspettiRiepilogativi.Frontend
{
	/// <summary>
	/// Summary description for ProspettiRiepilogativi_RF.
	/// </summary>
	public class ProspettiRiepilogativi_RF : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires = -1;
            DocsPAWA.DocsPaWR.FileDocumento theDoc = null;

			theDoc = (DocsPAWA.DocsPaWR.FileDocumento)utility.getSelectedFileReport(this);

			if (theDoc != null) 
			{
				Response.ContentType = theDoc.contentType;
                if (Session["percorso"] != null || !String.IsNullOrEmpty(Request["showAsAttachment"]))
                {
                    Response.AddHeader("content-disposition", "attachment; filename=" + theDoc.name);
                    Session.Remove("percorso");
                }
				else
				{
					Response.AddHeader("content-disposition", "inline; filename=" + theDoc.name);
					
				}
				Response.AddHeader("content-length", theDoc.content.Length.ToString());				
				Response.BinaryWrite(theDoc.content);
				Response.Flush();
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
