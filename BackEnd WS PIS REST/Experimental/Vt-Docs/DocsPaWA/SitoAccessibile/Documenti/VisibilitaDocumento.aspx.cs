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

namespace DocsPAWA.SitoAccessibile.Documenti
{
	/// <summary>
	/// Summary description for VisibilitaDocumento.
	/// </summary>
	public class VisibilitaDocumento : SessionWebPage
	{
		protected string idProfile;
		protected string docNum;
		protected string activeMenu;
		protected System.Web.UI.WebControls.Button btnBack;
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		private SchedaDocumento _schedaDocumento=null;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{

				// Reperimento parametri query string
				this.idProfile=this.GetQueryStringParameter("iddoc");
				this.docNum=this.GetQueryStringParameter("docnum");
				this.activeMenu=this.GetQueryStringParameter("activemenu");
				
				if (idProfile!=string.Empty && docNum!=string.Empty)
				{
					DocumentoHandler handler=new DocumentoHandler();
					this._schedaDocumento=handler.GetDocumento(this.idProfile,this.docNum);
					
					this.Fetch();
				}
				else
				{
					throw new ApplicationException("Parametri mancanti");
				}
			}
			catch(Exception ex)
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

		private void Fetch()
		{
			DettagliVisibilitaDocumento details=this.GetControlDettagliVisibilita();
			details.Fetch(this._schedaDocumento.systemId);
		}

		private DettagliVisibilitaDocumento GetControlDettagliVisibilita()
		{
			return this.FindControl("DettagliVisibilitaDocumento") as DettagliVisibilitaDocumento;
		}

		private void btnBack_Click(object sender, System.EventArgs e)
		{
			Response.Redirect(this.BackUrl);
		}
	}
}
