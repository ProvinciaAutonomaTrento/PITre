using NttDataWA.DocsPaWR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NttDataWA.ImportDati
{
    /// <summary>
    /// Summary description for DocumentsUploadHandler
    /// </summary>
    public class DocumentsUploadHandler : FileTransferHandler, IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");

            this._handleRequest(context);
        }

        protected override bool Upload(HttpPostedFile file, string fileName, InfoUtente infoUtente)
        {
            bool result;
            try
            {
                result = UIManager.ImportDocumentManager.UploadFileOnServer(file.InputStream, fileName, infoUtente);
            }
            catch (Exception) { result = false; }
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