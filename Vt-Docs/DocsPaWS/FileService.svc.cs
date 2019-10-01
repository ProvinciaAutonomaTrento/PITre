using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DocsPaVO.FileManager;
using DocsPaVO.documento;
using System.IO;
using log4net;

namespace DocsPaWS
{
    /// <summary>
    /// Classe per l'esposizione dei servizi di download di file
    /// </summary>
    public class FileService : IFileManager
    {
        //instanzio il loggher
        private static ILog logger = LogManager.GetLogger(typeof(FileService));
        /// <summary>
        /// Metodo per il download di un file
        /// </summary>
        /// <param name="request">Informazioni sul file da scricare</param>
        /// <returns>Informazioni sul file scaricato e riferimento all'oggetto responsabile della gestione dello streaming dati</returns>
        RemoteFileInfo IFileManager.DownloadFile(SendFileRequest request)
        {
            logger.Debug("BEGIN");
            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente()
            {
                idAmministrazione = request.AdministrationId
            };
            FileRequest fr = new FileRequest()
            {
                docServerLoc = request.DocumentServerLocation,
                path = request.FilePath,
                fileName = request.FileName,
                docNumber = request.DocumentNumber,
                versionId = request.VersionId,
                versionLabel = request.VersionLabel,
                version = request.Version

            };
            byte[] lastTSR = null;
            try
            {
                lastTSR = BusinessLogic.Interoperabilità.InteroperabilitaUtils.GetTSRForDocument(infoUtente, fr);
            }
            catch
            { }

            FileDocumento docFile = BusinessLogic.Documenti.FileManager.getFileFirmato(
                fr,
                infoUtente,
                false);

            if (lastTSR != null)
            {
                logger.Debug("TSR presente, incapsulo in TSDiS");
                //controllo il match
                if (!BusinessLogic.Interoperabilità.InteroperabilitaUtils.MatchTSR(lastTSR, docFile.content))
                    lastTSR = null;

                //Gestione TSDis (DOC+TSR)
                //Siccome non si possono mandare piu file allo stesso tempo, ne è possibile alterare la struttura
                //si è pensato:

                //In fase di invio, in caso di documenti con TSR associato, l'invio dello stesso un formato
                //TSD, come marca verrà presa l'ultima disponibile, e unita al documento, creadno un file con estensione TSDis

                //In fase di ricezione il TsdIs, sarà poi spacchettato, il TSR messo in DPA_TIMESTAMP dopo verifica hash e 
                //infine il payload messo in documentale con la putfile.



                //tsr presente, combino il tutto per creare un TSDis
                if (lastTSR != null)
                {
                    try
                    {
                        BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.tsd tsdMgr = new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.tsd();
                        BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile file = new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = docFile.content, Name = docFile.name, MessageFileType = BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.fileType.Binary };
                        List<BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile> tsrLst = new List<BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile>();
                        BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile tsrfile = new BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.CryptoFile { Content = lastTSR, MessageFileType = BusinessLogic.Documenti.DigitalSignature.PKCS_Utils.fileType.Binary };
                        tsrLst.Add(tsrfile);
                        docFile.content = tsdMgr.create(file, tsrLst.ToArray()).Content;
                        request.FileName = request.FileName + ".TSDis";
                    }
                    catch (Exception e)
                    {

                        logger.ErrorFormat("Error creating TSDID {0} {1}", e.Message, e.StackTrace);
                        //Manca il logger pdpdpdpdp
                    }
                }
            }
            //FileDocumento docFile = BusinessLogic.Documenti.FileManager.getFileFirmato(
            //    new FileRequest()
            //    {
            //        docServerLoc = request.DocumentServerLocation,
            //        path = request.FilePath,
            //        fileName = request.FileName,
            //        docNumber = request.DocumentNumber,
            //        versionId = request.VersionId,
            //        versionLabel = request.VersionLabel
            //    },
            //    new DocsPaVO.utente.InfoUtente()
            //    {
            //        idAmministrazione = request.AdministrationId
            //    },
            //    false);
            logger.Debug("END");
            return new RemoteFileInfo
            {
                FileData = new MemoryStream(docFile.content, false),
                FileTransferInfo = new FileTransferInfo()
                {
                    FileName = request.FileName,
                    FileLength = docFile.content.Length
                }
            };
        }

    }
}
