using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using DocsPaVO.UploadFiles;

namespace BusinessLogic.UploadFiles
{
    public class UploadFilesManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(UploadFilesManager));
        
        /// <summary>
        /// Metodo per l'estrazione degli eventi di notifica
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static List<FileInUpload> GetListFilesUploaded(DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<FileInUpload> listFileInUpload = new List<FileInUpload>();
            try
            {
                DocsPaDB.Query_DocsPAWS.UploadFiles uploadFiles = new DocsPaDB.Query_DocsPAWS.UploadFiles();
                listFileInUpload = uploadFiles.GetListFilesUploaded(infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.UploadFiles.UploadFilesManager  - metodo: GetEventNotification ", e);
            }
            return listFileInUpload;
        }

        public static bool PutFileFromUploadManager(ref DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.FileDocumento fileDocument, string fileName, string fileDescription, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool retVal = false;

            fileRequest.path = fileDocument.fullName;
            fileRequest.fileName = fileDocument.name;

            try
            {
                fileDocument.content = System.IO.File.ReadAllBytes(fileDocument.fullName);
            }
            catch(Exception ex)
            {
                logger.Debug("Errore in BusinessLogic.UploadFiles.UploadFilesManager  - metodo: PutFileFromUploadManager ", ex);
                return false;
            }

            string msgError = string.Empty;

            if (BusinessLogic.Documenti.FileManager.putFile(ref fileRequest, fileDocument, infoUtente, true, out msgError))
            {
                DocsPaDB.Query_DocsPAWS.UploadFiles uploadFiles = new DocsPaDB.Query_DocsPAWS.UploadFiles();
                //if (uploadFiles.DeleteFileInUpload(hashFile, infoUtente))
                if (uploadFiles.DeleteFileInUpload(fileName, fileDescription, infoUtente))
                {
                    System.IO.File.Delete(fileDocument.fullName);
                }

                retVal = true;
            }
            else
            {
                logger.Debug("Errore in BusinessLogic.UploadFiles.UploadFilesManager  - metodo: PutFileFromUploadManager ", new Exception(msgError));
                retVal = false;
            }

            return retVal;
        }
    }
}
