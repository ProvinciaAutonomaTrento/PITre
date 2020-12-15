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

namespace NttDataWA.CheckInOut
{
	/// <summary>
	/// Gestione eventuali commenti al CheckIn del documento
	/// </summary>
	public class CommentsPage : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox txtComments;
		protected System.Web.UI.HtmlControls.HtmlTable tblContainer;
		protected System.Web.UI.WebControls.Button btnCheckIn;
		protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.Label lblComments;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Expires=-1;
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
			this.btnCheckIn.Click += new System.EventHandler(this.btnCheckIn_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnCheckIn_Click(object sender, System.EventArgs e)
		{
            if (this.txtComments.Text.Length > 200)
            {
                this.RegisterClientScript("alert", "alert('Consentita lunghezza massima di 200 caratteri per il campo note di rilascio');");
            }
            else
            {
                // Impostazione dei commenti nell'oggetto di contesto del checkout
                if (CheckOutContext.Current != null)
                    CheckOutContext.Current.CheckInComments = this.txtComments.Text;

                this.RegisterClientScript("Close", "ClosePage(true);");
            }
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.RegisterClientScript("Close","ClosePage(false);");
		}

		/// <summary>
		/// Registrazione script client
		/// </summary>
		/// <param name="scriptKey"></param>
		/// <param name="scriptValue"></param>
		private void RegisterClientScript(string scriptKey,string scriptValue)
		{
			if(!this.Page.IsStartupScriptRegistered(scriptKey))
			{
				string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
				this.Page.RegisterStartupScript(scriptKey, scriptString);
			}
		}
	}
}
