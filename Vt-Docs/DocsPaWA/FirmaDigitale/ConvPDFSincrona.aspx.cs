using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace DocsPAWA.FirmaDigitale
{
    public partial class ConvPDFSincrona : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DocsPAWA.DocsPaWR.FileDocumento result = null;
            Response.Expires = -1;

            FirmaDigitale.FirmaDigitaleMng firmaDigitaleMng = new FirmaDigitale.FirmaDigitaleMng();
            DocsPAWA.DocsPaWR.FileDocumento fileFirmato = firmaDigitaleMng.GetSignedDocument(this);
            firmaDigitaleMng = null;

            // Se ho un file fisico da poter convertire
            if (fileFirmato != null)
                // procedo con la conversione sincrona
                result = new DocsPAWA.DocsPaWR.DocsPaWebService().GeneratePDFInSyncMod(fileFirmato);

            if (result != null)
            {


                this.Response.ContentType = "application/pdf";
                this.Response.AddHeader("content-length", result.content.Length.ToString());
                this.Response.BinaryWrite(result.content);
            }
            else
            {
                this.Response.StatusCode = 500;
                this.Response.StatusDescription = "Non è stato possibile convertire il documento in PDF lato server";
            }
        }        
    }
}
