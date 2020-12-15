using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DocsPaUtils.Interfaces.DbManagement;
using DocsPaDbManagement.Functions;
using DocsPaDB.Utils;
using DocsPaUtils;
using log4net;


namespace DocsPaDB.Query_DocsPAWS
{
    public class GestioneApplicazioni
    {
        private static ILog logger = LogManager.GetLogger(typeof(GestioneApplicazioni));

        //Creazione di una nuova delega 
        public static bool CreateNewExtApplication(DocsPaVO.utente.ExtApplication applicazione)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_APPLICAZIONE");
                    
                    q.setParam("paramA", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    q.setParam("paramB", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_EXT_APPS"));
                    q.setParam("param1", applicazione.codice);
                    q.setParam("param2", applicazione.descrizione);

                    queryString = q.getSQL();
                    logger.Debug("Inserimento nuova applicazione: ");
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }


        public static bool UpdateExtApplication(DocsPaVO.utente.ExtApplication applicazione)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_APPLICAZIONE");
                    q.setParam("param1", applicazione.codice);
                    q.setParam("param2", applicazione.descrizione);
                    q.setParam("param3", applicazione.systemId);
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }

        public static ArrayList GetExtApplications()
        {
            ArrayList appicazioni = new ArrayList();

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                logger.Debug("getExtApplication");
                Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_APPLICAZIONI");
                string commandText = query.getSQL();
                logger.Debug(commandText);
                DataSet dataSet;
                if (dbProvider.ExecuteQuery(out dataSet, "DPA_EXT_APPS", commandText))
                {
                    foreach (DataRow dataRow in dataSet.Tables["DPA_EXT_APPS"].Rows)
                    {
                        DocsPaVO.utente.ExtApplication extApp = new DocsPaVO.utente.ExtApplication();
                        extApp.systemId = dataRow["SYSTEM_ID"].ToString();
                        extApp.codice = dataRow["VAR_CODE"].ToString();
                        extApp.descrizione = dataRow["DESCRIPTION"].ToString();
                        appicazioni.Add(extApp);
                    }
                    dataSet.Dispose();
                }
                else
                {
                    logger.Debug("Errore nell'esecuzione della query in 'getExtApplications'");
                    throw new ApplicationException("Errore nell'esecuzione della query 'getExtApplications'");
                }
            }
            return appicazioni;
        }

        public static DocsPaVO.utente.ExtApplication GetExtApplicationByID(string applicationId)
        {
            DocsPaVO.utente.ExtApplication extApp = null;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                logger.Debug("getExtApplication");
                Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_APPLICAZIONI");
                string commandText = query.getSQL() + " WHERE SYSTEM_ID = '" + applicationId + "'";

                logger.Debug(commandText);
                DataSet dataSet;
                if (dbProvider.ExecuteQuery(out dataSet, "DPA_EXT_APPS", commandText))
                {
                    foreach (DataRow dataRow in dataSet.Tables["DPA_EXT_APPS"].Rows)
                    {
                        extApp = new DocsPaVO.utente.ExtApplication();
                        extApp.systemId = dataRow["SYSTEM_ID"].ToString();
                        extApp.codice = dataRow["VAR_CODE"].ToString();
                        extApp.descrizione = dataRow["DESCRIPTION"].ToString();
                    }
                    dataSet.Dispose();
                }
                else
                {
                    logger.Debug("Errore nell'esecuzione della query in 'getExtApplicationByID'");
                    throw new ApplicationException("Errore nell'esecuzione della query 'getExtApplicationByID'");
                }
            }

            return extApp;
        }

        public static DocsPaVO.utente.ExtApplication GetExtApplicationByCOD(string codiceApp)
        {
            DocsPaVO.utente.ExtApplication extApp = null;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                logger.Debug("getExtApplication");
                Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_APPLICAZIONI");
                string commandText = query.getSQL() + " WHERE VAR_CODE = '" + codiceApp + "'";

                logger.Debug(commandText);
                DataSet dataSet;
                if (dbProvider.ExecuteQuery(out dataSet, "DPA_EXT_APPS", commandText))
                {
                    foreach (DataRow dataRow in dataSet.Tables["DPA_EXT_APPS"].Rows)
                    {
                        extApp = new DocsPaVO.utente.ExtApplication();
                        extApp.systemId = dataRow["SYSTEM_ID"].ToString();
                        extApp.codice = dataRow["VAR_CODE"].ToString();
                        extApp.descrizione = dataRow["DESCRIPTION"].ToString();
                    }
                    dataSet.Dispose();
                }
                else
                {
                    logger.Debug("Errore nell'esecuzione della query in 'getExtApplicationByCOD'");
                    throw new ApplicationException("Errore nell'esecuzione della query 'getExtApplicationByCOD'");
                }
            }

            return extApp;
        }

        public static bool CreateRelExtApp_Ute(string idApplicazione, string idUtente)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {

                    //inserimento della nuova delega nella tabelle DPA_GestioneApplicazioni
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_REL_APP_UTENTE");
                    q.setParam("param1", idUtente);
                    q.setParam("param2", idApplicazione);

                    queryString = q.getSQL();
                    logger.Debug("Inserimento relazione applicazione utente: ");
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }

        public static bool CreateRelExtApp_Utents(string idApplicazione, string idUtenti)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {

                    //inserimento della nuova delega nella tabelle DPA_GestioneApplicazioni
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_REL_APP_UTENTI");
                    q.setParam("param1", idApplicazione);
                    q.setParam("param2", idUtenti);

                    queryString = q.getSQL();
                    logger.Debug("Inserimento relazione applicazione utente: ");
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }


        public static bool DeleteRelExtApp_Ute(string idApplicazione, string idUtente)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_REL_APP_UTENTE");
                    q.setParam("param1", idApplicazione + " AND ID_PEOPLE = " + idUtente);
                    queryString = q.getSQL();

                    logger.Debug("Cancellazione relazione applicazione utente: ");
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }

        public static bool DeleteRelExtApp_Utents(string idApplicazione, string idUtenti)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_REL_APP_UTENTE");
                    q.setParam("param1", idApplicazione + " AND ID_PEOPLE IN (" + idUtenti + ")");
                    queryString = q.getSQL();

                    logger.Debug("Cancellazione relazioni applicazione utenti: ");
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }

        public static bool DeleteExtApplication(string idApplicazione)
        {
            bool result = true;
            DocsPaUtils.Query q;
            string queryString;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_REL_APP_UTENTE");
                    q.setParam("param1", idApplicazione);

                    logger.Debug("Cancellazione relazioni applicazione utenti per eliminare Applicazione: ");
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }
                    else
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_APPLICAZIONE");
                        q.setParam("param1", idApplicazione);

                        logger.Debug("Cancellazione Applicazione: ");
                        logger.Debug(queryString);
                        if (!dbProvider.ExecuteNonQuery(queryString))
                        {
                            result = false;
                            dbProvider.RollbackTransaction();
                            throw new Exception();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result = false;
                logger.Debug(e.Message);
            }
            return result;
        }
    }
}
