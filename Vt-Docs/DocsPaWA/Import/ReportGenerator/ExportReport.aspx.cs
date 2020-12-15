using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.SiteNavigation;
using System.Data;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.Import.ReportGenerator
{
    public partial class ExportReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Si prova a prelevare dal call context il report
            FileDocumento report = CallContextStack.CurrentContext.ContextState["reportImport"] as FileDocumento;

            // Se il file documento è stato recuperato con successo, si procede alla visualizzazione
            if (report != null && report.length > 0)
            {
                Response.ContentType = report.contentType;
                Response.AddHeader("content-disposition", "attachment; filename=" + report.name);
                Response.AddHeader("content-length", report.content.Length.ToString());
                Response.BinaryWrite(report.content);
                Response.Flush();
            }

        }
    }
}
