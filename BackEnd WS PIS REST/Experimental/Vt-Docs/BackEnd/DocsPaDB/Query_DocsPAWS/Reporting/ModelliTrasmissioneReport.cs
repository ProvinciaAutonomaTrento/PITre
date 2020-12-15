using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DocsPaUtils;
using DocsPaVO.filtri;
using DocsPaVO.filtri.trasmissione;
using DocsPaVO.Report;
using DocsPaVO.utente;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{
    /// <summary>
    /// Questa classe è responsabile della generazione dei dati per l'export dei risultati della ricerca
    /// modelli di trasmissione.
    /// </summary>
    [ReportDataExtractorClass()]
    public class ModelliTrasmissioneReport
    {
        /// <summary>
        /// Metodo per la generazione del report relativo ai modelli di trasmissione
        /// </summary>
        /// <param name="userInfo">Informazioni sull'utente</param>
        /// <param name="searchFilter">Lista dei filtri da utilizzare per ricercare modelli di trasmissione che soddisfano determinati criteri</param>
        /// <returns>Dataset con i risultati restituiti dalla ricerca</returns>
        [ReportDataExtractorMethod(ContextName = "ModelliTrasmissione")]
        public DataSet GetModelliTrasmissioneReport(InfoUtente userInfo, List<FiltroRicerca> searchFilter)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire, creazione dei filtri ed esecuzione
                Query query = InitQuery.getInstance().getQuery("S_EXPORT_TRASM_MODEL");
                String whereCond = this.GenerateExportTransModelCond(searchFilter);
                query.setParam("ammId", userInfo.idAmministrazione);
                query.setParam("filterCond", whereCond);
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;
        }

        /// <summary>
        /// Metodo per la generazione del report relativo ai modelli di trasmissione per gli utenti
        /// </summary>
        /// <param name="userInfo">Informazioni sull'utente</param>
        /// <param name="searchFilter">Lista dei filtri da utilizzare per ricercare modelli di trasmissione che soddisfano determinati criteri</param>
        /// <returns>Dataset con i risultati restituiti dalla ricerca</returns>
        [ReportDataExtractorMethod(ContextName = "ModelliTrasmissioneUtente")]
        public DataSet GetModelliTrasmissioneUtenteReport(InfoUtente userInfo, List<FiltroRicerca> searchFilter)
        {
            DataSet dataSet = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Reperimento query da eseguire, creazione dei filtri ed esecuzione
                Query query = InitQuery.getInstance().getQuery("S_EXPORT_TRASM_MODEL_UTENTE");
                String whereCond = this.GenerateExportTransModelCond(searchFilter);
                query.setParam("ammId", userInfo.idAmministrazione);
                query.setParam("idPeople", userInfo.idPeople);
                query.setParam("idCorr", userInfo.idCorrGlobali);
                query.setParam("filterCond", whereCond);
                dbProvider.ExecuteQuery(out dataSet, query.getSQL());

            }

            return dataSet;
        }

        /// <summary>
        /// Metodo per la generazione dei filtri di ricerca da utilizzare con la query per l'export
        /// modelli trasmissione
        /// </summary>
        /// <param name="searchFilter">Filtri di ricerca</param>
        /// <returns>Clausola where</returns>
        private string GenerateExportTransModelCond(List<FiltroRicerca> searchFilter)
        {
            StringBuilder whereClause = new StringBuilder();

            if (searchFilter != null)
            {
                foreach (FiltroRicerca f in searchFilter)
                {
                    // Parsing del filtro di ricerca e aggiunta della condizione di filtro
                    listaArgomentiModelliTrasmissione filter =
                        (listaArgomentiModelliTrasmissione)
                        Enum.Parse(typeof(listaArgomentiModelliTrasmissione), f.argomento, true);

                    switch (filter)
                    {
                        // Codice modello
                        case listaArgomentiModelliTrasmissione.CODICE_MODELLO:
                            if (!String.IsNullOrEmpty(f.valore))
                            {
                                string cond = f.valore.Substring(f.valore.IndexOf("_") + 1);
                                whereClause.AppendFormat(" AND mt.system_id LIKE '%{0}%'", cond.ToUpper().Replace("'", "''"));
                            }
                            break;
                        case listaArgomentiModelliTrasmissione.DESCRIZIONE_MODELLO:
                            if (String.IsNullOrEmpty(f.valore))
                                whereClause.Append(" AND ID_PEOPLE IS NULL ");
                            else
                                whereClause.AppendFormat(" AND UPPER(mt.nome) LIKE UPPER('%{0}%')", f.valore.Replace("'", "''"));
                            break;
                        case listaArgomentiModelliTrasmissione.RUOLI_DISABLED_RIC_TRASM:
                            //whereClause.Append(" AND (md.cha_tipo_urp = 'R' AND md.cha_tipo_mitt_dest = 'D' AND md.id_corr_globali = cg.system_id AND cg.cha_disabled_trasm ='1')");
                            whereClause.Append(" AND mt.SYSTEM_ID in (select dpa_modelli_mitt_dest.id_modello from dpa_modelli_mitt_dest,dpa_corr_globali where  dpa_modelli_mitt_dest.cha_tipo_urp = 'R' AND dpa_modelli_mitt_dest.cha_tipo_mitt_dest = 'D' AND dpa_modelli_mitt_dest.id_corr_globali = dpa_corr_globali.system_id AND dpa_corr_globali.cha_disabled_trasm = '1')");
                            break;
                        case listaArgomentiModelliTrasmissione.NOTE:
                            whereClause.AppendFormat(" AND UPPER(mt.var_note_generali) LIKE UPPER('%{0}%')", f.valore.Replace("'", "''"));
                            break;
                        case listaArgomentiModelliTrasmissione.TIPO_TRASMISSIONE:
                            whereClause.AppendFormat(" AND UPPER(mt.cha_tipo_oggetto) = '{0}'", f.valore.ToUpper());
                            break;
                        case listaArgomentiModelliTrasmissione.ID_REGISTRO:
                            whereClause.AppendFormat(" AND mt.id_registro = {0}", f.valore);
                            break;
                        case listaArgomentiModelliTrasmissione.ID_RAGIONE_TRASMISSIONE:
                            whereClause.AppendFormat(" AND md.id_ragione = {0}", f.valore);
                            break;
                        case listaArgomentiModelliTrasmissione.CODICE_CORR_PER_VISIBILITA:
                            whereClause.AppendFormat(" AND (UPPER(cg.var_codice) = UPPER('{0}') AND cg.system_id = md.id_corr_globali AND UPPER(md.cha_tipo_mitt_dest) = 'M')", f.valore);
                            break;
                        case listaArgomentiModelliTrasmissione.CODICE_CORR_PER_DESTINATARIO:
                            whereClause.AppendFormat(" AND (UPPER(cg.var_codice) = UPPER('{0}') AND cg.system_id = md.id_corr_globali AND UPPER(md.cha_tipo_mitt_dest) = 'D')", f.valore);
                            break;
                        case listaArgomentiModelliTrasmissione.RUOLI_DEST_DISABLED:
                            //whereClause.Append(" AND (md.cha_tipo_urp = 'R' AND md.cha_tipo_mitt_dest = 'D' AND md.id_corr_globali = cg.system_id AND cg.dta_fine IS NOT NULL)");
                            whereClause.Append(" AND mt.SYSTEM_ID in (select dpa_modelli_mitt_dest.id_modello from dpa_modelli_mitt_dest,dpa_corr_globali where  dpa_modelli_mitt_dest.cha_tipo_urp = 'R' AND dpa_modelli_mitt_dest.cha_tipo_mitt_dest = 'D' AND dpa_modelli_mitt_dest.id_corr_globali = dpa_corr_globali.system_id AND dpa_corr_globali.dta_fine IS NOT NULL)");
                            break;
                        case listaArgomentiModelliTrasmissione.MODELLI_CREATI_DA_UTENTE:
                            whereClause.AppendFormat(" AND (mt.id_people IS NOT NULL)");
                            break;
                        case listaArgomentiModelliTrasmissione.MODELLI_CREATI_DA_AMMINISTRATORE:
                            whereClause.AppendFormat(" AND (mt.id_people IS NULL)");
                            break;
                        default:
                            break;
                    }

                }

            }

            return whereClause.ToString();
        }
    }
}
