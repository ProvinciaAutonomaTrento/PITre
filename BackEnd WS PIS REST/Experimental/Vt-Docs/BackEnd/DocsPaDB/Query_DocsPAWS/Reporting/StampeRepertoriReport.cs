using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;
using System.Data;
using DocsPaVO.utente;
using DocsPaVO.filtri;
using DocsPaUtils;
using log4net;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{
    /// <summary>
    /// Questa classe contiene i sottoreport che compongono una stampa repertori
    /// </summary>
    [ReportDataExtractorClass()]
    public class StampeRepertoriReport
    {
        /// <summary>
        /// Query per l'estrazione dei dati sui documenti di repertorio creati a partire dall'ultima stampa effettuata
        /// </summary>
        /// <param name="userInfo">Informazioni sull'utente richiedente</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <returns>Data set con i dati da esportare</returns>
        /// 
        private ILog logger = LogManager.GetLogger(typeof(StampeRepertoriReport));

        [ReportDataExtractorMethod(ContextName = "StampaRepertoriNuovi")]
        public DataSet GetStampaRepertoriNuovi(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire ed esecuzione
                Query query = InitQuery.getInstance().getQuery("REPORT_REGISTRI_REPERTORIO_NUOVI");
                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                // Impostazioni dei filtri di ricerca
                this.SetFilters(query, searchFilters);

                string qn = query.getSQL();

                logger.Debug("REPORT_REGISTRI_REPERTORIO_NUOVI: ");
                logger.Debug(qn);
                // Esecuzione della query
                dbProvider.ExecuteQuery(out dataSet, qn);

            }

            // Restituzione risultato query
            return dataSet;
        }

        /// <summary>
        /// Metodo per l'estrazione dei dati sui repertori modificati dopo l'ultima stampa
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="searchFilters"></param>
        /// <returns></returns>
        [ReportDataExtractorMethod(ContextName = "StampaRepertoriAggiornati")]
        public DataSet GetStampaRepertoriModificati(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();
            bool includiModificati = true;
            try
            {
                string modX = searchFilters.Where(f => f.argomento == "RECUPERO_evitaMod").FirstOrDefault().valore;
                if (modX.ToUpper() == "TRUE")
                {
                    includiModificati = false;
                }
            }
            catch (Exception exNoLast)
            {

            }

            if (includiModificati)
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    // Reperimento query da eseguire ed esecuzione
                    Query query = InitQuery.getInstance().getQuery("REPORT_REGISTRI_REPERTORIO_MOD");
                    query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                    // Impostazioni dei filtri di ricerca
                    this.SetFilters(query, searchFilters);
                    string q = query.getSQL();
                    // Esecuzione della query
                    logger.Debug("REPORT_REGISTRI_REPERTORIO_MOD: ");
                    logger.Debug(q);
                    dbProvider.ExecuteQuery(out dataSet, q);
                }
            }
            // Restituzione risultato query
            return dataSet;
        }

        /// <summary>
        /// Metodo per la sostituzione dei parametri nella query
        /// </summary>
        /// <param name="query">Query in cui sostituire i parametri</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        private void SetFilters(Query query, List<FiltroRicerca> searchFilters)
        {
            // Recupero dell'id contatore
            String idCounter = searchFilters.Where(f => f.argomento == "idCounter").FirstOrDefault().valore;

            // Recupero dellid del registro
            String idRegistry = searchFilters.Where(f => f.argomento == "idRegistry").FirstOrDefault().valore;

            // Recupero dell'id dell'RF
            String idRf = searchFilters.Where(f => f.argomento == "idRf").FirstOrDefault().valore;

            // Costruzione della lista di registry / RF
            String registryRfList = String.Empty;
            // Se sono nulli entrambi i registri si tratta si contatore di tipologia
            if (String.IsNullOrEmpty(idRegistry) && String.IsNullOrEmpty(idRf))
                registryRfList = "0";
            else
                // Altrimenti uno dei due non è null ed è quello che deve essere utilizzato nella condizione di selezione
                registryRfList = String.IsNullOrEmpty(idRegistry) ? idRf : idRegistry;

            String regId = String.Empty;
            if (String.IsNullOrEmpty(idRegistry))
                regId = idRf;
            else
                regId = idRegistry;

            // Recupero dell'anno di stampa
            String year = searchFilters.Where(f => f.argomento == "year").FirstOrDefault().valore;

            // Recupero del primo numero da stampare
            String firstNumber = searchFilters.Where(f => f.argomento == "lastPrintedNumber").FirstOrDefault().valore;
            String lastNumber = "";
            try
            {
                lastNumber = searchFilters.Where(f => f.argomento == "lastNumberToPrint").FirstOrDefault().valore;
            }
            catch (Exception exNoLast)
            {
                lastNumber = "";
            }
            query.setParam("year", year);
            if (string.IsNullOrEmpty(lastNumber))
            {
                query.setParam("firstNumber", firstNumber);
            }
            else
            {
                query.setParam("firstNumber", firstNumber + " AND TO_NUMBER (valore_oggetto_db) <= " + lastNumber);
            }
            query.setParam("regId", String.IsNullOrEmpty(regId) ? " is null " : String.Format(" = {0} ", regId));

            // Sostituzione dei parametri
            query.setParam("idCounter", idCounter);
            query.setParam("idRegistry", String.IsNullOrEmpty(idRegistry) ? " is null " : " = " + idRegistry);
            query.setParam("idRf", String.IsNullOrEmpty(idRf) ? " is null " : " = " + idRf);
            query.setParam("registryRfList", registryRfList);

        }


    }
}
