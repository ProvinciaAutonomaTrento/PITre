using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using NttDataWA.Utils;


namespace NttDataWA.SmartClient
{
    public partial class UploadEngine : System.Web.UI.Page
    {

        #region Properties

        protected int FileAcquisitionSizeMax
        {
            get
            {
                int result = 20480 * 1024;
                if (HttpContext.Current.Session["FileAcquisitionSizeMax"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["FileAcquisitionSizeMax"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["FileAcquisitionSizeMax"] = value;
            }
        }

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (this.IsPostBack)
                {
                    UploadDetail Upload = Session["UploadDetail"] as UploadDetail;
                    //NttDataWA.DocsPaWR.FileDocumento fileDoc = Session["fileDoc"] as NttDataWA.DocsPaWR.FileDocumento;
                    NttDataWA.DocsPaWR.FileDocumento fileDoc = new NttDataWA.DocsPaWR.FileDocumento();
                    if (Upload != null && fileDoc != null)
                    {
                        Upload.IsReady = false;

                        //using (StreamWriter log = File.AppendText(HttpContext.Current.Server.MapPath("../UserFiles/debug.log")))
                        //{
                        //    log.WriteLine(DateTime.Now.ToString() + ": PostedFile is null=" + (this.fileUpload.PostedFile==null).ToString());
                        //}

                        if (this.fileUpload.PostedFile != null && this.fileUpload.PostedFile.ContentLength > 0 && this.fileUpload.PostedFile.ContentLength <= this.FileAcquisitionSizeMax)
                        {
                            string path = this.Server.MapPath(@"Uploads");
                            string fileName = Path.GetFileName(this.fileUpload.PostedFile.FileName);
                            string exten = Path.GetExtension(Path.Combine(path, fileName));

                            Upload.ContentLength = this.fileUpload.PostedFile.ContentLength;
                            Upload.FileName = fileName;
                            Upload.UploadedLength = 0;

                            Upload.IsReady = true;

                            //using (StreamWriter log = File.AppendText(HttpContext.Current.Server.MapPath("../UserFiles/debug.log")))
                            //{
                            //    log.WriteLine(DateTime.Now.ToString() + ": Upload.FileName=" + Upload.FileName + ", Upload.ContentLength=" + Upload.ContentLength);
                            //}

                            fileDoc.name = fileName;// System.IO.Path.GetFileName(file.FileName);
                            fileDoc.fullName = Path.Combine(path, fileName);
                            fileDoc.contentType = NttDataWA.UIManager.FileManager.GetMimeType(fileName);
                            fileDoc.length = this.fileUpload.PostedFile.ContentLength;// ContentLength;// .FileSize;
                            fileDoc.content = new Byte[fileDoc.length];

                            int bufferSize = 1;
                            byte[] buffer = new byte[bufferSize];

                            while (Upload.UploadedLength < Upload.ContentLength)
                            {
                                //Fill the buffer from the input stream
                                int bytes = this.fileUpload.PostedFile.InputStream.Read(buffer, 0, bufferSize);
                                fileDoc.content.SetValue(buffer[0], Upload.UploadedLength);

                                Upload.UploadedLength += bytes;
                                Session["UploadDetail"] = Upload;
                            }

                            Upload.UploadedLength = fileDoc.length;
                            Session["UploadDetail"] = Upload;
                            Session["fileDoc"] = fileDoc;

                            const string js = "window.parent.endProgress('{0}','{1}', '{2}');";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "progress", string.Format(js, fileName.Replace("'", " "), Upload.UploadedLength, Upload.ContentLength), true);
                        }
                        else
                        {
                            string msg = "ErrorFileUpload";
                            if (this.fileUpload.PostedFile.ContentLength>this.FileAcquisitionSizeMax)
                                msg = "ErrorFileUploadMaxSizeExceeded";

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.parent.fra_main) {parent.parent.fra_main.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'error', '');} else {parent.parent.ajaxDialogModal('" + utils.FormatJs(msg) + "', 'error', '');}; parent.reallowOp();", true);
                        }

                        Upload.IsReady = false;
                    }
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