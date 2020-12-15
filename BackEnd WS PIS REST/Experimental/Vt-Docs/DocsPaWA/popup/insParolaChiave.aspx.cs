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
	/// Summary description for insParolaChiave.
	/// </summary>
    public class insParolaChiave : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btn_Insert;
		protected System.Web.UI.WebControls.Label lbl_parolaC;
		protected System.Web.UI.WebControls.TextBox txt_parolaC;
		protected System.Web.UI.WebControls.Button Button1;
		protected System.Web.UI.WebControls.Label Label1;
	
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
			this.btn_Insert.Click += new System.EventHandler(this.btn_Insert_Click);
			this.Button1.Click += new System.EventHandler(this.Button1_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btn_Insert_Click(object sender, System.EventArgs e)
		{
			try 
			{
				DocsPaWR.DocumentoParolaChiave parolaC = new DocsPAWA.DocsPaWR.DocumentoParolaChiave();
				
				string msg;
				
				//controllo sull'inserimento della parola chiave
				if (this.txt_parolaC.Text.Equals("") || this.txt_parolaC.Text == null ) 
				{
					msg = "Inserire il valore: parola chiave";
					Response.Write("<script>alert('" + msg + "');</script>");
					return;
				}
				
				parolaC.descrizione = this.txt_parolaC.Text.ToUpper();


				parolaC = DocumentManager.addParolaChiave(this, parolaC);

				if (parolaC != null)
				{
					Response.Write("<script>window.opener.paroleChiave.h_aggiorna.value='S'; alert('Operazione effettuata con successo');</script>");
					this.txt_parolaC.Text = "";
				}
				else /* modifica per gestione dato presente */
				{
					Response.Write("<script>window.opener.paroleChiave.h_aggiorna.value='N'; alert('Attenzione.Parola chiave già presente');</script>");
				}
			}
			catch(Exception es)
			{
				ErrorManager.redirectToErrorPage(this,es);
				
			}

		}

		private void Button1_Click(object sender, System.EventArgs e)
		{
		Response.Write("<script> window.opener.paroleChiave.submit(); window.close();</script>");
		}
	}
}
