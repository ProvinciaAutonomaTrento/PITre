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
    public class ExportTrasmissioniPendentiReport
    {
        private ILog logger = LogManager.GetLogger(typeof(ExportTrasmissioniPendentiReport));

        [ReportDataExtractorMethod(ContextName = "ExportTrasmissioniPendentiDoc")]
        public DataSet GetTrasmissioniPendentiConWorkflowUtenteDoc(InfoUtente infoUt, List<FiltroRicerca> filters)
        {
            string idPeople = filters.Where(f => f.argomento == "idPeople").FirstOrDefault().valore;
            string idCorrGlobali = filters.Where(f => f.argomento == "idCorrGlobali").FirstOrDefault().valore;
            DataSet dataSet = new DataSet();
            try
            {
                Query query = InitQuery.getInstance().getQuery("S_DPA_TRASM_PENDENTI_PEOPLE_DOC");
                query.setParam("idCorrGlobali", idCorrGlobali);
                query.setParam("idPeople", idPeople);

                string commandText = query.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteQuery(out dataSet, commandText);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetTrasmissioniPendentiConWorkflowUtente: " + e);
            }

            return dataSet;
        }

        [ReportDataExtractorMethod(ContextName = "ExportTrasmissioniPendentiFasc")]
        public DataSet GetTrasmissioniPendentiConWorkflowUtenteFasc(InfoUtente infoUt, List<FiltroRicerca> filters)
        {
            string idPeople = filters.Where(f => f.argomento == "idPeople").FirstOrDefault().valore;
            string idCorrGlobali = filters.Where(f => f.argomento == "idCorrGlobali").FirstOrDefault().valore;
            DataSet dataSet = new DataSet();
            try
            {
                Query query = InitQuery.getInstance().getQuery("S_DPA_TRASM_PENDENTI_PEOPLE_FASC");
                query.setParam("idCorrGlobali", idCorrGlobali);
                query.setParam("idPeople", idPeople);

                string commandText = query.getSQL();
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteQuery(out dataSet, commandText);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in GetTrasmissioniPendentiConWorkflowUtente: " + e);
            }

            return dataSet;
        }

    }
}
