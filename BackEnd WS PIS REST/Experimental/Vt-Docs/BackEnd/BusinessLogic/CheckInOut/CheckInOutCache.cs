using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;
using DocsPaVO.utente;
using System.IO;
using log4net;

namespace BusinessLogic.CheckInOut
{
    static class CheckInOutCache
    {
        private static ILog logger = LogManager.GetLogger(typeof(CheckInOutCache));
        public static bool CheckIn(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, byte[] content, string checkInComments, InfoUtente infoUtente)
        {
            bool retValue = false;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_CHECKIN_DOCUMENT");
                queryDef.setParam("id", checkOutStatus.ID.ToString());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    retValue = dbProvider.ExecuteNonQuery(commandText);

                if (retValue)
                    // Creazione della versione del documento
                    retValue = CreateDocumentVersion(checkOutStatus, content, checkInComments, infoUtente);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nel CheckIn del documento. IDDocumento: " + checkOutStatus.IDDocument, ex);
            }

            return retValue;
        }

        public static DocsPaVO.documento.FileDocumento CreateFileDocument(string documentLocation, byte[] content)
        {
            FileDocumento fileDocument = new FileDocumento();

            FileInfo fileInfo = new FileInfo(documentLocation);
            fileDocument.fullName = fileInfo.FullName;
            fileDocument.name = fileInfo.Name;
            fileDocument.estensioneFile = fileInfo.Extension.Replace(".", string.Empty);

            if (content != null)
            {
                fileDocument.content = content;
                fileDocument.length = content.Length;
            }
            fileDocument.path = GetDocRootPath();

            return fileDocument;
        }

        public static bool CreateDocumentVersion(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, byte[] checkedOutFileContent, string checkInComments, DocsPaVO.utente.InfoUtente checkOutOwner)
        {
            bool retValue = false;

            DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();

            // Reperimento dell'ultima versione del documento
            FileRequest fileRequest = checkInOutDb.GetFileRequest(checkOutStatus.IDDocument);

            FileDocumento fileDocument = CreateFileDocument(checkOutStatus.DocumentLocation, checkedOutFileContent);

            if (checkInOutDb.IsAcquired(fileRequest))
            {
                // Se per l'ultima versione del documento è stato acquisito un file,
                // viene creata nuova versione per il documento
                fileRequest = new FileRequest();
                fileRequest.fileName = checkOutStatus.DocumentLocation;
                fileRequest.docNumber = checkOutStatus.DocumentNumber;

                // Impostazione degli eventuali commenti da aggiungere alla versione
                fileRequest.descrizione = checkInComments;


                retValue = AddVersion(fileRequest, false, checkOutOwner);
            }
            else
            {
                // Se per l'ultima versione del documento non è stato acquisito un file,
                // il file viene acquisito per l'ultima versione
                fileRequest.fileName = fileDocument.fullName;

                // Impostazione degli eventuali commenti da aggiungere alla versione
                fileRequest.descrizione = checkInComments;

                retValue = true;
            }

            if (retValue && fileDocument != null &&
                fileDocument.content != null &&
                fileDocument.content.Length > 0)
            {

                string errore= string.Empty;
                retValue = Documenti.CacheFileManager.PutFile(checkOutOwner, fileRequest, fileDocument, fileDocument.estensioneFile, out errore);
            }

            return retValue;
        }

        private static string GetDocRootPath()
        {
            return System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"];
        }

        public static bool AddVersion(DocsPaVO.documento.FileRequest fileRequest, bool daInviare, InfoUtente infoUtente)
        {
            bool result = true;

            bool update = false;
            string oldApp = null;

            System.Data.DataSet ds;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            try
            {
                if (fileRequest.applicazione != null)
                {
                    if (fileRequest.applicazione.systemId == null)
                    {
                        logger.Debug("sysid vuoto");

                        DocsPaVO.documento.Applicazione res = new DocsPaVO.documento.Applicazione();
                        doc.GetExt(fileRequest.applicazione.estensione, ref res);

                        fileRequest.applicazione = res;
                    }
                    logger.Debug("Update della tabella profile");
                    string param = "(APPLICATION is NULL OR APPLICATION != " + fileRequest.applicazione.systemId + ") AND DOCNUMBER=" + fileRequest.docNumber;
                    doc.GetApplication(out oldApp, fileRequest.docNumber, fileRequest.applicazione.systemId, param);

                    update = true;
                }

                DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                documentale.AddVersion(ref fileRequest, infoUtente.idPeople, infoUtente.userId);

                //ESTRAZIONE DEL FILENAME, VERSION, LASTEDITTIME
                doc.SetCompVersions(fileRequest.versionId, fileRequest.docNumber, out ds);

                fileRequest.fileName = ds.Tables["VERS"].Rows[0]["PATH"].ToString();
                fileRequest.version = ds.Tables["VERS"].Rows[0]["VERSION"].ToString();
                fileRequest.subVersion = ds.Tables["VERS"].Rows[0]["SUBVERSION"].ToString();
                fileRequest.versionLabel = ds.Tables["VERS"].Rows[0]["VERSION_LABEL"].ToString();
                fileRequest.dataInserimento = ds.Tables["VERS"].Rows[0]["DTA_CREAZIONE"].ToString();
                DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
                string full_name_utente = u.getUtenteById(infoUtente.idPeople).descrizione;
                if (full_name_utente != null)
                    fileRequest.autore = full_name_utente;

                //EMosca 29/11/2004
                /*Aggiunto && oldApp!="" nell'if.
                 * oldApp risulta vuoto per tutte le versioni 
                 * (tranne Hummingbird che inserisce di default un pdf di size=0 alla creazione del doc.)
                 */
                if (update && oldApp != "")
                {
                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                    documenti.UpdateApplication(oldApp, fileRequest.docNumber);
                }

                DocsPaDB.Query_DocsPAWS.Documenti documenti2 = new DocsPaDB.Query_DocsPAWS.Documenti();
                documenti2.UpdateVersionManager(fileRequest, daInviare);

                logger.Debug("Fine addVersion");
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante l'aggiunta di una versione.", exception);

                if (update)
                {
                    DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
                    documenti.UpdateApplication(oldApp, fileRequest.docNumber);
                }

                result = false;
            }

            return result;
        }

    }
}
