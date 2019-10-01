namespace Amministrazione.UserControl
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for MenuLog.
	/// </summary>
	public class MenuLog : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.HyperLink Hyperlink1;
		protected System.Web.UI.WebControls.HyperLink Hyperlink3;
		protected System.Web.UI.WebControls.HyperLink Hyperlink2;
        protected System.Web.UI.WebControls.HyperLink Hyperlink4;

        protected System.Web.UI.WebControls.Panel pnl_estrazione_log;

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
            if (!IsPostBack)
            {
                string valoreChiave = SAAdminTool.utils.InitConfigurationKeys.GetValue("0", "FE_ESTRAZIONE_LOG");
                if (!string.IsNullOrEmpty(valoreChiave))
                {
                    this.pnl_estrazione_log.Visible = true;
                }
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
