using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace DocsPAWA.ImportMassivoDoc
{
    public partial class visPdfReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            string templateFilePath = Server.MapPath("formatPdfExport.xml");
            DocsPAWA.DocsPaWR.FileDocumento fileRep = new DocsPAWA.DocsPaWR.FileDocumento();
            if ((Session["dsData"].ToString() != null) && (Session["dsData"].ToString() != ""))
            {
                fileRep = global::ProspettiRiepilogativi.Frontend.PdfReport.do_MakePdfReport(global::ProspettiRiepilogativi.Frontend.ReportDisponibili.ReportLogMassiveImport,
                    templateFilePath,
                    (DataSet)Session["dsData"], null);

                if (fileRep.length > 0)
                {
                    Response.ContentType = fileRep.contentType;
                    Response.AddHeader("content-disposition", "inline; filename=" + fileRep.name);
                    Response.AddHeader("content-length", fileRep.content.Length.ToString());
                    Response.BinaryWrite(fileRep.content);
                    Response.Flush();
                }

            }
        }
    }
}
