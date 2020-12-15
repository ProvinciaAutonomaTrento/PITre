using System;
using System.Configuration;
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
	/// Summary description for SessionAborted.
	/// </summary>
	public class SessionAborted : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label errorLabel;
        protected System.Web.UI.WebControls.Image img_logologin;

		private void Page_Load(object sender, System.EventArgs e)
		{
            if (fileExist("logo.gif", "LoginFE"))
                this.img_logologin.ImageUrl = "images/loghiAmministrazioni/logo.gif";
            else
                this.img_logologin.ImageUrl = "images/logo.gif";
            
            if (Request.QueryString["showDetails"] == null)
			{
				string script = "<script>";
				script += "window.open('" + ConfigurationManager.AppSettings["invalidSessionUrl"] + "?result=" + Request.QueryString["result"] + "&showDetails=true', '_blank', 'fullscreen=no, toolbar=no, directories=no, statusbar=no, menubar=no, resizable=no, scrollbars=mo');";
				script += "window.top.close();";
				script += "</script>";
				Response.Write(script);
			}
			else
			{
				string result = Request.QueryString["result"];

				if(result == DocsPAWA.DocsPaWR.ValidationResult.SESSION_EXPIRED.ToString())
				{				
					this.errorLabel.Text = "La sessione e' scaduta.";
				}
				else if(result == DocsPAWA.DocsPaWR.ValidationResult.SESSION_DROPPED.ToString())
				{				
					this.errorLabel.Text = "L'utente si e' connesso da un'altra postazione.";
				}
				else if(result == DocsPAWA.DocsPaWR.ValidationResult.APPLICATION_ERROR.ToString())
				{				
					this.errorLabel.Text = "Errore imprevisto dell'applicazione.";
				}				

				this.errorLabel.Visible = true;									
			}
		}
       
        private bool fileExist(string fileName, string type)
        {
            return FileManager.fileExist(fileName, type);
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
