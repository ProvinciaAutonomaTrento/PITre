using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA
{
    public partial class AutenticazioneCentralizzata : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if( !string.IsNullOrEmpty(this.Request.QueryString["userID"]) &&
                !string.IsNullOrEmpty(this.Request.QueryString["password"]) 
                )
            {
                txt_userID.Value = this.Request.QueryString["userID"];
                txt_password.Value = this.Request.QueryString["password"];
                
                DocsPaWR.UserLogin userLogin = CreateUserLogin();

                DocsPaWR.LoginResult loginResult;
                string ipAddress = string.Empty;

                DocsPaWR.Utente utente = UserManager.login(this, userLogin, out loginResult, out ipAddress);
                Session["userData"] = utente;

                if (loginResult == DocsPAWA.DocsPaWR.LoginResult.OK)
                {
                    utente.urlWA = Utils.getHttpFullPath(this);
                    LaunchApplication();
                }
                else
                {
                    this.Server.Transfer("login.aspx");  
                }                
            }            
        }

        private DocsPaWR.UserLogin CreateUserLogin()
        {
            DocsPaWR.UserLogin userLogin = new DocsPAWA.DocsPaWR.UserLogin();
            userLogin.UserName = txt_userID.Value;
            userLogin.Password = txt_password.Value;
            
            //string returnMsg = string.Empty;
            //DocsPaWR.Amministrazione[] amministrazioni = UserManager.getListaAmministrazioni(this, out returnMsg);
            //userLogin.IdAmministrazione = amministrazioni[0].systemId;
            
            userLogin.IPAddress = this.Request.UserHostAddress;
            return userLogin;
        }

        private void LaunchApplication()
        {
            string script = " var popup = window.open('index.aspx','Index','location=0,resizable=yes');";
            script += " popup.moveTo(0,0);";
            script += " popup.resizeTo(screen.availWidth,screen.availHeight);";
            script += " if(popup!=self) {window.opener=null;self.close();}";
            this.ClientScript.RegisterStartupScript(this.GetType(), "Index", script, true);                    
        }
    }   
}

