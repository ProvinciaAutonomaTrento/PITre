using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Gestione_Logs
{
    public partial class ReportFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Estrazione"] != null)
            {
                FileDocumento fileRep = Session["Estrazione"] as FileDocumento;

                Session["Estrazione"] = null;

                Response.ContentType = fileRep.contentType;
                Response.AddHeader("content-disposition", "attachment; filename=" + fileRep.name);

                Response.AddHeader("content-length", fileRep.content.Length.ToString());
                Response.BinaryWrite(fileRep.content);
                Response.Flush();
            }
        }
    }
}