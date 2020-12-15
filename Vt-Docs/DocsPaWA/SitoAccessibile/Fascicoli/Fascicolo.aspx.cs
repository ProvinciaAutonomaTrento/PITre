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
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Fascicoli
{
	/// <summary>
	/// Summary description for Fascicolo.
	/// </summary>
	public class Fascicolo : SessionWebPage
	{
		protected System.Web.UI.WebControls.Button btnBack;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				string idFascicolo=this.GetQueryStringParameter("idFascicolo");
				string idRegistro=this.GetQueryStringParameter("idRegistro");

				this.GetControlDettagliFascicolo().Fetch(idFascicolo,idRegistro,true);
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

		/// <summary>
		/// Reperimento controllo dettagli del fascicolo
		/// </summary>
		/// <returns></returns>
		private DettagliFascicolo GetControlDettagliFascicolo()
		{
			return this.FindControl("DettagliFascicolo") as DettagliFascicolo;
		}

		private void btnBack_Click(object sender, System.EventArgs e)
		{
			CallerContext context=CallerContext.GetCallerContext();
			context.AdditionalParameters.Add("searchPage",context.PageNumber);
			Response.Redirect(context.Url);
		}

		/// <summary>
		/// Reperimento, in caso di provenienza da ricerche,
		/// della pagina da cui è stato visualizzato il fascicolo
		/// </summary>
		/// <returns></returns>
		private int FromSearchPageNumber()
		{
			int pageNumber=1;

			if (this.GetQueryStringParameter("fromSearchPage")!=string.Empty)
			{
				try
				{
					pageNumber=Convert.ToInt32(this.GetQueryStringParameter("fromSearchPage"));
				}
				catch
				{
				}
			}

			return pageNumber;
		}
	}
}
