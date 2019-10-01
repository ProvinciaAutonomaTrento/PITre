using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;

namespace NttDataWA
{
    public partial class LogOut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoginManager.LogOut(this.Session.SessionID);
            if ((string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.DISABLE_LOGOUT_CLOSE_BUTTON.ToString()]) || System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.DISABLE_LOGOUT_CLOSE_BUTTON.ToString()].ToUpper() != Boolean.TrueString.ToUpper()))
            {
                Session.Abandon();
            }
            if (!string.IsNullOrEmpty(Request.QueryString["param"]) && Request.QueryString["param"].Equals("CloseWindow"))
            {
                Context.Response.Write("<script type='text/javascript'>window.close();</script>");
            }
            else
            {
                string logoutRedirectUrl = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.LOGOUT_REDIRECT_URL.ToString()];
                if (string.IsNullOrEmpty(logoutRedirectUrl))
                    Response.Redirect("Login.aspx");
                else
                    System.Web.HttpContext.Current.Response.Redirect(logoutRedirectUrl,true);
            }
        }
    }
}