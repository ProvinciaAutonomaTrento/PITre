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

namespace DocsPAWA.SitoAccessibile.Rubrica
{
	/// <summary>
	/// Summary description for Dettagli.
	/// </summary>
	public class Dettagli : SessionWebPage
	{
		string codice = null;
		DocsPaWR.ElementoRubrica er = null;
		Rubrica.SearchContainer srcContainer = null;
		protected System.Web.UI.WebControls.Button btnBack;

		private void Page_Load(object sender, System.EventArgs e)
		{
			QuickSearch();

			// Put user code to initialize the page here
			codice = (string)this.Context.Request.Params["cod"];
			srcContainer = (Rubrica.SearchContainer)Session["DocsPAWA.SitoAccessibile.Rubrica.Rubrica.SearchContainer"];
			er = srcContainer.Get(codice);
		}

		public string Codice
		{
			get { return codice; }
		}

		public DocsPAWA.DocsPaWR.ElementoRubrica ElementoRubrica
		{
			get { return er; }
		}

		public DocsPAWA.DocsPaWR.Corrispondente[] Corrispondenti
		{
			get { return srcContainer.ConvertToCorrispondente(codice); }
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
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

		private void btnBack_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("./Rubrica.aspx");
		}
	}
}
