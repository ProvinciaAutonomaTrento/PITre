using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;
using System.Data;
using DocsPaVO.utente;
using DocsPaVO.filtri;
using DocsPaUtils;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{
    [ReportDataExtractorClass()]
    public class CopiaVisibilitaReport
    {
        [ReportDataExtractorMethod(ContextName = "CopiaVisibilita")]
        public DataSet GetCopiaVisibilitaReport(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();
            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire per produrre il report
                Query query = InitQuery.getInstance().getQuery("R_COPIA_VISIBILITA");
                //Query query = InitQuery.getInstance().getQuery("R_COPIA_VISIBILITA_TEST");

                // Esecuzione della query
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());
            }

            // Restituzione risultato query
            return dataSet;
        }
    }
}