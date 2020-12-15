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
    class NotificheRifiutoReport
    {

        private static ILog logger = LogManager.GetLogger(typeof(NotificheRifiutoReport));

        [ReportDataExtractorMethod(ContextName = "NotificheRifiutoVerifiche")]
        public DataSet GetInfoNotificheRifiutoVerifiche(InfoUtente infoUtente, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_NOTIFICHE_RIFIUTO_REPORT");
                this.SetFilters(query, filters);

                string commandText = query.getSQL();

                logger.Debug("QUERY - SQL: " + commandText);
                dbProvider.ExecuteQuery(out dataSet, commandText);

            }

            return dataSet;
        }

        [ReportDataExtractorMethod(ContextName = "NotificheRifiutoPolicy")]
        public DataSet GetInfoNotificheRifiutoPolicy(InfoUtente infoUtente, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_NOTIFICHE_RIFIUTO_REPORT_POLICY");
                this.SetFilters(query, filters);

                string commandText = query.getSQL();

                logger.Debug("QUERY - SQL: " + commandText);
                dbProvider.ExecuteQuery(out dataSet, commandText);

            }

            return dataSet;
        }

        private void SetFilters(Query query, List<FiltroRicerca> filters)
        {

            //id istanza
            string idIstanza = filters.Where(f => f.argomento == "idIstanza").FirstOrDefault().valore;
            query.setParam("idIst", idIstanza);

        }

    }
}
