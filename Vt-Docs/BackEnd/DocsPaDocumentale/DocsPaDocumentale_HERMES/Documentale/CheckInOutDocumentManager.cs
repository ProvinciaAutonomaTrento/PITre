using System;
using System.Data;
using System.Collections;
using DocsPaVO.utente;
using DocsPaVO.CheckInOut;
using DocsPaVO.documento;
using DocsPaDocumentale.Interfaces;
using System.IO;
using log4net;

namespace DocsPaDocumentale_HERMES.Documentale
{
    /// <summary>
    /// Gestione del checkin/checkout di documenti per il documentale ETDOCS
    /// </summary>
    public class CheckInOutDocumentManager : ICheckInOutDocumentManager
    {
        private ILog logger = LogManager.GetLogger(typeof(CheckInOutDocumentManager));
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

        #region Public methods

        /// <summary>
        /// Reperimento delle informazioni di stato su un documento in stato checkedout
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public CheckOutStatus GetCheckOutStatus(string idDocument, string documentNumber)
        {
            CheckOutStatus status = null;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                status = this.GetCheckOutStatus(dbProvider, idDocument, documentNumber);

            return status;
        }

        /// <summary>
        /// Annullamento dello stato CheckedOut per un documento
        /// </summary>
        /// <param name="checkOutInfo"></param>
        /// <returns></returns>
        public bool UndoCheckOut(CheckOutStatus checkOutStatus)
        {
            bool retValue = false;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_UNDO_CHECKOUT_DOCUMENT");
                queryDef.setParam("id", checkOutStatus.ID.ToString());

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    retValue = dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nell'UndoCheckOut del documento. IDDocumento: " + checkOutStatus.IDDocument, ex);
            }

            return retValue;
        }

        /// <summary>
        /// CheckIn di un documento
        /// </summary>
        /// <param name="checkOutInfo">Informazioni di stato sul documento in checkOut</param>
        /// <returns></returns>
        public bool CheckIn(CheckOutStatus checkOutStatus, byte[] content, string checkInComments)
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
                    retValue = this.CreateDocumentVersion(checkOutStatus, content, checkInComments, this.InfoUtente);
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nel CheckIn del documento. IDDocumento: " + checkOutStatus.IDDocument, ex);
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se un particolare utente abbia impostato o meno
        /// lo stato di un documento a checkedout
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        //public bool IsOwnerCheckedOut(string idDocument, string documentNumber, string idUser)
        //{
        //    bool retValue = false;

        //    try
        //    {
        //        DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_IS_OWNER_CHECKED_OUT");
        //        queryDef.setParam("idDocument", idDocument);
        //        queryDef.setParam("documentNumber", documentNumber);
        //        queryDef.setParam("idUser", idUser);

        //        string commandText = queryDef.getSQL();
        //        logger.Debug(commandText);

        //        using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
        //        {
        //            string outParam;

        //            if (dbProvider.ExecuteScalar(out outParam, commandText))
        //            {
        //                try
        //                {
        //                    retValue = (Convert.ToInt32(outParam) > 0);
        //                }
        //                catch
        //                {
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Debug(ex.Message, ex);

        //        throw new ApplicationException("Errore nella verifica dello stato del documento estratto. IDDocumento: " + idDocument, ex);
        //    }

        //    return retValue;
        //}

        /// <summary>
        /// Verifica se un documento è in stato checkedout
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="checkOutInfo">Informazioni sullo stato del checkout</param>
        /// <returns></returns>
        public bool IsCheckedOut(string idDocument, string documentNumber, out string ownerUser)
        {
            bool retValue = false;
            ownerUser = string.Empty;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_IS_CHECKOUT_DOCUMENT");
                queryDef.setParam("idDocument", idDocument);
                queryDef.setParam("documentNumber", documentNumber);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            retValue = true;

                            // Reperimento nome utente
                            ownerUser = reader.GetValue(reader.GetOrdinal("USER_NAME")).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nella verifica dello stato del documento estratto. IDDocumento: " + idDocument, ex);
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se un documento è in stato checkedout
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public bool IsCheckedOut(string idDocument, string documentNumber)
        {
            bool retValue = false;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_IS_CHECKOUT_DOCUMENT");
                queryDef.setParam("idDocument", idDocument);
                queryDef.setParam("documentNumber", documentNumber);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                        if (reader.Read())
                            retValue = true;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message, ex);

                throw new ApplicationException("Errore nella verifica dello stato del documento estratto. IDDocumento: " + idDocument, ex);
            }

            return retValue;
        }

        /// <summary>
        /// CheckOut di un documento
        /// </summary>
        /// <param name="checkOutInfo"></param>
        /// <returns></returns>
        public bool CheckOut(string idDocument, string documentNumber, string documentLocation, string machineName, out CheckOutStatus checkOutStatus)
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

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    try
                    {
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

        #endregion

        #region Protected methods

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
        /// Creazione di una nuova versione del documento
        /// </summary>
        /// <param name="idDocument"></param>
        /// <param name="checkOutStatus"></param>
        /// <param name="checkedOutFile"></param>
        /// <param name="checkInComments"></param>
        /// <returns></returns>
        protected bool CreateDocumentVersion(CheckOutStatus checkOutStatus, byte[] checkedOutFileContent, string checkInComments, InfoUtente checkOutOwner)
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

                DocumentManager documentManager = new DocumentManager(this.InfoUtente);
                retValue = documentManager.AddVersion(fileRequest, false);
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
                // Inserimento del nuovo file per la versione
                DocumentManager documentManager = new DocumentManager(this.InfoUtente);
                retValue = documentManager.PutFile(fileRequest, fileDocument, fileDocument.estensioneFile);
            }

            return retValue;
        }

        /// <summary>
        /// Creazione di un oggetto FileDocumento a partire dal percorso del file
        /// </summary>
        /// <param name="documentLocation"></param>
        /// <returns></returns>
        protected FileDocumento CreateFileDocument(string documentLocation, byte[] content)
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

        /// <summary>
        /// Reperimento percorso principale del documentale
        /// </summary>
        /// <returns></returns>
        protected string GetDocRootPath()
        {
            return System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"];
        }

        #endregion

        #region Private methods

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

        /// <summary>
        /// Caricamento dati oggetto "CheckOutInfo" da datareader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="status"></param>
        /// <returns></returns>
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

            if (!reader.IsDBNull(reader.GetOrdinal("DOCUMENT_LOCATION")))
                status.DocumentLocation = reader.GetValue(reader.GetOrdinal("DOCUMENT_LOCATION")).ToString();

            if (!reader.IsDBNull(reader.GetOrdinal("MACHINE_NAME")))
                status.MachineName = reader.GetValue(reader.GetOrdinal("MACHINE_NAME")).ToString(); // Nome macchina non gestito dal documente Hummingbird

            if (!reader.IsDBNull(reader.GetOrdinal("ID_DOCUMENTO_PRINCIPALE")))
                status.IsAllegato = !string.IsNullOrEmpty(reader.GetValue(reader.GetOrdinal("ID_DOCUMENTO_PRINCIPALE")).ToString());
        }

        #endregion
    }
}