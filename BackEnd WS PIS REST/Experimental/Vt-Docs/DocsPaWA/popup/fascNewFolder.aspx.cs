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
	/// Summary description for fascNewFolder.
	/// </summary>
    public class fascNewFolder : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label lbl_note;
		protected System.Web.UI.WebControls.Button btn_salva;
		protected System.Web.UI.WebControls.TextBox txt_descFolder;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Button btn_annulla;
	
		private void Page_PreRender(object sender, System.EventArgs e)
		{

		}

		private void Page_Load(object sender, System.EventArgs e)
		{
		
			try
			{
				
			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
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
			this.btn_salva.Click += new System.EventHandler(this.btn_salva_Click);
			this.btn_annulla.Click += new System.EventHandler(this.btn_annulla_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion

		private void btn_annulla_Click(object sender, System.EventArgs e)
		{
			//Session["descNewFolder"]=null;
			Session.Remove("descNewFolder");
			string funct=" window.close(); ";
			Response.Write("<script> " + funct + "</script>");		
		}

		private void btn_salva_Click(object sender, System.EventArgs e)
		{
			try
			{
				string page = Request.QueryString["page"];
				if ("" == txt_descFolder.Text)
				{
					Response.Write("<script>alert('Attenzione! inserire il nome del sotto fascicolo');</script>");					
					return;
				}
				else
				{
					Session["descNewFolder"]=txt_descFolder.Text;
                    if (page.Equals("dettagliFasc"))
                    {
                        //	Response.Write("<script>var k=window.open('../fascicolo/fascDettagliFasc.aspx','iFrame_cn'); window.close();</script>");
                        Response.Write("<script> if ( window.opener!=null){window.opener.document.forms[0].submit();self.close();}</script>");
                    
                    }
                    else
                        Response.Write("<script>var k=window.open('../fascicolo/fascDocumenti.aspx','IframeTabs'); window.close();</script>");
				}
			}
			catch(System.Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}	
		}
	}
}
