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
    public class AmmExportMicroFunzioniReport
    {

        private ILog logger = LogManager.GetLogger(typeof(AmmExportMicroFunzioniReport));

        [ReportDataExtractorMethod(ContextName = "AmmExportMicroFunzioni")]
        public DataSet GetDataReportMicro(InfoUtente infoUt, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    Query query = InitQuery.getInstance().getQuery("S_AMM_EXPORT_MICRO_FUNZIONI");
                    query.setParam("idAmm", filters.Where(f => f.argomento == "id_amm").FirstOrDefault().valore);
                    query.setParam("codFunz", filters.Where(f => f.argomento == "id_funzione").FirstOrDefault().valore);
                    query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

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
