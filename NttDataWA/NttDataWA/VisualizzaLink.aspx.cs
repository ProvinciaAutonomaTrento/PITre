using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA
{
    public partial class VisualizzaLink : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = "~/CheckInOut/OpenDirectLink.aspx";
            string qs = "from=file&"+Request.QueryString.ToString();

            // old: groupId=76210346&docNumber=77992841&idProfile=77992841&numVersion=

            Response.Redirect(path+"?"+qs);
        }
    }
}