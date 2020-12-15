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

namespace DocsPAWA.SitoAccessibile
{
	/// <summary>
	/// Summary description for SessionAborted.
	/// </summary>
	public class SessionAborted : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button btnLogin;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlMessage;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			string result = Request.QueryString["result"];

			if(result == DocsPAWA.DocsPaWR.ValidationResult.SESSION_EXPIRED.ToString())
				this.pnlMessage.InnerText="La sessione e' scaduta.";

			else if(result == DocsPAWA.DocsPaWR.ValidationResult.SESSION_DROPPED.ToString())
				this.pnlMessage.InnerText="L'utente si e' connesso da un'altra postazione.";

			else if(result == DocsPAWA.DocsPaWR.ValidationResult.APPLICATION_ERROR.ToString())
				this.pnlMessage.InnerText="Errore imprevisto dell'applicazione.";
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
			this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// Reindirizzamento alla pagina di login
		/// </summary>
		private void RedirectToLoginPage()
		{
			Response.Redirect(EnvironmentContext.RootPath + "Login.aspx");
		}

		private void btnLogin_Click(object sender, System.EventArgs e)
		{
			this.RedirectToLoginPage();
		}
	}
}
