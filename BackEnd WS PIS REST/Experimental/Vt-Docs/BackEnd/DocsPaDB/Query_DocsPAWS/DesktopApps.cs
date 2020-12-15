using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using log4net;
using DocsPaVO.DesktopApps;
using DocsPaUtils;

namespace DocsPaDB.Query_DocsPAWS
{
    public class DesktopApps : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(DocsPaDB.Query_DocsPAWS.Fatturazione));

        public List<DesktopApp> getDesktopApps()
        {
            List<DesktopApp> retList = new List<DesktopApp>();

            try
            {
                Query query = InitQuery.getInstance().getQuery("S_DPA_DESKTOP_APPS");
                string commandText = query.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader reader = ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            DesktopApp tempApp = new DesktopApp();
                            tempApp.Nome = (reader.GetValue(reader.GetOrdinal("NOME")).ToString());
                            tempApp.Versione = (reader.GetValue(reader.GetOrdinal("VERSIONE")).ToString());
                            tempApp.Path = (reader.GetValue(reader.GetOrdinal("PATH")).ToString());
                            tempApp.Descrizione = (reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString());

                            retList.Add(tempApp);
                        }
                    }
                }

                return retList;

            }
            catch (Exception ex)
            {
                logger.Debug(ex);
                throw new Exception(ex.Message);
            }
        }

        public bool NewDesktopApp(DesktopApp app)
        {
            logger.Debug("Inizio Metodo NewDesktopApp in DocsPaDb.Query_DocsPAWS.DesktopApps");
            bool retValue = true;
            
                try
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_DESKTOP_APPS");
                    q.setParam("nome", app.Nome);
                    q.setParam("versione", app.Versione);
                    q.setParam("path", app.Path);
                    q.setParam("descrizione", app.Descrizione);

                    string query = q.getSQL();
                    logger.Debug("NewDesktopApp: " + query);
                    if (!ExecuteNonQuery(query))
                    {
                        logger.Error("Errore durante l'inserimento in DPA_PASSO_DPA_EVENTO: " + query);
                        retValue = false;
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Errore nel Metodo NewDesktopApp in DocsPaDb.Query_DocsPAWS.DesktopApps: " + e.Message);
                    return false;
                }

                logger.Debug("Fine Metodo NewDesktopApp in DocsPaDb.Query_DocsPAWS.DesktopApps");
            return retValue;
        }

        private bool UpdateDesktopApp(string nome, string versione, string path, string description)
        {
            logger.Debug("Inizio Metodo NewDesktopApp in DocsPaDb.Query_DocsPAWS.DesktopApps");
            bool retValue = true;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_DESKTOP_APP");
                q.setParam("nome", nome);
                
                string parametri = string.Empty;

                if (!string.IsNullOrEmpty(versione))
                {
                    parametri = "SET VERSIONE = '" + versione + "'";
                }

                if (!string.IsNullOrEmpty(path))
                {
                    path = "PATH = '" + path + "'";
                    if (string.IsNullOrEmpty(parametri))
                        parametri = "SET " + path;
                    else
                        parametri = ", " + path;
                }

                if (!string.IsNullOrEmpty(description))
                {
                    description = "DESCRIZIONE = '" + description + "'";
                    if (string.IsNullOrEmpty(parametri))
                        parametri = "SET " + description;
                    else
                        parametri = ", " + description;
                }

                q.setParam("parametri", parametri);

                string query = q.getSQL();
                logger.Debug("NewDesktopApp: " + query);
                if (!ExecuteNonQuery(query))
                {
                    logger.Error("Errore durante l'inserimento in DPA_DESKTOP_APPS: " + query);
                    retValue = false;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo NewDesktopApp in DocsPaDb.Query_DocsPAWS.DesktopApps: " + e.Message);
                return false;
            }

            logger.Debug("Fine Metodo NewDesktopApp in DocsPaDb.Query_DocsPAWS.DesktopApps");
            return retValue;
        }

        public bool UpdateDesktopAppVersion(string nome, string versione)
        {
            return UpdateDesktopApp(nome,versione,null,null);
        }

        public bool UpdateDesktopAppPath(string nome, string Path)
        {
            return UpdateDesktopApp(nome, null, Path, null);
        }

        public bool UpdateDesktopAppDescription(string nome, string description)
        {
            return UpdateDesktopApp(nome, null, null, description);
        }

        public bool DeleteDesktopApp(string nome)
        {
            logger.Debug("Inizio Metodo DeleteDesktopApp in DocsPaDb.Query_DocsPAWS.DesktopApps");
            bool retValue = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_DESKTOP_APP");
                q.setParam("nome", nome);

                string query = q.getSQL();
                logger.Debug("DeleteOpzioniNotifiche: " + query);
                if (!ExecuteNonQuery(query))
                {
                    logger.Error("Errore durante la rimozione in DPA_DESKTOP_APPS: " + query);
                    retValue = false;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo DeleteDesktopApp in DocsPaDb.Query_DocsPAWS.DesktopApps: " + e.Message);
                return false;
            }

            logger.Debug("Fine Metodo DeleteDesktopApp in DocsPaDb.Query_DocsPAWS.DesktopApps");
            return retValue;
        }
    }
}
