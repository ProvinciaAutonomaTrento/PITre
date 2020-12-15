using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace NttDataWA.Popup
{
    /// <summary>
    /// Summary description for UploadAttachmentsHandler
    /// </summary>
    public class UploadAttachmentsHandler : NttDataWA.ImportDati.FileUploadHandlerBase, IHttpHandler, IRequiresSessionState
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
                SchedaDocumento documentTab = DocumentManager.getSelectedRecord();
                List<DocsPaWR.Allegato> attachments = new List<DocsPaWR.Allegato>(documentTab.allegati);
                DocumentManager.setSelectedAttachId(null);
                Allegato attachment = new DocsPaWR.Allegato()
                {
                    descrizione = Path.GetFileNameWithoutExtension(file.FileName), //file.FileName,
                    numeroPagine = 1,
                    docNumber = documentTab.docNumber,
                    dataInserimento = DateTime.Today.ToString(),
                    version = "1",
                    TypeAttachment = 1,
                    idPeopleDelegato = UserManager.GetInfoUser().delegato != null ? UserManager.GetInfoUser().delegato.idPeople : "0",
                    repositoryContext = documentTab.repositoryContext,
                    position = (documentTab.allegati.Length + 1)
                };

                attachment = DocumentManager.AddAttachment(attachment);
                attachment.versionLabel = string.Format("A{0:0#}", DocumentManager.getSelectedRecord().allegati.Count() + 1);

                attachments.Add(attachment);
                documentTab.allegati = attachments.ToArray();
                DocumentManager.setSelectedRecord(documentTab);

                System.Web.HttpContext.Current.Session["selectedAttachmentId"] = attachment.versionId;

                // aggiungi file all'allegato
                //FileRequest attachFileReq = FileManager.GetFileRequest(attachment.versionId);


                // File DOcumento...
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
                fileDoc.content = _ms.ToArray();

                bool _pdf = "1".Equals((string)System.Web.HttpContext.Current.Session["UploadMassivoConversionePDF"]);
                bool _paper = "1".Equals((string)System.Web.HttpContext.Current.Session["UploadMassivoCartaceo"]);
                bool _pdfConversionSynchronousLC = false;
                if (HttpContext.Current.Session["PdfConversionSynchronousLC"] != null)
                {
                    _pdfConversionSynchronousLC = (bool)HttpContext.Current.Session["PdfConversionSynchronousLC"];
                }

                result = FileManager.uploadFile(null, fileDoc, _paper, _pdf, _pdfConversionSynchronousLC);

                if (!String.IsNullOrWhiteSpace(result))
                {
                    // rimuove l'allegato ocreato
                    attachments.Remove(attachment);
                    bool resultRemove = DocumentManager.RemoveAttachment(1,1,attachment);
                    DocumentManager.RemoveSelectedAttachId();
                    if (resultRemove)
                    {
                        documentTab.allegati = attachments.ToArray();
                        DocumentManager.setSelectedRecord(documentTab);
                    }
                }


                //if (String.IsNullOrWhiteSpace(documentTab.docNumber))
                //{
                //    var filereq = FileManager.uploadFile(null, fileDoc, false);
                //}
                //else
                //{
                //    error = FileManager.uploadFile(null, fileDoc, _paper, _pdf, _pdfConversionSynchronousLC);
                //}
                


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