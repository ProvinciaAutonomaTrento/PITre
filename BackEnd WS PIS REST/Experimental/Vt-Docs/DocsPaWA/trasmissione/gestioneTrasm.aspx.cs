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

namespace DocsPAWA.trasmissione
{
	/// <summary>
	/// Summary description for gestioneTrasm.
	/// </summary>
	public class gestioneTrasm : System.Web.UI.Page
	{
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_sx;
		protected DocsPaWebCtrlLibrary.IFrameWebControl iFrame_dx;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			Utils.startUp(this);
			Response.Expires = 0;

			if (!this.IsPostBack)
				this.SetContext();
            
            // querystring passata da "DocsPaWA.documento.docTrasmissioni.aspx.cs" tramite i tasti [Nuova] e [Modifica]
            string azioneQS = Request.QueryString["azione"]; // al momento i possibili valori sono : "Nuova", "Modifica"

			if(Session["OggettoDellaTrasm"]!=null && Session["OggettoDellaTrasm"].ToString().Equals("DOC"))
			{
                this.iFrame_sx.NavigateTo = "trasmDatiTrasm_sx.aspx?azione=" + azioneQS;
                this.iFrame_dx.NavigateTo = "trasmDatiTrasm_dx.aspx?azione=" + azioneQS;
			}
			else
			{
                this.iFrame_sx.NavigateTo = "trasmFascDatiTras_sx.aspx?azione=" + azioneQS;
                this.iFrame_dx.NavigateTo = "trasmFascDatiTras_dx.aspx?azione=" + azioneQS;				
			}
		}

		/// <summary>
		/// Impostazione contesto corrente
		/// </summary>
		private void SetContext()
		{
			string url=DocsPAWA.Utils.getHttpFullPath() + "/trasmissione/gestioneTrasm.aspx";

			SiteNavigation.CallContext newContext=new SiteNavigation.CallContext(SiteNavigation.NavigationKeys.TRASMISSIONE,url);
			newContext.ContextFrameName="top.principale";
			
			if (SiteNavigation.CallContextStack.SetCurrentContext(newContext))
				SiteNavigation.NavigationContext.RefreshNavigation();
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
