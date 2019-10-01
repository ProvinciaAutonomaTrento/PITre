using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using System.IO;

namespace NttDataWA.Project.ImportExport
{
    public partial class RiceviFascicolo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string idFascicolo = Request.Params.Get("idFascicolo");

            if (idFascicolo == null)
                idFascicolo = "0";

            DocsPaWR.InfoUtente ui = UserManager.GetInfoUser();
            
            
        }
    }
}