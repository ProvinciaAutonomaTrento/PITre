using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Xml;

namespace DocsPAWA
{
    /// <summary>
    /// Summary description for upload.
    /// </summary>
    public class upload : System.Web.UI.Page
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                bool cartaceo;
                bool.TryParse(Request.QueryString["cartaceo"], out cartaceo);
                bool convertiPDFServer;
                bool.TryParse(Request.QueryString["convertiPDFServer"], out convertiPDFServer);

                bool convertiPDF;
                bool.TryParse(Request.QueryString["convertiPDF"], out convertiPDF);

                bool convertiPDFSincrono;
                bool.TryParse(Request.QueryString["convertiPDFServerSincrono"], out convertiPDFSincrono);

                XmlDocument xmlDom = new XmlDocument();
                xmlDom.Load(Request.InputStream);

                this.UploadFile(xmlDom, "CurrentVersion", false, cartaceo, convertiPDF, convertiPDFServer, convertiPDFSincrono);

                // Se la conversione richiesta è la asincrona...
                if (convertiPDF && convertiPDFServer && !convertiPDFSincrono)
                    // ...bisogna aggiornare lo stato di checkin checkout del documento
                    CheckInOut.CheckInOutServices.RefreshCheckOutStatus();

                this.UploadFile(xmlDom, "NewVersion", true, cartaceo, convertiPDF, convertiPDFServer, convertiPDFSincrono);
            }
            catch (Exception ex)
            {
                ErrorManager.setError(this, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDom"></param>
        /// <param name="nodeName"></param>
        /// <param name="addVersion"></param>
        /// <param name="cartaceo"></param>
        private void UploadFile(XmlDocument xmlDom, string nodeName, bool addVersion, bool cartaceo, bool convertiPDF, bool convertiPDFServer, bool conversionePDFServerSincrona)
        {
            XmlNode file = xmlDom.SelectSingleNode("root/" + nodeName);
            if (file != null)
            {
                string fileBuf = file.FirstChild.Value;

                DocsPaWR.FileDocumento fileDoc = new DocsPAWA.DocsPaWR.FileDocumento();
                fileDoc.name = file.Attributes["FileName"].Value;
                fileDoc.fullName = fileDoc.name;
                fileDoc.content = Convert.FromBase64String(fileBuf);
                fileDoc.length = fileDoc.content.Length;
                fileDoc.cartaceo = cartaceo;
                //if(Convert.ToBoolean(DocsPAWA.Utils.isEnableConversionePdfLatoServer()) || Boolean.Parse(Utils.IsEbabledConversionePdfLatoServerSincrona()))
                // Se è richiesta conversione pdf centralizzata
                if (convertiPDF && convertiPDFServer)
                    FileManager.getInstance(Session.SessionID).uploadFile(this, fileDoc, addVersion, convertiPDFServer, conversionePDFServerSincrona);
                else
                    // ...altrimenti si procede con upload del file senza coversione
                    FileManager.getInstance(Session.SessionID).uploadFile(this, fileDoc, addVersion);
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion

    }
}
