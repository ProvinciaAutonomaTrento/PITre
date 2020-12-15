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

namespace DocsPAWA.ProspettiRiepilogativi.Frontend
{
	/// <summary>
	/// Summary description for gestioneProspetti.
	/// </summary>
	public class gestioneProspetti : System.Web.UI.Page
	{
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_elenco;
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_dettagli;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
            Session["Bookmark"] = "GestioneProspetti";
            
            if (!this.IsPostBack)
			{
				// Impostazione contesto corrente
				this.SetContext();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Impostazione contesto corrente
		/// </summary>
		private void SetContext()
		{
			string url=DocsPAWA.Utils.getHttpFullPath() + "/ProspettiRiepilogativi/Frontend/gestioneProspetti.aspx";

			SiteNavigation.CallContext newContext=new SiteNavigation.CallContext(SiteNavigation.NavigationKeys.PROSPETTI_RIEPILOGATIVI,url);
			newContext.ContextFrameName="top.principale";

			if (SiteNavigation.CallContextStack.SetCurrentContext(newContext))
				SiteNavigation.NavigationContext.RefreshNavigation();
		}
	}
}
