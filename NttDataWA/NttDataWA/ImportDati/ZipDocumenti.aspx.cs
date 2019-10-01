using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;


namespace NttDataWA.ImportDati
{
    public partial class ZipDocumenti : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Si prova a prelevare dal call context il report
            ResultsContainer report = HttpContext.Current.Session["report"] as ResultsContainer;
            byte[] temp = null;
            try
            {
                temp = ImportDocumentManager.CreateZipFromReport(report, UserManager.GetInfoUser());

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
            
        }
    }
}