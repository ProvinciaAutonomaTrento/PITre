using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class ReportContent : System.Web.UI.Page
    {

        #region Property

        /// <summary>
        ///  
        /// </summary>
        private FileDocumento FileDocPrintReport
        {
            get
            {
                return HttpContext.Current.Session["fileDocPrintReport"] as FileDocumento;
            }
            set
            {
                HttpContext.Current.Session["fileDocPrintReport"] = value;
            }
        }


        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (FileDocPrintReport != null)
            {
                Response.Clear();
                Response.ContentType = FileDocPrintReport.contentType;
                Response.AddHeader("content-disposition", "attachment;filename=" + FileDocPrintReport.name);
                Response.BinaryWrite(FileDocPrintReport.content);
                Response.Flush();
                Response.End();
            }
        }
    }
}