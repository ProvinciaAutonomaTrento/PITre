using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Project.ImportExport
{
    public partial class setLastPath : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string lastPath = (Request.QueryString["lastPath"] != null ? Request.QueryString["lastPath"].ToString() : String.Empty);

            if (!string.IsNullOrEmpty(lastPath))
            {
                Session["lastPath"] = Utils.utils.FormatJs(lastPath);
            }
        }
    }
}