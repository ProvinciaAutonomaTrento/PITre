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
using DocsPAWA.SitoAccessibile.Documenti.Trasmissioni;

namespace DocsPAWA.SitoAccessibile.Ricerca
{
	/// <summary>
	/// Summary description for EsitoRicercaTrasmissioniEffettuate.
	/// </summary>
	public class EsitoRicercaTrasmissioniEffettuate : SessionWebPage
	{
		protected System.Web.UI.WebControls.Button btnBack;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlMessage;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (!this.IsPostBack)
				{
					this.Fetch();
				}
			}
			catch (Exception ex)
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
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		protected override string GetTitleExtraInformation()
		{
			string retValue=string.Empty;
	
			
			int pageNumber=this.GetControlTrasmissioniEffettuate().PagingContext.PageNumber;

			
			retValue=pageNumber.ToString();

			return retValue;
		}

		/// <summary>
		/// Caricamento dati trasmissioni effettuate
		/// </summary>
		private void Fetch()
		{
			this.GetControlTrasmissioniEffettuate().Fetch();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private TrasmissioniEffettuate GetControlTrasmissioniEffettuate()
		{
			return this.FindControl("trasmissioniEffettuate") as TrasmissioniEffettuate;
		}

		private void btnBack_Click(object sender, System.EventArgs e)
		{
			Response.Redirect(EnvironmentContext.RootPath + "Ricerca/Trasmissioni.aspx");
		}
	}
}
