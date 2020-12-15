using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DocsPaVO.filtri;
using DocsPaVO.Report;
using DocsPaVO.utente;
using DocsPaUtils;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{

    [ReportDataExtractorClass()]
    public class EsportaLogConservazioneReport
    {
        [ReportDataExtractorMethod(ContextName="LogConservazione")]
        public DataSet GetDataExportLogConservazione(InfoUtente infoUtente, List<FiltroRicerca> filters)
        {
            
            //setto i parametri di ricerca
            string dataDa = filters.Where(f => f.argomento == "dataFrom").FirstOrDefault().valore;
            string dataA = filters.Where(f => f.argomento == "dataTo").FirstOrDefault().valore;
            string utente = filters.Where(f => f.argomento == "utente").FirstOrDefault().valore;
            string idOgg = filters.Where(f => f.argomento == "idIstanza").FirstOrDefault().valore;
            string azione = filters.Where(f => f.argomento == "azione").FirstOrDefault().valore;
            string esito = filters.Where(f => f.argomento == "esito").FirstOrDefault().valore;
            string idAmm = GetAmministrazione(infoUtente.idPeople);

            //log non archiviati (DPA_LOG) = 1
            //log archviati (DPA_LOG_STORICO) = 2
            //tutti i log (union all delle due) = 3
            int queryTable = 3;
            DataSet dataSet = new Log().GetXmlLogFiltrato(dataDa, dataA, utente, idOgg, "CONSERVAZIONE", azione, idAmm, esito, string.Empty, queryTable);
            return dataSet;
        }

        private string GetAmministrazione(string idPeople)
        {

            string commandText = string.Format("SELECT A.VAR_CODICE_AMM FROM DPA_AMMINISTRA A INNER JOIN PEOPLE P ON A.SYSTEM_ID = P.ID_AMM WHERE P.SYSTEM_ID = {0}", idPeople);
            string field;

            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteScalar(out field, commandText);
            }

            return field;

        }

        public string GetAzione(string codAzione)
        {
            string commandText = string.Format("SELECT DISTINCT var_descrizione FROM dpa_anagrafica_log WHERE var_codice='{0}' AND var_oggetto='CONSERVAZIONE'", codAzione);
            string field = string.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteScalar(out field, commandText);
            }

            return field;
        }
    }
}
