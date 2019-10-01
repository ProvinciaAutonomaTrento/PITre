using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class ImportPeviousReport_exportDatiPage : System.Web.UI.Page
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

        private bool IsSocket
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.QueryString["issocket"]))
                    return true;
                else
                    return false;
            }
        }

        public DocsPaWR.FileDocumento FileDocumento
        {
            get
            {
                return HttpContext.Current.Session["fileDocumento"] as DocsPaWR.FileDocumento;
            }
            set
            {
                HttpContext.Current.Session["fileDocumento"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (this.FileDocumento != null && this.FileDocumento.content.Length > 0)
                {
                    Response.ContentType = this.FileDocumento.contentType;
                    Response.AddHeader("content-disposition", "inline;filename=export.xls");
                    Response.AddHeader("content-lenght", this.FileDocumento.content.Length.ToString());
                    if (this.IsApplet || IsSocket)
                    {
                        string base64String = System.Convert.ToBase64String(this.FileDocumento.content, 0, this.FileDocumento.content.Length);
                        Response.Write(base64String);
                    }
                    else
                    {
                        Response.BinaryWrite(this.FileDocumento.content);
                        Response.Flush();
                    }
                    this.FileDocumento = null;
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