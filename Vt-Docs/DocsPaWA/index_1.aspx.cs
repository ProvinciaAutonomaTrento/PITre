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

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for index_1.
	/// </summary>
	public class index_1 : System.Web.UI.Page
	{
		protected DocsPaWebCtrlLibrary.IFrameWebControl superiore;
		protected DocsPaWebCtrlLibrary.IFrameWebControl principale;
		protected DocsPaWebCtrlLibrary.IFrameWebControl inferiore;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			// CaricoFrame
			this.superiore.NavigateTo = "testata320.aspx";
			this.principale.NavigateTo = "sceltaRuolo.aspx";
			this.inferiore.NavigateTo = "bottom.aspx";

			
			// Put user code to initialize the page here
			string tipoOggetto = Request.QueryString["tipoOggetto"];
			string idOggetto = Request.QueryString["idObj"];
			
		
			
			if(tipoOggetto != null && idOggetto != null) 
			{
				DocsPaWR.InfoDocumento infoDoc = DocumentManager.getRisultatoRicerca(this);
				
				if(infoDoc != null)
				{	
					string nomeTab;
					if (infoDoc.numProt != null && infoDoc.numProt.Length > 0)
						nomeTab = "protocollo";
					else
						nomeTab = "profilo";
					
					this.principale.NavigateTo = "documento/gestionedoc.aspx?tab" + nomeTab;
				}

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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
