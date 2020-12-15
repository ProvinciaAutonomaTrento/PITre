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
using System.Web.Security;

namespace Amministrazione
{
    /// <summary>
    /// Summary description for Exit.
    /// </summary>
    public class Exit : System.Web.UI.Page
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            this.Session.Abandon();

            string sessionID = Session.SessionID;

            SAAdminTool.AdminTool.Manager.SessionManager sessionManager = new SAAdminTool.AdminTool.Manager.SessionManager();
            
            AmmUtils.WebServiceLink web = new AmmUtils.WebServiceLink();
            web.Logout(sessionManager.getUserAmmSession());

            switch (Request.QueryString["FROM"])
            {
                case "ABORT":
                    Response.Redirect("login.htm");
                    break;

                case "EXPIRED":
                    FormsAuthentication.SignOut();
                    Response.Redirect("login.htm");
                    break;
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
