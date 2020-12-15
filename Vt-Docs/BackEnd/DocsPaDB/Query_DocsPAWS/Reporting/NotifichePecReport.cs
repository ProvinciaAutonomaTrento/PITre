using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DocsPaVO.utente;
using DocsPaVO.filtri;
using DocsPaUtils;
using DocsPaVO.Report;
using DocsPaVO.DatiCert;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{
    /// <summary>
    /// Questa classe viene utilizzata per l'estrazione dei dati relativi alle notifiche pec
    /// </summary>
    [ReportDataExtractorClass()]
    class NotifichePecReport
    {
        [ReportDataExtractorMethod(ContextName = "NotificheSpedizione")]
        public DataSet GetNotifichePecData(InfoUtente userInfo, List<FiltroRicerca> searchFilters)
        {
            DataSet dataSet = new DataSet();
            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire ed esecuzione
                Query query = InitQuery.getInstance().getQuery("S_DPA_NOTIFICA2");
                query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                query.setParam("docnumber", "docnumber = " + searchFilters[0].valore);
                // Impostazione filtri
                query.setParam("filterCond", this.GetConditionFilter(searchFilters));
             
                //verifica se è necessario stampare anche le conferme di ricezione
                if (IsPrintConfirmRecept(searchFilters))
                    query.setParam("unionConfermaRicevute", GetQueryConfirmRecept(searchFilters)); //modifico la queryper le conferme di ricezione
                else
                    query.setParam("unionConfermaRicevute", string.Empty);
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());
            }
            return dataSet;
        }

        private string GetConditionFilter(List<FiltroRicerca> searchFilters)
        {
            bool existFilterType = ((from f in searchFilters where f.argomento.Equals("filterTipo") && !string.IsNullOrEmpty(f.valore) select f).Count() > 0) ? true : false;
            bool existFilterMail = ((from f in searchFilters where (f.argomento.Equals("filterDest") && !string.IsNullOrEmpty(f.valore)) select f).Count() > 0) ? true : false;
            bool existFilterCode = ((from f in searchFilters where (f.argomento.Equals("filterCodiceIS") && !string.IsNullOrEmpty(f.valore)) select f).Count() > 0) ? true : false;
            
            // Impostazione filtri
            StringBuilder filterCond = new StringBuilder();
            //filtro tipo ricevuta
            if (existFilterType)
            {
                string[] arrType = (from f in searchFilters where f.argomento.Equals("filterTipo") select f.valore).First().Split('#');
                filterCond.AppendFormat(" AND(");
                // PALUMBO: modifica per consentire il filtro delle ricevute di preavviso-errore-consegna trattate come mancata consegna
                if (arrType[0].Equals("errore-consegna"))
                    filterCond.AppendFormat("tn.var_codice_notifica LIKE '%{0}'", arrType[0]);
                else
                    filterCond.AppendFormat("tn.var_codice_notifica='{0}'", arrType[0]);
                
                for (int i = 1; i < arrType.Length - 1; i++)
                {
                    
                    if (arrType[i].Equals("errore-consegna"))
                        filterCond.AppendFormat(" OR tn.var_codice_notifica LIKE '%{0}'", arrType[i]);                        
                    else
                        filterCond.AppendFormat(" OR tn.var_codice_notifica='{0}'", arrType[i]);
                }
                filterCond.AppendFormat(")");
            }

            if (existFilterMail || existFilterCode)
            {
                filterCond.AppendFormat(" AND(");
                // Filtro Mail
                if (existFilterMail)
                {
                    string[] arrMail = (from f in searchFilters where (f.argomento.Equals("filterDest")) select f.valore).First().Split('#');
                    filterCond.AppendFormat("lower(VAR_DESTINATARIO) LIKE '%{0}%'", arrMail[0]);
                    for (int i = 1; i < arrMail.Length - 1; i++)
                    {
                        filterCond.AppendFormat(" OR lower(VAR_DESTINATARIO) LIKE '%{0}%'", arrMail[i]);
                    }
                }
                // Filtro Codice corrispondente(IS)
                if (existFilterCode)
                {
                    if (existFilterMail)
                        filterCond.AppendFormat(" OR ");
                    filterCond.AppendFormat("lower(VAR_DESTINATARIO) LIKE lower('%{0}%')", (from f in searchFilters where (f.argomento.Equals("filterCodiceIS")) select f.valore).First());
                }
                filterCond.AppendFormat(")");
            }
            return filterCond.ToString();
        }

        private bool IsPrintConfirmRecept(List<FiltroRicerca> searchFilters)
        {
            bool res = false;
            // Analisi dei filtri
            foreach (FiltroRicerca filter in searchFilters)
            {
                switch (filter.argomento)
                {
                    case "filterTipo":
                        if (String.IsNullOrEmpty(filter.valore) || filter.valore.IndexOf("ricevuta") > -1 || filter.valore.IndexOf("annulla") > -1)
                        {
                            return res = true;
                        }
                        else
                            return res = false;
                        break;
                }
            }
            return res;
        }

        private string GetQueryConfirmRecept(List<FiltroRicerca> searchFilters)
        {
            System.Text.StringBuilder buildQuery = new StringBuilder();
            string idDoc = string.Empty;
            string idDest = string.Empty;
            bool ricevuta = false;
            bool annulla = false;
            // Analisi dei filtri
            foreach (FiltroRicerca filter in searchFilters)
            {
                switch (filter.argomento)
                {
                    case "filterIdDoc":   // Filtro tipo ricevuta
                        idDoc = filter.valore;
                        break;
                    case "filterExportInterop":
                        idDest = (!string.IsNullOrEmpty(filter.valore) && !filter.valore.Equals("T")) ? filter.valore : string.Empty;
                        break;
                    case "filterTipo":
                        if (String.IsNullOrEmpty(filter.valore))
                        {
                            ricevuta = true;
                            annulla = true;
                        }
                        else
                        {
                            if(filter.valore.IndexOf("ricevuta") > -1)
                                ricevuta = true;
                            if (filter.valore.IndexOf("annulla") > -1)
                                annulla = true;
                        }
                        break;
                }
            }
            //costruisco la seconda parte della query
            string condIdDest = string.Empty;
            if (!string.IsNullOrEmpty(idDest))
                condIdDest = " AND s.ID_CORR_GLOBALE = " + idDest;
            if(ricevuta)
                buildQuery.Append("UNION" + "\n" +
                                    "(SELECT" +
                                    "'Conferma di ricezione' as tipo," + "\n" +
                                    "c.var_desc_corr as destinatario," + "\n" +
                                    "'AMM: ' || s.VAR_CODICE_AMM || ' AOO: ' || s.VAR_CODICE_AOO || ' DATA PROT: ' ||" + "\n" +
                                    "to_char(s.DTA_PROTO_DEST,'dd/mm/yyyy') || ' PROTOC: ' || s.VAR_PROTO_DEST AS dettagli" + "\n" +
                                    "FROM DPA_STATO_INVIO s, dpa_corr_globali c" + "\n" +
                                    "WHERE s.id_corr_globale=c.system_id" + "\n" +
                                    "AND s.ID_PROFILE=" + idDoc + "\n" +
                                      condIdDest + "\n" +
                                    "AND s.var_proto_dest is not null" + "\n" +
                                    ")");
            if (annulla)
                buildQuery.Append("UNION" + "\n" +
                               "(SELECT" +
                               "'Notifica Annullamento' as tipo," + "\n" +
                               "c.var_desc_corr as destinatario," + "\n" +
                               "'AMM: ' || s.VAR_CODICE_AMM || ' AOO: ' || s.VAR_CODICE_AOO || ' DATA PROT: ' ||" + "\n" +
                               "to_char(s.DTA_PROTO_DEST,'dd/mm/yyyy') || ' PROTOC: ' || s.VAR_PROTO_DEST ||" + "\n" +
                               "' MOTIVO ANNULLAMENTO: ' || s.VAR_MOTIVO_ANNULLA  AS dettagli" + "\n" +
                               "FROM DPA_STATO_INVIO s, dpa_corr_globali c" + "\n" +
                               "WHERE s.id_corr_globale=c.system_id" + "\n" +
                               "AND s.ID_PROFILE=" + idDoc + "\n" +
                               condIdDest + "\n" +
                               "AND s.var_proto_dest is not null" + "\n" +
                               "AND s.CHA_ANNULLATO = '1'" + "\n" +
                               ")");
            return buildQuery.ToString();
        }
    }
}
