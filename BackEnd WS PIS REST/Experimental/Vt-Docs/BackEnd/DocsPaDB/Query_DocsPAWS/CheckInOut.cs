using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DocsPaVO.documento;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Classe per la gestione dell'accesso ai dati per i servizi di checkinout
    /// </summary>
    public class CheckInOut
    {
        private ILog logger = LogManager.GetLogger(typeof(CheckInOut));
        /// <summary>
        /// 
        /// </summary>
        public CheckInOut()
        {
        }

        /// <summary>
        /// Reperimento oggetto "FileRequest" relativamente all'ultima
        /// versione del documento corrente
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public FileRequest GetFileRequest(string idDocument)
        {
            FileRequest retValue = null;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECKINOUT_GET_LAST_VERSION_DATA");
                queryDef.setParam("idDocument", idDocument);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            retValue = new FileRequest();

                            retValue.fileSize = reader.GetValue(reader.GetOrdinal("FILE_SIZE")).ToString();
                            retValue.docNumber = reader.GetValue(reader.GetOrdinal("DOCNUMBER")).ToString();
                            retValue.versionId = reader.GetValue(reader.GetOrdinal("VERSION_ID")).ToString();
                            retValue.fileName = reader.GetValue(reader.GetOrdinal("PATH")).ToString();
                            retValue.docServerLoc = GetDocRootPath();
                            retValue.version = reader.GetValue(reader.GetOrdinal("VERSION")).ToString();
                            retValue.subVersion = reader.GetValue(reader.GetOrdinal("SUBVERSION")).ToString();
                            retValue.versionLabel = reader.GetValue(reader.GetOrdinal("VERSION_LABEL")).ToString();
                            retValue.firmatari = new System.Collections.ArrayList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);

                throw new ApplicationException("Errore nel reperimento dell'oggetto 'FileRequest'. IDDocument: " + idDocument, ex);
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento percorso principale del documentale
        /// </summary>
        /// <returns></returns>
        protected string GetDocRootPath()
        {
            return System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public bool IsAcquired(string idDocument)
        {
            FileRequest fileRequest = this.GetFileRequest(idDocument);

            return this.IsAcquired(fileRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public int GetNextVersionId(FileRequest fileRequest)
        {
            int nextVersionId = -1;

            int versionId;
            if (Int32.TryParse(fileRequest.version, out versionId))
            {
                if (this.IsAcquired(fileRequest))
                    nextVersionId = versionId + 1;
                else
                    nextVersionId = versionId;
            }

            return nextVersionId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public int GetNextVersionId(string idDocument)
        {
            FileRequest fileRequest = this.GetFileRequest(idDocument);

            return this.GetNextVersionId(fileRequest);
        }

        /// <summary>
        /// Verifica se per l'ultima versione del documento corrente è stato acquisito un file
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public bool IsAcquired(FileRequest fileRequest)
        {
            return (fileRequest != null && fileRequest.fileName != null && fileRequest.fileName != string.Empty &&
                    fileRequest.fileSize != null && fileRequest.fileSize != "0");
        }
    }
}
