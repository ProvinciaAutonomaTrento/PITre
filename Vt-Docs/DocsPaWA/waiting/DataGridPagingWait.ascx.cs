namespace DocsPAWA.waiting
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for DataGridPagingWait.
	/// </summary>
	public class DataGridPagingWait : System.Web.UI.UserControl
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		public string DataGridID
		{
			get
			{
				if (this.ViewState["dataGridID"]!=null)
					return this.ViewState["dataGridID"].ToString();
				else
					return string.Empty;
			}
			set
			{
				this.ViewState["dataGridID"]=value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string WaitScriptCallback
		{
			get
			{
				if (this.ViewState["waitScriptCallback"]!=null)
					return this.ViewState["waitScriptCallback"].ToString();
				else
					return string.Empty;
			}
			set
			{
				this.ViewState["waitScriptCallback"]=value;
			}
		}
	}
}
