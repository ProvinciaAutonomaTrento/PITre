using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;

namespace NttDataWA.ImportDati
{
    public partial class PreviewInvoice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            NttDataWA.DocsPaWR.FileDocumento theDoc = null;

            theDoc = (NttDataWA.DocsPaWR.FileDocumento)Session["invoicePreview"];

            if (theDoc != null)
            {
                Response.ContentType = theDoc.contentType;

                Response.BinaryWrite(theDoc.content);
                Response.Flush();
                Response.End();
            }
        }
    }
}