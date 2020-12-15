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

namespace DocsPAWA.AdminTool.Gestione_Logs
{
	/// <summary>
	/// Summary description for exportLogPage.
	/// </summary>
	public class exportLogPage : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{			
			try
			{				
				DocsPaWR.FileDocumento file = new DocsPAWA.DocsPaWR.FileDocumento();

				exportLogSessionManager sessioneManager = new exportLogSessionManager();
				file = sessioneManager.GetSessionExportFile();
				sessioneManager.ReleaseSessionExportFile();
				
				if(file!=null && file.content.Length>0)
				{
					Response.ContentType = file.contentType;					
					Response.AddHeader("content-disposition", "inline;filename=" + file.fullName);
					Response.AddHeader("content-lenght", file.content.Length.ToString());				
					Response.BinaryWrite(file.content);						
				}
				else
				{
					this.executeJS("<SCRIPT>alert('Impossibile generare il documento di esportazione dei dati.\nContattare l'amministratore di sistema.');self.close();</SCRIPT>");
				}				
			}
			catch
			{
				this.executeJS("<SCRIPT>alert('Impossibile generare il documento di esportazione dei dati.\nContattare l'amministratore di sistema.');self.close();</SCRIPT>");
			}

		}

		private void executeJS(string key)
		{
			if(!this.Page.IsStartupScriptRegistered("theJS"))			
				this.Page.RegisterStartupScript("theJS", key);
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
