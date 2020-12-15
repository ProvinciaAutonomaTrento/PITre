using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.SiteNavigation;
using DocsPAWA.DocsPaWR;
using DocsPAWA.utils;

namespace DocsPAWA.Import.Documenti
{
    public partial class ZipDocumenti : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Si prova a prelevare dal call context il report
            ResultsContainer report = CallContextStack.CurrentContext.ContextState["report"] as ResultsContainer;
            byte[] temp = null;
            try
            {
                temp = ImportDocumentsUtils.CreateZipFromReport(report, UserManager.getInfoUtente());

            }
            catch (Exception) { }
            Response.ContentType = "zip";
            Response.AddHeader("content-disposition", "attachment; filename=documenti.zip");
            if (temp != null)
            {
                Response.AddHeader("content-length", "" + temp.Length);
                Response.BinaryWrite(temp);
            }
            else
            {
                Response.AddHeader("content-length", "0");
            }
            Response.Flush();
        }
    }
}