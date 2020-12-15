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
    [ReportDataExtractorClass]
    public class AmmExportPolicyPARERReport
    {
        private ILog logger = LogManager.GetLogger(typeof(AmmExportPolicyPARERReport));

        [ReportDataExtractorMethod(ContextName="AmmExportPolicyPARER")]
        public DataSet GetData(InfoUtente utente, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    Query query = InitQuery.getInstance().getQuery("S_GET_POLICY_FOR_EXPORT");
                    query.setParam("idPolicy", filters.Where(f => f.argomento == "idPolicy").FirstOrDefault().valore);

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
