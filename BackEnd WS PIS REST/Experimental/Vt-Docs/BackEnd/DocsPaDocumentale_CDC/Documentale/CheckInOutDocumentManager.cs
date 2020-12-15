using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.utente;
using DocsPaVO.CheckInOut;
using DocsPaVO.documento;
using CheckInOutManagerOCS = DocsPaDocumentale_OCS.Documentale.CheckInOutDocumentManager;
using CheckInOutManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.CheckInOutDocumentManager;
using DocumentManagerETDOCS = DocsPaDocumentale_ETDOCS.Documentale.DocumentManager;
using DocumentManagerOCS = DocsPaDocumentale_OCS.Documentale.DocumentManager;
using log4net;



namespace DocsPaDocumentale_CDC.Documentale
{
    /// <summary>
    /// Classe per la gestione del checkIn/checkOut dell'ultima versione di un documento
    /// </summary>
    public class CheckInOutDocumentManager : ICheckInOutDocumentManager
    {
        private ILog logger = LogManager.GetLogger(typeof(CheckInOutDocumentManager));
        #region Ctros, variables, constants

        /// <summary>
        /// 
        /// </summary>
        private CheckInOutManagerOCS _checkInOutOCS = null;
        private CheckInOutManagerETDOCS _checkInOutETDOCS = null;
        private DocumentManagerOCS _documentManagerOCS = null;
        private DocumentManagerETDOCS _documentManagerETDOCS = null;

        /// <summary>
        /// 
        /// </summary>
        private InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public CheckInOutDocumentManager(InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public Members

        public CheckOutStatus GetCheckOutStatus(string idDocument, string documentNumber)
        {
            //Non so se deve funzionare così
            return this.CheckInOutManagerETDOCS.GetCheckOutStatus(idDocument, documentNumber);
        }

        public bool IsCheckedOut(string idDocument, string documentNumber)
        {
            return this.CheckInOutManagerETDOCS.IsCheckedOut(idDocument, documentNumber);
        }

        public bool IsCheckedOut(string idDocument, string documentNumber, out string ownerUser)
        {
            return this.CheckInOutManagerETDOCS.IsCheckedOut(idDocument, documentNumber, out ownerUser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <param name="user"></param>
        /// <param name="library"></param>
        /// <returns></returns>
        public bool CheckOut(string idDocument, string documentNumber, string documentLocation, string machineName, out CheckOutStatus checkOutStatus)
        {
            //faccio il checkout su ETDOCS, se và bene metto in lock il documento su OCS
            bool retValue = false;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                retValue = CheckOutDocsPa(idDocument, documentNumber,documentLocation,machineName, out checkOutStatus);

                if (retValue)
                {
                    CheckOutStatus checkOutStatusNew;
                    retValue = this.CheckInOutManagerOCS.CheckOut(idDocument, documentNumber, documentLocation, machineName, out checkOutStatusNew);
                }
                    if (retValue)
                    transactionContext.Complete();
            }
            return retValue;
        }

        public bool CheckIn(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus, byte[] content, string checkInComments)
        {
            //preparo il checkin su ETDOCS, se và bene lo faccio su OCS

            bool retValue = false;
            try
            {
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_CHECKIN_DOCUMENT");
                    queryDef.setParam("id", checkOutStatus.ID.ToString());

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                        retValue = dbProvider.ExecuteNonQuery(commandText);
                    if (retValue)
                    {
                        // Creazione della versione del documento in etdocs
                        // Reperimento dell'ultima versione del documento
                        bool fileIsAcquired = false;
                        FileRequest fileRequest = getLastVersion(checkOutStatus.IDDocument);
                        DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();
                        fileIsAcquired = checkInOutDb.IsAcquired(fileRequest);

                        retValue = this.CreateDocumentVersion(checkOutStatus, checkInComments, content, fileIsAcquired, ref fileRequest);

                        if (retValue)
                        {
                            //lavoro su OCS
                            if (fileIsAcquired)  //devo aggiungere una nuova versione su OCS
                            {
                                retValue = this.DocumentManagerOCS.AddVersion(fileRequest, false);
                            }
                            if (retValue)
                            {
                                FileDocumento fileDoc = new FileDocumento();
                                string estensione = fileRequest.fileName.Substring(fileRequest.fileName.LastIndexOf(".") + 1);

                                fileDoc.cartaceo = fileRequest.cartaceo;
                                fileDoc.content = content;

                                fileDoc.estensioneFile = estensione;
                                fileDoc.name = fileRequest.fileName;
                                retValue = this.DocumentManagerOCS.PutFile(fileRequest, fileDoc, estensione);
                            }
                            if (retValue)
                                this.CheckInOutManagerOCS.CheckIn(checkOutStatus, content, checkInComments);

                            //NB: gestire gli errori e le transazioni !!!

                        }
                    }
                    if (retValue)
                    {
                        transactionContext.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nel CheckIn del documento. IDDocumento: " + checkOutStatus.IDDocument, ex);
            }

            return retValue;
        }

        public bool UndoCheckOut(DocsPaVO.CheckInOut.CheckOutStatus checkOutStatus)
        {
            bool retValue = false;

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                retValue = this.CheckInOutManagerETDOCS.UndoCheckOut(checkOutStatus);
                if (retValue)
                    retValue = this.CheckInOutManagerOCS.UndoCheckOut(checkOutStatus);

                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;

        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Creazione di una nuova versione del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="checkOutStatus"></param>
        /// <param name="checkedOutFile"></param>
        /// <param name="checkInComments"></param>
        /// <returns></returns>
        protected bool CreateDocumentVersion(CheckOutStatus checkOutStatus, string checkInComments, byte[] content, bool fileIsAcquired, ref FileRequest fileRequest)
        {
            bool retValue = false;

            DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();

            retValue = this.AddCheckInVersion(this.InfoUtente, checkOutStatus, checkInComments, ref fileRequest, content, fileIsAcquired);

            return retValue;
        }

        protected FileRequest getLastVersion(string idDocumento)
        {
            DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();

            // Reperimento dell'ultima versione del documento
            FileRequest fileRequest = checkInOutDb.GetFileRequest(idDocumento);
            return fileRequest;
        }

        /// <summary>
        /// 
        /// </summary>
        protected CheckInOutManagerOCS CheckInOutManagerOCS
        {
            get
            {
                if (this._checkInOutOCS == null)
                    this._checkInOutOCS = new CheckInOutManagerOCS(this.InfoUtente);
                return this._checkInOutOCS;
            }
        }

        /// </summary>
        protected CheckInOutManagerETDOCS CheckInOutManagerETDOCS
        {
            get
            {
                if (this._checkInOutETDOCS == null)
                    this._checkInOutETDOCS = new CheckInOutManagerETDOCS(this.InfoUtente);
                return this._checkInOutETDOCS;
            }
        }

        /// </summary>
        protected DocumentManagerOCS DocumentManagerOCS
        {
            get
            {
                if (this._documentManagerOCS == null)
                    this._documentManagerOCS = new DocumentManagerOCS(this.InfoUtente);
                return this._documentManagerOCS;
            }
        }

        /// </summary>
        protected DocumentManagerETDOCS DocumentManagerETDOCS
        {
            get
            {
                if (this._documentManagerETDOCS == null)
                    this._documentManagerETDOCS = new DocumentManagerETDOCS(this.InfoUtente);
                return this._documentManagerETDOCS;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected InfoUtente InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        /// <summary>
        /// Creazione dei metadati di una nuova versione di un documento in etdocs
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileContent"></param>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        protected virtual bool AddCheckInVersion(InfoUtente infoUtente, CheckOutStatus checkOutStatus, string checkInComments, ref FileRequest fileRequest, byte[] fileContent, bool fileIsAcquired)
        {
            bool retValue = false;

            DocsPaDocumentale_ETDOCS.Documentale.DocumentManager documentManager = new DocsPaDocumentale_ETDOCS.Documentale.DocumentManager(infoUtente);

            //DocsPaDB.Query_DocsPAWS.CheckInOut checkInOutDb = new DocsPaDB.Query_DocsPAWS.CheckInOut();
            //if (checkInOutDb.IsAcquired(fileRequest))

            if (fileIsAcquired)
            {
                // Se per l'ultima versione del documento è stato acquisito un file,
                // viene creata nuova versione per il documento
                fileRequest = new FileRequest();
                fileRequest.docNumber = checkOutStatus.DocumentNumber;
                fileRequest.descrizione = checkInComments;
            }
            else
            {
                // Se per l'ultima versione del documento non è stato acquisito un file, il file viene acquisito per l'ultima versione
                fileRequest.descrizione = checkInComments;
                retValue = true;
            }

            // Inserimento nuova versione in etdocs
            retValue = documentManager.AddVersion(fileRequest, false);

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(checkOutStatus.DocumentLocation);
            //in OCS il nome del file coincide con il docNumber non con il versionId
            fileRequest.fileName = fileRequest.docNumber + fileInfo.Extension;
            fileRequest.fileSize = fileContent.Length.ToString();

            // Impostazione del file come inserito in etdocs
            this.SetFileAsInserted(fileRequest, fileContent);

            return retValue;
        }

        /// <summary>
        /// Inserimento dei metadati di un file inserito in etdocs
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileContent"></param>
        /// <param name="fileExtension"></param>
        protected virtual void SetFileAsInserted(FileRequest fileRequest, byte[] fileContent)
        {
            // Aggiornamento tabella COMPONENTS				
            string varImpronta = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta256(fileContent);
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.UpdateComponents(fileRequest.fileSize, varImpronta, fileRequest.versionId, fileRequest.docNumber);

            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            documentale.UpdateFileName(fileRequest.fileName, fileRequest.versionId);

            //Aggiornamento tabella PROFILE
            int version;
            if (Int32.TryParse(fileRequest.version, out version))
            {
                if (version > 0)
                    doc.SetImg(fileRequest.docNumber);
            }
        }

        protected bool CheckOutDocsPa(string idDocument, string documentNumber, string documentLocation, string machineName, out CheckOutStatus checkOutStatus)
        {
            checkOutStatus = null;
            bool retValue = false;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_CHECKOUT_DOCUMENT");
                queryDef.setParam("colId", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                queryDef.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(null));
                queryDef.setParam("idDocument", idDocument);
                queryDef.setParam("documentNumber", documentNumber);
                queryDef.setParam("idUser", this.InfoUtente.idPeople);
                queryDef.setParam("idRole", this.InfoUtente.idCorrGlobali);
                queryDef.setParam("checkOutDate", DocsPaDbManagement.Functions.Functions.GetDate(true));
                queryDef.setParam("documentLocation", documentLocation);
                queryDef.setParam("machineName", machineName);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                {
                    try
                    {
                        DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

                        int rowsAffected;

                        if (dbProvider.ExecuteNonQuery(commandText, out rowsAffected))
                        {
                            commandText = DocsPaDbManagement.Functions.Functions.GetQueryLastSystemIdInserted();
                            logger.Debug(commandText);

                            string id;

                            if (dbProvider.ExecuteScalar(out id, commandText))
                            {
                                checkOutStatus = this.GetCheckOutStatus(dbProvider, idDocument, documentNumber);
                                retValue = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nel checkout del documento. IDDocumento: " + checkOutStatus.ID, ex);
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento delle informazioni di stato su un documento in stato checkedout
        /// </summary>
        /// <param name="dbProvider"></param>
        /// <param name="idDocument"></param>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        private CheckOutStatus GetCheckOutStatus(DocsPaDB.DBProvider dbProvider, string idDocument, string documentNumber)
        {
            CheckOutStatus status = null;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_GET_DOCUMENT_STATUS");
                queryDef.setParam("idDocument", idDocument);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        status = new CheckOutStatus();
                        this.FetchCheckOutInfo(reader, status);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nel reperimento dello stato del documento in CheckOut. IDDocumento: " + idDocument, ex);
            }

            return status;
        }
        private void FetchCheckOutInfo(IDataReader reader, CheckOutStatus status)
        {
            status.ID = reader.GetValue(reader.GetOrdinal("SYSTEM_ID")).ToString();
            status.IDDocument = reader.GetValue(reader.GetOrdinal("ID_DOCUMENT")).ToString();
            status.DocumentNumber = reader.GetValue(reader.GetOrdinal("DOCUMENT_NUMBER")).ToString();
            status.Segnature = reader.GetValue(reader.GetOrdinal("VAR_SEGNATURA")).ToString();
            status.IDUser = reader.GetValue(reader.GetOrdinal("ID_USER")).ToString();
            status.UserName = reader.GetValue(reader.GetOrdinal("USER_NAME")).ToString();
            status.IDRole = reader.GetValue(reader.GetOrdinal("ID_ROLE")).ToString();
            status.RoleName = reader.GetValue(reader.GetOrdinal("ROLE_NAME")).ToString();
            status.CheckOutDate = reader.GetDateTime(reader.GetOrdinal("CHECK_OUT_DATE"));
            status.DocumentLocation = reader.GetString(reader.GetOrdinal("DOCUMENT_LOCATION"));
            status.MachineName = reader.GetValue(reader.GetOrdinal("MACHINE_NAME")).ToString(); // Nome macchina non gestito dal documente Hummingbird
        }

        #endregion
    }
}
