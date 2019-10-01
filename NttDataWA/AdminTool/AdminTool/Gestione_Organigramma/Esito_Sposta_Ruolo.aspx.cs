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

namespace SAAdminTool.AdminTool.Gestione_Organigramma
{
	/// <summary>
	/// Summary description for Esito_Sposta_Ruolo.
	/// </summary>
	public class Esito_Sposta_Ruolo : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Button si;
		protected System.Web.UI.HtmlControls.HtmlInputHidden idAmm;
		protected System.Web.UI.HtmlControls.HtmlInputHidden idCorrGlobUO;
		protected System.Web.UI.HtmlControls.HtmlInputHidden idCorrGlobRuolo;
		protected System.Web.UI.HtmlControls.HtmlInputHidden idGruppo;
		protected System.Web.UI.WebControls.Button no;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				this.idAmm.Value = this.Request.QueryString["idAmm"].ToString();
				this.idCorrGlobUO.Value = this.Request.QueryString["idCorrGlobUO"].ToString();
				this.idCorrGlobRuolo.Value = this.Request.QueryString["idCorrGlobRuolo"].ToString();
				this.idGruppo.Value = this.Request.QueryString["idGruppo"].ToString();
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
			this.si.Click += new System.EventHandler(this.si_Click);
			this.no.Click += new System.EventHandler(this.no_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void no_Click(object sender, System.EventArgs e)
		{
			this.executeJS("<SCRIPT>window.returnValue = 'Y'; self.close();</SCRIPT>");
		}
		
		private void si_Click(object sender, System.EventArgs e)
		{
			string qs = "?from=SR&idAmm="+this.idAmm.Value+"&idCorrGlobUO="+this.idCorrGlobUO.Value+"&idCorrGlobRuolo="+this.idCorrGlobRuolo.Value+"&idGruppo="+this.idGruppo.Value;
			Response.Redirect("GestVisibilita.aspx" + qs);
		}

		private void executeJS(string key)
		{
			if(!this.Page.IsStartupScriptRegistered("theJS"))			
			//this.Page.RegisterStartupScript("theJS", key);
            this.ClientScript.RegisterStartupScript(this.GetType(), "theJS", key);
		}
	}
}
