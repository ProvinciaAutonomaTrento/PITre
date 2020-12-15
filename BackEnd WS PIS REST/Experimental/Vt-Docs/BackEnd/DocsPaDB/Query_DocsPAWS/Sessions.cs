using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using log4net;
using System.IO;

namespace DocsPaDB.Query_DocsPAWS
{
    public class Sessions : DBProvider
    {
        #region Const

        private ILog logger = LogManager.GetLogger(typeof(Sessions));

        #endregion

        public List<string> GetSession(string sessId)
        {
            logger.Debug("Inizio Metodo GetSession in DocsPaDb.Query_DocsPAWS.Sessions");

            List<string> retVal = null;
            
            try
            {
                DataSet ds = new DataSet();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_SESSIONS");
                q.setParam("sessionId", sessId);

                string query = q.getSQL();
                logger.Debug("GetSession: " + query);

                if (this.ExecuteQuery(out ds, "Dpa_Sessions", query))
                {
                    if (ds.Tables["Dpa_Sessions"] != null && ds.Tables["Dpa_Sessions"].Rows.Count > 0)
                    {
                        retVal = new List<string>();
                        foreach (DataRow row in ds.Tables["Dpa_Sessions"].Rows)
                        {
                            retVal.Add(!string.IsNullOrEmpty(row["Serializzed_Session"].ToString()) ? row["Serializzed_Session"].ToString() : string.Empty);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo GetSession in DocsPaDb.Query_DocsPAWS.Sessions: " + e.Message);
            }

            return retVal;
        }

        public bool StoreSession(string sessId, List<string> serializzedSessList)
        {
            logger.Debug("Inizio Metodo StoreSession in DocsPaDb.Query_DocsPAWS.Sessions");
            bool retValue = true;
            try
            {
                if (serializzedSessList != null && serializzedSessList.Count>0)
                {
                    BeginTransaction();
                    DocsPaUtils.Query q;
                    
                        String strSqlQuery = string.Empty;

                        foreach (string serializzedSession in serializzedSessList)
                        {
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_SESSIONS");
                            q.setParam("sessionId", sessId);
                            q.setParam("sessionValue", serializzedSession.Replace("'", "''"));

                            string query = q.getSQL();
                            //logger.Debug("InsertSession: " + query);
                            if (!ExecuteNonQuery(query))
                            {
                                throw new Exception("Errore durante l'inserimento in DPA_SESSIONS:  " + query);
                            }
                        }
                    /*
                        foreach (string serializzedSession in serializzedSessList)
                        {
                            if (!string.IsNullOrEmpty(strSqlQuery))
                            {
                                strSqlQuery = strSqlQuery + " UNION ";
                            }
                            strSqlQuery = strSqlQuery + "Select '" + sessId + "', '" + serializzedSession.Replace("'","''") + "' FROM DUAL";
                        }
                        q.setParam("selectValue", strSqlQuery);
                        //dbProvider.SetLargeText("DPA_ITEMS_CONSERVAZIONE", sysId, "VAR_XML_METADATI", xmlMetadati[1]);

                        string query = q.getSQL();
                        logger.Debug("InsertSession: " + query);
                        if (!ExecuteNonQuery(query))
                        {
                            throw new Exception("Errore durante l'inserimento in DPA_SESSIONS:  " + query);
                        }
                    */
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo StoreSession in DocsPaDb.Query_DocsPAWS.Sessions: " + ex.Message);
                RollbackTransaction();
                return false;
            }
            logger.Debug("Fine Metodo StoreSession in DocsPaDb.Query_DocsPAWS.Sessions");
            CommitTransaction();
            return retValue;
        }

        public bool DeleteSession(string sessId)
        {
            logger.Debug("Inizio Metodo DeleteSession in DocsPaDb.Query_DocsPAWS.Sessions");
            bool retValue = true;
            try
            {
                if (!string.IsNullOrEmpty(sessId))
                {
                    BeginTransaction();
                    DocsPaUtils.Query q;

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_SESSIONS");
                    q.setParam("sessionId", sessId);

                    string query = q.getSQL();
                    logger.Debug("DeleteSession: " + query);
                    if (!ExecuteNonQuery(query))
                    {
                        throw new Exception("Errore durante l'eliminazione di un record in DPA_SESSIONS:  " + query);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore nel Metodo DeleteSession in DocsPaDb.Query_DocsPAWS.Sessions: " + ex.Message);
                RollbackTransaction();
                return false;
            }
            logger.Debug("Fine Metodo DeleteSession in DocsPaDb.Query_DocsPAWS.Sessions");
            CommitTransaction();
            return retValue;
        }
    }
}
