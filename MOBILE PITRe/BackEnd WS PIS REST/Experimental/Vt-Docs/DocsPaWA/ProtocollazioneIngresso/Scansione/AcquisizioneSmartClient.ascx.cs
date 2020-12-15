namespace ProtocollazioneIngresso.Scansione
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for AcquisizioneSmartClient.
	/// </summary>
	public class AcquisizioneSmartClient : System.Web.UI.UserControl
	{
     
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			//this.RegisterClientScript("ShowImageViewer","ShowImageViewer();");
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


        

        //afiordi
        protected DocsPAWA.DocsPaWR.SmartClientConfigurations SmartClientConfigurations
        {
            get
            {
                if (this.ViewState["SmartClientConfigurations"] == null)
                    this.ViewState["SmartClientConfigurations"] = DocsPAWA.SmartClient.Configurations.GetConfigurationsPerUser();
                return (DocsPAWA.DocsPaWR.SmartClientConfigurations)this.ViewState["SmartClientConfigurations"];
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
			this.PreRender += new EventHandler(this.Page_PreRender);
		}
		#endregion
	}
}
