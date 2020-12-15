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
    public class AmmExportMacroFunzioniReport
    {

        private ILog logger = LogManager.GetLogger(typeof(AmmExportMacroFunzioniReport));

        [ReportDataExtractorMethod(ContextName = "AmmExportMacroFunzioni_Micro")]
        public DataSet GetDataMicro(InfoUtente infoUt, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    Query query = InitQuery.getInstance().getQuery("S_AMM_EXPORT_MACRO_FUNZIONI");
                    query.setParam("idAmm", filters.Where(f => f.argomento == "id_amm").FirstOrDefault().valore);
                    query.setParam("tipoFunz", filters.Where(f => f.argomento == "id_funzione").FirstOrDefault().valore);
                    
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

        [ReportDataExtractorMethod(ContextName = "AmmExportMacroFunzioni_Ruoli")]
        public DataSet GetDataUtenti(InfoUtente infoUt, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    Query query = InitQuery.getInstance().getQuery("S_AMM_EXPORT_MACRO_RUOLI");
                    query.setParam("idAmm", filters.Where(f => f.argomento == "id_amm").FirstOrDefault().valore);
                    query.setParam("tipoFunz", filters.Where(f => f.argomento == "id_funzione").FirstOrDefault().valore);
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
