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
    public class ReportVersamentiReport
    {
        private ILog logger = LogManager.GetLogger(typeof(ReportVersamentiReport));

        [ReportDataExtractorMethod(ContextName="ReportVersamentiPARER")]
        public DataSet GetDataSet(InfoUtente infoUt, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    Query query = InitQuery.getInstance().getQuery("S_GET_REPORT_POLICY_PARER");
                    query.setParam("stato", filters.Where(f => f.argomento.Equals("stato")).FirstOrDefault().valore);
                    query.setParam("idAmm", filters.Where(f => f.argomento.Equals("idAmm")).FirstOrDefault().valore);

                    string command = query.getSQL();
                    logger.Debug("QUERY - " + command);
                    dbProvider.ExecuteQuery(out dataSet, command);
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);
                }
            }

            return dataSet;
        }
    }
}
