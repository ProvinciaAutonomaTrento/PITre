using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DocsPaVO.filtri;
using DocsPaVO.Report;
using DocsPaVO.utente;
using DocsPaUtils;
using log4net;


namespace DocsPaDB.Query_DocsPAWS.Reporting
{

    [ReportDataExtractorClass()]
    class StampaRegistroConservazioneReport
    {

        private static ILog logger = LogManager.GetLogger(typeof(RegistroConservazionePrintManager));

        [ReportDataExtractorMethod(ContextName = "rcIstanze")]
        public DataSet GetStampaIstanze(InfoUtente infoUtente, List<FiltroRicerca> searchFilters)
        {
            //rivedere, probabilmente non ci serve infoutente...
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_REG_CONS_STAMPA_ISTANZA");

                //metodo per l'impostazione dei filtri
                this.SetFilters(query, searchFilters, "IST");

                logger.Debug(query.getSQL());

                //esecuzione della query
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;

        }

        [ReportDataExtractorMethod(ContextName = "rcDocumenti")]
        public DataSet GetStampaDocumenti(InfoUtente infoUtente, List<FiltroRicerca> searchFilters)
        {
            //rivedere, probabilmente non ci serve infoutente...
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_REG_CONS_STAMPA_DOCUMENTO");

                //metodo per l'impostazione dei filtri
                this.SetFilters(query, searchFilters, "DOC");

                logger.Debug(query.getSQL());

                //esecuzione della query
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;

        }

        /// <summary>
        /// Metodo per la sostituzione dei parametri nella query
        /// </summary>
        /// <param name="query">Query in cui sostituire i parametri</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        private void SetFilters(Query query, List<FiltroRicerca> searchFilters, string context)
        {
            //id amministrazione
            string idAmm = searchFilters.Where(f => f.argomento == "id_amm").FirstOrDefault().valore;

            //system id da cui partire
            string nextSysId = searchFilters.Where(f => f.argomento == "next_system_id").FirstOrDefault().valore;

            //ultimo system id da stampare
            string lastSysId = searchFilters.Where(f => f.argomento == "last_system_id").FirstOrDefault().valore;

            //id dell'istanza
            string idIst = searchFilters.Where(f => f.argomento == "id_istanza").FirstOrDefault().valore;

            //imposto i parametri della query
            query.setParam("id_amm", idAmm);
            query.setParam("next_system_id", nextSysId);
            query.setParam("last_system_id", lastSysId);
            query.setParam("id_istanza", idIst);

            //se è un documento, imposto anche il filtro corrispondente
            if (context == "DOC")
            {
                string idDoc = searchFilters.Where(f => f.argomento == "id_oggetto").FirstOrDefault().valore;
                query.setParam("id_oggetto", idDoc);

            }


        }
    }
}
