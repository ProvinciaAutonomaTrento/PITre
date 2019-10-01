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

namespace Amministrazione
{
	/// <summary>
	/// Summary description for CambiaPwd.
	/// </summary>
	public class CambiaPwd : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox txt_newPwd;
		protected System.Web.UI.WebControls.TextBox txt_Conf_newPwd;
		protected System.Web.UI.WebControls.Button btn_cambia;
		protected System.Web.UI.WebControls.Label lbl_userid;
		protected System.Web.UI.WebControls.Label lbl_msg;
		protected System.Web.UI.WebControls.Label lbl_esito;
		protected System.Web.UI.WebControls.Panel pnl_esito;
		protected System.Web.UI.WebControls.Panel pnl_login;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			//----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
			if(Session.IsNewSession)
			{
				Response.Redirect("../Exit.aspx?FROM=EXPIRED");
			}
			
			AmmUtils.WebServiceLink  ws=new AmmUtils.WebServiceLink();
			if(!ws.CheckSession(Session.SessionID))
			{
				Response.Redirect("../Exit.aspx?FROM=ABORT");
			}
			// ---------------------------------------------------------------

			lbl_userid.Text = (string) Session["UserIdAdmin"];
			pnl_esito.Visible = false;
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
			this.btn_cambia.Click += new System.EventHandler(this.btn_cambia_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
        
		private void btn_cambia_Click(object sender, System.EventArgs e)
		{
            string failDetails;
            
            // Modifica password
            if (SetNewPassword(this.lbl_userid.Text, this.txt_newPwd.Text, this.txt_Conf_newPwd.Text, out failDetails))
            {
                this.lbl_msg.Text = "";
                this.pnl_login.Visible = false;
                this.pnl_esito.Visible = true;
                this.lbl_esito.Text = "Password aggiornata con successo.";
            }
            else
            {
                this.lbl_msg.Text = failDetails;
            }
		}

        /// <summary>
        /// Inserimento della nuova password per l'utente amministratore
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <param name="passwordConfirm"></param>
        /// <param name="failDetails"></param>
        /// <returns></returns>
        protected virtual bool SetNewPassword(string userID,
                                string pwd, string pwdConfirm,
                                out string failDetails)
        {
            bool retValue = false;
            failDetails = string.Empty;

            // Verifica della validità della nuova password immessa
            retValue = (pwd.Trim().Length > 0 && pwdConfirm.Trim().Length > 0);

            if (!retValue)
            {
                failDetails = "Immettere la nuova password";
            }
            else if (string.Compare(pwd, pwdConfirm, false) != 0)
            {
                failDetails = "I valori immessi nei campi password e conferma password non coincidono";
                retValue = false;
            }

            if (retValue)
            {
                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                SAAdminTool.DocsPaWR.ValidationResultInfo result = ws.AdminChangePassword(this.txt_newPwd.Text);

                retValue = (result.Value);

                if (!retValue)
                {
                    failDetails = result.BrokenRules[0].Description;
                }
            }

            return retValue;
        }
	}
}
