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
	/// Summary description for basePopup.
	/// </summary>
    public class basePopup : DocsPAWA.CssPage
	{
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

		protected void Close()
		{
			Response.Write("<script>window.self.close();</script>");
		}

		protected void Close(string message)
		{
			Response.Write("<script>alert(\""+message+"\");window.self.close();</script>");
		}

		protected void CloseAndPostback()
		{
			CloseAndPostback(null);
		}

		protected void CloseAndPostback(string message)
		{
			string alertScript = "";
			string refreshScript = "window.dialogArguments.location.href = window.dialogArguments.location.href;";

			if (message!=null)
				alertScript = "alert(\""+message+"\");";

			string script = "<script>"+alertScript+refreshScript+"window.self.close();</script>";
			Response.Write(script);
		}

	}
}
