using NttDataWA.DocsPaWR;
using NttDataWA.ImportDati;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NttDataWA.Handler
{
    /// <summary>
    /// Summary description for UploadAllegatiFormazione
    /// </summary>
    public class UploadAllegatiFormazione : FileTransferHandler, IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");

            this._handleRequest(context);
        }

        protected override bool Upload(HttpPostedFile file, string fileName, InfoUtente infoUtente)
        {
            bool result = true;

            try
            {
                string idUO = System.Web.HttpContext.Current.Session["Formazione_idUO"] as string;
                if (idUO == null)
                {
                    throw new ArgumentNullException("idUO");
                }
                result = UIManager.ImportDocumentManager.UploadAllegatiFormazione(file.InputStream, fileName, idUO, infoUtente);
            }
            catch (Exception)
            {
                result = false;
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