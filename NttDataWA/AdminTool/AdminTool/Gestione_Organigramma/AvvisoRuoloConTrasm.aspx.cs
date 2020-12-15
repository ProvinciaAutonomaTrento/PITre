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

namespace Amministrazione.Gestione_Organigramma
{
	/// <summary>
	/// Summary description for AvvisoRuoloConTrasm.
	/// </summary>
	public class AvvisoRuoloConTrasm : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button btn_si;
		protected System.Web.UI.WebControls.Image img_alert;
		protected System.Web.UI.WebControls.Label lbl_utente;
		protected System.Web.UI.WebControls.Button btn_no;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			this.lbl_utente.Text = Request.QueryString["utente"];

			this.btn_si.Attributes.Add("onclick", "window.returnValue = 'Y'; window.close();");
			this.btn_no.Attributes.Add("onclick", "window.returnValue = 'N'; window.close();");
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
