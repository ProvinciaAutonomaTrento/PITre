using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Classe per la gestione dei tipi di word processor client side utilizzabili
    /// per la stampa unione dei documenti tramite modelli
    /// </summary>
    public class ClientSideModelProcessor
    {
        private ILog logger = LogManager.GetLogger(typeof(ClientSideModelProcessor));
        #region Public members

        /// <summary>
        /// 
        /// </summary>
        public ClientSideModelProcessor()
        {
        }

        /// <summary>
        /// Reperimento dei metadati relativi a tutti i word processors disponibili nel sistema
        /// </summary>
        /// <returns></returns>
        public DocsPaVO.Modelli.ModelProcessorInfo[] GetModelProcessors()
        {
            List<DocsPaVO.Modelli.ModelProcessorInfo> list = new List<DocsPaVO.Modelli.ModelProcessorInfo>();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_CLIENT_MODEL_PROCESSORS");

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            list.Add(this.Create(reader));
                        }
                    }

                    return list.ToArray();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Errore nel reperimento delle informazioni sui Word Processor disponibili nel sistema";

                logger.Debug(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Reperimento dei metadati relativi al word processor utilizzato dall'amministrazione
        /// </summary>
        /// <param name="idAdmin"></param>
        /// <returns></returns>
        public DocsPaVO.Modelli.ModelProcessorInfo GetModelProcessorForAdmin(string idAdmin)
        {
            try
            {
                DocsPaVO.Modelli.ModelProcessorInfo retValue = null;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_CLIENT_MODEL_PROCESSOR_FOR_ADMIN");
                    queryDef.setParam("idAdmin", idAdmin);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            retValue = this.Create(reader);
                        }
                    }

                    return retValue;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Errore nel reperimento delle informazioni sul Word Processor impostato per l'amministrazione";

                logger.Debug(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Reperimento dei metadati relativi al word processor utilizzato dall'utente
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public DocsPaVO.Modelli.ModelProcessorInfo GetModelProcessorForUser(string idUser)
        {
            try
            {
                DocsPaVO.Modelli.ModelProcessorInfo retValue = null;

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_CLIENT_MODEL_PROCESSOR_FOR_USER");
                    queryDef.setParam("idUser", idUser);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            retValue = this.Create(reader);
                        }
                    }

                    return retValue;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "Errore nel reperimento delle informazioni sul Word Processor impostato per l'utente";

                logger.Debug(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        public byte[] GetLicense(string code)
        {
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_LICENSE");
                    query.setParam("codApp", code);

                    string commandText = query.getSQL();
                    string field = string.Empty;

                    logger.Debug(commandText);
                    if(!dbProvider.ExecuteScalar(out field, commandText))
                        throw new Exception(dbProvider.LastExceptionMessage);

                    return System.Text.Encoding.ASCII.GetBytes(field);

                }
            }
            catch (Exception ex)
            {
                logger.DebugFormat("{0}\r\n{1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        #endregion

        #region Protected members

        /// <summary>
        /// Creazione nuovo oggetto ModelProcessorInfo da datareader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected DocsPaVO.Modelli.ModelProcessorInfo Create(System.Data.IDataReader reader)
        {
            DocsPaVO.Modelli.ModelProcessorInfo item = new DocsPaVO.Modelli.ModelProcessorInfo();
            item.Id = Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "SYSTEM_ID", false));
            item.Name = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "NAME", false);
            item.ClassId = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "CLASS_ID", false);
            item.SupportedExtensions = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "SUPPORTED_EXTENSIONS", false, string.Empty);
            return item;
        }

        #endregion

        #region Private members


        #endregion
    }
}
