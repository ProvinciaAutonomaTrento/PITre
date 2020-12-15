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

namespace DocsPAWA.popup
{
    public partial class avvisoErroreConnessioneMail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            erroreMail.Text = string.Empty;
            this.erroreMail.Text = Session["messaggiErrorePosta"].ToString();
        }

        protected void btn_si_Click(object sender, EventArgs e)
        {
            erroreMail.Text = string.Empty;
        }
    }
}
