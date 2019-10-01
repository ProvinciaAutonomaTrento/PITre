using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DocsPaUtils;
using DocsPaVO.filtri;
using DocsPaVO.Report;
using DocsPaVO.utente;
using log4net;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{
    [ReportDataExtractorClass()]
    public class RegistroAccessiExportReport
    {
        private ILog logger = LogManager.GetLogger(typeof(RegistroAccessiExportReport));

        [ReportDataExtractorMethod(ContextName = "RegistroAccessiExport")]
        public DataSet GetDataAccessoDocumentale(InfoUtente infoUtente, List<FiltroRicerca> filters)
        {
            logger.Debug("BEGIN");

            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    Query query = InitQuery.getInstance().getQuery("S_REGISTRO_ACCESSI");
                    this.SetFilters(query, filters);

                    if (dbProvider.DBType.ToUpper() == "SQL")
                        query.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                    string command = query.getSQL();
                    logger.Debug("QUERY - " + command);

                    if (!dbProvider.ExecuteQuery(out dataSet, command))
                        throw new Exception(dbProvider.LastExceptionMessage);

                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);
                }
            }

            logger.Debug("END");

            return dataSet;
        }

        private void SetFilters(Query query, List<FiltroRicerca> filters)
        {
            logger.Debug("BEGIN");
            // id amministrazione
            query.setParam("id_amm", filters.Where(f => f.argomento == "id_amm").FirstOrDefault().valore);

            // tipologia
            query.setParam("filtro_tipologia", String.Format("UPPER(VAR_DESC_FASC)=UPPER('{0}')", filters.Where(f => f.argomento == "tipologia").FirstOrDefault().valore));

            // stato del fascicolo
            String folderStatus = filters.Where(f => f.argomento == "stato").FirstOrDefault().valore;
            String statusFilter = string.Empty;
            if(folderStatus == "O")
            {
                statusFilter = "AND A.CHA_STATO='A'";
            }
            else if(folderStatus == "C")
            {
                statusFilter = "AND A.CHA_STATO='C'";
            }
            query.setParam("filtro_stato", statusFilter);

            // data di creazione
            String dateFilter = string.Empty;
            String creationDateInterval = filters.Where(f => f.argomento == "data_creazione").FirstOrDefault().valore;
            String creationDateFrom = filters.Where(f => f.argomento == "data_creazione_da").FirstOrDefault().valore;
            String creationDateTo = filters.Where(f => f.argomento == "data_creazione_a").FirstOrDefault().valore;

            if(creationDateInterval == "0")
            {
                // Valore singolo
                if(!string.IsNullOrEmpty(creationDateFrom))
                {
                    dateFilter = String.Format("AND A.DTA_CREAZIONE >={0} AND A.DTA_CREAZIONE <={1}",
                        DocsPaDbManagement.Functions.Functions.ToDateBetween(creationDateFrom, true), 
                        DocsPaDbManagement.Functions.Functions.ToDateBetween(creationDateFrom, false));
                }                
            }
            if(creationDateInterval == "1")
            {
                // Intervallo
                if(!string.IsNullOrEmpty(creationDateFrom))
                {
                    dateFilter = "AND A.DTA_CREAZIONE >= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(creationDateFrom, true);
                }
                if(!string.IsNullOrEmpty(creationDateTo))
                {
                    dateFilter = dateFilter + " AND A.DTA_CREAZIONE <= " + DocsPaDbManagement.Functions.Functions.ToDateBetween(creationDateTo, false);
                }
            }
            query.setParam("filtro_data", dateFilter);

            logger.Debug("END");
        }
    }
}
