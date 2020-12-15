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

namespace Amministrazione.Gestione_Homepage
{
	/// <summary>
	/// Summary description for Segn_Fasc.
	/// </summary>
	public class Segn_Fasc : System.Web.UI.Page
	{
		protected System.Web.UI.HtmlControls.HtmlTableRow COD_AMM;
		protected System.Web.UI.HtmlControls.HtmlTableRow COD_REG;
		protected System.Web.UI.HtmlControls.HtmlTableRow COD_TITOLO;
		protected System.Web.UI.HtmlControls.HtmlTableRow COD_UO;
		protected System.Web.UI.HtmlControls.HtmlTableRow DATA_ANNO;
		protected System.Web.UI.HtmlControls.HtmlTableRow DATA_COMP;
		protected System.Web.UI.HtmlControls.HtmlTableRow IN_OUT;
		protected System.Web.UI.HtmlControls.HtmlTableRow NUM_PROG;
		protected System.Web.UI.HtmlControls.HtmlTableRow NUM_PROTO;		
		protected System.Web.UI.WebControls.TextBox txt_tipo;
		protected System.Web.UI.WebControls.Label lbl_testa;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{	
				switch (Request.QueryString["tipo"])
				{
					case "segn":
						lbl_testa.Text = "Opzioni per la stringa di Segnatura";
						COD_TITOLO.Visible=false;
						NUM_PROG.Visible=false;
						break;

					case "fasc":
						lbl_testa.Text = "Opzioni per la stringa di Fascicolatura";
						NUM_PROTO.Visible=false;
						COD_REG.Visible=false;
						COD_AMM.Visible=false;
						IN_OUT.Visible=false;
						COD_UO.Visible=false;
						break;
				}		
				txt_tipo.Text = Request.QueryString["tipo"].ToString();				
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
	}
}
