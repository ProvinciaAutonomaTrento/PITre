using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA
{
    public partial class BrowserNoCompatible : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string language = Request.QueryString["lang"];
                this.LitBrowserNoCopmatible.Text = UIManager.LoginManager.GetMessageFromCode("WarningBrowserNotCompatible", language);
            }
        }
    }
}