using System;
using System.Net;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for bottom.
	/// </summary>
	public class bottom : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_ver;
        protected System.Web.UI.WebControls.Label lbl_ip;
		protected System.Web.UI.WebControls.Label lbl_copyright;
        protected System.Web.UI.WebControls.Xml Xml1;
        protected System.Web.UI.HtmlControls.HtmlImage bandiere;
        protected DocsPAWA.UserControls.AppTitleProvider appTitleProvider;

    
		private void Page_Load(object sender, System.EventArgs e)
		{
           
            this.lbl_ver.Text = this.appTitleProvider.ApplicationNameVersion;
            this.lbl_copyright.Text = this.appTitleProvider.CopyrightInformation;
            
            bandiere.Alt = this.Server.MachineName + this.getIPAddress();
            this.lbl_ip.Font.Bold = true;
            this.lbl_ip.Text = "IP "+ this.getIPAddress();
        }

        private string getIPAddress()
        {
            string retValue = string.Empty;
            try
            {
                retValue = " / " + Request.ServerVariables["LOCAL_ADDR"];
            }
            catch
            {
                retValue = "";
            }
            return retValue;
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
