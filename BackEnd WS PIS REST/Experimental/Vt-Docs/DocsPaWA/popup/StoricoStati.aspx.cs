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

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for StoricoStati.
	/// </summary>
    public class StoricoStati : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label lbl_NomeModello;
		protected System.Web.UI.WebControls.DataGrid dgStoricoStati;
		protected System.Web.UI.WebControls.Button btn_Chiudi;
		protected DocsPaWebService wws = new DocsPaWebService();
	
		private void Page_Load(object sender, System.EventArgs e)
		{
            string tipo=string.Empty;

            if (Request.QueryString["tipo"] != null)
            {
                tipo = Request.QueryString["tipo"].Substring(0, 1);
                switch (tipo)
                { 
                    case "D":
                        dgStoricoStati.DataSource = DocsPAWA.DiagrammiManager.getDiagrammaStoricoDoc(DocumentManager.getDocumentoSelezionato(this).docNumber,this);
                        dgStoricoStati.DataBind();
                        break;

                    case "F":
                        dgStoricoStati.DataSource = DocsPAWA.DiagrammiManager.getDiagrammaStoricoFasc(FascicoliManager.getFascicoloSelezionato(this).systemID,this);
                        dgStoricoStati.DataBind();
                        break;
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
			this.btn_Chiudi.Click += new System.EventHandler(this.btn_Chiudi_Click);
			this.dgStoricoStati.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgStoricoStati_PageIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btn_Chiudi_Click(object sender, System.EventArgs e)
		{
			RegisterStartupScript("chiudiFinestra","<script>chiudiFinestra();</script>");
		}

		private void dgStoricoStati_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
            
             string tipo=string.Empty;

            if (Request.QueryString["tipo"] != null)
            {
                tipo = Request.QueryString["tipo"].Substring(0, 1);

                switch (tipo)
                {
                    case "D":
                        dgStoricoStati.CurrentPageIndex = e.NewPageIndex;
                        dgStoricoStati.DataSource = DocsPAWA.DiagrammiManager.getDiagrammaStoricoDoc(DocumentManager.getDocumentoSelezionato(this).docNumber, this);
                        dgStoricoStati.DataBind();
                        break;

                    case "F":
                        dgStoricoStati.CurrentPageIndex = e.NewPageIndex;
                        dgStoricoStati.DataSource = DocsPAWA.DiagrammiManager.getDiagrammaStoricoFasc(FascicoliManager.getFascicoloSelezionato(this).systemID, this);
                        dgStoricoStati.DataBind();
                        break;
                }
            }            
		}
	}
}
