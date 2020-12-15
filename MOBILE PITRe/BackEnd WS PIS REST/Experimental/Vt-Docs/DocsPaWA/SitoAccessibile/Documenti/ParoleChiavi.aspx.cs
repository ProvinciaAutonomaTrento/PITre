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
	/// Gestione visualizzazione parole chiavi da selezionare
	/// </summary>
	public class ParoleChiavi : SessionWebPage
	{
		protected System.Web.UI.WebControls.Button btnBack;
		protected System.Web.UI.WebControls.Button btnOK;

		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
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
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private const string SESSION_KEY="ParoleChiavi";

		/// <summary>
		/// Verifica se si è in fase di selezione delle parole chiavi
		/// </summary>
		public static bool OnSelectParoleChiavi
		{
			get
			{
				return (HttpContext.Current.Session[SESSION_KEY]!=null);
			}
		}

		/// <summary>
		/// Reperimento parole chiavi selezionate da sessione
		/// </summary>
		/// <returns></returns>
		public static DocumentoParolaChiave[] GetParoleChiavi()
		{
			DocumentoParolaChiave[] retValue=null;

			if (HttpContext.Current.Session[SESSION_KEY]!=null)
			{
				retValue=(DocumentoParolaChiave[]) HttpContext.Current.Session[SESSION_KEY];
				HttpContext.Current.Session.Remove(SESSION_KEY);
			}

			return retValue;
		}

		/// <summary>
		/// Reperimento parole chiavi selezionate
		/// </summary>
		/// <returns></returns>
		private DocumentoParolaChiave[] GetSelectedParoleChiavi()
		{
			return this.GetControlListaParoleChiavi().GetSelectedParoleChiavi();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private ListaParoleChiavi GetControlListaParoleChiavi()
		{
			return this.FindControl("listaParoleChiavi") as ListaParoleChiavi;
		}

		/// <summary>
		/// Redirect alla pagina chiamante
		/// </summary>
		private void RedirectToPreviousPage()
		{
			Response.Redirect(this.BackUrl);
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{			
			HttpContext.Current.Session[SESSION_KEY]=GetSelectedParoleChiavi();

			this.RedirectToPreviousPage();
		}

		private void btnBack_Click(object sender, System.EventArgs e)
		{
			this.RedirectToPreviousPage();
		}
	}
}