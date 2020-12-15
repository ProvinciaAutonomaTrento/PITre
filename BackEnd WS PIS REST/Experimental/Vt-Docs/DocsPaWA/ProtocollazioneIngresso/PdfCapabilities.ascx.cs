namespace ProtocollazioneIngresso
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for PdfCapabilities.
	/// </summary>
	public class PdfCapabilities : System.Web.UI.UserControl
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			this.RegisterClientScript("EnableCheckRecognizeTextOcr","EnableCheckRecognizeTextOcr('" + this.IsPostBack.ToString().ToLower() + "');");
		}

		#region Gestione javascript

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}

		#endregion

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
			this.PreRender +=new EventHandler(Page_PreRender);
		}
		#endregion
	}
}
