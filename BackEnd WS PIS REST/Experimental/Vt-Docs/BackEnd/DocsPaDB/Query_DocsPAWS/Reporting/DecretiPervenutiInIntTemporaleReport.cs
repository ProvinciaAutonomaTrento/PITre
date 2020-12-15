using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.Report;
using System.Data;
using DocsPaVO.utente;
using DocsPaUtils;
using DocsPaVO.filtri;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{
    /// <summary>
    /// Questa classe viene utilizzata per l'estrazione dei dati relativi al report per l'estrazione dei
    /// decreti pervenuti in un dato intervallo temporale (reportistica sviluppata per Corte Dei Conti
    /// </summary>
    [ReportDataExtractorClass()]
    public class DecretiPervenutiInIntTemporaleReport
    {
        /// <summary>
        /// Metodo per l'estrazione dei dati relativi ai decreti in esame pervenuti in un intervallo temporare
        /// </summary>
        /// <param name="userInfo">Informazioni sull'utente che ha richiesto l'estrazione dati</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <returns>DataSet con i dati da esportare</returns>
        [ReportDataExtractorMethod(ContextName = "DecretiInEsamePervenutiInIntTemporale")]
        public DataSet GetDescretiInEsamePervenutiInRangeDateData(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire ed esecuzione
                Query query = InitQuery.getInstance().getQuery("CDC_REPORT_DECRETI_PERVENUTI_IN_INT_TEMPORALE");
                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                // Impostazione filtri
                query.setParam("filterCond", this.GetConditionFilter(searchFilters));

                // Impostazione lista degli identificativi dei ruoli in cui ricercare i documenti
                query.setParam("ufficio", this.GetRolesList(searchFilters.Where(e => e.argomento == "CDCUffici").First(), userInfo));

                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;
        }

        /// <summary>
        /// Metodo per il recupero dei filtri da utilizzare nell'estrazione dati
        /// </summary>
        /// <param name="searchFilters">Lista dei filtri da analizzare</param>
        /// <returns>Condizione di filtraggio</returns>
        private String GetConditionFilter(List<FiltroRicerca> searchFilters)
        {
            StringBuilder filterCondition = new StringBuilder();

            String dtaFrom = String.Empty, dtaTo = String.Empty;

            // Analisi dei filtri
            foreach (FiltroRicerca filter in searchFilters)
            {
                switch (filter.argomento)
                {
                    case "CDCDataDa":
                        dtaFrom = filter.valore;
                        break;
                    case "CDCDataA":
                        dtaTo = filter.valore;
                        break;
                    case "CDCRevisore":   // Codice del revisore
                        if (!String.IsNullOrEmpty(filter.valore))
                            filterCondition.AppendFormat(" AND (vm.Primo_Revisore = {0} OR vm.Secondo_Revisore = {0})", filter.valore);
                        break;
                    case "CDCMagistrato":     // Codice del magistrato
                        if (!String.IsNullOrEmpty(filter.valore))
                            filterCondition.AppendFormat(" AND (vm.Magistrato_Istruttore = {0})", filter.valore);
                        break;

                }
            }

            // Conversione date
            DateTime dateFrom = DateTime.Parse(dtaFrom);
            dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
            DateTime dateTo = DateTime.Parse(dtaTo);
            dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 59);

            filterCondition.AppendFormat(" AND (nvl(vm.dta_arrivo, vm.dta_protocollo) >= to_date('{0}', 'dd/mm/yyyy HH24.mi.ss') AND nvl(vm.dta_arrivo, vm.dta_protocollo) <= to_date('{1}', 'dd/mm/yyyy HH24.mi.ss'))", dateFrom.ToString("dd/MM/yyyy HH:mm:ss"), dateTo.ToString("dd/MM/yyyy HH:mm:ss"));
            
            return filterCondition.ToString();
        }

        /// <summary>
        /// Metodo per il recupero della lista dei ruoli presenti sotto un dato ufficio
        /// </summary>
        /// <param name="filtroRicerca">Filtro con le informazioni sull'ufficio di cui ricavare i ruoli</param>
        /// <param name="userInfo">Informazioni sull'utente</param>
        /// <returns>Lista degli id dei ruoli presenti sotto l'ufficio</returns>
        private string GetRolesList(FiltroRicerca filtroRicerca, InfoUtente userInfo)
        {
            StringBuilder rolesId = new StringBuilder();
            using (Amministrazione amm = new Amministrazione())
            {
                Ruolo[] roles = (Ruolo[])amm.getRuoliUO(new UnitaOrganizzativa() { systemId = filtroRicerca.valore, idAmministrazione = userInfo.idAmministrazione }).ToArray(typeof(Ruolo));

                foreach (var role in roles)
                    rolesId.AppendFormat("{0},", role.idGruppo);

                // Rimozione dell'ultima virgola
                rolesId = rolesId.Remove(rolesId.Length - 1, 1);

            }

            // Restituzione ruoli
            return rolesId.ToString();

        }
    }
}
