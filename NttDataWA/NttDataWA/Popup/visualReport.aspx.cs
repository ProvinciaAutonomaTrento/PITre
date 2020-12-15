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
    public partial class visualReport : System.Web.UI.Page
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
                if (!string.IsNullOrEmpty(Request.QueryString["IsSocket"]))
                    return true;
                else
                    return false;
            }
        }

        private bool AlreadyDownloaded
        {
            get
            {
                if (HttpContext.Current.Session["visualReportAlreadyDownloaded" + Session.SessionID]!=null)
                    return (bool)HttpContext.Current.Session["visualReportAlreadyDownloaded" + Session.SessionID];
                else
                    return false;
            }
            set
            {
                HttpContext.Current.Session["visualReportAlreadyDownloaded" + Session.SessionID] = value;
            }
        }


        public void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Response.Expires = -1;

                if (!IsPostBack && !this.AlreadyDownloaded)
                {
                    //    string id = Session.SessionID;
                    //    DocsPaWR.FileDocumento theDoc = FileManager.getInstance(id).getReport(this);

                    //    Response.ContentType = theDoc.contentType;
                    //    Response.ContentType = "application/octet-stream";
                    //    Response.AddHeader("content-disposition", "attachment;filename=" + theDoc.name);
                    //    Response.AddHeader("content-lenght", theDoc.content.Length.ToString());
                    //    Response.BinaryWrite(theDoc.content);





                    DocsPaWR.FileDocumento file = new DocsPaWR.FileDocumento();

                    exportDatiSessionManager sessioneManager = new exportDatiSessionManager();
                    file = sessioneManager.GetSessionExportFile();
                    sessioneManager.ReleaseSessionExportFile();

                    if (file != null && file.content.Length > 0)
                    {
                        Response.ContentType = file.contentType;
                        Response.AddHeader("content-disposition", "inline;filename=" + file.fullName);
                        Response.AddHeader("content-lenght", file.content.Length.ToString());
                        if (this.IsApplet || this.IsSocket)
                        {
                            string base64String = System.Convert.ToBase64String(file.content, 0, file.content.Length);
                            Response.Write(base64String);
                        }
                        else
                        {
                            Response.BinaryWrite(file.content);
                            Response.Flush();
                            //Response.Close();
                            //Response.End();
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "parent.ajaxDialogModal('ErrorExportDati', 'warning', '');", true);
                    }



                    this.AlreadyDownloaded = true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

   

    }
}