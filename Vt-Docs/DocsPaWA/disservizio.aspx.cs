using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA
{
    public partial class disservizio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DocsPAWA.DocsPaWR.Disservizio disservizio = new DocsPaWR.Disservizio();
            disservizio = ProxyManager.getWS().getInfoDisservizio();
            lbl_mgserrore.Text = disservizio.testo_cortesia;

        }
    }
}