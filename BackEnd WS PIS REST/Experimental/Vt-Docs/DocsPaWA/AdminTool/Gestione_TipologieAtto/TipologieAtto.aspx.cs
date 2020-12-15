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

namespace DocsPAWA.AdminTool.Gestione_TipologieAtto
{
	/// <summary>
	/// Summary description for TipologieAtto.
	/// </summary>
	public class TipologieAtto : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lbl_tit;
		protected System.Web.UI.WebControls.Button btn_nuova;
		protected System.Web.UI.WebControls.DataGrid dg_Tipologie;
		protected System.Web.UI.WebControls.ImageButton btn_chiudiPnlInfo;
		protected System.Web.UI.WebControls.Panel pnl_info;
		protected System.Web.UI.WebControls.TextBox txt_descrizione;
		protected System.Web.UI.WebControls.Label lbl_msg;
		protected System.Web.UI.WebControls.Button btn_save;
		protected System.Web.UI.WebControls.Label lbl_position;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			
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
