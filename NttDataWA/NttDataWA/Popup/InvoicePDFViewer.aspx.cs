using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class InvoicePDFViewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            NttDataWA.DocsPaWR.FileDocumento fileDoc = null;

            fileDoc = (NttDataWA.DocsPaWR.FileDocumento)FileManager.getSelectedFileReport(this);

            if (fileDoc != null)
            {
                Response.ContentType = fileDoc.contentType;

                Response.AddHeader("Content-Disposition", "filename=" + fileDoc.fullName );
                Response.BinaryWrite(fileDoc.content);
                Response.Flush();
                Response.End();
            }
        }
    }
}