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

namespace DocsPAWA.documento
{
	/// <summary>
	/// Summary description for statoDocAcquisito.
	/// </summary>
    public class statoDocAcquisitoCaching : System.Web.UI.Page
	{
        protected Label L_msg;
		private void Page_Load(object sender, System.EventArgs e)
		{
            if(Session["ErrorManager.error"]!= null 
                && !string.IsNullOrEmpty(Session["ErrorManager.error"].ToString()))
                L_msg.Text = Session["ErrorManager.error"].ToString();
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
