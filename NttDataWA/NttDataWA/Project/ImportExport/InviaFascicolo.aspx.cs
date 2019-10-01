using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Diagnostics;
using System.Net;

using Ionic.Zip;
using Ionic.Zlib;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;


namespace NttDataWA.Project.ImportExport
{
    public partial class InviaFascicolo : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpPostedFile httpPF = Request.Files[0];


            string idFascicolo = Request.Params.Get("idProject");

            InfoUtente ui = UserManager.GetInfoUser();
            
            //ProjectFS.ImportFSToProject(ui, idFascicolo, httpPF.InputStream);
        }
    }
}