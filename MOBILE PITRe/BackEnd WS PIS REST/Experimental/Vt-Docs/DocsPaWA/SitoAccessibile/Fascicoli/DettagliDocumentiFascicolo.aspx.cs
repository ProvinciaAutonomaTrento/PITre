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

namespace DocsPAWA.SitoAccessibile.Fascicoli
{
	/// <summary>
	/// Pagina relativa ai documenti contenuti in un fascicolo
	/// </summary>
	public class DettagliDocumentiFascicolo : SessionWebPage
	{
		protected System.Web.UI.WebControls.Button btnBack;

		private string _idFascicolo=string.Empty;
		private string _idFolder=string.Empty;
		private string _idRegistro=string.Empty;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				this._idFascicolo=this.GetQueryStringParameter("idFascicolo");
				if (this._idFascicolo==string.Empty)
					throw new ApplicationException("Parametro 'idFascicolo' non fornito");

				this._idFolder=this.GetQueryStringParameter("idFolder");
				if (this._idFolder==string.Empty)
					throw new ApplicationException("Parametro 'idFolder' non fornito");

				this._idRegistro=this.GetQueryStringParameter("idRegistro");

				this.GetControlDocumentiFascicolo().Initialize(this._idFascicolo,this._idFolder,this._idRegistro);
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
		/// Reperimento controllo documenti contenuti nel fascicolo
		/// </summary>
		/// <returns></returns>
		private DocumentiFascicolo GetControlDocumentiFascicolo()
		{
			return this.FindControl("DocumentiFascicolo") as DocumentiFascicolo;
		}

		private void btnBack_Click(object sender, System.EventArgs e)
		{	
			string url=EnvironmentContext.RootPath + "Fascicoli/Fascicolo.aspx?idFascicolo=" + this._idFascicolo + "&idRegistro=" + this._idRegistro;

			Response.Redirect(url);
		}
	}
}
