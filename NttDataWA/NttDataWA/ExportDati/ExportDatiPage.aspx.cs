using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;


namespace NttDataWA.ExportDati
{
    public partial class ExportDatiPage : System.Web.UI.Page
    {

        private bool IsApplet
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.QueryString["isapplet"]))
                    return true;
                else
                    return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DocsPaWR.FileDocumento file = new DocsPaWR.FileDocumento();

                exportDatiSessionManager sessioneManager = new exportDatiSessionManager();
                file = sessioneManager.GetSessionExportFile();
                sessioneManager.ReleaseSessionExportFile();

                if (file != null && file.content.Length > 0)
                {
                    Response.ContentType = file.contentType;
                    Response.AddHeader("content-disposition", "inline;filename=" + file.fullName);
                    Response.AddHeader("content-lenght", file.content.Length.ToString());
                    if (this.IsApplet)
                    {
                        string base64String = System.Convert.ToBase64String(file.content, 0, file.content.Length);
                        Response.Write(base64String);
                    }
                    else
                    {
                        Response.BinaryWrite(file.content);
                        Response.Flush();
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('ErrorExportDati', 'warning', '');", true);
                }
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('ErrorExportDati', 'warning', '');", true);
            }
        }

    }
}