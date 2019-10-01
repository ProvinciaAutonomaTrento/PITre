using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using Aspose.Words;

namespace BusinessLogic.Modelli.AsposeModelProcessor
{
    public class DocModelProcessor : BaseDocModelProcessor
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocModelProcessor));

        private License license;

        public DocModelProcessor()
        {
            license = new License();
            DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor csmp = new DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor();

            byte[] licenseContent = csmp.GetLicense("ASPOSE");
            if (licenseContent != null)
            {
                System.IO.MemoryStream licenseStream = new System.IO.MemoryStream(licenseContent, 0, licenseContent.Length);
                license.SetLicense(licenseStream);
                licenseStream.Close();
            }
        }

        protected override DocsPaVO.Modelli.ModelKeyValuePair[] GetModelKeyValuePairs(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            throw new NotImplementedException();
        }

        public override DocsPaVO.Modelli.ModelResponse ProcessModel(DocsPaVO.Modelli.ModelRequest request)
        {
            throw new NotImplementedException();
        }

        public byte[] ConvertInvoiceToPdf(byte[] invoice)
        {
            byte[] result = null;

            try
            {
                MemoryStream input = new MemoryStream(invoice);
                MemoryStream output = new MemoryStream();

                Document doc = new Document(input);
                logger.Debug(">> XML CARICATO IN DOC ASPOSE <<");

                doc.Save(output, Aspose.Words.SaveFormat.Pdf);
                logger.Debug(">> CONVERSIONE PDF OK <<");

                result = output.ToArray();
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in ConvertInvoiceToPdf -", ex);
                result = null;
            }

            return result;
        }

        public void ConvertToPdfAsync(byte[] content, DocsPaVO.documento.FileRequest fileReq, DocsPaVO.utente.InfoUtente infoUtente)
        {
            AsyncCallback callback = new AsyncCallback(CallBack);
            ConvertToPdfDelegate conv = new ConvertToPdfDelegate(ConvertToPdf);
            conv.BeginInvoke(content, fileReq, infoUtente, callback, conv);
        }

        private delegate void ConvertToPdfDelegate(byte[] content, DocsPaVO.documento.FileRequest fileReq, DocsPaVO.utente.InfoUtente infoUtente);

        private static void CallBack(IAsyncResult result)
        {
            var del = result.AsyncState as ConvertToPdfDelegate;
            if (del != null)
            {
                del.EndInvoke(result);
            }
        }

        private void ConvertToPdf(byte[] content, DocsPaVO.documento.FileRequest fileReq, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("BEGIN");

            MemoryStream ms = new MemoryStream(content);
            Document doc = new Document(ms);

            using (MemoryStream pdfStream = new MemoryStream())
            {
                doc.Save(pdfStream, SaveFormat.Pdf);

                if (pdfStream != null && pdfStream.Length > 0)
                {
                    using (DocsPaDB.TransactionContext transaction = new DocsPaDB.TransactionContext())
                    {
                        try
                        {
                            DocsPaVO.documento.FileRequest convertedFileReq = new DocsPaVO.documento.FileRequest();
                            
                            convertedFileReq.cartaceo = false;
                            convertedFileReq.daAggiornareFirmatari = false;
                            convertedFileReq.descrizione = "Documento converito in PDF lato server";
                            convertedFileReq.docNumber = fileReq.docNumber;

                            convertedFileReq = BusinessLogic.Documenti.VersioniManager.addVersion(convertedFileReq, infoUtente, false);

                            DocsPaVO.documento.FileDocumento convertedFileDoc = new DocsPaVO.documento.FileDocumento();
                            convertedFileDoc.cartaceo = false;
                            convertedFileDoc.content = pdfStream.ToArray();
                            convertedFileDoc.contentType = "application/pdf";
                            convertedFileDoc.estensioneFile = "PDF";
                            convertedFileDoc.fullName = fileReq.fileName + ".pdf";
                            convertedFileDoc.length = convertedFileDoc.content.Length;
                            convertedFileDoc.name = fileReq.fileName + ".pdf";
                            convertedFileDoc.nomeOriginale = fileReq.fileName + ".pdf";
                            convertedFileDoc.path = "";

                            DocsPaVO.documento.FileRequest result = Documenti.FileManager.putFile(convertedFileReq, convertedFileDoc, infoUtente);

                            if (result == null)
                            {
                                throw new Exception();
                            }

                            transaction.Complete();

                        }
                        catch (Exception ex)
                        {
                            logger.Debug("Errore nella conversione PDF Aspose - ", ex);
                            
                        }
                    }

                }
                
            }

            logger.Debug("END");

        }


    }
}
