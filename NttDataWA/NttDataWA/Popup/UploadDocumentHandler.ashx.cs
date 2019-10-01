using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    /// <summary>
    /// Summary description for UploadDocumentHandler
    /// </summary>
    public class UploadDocumentHandler : NttDataWA.ImportDati.FileUploadHandlerBase, IHttpHandler, IRequiresSessionState
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        HttpContext _context;
        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");
            this._context = context;
            this._handleRequest(context);
        }

        protected override string Upload(HttpPostedFile file, string fileName)
        {
            string result = String.Empty;
            MemoryStream _ms = null;
            try
            {
                string path = this._context.Server.MapPath(@"Uploads");
                NttDataWA.DocsPaWR.FileDocumento fileDoc = new NttDataWA.DocsPaWR.FileDocumento();
                fileDoc.name = fileName;
                fileDoc.fullName = Path.Combine(path, fileName);
                fileDoc.contentType = NttDataWA.UIManager.FileManager.GetMimeType(fileName);
                fileDoc.length = file.ContentLength;

                byte[] buffer = new byte[16 * 1024];
                _ms = new MemoryStream();
                
                int read;
                while ((read = file.InputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    _ms.Write(buffer, 0, read);
                }
                fileDoc.content =  _ms.ToArray();

                System.Web.HttpContext.Current.Session["fileDoc"] = fileDoc;

                SchedaDocumento doc = DocumentManager.getSelectedRecord();
                if(doc!= null && String.IsNullOrWhiteSpace(doc.docNumber))
                {
                    UploadDetail Upload = new UploadDetail { IsReady = false };
                    HttpContext.Current.Session["UploadDetail"] = Upload;
                    Upload.FileName = fileName;

                    Upload.IsReady = true;

                    Upload.UploadedLength = fileDoc.length;
                    System.Web.HttpContext.Current.Session["UploadDetail"] = Upload;
                }

                bool _pdf = "1".Equals((string)System.Web.HttpContext.Current.Session["UploadMassivoConversionePDF"]);
                bool _paper = "1".Equals((string)System.Web.HttpContext.Current.Session["UploadMassivoCartaceo"]);
                bool _pdfConversionSynchronousLC = false;
                if (HttpContext.Current.Session["PdfConversionSynchronousLC"] != null)
                {
                    _pdfConversionSynchronousLC = (bool)HttpContext.Current.Session["PdfConversionSynchronousLC"];
                }

                result = FileManager.uploadFile(null, fileDoc, _paper, _pdf, _pdfConversionSynchronousLC);


                //SchedaDocumento _schedaDocumento = DocumentManager.getSelectedRecord();
                //FileManager.uploadFileFromSchedaDocumento(null, file, _schedaDocumento);
            }
            catch (Exception ex)
            {
                result = ex.Message;
                _logger.Error(ex.Message, ex);
            }
            return result;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}