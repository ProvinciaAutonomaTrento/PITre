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

namespace DocsPAWA.Diagnostics
{
	/// <summary>
	/// Summary description for index.
	/// </summary>
	public class index : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button StartButton;
		protected System.Web.UI.WebControls.Button ResetButton;
		protected System.Web.UI.WebControls.TextBox ExceptionMessage1;
		protected System.Web.UI.WebControls.TextBox ExceptionMessage2;
		protected System.Web.UI.WebControls.Label DPA_Amministra;
		protected System.Web.UI.WebControls.Label DatabaseConnectionTest;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
			{
				ResetButton_Click(null, null);
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
			this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
			this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StartButton_Click(object sender, System.EventArgs e)
		{
			string exceptionMessage;

			if(CheckDatabaseConnection(out exceptionMessage))
			{
				this.DatabaseConnectionTest.Text = "OK";
				this.ExceptionMessage1.Text = "No Exception";
			}
			else
			{
				this.DatabaseConnectionTest.Text = "Fallito";
				this.ExceptionMessage1.Text = exceptionMessage;
				Trace.Warn(exceptionMessage);
			}

			if(CheckDPA_Amministra(out exceptionMessage))
			{
				this.DPA_Amministra.Text = "OK";
				this.ExceptionMessage2.Text = "No Exception";
			}
			else
			{
				this.DPA_Amministra.Text = "Fallito";
				this.ExceptionMessage2.Text = exceptionMessage;
				Trace.Warn(exceptionMessage);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="exceptionMessage"></param>
		/// <returns></returns>
		private bool CheckDatabaseConnection(out string exceptionMessage)
		{
			DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();

			return docsPaWS.CheckDatabaseConnection(out exceptionMessage);
		}

		/// <summary>
		/// </summary>
		/// <param name="exceptionMessage"></param>
		/// <returns></returns>
		private bool CheckDPA_Amministra(out string exceptionMessage)
		{
			DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();

			return docsPaWS.CheckDPA_Amministra(out exceptionMessage);
		}

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ResetButton_Click(object sender, System.EventArgs e)
		{
			this.DatabaseConnectionTest.Text = "Da Iniziare";
			this.DPA_Amministra.Text = "Da Iniziare";
			this.ExceptionMessage1.Text = "";
			this.ExceptionMessage2.Text = "";
		}
	}
}

