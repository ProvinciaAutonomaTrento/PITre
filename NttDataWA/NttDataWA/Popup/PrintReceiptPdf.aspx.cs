using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class PrintReceiptPdf : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                this.Response.Expires = -1;
                this.PerformActionStampaRicevutaPdf();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected virtual void PerformActionStampaRicevutaPdf()
        {
            DocsPaWR.FileDocumento content = DocumentManager.StampaRicevutaProtocolloPdf();

            if (content != null)
            {

                System.IO.MemoryStream memStream = new System.IO.MemoryStream(content.content);
                Response.AppendHeader("content-disposition", "inline;filename=Ricevuta.pdf");
                Response.AppendHeader("Content-Length", memStream.Length.ToString());
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(memStream.ToArray());
                //Response.Flush();
                memStream.Close();
               // Response.End();
            }
        }

    }
}