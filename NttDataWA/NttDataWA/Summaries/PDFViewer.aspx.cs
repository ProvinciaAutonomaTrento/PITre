using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;

namespace NttDataWA.Summaries
{
    public partial class PDFViewer : System.Web.UI.Page
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            Response.Expires = -1;
            NttDataWA.DocsPaWR.FileDocumento theDoc = null;

            theDoc = (NttDataWA.DocsPaWR.FileDocumento)FileManager.getSelectedFileReport(this);

            if (theDoc != null)
            {
                Response.ContentType = theDoc.contentType;
              
                Response.BinaryWrite(theDoc.content);
                Response.Flush();
            }
        }


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
    }
}