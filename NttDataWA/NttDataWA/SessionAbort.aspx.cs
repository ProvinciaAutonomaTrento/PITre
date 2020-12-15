using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA
{
    public partial class SessionAbort : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {               
                this.GestLivelloChiusuraPopUp();
                this.InitializeLanguage();
                this.InitializePage();                
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {

            string layout = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.LAYOUT.ToString()];

            if (layout != null && layout.ToUpper().Equals("GFD"))
                img_logo_login.ImageUrl = "Images/Common/logo_login_gfd.jpg";
            else
                img_logo_login.ImageUrl = "Images/Common/logo_login.jpg";

        }

        private void GestLivelloChiusuraPopUp()
        {
            string MessageType = string.Empty;

            if (Request.QueryString["MessageType"] != null)
            {
                MessageType = Request.QueryString["MessageType"];
            }

            if (Convert.ToString(Request.QueryString["param"]).Equals("StepLogin"))
            {
                Context.Response.Write("<script type='text/javascript'>window.open('Login.aspx', '_parent');</script>");
            }

            if (!string.IsNullOrEmpty(Request.QueryString["param"]))
            {
                switch (Convert.ToString(Request.QueryString["param"]))
                {                
                    case "Step1":
                        Context.Response.Write("<script type='text/javascript'>window.open('SessionAbort.aspx?param=Step2&MessageType=" + MessageType + "', '_parent');</script>");
                        break;
                    case "Step2":
                        Context.Response.Write("<script type='text/javascript'>window.open('SessionAbort.aspx?param=StepStop&MessageType=" + MessageType + "', '_parent');</script>");                    
                        break;
                }
            }
            else
            {
                Context.Response.Write("<script type='text/javascript'>window.open('SessionAbort.aspx?param=Step1&MessageType=" + MessageType + "', '_parent');</script>");
            }
        }

        public void InitializeLanguage()
        {
            this.BtnLogin.Text = "Accedi";
        }

        public void InitializePage()
        {
            switch (Convert.ToString(Request.QueryString["MessageType"]))
            {
                case "1":
                    this.LblErrorIta.Text = "La sessione è scaduta.";
                    break;
                case "2":
                    this.LblErrorIta.Text = "L'utente si è connesso da un'altra postazione.";
                    break;
                default:
                    this.LblErrorIta.Text = "Si è verificato un problema.";
                    this.BtnLogin.Text = "Accedi";
                    break;
            }
        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}