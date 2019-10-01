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
	/// Summary description for insTipoAtto.
	/// </summary>
    public class insTipoAtto : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Label lbl_tipoAtto;
		protected System.Web.UI.WebControls.TextBox txt_tipoAtto;
		protected System.Web.UI.WebControls.Button btn_Insert;
		protected System.Web.UI.WebControls.Button Button1;
		protected string wnd;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			wnd = Request.QueryString["wnd"];
			SetFocus(txt_tipoAtto.ID);
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
				DocsPaWR.TipologiaAtto tipoAtto = new DocsPAWA.DocsPaWR.TipologiaAtto();
				
				string msg;
				
				this.txt_tipoAtto.Text = this.txt_tipoAtto.Text.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
				//controllo sull'inserimento della parola chiave
				if (this.txt_tipoAtto.Text.Equals("") || this.txt_tipoAtto.Text == null ) 
				{
					msg = "Inserire il valore: tipo Atto";
					Response.Write("<script>alert('" + msg + "');</script>");
					return;
				}
				
					tipoAtto.descrizione = this.txt_tipoAtto.Text.ToUpper();				


				tipoAtto = DocumentManager.addTipologiaAtto(this, tipoAtto);

				if (tipoAtto != null)
				{
					Response.Write("<script>window.opener.document."+ wnd +".h_tipoAtto.value='"+tipoAtto.systemId+"'; window.opener.document."+ wnd +".submit(); alert('Operazione effettuata con successo');</script>");
					this.txt_tipoAtto.Text = "";
				}
				else /* modifica per gestione dato presente */
				{
					Response.Write("<script>window.opener."+ wnd +".h_tipoAtto.value='N'; alert('Attenzione. Tipologia atto già presente');</script>");
				}
			}
			catch(Exception es)
			{
				ErrorManager.redirectToErrorPage(this,es);
				
			}

		}

		//imposta il focus su un controllo
		private void SetFocus(string controlID)
		{
			string s = "<SCRIPT language='javascript'>document.getElementById('" + controlID + "').focus() </SCRIPT>";
			RegisterStartupScript("focus", s);
		}

		private void Button1_Click(object sender, System.EventArgs e)
		{
			//Response.Write("<script> window.opener."+ wnd +".submit(); window.close();</script>");
			Response.Write("<script>window.close();</script>");

		}
	}
}
