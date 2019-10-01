using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace DocsPAWA.popup
{
    public partial class LogImportRubrica : DocsPAWA.CssPage
    {
        protected DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            if (!IsPostBack)
            {
                ArrayList fileLog = new ArrayList(ws.getLogImportRubrica());
                foreach (string sOutput in fileLog)
                    txt_area.Text += sOutput + "\n";
                txt_area.ReadOnly = true;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chiudeFinestra", "window.close();", true);
        }
    }
}
