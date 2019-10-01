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
    public class RigenerazioneIstanzaReport
    {
        private static ILog logger = LogManager.GetLogger(typeof(NotificheRifiutoReport));

        [ReportDataExtractorMethod(ContextName="RigenerazioneIstanza")]
        public DataSet GetInfoRigenerazioneIstanza(InfoUtente infoUtente, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                Query query = InitQuery.getInstance().getQuery("S_NOTIFICHE_RIFIUTO_REPORT");
                
                string idIstanza = filters.Where(f => f.argomento == "idIstanza").FirstOrDefault().valore;
                query.setParam("idIst", idIstanza);

                string commandText = query.getSQL();

                logger.Debug("QUERY - SQL: " + commandText);
                dbProvider.ExecuteQuery(out dataSet, commandText);

            }

            return dataSet;
        }

    }
}
