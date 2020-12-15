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

namespace DocsPAWA.gestione.registro
{
	/// <summary>
	/// Summary description for chkCasellaIst.
	/// </summary>
	/// 
	
	public class chkCasellaIst : System.Web.UI.Page
	{

		private void startServiteCheckIstitutionalMailbox()
		{
			DocsPaWR.Registro registro=GestManager.getRegistroSel(this);
            registro.email = GestManager.getCasellaSel();// imposto l'indirizzo della casella da interrogare
			DocsPaWR.MailAccountCheckResponse checkResponse;

			bool retValue=GestManager.startIstitutionalMailboxCheck(this,registro,out checkResponse);

			Interoperabilita.MailCheckResponseSessionManager.CurrentMailCheckResponse=checkResponse;

			this.RegisterClientScriptBlock("ShowMailCheckResultWindow","<script>ShowMailCheckResultWindow();</script>");
		}

		private void addScriptToExecute()
		{
			string callingScriptName="executePostBack";
			string callingScriptBody="<script language=jscript>document.forms[0].submit();</script>";
			Page.RegisterStartupScript(callingScriptName,callingScriptBody);
			
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			Utils.startUp(this);

            if (!IsPostBack)
			{
				addScriptToExecute();
			}
			else
			{
				startServiteCheckIstitutionalMailbox();
			}
			
//			string script="<script>alert('service started');</script>";
//			Page.RegisterStartupScript("message",script);
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
