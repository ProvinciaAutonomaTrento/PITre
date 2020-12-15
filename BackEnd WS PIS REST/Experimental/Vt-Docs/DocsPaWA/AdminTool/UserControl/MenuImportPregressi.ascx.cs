using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.UserControl
{
    public partial class MenuImportPregressi : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GestioneGrafica();
            }
        }

        protected void GestioneGrafica()
        {
            this.linkNuovoImport.Attributes.Add("onmouseover", "this.src='" + "../Images/up_admin_menu.gif'");
            this.linkNuovoImport.Attributes.Add("onmouseout", "this.src='" + "../Images/down_admin_menu.gif'");
            this.linkStatoImport.Attributes.Add("onmouseover", "this.src='" + "../Images/up_admin_menu.gif'");
            this.linkStatoImport.Attributes.Add("onmouseout", "this.src='" + "../Images/down_admin_menu.gif'");
        }
    }
}