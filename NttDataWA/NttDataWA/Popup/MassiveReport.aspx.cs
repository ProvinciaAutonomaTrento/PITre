using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class MassiveReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    DocsPaWR.FileDocumento theDoc = Session["reportImport"] as DocsPaWR.FileDocumento;

                    if (theDoc != null)
                    {
                        Response.ContentType = theDoc.contentType;
                        Response.AddHeader("content-disposition", "inline;filename=" + theDoc.name);
                        Response.AddHeader("content-lenght", theDoc.content.Length.ToString());
                        Response.BinaryWrite(theDoc.content);
                        Response.Flush();
                    }
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }
    }
}