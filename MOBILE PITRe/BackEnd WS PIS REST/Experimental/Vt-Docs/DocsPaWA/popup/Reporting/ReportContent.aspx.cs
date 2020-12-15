using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.SiteNavigation;

namespace DocsPAWA.popup.Reporting
{
    public partial class ReportContent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FileDocumento doc = CallContextStack.CurrentContext.ContextState["documentFile"] as FileDocumento;
            if (doc != null)
            {
                CallContextStack.CurrentContext.ContextState["documentFile"] = null;
                Response.Clear();
                Response.ContentType = doc.contentType;
                Response.AddHeader("content-disposition", "attachment;filename=" + doc.name);
                Response.BinaryWrite(doc.content);
                Response.Flush();
                Response.End();
            }
        }
    }
}
