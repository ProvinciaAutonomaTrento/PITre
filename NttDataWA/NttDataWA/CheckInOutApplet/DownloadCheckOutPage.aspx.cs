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
using NttDataWA.DocsPaWR;

namespace NttDataWA.CheckInOutApplet
{
	/// <summary>
	/// Summary description for DownloadCheckOutPage.
	/// </summary>
	public class DownloadCheckOutPage : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			this.RenderCheckedOutDocument();
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

		/// <summary>
		/// Rendering del documento checkedout
		/// </summary>
		private void RenderCheckedOutDocument()
		{
			byte[] content=null;

			if (CheckOutPage.SessionContent==null)
			{
				// Se il download del file non è stato fatto
				// contestualmente al CheckOut, viene reperito
				// il content del file dal server
				content=CheckInOutServices.GetCheckedOutFileDocument();
			}
			else
			{
				// Se il file è già stato scaricato contestualmente al checkout, 
				// viene reperito il content del file dalla sessione
				content=CheckOutPage.SessionContent;

				CheckOutPage.SessionContent=null;
			}

			if (content!=null)
			{
				Response.BinaryWrite(content);
               // Response.Flush();
			}
		}
	}
}
