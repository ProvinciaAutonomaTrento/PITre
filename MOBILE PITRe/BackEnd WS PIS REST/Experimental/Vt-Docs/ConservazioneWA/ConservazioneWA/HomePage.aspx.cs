using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace ConservazioneWA
{
    public partial class HomePage : ConservazioneWA.CssPage
    {
        protected System.Web.UI.HtmlControls.HtmlTableCell backgroundLogoEnte;

        protected void Page_Load(object sender, EventArgs e)
        {
            string ruolo = Request.QueryString["ruolo"];
            this.lbl_ruolo.Text = ruolo;

            this.backgroundLogoEnte.Attributes.Add("background", "App_Themes\\" + this.Page.Theme + "\\backgroundlogo.gif");
            this.img_logo.ImageUrl = "App_Themes\\" + this.Page.Theme + "\\Img_logo.gif";
        }
    }
}
