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
	/// Summary description for dettagliFirmaButtons.
	/// </summary>
    public class dettagliFirmaButtons : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Button btnChiudi;
		protected System.Web.UI.WebControls.Button btnVisualizza;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			this.btnChiudi.Attributes.Add("OnClick","CloseMask();");
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
			this.btnVisualizza.Click += new System.EventHandler(this.btnVisualizza_Click);
			this.btnChiudi.Click += new System.EventHandler(this.btnChiudi_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion


		private void btnVisualizza_Click(object sender, System.EventArgs e)
		{
			this.ShowSignedDocument();
		}

		private void ShowSignedDocument()
		{
			string pageUrl="../documento/docVisualizzaFrame.aspx?id=" + Session.SessionID;
			string targetName="zoom";
			string script = "<script language=JavaScript>%1%</script>";
			string scriptBody = null;
			scriptBody = "ApriFinestraMassimizzata('" +pageUrl +"','" +targetName+"')";
			script = script.Replace("%1%", scriptBody);
			this.RegisterStartupScript("ZoomDoc",script );
		}

		private void btnChiudi_Click(object sender, System.EventArgs e)
		{
			if (!this.LoadSignedDocumentFromSession())
				DocumentManager.RemoveSignedDocument();
			else
				Session["docToSign"]=null;
		}

		/// <summary>
		/// Reperimento parametro in query string per verificare la modalità
		/// con la quale è stato richiesto il servizio di visualizzazione 
		/// dettagli file firmato:
		/// - se "SIGNED_DOCUMENT_ON_SESSION" è presente ed è a "true",
		///   il file firmato (oggetto "DocsPaWR.FileDocumento")
		///   è reperito direttamente dalla session mediante il metodo 
		///   "DocumentManager.GetSignedDocument()"
		/// - se non presente o è impostato a "false", il file firmato
		///   viene reperito direttamente da backend (webmethod "docsPaWS.DocumentoGetFile")
		///   mediante un oggetto di tipo "DocsPaWR.FileRequest"
		/// </summary>
		/// <returns></returns>
		private bool LoadSignedDocumentFromSession()
		{
			bool retValue=false;
			string reqArgs=Request.QueryString["SIGNED_DOCUMENT_ON_SESSION"];
			
			if (reqArgs!=null && !reqArgs.Equals(string.Empty))
				retValue=Convert.ToBoolean(reqArgs);

			return retValue;
		}
	}
}
