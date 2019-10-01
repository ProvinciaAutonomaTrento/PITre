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
    public partial class VisibilitaTitolario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txt_errori.Text = string.Empty;
            string mes = Session["mex"].ToString();
            if(!string.IsNullOrEmpty(mes))
            {
                txt_errori.Text = mes.Replace("\n"," ");
                txt_errori.Text = txt_errori.Text.Replace("\\n", " ");
            }
        }
    }
}
