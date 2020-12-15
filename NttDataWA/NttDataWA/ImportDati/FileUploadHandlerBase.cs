using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace NttDataWA.ImportDati
{
    public abstract class FileUploadHandlerBase
    {
        private readonly JavaScriptSerializer _js = new JavaScriptSerializer();

        protected void _handleRequest(HttpContext context)
        {
            switch (context.Request.HttpMethod)
            {

                case "POST":
                case "PUT":
                    UploadFile(context);
                    break;

                default:
                    context.Response.ClearHeaders();
                    context.Response.StatusCode = 405;
                    break;
            }
        }

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
            var fullName = Path.GetFileName(fileName);

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
                if (userPostedFile == null) { continue; }
                if (String.IsNullOrWhiteSpace(userPostedFile.FileName)) { continue; }


                string error = this.Upload(userPostedFile, _tempFileName);

                FilesStatus _fs = new FilesStatus(_tempFileName, userPostedFile.ContentLength);
                _fs.error = error;
                statuses.Add(_fs);
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

            var jsonObj = _js.Serialize(statuses.ToArray());
            context.Response.Write(jsonObj);
        }

        protected abstract string Upload(HttpPostedFile file, string fileName);
    }
}