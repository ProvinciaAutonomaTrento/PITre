using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Classe per la gestione dei dati relativi all'utilizzo dei componenti SmartClient
    /// </summary>
    public class SmartClient
    {
        private ILog logger = LogManager.GetLogger(typeof(SmartClient));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public SmartClient(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this.InfoUtente = infoUtente;
        }

        public static bool EnableJavaAppletOption()
        {
            bool retVal = false;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("GET_TIPO_COMPONENTI");
                string commandText = queryDef.getSQL();
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        if (reader.GetValue(reader.GetOrdinal("CHA_TIPO_COMPONENTI")).ToString() == "3")
                            retVal = true;
                    }
                }

            }
            return retVal;
        }

        public static bool EnableHTML5SocketOption()
        {
            bool retVal = false;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("GET_TIPO_COMPONENTI");
                string commandText = queryDef.getSQL();
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        if (reader.GetValue(reader.GetOrdinal("CHA_TIPO_COMPONENTI")).ToString() == "4")
                            retVal = true;
                    }
                }

            }
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        protected DocsPaVO.utente.InfoUtente InfoUtente
        {
            get;
            set;
        }

        /// <summary>
        /// Reperimento dati di configurazione per i componenti smartclient
        /// </summary>
        /// <returns></returns>
        public DocsPaVO.SmartClient.SmartClientConfigurations GetConfigurationsPerUser()
        {
            try
            {
                DocsPaVO.SmartClient.SmartClientConfigurations retValue = new DocsPaVO.SmartClient.SmartClientConfigurations();

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    // Verifica se i componenti SmartClient risultano attivati per lo specifico utente
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("GET_SMART_CLIENT_CONFIGURATIONS_PER_USER");
                    queryDef.setParam("idPeople", this.InfoUtente.idPeople);

                    string commandText = queryDef.getSQL();
                    logger.Debug(string.Format("GET_SMART_CLIENT_CONFIGURATIONS_PER_USER: {0}", commandText));

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        if (reader.Read())
                        {
                            string user_tipo_components = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "user_tipo_components", true, "0");
                            string admin_tipo_components = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "admin_tipo_components", true, "0");

                            retValue.ComponentsType = (user_tipo_components == "0" ? (admin_tipo_components == "0" ? "1" : admin_tipo_components) : user_tipo_components);
                               
                            //retValue.IsEnabled = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "user_config_enabled", true, "0") == "1") ||
                            //                     (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "admin_config_enabled", true, "0") == "1");

                            //if (retValue.IsEnabled)
                            if (retValue.ComponentsType!="1")
                            {
                                retValue.ApplyPdfConvertionOnScan =
                                                (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "user_pdf_conv_enabled", true, "0") == "1") ||
                                                (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "admin_pdf_conv_enabled", true, "0") == "1");
                            }
                        }
                    }
                }

                return retValue;
            }
            catch (Exception ex)
            {
                string errorMessage = "Si è verificato un errore nell'esecuzione del metodo 'GetConfigurations'";

                logger.Error(errorMessage, ex);
                throw new ApplicationException(errorMessage, ex);
            }
        }
    }
}
