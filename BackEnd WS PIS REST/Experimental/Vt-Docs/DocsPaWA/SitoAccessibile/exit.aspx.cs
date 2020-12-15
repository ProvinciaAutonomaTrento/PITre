using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.SitoAccessibile
{
    public partial class exit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UserManager.logoff(Session);

            Session.Clear();

            Page.RegisterStartupScript("chiudi", "<script>chiudi();</script>");
        }
    }
}