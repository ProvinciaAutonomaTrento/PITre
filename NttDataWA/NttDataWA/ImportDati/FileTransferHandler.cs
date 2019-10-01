using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace NttDataWA.ImportDati
{
    public abstract class FileTransferHandler : IRequiresSessionState
    {
        private readonly JavaScriptSerializer js = new JavaScriptSerializer();

        public string StorageRoot
        {
            get { return "C:\\_ROOT\tmp\\"; }
        }

        protected void _handleRequest(HttpContext context)
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
        protected void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            DocsPaWR.InfoUtente infoUtente = UIManager.UserManager.GetInfoUser();

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


                bool result = this.Upload(userPostedFile, _tempFileName, infoUtente);

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


        protected abstract bool Upload(HttpPostedFile file, string fileName, DocsPaWR.InfoUtente infoUtente);

    }

    public class FilesStatus
    {
        public const string HandlerPath = "/";

        public string group { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string progress { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
        public string error { get; set; }

        public FilesStatus() { }

        public FilesStatus(FileInfo fileInfo) { SetValues(fileInfo.Name, (int)fileInfo.Length); }

        public FilesStatus(string fileName, int fileLength) { SetValues(fileName, fileLength); }

        private void SetValues(string fileName, int fileLength)
        {
            name = fileName;
            type = "image/png";
            size = fileLength;
            progress = "1.0";
            url = HandlerPath + "FileUploadHandler.ashx?f=" + fileName;
            thumbnail_url = HandlerPath + "Thumbnail.ashx?f=" + fileName;
            delete_url = HandlerPath + "FileUploadHandler.ashx?f=" + fileName;
            delete_type = "DELETE";
        }
    }
}