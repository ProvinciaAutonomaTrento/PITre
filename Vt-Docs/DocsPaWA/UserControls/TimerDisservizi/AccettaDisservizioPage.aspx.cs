using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.UserControls.TimerDisservizi
{
    public partial class AccettaDisservizioPage : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ProxyManager.getWS().accettaDisservizio(UserManager.getInfoUtente().idPeople);
        }
    }
}