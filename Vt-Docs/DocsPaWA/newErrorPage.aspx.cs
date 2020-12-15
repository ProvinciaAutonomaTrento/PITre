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

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for ErrorPage.
	/// </summary>
	public class newErrorePage : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button btn_rtnLogin;
		protected System.Web.UI.HtmlControls.HtmlForm form;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label proc;
		protected System.Web.UI.HtmlControls.HtmlInputButton btn_chiudi;
		protected System.Web.UI.WebControls.Label lbl_mgserrore;
	
		private void Page_Load(object sender, System.EventArgs e)

		{
			this.proc.Text="&nbsp;Attenzione: <br><br> procedura di&nbsp;";
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1; 
			this.btn_chiudi.Attributes.Add("onClick","window.close();");
			
					
				this.lbl_mgserrore.Text = "";
				string proc = Request.QueryString["nome_proc"];
				if(proc!=null && !proc.Equals(""))
				{
					this.proc.Visible=false;
					this.proc.Text= this.proc.Text + " " + Server.UrlDecode(proc.ToUpper()) +" non effettuata.";
				}
				else
				{
					this.proc.Visible=false;
				}
				if(Session["ErrorManager.error"]==null) //altrimenti se errore prima del login c'è il loop eterno!
				{
					if (!(Session != null && Session["userData"] != null))
						ErrorManager.redirectToLoginPage(this);	
					else 
					{
						string msg = (string) Session["ErrorManager.error"];
						this.lbl_mgserrore.Text = msg;
						Session.Remove("ErrorManager.error");
					}
				}
				else 
				{
					string msg = (string) Session["ErrorManager.error"];
                    this.lbl_mgserrore.Text = this.lbl_mgserrore.Text + "<BR>" +msg;
					Session.Remove("ErrorManager.error");
				}
			if ((string)Session["noruolo"] == "" || Session["noruolo"] == null)
			{}
			else
			{
				this.lbl_mgserrore.Text = (string)Session["noruolo"]; 
				Session["noruolo"] = "" ;
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
			this.btn_chiudi.ServerClick += new System.EventHandler(this.btn_chiudi_ServerClick);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.form_PreRender);

		}
		#endregion

		private void btn_rtnLogin_Click(object sender, System.EventArgs e)
		{
			
			//Session.Abandon();
			
			

		}
	
		private void form_PreRender(object sender, System.EventArgs e)
		{
			
		}

		private void btn_chiudi_ServerClick(object sender, System.EventArgs e)
		{
			
		}
	}
}
