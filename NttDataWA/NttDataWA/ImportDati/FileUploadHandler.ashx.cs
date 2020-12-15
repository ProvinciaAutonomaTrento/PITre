using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace NttDataWA.ImportDati
{
    /// <summary>
    /// Summary description for FileUploadHandler
    /// </summary>
    public class FileUploadHandler :  IHttpHandler, IRequiresSessionState
    {
        private readonly JavaScriptSerializer js = new JavaScriptSerializer();

        public string StorageRoot
        {
            get { return "C:\\_ROOT\tmp\\"; }
        }
        public bool IsReusable { get { return false; } }


        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");

            this._handleRequest(context);
        }

        private void _handleRequest(HttpContext context)
        {
            switch (context.Request.HttpMethod)
            {
                case "HEAD":
                case "GET":
                    //if (GivenFilename(context)) DeliverFile(context);
                    //else ListCurrentFiles(context);
                    //break;

                case "POST":
                case "PUT":
                    UploadFile(context);
                    break;

                case "DELETE":
                    //DeleteFile(context);
                    break;

                case "OPTIONS":
                    //ReturnOptions(context);
                    break;

                default:
                    context.Response.ClearHeaders();
                    context.Response.StatusCode = 405;
                    break;
            }
        }

        // Upload file to the server
        private void UploadFile(HttpContext context)
        {
            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], context, statuses);
            }

            WriteJsonIframeSafe(context, statuses);
        }

        // Upload partial file
        private void UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses)
        {
            if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var inputStream = context.Request.Files[0].InputStream;
            var fullName = StorageRoot + Path.GetFileName(fileName);

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            statuses.Add(new FilesStatus(new FileInfo(fullName)));
        }

        // Upload entire file
        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            DocsPaWR.InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();

            //if (context.Session["InfoUserForUploadDocument"] != null)
            //infoUtente = context.Session["InfoUserForUploadDocument"] as DocsPaWR.InfoUtente;
            string _tempFileName;

            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                var userPostedFile = context.Request.Files[i];


                if (userPostedFile.FileName.IndexOf("\\") >= 0 || userPostedFile.FileName.IndexOf("/") >= 0)
                {
                    _tempFileName = Path.GetFileName(userPostedFile.FileName);
                }
                else
                {
                    _tempFileName = userPostedFile.FileName;
                }
                if(userPostedFile == null) { continue; }
                if (String.IsNullOrWhiteSpace(userPostedFile.FileName) || userPostedFile.FileName.Equals("formatPdfExport.xml")) { continue; }
                bool result = UIManager.ImportDocumentManager.UploadFileOnServer(userPostedFile.InputStream, _tempFileName, infoUtente);
                //if (!result)
                //{
                //    throw new Exception("Errore nell'upload del file");
                //}


                //if(userPostedFile.FileName.IndexOf("\\") >= 0 || userPostedFile.FileName.IndexOf("/") >= 0)
                //{
                //    userPostedFile.SaveAs(StorageRoot + _tempFileName);
                //}
                

                //string fullName = Path.GetFileName(userPostedFile.FileName);
                statuses.Add(new FilesStatus(_tempFileName, userPostedFile.ContentLength));
            }
        }

        private void WriteJsonIframeSafe(HttpContext context, List<FilesStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try
            {
                if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                    context.Response.ContentType = "application/json";
                else
                    context.Response.ContentType = "text/plain";
            }
            catch
            {
                context.Response.ContentType = "text/plain";
            }

            var jsonObj = js.Serialize(statuses.ToArray());
            context.Response.Write(jsonObj);
        }

    }


    
}