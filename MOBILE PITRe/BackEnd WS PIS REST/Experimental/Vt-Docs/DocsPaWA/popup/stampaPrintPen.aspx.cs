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

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for stampaPrintPen.
	/// </summary>
    public class stampaPrintPen : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Button btn_chiudi;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.Label lbl_segnatura;
	
		private void Page_Load(object sender, System.EventArgs e)
		{			
			if (!this.IsPostBack)
			{
				this.btn_ok.Attributes.Add("onclick","stampaSegnatura();");
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
			this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
			this.btn_chiudi.Click += new System.EventHandler(this.btn_chiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.stampaPrintPen_PreRender);

		}
		#endregion

		private void btn_chiudi_Click(object sender, System.EventArgs e)
		{
			Response.Write("<script> window.close();</script>");
		}

		private void stampaPrintPen_PreRender(object sender, System.EventArgs e)
		{
			string segnatura = Request.QueryString["segn"];
			this.lbl_segnatura.Text = segnatura;
		}

		private void btn_ok_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}
