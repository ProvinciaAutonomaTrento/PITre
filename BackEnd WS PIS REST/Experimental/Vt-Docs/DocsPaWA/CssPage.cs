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
	/// Summary description for CssPage.
	/// </summary>
    public class CssPage : System.Web.UI.Page
    {
        public string CssTheme;

        public CssPage()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private string GetCssAmministrazione()
        {
            string Tema = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
            {
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                DocsPAWA.UserManager userM = new UserManager();
                Tema = utils.InitImagePath.getInstance(idAmm).getPath("CSS");
               
            }
            else
            {
                //if (UserManager.getInfoUtente() != null && !string.IsNullOrEmpty(UserManager.getInfoUtente().idAmministrazione))
                if (UserManager.getUtente() != null && !string.IsNullOrEmpty(UserManager.getUtente().idAmministrazione))
                {
                    string idAmm = UserManager.getInfoUtente().idAmministrazione;
                    DocsPAWA.UserManager userM = new UserManager();
                    Tema = utils.InitImagePath.getInstance(idAmm).getPath("CSS");
                }
            }
            return Tema;
        }

        protected override void OnPreInit(EventArgs e)
        {
            string Tema = GetCssAmministrazione();
            if (Tema != null && !Tema.Equals(""))
            {
                string[] realTema = Tema.Split('^');
                base.Theme = realTema[0];
            }
            else
                base.Theme = "TemaRosso";

            base.OnPreInit(e);
        }

        protected override void OnInit(EventArgs e)
        {
            //base.Page.MaintainScrollPositionOnPostBack = true;
            try
            {
                base.OnInit(e);
            }
            catch (Exception er)
            {
                string error = er.Message;
            }
        }
    }
}
