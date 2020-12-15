using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DocsPaVO.utente;
using DocsPaVO.filtri;
using DocsPaUtils;
using DocsPaVO.Report;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{
    /// <summary>
    /// Classe per l'estrazione dei dati per il report di consistenza dell'organigramma
    /// </summary>
    [ReportDataExtractorClass()]
    public class ConsistenzaOrganigrammaReport
    {
        /// <summary>
        /// Metodo per l'estrazion e dei dati relativi ai ruoli disabilitati
        /// </summary>
        /// <param name="userInfo">Informazioni sull'utente che ha richiesto l'estrazione</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <returns>DataSet con i dati da esportare</returns>
        [ReportDataExtractorMethod(ContextName = "DisabledRoles")]
        public DataSet GetDisabledRoleReport(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire ed esecuzione
                Query query = InitQuery.getInstance().getQuery("S_REPORT_ROLES_DISABLED");
                query.setParam("ammId", userInfo.idAmministrazione);
                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;
        }

        /// <summary>
        /// Metodo per l'estrazione dei dati relativi ai ruoli inibiliti alla ricezione di trasmissioni
        /// </summary>
        /// <param name="userInfo">Informaizoni sull'utente che ha richiesto l'estrazione</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <returns>DataSet con i dati da esportare</returns>
        [ReportDataExtractorMethod(ContextName="InhibitedRoles")]
        public DataSet GetInhibitedRolesReport(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire ed esecuzione
                Query query = InitQuery.getInstance().getQuery("S_REPORT_ROLES_INHIBITED");
                query.setParam("ammId", userInfo.idAmministrazione);
                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;
        }

        /// <summary>
        /// Metodo per l'estrazione dei dati relativi ai ruoli senza utenti
        /// </summary>
        /// <param name="userInfo">Informaizoni sull'utente che ha richiesto l'esportazione</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <returns>DataSet con i dati da esportare</returns>
        [ReportDataExtractorMethod(ContextName = "RolesWOutUsers")]
        public DataSet GetRolesWithoutUsersReport(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire ed esecuzione
                Query query = InitQuery.getInstance().getQuery("S_REPORT_ROLES_WOUT_USERS");
                query.setParam("ammId", userInfo.idAmministrazione);
                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;
        }

        /// <summary>
        /// Metodo per l'estrazione dei dati relativi ai ruoli senza Registri o RF
        /// </summary>
        /// <param name="userInfo">Informazioni sull'utente che ha richiesto l'estrazione dati</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <returns>DataSet con i dati da esportare</returns>
        [ReportDataExtractorMethod(ContextName = "RolesWOutRegsOrRF")]
        public DataSet GetRolesWithOutReisOrRFReport(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire ed esecuzione
                Query query = InitQuery.getInstance().getQuery("S_REPORT_ROLES_WOUT_REGS_OR_RF");
                query.setParam("ammId", userInfo.idAmministrazione);
                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;
        }

        /// <summary>
        /// Metodo per l'estrazione dei ruoli senza funzioni
        /// </summary>
        /// <param name="userInfo">Informaizioni sull'utente che ha richiesto l'esportazione</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <returns>DataSet con i dati da esportare</returns>
        [ReportDataExtractorMethod(ContextName = "RolesWOutFunctions")]
        public DataSet GetRolesWithoutFunziontsReport(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire ed esecuzione
                Query query = InitQuery.getInstance().getQuery("S_REPORT_WOUT_FUNCTIONS");
                query.setParam("ammId", userInfo.idAmministrazione);
                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;
        }

        /// <summary>
        /// Metodo per l'estrazione delle UO senza Ruoli
        /// </summary>
        /// <param name="userInfo">Informazioni sull'utente che ha richiesto l'estrazione dei dati</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <returns>DataSet con i dati da esportare</returns>
        [ReportDataExtractorMethod(ContextName = "UOWOutRoles")]
        public DataSet GetUOWithoutRolesReport(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire ed esecuzione
                Query query = InitQuery.getInstance().getQuery("S_REPORT_UO_WOUT_ROLES");
                query.setParam("ammId", userInfo.idAmministrazione);
                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;
        }

        /// <summary>
        /// Metodo per l'estrazione dei dati relativi alle UO senza RF
        /// </summary>
        /// <param name="userInfo">Informazioni sull'utente che ha richiesto l'estrazione dati</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <returns>DataSet con i dati da esportare</returns>
        [ReportDataExtractorMethod(ContextName = "UOWOutRF")]
        public DataSet GetUOWithoutRFReport(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire ed esecuzione
                Query query = InitQuery.getInstance().getQuery("S_REPORT_WOUT_RF");
                query.setParam("ammId", userInfo.idAmministrazione);
                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;
        }
    }
}
