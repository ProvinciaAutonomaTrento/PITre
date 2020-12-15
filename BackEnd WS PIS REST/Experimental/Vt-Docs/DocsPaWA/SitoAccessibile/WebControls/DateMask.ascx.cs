namespace DocsPAWA.SitoAccessibile.WebControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Globalization;

	/// <summary>
	///		Summary description for DateMask.
	/// </summary>
	public class DateMask : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.TextBox txtMonth;
		protected System.Web.UI.WebControls.TextBox txtYear;
		protected System.Web.UI.WebControls.TextBox txtDay;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
			{
				this.txtDay.CssClass=this.CssClass;
				this.txtMonth.CssClass=this.CssClass;
				this.txtYear.CssClass=this.CssClass;
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

		/// <summary>
		/// 
		/// </summary>
		public string CssClass
		{
			get
			{
				if (this.ViewState["CssClass"]!=null)
					return this.ViewState["CssClass"] as string;
				else
					return string.Empty;
			}
			set
			{
				this.ViewState["CssClass"]=value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string Text
		{
			get
			{
				string dateValue=string.Empty;

				if (this.txtDay.Text.Length>0)
					dateValue=this.txtDay.Text;

				if (dateValue!=string.Empty)
					dateValue+="/";

				if (this.txtMonth.Text.Length>0)
					dateValue+=this.txtMonth.Text;

				if (dateValue!=string.Empty)
					dateValue+="/";

				if (this.txtYear.Text.Length>0)
					dateValue+=this.txtYear.Text;

				return dateValue;
			}
			set
			{
				if (value!=null && value!=string.Empty && this.IsValidDate(value))
				{
					string[] items=value.Split('/');

					if (items.Length==3)
					{
						this.txtDay.Text=items[0];
						this.txtMonth.Text=items[1];
						this.txtYear.Text=items[2];
					}
				}
				else
				{
					this.txtDay.Text=string.Empty;
					this.txtMonth.Text=string.Empty;
					this.txtYear.Text=string.Empty;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dateValue"></param>
		/// <returns></returns>
		private bool IsValidDate(string dateValue)
		{
			bool retValue=true;

			try
			{
				Convert.ToDateTime(dateValue);
			}
			catch
			{
				retValue=false;
			}

			return retValue;
		}
	}
}