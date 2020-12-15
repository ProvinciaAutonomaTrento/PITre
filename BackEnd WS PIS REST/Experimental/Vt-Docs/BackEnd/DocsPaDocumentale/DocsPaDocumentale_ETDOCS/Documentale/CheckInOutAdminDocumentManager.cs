using System;
using System.Data;
using System.Collections;
using DocsPaVO.CheckInOut;
using DocsPaVO.utente;
using DocsPaDocumentale.Interfaces;
using log4net;

namespace DocsPaDocumentale_ETDOCS.Documentale
{
    /// <summary>
    /// Gestione del checkin/checkout di documenti per l'amministrazione
    /// per il documentale ETDOCS
    /// </summary>
    public class CheckInOutAdminDocumentManager : ICheckInOutAdminDocumentManager
    {
        private ILog logger = LogManager.GetLogger(typeof(CheckInOutAdminDocumentManager));
        #region Ctors, constants, variables

        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public CheckInOutAdminDocumentManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Reperimento delle informazioni di stato sui documenti in stato checkedout 
        /// </summary>
        /// <param name="idAmministration"></param>
        /// <returns></returns>
        public CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_GET_CHECKED_OUT_DOCUMENTS");
            queryDef.setParam("idAmm", idAmministration);

            return this.GetCheckOutStatusDocuments(queryDef);
        }

        /// <summary>
        /// Reperimento delle informazioni di stato sui documenti in stato checkedout
        /// relativamente ad un utente
        /// </summary>
        /// <param name="idAmministration"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public CheckOutStatus[] GetCheckOutStatusDocuments(string idAmministration, string idUser)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_GET_CHECKED_OUT_DOCUMENTS_USER");
            queryDef.setParam("idAmm", idAmministration);
            queryDef.setParam("idUser", idUser);

            return this.GetCheckOutStatusDocuments(queryDef);
        }

        /// <summary>
        /// Annullamento del blocco del documento nel documentale
        /// </summary>
        /// <param name="adminInfoUtente"></param>
        /// <param name="checkOutAdminStatus"></param>
        /// <returns></returns>
        public bool ForceUndoCheckOut(CheckOutStatus checkOutAdminStatus)
        {
            bool retValue = false;

            if (this.CanForceUndoCheckOut())
            {
                try
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_FORCE_UNDO_CHECKOUT_DOCUMENT");
                    queryDef.setParam("id", checkOutAdminStatus.ID.ToString());

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                        retValue = dbProvider.ExecuteNonQuery(commandText);
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message, ex);

                    throw new ApplicationException("Errore nell'UndoCheckOut del documento. IDDocumento: " + checkOutAdminStatus.IDDocument, ex);
                }
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se, per il documentale, è possibile annullare il blocco
        /// </summary>
        /// <param name="adminInfoUtente"></param>
        /// <returns></returns>
        public bool CanForceUndoCheckOut()
        {
            return true;
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

        #endregion

        #region Private methods

        /// <summary>
        /// Creazione oggetto "CheckOutAdminStatus" da datareader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private CheckOutStatus GetCheckOutAdminStatus(IDataReader reader)
        {
            CheckOutStatus retValue = new CheckOutStatus();

            retValue.ID = reader.GetValue(reader.GetOrdinal("ID_STATUS")).ToString();
            retValue.IDDocument = reader.GetValue(reader.GetOrdinal("ID_DOCUMENT")).ToString();
            retValue.DocumentNumber = reader.GetValue(reader.GetOrdinal("DOCUMENT_NUMBER")).ToString();
            retValue.Segnature = reader.GetValue(reader.GetOrdinal("VAR_SEGNATURA")).ToString();
            retValue.IDUser = reader.GetValue(reader.GetOrdinal("ID_USER")).ToString();
            retValue.UserName = reader.GetValue(reader.GetOrdinal("USER_NAME")).ToString();
            retValue.IDRole = reader.GetValue(reader.GetOrdinal("ID_ROLE")).ToString();
            retValue.RoleName = reader.GetValue(reader.GetOrdinal("ROLE_NAME")).ToString();
            retValue.CheckOutDate = reader.GetDateTime(reader.GetOrdinal("CHECK_OUT_DATE"));

            if (!reader.IsDBNull(reader.GetOrdinal("DOCUMENT_LOCATION")))
                retValue.DocumentLocation = reader.GetValue(reader.GetOrdinal("DOCUMENT_LOCATION")).ToString();
            
            if (!reader.IsDBNull(reader.GetOrdinal("MACHINE_NAME")))
                retValue.MachineName = reader.GetValue(reader.GetOrdinal("MACHINE_NAME")).ToString(); // Nome macchina non gestito dal documente Hummingbird

            if (!reader.IsDBNull(reader.GetOrdinal("ID_DOCUMENTO_PRINCIPALE")))
                retValue.IsAllegato = !string.IsNullOrEmpty(reader.GetValue(reader.GetOrdinal("ID_DOCUMENTO_PRINCIPALE")).ToString());

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryDef"></param>
        /// <returns></returns>
        private CheckOutStatus[] GetCheckOutStatusDocuments(DocsPaUtils.Query queryDef)
        {
            ArrayList retValue = new ArrayList();

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                            retValue.Add(this.GetCheckOutAdminStatus(reader));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);

                throw ex;
            }

            return (CheckOutStatus[])retValue.ToArray(typeof(CheckOutStatus));
        }

        #endregion
    }
}
