using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DocsPaVO.utente;
using DocsPaVO.filtri;
using DocsPaUtils;
using DocsPaVO.Report;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{
    /// <summary>
    /// Questa classe viene utilizzata per l'estrazione dei dati relativi al report per l'estrazione dei
    /// decreti in esame (reportistica sviluppata per Corte Dei Conti
    /// </summary>
    [ReportDataExtractorClass()]
    public class DecretiInEsameSRCReport
    {

        [ReportDataExtractorMethod(ContextName = "DecretiInEsameSRC")]
        public DataSet GetDescretiInEsameData(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire ed esecuzione
                Query query = InitQuery.getInstance().getQuery("CDC_REPORT_DECRETI_IN_ESAME_SRC");
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

            // Analisi dei filtri
            foreach (FiltroRicerca filter in searchFilters)
            {
                switch (filter.argomento)
                {
                    case "CDCRevisore":   // Codice del revisore
                        if (!String.IsNullOrEmpty(filter.valore))
                            filterCondition.AppendFormat(" AND (vm.Revisore = {0})", filter.valore);
                        break;
                    case "CDCMagistrato":     // Codice del magistrato
                        if (!String.IsNullOrEmpty(filter.valore))
                            filterCondition.AppendFormat(" AND (vm.Magistrato_Istruttore = {0})", filter.valore);
                        break;

                }
            }

            return filterCondition.ToString();
        }

        /// <summary>
        /// Funzione per il recupero della lista dei ruoli presenti sotto un dato ufficio
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
