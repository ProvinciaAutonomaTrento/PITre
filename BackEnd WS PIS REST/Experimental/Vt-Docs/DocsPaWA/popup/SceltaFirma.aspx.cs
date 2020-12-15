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
	/// Summary description for SceltaFirma.
	/// </summary>
    public class SceltaFirma : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.RadioButtonList RadioButtonList1;
		protected System.Web.UI.WebControls.Button Button1;
	
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Button1.Click += new System.EventHandler(this.Button1_Click);
			this.RadioButtonList1.SelectedIndexChanged += new System.EventHandler(this.RadioButtonList1_SelectedIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void RadioButtonList1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(RadioButtonList1.SelectedItem.Value.Equals("sign"))
				Response.Write("<script>return FirmaDoc(this.form,\"sign\");</script>");
			else
				Response.Write("<script>return FirmaDoc(this.form,\"cosign\");</script>");
			


		}

		private void Button1_Click(object sender, System.EventArgs e)
		{
			Response.Write("<script>window.close();</script>");
		}
	}
}
