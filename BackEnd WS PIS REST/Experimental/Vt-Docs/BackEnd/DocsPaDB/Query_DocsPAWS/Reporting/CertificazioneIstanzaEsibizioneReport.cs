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
    public class CertificazioneIstanzaEsibizioneReport
    {

        private ILog logger = LogManager.GetLogger(typeof(CertificazioneIstanzaEsibizioneReport));

        [ReportDataExtractorMethod(ContextName = "CertificazioneEsibizione")]
        public DataSet GetDataCertificazione(InfoUtente infoUtente, List<FiltroRicerca> filters)
        {
            DataSet dataSet = new DataSet();
            string idEsibizione = filters.Where(f => f.argomento == "systemId").FirstOrDefault().valore;

            using (DBProvider dbProvider = new DBProvider())
            {

                Query query = InitQuery.getInstance().getQuery("S_ESIBIZIONE_CERTIF");

                query.setParam("idEsib", idEsibizione);
                
                string commandText = query.getSQL();
                logger.Debug("QUERY - SQL: " + commandText);

                dbProvider.ExecuteQuery(out dataSet, commandText);

            }

            return dataSet;
        }

    }
}
