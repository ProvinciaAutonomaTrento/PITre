using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interoperability.Controller;
using Interoperability.Domain;
using DocsPaVO.documento;
using BusinessLogic.Documenti;
using DocsPaVO.utente;
using System.Web;
using System.IO;
using log4net;

namespace BusinessLogic.interoperabilita.Semplificata
{
    /// <summary>
    /// Questa classe fornisce una serie di metodi di utilità per la gestione dei file 
    /// associati ai documenti spediti per IS
    /// </summary>
    public class SimplifiedInteroperabilityFileManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(SimplifiedInteroperabilityFileManager));
        /// <summary>
        /// Metodo per il download di un file associato ad un documento
        /// </summary>
        /// <param name="documentInfo">Informazioni sul documento</param>
        /// <param name="senderAdministrationId">Id dell'amministrazione</param>
        /// <param name="fileRequest">Informazioni sul file richiesto</param>
        /// <param name="userInfo">Informazioni sull'utente con cui recuperare il file</param>
        /// <param name="senderFileManagerUrl">Url del servizio per la gestione dei file</param>
        public static void DownloadFile(DocumentInfo documentInfo, String senderAdministrationId, FileRequest fileRequest, InfoUtente userInfo, String senderFileManagerUrl, out string errPutFile)
        {
            logger.Debug("BEGIN");
            // Reperimento del file
            InteroperabilityController interoperabilityController = new InteroperabilityController();
            Interoperability.Service.Library.FileServiceReference.RemoteFileInfo fileResponse = interoperabilityController.DownloadFile(
                new Interoperability.Service.Library.FileServiceReference.SendFileRequest(
                    senderAdministrationId,
                    documentInfo.DocumentNumber,
                    documentInfo.DocumentServerLocation,
                    documentInfo.FileName,
                    documentInfo.FilePath,
                    documentInfo.Version,
                    documentInfo.VersionId,
                    documentInfo.VersionLabel),
                    senderFileManagerUrl);

            // Caricamento del file in DocsPa
            byte[] documentContent = ReadFileContent(fileResponse.FileData, (int)fileResponse.FileTransferInfo.FileLength);

            FileDocumento fileDocument = new FileDocumento()
            {
                content = documentContent,
                length = (int)fileResponse.FileTransferInfo.FileLength,
                name = fileResponse.FileTransferInfo.FileName
            };

            String err = String.Empty;

            //Gestione TSDis (DOC+TSR)
            //Siccome non si possono mandare piu file allo stesso tempo, ne è possibile alterare la struttura
            //si è pensato in fase di invio, in caso di documenti con TSR associato, l'invio dello stesso un formato
            //TSD, come marca verrà presa l'ultima disponibile, e unita al documento, creadno un file con estensione TSDis
            //In fase di ricezione il TsdIs, sarà poi spacchettato, il TSR messo in DPA_TIMESTAMP dopo verifica hash e 
            //infine il payload messo in documentale con la putfile.


            DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = null;
            if (Path.GetExtension(fileDocument.name) == ".TSDis")
            {
                try
                {
                    Documenti.DigitalSignature.PKCS_Utils.tsd tsdMgr = new Documenti.DigitalSignature.PKCS_Utils.tsd();
                    tsdMgr.explode(fileDocument.content);
                    fileDocument.content = tsdMgr.Data.Content;
                    fileDocument.length = (int)fileDocument.content.Length;
                    fileDocument.name = fileDocument.name.Replace(".TSDis", string.Empty);
                    Documenti.DigitalSignature.VerifyTimeStamp vts = new Documenti.DigitalSignature.VerifyTimeStamp();
                    byte[] tsrFile = tsdMgr.TSR.FirstOrDefault().Content;
                    if (vts.machTSR(tsrFile, fileDocument.content))
                        resultMarca = vts.Verify(fileDocument.content, tsrFile);
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Errore gestendo il TSDis {0} {1}", e.Message, e.StackTrace);
                    //manca il loggher pdpdpdpdp
                }
            }

            FileManager.putFile(ref fileRequest, fileDocument, userInfo, out errPutFile, true);
            if (resultMarca != null && resultMarca.esito == "OK")
            {
                DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();
                timestampDoc.saveTSR(userInfo, resultMarca, fileRequest);
            }

            logger.Debug("END");
        }

        /// <summary>
        /// Metodo per la lettura del contenuto del documento da scaricare
        /// </summary>
        /// <param name="contentStream">Stream per lo scorrimento del contenuto del documento</param>
        /// <param name="fileLength">Dimensione del file</param>
        /// <returns>Contenuto del documento</returns>
        private static byte[] ReadFileContent(Stream contentStream, long fileLength)
        {

            byte[] retVal = new byte[fileLength];
            int offset = 0;
            int readed = 0;
            int count = (int)fileLength;
            while ((readed = contentStream.Read(retVal, offset, count)) > 0)
            {
                offset += readed;
                count -= readed;
            }
            return retVal;

        }
    }
}
