using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;
using NttDataWA.ImportDati;

namespace NttDataWA.Handler
{
    /// <summary>
    /// Summary description for UploadFileFormazioneHandler
    /// </summary>
    public class UploadTemplateFormazioneHandler : FileTransferHandler, IHttpHandler
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
                if(idUO == null)
                {
                    throw new ArgumentNullException("idUO");
                }
                result = UIManager.ImportDocumentManager.UploadTemplateFormazione(file.InputStream, idUO, infoUtente);
            }
            catch(Exception)
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