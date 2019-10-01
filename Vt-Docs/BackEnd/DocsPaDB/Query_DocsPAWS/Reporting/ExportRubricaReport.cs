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
    public class ExportRubricaReport
    {

        private ILog logger = LogManager.GetLogger(typeof(ExportRubricaReport));

        [ReportDataExtractorMethod(ContextName="ExportRubrica")]
        public DataSet GetDataListaCorrExport(InfoUtente infoUtente, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    Query query = InitQuery.getInstance().getQuery("S_RICERCA_ALL_CORRISPONDENTI");

                    string registri = filters.Where(f => f.argomento == "registri").FirstOrDefault().valore;
                    if (!string.IsNullOrEmpty(registri))
                        query.setParam("registri", " AND ( r.system_id in (" + registri + ") OR r.system_id IS NULL)");
                    else
                        query.setParam("registri", string.Empty);
                    if (infoUtente != null && !string.IsNullOrEmpty(infoUtente.idAmministrazione))
                        query.setParam("idamm", infoUtente.idAmministrazione);
                    else
                        query.setParam("idamm", string.Empty);

                    string commandText = query.getSQL();
                    logger.Debug("QUERY - " + commandText);

                    dbProvider.ExecuteQuery(out dataSet, commandText);
                    
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
